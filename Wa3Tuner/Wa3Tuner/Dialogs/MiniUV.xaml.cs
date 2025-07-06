using MdxLib.Model;
using MdxLib.Primitives;
using SharpGL.SceneGraph;
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
using Path = System.IO.Path;
using W3_Texture_Finder;
using Wa3Tuner.Helper_Classes;

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for MiniUV.xaml
    /// </summary>
    public partial class MiniUV : Window
    {
        List<CGeosetTriangle> Triangles = new List<CGeosetTriangle> ();
        List<CGeosetVertex> Vertices = new List<CGeosetVertex> ();
        CGeoset? FirstGeoset;
        CModel Model;
        private Dictionary<CGeosetVertex, List<CVector2>> History = new();
        private int CurrentHistoryIndex = 0;
        private int CurrentGridSize = 0;


private float ImageWidth, ImageHeight;

        public MiniUV(List<CGeosetTriangle> g, ListBox l, CModel m)
        {
            InitializeComponent();
            Triangles = g;
            Model = m;
            ButtonCopy.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "copy.png")) };
            ButtonCopyM.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "copy.png")) };
            ButtonPaste.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "paste.png")) };
            ButtonPasteM.Background = new ImageBrush() { ImageSource = ImageLoader.LoadPNGImageSource(Path.Combine(AppHelper.IconFolder, "paste.png")) };
            Vertices = Triangles.SelectMany(t => t.Vertices).Distinct(). ToList();
            FirstGeoset = FindGeoset();
            if (FirstGeoset == null) { Hide(); }
            Fill(m, l);
            InitiateHistory();
        }

        private CGeoset? FindGeoset()
        {
           foreach (var geoset in Model.Geosets)
            {
                if (geoset.Triangles.Contains(Triangles[0]))return geoset;
            }
            return null;
        }

        private void InitiateHistory()
        {
            
            CurrentHistoryIndex = 0;
            History.Clear();
            foreach (var vertex in  Vertices)
            {
                if (History.ContainsKey(vertex)) continue;
                History.Add(vertex, new List<CVector2>());
                History[vertex].Add(new CVector2(vertex.TexturePosition));
            }
        }
        private void AddHistory()
        {
            CurrentHistoryIndex++;
            foreach (var vertex in  Vertices)
            {
                History[vertex].Add(new CVector2(vertex.TexturePosition));
            }
        }

        private void Fill(CModel m, ListBox list)
        {

            SelectonContainer_MouseMove(null,null);
            if (FirstGeoset  == null) {Close(); return;}
            if (FirstGeoset.Material.Object == null) {Close(); return;}
            if (FirstGeoset.Material.Object.Layers.Count==0) {Close(); return;}
            var l = FirstGeoset.Material.Object.Layers[FirstGeoset.Material.Object.Layers.Count - 1];
            var t = l.Texture.Object;
            int index = m.Textures.IndexOf(t);
            ListBoxItem? i = list.Items[index] as ListBoxItem; if (i == null) { return; }
            Image? im = i.ToolTip as Image; if (im == null) { return; }
            image.Source = ResizeImageSource(im.Source);
            ImageWidth =(float)im.Source.Width;
            ImageHeight = (float)im.Source.Height;  
            RefreshUVRender();
        }
        private static ImageSource? ResizeImageSource(ImageSource source, int width = 512, int height = 512)
        {
            if (source == null)
                return null;

            // Convert to BitmapSource if it's not already
            if (source is not BitmapSource bitmapSource)
                return null;

            // Create a DrawingVisual to draw the resized image
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(bitmapSource, new System.Windows.Rect(0, 0, width, height));
            }

            // Render the visual to a RenderTargetBitmap
            RenderTargetBitmap resizedBitmap = new RenderTargetBitmap(
                width, height,       // pixel width and height
                96, 96,              // dpiX and dpiY
                PixelFormats.Pbgra32);

            resizedBitmap.Render(drawingVisual);

            return resizedBitmap;
        }
        private void RefreshUVRender()
        {
            DrawCanvas.Children.Clear(); 
            Brush edge = Brushes.Blue;

            foreach (CGeosetTriangle triangle in  Triangles)
            {
                var vertex1 = triangle.Vertex1.Object;
                var vertex2 = triangle.Vertex2.Object;
                var vertex3 = triangle.Vertex3.Object;

                Brush color1 = !vertex1.isSelected ? Brushes.Red : Brushes.Green;
                Brush color2 = !vertex2.isSelected ? Brushes.Red : Brushes.Green;
                Brush color3 = !vertex3.isSelected ? Brushes.Red : Brushes.Green;

                float[] positionInImage1 = NormalizedPositionToImagePosition(vertex1.TexturePosition.X, vertex1.TexturePosition.Y);
                float[] positionInImage2 = NormalizedPositionToImagePosition(vertex2.TexturePosition.X, vertex2.TexturePosition.Y);
                float[] positionInImage3 = NormalizedPositionToImagePosition(vertex3.TexturePosition.X, vertex3.TexturePosition.Y);

                // Draw triangle edges
                DrawLine(positionInImage1, positionInImage2, edge);
                DrawLine(positionInImage2, positionInImage3, edge);
                DrawLine(positionInImage3, positionInImage1, edge);

                // Draw UV points
                DrawUVPoint(positionInImage1, color1);
                DrawUVPoint(positionInImage2, color2);
                DrawUVPoint(positionInImage3, color3);
            }
        }

        private void DrawLine(float[] p1, float[] p2, Brush stroke)
        {
            Line line = new Line
            {
                X1 = p1[0],
                Y1 = p1[1],
                X2 = p2[0],
                Y2 = p2[1],
                Stroke = stroke,
                StrokeThickness = 1
            };
            DrawCanvas.Children.Add(line);
        }

        private void DrawUVPoint(float[] position, Brush fill)
        {
            Ellipse point = new Ellipse
            {
                Width = 4,
                Height = 4,
                Fill = fill
            };
            Canvas.SetLeft(point, position[0] - 2); // center the point
            Canvas.SetTop(point, position[1] - 2);
            DrawCanvas.Children.Add(point);
        }
        private static float[] NormalizedPositionToImagePosition(
      float normalizedX, // 0 - 1
      float normalizedY  // 0 - 1
  )
        {
            if (normalizedX < 0 || normalizedX > 1 || normalizedY < 0 || normalizedY > 1)
            {
                return new float[] { 0, 0 };
            }

            float imageHeight = 512;
            float imageWidth = 512;

            float x = normalizedX * imageWidth;
            float y = normalizedY * imageHeight;

            return new float[] { x, y };
        }


        enum MouseMode
        {
            None, Selecting, Dragging, Rotating, Scaling,
            ScalingHor,
            ScalingVer
        }

        private MouseMode mouseMode = MouseMode.None;
        private Point selectionStart; // screen coordinates
        private Rectangle selectionRectangle = new Rectangle
        {
            Stroke = Brushes.White,
            StrokeThickness = 1,
            StrokeDashArray = new DoubleCollection { 2 }
        };

        private Point lastDragMousePos;

        // Mouse down
        private void SelectonContainer_MouseDown(object? sender, MouseButtonEventArgs e)
        {
            
            if (e.ChangedButton == MouseButton.Left)
            {
                mouseMode = MouseMode.Selecting;
                selectionStart = e.GetPosition(SelectonContainer);
                if (!SelectonContainer.Children.Contains(selectionRectangle))
                    SelectonContainer.Children.Add(selectionRectangle);
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                mouseMode = MouseMode.Dragging;
                lastDragMousePos = e.GetPosition(SelectonContainer);
            }

            Mouse.Capture(SelectonContainer);
        }

        // Mouse up
        private void SelectonContainer_MouseUp(object? sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);

            if (mouseMode == MouseMode.Selecting )
            {
                FinalizeSelection(); // Let this handle UV bounds + selection
            }
            if (mouseMode == MouseMode.Dragging)
            {
                AddHistory();
                // update maniual input
                List<CGeosetVertex> Selected = Vertices.Where(x=>x.isSelected).ToList();
                SetManualInput(Selected);
            }
            SelectonContainer.Children.Remove(selectionRectangle);
            mouseMode = MouseMode.None;
          
                
        }
        Point LastMousePosition = new Point();
        // Mouse move
        private void SelectonContainer_MouseMove(object? sender, MouseEventArgs? e)
        {
            if (e == null) return;

            if (mouseMode == MouseMode.Selecting)
            {
                Point current = e.GetPosition(SelectonContainer);

                double x = Math.Min(current.X, selectionStart.X);
                double y = Math.Min(current.Y, selectionStart.Y);
                double width = Math.Abs(current.X - selectionStart.X);
                double height = Math.Abs(current.Y - selectionStart.Y);

                Canvas.SetLeft(selectionRectangle, x);
                Canvas.SetTop(selectionRectangle, y);
                selectionRectangle.Width = width;
                selectionRectangle.Height = height;
            }
            else if (mouseMode == MouseMode.Dragging)
            {

                Point current = e.GetPosition(SelectonContainer);
                Direction d = Direction.None;
                if (current.X < LastMousePosition.X)d = Direction.Left;
                if (current.X > LastMousePosition.X)d = Direction.Right;
                if (current.Y > LastMousePosition.Y) d = Direction.Down;
                if (current.Y < LastMousePosition.Y) d = Direction.Up;

                MoveVertices(d,0.5f);
                LastMousePosition = current;    
            }
            WriteMouseCoordinates();

        }

        private void WriteMouseCoordinates()
        {
            CVector2 uv = GetMousePositionAsUVPosition();

            // If the mouse is outside the image, uv will be (0, 0)
            // Optionally, you can treat that as an invalid position
            if (uv.X < 0f || uv.X > 1f || uv.Y < 0f || uv.Y > 1f)
            {
                Label_U.Text = string.Empty;
                Label_V.Text = string.Empty;
                return;
            }

            // Show the UV coordinates with a bit of precision
            Label_U.Text = $"U: {uv.X:F3}";
            Label_V.Text = $"V: {uv.Y:F3}";
        }


        private void FinalizeSelection()
        {
            Point start = selectionStart;
            Point end = Mouse.GetPosition(SelectonContainer);

            double left = Math.Min(start.X, end.X);
            double right = Math.Max(start.X, end.X);
            double top = Math.Min(start.Y, end.Y);
            double bottom = Math.Max(start.Y, end.Y);

            double containerWidth = 512;
            double containerHeight = 512;

            // Flip Y for UVs
            float uvLeft = (float)(left / containerWidth);
            float uvRight = (float)(right / containerWidth);
            float uvTop =  (float)(top / containerHeight);
            float uvBottom =   (float)(bottom / containerHeight);

            // Ensure proper bounds
            float finalLeft = Math.Min(uvLeft, uvRight);
            float finalRight = Math.Max(uvLeft, uvRight);
            float finalTop = Math.Max(uvTop, uvBottom);     // top is HIGHER in UV
            float finalBottom = Math.Min(uvTop, uvBottom);  // bottom is LOWER in UV
            //MessageBox.Show("called finalize");
            SelectAffectedVertices(finalRight, finalLeft, finalTop, finalBottom);
        }







        // Select affected vertices within bounds
        private void SelectAffectedVertices(float rightUV, float leftUV, float topUV, float bottomUV)
        {
             
            if (AltPressed)
            {
               
                List<CGeosetVertex> preSelected = Vertices.Where(x => x.isSelected).ToList();
                if (preSelected.Count == 4)
                {
                    preSelected[0].TexturePosition = new(leftUV, topUV);      // Top-left
                    preSelected[1].TexturePosition = new(rightUV, topUV);     // Top-right
                    preSelected[2].TexturePosition = new(rightUV, bottomUV);   // Bottom-left
                    preSelected[3].TexturePosition = new(leftUV, bottomUV);  // Bottom-right
                    RefreshUVRender();
                }
               

            }

                    
            List<CGeosetVertex> Selected = new List<CGeosetVertex>();
            foreach (var vertex in Vertices)
            {
                bool selected =  VertexInsideSelection(vertex, rightUV, leftUV, topUV, bottomUV);
                vertex.isSelected = selected;
                if (selected) Selected.Add(vertex);
            }
            SetManualInput(Selected);
            RefreshUVRender();
            RefreshTitle();
        }

        private void SetManualInput(List<CGeosetVertex> selected)
        {
            if (selected.Count == 0)
            {
                InputU.Text = string.Empty;
                InputV.Text = string.Empty;
            }
            else if (selected.Count == 1)
            {
                InputU.Text = selected[0].TexturePosition.X.ToString();
                InputV.Text = selected[0].TexturePosition.Y.ToString();
            }
            else
            {
                CVector2 c = Calculator.GetCentroidOfUV(selected.Select(x=>x.TexturePosition).ToList());
                InputU.Text = c.X.ToString();
                InputV.Text = c.Y.ToString();
            }
        }


        // Check if vertex is inside selection area
        private static bool VertexInsideSelection(CGeosetVertex vertex, float rightX, float leftX, float topY, float bottomY)
        {
            float x = vertex.TexturePosition.X;
            float y = vertex.TexturePosition.Y;
            return (x >= leftX && x <= rightX) && (y >= bottomY && y <= topY);
        }

        enum Side
        {
            Front, Back, Top, Right, Left, Bottom
        }
        private void Project(Side side)
        {
            var SelectedVertices = Vertices.Where(x => x.isSelected).ToList();
            if (SelectedVertices.Count == 0)
                return;

            // Calculate bounding box
            float minX = SelectedVertices.Min(v => v.Position.X);
            float maxX = SelectedVertices.Max(v => v.Position.X);
            float minY = SelectedVertices.Min(v => v.Position.Y);
            float maxY = SelectedVertices.Max(v => v.Position.Y);
            float minZ = SelectedVertices.Min(v => v.Position.Z);
            float maxZ = SelectedVertices.Max(v => v.Position.Z);

            float deltaX = Math.Max(maxX - minX, 0.0001f);
            float deltaY = Math.Max(maxY - minY, 0.0001f);
            float deltaZ = Math.Max(maxZ - minZ, 0.0001f);

            foreach (var vertex in SelectedVertices)
            {
                float x = vertex.Position.X;
                float y = vertex.Position.Y;
                float z = vertex.Position.Z;

                float projectedX = 0;
                float projectedY = 0;

                switch (side)
                {
                    case Side.Top:
                    case Side.Bottom:
                        // Project X and Z
                        projectedX = (x - minX) / deltaX;
                        projectedY = (z - minZ) / deltaZ;
                        break;

                    case Side.Front:
                    case Side.Back:
                        // Project X and Y
                        projectedX = (x - minX) / deltaX;
                        projectedY = (y - minY) / deltaY;
                        break;

                    case Side.Left:
                    case Side.Right:
                        // Project Z and Y
                        projectedX = (z - minZ) / deltaZ;
                        projectedY = (y - minY) / deltaY;
                        break;

                    default:
                        break;
                }

                // Clamp to [0,1] to be safe
                projectedX = Math.Clamp(projectedX, 0f, 1f);
                projectedY = Math.Clamp(projectedY, 0f, 1f);

                vertex.TexturePosition.X = projectedX;
                vertex.TexturePosition.Y = projectedY;
            }

            RefreshUVRender();
        }


        private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
        enum Direction
        {
            Up, Down,Left, Right,
            None
        }
        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            AltPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);
            if (e.Key == Key.Escape) { Hide(); }
           else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.Z) Undo();
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.Y) Redo();
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.E) pasteUVEach(null,null);
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.C) copyUV(null,null);
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) pasteUV(null,null);

            else if (e.Key == Key.Left) { MoveVertices(Direction.Left); }
            else if (e.Key == Key.Right) { MoveVertices(Direction.Right); }
            else if (e.Key == Key.Up) { MoveVertices(Direction.Up); }
            else if (e.Key == Key.Down) { MoveVertices(Direction.Down); }
            else if (e.Key == Key.F) { Project(Side.Front); }
            else if (e.Key == Key.C) { Project(Side.Back); }
            else if (e.Key == Key.R) { Project(Side.Right); }
            else if (e.Key == Key.L) { Project(Side.Left); }
            else if (e.Key == Key.B) { Project(Side.Bottom); }
            else if (e.Key == Key.T) { Project(Side.Top); }
            else if (e.Key == Key.A) { SelectAll(); }
            else if (e.Key == Key.M) { copuUV_Mouse(null, null); }
            else if (e.Key == Key.N) { SelectNone(); }
            else if (e.Key == Key.G) { AlignHorizontally(); }
            else if (e.Key == Key.Y) { AlignVertically(); }
            else if (e.Key == Key.S) { SnapSelectionToMousePosition(null, null); }
            else if (e.Key == Key.Q) { SnapSelectionToMousePositionC(null, null); }

            else if (e.Key == Key.I) { SelectInvert(); }
            else if (e.Key == Key.H) { FlipHorizontally(); }
            else if (e.Key == Key.V) { FlipVertically(); }
            else if (e.Key == Key.W) { SwapTwoVertices(); }

            else if (e.Key == Key.O) { FormAsShape(); }
            else if (e.Key == Key.Z) { FitVerticesInsideImage(); }

            else if (e.Key == Key.LeftCtrl) { mouseMode = MouseMode.Rotating; }

            else if (e.Key == Key.LeftAlt) { mouseMode = MouseMode.ScalingHor; }
            else if (e.Key == Key.LeftShift) { mouseMode = MouseMode.ScalingVer; }
           

        }

        private void FitVerticesInsideImage()
        {
            var SelectedVertices = Vertices.Where(x => x.isSelected).ToList();

            // Ensure there are exactly 4 vertices selected
            if (SelectedVertices.Count != 4)
            {
                MessageBox.Show( "Select exactly 4 vertices");
                return; // Do nothing if the number of selected vertices is not 4
            }

            // Find the bounding box of the selected vertices
            float minX = SelectedVertices.Min(v => v.TexturePosition.X);
            float maxX = SelectedVertices.Max(v => v.TexturePosition.X);
            float minY = SelectedVertices.Min(v => v.TexturePosition.Y);
            float maxY = SelectedVertices.Max(v => v.TexturePosition.Y);

            // Calculate the width and height of the bounding box
            float boxWidth = maxX - minX;
            float boxHeight = maxY - minY;

            // Define the target image dimensions (0-1 UV space)
            float targetWidth = 1f;
            float targetHeight = 1f;

            // Calculate scale factors
            float scaleX = targetWidth / boxWidth;
            float scaleY = targetHeight / boxHeight;

            // Find the scale to use for uniform scaling
            float scale = Math.Min(scaleX, scaleY);

            // Find the center of the bounding box
            float centerX = minX + boxWidth / 2;
            float centerY = minY + boxHeight / 2;

            // Calculate the new position for each vertex to fit within the image bounds
            foreach (var vertex in SelectedVertices)
            {
                // Translate vertex to center at (0, 0), scale it, and then move it to the center of the image
                float newX = (vertex.TexturePosition.X - centerX) * scale + targetWidth / 2;
                float newY = (vertex.TexturePosition.Y - centerY) * scale + targetHeight / 2;

                // Clamp the new position to ensure it stays within bounds
                newX = Math.Clamp(newX, 0f, 1f);
                newY = Math.Clamp(newY, 0f, 1f);

                // Update the vertex position
                vertex.TexturePosition.X = newX;
                vertex.TexturePosition.Y = newY;
            }

            // Refresh the UV render after updating the vertex positions
            RefreshUVRender();
        }


        private void FormAsShape()
        {
            var SelectedVertices = Vertices.Where(x => x.isSelected).ToList();

            // Use ImageWidth and ImageHeight to get the center of the image
            float centerX = ImageWidth / 2f;
            float centerY = ImageHeight / 2f;

            // If fewer than 3 vertices are selected, can't form a shape
            if (SelectedVertices.Count < 3)
            {
                MessageBox.Show("Select more than 2 vertices");
                return;
            }

            // Define a 20% radius for the shape
            float radiusPercentage = 0.2f; // 20% of the image size
            float maxRadiusX = ImageWidth * radiusPercentage;
            float maxRadiusY = ImageHeight * radiusPercentage;

            // Step 1: Calculate the angle for each vertex and reposition them around the center
            int numVertices = SelectedVertices.Count;
            float angleStep = 360f / numVertices;

            for (int i = 0; i < numVertices; i++)
            {
                float angle = i * angleStep; // Angle for this vertex
                float angleInRadians = (float)(angle * Math.PI / 180);

                // Calculate the new position around the center of the image with the 20% radius
                float newX = centerX + maxRadiusX * (float)Math.Cos(angleInRadians);
                float newY = centerY + maxRadiusY * (float)Math.Sin(angleInRadians);

                // Normalize the coordinates to UV space [0, 1]
                newX = Math.Clamp(newX / ImageWidth, 0f, 1f); // Clamp X to [0, 1]
                newY = Math.Clamp(newY / ImageHeight, 0f, 1f); // Clamp Y to [0, 1]

                // Update the vertex position
                var vertex = SelectedVertices[i];
                vertex.TexturePosition.X = newX;
                vertex.TexturePosition.Y = newY;
            }

            // Refresh the UV render after updating the vertex positions
            RefreshUVRender();
        }



        private void DisperseOverlappingVertices()
        {
            throw new NotImplementedException();
        }

        private void SwapTwoVertices()
        {
            var SelectedVertices = Vertices.Where(x => x.isSelected).ToList();
            if (SelectedVertices.Count == 2)
            {
                float x = SelectedVertices[1].TexturePosition.X;
                float y = SelectedVertices[1].TexturePosition.Y;
                SelectedVertices[1].TexturePosition = new CVector2(SelectedVertices[0].TexturePosition.X, SelectedVertices[0].TexturePosition.Y);
                SelectedVertices[0].TexturePosition = new CVector2(x, y);

            }
            else { MessageBox.Show("Select exactly 2 vertices"); }
        }

        private void AlignVertically()
        {
            var selectedVertices =  Vertices.Where(x => x.isSelected).ToList();
            if (selectedVertices.Count < 2) {return; }  
            for (int i = 1; i < selectedVertices.Count; i++)
            {
                selectedVertices[i].TexturePosition.Y = selectedVertices[0].TexturePosition.Y;
            }
            RefreshUVRender();
        }

        private void AlignHorizontally()
        {
            var selectedVertices = Vertices.Where(x => x.isSelected).ToList();
            if (selectedVertices.Count < 2) { return; }
            for (int i = 1; i < selectedVertices.Count; i++)
            {
                selectedVertices[i].TexturePosition.X = selectedVertices[0].TexturePosition.X;
            }
            RefreshUVRender();
        }

        private void MoveVertices(Direction direction, float stepPercent = 1)
        {
            if (direction == Direction.None) return;
            var selectedVertices = Vertices.Where(x => x.isSelected).ToList();

            if (selectedVertices.Count == 0)
                return;

            // Calculate step size based on image dimensions
            float stepX = (1f / ImageWidth) * stepPercent;  // Horizontal step based on image width
            float stepY = (1f / ImageHeight) * stepPercent; // Vertical step based on image height

            foreach (var vertex in selectedVertices)
            {
                switch (direction)
                {
                    case Direction.Left:
                        vertex.TexturePosition.X -= stepX;  // Move left
                        break;
                    case Direction.Right:
                        vertex.TexturePosition.X += stepX;  // Move right
                        break;
                    case Direction.Up:
                        vertex.TexturePosition.Y -= stepY;  // Move up
                        break;
                    case Direction.Down:
                        vertex.TexturePosition.Y += stepY;  // Move down
                        break;
                }

                // Clamp the values to stay within UV bounds (0 to 1)
                vertex.TexturePosition.X = Math.Clamp(vertex.TexturePosition.X, 0f, 1f);
                vertex.TexturePosition.Y = Math.Clamp(vertex.TexturePosition.Y, 0f, 1f);
            }

            RefreshUVRender();
        }

        private void Window_KeyUp(object? sender, KeyEventArgs e)
        {

              if (e.Key == Key.LeftCtrl) { mouseMode = MouseMode.None;}
            else if (e.Key == Key.LeftShift) { mouseMode = MouseMode.None; }
            else if (e.Key == Key.LeftAlt) { mouseMode = MouseMode.None;  }
            else if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
            {
                AddHistory();
            }
            AltPressed = false;
        }

        private void FlipVertically()
        {
            var SelectedVertices = Vertices.Where(x => x.isSelected).ToList();

            foreach (var vr in SelectedVertices)
            {
                
                vr.TexturePosition.Y = Calculator.FlipV(vr.TexturePosition.Y);
            }
            RefreshUVRender();
        }

        private void FlipHorizontally()
        {
            var SelectedVertices = Vertices.Where(x => x.isSelected).ToList();

            foreach (var vr in SelectedVertices)
            {
                
                vr.TexturePosition.Y = Calculator.FlipV(vr.TexturePosition.Y);
            }
           RefreshUVRender();
        }

      

        private void ActivateScaleMode()
        {
            mouseMode = MouseMode.Scaling;
        }

        private void SelectInvert()
        {
            foreach (var vertex in Vertices)
            {
                vertex.isSelected = !vertex.isSelected;
            }
            RefreshUVRender();
            RefreshTitle();
        }

        private void SelectNone()
        {
          
            
            foreach (var vertex in  Vertices) { vertex.isSelected = false; }
            RefreshUVRender();
            RefreshTitle();
        }
        private string SelectedCount = "0";
        private void RefreshTitle()
        {
            SelectedCount =  Vertices.Count(x => x.isSelected).ToString();
           Title = $"Mini UV Mapper - Selected {SelectedCount} vertices";
        } 

        private void SelectAll()
        {
            foreach (var vertex in  Vertices) vertex.isSelected = true;
            RefreshUVRender();
            RefreshTitle();
        }

      
        private void SelectonContainer_MouseWheel(object? sender, MouseWheelEventArgs e)
        {
            var SelectedVertices =  Vertices.Where(x => x.isSelected).ToList();
            if (e.Delta > 0)
            {
              
                if (mouseMode == MouseMode.Rotating) { RotateVertices(SelectedVertices, true); }
                
                else
                {
                    ScaleVertices(SelectedVertices, true);
                }
            }
            else if (e.Delta < 0)
            {
                
                if (mouseMode == MouseMode.Rotating)
                {
                    RotateVertices(SelectedVertices, false);
                }
                else  
                {
                    ScaleVertices(SelectedVertices, false);
                }
            }
                
            }

        private void RotateVertices(List<CGeosetVertex> selectedVertices, bool Clockwise)
        {
            if (selectedVertices.Count == 0)
                return;

            // Rotation angle: 1 degree in radians
            float angle = (float)(Math.PI / 180); // 1 degree
            if (!Clockwise)
                angle = -angle;

            // Calculate centroid
            float centerX = selectedVertices.Average(v => v.TexturePosition.X);
            float centerY = selectedVertices.Average(v => v.TexturePosition.Y);

            foreach (var vertex in selectedVertices)
            {
                float offsetX = vertex.TexturePosition.X - centerX;
                float offsetY = vertex.TexturePosition.Y - centerY;

                float rotatedX = offsetX * (float)Math.Cos(angle) - offsetY * (float)Math.Sin(angle);
                float rotatedY = offsetX * (float)Math.Sin(angle) + offsetY * (float)Math.Cos(angle);

                vertex.TexturePosition.X = Math.Clamp(centerX + rotatedX, 0f, 1f);
                vertex.TexturePosition.Y = Math.Clamp(centerY + rotatedY, 0f, 1f);
            }

            RefreshUVRender();
        }



        private void ScaleVertices(List<CGeosetVertex> selectedVertices, bool increase)
        {
            if (selectedVertices.Count == 0)
                return;

            // Scale steps (1% per operation)
            float horizontalStep = 0.01f;
            float verticalStep = 0.01f;

            // Compute the centroid of selected vertices
            float centerX = selectedVertices.Average(v => v.TexturePosition.X);
            float centerY = selectedVertices.Average(v => v.TexturePosition.Y);

            float scaleX = 1f;
            float scaleY = 1f;

            // Determine scale direction based on mouseMode
            if (mouseMode == MouseMode.ScalingHor)
            {
                scaleX = increase ? 1 + horizontalStep : 1 - horizontalStep;  // Horizontal scaling
            }
            else if (mouseMode == MouseMode.ScalingVer)
            {
                scaleY = increase ? 1 + verticalStep : 1 - verticalStep;  // Vertical scaling
            }
            else
            {
                scaleX = increase ? 1 + horizontalStep : 1 - horizontalStep;  // Both axes
                scaleY = increase ? 1 + verticalStep : 1 - verticalStep;
            }

            // Apply scaling based on the selected mode
            foreach (var vertex in selectedVertices)
            {
                float offsetX = vertex.TexturePosition.X - centerX;
                float offsetY = vertex.TexturePosition.Y - centerY;

                float newX = centerX + offsetX * scaleX;
                float newY = centerY + offsetY * scaleY;

                vertex.TexturePosition.X = Math.Clamp(newX, 0f, 1f);
                vertex.TexturePosition.Y = Math.Clamp(newY, 0f, 1f);
            }

            RefreshUVRender();
        }

        private void a(object? sender, RoutedEventArgs? e)
        {
            SelectAll();
        }

        private void n(object? sender, RoutedEventArgs? e)
        {
            SelectNone();
        }

        private void i(object? sender, RoutedEventArgs? e)
        {
            SelectInvert();
        }

        private void projT(object? sender, RoutedEventArgs? e)
        {
            Project(Side.Top);
        }

        private void projB(object? sender, RoutedEventArgs? e)
        {
            Project(Side.Bottom);
        }

        private void projF(object? sender, RoutedEventArgs? e)
        {
            Project(Side.Front);
        }

        private void projC(object? sender, RoutedEventArgs? e)
        {
            Project(Side.Back);
        }

        private void pojL(object? sender, RoutedEventArgs? e)
        {
            Project(Side.Left);
        }

        private void pojr(object? sender, RoutedEventArgs? e)
        {
            Project(Side.Right);
        }

        private void ft(object? sender, RoutedEventArgs? e)
        {
            FitVerticesInsideImage();
        }

        private void sw(object? sender, RoutedEventArgs? e)
        {
            SwapTwoVertices();
        }

        private void au(object? sender, RoutedEventArgs? e)
        {
            AlignHorizontally();
        }

        private void av(object? sender, RoutedEventArgs? e)
        {
            AlignVertically();
        }

        private void fl1(object? sender, RoutedEventArgs? e)
        {
            FlipHorizontally();
        }

        private void fl2(object? sender, RoutedEventArgs? e)
        {
            FlipVertically();
        }

        private void sh(object? sender, RoutedEventArgs? e)
        {
            FormAsShape();
        }
        private CVector2 CopiedVector = new CVector2();
        private bool AltPressed = false;

        private void copyUV(object? sender, RoutedEventArgs? e)
        {
            var SelectedVertices = Vertices.Where(x => x.isSelected).ToList();
            if (SelectedVertices.Count ==1)
            {
                CopiedVector = new CVector2( SelectedVertices[0].TexturePosition);
            }
            if (SelectedVertices.Count  > 1)
            {
                CopiedVector = GetUVCentroid(SelectedVertices);

            }
        }

        private static CVector2 GetUVCentroid(List<CGeosetVertex> selectedVertices)
        {
            CVector2 v = new CVector2(0f, 0f);

            if (selectedVertices == null || selectedVertices.Count == 0)
                return v;

            foreach (var vertex in selectedVertices)
            {
                v.X += vertex.TexturePosition.X;
                v.Y += vertex.TexturePosition.Y;
            }

            v.X /= selectedVertices.Count;
            v.Y /= selectedVertices.Count;

            return v;
        }

        private void pasteUV(object? sender, RoutedEventArgs? e)
        {
            var SelectedVertices = Vertices.Where(x => x.isSelected).ToList();
            if (SelectedVertices.Count == 0) { return; }
            {
                if (SelectedVertices.Count == 1)
                {
                    SelectedVertices[0].TexturePosition = new CVector2(CopiedVector);

                }
                if (SelectedVertices.Count > 1)
                {
                    MoveUVCentroid(SelectedVertices, CopiedVector);

                }
                RefreshUVRender();
            }
        }

        private static void MoveUVCentroid(List<CGeosetVertex> vertices, CVector2 targetCentroid)
        {
            if (vertices == null || vertices.Count == 0)
                return;

            CVector2 currentCentroid = GetUVCentroid(vertices);
            float offsetX = targetCentroid.X - currentCentroid.X;
            float offsetY = targetCentroid.Y - currentCentroid.Y;

            foreach (var vertex in vertices)
            {
                vertex.TexturePosition.X += offsetX;
                vertex.TexturePosition.Y += offsetY;
            }
        }

        private void AverageSimilarUVCoordinates(object? sender, RoutedEventArgs? e)
        {
            // if all uv coordinates are within +-1% close to each other, get the centroid of all uv coordinates and set that centroid to all vertices
            var SelectedVertices = Vertices.Where(x => x.isSelected).ToList();
            if (SelectedVertices.Count > 1)
            {
                CVector2 first = SelectedVertices[0].TexturePosition;
                float Width1Percent = 1f / ImageWidth;   // size of 1% width
                float Height1Percent = 1f / ImageHeight; // size of 1% height

                bool allClose = true;
                for (int i = 1; i < SelectedVertices.Count; i++)
                {
                    var uv = SelectedVertices[i].TexturePosition;
                    if (Math.Abs(uv.X - first.X) > Width1Percent || Math.Abs(uv.Y - first.Y) > Height1Percent)
                    {
                        allClose = false;
                        break;
                    }
                }

                if (allClose)
                {
                    float sumX = 0f;
                    float sumY = 0f;
                    foreach (var vertex in SelectedVertices)
                    {
                        sumX += vertex.TexturePosition.X;
                        sumY += vertex.TexturePosition.Y;
                    }

                    float averageX = sumX / SelectedVertices.Count;
                    float averageY = sumY / SelectedVertices.Count;
                    var centroid = new CVector2(averageX, averageY);

                    foreach (var vertex in SelectedVertices)
                    {
                        vertex.TexturePosition =new CVector2( centroid);
                    }
                }
            }
            RefreshUVRender();
        }


        internal void Update(List<CGeosetTriangle> g, ListBox l, CModel m)
        {
            Triangles = g;
            Vertices = Triangles.SelectMany(t => t.Vertices).ToList();
            FirstGeoset = FindGeoset();
            Fill(m, l);
            History.Clear();

            InitiateHistory();
        }
        private void UpdateFromHistory()
        {
            foreach (var vertex in Vertices)
            {
                vertex.TexturePosition = new CVector2(History[vertex][CurrentHistoryIndex]);
            }
            RefreshUVRender();

        }
        private void Undo()
        {
            if (CurrentHistoryIndex > 0)
            {
                CurrentHistoryIndex--;
                UpdateFromHistory();
            }
        }

        private void InputU_KeyDown(object? sender, KeyEventArgs e)
        {
            var selectedVertices = Vertices.Where(x => x.isSelected).ToList();
            float value = GetFloat(InputU.Text);
            if (float.IsNaN(value)) return;

            if (selectedVertices.Count == 1)
            {
                selectedVertices[0].TexturePosition.X = value;
            }
            else if (selectedVertices.Count > 1)
            {
                CVector2 centroid = Calculator.GetCentroidOfUV(selectedVertices.Select(x => x.TexturePosition).ToList());
                float delta = value - centroid.X;
                foreach (var vertex in selectedVertices)
                {
                    vertex.TexturePosition.X += delta;
                }
            }
            RefreshUVRender();
        }

        private void InputV_KeyDown(object? sender, KeyEventArgs e)
        {
            var selectedVertices = Vertices.Where(x => x.isSelected).ToList();
            float value = GetFloat(InputV.Text);
            if (float.IsNaN(value)) return;

            if (selectedVertices.Count == 1)
            {
                selectedVertices[0].TexturePosition.Y = value;
            }
            else if (selectedVertices.Count > 1)
            {
                CVector2 centroid = Calculator.GetCentroidOfUV(selectedVertices.Select(x => x.TexturePosition).ToList());
                float delta = value - centroid.Y;
                foreach (var vertex in selectedVertices)
                {
                    vertex.TexturePosition.Y += delta;
                }
            }
            RefreshUVRender();
        }


        private static  float GetFloat(string text)
        {
           bool f = float.TryParse(text, out float value);

            if (f) return value;
            else return float.NaN;
        }

        private CVector2 GetMousePositionAsUVPosition()
        {
            CVector2 defaultVector = new(); // (0, 0) if failure

            if (SelectonContainer == null || ImageWidth == 0 || ImageHeight == 0)
            {    return defaultVector;  }

            // Get mouse position relative to the image container (canvas)
            Point mousePos = Mouse.GetPosition(SelectonContainer);

            double canvasWidth = SelectonContainer.ActualWidth;
            double canvasHeight = SelectonContainer.ActualHeight;

            // Calculate aspect ratios
            double imageAspect = (double)ImageWidth / ImageHeight;
            double canvasAspect = canvasWidth / canvasHeight;

            double drawWidth, drawHeight;
            double offsetX = 0, offsetY = 0;

            // Determine how the image is drawn inside the canvas (assuming Stretch=Uniform)
            if (canvasAspect > imageAspect)
            {
                // Canvas is wider than image — vertical full fit
                drawHeight = canvasHeight;
                drawWidth = drawHeight * imageAspect;
                offsetX = (canvasWidth - drawWidth) / 2;
            }
            else
            {
                // Canvas is taller than image — horizontal full fit
                drawWidth = canvasWidth;
                drawHeight = drawWidth / imageAspect;
                offsetY = (canvasHeight - drawHeight) / 2;
            }

            // Check if mouse is within drawn image
            if (mousePos.X < offsetX || mousePos.X > offsetX + drawWidth ||
                mousePos.Y < offsetY || mousePos.Y > offsetY + drawHeight)
            {
                return defaultVector;
            }

            // Normalize to UV (0 to 1)
            double u = (mousePos.X - offsetX) / drawWidth;
            double v =  (mousePos.Y - offsetY) / drawHeight ; // Flip vertically

            return new CVector2((float)u, (float)v);
        }


        private void copuUV_Mouse(object? sender, RoutedEventArgs? e)
        {
          CopiedVector=  GetMousePositionAsUVPosition();
            
        }

        private void SnapSelectionToMousePosition(object? sender, RoutedEventArgs? e)
        {
            var temp = new CVector2(CopiedVector);
            CopiedVector = GetMousePositionAsUVPosition();
        
            pasteUV(null,null);
            CopiedVector = new CVector2(temp);
        }

         

        private void DrawGrid(int GridSize)
        {
            CurrentGridSize = GridSize;
            // Clear any existing content in the canvas
            GridCanvas.Children.Clear();

            // Ensure the GridSize doesn't exceed the canvas size limits
            if (GridSize <= 0) return;
            if (GridSize > 256) return;

            // Calculate the spacing between grid lines
            double step = 512.0 / GridSize;

            // Draw vertical grid lines
            for (int i = 0; i <= GridSize; i++)
            {
                var line = new Line
                {
                    X1 = i * step,
                    Y1 = 0,
                    X2 = i * step,
                    Y2 = 512,
                    Stroke = Brushes.Green,
                    StrokeThickness = 0.5
                };
                GridCanvas.Children.Add(line);
            }

            // Draw horizontal grid lines
            for (int i = 0; i <= GridSize; i++)
            {
                var line = new Line
                {
                    X1 = 0,
                    Y1 = i * step,
                    X2 = 512,
                    Y2 = i * step,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5
                };
                GridCanvas.Children.Add(line);
            }
        }
        
        private void SnapSelection(object? sender, RoutedEventArgs? e)
        {
            var d = new Snap_Selector_2d();
            if (d.ShowDialog() == true)
            {
                SnapVertices(d.Type, true);
            }
            

        }
        private void SnapVertices(SnapType2D type, bool snapEach)
        {
            var selectedVertices = Vertices.Where(v => v.isSelected).ToList();
            if (selectedVertices.Count == 0 || CurrentGridSize <= 0 || CurrentGridSize > 255)
                return;

            foreach (var vertex in selectedVertices)
            {
                // UV position (normalized 0-1)
                float u = vertex.TexturePosition.X;
                float v = vertex.TexturePosition.Y;

                // Create grid square around this vertex
                GridSquare square = GridSquare.FromUV(u, v, CurrentGridSize);

                // Snap based on type
                (float newU, float newV) = type switch
                {
                    SnapType2D.TopLeft => (square.MinX, square.MaxY),
                    SnapType2D.TopRight => (square.MaxX, square.MaxY),
                    SnapType2D.BottomLeft => (square.MinX, square.MinY),
                    SnapType2D.BottomRight => (square.MaxX, square.MinY),
                    SnapType2D.Nearest => square.GetNearest(u, v),
                    _ => (u, v) // Default: no snapping
                };

                vertex.TexturePosition.X = newU;
                vertex.TexturePosition.Y = newV;
            }

            AddHistory();
            RefreshUVRender();
            refreshManualInput(selectedVertices);
        }


        // Helper for distance squared (avoids sqrt for better performance)
         


        private void refreshManualInput(List<CGeosetVertex>? selectedVertices = null)
        {
            if (selectedVertices != null)
            {
              var c = GetUVCentroid(selectedVertices);
                InputU.Text = c.X.ToString();   
                InputV.Text = c.Y.ToString();   
            }
            else
            {
                  var vertices = Vertices.Where(x => x.isSelected).ToList();
                var c = GetUVCentroid(vertices);
                InputU.Text = c.X.ToString();
                InputV.Text = c.Y.ToString();
            }
             
        }

        private void SnapSelectionG(object? sender, RoutedEventArgs? e)
        {
            var dialog = new Snap_Selector_2d();
            if (dialog.ShowDialog() == true)
            {
                SnapVertices(dialog.Type, false);
            }
        }

        private void inputGrid_TextChanged(object? sender, TextChangedEventArgs e)
        {
            if (int.TryParse(inputGrid.Text, out int p))
            {
                if (p >= 0 && p <= 256)
                {
                    
                    DrawGrid(p);
                }
                else
                {
                    GridCanvas.Children.Clear();
                }

            }
            else
            {
                GridCanvas.Children.Clear();
            }
        }

        private void SnapSelectionToMousePositionC(object? sender, RoutedEventArgs? e)
        {
            var temp = new CVector2(CopiedVector);
            CopiedVector = GetMousePositionAsUVPosition();

            pasteUVEach(null, null);
            CopiedVector = new CVector2(temp);
        }

         

        private void collapse(object? sender, RoutedEventArgs? e)
        {
            var SelectedVertices = Vertices.Where(x => x.isSelected).ToList();

            if (SelectedVertices.Count  < 2) { return; }
            for (int i = 1; i < SelectedVertices.Count; i++)
            {
                SelectedVertices[i].TexturePosition = new CVector2(SelectedVertices[0].TexturePosition);
            }
            RefreshUVRender();
        }

        private void pasteUVEach(object? sender, RoutedEventArgs? e)
        {
            var SelectedVertices = Vertices.Where(x => x.isSelected).ToList();

            if (SelectedVertices.Count == 0) { return; }
            for (int i = 0; i < SelectedVertices.Count; i++)
            {
                SelectedVertices[0].TexturePosition = new CVector2(CopiedVector);
            }
            RefreshUVRender();
        }

        private void pojtris(object? sender, RoutedEventArgs? e)
        {
            Calculator.ProjectTriangleIslands(Triangles);
            RefreshUVRender();
            AddHistory();
        }

        private void WeldSimilarVertexPositions(object? sender, RoutedEventArgs? e)
        {
            Input it = new Input("Percentage");
            if (it.ShowDialog() == true)
            {
                float imageWidth = ImageWidth;
                float imageHeight = ImageHeight;

                if (float.TryParse(it.Result, out float thresholdPercentage))
                {
                    if (thresholdPercentage <= 0) return;

                    float thresholdHeight = imageHeight * thresholdPercentage / 100;
                    float thresholdWidth = imageWidth * thresholdPercentage / 100;

                    var vertices = Vertices.Where(x => x.isSelected).ToList();

                    for (int i = 0; i < vertices.Count; i++)
                    {
                        var v1 = vertices[i];
                        for (int j = i + 1; j < vertices.Count; j++)
                        {
                            var v2 = vertices[j];

                            float dx = Math.Abs(v1.TexturePosition.X - v2.TexturePosition.X) * imageWidth;
                            float dy = Math.Abs(v1.TexturePosition.Y - v2.TexturePosition.Y) * imageHeight;

                            if (dx <= thresholdWidth && dy <= thresholdHeight)
                            {
                                // Weld: set v2's texture position to v1's
                                v2.TexturePosition = new CVector2( v1.TexturePosition);
                            }
                        }
                    }
                    RefreshUVRender();
                }
                else
                {
                    MessageBox.Show("Input is not a valid float.");
                }
            }
        }

        private void clamp(object? sender, RoutedEventArgs? e)
        {
            foreach (var v in Vertices)
            {
                v.TexturePosition= Calculator.ClampUV(v.TexturePosition);
                RefreshUVRender();
            }
        }

        private void Redo()
        {
            if (CurrentHistoryIndex< History.First().Value.Count - 1)
            {
                CurrentHistoryIndex++;
                UpdateFromHistory();
            }
            
        }
    }
      class GridSquare
    {
        public float MinX { get; }
        public float MinY { get; }
        public float MaxX => MinX + Size;
        public float MaxY => MinY + Size;
        public float Size { get; }

        private GridSquare(float minX, float minY, float size)
        {
            MinX = minX;
            MinY = minY;
            Size = size;
        }

        public static GridSquare FromUV(float u, float v, int gridDivisions)
        {
            float size = 1f / gridDivisions;
            int cellX = (int)(u / size);
            int cellY = (int)(v / size);
            float minX = cellX * size;
            float minY = cellY * size;
            return new GridSquare(minX, minY, size);
        }

        public (float u, float v) GetNearest(float u, float v)
        {
            // Snap to the nearest corner of this grid square
            float nearestX = (Math.Abs(u - MinX) < Math.Abs(u - MaxX)) ? MinX : MaxX;
            float nearestY = (Math.Abs(v - MinY) < Math.Abs(v - MaxY)) ? MinY : MaxY;
            return (nearestX, nearestY);
        }
    }

}
