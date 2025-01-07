using MdxLib.Model;
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

namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for cameraManager.xaml
    /// </summary>
    public partial class cameraManager : Window
    {
        CModel model;
        public cameraManager(CModel _model)
        {
            InitializeComponent();
            this.model = _model;
            Fill();
        }
        private void Fill()
        {
            CameraList.Items.Clear();
            foreach (CCamera cam in model.Cameras)
            {
                CameraList.Items.Add(new ListBoxItem() {Content= cam.Name });
              
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { Close(); }
        }

        private void SelectedCam(object sender, SelectionChangedEventArgs e)
        {
            if (CameraList.SelectedItem != null)
            {
                Selected = GetSelectedCam();
                CameraName.Text = Selected.Name;
                PositionX.Text = Selected.Position.X.ToString();
                PositionY.Text = Selected.Position.Y.ToString();
                PositionZ.Text = Selected.Position.Z.ToString();
                TargetX.Text = Selected.TargetPosition.X.ToString();
                TargetY.Text = Selected.TargetPosition.Y.ToString();
                TargetZ.Text = Selected.TargetPosition.Z.ToString();
                FieldOfView.Text = Selected.FieldOfView.ToString();
                NearDistance.Text = Selected.NearDistance.ToString();
                FarDistance.Text = Selected.FarDistance.ToString();
            }
        }
        private CCamera Selected;
        private CCamera GetSelectedCam()
        {
          ListBoxItem item = CameraList.SelectedItem as ListBoxItem;
            return model.Cameras.First(X => X.Name == item.Content.ToString());
        }

        private void CameraName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Selected == null) { return; }
            
            string input = CameraName.Text.Trim();
            if (model.Cameras.Any(x => x.Name == input) == false)
            {

                Selected.Name = input;
               




            }
        }
        private float GetFloat(TextBox box)
        {
            if (float.TryParse(box.Text, out float value))
            {
                return value;
            }
            return 0;
        }
        private void SetPosX(object sender, TextChangedEventArgs e)
        {
            if (Selected == null) { return; }
            Selected.Position.X = GetFloat(PositionX);
        }

        private void SetPosY(object sender, TextChangedEventArgs e)
        {
            if (Selected == null) { return; }
            Selected.Position.Y = GetFloat(PositionY);
        }

        private void SetPosZ(object sender, TextChangedEventArgs e)
        {
            if (Selected == null) { return; }
            Selected.Position.Z = GetFloat(PositionZ);
        }

        private void setTarX(object sender, TextChangedEventArgs e)
        {
            if (Selected == null) { return; }
            Selected.TargetPosition.X = GetFloat(TargetX);
        }

        private void setTarY(object sender, TextChangedEventArgs e)
        {
            if (Selected == null) { return; }
            Selected.TargetPosition.Y = GetFloat(TargetY);
        }

        private void setTarZ(object sender, TextChangedEventArgs e)
        {
            if (Selected == null) { return; }
            Selected.TargetPosition.Z = GetFloat(TargetZ);
        }

        private void SetFOV(object sender, TextChangedEventArgs e)
        {
            if (Selected == null) { return; }
            Selected.FieldOfView = GetFloat(FieldOfView);
            
        }

        private void SetND(object sender, TextChangedEventArgs e)
        {
            if (Selected == null) { return; }
            Selected.NearDistance = GetFloat(NearDistance);
        }

        private void SetFD(object sender, TextChangedEventArgs e)
        {
            if (Selected == null) { return; }
            Selected.FarDistance = GetFloat(FarDistance);
        }

        private void SetAnimatedPos(object sender, RoutedEventArgs e)
        {
            if (Selected == null) { return; }
            transformation_editor te = new transformation_editor(model, Selected.Translation, false, TransformationType.Translation);

            te.ShowDialog();
        }

        private void SetAniamtedTar(object sender, RoutedEventArgs e)
        {
            if (Selected == null) { return; }
            transformation_editor te = new transformation_editor(model, Selected.TargetTranslation, false, TransformationType.Translation);
            te.ShowDialog();
        }

        private void SetRotationTarget(object sender, RoutedEventArgs e)
        {
            if (Selected == null) { return; }
            transformation_editor te = new transformation_editor(model, Selected.Rotation, false, TransformationType.Float);
            te.ShowDialog();
        }

        private void delcam(object sender, RoutedEventArgs e)
        {
            if (CameraList.SelectedItem!= null)
            {
                CCamera cam = GetSelectedCam();
                CameraList.Items.Remove(CameraList.SelectedItem);
                model.Cameras.Remove(cam);
            }
        }

        private void newcam(object sender, RoutedEventArgs e)
        {
            string name = CameraName.Text.Trim();
            if (name.Length > 0)
            {
                if (model.Cameras.Any(x => x.Name == name))
                {
                    MessageBox.Show("There is a camera with that name already"); return;
                }
                CCamera cam = new CCamera(model);
                cam.Name = name;
                cam.Position = new MdxLib.Primitives.CVector3(GetFloat(PositionX), GetFloat(PositionY), GetFloat(PositionZ));
                cam.TargetPosition = new MdxLib.Primitives.CVector3(GetFloat(TargetX), GetFloat(TargetY), GetFloat(TargetZ));
                cam.FieldOfView = GetFloat(FieldOfView);
                cam.NearDistance = GetFloat(NearDistance);
                cam.FarDistance = GetFloat(FarDistance);
                model.Cameras.Add(cam);
                Fill();
            }
             
        }
    }
    
}
