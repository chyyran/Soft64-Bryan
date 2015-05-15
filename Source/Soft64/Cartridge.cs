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
using Soft64.PI;

/* Notes
 * ---------------------------
 * Cartridges have a JTAG clock pin that could probably let the N64 debug it?
 */

namespace Soft64
{
    public abstract class Cartridge : IDisposable
    {
        private Boolean m_Diposed;
        private static Cartridge s_Current;

        /* Cartridge Interrupts */

        public event EventHandler VideoClockTick;

        public event EventHandler Tick;

        public static Cartridge Current
        {
            get { return s_Current; }
        }

        protected Cartridge()
        {
            s_Current = this;
        }

        #region Cartrige API

        public abstract void Open();

        public void Close()
        {
            Dispose();
        }

        public CartridgeInfo GetCartridgeInfo()
        {
            return new CartridgeInfo(this);
        }

        public virtual ICartRom RomImage { get; protected set; }

        public abstract Stream PiCartridgeStream { get; }

        public Boolean IsReal { get; protected set; }

        public virtual Boolean IsOpened { get; protected set; }

        #endregion Cartrige API

        #region PIF-CIC Access API

        public CICKey LockoutKey { get; protected set; }

        #endregion PIF-CIC Access API

        #region Reset API

        public abstract void ColdReset();

        public abstract void NMIReset();

        #endregion Reset API

        #region Static Format Detect Functions

        public static RomFormat DetectFormatHeader(Stream romStream)
        {
            romStream.Position = 0;

            /* BinaryReader converts reads into little endian */
            BinaryReader reader = new BinaryReader(new Int32SwapStream(romStream));
            UInt32 check = reader.ReadUInt32();

            switch (check)
            {
                case 0x80371240: return RomFormat.Z64;
                case 0x40123780: return RomFormat.N64;
                case 0x37804012: return RomFormat.V64;
                default: return RomFormat.Unknown;
            }
        }

        private static BinaryReader GetCrcBinReader(Stream romStream)
        {
            var reader = new BinaryReader(romStream);
            reader.BaseStream.Position = 16;
            return reader;
        }

        public static RomFormat DectectFormatCIC(Stream romStream)
        {
            try
            {
                var result = (
                    from fmt in new RomFormat[] { RomFormat.Z64, RomFormat.V64, RomFormat.N64 }
                    let stream = GetCorrectStream(romStream, fmt)
                    where new BootRom(stream).CIC != CICKeyType.Unknown
                    select fmt
                        ).Single();

                if (romStream is _RomBlockStream)
                    (romStream as _RomBlockStream).ClearCache();

                GC.Collect();

                return result;
            }
            catch (Exception)
            {
                return RomFormat.Unknown;
            }
        }

        public static RomFormat DectectFormatCRC(Stream romStream)
        {
            /* Test each known rom format and see if the CIC is valid and the CRC's match */

            try
            {
                var result = (
                    from fmt in new RomFormat[] { RomFormat.Z64, RomFormat.V64, RomFormat.N64 }
                    let stream = GetCorrectStream(romStream, fmt)
                    let reader = GetCrcBinReader(stream)
                    let crcs = new { Crc1 = reader.ReadInt32(), Crc2 = reader.ReadUInt32() }
                    let cic = new BootRom(stream).CIC
                    let testDat = CopyTestDatFromRom(stream, fmt)
                    let romcs = new RomChecksum(cic)
                    let temp = romcs.ComputeHash(testDat, 0, RomChecksum.InputSize)
                    where crcs.Crc1 == romcs.CRC1 && crcs.Crc2 == romcs.CRC2
                    select fmt
                    ).Single();

                if (romStream is _RomBlockStream)
                    (romStream as _RomBlockStream).ClearCache();

                GC.Collect();

                return result;
            }
            catch (Exception)
            {
                return RomFormat.Unknown;
            }
        }

        public static Stream GetCorrectStream(Stream str, RomFormat format)
        {
            switch (format)
            {
                case RomFormat.V64: return new Int16SwapStream(str);
                case RomFormat.N64: return new Int32SwapStream(str);
                default: return str;
            }
        }

        private static Byte[] CopyTestDatFromRom(Stream romStream, RomFormat format)
        {
            romStream.Position = 0;
            Byte[] buff = new Byte[RomChecksum.InputSize];

            switch (format)
            {
                case RomFormat.Z64:
                case RomFormat.V64: romStream = new Int32SwapStream(romStream); break;
                default: break;
            }

            romStream.Read(buff, 0, buff.Length);
            return buff;
        }

        #endregion Static Format Detect Functions

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(Boolean disposing)
        {
            if (!m_Diposed)
            {
                if (disposing)
                {
                    LockoutKey = null;
                    RomImage = null;
                    PiCartridgeStream.Dispose();
                    IsOpened = false;
                }

                m_Diposed = true;
            }
        }

        #endregion IDisposable Members
    }
}