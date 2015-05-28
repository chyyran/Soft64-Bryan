using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Soft64.Toolkits.WPF;

namespace Soft64.Toolkits.WPF
{
    /// <summary>
    /// Interaction logic for HexEditor.xaml
    /// </summary>
    public partial class HexEditor : UserControl
    {
        public static readonly DependencyProperty StreamSourceProperty =
            DependencyProperty.Register("StreamSource", typeof(StreamViewModel), typeof(HexEditor),
            new PropertyMetadata(StreamVMChanged));

        private static readonly DependencyProperty DataRowsProperty =
            DependencyProperty.Register("DataRows", typeof(ObservableCollection<HexEditorRow>), typeof(HexEditor),
            new PropertyMetadata());

        public static readonly DependencyProperty GridLineBrushProperty =
            DependencyProperty.Register("GridLineBrush", typeof(Brush), typeof(HexEditor),
            new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public static readonly DependencyProperty BaseAddressProperty =
            DependencyProperty.Register("BaseAddress", typeof(Int64), typeof(HexEditor),
            new PropertyMetadata((o, a) =>
            {
                HexEditor editor = o as HexEditor;
                if (editor.m_ClonedStreamVM != null)
                {
                    editor.m_ClonedStreamVM.StreamPosition = (Int64)a.NewValue;
                    editor.Refresh();
                }
            }));

        public static readonly DependencyProperty RefreshOnResizeProperty =
            DependencyProperty.Register("RefreshOnResize", typeof(Boolean), typeof(HexEditor),
            new PropertyMetadata());

        private static readonly DependencyProperty HexLUTProperty =
            DependencyProperty.Register("HexLUT", typeof(Dictionary<Int32, HexEditorTextBlock>), typeof(HexEditor),
            new PropertyMetadata(new Dictionary<Int32, HexEditorTextBlock>()));

        private static readonly DependencyProperty AsciiLUTProperty =
            DependencyProperty.Register("AsciiLUT", typeof(Dictionary<Int32, HexEditorTextBlock>), typeof(HexEditor),
            new PropertyMetadata(new Dictionary<Int32, HexEditorTextBlock>()));

        private Int32 m_GridWidth;
        private Int32 m_GridHeight;
        private Boolean m_HexUpperNibble;
        private Size m_FontSize;
        private StreamViewModel m_ClonedStreamVM;

        static HexEditor()
        {
            SetupFontProperties();
        }

        public Int64 BaseAddress
        {
            get { return (Int64)GetValue(BaseAddressProperty); }
            set { SetValue(BaseAddressProperty, value); }
        }

        public Brush GridLineBrush
        {
            get { return (Brush)GetValue(GridLineBrushProperty); }
            set { SetValue(GridLineBrushProperty, value); }
        }

        public Boolean RefreshOnResize
        {
            get { return (Boolean)GetValue(RefreshOnResizeProperty); }
            set { SetValue(RefreshOnResizeProperty, value); }
        }

        private static void SetupFontProperties()
        {
            DependencyProperty[] properties = new[]
                {
                    UserControl.FontFamilyProperty,
                    UserControl.FontSizeProperty,
                    UserControl.FontStretchProperty,
                    UserControl.FontStyleProperty,
                    UserControl.FontWeightProperty
                };

            foreach (var prop in properties)
            {
                prop.AddOwner(typeof(HexEditor), new FrameworkPropertyMetadata(FontPropertyChanged));
            }
        }

        public HexEditor()
        {
            InitializeComponent();
            DataRows = new ObservableCollection<HexEditorRow>();
            xaml_rootGrid.MouseLeftButtonDown += xaml_rootGrid_MouseLeftButtonDown;
            this.TextInput += HexEditor_TextInput;
            this.KeyDown += HexEditor_KeyDown;
            this.SizeChanged += HexEditor_SizeChanged;
        }

        private void HexEditor_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            /* Compute the number of rows and columns that fit into the avaiable graphics space */
            m_GridHeight = (Int32)Math.Floor((e.NewSize.Height / 1.10) / m_FontSize.Height);
            m_GridWidth = (Int32)Math.Floor((e.NewSize.Width / 2.5) / m_FontSize.Width);

            /* If some reason grid lengths are invalid, keep sizes to 0 */
            if (m_GridHeight < 0 || m_GridWidth < 0)
            {
                m_GridHeight = 0;
                m_GridWidth = 0;
            }

            Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    Dispatcher.Invoke(Refresh);
                });
        }

        private void UpdateHexGrid()
        {
            if (!IsInitialized || m_ClonedStreamVM == null)
                return;

            DataRows.Clear();

            Byte[] readBuffer = m_ClonedStreamVM.ReadBuffer;
            Int64 position = m_ClonedStreamVM.StreamPosition;

            if (readBuffer == null)
                return;

            for (Int32 i = 0; i < m_GridHeight; i++)
            {
                Byte[] rowBuffer = new Byte[m_GridWidth];
                Array.Copy(readBuffer, m_GridWidth * i, rowBuffer, 0, m_GridWidth);

                HexEditorRow row = new HexEditorRow();
                row.SetBytes(rowBuffer);
                row.Address = position + (m_GridWidth * i);
                row.RowIndex = i;

                DataRows.Add(row);
            }
        }

        private void HexEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (xaml_CaretControl.TargetControl == null)
            {
                MoveCaret(0, 0, BlockType.Hex);
            }

            HexEditorTextBlock block = xaml_CaretControl.TargetControl as HexEditorTextBlock;

            Int32 row = block.RowIndex;
            Int32 col = block.Index;
            BlockType type = block.BlockType;

            switch (e.Key)
            {
                case Key.Left: MoveCaret(row, col - 1, type); e.Handled = true; break;
                case Key.Right: MoveCaret(row, col + 1, type); e.Handled = true; break;
                case Key.Up: MoveCaret(row - 1, col, type); e.Handled = true; break;
                case Key.Down: MoveCaret(row + 1, col, type); e.Handled = true; break;
                default: return;
            }
        }

        private void HexEditor_TextInput(object sender, TextCompositionEventArgs e)
        {
            HexEditorTextBlock block = xaml_CaretControl.TargetControl as HexEditorTextBlock;
            Int32 col = block.Index % m_GridWidth;

            if (block != null)
            {
                if (block.BlockType == BlockType.Ascii)
                {
                    WriteAsciiCharacter(e, block.RowIndex, col, block.BlockType);
                }
                else
                {
                    WriteHexCharacter(e, block.RowIndex, col, block.BlockType);
                }
            }
        }

        private void WriteAsciiCharacter(TextCompositionEventArgs e, Int32 row, Int32 col, BlockType type)
        {
            if (!String.IsNullOrEmpty(e.Text))
            {
                WriteByte((byte)e.Text[0], row, col);
            }

            MoveCaret(row, col + 1, BlockType.Ascii);
        }

        private void WriteHexCharacter(TextCompositionEventArgs e, Int32 row, Int32 col, BlockType type)
        {
            Byte nibble = 0;

            switch (e.Text.ToUpper()[0])
            {
                case '0': nibble = 0; break;
                case '1': nibble = 1; break;
                case '2': nibble = 2; break;
                case '3': nibble = 3; break;
                case '4': nibble = 4; break;
                case '5': nibble = 5; break;
                case '6': nibble = 6; break;
                case '7': nibble = 7; break;
                case '8': nibble = 8; break;
                case '9': nibble = 9; break;
                case 'A': nibble = 0xA; break;
                case 'B': nibble = 0xB; break;
                case 'C': nibble = 0xC; break;
                case 'D': nibble = 0xD; break;
                case 'E': nibble = 0xE; break;
                case 'F': nibble = 0xF; break;
                default: return;
            }

            Byte value = DataRows[row].GetByteValue(col);

            if (!m_HexUpperNibble)
            {
                m_HexUpperNibble = true;
                value = (Byte)((value & 0x0F) | (nibble << 4));
            }
            else
            {
                m_HexUpperNibble = false;
                MoveCaret(row, col + 1, BlockType.Hex);
                value = (Byte)((value & 0xF0) | nibble);
            }

            WriteByte(value, row, col);
        }

        private void MoveCaret(Int32 row, Int32 col, BlockType type)
        {
            if (col < 0)
            {
                col = 0;
                row--;
            }

            if (row < 0)
            {
                row = 0;
            }

            if (col >= m_GridWidth)
            {
                col = 0;
                row++;
            }

            if (row >= DataRows.Count)
            {
                row = 0;
            }

            UIElement element = HexEditorTextBlock.GetBlockAt(row, col, type == BlockType.Hex ? HexLUT : AsciiLUT);

            if (element != null)
            {
                xaml_CaretControl.TargetControl = element as FrameworkElement;
            }
        }

        private void WriteByte(Byte b, Int32 row, Int32 offset)
        {
            if (m_ClonedStreamVM != null)
            {
                DataRows[row].WriteByte(m_ClonedStreamVM.GetSteamSource(), offset, b);
            }
        }

        private void xaml_rootGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(this);

            TextBlock targettedTextBlock = e.OriginalSource as HexEditorTextBlock;

            if (targettedTextBlock != null)
            {
                xaml_CaretControl.TargetControl = targettedTextBlock;
            }
        }

        private static void FontPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs a)
        {
            HexEditor editor = o as HexEditor;

            if (editor != null)
            {
                var formattedText = new FormattedText(
                    "00",
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface(editor.FontFamily, editor.FontStyle, editor.FontWeight, editor.FontStretch),
                    editor.FontSize,
                    Brushes.Black);

                editor.m_FontSize = new Size(formattedText.Width, formattedText.Height);
                editor.Refresh();
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        protected static void StreamVMChanged(DependencyObject o, DependencyPropertyChangedEventArgs a)
        {
            HexEditor editor = o as HexEditor;

            if (editor != null)
            {
                StreamViewModel svm = a.NewValue as StreamViewModel;

                if (svm != null)
                {
                    editor.m_ClonedStreamVM = svm.DeepCopy();
                    editor.m_ClonedStreamVM.PropertyChanged += editor.m_ClonedStreamVM_PropertyChanged;
                    editor.m_ClonedStreamVM.StreamPosition = editor.BaseAddress;
                    editor.Refresh();
                }
            }
        }

        private void m_ClonedStreamVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ReadBuffer")
                UpdateHexGrid();
        }

        public void Refresh()
        {
            if (!IsInitialized || m_GridWidth <= 0 || m_GridHeight <= 0)
                return;

            if (m_ClonedStreamVM != null)
            {
                m_ClonedStreamVM.BufferSize = m_GridHeight * m_GridWidth;
                m_ClonedStreamVM.Refresh();
            }
        }

        public StreamViewModel StreamSource
        {
            get { return (StreamViewModel)GetValue(StreamSourceProperty); }
            set { SetValue(StreamSourceProperty, value); }
        }

        internal ObservableCollection<HexEditorRow> DataRows
        {
            get { return (ObservableCollection<HexEditorRow>)GetValue(DataRowsProperty); }
            set { SetValue(DataRowsProperty, value); }
        }

        public Int32 NumRows
        {
            get { return m_GridHeight; }
        }

        public Int32 NumCols
        {
            get { return m_GridWidth; }
        }

        private Dictionary<Int32, HexEditorTextBlock> HexLUT
        {
            get { return (Dictionary<Int32, HexEditorTextBlock>)GetValue(HexLUTProperty); }
            set { SetValue(HexLUTProperty, value); }
        }

        private Dictionary<Int32, HexEditorTextBlock> AsciiLUT
        {
            get { return (Dictionary<Int32, HexEditorTextBlock>)GetValue(AsciiLUTProperty); }
            set { SetValue(AsciiLUTProperty, value); }
        }
    }
}