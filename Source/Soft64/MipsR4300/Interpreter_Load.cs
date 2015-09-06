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
            Int64 address = ComputeAddress64(inst);

            if ((address & 3) != 0)
            {
                CauseException = ExceptionCode.AddressErrorLoad;
            }
            else
            {
                if (MipsState.Is32BitMode())
                    MipsState.WriteGPR32Unsigned(inst.Rt, DataManipulator.ReadWordUnsigned(address));
                else
                    MipsState.WriteGPRSigned(inst.Rt, DataManipulator.ReadWordSigned(address));
            }
        }

        [OpcodeHook("LWU")]
        private void Inst_Lwu(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                CauseException = ExceptionCode.ReservedInstruction;
                return;
            }

            Int64 address = ComputeAddress64(inst);

            if ((address & 3) != 0)
            {
                CauseException = ExceptionCode.AddressErrorLoad;
            }
            else
            {
                MipsState.WriteGPRUnsigned(inst.Rt, DataManipulator.ReadWordUnsigned(address));
            }
        }

        [OpcodeHook("LD")]
        private void Inst_Ld(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                CauseException = ExceptionCode.ReservedInstruction;
            }
            else
            {
                Int64 address = ComputeAddress64(inst);

                if ((address & 3) != 0)
                {
                    CauseException = ExceptionCode.AddressErrorLoad;
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
                    case TLBExceptionType.Invalid: CauseException = ExceptionCode.Invalid; break;
                    case TLBExceptionType.Mod: CauseException = ExceptionCode.TlbMod; break;
                    case TLBExceptionType.Refill: CauseException = ExceptionCode.TlbStore; break;
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
                    case TLBExceptionType.Invalid: CauseException = ExceptionCode.Invalid; break;
                    case TLBExceptionType.Mod: CauseException = ExceptionCode.TlbMod; break;
                    case TLBExceptionType.Refill: CauseException = ExceptionCode.TlbStore; break;
                    default: break;
                }
            }
        }

        [OpcodeHook("LDC1")]
        private void Inst_Ldc1(MipsInstruction inst)
        {
            Int64 address = ComputeAddress64(inst);

            if ((address & 3) != 0)
                CauseException = ExceptionCode.AddressErrorLoad;

            if ((inst.Rt & 1) != 0)
                return;

            MipsState.CP0Regs[inst.Rt]  = DataManipulator.ReadDoublewordUnsigned(address);
        }

        [OpcodeHook("LDL")]
        private void Inst_Ldl(MipsInstruction inst)
        {
            if (MipsState.Is64BitMode())
            {
                Int64 address = ComputeAddress64(inst);
                UInt64 value = DataManipulator.ReadDoublewordUnsigned(address);
                UInt64 reg = MipsState.ReadGPRUnsigned(inst.Rt);
                Int32 shiftAmount = (Int32)(address % 8);
                value <<= shiftAmount;
                reg <<= (8 - shiftAmount);
                reg >>= shiftAmount;
                value |= reg;
                MipsState.WriteGPRUnsigned(inst.Rt, value);
            }
            else
            {
                CauseException = ExceptionCode.ReservedInstruction;
            }
        }

        [OpcodeHook("LDR")]
        private void Inst_Ldr(MipsInstruction inst)
        {
            if (MipsState.Is64BitMode())
            {
                Int64 address = ComputeAddress64(inst);
                UInt64 value = DataManipulator.ReadDoublewordUnsigned(address);
                UInt64 reg = MipsState.ReadGPRUnsigned(inst.Rt);
                Int32 shiftAmount = (Int32)(address % 8);
                value >>= shiftAmount;
                reg >>= (8 - shiftAmount);
                reg <<= shiftAmount;
                value |= reg;
                MipsState.WriteGPRUnsigned(inst.Rt, value);
            }
            else
            {
                CauseException = ExceptionCode.ReservedInstruction;
            }
        }

        [OpcodeHook("LH")]
        private void Inst_Lh(MipsInstruction inst)
        {
            try
            {
                Int64 address = ComputeAddress64(inst);

                if ((address & 3) != 0)
                {
                    CauseException = ExceptionCode.AddressErrorLoad;
                    return;
                }

                if (MipsState.Is32BitMode())
                {
                    MipsState.WriteGPR32Signed(inst.Rt, DataManipulator.ReadHalfwordSigned(address));
                }
                else
                {
                    MipsState.WriteGPRSigned(inst.Rt, DataManipulator.ReadHalfwordSigned(address));
                }
            }
            catch (TLBException tlbe)
            {
                switch (tlbe.ExceptionType)
                {
                    case TLBExceptionType.Invalid: CauseException = ExceptionCode.Invalid; break;
                    case TLBExceptionType.Mod: CauseException = ExceptionCode.TlbMod; break;
                    case TLBExceptionType.Refill: CauseException = ExceptionCode.TlbStore; break;
                    default: break;
                }
            }
        }

        [OpcodeHook("LHU")]
        private void Inst_Lhu(MipsInstruction inst)
        {
            try
            {
                Int64 address = ComputeAddress64(inst);

                if ((address & 1) != 0)
                {
                    CauseException = ExceptionCode.AddressErrorLoad;
                    return;
                }

                if (MipsState.Is32BitMode())
                {
                    MipsState.WriteGPR32Unsigned(inst.Rt, DataManipulator.ReadHalfwordUnsigned(address));
                }
                else
                {
                    MipsState.WriteGPRUnsigned(inst.Rt, DataManipulator.ReadHalfwordUnsigned(address));
                }
            }
            catch (TLBException tlbe)
            {
                switch (tlbe.ExceptionType)
                {
                    case TLBExceptionType.Invalid: CauseException = ExceptionCode.Invalid; break;
                    case TLBExceptionType.Mod: CauseException = ExceptionCode.TlbMod; break;
                    case TLBExceptionType.Refill: CauseException = ExceptionCode.TlbStore; break;
                    default: break;
                }
            }
        }

        [OpcodeHook("LL")]
        private void Inst_Ll(MipsInstruction inst)
        {
            /* This is used in MIPS atomic memory operations, however
             * this will not function like the real processor since the emulator
             * does used a cache system */

            /* Do a normal load word operation, assuming no cache is in place */

            if (logger.IsDebugEnabled)
                logger.Debug("Load Linked: " + inst.Address.ToString("X8"));

            Inst_Lw(inst);
        }

        [OpcodeHook("LLD")]
        private void Inst_Lld(MipsInstruction inst)
        {
            /* This is used in MIPS atomic memory operations, however
             * this will not function like the real processor since the emulator
             * does used a cache system */

            /* Do a normal load doubleword operation, assuming no cache is in place */

            if (logger.IsDebugEnabled)
                logger.Debug("Load Linked Doubleword: " + inst.Address.ToString("X8"));

            Inst_Ld(inst);
        }

        [OpcodeHook("LWC1")]
        private void Inst_Lcd1(MipsInstruction inst)
        {
            try
            {
                Int64 address = ComputeAddress64(inst);

                if ((address & 3) != 0)
                {
                    CauseException = ExceptionCode.AddressErrorLoad;
                    return;
                }

                MipsState.CP0Regs[inst.Rt] = DataManipulator.ReadWordUnsigned(address);

            }
            catch (TLBException tlbe)
            {
                switch (tlbe.ExceptionType)
                {
                    case TLBExceptionType.Invalid: CauseException = ExceptionCode.Invalid; break;
                    case TLBExceptionType.Mod: CauseException = ExceptionCode.TlbMod; break;
                    case TLBExceptionType.Refill: CauseException = ExceptionCode.TlbStore; break;
                    default: break;
                }
            }
        }

        [OpcodeHook("LWL")]
        private void Inst_Lwl(MipsInstruction inst)
        {
            Int64 address = ComputeAddress64(inst);
            UInt32 value = DataManipulator.ReadWordUnsigned(address);
            UInt32 reg = MipsState.ReadGPR32Unsigned(inst.Rt);
            Int32 shiftAmount = (Int32)(address % 4);
            value <<= shiftAmount;
            reg <<= (4 - shiftAmount);
            reg >>= shiftAmount;
            value |= reg;
            MipsState.WriteGPR32Unsigned(inst.Rt, value);

            if (MipsState.Is64BitMode())
            {
                MipsState.WriteGPR32Unsigned(inst.Rt, value);
            }
            else
            {
                MipsState.WriteGPRSigned(inst.Rt, (Int32)value);
            }
        }

        [OpcodeHook("LWR")]
        private void Inst_Lwr(MipsInstruction inst)
        {
            Int64 address = ComputeAddress64(inst);
            UInt32 value = DataManipulator.ReadWordUnsigned(address);
            UInt32 reg = MipsState.ReadGPR32Unsigned(inst.Rt);
            Int32 shiftAmount = (Int32)(address % 4);
            value >>= shiftAmount;
            reg >>= (4 - shiftAmount);
            reg <<= shiftAmount;
            value |= reg;
            MipsState.WriteGPR32Unsigned(inst.Rt, value);

            if (MipsState.Is64BitMode())
            {
                MipsState.WriteGPR32Unsigned(inst.Rt, value);
            }
            else
            {
                MipsState.WriteGPRSigned(inst.Rt, (Int32)value);
            }
        }
    }
}