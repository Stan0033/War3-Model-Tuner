using MdxLib.Model;
 
using System.Collections.Generic;
using System.Linq;
 
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Wa3Tuner.Helper_Classes;

namespace Wa3Tuner
{
    enum DrawMethod { Pencil, Line, Edit
    }
    public class DrawLine
    {
        public Point From, To;
        public DrawLine? ConnectTo;
    }
    public partial class DrawShapeWindow : Window
    {
      //variables
        private CModel OwnerModel;
        private DrawMethod Method = DrawMethod.Pencil;
         private List<DrawLine> CurentDrawnLines = new List<DrawLine>();
        Stack<List<DrawLine>> Stack1 = new(); //undo
        Stack<List<DrawLine>> Stack2 = new  (); //redo
      //--------------------------------------
        public DrawShapeWindow()
        {
            InitializeComponent();
        }
        public DrawShapeWindow(CModel currentModel)
        {
            this.OwnerModel = currentModel;
            InitializeComponent();
        }
        private void SetModelPencil(object? sender, RoutedEventArgs? e)
        {
            Method = DrawMethod.Pencil;
            ButtonPencil.Background = Brushes.LightSeaGreen;
            ButtonLine.Background = Brushes.LightGray;
            ButtonEdit.Background = Brushes.LightGray;
        }
        private void SetModeLine(object? sender, RoutedEventArgs? e)
        {
            Method = DrawMethod.Line;
            ButtonPencil.Background = Brushes.LightGray;
            ButtonLine.Background = Brushes.LightSeaGreen;
            ButtonEdit.Background = Brushes.LightGray;
        }
        private void SetModeEdit(object? sender, RoutedEventArgs? e)
        {
            //able to select and drag nodes
            Method = DrawMethod.Edit;
            ButtonPencil.Background = Brushes.LightGray;
            ButtonLine.Background = Brushes.LightGray;
            ButtonEdit.Background = Brushes.LightSeaGreen;
        }

        private void ok(object? sender, RoutedEventArgs? e)
        {
            // get selected axes and extrude amount
            Axes axes = Axes.None;
            if (check_x.IsChecked == true) { axes = Axes.X; }
            else if (check_y.IsChecked == true) {  axes = Axes.Y; }
            else if (check_z.IsChecked == true) { axes = Axes.Z; }
            if (float.TryParse(InputExtrude.Text, out float ExtrudeAmount))
            {
                if (ExtrudeAmount<= 0) { MessageBox.Show("Extrude cannot be <= 0");return; }
                if (CurentDrawnLines.Count == 0)
                {
                    MessageBox.Show("Nothing was drawn"); return;
                }
                FinalizeShape(axes, ExtrudeAmount);
            }
            else
            {
                MessageBox.Show("Extrude input invalid"); return;
            }

        }

        private void FinalizeShape(Axes axes, float extrudeAmount)
        {
            CGeoset generatedGeoset = new CGeoset(OwnerModel);
            generatedGeoset.Material.Attach(OwnerModel.Materials[0]);

            foreach (var line in CurentDrawnLines)
            {
                // Create 4 vertices (bottom and top)
                var fromBottom = new MdxLib.Model.CGeosetVertex(OwnerModel);
                var toBottom = new MdxLib.Model.CGeosetVertex(OwnerModel);
                var fromTop = new MdxLib.Model.CGeosetVertex(OwnerModel);
                var toTop = new MdxLib.Model.CGeosetVertex(OwnerModel);

                // Set positions
                fromBottom.Position = MakeVector3(line.From, 0, axes);
                toBottom.Position = MakeVector3(line.To, 0, axes);
                fromTop.Position = MakeVector3(line.From, extrudeAmount, axes);
                toTop.Position = MakeVector3(line.To, extrudeAmount, axes);

                // Add vertices
                generatedGeoset.Vertices.Add(fromBottom);
                generatedGeoset.Vertices.Add(toBottom);
                generatedGeoset.Vertices.Add(fromTop);
                generatedGeoset.Vertices.Add(toTop);

                // Create faces (2 triangles to form quad)
                var face1 = new MdxLib.Model.CGeosetTriangle(OwnerModel);
               
                face1.Vertex1.Attach(fromBottom);
                face1.Vertex2.Attach(toBottom);
                face1.Vertex3.Attach(fromTop);
                var face2 = new MdxLib.Model.CGeosetTriangle(OwnerModel);
                face2.Vertex1.Attach(toBottom);
                face2.Vertex2.Attach(toTop);
                face2.Vertex3.Attach(fromTop);
                

                generatedGeoset.Triangles.Add(face1);
                generatedGeoset.Triangles.Add(face2);
            }

            OwnerModel.Geosets.Add(generatedGeoset);
            DialogResult = true;
        }
        private static MdxLib.Primitives.CVector3 MakeVector3(Point p, float extrude, Axes axes)
        {
            switch (axes)
            {
                case Axes.X: return new MdxLib.Primitives.CVector3(extrude, (float)p.X, (float)-p.Y);
                case Axes.Y: return new MdxLib.Primitives.CVector3((float)p.X, extrude, (float)-p.Y);
                case Axes.Z: return new MdxLib.Primitives.CVector3((float)p.X, (float)-p.Y, extrude);
                default: return new MdxLib.Primitives.CVector3((float)p.X, (float)-p.Y, 0);
            }
        }


