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

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp.Common
{
    /// <summary>
    /// Interaction logic for GenericWindowDialog.xaml
    /// </summary>
    public partial class GenericWindowDialog : Window
    {
        public GenericWindowDialog(GenericWindowDialogViewModel viewmodel)
        {
            InitializeComponent();
            this.DataContext = viewmodel;
        }

        private void button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
