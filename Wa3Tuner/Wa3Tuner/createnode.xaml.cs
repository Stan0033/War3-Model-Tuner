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
   public enum NodeType
    {
        Bone, Helper, Cols, Light, Emitter1, Emitter2, Ribbon, Attachment
    }
    /// <summary>
    /// Interaction logic for createnode.xaml
    /// </summary>
    public partial class createnode : Window
    {
        CModel model;
        public string ResultName;
        public NodeType Result;
        public createnode(CModel m)
        {
            InitializeComponent();
            model = m;
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            if (box.Text.Trim().Length == 0) { return; }
            string input = box.Text.Trim();
            if (model.Nodes.Any(x=>x.Name.ToLower() == input.ToLower()))
            {
                MessageBox.Show("A node with this name exists");return;
            }
            ResultName = input;
            Result = (NodeType)List_Type.SelectedIndex;
            DialogResult = true;
        }
    }
}