        private void undo(object? sender, RoutedEventArgs? e)
        {
            if (Stack1.Count == 0) return;
            var temp = CurentDrawnLines.ToList();
            Stack2.Push(temp);
            CurentDrawnLines = Stack1.Pop();
            RefreshCanvas();
        }
        private void redo(object? sender, RoutedEventArgs? e)
        {
            if (Stack2.Count == 0) return;
            var temp = CurentDrawnLines.ToList();
            Stack1.Push(temp);
            CurentDrawnLines = Stack2.Pop();
            RefreshCanvas();
        }
        private Point? StartPoint = null;
        private DrawLine? CurrentLine = null;
        private bool IsMouseDown = false;

        private void RefreshCanvas()
        {
            Canvas_Draw.Children.Clear();

            foreach (var line in CurentDrawnLines)
            {
                // Draw the line
                var visualLine = new System.Windows.Shapes.Line
                {
                    X1 = line.From.X,
                    Y1 = line.From.Y,
                    X2 = line.To.X,
                    Y2 = line.To.Y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                Canvas_Draw.Children.Add(visualLine);

                // Draw points at start and end
                DrawPoint(line.From);
                DrawPoint(line.To);
            }
        }

        private void DrawPoint(Point p)
        {
            var ellipse = new System.Windows.Shapes.Ellipse
            {
                Width = 6,
                Height = 6,
                Fill = Brushes.Blue,
            };
            Canvas.SetLeft(ellipse, p.X - 3);
            Canvas.SetTop(ellipse, p.Y - 3);
            Canvas_Draw.Children.Add(ellipse);
        }


        private void Canvas_Draw_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!IsMouseDown) return;

            Point currentPos = e.GetPosition(Canvas_Draw);

            if (Method == DrawMethod.Pencil && CurrentLine != null)
            {
                CurrentLine.To = currentPos;
                CurrentLine = new DrawLine { From = currentPos, To = currentPos };
                CurentDrawnLines.Add(CurrentLine);
                RefreshCanvas();
            }
            else if (Method == DrawMethod.Line && StartPoint.HasValue)
            {
                // Show preview by temporary line
                RefreshCanvas();

                var previewLine = new System.Windows.Shapes.Line
                {
                    X1 = StartPoint.Value.X,
                    Y1 = StartPoint.Value.Y,
                    X2 = currentPos.X,
                    Y2 = currentPos.Y,
                    Stroke = Brushes.Red,
                    StrokeDashArray = new DoubleCollection { 2, 2 },
                    StrokeThickness = 2
                };
                Canvas_Draw.Children.Add(previewLine);
            }
        }


        private void Canvas_Draw_MouseDown(object? sender, MouseButtonEventArgs e)
        {
            if (Method == DrawMethod.Pencil || Method == DrawMethod.Line)
            {
                // Save history BEFORE modification
                Stack1.Push(CurentDrawnLines.ToList());
                Stack2.Clear();

                IsMouseDown = true;
                StartPoint = e.GetPosition(Canvas_Draw);

                if (Method == DrawMethod.Pencil)
                {
                    CurrentLine = new DrawLine { From = StartPoint.Value, To = StartPoint.Value };
                    CurentDrawnLines.Add(CurrentLine);
                }
            }
        }




        private void Canvas_Draw_MouseUp(object? sender, MouseButtonEventArgs? e)
        {
            if (!IsMouseDown) return;
            IsMouseDown = false;
            if (e == null) { return; }
            Point endPoint = e.GetPosition(Canvas_Draw);

            if (Method == DrawMethod.Line && StartPoint.HasValue)
            {
                CurentDrawnLines.Add(new DrawLine
                {
                    From = StartPoint.Value,
                    To = endPoint
                });
            }
            else if (Method == DrawMethod.Pencil && CurrentLine != null)
            {
                // Update last line's endpoint
                CurrentLine.To = endPoint;
            }

            StartPoint = null;
            CurrentLine = null;
            RefreshCanvas();

            // Save to undo history
            Stack1.Push(CurentDrawnLines.ToList());
            Stack2.Clear();
        }



        private void ClearHistory(object? sender, RoutedEventArgs? e)
        {
            Stack1.Clear();
            Stack2.Clear();
        }

        private void clearall(object? sender, RoutedEventArgs? e)
        {
            Stack1.Clear();
            Stack2.Clear();
            CurentDrawnLines.Clear();
            Canvas_Draw.Children.Clear();
        }
    }
     
}
