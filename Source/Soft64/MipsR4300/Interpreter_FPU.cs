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
        [OpcodeHook("CFC1")]
        private void Inst_Cfc1(MipsInstruction inst)
        {
            if (inst.Rd == 0)
            {
                MipsState.WriteGPRUnsigned(inst.Rd, MipsState.FCR0);
            }

            if (inst.Rd == 31)
            {
                MipsState.WriteGPRUnsigned(inst.Rd, MipsState.FCR31.RegisterValue);
            }
        }

        [OpcodeHook("CTC1")]
        private void Inst_Ctc1(MipsInstruction inst)
        {
            if (inst.Fs == 0)
            {
                MipsState.FCR0 = MipsState.ReadGPR32Unsigned(inst.Rt);
            }

            if (inst.Fs == 31)
            {
                MipsState.FCR31.RegisterValue = MipsState.ReadGPR32Unsigned(inst.Rt);
            }
        }

        [OpcodeHook("FPU_ABS")]
        private void Inst_FpuAbs(MipsInstruction inst)
        {
            if (!MipsState.CP0Regs.StatusReg.AdditionalFPR)
                return;

            DataFormat format = inst.DecodeDataFormat();

            if (format == DataFormat.Single || format == DataFormat.Double)
            {
                FPUEntity fpuEntitiy = new FPUEntity(format, MipsState);
                fpuEntitiy.Load(inst.Fs);
                FPUEntity.SetRoundingMode(MipsState.FCR31.RM);
                fpuEntitiy.Value = Math.Abs(fpuEntitiy.Value);
                fpuEntitiy.Store(inst.Fd);
            }
            else
            {
                CauseException = ExceptionCode.Invalid;
            }
        }

        [OpcodeHook("FPU_ADD")]
        private void Inst_FpuAdd(MipsInstruction inst)
        {
            if (!MipsState.CP0Regs.StatusReg.AdditionalFPR)
                return;

            DataFormat format = inst.DecodeDataFormat();

            if (format == DataFormat.Single || format == DataFormat.Double)
            {
                FPUEntity left = new FPUEntity(format, MipsState);
                FPUEntity right = new FPUEntity(format, MipsState);
                FPUEntity result = new FPUEntity(format, MipsState);

                left.Load(inst.Fs);
                right.Load(inst.Ft);

                try
                {
                    FPUEntity.SetRoundingMode(MipsState.FCR31.RM);
                    result.Value = left + right;
                    result.Store(inst.Fd);
                }
                catch (ArithmeticException)
                {
                    CauseException = ExceptionCode.FloatingPoint;
                }

            }
            else
            {
                CauseException = ExceptionCode.Invalid;
            }
        }

        [OpcodeHook("FPU_COND")]
        private void Inst_FpuCond(MipsInstruction inst)
        {
            if (!MipsState.CP0Regs.StatusReg.AdditionalFPR)
                return;

            DataFormat format = inst.DecodeDataFormat();

            if (format == DataFormat.Single || format == DataFormat.Double)
            {
                FPUEntity a = new FPUEntity(format, MipsState);
                FPUEntity b = new FPUEntity(format, MipsState);
                Boolean less = false;
                Boolean unordered = false;
                Boolean equal = false;
                Boolean condU = (inst.Function & 1) != 0;
                Boolean condE = ((inst.Function >> 1) & 1) != 0;
                Boolean condL = ((inst.Function >> 2) & 1) != 0;

                a.Load(inst.Fs);
                b.Load(inst.Ft);

                if (a.IsNaN && b.IsNaN)
                {
                    unordered = true;

                    if ((inst.Function & 8) != 0)
                    {
                        CauseException = ExceptionCode.Invalid;
                        return;
                    }
                }
                else
                {
                    less = a < b;
                    equal = a == b;
                }

                MipsState.FCR31.Condition = ((condL && less) || (condE && equal) || (condU && unordered));
            }
        }
    }
}