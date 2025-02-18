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
    /// Interaction logic for GeosetExtents.xaml
    /// </summary>
    public partial class GeosetExtents : Window
    {
        CGeoset CurrentGeoset;
        public GeosetExtents(CGeoset geoset, List<CSequence> sequences)
        {
            InitializeComponent();
            CurrentGeoset = geoset;
            for (int i = 0; i < geoset.Extents.Count; i++)
            {
                var item = new ListBoxItem();
                item.Content = sequences[i].Name;
                ListExtents.Items.Add(item);
            }
        }

        private void ListExtents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Edit_Extent ee = new Edit_Extent(CurrentGeoset.Extents[ListExtents.SelectedIndex].Extent);
            ee.ShowDialog();
        }
    }
}
