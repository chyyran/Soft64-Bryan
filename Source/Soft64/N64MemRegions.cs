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

namespace Soft64
{
    public enum N64MemRegions : int
    {
        Rdram = 0,
        RdramRegs,
        SPDMem,
        SPIMem,
        SPRegs,
        DPCmdRegs,
        DPSpanRegs,
        MIRegs,
        VIRegs,
        AIRegs,
        PIRegs,
        RIRegs,
        SIRegs,
        Unused,
        CD2A1,
        CD1A1,
        CD2A2,
        CD1A2,
        PIFMem,
        CD1A3,
        SysAD,
        TLBMapped
    }

    public static class N64MemRegionsExtensions
    {
        public static Int64 ToRegionAddress(this N64MemRegions region)
        {
            if (region < N64MemRegions.Rdram || region > N64MemRegions.SysAD)
                throw new ArgumentOutOfRangeException("region");

            switch (region)
            {
                case N64MemRegions.Rdram: return 0x00000000;
                case N64MemRegions.RdramRegs: return 0x03F00000;
                case N64MemRegions.SPDMem: return 0x04000000;
                case N64MemRegions.SPIMem: return 0x04001000;
                case N64MemRegions.SPRegs: return 0x04040000;
                case N64MemRegions.DPCmdRegs: return 0x04100000;
                case N64MemRegions.DPSpanRegs: return 0x04200000;
                case N64MemRegions.MIRegs: return 0x04300000;
                case N64MemRegions.VIRegs: return 0x04400000;
                case N64MemRegions.AIRegs: return 0x04500000;
                case N64MemRegions.PIRegs: return 0x04600000;
                case N64MemRegions.RIRegs: return 0x04700000;
                case N64MemRegions.SIRegs: return 0x04800000;
                case N64MemRegions.Unused: return 0x04900000;
                case N64MemRegions.CD2A1: return 0x05000000;
                case N64MemRegions.CD1A1: return 0x06000000;
                case N64MemRegions.CD2A2: return 0x08000000;
                case N64MemRegions.CD1A2: return 0x10000000;
                case N64MemRegions.PIFMem: return 0x1FC00000;
                case N64MemRegions.CD1A3: return 0x1FD00000;
                case N64MemRegions.SysAD: return 0x80000000;
                default: return -1;
            }
        }
    }
}