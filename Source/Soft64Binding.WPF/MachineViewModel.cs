using System;
using System.Windows;
using Soft64;

namespace Soft64Binding.WPF
{
    public sealed class MachineViewModel : DependencyObject
    {
        private static readonly DependencyPropertyKey CurrentMachinePropertyKey =
            DependencyProperty.RegisterReadOnly("CurrentMachine", typeof(Machine), typeof(MachineViewModel),
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

        private static readonly DependencyPropertyKey EnginePropertyKey =
            DependencyProperty.RegisterReadOnly("Engine", typeof(EmulatorEngineViewModel), typeof(MachineViewModel),
            new PropertyMetadata());

        public static readonly DependencyProperty CurrentMachineProperty =
            CurrentMachinePropertyKey.DependencyProperty;

        public static readonly DependencyProperty CartridgeProperty =
            CartridgePropertyKey.DependencyProperty;

        public static readonly DependencyProperty RcpProperty =
            RcpPropertyKey.DependencyProperty;

        public static readonly DependencyProperty CpuProperty =
            CpuPropertyKey.DependencyProperty;

        public static readonly DependencyProperty EngineProperty =
            EnginePropertyKey.DependencyProperty;

        public MachineViewModel()
        {
            /* Important: Prevent crashes if the machine hasn't been created yet */
            if (Machine.Current == null)
                return;

            SetValue(CurrentMachinePropertyKey, Machine.Current);
            SetValue(CartridgePropertyKey, new CartridgeViewModel(this));
            SetValue(RcpPropertyKey, new RcpViewModel(this));
            SetValue(CpuPropertyKey, new CpuViewModel(this));
            SetValue(EnginePropertyKey, new EmulatorEngineViewModel(this));
        }

        public Machine TargetMachine
        {
            get { return (Machine)GetValue(CurrentMachineProperty); }
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

        public EmulatorEngineViewModel Engine
        {
            get { return (EmulatorEngineViewModel)GetValue(EngineProperty); }
        }

        public event EventHandler<MachineEventNotificationArgs> MachineEventNotification
        {
            add
            {
                WeakEventManager<Machine, MachineEventNotificationArgs>.AddHandler(TargetMachine,  "MachineEventNotification", value);
            }

            remove
            {
                WeakEventManager<Machine, MachineEventNotificationArgs>.RemoveHandler(TargetMachine, "MachineEventNotification", value);
            }
        }
    }
}