using System;
using System.IO;
using System.Windows;
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
            xaml_MenuBtnRefreshDisam.Click += xaml_MenuBtnRefreshDisam_Click;

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

        void xaml_MenuBtnRefreshDisam_Click(object sender, RoutedEventArgs e)
        {
            
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