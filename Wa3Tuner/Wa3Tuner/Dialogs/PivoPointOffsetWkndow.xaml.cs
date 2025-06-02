using MdxLib.Primitives;
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
    /// Interaction logic for PivoPointOffsetWkndow.xaml
    /// </summary>
    public partial class PivoPointOffsetWkndow : Window
    {
        public CVector3 Point = new CVector3();
        public float Angle = 0;
        public float Distance = 0;
        public Axes   axes = Axes.None;
        public PivoPointOffsetWkndow()
        {
            InitializeComponent();
        }

        private void ok(object? sender, RoutedEventArgs? e)
        {
            bool one = float.TryParse(XInput.Text, out float x);
            bool two = float.TryParse(YInput.Text, out float y);
            bool tr = float.TryParse(ZInput.Text, out float z);
            bool ds = float.TryParse(DistanceInput.Text, out float distance);
            bool an = float.TryParse(AngleInput.Text, out float angle); 
            if (one && two && tr && ds && an)
            {
                Point = new CVector3(x, y, z);
                Distance = distance;
                Angle = angle;
                if (Distance <= 0) { return; }
                if (Angle <0 || Angle > 360) { return; }
                axes = (Axes)AxisSelector.SelectedIndex;
                DialogResult = true;
            }
           

        }
    }
}
