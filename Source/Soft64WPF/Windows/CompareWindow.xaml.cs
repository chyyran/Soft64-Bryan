using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CompareEngineMupen;
using Soft64;

namespace Soft64WPF.Windows
{
    /// <summary>
    /// Interaction logic for CompareWindow.xaml
    /// </summary>
    public partial class CompareWindow : Window
    {
        public CompareWindow()
        {
            InitializeComponent();
        }

        private void xaml_ChkBoxMupen_Checked(object sender, RoutedEventArgs e)
        {
            MupenEngine engine = new MupenEngine();
            Machine.Current.MipsCompareEngine = engine;
        }
    }
}
