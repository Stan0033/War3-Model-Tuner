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
    /// Interaction logic for edittags.xaml
    /// </summary>
    public partial class edittags : Window
    {
        public edittags(MdxLib.Model.INode node)
        {
            InitializeComponent();

            Check_b.IsChecked = node.Billboarded;
            Check_bx.IsChecked = node.BillboardedLockX;
            Check_by.IsChecked = node.BillboardedLockY;
            Check_bz.IsChecked = node.BillboardedLockZ;
            Check_a.IsChecked = node.CameraAnchored;
            Check_d1.IsChecked = node.DontInheritTranslation;
            Check_d2.IsChecked = node.DontInheritRotation;
            Check_d3.IsChecked = node.DontInheritScaling;
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
