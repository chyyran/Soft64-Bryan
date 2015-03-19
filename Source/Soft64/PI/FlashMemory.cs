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

namespace Soft64.Media.Flash
{
    public abstract class FlashMemory : Stream
    {
        private MemoryStream m_FlashMemory;

        public sealed override bool CanRead { get { return true; } }

        public sealed override bool CanSeek { get { return true; } }

        public sealed override bool CanWrite { get { return true; } }

        public abstract override void Flush();

        public sealed override long Length
        {
            get { return m_FlashMemory.Length; }
        }

        public sealed override long Position
        {
            get
            {
                return m_FlashMemory.Position;
            }
            set
            {
                m_FlashMemory.Position = value;
            }
        }

        public sealed override int Read(byte[] buffer, int offset, int count)
        {
            return m_FlashMemory.Read(buffer, offset, count);
        }

        public sealed override long Seek(long offset, SeekOrigin origin)
        {
            return m_FlashMemory.Seek(offset, origin);
        }

        public sealed override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public sealed override void Write(byte[] buffer, int offset, int count)
        {
            m_FlashMemory.Write(buffer, offset, count);
        }
    }
}