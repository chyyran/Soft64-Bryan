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
using Soft64.MipsR4300.Interpreter;

namespace Soft64.MipsR4300.Debugging
{
    public static partial class Disassembler
    {
        /* This is here to to make sure the compiler compiles in this table initialization before anything else */

        #region Format LUT

        private static OperandFunc[] s_OperandFormatLUT =
        {
            /* 0: Instructions using Sa operand */
            (inst, o32) => { return String.Format("{0}, {1}, 0x{2:x2}", DecodeGprReg(inst.Rt, o32), DecodeGprReg(inst.Rd, o32), inst.ShiftAmount); },

            /* 1: Instructions that do not use the Rd operand */
            (inst, o32) => { return String.Format("{0}, {1}", DecodeGprReg(inst.Rs, o32),  DecodeGprReg(inst.Rt, o32)); },

            /* 2: Instructions using only the Rd operand */
            (inst, o32) => { return DecodeGprReg(inst.Rd, o32); },

            /* 3: Instructions using only the Rs operand */
            (inst, o32) => { return DecodeGprReg(inst.Rs, o32); },

            /* 4: Common Register format */
            (inst, o32) => { return String.Format("{0}, {1}, {2}", DecodeGprReg(inst.Rs, o32),  DecodeGprReg(inst.Rt, o32),  DecodeGprReg(inst.Rd, o32));},

            /* 5: Instructions that use Rt, Rd, Rs only */
            (inst, o32) => { return String.Format("{0}, {1}, {2}", DecodeGprReg(inst.Rt, o32),  DecodeGprReg(inst.Rd, o32),  DecodeGprReg(inst.ShiftAmount, o32));},

            /* 6: Instructions that use Rs and Immediate only */
            (inst, o32) => { return String.Format("{0}, 0x{1:x4}", DecodeGprReg(inst.Rs, o32), inst.Immediate); },

            /* 7: Instructions that use base, rt and offset */
            (inst, o32) => { return String.Format("{0}, 0x{2:x8}({1})", DecodeGprReg(inst.Rt, o32), DecodeGprReg(inst.Rs, o32), inst.Immediate); },

            /* 8: Sync instruction format */
            (inst, o32) => { return inst.ShiftAmount.ToString(); },

            /* 9: Immediate Instructions that only use rt operand */
            (inst, o32) => { return String.Format("{0}, 0x{1:x4}", DecodeGprReg(inst.Rt, o32), inst.Immediate); },

            /* 10: Immediate Instructions in RegImm that use only rs + offset */
            (inst, o32) => { return String.Format("{0}, 0x{1:x4}", DecodeGprReg(inst.Rs, o32), inst.Immediate); },

            /* 11: Common Jump Format */
            (inst, o32) => { return "0x" + inst.Offset.ToString("x8"); },

            /* 12: Instructions that only use Rs and Rd */
            (inst, o32) => { return String.Format("{0}, {1}", DecodeGprReg(inst.Rs, o32), DecodeGprReg(inst.Rd, o32)); },

            /* 13: For break and syscall */
            (inst, o32) => { return ((inst.Instruction & 0x3FFFFFF) >> 6).ToString("x8"); },

            /* 14: For register based trap instructions */
            (inst, o32) => { return String.Format("{0}, {1}, 0x{2:x4}", DecodeGprReg(inst.Rs, o32), DecodeGprReg(inst.Rt, o32), (inst.Instruction & 0xFFC0) >> 6); },

            /* 15: Cache format (base | op | offset) */
            (inst, o32) => { return String.Format("{0:x2}, {1:x2}, 0x{2:x4}", inst.Rs, inst.Rt, inst.Immediate); },

            /* 16: Instructions use a CO bit */
            (inst, o32) => { return Convert.ToBoolean((inst.Instruction & 0x3FFFFFF) >> 26).ToString(); },

            /* 17: Instructions using rt and fs */
            (inst, o32) => { return String.Format("{0}, {1}", DecodeGprReg(inst.Rt, o32), DecodeCop0Reg(inst.Rd)); },

            /* 18: BC1T type instructions */
            (inst, o32) => { return String.Format("ND:{0}, TF:{1}, 0x{2:x4}", Convert.ToBoolean((inst.Rt >> 1) & 1), Convert.ToBoolean(inst.Rt & 1), inst.Immediate); },

            /* 19: FPU Instructions using fs, fd */
            (inst, o32) => { return String.Format("{0}, {1}", DecodeFpuReg(inst.Rd), DecodeFpuReg(inst.ShiftAmount)); },

            /* 20: FPU Instructions using ft, fs, fd */
            (inst, o32) => { return String.Format("{0}, {1}, {2}", DecodeFpuReg(inst.Rt), DecodeFpuReg(inst.Rd), DecodeFpuReg(inst.ShiftAmount)); },

            /* 21: FPU Instructions using ft, fs */
            (inst, o32) => { return String.Format("{0}, {1}", DecodeFpuReg(inst.Rt), DecodeFpuReg(inst.ShiftAmount)); },

            /* 22: FPU Instructions using rt, and fs */
            (inst, o32) => { return String.Format("{0}, {1}", DecodeGprReg(inst.Rt, o32), DecodeFpuReg(inst.Rd)); },

            /* 23: Immediate Instructions using FPR reg */
            (inst, o32) => { return String.Format("{0:x2}, {1}, 0x{2:x4}", inst.Rs, DecodeFpuReg(inst.Rt), inst.Immediate); },

            /* 24: Common Immediate format */
            (inst, o32) => { return String.Format("{0}, {1}, 0x{2:x8}",  DecodeGprReg(inst.Rs, o32), DecodeGprReg(inst.Rt, o32), inst.Immediate); },

            /* 25: Conditional Branch formats */
            (inst, o32) => { return String.Format("{0}, {1}, 0x{2:x8} -->{3:X8}", DecodeGprReg(inst.Rs, o32), DecodeGprReg(inst.Rt, o32), inst.Immediate,
                                        PureInterpreter.BranchComputeTargetAddress(inst.Address, inst.Immediate)); },
        };

