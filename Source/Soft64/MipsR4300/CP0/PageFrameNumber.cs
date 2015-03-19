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

namespace Soft64.MipsR4300.CP0
{
    /// <summary>
    /// Stores physical page information used in TLB address translation.
    /// </summary>
    [CLSCompliant(false)]
    public struct PageFrameNumber
    {
        private Int32 m_PFN; // Page Frame Number
        private Byte m_C;    // Coherency bits
        private Boolean m_D; // Dirty
        private Boolean m_V; // Valid
        private Boolean m_G; // Global

        /// <summary>
        /// Initializes the page frame number using a entry lo register.
        /// </summary>
        /// <param name="entryLoReg"></param>
        public PageFrameNumber(UInt64 entryLoReg)
        {
            m_G = Convert.ToBoolean(entryLoReg & 1);
            m_V = Convert.ToBoolean((entryLoReg >> 1) & 1);
            m_D = Convert.ToBoolean((entryLoReg >> 2) & 1);
            m_C = Convert.ToByte((entryLoReg >> 3) & 0xFF);
            m_PFN = Convert.ToInt32((entryLoReg >> 6) & 0x1FFFFFF);
        }

        public PageFrameNumber(Int32 pfn)
        {
            m_D = false;
            m_V = true;
            m_G = false;
            m_C = 0;
            m_PFN = pfn;
        }

        private PageCoherencyMode ToCohMode(byte p)
        {
            switch (p)
            {
                case 0:
                case 1:
                case 7: return PageCoherencyMode.Reserved;
                case 2: return PageCoherencyMode.Uncached;
                case 3: return PageCoherencyMode.Noncoherent;
                case 4: return PageCoherencyMode.Exclusive;
                case 5: return PageCoherencyMode.Sharable;
                case 6: return PageCoherencyMode.Update;
                default: return PageCoherencyMode.Reserved;
            }
        }

        /// <summary>
        /// Gets the page frame number.
        /// </summary>
        public Int32 PFN
        {
            get { return m_PFN; }
            set { m_PFN = value; }
        }

        /// <summary>
        /// Gets the coherency mode that determines whether the page access hits the cache.
        /// </summary>
        public PageCoherencyMode CoherencyMode
        {
            get { return ToCohMode(m_C); }
        }

        public Byte CoherencyBits
        {
            get { return m_C; }
            set { m_C = value; }
        }

        /// <summary>
        /// Gets whether if the page can be written to.
        /// </summary>
        public Boolean IsDirty
        {
            get { return m_D; }
            set { m_D = value; }
        }

        /// <summary>
        /// Gets whether the page is valid to be used in address translation.
        /// </summary>
        public Boolean IsValid
        {
            get { return m_V; }
            set { m_V = value; }
        }

        /// <summary>
        /// Gets whether the page ignores the ASID feature.
        /// </summary>
        public Boolean IsGlobal
        {
            get { return m_G; }
            set { m_G = value; }
        }

        public Int64 ToVirtual(Int32 offset, Int32 offsetBitSize)
        {
            return (m_PFN << offsetBitSize) + offset;
        }

        public Int64 MapPhysical(PageSize pageSize, UInt64 virtualAddress)
        {
            UInt64 address = 0;

            unchecked
            {
                address |= (UInt64)m_PFN * (UInt64)pageSize.Size;
                address |= (UInt64)virtualAddress & pageSize.AddressOffsetMask;
            }

            return (Int64)address;
        }

        public static implicit operator PageFrameNumber(UInt64 value)
        {
            return new PageFrameNumber(value);
        }

        public static implicit operator UInt64(PageFrameNumber pfn)
        {
            UInt64 value = 0;

            value |= Convert.ToUInt64(pfn.m_G);
            value |= Convert.ToUInt64(pfn.m_V) << 1;
            value |= Convert.ToUInt64(pfn.m_D) << 2;
            value |= Convert.ToUInt64(pfn.m_C) << 3;
            value |= Convert.ToUInt64(pfn.m_PFN) << 6;

            return value;
        }
    }
}