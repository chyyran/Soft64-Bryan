using System;
using System.IO;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;

namespace Soft64.Toolkits.WPF
{
    internal class HexEditorRow : DependencyObject, INotifyPropertyChanged
    {
        private List<Byte> m_RowBytes;
        private Int64 m_Address;
        private Int32 m_RowIndex;
        private StackPanel m_HexRootPanel;
        private StackPanel m_AsciiRootPanel;
        private List<HexEditorTextBlock> m_HexBlocks;
        private List<HexEditorTextBlock> m_AsciiBlocks;

        private static readonly HashSet<Char> s_PrintableHashSet = new HashSet<char>((
            from index in Enumerable.Range(0, 128)
            let ch = (Char)index
            where Char.IsLetterOrDigit(ch) || Char.IsSymbol(ch) || Char.IsPunctuation(ch)
            select ch));

        public HexEditorRow()
        {
            m_RowBytes = new List<Byte>();
            m_HexRootPanel = new StackPanel();
            m_HexRootPanel.Orientation = Orientation.Horizontal;
            m_HexRootPanel.Unloaded += m_HexRootPanel_Unloaded;
            m_HexRootPanel.Loaded += m_HexRootPanel_Loaded;
            m_AsciiRootPanel = new StackPanel();
            m_AsciiRootPanel.Orientation = Orientation.Horizontal;
            m_AsciiRootPanel.Unloaded += m_AsciiRootPanel_Unloaded;
            m_AsciiRootPanel.Loaded += m_AsciiRootPanel_Loaded;
            m_HexBlocks = new List<HexEditorTextBlock>();
            m_AsciiBlocks = new List<HexEditorTextBlock>();
        }

        void m_AsciiRootPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_AsciiRootPanel.Children.Count <= 0)
            {
                foreach (var child in m_AsciiBlocks)
                    m_AsciiRootPanel.Children.Add(child);
            }
        }

        void m_HexRootPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_HexRootPanel.Children.Count <= 0)
            {
                foreach (var child in m_HexBlocks)
                {
                    m_HexRootPanel.Children.Add(child);
                }
            }
        }

        void m_AsciiRootPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            m_AsciiRootPanel.Children.Clear();
        }

        void m_HexRootPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            m_HexRootPanel.Children.Clear();
        }

        public void SetBytes(List<HexEditorTextBlock> blockCache, Byte[] bytes, Dictionary<Int32, HexEditorTextBlock> hexLUT, Dictionary<Int32, HexEditorTextBlock> asciiLUT)
        {
            for(Int32 i = 0; i < bytes.Length; i++)
            {
                m_RowBytes.Add(bytes[i]);
                Int32 index = i + (bytes.Length * RowIndex * 2) + (i * 1);

                HexEditorTextBlock hexBlock = blockCache[index];
                hexBlock.SetEditorPosition(BlockType.Hex, RowIndex, i, hexLUT);
                hexBlock.BlockText = bytes[i].ToString("X2");

                HexEditorTextBlock asciiBlock = blockCache[index + 1];
                asciiBlock.SetEditorPosition(BlockType.Ascii, RowIndex, i, asciiLUT);
                asciiBlock.BlockText = new String(GetAscii(bytes[i]), 1);

                m_HexBlocks.Add(hexBlock);
                m_AsciiBlocks.Add(asciiBlock);
            }
        }

        private Char GetAscii(Byte value)
        {
            if (s_PrintableHashSet.Contains((char)value))
                return (char)value;
            else
                return '.';
        }

        public Int64 Address
        {
            get { return m_Address; }
            set { m_Address = value; }
        }

        public IList<Byte> Bytes
        {
            get { return m_RowBytes; }
        }

        public Int32 RowIndex
        {
            get { return m_RowIndex; }
            set { m_RowIndex = value; }
        }

        public void WriteByte(Stream source, Int32 byteOffset, Byte b)
        {
            Bytes[byteOffset] = b;
            source.Position = Address + byteOffset;
            source.WriteByte(b);
            m_AsciiBlocks[byteOffset].Text = new String(GetAscii(b), 1);
            m_HexBlocks[byteOffset].Text = b.ToString("X2");
        }

        public Byte GetByteValue(Int32 offset)
        {
            return m_RowBytes[offset];
        }

        public Panel HexContent
        {
            get {  return m_HexRootPanel;  }
        }

        public Panel AsciiContent
        {
            get  { return m_AsciiRootPanel;  }
        }

        internal void UpdateText()
        {
            foreach (var h in m_HexBlocks)
                h.Text = h.BlockText;

            foreach (var a in m_AsciiBlocks)
                a.Text = a.BlockText;

            var e = PropertyChanged;

            if (e != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Address"));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}