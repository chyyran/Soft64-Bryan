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

namespace Soft64.MipsR4300.Interpreter
{
    public partial class PureInterpreter
    {
        [OpcodeHook("TLBR")]
        private void Inst_tlbr(MipsInstruction inst)
        {
            /* Read Indexed TLB Entry:  TLB Entry[index] => TLB Registers */
            ParentMips.Tlb.Read();
        }

        [OpcodeHook("TLBWI")]
        private void Inst_tlbwi(MipsInstruction inst)
        {
            /* Write Indexed TLB Entry: TLB Registers => TLB Entry[index] */
            ParentMips.Tlb.Write();
        }

        [OpcodeHook("TLBWR")]
        private void Inst_tlbwr(MipsInstruction inst)
        {
            /* Write Random TLB Entry: TLB Registers => TLB Entry[random_index] */
            ParentMips.Tlb.WriteRandom();
        }

        [OpcodeHook("TLBP")]
        private void Inst_tlbp(MipsInstruction inst)
        {
            /* Probe the TLB */
            ParentMips.Tlb.Probe();
        }

        [OpcodeHook("ERET")]
        private void Inst_Eret(MipsInstruction inst)
        {
            if (MipsState.CP0Regs.StatusReg.CopUsable0)
                MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.CopUnstable;

            if (MipsState.CP0Regs.StatusReg.ExceptionLevel)
            {
                MipsState.PC = (Int64)MipsState.CP0Regs.ErrorEPC;
                MipsState.CP0Regs.StatusReg.ExceptionLevel = false;
            }
            else
            {
                MipsState.PC = (Int64)MipsState.CP0Regs.EPC;
                MipsState.CP0Regs.StatusReg.ExceptionLevel = false;
            }

            MipsState.LLBit = false;
        }
    }
}