using MdxLib.Model;
using MdxLib.Primitives;
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
    /// Interaction logic for edit_emitter2.xaml
    /// </summary>
    public partial class edit_emitter2 : Window
    {
        CModel Model;
        CParticleEmitter2 Emitter;
        bool Initialized = false;
        public edit_emitter2(CParticleEmitter2 emitter, CModel model)
        {
            InitializeComponent();
            if (model.Textures.Count == 0) { MessageBox.Show("No textures"); All.IsEnabled = false; }
            Emitter = emitter;
            Model = model;
            Fill();
            Initialized = true;
        }
        void Fill()
        {
            ComboFilterMode.SelectedIndex = (int)Emitter.FilterMode;
            foreach (CTexture textue in Model.Textures)
            {
                string name= "";
                if (textue.ReplaceableId == 0) { name = textue.FileName; }
              else   if (textue.ReplaceableId == 1) { name = "Team Color"; }
                else if (textue.ReplaceableId == 2) { name = "Team Glow"; }
                else  { name = $"Replaceable ID {textue.ReplaceableId}"; }
                ComboTexture.Items.Add(new ListBoxItem() { Content = name });
            }
            if ((Emitter.Texture == null || Emitter.Texture.Object == null) == false) {
                ComboTexture.SelectedIndex = Model.Textures.IndexOf(Emitter.Texture.Object); }
            Check_Unshaded.IsChecked = Emitter.Unshaded;
            Check_Unfogged.IsChecked = Emitter.Unfogged;
            Check_Tail.IsChecked = Emitter.Tail;
            Check_Head.IsChecked = Emitter.Head;
            Check_LineEmitter.IsChecked = Emitter.LineEmitter;
            Check_ModelSpace.IsChecked = Emitter.ModelSpace;
            Check_Sort.IsChecked = Emitter.SortPrimitivesFarZ;
            Check_Squirt.IsChecked = Emitter.Squirt;
            Check_XY.IsChecked = Emitter.XYQuad;
            Check_Sort.IsChecked = Emitter.SortPrimitivesFarZ;
            ButtonVisibility.Content = Emitter.Visibility.Static ?
                $"Visibility: {GetVisibility(Emitter.Visibility.GetValue())}" :
                $"Visibility: ({Emitter.Visibility.Count})";
            ButtonEmissionRate.Content = Emitter.Visibility.Static ?
                $"Emission Rate: {Emitter.EmissionRate.GetValue()}" :
                $"Emission Rate: ({Emitter.EmissionRate.Count})";
            ButtonSpeed.Content = Emitter.Visibility.Static ?
                $"Speed: {Emitter.Speed.GetValue()}" :
                $"Speed: ({Emitter.Speed.Count})";
            ButtonVariation.Content = Emitter.Variation.Static ?
               $"Variation: {Emitter.Variation.GetValue()}" :
               $"Variation: ({Emitter.Variation.Count})";
            ButtonLatitude.Content = Emitter.Latitude.Static ?
              $"Latitude: {Emitter.Latitude.GetValue()}" :
              $"Latitude: ({Emitter.Latitude.Count})";
            ButtonWidth.Content = Emitter.Width.Static ?
              $"Width: {Emitter.Width.GetValue()}" :
              $"Width: ({Emitter.Width.Count})";
            ButtonLength.Content = Emitter.Length.Static ?
              $"Length: {Emitter.Length.GetValue()}" :
              $"Length: ({Emitter.Length.Count})";
            ButtonGravity.Content = Emitter.Gravity.Static ?
          $"Gravity: {Emitter.Gravity.GetValue()}" :
          $"Gravity: ({Emitter.Gravity.Count})";
            ButtonColor1.Background = Calculator.War3ColorToBrush( Emitter.Segment1.Color);
             ButtonColor2.Background = Calculator.War3ColorToBrush( Emitter.Segment2.Color);
            ButtonColor3.Background = Calculator.War3ColorToBrush( Emitter.Segment3.Color);
            //----------------------------------------------------------------
            InputHeadStart.Text = Emitter.HeadLife.Start.ToString();
            InputHeadEnd.Text = Emitter.HeadLife.End.ToString();
            InputHeadRepeat.Text = Emitter.HeadLife.Repeat.ToString();
            //----------------------------------------------------------------
            InputHeadStartDecay.Text = Emitter.HeadDecay.Start.ToString();
            InputHeadEndDecay.Text = Emitter.HeadDecay.End.ToString();
            InputHeadRepeatDecay.Text = Emitter.HeadDecay.Repeat.ToString();
           //----------------------------------------------------------------
            InputTailLifespanStart.Text = Emitter.TailLife.Start.ToString();
            InputTailLifespanEnd.Text = Emitter.TailLife.End.ToString();
            InputTailLifespanRepeat.Text = Emitter.TailLife.Repeat.ToString();
            //----------------------------------------------------------------
            InputTailDecayStart.Text = Emitter.TailDecay.Start.ToString();
            InputTailDecayEnd.Text = Emitter.TailDecay.End.ToString();
            InputTailDecayRepeat.Text = Emitter.TailDecay.Repeat.ToString();
            //----------------------------------------------------------------
            InputRows.Text = Emitter.Rows.ToString();
            InputColumns.Text = Emitter.Columns.ToString();
            InputLifespan.Text = Emitter.LifeSpan.ToString();
            InputTailLength.Text = Emitter.TailLength.ToString();
            InputPriorityPlane.Text = Emitter.PriorityPlane.ToString();
            InputTime.Text = Emitter.Time.ToString();
            InputReplaceableID.Text = Emitter.ReplaceableId.ToString();
            InputAlpha1.Text = Calculator._255ToPercentage(Emitter.Segment1.Alpha).ToString();
            InputAlpha2.Text = Calculator._255ToPercentage(Emitter.Segment2.Alpha).ToString();
            InputAlpha3.Text = Calculator._255ToPercentage(Emitter.Segment3.Alpha).ToString();
            InputScaling1.Text = (Emitter.Segment1.Scaling ).ToString();
            InputScaling2.Text = (Emitter.Segment2.Scaling ).ToString();
            InputScaling3.Text = (Emitter.Segment3.Scaling ).ToString();
        }
        private object GetVisibility(float v)
        {
            if (v > 0) return "Visible";
            return "Invisible";
        }
        private void EditVisibility(object sender, RoutedEventArgs e)
        {
            transformation_editor editor = new transformation_editor(Model, Emitter.Visibility, true, TransformationType.Visibility)
               ;editor.ShowDialog();
            Fill();
        }
        private void EditEmissionRate(object sender, RoutedEventArgs e)
        {
            transformation_editor editor = new transformation_editor(Model, Emitter.EmissionRate, true, TransformationType.Float)
             ; editor.ShowDialog();
            Fill();
        }
        private void EditSpeed(object sender, RoutedEventArgs e)
        {
            transformation_editor editor = new transformation_editor(Model, Emitter.Speed, 
                true, TransformationType.Float)
          ; editor.ShowDialog();
            Fill();
        }
        private void EditVariation(object sender, RoutedEventArgs e)
        {
            transformation_editor editor = new transformation_editor(Model, Emitter.Variation,
                true, TransformationType.Float)
          ; editor.ShowDialog();
            Fill();
        }
        private void EditLength(object sender, RoutedEventArgs e)
        {
            transformation_editor editor = new transformation_editor(Model, Emitter.Length,
               true, TransformationType.Float)
         ; editor.ShowDialog();
            Fill();
        }
        private void EditWidth(object sender, RoutedEventArgs e)
        {
            transformation_editor editor = new transformation_editor(Model, Emitter.Width,
               true, TransformationType.Float)
         ; editor.ShowDialog();
            Fill();
        }
        private void EditLatitude(object sender, RoutedEventArgs e)
        {
            if (!Initialized) { return; }
            transformation_editor editor = new transformation_editor(Model, Emitter.Latitude,
               true, TransformationType.Float)
         ; editor.ShowDialog();
            Fill();
        }
        private void Checked_Unshaded(object sender, RoutedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.Unshaded = Check_Unshaded.IsChecked == true;
        }
        private void Checked_Unfogged(object sender, RoutedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.Unfogged = Check_Unfogged.IsChecked == true;
        }
        private void Checked_alphakey(object sender, RoutedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.FilterMode = EParticleEmitter2FilterMode.AlphaKey;
        }
        private void Checked_LineEmitter(object sender, RoutedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.LineEmitter = Check_LineEmitter.IsChecked == true;
        }
        private void SelectedFilterMode(object sender, SelectionChangedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.FilterMode = (EParticleEmitter2FilterMode)ComboFilterMode.SelectedIndex;
        }
        private void Checked_Sort(object sender, RoutedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.SortPrimitivesFarZ = Check_Sort.IsChecked == true;
        }
        private void Checked_ModelSpace(object sender, RoutedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.ModelSpace = Check_ModelSpace.IsChecked == true;
        }
        private void Checked_XY(object sender, RoutedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.XYQuad = Check_XY.IsChecked == true;    
        }
        private void Checked_Squirt(object sender, RoutedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.Squirt = Check_Squirt.IsChecked == true;    
        }
        private void Checked_Head(object sender, RoutedEventArgs e)
        {
            Emitter.Head = Check_Head.IsChecked == true;
            InputHeadStart.IsEnabled = Check_Head.IsChecked == true;
            InputHeadEnd.IsEnabled = Check_Head.IsChecked == true;
            InputHeadRepeat.IsEnabled = Check_Head.IsChecked == true;
            InputHeadStartDecay.IsEnabled = Check_Head.IsChecked == true;
            InputHeadEndDecay.IsEnabled = Check_Head.IsChecked == true;
            InputHeadRepeatDecay.IsEnabled = Check_Head.IsChecked == true;
        }
        private void Checked_Tail(object sender, RoutedEventArgs e)
        {
            Emitter.Tail = Check_Tail.IsChecked == true;
            InputTailDecayStart.IsEnabled = Check_Tail.IsChecked == true;
            InputTailDecayEnd.IsEnabled = Check_Tail.IsChecked == true;
            InputTailDecayRepeat.IsEnabled = Check_Tail.IsChecked == true;
            InputTailLifespanStart.IsEnabled = Check_Tail.IsChecked == true;
            InputTailLifespanEnd.IsEnabled = Check_Tail.IsChecked == true;
            InputTailLifespanRepeat.IsEnabled = Check_Tail.IsChecked == true;
            InputTailLength.IsEnabled = Check_Tail.IsChecked == true;
        }
        private void SelectedTexture(object sender, SelectionChangedEventArgs e)
        {
            if (!Initialized) { return; }
            if (ComboTexture.SelectedItem != null)
            {
                Emitter.Texture.Attach(Model.Textures[ComboTexture.SelectedIndex]);
            }
        }
        private int GetInt(TextBox t)
        {
            string text = t.Text.Trim();
            if (text.Length > 0)
            {
                bool parse = int.TryParse(text, out int value);
                if (value >= 0) {return value;}
            }
            return 0;
        }
        private float GetFloat(TextBox t)
        {
            string text = t.Text.Trim();
            if (text.Length > 0)
            {
                bool parse = float.TryParse(text, out float value);
                if (parse && value >= 0) { return value; }
            }
            return 0;
        }
        private void SetRows(object sender, TextChangedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.Rows = GetInt(InputRows);
        }
        private void SetColumns(object sender, TextChangedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.Columns = GetInt(InputColumns);
        }
        private void SetLifespan(object sender, TextChangedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.LifeSpan = GetFloat(InputLifespan);
        }
        private void SetTailLength(object sender, TextChangedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.TailLength = GetFloat(InputTailLength);
        }
        private void SetPriorityPlane(object sender, TextChangedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.PriorityPlane = GetInt(InputPriorityPlane);
        }
        private void SetTime(object sender, TextChangedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.Time = GetFloat(InputTime);
        }
        internal static int PercentageToByte(int percentage)
        {
            if (percentage <= 0) return 0;
            if (percentage >= 100) return 255;
            return (int)Math.Round(percentage * (255.0 / 100.0));
        }

        private float GetPercentage255(TextBox inputAlpha1)
        {
           int value = GetInt(inputAlpha1);
            return PercentageToByte(value);
        }
        private CVector3 GetColor(Button buttonColor1)
        {
           int[] values= Calculator.GetColor(buttonColor1);
            return new CVector3(values[2]/ 255, values[1] / 255, values[0] / 255);
        }
        private void SetSegment1(object sender, TextChangedEventArgs e)
        {
            if (!Initialized) { return; }
            CVector3 color = GetColor(ButtonColor1);
            float alpha = GetPercentage255(InputAlpha1); // from percentage
            float scaling = GetFloat(InputScaling1)  ;
            Emitter.Segment1 = new CSegment(color, alpha, scaling);
        }
        private void SetSegment2(object sender, TextChangedEventArgs e)
        {
            if (!Initialized) { return; }
            CVector3 color = GetColor(ButtonColor2);
            float alpha = GetPercentage255(InputAlpha2);
            float scaling = GetFloat(InputScaling2) ;
            Emitter.Segment2 = new CSegment(color, alpha, scaling);
        }
        private void SetSegment3(object sender, TextChangedEventArgs e)
        {
            if (!Initialized) { return; }
            CVector3 color = GetColor(ButtonColor3);
            float alpha = GetPercentage255(InputAlpha3);
            float scaling = GetFloat(InputScaling3);
            Emitter.Segment3 = new CSegment(color, alpha, scaling);
        }
        private void SetColor1(object sender, RoutedEventArgs e)
        {
            if (!Initialized) { return; }
            SetColor(sender as Button);
            SetSegment1(null,null);
        }
        private void SetColor(Button? button)
        {

            //pik color
            System.Windows.Media.Brush newColor = ColorPickerHandler.Pick(button.Background);
            // set to button
                button.Background = newColor;
            
        }
        private void SetColor2(object sender, RoutedEventArgs e)
        {
            if (!Initialized) { return; }
            SetColor(sender as Button);
            SetSegment1(null,null);
        }
        private void SetColor3(object sender, RoutedEventArgs e)
        {
            if (!Initialized) { return; }
            SetColor(sender as Button);
            SetSegment1(null,null);
        }
        private void SetHeadLife(object sender, TextChangedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.HeadLife =
                new CInterval(
                GetInt( InputHeadStart),
                GetInt(InputHeadEnd),
                GetInt(InputHeadRepeat));
         }
        private void SetHedDecay(object sender, TextChangedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.HeadDecay = new CInterval(GetInt(InputHeadStartDecay), GetInt(InputHeadEndDecay), GetInt(InputHeadRepeatDecay));
        }
        private void SetTailDecay(object sender, TextChangedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.TailDecay = new CInterval(GetInt(InputTailDecayStart), GetInt(InputTailDecayEnd), GetInt(InputTailDecayRepeat));
        }
        private void SetTailLife(object sender, TextChangedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.TailLife = new CInterval(GetInt(InputTailLifespanStart), GetInt(InputTailLifespanEnd), GetInt(InputTailLifespanRepeat));
        }
        private void SetRepalceableID(object sender, TextChangedEventArgs e)
        {
            if (!Initialized) { return; }
            Emitter.ReplaceableId = GetInt(InputReplaceableID);
        }
        private void EditGravity(object sender, RoutedEventArgs e)
        {
            transformation_editor editor = new transformation_editor(Model, Emitter.Gravity, true, TransformationType.Float)
             ; editor.ShowDialog();
            Fill();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; }
        }
    }
}
