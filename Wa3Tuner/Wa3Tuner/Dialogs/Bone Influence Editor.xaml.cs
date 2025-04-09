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
    /// Interaction logic for Bone_Influence_Editor.xaml
    /// </summary>
    public partial class Bone_Influence_Editor : Window
    {
        private INode Node;
        public Bone_Influence_Editor(INode n)
        {
            InitializeComponent();
            Node = n;
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            //unfinished
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
            }
        }
    }
}
