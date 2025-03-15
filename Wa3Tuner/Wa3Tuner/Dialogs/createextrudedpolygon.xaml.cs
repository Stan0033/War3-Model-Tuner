using MdxLib.Model;
using MdxLib.Primitives;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wa3Tuner.Helper_Classes;
using static Wa3Tuner.MainWindow;
namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for createextrudedpolygon.xaml
    /// </summary>
    public class DrawnVertex
    {
        public Point Position;
        public Ellipse Object;
        public bool Selected = false;
        public DrawnVertex(double x, double y, Ellipse ellipse)
        {
            Position = new Point(x, y);
            Object = ellipse;
        }
    }
    public class DrawnLine
    {
        public Line Line;
        public DrawnVertex Vertex1;
        public DrawnVertex Vertex2;
    }
   
    public partial class CreatePolygonWindow : Window
    {
        private int SelectedVerticesCount = 3;
        private List<DrawnLine> Lines = new List<DrawnLine>();
        private List<DrawnVertex> Vertices = new List<DrawnVertex>();
        CModel Model;
        public CreatePolygonWindow(CModel model)
        {
            InitializeComponent();
           // ImageComparison.Height = ImageHolder.ActualHeight;
           //ImageComparison.Width = ImageHolder.ActualWidth;
           // MessageBox.Show($"{ImageComparison.Height} x {ImageComparison.Width}");
            Model = model;
        }
        public static CGeoset CreateShape(bool extrude, int Z_Extrusion, List<DrawnLine> Lines, CModel ParentModel)
        {
            CGeoset generated = new CGeoset(ParentModel);
            Dictionary<DrawnVertex, CGeosetVertex> vertexMap = new Dictionary<DrawnVertex, CGeosetVertex>();

            // Create base vertices
            foreach (var line in Lines)
            {
                if (!vertexMap.ContainsKey(line.Vertex1))
                {
                    var v1 = new CGeosetVertex(ParentModel)
                    {
                        Position = new MdxLib.Primitives.CVector3((float)line.Vertex1.Position.X, (float)line.Vertex1.Position.Y, 0)
                    };
                    vertexMap[line.Vertex1] = v1;
                    generated.Vertices.Add(v1);
                }
                if (!vertexMap.ContainsKey(line.Vertex2))
                {
                    var v2 = new CGeosetVertex(ParentModel)
                    {
                        Position = new MdxLib.Primitives.CVector3((float)line.Vertex2.Position.X, (float)line.Vertex2.Position.Y, 0)
                    };
                    vertexMap[line.Vertex2] = v2;
                    generated.Vertices.Add(v2);
                }
            }

            // Triangulate the 2D shape (assumes the lines form a closed polygon, use a triangulation algorithm)
            List<CGeosetVertex> polygonVertices = vertexMap.Values.ToList();
            for (int i = 1; i < polygonVertices.Count - 1; i++)
            {
                CGeosetTriangle triangle = new CGeosetTriangle(ParentModel);
                triangle.Vertex1.Attach(polygonVertices[0]);
                triangle.Vertex2.Attach(polygonVertices[i]);
                triangle.Vertex3.Attach(polygonVertices[i + 1]);
                generated.Triangles.Add(triangle);
            }

            if (extrude)
            {
                Dictionary<DrawnVertex, CGeosetVertex> topVertexMap = new Dictionary<DrawnVertex, CGeosetVertex>();

                // Create extruded vertices
                foreach (var pair in vertexMap)
                {
                    var v = new CGeosetVertex(ParentModel)
                    {
                        Position = new MdxLib.Primitives.CVector3(pair.Value.Position.X, pair.Value.Position.Y, Z_Extrusion)
                    };
                    topVertexMap[pair.Key] = v;
                    generated.Vertices.Add(v);
                }

                // Create side faces
                foreach (var line in Lines)
                {
                    var v1 = vertexMap[line.Vertex1];
                    var v2 = vertexMap[line.Vertex2];
                    var v3 = topVertexMap[line.Vertex2];
                    var v4 = topVertexMap[line.Vertex1];

                    CGeosetTriangle sideTri1 = new CGeosetTriangle(ParentModel);
                    sideTri1.Vertex1.Attach(v1);
                    sideTri1.Vertex2.Attach(v2);
                    sideTri1.Vertex3.Attach(v3);
                    generated.Triangles.Add(sideTri1);

                    CGeosetTriangle sideTri2 = new CGeosetTriangle(ParentModel);
                    sideTri2.Vertex1.Attach(v1);
                    sideTri2.Vertex2.Attach(v3);
                    sideTri2.Vertex3.Attach(v4);
                    generated.Triangles.Add(sideTri2);
                }

                // Create top face
                List<CGeosetVertex> topVertices = topVertexMap.Values.ToList();
                for (int i = 1; i < topVertices.Count - 1; i++)
                {
                    CGeosetTriangle topTriangle = new CGeosetTriangle(ParentModel);
                    topTriangle.Vertex1.Attach(topVertices[0]);
                    topTriangle.Vertex2.Attach(topVertices[i + 1]);
                    topTriangle.Vertex3.Attach(topVertices[i]);
                    generated.Triangles.Add(topTriangle);
                }
            }

            return generated;
        }


        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(InputTextBox.Text, out int val))
            {
                if (val > 2 && val < 101)
                {
                    SelectedVerticesCount = val; DrawPolygon(SelectedVerticesCount);
                }
            }
        }
        private void DrawPolygon(int selectedVerticesCount)
        {
            //------------------------------------
            MainCanvas.Children.Clear();
            Vertices.Clear();
            Lines.Clear();
            //------------------------------------
            // Define the center and radius for the polygon
            double centerX = MainCanvas.ActualWidth / 2;
            double centerY = MainCanvas.ActualHeight / 2;
            double radius = Math.Min(centerX, centerY) * 0.8; // Ensure it fits within the canvas
            // Calculate the angle step for evenly spaced vertices
            double angleStep = 2 * Math.PI / selectedVerticesCount;
            // Create vertices and add them to the canvas
            for (int i = 0; i < selectedVerticesCount; i++)
            {
                // Calculate vertex position
                double angle = i * angleStep;
                double x = centerX + radius * Math.Cos(angle);
                double y = centerY + radius * Math.Sin(angle);
                // Create an Ellipse for the vertex
                Ellipse ellipse = new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    Fill = System.Windows.Media.Brushes.Red,
                    Stroke = System.Windows.Media.Brushes.Black,
                    StrokeThickness = 1
                };
                // Position the ellipse on the canvas
                Canvas.SetLeft(ellipse, x - ellipse.Width / 2);
                Canvas.SetTop(ellipse, y - ellipse.Height / 2);
                MainCanvas.Children.Add(ellipse);
                // Create and store the DrawnVertex
                DrawnVertex vertex = new DrawnVertex(x, y, ellipse);
                Vertices.Add(vertex);
            }
            // Create lines between the vertices
            for (int i = 0; i < selectedVerticesCount; i++)
            {
                DrawnVertex v1 = Vertices[i];
                DrawnVertex v2 = Vertices[(i + 1) % selectedVerticesCount]; // Wrap around to the first vertex
                // Create a line connecting the vertices
                Line line = new Line
                {
                    X1 = v1.Position.X,
                    Y1 = v1.Position.Y,
                    X2 = v2.Position.X,
                    Y2 = v2.Position.Y,
                    Stroke = System.Windows.Media.Brushes.Black,
                    StrokeThickness = 2
                };
                MainCanvas.Children.Add(line);
                // Create and store the DrawnLine
                DrawnLine drawnLine = new DrawnLine { Line = line, Vertex1 = v1, Vertex2 = v2 };
                Lines.Add(drawnLine);
            }
        }
        private Point selectionStartPoint; // The point where the selection starts (MouseDown)
        private Rectangle selectionRectangle; // The rectangle used for selection
        private Rectangle Selection; // The current selection rectangle
        private Point LastSelectionPoint;
        private enum MouseMode
        {
            Select, Move, None
        }
        MouseMode CanvasMode_ = MouseMode.None;
        MouseMode CanvasMode
        {
            get => CanvasMode_;
            set { CanvasMode_ = value; ModeDisplay.Text ="Work mode: "+ CanvasMode_.ToString(); }
        }
        private void SelectionLayer_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Capture the starting point of the selection
            selectionStartPoint = e.GetPosition(SelectionLayer);
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                CanvasMode = MouseMode.Select;
                // Create the selection rectangle (initially 0 size)
                selectionRectangle = new Rectangle
                {
                    Stroke = System.Windows.Media.Brushes.Blue,
                    StrokeThickness = 2,
                    Fill = System.Windows.Media.Brushes.LightBlue,
                    Opacity = 0.5
                };
                // Position it where the selection starts
                Canvas.SetLeft(selectionRectangle, selectionStartPoint.X);
                Canvas.SetTop(selectionRectangle, selectionStartPoint.Y);
                SelectionLayer.Children.Add(selectionRectangle);
                // Mark the selection as in progress
                Selection = selectionRectangle;
                return;
            }
            if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                CanvasMode = MouseMode.Move;
            }
        }
        private void SelectionLayer_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(SelectionLayer);
            // Update the width and height based on the mouse position
            double width = currentPoint.X - selectionStartPoint.X;
            double height = currentPoint.Y - selectionStartPoint.Y;
            if (CanvasMode == MouseMode.Select)
            {
                // Update the rectangle size
                selectionRectangle.Width = Math.Abs(width);
                selectionRectangle.Height = Math.Abs(height);
                // Update the position to move the rectangle with the mouse
                if (width < 0)
                    Canvas.SetLeft(selectionRectangle, currentPoint.X);
                if (height < 0)
                    Canvas.SetTop(selectionRectangle, currentPoint.Y);
                return;
            }
            if (CanvasMode == MouseMode.Move)
            {
                const double amount = 0.4;
                double x = 0;
                double y = 0;
                if (currentPoint.X > selectionStartPoint.X) { x = amount; }
                if (currentPoint.X  < selectionStartPoint.X) { x =  -amount; }
                if (currentPoint.Y > selectionStartPoint.Y) { y = amount; }
                if (currentPoint.Y < selectionStartPoint.Y) { y = -amount; }
                MoveSelectedVertices(x,y );
                ReDraw();
            }
        }
        private void MoveSelectedVertices(double x, double y)
        {
            foreach (var vertex in Vertices)
            {
                if (vertex.Selected)
                {
                    vertex.Position.X += x;
                    vertex.Position.Y += y;
                }
            }
        }
        private void SelectionLayer_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (CanvasMode == MouseMode.Select)
            {
                SelectionLayer.Children.Clear();
                // Select vertices inside the rectangle
                 SelectAffectedVertices(Selection);
            }
            CanvasMode = MouseMode.None;
        }
        private void SelectAffectedVertices(Rectangle rect)
        {
            double rectLeft = Canvas.GetLeft(rect);
            double rectTop = Canvas.GetTop(rect);
            double rectRight = rectLeft + rect.Width;
            double rectBottom = rectTop + rect.Height;
            foreach (var vertex in Vertices)
            {
                // Check if the vertex's position is inside the selection rectangle
                if (vertex.Position.X >= rectLeft && vertex.Position.X <= rectRight &&
                    vertex.Position.Y >= rectTop && vertex.Position.Y <= rectBottom)
                {
                    vertex.Selected = true;
                }
                else
                {
                    if (!ControlHeld)  vertex.Selected = false;
                }
            }
            ReDraw();
        }
        private void ReDraw()
        {
            // Clear the main canvas before redrawing
            MainCanvas.Children.Clear();
            // Redraw the lines
            foreach (var line in Lines)
            {
                Line newLine = new Line
                {
                    X1 = line.Vertex1.Position.X,
                    Y1 = line.Vertex1.Position.Y,
                    X2 = line.Vertex2.Position.X,
                    Y2 = line.Vertex2.Position.Y,
                    Stroke = System.Windows.Media.Brushes.Black,
                    StrokeThickness = 2
                };
                MainCanvas.Children.Add(newLine);
            }
            // Redraw the vertices
            foreach (var vertex in Vertices)
            {
                Ellipse newEllipse = new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    Fill = vertex.Selected ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red,
                    Stroke = System.Windows.Media.Brushes.Black,
                    StrokeThickness = 1
                };
                Canvas.SetLeft(newEllipse, vertex.Position.X - newEllipse.Width / 2);
                Canvas.SetTop(newEllipse, vertex.Position.Y - newEllipse.Height / 2);
                MainCanvas.Children.Add(newEllipse);
            }
        }
        private void loadimage(object sender, RoutedEventArgs e)
        {
            LoadImageIntoControl(ImageComparison);
        }
        private void finish(object sender, RoutedEventArgs e)
        {
            bool extrude = false;
            bool extrusion = int.TryParse(ExtrusionInput.Text, out int extrusionSize);
            if (CheckExtruded.IsChecked == true)
            {
                if (extrusion == false)
                {
                    MessageBox.Show("Extruded is checked, but the input is not a valid integer"); return;
                }
                else
                {
                    if (extrusionSize <= 0)
                    {
                        MessageBox.Show("Extruded is checked, but the input is not a valid integer"); return;
                    }
                }
                extrude = true;
            }
            bool validSides = int.TryParse(InputTextBox.Text, out int validSidesSize);
            if (!validSides) { MessageBox.Show("Invalid input for number of vertices");return; }
            if (validSidesSize < 3 || validSidesSize > 100) { MessageBox.Show("Invalid input for number of vertices"); return; }
            CGeoset geoset = CreateShape(extrude, extrusionSize, Lines, Model);
            AttachVertices(geoset);
            geoset.Material.Attach(Model.Materials[0]);
            Model.Geosets.Add(geoset);
            if (CheckRotate.IsChecked == true) { Calculator.RotateGeoset(geoset, 90, 90, 0); }
            DialogResult = true;
        }

        

        private void AttachVertices(CGeoset geoset)
        {
            CBone bone = new CBone(Model);
            bone.Name = "GeneratedPolygonBone_" + IDCounter.Next_(); ;
            Model.Nodes.Add(bone);  
            CGeosetGroup group = new CGeosetGroup(Model);
            CGeosetGroupNode node = new CGeosetGroupNode(Model);
            node.Node.Attach(bone);
            group.Nodes.Add(node);
            geoset.Groups.Add(group);
            foreach (var vertex in  geoset.Vertices) {vertex.Group.Attach(group); }
        }

        public static void LoadImageIntoControl(Image imageControl)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp",
                Title = "Select an Image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(openFileDialog.FileName, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; // Prevents file locking

                    // Only set DecodePixelWidth/Height if the control has a valid size
                    if (imageControl.ActualWidth > 0 && imageControl.ActualHeight > 0)
                    {
                        bitmap.DecodePixelWidth = (int)imageControl.ActualWidth;
                        bitmap.DecodePixelHeight = (int)imageControl.ActualHeight;
                    }

                    bitmap.EndInit();
                    bitmap.Freeze(); // Freezing allows safe cross-thread UI updates

                    imageControl.Source = bitmap;
                    imageControl.Stretch = System.Windows.Media.Stretch.Uniform; // Preserve aspect ratio
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool ControlHeld = false;
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.LeftCtrl) { ControlHeld = true; }
            if (e.Key == Key.Enter) finish(null, null);
        }
        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ControlHeld = false;
        }
        private void EnableExtruded(object sender, RoutedEventArgs e)
        {
            ExtrusionInput.IsEnabled = CheckExtruded.IsChecked == true;
        }

        private void import(object sender, RoutedEventArgs e)
        {
            string loadPath = GetOpenPath();
            if (loadPath.Length > 0)
            {
               
                 
                    string[] lines = File.ReadLines(loadPath).Select(x=>x.Trim()).ToArray();
                foreach (var item in lines)
                {
                    if (item.Split(' ').Length != 2) { MessageBox.Show("Invalid file"); return; }
                }

                if (lines.Length < 3  || lines.Length > 100) { MessageBox.Show("Invalid file"); return; }
                Vertices.Clear();
                List< Vector2> tempList = new List< Vector2>();
                SelectedVerticesCount = lines.Length;
                DrawPolygon(SelectedVerticesCount);
                
                for (int i = 0; i< Vertices.Count; i++)
                {
                    string[] parts = lines[i].Split(" ");
                    float x = float.Parse(parts[0]);  float y = float.Parse(parts[1]);
                    Vertices[i].Position = new Point(x,y);
                    
                }
                ReDraw();
                
            }
        }
        private string GetOpenPath()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "TLI Files (*.tli)|*.tli",
                DefaultExt = ".tli",
                Title = "Open File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return "";
        }
        private void export(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(InputTextBox.Text, out int val))
            {
                if (val > 2 && val < 101)
                {
                    SelectedVerticesCount = val;
                    string content = GetVerticesList();
                    string savePath = GetSavePath();
                        if (savePath.Length > 0)
                    {
                        File.WriteAllText(savePath, content);
                    }

                }
                else
                {
                    MessageBox.Show("Invalid input for nuber of vertices. Must be between 3 and 100");
                }
            }
            else
            {
                MessageBox.Show("Invalid input for nuber of vertices");
            }
        }

        private string GetSavePath()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "TLI Files (*.tli)|*.tli",
                DefaultExt = ".tli",
                Title = "Save File"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }

            return "";
        }

        string GetVerticesList()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var vertex in Vertices)
            {
                sb.AppendLine($"{vertex.Position.X} {vertex.Position.Y}");
            }
            return sb.ToString();
        }
    }
}
