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

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for Mass_create_global_sequences.xaml
    /// </summary>
    public partial class Mass_create_global_sequences : Window
    {
        CModel model;
        public Mass_create_global_sequences(CModel m)
        {
            InitializeComponent();
            model = m;
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            string[] lines = Input.Text.Split('\n').ToArray();
            List<int> values = new List<int>();
            foreach (var line in lines)
            {
                bool valid = int.TryParse(line, out int value);
                if (valid)
                {
                    if (model.GlobalSequences.Any(x=>x.Duration == value))
                    {
                        MessageBox.Show($"A global sequence with duration {value} already exists");return;
                    }
                    else
                    {
                        if (value <= 0)
                        {
                            MessageBox.Show("0 or negative is not allowed");return;
                        }
                        if (values.Contains(value)){
                            MessageBox.Show($"The duration {value} is present a second time."); return;
                        }
                        values.Add(value);
                    }
                }
                else
                {
                    MessageBox.Show("Expected only integer values");return;
                }
            }
            if (values.Count > 0)
            {
                for (int i =0;i<values.Count; i++)
                {
                    CGlobalSequence gs = new CGlobalSequence(model) { Duration = values[i] };
                    model.GlobalSequences.Add(gs);
                }
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Expected at least one duration");
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) ok(null, null);

        }
    }
}
