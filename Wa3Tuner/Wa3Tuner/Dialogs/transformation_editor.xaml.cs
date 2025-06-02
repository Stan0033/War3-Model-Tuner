 
using MdxLib.Animator;
using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
 
using System.Linq;
 
using System.Text;
using System.Windows;
using System.Windows.Controls;
 
using System.Windows.Input;
using System.Windows.Media;
using Wa3Tuner;
using Wa3Tuner.Dialogs;
using Wa3Tuner.Helper_Classes;
namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for tranformation_editor.xaml
    /// </summary>
    public enum TransformationType
    {
        Translation, Vector4, Float, Color, Int,
        Scaling,
        Visibility,
        Rotation,
        Alpha,
        None
    }

    public partial class transformation_editor : Window
    {
        CModel Model = new CModel();
        public bool InitializedW = false;
        private TransformationType Type;
        public CAnimator<int>? Dummy_int;
        public CAnimator<float>? Dummy_float;
        public CAnimator<CVector3>? Dummy_Vector3;
        public CAnimator<CVector4> ?Dummy_Vector4;
        private List<Ttrack> Tracks = new List<Ttrack>();
        public transformation_editor(CModel model, CAnimator<float> animator, bool canBeStatic,
            TransformationType type)
        {
            if (animator == null) { Close(); return; }
            InitializeComponent();
            if (model.Sequences.Count == 0) { MessageBox.Show("There are no sequences", "Nothing to work with"); All.IsEnabled = false; }
            InitializedW = true;
            Dummy_float = animator;
            Type = type;
            Model = model;
            if (canBeStatic == false) { RadioStatic.IsEnabled = false; RadioDynamic.IsChecked = true; }
            if (animator.Static) { RadioStatic.IsChecked = true; } else { RadioDynamic.IsChecked = true; }
            Combo_InterpType.SelectedIndex = (int)animator.Type;
            ButtonColor.IsEnabled = false;
            foreach (var item in animator)
            {
                Tracks.Add(new Ttrack(item.Time, item.Value));
            }
            if (type == TransformationType.Alpha) ConvertAlphaTracks(); SetSpecial.IsEnabled = true;
            if (type == TransformationType.Visibility)
            {
                StaticInput.Visibility = Visibility.Collapsed;
                ValueInput.Visibility = Visibility.Collapsed;
                ButtonColor.Visibility = Visibility.Collapsed;
                Check_InputVisible.Visibility = Visibility.Visible;
                Check_StaticVisible.Visibility = Visibility.Visible;
                SetSpecial.IsEnabled = true;
            }
            RefreshTracks();
            FillSequences();
            Title = $"Transformation Editor - {type}";
            FillGlobalSEquence(animator.GlobalSequence.Object);
        }
        private void FillStaticValue()
        {
            if (Type == TransformationType.Alpha && Dummy_float != null)
            {
                StaticInput.Text = (Dummy_float.GetValue() * 100).ToString();
            }

            if (Type == TransformationType.Color && Dummy_Vector3 != null)
            {
                StaticInput.Text = Calculator.BGRnToRGB(Dummy_Vector3.GetValue());
            }

            if (Type == TransformationType.Translation && Dummy_Vector3 != null)
            {
                StaticInput.Text = GetStatic(Dummy_Vector3.GetValue());
            }

            if (Type == TransformationType.Scaling && Dummy_Vector3 != null)
            {
                StaticInput.Text = GetStaticP(Dummy_Vector3.GetValue());
            }

            if (Type == TransformationType.Rotation && Dummy_Vector4 != null)
            {
                StaticInput.Text = Calculator.QuaternionToEuler_(Dummy_Vector4.GetValue());
            }

            if (Type == TransformationType.Visibility && Dummy_float != null)
            {
                Check_StaticVisible.IsChecked = GetVisibileBool(Dummy_float.GetValue());
            }

            if (Type == TransformationType.Int && Dummy_float != null)
            {
                StaticInput.Text = Dummy_float.GetValue().ToString();
            }

            if (Type == TransformationType.Float && Dummy_float != null)
            {
                StaticInput.Text = Dummy_float.GetValue().ToString();
            }
        }

        private static bool GetVisibileBool(float value)
        {
            if (value >= 1) { return true; }
            return false;
        }
        private static string GetStaticP(CVector3 cVector3)
        {
            return $"{cVector3.X * 100}, {cVector3.Y * 100}, {cVector3.Z * 100}";
        }
        private static string GetStatic(CVector3 cVector3)
        {
            return $"{cVector3.X}, {cVector3.Y}, {cVector3.Z}";
        }
        private void FillGlobalSEquence(CGlobalSequence gs_)
        {
            Combo_GlobalSequence.Items.Add(new ComboBoxItem() { Content = "None" });
            foreach (CGlobalSequence gs in Model.GlobalSequences)
            {
                Combo_GlobalSequence.Items.Add(new ComboBoxItem() { Content = $"{gs.ObjectId} ({gs.Duration})" });
            }
            if (gs_ != null)
            {
                Combo_GlobalSequence.SelectedIndex = Model.GlobalSequences.IndexOf(gs_) + 1;
            }
            else
            {
                Combo_GlobalSequence.SelectedIndex = 0;
            }
        }
        private void ConvertAlphaTracks()
        {
            foreach (var track in Tracks)
            {
                track.ToPercentage();
            }
        }
        public transformation_editor(CModel model, CAnimator<int> animator, bool canBeStatic)
        {
            if (animator == null) { Close(); return; }
            InitializeComponent();
            if (model.Sequences.Count == 0) { MessageBox.Show("There are no sequences", "Nothing to work with"); All.IsEnabled = false; }
            InitializedW = true;
            Type = TransformationType.Int;
            if (canBeStatic == false) { RadioStatic.IsEnabled = false; RadioDynamic.IsChecked = true; }
            if (animator.Static) { RadioStatic.IsChecked = true; } else { RadioDynamic.IsChecked = true; }
            Model = model;
            Combo_InterpType.SelectedIndex = (int)animator.Type;
            ButtonColor.IsEnabled = false;
            Dummy_int = animator;
            foreach (var item in animator)
            {
                Tracks.Add(new Ttrack(item.Time, item.Value));
            }
            RefreshTracks();
            FillSequences(); Title = $"Transformation Editor - Int";
            FillGlobalSEquence(animator.GlobalSequence.Object);
        }
        public transformation_editor(CModel  model, CAnimator<CVector3>  animator, bool  canBeStatic,
            TransformationType type)
        {
            if (animator == null) { Close(); return; }
            InitializeComponent();
            if (model.Sequences.Count == 0) { MessageBox.Show("There are no sequences", "Nothing to work with"); All.IsEnabled = false; }
            InitializedW = true;
            Type = type;
            Dummy_Vector3 = animator;
            Model = model;
            Combo_InterpType.SelectedIndex = (int)animator.Type;
            if (canBeStatic == false) { RadioStatic.IsEnabled = false; RadioDynamic.IsChecked = true; }
            if (animator.Static) { RadioStatic.IsChecked = true; } else { RadioDynamic.IsChecked = true; }
            ButtonColor.Visibility = type == TransformationType.Color ? Visibility.Visible : Visibility.Collapsed;
            foreach (var item in animator)
            {
                Tracks.Add(new Ttrack(item.Time, item.Value.X, item.Value.Y, item.Value.Z));
            }
            if (type == TransformationType.Color)
            {
                ConvertColorTracks();
                StaticInputColor.Visibility = Visibility.Visible;
                StaticInput.Visibility = Visibility.Collapsed;
                ValueInput.Visibility = Visibility.Collapsed;
                //------------------------------------------
                var rawColor = animator.GetValue();

                 
                //------------------------------------------
                SolidColorBrush brush = new SolidColorBrush();
                System.Windows.Media.Color color = new System.Windows.Media.Color();
                color.A = 255;
                color.R = (byte)(rawColor.X * 255f);
                color.G = (byte)(rawColor.Y * 255f);
                color.B = (byte)(rawColor.Z * 255f);
                brush.Color = color;
                StaticInputColor.Background = brush;

                //---------------------------------------------------------------------
            }
            if (type == TransformationType.Scaling) ConvertScalingTracks();
            RefreshTracks();
            FillSequences(); Title = $"Transformation Editor - {type}";
            FillGlobalSEquence(animator.GlobalSequence.Object);
        } //GetStaticQuaternion(animator.GetValue());
       
        public transformation_editor()
        {
            InitializeComponent();
            Close();
        }
        public transformation_editor(CModel model, CAnimator<CVector4> animator, bool canBeStatic)
        {
            if (animator == null) { Close(); return; }
            InitializeComponent();
            if (model.Sequences.Count == 0) { MessageBox.Show("There are no sequences", "Nothing to work with"); All.IsEnabled = false; }
            InitializedW = true;
            Type = TransformationType.Rotation;
            Dummy_Vector4 = animator;
            if (animator.Static) { RadioStatic.IsChecked = true; } else { RadioDynamic.IsChecked = true; }
            Model = model;
            if (canBeStatic == false) { RadioStatic.IsEnabled = false; RadioDynamic.IsChecked = true; }
            if (animator.Static) { RadioStatic.IsChecked = true; } else { RadioDynamic.IsChecked = true; }
            ButtonColor.IsEnabled = false;
            Combo_InterpType.SelectedIndex = (int)animator.Type;
            foreach (var item in animator)
            {
                Tracks.Add(new Ttrack(item.Time, item.Value.X, item.Value.Y, item.Value.Z, item.Value.W));
            }
            ConvertRotationTracks();
            RefreshTracks();
            FillSequences(); Title = $"Transformation Editor - Rotation";
            FillGlobalSEquence(animator.GlobalSequence.Object);
        }
        private void ConvertRotationTracks()
        {
            foreach (Ttrack track in Tracks)
            {
                track.ToEuler();
            }
        }
        private void FillSequences()
        {
            foreach (CSequence sequence in Model.Sequences)
            {
                string name = $"{sequence.Name} [{sequence.IntervalStart} - {sequence.IntervalEnd}]";
                SequenceSelector.Items.Add(new ComboBoxItem() { Content = name });
            }
        }
        private void RefreshTracks()
        {
            Tracks = Tracks.OrderBy(x => x.Time).ToList(); ;
            Info_Keyframes.Text = $"{Tracks.Count} keyframes";
            MainList.Items.Clear();
            int count = 0;
            foreach (Ttrack track in Tracks)
            {
                ListBoxItem item = new ListBoxItem();
                StackPanel panel = new StackPanel();
                TextBlock number = new TextBlock();
                TextBlock sequence = new TextBlock();
                sequence.Foreground = System.Windows.Media.Brushes.White;
                sequence.Background = System.Windows.Media.Brushes.Gray;
                sequence.Width = 200;
                sequence.Text = GetSequence(track.Time);
                TextBlock value = new TextBlock();
                value.Margin = new Thickness(5, 0, 5, 0);
                value.Width = 400;
                value.Text = track.GetValue(Type);
                number.Width = 50;
                number.Text = count.ToString();
                count++;
                TextBlock time = new TextBlock();
                time.Width = 50;
                time.Text = track.Time.ToString();
                time.Margin = new Thickness(5, 0, 5, 0);
                panel.Orientation = Orientation.Horizontal;
                panel.Children.Add(number);
                panel.Children.Add(sequence);
                panel.Children.Add(time);
                panel.Children.Add(value);
                item.Content = panel;
                MainList.Items.Add(item);
            }
        }
        private string GetSequence(int time)
        {
            foreach (CSequence sequence in Model.Sequences)
            {
                if (time >= sequence.IntervalStart && time <= sequence.IntervalEnd)
                {
                    string suffix = "";
                    if (time == sequence.IntervalStart) { suffix = " (Start)"; }
                    if (time == sequence.IntervalEnd) { suffix = " (End)"; }
                    return sequence.Name + suffix;
                }
            }
            return "";
        }
        private void ConvertScalingTracks()
        {
            foreach (var track in Tracks)
            {
                track.ToPercentage();
            }
        }
        private void ConvertColorTracks()
        {
            foreach (var track in Tracks)
            {
                track.ToColor();
            }
        }
        private void SetStatic(object? sender, RoutedEventArgs? e)
        {
            if (!InitializedW) return;
            StaticInput.IsEnabled = true;
            StaticInputColor.IsEnabled = true;
            MainList.IsEnabled = false;
            stack1.IsEnabled = false;
            stack2.IsEnabled = false;
            stack0.IsEnabled = false;
            Check_StaticVisible.IsEnabled = true; ;
            FillStaticValue();
        }
        private void explain(object? sender, RoutedEventArgs? e)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("For rotations, this app uses euler(x, y, z) instead of quaternion (x, y, z, w).");
            messageBuilder.AppendLine("For scalings, this app uses 0+ percentage instead of normalized percentage.");
            messageBuilder.AppendLine("For alphas, this app uses 0-100 percentage instead of normalized percentage.");
            messageBuilder.AppendLine("For colors, this app uses standard RGB instead of normalized reversed RGB.");
            messageBuilder.AppendLine("For visibility, this app uses 'visible' and 'invisible' (enter 1 or 0 in the box) instead of 1 and 0.");
            messageBuilder.AppendLine("Int and float transformations remain the same.");
            MessageBox.Show(messageBuilder.ToString());
        }
        private void SeletedSequence(object? sender, SelectionChangedEventArgs e)
        {
            int index = SequenceSelector.SelectedIndex;
            TrackInput.Text = Model.Sequences[index].IntervalStart.ToString();
        }
        private void del(object? sender, RoutedEventArgs? e)
        {
            if (MainList.SelectedItem != null)
            {
                int index = MainList.SelectedIndex;
                Tracks.RemoveAt(index);
                MainList.Items.Remove(MainList.SelectedItem);

            }
        }
        
        private bool TimeExists(int time)
        {
            foreach (CSequence sequence in Model.Sequences)
            {
                if (time >= sequence.IntervalStart && time <= sequence.IntervalEnd)
                {
                    return true;
                }
            }
            return false;
        }
        private bool InputTrackCorrect(string time_, string input_)
        {
            bool parseTime = int.TryParse(time_, out int time);
            if (!parseTime) { return false; }
            if (time < 0) { MessageBox.Show("Negative track", "Error"); return false; }
            if (TimeExists(time) == false) { MessageBox.Show("This track does not exist in any sequence", "Error"); return false; }
            if (input_.Length == 0) { return false; }
            string[] parts = input_.Split(',').Select(x => x.Trim()).ToArray();
            if (Type == TransformationType.Int)
            {
                bool parsed1 = int.TryParse(parts[0], out int x);
                if (parsed1) { return true; }
            }
            else if (Type == TransformationType.Visibility)
            {
                bool parsed1 = int.TryParse(parts[0], out int x);
                if (parsed1)
                {
                    if (x == 0 | x == 1) { return true; }
                }
            }
            else if (Type == TransformationType.Float)
            {
                bool parsed1 = float.TryParse(parts[0], out float x);
                if (parsed1) { return true; }
            }
            else if (Type == TransformationType.Translation)
            {
                if (parts.Length != 3) { return false; }
                bool parsed1 = float.TryParse(parts[0], out float x);
                bool parsed2 = float.TryParse(parts[1], out float y);
                bool parsed3 = float.TryParse(parts[2], out float z);
                if (parsed1 && parsed2 && parsed3)
                {

                    return true;

                }
            }
            else if (Type == TransformationType.Rotation)
            {
                bool parsed1 = float.TryParse(parts[0], out float x);
                bool parsed2 = float.TryParse(parts[1], out float y);
                bool parsed3 = float.TryParse(parts[2], out float z);
                if (parsed1 && parsed2 && parsed3)
                {
                    if (
                        x >= -360 && x <= 360 &&
                        y >= -360 && y <= 360 &&
                        z >= -360 && z <= 360)
                    {
                        return true;
                    }
                }
            }
            else if (Type == TransformationType.Color)
            {
                bool parsed1 = float.TryParse(parts[0], out float x);
                bool parsed2 = float.TryParse(parts[1], out float y);
                bool parsed3 = float.TryParse(parts[2], out float z);
                if (parsed1 && parsed2 && parsed3)
                {
                    if (
                        x >= 0 && x <= 255 &&
                        y >= 0 && y <= 255 &&
                        z >= 0 && z <= 255)
                    {
                        return true;
                    }
                }
            }
            else if (Type == TransformationType.Alpha)
            {
                bool parsed1 = float.TryParse(parts[0], out float x);
                return x >= 0 && x <= 100;
            }
            else if (Type == TransformationType.Scaling)
            {
                bool parsed1 = float.TryParse(parts[0], out float x);
                bool parsed2 = float.TryParse(parts[1], out float y);
                bool parsed3 = float.TryParse(parts[2], out float z);
                if (parsed1 && parsed2 && parsed3)
                {
                    if (
                        x >= 0 &&
                        y >= 0 &&
                        z >= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void newItem(object? sender, RoutedEventArgs? e)
        {
            string time = TrackInput.Text.Trim();
            string input = ValueInput.Text.Trim();

            bool parsedTime = int.TryParse(time, out int time_);
            if (!parsedTime) { MessageBox.Show("Expected integer for time"); return; }
            if (Tracks.Any(x => x.Time == time_))
            {
                MessageBox.Show("There is already a track with this time"); return;
            }
            Ttrack track = new  ();
            if (Type == TransformationType.Visibility)
            {
                int visible = Check_InputVisible.IsChecked == true ? 1 : 0;
                track = new Ttrack(time_, visible);
                Tracks.Add(track);
                RefreshTracks();
            }
            else
            {
                if (InputTrackCorrect(time, input))
                {
                    if (Type == TransformationType.Visibility)
                    {
                        int visible = Check_InputVisible.IsChecked == true ? 1 : 0;
                        track = new Ttrack(time_, visible);
                    }
                    else
                    {
                        float[] values = ExtractValues(input);
                        if (Type == TransformationType.Rotation) track = new Ttrack(time_, values[0], values[1], values[2]);
                        if (Type == TransformationType.Scaling) track = new Ttrack(time_, values[0], values[1], values[2]);
                        if (Type == TransformationType.Color) track = new Ttrack(time_, values[0], values[1], values[2]);
                        if (Type == TransformationType.Translation) track = new Ttrack(time_, values[0], values[1], values[2]);
                        if (Type == TransformationType.Alpha) track = new Ttrack(time_, values[0]);
                    }
                    Tracks.Add(track);
                    RefreshTracks();
                }
                else
                {
                    MessageBox.Show("Incorrect input or format"); return;
                }
            }
        }
        private void edit(object? sender, RoutedEventArgs? e)
        {
            if (MainList.SelectedItem != null)
            {
                string time = TrackInput.Text.Trim();
                string input = ValueInput.Text.Trim();
                bool parsedTime = int.TryParse(time, out int time_);
                if (!parsedTime) { MessageBox.Show("Expected integer for time"); return; }
                if (Type == TransformationType.Visibility)
                {
                    int visible = Check_InputVisible.IsChecked == true ? 1 : 0;
                    Tracks[MainList.SelectedIndex].X = visible;
                    RefreshTracks();
                }
                else
                {
                    if (InputTrackCorrect(time, input))
                    {
                        Ttrack track = Tracks[MainList.SelectedIndex];
                        float[] values = ExtractValues(input);
                        track.Update(values);
                        RefreshTracks();
                    }
                    else
                    {
                        MessageBox.Show("Incorrect input or format"); return;
                    }
                }
            }
        }
        private static float[] ExtractValues(string item)
        {
            List<float> vals = new List<float>();
            string[] i = item.Split(',').Select(x => x.Trim()).ToArray();
            foreach (string s in i)
            {
                vals.Add(float.Parse(s));
            }
            return vals.ToArray();
        }
        private void SelectedTrack(object? sender, SelectionChangedEventArgs e)
        {
            int index = MainList.SelectedIndex;
            if (index == -1) { return; }
            TrackInput.Text = Tracks[index].Time.ToString();
            if (Type == TransformationType.Visibility)
            {
                string value = Tracks[index].GetValue(Type);
                Check_InputVisible.IsChecked = value == "Visible";
            }
            else if (Type == TransformationType.Color)
            {
                int r = (int)Tracks[index].X;
                int g = (int)Tracks[index].Y;
                int b = (int)Tracks[index].Z;
                ButtonColor.Background = Calculator.RGBToBrush(r, g, b);
            }
            else
            {
                ValueInput.Text = Tracks[index].GetValue(Type);
            }
        }
        private void setColor(object? sender, RoutedEventArgs? e)
        {

            System.Windows.Media.Brush newColor = ColorPickerHandler.Pick(StaticInputColor.Background);
            // set to button
            ButtonColor.Background = newColor;
            //set to data
            var color = Calculator.BrushToColor(newColor);
            CVector3 c = Calculator.ColorToWar3Color(color);

        }
        private CVector3?  GetStaticV3(bool scaling = false)
        {
            string input = StaticInput.Text.Trim();
            if (input.Length == 0) { return null; }
            string[] parts = input.Split(',').Select(u => u.Trim()).ToArray(); ;
            if (parts.Length != 3) { return null; }
            bool parse1 = float.TryParse(parts[0], out float x);
            bool parse2 = float.TryParse(parts[1], out float y);
            bool parse3 = float.TryParse(parts[2], out float z);
            if (scaling)
            {
                x /= 100;
                y /= 100;
                z /= 100;
            }
            if (!parse1 || !parse2 || !parse3) { return null; }
            return new CVector3(x, y, z);
        }
        private void update_click(object? sender, RoutedEventArgs? e)
        {
            
            Tracks = Tracks.OrderBy(X => X.Time).ToList();
            if (Type == TransformationType.Translation)
            {
                if (Dummy_Vector3 == null) { MessageBox.Show("Null transformation container", "Error. Report to the developer."); return; }
              
                Dummy_Vector3.Clear();
                if (RadioStatic.IsChecked == true)
                {
                    CVector3? vector = GetStaticV3(); if (vector == null) return;
                    if (vector == null) { MessageBox.Show("Invalid static value input"); return; }
                    Dummy_Vector3.MakeStatic(vector);
                    DialogResult = true;
                    return;
                }
                else
                {
                    Dummy_Vector3.MakeAnimated();
                    Dummy_Vector3.Clear();
                    foreach (var track in Tracks)
                    {
                        Dummy_Vector3.Add(new CAnimatorNode<CVector3>(track.Time, new CVector3(track.X, track.Y, track.Z)));
                    }
                    DialogResult = true;
                }
            }
            else if (Type == TransformationType.Alpha)
            {
                if (Dummy_float == null) { MessageBox.Show("Null transformation container", "Error. Report to the developer.");return; }

                if (RadioStatic.IsChecked == true)
                {
                    bool parsed = float.TryParse(StaticInput.Text, out float value);
                    if (parsed)
                    {
                        if (Dummy_float == null) return;
                        Dummy_float.MakeStatic(value / 100);
                        DialogResult = true;
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Invalid input for static value");
                        return;
                    }
                }
                else
                {
                    if (Dummy_float == null) return;
                    Dummy_float.Clear();
                    Dummy_float.MakeAnimated();
                    foreach (var track in Tracks)
                    {
                        Dummy_float.Add(new CAnimatorNode<float>(track.Time, track.X / 100));
                    }
                    DialogResult = true;
                }
            }
            else if (Type == TransformationType.Scaling)
            {
                if (Dummy_Vector3 == null) { MessageBox.Show("Null transformation container", "Error. Report to the developer."); return; }

                if (RadioStatic.IsChecked == true)
                {
                    CVector3 ? vector = GetStaticV3(true); if (vector == null) return;
                    if (vector == null) { MessageBox.Show("Invalid static value input"); return; }
                    Dummy_Vector3.MakeStatic(vector);
                    DialogResult = true;
                    return;
                }
                else
                {
                    Dummy_Vector3.MakeAnimated();
                    Dummy_Vector3.Clear();
                    foreach (var track in Tracks)
                    {
                        track.ToNormalizedPercentage();
                        Dummy_Vector3.Add(new CAnimatorNode<CVector3>(track.Time, new CVector3(track.X, track.Y, track.Z)));
                    }
                    DialogResult = true;
                }
            }
            else if (Type == TransformationType.Rotation)
            {
                if (Dummy_Vector4 == null) { MessageBox.Show("Null transformation container", "Error. Report to the developer."); return; }

                if (RadioStatic.IsChecked == true)
                {
                    if (Dummy_Vector4 == null) return;
                    CVector4? vector = GetStaticV4();
                    if (vector == null) { MessageBox.Show("Invalid static value input"); return; }
                    Dummy_Vector4.MakeStatic(vector);
                    DialogResult = true;
                    return;
                }
                else
                {
                    if (Dummy_Vector4 == null) return;
                    Dummy_Vector4.MakeAnimated();
                    Dummy_Vector4.Clear();
                    foreach (var track in Tracks)
                    {
                        track.ToQuaternion();
                        Dummy_Vector4.Add(new CAnimatorNode<CVector4>(track.Time, new CVector4(track.X, track.Y, track.Z, track.W)));
                    }
                    DialogResult = true;
                }
            }
            else if (Type == TransformationType.Int)
            {
                if (Dummy_int == null) { MessageBox.Show("Null transformation container", "Error. Report to the developer."); return; }

                if (RadioStatic.IsChecked == true)
                {
                    bool parsed = int.TryParse(StaticInput.Text, out int value);
                    if (parsed)
                    {
                        if (Dummy_int == null) return;
                        Dummy_int.MakeStatic(value);
                        DialogResult = true;
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Invalid input for static value");
                        return;
                    }
                }
                else
                {
                    if (Dummy_int == null) return;
                    Dummy_int.MakeAnimated();
                    Dummy_int.Clear();
                    foreach (var track in Tracks)
                    {
                        Dummy_int.Add(new CAnimatorNode<int>(track.Time, (int)track.X));
                    }
                    DialogResult = true;
                }
            }

            else if (Type == TransformationType.Float)
            {
                if (Dummy_float == null) { MessageBox.Show("Null transformation container", "Error. Report to the developer."); return; }

                if (RadioStatic.IsChecked == true)
                {
                    bool parsed = float.TryParse(StaticInput.Text, out float value);
                    if (parsed)
                    {
                        if (Dummy_float == null) return;
                        Dummy_float.MakeStatic(value);
                        DialogResult = true;
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Invalid input for static value");
                        return;
                    }
                }
                else
                {
                    if (Dummy_float == null) return;
                    Dummy_float.MakeAnimated();
                    Dummy_float.Clear();
                    foreach (var track in Tracks)
                    {
                        Dummy_float.Add(new CAnimatorNode<float>(track.Time, track.X));
                    }
                    DialogResult = true;
                }
            }
            else if (Type == TransformationType.Color)
            {
                if (Dummy_Vector3 == null) { MessageBox.Show("Null transformation container", "Error. Report to the developer."); return; }

                Dummy_Vector3.Clear();
                if (RadioStatic.IsChecked == true)
                {
                    CVector3 vector = getstaticColorInput();
                    
                    
                    if (vector == null) { MessageBox.Show("Invalid static value input"); return; }
                    Dummy_Vector3.MakeStatic(vector);
                    DialogResult = true;
                    return;
                }
                else
                {
                    Dummy_Vector3.MakeAnimated();
                    foreach (var track in Tracks)
                    {
                        track.ToNormalizedColor();
                        Dummy_Vector3.Add(new CAnimatorNode<CVector3>(track.Time, new CVector3(track.X, track.Y, track.Z)));
                    }
                    DialogResult = true;
                }
            }
            else if (Type == TransformationType.Visibility)
            {
                if (Dummy_float == null) { MessageBox.Show("Null transformation container", "Error. Report to the developer."); return; }

                if (RadioStatic.IsChecked == true)
                {
                    if (Dummy_float == null) return;
                    float visible = GetStaticVisibility();
                    Dummy_float.MakeStatic(visible);
                    DialogResult = true;
                }
                else
                {
                    if (Dummy_float == null) return;
                    Dummy_float.MakeAnimated();
                    Dummy_float.Clear();
                    foreach (var track in Tracks)
                    {
                        Dummy_float.Add(new CAnimatorNode<float>(track.Time, track.X));
                    }
                    DialogResult = true;
                }
            }
            if (DialogResult != true)
            {
                MessageBox.Show("All checks failed. Unknown condition.");
            }
        }
        private float GetStaticVisibility()
        {
            bool parsed = float.TryParse(StaticInput.Text, out float value);
            if (parsed)
            {
                if (value >= 1) return 1;
            }
            return 0;
        }
        private CVector3 getstaticColorInput()
        {
            var color = Calculator.BrushToColor(StaticInputColor.Background);
            return Calculator.ColorToWar3Color(color);

        }
        private CVector4? GetStaticV4()
        {
            string input = StaticInput.Text.Trim();
            if (input.Length == 0) { return null; }
            string[] parts = input.Split(',').Select(u => u.Trim()).ToArray(); ;
            if (parts.Length != 3) { return null; }
            bool parse1 = float.TryParse(parts[0], out float x);
            bool parse2 = float.TryParse(parts[1], out float y);
            bool parse3 = float.TryParse(parts[2], out float z);
            if (!parse1 || !parse2 || !parse3) { return null; }
            float[] quaternion = Calculator.EulerToQuaternion(x, y, z);
            return new CVector4(quaternion[0], quaternion[1], quaternion[2], quaternion[3]);
        }
        private void clearall(object? sender, RoutedEventArgs? e)
        {
            Tracks.Clear();
            RefreshTracks();
        }
        private void reverseinstructions(object? sender, RoutedEventArgs? e)
        {
            ReverseData(Tracks);
            RefreshTracks();
        }
        private void leaveonlystarts(object? sender, RoutedEventArgs? e)
        {
            foreach (var track in Tracks.ToList())
            {
                if (TrackIsStart(track.Time) == false)
                {
                    Tracks.Remove(track);
                }
            }
            RefreshTracks();
        }
        private bool TrackIsStart(int time)
        {
            foreach (CSequence sequence in Model.Sequences)
            {
                if (time == sequence.IntervalStart) { return true; }
            }
            return false;
        }
        private bool TrackInEnd(int time)
        {
            foreach (CSequence sequence in Model.Sequences)
            {
                if (time == sequence.IntervalEnd) { return true; }
            }
            return false;
        }
        public static void ReverseData(List<Ttrack> tracks)
        {
            int count = tracks.Count;
            // Reverse the tracks list, but leave the time property intact
            for (int i = 0; i < count / 2; i++)
            {
                // Use GetData to copy the data between tracks without changing the Time
                Ttrack temp = new Ttrack { Time = tracks[i].Time };
                temp.GetData(tracks[count - i - 1]);
                tracks[count - i - 1].GetData(tracks[i]);
                tracks[i].GetData(temp);
            }
        }
        private void removeallof(object? sender, RoutedEventArgs? e)
        {
            if (SequenceSelector.SelectedIndex != -1 && SequenceSelector.Items.Count > 0)
            {
                int index = SequenceSelector.SelectedIndex;
                foreach (var track in Tracks.ToList())
                {
                    if (track.Time >= Model.Sequences[index].IntervalStart && track.Time <= Model.Sequences[index].IntervalEnd)
                    {
                        Tracks.Remove(track);
                    }
                }
                RefreshTracks();
            }
        }
        private void removeallofexcept(object? sender, RoutedEventArgs? e)
        {
            int index = SequenceSelector.SelectedIndex;
            foreach (var track in Tracks.ToList())
            {
                if ((track.Time >= Model.Sequences[index].IntervalStart && track.Time <= Model.Sequences[index].IntervalEnd) == false)
                {
                    Tracks.Remove(track);
                }
            }
            RefreshTracks();
        }
        private void reversetimes(object? sender, RoutedEventArgs? e)
        {
            ReverseTimes(Tracks);
            RefreshTracks();
        }
        public static void ReverseTimes(List<Ttrack> tracks)
        {
            if (tracks == null || tracks.Count == 0)
                return;
            int n = tracks.Count;
            for (int i = 0; i < n / 2; i++)
            {
                // Swap Time values
                int tempTime = tracks[i].Time;
                tracks[i].Time = tracks[n - 1 - i].Time;
                tracks[n - 1 - i].Time = tempTime;
            }
        }
        private void leaveonlystartsends(object? sender, RoutedEventArgs? e)
        {
            foreach (var track in Tracks.ToList())
            {
                if (TrackIsStart(track.Time) == false && TrackInEnd(track.Time) == false)
                {
                    Tracks.Remove(track);
                }
            }
            RefreshTracks();
        }
        private void SetDynamix(object? sender, RoutedEventArgs? e)
        {
            if (!InitializedW) return;
            StaticInput.IsEnabled = false;
            StaticInputColor.IsEnabled = false;
            MainList.IsEnabled = true;
            stack1.IsEnabled = true;
            stack2.IsEnabled = true;
            stack0.IsEnabled = true;
            Check_StaticVisible.IsEnabled = false; ;
            switch (Type)
            {
                case TransformationType.Scaling:
                case TransformationType.Translation:
                case TransformationType.Vector4:
                case TransformationType.Color:
                    if (Dummy_Vector3 == null) { break; }
                    Dummy_Vector3.MakeAnimated();
                    break;
                case TransformationType.Rotation:
                    if (Dummy_Vector4 == null) { break; }
                    Dummy_Vector4.MakeAnimated();
                    break;
                case TransformationType.Alpha:
                case TransformationType.Float:
                    if (Dummy_float == null) { break; }
                    Dummy_float.MakeAnimated();
                    break;
                case TransformationType.Int:
                    if (Dummy_int == null) { break; }
                    Dummy_int.MakeAnimated();
                    break;
            }
        }
        private void createstartsends(object? sender, RoutedEventArgs? e)
        {
            Tracks.Clear();
            foreach (CSequence sequence in Model.Sequences)
            {
                if (Type == TransformationType.Translation)
                {
                    Tracks.Add(new Ttrack(sequence.IntervalStart, 0, 0, 0));
                    Tracks.Add(new Ttrack(sequence.IntervalEnd, 0, 0, 0));
                }
                else if (Type == TransformationType.Scaling)
                {
                    Tracks.Add(new Ttrack(sequence.IntervalStart, 100, 100, 100));
                    Tracks.Add(new Ttrack(sequence.IntervalEnd, 100, 100, 100));
                }
                else if (Type == TransformationType.Vector4)
                {
                    Tracks.Add(new Ttrack(sequence.IntervalStart, 0, 0, 0));
                    Tracks.Add(new Ttrack(sequence.IntervalEnd, 0, 0, 0));
                }
                else if (Type == TransformationType.Alpha)
                {
                    Tracks.Add(new Ttrack(sequence.IntervalStart, 100));
                    Tracks.Add(new Ttrack(sequence.IntervalEnd, 100));
                }
                else if (Type == TransformationType.Float)
                {
                    Tracks.Add(new Ttrack(sequence.IntervalStart, 0));
                    Tracks.Add(new Ttrack(sequence.IntervalEnd, 0));
                }
                else if (Type == TransformationType.Int)
                {
                    Tracks.Add(new Ttrack(sequence.IntervalStart, 0));
                    Tracks.Add(new Ttrack(sequence.IntervalEnd, 0));
                }
                else if (Type == TransformationType.Visibility)
                {
                    Tracks.Add(new Ttrack(sequence.IntervalStart, 1));
                    Tracks.Add(new Ttrack(sequence.IntervalEnd, 1));
                }
                RefreshTracks();
            }
        }
        private void createstarts(object? sender, RoutedEventArgs? e)
        {
            Tracks.Clear();
            foreach (CSequence sequence in Model.Sequences)
            {
                if (Type == TransformationType.Translation)
                {
                    Tracks.Add(new Ttrack(sequence.IntervalStart, 0, 0, 0));
                }
                else if (Type == TransformationType.Scaling)
                {
                    Tracks.Add(new Ttrack(sequence.IntervalStart, 100, 100, 100));
                }
                else if (Type == TransformationType.Vector4)
                {
                    Tracks.Add(new Ttrack(sequence.IntervalStart, 0, 0, 0));
                }
                else if (Type == TransformationType.Alpha)
                {
                    Tracks.Add(new Ttrack(sequence.IntervalStart, 100));
                }
                else if (Type == TransformationType.Float)
                {
                    Tracks.Add(new Ttrack(sequence.IntervalStart, 0));
                }
                else if (Type == TransformationType.Int)
                {
                    Tracks.Add(new Ttrack(sequence.IntervalStart, 0));
                }
                else if (Type == TransformationType.Visibility)
                {
                    Tracks.Add(new Ttrack(sequence.IntervalStart, 1));
                }
                RefreshTracks();
            }
        }
        private void SelectedGS(object? sender, SelectionChangedEventArgs e)
        {
            if (Combo_GlobalSequence.SelectedItem != null)
            {
                int index = Combo_GlobalSequence.SelectedIndex;
                int indexIn = index - 1;
                if (Type == TransformationType.Alpha || Type == TransformationType.Float || Type == TransformationType.Visibility)
                {
                    if (Dummy_float == null) return;
                    if (Combo_GlobalSequence.SelectedIndex == 0)
                    {
                       
                        Dummy_float.GlobalSequence.Detach();
                    }
                    else
                    {
                       
                        Dummy_float.GlobalSequence.Attach(Model.GlobalSequences[indexIn]);
                    }
                }
                else if (Type == TransformationType.Int)
                {
                    if (Dummy_int == null) return;
                    if (Combo_GlobalSequence.SelectedIndex == 0)
                    {
                        Dummy_int.GlobalSequence.Detach();
                    }
                    else
                    {
                        Dummy_int.GlobalSequence.Attach(Model.GlobalSequences[indexIn]);
                    }
                }
                else if (Type == TransformationType.Rotation)
                {
                    if (Dummy_Vector4 == null) return;
                    if (Combo_GlobalSequence.SelectedIndex == 0)
                    {
                        Dummy_Vector4.GlobalSequence.Detach();
                    }
                    else
                    {
                        Dummy_Vector4.GlobalSequence.Attach(Model.GlobalSequences[indexIn]);
                    }
                }
                else if (Type == TransformationType.Color || Type == TransformationType.Translation || Type == TransformationType.Scaling)
                {
                    if (Combo_GlobalSequence.SelectedIndex == 0)
                    {
                        if (Dummy_Vector3 == null) return;
                        Dummy_Vector3.GlobalSequence.Detach();
                    }
                    else
                    {
                        if (Dummy_Vector3 == null) return;
                        Dummy_Vector3.GlobalSequence.Attach(Model.GlobalSequences[indexIn]);
                    }
                }
            }
        }
        private void Flip(object? sender, MouseButtonEventArgs e)
        {
            if (MainList.SelectedItem != null && Type == TransformationType.Visibility)
            {
                int index = MainList.SelectedIndex;
                bool visible = Tracks[index].X >= 1;
                Tracks[index].X = visible ? 0 : 1;
                RefreshTracks();
            }
        }
        private void loop(object? sender, RoutedEventArgs? e)
        {
            loopdialog ld = new loopdialog(Model, Tracks, Type);
            ld.ShowDialog();
            if (ld.DialogResult == true) RefreshTracks();
            /*
             in sequence
            times
            value 1
            value 2
             */
        }
        private void showmore(object? sender, RoutedEventArgs? e)
        {
            ButtonMore.ContextMenu.IsOpen = true;
        }
        private void SetInterpolation(object? sender, SelectionChangedEventArgs e)
        {
            if (InitializedW)
            {
                if (
                    Type == TransformationType.Color ||
                    Type == TransformationType.Scaling ||
                    Type == TransformationType.Translation
                    )
                {
                    if (Dummy_Vector3 == null) return;
                    Dummy_Vector3.Type = (EInterpolationType)Combo_InterpType.SelectedIndex;
                }
                else if (Type == TransformationType.Int)
                {
                    if (Dummy_int == null) return;
                    Dummy_int.Type = (EInterpolationType)Combo_InterpType.SelectedIndex;
                }
                else if (Type == TransformationType.Rotation)
                {
                    if (Dummy_Vector4 == null) return;
                    Dummy_Vector4.Type = (EInterpolationType)Combo_InterpType.SelectedIndex;
                }
                else
                {
                    if (Dummy_float == null) return;
                    Dummy_float.Type = (EInterpolationType)Combo_InterpType.SelectedIndex;
                }
            }
        }
        private void negatetrack(object? sender, RoutedEventArgs? e)
        {
            if (MainList.SelectedItem != null)
            {
                int index = MainList.SelectedIndex;
                Tracks[index].Negate(Type);
                RefreshTracks();
            }
        }
        private void negatetrackall(object? sender, RoutedEventArgs? e)
        {
            if (MainList.SelectedItem != null)
            {
                int index = MainList.SelectedIndex;
                Tracks[index].Negate(Type);
                RefreshTracks();
            }
        }
        private void setalltoinput(object? sender, RoutedEventArgs? e)
        {
            string time = Model.Sequences[0].IntervalStart.ToString();
            string input = ValueInput.Text.Trim();
            if (Type == TransformationType.Visibility)
            {
                int visibility = GetDynamicVisibility();
                foreach (Ttrack target in Tracks)
                {
                    target.X = visibility;
                }
            }
            else
            {
                if (InputTrackCorrect(time, input))
                {
                    Ttrack track = new Ttrack();
                    float[] values = ExtractValues(input);
                    foreach (Ttrack target in Tracks)
                    {
                        target.X = values[0];
                        if (values.Length == 3)
                        {
                            target.Y = values[1];
                            target.Z = values[2];
                        }
                    }
                    RefreshTracks();
                }
                else
                {
                    MessageBox.Show("Incorrect input or format"); return;
                }
            }
        }
        private int GetDynamicVisibility()
        {
            return Check_InputVisible.IsChecked == true ? 1 : 0;
        }
        private void addtoall(object? sender, RoutedEventArgs? e)
        {
            string time = Model.Sequences[0].IntervalStart.ToString();
            string input = ValueInput.Text.Trim();
            if (InputTrackCorrect(time, input))
            {
                Ttrack track = new Ttrack();
                float[] values = ExtractValues(input);
                foreach (Ttrack target in Tracks)
                {
                    target.Add(values, Type);
                }
                RefreshTracks();
            }
            else
            {
                MessageBox.Show("Incorrect input or format"); return;
            }
        }
        private void subtractfromall(object? sender, RoutedEventArgs? e)
        {
            string time = Model.Sequences[0].IntervalStart.ToString();
            string input = ValueInput.Text.Trim();
            if (InputTrackCorrect(time, input))
            {
                Ttrack track = new Ttrack();
                float[] values = ExtractValues(input);
                foreach (Ttrack target in Tracks)
                {
                    target.Subtract(values, Type);
                }
                RefreshTracks();
            }
            else
            {
                MessageBox.Show("Incorrect input or format"); return;
            }
        }
        private void multiplyall(object? sender, RoutedEventArgs? e)
        {
            string time = Model.Sequences[0].IntervalStart.ToString();
            string input = ValueInput.Text.Trim();
            if (InputTrackCorrect(time, input))
            {
                Ttrack track = new Ttrack();
                float[] values = ExtractValues(input);
                foreach (Ttrack target in Tracks)
                {
                    target.Divide(values, Type);
                }
                RefreshTracks();
            }
            else
            {
                MessageBox.Show("Incorrect input or format"); return;
            }
        }
        private void divideall(object? sender, RoutedEventArgs? e)
        {
            string time = Model.Sequences[0].IntervalStart.ToString();
            string input = ValueInput.Text.Trim();
            if (InputTrackCorrect(time, input))
            {
                Ttrack track = new Ttrack();
                float[] values = ExtractValues(input);
                foreach (float value in values)
                {
                    if (value == 0)
                    {
                        MessageBox.Show("Cannot divide by zero"); return;
                    }
                }
                foreach (Ttrack target in Tracks)
                {
                    target.Divide(values, Type);
                }
                RefreshTracks();
            }
            else
            {
                MessageBox.Show("Incorrect input or format"); return;
            }
        }
        private void setColorStatic(object? sender, RoutedEventArgs? e)
        {
                StaticInputColor.Background = ColorPickerHandler.Pick(StaticInputColor.Background);
         }
        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Delete) del(null, null);
            if (e.Key == Key.Enter) newItem(null, null);
        }
        private void setSpecial_(object? sender, RoutedEventArgs? e)
        {
            editvisibilities_window ew = new editvisibilities_window(Model, Tracks);
            ew.ShowDialog();
            RefreshTracks();
        }
        private void Checked_StaticVisibility(object? sender, RoutedEventArgs? e)
        {
            bool visible = Check_StaticVisible.IsChecked == true;
            if (Dummy_float == null) return;
            Dummy_float.MakeStatic(visible ? 1 : 0);
        }
        private Ttrack? copiedTrack;
        private void Copy(object? sender, RoutedEventArgs? e)
        {
            if (MainList.SelectedItem != null)
            {
                copiedTrack = Tracks[MainList.SelectedIndex];
            }
        }
        private bool ValidTime()
        {
            if (int.TryParse(TrackInput.Text, out int value))
            {
                if (Model.Sequences.Any(x => value >= x.IntervalStart && value <= x.IntervalEnd))
                {
                    if (Tracks.Any(x => x.Time == value))
                    {
                        MessageBox.Show("There is already a track with this time");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    MessageBox.Show("Invalid input for time. Must be a positive integer that exists in the sequences.");
                    return false;
                }
            }
            MessageBox.Show("Invalid input for time. Must be a positive integer that exists in the sequences.");
            return false;
        }
        private void Paste(object? sender, RoutedEventArgs? e)
        {
            if (copiedTrack != null)
            {
                if (ValidTime())
                {
                    int time = int.Parse(TrackInput.Text);
                    Ttrack ttrack = new Ttrack(copiedTrack);
                    Tracks.Add(ttrack);
                    RefreshTracks();
                }
            }
        }

        private void CopyToClipboard(object? sender, RoutedEventArgs? e)
        {
            if (MainList.SelectedItem != null)
            {
                int index = MainList.SelectedIndex;
                Clipboard.SetText(Tracks[index].ToStringData(Type));

            }
        }

        private void CopyAllToClipboard(object? sender, RoutedEventArgs? e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var track in Tracks)
            {
                sb.AppendLine(track.ToStringData(Type));
            }
            Clipboard.SetText(sb.ToString());
        }

        private void sak(object? sender, RoutedEventArgs? e)
        {
            if (Type == TransformationType.Translation

               )
            {
                InputVector i = new InputVector(AllowedValue.Both);

                if (i.ShowDialog() == true)
                {
                    float x = i.X;
                    float y = i.Y;
                    float z = i.Z;
                    foreach (var trac in Tracks)
                    {
                        trac.X = x;
                        trac.Y = y;
                        trac.Z = z;
                    }
                }
            }

            else if (
              Type == TransformationType.Scaling)
            {
                InputVector i = new InputVector(AllowedValue.Positive);

                if (i.ShowDialog() == true)
                {
                    float x = i.X;
                    float y = i.Y;
                    float z = i.Z;
                    foreach (var trac in Tracks)
                    {
                        trac.X = x;
                        trac.Y = y;
                        trac.Z = z;
                    }
                }
            }

            else if (
              Type == TransformationType.Rotation)
            {
                InputVector i = new InputVector(AllowedValue.Positive);

                if (i.ShowDialog() == true)
                {
                    float x = i.X;
                    float y = i.Y;
                    float z = i.Z;
                    if (x < -360 || x > 360 || y < -360 || y > 360 || z < -360 || z > 360)
                    {
                        MessageBox.Show("Invalid input for rotation"); return;
                    }
                    foreach (var trac in Tracks)
                    {
                        trac.X = x;
                        trac.Y = y;
                        trac.Z = z;
                    }
                }
            }
            else if (Type == TransformationType.Color)
            {
                InputVector i = new InputVector(AllowedValue.Positive);

                if (i.ShowDialog() == true)
                {
                    float x = i.X;
                    float y = i.Y;
                    float z = i.Z;
                    if (x < 0 || x > 255 || y < 0 || y > 255 || z < 0 || z > 255)
                    {
                        MessageBox.Show("Invalid input for color"); return;
                    }
                    foreach (var trac in Tracks)
                    {
                        trac.X = x;
                        trac.Y = y;
                        trac.Z = z;
                    }
                }
            }
            else if (Type == TransformationType.Visibility)
            {
                Input i = new  ("0");
                if (i.ShowDialog() == true)
                {
                    if (i.Result.Trim().ToLower() == "false")
                    {
                        foreach (var trac in Tracks)
                        {
                            trac.X = 0;

                        }
                    }
                    if (i.Result.Trim().ToLower() == "true")
                    {
                        foreach (var trac in Tracks)
                        {
                            trac.X = 1;

                        }
                    }
                    bool parsed = int.TryParse(i.Result.Trim(), out int v);
                    if (parsed)
                    {
                        if (v == 0 || v == 1)
                        {
                            foreach (var trac in Tracks)
                            {
                                trac.X = v;

                            }
                        }
                        else
                        {
                            MessageBox.Show("Expected 1 or 0");
                        }

                    }
                }
            }
            else if (Type == TransformationType.Alpha)
            {
                Input i = new Input("100");
                if (i.ShowDialog() == true)
                {

                    bool parsed = int.TryParse(i.Result.Trim(), out int v);
                    if (parsed)
                    {
                        if (v >= 0 && v <= 100)
                        {
                            foreach (var trac in Tracks)
                            {
                                trac.X = v;

                            }
                        }
                        else
                        {
                            MessageBox.Show("Expected 0 to 100");
                        }

                    }
                }
            }

            else
            {
                Input i = new Input("0");
                if (i.ShowDialog() == true)
                {
                    bool parsed = int.TryParse(i.Result, out int v);
                    if (parsed)
                    {
                        foreach (var trac in Tracks)
                        {
                            trac.X = v;

                        }
                    }
                }
            }
            RefreshTracks();
        }

        private void sak2(object? sender, RoutedEventArgs? e)
        {
            if (Model.Sequences.Count == 0)
            {
                MessageBox.Show("There are no sequences", "Nothing to work with"); return;
            }
            List<string> ss = Model.Sequences.Select(x => x.Name).ToList();
            Selector s = new Selector(ss);
            if (s.ShowDialog() == true)
            {
                CSequence sq = Model.Sequences[s.box.SelectedIndex];
                int from = sq.IntervalStart;
                int to = sq.IntervalEnd;

                //------------------------------------
                if (Type == TransformationType.Translation)
                {
                    InputVector i = new InputVector(AllowedValue.Both);

                    if (i.ShowDialog() == true)
                    {
                        float x = i.X;
                        float y = i.Y;
                        float z = i.Z;
                        foreach (var trac in Tracks)
                        {
                            if (trac.Time < from || trac.Time > to) { continue; }
                            trac.X = x;
                            trac.Y = y;
                            trac.Z = z;
                        }
                    }
                }

                else if (Type == TransformationType.Scaling)
                {
                    InputVector i = new InputVector(AllowedValue.Positive);

                    if (i.ShowDialog() == true)
                    {
                        float x = i.X;
                        float y = i.Y;
                        float z = i.Z;
                        foreach (var trac in Tracks)
                        {
                            if (trac.Time < from || trac.Time > to) { continue; }
                            trac.X = x;
                            trac.Y = y;
                            trac.Z = z;
                        }
                    }
                }

                else if (Type == TransformationType.Rotation)
                {
                    InputVector i = new InputVector(AllowedValue.Positive);

                    if (i.ShowDialog() == true)
                    {
                        float x = i.X;
                        float y = i.Y;
                        float z = i.Z;
                        if (x < -360 || x > 360 || y < -360 || y > 360 || z < -360 || z > 360)
                        {
                            MessageBox.Show("Invalid input for rotation"); return;
                        }
                        foreach (var trac in Tracks)
                        {
                            if (trac.Time < from || trac.Time > to) { continue; }
                            trac.X = x;
                            trac.Y = y;
                            trac.Z = z;
                        }
                    }
                }
                else if (Type == TransformationType.Color)
                {
                    InputVector i = new  (AllowedValue.Positive);

                    if (i.ShowDialog() == true)
                    {
                        float x = i.X;
                        float y = i.Y;
                        float z = i.Z;
                        if (x < 0 || x > 255 || y < 0 || y > 255 || z < 0 || z > 255)
                        {
                            MessageBox.Show("Invalid input for rotation"); return;
                        }
                        foreach (var trac in Tracks)
                        {
                            if (trac.Time < from || trac.Time > to) { continue; }
                            trac.X = x;
                            trac.Y = y;
                            trac.Z = z;
                        }
                    }
                }

                else if (Type == TransformationType.Visibility)
                {
                    Input i = new Input("0");
                    if (i.ShowDialog() == true)
                    {
                        if (i.Result.Trim().ToLower() == "false")
                        {
                            foreach (var trac in Tracks)
                            {
                                if (trac.Time < from || trac.Time > to) { continue; }
                                trac.X = 0;

                            }
                        }
                        if (i.Result.Trim().ToLower() == "true")
                        {
                            foreach (var trac in Tracks)
                            {
                                if (trac.Time < from || trac.Time > to) { continue; }
                                trac.X = 1;

                            }
                        }
                        bool parsed = int.TryParse(i.Result.Trim(), out int v);
                        if (parsed)
                        {
                            if (v == 0 || v == 1)
                            {
                                foreach (var trac in Tracks)
                                {
                                    if (trac.Time < from || trac.Time > to) { continue; }
                                    trac.X = v;

                                }
                            }
                            else
                            {
                                MessageBox.Show("Expected 1 or 0");
                            }

                        }
                    }
                }
                else if (Type == TransformationType.Alpha)
                {
                    Input i = new Input("100");
                    if (i.ShowDialog() == true)
                    {

                        bool parsed = int.TryParse(i.Result.Trim(), out int v);
                        if (parsed)
                        {
                            if (v >= 0 && v <= 100)
                            {
                                foreach (var trac in Tracks)
                                {
                                    if (trac.Time < from || trac.Time > to) { continue; }
                                    trac.X = v;

                                }
                            }
                            else
                            {
                                MessageBox.Show("Expected 0 to 100");
                            }

                        }
                    }
                }
                else
                {
                    Input i = new Input("0");
                    if (i.ShowDialog() == true)
                    {
                        bool parsed = int.TryParse(i.Result, out int v);
                        if (parsed)
                        {
                            foreach (var trac in Tracks)
                            {
                                if (trac.Time < from || trac.Time > to) { continue; }
                                trac.X = v;

                            }
                        }
                    }
                }
            }
            RefreshTracks();
        }

        private void starte(object? sender, RoutedEventArgs? e)
        {
            foreach (var sequence in Model.Sequences)
            {
                if (Type == TransformationType.Visibility)
                {
                    if (Tracks.Any(x => x.Time == sequence.IntervalStart))
                    {
                        var track = Tracks.First(x => x.Time == sequence.IntervalStart);
                        track.X = 1;
                    }
                    else
                    {
                        Tracks.Add(new Ttrack(sequence.IntervalStart, 1));
                    }
                }
                if (Type == TransformationType.Translation)
                {
                    if (Tracks.Any(x => x.Time == sequence.IntervalStart))
                    {
                        var track = Tracks.First(x => x.Time == sequence.IntervalStart);
                        track.X = 0;
                        track.Y = 0;
                        track.Z = 0;
                    }
                    else
                    {
                        Tracks.Add(new Ttrack(sequence.IntervalStart, 0, 0, 0));
                    }
                }
                if (Type == TransformationType.Rotation)
                {
                    if (Tracks.Any(x => x.Time == sequence.IntervalStart))
                    {
                        var track = Tracks.First(x => x.Time == sequence.IntervalStart);
                        track.X = 0;
                        track.Y = 0;
                        track.Z = 0;
                    }
                    else
                    {
                        Tracks.Add(new Ttrack(sequence.IntervalStart, 0, 0, 0, 1));
                    }
                }
                if (Type == TransformationType.Scaling)
                {
                    if (Tracks.Any(x => x.Time == sequence.IntervalStart))
                    {
                        var track = Tracks.First(x => x.Time == sequence.IntervalStart);
                        track.X = 100;
                        track.Y = 100;
                        track.Z = 100;
                    }
                    else
                    {
                        Tracks.Add(new Ttrack(sequence.IntervalStart, 100, 100, 100));
                    }
                }
                if (Type == TransformationType.Int)
                {
                    if (Tracks.Any(x => x.Time == sequence.IntervalStart))
                    {
                        var track = Tracks.First(x => x.Time == sequence.IntervalStart);
                        track.X = 0;
                    }
                    else
                    {
                        Tracks.Add(new Ttrack(sequence.IntervalStart, 0));
                    }
                }
                if (Type == TransformationType.Float)
                {
                    if (Tracks.Any(x => x.Time == sequence.IntervalStart))
                    {
                        var track = Tracks.First(x => x.Time == sequence.IntervalStart);
                        track.X = 0;
                    }
                    else
                    {
                        Tracks.Add(new Ttrack(sequence.IntervalStart, 0));
                    }
                }
                if (Type == TransformationType.Color)
                {
                    if (Tracks.Any(x => x.Time == sequence.IntervalStart))
                    {
                        var track = Tracks.First(x => x.Time == sequence.IntervalStart);
                        track.X = 255;
                        track.Y = 255;
                        track.Z = 255;
                    }
                    else
                    {
                        Tracks.Add(new Ttrack(sequence.IntervalStart, 255, 255, 255));
                    }
                }
                if (Type == TransformationType.Alpha)
                {
                    if (Tracks.Any(x => x.Time == sequence.IntervalStart))
                    {
                        var track = Tracks.First(x => x.Time == sequence.IntervalStart);
                        track.X = 100;
                    }
                    else
                    {
                        Tracks.Add(new Ttrack(sequence.IntervalStart, 100));
                    }
                }

            }
            Tracks = Tracks.OrderBy(x => x.Time).ToList();
            RefreshTracks();
        }

        private void ende(object? sender, RoutedEventArgs? e)
        {
            foreach (var sequence in Model.Sequences)
            {
                if (Type == TransformationType.Visibility)
                {
                    if (Tracks.Any(x => x.Time == sequence.IntervalEnd))
                    {
                        var track = Tracks.First(x => x.Time == sequence.IntervalEnd);
                        track.X = 1;
                    }
                    else
                    {
                        Tracks.Add(new Ttrack(sequence.IntervalEnd, 1));
                    }
                }
                if (Type == TransformationType.Translation)
                {
                    if (Tracks.Any(x => x.Time == sequence.IntervalEnd))
                    {
                        var track = Tracks.First(x => x.Time == sequence.IntervalEnd);
                        track.X = 0;
                        track.Y = 0;
                        track.Z = 0;
                    }
                    else
                    {
                        Tracks.Add(new Ttrack(sequence.IntervalEnd, 0, 0, 0));
                    }
                }
                if (Type == TransformationType.Rotation)
                {
                    if (Tracks.Any(x => x.Time == sequence.IntervalEnd))
                    {
                        var track = Tracks.First(x => x.Time == sequence.IntervalEnd);
                        track.X = 0;
                        track.Y = 0;
                        track.Z = 0;
                    }
                    else
                    {
                        Tracks.Add(new Ttrack(sequence.IntervalEnd, 0, 0, 0, 1));
                    }
                }
                if (Type == TransformationType.Scaling)
                {
                    if (Tracks.Any(x => x.Time == sequence.IntervalEnd))
                    {
                        var track = Tracks.First(x => x.Time == sequence.IntervalEnd);
                        track.X = 100;
                        track.Y = 100;
                        track.Z = 100;
                    }
                    else
                    {
                        Tracks.Add(new Ttrack(sequence.IntervalEnd, 100, 100, 100));
                    }
                }
                if (Type == TransformationType.Int)
                {
                    if (Tracks.Any(x => x.Time == sequence.IntervalEnd))
                    {
                        var track = Tracks.First(x => x.Time == sequence.IntervalEnd);
                        track.X = 0;
                    }
                    else
                    {
                        Tracks.Add(new Ttrack(sequence.IntervalEnd, 0));
                    }
                }
                if (Type == TransformationType.Float)
                {
                    if (Tracks.Any(x => x.Time == sequence.IntervalEnd))
                    {
                        var track = Tracks.First(x => x.Time == sequence.IntervalEnd);
                        track.X = 0;
                    }
                    else
                    {
                        Tracks.Add(new Ttrack(sequence.IntervalEnd, 0));
                    }
                }
                if (Type == TransformationType.Color)
                {
                    if (Tracks.Any(x => x.Time == sequence.IntervalEnd))
                    {
                        var track = Tracks.First(x => x.Time == sequence.IntervalEnd);
                        track.X = 255;
                        track.Y = 255;
                        track.Z = 255;
                    }
                    else
                    {
                        Tracks.Add(new Ttrack(sequence.IntervalEnd, 255, 255, 255));
                    }
                }
                if (Type == TransformationType.Alpha)
                {
                    if (Tracks.Any(x => x.Time == sequence.IntervalEnd))
                    {
                        var track = Tracks.First(x => x.Time == sequence.IntervalEnd);
                        track.X = 100;
                    }
                    else
                    {
                        Tracks.Add(new Ttrack(sequence.IntervalEnd, 100));
                    }
                }

            }
            Tracks = Tracks.OrderBy(x => x.Time).ToList();
            RefreshTracks();
        }

        private void stretch2(object? sender, RoutedEventArgs? e)
        {
            // Stretch the times of all keyframes/tracks to fit inside the selected sequence
            if (Model.Sequences.Count == 0)
            {
                MessageBox.Show("There are no sequences", "Nothing to work with");
                return;
            }
            foreach (CSequence sq in Model.Sequences)
            {

                int from = sq.IntervalStart;
                int to = sq.IntervalEnd;

                List<Ttrack> isolated = Tracks.Where(x => x.Time >= from && x.Time <= to).ToList();

                if (isolated.Count <= 1) { return; } // No keyframes or only one keyframe cannot be stretched
                if (isolated.Any(x => x.Time == from) && isolated.Any(x => x.Time == to)) { return; } // Already fits the interval

                // Find min and max keyframe times in the isolated tracks
                int minTime = isolated.Min(x => x.Time);
                int maxTime = isolated.Max(x => x.Time);

                if (minTime == maxTime) { return; } // Avoid division by zero if all keyframes have the same time

                // Stretch keyframes to fit within the new interval
                foreach (var track in isolated)
                {
                    track.Time = from + (track.Time - minTime) * (to - from) / (maxTime - minTime);
                }
            }
        }

        private void stretch1(object? sender, RoutedEventArgs? e)
        {
            // Stretch the times of all keyframes/tracks to fit inside the selected sequence
            if (Model.Sequences.Count == 0)
            {
                MessageBox.Show("There are no sequences", "Nothing to work with");
                return;
            }

            List<string> ss = Model.Sequences.Select(x => x.Name).ToList();
            Selector s = new Selector(ss);
            if (s.ShowDialog() == true)
            {
                CSequence sq = Model.Sequences[s.box.SelectedIndex];
                int from = sq.IntervalStart;
                int to = sq.IntervalEnd;

                List<Ttrack> isolated = Tracks.Where(x => x.Time >= from && x.Time <= to).ToList();

                if (isolated.Count <= 1) { return; } // No keyframes or only one keyframe cannot be stretched
                if (isolated.Any(x => x.Time == from) && isolated.Any(x => x.Time == to)) { return; } // Already fits the interval

                // Find min and max keyframe times in the isolated tracks
                int minTime = isolated.Min(x => x.Time);
                int maxTime = isolated.Max(x => x.Time);

                if (minTime == maxTime) { return; } // Avoid division by zero if all keyframes have the same time

                // Stretch keyframes to fit within the new interval
                foreach (var track in isolated)
                {
                    track.Time = from + (track.Time - minTime) * (to - from) / (maxTime - minTime);
                }
            }
        }

        
        private void copymovekfs(object? sender, RoutedEventArgs? e)
        {
            if (Model.Sequences.Count == 0) return;
            transformation_selector ts = new transformation_selector();
            ts.ShowDialog();
            if (ts.DialogResult == true)
            {
                if (ts.C1.IsChecked == true) { CopiedKeyframesData.CopiedNodeKeyframeType = Wa3Tuner.TransformationType.Translation; }
                else if (ts.C2.IsChecked == true) { CopiedKeyframesData.CopiedNodeKeyframeType = Wa3Tuner.TransformationType.Rotation; }
                else if (ts.C3.IsChecked == true) { CopiedKeyframesData.CopiedNodeKeyframeType = Wa3Tuner.TransformationType.Scaling; }
                CopiedKeyframesData.Sequence = Model.Sequences.Count == 1 ? Model.Sequences[0] : SelectSequence();
                CopiedKeyframesData.Cut = false;
            }

        }

        private CSequence SelectSequence()
        {
            var list = Model.Sequences.Select(x => x.Name).ToList();
            Selector s = new Selector(list);
            if (s.ShowDialog() == true)
            {
                return Model.Sequences[s.box.SelectedIndex];
            }
            return Model.Sequences[0];
        }

        private void cutmovekfs(object? sender, RoutedEventArgs? e)
        {
            if (Model.Sequences.Count == 0) return;
            transformation_selector ts = new transformation_selector();
            ts.ShowDialog();
            if (ts.DialogResult == true)
            {
                if (ts.C1.IsChecked == true) { CopiedKeyframesData.CopiedNodeKeyframeType = Wa3Tuner.TransformationType.Translation; }
                else if (ts.C2.IsChecked == true) { CopiedKeyframesData.CopiedNodeKeyframeType = Wa3Tuner.TransformationType.Rotation; }
                else if (ts.C3.IsChecked == true) { CopiedKeyframesData.CopiedNodeKeyframeType = Wa3Tuner.TransformationType.Scaling; }
                CopiedKeyframesData.Sequence = Model.Sequences.Count == 1 ? Model.Sequences[0] : SelectSequence();
                CopiedKeyframesData.Cut = true;
            }
        }

        private void pasteseelctkfs(object? sender, RoutedEventArgs? e)
        {
            if (Model.Sequences.Count == 0) { MessageBox.Show("There are no sequences", "Nothing to work with"); return; }
            if (Model.Sequences.Count == 1) { MessageBox.Show("There is only one sequence"); return; }
            if (CopiedKeyframesData.Sequence == null) return;
            var seq2 = SelectSequence();
            if (CopiedKeyframesData.Sequence == seq2) { MessageBox.Show("Copied and pasted sequences cannot be the same"); return; }
            var seq1 = CopiedKeyframesData.Sequence;
            List<Ttrack> isolated = Tracks.Where(X => X.Time >= seq1.IntervalStart && X.Time <= seq1.IntervalEnd).ToList();
            if (isolated.Count == 0)
            {
                MessageBox.Show("There are no keyframes belonging to the first sequence"); return;
            }
            CopyList(isolated, Tracks, seq2);
            Tracks = Tracks.OrderBy(x => x.Time).ToList();
            if (CopiedKeyframesData.Cut)
            {
                foreach (var track in isolated) { Tracks.Remove(track); }

            }
            RefreshTracks();
        }

        private static void CopyList(List<Ttrack> isolatedTracks, List<Ttrack> FullTrackList, CSequence ToWhichSequence)
        {
            int offset = ToWhichSequence.IntervalStart; // Use 'from', not 'to'

            foreach (var track in isolatedTracks)
            {
                var TrackCopy = new Ttrack(track); // Create a copy of the track
                TrackCopy.Time += offset; // Modify the copy, not the original
                FullTrackList.Add(TrackCopy);
            }

        }

        private void RemoveSequences(object? sender, RoutedEventArgs? e)
        {
            if (Model.Sequences.Count == 0) { MessageBox.Show("There are no sequences", "Nothing to work with"); }

            List<string> names = Model.Sequences.Select(x => x.Name).ToList();
            Multiselector_Window s = new Multiselector_Window(names);
            if (s.ShowDialog() == true)
            {
                foreach (int index in s.selectedIndexes)
                {
                    int from = Model.Sequences[index].IntervalStart;
                    int to = Model.Sequences[index].IntervalEnd;
                    Tracks.RemoveAll(x => x.Time >= from && x.Time <= to);
                }
            }
            RefreshTracks();
        }

        private void dups2ss(object? sender, RoutedEventArgs? e)
        {
            if (Model.Sequences.Count <= 1)
            {
                MessageBox.Show("At least 2 sequences must be present"); return;
            }
            SequenceToSequencesSelector d = new(Model.Sequences.ObjectList, Tracks);
            if (d.ShowDialog() == true) { RefreshTracks(); }
        }

        private void gkm(object? sender, RoutedEventArgs? e)
        {
            if (Model.Sequences.Count == 0) { return; }
            if (Type == TransformationType.Visibility)
            {
                MessageBox.Show("This command cannot work for visibility transformation"); return;
            }
            Gradual_Keyframe_Maker gk = new Gradual_Keyframe_Maker(Model.Sequences.ObjectList, Tracks, Type);
            if (gk.ShowDialog() == true)
            {
                RefreshTracks();
            }
        }

        private void editTime(object? sender, RoutedEventArgs? e)
        {
            if (MainList.SelectedItem != null)
            {
                string time = TrackInput.Text.Trim();
                bool p = int.TryParse(time, out int time_);
                if (Tracks.Any(x => x.Time == time_))
                {
                    MessageBox.Show("A keyframe with this time already exists");
                }
                else
                {
                    if (sequenceContainsTime(time_))
                    {
                        Tracks[MainList.SelectedIndex].Time = time_;
                        RefreshTracks();
                    }
                    else
                    {
                        MessageBox.Show("This track time is not in any sequence"); return;
                    }
                }


            }
        }

        private bool sequenceContainsTime(int time_)
        {
            return Model.Sequences.Any(x => time_ >= x.IntervalStart && time_ <= x.IntervalEnd);
        }




        private void movekfTimeStart(object? sender, RoutedEventArgs? e)
        {
            if (MainList.SelectedItem == null || Tracks.Count == 0) return;

            int index = MainList.SelectedIndex;
            if (index < 0 || index >= Tracks.Count) return; // Prevent out-of-range access

            var seq = GetSequenceOfTime(Tracks[index].Time);
            if (seq == null) return;

            int jump = seq.IntervalStart;
            if (Tracks.Any(x => x.Time == jump))
            {
                MessageBox.Show($"There is already a keyframe that uses the start time of that sequence at {jump}. Track {index} not changed");
                return;
            }

            Tracks[index].Time = jump;
            RefreshTracks();
        }

        CSequence? GetSequenceOfTime(int time)
        {
            return Model.Sequences.FirstOrDefault(x => time >= x.IntervalStart && time <= x.IntervalEnd);
        }


        private void movekfTimeEnd(object? sender, RoutedEventArgs? e)
        {
            if (MainList.SelectedItem == null || Tracks.Count == 0) return;

            int index = MainList.SelectedIndex;
            if (index < 0 || index >= Tracks.Count) return; // Prevent out-of-range access

            var seq = GetSequenceOfTime(Tracks[index].Time);
            if (seq == null) return;

            int jump = seq.IntervalEnd;
            if (Tracks.Any(x => x.Time == jump))
            {
                MessageBox.Show($"There is already a keyframe that uses the end time of that sequence at {jump}. Track {index} not changed");
                return;
            }

            Tracks[index].Time = jump;
            RefreshTracks();
        }

        private void SwapSequences(object? sender, RoutedEventArgs? e)
        {
            Swap_Sequences_Selector ss = new Swap_Sequences_Selector(Model.Sequences.ObjectList);
            if (ss.ShowDialog() == true)
            {
                CSequence from = Model.Sequences[ss.s1];
                CSequence to = Model.Sequences[ss.s2];
                SwapSequencesInsideTracks(from.IntervalStart, from.IntervalEnd, to.IntervalStart, to.IntervalEnd);
                RefreshTracks();
            }
        }

        private void SwapSequencesInsideTracks(int from1, int to1, int from2, int to2)
        {
            var tracksByTime = Tracks.ToDictionary(t => t.Time);

            var range1 = Tracks.Where(t => t.Time >= from1 && t.Time <= to1).OrderBy(t => t.Time).ToList();
            var range2 = Tracks.Where(t => t.Time >= from2 && t.Time <= to2).OrderBy(t => t.Time).ToList();
            if (range1.Count == 0) { MessageBox.Show("One of the sequences is not contained in the list of keyframes"); return; }
            if (range2.Count == 0) { MessageBox.Show("One of the sequences is not contained in the list of keyframes"); return; }
            int count1 = range1.Count;
            int count2 = range2.Count;

            if (count1 != count2)
            {
                List<Ttrack> shorter, longer;
                int startShort, endShort, startLong, endLong;

                if (count1 < count2)
                {
                    shorter = range1;
                    longer = range2;
                    startShort = from1;
                    endShort = to1;
                    startLong = from2;
                    endLong = to2;
                }
                else
                {
                    shorter = range2;
                    longer = range1;
                    startShort = from2;
                    endShort = to2;
                    startLong = from1;
                    endLong = to1;
                }

                // Can the shorter one be stretched to match the longer one?
                if (shorter.Count < 2)
                {
                    MessageBox.Show("Cannot interpolate with less than 2 keyframes.");
                    return;
                }

                // Interpolate new times for shorter sequence
                float interval = (endLong - startLong) / (float)(shorter.Count - 1);
                for (int i = 0; i < shorter.Count; i++)
                {
                    shorter[i].Time = (int)(startLong + i * interval);
                }

                // Re-fetch ranges after time adjustment
                range1 = Tracks.Where(t => t.Time >= from1 && t.Time <= to1).OrderBy(t => t.Time).ToList();
                range2 = Tracks.Where(t => t.Time >= from2 && t.Time <= to2).OrderBy(t => t.Time).ToList();

                // If counts are still different due to duplicates or sparse data
                if (range1.Count != range2.Count)
                {
                    MessageBox.Show("Unable to resample sequences to match keyframe counts.");
                    return;
                }
            }

            // Now both ranges are equal in count, perform the data swap
            for (int i = 0; i < range1.Count; i++)
            {
                var t1 = range1[i];
                var t2 = range2[i];

                (t1.X, t2.X) = (t2.X, t1.X);
                (t1.Y, t2.Y) = (t2.Y, t1.Y);
                (t1.Z, t2.Z) = (t2.Z, t1.Z);
                (t1.W, t2.W) = (t2.W, t1.W);
            }
        }

        private void RemoveAllRepeatingKeyframes(object? sender, RoutedEventArgs? e)
        {
            if (Model.Sequences.Count == 0)
            {
                MessageBox.Show("There are no sequences", "Nothing to work with");
                return;
            }

          foreach (var sequence in Model.Sequences)
            {
                RemoveRepeatingKeyframes(sequence);
            }
        }



      



        private void RemoveRepTimes_First(object? sender, RoutedEventArgs? e)
        {
            foreach (var sequence in Model.Sequences)
            {
                RemoveRepeatingTimes_First(sequence);
            }
        }


        private void RemoveRepTimes_Last(object? sender, RoutedEventArgs? e)
        {
         foreach (var sequence in Model.Sequences) { RemoveRepeatingTimes_Last(sequence); }   
        }



        private void RemoveRepTimes_First_inSequence(object? sender, RoutedEventArgs? e)
        {
            if (Tracks.Count == 0)
            {
                MessageBox.Show("There are no keyframes");
                return;
            }

            if (Model.Sequences.Count == 0)
            {
                MessageBox.Show("There are no sequences", "Nothing to work with");
                return;
            }

            Selector s = new Selector(Model.Sequences.Select(x => x.Name).ToList());
            if (s.ShowDialog() == true)
            {
                var sequence = Model.Sequences[s.box.SelectedIndex];
               
            }
        }
        private void RemoveRepeatingTimes_First(CSequence sequence)
        {
            var isolated = Tracks
                   .Where(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd)
                   .OrderBy(x => x.Time)
                   .ToList();

            if (isolated.Count <= 1)
            {
               
                return;
            }

            for (int i = 0; i < isolated.Count;)
            {
                if (i - 1 == -1) { continue; }
                if (isolated[i].Time == isolated[i - 1].Time)
                {
                    Tracks.Remove(isolated[i - 1]); // Keep the first
                    isolated.RemoveAt(i - 1);       // Keep isolated in sync
                }
                else
                {
                    i++;
                }
            }
        }
        private void RemoveSandwichKeyframes(CSequence sequence)
        {
            // Filter tracks inside the sequence range
            var isolated = Tracks
                .Where(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd)
                .OrderBy(x => x.Time)
                .ToList();

            if (isolated.Count < 3) return; // Need at least 3 to form a sandwich

            // We'll collect the ones to remove (can't modify list while iterating)
            var toRemove = new List<Ttrack>(); // Replace 'object' with your keyframe class type if it's known

            for (int i = 1; i < isolated.Count - 1; i++)
            {
                var prev = isolated[i - 1];
                var curr = isolated[i];
                var next = isolated[i + 1];

                if (curr.Time == prev.Time && curr.Time == next.Time &&
                    curr.SameData(prev) && curr.SameData(next))
                {
                    toRemove.Add(curr);
                }
            }

            foreach (var keyframe in toRemove)
                Tracks.Remove(keyframe);
        }

        private void RemoveRepTimes_Last_inSequence(object? sender, RoutedEventArgs? e)
        {
             
            if (Model.Sequences.Count == 0)
            {
                MessageBox.Show("There are no sequences", "Nothing to work with");
                return;
            }

            Selector s = new Selector(Model.Sequences.Select(x => x.Name).ToList());
            if (s.ShowDialog() == true)
            {
                var sequence = Model.Sequences[s.box.SelectedIndex];
                RemoveRepeatingTimes_Last(sequence);
            }
        }
        private void RemoveRepeatingTimes_Last(CSequence sequence)
        {
            var isolated = Tracks
                   .Where(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd)
                   .OrderBy(x => x.Time)
                   .ToList();

            if (isolated.Count <= 1)
            {
               
                return;
            }

            for (int i = 0; i < isolated.Count - 1;)
            {
                if (isolated[i].Time == isolated[i + 1].Time)
                {
                    Tracks.Remove(isolated[i + 1]); // Keep the first
                    isolated.RemoveAt(i + 1);       // Keep isolated in sync
                }
                else
                {
                    i++;
                }
            }
        }

        private void RemoveRepeatingKeyframesInSequence(object? sender, RoutedEventArgs? e)
        {
            Selector s = new Selector(Model.Sequences.Select(x => x.Name).ToList());
            if (s.ShowDialog() == true)
            {
                var sequence = Model.Sequences[s.box.SelectedIndex];
                RemoveRepeatingKeyframes(sequence);


                RefreshTracks();
            }
        }
        private void RemoveRepeatingKeyframes(CSequence sequence)
        {
            // 1. Extract the relevant keyframes (those within the selected sequence)
            var isolated = Tracks
                .Where(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd)
                .OrderBy(x => x.Time)
                .ToList();

            // 2. Deduplicate keyframes with the same data
            for (int i = 0; i < isolated.Count - 1;)
            {
                if (isolated[i].SameData(isolated[i + 1]))
                {
                    isolated.RemoveAt(i + 1);
                }
                else
                {
                    i++;
                }
            }

            // 3. Replace the original keyframes with the deduplicated list
            Tracks.RemoveAll(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd);
            Tracks.AddRange(isolated);

            // Optional: sort Tracks by time again if necessary
            Tracks = Tracks.OrderBy(x => x.Time).ToList();
        }
        private void RemoveSandwitchedSameKeyframes(object? sender, RoutedEventArgs? e)
        {
           
             foreach (var sequence in Model.Sequences)
            {
                RemoveSandwichKeyframes(sequence);
            }
            RefreshTracks();
            

           
        }
        private void RemoveSandwitchedSameKeyframes_Sequence(object? sender, RoutedEventArgs? e)
        {
            Selector s = new Selector(Model.Sequences.Select(x => x.Name).ToList());
            if (s.ShowDialog() == true)
            {
                var sequence = Model.Sequences[s.box.SelectedIndex];
                RemoveSandwichKeyframes(sequence);
            }
            RefreshTracks();
        }

        private void Refr(object? sender, RoutedEventArgs? e)
        {
            RefreshTracks();
        }

        private void delall(object sender, RoutedEventArgs e)
        {
            Tracks.Clear();
            RefreshTracks();
        }
    }
}  
    public class Ttrack
    {
        public int Time = 0;
        public float X, Y, Z, W = 0;
       
        public void GetData(Ttrack track)
        {
            X = track.X; Y = track.Y;
            Z = track.Z; W = track.W;
        }
        public Ttrack() { }
        public Ttrack(int time, float x, float y, float z)
        {
            Time = time;
            X = x;
            Y = y;
            Z = z;
        }
        public Ttrack(int time, float[] values)
        {
            Time = time;
            X = values[0];
            Y = values[1];
            Z = values[2];
        }
        public Ttrack(int time, float x, float y, float z, float w)
        {
            Time = time;
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
        public Ttrack(int time, float x)
        {
            Time = time;
            X = x;
        }
        public Ttrack(Ttrack copiedTrack)
        {
            Time = copiedTrack.Time;
            X = copiedTrack.X;
            Y = copiedTrack.Y;
            Z = copiedTrack.Z;
        }
        public string ToStringData(TransformationType type)
        {

            return $"{Time}: {GetValue(type)}";
        }
        internal string GetValue(TransformationType type)
        {
            if (type == TransformationType.Translation) { return $"{X}, {Y}, {Z}"; }
            else if (type == TransformationType.Rotation) { return $"{X}, {Y}, {Z}"; }
            else if (type == TransformationType.Color) { return $"{X}, {Y}, {Z}"; }
            else if (type == TransformationType.Scaling) { return $"{X}, {Y}, {Z}"; }
            else if (type == TransformationType.Visibility)
            {
                if (X >= 1) { return "Visible"; }
                return "Invisible";
            }
            else { return X.ToString(); }
        }
        internal void ToQuaternion()
        {
            float[] quaternion = Calculator.EulerToQuaternion(X, Y, Z);
            X = quaternion[0]; Y = quaternion[1]; Z = quaternion[2]; W = quaternion[3];
        }
        internal void ToEuler()
        {
            float[] euler = Calculator.QuaternionToEuler(X, Y, Z, W);
            X = euler[0];
            Y = euler[1];
            Z = euler[2];
        }
        internal void ToPercentage()
        {
            X *= 100; Y *= 100; Z *= 100;
        }
        internal void ToNormalizedPercentage()
        {
            X /= 100; Y /= 100; Z /= 100;
        }
        internal void ToColor()
        {
            X *= 255;
            Y *= 255;
            Z *= 255;
            float temp = X;
            X = Z;
            Z = temp;
        }
        internal void ToNormalizedColor()
        {
            X /= 255;
            Y /= 255;
            Z /= 255;
            float temp = X;
            X = Z;
            Z = temp;
        }
        internal void Update(float[] values)
        {
            X = values[0];
            if (values.Length == 3)
            {
                Y = values[1]; Z = values[2];
            }
        }
        internal void Negate(TransformationType type)
        {
            if (type == TransformationType.Visibility)
            {
                if (X >= 1) { X = 0; } else { X = 1; }
            }
            if (type == TransformationType.Translation || type == TransformationType.Rotation)
            {
                X = -X;
                Y = -Y;
                Z = -Z;
            }
            if (type == TransformationType.Alpha)
            {
                if (X == 0) { X = 100; }
                else if (X == 100) { X = 0; }
                else if (X > 0 & X < 100)
                {
                    X = 100 - X;
                }
            }
            if (type == TransformationType.Color)
            {
                X = 255 - X;
                Y = 255 - Y;
                Z = 255 - Z;
            }
        }
        internal void Add(float[] values, TransformationType type)
        {
            if (type == TransformationType.Alpha)
            {
                if (values[0] > 100) { X = 100; }
                else
                {
                    X += values[0];
                    if (X > 100) X = 100;
                }
            }
            if (type == TransformationType.Scaling || type == TransformationType.Translation)
            {
                X += values[0];
                Y += values[1];
                Z += values[2];
            }
            if (type == TransformationType.Color)
            {
                X += values[0];
                Y += values[1];
                Z += values[2];
                if (X > 255) X = 255;
                if (Y > 255) Y = 255;
                if (Z > 255) Z = 255;
            }
            if (type == TransformationType.Rotation)
            {
                X += values[0];
                Y += values[1];
                Z += values[2];
                if (X > 360) X = 360;
                if (Y > 360) Y = 360;
                if (Z > 360) Z = 360;
            }
            if (type == TransformationType.Int || type == TransformationType.Float)
            {
                X += values[0];
            }
        }
        internal void Subtract(float[] values, TransformationType type)
        {
            if (type == TransformationType.Alpha)
            {
                X -= values[0];
                if (X < 0) X = 0;
            }
            if (type == TransformationType.Translation)
            {
                X -= values[0];
                Y -= values[1];
                Z -= values[2];
            }
            if (type == TransformationType.Scaling)
            {
                X -= values[0];
                Y -= values[1];
                Z -= values[2];
                if (X < 0) X = 0;
                if (Y < 0) Y = 0;
                if (Z < 0) Z = 0;
            }
            if (type == TransformationType.Color)
            {
                X -= values[0];
                Y -= values[1];
                Z -= values[2];
                if (X < 0) X = 0;
                if (Y < 0) Y = 0;
                if (Z < 0) Z = 0;
            }
            if (type == TransformationType.Rotation)
            {
                X -= values[0];
                Y -= values[1];
                Z -= values[2];
                if (X < -360) X = -360;
                if (Y < -360) Y = -360;
                if (Z < -360) Z = -360;
            }
            if (type == TransformationType.Int || type == TransformationType.Float)
            {
                X -= values[0];
                if (X < 0) { X = 0; }
            }
        }
        internal void Divide(float[] values, TransformationType type)
        {
            if (type == TransformationType.Alpha)
            {
                X /= values[0];
                if (X < 0) X = 0;
            }
            if (type == TransformationType.Translation)
            {
                X /= values[0];
                Y /= values[1];
                Z /= values[2];
            }
            if (type == TransformationType.Scaling)
            {
                X /= values[0];
                Y /= values[1];
                Z /= values[2];
                if (X < 0) X = 0;
                if (Y < 0) Y = 0;
                if (Z < 0) Z = 0;
            }
            if (type == TransformationType.Color)
            {
                X /= values[0];
                Y /= values[1];
                Z /= values[2];
                if (X < 0) X = 0;
                if (Y < 0) Y = 0;
                if (Z < 0) Z = 0;
            }
            if (type == TransformationType.Rotation)
            {
                X /= values[0];
                Y /= values[1];
                Z /= values[2];
                if (X < -360) X = -360;
                if (Y < -360) Y = -360;
                if (Z < -360) Z = -360;
            }
            if (type == TransformationType.Int || type == TransformationType.Float)
            {
                X /= values[0];
                if (X < 0) { X = 0; }
            }
        }

    internal bool SameData(Ttrack t)
    {
        return X == t.X && Y == t.Y && Z == t.Z && W == t.W;
    }
}
 