using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using Soft64.Toolkits.WPF;
using Soft64Binding.WPF;

namespace Soft64WPF.Windows
{
    /// <summary>
    /// Interaction logic for MemoryEditorWindow.xaml
    /// </summary>
    public partial class MemoryEditorWindow : Window
    {
        private static readonly DependencyProperty CurrentMemoryStreamProperty =
            DependencyProperty.Register("CurrentMemoryStream", typeof(StreamViewModel), typeof(MemoryEditorWindow),
            new PropertyMetadata());

        private Stream m_PhysicalMem;

        static MemoryEditorWindow()
        {
        }

        public MemoryEditorWindow()
        {
            InitializeComponent();
            xaml_HexScrollBar.Scroll += xaml_HexScrollBar_Scroll;
            
            MouseWheel += MemoryEditorWindow_MouseWheel;
            xaml_ChkboxVAdressMode.Checked += xaml_ChkboxVAdressMode_Checked;
            xaml_ChkboxVAdressMode.Unchecked += xaml_ChkboxVAdressMode_Unchecked;
            m_PhysicalMem = new N64StreamWrapper();

            CurrentMemoryStream = StreamViewModel.NewModelFromStream(m_PhysicalMem);
        }

        void MemoryEditorWindow_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            Int32 sign = Math.Sign(e.Delta);

            CurrentAddress = CurrentAddress - sign * xaml_HexEditor.NumCols;
        }

        private void xaml_ChkboxVAdressMode_Unchecked(object sender, RoutedEventArgs e)
        {
            CurrentAddress -= 0xA0000000;
            CurrentMemoryStream = StreamViewModel.NewModelFromStream(m_PhysicalMem); ;
        }

        private void xaml_ChkboxVAdressMode_Checked(object sender, RoutedEventArgs e)
        {
            CurrentAddress += 0xA0000000;
            CurrentMemoryStream = ((MachineViewModel)DataContext).Cpu.DebugVirtualMemory;
        }

        private void xaml_HexScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollEventType == ScrollEventType.LargeDecrement)
            {
                CurrentAddress -= xaml_HexEditor.NumRows;
            }

            if (e.ScrollEventType == ScrollEventType.LargeIncrement)
            {
                CurrentAddress += xaml_HexEditor.NumRows;
            }

            if (e.ScrollEventType == ScrollEventType.SmallDecrement)
            {
                CurrentAddress -= xaml_HexEditor.NumCols;
            }

            if (e.ScrollEventType == ScrollEventType.SmallIncrement)
            {
                CurrentAddress += xaml_HexEditor.NumCols;
            }

            xaml_HexScrollBar.Value = 5;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        private void xaml_BtnRefreshHex_Click(object sender, RoutedEventArgs e)
        {
            xaml_HexEditor.Refresh();
        }

        private StreamViewModel CurrentMemoryStream
        {
            get { return (StreamViewModel)GetValue(CurrentMemoryStreamProperty); }
            set { SetValue(CurrentMemoryStreamProperty, value); }
        }

        private Int64 CurrentAddress
        {
            get { return Int64.Parse(txtBoxBaseAddress.Text, NumberStyles.AllowHexSpecifier); }
            set { txtBoxBaseAddress.Text = value.ToString("X8"); }
        }
    }
}