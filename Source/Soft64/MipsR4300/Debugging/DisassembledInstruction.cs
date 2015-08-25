using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300.Debugging
{
    public class DisassembledInstruction
    {
        private Int64 m_Address;
        private String m_Disasm;
        private MipsInstruction m_Instruction;
        private String m_FullLine;

        public DisassembledInstruction(Int64 address, String disasm, MipsInstruction instruction)
        {
            m_Address = address;
            m_Disasm = disasm;
            m_Instruction = instruction;

            Int32 hi = (Int32)(instruction.Instruction & 0xFF00) >> 16;
            Int32 lo = (Int32)(instruction.Instruction & 0x00FF);

            m_FullLine = String.Format("{0:X8} {1:X4} {2:X4} {3}",
                (UInt32)m_Address,
                hi,
                lo,
                m_Disasm);
        }

        public Int64 Address { get { return m_Address; } }

        public String Disassembly { get { return m_Disasm; } }

        public MipsInstruction Instruction { get { return m_Instruction; } }

        public String FullDisassembly { get { return m_FullLine; } }
    }
}
