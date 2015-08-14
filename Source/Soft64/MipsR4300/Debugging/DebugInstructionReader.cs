using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300.Debugging
{
    public sealed class DebugInstructionReader : InstructionReader
    {
        public DebugInstructionReader() : base(MemoryAccessMode.DebugVirtual)
        {

        }

        public DisassembledInstruction ReadDisasm(Boolean abiDecode)
        {
            DisassembledInstruction disasm = new DisassembledInstruction();
            disasm.Address = Position;

            MipsInstruction inst = ReadInstruction();
            disasm.Instruction = inst;


            disasm.MnemonicOp = Disassembler.DecodeOpName(inst);
            disasm.Operands = Disassembler.DecodeOperands(inst, abiDecode);
            disasm.BytesHi = ReadHi;
            disasm.BytesLo = ReadLo;

            return disasm;
        }
    }
}
