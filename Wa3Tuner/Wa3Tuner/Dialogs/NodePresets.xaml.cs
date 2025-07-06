using MdxLib.Model;
using System;
using System.Collections.Generic;
using System.IO;
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
using W3_Texture_Finder;
using Wa3Tuner.Helper_Classes;
using Path = System.IO.Path;

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for NodePresets.xaml
    /// </summary>
    public partial class NodePresets : Window
    {
        private List<string> Presets = new List<string>();
      
        private string SourceDirectory;
        CModel Model;
        INode? SelectedNode;
        public NodePresets(CModel m, INode? selectedNode)
        {
            InitializeComponent();
            Model = m;

            SourceDirectory = Path.Combine(AppHelper.Local, "NodePresets");
            Load();
            SelectedNode = selectedNode;
        }

        private void Load()
        {
            Presets = GetSpecialEffectPresets();
            if (Presets.Count == 0)
            {
                MessageBox.Show("There are no presets saved");DialogResult = false; return;
            }
            Fill(Presets);
        }

        private List<string> GetSpecialEffectPresets()
        {
           
            if (Directory.Exists(SourceDirectory))
            {
                return Directory
                    .GetFiles(SourceDirectory)
                    .Where(f => string.Equals(Path.GetExtension(f), ".war3SFXp", StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return new List<string>();
        }


        private void Fill(List<string> data)
        {
            foreach (string s in data)
            {
               string name = Path.GetFileNameWithoutExtension(s);
                list.Items.Add(new ListBoxItem() { Content = name + $" {ReadFirstLine(s)}"});
            }
            if (list.Items.Count > 0) {list.SelectedIndex = 0;}
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) create(null, null);
                if (e.Key == Key.Escape) DialogResult = false;
        }

        private void create(object sender, RoutedEventArgs e)
        {
            if (!NameValid())  return;
            if (list.SelectedItem != null)
            {

                string name = input.Text.Trim();
                int index = list.SelectedIndex;
                INode? Created = NodePresetHandler.Load(Presets[index], Model);
                if (Created == null) {MessageBox.Show("Null output"); return; }
                Created.Name = name;
                Model.Nodes.Add(Created);
                PutUnder(Created);
             
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Select an item");
            }
        }

        private void PutUnder(INode created)
        {
            bool u = check.IsChecked == true;
            if (!u) return;

            if (SelectedNode != null)
            {
               created.Parent.Attach( SelectedNode);
            }
            
        }

        private string ReadFirstLine(string filePath)
        {
            if (!File.Exists(filePath))
                return "";

            using (var reader = new StreamReader(filePath))
            {
                return reader.ReadLine() ?? "";
            }
        }

        private bool NameValid()
        {
            string i =input.Text.Trim();
            if (i.Length == 0) 
            {
                MessageBox.Show("Enter name");
                return false;
            }
            if (NameExist(i))
            {
                MessageBox.Show("Thre is a node with that name already");
                return false;
            }

            return true;
        }
        private bool NameExist(string s)
        {
            string c = s.ToLower();
            foreach (var sequence in Model.Sequences)
            {
                if (sequence.Name.ToLower() == c) return true;
            }
            return false;
        }
       

        private void del(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItem == null) return;
            {
                
            }
            string selected = getSelected();
            File.Delete(selected);
            //unfinished
        }

        private string getSelected()
        {
           ListBoxItem i= list.SelectedItem as ListBoxItem;
            return i.Content.ToString() ?? string.Empty;
        }
    }
}
