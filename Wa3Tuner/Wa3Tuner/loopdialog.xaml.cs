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
    /// Interaction logic for loopdialog.xaml
    /// </summary>
    public partial class loopdialog : Window
    {
        CModel Model;
        TransformationType Type;
        List<Ttrack> Tracks;

        public loopdialog(CModel model, List<Ttrack> tracks, TransformationType type)
        {
            InitializeComponent();
            Model = model;
            Type = type;
            Tracks = tracks;
            foreach (CSequence seq in model.Sequences)

            {
                string name = $"{seq.Name} [{seq.IntervalStart} - {seq.IntervalEnd}]";
                InputSequence.Items.Add(new ComboBoxItem() { Content = name});
            }
        }
        private void Clean(int sequenceIndex )
        {
            foreach (Ttrack track in Tracks.ToList())
            {
                if (track.Time>= Model.Sequences[sequenceIndex].IntervalStart && track.Time >= Model.Sequences[sequenceIndex].IntervalEnd)
                {
                    Tracks.Remove(track);   
                }
            }
        }
        private void loop(int sequenceIndex, int times, float[] value1, float[] value2)
        {
            // Check if we can loop
            int interval = Model.Sequences[sequenceIndex].IntervalEnd - Model.Sequences[sequenceIndex].IntervalStart;
            if (times * 2 > interval)
            {
                MessageBox.Show("The loops are more than the sequence's interval");
                return;
            }

            // Remove all tracks that use this sequence's interval before we put others
            Clean(sequenceIndex);

            // Prepare
            int startFrom = Model.Sequences[sequenceIndex].IntervalStart;
            int endAt = Model.Sequences[sequenceIndex].IntervalEnd;

            // Add alternating values to the tracks
            int time = startFrom;
            float[] currentValue = new float[value1.Length];
            Array.Copy(value1, currentValue, 3);
            int remainingTime = interval;

            for (int i = 0; i < times; i++)
            {
                // Add the current track with time and value
                Tracks.Add(new Ttrack(time, currentValue));

                // Calculate the time increment for the next track
                int timeIncrement = (remainingTime / (times - i)); // Distribute remaining time

                // Update remaining time and time
                remainingTime -= timeIncrement;
                time += timeIncrement;

                // Alternate between value1 and value2
                currentValue = (currentValue == value1) ? value2 : value1;
            }

            // Optionally, you can add a final track if needed
            // Tracks.Add(new Ttrack(time, currentValue)); 
        }
        private void loop(int sequenceIndex, int times, float value1, float value2)
        {
            // Check if we can loop
            int interval = Model.Sequences[sequenceIndex].IntervalEnd - Model.Sequences[sequenceIndex].IntervalStart;
            if (times * 2 > interval)
            {
                MessageBox.Show("The loops are more than the sequence's interval");
                return;
            }

            // Remove all tracks that use this sequence's interval before we put others
            Clean(sequenceIndex);

            // Prepare
            int startFrom = Model.Sequences[sequenceIndex].IntervalStart;
            int endAt = Model.Sequences[sequenceIndex].IntervalEnd;

            // Add alternating values to the tracks
            int time = startFrom;
            float currentValue = value1;
            int remainingTime = interval;

            for (int i = 0; i < times; i++)
            {
                // Add the current track with time and value
                Tracks.Add(new Ttrack(time, currentValue));

                // Calculate the time increment for the next track
                int timeIncrement = (remainingTime / (times - i)); // Distribute remaining time

                // Update remaining time and time
                remainingTime -= timeIncrement;
                time += timeIncrement;

                // Alternate between value1 and value2
                currentValue = (currentValue == value1) ? value2 : value1;
            }

            // Optionally, you can add a final track if needed
            // Tracks.Add(new Ttrack(time, currentValue)); 
        }
        float[] ParseThreeInts(string input)
        {
            // Split the string by commas
            string[] parts = input.Split(',') ;
            parts = parts.Select(x => x.Trim()).ToArray();
            // Check if there are exactly 3 parts
            if (parts.Length != 3)
            {
                return null; // Invalid format (not exactly 3 integers)
            }

            float[] result = new float[3];

            // Try to parse each part as an integer and check the range
            for (int i = 0; i < 3; i++)
            {
                if (!float.TryParse(parts[i], out result[i]) || result[i] < 0 || result[i] > 255)
                {
                    return null; // Invalid number or out of range
                }
            }

            // Return the array if all values are valid
            return result;
        }
        float[] ExtractValues(string input)
        {
            string[] parts = input.Split(",").Select(x=>x.Trim()).ToArray() ;
            if (parts.Length != 3) { return null; }
            bool parsed1 = float.TryParse(parts[0], out float one);
            bool parsed2 = float.TryParse(parts[0], out float two);
            bool parsed3 = float.TryParse(parts[0], out float three);
            if (parsed1 && parsed2 && parsed3)
            {
                if (one < 0 ||  two < 0 || three < 0) { return null; }
                return new float[]{one, two, three};
            }
            else { return null; }
            return null;
        }
        bool Values360(float[] values)
        {
            return
                values[0] >= -360 && values[0] <= 360 &&
                values[1] >= -360 && values[1] <= 360 &&
                values[2] >= -360 && values[2] <= 360;
        }
        private void OK(object sender, RoutedEventArgs e)
        {
            if (InputSequence.SelectedIndex == -1) { MessageBox.Show("select a sequence");  return; }
            int index = InputSequence.SelectedIndex;
            bool pTimes = int.TryParse(InputTimes.Text, out int times);
            if (!pTimes) { MessageBox.Show("Invalid input for times"); return; }
            string value1 = InputValue1.Text.Trim(); ;
            string value2 = InputValue2.Text.Trim();
            switch (Type)
            {
                case TransformationType.Translation:
                case TransformationType.Scaling:
                    float[] one = ExtractValues(value1);
                    float[] two = ExtractValues(value2);
                    if (one == null || two == null) { MessageBox.Show("expected 3 values sparated by comma"); return; }
                     loop(index, times, one, two);
                    break;
               
                    
                    case TransformationType.Rotation:
                    float[] rone = ExtractValues(value1);
                    float[] rtwo = ExtractValues(value2);
                    if (rone == null || rtwo == null) { MessageBox.Show("expected 3 values sparated by comma between -360 and 360"); return; }
                    if (!Values360(rone) || !Values360(rtwo)) { MessageBox.Show("expected 3 values sparated by comma between -360 and 360"); return; }
                    loop(index, times, rone, rtwo);
                    break;
                case TransformationType.Visibility:
                    bool parsedv1 = int.TryParse(value1, out int v1);
                    bool parsedv2 = int.TryParse(value1, out int v2);
                    if (!parsedv1 || !parsedv2) { MessageBox.Show("Expected 1 or 0");return; }  
                    if (v1 < 0 || v1 > 1 || v2 < 0 || v2 > 1) { MessageBox.Show("Expected 1 or 0"); return; }
                    loop(index, times, v1, v2);
                    break;
                case TransformationType.Color:
                     
                     float[] first = ParseThreeInts(value1);
                    float[] second = ParseThreeInts(value2);
                    if (first == null && second == null) { MessageBox.Show("Expected an rgb string in teh format r,g,b"); return; }
                    loop(index, times, first, second);

                    break;
                case TransformationType.Float:
                case TransformationType.Int:
                    bool pf1 = int.TryParse(value1, out int f1);
                    bool pf2 = int.TryParse(value1, out int f2);
                    if (!pf1 && !pf2) { MessageBox.Show("Invalid input, expected integers between 0 and 100"); return; }
                    if (f1 < 0 || f1 > 100 || f2 < 0 || f2 > 100) { MessageBox.Show("Invalid input, expected integers between 0 and 100"); return; }
                    loop(index, times, f1, f2);
                    break;
                case TransformationType.Alpha:
                    bool pInt1 = int.TryParse(value1, out int int1);
                    bool pInt2 = int.TryParse(value1, out int int2);
                    if (!pInt1 && !pInt2) { MessageBox.Show("Invalid input, expected integers between 0 and 100"); return; }
                   if (int1 <0 || int1 > 100 || int2 <0 || int2 > 100) { MessageBox.Show("Invalid input, expected integers between 0 and 100"); return; }
                    loop(index, times, int1, int2);
                    break;
            }
            DialogResult = true;

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
        }
    }
}
