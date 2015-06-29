using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soft64.MipsR4300;

namespace Soft64Binding.WPF
{
    public class ExecutionStateViewModel : DependencyObject
    {
        private ExecutionState m_State;

        public ExecutionStateViewModel(ExecutionState state)
        {
            m_State = state;
        }

        public void Load()
        {
            RegisterPC = (UInt64)m_State.PC;
            RegisterHi = m_State.Hi;
            RegisterLo = m_State.Lo;

            for (Int32 i = 0; i < 32; i++)
            {
                GPRegisters[i] = m_State.GPRRegs64[i];
            }
        }

        public void Save()
        {
            m_State.PC = (Int64)RegisterPC;
            m_State.Hi = RegisterHi;
            m_State.Lo = RegisterLo;

            for (Int32 i = 0; i < 32; i++)
            {
                m_State.GPRRegs64[i] = GPRegisters[i];
            }
        }

        public static readonly DependencyProperty RegisterPCProperty =
            DependencyProperty.Register("RegisterPC", typeof(UInt64), typeof(EmulatorEngineViewModel),
            new PropertyMetadata());

        public UInt64 RegisterPC
        {
            get { return (UInt64)GetValue(RegisterPCProperty); }
            set { SetValue(RegisterPCProperty, value); }
        }

        public static readonly DependencyProperty RegisterHiProperty =
            DependencyProperty.Register("RegisterHi", typeof(UInt64), typeof(EmulatorEngineViewModel),
            new PropertyMetadata());

        public UInt64 RegisterHi
        {
            get { return (UInt64)GetValue(RegisterHiProperty); }
            set { SetValue(RegisterHiProperty, value); }
        }

        public static readonly DependencyProperty RegisterLoProperty =
            DependencyProperty.Register("RegisterLo", typeof(UInt64), typeof(EmulatorEngineViewModel),
            new PropertyMetadata());

        public UInt64 RegisterLo
        {
            get { return (UInt64)GetValue(RegisterLoProperty); }
            set { SetValue(RegisterLoProperty, value); }
        }

        public static readonly DependencyPropertyKey GPRegistersPropertyKey =
            DependencyProperty.RegisterReadOnly("GPRegisters", typeof(UInt64[]), typeof(EmulatorEngineViewModel),
            new PropertyMetadata(new UInt64[32]));

        public static readonly DependencyProperty GPRegistersProperty =
            GPRegistersPropertyKey.DependencyProperty;

        public UInt64[] GPRegisters
        {
            get { return (UInt64[])GetValue(GPRegistersProperty); }
        }
    }
}
