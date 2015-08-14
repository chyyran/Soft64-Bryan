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
        private List<DisassembledInstruction> m_SniffedDisassembly;
        
        private InstructionReader m_InstReader;
        private List<Int64> m_PCRecord;
        /* Beakpoints here */

        private CodeDog m_CodeSniffer;

        public event EventHandler CodeScanned;

        public MipsDebugger()
        {
            m_Disassembly = new List<DisassembledInstruction>();
            m_SniffedDisassembly = new List<DisassembledInstruction>();
            m_CodeSniffer = new CodeDog();
            m_InstReader = new InstructionReader(MemoryAccessMode.DebugVirtual);
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
        public void DisassembleCode(Int64 offset, Int32 count, Boolean abi)
        {
            /* TODO: How this will work.....
             * Scan the selected memory region and build a disasm list
             * Compare the disasm list with a code sniffer LINQ based query
             * Merge in info found by the code sniffer (background disassembled code)
             * Figure out if code sniffer makes a mistake based on recorded PC path
             */

            Task.Factory.StartNew(() =>
            {
                for (Int64 i = offset; i < offset + count; i += 4)
                {
                    DisassembledInstruction diasm = Disassemble(abi);
                    m_Disassembly.Add(diasm);

                   if (m_CodeSniffer.Contains(diasm))
                   {
                       /* Get advanced code info (branch paths, symbols, functions) */
                   }
                   else
                   {
                       /* get basic info */
                   }
                }
            });
        }

        private DisassembledInstruction Disassemble(Boolean abi)
        {
            DisassembledInstruction disasm = new DisassembledInstruction();
            disasm.Address = m_InstReader.Position;

            MipsInstruction inst = m_InstReader.ReadInstruction();
            disasm.Instruction = inst;


            disasm.MnemonicOp = Disassembler.DecodeOpName(inst);
            disasm.Operands = Disassembler.DecodeOperands(inst, abi);
            disasm.BytesHi = m_InstReader.ReadHi;
            disasm.BytesLo = m_InstReader.ReadLo;

            return disasm;
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
                /* Get the Mips state */
                ExecutionState cpustate = Machine.Current.DeviceCPU.State;

                m_Disassembly.Clear();
                m_InstReader.Position = cpustate.PC;

                for (int i = 0; i < scanSize; i++)
                {
                    m_Disassembly.Add(Disassemble(abiNames));
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
