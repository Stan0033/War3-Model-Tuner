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

namespace Wa3Tuner.Dialogs
{
    
    /// <summary>
    /// Interaction logic for axis_picker.xaml
    /// </summary>
    public partial class axis_picker : Window
    {

        public Axes axis;
        public axis_picker(string title)
        {
            InitializeComponent();
            Title = title;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; }
            if (e.Key == Key.Enter) ok(null, null);
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            if (Check_x.IsChecked == true) { axis = Axes.X; }
            if (Check_y.IsChecked == true) { axis = Axes.Y; }
            if (Check_z.IsChecked == true) { axis = Axes.Z; }
            DialogResult = true;    
        }
    }
}
