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
    /// Interaction logic for modelstructure.xaml
    /// </summary>
    public partial class modelstructure : Window
    {
        public modelstructure()
        {
            InitializeComponent();
        }

        private void make(object sender, RoutedEventArgs e)
        {
            TreeViewItem model = new TreeViewItem() { Header = "Model"};
            TreeViewItem model_name = new TreeViewItem() { Header = "Name"};
            TreeViewItem extents = MakeExtents();
            TreeViewItem model_af = new TreeViewItem() { Header = "Animation File" };
            TreeViewItem model_blt = new TreeViewItem() { Header = "Blend Time" };
            model.Items.Add(model_name);
            model.Items.Add(model_af);
            model.Items.Add(model_blt);
            Tree.Items.Add(model);
        }
        private TreeViewItem MakeExtents()
        {
            TreeViewItem item = new TreeViewItem();
            TreeViewItem one = new TreeViewItem() { Header = "Mnimum X" };
            TreeViewItem two = new TreeViewItem() { Header = "Mnimum Y" };
            TreeViewItem three = new TreeViewItem() { Header = "Mnimum Z" };
            TreeViewItem four = new TreeViewItem() { Header = "Maximum X" };
            TreeViewItem five = new TreeViewItem() { Header = "Maximum Y" };
            TreeViewItem six = new TreeViewItem() { Header = "Maximum Z" };
            TreeViewItem sevem = new TreeViewItem() { Header = "Bounds Radius" };
            item.Items.Add(one);
            item.Items.Add(two);
            item.Items.Add(three);
            item.Items.Add(four);
            item.Items.Add(five);
            item.Items.Add(six);
            item.Items.Add(sevem);
           return item;
           
        }
    }
}
