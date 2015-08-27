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
        [OpcodeHook("LUI")]
        private void Inst_Lui(MipsInstruction inst)
        {
            UInt32 word = ((UInt32)inst.Immediate) << 16;

            if (MipsState.Is32BitMode())
            {
                MipsState.WriteGPR32Unsigned(inst.Rt, word);
            }
            else
            {
                MipsState.WriteGPRUnsigned(inst.Rt, (UInt64)(Int64)(Int32)word);
            }
        }

        [OpcodeHook("LW")]
        private void Inst_Lw(MipsInstruction inst)
        {
            Int64 address = (MipsState.ReadGPRSigned(inst.Rs) + (Int64)inst.Immediate).ResolveAddress();

            if ((address & 3) != 0)
            {
                MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.AddressErrorLoad;
            }
            else
            {

                DataBinaryReader.BaseStream.Position = address;
                UInt32 read = DataBinaryReader.ReadUInt32();

                if (MipsState.Is32BitMode())
                    MipsState.WriteGPR32Unsigned(inst.Rt, read);
                else
                    MipsState.WriteGPRUnsigned(inst.Rt, (UInt64)(Int64)(Int32)read);
            }
        }

        [OpcodeHook("LD")]
        private void Inst_Ld(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.ReservedInstruction;
                return;
            }

            Int64 address = (MipsState.ReadGPRSigned(inst.Rs) + (Int64)inst.Immediate).ResolveAddress();

            if ((address & 3) != 0)
            {
                MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.AddressErrorLoad;
            }
            else
            {

                DataBinaryReader.BaseStream.Position = address;
                MipsState.WriteGPRUnsigned(inst.Rt, DataBinaryReader.ReadUInt64());
            }
        }
    }
}