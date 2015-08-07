using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soft64.MipsR4300.Debugging;

namespace Soft64Binding.WPF
{
    public class MipsDebuggerViewModel : MachineComponentViewModel
    {
        private MipsDebugger m_Debugger;

        public MipsDebuggerViewModel(MachineViewModel machineVM) :
            base(machineVM)
        {
            m_Debugger = new MipsDebugger();
            Disassembly = m_Debugger.Disassembly;
        }

        private readonly static DependencyPropertyKey DisassemblyPropertyKey =
            DependencyProperty.RegisterReadOnly("Disassembly", typeof(IList<DisassemblyLine>), typeof(MipsDebuggerViewModel),
            new PropertyMetadata());

        public readonly static DependencyProperty DisassemblyProperty =
            DisassemblyPropertyKey.DependencyProperty;

        public IEnumerable<DisassemblyLine> Disassembly
        {
            get { return (IEnumerable<DisassemblyLine>)GetValue(DisassemblyProperty); }
            private set {SetValue(DisassemblyPropertyKey, value); }
        }

        public MipsDebugger Debugger
        {
            get { return m_Debugger; }
        }

    }
}
