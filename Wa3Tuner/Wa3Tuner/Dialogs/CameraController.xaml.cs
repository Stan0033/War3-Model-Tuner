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
        Viewport3D _viewport;
        bool gl = false;
        public CameraController(Viewport3D viewport)
        {
            _viewport = viewport;
            InitializeComponent();
            if (viewport.Camera is System.Windows.Media.Media3D.PerspectiveCamera camera)
            {
                CameraPositionX.Text = camera.Position.X.ToString();
                CameraPositionY.Text = camera.Position.Y.ToString();
                CameraPositionZ.Text = camera.Position.Z.ToString();

                TargetPositionX.Text = camera.LookDirection.X.ToString();
                TargetPositionY.Text = camera.LookDirection.Y.ToString();
                TargetPositionZ.Text = camera.LookDirection.Z.ToString();

                RollX.Text = "0"; // Placeholder, roll usually isn't a property in WPF PerspectiveCamera
                RollY.Text = "0";
                RollZ.Text = "0";
            }
        }
        public CameraController(OpenGL ogl )
        {
            gl = true;
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
        private void ok(object sender, RoutedEventArgs e)
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
                    if (gl)
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
                    else
                    {

                        if (_viewport.Camera is System.Windows.Media.Media3D.PerspectiveCamera camera)
                        {
                            if (camera.IsFrozen) { MessageBox.Show("The camera is currently frozen"); return; }
                            camera.Position = new Point3D(camX, camY, camZ);
                            camera.LookDirection = new Vector3D(targetX - camX, targetY - camY, targetZ - camZ);
                            // Roll properties are not natively supported in WPF's PerspectiveCamera
                        }
                        this.DialogResult = true;
                        this.Close();
                    }

                }
            }
            else
            {
                MessageBox.Show("Invalid input! Please enter valid float numbers.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
             
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) ok(null, null);
        }
    }
}
