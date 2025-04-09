using System.IO;
 
using System.Windows;
 
using System.Windows.Input;
 

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for bigTextContaienr.xaml
    /// </summary>
    public partial class bigTextContainer : Window
    {
        public bigTextContainer(string file)
        {
            InitializeComponent();
            box.Text = File.ReadAllText(file);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; }
        }
    }
}
