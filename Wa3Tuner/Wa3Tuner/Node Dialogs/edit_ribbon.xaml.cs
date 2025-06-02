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
    /// Interaction logic for edit_ribbon.xaml
    /// </summary>
    public partial class edit_ribbon : Window
    {
        CRibbonEmitter Emitter;
        CModel Model;
        public edit_ribbon(CModel model, CRibbonEmitter emitter)
        {
            InitializeComponent();
            Model = model;
            Emitter = emitter;
            if (model.Materials.Count == 0 ) { MessageBox.Show("No materials"); DialogResult = false;  }
            Fill();
        }
        void Fill()
        {
            foreach (CMaterial material in Model.Materials)
            {
                ComboTexture.Items.Add(new ListBoxItem() { Content = $"MaterialID {material.ObjectId}" });
            }
            ComboTexture.SelectedIndex = Model.Materials.IndexOf(Emitter.Material.Object);
            InputColumns.Text = Emitter.Columns.ToString();
            InputRows.Text = Emitter.Rows.ToString();   
            InputEmissionRate.Text = Emitter.EmissionRate.ToString();
            InputGravity.Text = Emitter.Gravity.ToString();
            InputLifespan.Text = Emitter.LifeSpan.ToString();
            ButtonVisibility.Content = Emitter.Visibility.Static ? $"Visibility: {Calculator.VisibilityValue(Emitter.Visibility.GetValue())}" : $"Visibility: ({Emitter.Visibility.Count})";
            ButtonAlpha.Content = Emitter.Alpha.Static ? $"Alpha: {Calculator.GetAlpha(Emitter.Alpha.GetValue())}" : $"Alpha: ({Emitter.Alpha.Count})";
     ButtonHeightAbove.Content = Emitter.HeightAbove.Static ? $"HeightAbove: { Emitter.HeightAbove.GetValue()}" : $"HeightAbove: ({Emitter.HeightAbove.Count})";
     ButtonHeightBelow.Content = Emitter.HeightBelow.Static ? $"HeightBelow: { Emitter.HeightBelow.GetValue()}" : $"HeightBelow: ({Emitter.HeightBelow.Count})";
     ButtonTextureSlot.Content = Emitter.TextureSlot.Static ? $"TextureSlot: { Emitter.TextureSlot.GetValue()}" : $"TextureSlot: ({Emitter.TextureSlot.Count})";
            ButtonColor.Content = Emitter.Color.Static == false ? $"Color: ({Emitter.Color.Count})" : "Color";
            if (Emitter.Color.Static) {
                ButtonColor.Background =
                    Calculator.BrushFromWar3Vector3(Emitter.Color.GetValue());
            }
        }
        private void editcolor(object? sender, RoutedEventArgs? e)
        {
            transformation_editor tr = new transformation_editor(Model, Emitter.Color, true, TransformationType.Color);
            tr.ShowDialog(); Fill();
        }
        private void editalpha(object? sender, RoutedEventArgs? e)
        {
            transformation_editor tr = new transformation_editor(Model, Emitter.Alpha, true, TransformationType.Alpha);
            tr.ShowDialog(); Fill();
        }
        private void editha(object? sender, RoutedEventArgs? e)
        {
            transformation_editor tr = new transformation_editor(Model, Emitter.HeightAbove, true, TransformationType.Float);
            tr.ShowDialog(); Fill();
        }
        private void edithb(object? sender, RoutedEventArgs? e)
        {
            transformation_editor tr = new transformation_editor(Model, Emitter.HeightBelow, true, TransformationType.Float);
            tr.ShowDialog(); Fill();
        }
        private void editts(object? sender, RoutedEventArgs? e)
        {
            transformation_editor tr = new transformation_editor(Model, Emitter.TextureSlot, true );
            tr.ShowDialog(); Fill();
        }
        private void editrows(object? sender, TextChangedEventArgs e)
        {
            string i = InputRows.Text;
            bool parsed = int.TryParse(i, out int rows);
            {
                if (parsed)
                {
                    if (rows >= 0) { Emitter.Rows = rows; }
                }
            }
        }
        private void editcolumns(object? sender, TextChangedEventArgs e)
        {
            string i = InputColumns.Text;
            bool parsed = int.TryParse(i, out int columns);
            ;
            {
                if (parsed)
                {
                    if (columns >= 0) { Emitter.Columns = columns; }
                }
            }
        }
        private void editemissionrate(object? sender, TextChangedEventArgs e)
        {
            string i = InputEmissionRate.Text;
            bool parsed = int.TryParse(i, out int er);
            {
                if (parsed)
                {
                    if (er >= 0) { Emitter.EmissionRate = er; }
                }
            }
        }
        private void editlifespan(object? sender, TextChangedEventArgs e)
        {
            string i = InputLifespan.Text;
            bool parsed = int.TryParse(i, out int ls);
            {
                if (parsed)
                {
                    if (ls >= 0) { Emitter.LifeSpan = ls; }
                }
            }
        }
        private void editgravity(object? sender, TextChangedEventArgs e)
        {
            string i = InputGravity.Text;
            bool parsed = int.TryParse(i, out int gravity);
            {
                if (parsed)
                {
                    if (gravity >= 0) { Emitter.Gravity = gravity; }
                }
            }
        }
        private void ComboTexture_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (ComboTexture.SelectedItem != null)
            {
                Emitter.Material.Attach(Model.Materials[ComboTexture.SelectedIndex]);
            }
        }
        private void editvis(object? sender, RoutedEventArgs? e)
        {
            transformation_editor tr = new transformation_editor(Model, Emitter.Visibility, true, TransformationType.Visibility);
            tr.ShowDialog();
            Fill();
        }
        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; }
        }
    }
}
