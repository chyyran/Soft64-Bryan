using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

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

        private Int32 m_GridWidth;
        private Int32 m_GridHeight;
        private Boolean m_HexUpperNibble;
        private Size m_FontSize;
        private StreamViewModel m_ClonedStreamVM;
        private Dictionary<Int32, HexEditorTextBlock> m_HexLUT = new Dictionary<int, HexEditorTextBlock>();
        private Dictionary<Int32, HexEditorTextBlock> m_AsciiLut = new Dictionary<int, HexEditorTextBlock>();
        private List<HexEditorTextBlock> m_BlockCache = new List<HexEditorTextBlock>();
        private Int32 m_OldGridWidth, m_OldGridHeight;

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

            Dispatcher.Invoke(Refresh);
        }

        private void UpdateHexGrid()
        {
            try
            {
                if (!IsInitialized || m_ClonedStreamVM == null || m_ClonedStreamVM.ReadBuffer == null)
                    return;

                AdjustDataRows();
                CheckBlockCache();
                ReadDataBytes();
            }
            catch (ArgumentException)
            {
                /* Ignore these since it can happen from too much resizing */
            }
        }

        private void ReadDataBytes()
        {
            Byte[] readBuffer = m_ClonedStreamVM.ReadBuffer;
            Int64 position = m_ClonedStreamVM.StreamPosition;

            /* Allocate the row objects on UI Thread */
            AllocateRows();

            /* Create a local list copy of the rows */
            HexEditorRow[] rows = new HexEditorRow[DataRows.Count];
            DataRows.CopyTo(rows, 0);

            /* Create an async task to fill in the data */
            Task.Factory.StartNew(() =>
            {
                for (Int32 i = 0; i < m_GridHeight; i++)
                {
                    Byte[] rowBuffer = new Byte[m_GridWidth];
                    Array.Copy(readBuffer, m_GridWidth * i, rowBuffer, 0, m_GridWidth);

                    HexEditorRow row = rows[i];
                    row.RowIndex = i;
                    row.Address = position + (m_GridWidth * i);
                    row.SetBytes(m_BlockCache, rowBuffer, HexLUT, AsciiLUT);
                }

                Dispatcher.Invoke(() =>
                {
                    foreach (var row in rows)
                        row.UpdateText();
                });
            });
        }

        private void AllocateRows()
        {
            for (Int32 i = 0; i < m_GridHeight; i++)
            {
                if (i >= DataRows.Count)
                    DataRows.Add(new HexEditorRow());
            }
        }

        private void CheckBlockCache()
        {
            /* Check block cache */
            Int32 size = m_GridWidth * m_GridHeight * 2;
            Int32 count = m_BlockCache.Count;

            if (count <= size)
            {
                for (Int32 i = 0; i < (size - count); i++)
                {
                    HexEditorTextBlock block = new HexEditorTextBlock();
                    block.Margin = new Thickness(2, 0, 0, 0);
                    m_BlockCache.Add(block);
                }
            }
        }

        private void AdjustDataRows()
        {
            if (m_GridHeight != m_OldGridHeight ||
                m_GridWidth != m_OldGridWidth)
            {
                /* if the grid size changed, then rebuild the data rows */
                DataRows.Clear();
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
            Int32 col = block.ColIndex;
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
            Int32 col = block.ColIndex % m_GridWidth;

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
            {
                UpdateHexGrid();

                m_OldGridWidth = m_GridWidth;
                m_OldGridHeight = m_GridHeight;
            }
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
            get { return m_HexLUT; }
        }

        private Dictionary<Int32, HexEditorTextBlock> AsciiLUT
        {
            get { return m_AsciiLut; }
        }
    }
}