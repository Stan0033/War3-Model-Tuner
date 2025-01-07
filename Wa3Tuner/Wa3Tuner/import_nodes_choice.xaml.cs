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
    /// Interaction logic for import_nodes_choice.xaml
    /// </summary>
    public partial class import_nodes_choice : Window
    {
        public int selected = 1;
        public import_nodes_choice()
        {
            InitializeComponent();
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            if (two.IsChecked == true) { selected = 2; }
            if (three.IsChecked == true) { selected = 3; }
            if (four.IsChecked == true) { selected = 4; }
            if (five.IsChecked == true) { selected = 5; }
            DialogResult = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
        }
    }
}
