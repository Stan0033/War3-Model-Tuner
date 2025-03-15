using MdxLib.Model;
 
using System.Linq;
 
using System.Windows;
 
using System.Windows.Input;
 
namespace Wa3Tuner
{
   public enum NodeType
    {
        Bone, Helper, Cols, Light, Emitter1, Emitter2, Ribbon, Attachment,
        Event
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
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; }
            if (e.Key == Key.Enter) ok(null, null);
        }
    }
}
