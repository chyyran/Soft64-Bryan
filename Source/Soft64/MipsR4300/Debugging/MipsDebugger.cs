using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300.Debugging
{
    public class MipsDebugger
    {
        private List<DisassemblyLine> m_Lines;
        private List<BranchRange> m_BranchRanges;
        private InstructionReader m_InstReader;
        /* Beakpoints here */
        private Int32 m_ScanSize;
        private Boolean m_ABIMarkings;
        private Boolean m_SymbolMarkings;

        public event EventHandler CodeScanned;

        public const Int32 DMEMSize = 0x1000;

        public MipsDebugger()
        {
            m_Lines = new List<DisassemblyLine>();
            m_InstReader = new InstructionReader(MemoryAccessMode.SafeVirtual);
        }

        /// <summary>
        /// Scan memory for selected size and build a diassembly list
        /// </summary>
        /// <param name="scanSize"></param>
        public void ScanCode(Int32 scanSize, Boolean abiNames)
        {
            /* Run the code scan on an asychronous thread */
            Task.Factory.StartNew(() =>
            {
                /* Pause the machine if machine is running */
                //if (Machine.Current.Engine.Status == Engines.EngineStatus.Running)
                //    Machine.Current.Engine.PauseThreads();

                /* Get the Mips state */
                ExecutionState cpustate = Machine.Current.DeviceCPU.State;

                m_Lines.Clear();
                m_InstReader.Position = cpustate.PC;

                for (int i = 0; i < scanSize; i++)
                {
                    DisassemblyLine line = new DisassemblyLine();
                    line.Address = m_InstReader.Position;

                    MipsInstruction inst = m_InstReader.ReadInstruction();
                    line.MnemonicOp = Disassembler.DecodeOpName(inst);
                    line.Operands = Disassembler.DecodeOperands(inst, abiNames);
                    line.BytesHi = m_InstReader.ReadHi;
                    line.BytesLo = m_InstReader.ReadLo;
                    m_Lines.Add(line);
                }

                //if (Machine.Current.Engine.IsPaused)
                //    Machine.Current.Engine.ResumeThreads();
                var e = CodeScanned;

                if (e != null)
                {
                    e(this, new EventArgs());
                }
            });
        }

        public IEnumerable<DisassemblyLine> Disassembly
        {
            get { return m_Lines; }
        }
    }

    public struct DisassemblyLine
    {
        public Int64 Address
        {
            get;
            internal set;
        }

        public String MnemonicOp
        {
            get;
            internal set;
        }

        public String Operands
        {
            get;
            internal set;
        }

        public Int32 BytesHi
        {
            get;
            internal set;
        }

        public Int32 BytesLo
        {
            get;
            internal set;
        }
    }

    public struct BranchRange
    {
        private Int64 m_Begin;
        private Int64 m_End;

        public BranchRange(Int64 begin, Int64 end)
        {
            m_Begin = begin;
            m_End = end;
        }

        public Int64 Begin { get { return m_Begin; } }

        public Int64 End { get { return m_End; } }
    }

    public class InstructionBreakpoint
    {
        public Int64 Address { get; set; }
    }
}
