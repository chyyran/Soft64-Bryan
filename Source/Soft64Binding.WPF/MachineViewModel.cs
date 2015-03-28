using System.Windows;
using Soft64;

namespace Soft64Binding.WPF
{
    public sealed class MachineViewModel : DependencyObject
    {
        private static readonly DependencyPropertyKey CurrentMachinePropertyKey =
            DependencyProperty.RegisterReadOnly("CurrentMachine", typeof(Machine), typeof(MachineViewModel),
            new PropertyMetadata());

        private static readonly DependencyPropertyKey MachineRunStatePropertyKey =
            DependencyProperty.RegisterReadOnly("MachineRunState", typeof(LifetimeState), typeof(MachineViewModel),
            new PropertyMetadata());

        private static readonly DependencyPropertyKey CartridgePropertyKey =
            DependencyProperty.RegisterReadOnly("Cartridge", typeof(CartridgeViewModel), typeof(MachineViewModel),
            new PropertyMetadata());

        private static readonly DependencyPropertyKey RcpPropertyKey =
            DependencyProperty.RegisterReadOnly("Rcp", typeof(RcpViewModel), typeof(MachineViewModel),
            new PropertyMetadata());

        private static readonly DependencyPropertyKey CpuPropertyKey =
            DependencyProperty.RegisterReadOnly("Cpu", typeof(CpuViewModel), typeof(MachineViewModel),
            new PropertyMetadata());

        /* TODO List:
         *  PIF View Model
         *  CPU View Model
         */

        public static readonly DependencyProperty CurrentMachineProperty =
            CurrentMachinePropertyKey.DependencyProperty;

        public static readonly DependencyProperty MachineRunStateProperty =
            MachineRunStatePropertyKey.DependencyProperty;

        public static readonly DependencyProperty CartridgeProperty =
            CartridgePropertyKey.DependencyProperty;

        public static readonly DependencyProperty RcpProperty =
            RcpPropertyKey.DependencyProperty;

        public static readonly DependencyProperty CpuProperty =
            CpuPropertyKey.DependencyProperty;

        public MachineViewModel()
        {
            /* Important: Prevent crashes if the machine hasn't been created yet */
            if (Machine.Current == null)
                return;

            WeakEventManager<Machine, LifeStateChangedArgs>
                .AddHandler(Machine.Current, "RuntimeStateChanged", RuntimeStateChangedHandler);

            SetValue(CurrentMachinePropertyKey, Machine.Current);
            SetValue(CartridgePropertyKey, new CartridgeViewModel(this));
            SetValue(MachineRunStatePropertyKey, Machine.Current.CurrentRuntimeState);
            SetValue(RcpPropertyKey, new RcpViewModel(this));
            SetValue(CpuPropertyKey, new CpuViewModel(this));
        }

        private void RuntimeStateChangedHandler(object sender, LifeStateChangedArgs e)
        {
            SetValue(MachineRunStatePropertyKey, e.NewState);
        }

        public Machine TargetMachine
        {
            get { return (Machine)GetValue(CurrentMachineProperty); }
        }

        public LifetimeState MachineRunState
        {
            get { return (LifetimeState)GetValue(MachineRunStateProperty); }
        }

        public CartridgeViewModel Cartridge
        {
            get { return (CartridgeViewModel)GetValue(CartridgeProperty); }
        }

        public RcpViewModel Rcp
        {
            get { return (RcpViewModel)GetValue(RcpProperty); }
        }

        public CpuViewModel Cpu
        {
            get { return (CpuViewModel)GetValue(CpuProperty); }
        }
    }
}