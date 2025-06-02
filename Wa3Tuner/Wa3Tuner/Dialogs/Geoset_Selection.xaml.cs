using MdxLib.Model;
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

namespace Wa3Tuner.Dialogs
{

    /// <summary>
    /// Interaction logic for Geoset_Selection.xaml
    /// </summary>

    public partial class Geoset_Selection : Window
    {
        CGeoset? geoset;
        private bool pause = true;
        public Geoset_Selection(CGeoset g)
        {
            InitializeComponent();
            geoset = g;
            if (g == null) { Close(); return; }
            Fill();
        }

        private void Fill()
        {
            InputSelectable.IsChecked = geoset.Unselectable;
            InputSelection.Text = geoset.SelectionGroup.ToString();
            pause = false;
        }

        private void SetUns(object sender, RoutedEventArgs e)
        {
            if (pause) return;
            if (geoset != null)
            {
                geoset.Unselectable = InputSelectable.IsChecked == true;
            }
        }

        private void InputSelection_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (pause) return;
            if (geoset != null)
            {
                if (int.TryParse(InputSelection.Text, out int value))
                {
                    if (value >= 0)
                    {
                        geoset.SelectionGroup = value;
                    }
                    else
                    {
                        geoset.SelectionGroup = 0;
                    }
                }
            }
        }
    }
}
