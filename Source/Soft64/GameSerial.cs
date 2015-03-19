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
using System.Text;

namespace Soft64
{
    public sealed class GameSerial
    {
        private Byte[] m_Serial;

        public GameSerial(Byte[] serial)
        {
            m_Serial = serial;
        }

        public String ManufacturerID
        {
            get { return ASCIIEncoding.ASCII.GetString(m_Serial, 0, 4).Replace('\0', ' ').Trim(); }
        }

        public String CountryID
        {
            get { return ASCIIEncoding.ASCII.GetString(m_Serial, 4, 2).Replace('\0', ' ').Trim(); }
        }

        public String CartridgeID
        {
            get { return ASCIIEncoding.ASCII.GetString(m_Serial, 6, 2).Replace('\0', ' ').Trim(); }
        }

        public String Serial
        {
            get { return ASCIIEncoding.ASCII.GetString(m_Serial, 0, 8).Replace('\0', ' ').Trim(); }
        }

        public RegionType GetRegionType()
        {
            switch (m_Serial[5])
            {
                case 0x44:
                case 0x46:
                case 0x49:
                case 0x50:
                case 0x53:
                case 0x55:
                case 0x58:
                case 0x59:
                    return RegionType.PAL;

                case 0x37:
                case 0x41:
                case 0x45:
                case 0x4A:
                default:
                    return RegionType.NTSC;
            }
        }

        public override int GetHashCode()
        {
            Int32 hash = 0;

            hash |= m_Serial[0];
            hash <<= 8;
            hash |= m_Serial[1];
            hash <<= 8;
            hash |= m_Serial[2];
            hash <<= 8;
            hash |= m_Serial[3];
            hash <<= 8;
            hash |= m_Serial[4];
            hash <<= 8;
            hash |= m_Serial[5];
            hash <<= 8;
            hash |= m_Serial[6];
            hash <<= 8;
            hash |= m_Serial[7];

            return hash;
        }

        public override string ToString()
        {
            if (!String.IsNullOrEmpty(Serial))
                return Serial;
            else
                return "<Empty Value>";
        }
    }
}