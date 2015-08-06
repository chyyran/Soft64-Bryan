/*
Soft64 - C# N64 Emulator
Copyright (C) Soft64 Project @ Codeplex
Copyright (C) 2013 - 2014 Bryan Perris

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

using System;
using System.IO;

namespace Soft64.MipsR4300.CP0
{
    internal sealed class TLBMapStream : Stream
    {
        private TLBCache m_TLBCache;
        private Int64 m_Length;
        private Int64 m_Position;

        public TLBMapStream(TLBCache tlbCache, Int64 length)
        {
            m_TLBCache = tlbCache;
            m_Length = length;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
        }

        public override long Length
        {
            get { return m_Length; }
        }

        public override long Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                m_Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            /* We first must translate the virtual address into a physical address */
            Int64 translatedAddress = m_TLBCache.TranslateVirtualAddress(m_Position) | 0x80000000;

            /* If this is an invalid translation */
            if (translatedAddress == -1)
            {
                Array.Clear(buffer, offset, count);
                return count;
            }

            /* If the address is not within physical memory range, then do nothing */
            if (translatedAddress < 0x80000000 || translatedAddress >= 0xC0000000)
            {
                Array.Clear(buffer, offset, count);
                //SystemEventLog.WriteWarn("Invalid TLB Mapping [Read]: " + translatedAddress.ToString("X16"), LogType.CPU);
                return count;
            }

            Machine.Current.N64MemorySafe.Position = translatedAddress - 0x80000000;
            Machine.Current.N64MemorySafe.Read(buffer, 0, buffer.Length);

            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Current: return Position + offset;
                case SeekOrigin.Begin: return offset;
                case SeekOrigin.End: return Position - offset;
                default: return -1;
            }
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            /* We first must translate the virtual address into a physical address */
            Int64 translatedAddress = m_TLBCache.TranslateVirtualAddress(m_Position) | 0x80000000;

            /* If this is an invalid translation */
            if (translatedAddress == -1)
            {
                return;
            }

            /* If the address is not within physical memory range, then do nothing */
            if (translatedAddress < 0x80000000 || translatedAddress >= 0xC0000000)
            {
                //SystemEventLog.WriteWarn("Invalid TLB Mapping [Write]: " + translatedAddress.ToString("X16"), LogType.CPU);
                return;
            }
            Machine.Current.N64MemorySafe.Position = translatedAddress - 0x80000000;
            Machine.Current.N64MemorySafe.Write(buffer, 0, buffer.Length);
        }
    }
}