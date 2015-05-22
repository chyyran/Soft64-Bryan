using System;
using System.Windows;

namespace Soft64.Toolkits.WPF
{
    internal class HexEditorRowViewModel : DependencyObject
    {
        public static readonly DependencyProperty AddressProperty =
            DependencyProperty.Register("Address", typeof(Int64), typeof(HexEditorRowViewModel),
            new PropertyMetadata());

        public static readonly DependencyProperty RowIndexProperty =
            DependencyProperty.Register("RowIndex", typeof(Int32), typeof(HexEditorRowViewModel),
            new PropertyMetadata());

        public HexEditorRowViewModel()
        {
            Bytes = new ObservableIndexByteCollection();
        }

        public Int64 Address
        {
            get { return (Int64)GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        public ObservableIndexByteCollection Bytes { get; set; }

        public Int32 RowIndex
        {
            get { return (Int32)GetValue(RowIndexProperty); }
            set { SetValue(RowIndexProperty, value); }
        }
    }
}