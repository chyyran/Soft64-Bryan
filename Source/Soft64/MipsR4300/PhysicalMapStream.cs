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

namespace Soft64.MipsR4300
{
    internal sealed class PhysicalMapStream : Stream
    {
        private Int64 m_Position;

        public PhysicalMapStream()
        {
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
            Machine.Current.N64MemorySafe.Flush();
        }

        public override long Length
        {
            get { return 0x20000000; }
        }

        public override long Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Int32 read = 0;
            Machine.Current.N64MemorySafe.Position = m_Position;
            read = Machine.Current.N64MemorySafe.Read(buffer, offset, count);
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            Int64 pos = 0;
            pos = Machine.Current.N64MemorySafe.Seek(offset, origin);
            return pos;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Machine.Current.N64MemorySafe.Position = m_Position;
            Machine.Current.N64MemorySafe.Write(buffer, offset, count);
        }
    }
}