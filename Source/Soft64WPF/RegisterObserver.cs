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
    public sealed class RegisterObserver : DependencyObject
    {
        private TextBox m_RegisterBox;

        public static readonly DependencyProperty RegNameProperty =
            DependencyProperty.Register("RegName", typeof(String), typeof(RegisterObserver),
            new PropertyMetadata(""));

        private static readonly DependencyPropertyKey TargetBindingPropertyKey =
            DependencyProperty.RegisterReadOnly("TargetBinding", typeof(Binding), typeof(RegisterObserver),
            new PropertyMetadata(new Binding()));

        public static readonly DependencyProperty TargetBindingProperty =
            TargetBindingPropertyKey.DependencyProperty;

        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(String), typeof(RegisterObserver),
            new PropertyMetadata("", PathChange));

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Object), typeof(RegisterObserver),
            new PropertyMetadata(null, SourceChange));

        public RegisterObserver()
        {
            TargetBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            TargetBinding.ValidatesOnDataErrors = true;
            TargetBinding.ValidationRules.Add(new DataErrorValidationRule());
        }

        private static void PathChange(DependencyObject o, DependencyPropertyChangedEventArgs a)
        {
            if (o != null)
            {
                RegisterObserver observer = o as RegisterObserver;

                if (observer != null)
                {
                    observer.TargetBinding.Path = new PropertyPath(a.NewValue as String);
                    observer.InvalidateProperty(RegisterObserver.TargetBindingProperty);
                }
            }
        }

        private static void SourceChange(DependencyObject o, DependencyPropertyChangedEventArgs a)
        {
            if (o != null)
            {
                RegisterObserver observer = o as RegisterObserver;

                if (observer != null)
                {
                    observer.TargetBinding.Source = a.NewValue;
                    observer.InvalidateProperty(RegisterObserver.TargetBindingProperty);
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

        public Object Source
        {
            get { return (Object)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public FrameworkElement TextBoxElement
        {
            get 
            
            {
                m_RegisterBox = new TextBox();
                m_RegisterBox.SetBinding(TextBox.TextProperty, TargetBinding);
                return m_RegisterBox;
            }
        }
    }
}
