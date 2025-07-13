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
    /// Interaction logic for MultipleNodeMover.xaml
    /// </summary>
    public partial class MultipleNodeMover : Window
    {
        CModel model;
        public MultipleNodeMover(CModel m)
        {
            InitializeComponent();
            model = m;
            if (model == null ) {DialogResult = false;return; }
            Fill();
        }

        private void Fill()
        {
           for (int i = 0; i < model.Nodes.Count; i++)
            {
                var node = model.Nodes[i];
                string name = $"{node.Name} ({node.GetType().Name})";
                list1.Items.Add(new ListBoxItem() { Content=name});
                list2.Items.Add(new ListBoxItem() { Content=name});
            }
           list2.SelectedIndex= 0;
        }
        private List<INode> GetSelectedNodes()
        {
            var list = new List<INode>();
          

            for (int i = 0; i < list1.Items.Count; i++) {
                if (list1.SelectedItems.Contains(list1.Items[i]))
                {
                    list.Add(model.Nodes[i]);
                }
            }
            return list;
        }
        private INode? GetTargetNode()
        {
            if (list2.SelectedIndex == -1)return null;
            return model.Nodes[list2.SelectedIndex];
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult=false; }
            if (e.Key == Key.Enter) { ok(null, null); }

        }

        private void ok(object sender, RoutedEventArgs e)
        {
            if (list1.Items.Count == 0)
            {
                MessageBox.Show("List1 cannot be empty. Restart the window.");
                return;
            }
            if (list2.Items.Count == 0)
            {
                MessageBox.Show("List2 cannot be empty. Restart the window.");
                return;
            }
            var targetNode = GetTargetNode();
            if (targetNode == null)
            {
                MessageBox.Show("Please select a valid target node.");
                return;
            }

            var nodesToBeMoved = GetSelectedNodes();
            if (nodesToBeMoved.Count == 0)
            {
                MessageBox.Show("Select at least one node from the first list.");
                return;
            }

            if (nodesToBeMoved.Contains(targetNode))
            {
                MessageBox.Show("The target node cannot be one of the selected nodes.");
                return;
            }

            foreach (var node in nodesToBeMoved)
            {
                if (node == null) continue;

                if (node == targetNode)
                {
                    MessageBox.Show($"{node.Name} cannot be moved under itself.");
                    continue;
                }

                // Check if the target node is a descendant of this node (prevent circular reference)
                if (IsDescendantOf(targetNode, node))
                {
                    MessageBox.Show($"Cannot move {node.Name} under its own descendant {targetNode.Name}.");
                    continue;
                }

                if (node.Parent?.Node == targetNode)
                {
                    MessageBox.Show($"{node.Name} is already a child of {targetNode.Name}.");
                    continue;
                }

                // Safe to move
                node.Parent.Attach(targetNode);
            }

            DialogResult = true;
        }

        private bool IsDescendantOf(INode potentialDescendant, INode potentialAncestor)
        {
            var current = potentialDescendant;
            while (current != null)
            {
                if (current == potentialAncestor)
                    return true;
                current = current.Parent?.Node;
            }
            return false;
        }

    }
}
