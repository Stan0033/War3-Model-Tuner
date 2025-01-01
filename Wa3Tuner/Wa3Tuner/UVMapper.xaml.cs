using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using W3_Texture_Finder;

namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for UVMapper.xaml
    /// </summary>
    enum UVEditMode
    {
        Move, Zoom, Rotate
    }
    public partial class UVMapper : Window
    {
        CModel Model;
        CGeoset CurrenctlySelectedGeoset;
        ImageSource CurrentlyLoadedTexture;
        List<CGeosetFace> SelectedTriangles = new List<CGeosetFace>();
        Dictionary<Ellipse, CGeosetVertex> VertexReference = new Dictionary<Ellipse, CGeosetVertex>();
        UVEditMode Mode = UVEditMode.Move;
        public UVMapper()
        {
            InitializeComponent();
        }
        public void Refresh(CModel model)
        {
            Model   = model;
            List_Geosets_UV.Items.Clear();
            foreach (CGeoset geoset in model.Geosets)
            {
                string item = $"Geoset {geoset.ObjectId} [{geoset.Faces.Count} triangles]";
                List_Geosets_UV.Items.Add(new ListBoxItem() { Content = item });
            }
            List_Faces_UV.Items.Clear();    
        }
        private void LoadTexture()
        {
            if (CurrenctlySelectedGeoset.Material.Object.Layers.Count == 0)
            {
                MessageBox.Show("This geoset's material doesn't have layers");
                CurrentlyLoadedTexture = null; return;
            }
             
            if (CurrenctlySelectedGeoset.Material.Object.Layers[0].TextureId.Static == false)
            {
                MessageBox.Show("This geoset's material's first layer's texture is not static");
                CurrentlyLoadedTexture = null; return;
            }
            CTexture texture = CurrenctlySelectedGeoset.Material.Object.Layers[0].Texture.Object;
            if ( Model.Textures.Contains(texture) == false)
            {
                MessageBox.Show("This geoset's material's layer is not part of the model's textures list");
                CurrentlyLoadedTexture = null; return;
            }
            string path = "";
            if (texture.ReplaceableId == 0)
            {
                path = texture.FileName;
            }
            if (texture.ReplaceableId == 1) { path = "ReplaceableTextures\\TeamColor\\TeamColor00.blp"; }
            if (texture.ReplaceableId == 2) { path = "ReplaceableTextures\\TeamGlow\\TeamGlow00.blp"; }
            if (texture.ReplaceableId > 2)
            {
                MessageBox.Show("The UV Mapper does not support displaying textures with replaceable id higher than 2");
                CurrentlyLoadedTexture = null; return;
            }
            if (MPQHelper.FileExists(path) == false)
            {
                MessageBox.Show("The texture was not found in the MPQs"); CurrentlyLoadedTexture = null; return;
            }
            CurrentlyLoadedTexture = MPQHelper.GetImageSource(path);
            Displayer_Texture.Source = CurrentlyLoadedTexture;


            //  CurrenctlySelectedGeoset.Material.Object.Layers[0].Texture
            //   CurrentlyLoadedTexture =
        }
        private void SelectedGeoset(object sender, SelectionChangedEventArgs e)
        {
            if (List_Geosets_UV.SelectedItem != null)
            {
                List_Faces_UV.Items.Clear();
                int index = List_Geosets_UV.SelectedIndex;
                CurrenctlySelectedGeoset = Model.Geosets[index];
                for (int i = 0; i < Model.Geosets[index].Faces.Count; i++)
                {
                    List_Faces_UV.Items.Add(new ListBoxItem() { Content = $"Triangle {i}" });
                }
                Canvas_UV_Draw.Children.Clear();
                SelectedTriangles.Clear();
                LoadTexture();
            }
        }
        private List<CGeosetFace> GetSelectedFaces()
        {
            
            List<CGeosetFace> list = new List<CGeosetFace> ();
            CGeoset SelectedGeoset = Model.Geosets[List_Geosets_UV.SelectedIndex];
            for (int i = 0; i < List_Faces_UV.Items.Count; i++)
            {
                if (List_Faces_UV.SelectedItems.Contains(List_Faces_UV.Items[i]))
                {
                    list.Add(SelectedGeoset.Faces[i]);
                }
            }
            return list;
        }
        private void SelectFaces(object sender, SelectionChangedEventArgs e)
        {

            if (List_Geosets_UV.SelectedItem != null && List_Faces_UV.SelectedItems.Count != 0)
            {
                SelectedTriangles.Clear();
                SelectedTriangles = GetSelectedFaces();
                Redraw();

            }
             
        }
        private void Redraw()
        {
            Canvas_UV_Draw.Children.Clear();
            VertexReference.Clear();
            if (CurrentlyLoadedTexture == null) { return; }

            double imageWidth = CurrentlyLoadedTexture.Width;
            double imageHeight = CurrentlyLoadedTexture.Height;

            foreach (var triangle in SelectedTriangles)
            {
                 Clamp(triangle.Vertex1.Object.TexturePosition);
                Clamp(triangle.Vertex2.Object.TexturePosition);
                 Clamp(triangle.Vertex3.Object.TexturePosition);
                // Extract texture coordinates (u, v) for each vertex of the triangle
                float u1 = triangle.Vertex1.Object.TexturePosition.X;
                float u2 = triangle.Vertex2.Object.TexturePosition.X;
                float u3 = triangle.Vertex3.Object.TexturePosition.X;
                float v1 = triangle.Vertex1.Object.TexturePosition.Y;
                float v2 = triangle.Vertex2.Object.TexturePosition.Y;
                float v3 = triangle.Vertex3.Object.TexturePosition.Y;

                // Convert the texture coordinates to canvas positions
                double position_u1 = imageHeight * u1;
                double position_u2 = imageHeight * u2;
                double position_u3 = imageHeight * u3;
                double position_v1 = imageWidth * v1;
                double position_v2 = imageWidth * v2;
                double position_v3 = imageWidth * v3;

                // Draw the vertices as points
                Ellipse ellipse1 = new Ellipse
                {
                    Fill = Brushes.Red,
                    Width = 5,
                    Height = 5
                };
                Canvas.SetLeft(ellipse1, position_v1 - 2.5);  // Center the point
                Canvas.SetTop(ellipse1, position_u1 - 2.5);
                Canvas_UV_Draw.Children.Add(ellipse1);

                Ellipse ellipse2 = new Ellipse
                {
                    Fill = Brushes.Red,
                    Width = 5,
                    Height = 5
                };
                Canvas.SetLeft(ellipse2, position_v2 - 2.5);
                Canvas.SetTop(ellipse2, position_u2 - 2.5);
                Canvas_UV_Draw.Children.Add(ellipse2);

                Ellipse ellipse3 = new Ellipse
                {
                    Fill = Brushes.Red,
                    Width = 5,
                    Height = 5
                };
                VertexReference.Add(ellipse1, triangle.Vertex1.Object);
                VertexReference.Add(ellipse2, triangle.Vertex2.Object);
                VertexReference.Add(ellipse3, triangle.Vertex3.Object);
                Canvas.SetLeft(ellipse3, position_v3 - 2.5);
                Canvas.SetTop(ellipse3, position_u3 - 2.5);
                Canvas_UV_Draw.Children.Add(ellipse3);

                // Draw the lines connecting the vertices
                Line line1 = new Line
                {
                    X1 = position_v1,
                    Y1 = position_u1,
                    X2 = position_v2,
                    Y2 = position_u2,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                Canvas_UV_Draw.Children.Add(line1);

                Line line2 = new Line
                {
                    X1 = position_v2,
                    Y1 = position_u2,
                    X2 = position_v3,
                    Y2 = position_u3,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                Canvas_UV_Draw.Children.Add(line2);

                Line line3 = new Line
                {
                    X1 = position_v3,
                    Y1 = position_u3,
                    X2 = position_v1,
                    Y2 = position_u1,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                Canvas_UV_Draw.Children.Add(line3);
            }
        }

        private void Clamp(CVector2 texturePosition)
        {
            
            texturePosition =  Calculator.ClampUV(texturePosition);
            
        }
        enum SetTo { U,V,Both}
        private void SetValueOfSelelection(float value, SetTo to )
        {

        }
        private void EnterU(object sender, KeyEventArgs e)
        {
            try
            {
                float value = float.Parse(InputU.Text);
                if (value < 0 || value > 1)
                {
                    MessageBox.Show("Invalid input. Expected float between 0 and 1");
                    return;
                }
                SetValueOfSelelection(value, SetTo.U);
            }
            catch
            {
                MessageBox.Show("Invalid input. Expected float between 0 and 1");
                return;
            }
        }

        private void EnterV(object sender, KeyEventArgs e)
        {
            try
            {
                float value = float.Parse(InputU.Text);
                if (value < 0 || value > 1)
                {
                    MessageBox.Show("Invalid input. Expected float between 0 and 1");
                    return;
                }
                SetValueOfSelelection(value, SetTo.V);
            }
            catch
            {
                MessageBox.Show("Invalid input. Expected float between 0 and 1");
                return;
            }
        }
        private float CopiedU, CopiedV = 0;
        private void CopyUV(object sender, RoutedEventArgs e)
        {

        }

        private void PasteUV(object sender, RoutedEventArgs e)
        {

        }
        private Vector2 GetCentroid()
        {
            if (SelectedTriangles.Count == 0) { return Vector2.Zero; }
            if (SelectedTriangles.Count == 1 ) { return Vector2.Zero; }


            // SelectedTriangles
            return new Vector2();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
          e.Cancel = true;
            Hide();
        }

        private void selectall(object sender, RoutedEventArgs e)
        {
            if (List_Geosets_UV.SelectedItem != null)
            {
                foreach (var item in List_Faces_UV.SelectedItems)
                {
                    List_Faces_UV.SelectedItems.Add(item);
                }
            }
            SelectFaces(null, null);
        }

        private void unselect(object sender, RoutedEventArgs e)
        {
            List_Faces_UV.SelectedItems.Clear();
            Canvas_UV_Draw.Children.Clear();
        }

        private void inverseselect(object sender, RoutedEventArgs e)
        {
            List<object> unselected = new List<object>();
            foreach (var item in List_Geosets_UV.Items)
            {
                if (List_Geosets_UV.SelectedItems.Contains(item) == false)
                {
                    unselected.Add(item);
                }
            }
            unselect(null,null);
            foreach (var item in unselected)
            {
                List_Geosets_UV.SelectedItems.Add( item);
            }
            SelectFaces(null,null);
        }

        internal void Insert(CModel currentModel)
        {
           Model = currentModel;
            
        }
        private bool IsSelecting = false;
        private bool InEditingMode = false;
        private double PreviousXPosition = 0;
        private double PreviousYPosition = 0;
        private void Canvas_UV_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsSelecting) 
            { 

            }
            else
            {
                switch (Mode)
                {
                    case UVEditMode.Move:break;
                    case UVEditMode.Rotate:break;
                    case UVEditMode.Zoom:break;
                }
            }
        }

        private void Canvas_UV_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                IsSelecting = true;
            }
            InEditingMode = true;
        }

        private void Canvas_UV_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                IsSelecting = false;
            }
            InEditingMode = false;
        }

        private void setmove(object sender, RoutedEventArgs e)
        {
            Mode = UVEditMode.Move;
        }

        private void setzoom(object sender, RoutedEventArgs e)
        {
            Mode = UVEditMode.Zoom;
        }

        private void setrotate(object sender, RoutedEventArgs e)
        {
            Mode = UVEditMode.Rotate;
        }
    }
}
