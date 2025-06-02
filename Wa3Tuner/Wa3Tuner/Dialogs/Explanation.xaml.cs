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
    /// Interaction logic for Explanation.xaml
    /// </summary>
    public partial class Explanation : Window
    {
        public Explanation()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; }
        }
    }
}
