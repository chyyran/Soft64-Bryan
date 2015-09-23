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
        private static readonly UInt32[] LWLMask = { 0x00000000, 0x000000FF, 0x0000FFFF, 0x00FFFFFF };

        private static readonly Int32[] LWLShift = { 0, 8, 16, 24 };

        private static readonly UInt32[] LWRMask = { 0xFFFFFF00, 0xFFFF0000, 0xFF000000, 0x0000000 };

        private static readonly Int32[] LWRShift = { 24, 16, 8, 0 };

        private static readonly UInt64[] LDLMask = { 0, 0xFF, 0xFFFF, 0xFFFFFF, 0xFFFFFFFF, 0xFFFFFFFFFF, 0xFFFFFFFFFFFF, 0xFFFFFFFFFFFFFF };

        private static readonly Int32[] LDLShift = { 0, 8, 16, 24, 32, 40, 48, 56 };

        private static readonly UInt64[] LDRMask = { 0xFFFFFFFFFFFFFF00, 0xFFFFFFFFFFFF0000,
                      0xFFFFFFFFFF000000, 0xFFFFFFFF00000000,
                      0xFFFFFF0000000000, 0xFFFF000000000000,
                      0xFF00000000000000, 0 };

        private static readonly Int32[] LDRShift = { 56, 48, 40, 32, 24, 16, 8, 0 };


        [OpcodeHook("LUI")]
        private void Inst_Lui(MipsInstruction inst)
        {
            UInt32 word = ((UInt32)inst.Immediate) << 16;

            if (!MipsState.Operating64BitMode)
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
                if (!MipsState.Operating64BitMode)
                    MipsState.WriteGPR32Unsigned(inst.Rt, DataManipulator.LoadWordUnsigned(address));
                else
                    MipsState.WriteGPRSigned(inst.Rt, DataManipulator.LoadWordSigned(address));
            }
        }

        [OpcodeHook("LWU")]
        private void Inst_Lwu(MipsInstruction inst)
        {
            if (!MipsState.Operating64BitMode)
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
                MipsState.WriteGPRUnsigned(inst.Rt, DataManipulator.LoadWordUnsigned(address));
            }
        }

        [OpcodeHook("LD")]
        private void Inst_Ld(MipsInstruction inst)
        {
            if (!MipsState.Operating64BitMode)
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
                    MipsState.WriteGPRUnsigned(inst.Rt, DataManipulator.LoadDoublewordUnsigned(address));
                }
            }
        }

        [OpcodeHook("LB")]
        private void Inst_Lb(MipsInstruction inst)
        {
            try
            {
                if (!MipsState.Operating64BitMode)
                {
                    MipsState.WriteGPR32Signed(inst.Rt, DataManipulator.LoadByteSigned(ComputeAddress32(inst)));
                }
                else
                {
                    MipsState.WriteGPRSigned(inst.Rt, DataManipulator.LoadByteSigned(ComputeAddress64(inst)));
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
                if (!MipsState.Operating64BitMode)
                {
                    MipsState.WriteGPR32Signed(inst.Rt, DataManipulator.LoadByteUnsigned(ComputeAddress32(inst)));
                }
                else
                {
                    MipsState.WriteGPRSigned(inst.Rt, DataManipulator.LoadByteUnsigned(ComputeAddress64(inst)));
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

        [OpcodeHook("LDL")]
        private void Inst_Ldl(MipsInstruction inst)
        {
            if (MipsState.Operating64BitMode)
            {
                /* Thanks to PJ64 Implementation */
                Int64 address = ComputeAddress64(inst);
                Int32 offset = (Int32)(address & 7);
                UInt64 value = DataManipulator.LoadDoublewordUnsigned(address & ~7);
                MipsState.WriteGPRUnsigned(inst.Rt, MipsState.ReadGPRUnsigned(inst.Rt) & LDLMask[offset]);
                MipsState.WriteGPRUnsigned(inst.Rt, MipsState.ReadGPRUnsigned(inst.Rt) + (value << LDLShift[offset]));
            }
            else
            {
                CauseException = ExceptionCode.ReservedInstruction;
            }
        }

        [OpcodeHook("LDR")]
        private void Inst_Ldr(MipsInstruction inst)
        {
            if (MipsState.Operating64BitMode)
            {
                /* Thanks to PJ64 Implementation */
                Int64 address = ComputeAddress64(inst);
                Int32 offset = (Int32)(address & 7);
                UInt64 value = DataManipulator.LoadDoublewordUnsigned(address & ~7);
                MipsState.WriteGPRUnsigned(inst.Rt, MipsState.ReadGPRUnsigned(inst.Rt) & LDRMask[offset]);
                MipsState.WriteGPRUnsigned(inst.Rt, MipsState.ReadGPRUnsigned(inst.Rt) + (value >> LDRShift[offset]));
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

                if (!MipsState.Operating64BitMode)
                {
                    MipsState.WriteGPR32Signed(inst.Rt, DataManipulator.LoadHalfwordSigned(address));
                }
                else
                {
                    MipsState.WriteGPRSigned(inst.Rt, DataManipulator.LoadHalfwordSigned(address));
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

                if (!MipsState.Operating64BitMode)
                {
                    MipsState.WriteGPR32Unsigned(inst.Rt, DataManipulator.LoadHalfwordUnsigned(address));
                }
                else
                {
                    MipsState.WriteGPRUnsigned(inst.Rt, DataManipulator.LoadHalfwordUnsigned(address));
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

        [OpcodeHook("LWL")]
        private void Inst_Lwl(MipsInstruction inst)
        {
            /* Thanks to PJ64 implementation */
            Int64 address = ComputeAddress64(inst);
            Int32 offset = (Int32)(address & 3);
            UInt32 value = DataManipulator.LoadWordUnsigned(address & ~3);
            value &= SWLMask[offset];
            value += MipsState.ReadGPR32Unsigned(inst.Rt) >> SWLShift[offset];
            DataManipulator.Store(address & ~3, value);
        }

        [OpcodeHook("LWR")]
        private void Inst_Lwr(MipsInstruction inst)
        {
            /* Thanks to PJ64 implementation */
            Int64 address = ComputeAddress64(inst);
            Int32 offset = (Int32)(address & 3);
            UInt32 value = DataManipulator.LoadWordUnsigned(address & ~3);
            value &= SWRMask[offset];
            value += MipsState.ReadGPR32Unsigned(inst.Rt) << SWRShift[offset];
            DataManipulator.Store(address & ~3, value);
        }
    }
}