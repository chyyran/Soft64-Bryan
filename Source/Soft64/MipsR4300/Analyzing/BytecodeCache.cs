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
using System.Collections.Generic;
using System.IO;

namespace Soft64.MipsR4300.Analyzing
{
    public sealed class BytecodeCache
    {
        private Stream m_SourceStream;
        private Byte[] m_Cache;
        private const Int32 c_CacheLineSize = 4 * 15;
        private const Int32 c_CacheSize = c_CacheLineSize * 20;
        private Dictionary<Int64, Int32> m_CacheLineLUT;

        public BytecodeCache(Stream sourceStream)
        {
            m_SourceStream = sourceStream;
            m_Cache = new Byte[c_CacheSize];
            m_CacheLineLUT = new Dictionary<long, int>();
        }

        public void Fill(Int64 programCounter)
        {
            Int32 slotIndex = 0;
            Int64 key = GetKey(programCounter);

            if (!m_CacheLineLUT.TryGetValue(key, out slotIndex))
            {
                slotIndex = m_CacheLineLUT.Count;

                if (m_SourceStream.CanRead)
                {
                    if (slotIndex * c_CacheLineSize >= m_Cache.Length)
                    {
                        m_CacheLineLUT.Clear();
                        slotIndex = 0;
                    }

                    m_SourceStream.Position = programCounter;
                    m_SourceStream.Read(m_Cache, slotIndex * c_CacheLineSize, c_CacheLineSize);
                    m_CacheLineLUT.Add(key, slotIndex);
                }
                else
                {
                    throw new IOException("Source stream is marked non-readable");
                }
            }
        }

        public Byte[] FetchBytecodeBlock(Int64 programCounter, out Int32 blockOffset)
        {
            Int64 key = GetKey(programCounter);
            blockOffset = GetOffset(key, programCounter);
            Int32 slotIndex = 0;

            Fill(programCounter);

            if (m_CacheLineLUT.TryGetValue(key, out slotIndex))
            {
                Byte[] buffer = new Byte[c_CacheLineSize];
                Array.Copy(m_Cache, slotIndex * c_CacheLineSize, buffer, 0, buffer.Length);
                return buffer;
            }
            else
            {
                throw new InvalidOperationException("Could not find cache line block at address: " + programCounter.ToString("X8"));
            }
        }

        private Int64 GetKey(Int64 position)
        {
            return position & c_CacheLineSize;
        }

        private Int32 GetOffset(Int64 key, Int64 position)
        {
            return (Int32)(position - key);
        }

        public Int32 BlockSize
        {
            get { return c_CacheLineSize; }
        }
    }
}