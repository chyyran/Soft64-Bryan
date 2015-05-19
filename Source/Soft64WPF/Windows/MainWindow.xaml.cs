/*
Soft64 - C# N64 Emulator
Copyright (C) Soft64 Project @ Codeplex
Copyright (C) 2013 - 2014 Bryan Perris

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using NLog;
using Soft64;
using Soft64.Media;
using Soft64.MipsR4300.Interpreter;
using Soft64WPF.Styles;

namespace Soft64WPF.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
            GlassStyleLoader.ApplyWindowGlassStyle(this);

            xamlControl_EmuRunButton.Click += xamlControl_EmuRunButton_Click;
            xamlControl_MainCartBrowseFileButton.Click += xamlControl_MainCartBrowseFileButton_Click;
            xaml_ButtonScript.Click += xaml_ButtonScript_Click;

            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        void xaml_ButtonScript_Click(object sender, RoutedEventArgs e)
        {
            PyWindow window = new PyWindow();
            window.Show();
        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Runtime Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void xamlControl_MainCartBrowseFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = @"(N64 Cartridge ROM (*.z64;*.n64;*.v64;*.bin)|*.n64;*.z64;*.v64;*.bin|All Files (*.*)|*.*;";

            if (dlg.ShowDialog() == true)
            {
                FileStream fs = File.OpenRead(dlg.FileName);
                VirtualCartridge cart = new VirtualCartridge(fs);
                Machine.Current.RCP.DevicePI.ReleaseCartridge();
                Machine.Current.RCP.DevicePI.MountCartridge(cart);
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        private void xamlControl_EmuRunButton_Click(object sender, RoutedEventArgs e)
        {
            logger.Trace("----- N64 Emulation Started -----");

            /* For now we are using default crap */
            Machine.Current.RCP.Engine = new PureInterpreter();
            Machine.Current.CPU.Engine = new PureInterpreter();
            Machine.Current.SystemBootMode = BootMode.HLE_IPL;
            Machine.Current.Initialize();
            Machine.Current.Run();
        }

        private void xaml_ButtonEjectCartridge_Click(object sender, RoutedEventArgs e)
        {
            Machine.Current.RCP.DevicePI.ReleaseCartridge();
        }

        private void xaml_ButtonToolMemoryEditor_Click(object sender, RoutedEventArgs e)
        {
            MemoryEditorWindow window = new MemoryEditorWindow();
            window.Show();
        }

        private void xaml_ButtonToolCPUDebugger_Click(object sender, RoutedEventArgs e)
        {
            CPUDebugger win = new CPUDebugger();
            win.Show();
        }

        private void xaml_ButtonElfRun_Click(object sender, RoutedEventArgs e)
        {
            /* TODO: Load elf file and run directly */
        }
    }
}