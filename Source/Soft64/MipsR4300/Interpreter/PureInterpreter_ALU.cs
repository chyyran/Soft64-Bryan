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
        [OpcodeHook("SLT")]
        private void Inst_Slt(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                if (MipsState.ReadGPR32Signed(inst.Rs) < MipsState.ReadGPR32Signed(inst.Rt))
                {
                    MipsState.GPRRegs[inst.Rd] = 1;
                }
                else
                {
                    MipsState.GPRRegs[inst.Rd] = 0;
                }
            }
            else
            {
                if (MipsState.ReadGPRSigned(inst.Rs) < MipsState.ReadGPRSigned(inst.Rt))
                {
                    MipsState.GPRRegs[inst.Rd] = 1;
                }
                else
                {
                    MipsState.GPRRegs[inst.Rd] = 0;
                }
            }
        }

        [OpcodeHook("SLTU")]
        private void Inst_Sltu(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                if (MipsState.ReadGPR32Unsigned(inst.Rs) < MipsState.ReadGPR32Unsigned(inst.Rt))
                {
                    MipsState.GPRRegs[inst.Rd] = 1;
                }
                else
                {
                    MipsState.GPRRegs[inst.Rd] = 0;
                }
            }
            else
            {
                if (MipsState.ReadGPRUnsigned(inst.Rs) < MipsState.ReadGPRUnsigned(inst.Rt))
                {
                    MipsState.GPRRegs[inst.Rd] = 1;
                }
                else
                {
                    MipsState.GPRRegs[inst.Rd] = 0;
                }
            }
        }

        [OpcodeHook("ADDIU")]
        private void Inst_Addiu(MipsInstruction inst)
        {
            unchecked
            {
                if (MipsState.Is32BitMode())
                {
                    MipsState.WriteGPR32Unsigned(inst.Rt, MipsState.ReadGPR32Unsigned(inst.Rs) + (UInt32)(Int32)(Int16)inst.Immediate);
                }
                else
                {
                    if (MipsState.ReadGPRUnsigned(inst.Rs).IsSigned32())
                    {
                        MipsState.WriteGPRUnsigned(inst.Rt, MipsState.ReadGPRUnsigned(inst.Rs) + (UInt64)(Int64)(Int16)inst.Immediate);
                    }
                }
            }
        }

        [OpcodeHook("ADDU")]
        private void Inst_Addu(MipsInstruction inst)
        {
            unchecked
            {
                if (MipsState.Is32BitMode())
                {
                    MipsState.WriteGPR32Unsigned(inst.Rd, MipsState.ReadGPR32Unsigned(inst.Rs) + MipsState.ReadGPR32Unsigned(inst.Rt));
                }
                else
                {
                    if (MipsState.ReadGPRUnsigned(inst.Rs).IsSigned32() && MipsState.ReadGPRUnsigned(inst.Rt).IsSigned32())
                    {
                        MipsState.WriteGPRUnsigned(inst.Rd, MipsState.ReadGPRUnsigned(inst.Rs) + MipsState.ReadGPRUnsigned(inst.Rt));
                    }
                }
            }
        }

        [OpcodeHook("ADD")]
        private void Inst_Add(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                try
                {
                    MipsState.WriteGPR32Signed(inst.Rd, MipsState.ReadGPR32Signed(inst.Rs) + MipsState.ReadGPR32Signed(inst.Rt));
                }
                catch (OverflowException)
                {
                    MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.OverFlow;
                }
                
            }
            else
            {
                if (MipsState.ReadGPRUnsigned(inst.Rs).IsSigned32() && MipsState.ReadGPRUnsigned(inst.Rt).IsSigned32())
                {
                    try
                    {
                        MipsState.WriteGPRSigned(inst.Rd, MipsState.ReadGPRSigned(inst.Rs) + MipsState.ReadGPRSigned(inst.Rt));
                    }
                    catch (OverflowException)
                    {
                        MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.OverFlow;
                    }
                }
            }
        }

        [OpcodeHook("ADDI")]
        private void Inst_Addi(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                try
                {
                    MipsState.WriteGPR32Signed(inst.Rt, MipsState.ReadGPR32Signed(inst.Rs) + (Int32)(Int16)inst.Immediate);
                }
                catch (OverflowException)
                {
                    MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.OverFlow;
                }
            }
            else
            {
                try
                {
                    MipsState.WriteGPRSigned(inst.Rt, MipsState.ReadGPRSigned(inst.Rs) + (Int64)(Int16)inst.Immediate);
                }
                catch (OverflowException)
                {
                    MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.OverFlow;
                }
            }
        }

        [OpcodeHook("DADD")]
        private void Inst_Dadd(MipsInstruction inst)
        {
            if (MipsState.Is64BitMode())
            {
                try
                {
                    MipsState.WriteGPRSigned(inst.Rd, MipsState.ReadGPRSigned(inst.Rs) + MipsState.ReadGPRSigned(inst.Rt));
                }
                catch (OverflowException)
                {
                    MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.OverFlow;
                }
            }
            else
            {
                MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.ReservedInstruction;
            }
        }

        [OpcodeHook("DADDI")]
        private void Inst_Dadid(MipsInstruction inst)
        {
            if (MipsState.Is64BitMode())
            {
                try
                {
                    MipsState.WriteGPRSigned(inst.Rd, MipsState.ReadGPRSigned(inst.Rs) + (Int64)(Int16)inst.Immediate);
                }
                catch (OverflowException)
                {
                    MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.OverFlow;
                }
            }
            else
            {
                MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.ReservedInstruction;
            }
        }

        [OpcodeHook("DADDU")]
        private void Inst_Daddu(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.ReservedInstruction;
            }
            else
            {
                unchecked
                {
                    MipsState.WriteGPRUnsigned(inst.Rd, MipsState.ReadGPRUnsigned(inst.Rs) + MipsState.ReadGPRUnsigned(inst.Rt));
                }
            }
        }

        [OpcodeHook("DADDIU")]
        private void Inst_Daddiu(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.ReservedInstruction;
            }
            else
            {
                unchecked
                {
                    MipsState.WriteGPRUnsigned(inst.Rd, MipsState.ReadGPRUnsigned(inst.Rs) + (UInt64)(Int64)(Int16)inst.Immediate);
                }
            }
        }


        [OpcodeHook("ORI")]
        private void Inst_Ori(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
                MipsState.WriteGPR32Unsigned(inst.Rt, MipsState.ReadGPR32Unsigned(inst.Rs) | (UInt32)inst.Immediate);
            else
                MipsState.WriteGPRUnsigned(inst.Rt, MipsState.ReadGPRUnsigned(inst.Rs) | (UInt64)inst.Immediate);
        }

        [OpcodeHook("OR")]
        private void Inst_Or(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
                MipsState.WriteGPR32Unsigned(inst.Rd, MipsState.ReadGPR32Unsigned(inst.Rs) | MipsState.ReadGPR32Unsigned(inst.Rt));
            else
                MipsState.WriteGPRUnsigned(inst.Rd, MipsState.ReadGPRUnsigned(inst.Rs) | MipsState.ReadGPR32Unsigned(inst.Rt));
        }

        [OpcodeHook("SLTI")]
        private void Inst_Slti(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                Boolean condition = MipsState.ReadGPR32Signed(inst.Rs) < ((Int32)(Int16)inst.Immediate);
                MipsState.WriteGPR32Unsigned(inst.Rt, condition ? 1U : 0U);
            }
            else
            {
                Boolean condition = MipsState.ReadGPRSigned(inst.Rs) < ((Int64)(Int16)inst.Immediate);
                MipsState.WriteGPRUnsigned(inst.Rt, condition ? 1UL : 0UL);
            }
        }

        [OpcodeHook("ANDI")]
        private void Inst_Andi(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                MipsState.WriteGPR32Unsigned(inst.Rt, MipsState.ReadGPR32Unsigned(inst.Rs) & (UInt32)inst.Immediate);
            }
            else
            {
                MipsState.WriteGPRUnsigned(inst.Rt, MipsState.ReadGPRUnsigned(inst.Rs) & (UInt64)inst.Immediate);
            }
        }

        [OpcodeHook("AND")]
        private void Inst_And(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                MipsState.WriteGPR32Unsigned(inst.Rd, MipsState.ReadGPR32Unsigned(inst.Rs) & MipsState.ReadGPR32Unsigned(inst.Rt));
            }
            else
            {
                MipsState.WriteGPRUnsigned(inst.Rd, MipsState.ReadGPRUnsigned(inst.Rs) & MipsState.ReadGPR32Unsigned(inst.Rt));
            }
        }

        [OpcodeHook("XORI")]
        private void Inst_Xori(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                MipsState.WriteGPR32Unsigned(inst.Rt, MipsState.ReadGPR32Unsigned(inst.Rs) ^ (UInt32)inst.Immediate);
            }
            else
            {
                MipsState.WriteGPRUnsigned(inst.Rt, MipsState.ReadGPRUnsigned(inst.Rs) ^ (UInt64)inst.Immediate);
            }
        }

        [OpcodeHook("SLL")]
        private void Inst_Sll(MipsInstruction inst)
        {
            UInt32 result = MipsState.ReadGPR32Unsigned(inst.Rt) << inst.ShiftAmount;

            if (MipsState.Is32BitMode())
            {
                MipsState.WriteGPR32Unsigned(inst.Rd, result);
            }
            else
            {
                MipsState.WriteGPRUnsigned(inst.Rd, (UInt64)(Int64)(Int16)result);
            }
        }

        [OpcodeHook("SRL")]
        private void Inst_Srl(MipsInstruction inst)
        {
            UInt32 result = MipsState.ReadGPR32Unsigned(inst.Rt) >> inst.ShiftAmount;

            if (MipsState.Is32BitMode())
            {
                MipsState.WriteGPR32Unsigned(inst.Rd, result);
            }
            else
            {
                MipsState.WriteGPRUnsigned(inst.Rd, (UInt64)(Int64)(Int16)result);
            }
        }

        [OpcodeHook("DDIV")]
        private void Inst_Ddiv(MipsInstruction inst)
        {
            if (MipsState.Is64BitMode())
            {
                try
                {
                    MipsState.Hi = (UInt64)(MipsState.ReadGPRSigned(inst.Rs) / MipsState.ReadGPRSigned(inst.Rt));
                    MipsState.Lo = (UInt64)(MipsState.ReadGPRSigned(inst.Rs) % MipsState.ReadGPRSigned(inst.Rt));
                }
                catch (OverflowException)
                {
                    MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.OverFlow
                }
            }
            else
            {
                MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.ReservedInstruction;
            }
        }

        [OpcodeHook("DDIVU")]
        private void Inst_Ddivu(MipsInstruction inst)
        {
            if (MipsState.Is64BitMode())
            {
                unchecked
                {
                    MipsState.Hi = (UInt64)(MipsState.ReadGPRSigned(inst.Rs) / MipsState.ReadGPRSigned(inst.Rt));
                    MipsState.Lo = (UInt64)(MipsState.ReadGPRSigned(inst.Rs) % MipsState.ReadGPRSigned(inst.Rt));
                }
            }
            else
            {
                MipsState.CP0Regs.CauseReg.ExceptionType = CP0.ExceptionCode.ReservedInstruction;
            }
        }
    }
}