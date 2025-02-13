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
        internal List<string> SelectedList;
        public Selector(List<string> ids, string title = "Selector", bool Multiselect = false)
        {
            InitializeComponent();
            foreach (string id in ids)
            {
                box.Items.Add(new ListBoxItem() { Content = id });
            }
            Title = title;
            if (Multiselect)
            {
                box.SelectionMode = SelectionMode.Multiple;
            }
            }
        private void ok(object sender, RoutedEventArgs e)
        {
            if (box.SelectedItem != null)
            {
                Selected = (box.SelectedItem as ListBoxItem).Content.ToString();
                DialogResult = true;
                if (box.SelectionMode == SelectionMode.Multiple)
                {
                    foreach (object id in box.SelectedItems)
                    {
                        SelectedList.Add((id as ListBoxItem).Content.ToString());
                    }
                }
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
        }
    }
}
