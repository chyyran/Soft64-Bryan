using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            Disassembly = new ObservableCollection<DisassemblyLine>();

            WeakEventManager<MipsDebugger, EventArgs>.AddHandler(
            m_Debugger,
            "CodeScanned",
            (o, e) =>
            {
                Dispatcher.InvokeAsync(() =>
                {
                    Disassembly.Clear();

                    foreach (var l in m_Debugger.Disassembly)
                    {
                        Disassembly.Add(l);
                    }
                });
            });
        }

        private readonly static DependencyPropertyKey DisassemblyPropertyKey =
            DependencyProperty.RegisterReadOnly("Disassembly", typeof(ObservableCollection<DisassemblyLine>), typeof(MipsDebuggerViewModel),
            new PropertyMetadata());

        public readonly static DependencyProperty DisassemblyProperty =
            DisassemblyPropertyKey.DependencyProperty;

        public ObservableCollection<DisassemblyLine> Disassembly
        {
            get { return (ObservableCollection<DisassemblyLine>)GetValue(DisassemblyProperty); }
            private set {SetValue(DisassemblyPropertyKey, value); }
        }

        public MipsDebugger Debugger
        {
            get { return m_Debugger; }
        }

    }
}
