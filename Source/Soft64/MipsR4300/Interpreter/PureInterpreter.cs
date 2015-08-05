/*
Soft64 - C# N64 Emulator
Copyright (C) Soft64 Project @ Codeplex
Copyright (C) 2013 - 2014 Bryan Perris

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NLog;

namespace Soft64.MipsR4300.Interpreter
{
    public partial class PureInterpreter : BaseInterpreter
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private InterpreterTable m_OpTable;
        private Boolean m_NullifiedInstruction;
        private Int64 m_BranchDelaySlot;
        private Boolean m_IsBranch;
        private Int64 m_BranchTarget;

        internal sealed class OpcodeHookAttribute : Attribute
        {
            public String OpcodeName { get; private set; }

            public OpcodeHookAttribute(String name)
            {
                OpcodeName = name;
            }
        }

        public PureInterpreter()
        {
            m_OpTable = new InterpreterTable();
            HookOpcodeMethods();
            m_OpTable.InitializeCallTables();
        }

        public override void Step()
        {
            /* Fetch the instruction into a UInt32 */
            try
            {
                /* Go to branch */
                if (m_IsBranch)
                    MipsState.PC = m_BranchTarget;

                /* Fetch instruction at PC */
                MipsInstruction inst = FetchInstruction(MipsState.PC);

                /* If we branched, execute the delay slot */
                if (m_IsBranch)
                {
                    MipsInstruction bsdInst = FetchInstruction(m_BranchDelaySlot);
                    TraceOp(m_BranchDelaySlot, bsdInst);
                    m_OpTable.CallInstruction(bsdInst);
                    m_IsBranch = false;
                }
                else
                {

                    /* Execute the instruction unless its nullifed */
                    if (!m_NullifiedInstruction)
                    {
                        TraceOp(MipsState.PC, inst);
                        m_OpTable.CallInstruction(inst);
                    }
                    else
                    {
                        m_NullifiedInstruction = false;
                    }

                    /* If branching has been set, skip PC increment */
                    if (!m_IsBranch)
                        MipsState.PC += 4;
                    else
                        MipsState.PC += 8;
                }

            }
            catch (Exception e)
            {
                MipsState.PC -= 4; /* Move the counter back to the instruction that has faulted */
                HasFaulted = true;

                /* Important: This ensures that the engine thread comes to an halt seeing the thrown exception */
                throw e;
            }
        }

        [Conditional("DEBUG")]
        public void TraceOp(Int64 pc, MipsInstruction inst)
        {
            if (logger.IsDebugEnabled)
                logger.Debug("{0:X8} {1:X4} {2:X4} {3}", pc, inst.Instruction >> 16, inst.Instruction & 0xFFFF, inst.ToString());
        }

        public void LinkAddress(UInt64 address)
        {
            MipsState.GPRRegs64[31] = address;
        }

        private void HookOpcodeMethods()
        {
            var methodQuery =
                from method in this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).AsParallel()
                let attribute = method.GetCustomAttribute<OpcodeHookAttribute>()
                where attribute != null
                select new { MethodTarget = method.CreateDelegate(typeof(Action<MipsInstruction>), this), OpName = attribute.OpcodeName };

            var opTablePropDictionary = m_OpTable.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .AsParallel()
                        .ToDictionary(prop => prop.Name, prop => prop);

            ParallelEnumerable.ForAll(methodQuery, result =>
                {
                    PropertyInfo propInfo = null;

                    if (opTablePropDictionary.TryGetValue(result.OpName, out propInfo))
                    {
                        propInfo.SetValue(m_OpTable, result.MethodTarget);
                    }
                });
        }
    }
}