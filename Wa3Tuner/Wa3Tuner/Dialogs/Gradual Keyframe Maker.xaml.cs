using MdxLib.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
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

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for Gradual_Keyframe_Maker.xaml
    /// </summary>
    public partial class Gradual_Keyframe_Maker : Window
    {
        TransformationType Type;
        List<CSequence> Sequences;
        List<Ttrack> Tracks;
        public Gradual_Keyframe_Maker(List<CSequence> s, List<Ttrack> tracks, TransformationType type)
        {
            InitializeComponent();
            Sequences = s;
            Tracks = tracks;
            Type = type;
            Title = $"Gradual Keyframe Maker - {type}";
            Fill();
        }
        void Fill()
        {
            foreach (var sequence in Sequences)
            {
                list.Items.Add(new ListBoxItem() { Content=$"{sequence.Name} [{sequence.IntervalStart} - {sequence.IntervalEnd}]"});
            }
            list.SelectedIndex = 0;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; }
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            var SelectedSequence = Sequences[list.SelectedIndex];
            int Fullrange = SelectedSequence.IntervalEnd - SelectedSequence.IntervalStart;

            bool p1 = int.TryParse(inputFrom.Text, out int from);
            bool p2 = int.TryParse(inputTo.Text, out int to);
            Vector3? initial = GetInputValue(inputValue.Text);
            bool Increment = checkIncrement.IsChecked == true;

            if (initial == null) { MessageBox.Show("Invalid input value"); return; }
            if (!p1 || !p2) { MessageBox.Show("Invalid range input"); return; }
            if (from == to) { MessageBox.Show("from and to cannot be the same"); return; }
            if (from < SelectedSequence.IntervalStart || to > SelectedSequence.IntervalEnd)
            {
                MessageBox.Show("Range is not in the selected sequence");
                return;
            }

            int SelectedRange = to - from;

            if (c1.IsChecked == true) // Generate keyframes based on interval
            {
                bool kf = int.TryParse(inputKF.Text, out int count);
                if (!kf || count <= 0) { MessageBox.Show("Invalid keyframe count"); return; }
                GenerateKeyframes_Interval(from, to, initial, count, Increment);
            }
            else if (c2.IsChecked == true) // Generate keyframes based on step
            {
                Vector3? step = GetInputValue(inputStep.Text);
                if (step == null) { MessageBox.Show("Invalid step input"); return; }

                GenerateKeyframes_Step(from, to, initial, step, Increment);
            }
            Tracks = Tracks.OrderBy(x => x.Time).ToList();
            DialogResult = true;
        }

        private void GenerateKeyframes_Interval(int from, int to, Vector3? initial, int numberOfKeyframes, bool increment)
        {
            if (initial == null) return;

            // Clear existing keyframes in the range
            Tracks.RemoveAll(x => x.Time >= from && x.Time <= to);

            float step = (float)(to - from) / (numberOfKeyframes - 1);
            Vector3 currentValue = initial.Value;

            for (int i = 0; i < numberOfKeyframes; i++)
            {
                int time = (int)Math.Round(from + i * step);
                Tracks.Add(new Ttrack(time, currentValue.X, currentValue.Y, currentValue.Z));

                if (increment) // If incrementing, modify value each step
                    currentValue += new Vector3(step, step, step);
            }
        }

        private void GenerateKeyframes_Step(int from, int to, Vector3? initial, Vector3? step, bool increment)
        {
            if (initial == null || step == null) return;

            // Clear existing keyframes in the range
            Tracks.RemoveAll(x => x.Time >= from && x.Time <= to);

            Vector3 currentValue = initial.Value;
            for (int time = from; time <= to; time += (int)step.Value.X) // Using X as step interval
            {
                Tracks.Add(new Ttrack(time, currentValue.X, currentValue.Y, currentValue.Z));

                if (increment) // If incrementing, add the step value
                    currentValue += step.Value;
            }
        }


        private Vector3? GetInputValue(string t)
        {
            string text = t;
            string[] parts = text.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);


            if (parts.Length < 1 || parts.Length > 3)
                return null;
            if (
                Type == TransformationType.Translation || Type == TransformationType.Rotation ||
               Type == TransformationType.Color
                )
            {
                if (parts.Length != 3) { return null; }
            }
            float[] values = new float[3]; // Default to (0,0,0)

            for (int i = 0; i < parts.Length; i++)
            {
                if (!float.TryParse(parts[i], NumberStyles.Float, CultureInfo.InvariantCulture, out values[i]))
                    return null;
            }
           
            return new Vector3(values[0], values[1], values[2]);
        }


        private void chek1(object sender, RoutedEventArgs e)
        {
            d1.Visibility = Visibility.Visible;
            d2.Visibility = Visibility.Collapsed;
        }

        private void chek2(object sender, RoutedEventArgs e)
        {
            d1.Visibility = Visibility.Collapsed;
            d2.Visibility = Visibility.Visible;
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var s = Sequences[list.SelectedIndex];
            inputFrom.Text = s.IntervalStart.ToString();
            inputFrom.Text = s.IntervalEnd.ToString();
        }

        private void Window_KeyDown_1(object sender, KeyEventArgs e)
        {

        }
    }
}
