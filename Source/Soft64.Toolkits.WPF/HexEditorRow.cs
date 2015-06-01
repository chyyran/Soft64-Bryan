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
    internal class HexEditorRow : DependencyObject
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
            m_AsciiRootPanel = new StackPanel();
            m_AsciiRootPanel.Orientation = Orientation.Horizontal;
            m_HexBlocks = new List<HexEditorTextBlock>();
            m_AsciiBlocks = new List<HexEditorTextBlock>();
        }

        public void SetBytes(Byte[] bytes, Dictionary<Int32, HexEditorTextBlock> hexLUT, Dictionary<Int32, HexEditorTextBlock> asciiLUT)
        {
            m_RowBytes.Clear();
            m_HexRootPanel.Children.Clear();
            m_AsciiRootPanel.Children.Clear();
            m_HexBlocks.Clear();
            m_AsciiBlocks.Clear();

            for(Int32 i = 0; i < bytes.Length; i++)
            {
                m_RowBytes.Add(bytes[i]);

                HexEditorTextBlock hexBlock = new HexEditorTextBlock();
                hexBlock.SetEditorPosition(BlockType.Hex, RowIndex, i, hexLUT);
                hexBlock.Text = bytes[i].ToString("X2");
                hexBlock.Margin = new Thickness(2, 0, 0, 0);

                HexEditorTextBlock asciiBlock = new HexEditorTextBlock();
                asciiBlock.SetEditorPosition(BlockType.Ascii, RowIndex, i, asciiLUT);
                asciiBlock.Text = new String(GetAscii(bytes[i]), 1);
                asciiBlock.Margin = new Thickness(2, 0, 0, 0);

                m_HexBlocks.Add(hexBlock);
                m_HexRootPanel.Children.Add(hexBlock);
                m_AsciiRootPanel.Children.Add(asciiBlock);
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
            m_AsciiBlocks[byteOffset].Text = new String((char)b, 1);
            m_HexBlocks[byteOffset].Text = b.ToString("X2");
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
    }
}