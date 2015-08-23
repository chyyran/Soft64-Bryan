using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Soft64.MipsR4300.Interpreter;

namespace Soft64.MipsR4300.Debugging
{
    public class MipsDebugger
    {
        private List<DisassembledInstruction> m_Disassembly;
        private DebugInstructionReader m_InstReader;
        private CodeDog m_CodeSniffer;
        private Int32 m_Break = 0;

        public event EventHandler CodeScanned;

        public MipsDebugger()
        {
            m_Disassembly = new List<DisassembledInstruction>();
            m_CodeSniffer = new CodeDog();
            m_InstReader = new DebugInstructionReader();
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
                m_Disassembly.Clear();

                for (Int64 i = offset; i < offset + (4 * count); i += 4)
                {
                    m_InstReader.Position = i;
                    m_Disassembly.Add(m_InstReader.ReadDisasm(true));
                }

                var e = CodeScanned;

                if (e != null)
                {
                    e(this, new EventArgs());
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
            if (m_Break == 1)
            {
                Interlocked.Decrement(ref m_Break);
                Machine.Current.Pause();
            }
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
