using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Soft64WPF
{
    public sealed class RegisterObserver : FrameworkElement
    {
        public static readonly DependencyProperty RegNameProperty =
            DependencyProperty.Register("RegName", typeof(String), typeof(RegisterObserver),
            new PropertyMetadata(""));

        private static readonly DependencyPropertyKey TargetBindingPropertyKey =
            DependencyProperty.RegisterReadOnly("TargetBinding", typeof(Binding), typeof(RegisterObserver),
            new PropertyMetadata());

        public static readonly DependencyProperty TargetBindingProperty =
            TargetBindingPropertyKey.DependencyProperty;

        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(String), typeof(RegisterObserver),
            new PropertyMetadata(""));

        public RegisterObserver()
        {
            Binding binding = new Binding();
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.ValidatesOnDataErrors = true;
            binding.ValidationRules.Add(new DataErrorValidationRule());
            binding.Source = DataContext;
        }

        private static void PathChange(DependencyObject o, DependencyPropertyChangedEventArgs a)
        {
            if (o != null)
            {
                RegisterObserver observer = o as RegisterObserver;

                if (observer != null)
                {
                    observer.TargetBinding.Path = new PropertyPath(a.NewValue as String);
                }
            }
        }

        public String RegName
        {
            get { return (String)GetValue(RegNameProperty); }
            set { SetValue(RegNameProperty, value); }
        }

        public Binding TargetBinding
        {
            get { return (Binding)GetValue(TargetBindingProperty); }
            private set { SetValue(TargetBindingPropertyKey, value); }
        }

        public String Path
        {
            get { return (String)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }
    }
}
