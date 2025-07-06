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
using static Wa3Tuner.Helper_Classes.History;

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for GeosetExtents.xaml
    /// </summary>
    public partial class GeosetExtents : Window
    {
        CGeoset CurrentGeoset;
        List<CSequence> Sequences;
        CModel Model;
        public GeosetExtents(CGeoset geoset, List<CSequence> sequences, CModel m)
        {
            InitializeComponent();
            CurrentGeoset = geoset;
            Sequences = sequences;
            Model   = m;
            RefreshList();
        }
        private void RefreshList()
        {
            ListExtents.Items.Clear();
            if (CurrentGeoset.Extents.Count == 0) { return; }
            for (int i = 0; i < CurrentGeoset.Extents.Count; i++)
            {
                if (i >= Sequences.Count) { break; }
                var item = new ListBoxItem();
                item.Content = Sequences[i].Name;
                ListExtents.Items.Add(item);
            }
        }
        private void ListExtents_MouseDoubleClick(object? sender, MouseButtonEventArgs e)
        {
            Edit_Extent ee = new Edit_Extent(CurrentGeoset.Extents[ListExtents.SelectedIndex].Extent);
            ee.ShowDialog();
        }

        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }

        private void Moveup(object sender, RoutedEventArgs e)
        {
            if (ListExtents.SelectedItem==null) return; 
            int index = ListExtents.SelectedIndex;
            MoveElement(CurrentGeoset.Extents.ObjectList, index, true);
            RefreshList();
        }

        private void Movedown(object sender, RoutedEventArgs e)
        {
            if (ListExtents.SelectedItem == null) return;
            int index = ListExtents.SelectedIndex;
            MoveElement(CurrentGeoset.Extents.ObjectList, index, false);
            RefreshList();
        }
        public static void MoveElement<T>(List<T> list, int index, bool up)
        {
            if (list == null || list.Count == 0 || index < 0 || index >= list.Count)
                return;

            int newIndex = up ? index - 1 : index + 1;

            if (newIndex < 0 || newIndex >= list.Count)
                return;

            // Swap the elements
            T temp = list[index];
            list[index] = list[newIndex];
            list[newIndex] = temp;
        }

        private void Duplicate(object sender, RoutedEventArgs e)
        {
            if (CurrentGeoset.Extents.Count >= Sequences.Count) {
                MessageBox.Show("Cannot have more extens than sequences");return;
            }
            if (ListExtents.SelectedItem != null)
            {
                int index = ListExtents.SelectedIndex;
                var extent = CurrentGeoset.Extents[index].Extent;
                CGeosetExtent gs = new CGeosetExtent(Model);

                gs.Extent = new MdxLib.Primitives.CExtent(extent);
                CurrentGeoset.Extents.Add(gs);
                RefreshList();

            }
        }

        private void del(object sender, RoutedEventArgs e)
        {
            if (ListExtents.SelectedItem != null)
            {
                int index = ListExtents.SelectedIndex;
                
                CurrentGeoset.Extents.RemoveAt(index);
                RefreshList();
            }
        }

        private void clikc_Editextent(object sender, RoutedEventArgs e)
        {
            ListExtents_MouseDoubleClick(null, null);
        }
    }
}
