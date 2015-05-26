using System;
using System.Collections.Generic;
using System.IO;
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
using Soft64;
using Soft64.Debugging;
using Soft64.MipsR4300.Debugging;
using Soft64Binding.WPF;

namespace Soft64WPF.Windows
{
    /// <summary>
    /// Interaction logic for CPUDebugger.xaml
    /// </summary>
    public partial class CPUDebugger : Window
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

            //WeakEventManager<Machine, LifeStateChangedArgs>.AddHandler(
            //    Machine.Current,
            //    "LifetimeStateChanged",
            //    MachineStateChangedHandler
            //    );
        }

        private void MachineStateChangedHandler(Object o, LifeStateChangedArgs args)
        {
            if (args.NewState == LifetimeState.Running)
            {

            }
        }

        private void ScanCode()
        {
            Stream vmemory = Machine.Current.DeviceCPU.VirtualMemoryStream;
            Int64 begin = 0xA4000040;
            Int64 current = vmemory.Position;
            Stream view = new VMemViewStream();
            view.Position = begin;
            BinaryReader reader = new BinaryReader(view);

            while (vmemory.Position < vmemory.Length)
            {
                try
                {
                    UInt64 read = reader.ReadUInt64();

                    /* Parse instructions */
                }
                catch
                {
                    break;
                }
            }

            


        }

        private void xaml_BtnPause_Click(object sender, RoutedEventArgs e)
        {
            Debugger.Current.Pause();

            ScanCode();
        }

        private void xaml_BtnResume_Click(object sender, RoutedEventArgs e)
        {
            Debugger.Current.Resume();
        }
    }
}
