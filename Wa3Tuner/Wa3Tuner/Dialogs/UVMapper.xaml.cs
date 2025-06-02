using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using Wa3Tuner.Helper_Classes;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using Size = System.Windows.Size;

namespace Wa3Tuner.Dialogs
{
    enum UVEditMode { Move, Rotate, Scale,
        ScaleX,
        ScaleY
    }
    /// <summary>
    /// Interaction logic for UVMapper.xaml
    /// </summary>
    /// 
    public partial class UVMapper : Window
    {
        UVEditMode Mode = UVEditMode.Move;
        UVLockType LockType = UVLockType.None;
        Vector2 CopiedUV = new Vector2();
        private int CurrentGridSize = 0;
        CGeoset? CurrentlySelectedGeoset;
        CModel? Model;
        private string TeamColor = "ReplaceableTextures\\TeamColor\\TeamColor00.blp";
        private string TeamGlow = "ReplaceableTextures\\TeamGlow\\TeamGlow00.blp";
        private string White = "Textures\\white.blp";
        private bool Pause;
        double CurrentTextureHeight = 0;
        double CurrentTextureWidth = 0;
       
        public UVMapper()
        {
            InitializeComponent();
            
        }
        public void SetData(CModel model, int geoset)
        {
            Model = model;
            CurrentlySelectedGeoset = model.Geosets[geoset];
            list_geosets.Items.Clear();
          Pause = true;
            foreach (var g in model.Geosets)
            {
                list_geosets.Items.Add(new ListBoxItem() { Content = g.ObjectId.ToString() });
            }
            list_geosets.SelectedIndex = geoset;

            foreach (var item in list_triangles.Items)
            {
                list_triangles.SelectedItems.Add(item);
            }
         Pause = false;
          RefreshUVMap();
        }
        public void SetData(CModel model, CGeoset geoset, List<CGeosetTriangle> triangles)
        {
            Model = model;
            CurrentlySelectedGeoset = geoset;
            list_geosets.SelectedIndex = model.Geosets.IndexOf(geoset);
            foreach (var riangle in geoset.Triangles)
            {
                riangle.SelectVertices();
            }
            RefreshUVMap() ;
            // unfinished
        }
        private void list_triangles_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
        {
            if (Pause) return;
            RefreshUVMap();
        }
        private void DrawTiledTextureForUVMApper(int whichGeoset, int whichTexture)
        {
          if (Model==null) return;
            CurrentlySelectedGeoset = Model.Geosets[whichGeoset];
            var material = CurrentlySelectedGeoset.Material.Object;
            int position = 0; // the default layer is 0
            if (whichTexture < material.Layers.Count   )
            {
                position = whichTexture;
            }
            if (position <0) { throw new Exception($"Invalid index {position} inside layers"); }
            if (material.Layers == null) { throw new Exception("Null layers container"); }
            var texture = material.Layers[position].Texture.Object;
            if (texture == null) { throw new Exception("Null texture"); }
            ImageSource? image;
           
            int repalceableID = texture.ReplaceableId;
            if (repalceableID == 0)
            {
                string path = material.Layers[position].Texture.Object.FileName;
                if (MPQHelper.FileExists(path))
                {
                    image = MPQHelper.GetImageSource(path);
                }
                else
                {
                    image = MPQHelper.GetImageSource(White);
                }
                   
            }
            else if (repalceableID == 1)
            {
                image = MPQHelper.GetImageSource(TeamColor);

            }
            else if (repalceableID == 2)
            {
                image = MPQHelper.GetImageSource(TeamGlow);
            }
            else
            {
                image = MPQHelper.GetImageSource(White);
            }
           
            if (image == null) { throw new Exception("Null image");  }
            CurrentTextureHeight = image.Height;
            CurrentTextureWidth = image.Width;
            DrawImageInCanvas(image, Canvas_UV);
        }

