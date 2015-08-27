using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soft64.MipsR4300;

namespace Soft64Binding.WPF
{
    public class ExecutionStateViewModel : MachineComponentViewModel
    {
        private ExecutionState m_State;

        public ExecutionStateViewModel(MachineViewModel machineVm, ExecutionState state) :
            base(machineVm)
        {
            m_State = state;
        }

        public void Load()
        {
            RegisterPC.RegValue = (UInt64)m_State.PC;
            RegisterHi.RegValue = m_State.Hi;
            RegisterLo.RegValue = m_State.Lo;

            for (Int32 i = 0; i < 32; i++)
            {
                GPRegisters[i].RegValue = m_State.GPRRegs[i];
            }
        }

        public void Store()
        {
            m_State.PC = (Int64)RegisterPC.RegValue;
            m_State.Hi = RegisterHi.RegValue;
            m_State.Lo = RegisterLo.RegValue;

            for (Int32 i = 0; i < 32; i++)
            {
                m_State.GPRRegs[i] = GPRegisters[i].RegValue;
            }
        }

        public static readonly DependencyProperty RegisterPCProperty =
            DependencyProperty.Register("RegisterPC", typeof(RegisterValue), typeof(EmulatorEngineViewModel),
            new PropertyMetadata(new RegisterValue()));

        public RegisterValue RegisterPC
        {
            get { return (RegisterValue)GetValue(RegisterPCProperty); }
            set { SetValue(RegisterPCProperty, value); }
        }

        public static readonly DependencyProperty RegisterHiProperty =
            DependencyProperty.Register("RegisterHi", typeof(RegisterValue), typeof(EmulatorEngineViewModel),
            new PropertyMetadata(new RegisterValue()));

        public RegisterValue RegisterHi
        {
            get { return (RegisterValue)GetValue(RegisterHiProperty); }
            set { SetValue(RegisterHiProperty, value); }
        }

        public static readonly DependencyProperty RegisterLoProperty =
            DependencyProperty.Register("RegisterLo", typeof(RegisterValue), typeof(EmulatorEngineViewModel),
            new PropertyMetadata(new RegisterValue()));

        public RegisterValue RegisterLo
        {
            get { return (RegisterValue)GetValue(RegisterLoProperty); }
            set { SetValue(RegisterLoProperty, value); }
        }

        public static readonly DependencyPropertyKey GPRegistersPropertyKey =
            DependencyProperty.RegisterReadOnly("GPRegisters", typeof(RegisterValue[]), typeof(EmulatorEngineViewModel),
            new PropertyMetadata(new RegisterValue[32] {
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
                new RegisterValue(),
            }));

        public static readonly DependencyProperty GPRegistersProperty =
            GPRegistersPropertyKey.DependencyProperty;

        public RegisterValue[] GPRegisters
        {
            get { return (RegisterValue[])GetValue(GPRegistersProperty); }
        }
    }
}
