using System;
using System.Windows;
using System.Windows.Documents;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Win32;
using Soft64;

namespace Soft64WPF.Windows
{
    /// <summary>
    /// Interaction logic for PyWindow.xaml
    /// </summary>
    public partial class PyWindow : Window
    {
        private ScriptEngine m_PyEngine;

        public PyWindow()
        {
            InitializeComponent();
            InitPython();

            xaml_BtnRunScript.Click += xaml_BtnRunScript_Click;
            xaml_BtnRunScriptFile.Click += xaml_BtnRunScriptFile_Click;
        }

        private void xaml_BtnRunScriptFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "IronPython Script (*.py) | *.py;";

            if (dlg.ShowDialog() == true)
            {
                m_PyEngine.ExecuteFile(dlg.FileName);
            }
        }

        private void InitPython()
        {
            m_PyEngine = Python.CreateEngine();
            m_PyEngine.Runtime.LoadAssembly(typeof(Machine).Assembly);
            m_PyEngine.Runtime.LoadAssembly(typeof(Console).Assembly);
            m_PyEngine.Runtime.LoadAssembly(typeof(MessageBox).Assembly);
        }

        private void xaml_BtnRunScript_Click(object sender, RoutedEventArgs e)
        {
            String currentScript =
                new TextRange(
                    xaml_RtbPyScript.Document.ContentStart,
                    xaml_RtbPyScript.Document.ContentEnd).Text;

            try
            {
                m_PyEngine.Execute(currentScript);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, "Script Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}