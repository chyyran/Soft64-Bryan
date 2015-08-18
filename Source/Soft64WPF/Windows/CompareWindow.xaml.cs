using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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
using Soft64;
using Soft64.Engines;
using Soft64.MipsR4300;
using Soft64Binding.WPF;

namespace Soft64WPF.Windows
{
    /// <summary>
    /// Interaction logic for CompareWindow.xaml
    /// </summary>
    public partial class CompareWindow : Window
    {
        public CompareWindow()
        {
            InitializeComponent();

            MachineViewModel mvm = (MachineViewModel)FindResource("machineVM");
            mvm.MachineEventNotification += mvm_MachineEventNotification;
        }

        void mvm_MachineEventNotification(object sender, MachineEventNotificationArgs e)
        {
            if (e.EventType == MachineEventType.Paused)
                Dispatcher.InvokeAsync(DoComparision);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (xaml_EngineSelection.SelectedIndex == 1)
            {
                Assembly mupenAssembly = AppDomain.CurrentDomain.Load("CompareEngine.Mupen");

                if (mupenAssembly != null)
                {
                    dynamic engine = mupenAssembly.CreateInstance("CompareEngine.Mupen.MupenEngine");
                    Machine.Current.MipsCompareEngine = engine;

                    xaml_CompareResults.Document.Blocks.Clear();
                    xaml_CompareResults.Document.Blocks.Add(new Paragraph());

                    xaml_CompareResults.AppendText("Secondary emulator core attached, ready for comparsion");
                }
                else
                {
                    MessageBox.Show("Error: Cannot load mupen compare engine assembly", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DoComparision()
        {
            if (Machine.Current.MipsCompareEngine != null)
            {
                MipsSnapshot snapshotA = Machine.Current.DeviceCPU.CreateSnapshot();
                MipsSnapshot snapshotB = Machine.Current.MipsCompareEngine.TakeSnapshot();

                /* Have WPF UI compare snapshot and show results */
                CompareSnapshot(snapshotA, snapshotB);
            }
        }

        public void NextStep()
        {
            if (Machine.Current.MipsCompareEngine != null)
                Machine.Current.MipsCompareEngine.ThreadUnlock();
        }

        private void CompareSnapshot(MipsSnapshot snapshotA, MipsSnapshot snapshotB)
        {
            xaml_CompareResults.Document.Blocks.Clear();
            xaml_CompareResults.Document.Blocks.Add(new Paragraph());
            
            xaml_CompareResults.AppendText(String.Format("        {0}  |  {1}\n", snapshotA.Name, snapshotB.Name));
            xaml_CompareResults.AppendText(String.Format("PC    0x{0:X16} | 0x{1:X16}\n", snapshotA.PC, snapshotB.PC));
            xaml_CompareResults.AppendText(String.Format("Lo    0x{0:X16} | 0x{1:X16}\n", snapshotA.Lo, snapshotB.Lo));
            xaml_CompareResults.AppendText(String.Format("Hi    0x{0:X16} | 0x{1:X16}\n", snapshotA.Hi, snapshotB.Hi));

            for (Int32 i = 0; i < 32; i++)
            {
                xaml_CompareResults.AppendText(String.Format("GPR{2:D2} 0x{0:X16} | 0x{1:X16}\n", snapshotA.GPR[i], snapshotB.GPR[i], i));
            }
        }
    }
}