        public   void DrawImageInCanvas(ImageSource imageSource, Canvas canvas)
        {
            canvas.Children.Clear();

            double imageWidth = imageSource.Width;
            double imageHeight = imageSource.Height;
            double totalWidth = imageWidth * 10;
            double totalHeight = imageHeight * 10;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Image image = new Image
                    {
                        Source = imageSource,
                        Width = imageWidth,
                        Height = imageHeight
                    };

                    Canvas.SetLeft(image, i * imageWidth);
                    Canvas.SetTop(image, j * imageHeight);
                    canvas.Children.Add(image);
                }
            }
            // Update Canvas and Grid size
            canvas.Width = totalWidth;
            canvas.Height = totalHeight;
            Canvas_Grid.Width = totalWidth;
            Canvas_Grid.Height = totalHeight;
            Canvas_Vertices.Width = totalWidth;
            Canvas_Vertices.Height = totalHeight;

            Canvas_Selection.Width = totalWidth;
            Canvas_Selection.Height = totalHeight;
        }

        private void sall(object? sender, RoutedEventArgs? e)
        {
            Pause = true;
            foreach (var item in list_triangles.Items)
            {
                list_triangles.SelectedItems.Add(item);
            }
            Pause = false;
            list_triangles_SelectionChanged(null,null);

        }

        private void snone(object? sender, RoutedEventArgs? e)
        {
            Pause = true;
            list_triangles.SelectedItems.Clear();
               Pause = false;
            list_triangles_SelectionChanged(null, null);
        }

        private void sin(object? sender, RoutedEventArgs? e)
        {
            Pause = true;
            foreach (var item in list_triangles.Items)
            {
                if (list_triangles.SelectedItems.Contains(item))
                {
                    list_triangles.SelectedItems.Remove(item);
                }
                else
                {
                    list_triangles.SelectedItems.Add(item);
                }
                     
            }
            Pause = false;
            list_triangles_SelectionChanged(null, null);
        }

        private void setmode_move(object? sender, RoutedEventArgs? e)
        {
            Mode = UVEditMode.Move;
            buttonMove.BorderBrush = Brushes.Green;
            buttonScale.BorderBrush = Brushes.Gray;
            buttonScaleY.BorderBrush = Brushes.Gray;
            buttonScaleX.BorderBrush = Brushes.Gray;
            buttonRotate.BorderBrush = Brushes.Gray;
        }

        private void setmode_rotate(object? sender, RoutedEventArgs? e)
        {
            Mode = UVEditMode.Rotate;
            buttonMove.BorderBrush = Brushes.Gray;
            buttonScale.BorderBrush = Brushes.Gray;
            buttonScaleY.BorderBrush = Brushes.Gray;
            buttonScaleX.BorderBrush = Brushes.Gray;
            buttonRotate.BorderBrush = Brushes.Green;

        }

        private void setmode_scale(object? sender, RoutedEventArgs? e)
        {
            Mode = UVEditMode.Scale;
            buttonMove.BorderBrush = Brushes.Gray;
            buttonScale.BorderBrush = Brushes.Green;
            buttonScaleY.BorderBrush = Brushes.Gray;
            buttonScaleX.BorderBrush = Brushes.Gray;
            buttonRotate.BorderBrush = Brushes.Gray;

        }

        private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Hide();
        }

        private void inputGrid_TextChanged(object? sender, TextChangedEventArgs e)
        {
            bool p = int.TryParse(inputGrid.Text, out int v);
            if (!p) return;
            if (v > 0)
            {
                CurrentGridSize = v;
                DrawGrid(Canvas_Grid, v);
            }
        }
        private void DrawGrid(Canvas canvas, int count)
        {
            canvas.Children.Clear();
            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;
            double xStep = width / count;
            double yStep = height / count;

            for (int i = 0; i <= count; i++)
            {
                double x = i * xStep;
                double y = i * yStep;

                Line vertical = new Line
                {
                    X1 = x,
                    Y1 = 0,
                    X2 = x,
                    Y2 = height,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 1
                };

                Line horizontal = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = width,
                    Y2 = y,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 1
                };

                canvas.Children.Add(vertical);
                canvas.Children.Add(horizontal);
            }
        }

        private void lockunlockU(object? sender, RoutedEventArgs? e)
        {
            if (LockType == UVLockType.None) { LockType = UVLockType.U; buttonLockU.BorderBrush = Brushes.Green; }
            else if (LockType == UVLockType.U) { LockType = UVLockType.None; buttonLockU.BorderBrush = Brushes.Gray; }
        }

        private void lockunlockV(object? sender, RoutedEventArgs? e)
        {
            if (LockType == UVLockType.None) { LockType = UVLockType.V; buttonLockV.BorderBrush = Brushes.Green; }
            else if (LockType == UVLockType.V){ LockType = UVLockType.None; buttonLockV.BorderBrush = Brushes.Gray;}
        }

        private void copy(object? sender, RoutedEventArgs? e)
        {
            bool i = float.TryParse(inputU.Text, out float u);
            bool a = float.TryParse(inputU.Text, out float v);
            if (i && a)
            {
                if (u >= -10 && u <= 10 && v >= -10 && v <=10) CopiedUV = new Vector2(u,v);
                else
                {
                    MessageBox.Show("Invalid input coordinates. Must be between -10 and 10");
                }
            }
        }

        private void FillTrianglesOfGeoset(object? sender, SelectionChangedEventArgs e)
        {
            if (Pause) return;
            if (Model == null) return;
            if (list_geosets.SelectedItem != null)
            {
                int index = list_geosets.SelectedIndex;
                FillTrianglesOfGeoset(Model.Geosets[index]);
                FillTexturesOfGeoset(Model.Geosets[index]);
            }
        }

        private void FillTexturesOfGeoset(CGeoset g)
        {
            Pause = true;
            ComboTexture.Items.Clear();
            var mat = g.Material.Object;
            foreach (var layer in mat.Layers)
            {
                var tx = layer.Texture.Object;
                if (tx.ReplaceableId == 0)
                {
                    ComboTexture.Items.Add(tx.FileName);
                }
               else 
                {
                    ComboTexture.Items.Add($"ReplaceableId {tx.ReplaceableId}");
                }
                
               
            }
            Pause = false;
            ComboTexture.SelectedIndex = 0;
        }

        private void FillTrianglesOfGeoset(CGeoset geo)
        {
            if (Pause) return;
            list_triangles.Items.Clear();
            foreach (var triangle in geo.Triangles)
            {
                string v1 = "("+triangle.Vertex1.Object.TexturePosition.X.ToString() + " " + triangle.Vertex1.Object.TexturePosition.Y.ToString()+ ")";
                string v2 = "("+triangle.Vertex2.Object.TexturePosition.X.ToString() + " " + triangle.Vertex2.Object.TexturePosition.Y.ToString()+ ")";
                string v3 = "("+triangle.Vertex3.Object.TexturePosition.X.ToString() + " " + triangle.Vertex3.Object.TexturePosition.Y.ToString()+ ")";
                list_triangles.Items.Add(new ListBoxItem() { Content = $"{triangle.ObjectId} {v1}{v2}{v3}" });
            }
        }

        private void ComboTexture_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {if (Pause) return;
           // if (ComboTexture.SelectedIndex == -1) return;
            DrawTiledTextureForUVMApper(list_geosets.SelectedIndex, ComboTexture.SelectedIndex);
        }

        private void resetto0(object? sender, RoutedEventArgs? e)
        {
            if (Model==null) return;
            if (list_geosets.SelectedItem != null && list_triangles.SelectedItems.Count > 0)
            {
                var g = Model.Geosets[list_geosets.SelectedIndex];

                List<CGeosetVertex> selected = new List<CGeosetVertex>();
                if (selected.Count <= 1) { MessageBox.Show("Select at least 2 vertices"); return; }
                var v1 = selected[0];
                foreach (var triangle in g.Triangles)
                {
                    if (triangle.Vertex1.Object.isSelected) selected.Add(triangle.Vertex1.Object);
                    if (triangle.Vertex2.Object.isSelected) selected.Add(triangle.Vertex2.Object);
                    if (triangle.Vertex3.Object.isSelected) selected.Add(triangle.Vertex3.Object);
                }

                for (int i = 0; i < selected.Count; i++)
                {
                    selected[0].TexturePosition = new MdxLib.Primitives.CVector2();
                }
                RefreshUVMap();
            }

        }
        private void RefreshUVMap()
        {
            if (Pause) return;

            if (list_geosets.SelectedItem != null && list_triangles.SelectedItems.Count > 0)
            {
                if (Model==null) return;
                var geoset = Model.Geosets[list_geosets.SelectedIndex];
                List<CGeosetTriangle> selected = new();
                foreach (var item in list_triangles.SelectedItems)
                {
                    selected.Add(geoset.Triangles[list_triangles.Items.IndexOf(item)]);
                }

                double imageHeight = CurrentTextureHeight;
                double imageWidth = CurrentTextureWidth;
                Canvas c = Canvas_Vertices;
                c.Children.Clear(); // Clear previous drawings

                double canvasWidth = c.ActualWidth;
                double canvasHeight = c.ActualHeight;
                double pointSize = 5; // Size of the vertex points

                foreach (var triangle in selected)
                {
                    var uv1 = triangle.Vertex1.Object.TexturePosition;
                    var uv2 = triangle.Vertex2.Object.TexturePosition;
                    var uv3 = triangle.Vertex3.Object.TexturePosition;

                    // Convert UV coordinates (-10 to 10) to canvas space
                    float x1 = Calculator.GetCanvasPositionFromU(uv1.X, imageWidth, canvasWidth);
                    float y1 = Calculator.GetCanvasPositionFromV(uv1.Y, imageHeight, canvasHeight);

                    float x2 = Calculator.GetCanvasPositionFromU(uv2.X, imageWidth, canvasWidth);
                    float y2 = Calculator.GetCanvasPositionFromV(uv2.Y, imageHeight, canvasHeight);

                    float x3 = Calculator.GetCanvasPositionFromU(uv3.X, imageWidth, canvasWidth);
                    float y3 = Calculator.GetCanvasPositionFromV(uv3.Y, imageHeight, canvasHeight);

                    // Draw lines between vertices
                    DrawLine(c, x1, y1, x2, y2);
                    DrawLine(c, x2, y2, x3, y3);
                    DrawLine(c, x3, y3, x1, y1);

                    // Draw vertices
                    DrawVertex(c, x1, y1, pointSize);
                    DrawVertex(c, x2, y2, pointSize);
                    DrawVertex(c, x3, y3, pointSize);
                }
            }
            else
            {
                Canvas_Vertices.Children.Clear();
            }
        }


        private void DrawLine(Canvas canvas, double x1, double y1, double x2, double y2)
        {
            Line line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.White,
                StrokeThickness = 1
            };
            canvas.Children.Add(line);
        }

        private void DrawVertex(Canvas canvas, double x, double y, double size)
        {
            Ellipse vertex = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = Brushes.Red,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Canvas.SetLeft(vertex, x - size / 2);
            Canvas.SetTop(vertex, y - size / 2);
            canvas.Children.Add(vertex);
        }


        private void negateus(object? sender, RoutedEventArgs? e)
        {
            if (list_geosets.SelectedItem != null && list_triangles.SelectedItems.Count > 0)
            {
                if (Model == null) return;
                var g = Model.Geosets[list_geosets.SelectedIndex];

                List<CGeosetVertex> selected = new List<CGeosetVertex>();
                if (selected.Count <= 1) { MessageBox.Show("Select at least 2 vertices"); return; }
                var v1 = selected[0];
                foreach (var triangle in g.Triangles)
                {
                    if (triangle.Vertex1.Object.isSelected) selected.Add(triangle.Vertex1.Object);
                    if (triangle.Vertex2.Object.isSelected) selected.Add(triangle.Vertex2.Object);
                    if (triangle.Vertex3.Object.isSelected) selected.Add(triangle.Vertex3.Object);
                }

                for (int i = 0; i < selected.Count; i++)
                {
                    Calculator.NegateInside(selected[i].TexturePosition, true, false);
                    
                }
                RefreshUVMap();
            }
        }

        private void mirrorvs(object? sender, RoutedEventArgs? e)
        {
            if (Model == null) return;
            if (list_geosets.SelectedItem != null && list_triangles.SelectedItems.Count > 0)
            {
                var g = Model.Geosets[list_geosets.SelectedIndex];

                List<CGeosetVertex> selected = new List<CGeosetVertex>();
                if (selected.Count <= 1) { MessageBox.Show("Select at least 2 vertices"); return; }
                var v1 = selected[0];
                foreach (var triangle in g.Triangles)
                {
                    if (triangle.Vertex1.Object.isSelected) selected.Add(triangle.Vertex1.Object);
                    if (triangle.Vertex2.Object.isSelected) selected.Add(triangle.Vertex2.Object);
                    if (triangle.Vertex3.Object.isSelected) selected.Add(triangle.Vertex3.Object);
                }

                for (int i = 0; i < selected.Count; i++)
                {
                    Calculator.NegateInside(selected[i].TexturePosition, false, true);

                }
                RefreshUVMap();
            }
        }

        private void swapuvs(object? sender, RoutedEventArgs? e)
        {
            if (Model == null) return;
            if (list_geosets.SelectedItem != null && list_triangles.SelectedItems.Count > 0)
            {
                var g = Model.Geosets[list_geosets.SelectedIndex];

                List<CGeosetVertex> selected = new List<CGeosetVertex>();
                if (selected.Count <= 1) { MessageBox.Show("Select at least 2 vertices"); return; }
                var v1 = selected[0];
                foreach (var triangle in g.Triangles)
                {
                    if (triangle.Vertex1.Object.isSelected) selected.Add(triangle.Vertex1.Object);
                    if (triangle.Vertex2.Object.isSelected) selected.Add(triangle.Vertex2.Object);
                    if (triangle.Vertex3.Object.isSelected) selected.Add(triangle.Vertex3.Object);
                }

                for (int i = 0; i < selected.Count; i++)
                {
                    Calculator.SwapUV(selected[i].TexturePosition);
                   

                }
                RefreshUVMap();
            }
        }

        private void paste1(object? sender, RoutedEventArgs? e)
        {
            var vertices = GetSelectedVertcesUV();
            if (vertices.Count == 1)
            {
                vertices[0].TexturePosition = new CVector2(CopiedUV.X, CopiedUV.Y);
            }
            else if (vertices.Count > 1)
            {
                CVector2 centroid = Calculator.GetCentroidOfUV(vertices.Select(x => x.TexturePosition).ToList());
                foreach (var uv in vertices)
                {
                    // Adjust each vertex's TexturePosition relative to the copied UV and centroid
                    uv.TexturePosition = new CVector2(CopiedUV.X + (uv.TexturePosition.X - centroid.X), CopiedUV.Y + (uv.TexturePosition.Y - centroid.Y));
                }
            }
        }
    

        private void flattenU(object? sender, RoutedEventArgs? e)
        {
            if (list_geosets.SelectedItem != null && list_triangles.SelectedItems.Count > 0)
            {if (Model == null) return;
                var g = Model.Geosets[list_geosets.SelectedIndex];

                List<CGeosetVertex> selected = new List<CGeosetVertex>();
                if (selected.Count <= 1) { MessageBox.Show("Select at least 2 vertices"); return; }
                var v1 = selected[0];
                foreach (var triangle in g.Triangles)
                {
                    if (triangle.Vertex1.Object.isSelected) selected.Add(triangle.Vertex1.Object);
                    if (triangle.Vertex2.Object.isSelected) selected.Add(triangle.Vertex2.Object);
                    if (triangle.Vertex3.Object.isSelected) selected.Add(triangle.Vertex3.Object);
                }

                for (int i = 1; i < selected.Count; i++)
                {
                    selected[0].TexturePosition.X = v1.TexturePosition.X;
                }
                RefreshUVMap();
            }
        }

        private void flattenV(object? sender, RoutedEventArgs? e)
        {
            
            if (list_geosets.SelectedItem != null && list_triangles.SelectedItems.Count >0)
            {
                if (Model == null) return;
                var g = Model.Geosets[list_geosets.SelectedIndex];
                
                List<CGeosetVertex> selected = new List<CGeosetVertex>();
                if (selected.Count <= 1) { MessageBox.Show("Select at least 2 vertices");return; }
                var v1 = selected[0];
                foreach (var triangle in g.Triangles)
                {
                    if (triangle.Vertex1.Object.isSelected) selected.Add(triangle.Vertex1.Object);
                    if (triangle.Vertex2.Object.isSelected) selected.Add(triangle.Vertex2.Object);
                    if (triangle.Vertex3.Object.isSelected) selected.Add(triangle.Vertex3.Object);
                }
                
               for (int i =1; i < selected.Count; i++)
                {
                    selected[0].TexturePosition.Y = v1.TexturePosition.Y;
                }
                RefreshUVMap();
            }
        }

        private void clamp(object? sender, RoutedEventArgs? e)
        {
            if (list_geosets.SelectedItem != null && list_triangles.SelectedItems.Count > 0)
            {
                if (Model == null) return;
                var g = Model.Geosets[list_geosets.SelectedIndex];

                List<CGeosetVertex> selected = new List<CGeosetVertex>();
                foreach (var v in selected)
                {
                    v.TexturePosition.X = Calculator.ClampUV(v.TexturePosition.X);
                    v.TexturePosition.Y = Calculator.ClampUV(v.TexturePosition.Y);
                }
                RefreshUVMap();
            }

        }

        private void swaptwo(object? sender, RoutedEventArgs? e)
        {
            if (list_geosets.SelectedItem != null && list_triangles.SelectedItems.Count > 0)
            {
                if (Model == null) return;
                var g = Model.Geosets[list_geosets.SelectedIndex];

                List<CGeosetVertex> selected = new List<CGeosetVertex>();
                if (selected.Count  != 2) { MessageBox.Show("Select exatcly two vertices"); return; }
                var v1 = selected[0];
                var v2 = selected[2];

                Calculator.SwapTwo(v1,v2);
                RefreshUVMap();
            }
        }

        private void project(object? sender, RoutedEventArgs? e)
        {
            // unfinished
        }
        private float GetU()
        {
            if (float.TryParse(inputU.Text, out float f))
            {
                return f;
            }
            else
            {
                return 0;
            }
        }
        private float GetV()
        {
            if (float.TryParse(inputV.Text, out float f))
            {
                return f;
            }
            else
            {
                return 0;
            }
        }
        private void inputU_KeyDown(object? sender, KeyEventArgs e)
        {
            List<CGeosetVertex> list = GetSelectedVertcesUV();
            float val = GetU();

            if (list.Count == 1)
            {
                var t = list[0].TexturePosition.Y;
                list[0].TexturePosition = new CVector2(val, t);
            }
            else if (list.Count > 1)
            {
                CVector2 centroid = Calculator.GetCentroidOfUV(list.Select(x => x.TexturePosition).ToList());
                float delta = val - centroid.X; // Compute the difference between new U and the centroid's U

                foreach (var vertex in list)
                {
                    CVector2 uv = vertex.TexturePosition;
                    vertex.TexturePosition = new CVector2(uv.X + delta, uv.Y);
                }
            }

            RefreshUVMap();
        }

       
        private List<CGeosetVertex> GetSelectedVertcesUV()
        {
            if (Model == null) return new List<CGeosetVertex> { };
            var list = new   List<CGeosetVertex>();
            if (list_geosets.SelectedItem == null) return new List<CGeosetVertex>();
            if (list_triangles.SelectedItems.Count == 0) return new List<CGeosetVertex>();
            var geo = Model.Geosets[list_geosets.SelectedIndex];

            for (int i = 0; i < list_geosets.Items.Count; i++)
            {
                list.Add(geo.Vertices[i]);
            }
            return list;
            
        }

        private void buttonLockU_KeyDown(object? sender, KeyEventArgs e)
        {
            // incorrect - based on centroid
            bool b = float.TryParse(inputV.Text, out float f);
            if (b)
            {
                if (f >= -10 && f <= 10)
                {
                    if (list_geosets.SelectedItem != null && list_triangles.SelectedItems.Count > 0)
                    {
                        if (Model == null) return;
                        var g = Model.Geosets[list_geosets.SelectedIndex];

                        List<CGeosetVertex> selected = new List<CGeosetVertex>();
                        foreach (var v in selected) v.TexturePosition.Y = f;
                    }
                }
                else
                {
                    MessageBox.Show("expected integer or float between -10 and 10"); return;
                }
            }
            else
            {
                MessageBox.Show("expected integer or float"); return;
            }
            RefreshUVMap();
        }

        private void Canvas_Selection_MouseUp(object? sender, MouseButtonEventArgs e)
        {
            if (SelectionRect==null) return;
            // On mouse release, finalize the selection
            SelectVericesBasedOnSelection(SelectionRect);
            Canvas_Selection.Children.Remove(SelectionRect);  // Remove the selection rectangle from the canvas
        }

        private Rectangle? SelectionRect; // Use Rectangle here

        private Point startPoint;

        private void Canvas_Selection_MouseDown(object? sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Pressed == false)
            {

               
                // When mouse button is pressed, start drawing the selection rectangle
                startPoint = e.GetPosition(Canvas_Selection);
                SelectionRect = new Rectangle
                {
                    Stroke = Brushes.Blue,
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent,
                    Width = 0,
                    Height = 0
                };

                // Add the rectangle to the canvas
                Canvas_Selection.Children.Add(SelectionRect);
            }
            else if (e.RightButton == MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Pressed == false)
            {
              

            }
                
        }
     
        private double LastX = 0;
        private double LastY = 0;
        private double DiffX = 0;
        private double DiffY = 0;
        
        private const float UVIncrement = 0.3f;
        private float Clamp(float f)
        {
            if (f < -10) return -10;
            if (f  > 10) return 10;
            return f;
        }
        private void Canvas_Selection_MouseMove(object? sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(Canvas_Selection);
            DiffX = pos.X  - LastX;
            DiffY = pos.Y - LastY;
            LastX = pos.X;
            LastY = pos.Y;
            // As the mouse moves, update the selection rectangle
            if (e.LeftButton == MouseButtonState.Pressed && SelectionRect != null)
            {
                Point currentPoint = e.GetPosition(Canvas_Selection);
                double width = Math.Abs(currentPoint.X - startPoint.X);
                double height = Math.Abs(currentPoint.Y - startPoint.Y);

                SelectionRect.Width = width;
                SelectionRect.Height = height;

                // Position the rectangle at the minimum x and y position
                Canvas.SetLeft(SelectionRect, Math.Min(startPoint.X, currentPoint.X));
                Canvas.SetTop(SelectionRect, Math.Min(startPoint.Y, currentPoint.Y));
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                if (Mode == UVEditMode.Move)
                {
                    foreach (var v in SelectedVertices)
                    {
                        if (LockType == UVLockType.None || LockType == UVLockType.V)
                        {
                            float tempPosX = DiffX > 0 ? v.TexturePosition.X + UVIncrement : v.TexturePosition.X - UVIncrement;
                            v.TexturePosition.X = Clamp(tempPosX);

                        }
                        if (LockType == UVLockType.None || LockType == UVLockType.U)
                        {
                            float tempPosY = DiffY > 0 ? v.TexturePosition.Y + UVIncrement : v.TexturePosition.Y - UVIncrement;
                            v.TexturePosition.Y = Clamp(tempPosY);
                        }


                    }
                }
                else if (Mode == UVEditMode.Rotate)
                {
                    if (SelectedVertices.Count > 1)
                    {
                        var list = SelectedVertices.Select(x => x.TexturePosition).ToList();
                        var centrod = Calculator.GetCentroidOfUV(list);
                        foreach (var v in SelectedVertices)
                        {
                            v.TexturePosition = Calculator.RotateUVAroundCentroid(centrod, v.TexturePosition, UVIncrement);
                        }
                    }
                }
                else if (Mode == UVEditMode.Scale)
                {
                    if (SelectedVertices.Count > 1)
                    {
                        var list = SelectedVertices.Select(x => x.TexturePosition).ToList();
                        var centrod = Calculator.GetCentroidOfUV(list);


                        foreach (var v in SelectedVertices)
                        {
                            v.TexturePosition = Calculator.ScaleUVAroundCentroid(centrod, v.TexturePosition, UVIncrement/100);
                        }

                    }


                } }
                //unfinished
            }

       
        private List<CGeosetVertex> SelectedVertices = new List<CGeosetVertex>();
        private void SelectVericesBasedOnSelection(Rectangle selectionRect)
        {
            if (list_geosets.SelectedItem == null) return;
            if (Model == null) return;
            var geoset = Model.Geosets[list_geosets.SelectedIndex];

            for (int i = 0; i < geoset.Vertices.Count; i++)
            {
                geoset.Vertices[i].isSelected = VerrexInsideRect(selectionRect, geoset.Vertices[i]);
            }
            SelectedVertices = GetSelectedVertcesUV();
            RefreshUVMap();
        }

        private bool VerrexInsideRect(Rectangle selectionRect, CGeosetVertex cGeosetVertex)
        {
            // Assuming Position is a Point or a way to get X/Y
            double x = cGeosetVertex.Position.X;
            double y = cGeosetVertex.Position.Y;

            // Get the position of the selection rectangle
            double rectX = Canvas.GetLeft(selectionRect);
            double rectY = Canvas.GetTop(selectionRect);

            // Check if the vertex is inside the bounds of the rectangle
            return x >= rectX && x <= rectX + selectionRect.Width && y >= rectY && y <= rectY + selectionRect.Height;
        }



        private void setmode_scaleX(object? sender, RoutedEventArgs? e)
        {
            Mode = UVEditMode.ScaleX;
            buttonMove.BorderBrush = Brushes.Gray;
            buttonScale.BorderBrush = Brushes.Gray;
            buttonScaleY.BorderBrush = Brushes.Gray;
            buttonScaleX.BorderBrush = Brushes.Green;
            buttonRotate.BorderBrush = Brushes.Gray;
        }

        private void setmode_scaleY(object? sender, RoutedEventArgs? e)
        {
            Mode = UVEditMode.ScaleY;
            buttonMove.BorderBrush = Brushes.Gray;  
            buttonScale.BorderBrush = Brushes.Gray;
            buttonScaleY.BorderBrush = Brushes.Green;
            buttonScaleX.BorderBrush = Brushes.Gray;
            buttonRotate.BorderBrush = Brushes.Gray;
        }

        private void TextBlock_KeyDown(object? sender, KeyEventArgs e)
        {

        }

        private void inputV_KeyDown(object? sender, KeyEventArgs e)
        {
            List<CGeosetVertex> list = GetSelectedVertcesUV();
            float val = GetV();

            if (list.Count == 1)
            {
                var t = list[0].TexturePosition.X;
                list[0].TexturePosition = new CVector2(t, val);
            }
            else if (list.Count > 1)
            {
                CVector2 centroid = Calculator.GetCentroidOfUV(list.Select(x => x.TexturePosition).ToList());
                float delta = val - centroid.Y; // Compute the difference between new U and the centroid's U

                foreach (var vertex in list)
                {
                    CVector2 uv = vertex.TexturePosition;
                    vertex.TexturePosition = new CVector2(uv.X, uv.Y + delta);
                }
            }

            RefreshUVMap();
        }

        private void SetToGeoset_s(object? sender, RoutedEventArgs? e)
        {
            if (list_geosets.SelectedItem != null)
            {
                if (Model == null) return;
                var g = Model.Geosets[list_geosets.SelectedIndex];
                var mat = g.Material.Object;
                var t = mat.Layers[0].Texture.Object;
               int tID = Model.Textures.IndexOf(t);
                ComboTexture.SelectedIndex = tID;
            }
        }
    }
}
