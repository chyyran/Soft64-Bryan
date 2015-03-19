using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

namespace Soft64.Toolkits.WPF
{
    internal sealed class ObservableIndexByteCollection : ObservableCollection<IndexedByte>
    {
        private Int32 m_RowLength = 0;

        public ObservableIndexByteCollection()
        {
        }

        private void UpdateDummyBytes(Int32 newLength, Int32 oldLength)
        {
            if (newLength == oldLength)
                return;

            if (newLength >= oldLength)
            {
                for (Int32 i = 0; i < newLength - oldLength; i++)
                {
                    Add(new IndexedByte { ByteValue = 0, Index = Count });
                }
            }
            else
            {
                for (Int32 i = 0; i < oldLength - newLength; i++)
                {
                    Int32 index = Count - 1 - i;

                    if (index > 0)
                        RemoveAt(index);
                }
            }
        }

        public void UpdateRowSize(Int32 size)
        {
            UpdateDummyBytes(size, m_RowLength);
            m_RowLength = size;
        }
    }
}