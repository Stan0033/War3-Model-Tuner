using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for axis_selector.xaml
    /// </summary>
    public partial class axis_selector : Window
    {
        public bool x, y, z;
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; }
        }
        public axis_selector()
        {
            InitializeComponent();
        }
        private void ok(object sender, RoutedEventArgs e)
        {
            x = Check_x.IsChecked == true;
            y = Check_y.IsChecked == true;
            z = Check_z.IsChecked == true;
            DialogResult = true;
        }
    }
}
