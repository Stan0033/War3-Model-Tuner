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
namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for FlattenSide.xaml
    /// </summary>
    /// 
   public enum Side
    {
        Top, Bottom, Left, Right, Front, Back
    }
    public partial class FlattenSide : Window
    {
        public Side side;
        public FlattenSide()
        {
            InitializeComponent();
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Options.Children.Count; i++)
            {
                if (Options.Children[i] is RadioButton button)
                {
                    if (button.IsChecked == true)
                    {
                        side = (Side)i; break;
                    }
                }
            }
            DialogResult = true;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) OkButton_Click(null, null);
        }
    }
}
