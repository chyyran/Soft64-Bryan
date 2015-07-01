using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Soft64WPF
{

    public partial class RegisterObserver : ContentControl
    {
        public static readonly DependencyProperty RegNameProperty =
            DependencyProperty.Register("RegName", typeof(String), typeof(RegisterObserver),
            new PropertyMetadata(""));

        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(String), typeof(RegisterObserver),
            new PropertyMetadata(""));

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Object), typeof(RegisterObserver),
            new PropertyMetadata(null));

        public RegisterObserver()
        {
            InitializeComponent();
            Loaded += RegisterObserver_Loaded;
        }

        void RegisterObserver_Loaded(object sender, RoutedEventArgs e)
        {
            xaml_RegNameBlock.Text = RegName;

            Binding binding = new Binding();
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.ValidatesOnDataErrors = true;
            binding.ValidationRules.Add(new DataErrorValidationRule());
            binding.Source = Source;
            binding.Path = new PropertyPath(Path);
            xaml_RegValueBox.SetBinding(TextBox.TextProperty, binding);
        }

        public String RegName
        {
            get { return (String)GetValue(RegNameProperty); }
            set { SetValue(RegNameProperty, value); }
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
    }
}
