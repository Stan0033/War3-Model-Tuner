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
    /// Interaction logic for GeosetGroupViewer.xaml
    /// </summary>
    public partial class GeosetGroupViewer : Window
    {
        Dictionary<int, List<string>> attached = new Dictionary<int, List<string>>();
        public GeosetGroupViewer(CGeoset g)
        {
            InitializeComponent();
            Fill(g);
        }

        private void Fill(CGeoset g)
        {
            Title = Title + $" - {g.Groups.Count} groups";
           for (int i =0; i< g.Groups.Count; i++)
            {
                List<string> nodes = new();
                foreach (var gnode in g.Groups[i].Nodes)
                {
                    nodes.Add(gnode.Node.Node.Name);
                }
                attached.Add(i, nodes);
                list1.Items.Add(new ListBoxItem() { Content = i.ToString() });

            }
            list1.SelectedIndex = 0;
        }

        private void list1_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            int index = list1.SelectedIndex;
            list2.Items.Clear();
            foreach (string s in attached[index])
            {
                list2.Items.Add(new ListBoxItem() { Content = s });
            }
        }
    }
}
