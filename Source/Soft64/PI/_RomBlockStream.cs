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

namespace Soft64.PI
{
    internal sealed class _RomBlockStream : BlockStream
    {
        private Stream m_RomSourceStream;
        private Boolean m_Disposed;

        public _RomBlockStream(Stream romSourceStream)
            : base()
        {
            m_RomSourceStream = romSourceStream;
        }

        public override int BlockSize
        {
            get { return 1024 * 15; }
        }

        protected override void LoadBlock(bool writeMode, Lazy<byte[]> lazyBuffer, Int64 position)
        {
            m_RomSourceStream.Position = position;
            Byte[] buffer = lazyBuffer.Value;
            m_RomSourceStream.Read(buffer, 0, buffer.Length);
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
            get { return m_RomSourceStream.CanWrite; }
        }

        public override void Flush()
        {
            m_RomSourceStream.Flush();
        }

        public override long Length
        {
            get { return m_RomSourceStream.Length; }
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            return m_RomSourceStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            m_RomSourceStream.SetLength(value);
        }

        protected override void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    m_RomSourceStream.Dispose();
                }

                m_Disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}