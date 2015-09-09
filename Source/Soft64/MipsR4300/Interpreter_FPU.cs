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
            if (MipsState.Is32BitMode())
            {
                MipsState.WriteGPR32Unsigned(inst.Rt, MipsState.Fpr.ReadFPR32Unsigned(inst.Rd));
            }
            else
            {
                MipsState.WriteGPRUnsigned(inst.Rt, MipsState.Fpr.ReadFPRUnsigned(inst.Rd));
            }
        }

        [OpcodeHook("CTC1")]
        private void Inst_Ctc1(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
            {
                MipsState.Fpr.WriteFPR32Unsigned(inst.Rd, MipsState.ReadGPR32Unsigned(inst.Rt));
            }
            else
            {
                MipsState.Fpr.WriteFPRUnsigned(inst.Rd, MipsState.ReadGPRUnsigned(inst.Rt));
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
                fpuEntitiy.Value = Math.Abs(fpuEntitiy.Value);
                fpuEntitiy.Store(inst.Fd);
            }
            else
            {
                CauseException = ExceptionCode.Invalid;
            }
        }
    }
}