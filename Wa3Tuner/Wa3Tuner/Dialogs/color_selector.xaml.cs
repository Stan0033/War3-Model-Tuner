 
using System.Windows;
 
using System.Windows.Input;
using System.Windows.Media;
 
namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for color_selector.xaml
    /// </summary>
    public partial class color_selector : Window
    {
        public Color SelectedColor = Colors.White;
        public Brush SelectedBrush = Brushes.White;
       
       
        bool initialized = false;
        public color_selector(Brush brush)
        {
            InitializeComponent();
            initialized = true;
            Color color = Calculator.BrushToColor(brush);
            RedSlider.Value = color.R;
            GreenSlider.Value = color.G;
            BlueSlider.Value = color.B;
        }
        private void OnColorChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
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
            DisplayR.Text = red.ToString();
            DisplayG.Text = green.ToString();
            DisplayB.Text = blue.ToString();
        }
        private void OkButton_Click(object? sender, RoutedEventArgs? e)
        {
            DialogResult = true;
            Close();
        }
        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; }
        }
    }
}
