using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using NLog;
using Soft64;
using Soft64.Debugging;
using Soft64.Engines;
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
        private MipsDebugger m_Debugger;
        

        static CPUDebugger()
        {
            
        }

        public CPUDebugger()
        {
            InitializeComponent();
            m_Debugger = new MipsDebugger();
            m_Debugger.Attach();

            m_CompareWindow = new CompareWindow();
            xaml_MenuBtnRefreshDisam.Click += xaml_MenuBtnRefreshDisam_Click;
            xaml_MenuBtnSaveChanges.Click += xaml_MenuBtnSaveChanges_Click;

            Loaded += CPUDebugger_Loaded;

            m_MachineModel = (MachineViewModel)DataContext;
            m_MachineModel.MachineEventNotification += m_MachineModel_MachineEventNotification;
        }

        void m_MachineModel_MachineEventNotification(object sender, MachineEventNotificationArgs e)
        {
            if (e.EventType == MachineEventType.Paused)
            {
                ReadCpu();
            }
        }

        void CPUDebugger_Loaded(object sender, RoutedEventArgs e)
        {
            ReadCpu();
        }

        private void ReadCpu()
        {
            DiassembleCode();
            m_MachineModel.Cpu.State.Load();
        }

        private void EngineStateChangedHandler(Object o, EngineStatusChangedArgs args)
        {
            if (args.NewStatus == EngineStatus.Paused)
            {
                Dispatcher.InvokeAsync(() =>
                {
                    ReadCpu();
                });
            }
        }

        void xaml_MenuBtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            m_MachineModel.Cpu.State.Store();
            ReadCpu();
        }

        void xaml_MenuBtnRefreshDisam_Click(object sender, RoutedEventArgs e)
        {
            ReadCpu();
        }

        private void xaml_BtnStep_Click(object sender, RoutedEventArgs e)
        {
            /* Step Soft64 */
            m_Debugger.Break();
            
            /* If attached, step comparing core*/
            m_CompareWindow.NextStep();

            /* If attached, compare core states */
            m_CompareWindow.DoComparision();

            ReadCpu();
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

        private void DiassembleCode()
        {
            // TODO: Eventual dump huge chunk of memory and use virtualizing panels to show portions
            Int32 lineCount = (Int32)(xaml_DataGridDiassembly.ActualHeight / xaml_DataGridDiassembly.FontSize);
            m_MachineModel.Cpu.Debugger.Debugger.DisassembleCode(m_MachineModel.TargetMachine.DeviceCPU.State.PC, lineCount);
        }
    }
}