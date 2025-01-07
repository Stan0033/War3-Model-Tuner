using MdxLib.Model;
 
using System.Windows;
 
using System.Windows.Input;
 

namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for edit_emitter1.xaml
    /// </summary>
   
    public partial class edit_emitter1 : Window
    {
        CModel Model;
        CParticleEmitter Emitter;
        public edit_emitter1(CParticleEmitter emitter, CModel model)
        {
            InitializeComponent();
            Model = model;
            Emitter = emitter;
            Fill();
        }
       
        void Fill()
        {
            if (Emitter.Visibility.Static)
            {
                string visible = Calculator.VisibilityValue(Emitter.Visibility.GetValue());
                ButtonVisibility.Content = $"Visibility: ({visible})";
            }
            else
            {
                ButtonVisibility.Content = $"Visibility: ({Emitter.Visibility.Count})";
            }
            ButtonInitialVelocity.Content = Emitter.InitialVelocity.Static ? $"Initial Velocity: {Emitter.InitialVelocity.GetValue()}" : $"Initial Velocity: ({Emitter.InitialVelocity.Count})";
            Check_usemd.IsChecked = Emitter.EmitterUsesMdl;
            Check_usetga.IsChecked = Emitter.EmitterUsesTga;
            ButtonLifespan.Content = Emitter.LifeSpan.Static ? $"Lifespan: {Emitter.LifeSpan.GetValue()}" : $"Lifespan: ({Emitter.LifeSpan.Count})";
            ButtonGravity.Content = Emitter.Gravity.Static ? $"Gravity: {Emitter.Gravity.GetValue()}" : $"Gravity: ({Emitter.Gravity.Count})";
            ButtonLongtitude.Content = Emitter.Longitude.Static ? $"Longitude: {Emitter.Longitude.GetValue()}" : $"Longitude: ({Emitter.Longitude.Count})";
            ButtonLatitude.Content = Emitter.Latitude.Static ? $"Latitude: {Emitter.Latitude.GetValue()}" : $"Latitude: ({Emitter.Latitude.Count})";
        }

        private void editemission(object sender, RoutedEventArgs e)
        {
            transformation_editor editor = new transformation_editor(Model, Emitter.EmissionRate, true, TransformationType.Float);
            editor.ShowDialog();
        }

        private void editlifespan(object sender, RoutedEventArgs e)
        {
            transformation_editor editor = new transformation_editor(Model, Emitter.LifeSpan, true, TransformationType.Float);
            editor.ShowDialog(); Fill();
        }

        private void editinitial(object sender, RoutedEventArgs e)
        {
            transformation_editor editor = new transformation_editor(Model, Emitter.InitialVelocity, true, TransformationType.Float);
            editor.ShowDialog(); Fill();
        }

        private void editgravity(object sender, RoutedEventArgs e)
        {
            transformation_editor editor = new transformation_editor(Model, Emitter.Gravity, true, TransformationType.Float);
            editor.ShowDialog(); Fill();
        }

        private void editlongti(object sender, RoutedEventArgs e)
        {
            transformation_editor editor = new transformation_editor(Model, Emitter.Longitude, true, TransformationType.Float);
            editor.ShowDialog(); Fill();
        }

        private void editlat(object sender, RoutedEventArgs e)
        {
            transformation_editor editor = new transformation_editor(Model, Emitter.Latitude, true, TransformationType.Float);
            editor.ShowDialog(); Fill();
        }

        private void editvis(object sender, RoutedEventArgs e)
        {
            transformation_editor editor = new transformation_editor(Model,  Emitter.Visibility, true, TransformationType.Visibility);
            editor.ShowDialog(); Fill();
        }

        private void ChckedUseMDL(object sender, RoutedEventArgs e)
        {
            Emitter.EmitterUsesMdl = Check_usemd.IsChecked == true;
        }

        private void CheckedUsesTGA(object sender, RoutedEventArgs e)
        {
            Emitter.EmitterUsesTga = Check_usetga.IsChecked == true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; }
        }
    }
}
