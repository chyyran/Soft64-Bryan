using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Soft64.MipsR4300.Interpreter;

namespace Soft64.MipsR4300.Debugging
{
    public class MipsDebugger : INotifyPropertyChanged
    {
        private List<DisassembledInstruction> m_Disassembly;
        private DebugInstructionReader m_InstReader;
        private CodeDog m_CodeSniffer;
        private Int32 m_Break = 0;
        private HashSet<Int64> m_InstructionBreakpoints;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private Object m_Lock = new object();
        private Int64 m_LastPC;

        public event EventHandler CodeScanned;

        public MipsDebugger()
        {
            m_Disassembly = new List<DisassembledInstruction>();
            m_CodeSniffer = new CodeDog();
            m_InstReader = new DebugInstructionReader();
            m_InstructionBreakpoints = new HashSet<long>();
        }

        public void StartDebugging()
        {
            m_CodeSniffer.Start();
        }

        /// <summary>
        /// Disassemble the current region of memory
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="abi"></param>
        public void DisassembleCode(Int64 offset, Int32 count)
        {
            Task.Factory.StartNew(() =>
            {
                lock (m_Lock)
                {
                    m_Disassembly.Clear();

                    for (Int64 i = offset; i < offset + (4 * count); i += 4)
                    {
                        if (i > 0xFFFFFFFFL)
                            continue;

                        m_InstReader.Position = i;
                        m_Disassembly.Add(m_InstReader.ReadDisasm(true));
                    }

                    var e = CodeScanned;

                    if (e != null)
                    {
                        e(this, new EventArgs());
                    }
                }
            });
        }

        //public void FindBranches()
        //{
        //    m_BranchRanges.Clear();

        //    for (Int32 i = 0; i < m_Disassembly.Count; i++)
        //    {
        //        if (Regex.IsMatch(m_Disassembly[i].MnemonicOp, "b{2,4}"))
        //        {

        //            Int64 targetAddress =
        //                PureInterpreter.BranchComputeTargetAddress(
        //                m_Disassembly[i].Instruction.Address,
        //                m_Disassembly[i].Instruction.Immediate).ResolveAddress();

        //            m_BranchRanges.Add(
        //                new BranchRange(
        //                    (Int32)m_Disassembly[i].Instruction.Address,
        //                    targetAddress));
        //        }
        //    }
        //}

        public IEnumerable<DisassembledInstruction> Disassembly
        {
            get { return m_Disassembly; }
        }

        public void Break()
        {
            Interlocked.Increment(ref m_Break);
        }

        public void Attach()
        {
            Machine.Current.DeviceCPU.EnableDebugMode();
            Machine.Current.DeviceCPU.DebugStep += DeviceCPU_DebugStep;
        }

        void DeviceCPU_DebugStep(object sender, EventArgs e)
        {
            if (m_InstructionBreakpoints.Count > 0 && m_InstructionBreakpoints.Contains(Machine.Current.DeviceCPU.State.PC))
            {
                if (m_LastPC != Machine.Current.DeviceCPU.State.PC)
                {
                    m_LastPC = Machine.Current.DeviceCPU.State.PC;
                    logger.Debug("Breakpoint hit: " + Machine.Current.DeviceCPU.State.PC.ToString("X8"));
                    m_Break = 1;
                }
            }

            if (m_Break == 1)
            {
                Interlocked.Decrement(ref m_Break);
                Machine.Current.Pause();
            }

            m_LastPC = Machine.Current.DeviceCPU.State.PC;
        }

        public void Step()
        {
            if (Machine.Current.IsRunning)
            {
                Interlocked.Increment(ref m_Break);
            }
            else
            {
                m_Break = 1;
                Machine.Current.Run();
            }
        }

        public IEnumerable<Int64> Breakpoints
        {
            get { return m_InstructionBreakpoints; }
        }

        public void AddBreakpoint(Int64 address)
        {
            if (m_InstructionBreakpoints.Contains(address))
                return;

            m_InstructionBreakpoints.Add(address);
            OnPropertyChanged("Breakpoints");
        }

        public void RemoveBreakpoint(Int64 address)
        {
            if (!m_InstructionBreakpoints.Contains(address))
                return;

            m_InstructionBreakpoints.Remove(address);
            OnPropertyChanged("Breakpoints");
        }

        public Boolean ContainsBreakpoint(Int64 address)
        {
            return m_InstructionBreakpoints.Contains(address);
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            var e = PropertyChanged;

            if (e != null)
            {
                e(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
