using System;
using System.IO;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Soft64.Toolkits.WPF
{
    internal class HexEditorRow : DependencyObject
    {
        private ObservableCollection<IndexedByte> m_RowBytes;
        private Int64 m_Address;
        private Int32 m_RowIndex;
        private StackPanel m_HexRootPanel;
        private StackPanel m_AsciiRootPanel;
        private List<HexEditorTextBlock> m_HexBlocks;
        private List<HexEditorTextBlock> m_AsciiBlocks;

        public HexEditorRow()
        {
            m_RowBytes = new ObservableCollection<IndexedByte>();
            m_HexRootPanel = new StackPanel();
            m_HexRootPanel.Orientation = Orientation.Horizontal;
            m_AsciiRootPanel = new StackPanel();
            m_AsciiRootPanel.Orientation = Orientation.Horizontal;
            m_HexBlocks = new List<HexEditorTextBlock>();
            m_AsciiBlocks = new List<HexEditorTextBlock>();
        }

        public void SetBytes(Byte[] bytes)
        {
            m_RowBytes.Clear();
            m_HexRootPanel.Children.Clear();
            m_AsciiRootPanel.Children.Clear();
            m_HexBlocks.Clear();
            m_AsciiBlocks.Clear();

            for(Int32 i = 0; i <bytes.Length; i++)
            {
                m_RowBytes.Add(new IndexedByte() { ByteValue = bytes[i], Index = i });

                HexEditorTextBlock hexBlock = new HexEditorTextBlock();
                hexBlock.Index = i;
                hexBlock.BlockType = BlockType.Hex;
                hexBlock.RowIndex = RowIndex;
                hexBlock.Text = bytes[i].ToString("X2");
                hexBlock.Margin = new Thickness(2, 0, 0, 0);

                HexEditorTextBlock asciiBlock = new HexEditorTextBlock();
                asciiBlock.Index = i;
                asciiBlock.BlockType = BlockType.Ascii;
                asciiBlock.RowIndex = RowIndex;
                asciiBlock.Text = new String((char)bytes[i], 1);
                asciiBlock.Margin = new Thickness(2, 0, 0, 0);

                m_HexBlocks.Add(hexBlock);
                m_HexRootPanel.Children.Add(hexBlock);
                m_AsciiRootPanel.Children.Add(asciiBlock);
                m_AsciiBlocks.Add(asciiBlock);
            }
        }

        public Int64 Address
        {
            get { return m_Address; }
            set { m_Address = value; }
        }

        public ObservableCollection<IndexedByte> Bytes
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
            Bytes[byteOffset] = new IndexedByte() { Index = byteOffset, ByteValue = b };
            source.Position = Address + byteOffset;
            source.WriteByte(b);
            m_AsciiBlocks[byteOffset].Text = new String((char)b, 1);
            m_HexBlocks[byteOffset].Text = b.ToString("X2");
        }

        public Byte GetByteValue(Int32 offset)
        {
            return m_RowBytes[offset].ByteValue;
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