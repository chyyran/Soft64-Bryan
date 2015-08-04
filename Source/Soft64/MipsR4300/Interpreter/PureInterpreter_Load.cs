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
        /* TODO: check over GPR selected regs */
        /* TODO: Using UInt32 hack to cast sign extended 64bit integers into 32bits so addressing works */

        [OpcodeHook("LUI")]
        private void Inst_Lui(MipsInstruction inst)
        {
            UInt32 word = ((UInt32)inst.Immediate << 16);

            if (MipsState.Is32BitMode())
                MipsState.GPRRegs64.GPRRegs32[inst.Rt] = word;
            else
                MipsState.GPRRegs64.GPRRegs64S[inst.Rt] = word.SignExtended64();
        }

        [OpcodeHook("LW")]
        private void Inst_Lw(MipsInstruction inst)
        {
            Int64 baseValue = MipsState.GPRRegs64.GPRRegs64S[inst.Rs];
            Int32 imm = inst.Immediate.SignExtended32();
            Int64 address = baseValue + imm;
            DataBinaryReader.BaseStream.Position = address.ResolveAddress();
            UInt32 read = DataBinaryReader.ReadUInt32();

            if ((address & 3) != 0)
            {
                MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.AddressErrorLoad;
                return;
            }

            if (MipsState.Is32BitMode())
                MipsState.GPRRegs64.GPRRegs32[inst.Rt] = read;
            else
                MipsState.GPRRegs64.GPRRegs64S[inst.Rt] = read.SignExtended64();
        }

        [OpcodeHook("LD")]
        private void Inst_Ld(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.ReservedInstruction;
                return;
            }

            Int64 baseValue = MipsState.GPRRegs64.GPRRegs64S[inst.Rs];
            Int32 imm = inst.Immediate.SignExtended32();
            Int64 address = baseValue + imm;

            if ((address & 7) != 0)
            {
                MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.AddressErrorLoad;
                return;
            }

            DataBinaryReader.BaseStream.Position = address.ResolveAddress();
            MipsState.GPRRegs64[inst.Rt] = DataBinaryReader.ReadUInt64();
        }
    }
}