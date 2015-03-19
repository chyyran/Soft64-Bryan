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

namespace Soft64.IO
{
    public abstract class BlockStream : Stream
    {
        private Int64 m_Position;
        private Lazy<Byte[][]> m_LazyMemoryBlocks;

        protected BlockStream()
        {
            ClearCache();
        }

        public abstract Int32 BlockSize { get; }

        private Boolean BlockExists(Int32 index)
        {
            return m_LazyMemoryBlocks.Value[index] != null;
        }

        protected abstract void LoadBlock(Boolean writeMode, Lazy<Byte[]> lazyBuffer, Int64 position);

        protected Int32 BlockCount
        {
            get
            {
                return (Int32)(Length / BlockSize) + 1;
            }
        }

        public abstract override bool CanRead { get; }

        public abstract override bool CanSeek { get; }

        public abstract override bool CanWrite { get; }

        public abstract override void Flush();

        public abstract override long Length { get; }

        public sealed override long Position
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

        public sealed override int Read(byte[] buffer, int offset, int count)
        {
            Int32 read = 0;

            while (count > 0)
            {
                /* Get the block offset and index */
                Int64 blockOffset = Position % BlockSize;
                Int32 blockIndex = (Int32)(Position / BlockSize);

                /* Check if a new block should be created */
                if (!BlockExists(blockIndex))
                {
                    var _c = (count >= BlockSize) ? BlockSize : count;

                    /* Use a lazy class to determine if the child class will load a buffer */
                    Lazy<Byte[]> lazyBuffer = new Lazy<byte[]>(() => new Byte[BlockSize]);
                    LoadBlock(false, lazyBuffer, blockIndex * BlockSize);

                    /* If the buffer has been initialized, then put it in the table and read from it */
                    if (lazyBuffer.IsValueCreated)
                    {
                        m_LazyMemoryBlocks.Value[blockIndex] = lazyBuffer.Value;
                        Array.Copy(m_LazyMemoryBlocks.Value[blockIndex], blockOffset, buffer, offset, _c);
                    }

                    count -= _c;
                    read += _c;
                    offset += _c;
                    m_Position += _c;
                    continue;
                }

                var _Count = (count <= BlockSize - blockOffset ? count : (BlockSize - (Int32)blockOffset));

                Array.Copy(m_LazyMemoryBlocks.Value[blockIndex], blockOffset, buffer, offset, _Count);

                count -= _Count;
                read += _Count;
                offset += _Count;
                m_Position += _Count;
            }

            return read;
        }

        public abstract override long Seek(long offset, SeekOrigin origin);

        public abstract override void SetLength(long value);

        public sealed override void Write(byte[] buffer, int offset, int count)
        {
            while (count > 0)
            {
                Int64 blockOffset = Position % BlockSize;
                Int32 blockIndex = (Int32)(Position / BlockSize);

                if (!BlockExists(blockIndex))
                {
                    /* Use a lazy class to determine if the child class will load a buffer */
                    Lazy<Byte[]> lazyBuffer = new Lazy<byte[]>(() => new Byte[BlockSize]);
                    LoadBlock(true, lazyBuffer, blockIndex * BlockSize);

                    /* If the buffer has been initialized, then put it in the table and read from it */
                    if (lazyBuffer.IsValueCreated)
                    {
                        m_LazyMemoryBlocks.Value[blockIndex] = lazyBuffer.Value;
                    }
                    else
                    {
                        return;
                    }
                }

                var _Count = 0;

                if (count <= BlockSize - blockOffset)
                {
                    _Count = count;
                }
                else
                {
                    _Count = (Int32)(count - (BlockSize - blockOffset));
                }

                Array.Copy(buffer, offset, m_LazyMemoryBlocks.Value[blockIndex], blockOffset, _Count);

                count -= _Count;
                offset += _Count;
                m_Position += _Count;
            }
        }

        public void ClearCache()
        {
            m_LazyMemoryBlocks = new Lazy<Byte[][]>(() => new Byte[BlockCount][]);
        }
    }
}