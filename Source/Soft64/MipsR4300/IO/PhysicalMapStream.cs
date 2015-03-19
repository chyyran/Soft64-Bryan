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

using System.IO;

namespace Soft64.MipsR4300.IO
{
    internal sealed class PhysicalMapStream : Stream
    {
        private Stream m_PhysicalMemoryStream;

        public PhysicalMapStream(Stream stream)
        {
            m_PhysicalMemoryStream = stream;
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
            m_PhysicalMemoryStream.Flush();
        }

        public override long Length
        {
            get { return 0x20000000; }
        }

        public override long Position
        {
            get { return m_PhysicalMemoryStream.Position; }
            set { m_PhysicalMemoryStream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return m_PhysicalMemoryStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return m_PhysicalMemoryStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            m_PhysicalMemoryStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            m_PhysicalMemoryStream.Write(buffer, offset, count);
        }
    }
}