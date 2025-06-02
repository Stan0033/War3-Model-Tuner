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
using Wa3Tuner.Helper_Classes;

namespace Wa3Tuner.Dialogs
{

    /// <summary>
    /// Interaction logic for SnapSettingsWindow.xaml
    /// </summary>
    public partial class SnapSettingsWindow : Window
    {
        public SnapType Type;
        public SnapSettingsWindow()
        {
            InitializeComponent();
        }

        private void ok(object? sender, RoutedEventArgs? e)
        {
            if (SnapToNearestRadio.IsChecked == true) { Type = SnapType.Nearest; }
            else if (radioSnappTopFrontLeft.IsChecked == true) { Type = SnapType.TopFrontLeft; }
            else if (radioSnappTopFrontRight.IsChecked == true) { Type = SnapType.TopFrontRight; }
            else if (radioSnappTopBackLeft.IsChecked == true) { Type = SnapType.TopBackLeft; }
            else if (radioSnappTopBackRight.IsChecked == true) { Type = SnapType.TopBackRight; }
            else if (radioSnappBottomFrontLeft.IsChecked == true) { Type = SnapType.BottomFrontLeft; }
            else if (radioSnappBottomFrontRight.IsChecked == true) { Type = SnapType.BottomFrontRight; }
            else if (radioSnappBottomBackLeft.IsChecked == true) { Type = SnapType.BottomBackLeft; }
            else if (radioSnappBottomBackRight.IsChecked == true) { Type = SnapType.BottomBackRight; }
            DialogResult = true;
        }


        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; }
            if (e.Key == Key.Enter) { ok(null, null); }
        }
    }
}
