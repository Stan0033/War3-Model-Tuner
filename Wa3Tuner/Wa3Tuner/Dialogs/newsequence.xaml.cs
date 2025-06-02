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
        CModel? model;
      public  CSequence? CreatedSequence;
        public newsequence(CModel model_)
        {
            InitializeComponent();
            this.model = model_;
        }
        private bool SequenceValid(string name, int from, int to, bool duration = false)
        {
            if (   model == null ) {return false;}  
            if (name.Trim().Length == 0) { return false; }
            if (char.IsLetter(name[0]) == false) { return false; }
            foreach (CSequence sequence in model.Sequences)
            {
                if (sequence.Name.ToLower() == name.ToLower())
                {
                    MessageBox.Show("There is already a sequence with this name"); return false;
                }
            }
                if (!duration)
            {
                if (from < 0 || to < 0) { MessageBox.Show("No negative values"); return false; }
                if (from == to) { MessageBox.Show("From and to cannot be equal"); return false; }
                if (from > to) { MessageBox.Show("From cannot be greater than to"); return false; }
                if (to > 999999) { MessageBox.Show("'To' cannot be greater than 999,999"); return false; }
                foreach (CSequence sequence in model.Sequences)
                {
                    if (from >= sequence.IntervalStart && from <= sequence.IntervalEnd)
                    {
                        MessageBox.Show("'From' cannot exist in another sequence"); return false;
                    }
                }
                     
            }
            
            
         
            return true;
        }
        private void ok(object? sender, RoutedEventArgs? e)
        {
            string name = Input_Name.Text;
            bool parsed1 = int.TryParse(Input_From.Text, out int from);
            bool parsed2 = int.TryParse(Input_To.Text, out int to);
            
            if (SequenceValid(name, from, to, Radio1.IsChecked == false))
            {
                if (model == null) return;
                if (Radio1.IsChecked == true)
                {
                    if (!parsed1 || !parsed2) { return; }
                 
                    CSequence _new = new CSequence(model);
                    _new.Name = CapitalizeEachWord(name);
                    _new.IntervalStart = from;
                    _new.IntervalEnd = to;
                    model.Sequences.Add(_new);
                    DialogResult = true;
                }
                else
                {
                    if (!parsed1  ) { return; }
                    
                    if (from < 100) { MessageBox.Show("Duration msut be at least 100");  return; }
                    
                    int FirstFrom = FindFirstFreeInterval(from);
                    
                    if (FirstFrom == -1) { return; }
                    if (FoundValidInerval)
                    {
                        CSequence _new = new  (model);
                        _new.Name = CapitalizeEachWord(name);


                        _new.IntervalStart = FirstFrom;
                        int duration = from;
                        _new .IntervalEnd = FirstFrom + duration;
                         
                        
                        model.Sequences.Add(_new);
                        CreatedSequence = _new;
                        DialogResult = true;
                    }
                    else
                    {
                        MessageBox.Show("Could not find a place for this interval");return;
                    }
                    
                }
               
            }
        }
        private bool FoundValidInerval = true;
        private int FindFirstFreeInterval(int duration)
        {
          
            const int minRange = 1;
            const int maxRange = 999999;
            if (model == null) return 0;
            // Store taken intervals in a sorted list
            List<Tuple<int, int>> takenIntervals = model.Sequences
                .Select(seq => Tuple.Create(seq.IntervalStart, seq.IntervalEnd))
                .OrderBy(interval => interval.Item1)
                .ToList();

            int currentPosition = minRange;

            foreach (var interval in takenIntervals)
            {
                int from = interval.Item1;
                int to = interval.Item2;

                // Check for free space before this interval
                if (from - currentPosition >= duration)
                {
                 
                    return currentPosition;
                }

                // Move past this interval
                currentPosition = Math.Max(currentPosition, to + 1);
            }

            // Check if there's space at the end
            if (maxRange - currentPosition + 1 >= duration)
            {
                return currentPosition;
            }
            
            return -1; // No free interval found
        }


        static string CapitalizeEachWord(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(input.ToLower());
        }
        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) ok(null, null);
        }

        private void CheckRadio1(object? sender, RoutedEventArgs? e)
        {
            LabelFrom.Text = "From";
            LabelTo.Text = "To";
            Input_To.Visibility = Visibility.Visible;
        }

        private void CheckRadio2(object? sender, RoutedEventArgs? e)
        {
            LabelFrom.Text = "Duration";
            LabelTo.Text = "";
            Input_To.Visibility = Visibility.Hidden;
        }
    }
}
