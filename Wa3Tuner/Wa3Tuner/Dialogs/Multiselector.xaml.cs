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
    /// Interaction logic for Multiselector.xaml
    /// </summary>
    public partial class Multiselector : Window
    {
        public List<string> selected = new List<string>();
        public Multiselector(List<string> items)
        {
            InitializeComponent();
            foreach (var item in items)
            {
                list.Items.Add( new ListBoxItem() { Content = item });
            }
        }
        private void sall(object sender, RoutedEventArgs e)
        {
            list.SelectAll();
        }
        private void sone(object sender, RoutedEventArgs e)
        {
            list.SelectedItems.Clear();
        }
        private void reverse(object sender, RoutedEventArgs e)
        {
            List<object>selected = new List<object>();    
            foreach (object item in list.SelectedItems) { selected.Add(item); }
            list.SelectedItems.Clear();
            foreach (object item in selected)
            {
                 if (list.SelectedItems.Contains(item) == false) {  list.SelectedItems.Add(item); }
            }
        }
        private void ok(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItems.Count >= 2)
            {
                foreach (object item in list.SelectedItems)
                {
                    string s = (item as ListBoxItem).Content.ToString();
                    selected.Add(s);
                }
                DialogResult = true;
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
        }
    }
}
