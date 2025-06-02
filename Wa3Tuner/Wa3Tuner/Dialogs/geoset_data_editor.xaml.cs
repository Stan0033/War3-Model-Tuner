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
    /// Interaction logic for geoset_data_editor.xaml
    /// </summary>
    public partial class geoset_data_editor : Window
    {
        CGeoset geoset;
        private bool Pause = true;
        public geoset_data_editor(CGeoset g)
        {
            InitializeComponent();
            geoset = g;
            if (g == null) return;
            Fill();
        }

        private void Fill()
        {
            foreach (var vertex in geoset.Vertices)
            {
                ListBoxItem item = new ListBoxItem();
               
                ListBoxItem item2 = new ListBoxItem();
                ListBoxItem item3 = new ListBoxItem();
                ListBoxItem item4 = new ListBoxItem();
                item.Content = $"{vertex.ObjectId}";
                item3.Content = $"{vertex.ObjectId}";
                item2.Content = $"{vertex.ObjectId}";
                item4.Content = $"{vertex.ObjectId}";
               
               
                ListVertices.Items.Add(vertex);
                Selector_Vertex1.Items.Add(item2);
                Selector_Vertex2.Items.Add(item3);
                Selector_Vertex3.Items.Add(item4);
            }
            foreach (var triangle in geoset.Triangles)
            {
                ListBoxItem item5 = new ListBoxItem();
                item5.Content = $"{triangle.ObjectId}";
                ListTriangles.Items.Add(triangle);  
            }
            
            Pause = false;
        }

        private void ListVertices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Pause) return;
            if (ListVertices.SelectedItem != null)
            {
                Pause = true;
                var vertex = geoset.Vertices[ListVertices.SelectedIndex];
                InputX.Text = vertex.Position.X.ToString();
                InputY.Text = vertex.Position.Y.ToString();
                InputZ.Text = vertex.Position.Z.ToString();
                InputXn.Text = vertex.Normal.X.ToString();
                InputYn.Text = vertex.Normal.Y.ToString();
                InputZn.Text = vertex.Normal.Z.ToString();
                InputXt.Text = vertex.TexturePosition.X.ToString();
                InputYt.Text = vertex.TexturePosition.Y.ToString();
                Pause = false;


            }
        }

        private void ListTriangles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Pause) return;
            if (ListTriangles.SelectedItem != null)
            {
                Pause   = true;
                var triangle = geoset.Triangles[ListTriangles.SelectedIndex];
                Selector_Vertex1.SelectedIndex = geoset.Vertices.IndexOf(triangle.Vertex1.Object);
                Selector_Vertex2.SelectedIndex = geoset.Vertices.IndexOf(triangle.Vertex2.Object);
                Selector_Vertex3.SelectedIndex = geoset.Vertices.IndexOf(triangle.Vertex3.Object);
                Pause = false;
            }

        }

        private void InputX_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ListVertices.SelectedItem != null)
            {
                var vertex = geoset.Vertices[ListVertices.SelectedIndex];
                if (float.TryParse(InputX.Text, out float value))
                {
                    vertex.Position.X = value;
                }
            }
        }

        private void InputY_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ListVertices.SelectedItem != null)
            {
                var vertex = geoset.Vertices[ListVertices.SelectedIndex];
                if (float.TryParse(InputY.Text, out float value))
                {
                    vertex.Position.Y = value;
                }
            }
        }

        private void InputZ_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ListVertices.SelectedItem != null)
            {
                var vertex = geoset.Vertices[ListVertices.SelectedIndex];
                if (float.TryParse(InputZ.Text, out float value))
                {
                    vertex.Position.Z = value;
                }
            }
        }

        private void InputXn_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ListVertices.SelectedItem != null)
            {
                var vertex = geoset.Vertices[ListVertices.SelectedIndex];
                if (float.TryParse(InputXn.Text, out float value))
                {
                    vertex.Normal.X = value;
                }
            }
        }

        private void InputYn_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ListVertices.SelectedItem != null)
            {
                var vertex = geoset.Vertices[ListVertices.SelectedIndex];
                if (float.TryParse(InputYn.Text, out float value))
                {
                    vertex.Normal.Y = value;
                }
            }
        }

        private void InputZn_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ListVertices.SelectedItem != null)
            {
                var vertex = geoset.Vertices[ListVertices.SelectedIndex];
                if (float.TryParse(InputZn.Text, out float value))
                {
                    vertex.Normal.Z = value;
                }
            }
        }

        private void InputXt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ListVertices.SelectedItem != null)
            {
                var vertex = geoset.Vertices[ListVertices.SelectedIndex];
                if (float.TryParse(InputXt.Text, out float value))
                {
                    vertex.TexturePosition.X = value;
                }
            }
        }

        private void InputYt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ListVertices.SelectedItem != null)
            {
                var vertex = geoset.Vertices[ListVertices.SelectedIndex];
                if (float.TryParse(InputYt.Text, out float value))
                {
                    vertex.TexturePosition.Y = value;
                }
            }
        }

        private void Selector_Vertex1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListTriangles.SelectedItem != null)
            {
                var triangle = geoset.Triangles[ListTriangles.SelectedIndex];
                if (Selector_Vertex1.SelectedItem != null)
                {
                    triangle.Vertex1.Attach(geoset.Vertices[Selector_Vertex1.SelectedIndex]);
                }
            }
        }
        private void Selector_Vertex2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListTriangles.SelectedItem != null)
            {
                var triangle = geoset.Triangles[ListTriangles.SelectedIndex];
                if (Selector_Vertex2.SelectedItem != null)
                {
                    triangle.Vertex2.Attach(geoset.Vertices[Selector_Vertex2.SelectedIndex]);
                }
            }
        }

        private void Selector_Vertex3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListTriangles.SelectedItem != null)
            {
                var triangle = geoset.Triangles[ListTriangles.SelectedIndex];
                if (Selector_Vertex3.SelectedItem != null)
                {
                    triangle.Vertex3.Attach(geoset.Vertices[Selector_Vertex3.SelectedIndex]);
                }
            }
        }
    }
}
