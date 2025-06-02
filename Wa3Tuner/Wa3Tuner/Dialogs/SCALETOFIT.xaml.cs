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
    /// Interaction logic for SCALETOFIT.xaml
    /// </summary>
    public partial class SCALETOFIT : Window
    {
        internal CExtent Extent = new CExtent();
        public bool Each = false;
        public SCALETOFIT()
        {
            InitializeComponent();
        }
        private void ok(object? sender, RoutedEventArgs? e)
        {
            Each = SelectedGeosetsEach.IsChecked == true;
            bool parseminx = float.TryParse(MinXInput.Text, out float minx);
            bool parseminy = float.TryParse(MinYInput.Text, out float miny);
            bool parseminz = float.TryParse(MinZInput.Text, out float minz);
            bool parsedmaxx = float.TryParse(MaxXInput.Text, out float maxx);
            bool parsedmaxy = float.TryParse(MaxYInput.Text, out float maxy);
            bool parsedmaxz = float.TryParse(MaxZInput.Text, out float maxz);
            if (parsedmaxx && parsedmaxy && parsedmaxz && parsedmaxx && parsedmaxy && parsedmaxz)
            {
                Extent = new CExtent(new CVector3(minx, miny, minz), new CVector3(maxx, maxy, maxz), 0);
                DialogResult = true;
            }
        }
        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) ok(null, null);
        }
    }
}
