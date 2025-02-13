using MdxLib.Model;
using MdxLib.Primitives;
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
    /// Interaction logic for window_edit_cols.xaml
    /// </summary>
    public partial class window_edit_cols : Window
    {
        CModel Model;
        CCollisionShape cols;
        public window_edit_cols(MdxLib.Model.CCollisionShape bone, MdxLib.Model.CModel currentModel)
        {
            InitializeComponent();
            cols = (CCollisionShape)bone;
            Model = currentModel;
            FillData();
        }
        private void FillData()
        {
           BoxRadioButton.IsChecked = cols.Type == ECollisionShapeType.Box;
           SphereRadioButton.IsChecked = cols.Type != ECollisionShapeType.Box;
            NegativeExtentXTextBox.IsEnabled = cols.Type == ECollisionShapeType.Box;
            NegativeExtentYTextBox.IsEnabled = cols.Type == ECollisionShapeType.Box;
            NegativeExtentZTextBox.IsEnabled = cols.Type == ECollisionShapeType.Box;
            PositiveExtentZTextBox.IsEnabled = cols.Type == ECollisionShapeType.Box;
            PositiveExtentYTextBox.IsEnabled = cols.Type == ECollisionShapeType.Box;
            PositiveExtentXTextBox.IsEnabled = cols.Type == ECollisionShapeType.Box;
            RadiusTextBox.IsEnabled = cols.Type == ECollisionShapeType.Sphere;
            if (cols.Type == ECollisionShapeType.Sphere)
            {
                RadiusTextBox.Text = cols.Radius.ToString();
            }
            else
            {
                NegativeExtentXTextBox.Text = cols.Vertex1.X.ToString();
                NegativeExtentYTextBox.Text = cols.Vertex1.Y.ToString();
                NegativeExtentZTextBox.Text = cols.Vertex1.Z.ToString();
                PositiveExtentXTextBox.Text = cols.Vertex2.X.ToString();
                PositiveExtentYTextBox.Text = cols.Vertex2.Y.ToString();
                PositiveExtentZTextBox.Text = cols.Vertex2.Z.ToString();
            }
        }
        private void ok(object sender, RoutedEventArgs e)
        {
            if (BoxRadioButton.IsChecked == true)
            {
                if (ValuesOK(NegativeExtentXTextBox, NegativeExtentYTextBox, NegativeExtentZTextBox, PositiveExtentXTextBox,
                   PositiveExtentYTextBox, PositiveExtentZTextBox))
                {
                    cols.Type = ECollisionShapeType.Box;
                    float minx = float.Parse(NegativeExtentXTextBox.Text);
                    float miny = float.Parse(NegativeExtentYTextBox.Text);
                    float minz = float.Parse(NegativeExtentZTextBox.Text);
                    float maxx = float.Parse(PositiveExtentXTextBox.Text);
                    float maxy = float.Parse(PositiveExtentYTextBox.Text);
                    float maxz = float.Parse(PositiveExtentZTextBox.Text);
                    if (minx >= maxx || miny >= maxy || minz >= maxz)
                    {
                        MessageBox.Show("The minimums cannto be equal or greater than the maximums"); return;
                    }
                    CVector3 min = new CVector3(minx,miny,minz);   
                    CVector3 max = new CVector3(maxx,maxy,maxz);
                    cols.Vertex1 = min;
                    cols.Vertex2 = max;
                    DialogResult = true;
                }
                else
                {
                     MessageBox.Show("Invalid input"); return;
                }
            }
            else
            {
                if (ValuesOK(RadiusTextBox))
                {
                    cols.Type = ECollisionShapeType.Sphere;
                    cols.Vertex1 = new CVector3();
                    cols.Vertex2 = new CVector3();
                    cols.Radius = float.Parse(RadiusTextBox.Text);
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Invalid input"); return;
                }
            }
        }
        private void SetBox(object sender, RoutedEventArgs e)
        {
            NegativeExtentXTextBox.IsEnabled = true;
            NegativeExtentYTextBox.IsEnabled = true;
            NegativeExtentZTextBox.IsEnabled = true;
            PositiveExtentZTextBox.IsEnabled = true;
            PositiveExtentYTextBox.IsEnabled = true;
            PositiveExtentXTextBox.IsEnabled = true;
            RadiusTextBox.IsEnabled =   false;
        }
        private void SetSphere(object sender, RoutedEventArgs e)
        {
            NegativeExtentXTextBox.IsEnabled = false;
            NegativeExtentYTextBox.IsEnabled = false;
            NegativeExtentZTextBox.IsEnabled = false;
            PositiveExtentZTextBox.IsEnabled = false;
            PositiveExtentYTextBox.IsEnabled = false;
            PositiveExtentXTextBox.IsEnabled = false;
            RadiusTextBox.IsEnabled = true;
        }
        private bool ValuesOK(params TextBox[] boxes)
        {
            foreach (TextBox box in boxes)
            {
                // Try to parse the text of the TextBox as a float
                if (!float.TryParse(box.Text, out _))
                {
                    // Return false if any value is not a valid float
                    return false;
                }
            }
            // Return true if all values are valid floats
            return true;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
        }
    }
}
