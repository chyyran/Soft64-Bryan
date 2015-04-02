using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Soft64.Debugging;
using Soft64Binding.WPF;

namespace Soft64WPF.Windows
{
    /// <summary>
    /// Interaction logic for CPUDebugger.xaml
    /// </summary>
    public partial class CPUDebugger : RibbonWindow
    {
        private MachineViewModel machineModel;

        public CPUDebugger()
        {
            InitializeComponent();

            if (Debugger.Current == null)
            {
                throw new InvalidOperationException("No debugger is attached to core");
            }

            machineModel = (MachineViewModel)DataContext;
        }

        private void xaml_BtnPause_Click(object sender, RoutedEventArgs e)
        {
            Debugger.Current.Pause();
        }

        private void xaml_BtnResume_Click(object sender, RoutedEventArgs e)
        {
            Debugger.Current.Resume();
        }
    }
}
