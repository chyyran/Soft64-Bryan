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

namespace Soft64.MipsR4300.Interpreter
{
    public partial class PureInterpreter
    {
        [OpcodeHook("MTC0")]
        private void Inst_Mtc0(MipsInstruction inst)
        {
            /* TODO: Implement this instruction better if needed */
            MipsState.CP0Regs.SetReg(inst.Rd, MipsState.ReadGPRUnsigned(inst.Rt));
        }

        //[OpcodeHook("MFC0")]
        //private void Inst_Mfc0(MipsInstruction inst)
        //{

        //}
    }
}