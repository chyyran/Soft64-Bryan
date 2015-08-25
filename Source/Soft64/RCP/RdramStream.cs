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
using Soft64.IO;

namespace Soft64.RCP
{
    public enum RdramInterfaceRegs
    {
        Config,
        DeviceID,
        Delay,
        Mode,
        RefInterval,
        RefRow,
        RasInterval,
        MinInterval,
        AddressSelect,
        DeviceManufacturer,
    }
    // TODO: Implement rdram control system, and reg properties

    /// <summary>
    /// RDRAM stream device which supports register interface
    /// </summary>
    public sealed class RdramStream : Stream
    {
        private _RdramStream m_RamStream = new _RdramStream();
        private UInt32[] m_Regs = new UInt32[10];
        private Int64 m_Position;

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
            m_RamStream.Flush();
        }

        public override long Length
        {
            get { return m_RamStream.Length; }
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
            m_RamStream.Read(buffer, offset, count);
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return m_RamStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            m_RamStream.Write(buffer, offset, count);
        }
    }


    /// <summary>
    /// Actual ram stream
    /// </summary>
    public sealed class _RdramStream : BlockStream
    {
        private Byte[][] m_MemoryBlocks;

        public _RdramStream()
        {
            m_MemoryBlocks = new Byte[BlockCount][];
        }

        public override int BlockSize
        {
            get { return 0x100000 / 2; } /* 512 KB */
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
            get { return 0x03FFFFFF + 1; }
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            switch (origin)
            {
                case System.IO.SeekOrigin.Begin: return offset;
                case System.IO.SeekOrigin.Current: return Position + offset;
                case System.IO.SeekOrigin.End: return Position - offset;
                default: return offset;
            }
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        protected override void LoadBlock(bool writeMode, Lazy<byte[]> lazyBuffer, Int64 position)
        {
            if (writeMode)
            {
                Byte[] buffer = lazyBuffer.Value;
                Array.Clear(buffer, 0, buffer.Length);
            }
        }

        public override string ToString()
        {
            return "RDRAM Memory";
        }
    }
}