using MdxLib.Animator;
using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        Alpha
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

        internal string GetValue(TransformationType type)
        {
            if (type == TransformationType.Translation) { return $"{X}, {Y}, {Z}"; }
            else if (type == TransformationType.Rotation) { return $"{X}, {Y}, {Z}"; }
            else if (type == TransformationType.Visibility)
            {
                return X >= 1 ? "Visible" : "Invisible";
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
                if (values[0] > 100) { X = 100; } else
                {
                    X += values[0];
                    if (X > 100) X = 100;
                }
            }
            if (type == TransformationType.Scaling || type == TransformationType.Translation)
            {
                X+= values[0];
                Y+= values[1];
                Z+= values[2];

            }
            if ( type == TransformationType.Color)  {
                X += values[0];
                Y += values[1];
                Z += values[2];
                if (X > 255) X = 255;
                if (Y > 255) Y = 255;
                if ( Z > 255) Z = 255;

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
                    if (X  <0) X = 0;
                
            }
            if (  type == TransformationType.Translation)
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
    }
        public partial class transformation_editor : Window
        {
            CModel Model;
            public bool Initialized = false;
            private TransformationType Type;
            public CAnimator<int> Dummy_int;
            public CAnimator<float> Dummy_float;
            public CAnimator<CVector3> Dummy_Vector3;
            public CAnimator<CVector4> Dummy_Vector4;
            private List<Ttrack> Tracks = new List<Ttrack>();
            public transformation_editor(CModel model, CAnimator<float> animator, bool canBeStatic, TransformationType type)
            {
                InitializeComponent();
                Initialized = true;
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
                if (type == TransformationType.Alpha) ConvertAlphaTracks();
                RefreshTracks();
                FillSequences();
                Title = $"Transformation Editor - {type}";
                FillGlobalSEquence(animator.GlobalSequence.Object);
            }
            private void FillStaticValue()
            {
                if (Type == TransformationType.Alpha) { StaticInput.Text = (Dummy_float.GetValue() * 100).ToString(); }
                if (Type == TransformationType.Color) { StaticInput.Text = Calculator.BGRnToRGB(Dummy_Vector3.GetValue()); }
                if (Type == TransformationType.Translation) { StaticInput.Text = GetStatic(Dummy_Vector3.GetValue()); }
                if (Type == TransformationType.Scaling) { { StaticInput.Text = GetStaticP(Dummy_Vector3.GetValue()); } }
                if (Type == TransformationType.Rotation) { StaticInput.Text = Calculator.QuaternionToEuler_(Dummy_Vector4.GetValue()); }
                if (Type == TransformationType.Visibility) { StaticInput.Text = Dummy_float.GetValue().ToString("F0"); }
                if (Type == TransformationType.Int) { StaticInput.Text = Dummy_float.GetValue().ToString(); }
                if (Type == TransformationType.Float) { StaticInput.Text = Dummy_float.GetValue().ToString(); }
            }

            private string GetStaticP(CVector3 cVector3)
            {
                return $"{cVector3.X * 100}, {cVector3.Y * 100}, {cVector3.Z * 100}";
            }

            private string GetStatic(CVector3 cVector3)
            {
                return $"{cVector3.X}, {cVector3.Y}, {cVector3.Z}";
            }

            private void FillGlobalSEquence(CGlobalSequence gs_)
            {
                Combo_GlobalSequence.Items.Add(new ComboBoxItem() { Content = "None" });

                foreach (CGlobalSequence gs in Model.GlobalSequences)
                {
                    Combo_GlobalSequence.Items.Add(new ComboBoxItem() { Content = gs.ObjectId.ToString() });
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
                InitializeComponent();
                Initialized = true;
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
            public transformation_editor(CModel model, CAnimator<CVector3> animator, bool canBeStatic, TransformationType type)
            {
                InitializeComponent();
                Initialized = true;
                Type = type;
                Dummy_Vector3 = animator;
                Model = model;
                Combo_InterpType.SelectedIndex = (int)animator.Type;
                if (canBeStatic == false) { RadioStatic.IsEnabled = false; RadioDynamic.IsChecked = true; }
                if (animator.Static) { RadioStatic.IsChecked = true; } else { RadioDynamic.IsChecked = true; }

                ButtonColor.IsEnabled = type == TransformationType.Color;
                foreach (var item in animator)
                {
                    Tracks.Add(new Ttrack(item.Time, item.Value.X, item.Value.Y, item.Value.Z));
                }
                if (type == TransformationType.Color) ConvertColorTracks();
                if (type == TransformationType.Scaling) ConvertScalingTracks();
                RefreshTracks();
                FillSequences(); Title = $"Transformation Editor - {type}";
                FillGlobalSEquence(animator.GlobalSequence.Object);

            } //GetStaticQuaternion(animator.GetValue());

            private string GetStaticValue(CVector3 val, TransformationType type)
            {
                if (type == TransformationType.Scaling)
                {
                    return $"{val.X * 100}, {val.Y * 100}, {val.Z * 100}";
                }
                if (type == TransformationType.Color)
                {
                    return Calculator.BGRnToRGB(val);
                }
                return $"{val.X}, {val.Y}, {val.Z}";
            }


            public transformation_editor(CModel model, CAnimator<CVector4> animator, bool canBeStatic)
            {
                InitializeComponent();
                Initialized = true;
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
                foreach (Ttrack track in Tracks)
                {
                    ListBoxItem item = new ListBoxItem();
                    StackPanel panel = new StackPanel();
                    TextBlock sequence = new TextBlock();
                    sequence.Foreground = Brushes.White;
                    sequence.Background = Brushes.Gray;
                    sequence.Width = 200;
                    sequence.Text = GetSequence(track.Time);
                    TextBlock value = new TextBlock();
                    value.Margin = new Thickness(5, 0, 5, 0);
                    value.Width = 400;
                    value.Text = track.GetValue(Type);
                    TextBlock time = new TextBlock();
                    time.Width = 50;
                    time.Text = track.Time.ToString();
                    time.Margin = new Thickness(5, 0, 5, 0);


                    panel.Orientation = Orientation.Horizontal;
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
                    if (time >= sequence.IntervalStart && time <= sequence.IntervalEnd) { return sequence.Name; }
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

            private void SetStatic(object sender, RoutedEventArgs e)
            {
                if (!Initialized) return;
                StaticInput.IsEnabled = true;
                MainList.IsEnabled = false;
                stack1.IsEnabled = false;
                stack2.IsEnabled = false;
                stack0.IsEnabled = false;
                FillStaticValue();
            }

            private void explain(object sender, RoutedEventArgs e)
            {
                MessageBox.Show("For rotations this app uses euler(x,y,z) instead of quaternion (x,y,z,w)\nFor scalings (scaling and alpha) this app uses percentage instead of normalized percentage.\n For colors this app uses standard RGB instead of normalized reversed rgb.\nFor visibility this app uses 'visible' and 'invisible' (enter 1 or 0 in the box) instead of 1 and 0.\nInt and float transformations remain the same. ");
            }

            private void SeletedSequence(object sender, SelectionChangedEventArgs e)
            {
                int index = SequenceSelector.SelectedIndex;
                TrackInput.Text = Model.Sequences[index].IntervalStart.ToString();
            }

            private void del(object sender, RoutedEventArgs e)
            {
                if (MainList.SelectedItem != null)
                {
                    int index = MainList.SelectedIndex;
                    Tracks.RemoveAt(index);
                    MainList.Items.Remove(MainList.SelectedItem);
                }
            }
            private int GetSelectedIndex()
            {
                return 0;
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
                if (Type == TransformationType.Visibility)
                {
                    bool parsed1 = int.TryParse(parts[0], out int x);
                    if (parsed1)
                    {
                        if (x == 0 | x == 1) { return true; }
                    }
                }
                if (Type == TransformationType.Float)
                {
                    bool parsed1 = float.TryParse(parts[0], out float x);
                    if (parsed1) { return true; }
                }
                if (Type == TransformationType.Translation)
                {
                    bool parsed1 = float.TryParse(parts[0], out float x);
                    bool parsed2 = float.TryParse(parts[1], out float y);
                    bool parsed3 = float.TryParse(parts[2], out float z);
                    if (parsed1 && parsed2 && parsed3)
                    {
                        if (x >= 0 && y >= 0 && z >= 0)
                        {
                            return true;
                        }
                    }
                }
                if (Type == TransformationType.Rotation)
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
                if (Type == TransformationType.Color)
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
                if (Type == TransformationType.Alpha)
                {
                    bool parsed1 = float.TryParse(parts[0], out float x);
                    return x >= 0 && x <= 100;
                }
                return false;
            }
            private void newItem(object sender, RoutedEventArgs e)
            {
                string time = TrackInput.Text.Trim();
                string input = ValueInput.Text.Trim();
                if (InputTrackCorrect(time, input))
                {
                    Ttrack track = new Ttrack();
                    int time_ = int.Parse(time);
                    float[] values = ExtractValues(input);
                    if (Type == TransformationType.Rotation) track = new Ttrack(time_, values[0], values[1], values[2], values[3]);
                    if (Type == TransformationType.Scaling) track = new Ttrack(time_, values[0], values[1], values[2]);
                    if (Type == TransformationType.Color) track = new Ttrack(time_, values[0], values[1], values[2]);
                    if (Type == TransformationType.Translation) track = new Ttrack(time_, values[0], values[1], values[2]);
                    if (Type == TransformationType.Visibility) track = new Ttrack(time_, values[0]);
                    if (Type == TransformationType.Alpha) track = new Ttrack(time_, values[0]);

                    Tracks.Add(track);
                    RefreshTracks();
                }
                else
                {
                    MessageBox.Show("Incorrect input or format"); return;
                }
            }
            private float[] ExtractValues(string item)
            {
                List<float> vals = new List<float>();
                string[] i = item.Split(',').Select(x => x.Trim()).ToArray();
                foreach (string s in i)
                {
                    vals.Add(float.Parse(s));
                }

                return vals.ToArray();
            }
            private void edit(object sender, RoutedEventArgs e)
            {
                if (MainList.SelectedItem != null)
                {
                    string time = TrackInput.Text.Trim();
                    string input = ValueInput.Text.Trim();

                    if (InputTrackCorrect(time, input))
                    {
                        Ttrack track = Tracks[MainList.SelectedIndex];
                        int time_ = int.Parse(time);
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

            private void SelectedTrack(object sender, SelectionChangedEventArgs e)
            {

                int index = MainList.SelectedIndex;
                if (index == -1) { return; }

                TrackInput.Text = Tracks[index].Time.ToString();
                ValueInput.Text = Tracks[index].GetValue(Type);
            }

            private void setColor(object sender, RoutedEventArgs e)
            {

                color_selector selector = new color_selector();
                selector.ShowDialog();
                if (selector.DialogResult == true)
                {
                    ButtonColor.Background = selector.SelectedBrush;
                    ValueInput.Text = $"{selector.SelectedColor.R}, {selector.SelectedColor.G}, {selector.SelectedColor.B}";
                }
            }
            private CVector3 GetStaticV3(bool scaling = false)
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

            private void ok(object sender, RoutedEventArgs e)
            {
                Tracks = Tracks.OrderBy(X => X.Time).ToList();
                if (Type == TransformationType.Translation)
                {
                    Dummy_Vector3.Clear();

                    if (RadioStatic.IsChecked == true)
                    {
                        CVector3 vector = GetStaticV3();
                        if (vector == null) { MessageBox.Show("Invalid static value input"); return; }
                        Dummy_Vector3.MakeStatic(vector);
                        DialogResult = true;
                        return;
                    }
                    foreach (var track in Tracks)
                    {
                        Dummy_Vector3.Add(new CAnimatorNode<CVector3>(track.Time, new CVector3(track.X, track.Y, track.Z)));
                    }
                }
                if (Type == TransformationType.Alpha)
                {
                    if (RadioStatic.IsChecked == true)
                    {
                        bool parsed = float.TryParse(StaticInput.Text, out float value);
                        if (parsed)
                        {
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
                    foreach (var track in Tracks)
                    {
                        track.ToNormalizedPercentage();
                        Dummy_float.Add(new CAnimatorNode<float>(track.Time, track.X));
                    }
                }
                if (Type == TransformationType.Scaling)
                {
                    if (RadioStatic.IsChecked == true)
                    {
                        CVector3 vector = GetStaticV3(true);
                        if (vector == null) { MessageBox.Show("Invalid static value input"); return; }
                        Dummy_Vector3.MakeStatic(vector);
                        DialogResult = true;
                        return;
                    }
                    foreach (var track in Tracks)
                    {
                        track.ToNormalizedPercentage();
                        Dummy_Vector3.Add(new CAnimatorNode<CVector3>(track.Time, new CVector3(track.X, track.Y, track.Z)));
                    }
                }
                if (Type == TransformationType.Rotation)
                {
                    if (RadioStatic.IsChecked == true)
                    {
                        CVector4 vector = GetStaticV4();
                        if (vector == null) { MessageBox.Show("Invalid static value input"); return; }
                        Dummy_Vector4.MakeStatic(vector);
                        DialogResult = true;
                        return;
                    }
                    foreach (var track in Tracks)
                    {
                        track.ToQuaternion();
                        Dummy_Vector4.Add(new CAnimatorNode<CVector4>(track.Time, new CVector4(track.X, track.Y, track.Z, track.W)));
                    }
                }
                if (Type == TransformationType.Int)
                {
                    if (RadioStatic.IsChecked == true)
                    {
                        bool parsed = int.TryParse(StaticInput.Text, out int value);
                        if (parsed)
                        {
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
                    foreach (var track in Tracks)
                    {

                        Dummy_int.Add(new CAnimatorNode<int>(track.Time, (int)track.X));
                    }
                }
                if (Type == TransformationType.Float)
                {
                    if (RadioStatic.IsChecked == true)
                    {
                        bool parsed = float.TryParse(StaticInput.Text, out float value);
                        if (parsed)
                        {
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
                    foreach (var track in Tracks)
                    {

                        Dummy_float.Add(new CAnimatorNode<float>(track.Time, track.X));
                    }
                }
                DialogResult = true;
            }

            private CVector4 GetStaticV4()
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

            private void clearall(object sender, RoutedEventArgs e)
            {
                Tracks.Clear();
                RefreshTracks();
            }

            private void reverseinstructions(object sender, RoutedEventArgs e)
            {
                ReverseData(Tracks);
                RefreshTracks();
            }

            private void leaveonlystarts(object sender, RoutedEventArgs e)
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

            private void removeallof(object sender, RoutedEventArgs e)
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

            private void removeallofexcept(object sender, RoutedEventArgs e)
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

            private void reversetimes(object sender, RoutedEventArgs e)
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

            private void leaveonlystartsends(object sender, RoutedEventArgs e)
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

            private void SetDynamix(object sender, RoutedEventArgs e)
            {
                if (!Initialized) return;
                StaticInput.IsEnabled = false;
                MainList.IsEnabled = true;
                stack1.IsEnabled = true;
                stack2.IsEnabled = true;
                stack0.IsEnabled = true;

                switch (Type)
                {
                    case TransformationType.Scaling:

                    case TransformationType.Translation:
                    case TransformationType.Vector4:
                    case TransformationType.Color:
                        Dummy_Vector3.MakeAnimated();

                        break;
                    case TransformationType.Rotation:
                        Dummy_Vector4.MakeAnimated();
                        break;
                    case TransformationType.Alpha:
                    case TransformationType.Float:
                        Dummy_float.MakeAnimated();
                        break;
                    case TransformationType.Int:
                        Dummy_int.MakeAnimated();
                        break;
                }
            }

            private void createstartsends(object sender, RoutedEventArgs e)
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

            private void createstarts(object sender, RoutedEventArgs e)
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

            private void SelectedGS(object sender, SelectionChangedEventArgs e)
            {
                if (Combo_GlobalSequence.SelectedItem != null)
                {

                    int index = Combo_GlobalSequence.SelectedIndex;
                    int indexIn = index - 1;
                    if (Type == TransformationType.Alpha || Type == TransformationType.Float || Type == TransformationType.Visibility)
                    {
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
                            Dummy_Vector3.GlobalSequence.Detach();
                        }
                        else
                        {
                            Dummy_Vector3.GlobalSequence.Attach(Model.GlobalSequences[indexIn]);
                        }

                    }
                }
            }

            private void Flip(object sender, MouseButtonEventArgs e)
            {
                if (MainList.SelectedItem != null && Type == TransformationType.Visibility)
                {
                    int index = MainList.SelectedIndex;
                    bool visible = Tracks[index].X >= 1;
                    Tracks[index].X = visible ? 0 : 1;
                    RefreshTracks();
                }
            }

            private void loop(object sender, RoutedEventArgs e)
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

            private void showmore(object sender, RoutedEventArgs e)
            {
                ButtonMore.ContextMenu.IsOpen = true;
            }

            private void SetInterpolation(object sender, SelectionChangedEventArgs e)
            {
                Dummy_float.Type = (EInterpolationType)Combo_InterpType.SelectedIndex;
            }

            private void negatetrack(object sender, RoutedEventArgs e)
            {
                if (MainList.SelectedItem != null)
                {
                    int index = MainList.SelectedIndex;
                    Tracks[index].Negate(Type);
                RefreshTracks();
                }
            }

        private void negatetrackall(object sender, RoutedEventArgs e)
        {
            if (MainList.SelectedItem != null)
            {
                int index = MainList.SelectedIndex;
                Tracks[index].Negate(Type);
                RefreshTracks();
            }
        }

        private void setalltoinput(object sender, RoutedEventArgs e)
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

        private void addtoall(object sender, RoutedEventArgs e)
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

        private void subtractfromall(object sender, RoutedEventArgs e)
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

        private void multiplyall(object sender, RoutedEventArgs e)
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

        private void divideall(object sender, RoutedEventArgs e)
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
                        MessageBox.Show("Cannot divide by zero");return;
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
    }
    }
 