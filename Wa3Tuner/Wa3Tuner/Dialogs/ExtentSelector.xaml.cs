using MdxLib.Model;
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

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for ExtentSelector.xaml
    /// </summary>
    public partial class ExtentSelector : Window
    {
        private INode? Node;
        private CModel Model;
        private CVector3? Vector;
        private CExtent? Extent;
        public ExtentSelector(CModel model, CVector3 vector)
        {
            InitializeComponent();
            Model = model;
            Vector = vector;
            if (model.Geosets.Count == 0) { m2.IsEnabled = false; m3.IsEnabled = false; }
            else
            {
                foreach (var geoset in model.Geosets)
                {
                    list.Items.Add($"{geoset.ObjectId} {geoset.Vertices.Count} vertices, {geoset.Triangles.Count} triangles");
                }
                list.SelectedIndex = 0;
            }
            if (model.Sequences.Count == 0) { m4.IsEnabled = false; }
            else
            {
                foreach (var sequence in model.Sequences)
                {
                    list.Items.Add($"{sequence.Name} [{sequence.IntervalStart} - {sequence.IntervalEnd}]");
                }
                list.SelectedIndex = 0;
            }
            m5.IsEnabled = false;
        }
        public ExtentSelector(CModel model, INode WhichNode)
        {
            InitializeComponent();
            Model = model;
            Node = WhichNode;
            Vector = Node.PivotPoint;
            Extent = Calculator.GetExtentFromAttachedVertices(Model, WhichNode);
        }

        private void ok(object? sender, RoutedEventArgs? e)
        {

            GetSelectedExtent();
            if (Extent == null) { return; }
            Extent_Orientation_Selector eos = new Extent_Orientation_Selector();
            if (eos.ShowDialog() == true)
            {
                if (Vector == null) { return; }
                Calculator.CenterVectorAtExtent(Vector, Extent, eos.Position);
            }
        }

        private void GetSelectedExtent()
        {
           if (m1.IsChecked == true) { Extent = Model.Extent; }
           if (m2.IsChecked == true) { Extent = Model.Geosets[list.SelectedIndex].Extent; }
           if (m3.IsChecked == true) 
            {
                var geoset = Model.Geosets[list.SelectedIndex];
                if (list2.Items.Count > 0)
                {
                    Extent = geoset.Extents[list2.SelectedIndex].Extent;
                }
                else
                {
                    Extent = null;
                }
            }
           if (m4.IsChecked == true) { Extent = Model.Sequences[list.SelectedIndex].Extent; }
        }

        private void list_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (m3.IsChecked == true)
            {
                list2.Items.Clear();
                var geoset = Model.Geosets[list.SelectedIndex];
                if (list2.Items.Count > 0)
                {
                    foreach (var extent in geoset.Extents)
                    {
                        list2.Items.Add(new ListBoxItem() { Content = $"{extent.ObjectId}" });
                    }
                }
            }
        }

        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; }
        }
    }
}
