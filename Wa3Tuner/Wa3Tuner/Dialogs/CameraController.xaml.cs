using SharpGL;
using SharpGL.SceneGraph.Cameras;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Wa3Tuner.Helper_Classes;

namespace Wa3Tuner.Dialogs
{
    
    /// <summary>
    /// Interaction logic for CameraController.xaml
    /// </summary>
    public partial class CameraController : Window
    {
       
     
       
        public CameraController(OpenGL ogl )
        {
             
            InitializeComponent();
            CameraPositionX.Text = CameraControl.eyeX.ToString();
            CameraPositionY.Text = CameraControl.eyeY.ToString();
            CameraPositionZ.Text = CameraControl.eyeZ.ToString();

            TargetPositionX.Text = CameraControl.CenterX.ToString();
            TargetPositionY.Text = CameraControl.CenterY.ToString();
            TargetPositionZ.Text = CameraControl.CenterZ.ToString();

            RollX.Text = CameraControl.UpX.ToString();
            RollY.Text = CameraControl.UpY.ToString();
            RollZ.Text = CameraControl.UpZ.ToString();
        }
        private void ok(object? sender, RoutedEventArgs? e)
        {
            if (float.TryParse(CameraPositionX.Text, out float camX) &&
       float.TryParse(CameraPositionY.Text, out float camY) &&
       float.TryParse(CameraPositionZ.Text, out float camZ) &&
       float.TryParse(TargetPositionX.Text, out float targetX) &&
       float.TryParse(TargetPositionY.Text, out float targetY) &&
       float.TryParse(TargetPositionZ.Text, out float targetZ) &&
       float.TryParse(RollX.Text, out float rollX) &&
       float.TryParse(RollY.Text, out float rollY) &&
       float.TryParse(RollZ.Text, out float rollZ))
            {


                {
                    CameraControl.eyeX = camX;
                    CameraControl.eyeY = camY;
                    CameraControl.eyeZ = camZ;
                    CameraControl.CenterX = targetX;
                    CameraControl.CenterY = targetY;
                    CameraControl.CenterZ = targetZ;
                    CameraControl.UpX = rollX;
                    CameraControl.UpY = rollY;
                    CameraControl.UpZ = rollZ;
                    DialogResult = true;

                }
            }
            else
            {
                MessageBox.Show("Invalid input! Please enter valid float numbers.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
             
        }

        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) ok(null, null);
        }
    }
}
