﻿/*
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
        [OpcodeHook("SW")]
        private void Inst_Sw(MipsInstruction inst)
        {
            Int64 address = (MipsState.ReadGPRSigned(inst.Rs) + (Int64)inst.Immediate).ResolveAddress();

            if ((address & 3) != 0)
            {
                MipsState.CP0Regs.CauseReg.ExceptionType = ExceptionCode.AddressErrorStore;
            }
            else
            {

                DataManipulator.Write(address, MipsState.ReadGPR32Unsigned(inst.Rt));
            }
        }

        [OpcodeHook("SD")]
        private void Inst_Sd(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
                throw new InvalidOperationException("Instruction reserved");

            Int64 address = (MipsState.ReadGPRSigned(inst.Rs) + (Int64)inst.Immediate).ResolveAddress();

            if ((address & 7) != 0)
            {
                MipsState.CP0Regs.CauseReg.ExceptionType = ExceptionCode.AddressErrorStore;
            }
            else
            {
                DataManipulator.Write(address, MipsState.ReadGPRUnsigned(inst.Rt));
            }
        }

        [OpcodeHook("SB")]
        private void Inst_Sb(MipsInstruction inst)
        {
            Int64 address = ComputeAddress64(inst);
            DataManipulator.Write(address, MipsState.ReadGPRUnsigned(inst.Rt) & 0xFF);
        }

        [OpcodeHook("SC")]
        private void Inst_Sc(MipsInstruction inst)
        {
            /* This is used in atomic operations, for now let it always pass */
            MipsState.WriteGPRUnsigned(inst.Rt, 1);
            Inst_Sw(inst);
        }

        [OpcodeHook("SCD")]
        private void Inst_Scd(MipsInstruction inst)
        {
            /* This is used in atomic operations, for now let it always pass */
            MipsState.WriteGPRUnsigned(inst.Rt, 1);
            Inst_Sd(inst);
        }
    }
}