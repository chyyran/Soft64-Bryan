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
        [OpcodeHook("SW")]
        private void Inst_Sw(MipsInstruction inst)
        {
            UInt64 baseValue = MipsState.GPRRegs64[inst.Rs];
            UInt32 imm = (UInt32)inst.Immediate.SignExtended32();
            UInt64 address = baseValue + imm;

            if ((address & 3) != 0)
            {
                throw new InvalidOperationException("Address error");
            }

            DataBinaryReader.BaseStream.Position = (Int64)(address);
            DataBinaryWriter.Write(MipsState.GPRRegs64.GPRRegs32[inst.Rt]);
        }

        [OpcodeHook("SD")]
        private void Inst_Sd(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
                throw new InvalidOperationException("Instruction reserved");

            UInt64 baseValue = MipsState.GPRRegs64[inst.Rs];
            UInt32 imm = (UInt32)inst.Immediate.SignExtended32();
            UInt64 address = baseValue + imm;

            if ((address & 7) != 0)
            {
                throw new InvalidOperationException("Address error");
            }

            DataBinaryReader.BaseStream.Position = (Int64)(address);
            DataBinaryWriter.Write(MipsState.GPRRegs64[inst.Rt]);
        }
    }
}