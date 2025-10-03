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
    /// Interaction logic for stringContainer.xaml
    /// </summary>
    public partial class stringContainer : Window
    {
        private List<string> strings = new List<string>();
        public stringContainer(ref List<string> s)
        {
            InitializeComponent();
            strings = s;
            foreach (var str in strings)
            {
                list.Items.Add(new ListBoxItem() { Content=str});
            }
        }

        private void copy(object sender, RoutedEventArgs e)
        {
            if (strings.Count == 0)
            { MessageBox.Show("No strings to copy!"); return; }
            int index = list.SelectedIndex;
            Clipboard.SetText(strings[index]);
            DialogResult = true;
            this.Close();   
        }
    }
}
