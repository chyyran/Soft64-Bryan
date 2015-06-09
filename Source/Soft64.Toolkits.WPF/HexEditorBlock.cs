using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Soft64.Toolkits.WPF
{
    public sealed class HexEditorBlock : Control
    {
        private BlockType m_Type;
        private Int32 m_RowIndex;
        private Int32 m_ColIndex;
        private Dictionary<Int32, HexEditorBlock> m_LUT;
        private Int32 m_BlockHash;
        private GeometryCollection m_HexGeo;
        private GeometryCollection m_AsciiGeo;
        private GlyphRun m_Run;
        private static Brush m_InvisiBrush;

        static HexEditorBlock()
        {
            m_InvisiBrush = new SolidColorBrush(Colors.Transparent);
            m_InvisiBrush.Freeze();
        }

        public HexEditorBlock(GeometryCollection hexGeos, GeometryCollection ascciGeos)
        {
            m_Run = HexEditor.CreateGlyphRun(new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), "0", FontSize, new Point(0, FontSize));

            Unloaded += HexEditorTextBlock_Unloaded;
            Loaded += HexEditorTextBlock_Loaded;
            Width = 50;
            Height = 50;
            m_HexGeo = hexGeos;
            m_AsciiGeo = ascciGeos;
        }

        private void HexEditorTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(m_InvisiBrush, null, new Rect(0, 0, ActualWidth, ActualHeight));

            if (BlockType == WPF.BlockType.Hex)
            {
                drawingContext.DrawGeometry(Foreground, null, m_HexGeo[ByteValue]);
            }
            else
            {
                drawingContext.DrawGeometry(Foreground, null, m_AsciiGeo[ByteValue]);
            }
        }

        public void SetEditorPosition(BlockType type, Int32 row, Int32 col, Dictionary<Int32, HexEditorBlock> lut)
        {
            try
            {
                m_Type = type;
                m_RowIndex = row;
                m_ColIndex = col;
                m_LUT = lut;
                m_BlockHash = GetBlockHash(row, col);

                if (m_LUT.ContainsKey(m_BlockHash))
                {
                    m_LUT.Remove(m_BlockHash);
                }

                m_LUT.Add(m_BlockHash, this);
            }
            catch
            {
                return;
            }
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            return arrangeBounds;
        }

        private void HexEditorTextBlock_Unloaded(object sender, RoutedEventArgs e)
        {
            if (m_LUT == null)
                return;

            m_LUT.Remove(m_BlockHash);
        }

        public Int32 RowIndex
        {
            get { return m_RowIndex; }
        }

        public Int32 ColIndex
        {
            get { return m_ColIndex; }
        }

        public BlockType BlockType
        {
            get { return m_Type; }
        }

        public Int32 BlockHash
        {
            get { return m_BlockHash; }
        }

        public Byte ByteValue
        {
            get;
            set;
        }

        private static Int32 GetBlockHash(Int32 row, Int32 col)
        {
            /* REF: http://stackoverflow.com/questions/682438/hash-function-providing-unique-uint-from-an-integer-coordinate-pair */
            return (col * 0x1f1f1f1f) ^ row;
        }

        public static HexEditorBlock GetBlockAt(Int32 row, Int32 col, Dictionary<Int32, HexEditorBlock> lut)
        {
            HexEditorBlock element = null;
            Int32 targetHash = GetBlockHash(row, col);
            lut.TryGetValue(targetHash, out element);
            return element;
        }
    }
}