using MdxLib.Model;
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
    /// Interaction logic for EditGS_W.xaml
    /// </summary>
    public partial class EditGS_W : Window
    {
        CModel Model;
        public EditGS_W(CModel model)
        {
            InitializeComponent();
            Model = model;
            RefreshList();
        }

        private void RefreshList()
        {
            ListGS.Items.Clear();
            foreach (CGlobalSequence gs in Model.GlobalSequences) {
                ListGS.Items.Add($"{gs.ObjectId}: {gs.Duration}");
            }
        }
        private void add(object sender, RoutedEventArgs e)
        {
            bool parse = int.TryParse(box.Text, out int value);
            if (parse)
            {
                Model.GlobalSequences.Add(new CGlobalSequence(Model) { Duration = value });
                RefreshList();
            }
        }

        private void ListGS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListGS.SelectedItem != null) {
                box.Text = Model.GlobalSequences[ListGS.SelectedIndex].Duration.ToString();
            }
           
        }

        private void del(object sender, RoutedEventArgs e)
        {
            if (ListGS.SelectedItem != null)
            {
                Model.GlobalSequences.RemoveAt(ListGS.SelectedIndex);
                 RefreshList();
            }
        }

        private void edit(object sender, RoutedEventArgs e)
        {
            if (ListGS.SelectedItem != null)
            {
                bool parse = int.TryParse(box.Text, out int value);
                if (parse)
                {
                    Model.GlobalSequences[ListGS.SelectedIndex].Duration = value;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)  DialogResult = false;
        }
    }
}
