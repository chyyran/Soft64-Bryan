using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Soft64.MipsR4300.Interpreter;

namespace Soft64.MipsR4300.Debugging
{
    public class MipsDebugger
    {
        private List<DisassembledInstruction> m_Disassembly;
        private DebugInstructionReader m_InstReader;
        private List<Int64> m_PCRecord;
        private CodeDog m_CodeSniffer;

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
                List<DisassembledInstruction> disasm = new List<DisassembledInstruction>();

                for (Int64 i = offset; i < offset + count; i += 4)
                {
                    disasm.Add(m_InstReader.ReadDisasm(true));
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
            throw new NotImplementedException();
        }

        public void Attach()
        {
            throw new NotImplementedException();
        }
    }

    public struct DisassembledInstruction
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

        public MipsInstruction Instruction
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
