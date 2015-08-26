using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Soft64.MipsR4300.Debugging;
using Soft64Binding.WPF;

namespace Soft64WPF.Windows
{
    /// <summary>
    /// Interaction logic for BreakpointsWindow.xaml
    /// </summary>
    public partial class BreakpointsWindow : Window
    {
        private MipsDebugger m_Debugger;

        public BreakpointsWindow()
        {
            InitializeComponent();

            MachineViewModel model = (MachineViewModel)FindResource("machineVM");
            m_Debugger = model.Cpu.Debugger.Debugger;

            WeakEventManager<MipsDebugger, PropertyChangedEventArgs>.AddHandler(m_Debugger, "PropertyChanged", MipsDebuggerPropertyChangedHandler);
        }

        private void MipsDebuggerPropertyChangedHandler(Object obj, PropertyChangedEventArgs args)
        {
            /* TODO: Update list of breakpoints */
        }
    }
}
