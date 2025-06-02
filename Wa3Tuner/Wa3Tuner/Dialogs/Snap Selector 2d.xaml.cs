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
using Wa3Tuner.Helper_Classes;

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for Snap_Selector_2d.xaml
    /// </summary>
    public partial class Snap_Selector_2d : Window
    {
        public SnapType2D Type;
        public Snap_Selector_2d()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)  DialogResult = false;
            if (e.Key == Key.Enter)  ok(null, null);


        }

        private void ok(object? sender, RoutedEventArgs? e)
        {
           if (r_n.IsChecked == true) Type = SnapType2D.Nearest;
           else if (r_tr.IsChecked == true) Type = SnapType2D.TopRight;
           else if (r_tl.IsChecked == true) Type = SnapType2D.TopLeft;
           else if (r_br.IsChecked == true) Type = SnapType2D.BottomRight;
           else if (r_bl.IsChecked == true) Type = SnapType2D.BottomLeft;

           DialogResult = true; 
        }
    }
}
