using MdxLib.Model;
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
    /// Interaction logic for window_editbone_data.xaml
    /// </summary>
    public partial class window_editbone_data : Window
    {
        CBone Bone;
        CModel Model;
        public window_editbone_data(INode node, CModel model)
        {
            InitializeComponent();
            Bone = (CBone)node;
            Model = model;
            List_g.Items.Add(new ListBoxItem() { Content = "None"});
            List_ga.Items.Add(new ListBoxItem() { Content = "None"});
            List_g.SelectedIndex = 0;
            List_ga.SelectedIndex = 0;
            foreach (CGeoset geo in Model.Geosets)
            {
                List_g.Items.Add(new ListBoxItem() { Content ="Geoset "+ geo.ObjectId.ToString()});
            }
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {
                List_ga.Items.Add(new ListBoxItem() { Content = "Geoset Animation" + ga.ObjectId.ToString() });
            }
            SelectData();
        }
        private void SelectData()
        {
            for (int i = 0; i < Model.Geosets.Count; i++)
            {
                if (Bone.Geoset.Object == Model.Geosets[i])
                {
                    List_g.SelectedIndex = i+1;
                }
            }
            for (int i = 0; i < Model.GeosetAnimations.Count; i++)
            {
                if (Bone.GeosetAnimation.Object == Model.GeosetAnimations[i])
                {
                    List_ga.SelectedIndex = i + 1;
                }
            }
        }
        private void SelectedG(object? sender, SelectionChangedEventArgs e)
        {
            if (List_g.SelectedItem != null)
            {
                int index = List_g.SelectedIndex;
                if (index == 0) { Bone.Geoset.Detach(); return; }
                Bone.Geoset.Attach(Model.Geosets[index+1]);
            }
        }
        private void SelectedGA(object? sender, SelectionChangedEventArgs e)
        {
            if (List_ga.SelectedItem != null)
            {
                int index = List_ga.SelectedIndex;
                if (index == 0) { Bone.GeosetAnimation.Detach(); return; }
                Bone.GeosetAnimation.Attach(Model.GeosetAnimations[index+1]);
            }
        }
        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
        }
    }
}
