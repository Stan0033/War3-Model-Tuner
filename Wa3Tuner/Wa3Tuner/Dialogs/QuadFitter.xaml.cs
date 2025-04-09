using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using Wa3Tuner.Helper_Classes;

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for QuadFitter.xaml
    /// </summary>
    public partial class QuadFitter : Window
    {
        public QuadFitter()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; }
            if (e.Key == Key.Enter) { ok(null, null); }
        }


        private void ok(object sender, RoutedEventArgs e)
        {
            if (c_full.IsChecked == true)
            {
                QuadCollector.Fit();
            }
            else
            {
                var vectors = GetVectors();
                if (vectors == null)
                {
                    MessageBox.Show("Invalid inputs");return;
                }
                QuadCollector.FitCustom(
                    vectors[0].X,
                    vectors[0].Y,
                    vectors[1].X,
                    vectors[1].Y,
                     vectors[2].X,
                    vectors[2].Y,
                     vectors[3].X,
                    vectors[3].Y
                    );
            }
        }
        private Vector2[] GetVectors()
        {
            string[] inputs = { input_TR.Text, input_TL.Text, input_BR.Text, input_BL.Text };
            Vector2[] vectors = new Vector2[4];

            for (int i = 0; i < inputs.Length; i++)
            {
                string input = inputs[i];
                string[] parts = input.Split(',');

                // Check if input contains exactly two parts
                if (parts.Length != 2)
                {
                    return null;
                }

                // Try parsing the two parts as floats
                if (float.TryParse(parts[0], out float x) && float.TryParse(parts[1], out float y))
                {
                    // Check if the values are within the 0-1 range
                    if (x < 0 || x > 1 || y < 0 || y > 1)
                    {
                        return null;
                    }

                    vectors[i] = new Vector2(x, y);
                }
                else
                {
                    return null; // Invalid format
                }
            }

            return vectors;
        }

    }
}