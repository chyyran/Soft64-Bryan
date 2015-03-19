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
using Soft64.MipsR4300;
using Soft64.MipsR4300.CP0;

namespace Soft64.HLE_OS
{
    internal sealed class KernelInterop
    {
        private MipsR4300Core m_CurrentCore;

        public KernelInterop(MipsR4300Core core)
        {
            m_CurrentCore = core;
        }

        public void PrepareTLBAccess(long virtualAddress)
        {
            /* We must simulate the OS preparing the TLB cache */
            TLBCache tlb = m_CurrentCore.VirtualMemoryStream.TLB;

            /* Find the entry with a VPN matching the virtualAddress */
            Int32 tlbIndex = tlb.GetIndex(new VirtualPageNumber2(0, (UInt64)virtualAddress & new PageSize(tlb.PageMask).AddressBaseMask));

            if (tlbIndex != -1)
            {
                tlb.Index = (UInt64)tlbIndex;
                tlb.Read();
            }
        }
    }
}