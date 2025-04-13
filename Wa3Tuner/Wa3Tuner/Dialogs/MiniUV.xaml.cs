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

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for MiniUV.xaml
    /// </summary>
    public partial class MiniUV : Window
    {
        CGeoset geoset;
         
private float ImageWidth, ImageHeight;

        public MiniUV(CGeoset g, ListBox l, CModel m)
        {
            InitializeComponent();
            geoset = g;
            Fill(m, l);
        }
        private void Fill(CModel m, ListBox lb)
        {

            SelectonContainer_MouseMove(null,null);
            if (geoset.Material.Object == null) {Close(); return;}
            if (geoset.Material.Object.Layers.Count==0) {Close(); return;}
            var l = geoset.Material.Object.Layers[geoset.Material.Object.Layers.Count - 1];
            var t = l.Texture.Object;
            int index = m.Textures.IndexOf(t);
            ListBoxItem i = lb.Items[index] as ListBoxItem;
            Image im = i.ToolTip as Image;
            image.Source = ResizeImageSource(im.Source);
            ImageWidth =(float)im.Source.Width;
            ImageHeight = (float)im.Source.Height;  
            RefreshUVRender();
        }
        private ImageSource ResizeImageSource(ImageSource source, int width = 512, int height = 512)
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

            foreach (CGeosetTriangle triangle in geoset.Triangles)
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
        private float[] NormalizedPositionToImagePosition(
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
        private void SelectonContainer_MouseDown(object sender, MouseButtonEventArgs e)
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
        private void SelectonContainer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);

            if (mouseMode == MouseMode.Selecting)
            {
                FinalizeSelection(); // Let this handle UV bounds + selection
            }

            SelectonContainer.Children.Remove(selectionRectangle);
            mouseMode = MouseMode.None;
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

            SelectAffectedVertices(finalRight, finalLeft, finalTop, finalBottom);
        }



        // Mouse move
        private void SelectonContainer_MouseMove(object sender, MouseEventArgs e)
        {
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
        }



        // Select affected vertices within bounds
        private void SelectAffectedVertices(float rightUV, float leftUV, float topUV, float bottomUV)
        {
            foreach (var vertex in geoset.Vertices)
            {
                vertex.isSelected = VertexInsideSelection(vertex, rightUV, leftUV, topUV, bottomUV);
            }

            RefreshUVRender();
            RefreshTitle();
        }


        // Check if vertex is inside selection area
        private bool VertexInsideSelection(CGeosetVertex vertex, float rightX, float leftX, float topY, float bottomY)
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
            var SelectedVertices = geoset.Vertices.Where(x => x.isSelected).ToList();

            if (SelectedVertices.Count == 0) { return; }

            float TextureImageHeight = ImageHeight;
            float TextureImageWidth = ImageWidth;

            // Center of the image to project to
            float centerX = TextureImageWidth / 2f;
            float centerY = TextureImageHeight / 2f;

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
                        // For a top projection, ignore Y and project X and Z
                        projectedX = (x / Math.Abs(z)) * centerX + centerX;  // Scale based on Z for perspective
                        projectedY = (y / Math.Abs(z)) * centerY + centerY;  // Scale based on Z for perspective
                        break;

                    case Side.Bottom:
                        // For bottom projection, it's essentially the same as top but inverted
                        projectedX = (x / Math.Abs(z)) * centerX + centerX;
                        projectedY = (y / Math.Abs(z)) * centerY + centerY;
                        break;

                    case Side.Front:
                        // For front projection, project based on Z (depth)
                        projectedX = (x / Math.Abs(z)) * centerX + centerX;
                        projectedY = (y / Math.Abs(z)) * centerY + centerY;
                        break;

                    case Side.Back:
                        // For back projection, essentially the same as front but flipped in Z
                        projectedX = (x / Math.Abs(z)) * centerX + centerX;
                        projectedY = (y / Math.Abs(z)) * centerY + centerY;
                        break;

                    case Side.Left:
                        // For left projection, project based on X (side view)
                        projectedX = (z / Math.Abs(x)) * centerX + centerX;
                        projectedY = (y / Math.Abs(x)) * centerY + centerY;
                        break;

                    case Side.Right:
                        // For right projection, essentially the same as left but flipped in X
                        projectedX = (z / Math.Abs(x)) * centerX + centerX;
                        projectedY = (y / Math.Abs(x)) * centerY + centerY;
                        break;

                    default:
                        // Handle any other cases if necessary
                        break;
                }

                // Ensure the projected coordinates are within bounds (0 to 1 range)
                projectedX = Math.Clamp(projectedX / TextureImageWidth, 0, 1);
                projectedY = Math.Clamp(projectedY / TextureImageHeight, 0, 1);

                // Assign the new projected coordinates to the vertex
                vertex.TexturePosition.X = projectedX;
                vertex.TexturePosition.Y = projectedY;
            }

            RefreshUVRender();
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
        enum Direction
        {
            Up, Down,Left, Right
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { Hide(); }
           else  if (e.Key == Key.Left) { MoveVertices(Direction.Left); }
           else  if (e.Key == Key.Right) { MoveVertices(Direction.Right); }
           else  if (e.Key == Key.Up) { MoveVertices(Direction.Up); }
           else  if (e.Key == Key.Down) { MoveVertices(Direction.Down); }
           else  if (e.Key == Key.F) { Project(Side.Front); }
            else if (e.Key == Key.C) { Project(Side.Back); }
            else if (e.Key == Key.R) { Project(Side.Right); }
            else if (e.Key == Key.L) { Project(Side.Left); }
            else if (e.Key == Key.B) { Project(Side.Bottom); }
            else if (e.Key == Key.T) { Project(Side.Top); }
            else if (e.Key == Key.A) { SelectAll(); }
            else if (e.Key == Key.N) { SelectNone(); }
            else if (e.Key == Key.G) { AlignHorizontally(); }
            else if (e.Key == Key.Y) { AlignVertically(); }

            else if (e.Key == Key.I) { SelectInvert(); }
            else if (e.Key == Key.H) { FlipHorizontally(); }
            else if (e.Key == Key.V) { FlipVertically(); }
            else if (e.Key == Key.W) { SwapTwoVertices(); }
          
            else if (e.Key == Key.O) { FormAsShape(); }
            else if (e.Key == Key.Z) { FitVerticesInsideImage(); }

            else if (e.Key == Key.LeftCtrl) { mouseMode = MouseMode.Rotating;  }
          
            else if (e.Key == Key.LeftAlt) { mouseMode = MouseMode.ScalingHor;  }
            else if (e.Key == Key.LeftShift) { mouseMode = MouseMode.ScalingVer;  }
            
        }

        private void FitVerticesInsideImage()
        {
            var SelectedVertices = geoset.Vertices.Where(x => x.isSelected).ToList();

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
            var SelectedVertices = geoset.Vertices.Where(x => x.isSelected).ToList();

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
            var SelectedVertices = geoset.Vertices.Where(x => x.isSelected).ToList();
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
            var selectedVertices = geoset.Vertices.Where(x => x.isSelected).ToList();
            if (selectedVertices.Count < 2) {return; }  
            for (int i = 1; i < selectedVertices.Count; i++)
            {
                selectedVertices[i].TexturePosition.Y = selectedVertices[0].TexturePosition.Y;
            }
            RefreshUVRender();
        }

        private void AlignHorizontally()
        {
            var selectedVertices = geoset.Vertices.Where(x => x.isSelected).ToList();
            if (selectedVertices.Count < 2) { return; }
            for (int i = 1; i < selectedVertices.Count; i++)
            {
                selectedVertices[i].TexturePosition.X = selectedVertices[0].TexturePosition.X;
            }
            RefreshUVRender();
        }

        private void MoveVertices(Direction direction)
        {
            var selectedVertices = geoset.Vertices.Where(x => x.isSelected).ToList();

            if (selectedVertices.Count == 0)
                return;

            // Calculate step size based on image dimensions
            float stepX = 1f / ImageWidth;  // Horizontal step based on image width
            float stepY = 1f / ImageHeight; // Vertical step based on image height

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

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
              if (e.Key == Key.LeftCtrl) { mouseMode = MouseMode.None; }
              if (e.Key == Key.LeftShift) { mouseMode = MouseMode.None; }
              if (e.Key == Key.LeftAlt) { mouseMode = MouseMode.None; }
        }

        private void FlipVertically()
        {
            var SelectedVertices = geoset.Vertices.Where(x => x.isSelected).ToList();

            foreach (var vr in SelectedVertices)
            {
                
                vr.TexturePosition.Y = Calculator.FlipV(vr.TexturePosition.Y);
            }
            RefreshUVRender();
        }

        private void FlipHorizontally()
        {
            var SelectedVertices = geoset.Vertices.Where(x => x.isSelected).ToList();

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
            foreach (var vertex in geoset.Vertices)
            {
                vertex.isSelected = !vertex.isSelected;
            }
            RefreshUVRender();
            RefreshTitle();
        }

        private void SelectNone()
        {
          
            
            foreach (var vertex in geoset.Vertices) { vertex.isSelected = false; }
            RefreshUVRender();
            RefreshTitle();
        }
        private string SelectedCount = "0";
        private void RefreshTitle()
        {
            SelectedCount = geoset.Vertices.ObjectList.Count(x => x.isSelected).ToString();
           Title = $"Mini UV Mapper - Selected {SelectedCount} vertices";
        } 

        private void SelectAll()
        {
            foreach (var vertex in geoset.Vertices) vertex.isSelected = true;
            RefreshUVRender();
            RefreshTitle();
        }

      
        private void SelectonContainer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var SelectedVertices = geoset.Vertices.Where(x => x.isSelected).ToList();
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

        private void a(object sender, RoutedEventArgs e)
        {
            SelectAll();
        }

        private void n(object sender, RoutedEventArgs e)
        {
            SelectNone();
        }

        private void i(object sender, RoutedEventArgs e)
        {
            SelectInvert();
        }

        private void projT(object sender, RoutedEventArgs e)
        {
            Project(Side.Top);
        }

        private void projB(object sender, RoutedEventArgs e)
        {
            Project(Side.Bottom);
        }

        private void projF(object sender, RoutedEventArgs e)
        {
            Project(Side.Front);
        }

        private void projC(object sender, RoutedEventArgs e)
        {
            Project(Side.Back);
        }

        private void pojL(object sender, RoutedEventArgs e)
        {
            Project(Side.Left);
        }

        private void pojr(object sender, RoutedEventArgs e)
        {
            Project(Side.Right);
        }

        private void ft(object sender, RoutedEventArgs e)
        {
            FitVerticesInsideImage();
        }

        private void sw(object sender, RoutedEventArgs e)
        {
            SwapTwoVertices();
        }

        private void au(object sender, RoutedEventArgs e)
        {
            AlignHorizontally();
        }

        private void av(object sender, RoutedEventArgs e)
        {
            AlignVertically();
        }

        private void fl1(object sender, RoutedEventArgs e)
        {
            FlipHorizontally();
        }

        private void fl2(object sender, RoutedEventArgs e)
        {
            FlipVertically();
        }

        private void sh(object sender, RoutedEventArgs e)
        {
            FormAsShape();
        }
        private CVector2 CopiedVector = new CVector2();
        private void copyUV(object sender, RoutedEventArgs e)
        {
            var SelectedVertices = geoset.Vertices.Where(x => x.isSelected).ToList();
            if (SelectedVertices.Count ==1)
            {
                CopiedVector = new CVector2( SelectedVertices[0].TexturePosition);
            }
            if (SelectedVertices.Count  > 1)
            {
                CopiedVector = GetUVCentroid(SelectedVertices);

            }
        }

        private CVector2 GetUVCentroid(List<CGeosetVertex> selectedVertices)
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

        private void pasteUV(object sender, RoutedEventArgs e)
        {
            var SelectedVertices = geoset.Vertices.Where(x => x.isSelected).ToList();
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

        private void MoveUVCentroid(List<CGeosetVertex> vertices, CVector2 targetCentroid)
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


        internal void Update(CGeoset g, ListBox l, CModel m)
        {
            geoset = g;
            Fill(m, l);
        }
    }
}
