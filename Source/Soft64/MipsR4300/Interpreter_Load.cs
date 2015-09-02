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
                MipsState.CP0Regs.CauseReg.ExceptionType = ExceptionCode.AddressErrorLoad;
            }
            else
            {
                UInt32 read = DataManipulator.ReadWordUnsigned(address);

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
                MipsState.CP0Regs.CauseReg.ExceptionType = ExceptionCode.ReservedInstruction;
            }
            else
            {
                Int64 address = ComputeAddress64(inst);

                if ((address & 3) != 0)
                {
                    MipsState.CP0Regs.CauseReg.ExceptionType = ExceptionCode.AddressErrorLoad;
                }
                else
                {
                    MipsState.WriteGPRUnsigned(inst.Rt, DataManipulator.ReadDoublewordUnsigned(address));
                }
            }
        }

        [OpcodeHook("LB")]
        private void Inst_Lb(MipsInstruction inst)
        {
            try
            {
                if (MipsState.Is32BitMode())
                {
                    MipsState.WriteGPR32Signed(inst.Rt, DataManipulator.ReadByteSigned(ComputeAddress32(inst)));
                }
                else
                {
                    MipsState.WriteGPRSigned(inst.Rt, DataManipulator.ReadByteSigned(ComputeAddress64(inst)));
                }
            }
            catch (TLBException tlbe)
            {
                switch (tlbe.ExceptionType)
                {
                    case TLBExceptionType.Invalid: MipsState.CP0Regs.CauseReg.ExceptionType = ExceptionCode.Invalid; break;
                    case TLBExceptionType.Mod: MipsState.CP0Regs.CauseReg.ExceptionType = ExceptionCode.TlbMod; break;
                    case TLBExceptionType.Refill: MipsState.CP0Regs.CauseReg.ExceptionType = ExceptionCode.TlbStore; break;
                    default: break;
                }
            }
        }

        [OpcodeHook("LBU")]
        private void Inst_Lbu(MipsInstruction inst)
        {
            try
            {
                if (MipsState.Is32BitMode())
                {
                    MipsState.WriteGPR32Signed(inst.Rt, DataManipulator.ReadByteUnsigned(ComputeAddress32(inst)));
                }
                else
                {
                    MipsState.WriteGPRSigned(inst.Rt, DataManipulator.ReadByteUnsigned(ComputeAddress64(inst)));
                }
            }
            catch (TLBException tlbe)
            {
                switch (tlbe.ExceptionType)
                {
                    case TLBExceptionType.Invalid: MipsState.CP0Regs.CauseReg.ExceptionType = ExceptionCode.Invalid; break;
                    case TLBExceptionType.Mod: MipsState.CP0Regs.CauseReg.ExceptionType = ExceptionCode.TlbMod; break;
                    case TLBExceptionType.Refill: MipsState.CP0Regs.CauseReg.ExceptionType = ExceptionCode.TlbStore; break;
                    default: break;
                }
            }
        }
    }
}