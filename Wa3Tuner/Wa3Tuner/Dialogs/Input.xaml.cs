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
    /// Interaction logic for Input.xaml
    /// </summary>
    public partial class Input : Window
    {
        public string Result;
        public Input(string original)
        {
            InitializeComponent();
            box.Text = original;
        }
        public Input(string original, string title)
        {
            InitializeComponent();
            box.Text = original;
            Title = title;
        }
        private void ok(object sender, RoutedEventArgs e)
        {
            string name = box.Text.Trim();
            if (name.Length == 0 ) { return; }
            Result = name;
            DialogResult = true;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) ok(null,null);
        }
    }
}
