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

namespace Soft64.Debugging
{
    internal sealed class PMemMirrorStream : Stream
    {
        private Int64 m_PhysicalOffset;
        private Int64 m_MirrorSize;
        private Int64 m_CurrentOffset;
        private Stream m_SourceStream;

        public PMemMirrorStream(Stream physicalStream, Int64 physicalOffset, Int64 mirrorSize)
        {
            m_PhysicalOffset = physicalOffset;
            m_MirrorSize = mirrorSize;
            m_CurrentOffset = 0;
            m_SourceStream = physicalStream;
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
            m_SourceStream.Flush();
        }

        public override long Length
        {
            get { return m_MirrorSize; }
        }

        public override long Position
        {
            get
            {
                return m_CurrentOffset;
            }
            set
            {
                m_CurrentOffset = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (m_CurrentOffset + count >= Length)
            {
                count = (Int32)((m_CurrentOffset + count) - (Length - 1));
            }

            m_SourceStream.Position = m_PhysicalOffset + m_CurrentOffset;
            m_SourceStream.Read(buffer, offset, count);
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin: return offset;
                case SeekOrigin.Current: return m_CurrentOffset + offset;
                case SeekOrigin.End: return m_CurrentOffset - offset;
                default: return -1;
            }
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (m_CurrentOffset + count >= Length)
            {
                count = (Int32)((m_CurrentOffset + count) - (Length - 1));
            }

            m_SourceStream.Position = m_PhysicalOffset + m_CurrentOffset;
            m_SourceStream.Write(buffer, offset, count);
        }
    }
}