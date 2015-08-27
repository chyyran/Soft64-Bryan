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
        public static Int64 BranchComputeTargetAddress(Int64 pc, UInt16 immediate)
        {
            return (pc + 4) + (((Int64)(Int16)immediate) << 2);
        }

        private void DoBranch(Boolean condition, MipsInstruction inst)
        {
            m_IsBranch = true;
            m_BranchDelaySlot = MipsState.PC + 4;
            m_BranchTarget = condition ? BranchComputeTargetAddress(inst.Address, inst.Immediate).ResolveAddress() : MipsState.PC + 8;
        }

        private void DoBranchLikely(Boolean condition, MipsInstruction inst)
        {
            m_NullifiedInstruction = !condition;
            DoBranch(condition, inst);
        }

        private void DoJump(Int64 addressTarget)
        {
            m_IsBranch = true;
            m_BranchDelaySlot = MipsState.PC + 4;
            m_BranchTarget = addressTarget.ResolveAddress();
        }

        [OpcodeHook("BNE")]
        private void Inst_Bne(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                DoBranch(MipsState.ReadGPR32Unsigned(inst.Rs) != MipsState.ReadGPR32Unsigned(inst.Rt), inst);
            }
            else
            {
                DoBranch(MipsState.ReadGPRUnsigned(inst.Rs) != MipsState.ReadGPRUnsigned(inst.Rt), inst);
            }
        }

        [OpcodeHook("J")]
        private void Inst_J(MipsInstruction inst)
        {
            DoJump((inst.Offset << 2) | ((inst.Address + 4) & 0xFFFF0000));
        }

        [OpcodeHook("JAL")]
        private void Inst_Jal(MipsInstruction inst)
        {
            LinkAddress(inst.Address + 8);
            DoJump(((inst.Address + 4) & 0xFFFF0000) | (inst.Offset << 2));
        }

        [OpcodeHook("BEQL")]
        private void Inst_Beql(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                DoBranchLikely(MipsState.ReadGPR32Unsigned(inst.Rs) == MipsState.ReadGPR32Unsigned(inst.Rt), inst);
            }
            else
            {
                DoBranchLikely(MipsState.ReadGPRUnsigned(inst.Rs) == MipsState.ReadGPRUnsigned(inst.Rt), inst);
            }
        }

        [OpcodeHook("BEQ")]
        private void Inst_Beq(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                DoBranch(MipsState.ReadGPR32Unsigned(inst.Rs) == MipsState.ReadGPR32Unsigned(inst.Rt), inst);
            }
            else
            {
                DoBranch(MipsState.ReadGPRUnsigned(inst.Rs) == MipsState.ReadGPRUnsigned(inst.Rt), inst);
            }
        }

        [OpcodeHook("JR")]
        private void Inst_Jr(MipsInstruction inst)
        {
            DoJump(MipsState.ReadGPRSigned(inst.Rs));
        }

        [OpcodeHook("BNEL")]
        private void Inst_Bnel(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                DoBranchLikely(MipsState.ReadGPR32Unsigned(inst.Rs) != MipsState.ReadGPR32Unsigned(inst.Rt), inst);
            }
            else
            {
                DoBranchLikely(MipsState.ReadGPRUnsigned(inst.Rs) != MipsState.ReadGPRUnsigned(inst.Rt), inst);
            }
        }

        [OpcodeHook("BLEZL")]
        private void Inst_Blezl(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                DoBranchLikely(MipsState.ReadGPR32Unsigned(inst.Rs) <= MipsState.ReadGPR32Unsigned(inst.Rt), inst);
            }
            else
            {
                DoBranchLikely(MipsState.ReadGPRUnsigned(inst.Rs) <= MipsState.ReadGPRUnsigned(inst.Rt), inst);
            }
        }
    }
}