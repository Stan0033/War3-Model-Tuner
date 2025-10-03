 
using System.Text;
 
using System.Windows;
 
using System.Windows.Input;
 

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for about_w.xaml
    /// </summary>
    public partial class about_w : Window
    {
        public about_w()
        {
            InitializeComponent();
            StringBuilder sb = new StringBuilder("War3ModelTuner v1.3.4 (10/October/2025) by stan0033 built using C#, .NET 5.0, Visual Studio 2022.");
            sb.AppendLine("Credits:");
            sb.AppendLine("Magos' MDXLib - read/write MDL/MDX, format 800");
            sb.AppendLine("War3NET libraries by Drake53 - read/write MPQ/BLP");
            sb.AppendLine("Color picker for WPF by dsafa");
            sb.AppendLine("SharpGL by Dave Kerr - for rendering");
            // MessageBox.Show(sb.ToString());
            Data.Text = sb.ToString();
        }

        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }
    }
}
