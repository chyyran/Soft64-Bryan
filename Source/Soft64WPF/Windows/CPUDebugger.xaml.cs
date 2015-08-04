using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using NLog;
using Soft64;
using Soft64.Debugging;
using Soft64.MipsR4300;
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
        private CompareWindow m_CompareWindow;
        

        static CPUDebugger()
        {
            
        }

        public CPUDebugger()
        {
            InitializeComponent();

            m_CompareWindow = new CompareWindow();
            xaml_MenuBtnRefreshDisam.Click += xaml_MenuBtnRefreshDisam_Click;
            xaml_MenuBtnSaveChanges.Click += xaml_MenuBtnSaveChanges_Click;

            Loaded += CPUDebugger_Loaded;

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

        void CPUDebugger_Loaded(object sender, RoutedEventArgs e)
        {
            xaml_DiassemblyView.RefreshDisasm();
            m_MachineModel.Cpu.State.Load();
        }

        void xaml_MenuBtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            m_MachineModel.Cpu.State.Store();
            xaml_DiassemblyView.RefreshDisasm();
            m_MachineModel.Cpu.State.Load();
        }

        void xaml_MenuBtnRefreshDisam_Click(object sender, RoutedEventArgs e)
        {
            xaml_DiassemblyView.RefreshDisasm();
            m_MachineModel.Cpu.State.Load();
        }

        private void MachineStateChangedHandler(Object o, LifeStateChangedArgs args)
        {
            if (args.NewState == LifetimeState.Running)
            {
            }
        }

        private void xaml_BtnStep_Click(object sender, RoutedEventArgs e)
        {
            m_CompareWindow.DoComparision();
            Debugger.Current.StepOnce();
            xaml_DiassemblyView.RefreshDisasm();
            m_MachineModel.Cpu.State.Load();
        }

        private void xaml_BtnCompare_Click(object sender, RoutedEventArgs e)
        {
            m_CompareWindow.Show();
        }

        private void xaml_BtnResHooks_Click(object sender, RoutedEventArgs e)
        {
            ResourceDebugger resDebugger = new ResourceDebugger();
            resDebugger.Show();
        }
    }
}