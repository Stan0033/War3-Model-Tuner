﻿using System;
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
    /// Interaction logic for Multiselector.xaml
    /// </summary>
    public partial class Multiselector_Window : Window
    {
        public List<string> selected = new();
        public List<int> selectedIndexes = new List<int>();
        public Multiselector_Window(List<string> items, string title = "Multiselector")
        {
            InitializeComponent();
            foreach (var item in items)
            {
                list.Items.Add( new ListBoxItem() { Content = item });
            }
            Title = title;
        }
        private void sall(object? sender, RoutedEventArgs? e)
        {
            list.SelectAll();
        }
        private void sone(object? sender, RoutedEventArgs? e)
        {
            list.SelectedItems.Clear();
        }
        private void reverse(object? sender, RoutedEventArgs? e)
        {
            List<object>selected = new List<object>();    
            foreach (object item in list.SelectedItems) { selected.Add(item); }
            list.SelectedItems.Clear();
            foreach (object item in selected)
            {
                 if (list.SelectedItems.Contains(item) == false) {  list.SelectedItems.Add(item); }
            }
        }
        private void ok(object? sender, RoutedEventArgs? e)
        {
            if (list.SelectedItems.Count >= 1)
            {
                foreach (object item in list.SelectedItems)
                {
                    string? s = Extractor.GetString(item);
                    if (s == null) continue;
                    selected.Add(s);
                    selectedIndexes.Add(list.Items.IndexOf(item));
                }
                DialogResult = true;
            }
        }
        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) ok(null, null);
        }
    }
}
