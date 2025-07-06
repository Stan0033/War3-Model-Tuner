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
using Wa3Tuner.Helper_Classes;
namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for Selector.xaml
    /// </summary>
    public partial class Selector : Window
    {
        internal string? Selected;
        internal List<string> SelectedList = new List<string>();
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
        private void Ok(object? sender, RoutedEventArgs? e)
        {
            if (box == null || box.SelectedItem == null)
                return;

            if (box.SelectedItem is ListBoxItem selectedItem && selectedItem.Content != null)
            {
                Selected = selectedItem.Content.ToString();
            }
            else
            {
                return; // Cannot proceed safely
            }

            DialogResult = true;

            if (box.SelectionMode == SelectionMode.Multiple && box.SelectedItems != null)
            {
                SelectedList.Clear();

                foreach (object? item in box.SelectedItems)
                {
                    if (item is ListBoxItem listItem && listItem.Content != null)
                    {
                      string? x=  Extractor.GetString(listItem);
                        if (x != null)
                        {
                            SelectedList.Add(x);
                        }
                    }
                }
            }
        }


        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) Ok(null,null);
        }
    }
}
