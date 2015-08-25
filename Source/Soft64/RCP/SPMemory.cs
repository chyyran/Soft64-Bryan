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

/* Notes about the SP Memory
 * ----------------------------
 * It is divided into 2 regions called IMEM and DMEM.
 * DMEM can store 16 segment base registers for SP virtual address (segment base + segment offset)
 */

namespace Soft64.RCP
{
    /// <summary>
    /// Stores memory used by the Signal processor.
    /// </summary>
    public sealed class SPMemory : Stream
    {
        private MemoryStream m_SPMemoryStream;

        public SPMemory()
        {
            m_SPMemoryStream = new MemoryStream(new Byte[0x1FFE + 1]);
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
            m_SPMemoryStream.Flush();
        }

        public override long Length
        {
            get { return m_SPMemoryStream.Length; }
        }

        public override long Position
        {
            get
            {
                return m_SPMemoryStream.Position;
            }
            set
            {
                m_SPMemoryStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (m_SPMemoryStream.Position < m_SPMemoryStream.Length)
                return m_SPMemoryStream.Read(buffer, offset, count);
            else
                return -1;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return m_SPMemoryStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (m_SPMemoryStream.Position < m_SPMemoryStream.Length)
                m_SPMemoryStream.Write(buffer, offset, count);
        }

        public override string ToString()
        {
            return "Signal Processor Memory";
        }
    }
}