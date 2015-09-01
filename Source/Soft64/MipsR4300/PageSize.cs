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
    public enum NamedPageSize
    {
        SizeUnknown,
        Size4KB,
        Size16KB,
        Size64KB,
        Size256KB,
        Size1MB,
        Size4MB,
        Size16MB
    }

    public struct PageSize
    {
        private Int32 m_Size;

        public PageSize(UInt64 pageMaskRegister)
        {
            /* This converts the page mask into the equalivent TLB page size */
            m_Size = 1 + (Int32)((pageMaskRegister >> 1) | 0xFFF);
        }

        public PageSize(Int32 pageSize)
        {
            m_Size = pageSize;
        }

        public PageSize(NamedPageSize namedSize)
        {
            if (namedSize > NamedPageSize.SizeUnknown && namedSize <= NamedPageSize.Size16MB)
            {
                switch (namedSize)
                {
                    case NamedPageSize.Size4KB: m_Size = 0x1000; break;
                    case NamedPageSize.Size16KB: m_Size = 0x4000; break;
                    case NamedPageSize.Size64KB: m_Size = 0x1000 << 4; break;
                    case NamedPageSize.Size256KB: m_Size = 0x4000 << 4; break;
                    case NamedPageSize.Size1MB: m_Size = 0x1000 << 8; break;
                    case NamedPageSize.Size4MB: m_Size = 0x4000 << 8; break;
                    case NamedPageSize.Size16MB: m_Size = 0x1000 << 12; break;
                    default: m_Size = 0; break;
                }
            }
            else
            {
                m_Size = 0;
            }
        }

        public Int32 Size
        {
            get { return m_Size; }
        }

        public NamedPageSize NamedSize
        {
            get
            {
                switch (m_Size)
                {
                    case 0x1000: return NamedPageSize.Size4KB;
                    case 0x4000: return NamedPageSize.Size16KB;
                    case 0x10000: return NamedPageSize.Size64KB;
                    case 0x40000: return NamedPageSize.Size256KB;
                    case 0x100000: return NamedPageSize.Size1MB;
                    case 0x400000: return NamedPageSize.Size4MB;
                    case 0x1000000: return NamedPageSize.Size16MB;
                    default: return NamedPageSize.SizeUnknown;
                }
            }
        }

        public UInt32 AddressOffsetMask
        {
            get
            {
                return (UInt32)(m_Size - 1);
            }
        }

        public UInt32 AddressBaseMask
        {
            get
            {
                return ~AddressOffsetMask;
            }
        }

        public UInt64 PageMask
        {
            get
            {
                return (((UInt64)m_Size - 1) & 0x1FFE000) << 1;
            }
        }

        public override string ToString()
        {
            return NamedSize.ToString();
        }
    }
}