        #endregion Format LUT

        public static string DecodeOpName(MipsInstruction asmInstruction)
        {
            if (asmInstruction.Instruction == 0)
            {
                return "nop";
            }

            String mainOp = s_OpTableMain[asmInstruction.Opcode];
            String opName = "";

            if (mainOp != null)
            {
                switch (mainOp)
                {
                    case "_SPECIAL": opName = s_OpTableSpecial[asmInstruction.Function]; break;
                    case "_REGIMM": opName = s_OpTableRegImm[asmInstruction.Rt]; break;
                    case "_COP0":
                        {
                            switch (asmInstruction.Rs)
                            {
                                case 0x10: opName = s_OpTableTlb[asmInstruction.Function]; break;
                                default: opName = s_OpTableCop0[asmInstruction.Rs]; break;
                            }
                            break;
                        }
                    case "_COP1":
                        {
                            switch (s_OpTableCop1[asmInstruction.Rs])
                            {
                                case "_BC1": opName = s_OpTableBC1[asmInstruction.Rt]; break;
                                case "_SI": opName = s_OpTableFpu_Single[asmInstruction.Function]; break;
                                case "_DI": opName = s_OpTableFpu_Double[asmInstruction.Function]; break;
                                case "_WI": opName = s_OpTableFpu_FixedPoint[asmInstruction.Function]; break;
                                case "_LI": opName = s_OpTableFpu_FixedPoint[asmInstruction.Function]; break;
                                default: opName = s_OpTableCop1[asmInstruction.Rs]; break;
                            }
                            break;
                        }
                    default: return mainOp;
                }

                return opName;
            }
            else
            {
                return null;
            }
        }

        public static string DecodeGprReg(byte reg, Boolean o32)
        {
            if (o32)
                return s_O32GprLabel[reg];
            else
                return s_GprLabel[reg];
        }

        private static String DecodeOperand(MipsInstruction inst, Boolean o32, OperandDictionary lut, Byte reg, String defaultValue)
        {
            if (inst.Instruction == 0)
                return "";

            if (lut.ContainsKey(reg))
                return lut[reg](inst, o32);
            else
                return defaultValue;
        }

        public static string DecodeOperands(MipsInstruction inst, Boolean o32)
        {
            /* First figure out the general format of the instruction */
            try
            {
                switch (inst.Opcode)
                {
                    /* Special Instructions */
                    /* Defaults to dictionary of Special opcodes */
                    case 0: return DecodeOperand(inst, o32, s_OperandLUT_Special, inst.Function, s_OperandFormatLUT[4](inst, o32));

                    /* Register-Immediate Instructions */
                    case 1: return s_OperandFormatLUT[6](inst, o32);

                    /* Cop0 Instructions */
                    case 16:
                        {
                            switch (inst.Rs)
                            {
                                /* TLB Instructions */
                                case 16: return s_OperandFormatLUT[16](inst, o32);

                                /* Defaults to dictionary of Cop0 Opcodes */
                                default: return DecodeOperand(inst, o32, s_OperandLUT_Cop0, inst.Rs, s_OperandFormatLUT[05](inst, o32));
                            }
                        }

                    /* FPU Instructions */
                    case 17:
                        {
                            switch (inst.Rs)
                            {
                                /* BC1T Instructions */
                                case 08: return s_OperandFormatLUT[18](inst, o32);

                                /* All FPU Arithmetic Cases */
                                case 16: /* Fmt = Single */
                                case 17: /* Fmt = Double */
                                case 20: /* Fmt = Word */
                                case 21: /* Fmt = Long */
                                    return DecodeOperand(inst, o32, s_OperandLUT_FPU, inst.Function, s_OperandFormatLUT[04](inst, o32));

                                /* Defaults to dictionary of FPU Opcodes */
                                default: return DecodeOperand(inst, o32, s_OperandLUT_Cop1, inst.Rs, s_OperandFormatLUT[05](inst, o32));
                            }
                        }

                    /* Defaults to dictionary of Main Opcodes */
                    default: return DecodeOperand(inst, o32, s_OperandLUT_Main, inst.Opcode, s_OperandFormatLUT[24](inst, o32));
                }
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public static string DecodeFpuReg(Byte reg)
        {
            return s_Cop1RegLabel[reg];
        }

        public static string DecodeCop0Reg(Byte reg)
        {
            return s_Cop0RegLabel[reg];
        }
    }
}