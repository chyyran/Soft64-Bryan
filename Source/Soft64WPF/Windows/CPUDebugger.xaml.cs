using System;
using System.IO;
using System.Windows;
using NLog;
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
        private MachineViewModel m_MachineModel;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        

        static CPUDebugger()
        {
            
        }

        public CPUDebugger()
        {
            InitializeComponent();
            xaml_MenuBtnRefreshDisam.Click += xaml_MenuBtnRefreshDisam_Click;

            if (Debugger.Current == null)
            {
                throw new InvalidOperationException("No debugger is attached to core");
            }

            m_MachineModel = (MachineViewModel)DataContext;

            //WeakEventManager<Machine, LifeStateChangedArgs>.AddHandler(
            //    Machine.Current,
            //    "LifetimeStateChanged",
            //    MachineStateChangedHandler
            //    );
        }

        void xaml_MenuBtnRefreshDisam_Click(object sender, RoutedEventArgs e)
        {
            xaml_DiassemblyView.RefreshDisasm();
        }

        private void MachineStateChangedHandler(Object o, LifeStateChangedArgs args)
        {
            if (args.NewState == LifetimeState.Running)
            {
            }
        }

        private void xaml_BtnStep_Click(object sender, RoutedEventArgs e)
        {
            if (Machine.Current.MipsCompareEngine != null)
            {
                Boolean result = Machine.Current.MipsCompareEngine.CompareState(Machine.Current.DeviceCPU.State);
                Machine.Current.MipsCompareEngine.Release();

                logger.Debug("Core Compare Result [0x{0:X8}]: {1}", Machine.Current.DeviceCPU.State.PC, result);
            }

            Debugger.Current.StepOnce();
            xaml_DiassemblyView.RefreshDisasm();
        }
    }
}