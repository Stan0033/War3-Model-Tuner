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
using Wa3Tuner.Helper_Classes;

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for Render_Preferences.xaml
    /// </summary>
    public partial class Render_Preferences : Window
    {
        public Render_Preferences()
        {
            InitializeComponent();
            Flll();
        }
        private void Flll()
        {
            InputVertexSize.Text = RenderSettings.VertexSize.ToString();
            InputNodeSize.Text = RenderSettings.NodeSize.ToString();
            InputPNodeSize.Text = RenderSettings.PathNodeSize.ToString();
            InputPathNodeColor.Background = GetColor(RenderSettings.Path_Node);
            InputPathNodeColorS.Background = GetColor(RenderSettings.Path_Node_Selected);
            InputVertexColor.Background = GetColor(RenderSettings.Color_Vertex);
            InputVertexColorS.Background = GetColor(RenderSettings.Color_VertexSelected);
            InputVertexColorR.Background = GetColor(RenderSettings.Color_VertexRigged);
            InputVertexColorRS.Background = GetColor(RenderSettings.Color_VertexRiggedSelected);
            InputColls.Background = GetColor(RenderSettings.Color_CollisionShape);
            InputNode.Background = GetColor(RenderSettings.Color_Node);
            InputNodeS.Background = GetColor(RenderSettings.Color_NodeSelected);
            InputBackground.Background = GetColor(RenderSettings.BackgroundColor);
            InputPathLine.Background = GetColor(RenderSettings.Path_Line);
            InputEdge.Background = GetColor(RenderSettings.Color_Edge);
            InputEdgeS.Background = GetColor(RenderSettings.Color_Edge_Selected);
            InputExtents.Background = GetColor(RenderSettings.Color_Extent);
            InputSkeleton.Background = GetColor(RenderSettings.Color_Skeleton);
            InputGridC.Background = GetColor(RenderSettings.GridColor);
            InputNormals.Background = GetColor(RenderSettings.Color_Normals);
            InputSkinning.Background = GetColor(RenderSettings.Color_Skinning);
        }

        private static Brush GetColor(float[] values)
        {
            if (values == null || values.Length < 3)
                return Brushes.Transparent;

            // Clamp values to [0, 1] and convert to byte [0, 255]
            byte r = (byte)(Math.Clamp(values[0], 0f, 1f) * 255);
            byte g = (byte)(Math.Clamp(values[1], 0f, 1f) * 255);
            byte b = (byte)(Math.Clamp(values[2], 0f, 1f) * 255);

            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }


        private void SetPathNodeColor(object? sender, RoutedEventArgs? e)
        {
            Button ? b = sender as Button;
            if (b == null) {return; }
            SetColor(b, ref RenderSettings.Path_Node);
        }

        private void SetPathNodeColorS(object? sender, RoutedEventArgs? e)
        {
            Button? b = sender as Button;
            if (b == null) { return; }
            SetColor(b, ref RenderSettings.Path_Node_Selected);
        }

        private void SetVertexColor(object? sender, RoutedEventArgs? e)
        {
            Button? b = sender as Button;
            if (b == null) { return; }
            SetColor(b, ref RenderSettings.Color_Vertex);
        }

        private void SetVertexColorS(object? sender, RoutedEventArgs? e)
        {
            Button? b = sender as Button;
            if (b == null) { return; }
            SetColor(b, ref RenderSettings.Color_VertexSelected);
        }

        private void SetVertexColorRS(object? sender, RoutedEventArgs? e)
        {
            Button? b = sender as Button;
            if (b == null) { return; }
            SetColor(b, ref RenderSettings.Color_VertexRiggedSelected);
        }

        private void setCols(object? sender, RoutedEventArgs? e)
        {
            Button? b = sender as Button;
            if (b == null) { return; }
            SetColor(b, ref RenderSettings.Color_CollisionShape);
        }

        private void setNormals(object? sender, RoutedEventArgs? e)
        {
            Button ? b = sender as Button;
            if (b == null) {return; }
            SetColor(b, ref RenderSettings.Color_Normals);
        }

        private void setbg(object? sender, RoutedEventArgs? e)
        {
            Button? b = sender as Button;
            if (b == null) { return; }
            SetColor(b, ref RenderSettings.BackgroundColor);
        }

        private void setExtents(object? sender, RoutedEventArgs? e)
        {
            Button? b = sender as Button;
            if (b == null) { return; }
            SetColor(b, ref RenderSettings.Color_Extent);
        }

        private void setskin(object? sender, RoutedEventArgs? e)
        {
            Button? b = sender as Button;
            if (b == null) { return; }
            SetColor(b, ref RenderSettings.Color_Skinning);
        }

        private void setsk(object? sender, RoutedEventArgs? e)
        {
            Button? b = sender as Button;
            if (b == null) { return; }
            SetColor(b, ref RenderSettings.Color_Skeleton);
        }

        private void SetEdge(object? sender, RoutedEventArgs? e)
        {
            Button? b = sender as Button;
            if (b == null) { return; }
            SetColor(b, ref RenderSettings.Color_Edge);
        }

        private void SetEdgeS(object? sender, RoutedEventArgs? e)
        {
            Button? b = sender as Button;
            if (b == null) { return; }
            SetColor(b, ref RenderSettings.Color_Edge_Selected);
        }

        private void setgrid(object? sender, RoutedEventArgs? e)
        {
            Button? b = sender as Button;
            if (b == null) { return; }
            SetColor(b, ref RenderSettings.GridColor);
        }

        private void setnode(object? sender, RoutedEventArgs? e)
        {
            Button? b = sender as Button;
            if (b == null) { return; }
            SetColor(b, ref RenderSettings.Color_Node);
           
        }

        private static void ParseAndSet(TextBox t, ref float field)
        {
            bool p = float.TryParse(t.Text, out float value);
            if (p)
            {
                if (value > 0)
                {
                    field = value;
                }
                else
                {
                    field = 0.5f;
                }
            }
            else
            {
                field = 0.5f;
            }
            
        }

        private void setPathLine(object? sender, RoutedEventArgs? e)
        {
            Button? b = sender as Button;
            SetColor(b, ref RenderSettings.Path_Line);
        }

        private void SetVertexColorR(object? sender, RoutedEventArgs? e)
        {
            Button? b = sender as Button;
            SetColor(b, ref RenderSettings.Color_VertexRigged);
        }
        private static void SetColor(Button? button, ref float[] field)
        {
            // Pick color from the button's background
            if (button == null) { return; }
            System.Windows.Media.Brush newColor = ColorPickerHandler.Pick(button.Background);

            // Set the button's background to the new color
            button.Background = newColor;

            // Convert brush to color (assuming this returns a struct with R, G, B as bytes)
            var color = Calculator.BrushToColor(newColor);

            // Store the normalized RGB values in the float[] array (0.0 to 1.0 range)
            field[0] = color.R / 255f;
            field[1] = color.G / 255f;
            field[2] = color.B / 255f;
        }


        private void SetVertexSize(object? sender, TextChangedEventArgs e)
        {
            ParseAndSet(InputVertexSize, ref RenderSettings.VertexSize);
        }

        private void SetNodeSize(object? sender, TextChangedEventArgs e)
        {
            ParseAndSet(InputNodeSize, ref RenderSettings.NodeSize);
        }

        private void SetPathNodeSize(object? sender, TextChangedEventArgs e)
        {
            ParseAndSet(InputPNodeSize, ref RenderSettings.PathNodeSize);
        }

        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) {DialogResult = true; return; }
        }
    }
}
