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
    public enum AllowedValue
    {
        Negative, Positive, Both
    }
    /// <summary>
    /// Interaction logic for InputVector.xaml
    /// </summary>

    public partial class InputVector : Window
    {
        AllowedValue allowedValue;
        public float X, Y, Z = 0;
        
        public InputVector(AllowedValue allowed, string title = "Vector")
        {
            InitializeComponent();
            Title = title;
            allowedValue = allowed;
        }
        public InputVector(AllowedValue allowed,   CVector3 pivotPoint, string title = "Vector")
        {
            InitializeComponent();
            X = pivotPoint.X;
            Y = pivotPoint.Y;
            Z = pivotPoint.Z;
            x.Text = pivotPoint.X.ToString();
            y.Text = pivotPoint.Y.ToString();
            z.Text = pivotPoint.Z.ToString();
            Title = title;
            allowedValue = allowed;
        }
        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) ok(null,null);
        }
        private void Window_Loaded(object? sender, RoutedEventArgs? e)
        {
        }
        private void ok(object? sender, RoutedEventArgs? e)
        {
            bool parsed1 = float.TryParse(x.Text, out float val1);
            bool parsed2 = float.TryParse(y.Text, out float val2);
            bool parsed3 = float.TryParse(z.Text, out float val3);
            if (parsed1 && parsed2 && parsed3)
            {
                X = val1; Y = val2; Z = val3;
                if (allowedValue == AllowedValue.Both)
                {
                    DialogResult = true;
                }
                else if (allowedValue == AllowedValue.Positive)
                {
                    if (X < 0 || Y < 0 || Z < 0)
                    {
                        MessageBox.Show("Expected positive values"); return;
                    }
                    DialogResult = true;
                }
                else if (allowedValue == AllowedValue.Negative)
                {
                    if (X > 0 || Y > 0 || Z > 0)
                    {
                        MessageBox.Show("Expected negative values"); return;
                    }
                    DialogResult = true;
                }
            }
        }
    }
}
