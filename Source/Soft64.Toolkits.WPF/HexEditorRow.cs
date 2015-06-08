using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Soft64.Toolkits.WPF
{
    internal class HexEditorRow : DependencyObject, INotifyPropertyChanged
    {
        private List<Byte> m_RowBytes;
        private Int64 m_Address;
        private Int32 m_RowIndex;
        private StackPanel m_HexRootPanel;
        private StackPanel m_AsciiRootPanel;
        private List<HexEditorBlock> m_HexBlocks;
        private List<HexEditorBlock> m_AsciiBlocks;
        private Delegate m_UpdateTextEvent;
        private Int32 m_UsageCounter = 0;

        private static readonly HashSet<Char> s_PrintableHashSet = new HashSet<char>((
            from index in Enumerable.Range(0, 128)
            let ch = (Char)index
            where Char.IsLetterOrDigit(ch) || Char.IsSymbol(ch) || Char.IsPunctuation(ch)
            select ch));

        private static readonly String[] s_PrintableHexTable;
        private static readonly String[] s_PrintableAsciiTable;

        static HexEditorRow()
        {
            s_PrintableHexTable = new String[256];
            s_PrintableAsciiTable = new String[256];

            for(Int32 i = 0; i < 256; i++)
            {
                s_PrintableHexTable[i] = i.ToString("X2");
                s_PrintableAsciiTable[i] = new String(GetAscii(Convert.ToByte(i)), 1);
            }
        }

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
            m_HexBlocks = new List<HexEditorBlock>();
            m_AsciiBlocks = new List<HexEditorBlock>();
            m_UpdateTextEvent = new Action<Size, Size>((a, b) => { });
        }

        private void AddChildren()
        {
            if (m_AsciiRootPanel.Children.Count <= 0)
            {
                foreach (var child in m_AsciiBlocks)
                    m_AsciiRootPanel.Children.Add(child);
            }

            if (m_HexRootPanel.Children.Count <= 0)
            {
                foreach (var child in m_HexBlocks)
                {
                    m_HexRootPanel.Children.Add(child);
                }
            }
        }

        private void m_HexRootPanel_Loaded(object sender, RoutedEventArgs e)
        {
            AddChildren();
        }

        private void m_AsciiRootPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            m_AsciiRootPanel.Children.Clear();
        }

        private void m_HexRootPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            m_HexRootPanel.Children.Clear();
        }

        public void SetBytes(List<HexEditorBlock> blockCache, Byte[] bytes, Dictionary<Int32, HexEditorBlock> hexLUT, Dictionary<Int32, HexEditorBlock> asciiLUT)
        {
            for (Int32 i = 0; i < bytes.Length; i++)
            {
                m_RowBytes.Add(bytes[i]);
                Int32 index = i + (bytes.Length * RowIndex * 2) + (i * 1);

                HexEditorBlock hexBlock = blockCache[index];
                hexBlock.SetEditorPosition(BlockType.Hex, RowIndex, i, hexLUT);
                hexBlock.ByteValue = bytes[i];

                HexEditorBlock asciiBlock = blockCache[index + 1];
                asciiBlock.SetEditorPosition(BlockType.Ascii, RowIndex, i, asciiLUT);
                asciiBlock.ByteValue = bytes[i];

                if (m_UsageCounter > bytes.Length)
                {
                    Int32 diff = m_UsageCounter - bytes.Length;
                    m_UsageCounter -= diff;
                    m_HexBlocks.RemoveRange((m_HexBlocks.Count - diff) - 1, diff);
                    m_AsciiBlocks.RemoveRange((m_AsciiBlocks.Count - diff) - 1, diff);
                }

                if (m_UsageCounter < bytes.Length)
                {
                    m_HexBlocks.Add(hexBlock);
                    m_AsciiBlocks.Add(asciiBlock);
                    m_UsageCounter++;
                }
            }
        }

        internal static Char GetAscii(Byte value)
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
            m_AsciiBlocks[byteOffset].ByteValue = b;
            m_HexBlocks[byteOffset].ByteValue = b;
        }

        public Byte GetByteValue(Int32 offset)
        {
            return m_RowBytes[offset];
        }

        public Panel HexContent
        {
            get { return m_HexRootPanel; }
        }

        public Panel AsciiContent
        {
            get { return m_AsciiRootPanel; }
        }

        internal void UpdateText(Size hSize, Size aSize)
        {
            for (Int32 i = 0; i < m_UsageCounter; i++)
            {
                m_HexBlocks[i].Width = hSize.Width;
                m_HexBlocks[i].Height = hSize.Height;
                m_AsciiBlocks[i].Width = aSize.Width;
                m_AsciiBlocks[i].Height = aSize.Height;
                m_HexBlocks[i].InvalidateVisual();
                m_AsciiBlocks[i].InvalidateVisual();
            }

            var e = PropertyChanged;

            if (e != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Address"));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members
    }
}