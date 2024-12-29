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
    /// Interaction logic for color_selector.xaml
    /// </summary>
    public partial class color_selector : Window
    {
        public Color SelectedColor = Colors.White;
        public Brush SelectedBrush = Brushes.White;
        CModel Model;
        CGeoset Geoset;
        bool initialized = false;
        public color_selector( )
        {
            InitializeComponent();
            
            initialized = true;
        }
        public color_selector(Brush brush)
        {
            InitializeComponent();

            initialized = true;
            Color color = Calculator.BrushToColor(brush);
            RedSlider.Value = color.R;
            GreenSlider.Value = color.G;
            BlueSlider.Value = color.B;
        }
        private void OnColorChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!initialized) { return; }
            UpdateColorCanvas();
        }

        private void UpdateColorCanvas()
        {
            if (!initialized) { return; }
            byte red = (byte)RedSlider.Value;
            byte green = (byte)GreenSlider.Value;
            byte blue = (byte)BlueSlider.Value;

            SelectedColor = Color.FromRgb(red, green, blue);
            SelectedBrush = new SolidColorBrush(SelectedColor);
            ColorCanvas.Background = new SolidColorBrush(SelectedColor);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
