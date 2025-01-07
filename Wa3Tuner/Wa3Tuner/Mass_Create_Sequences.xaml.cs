using MdxLib.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
    /// Interaction logic for Mass_Create_Sequences.xaml
    /// </summary>
    public partial class Mass_Create_Sequences : Window
    {
        CModel Model;
        public Mass_Create_Sequences(CModel model)
        {
            InitializeComponent();
           
            Model = model;
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            List<string> lines = Input.Text.Split('\n').ToList();
            // check
            for (int i = 0; i < lines.Count; i++)
            {
                string[] parts = lines[i].Split('-').ToArray();
                if (parts.Length == 2)
                {
                    string[] parts2 = parts[1].Split(' ');
                    bool one = int.TryParse(parts2[0], out int num);
                    bool two = int.TryParse(parts2[1], out int num2);
                    if (!one && !two)
                    {
                        MessageBox.Show($"Incorrect format at line {i}: Expected 'sequence name - from to'"); return;

                    }
                }
                else
                {
                    MessageBox.Show($"Incorrect format at line {i}: Expected 'sequence name - from to'"); return;
                }
            }
            for (int i = 0; i < lines.Count; i++)
            {
                string[] parts = lines[i].Split('-').ToArray();
                 string name = CapitalizeEachWord(parts[0].Trim());
                string interval = parts[1].Trim();
                string[] values = interval.Split(' ').ToArray();
                int from = int.Parse(values[0].Trim());
                int to = int.Parse(values[1].Trim());
                if (from > 999999 || to > 999999)
                {
                    MessageBox.Show("From or to cannot be greater than 999999"); return;
                }
                if (from >= to)
                {
                    MessageBox.Show("From must be less than to"); return;
                }
                if (Model.Sequences.Any(x => x.Name.ToLower() == name.ToLower()))
                {
                    MessageBox.Show($"'{name}' already exists"); continue;
                }
                if (Model.Sequences.Any(x => x.IntervalStart <= to && x.IntervalEnd >= from))
                {
                    MessageBox.Show($"Invalid interval for would-be '{name}': overlaps with existing sequences"); continue;
                }
                CSequence sequence = new CSequence(Model);
                sequence.Name = name;
                sequence.IntervalStart = from;
                sequence.IntervalEnd = to;
                Model.Sequences.Add(sequence);
            }
            DialogResult = true;
        }
        public static string CapitalizeEachWord(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(input.ToLower());
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
        }
    }
}
