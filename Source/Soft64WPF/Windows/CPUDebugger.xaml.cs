using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        private Int64 m_LastAddress;

        static CPUDebugger()
        {
            
        }

        public CPUDebugger()
        {
            InitializeComponent();

            m_CompareWindow = new CompareWindow();
            xaml_MenuBtnRefreshDisam.Click += xaml_MenuBtnRefreshDisam_Click;
            xaml_MenuBtnSaveChanges.Click += xaml_MenuBtnSaveChanges_Click;
            xaml_BtnCompare.Click += xaml_BtnCompare_Click;
            xaml_BtnResHooks.Click += xaml_BtnResHooks_Click;
            Loaded += CPUDebugger_Loaded;
            Unloaded += CPUDebugger_Unloaded;

            m_MachineModel = (MachineViewModel)DataContext;
            m_MachineModel.MachineEventNotification += m_MachineModel_MachineEventNotification;

            m_Debugger = m_MachineModel.Cpu.Debugger.Debugger;
            m_Debugger.Attach();
        }

        void CPUDebugger_Unloaded(object sender, RoutedEventArgs e)
        {
            m_MachineModel.Cpu.Debugger.Finished -= Debugger_Finished;
        }

        void Debugger_Finished(object sender, EventArgs e)
        {
            SetPCLine(Machine.Current.DeviceCPU.State.PC);
        }

        void m_MachineModel_MachineEventNotification(object sender, MachineEventNotificationArgs e)
        {
            if (e.EventType == MachineEventType.Paused)
            {
                Dispatcher.InvokeAsync(() =>
                {
                    ReadCpu();
                });
            }
        }

        void CPUDebugger_Loaded(object sender, RoutedEventArgs e)
        {
            ReadCpu();
            m_MachineModel.Cpu.Debugger.Finished += Debugger_Finished;
        }

        private void ReadCpu()
        {
            DiassembleCode();
            m_MachineModel.Cpu.State.Load();
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
            m_Debugger.Step();
            
            /* If attached, step comparing core*/
            m_CompareWindow.NextStep();

            /* If attached, compare core states */
            m_CompareWindow.DoComparision();
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

        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

        private void SetPCLine(Int64 address)
        {
            Int32 index = (Int32)(address - m_LastAddress) / 4;

            if (index < 0)
                return;

            ProgramCounterArrowGroup.SetActive("debugger", index);
            xaml_DataGridDisassembly.SelectedIndex = index;
            xaml_DataGridDisassembly.ScrollIntoView(xaml_DataGridDisassembly.SelectedItem);
        }

        private void DiassembleCode()
        {
            Int64 pc = Machine.Current.DeviceCPU.State.PC;
            Int32 lineCount = (Int32)(xaml_DataGridDisassembly.ActualHeight / (FontSize * 1.50));
            Int32 byteCount = (4 * lineCount);
            Int64 offset = pc;
            Int64 end = m_LastAddress + byteCount;

            /* If Pc is moving */
            /* Goto PC if we are far away from where the view was */
            if (pc > m_LastAddress + (2 * byteCount) ||
                pc < m_LastAddress ||
                xaml_DataGridDisassembly.Items.Count < 1)
            {
                m_LastAddress = pc;
                m_Debugger.DisassembleCode(m_LastAddress, lineCount);
            }
            else if (pc >= end)
            {
                m_LastAddress += 4;
                m_Debugger.DisassembleCode(m_LastAddress, lineCount);
            }
            else
            {
                SetPCLine(Machine.Current.DeviceCPU.State.PC);
            }
        }

        private void xaml_BtnBreak_Click(object sender, RoutedEventArgs e)
        {
            m_Debugger.Break();
        }

        private void xaml_BtnContinue_Click(object sender, RoutedEventArgs e)
        {
            m_MachineModel.TargetMachine.Run();
        }
    }
}