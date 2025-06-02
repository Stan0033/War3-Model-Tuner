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
    /// Interaction logic for translation_selector.xaml
    /// </summary>
    public partial class transformation_selector : Window
    {
        public transformation_selector()
        {
            InitializeComponent();
        }
        private void ok(object? sender, RoutedEventArgs? e)
        {
            if (C1.IsChecked == false && C2.IsChecked == false && C3.IsChecked == false) {
                return;
            }
            DialogResult = true;
        }
        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) ok(null, null);
        }
    }
}
