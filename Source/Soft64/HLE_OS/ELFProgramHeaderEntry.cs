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

namespace Soft64.HLE_OS
{
    public struct ELFProgramHeaderEntry
    {
        private const Int32 HeaderSize = 64;
        private Int32 m_Type;
        private Int32 m_Offset;
        private Int32 m_VAddress;
        private Int32 m_PAddress;
        private Int32 m_FileSize;
        private Int32 m_MemorySize;
        private Int32 m_Flags;
        private Int32 m_Align;

        internal ELFProgramHeaderEntry(Stream source)
        {
            BinaryReader binReader = new BinaryReader(source);

            /* Read the header */
            m_Type = binReader.ReadInt32();
            m_Offset = binReader.ReadInt32();
            m_VAddress = binReader.ReadInt32();
            m_PAddress = binReader.ReadInt32();
            m_FileSize = binReader.ReadInt32();
            m_MemorySize = binReader.ReadInt32();
            m_Flags = binReader.ReadInt32();
            m_Align = binReader.ReadInt32();
        }

        internal void CopySegment(Stream elfStream, Stream destStream)
        {
            elfStream.Position = m_Offset;
            destStream.Position = m_VAddress;
            Byte[] buffer = new Byte[m_FileSize];
            elfStream.Read(buffer, 0, buffer.Length);
            destStream.Write(buffer, 0, buffer.Length);
        }

        public Int32 Type
        {
            get { return m_Type; }
        }

        public Int32 Offset
        {
            get { return m_Offset; }
        }

        public Int32 VAddress
        {
            get { return m_VAddress; }
        }

        public Int32 PAddress
        {
            get { return m_PAddress; }
        }

        public Int32 MemorySize
        {
            get { return m_MemorySize; }
        }

        public Int32 FileSize
        {
            get { return m_FileSize; }
        }

        public Int32 Flags
        {
            get { return m_Flags; }
        }

        public Int32 Align
        {
            get { return m_Align; }
        }
    }
}