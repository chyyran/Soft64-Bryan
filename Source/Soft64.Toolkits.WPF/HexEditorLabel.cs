using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Soft64.Toolkits.WPF
{
    public sealed class HexEditorLabel : Label
    {
        private BlockType m_Type;
        private Int32 m_RowIndex;
        private Int32 m_ColIndex;
        private Dictionary<Int32, HexEditorLabel> m_LUT;
        private Int32 m_BlockHash;
        private String m_Text;

        static HexEditorLabel()
        {
        }

        public HexEditorLabel()
        {
            Unloaded += HexEditorTextBlock_Unloaded;
            Loaded += HexEditorTextBlock_Loaded;
        }

        private void HexEditorTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            this.Content = m_Text;
        }

        public void SetEditorPosition(BlockType type, Int32 row, Int32 col, Dictionary<Int32, HexEditorLabel> lut)
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

        public String BlockText
        {
            get { return m_Text; }
            set { m_Text = value; }
        }

        private static Int32 GetBlockHash(Int32 row, Int32 col)
        {
            /* REF: http://stackoverflow.com/questions/682438/hash-function-providing-unique-uint-from-an-integer-coordinate-pair */
            return (col * 0x1f1f1f1f) ^ row;
        }

        public static HexEditorLabel GetBlockAt(Int32 row, Int32 col, Dictionary<Int32, HexEditorLabel> lut)
        {
            HexEditorLabel element = null;
            Int32 targetHash = GetBlockHash(row, col);
            lut.TryGetValue(targetHash, out element);
            return element;
        }
    }
}