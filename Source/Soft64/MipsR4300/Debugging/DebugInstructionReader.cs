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
            MipsInstruction inst = ReadInstruction();

            return new DisassembledInstruction
                (
                  address: inst.Address,
                  disasm: Disassembler.DecodeOpName(inst) + " " + Disassembler.DecodeOperands(inst, abiDecode),
                  instruction: inst
                );
        }
    }
}
