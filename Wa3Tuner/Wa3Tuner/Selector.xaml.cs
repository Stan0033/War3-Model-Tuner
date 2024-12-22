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
    /// Interaction logic for Selector.xaml
    /// </summary>
    public partial class Selector : Window
    {
        internal string Selected;

        public Selector(List<string> ids, string title = "Selector")
        {
            InitializeComponent();
            foreach (string id in ids)
            {
                box.Items.Add(new ListBoxItem() { Content = id });
            }
            Title = title;
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            if (box.SelectedItem != null)
            {
                Selected = (box.SelectedItem as ListBoxItem).Content.ToString();
                DialogResult = true;
            }
        }
    }
}
