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

namespace Soft64.MipsR4300
{
    public struct VirtualPageNumber2
    {
        private Byte m_Asid;
        private UInt64 m_Vpn2;

        public VirtualPageNumber2(UInt64 entryHi)
            : this()
        {
            m_Asid = Convert.ToByte(entryHi & 0xFF);
            m_Vpn2 = entryHi >> 12;
        }

        public VirtualPageNumber2(Byte asid, UInt64 vpn2)
        {
            m_Asid = asid;
            m_Vpn2 = vpn2;
        }

        public override int GetHashCode()
        {
            return EntryHi.GetHashCode();
        }

        public UInt64 EntryHi
        {
            get { return (m_Vpn2 << 12) | m_Asid; }
        }

        public Int64 ComparableVPN2
        {
            get { return (Int64)Vpn2; }
        }

        public Byte Asid
        {
            get { return m_Asid; }
            set { m_Asid = value; }
        }

        public UInt64 Vpn2
        {
            get { return m_Vpn2; }
            set { m_Vpn2 = value; }
        }
    }
}