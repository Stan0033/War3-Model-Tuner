using MdxLib.Model;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for resizeSequence_window.xaml
    /// </summary>
    public partial class resizeSequence_window : Window
    {
      public  int Result;
        CSequence CurrentSequence;
        public resizeSequence_window(CSequence sequence)
        {
            InitializeComponent();
            Label_resizing.Text = $"Resizing sequence '{sequence.Name}'";
            Label_currentEnd.Text= $"from {sequence.IntervalStart} to {sequence.IntervalEnd}";
            CurrentSequence = sequence; 
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            bool parse = int.TryParse(box.Text, out int r);
            if (parse) {
                Result = r; 
                if (Result > CurrentSequence.IntervalStart + 100) 
                {DialogResult = true; } 
            }
        }
    }
}
