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
        public axis_selector()
        {
            InitializeComponent();
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
