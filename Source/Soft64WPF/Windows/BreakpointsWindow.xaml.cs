using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

            xaml_BreakpointListBox.ItemsSource = m_Debugger.Breakpoints;
        }

        private void MipsDebuggerPropertyChangedHandler(Object obj, PropertyChangedEventArgs args)
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (args.PropertyName == "Breakpoints")
                {
                    xaml_BreakpointListBox.Items.Refresh();
                }
            });
        }

        private void xaml_BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Int64 address = Int64.Parse(xaml_TextBoxAddress.Text, NumberStyles.AllowHexSpecifier);
                m_Debugger.AddBreakpoint(address);
                xaml_TextBoxAddress.Clear();
            }
            catch (FormatException)
            {
                MessageBox.Show("Bad hex format", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void xaml_BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (xaml_BreakpointListBox.SelectedItem != null)
            {
                Int64 address = (Int64)xaml_BreakpointListBox.SelectedItem;
                m_Debugger.RemoveBreakpoint(address);
            }
        }
    }
}
