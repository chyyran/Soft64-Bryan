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
using Soft64.IO;

namespace Soft64.DeviceMemory
{
    public sealed class RdramStream : BlockStream
    {
        private Byte[][] m_MemoryBlocks;

        public RdramStream()
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