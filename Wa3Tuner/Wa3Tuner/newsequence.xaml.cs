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
    /// Interaction logic for newsequence.xaml
    /// </summary>
    public partial class newsequence : Window
    {
        CModel model;
        public newsequence(CModel model_)
        {
            InitializeComponent();
            this.model = model_;
        }
        private bool NameValid(string name, int from, int to)
        {
            if (name.Trim().Length == 0) { return false; }
                if (char.IsLetter(name[0]) == false) { return false; }
                if (from == to) { MessageBox.Show("From and to cannot be equal"); return false; }
                if (from  > to) { MessageBox.Show("From cannot be greater than to"); return false; }
            
                foreach (CSequence sequence in model.Sequences)
            {

                if (sequence.Name == name)
                {
                    MessageBox.Show("There is already a sequence with this name"); return false;
                }
               if (from >= sequence.IntervalStart && from <= sequence.IntervalEnd)
                {
                    MessageBox.Show("From cannot exist in another sequence"); return false;
                }
                if (to >= sequence.IntervalStart && to <= sequence.IntervalEnd)
                {
                    MessageBox.Show("To cannot exist in another sequence"); return false;
                }
                if (from < 0 || to < 0) { MessageBox.Show("No negative values"); return false; }
                if (to > 999999) { MessageBox.Show("'To' cannot be greater than 999,999"); return false; }
            }
            return true;
        }
        private void ok(object sender, RoutedEventArgs e)
        {
            string name = Input_Name.Text;

            bool parsed1 = int.TryParse(Input_From.Text, out int from);
            bool parsed2 = int.TryParse(Input_To.Text, out int to);
            if (!parsed1 || !parsed2) { return; }
            if (NameValid(name, from, to))
            {
               
                CSequence _new = new CSequence(model);
                _new.Name = CapitalizeEachWord( name);
                _new.IntervalStart = from;
                _new.IntervalEnd = to;
                model.Sequences.Add(_new);
                DialogResult = true;
            }
        }
        static string CapitalizeEachWord(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
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
