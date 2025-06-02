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
    /// Interaction logic for Nodes_Renamer.xaml
    /// </summary>
    public partial class Nodes_Renamer : Window
    {
        List<INode> Nodes;
        public Nodes_Renamer(List<INode> nodes)
        {
            InitializeComponent();
            Nodes = nodes;
            Fill();
        }

        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; }
        }
        private void Fill()
        {
            StringBuilder sb = new StringBuilder();
            foreach (INode node in Nodes)
            {
                sb.AppendLine($"{node.Name}->");
            }
        input.Text = sb.ToString();
        }
        private void ok(object? sender, RoutedEventArgs? e)
        {
            string text = input.Text;
            List<string> lines = text.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

            List<string> defined = new();
            if (lines.Count == 0) { MessageBox.Show("No lines"); return; }

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(new[] { "->" }, StringSplitOptions.None);
                if (parts.Length != 2)
                {
                    MessageBox.Show($"Invalid line: \"{line}\". Expected format: originalName -> newName", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string original = parts[0].Trim().ToLower();
                string replacement = parts[1].Trim().ToLower();
                if (replacement.Length == 0)
                {
                    MessageBox.Show("Empty name for replacement");
                    return;
                }
                if (defined.Contains(replacement))
                {
                    MessageBox.Show("Duplicates are not allowed");
                    return;
                }

                defined.Add(replacement);

                foreach (INode node in Nodes)
                {
                    if (node.Name.Trim().ToLower() == original)
                    {
                        if (Nodes.Any(x => x.Name.Trim().ToLower() == replacement) == false)
                        {
                            node.Name = replacement;
                        }
                    }
                }
            }
            DialogResult = true;
        }
    }
}
