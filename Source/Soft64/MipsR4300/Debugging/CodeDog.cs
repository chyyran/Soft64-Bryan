using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Soft64.MipsR4300.Debugging.Dog;

namespace Soft64.MipsR4300.Debugging
{
    public sealed class CodeDog : IQueryable<DisassembledInstruction>, IDisposable
    {
        private const Int64 BootCodeOffset = 0xA4000040;
        private const Int32 BootCodeSize = 4032;
        private DebugInstructionReader m_InstReader;
        private Boolean m_Disposed;
        private List<BranchRange> m_BranchRanges;
        private Boolean m_ABIMarkings;
        private Boolean m_SymbolMarkings;
        private List<DisassembledInstruction> m_BootDisassembly;
        private List<Block> m_RamRegions;
        private DogQueryProvider m_QueryProvider;
        private Expression m_DogExpression;

        /* Monitor PI DMA to capture ROM -> RAM transfers */

        public CodeDog()
        {
            m_InstReader = new DebugInstructionReader();
            m_RamRegions = new List<Block>();
            m_QueryProvider = new DogQueryProvider();

            BuildExpression();
        }

        private void BuildExpression()
        {
           /* TODO: Build expression tree comparing instructions */
        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                /* Bootcode Read */
                m_InstReader.Position = BootCodeOffset;
                for (Int32 i = 0; i < BootCodeSize / 4; i++)
                {
                    m_BootDisassembly.Add(ReadDisasm());
                }

                /* TODO: DMA scanners here */
            });
        }

        public IEnumerator<DisassembledInstruction> GetEnumerator()
        {
            yield return default(DisassembledInstruction);
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

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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

        #region IQueryable Members

        public Type ElementType
        {
            get { throw new NotImplementedException(); }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return m_DogExpression; }
        }

        public IQueryProvider Provider
        {
            get { return m_QueryProvider; }
        }

        #endregion
    }

    internal struct Block
    {
        public Int64 Offset { get; set; }

        public Int32 Size { get; set; }
    }
}
