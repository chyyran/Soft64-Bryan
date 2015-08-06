using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soft64.MipsR4300.Debugging;

namespace Soft64Binding.WPF
{
    public class MipsDebuggerViewModel : DependencyObject
    {
        private MipsDebugger m_Debugger;

        public MipsDebuggerViewModel()
        {
            m_Debugger = new MipsDebugger();
            Disassembly = m_Debugger.Disassembly;
        }

        private readonly static DependencyPropertyKey DisassemblyPropertyKey =
            DependencyProperty.RegisterReadOnly("Disassembly", typeof(IList<DisassemblyLine>), typeof(MipsDebuggerViewModel),
            new PropertyMetadata());

        public readonly static DependencyProperty DisassemblyProperty =
            DisassemblyPropertyKey.DependencyProperty;

        public IList<DisassemblyLine> Disassembly
        {
            get { return (IList<DisassemblyLine>)GetValue(DisassemblyProperty); }
            private set {SetValue(DisassemblyPropertyKey, value); }
        }
    }
}
