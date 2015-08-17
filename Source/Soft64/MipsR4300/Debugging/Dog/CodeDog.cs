using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Soft64.MipsR4300.Debugging.Dog;

namespace Soft64.MipsR4300.Debugging
{
    public sealed class CodeDog : 
        IDisposable
    {
        private const Int64 BootCodeOffset = 0xA4000040;
        private const Int32 BootCodeSize = 4032;
        private DebugInstructionReader m_InstReader;
        private Boolean m_Disposed;

        /* Monitor PI DMA to capture ROM -> RAM transfers */

        public CodeDog()
        {
            m_InstReader = new DebugInstructionReader();
        }

        public void Start()
        {

        }

        private DisassembledInstruction ReadDisasm()
        {
            DisassembledInstruction disasm = new DisassembledInstruction();
            disasm.Address = m_InstReader.Position;
            MipsInstruction inst = m_InstReader.ReadInstruction();
            disasm.BytesHi = m_InstReader.ReadHi;
            disasm.BytesLo = m_InstReader.ReadLo;
            disasm.Instruction = inst;
            return disasm;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(Boolean disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    m_InstReader.Dispose();
                }

                m_Disposed = true;
            }
        }
    }
}
