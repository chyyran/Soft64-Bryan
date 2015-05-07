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
using System.Linq;
using Soft64.IO;

namespace Soft64
{
    public sealed class ELFExecutable : IDisposable
    {
        private Stream m_ElfStream;
        private String m_Filename;
        private Byte m_BitFormat;
        private Byte m_Endianess;
        private Byte m_Version;
        private Byte m_ABI;
        private Byte m_ABIVersion;
        private UInt16 m_Type;
        private UInt16 m_MachineTarget;
        private UInt32 m_Version2;
        private UInt64 m_EntryPointAddress;
        private UInt64 m_ProgramHeaderTableAddress;
        private UInt64 m_SectionHeaderTableAddress;
        private UInt32 m_Flags;
        private UInt16 m_HeaderSize;
        private UInt16 m_ProgramHeaderTableEntrySize;
        private UInt16 m_ProgramHeaderTableEntryCount;
        private UInt16 m_SectionHeaderTableEntrySize;
        private UInt16 m_SectionHeaderTableEntryCount;
        private UInt16 m_SectionNamesIndex;
        private ELFProgramHeaderEntry[] m_ProgramHeaderEntries;

        public ELFExecutable(String name, Stream inStream)
        {
            m_Filename = name;
            inStream.Position = 0;

            /* First do a magic number check */
            Byte[] magic = new Byte[4];
            inStream.Read(magic, 0, magic.Length);
            if (!(
                magic[0] == (Byte)0x7F &&
                magic[1] == (Byte)'E' &&
                magic[2] == (Byte)'L' &&
                magic[3] == (Byte)'F'))
            {
                throw new InvalidDataException("Bad ELF magic check");
            }

            /* Read the header that is platform independent */
            m_BitFormat = (Byte)inStream.ReadByte();
            m_Endianess = (Byte)inStream.ReadByte();
            m_Version = (Byte)inStream.ReadByte();
            m_ABI = (Byte)inStream.ReadByte();
            m_ABIVersion = (Byte)inStream.ReadByte();

            /* Padding */
            inStream.Position += 7;

            BinaryReader reader_16;
            BinaryReader reader_32;

            if (m_Endianess == 2)
            {
                reader_16 = new BinaryReader(new Int16SwapStream(inStream));
                reader_32 = new BinaryReader(new Int32SwapStream(inStream));
            }
            else
            {
                reader_16 = new BinaryReader(inStream);
                reader_32 = new BinaryReader(inStream);
            }

            BinaryReader binReader = new BinaryReader(inStream);

            /* Platform specific byte ordered data */
            m_Type = reader_16.ReadUInt16();
            m_MachineTarget = reader_16.ReadUInt16();
            m_Version2 = reader_32.ReadUInt32();
            m_EntryPointAddress = reader_32.ReadUInt32();
            m_ProgramHeaderTableAddress = reader_32.ReadUInt32();
            m_SectionHeaderTableAddress = reader_32.ReadUInt32();
            m_Flags = reader_32.ReadUInt32();
            m_HeaderSize = reader_16.ReadUInt16();
            m_ProgramHeaderTableEntrySize = reader_16.ReadUInt16();
            m_ProgramHeaderTableEntryCount = reader_16.ReadUInt16();
            m_SectionHeaderTableEntrySize = reader_16.ReadUInt16();
            m_SectionHeaderTableEntryCount = reader_16.ReadUInt16();
            m_SectionNamesIndex = reader_16.ReadUInt16();

            m_ElfStream = inStream;

            m_ProgramHeaderEntries = GetProgramHeaderEntries();
        }

        private ELFProgramHeaderEntry[] GetProgramHeaderEntries()
        {
            ELFProgramHeaderEntry[] entries = new ELFProgramHeaderEntry[m_ProgramHeaderTableEntryCount];
            Stream readStream = new Int32SwapStream(m_ElfStream);
            m_ElfStream.Position = (Int64)m_ProgramHeaderTableAddress;

            for (Int32 i = 0; i < m_ProgramHeaderTableEntryCount; i++)
            {
                Int64 pos = readStream.Position;
                entries[i] = new ELFProgramHeaderEntry(readStream);
                readStream.Position = pos + m_ProgramHeaderTableEntrySize;
            }

            return entries;
        }

        public void CopyProgramSegment(
            ELFProgramHeaderEntry entry, 
            Boolean nextSegmentValid, 
            ELFProgramHeaderEntry nextSegment, 
            Stream destStream,
            Int64 destPosition)
        {
            Int32 prefixPadSize = entry.VAddress % entry.Align;

            /* Write to the physical memory directly */
            destStream.Position = destPosition + (entry.VAddress - prefixPadSize);

            /* Write 0 padding before the segment */
            Byte[] padding = new Byte[prefixPadSize];
            destStream.Write(padding, 0, padding.Length);

            /* Write the segment to memory */
            entry.CopySegment(m_ElfStream, destStream);

            /* Create extra data required by the segment */
            if (entry.MemorySize > entry.FileSize)
            {
                Byte[] extra = new Byte[entry.MemorySize - entry.FileSize];
                destStream.Write(extra, 0, extra.Length);
            }

            /* Now do the padding on the end */
            if (nextSegmentValid)
            {
                Int64 vaddress = nextSegment.VAddress - (nextSegment.VAddress & nextSegment.Align);
                destStream.Position = vaddress;
                Byte[] buffer = new Byte[vaddress - destStream.Position];
                destStream.Write(buffer, 0, buffer.Length);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public UInt64 EntryPointAddress
        {
            get { return m_EntryPointAddress; }
        }

        public ELFProgramHeaderEntry[] ProgramHeaderEntries
        {
            get
            {
                ELFProgramHeaderEntry[] entries = new ELFProgramHeaderEntry[m_ProgramHeaderEntries.Length];
                Array.Copy(m_ProgramHeaderEntries, entries, m_ProgramHeaderEntries.Length);
                return entries;
            }
        }
    }
}