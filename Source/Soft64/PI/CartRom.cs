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
using System.Text;

/* = NOTES ==============================
 * Rom should have its own custom checksum in case the emulator wants check if has been modified in RAM
 * The CRC could be its own type to make it simpler to reference
 *
 * = Byte-swapping Notes =================
 * All rom images are Big endian binary format for the N64, so they MUST be byteswapped for our Intel machines.
 * There are 2 kinds of cartridge dumps we must account for when doing byteswapping (.z64 are already swapped to Intel binary format)
 * .v64 - swapped every half word (this is more rare)
 * .n64 - big endian format, has to be swapped every 4 bytes
 *
 * = PI Bus Notes =======================
 * The PI manages its own bus which has special features allow different memory configuations based on the attachements connected to the system board.  Originally
 * the N64 was going to have a diskdrive and a cartridge be attached at the same time to increase ROM space and provide more RAM too.  Since the diskdrive
 * failed before launch, all the unused cartridge domains became use for accessing flash memory instead.  This is my educated guess since the PI maintains
 * reserved regions on the PI bus called Cartridge Domains.  Each domain has its own configurable bus timing, and it is obvious that the diskdrive was going
 * to add more memory to those domains such as more readable Rom (the disk itself), and its own drive memory increasing RAM to 16MB (my guess.)  I know for the fact
 * that cartridges provides save memory (SRAM) that mounted always to one of the domains.  The cartridge ROM size is limited probably due to the built-in reserved
 * domain size that always access the inserted cartridge ROM.  Another interesting thing, is there is a domain that is past the PIF boundaries and its function is
 * unknown.  It is likely unimportant and never used, even with a diskdrive mounted.
 */

namespace Soft64.PI
{
    public class CartRom
    {
        public const Int32 HeaderSize = 64;

        private Stream m_RomStream;
        private BootRom m_BootRom;
        private Boolean m_HeaderOnly = false;

        public const UInt32 ClockRateMask = 0xFFFFFFF0;

        /* Header information */
        private PiBusSpeedConfig m_BusConfig;
        private String m_Name;
        private Int32 m_Clockrate;
        private Int64 m_EntryPoint;
        private Int32 m_Release;
        private GameSerial m_SerialCode;
        private Int32 m_CRC1;
        private Int32 m_CRC2;
        private RegionType m_Region;

        public CartRom(Stream romDataStream, Boolean headerOnly, FormatVerifyFlag verifyFlag)
        {
            m_HeaderOnly = headerOnly;

            /* Check some properties about the source stream */
            if (romDataStream.Length < (HeaderSize + BootRom.Size))
                throw new ArgumentOutOfRangeException("Rom is too small to be valid");

            if (!romDataStream.CanRead)
                throw new ArgumentException("The source rom stream is not readable");

            /* Verify the rom format and make the nessarily action */
            RomFormat format = RomFormat.Unknown;

            switch (verifyFlag)
            {
                case FormatVerifyFlag.Skip: break;
                case FormatVerifyFlag.Header: format = Cartridge.DetectFormatHeader(romDataStream); break;
                case FormatVerifyFlag.Cic: format = Cartridge.DectectFormatCIC(romDataStream); break;
                case FormatVerifyFlag.Crc: format = Cartridge.DectectFormatCRC(romDataStream); break;
            }

            if (verifyFlag != FormatVerifyFlag.Skip && format == RomFormat.Unknown)
            {
                // SystemEventLog.WriteError("Tried to verify the rom but failed, emulator may crash or be unstable.", LogType.PI);
            }
            else
            {
                romDataStream = Cartridge.GetCorrectStream(romDataStream, format);
            }

            /* Create the header */
            CreateHeader(romDataStream);

            /* Create the bootcode information */
            m_BootRom = new BootRom(romDataStream);

            if (!headerOnly)
            {
                m_RomStream = romDataStream;
                romDataStream.Position = 0;
            }
        }

        private void CreateHeader(Stream romDataStream)
        {
            /* We convert the rom header into high level information */
            romDataStream.Position = 0;

            BinaryReader binReader = new BinaryReader(romDataStream);

            m_BusConfig = new PiBusSpeedConfig(binReader.ReadByte(),
                                     binReader.ReadByte(),
                                     binReader.ReadByte(),
                                     binReader.ReadByte());

            m_Clockrate = binReader.ReadInt32();
            m_EntryPoint = binReader.ReadInt32();
            m_Release = binReader.ReadInt32();
            m_CRC1 = binReader.ReadInt32();
            m_CRC2 = binReader.ReadInt32();

            romDataStream.Position += 8;

            Byte[] nameBytes = new Byte[20];
            binReader.Read(nameBytes, 0, nameBytes.Length);
            m_Name = ASCIIEncoding.ASCII.GetString(nameBytes);

            romDataStream.Position += 4;

            Byte[] serial = new Byte[8];
            binReader.Read(serial, 0, serial.Length);
            m_SerialCode = new GameSerial(serial);
            m_Region = m_SerialCode.GetRegionType();
        }

        public PiBusSpeedConfig BusConfig { get { return m_BusConfig; } }

        public String Name { get { return m_Name; } }

        public Int32 Clockrate { get { return (Int32)(m_Clockrate & ClockRateMask); } }

        public Int64 EntryPoint { get { return m_EntryPoint; } }

        public GameSerial Serial { get { return m_SerialCode; } }

        public Int32 CRC1 { get { return m_CRC1; } }

        public Int32 CRC2 { get { return m_CRC2; } }

        public Boolean IsHeaderOnly { get { return m_HeaderOnly; } }

        public BootRom BootRomInformation { get { return m_BootRom; } }

        public RegionType Region { get { return m_Region; } }

        public Int32 Release
        {
            get { return m_Release; }
        }

        public Int32 GetAIDacRate()
        {
            switch (m_Region)
            {
                case RegionType.PAL: return 49656530;
                case RegionType.MPAL: return 48628316;
                case RegionType.NTSC:
                default:
                    return 48681812;
            }
        }

        public Int32 GetVIRate()
        {
            if (m_Region == RegionType.NTSC || m_Region == RegionType.MPAL)
                return 60;
            else
                return 50;
        }

        public Stream RomStream
        {
            get { return m_RomStream; }
        }
    }
}