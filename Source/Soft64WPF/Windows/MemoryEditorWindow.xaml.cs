using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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

        static MemoryEditorWindow()
        {
        }

        public MemoryEditorWindow()
        {
            InitializeComponent();
            xaml_HexScrollBar.Scroll += xaml_HexScrollBar_Scroll;
            xaml_ChkboxVAdressMode.Checked +=xaml_ChkboxVAdressMode_Checked;
            xaml_ChkboxVAdressMode.Unchecked += xaml_ChkboxVAdressMode_Unchecked;
            CurrentMemoryStream = StreamViewModel.NewModelFromStream(new N64StreamWrapper());
        }

        void xaml_ChkboxVAdressMode_Unchecked(object sender, RoutedEventArgs e)
        {
            CurrentAddress -= 0xA0000000;
            CurrentMemoryStream = StreamViewModel.NewModelFromStream(new N64StreamWrapper()); ;
        }

        void xaml_ChkboxVAdressMode_Checked(object sender, RoutedEventArgs e)
        {
            CurrentAddress += 0xA0000000;
            CurrentMemoryStream = ((MachineViewModel)DataContext).Cpu.DebugVirtualMemory;
        }

        private void xaml_HexScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollEventType == ScrollEventType.LargeDecrement)
            {
                txtBoxBaseAddress.Text =
                    (Int64.Parse(txtBoxBaseAddress.Text, NumberStyles.AllowHexSpecifier) - xaml_HexEditor.NumRows).ToString("X8");
            }

            if (e.ScrollEventType == ScrollEventType.LargeIncrement)
            {
                txtBoxBaseAddress.Text =
                    (Int64.Parse(txtBoxBaseAddress.Text, NumberStyles.AllowHexSpecifier) + xaml_HexEditor.NumRows).ToString("X8");
            }

            if (e.ScrollEventType == ScrollEventType.SmallDecrement)
            {
                txtBoxBaseAddress.Text =
                   (Int64.Parse(txtBoxBaseAddress.Text, NumberStyles.AllowHexSpecifier) - xaml_HexEditor.NumCols).ToString("X8");
            }

            if (e.ScrollEventType == ScrollEventType.SmallIncrement)
            {
                txtBoxBaseAddress.Text =
                   (Int64.Parse(txtBoxBaseAddress.Text, NumberStyles.AllowHexSpecifier) + xaml_HexEditor.NumCols).ToString("X8");
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