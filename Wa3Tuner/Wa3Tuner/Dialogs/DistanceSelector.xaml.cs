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
    public enum DistaningMethod
    {
        Set, Add, Subtract,
        Multiply,
        Divide,
        Modulo
    }
    /// <summary>
    /// Interaction logic for DistanceSelector.xaml
    /// </summary>
    ///
    public partial class DistanceSelector : Window
    {
        public DistaningMethod Method;
      public  float X;
        public float Y;
        public float Z;
        public float Distance;
        public DistanceSelector()
        {
            InitializeComponent();
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            bool h_x = float.TryParse(InputX.Text, out float x);
            bool h_y = float.TryParse(InputY.Text, out float y);
            bool h_z = float.TryParse(InputZ.Text, out float z);
            bool h_d = float.TryParse(InputDistance.Text, out float d);

            if (RadioSet.IsChecked == true) Method = DistaningMethod.Set;
            if (Radiop.IsChecked == true) Method = DistaningMethod.Add;
            if (Radiom.IsChecked == true) Method = DistaningMethod.Subtract;
            if (RadioMul.IsChecked == true) Method = DistaningMethod.Multiply;
            if (RadioDiv.IsChecked == true) Method = DistaningMethod.Divide;
            if (RadioMod.IsChecked == true) Method = DistaningMethod.Modulo;
            if (h_x && h_y && h_z && h_d)
            {
                X = x;
                Y = y;
                Z = z;
                Distance = d;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) ok(null, null);
        }
    }
}
