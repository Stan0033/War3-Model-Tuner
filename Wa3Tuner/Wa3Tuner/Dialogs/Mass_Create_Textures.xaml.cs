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
    /// Interaction logic for Mass_Create_Textures.xaml
    /// </summary>
    public partial class Mass_Create_Textures : Window
    {
        public List<string> Paths = new List<string>();
        public Mass_Create_Textures()
        {
            InitializeComponent();
        }
        private void ok(object sender, RoutedEventArgs e)
        {
            Paths = box.Text.Split('\n').ToList();
            for (int i = 0; i < Paths.Count; i++)
            {
                Paths[i] = Paths[i].Trim();
            }
            DialogResult = true;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) ok(null, null);
        }
    }
}
