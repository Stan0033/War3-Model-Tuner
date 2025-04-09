using MdxLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for Create_Sphere.xaml
    /// </summary>
    public partial class Create_Sphere : Window
    {
        public int Slices, Section;
        public Create_Sphere( )
        {
            InitializeComponent();
            
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            bool s = int.TryParse(txtSections.Text, out int slices);
            bool c = int.TryParse(txtSections.Text, out int section);
            if (s && c)
            {
                if (slices < 3) { MessageBox.Show("Input for either has to be between 3 and 50"); return; }
           
                    Slices = slices;
                Section = section;
                DialogResult = true;
            }
            else { MessageBox.Show("Expected integers");return; }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { ok(null, null); }
            if (e.Key == Key.Escape) { DialogResult = false; }
        }
    }
}
