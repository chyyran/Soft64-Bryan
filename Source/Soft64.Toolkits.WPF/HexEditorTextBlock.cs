using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Soft64.Toolkits.WPF
{
    public sealed class HexEditorTextBlock : TextBlock
    {
        public static readonly DependencyProperty RowIndexProperty =
            DependencyProperty.Register("RowIndex", typeof(Int32), typeof(HexEditorTextBlock),
            new PropertyMetadata());

        public static readonly DependencyProperty BlockTypeProperty =
            DependencyProperty.Register("BlockType", typeof(BlockType), typeof(HexEditorTextBlock),
            new PropertyMetadata());

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(Int32), typeof(HexEditorTextBlock),
            new PropertyMetadata());

        public static readonly DependencyProperty ElementLUTProperty =
            DependencyProperty.Register("ElementLUT", typeof(Dictionary<Int32, HexEditorTextBlock>), typeof(HexEditorTextBlock),
            new PropertyMetadata());

        private Int32 m_BlockHash;

        static HexEditorTextBlock()
        {
            TextBlock.TextProperty.OverrideMetadata(typeof(HexEditorTextBlock), new FrameworkPropertyMetadata(
            (o, a) =>
            {
                HexEditorTextBlock b = o as HexEditorTextBlock;

                if (b != null)
                {
                    Storyboard sb = b.TryFindResource("ByteTextChangedAnimation") as Storyboard;

                    if (sb != null)
                    {
                        Storyboard.SetTarget(sb, b);
                        sb.Begin();
                    }
                }
            }));
        }

        public HexEditorTextBlock()
        {
            Loaded += HexEditorTextBlock_Loaded;
            Unloaded += HexEditorTextBlock_Unloaded;
        }

        private void HexEditorTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            if (ElementLUT == null)
                return;

            m_BlockHash = GetBlockHash(RowIndex, Index);

            var hashTable = ElementLUT;

            if (hashTable.ContainsKey(m_BlockHash))
            {
                hashTable.Remove(m_BlockHash);
            }

            hashTable.Add(m_BlockHash, this);
        }

        private void HexEditorTextBlock_Unloaded(object sender, RoutedEventArgs e)
        {
            if (ElementLUT == null)
                return;

            ElementLUT.Remove(m_BlockHash);
        }

        public Int32 RowIndex
        {
            get { return (Int32)GetValue(RowIndexProperty); }
            set { SetValue(RowIndexProperty, value); }
        }

        public Int32 Index
        {
            get { return (Int32)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        public BlockType BlockType
        {
            get { return (BlockType)GetValue(BlockTypeProperty); }
            set { SetValue(BlockTypeProperty, value); }
        }

        private static Int32 GetBlockHash(Int32 row, Int32 col)
        {
            /* REF: http://stackoverflow.com/questions/682438/hash-function-providing-unique-uint-from-an-integer-coordinate-pair */
            return (col * 0x1f1f1f1f) ^ row;
        }

        public static HexEditorTextBlock GetBlockAt(Int32 row, Int32 col, Dictionary<Int32, HexEditorTextBlock> lut)
        {
            HexEditorTextBlock element = null;
            Int32 targetHash = GetBlockHash(row, col);
            lut.TryGetValue(targetHash, out element);
            return element;
        }

        public Dictionary<Int32, HexEditorTextBlock> ElementLUT
        {
            get { return (Dictionary<Int32, HexEditorTextBlock>)GetValue(ElementLUTProperty); }
            set { SetValue(ElementLUTProperty, value); }
        }
    }
}