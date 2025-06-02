using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using W3_Texture_Finder;
using Wa3Tuner.Helper_Classes;
namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for ModelBrowser.xaml
    /// </summary>
    public partial class ModelBrowser : Window
    {
        public string? Selected = "";
        public ModelBrowser()
        {
            InitializeComponent();
            RefillList();
        }
        private void Ok(object? sender, RoutedEventArgs? e)
        {
            if (Data.SelectedItem != null)
            {
                Selected =Extractor.GetString(Data.SelectedItem);
                DialogResult = true;
            }
        }
        private void RefillList()
        {
            foreach (string item in MPQHelper.Listfile_Models)
            {
                Data.Items.Add(new ListBoxItem() { Content = item});
            }
        }
        private void Input_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string quiry = Input.Text.Trim().ToLower();
                if (quiry.Length > 0)
                {
                    Data.Items.Clear();
                    foreach (string item in MPQHelper.Listfile_Models)
                    {
                        if (item.ToLower().Contains(quiry))
                        {
                            Data.Items.Add(new ListBoxItem() { Content = item });
                        }
                    }
                }
                else
                {
                    RefillList();
                }
            }
        }

        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) Ok(null, null);
        }
    }
}
