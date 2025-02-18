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
    /// Interaction logic for SpreadVerticesIWindow.xaml
    /// </summary>
    public partial class SpreadVerticesIWindow : Window
    {
        public float Threshold;
        public float Distance;
        public SpreadVerticesIWindow()
        {
            InitializeComponent();
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            bool one = float.TryParse(inputT.Text, out float t);
            bool two = float.TryParse(inputD.Text, out float d);
            if (one && two)
            {
                Threshold = t;
                Distance = d;
                if (Distance > 0.5 && Threshold > 0)
                {
                    DialogResult = true;
                }

            }

        }
    }
}
