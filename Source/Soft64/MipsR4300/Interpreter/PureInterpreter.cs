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
using System.Linq;
using System.Reflection;
using NLog;

namespace Soft64.MipsR4300.Interpreter
{
    public partial class PureInterpreter : BaseInterpreter
    {
        private InterpreterTable m_OpTable;
        private Action m_BranchDelaySlotAction;
        private Boolean m_NullifiedInstruction;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

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
                MipsInstruction inst = FetchInstruction();

#if !FAST_UNSAFE_BUILD

                if (logger.IsDebugEnabled)
                    logger.Debug("{0:X8} {1:X8} {2}", Machine.Current.CPU.State.PC, inst.Instruction, inst.ToString());
#endif

                /* Check if we are executing a branch delay slot */
                Boolean doBranchDelay = m_BranchDelaySlotAction != null;

                /* Execute the instruction unless its nullifed */
                if (!m_NullifiedInstruction)
                    m_OpTable.CallInstruction(inst);
                else
                    m_NullifiedInstruction = true;

                /* Perform the branch jump if we have executed a branch delay slot */
                if (doBranchDelay)
                {
                    m_BranchDelaySlotAction();
                    m_BranchDelaySlotAction = null;
                }

                /* If we are going to do a branch delay slot executon next round, set the debug flag */
                BranchDelaySlot = m_BranchDelaySlotAction != null;

                /* Increment the PC */
                MipsState.PC += 4;
            }
            catch (Exception e)
            {
                MipsState.PC -= 4; /* Move the counter back to the instruction that has faulted */

                HasFaulted = true;

                /* Important: This ensures that the engine thread comes to an halt seeing the thrown exception */
                throw e;
            }
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