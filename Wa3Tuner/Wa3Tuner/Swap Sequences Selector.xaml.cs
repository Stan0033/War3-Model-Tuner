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
    /// Interaction logic for Swap_Sequences_Selector.xaml
    /// </summary>
    public partial class Swap_Sequences_Selector : Window
    {
        public int s1 = -1;
        public int s2 = -1;
        public Swap_Sequences_Selector(List<CSequence> sequences) 
        {
            InitializeComponent();
            Fill(sequences);
        }

        private void Fill(List<CSequence> sequences)
        {
            foreach (var sequence in sequences)
            {
                string s = $"{sequence.Name} [{sequence.IntervalStart} - {sequence.IntervalEnd}]";
                list1.Items.Add(new ListBoxItem() { Content = s });
                list2.Items.Add(new ListBoxItem() { Content = s });
            }
            list1.SelectedIndex = 0;
            list2.SelectedIndex = 0;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; }
            if (e.Key == Key.Enter) { ok(null, null); }
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            if (list1.SelectedItem != null && list2.SelectedItem != null)
            {
                s1 = list1.SelectedIndex;
                s2 = list2.SelectedIndex;
                if (s1 == s2) {return; }
                DialogResult = true;
                 
            }
        }
    }
}
