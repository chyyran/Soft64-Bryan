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
        public static Int64 BranchComputeTargetAddress(UInt64 pc, UInt16 immediate)
        {
            Int64 delayPC = (Int64)pc + 4;
            Int64 offset = (Int64)(immediate.SignExtended64() << 2);
            Int64 newPC = delayPC + offset;

            return newPC;
        }

        [OpcodeHook("BNE")]
        private void Inst_Bne(MipsInstruction inst)
        {
            Boolean condition = false;
            m_IsBranch = true;
            m_BranchDelaySlot = MipsState.PC + 4;

            if (MipsState.Is32BitMode())
                condition = MipsState.GPRRegs64.GPRRegs32[inst.Rs] != MipsState.GPRRegs64.GPRRegs32[inst.Rt];
            else
                condition = MipsState.GPRRegs64[inst.Rs] != MipsState.GPRRegs64[inst.Rt];

            m_BranchTarget = condition ? BranchComputeTargetAddress(inst.PC, inst.Immediate).ResolveAddress() : MipsState.PC + 8;
        }

        [OpcodeHook("J")]
        private void Inst_J(MipsInstruction inst)
        {
            UInt64 target = (inst.Offset << 2) | ((inst.PC + 4) & 0xFFFF0000);
            m_IsBranch = true;
            m_BranchDelaySlot = MipsState.PC + 4;
            m_BranchTarget = ((Int64)target).ResolveAddress();
        }

        [OpcodeHook("JAL")]
        private void Inst_Jal(MipsInstruction inst)
        {
            UInt64 target = ((inst.PC + 4) & 0xFFFF0000) | (inst.Offset << 2);
            LinkAddress(inst.PC + 8);
            m_IsBranch = true;
            m_BranchDelaySlot = MipsState.PC + 4;
            m_BranchTarget = ((Int64)target).ResolveAddress();
        }

        [OpcodeHook("BEQL")]
        private void Inst_Beql(MipsInstruction inst)
        {
            Boolean condition;
            Int64 target = 0;
            m_IsBranch = true;
            m_BranchDelaySlot = MipsState.PC + 4;

            if (MipsState.Is32BitMode())
            {
                condition = MipsState.GPRRegs64.GPRRegs32[inst.Rs] == MipsState.GPRRegs64.GPRRegs32[inst.Rt];
                target = (Int32)((Int32)MipsState.PC + 4) + (Int32)inst.Immediate.SignExtended32();
            }
            else
            {
                condition = MipsState.GPRRegs64[inst.Rs] == MipsState.GPRRegs64[inst.Rt];
                target = ((Int64)MipsState.PC + 4) + (Int64)(inst.Immediate.SignExtended64() << 2);
            }

            m_BranchTarget = condition ? target.ResolveAddress() : MipsState.PC + 8;
        }

        [OpcodeHook("JR")]
        private void Inst_Jr(MipsInstruction inst)
        {
            Int64 target = MipsState.GPRRegs64.GPRRegs64S[inst.Rs];
            m_IsBranch = true;
            m_BranchDelaySlot = MipsState.PC + 4;
            m_BranchTarget = target.ResolveAddress();
        }

        [OpcodeHook("BNEL")]
        private void Inst_Bnel(MipsInstruction inst)
        {
            Boolean condition = false;
            m_IsBranch = true;
            m_BranchDelaySlot = MipsState.PC + 4;

            if (MipsState.Is32BitMode())
                condition = MipsState.GPRRegs64.GPRRegs32[inst.Rs] != MipsState.GPRRegs64.GPRRegs32[inst.Rt];
            else
                condition = MipsState.GPRRegs64[inst.Rs] != MipsState.GPRRegs64[inst.Rt];

            m_NullifiedInstruction = !condition;

            m_BranchTarget = condition ? BranchComputeTargetAddress(inst.PC, inst.Immediate).ResolveAddress() : MipsState.PC + 8;
        }
    }
}