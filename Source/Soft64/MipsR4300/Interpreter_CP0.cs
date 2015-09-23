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
    public partial class Interpreter
    {
        [OpcodeHook("MTC0")]
        private void Inst_Mtc0(MipsInstruction inst)
        {
            if (MipsState.CP0Regs.StatusReg.CopUsable0)
            {
                MipsState.CP0Regs[inst.Rd] = MipsState.ReadGPRUnsigned(inst.Rt);
            }
            else
            {
                CauseException = ExceptionCode.CopUnstable;
            }
        }

        [OpcodeHook("DMFC0")]
        private void Inst_Dmfc0(MipsInstruction inst)
        {
            if (MipsState.Operating64BitMode)
            {
                MipsState.WriteGPRUnsigned(inst.Rt, MipsState.CP0Regs[inst.Rd]);
            }
            else
            {
                CauseException = ExceptionCode.ReservedInstruction;
            }
        }

        [OpcodeHook("DMTC0")]
        private void Inst_Dmtc0(MipsInstruction inst)
        {
            if (MipsState.Operating64BitMode)
            {
                MipsState.CP0Regs[inst.Rd] = MipsState.ReadGPRUnsigned(inst.Rt);
            }
            else
            {
                CauseException = ExceptionCode.ReservedInstruction;
            }
        }

        [OpcodeHook("MFC0")]
        private void Inst_Mfc0(MipsInstruction inst)
        {
            if (MipsState.CP0Regs.StatusReg.CopUsable0)
            {
                if (!MipsState.Operating64BitMode)
                {
                    MipsState.WriteGPR32Unsigned(inst.Rt, (UInt32)MipsState.CP0Regs[inst.Rd]);
                }
                else
                {
                    MipsState.WriteGPRUnsigned(inst.Rt, MipsState.CP0Regs[inst.Rd]);
                }
            }
            else
            {
                CauseException = ExceptionCode.CopUnstable;
            }
        }
    }
}