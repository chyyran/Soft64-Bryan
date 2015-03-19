using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soft64.MipsR4300;
using Soft64.MipsR4300.CP0;

namespace Soft64Binding.WPF
{
    public sealed class TlbCacheViewModel : MachineComponentViewModel
    {
        public TlbCacheViewModel(MachineViewModel parentMachineModel) : base(parentMachineModel)
        {
            Refresh();
        }

        public void Refresh()
        {
            ReadRegs();
            ReadEntries();
        }

        private void ReadRegs()
        {
            TLBCache cache = ParentMachine.TargetMachine.CPU.VirtualMemoryStream.TLB;

        }

        private void ReadEntries()
        {
            TlbEntries.Clear();

            TLBCache cache = ParentMachine.TargetMachine.CPU.VirtualMemoryStream.TLB;


        }

        private static void UpdateRegister(DependencyObject o, DependencyPropertyChangedEventArgs a)
        {
            TlbCacheViewModel model = o as TlbCacheViewModel;

            if (model != null)
            {
                TLBCache cache = model.ParentMachine.TargetMachine.CPU.VirtualMemoryStream.TLB;

                switch (a.Property.Name)
                {
                    case "PageMask": cache.PageMask = (UInt64)a.NewValue; break;
                    case "EntryHi": cache.EntryHi = (UInt64)a.NewValue; break;
                    case "EntryLo0": cache.EntryLo0 = (UInt64)a.NewValue; break;
                    case "EntryLo1": cache.EntryLo1 = (UInt64)a.NewValue; break;
                    case "Index": cache.Index = (UInt64)a.NewValue; break;
                    default: break;
                }
            }
        }

        private static readonly DependencyPropertyKey TlbEntriesPropertyKey =
        DependencyProperty.RegisterReadOnly("TlbEntries", typeof(ObservableCollection<TlbEntry>), typeof(TlbCacheViewModel),
        new PropertyMetadata(new ObservableCollection<TlbEntry>()));

        public static readonly DependencyProperty TlbEntriesProperty =
            TlbEntriesPropertyKey.DependencyProperty;

        public static readonly DependencyProperty PageMaskProperty =
            DependencyProperty.Register("PageMask", typeof(UInt64), typeof(TlbCacheViewModel),
            new PropertyMetadata(UpdateRegister));

        public static readonly DependencyProperty EntryHiProperty =
            DependencyProperty.Register("EntryHi", typeof(UInt64), typeof(TlbCacheViewModel),
            new PropertyMetadata(UpdateRegister));

        public static readonly DependencyProperty EntryLo0Property =
            DependencyProperty.Register("EntryLo0", typeof(UInt64), typeof(TlbCacheViewModel),
            new PropertyMetadata(UpdateRegister));

        public static readonly DependencyProperty EntryLo1Property =
            DependencyProperty.Register("EntryLo1", typeof(UInt64), typeof(TlbCacheViewModel),
            new PropertyMetadata(UpdateRegister));

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(UInt64), typeof(TlbCacheViewModel),
            new PropertyMetadata(UpdateRegister));

        public ObservableCollection<TlbEntry> TlbEntries
        {
            get { return (ObservableCollection<TlbEntry>)GetValue(TlbEntriesProperty); }
            private set { SetValue(TlbEntriesPropertyKey, value); }
        }

        public UInt64 PageMask
        {
            get { return (UInt64)GetValue(PageMaskProperty); }
            set { SetValue(PageMaskProperty, value); }
        }

        public UInt64 EntryHi
        {
            get { return (UInt64)GetValue(EntryHiProperty); }
            set { SetValue(EntryHiProperty, value); }
        }

        public UInt64 EntryLo0
        {
            get { return (UInt64)GetValue(EntryLo0Property); }
            set { SetValue(EntryLo0Property, value); }
        }

        public UInt64 EntryLo1
        {
            get { return (UInt64)GetValue(EntryLo1Property); }
            set { SetValue(EntryLo1Property, value); }
        }

        public UInt64 Index
        {
            get { return (UInt64)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }
    }

    public sealed class TlbEntry
    {
        private Int32 m_EntryIndex;
    }
}
