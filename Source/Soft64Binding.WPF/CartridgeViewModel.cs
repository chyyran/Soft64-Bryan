/*
Soft64 - C# N64 Emulator
Copyright (C) Soft64 Project @ Codeplex
Copyright (C) 2013 - 2014 Bryan Perris

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

using System;
using System.Windows;
using Soft64;
using Soft64.DeviceMemory;

namespace Soft64Binding.WPF
{
    public class CartridgeViewModel : MachineComponentViewModel
    {
        internal CartridgeViewModel(MachineViewModel currentModel) : base(currentModel)
        {
            Machine machine = (Machine)currentModel.TargetMachine;

            WeakEventManager<ParallelInterface, CartridgeChangedEventArgs>
                .AddHandler(machine.RCP.DevicePI, "CartridgeChanged", DevicePI_CartridgeChanged);
        }

        private void DevicePI_CartridgeChanged(object sender, CartridgeChangedEventArgs e)
        {
            if (e.NewCartridge != null)
            {
                SetValue(NamePropertyKey, e.NewCartridge.ToString());
                SetValue(HeaderPropertyKey, e.NewCartridge.GetCartridgeInfo());
                SetValue(IsCICSkippedPropertyKey, e.NewCartridge.LockoutKey == null);
                SetValue(IsCartridgeInsertedKey, true);
            }
            else
            {
                ClearValue(NamePropertyKey);
                ClearValue(HeaderPropertyKey);
                ClearValue(IsCICSkippedPropertyKey);
                ClearValue(IsCartridgeInsertedKey);
            }
        }

        private static readonly DependencyPropertyKey NamePropertyKey =
            DependencyProperty.RegisterReadOnly("Name", typeof(String), typeof(CartridgeViewModel),
            new PropertyMetadata("<Slot Empty>"));

        private static readonly DependencyPropertyKey HeaderPropertyKey =
            DependencyProperty.RegisterReadOnly("Header", typeof(CartridgeInfo), typeof(CartridgeViewModel),
            new PropertyMetadata());

        private static readonly DependencyPropertyKey IsCICSkippedPropertyKey =
            DependencyProperty.RegisterReadOnly("IsCICSkipped", typeof(Boolean), typeof(CartridgeViewModel),
            new PropertyMetadata());

        private static readonly DependencyPropertyKey IsCartridgeInsertedKey =
            DependencyProperty.RegisterReadOnly("IsCartridgeInserted", typeof(Boolean), typeof(CartridgeViewModel),
            new PropertyMetadata());

        public static readonly DependencyProperty NameProperty =
            NamePropertyKey.DependencyProperty;

        public static readonly DependencyProperty HeaderProperty =
            HeaderPropertyKey.DependencyProperty;

        public static readonly DependencyProperty IsCICSkippedProperty =
            IsCICSkippedPropertyKey.DependencyProperty;

        public static readonly DependencyProperty IsCartridgeInsertedProperty =
            IsCartridgeInsertedKey.DependencyProperty;

        public String Name
        {
            get { return (String)GetValue(NameProperty); }
            internal set { SetValue(NamePropertyKey, value); }
        }

        public CartridgeInfo Header
        {
            get { return (CartridgeInfo)GetValue(HeaderProperty); }
            internal set { SetValue(HeaderPropertyKey, value); }
        }

        public Boolean IsCICSkipped
        {
            get { return (Boolean)GetValue(IsCICSkippedProperty); }
            internal set { SetValue(IsCICSkippedPropertyKey, value); }
        }

        public Boolean IsCartridgeInserted
        {
            get { return (Boolean)GetValue(IsCartridgeInsertedProperty); }
            internal set { SetValue(IsCartridgeInsertedKey, value); }
        }
    }
}