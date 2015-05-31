using System;
using System.IO;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Soft64.Toolkits.WPF
{
    internal class HexEditorRow
    {
        private ObservableCollection<IndexedByte> m_RowBytes;
        private Int64 m_Address;
        private Int32 m_RowIndex;

        public HexEditorRow()
        {
            m_RowBytes = new ObservableCollection<IndexedByte>();
        }

        public void SetBytes(Byte[] bytes)
        {
            m_RowBytes.Clear();

            for(Int32 i = 0; i <bytes.Length; i++)
            {
                m_RowBytes.Add(new IndexedByte() { ByteValue = bytes[i], Index = i });
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
        }

        public Byte GetByteValue(Int32 offset)
        {
            return m_RowBytes[offset].ByteValue;
        }
    }
}