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
using static Wa3Tuner.Calculator;

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for Extent_Orientation_Selector.xaml
    /// </summary>
    public partial class Extent_Orientation_Selector : Window
    {
        public ExtentPosition Position = ExtentPosition.Center;
        public Extent_Orientation_Selector()
        {
            InitializeComponent();
        }

        private void ok(object? sender, RoutedEventArgs? e)
        {
            if (RadioTop.IsChecked == true) { Position = ExtentPosition.Top; }
            else if (RadioBottom.IsChecked == true) { Position = ExtentPosition.Bottom; }
            else if (RadioLeft.IsChecked == true) { Position = ExtentPosition.Left; }
            else if (RadioRight.IsChecked == true) { Position = ExtentPosition.Right; }
            else if (RadioFront.IsChecked == true) { Position = ExtentPosition.Front; }
            else if (RadioBack.IsChecked == true) { Position = ExtentPosition.Back; }
            else if (RadioTopLeft.IsChecked == true) { Position = ExtentPosition.TopLeft; }
            else if (RadioTopRight.IsChecked == true) { Position = ExtentPosition.TopRight; }
            else if (RadioBottomLeft.IsChecked == true) { Position = ExtentPosition.BottomLeft; }
            else if (RadioBottomRight.IsChecked == true) { Position = ExtentPosition.BottomRight; }
            else if (RadioTopFront.IsChecked == true) { Position = ExtentPosition.TopFront; }
            else if (RadioTopBack.IsChecked == true) { Position = ExtentPosition.TopBack; }
            else if (RadioBottomFront.IsChecked == true) { Position = ExtentPosition.BottomFront; }
            else if (RadioBottomBack.IsChecked == true) { Position = ExtentPosition.BottomBack; }
            else if (RadioLeftFront.IsChecked == true) { Position = ExtentPosition.LeftFront; }
            else if (RadioLeftBack.IsChecked == true) { Position = ExtentPosition.LeftBack; }
            else if (RadioRightFront.IsChecked == true) { Position = ExtentPosition.RightFront; }
            else if (RadioRightBack.IsChecked == true) { Position = ExtentPosition.RightBack; }
            else if (RadioTopLeftFront.IsChecked == true) { Position = ExtentPosition.TopLeftFront; }
            else if (RadioTopLeftBack.IsChecked == true) { Position = ExtentPosition.TopLeftBack; }
            else if (RadioTopRightFront.IsChecked == true) { Position = ExtentPosition.TopRightFront; }
            else if (RadioTopRightBack.IsChecked == true) { Position = ExtentPosition.TopRightBack; }
            else if (RadioBottomLeftFront.IsChecked == true) { Position = ExtentPosition.BottomLeftFront; }
            else if (RadioBottomLeftBack.IsChecked == true) { Position = ExtentPosition.BottomLeftBack; }
            else if (RadioBottomRightFront.IsChecked == true) { Position = ExtentPosition.BottomRightFront; }
            else if (RadioBottomRightBack.IsChecked == true) { Position = ExtentPosition.BottomRightBack; }
            else if (RadioCenter.IsChecked == true) { Position = ExtentPosition.Center; }

            DialogResult = true;

        }
    }
}
