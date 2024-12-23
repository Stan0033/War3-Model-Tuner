using MdxLib.Primitives;
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
    /// Interaction logic for InputVector.xaml
    /// </summary>
    public partial class InputVector : Window
    {
        public float X, Y, Z = 0;
        private CVector3 pivotPoint;

        public InputVector(string title = "Vector")
        {
            InitializeComponent();
            Title = title;
        }
        public InputVector(CVector3 pivotPoint, string title = "Vector")
        {
            InitializeComponent();
            X = pivotPoint.X;
            Y = pivotPoint.Y;
            Z = pivotPoint.Z;
            x.Text = pivotPoint.X.ToString();
            y.Text = pivotPoint.Y.ToString();
            z.Text = pivotPoint.Z.ToString();
            Title = title;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        

        private void ok(object sender, RoutedEventArgs e)
        {
            bool parsed1 = float.TryParse(x.Text, out float val1);
            bool parsed2 = float.TryParse(y.Text, out float val2);
            bool parsed3 = float.TryParse(z.Text, out float val3);
            if (parsed1 && parsed2 && parsed3)
            {

                X = val1; Y = val2; Z = val3;
                DialogResult = true;
            }
        }
    }
}
