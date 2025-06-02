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
    /// Interaction logic for Edit_light.xaml
    /// </summary>
    public partial class Edit_light : Window
    {
        private CLight Light;
        CModel Model;
        public Edit_light(CLight light, CModel model)
        {
            InitializeComponent();
            Light = light;
            Fill();
            Model = model;
        }
        void Fill()
        {
            btnColor.Content = Light.Color.Static ? $"Color: {Light.Color.GetValue()}" : $"Color: ({Light.Color.Count})";
            btnAmbientColor.Content = Light.Color.Static ? $"Ambient Color: {Light.AmbientColor.GetValue()}" : $"Ambient Color: ({Light.AmbientColor.Count})";
            btnIntensity.Content = Light.Intensity.Static ? $"Intensity: {Light.Intensity.GetValue()}" : $"Intensity: ({Light.Intensity.Count})";
            btnAmbientIntensity.Content = Light.AmbientIntensity.Static ? $"AmbientIntensity: {Light.AmbientIntensity.GetValue()}" : $"AmbientIntensity: ({Light.AmbientIntensity.Count})";
            btnAttenuationStart.Content = Light.AttenuationStart.Static ? $"AttenuationStart: {Light.AttenuationStart.GetValue()}" : $"AttenuationStart: ({Light.AttenuationStart.Count})";
            btnAttenuationEnd.Content = Light.AttenuationEnd.Static ? $"AttenuationEnd: {Light.AttenuationEnd.GetValue()}" : $"AttenuationEnd: ({Light.AttenuationEnd.Count})";
            ListType.SelectedIndex = (int)Light.Type;
            if (Light.Color.Static)
            {
                Brush color = Calculator.BrushFromWar3Vector3(Light.Color.GetValue());
                btnColor.Background = color;
            }
            if (Light.AmbientColor.Static)
            {
                Brush amcolor = Calculator.BrushFromWar3Vector3(Light.AmbientColor.GetValue());
                btnAmbientColor.Background = amcolor;
            }
        }
        private void SelectedType(object? sender, SelectionChangedEventArgs e)
        {
            Light.Type = (MdxLib.Model.ELightType) ListType.SelectedIndex;
        }
        private void editcolor(object? sender, RoutedEventArgs? e)
        {
            transformation_editor editor = new transformation_editor(Model, Light.Color, true, TransformationType.Color);
            editor.ShowDialog(); Fill();
        }
        private void editamcolor(object? sender, RoutedEventArgs? e)
        {
            transformation_editor editor = new transformation_editor(Model, Light.AmbientColor, true, TransformationType.Color);
            editor.ShowDialog(); Fill();
        }
        private void editintensity(object? sender, RoutedEventArgs? e)
        {
            transformation_editor editor = new transformation_editor(Model, Light.Intensity, true, TransformationType.Float);
            editor.ShowDialog(); Fill();
        }
        private void editamintensity(object? sender, RoutedEventArgs? e)
        {
            transformation_editor editor = new transformation_editor(Model, Light.AmbientIntensity, true, TransformationType.Float);
            editor.ShowDialog(); Fill();
        }
        private void editattstart(object? sender, RoutedEventArgs? e)
        {
            transformation_editor editor = new transformation_editor(Model, Light.AttenuationStart, true, TransformationType.Float);
            editor.ShowDialog(); Fill();
        }
        private void editattend(object? sender, RoutedEventArgs? e)
        {
            transformation_editor editor = new transformation_editor(Model, Light.AttenuationEnd, true, TransformationType.Float);
            editor.ShowDialog();
        }
        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; }
        }
    }
}
