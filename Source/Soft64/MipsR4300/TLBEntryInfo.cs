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

namespace Soft64.MipsR4300
{
    public sealed class TLBEntryInfo
    {
        private Int32 m_TLBIndex;
        private TLBEntry m_Entry;

        internal TLBEntryInfo(Int32 index, TLBEntry entry)
        {
            m_TLBIndex = index;
            m_Entry = entry;
        }

        public TLBEntry AssociatedEntry
        {
            get { return m_Entry; }
        }

        public Int32 SelectedIndex
        {
            get { return m_TLBIndex; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Entry ");
            sb.Append(m_TLBIndex);
            sb.Append(" : 0x");
            sb.Append(m_Entry.MappedVirtualAddress.ToString("X16"));
            return sb.ToString();
        }
    }
}