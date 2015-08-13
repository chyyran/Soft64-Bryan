using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300.Debugging
{
    /* Sniffs out where the program lies in physical memory */
    internal sealed class CodeSniffer
    {
        public const Int64 BootCodeOffset = 0x04000040;
        public const Int32 BootCodeSize = 4032;
        public List<Block> m_RamRegions;

        private InstructionReader m_InstReader;

        /* Monitor PI DMA to capture ROM -> RAM transfers */

        public CodeSniffer()
        {
            m_InstReader = new InstructionReader(MemoryAccessMode.Physical);
        }

        public IList<DisassemblyLine> FindCodeBlocks()
        {
            List<DisassemblyLine> lines = new List<DisassemblyLine>();


            // TODO

            return lines;
        }
    }

    internal struct Block
    {
        public Int64 Offset { get; set; }

        public Int32 Size { get; set; }
    }
}
