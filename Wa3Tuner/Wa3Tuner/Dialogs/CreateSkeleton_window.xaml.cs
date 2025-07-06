using MdxLib.Model;
using SharpGL.SceneGraph;
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
    /// Interaction logic for CreateSkeleton_window.xaml
    /// </summary>
    public partial class CreateSkeleton_window : Window
    {
        private CModel Model;
        string BaseDirectory;
        private string Extension = ".war3Skeleton";
        private List<string> LoadedFiles = new();
        private List<tNode> CurrentTreePreview = new List<tNode>();
        public CreateSkeleton_window(CModel model)
        {
            InitializeComponent();
            Model = model;
            BaseDirectory = Path.Combine(AppHelper.Local, "Skeletons");
            if (Directory.Exists(BaseDirectory) == false) { Directory.CreateDirectory(BaseDirectory); }
            LoadFiles(BaseDirectory);
        }

        private void LoadFiles(string folder)
        {
            // files have extension .war3Skeleton
            LoadedFiles .Clear();
            ListLoaded.Items.Clear();
            string[] files = Directory.GetFiles(folder);
            foreach (string file in files)
            {
                string ext = Path.GetExtension(file);
                if (ext == Extension)
                {
                    LoadedFiles.Add(file);
                }
                string name= Path.GetFileNameWithoutExtension(file);
                ListLoaded.Items.Add(new ListBoxItem() { Content=name });
            }
            
        }

        List<tNode> CreatedNodes = new List<tNode>();
        private void create(object sender, RoutedEventArgs e)
        {
            string name =InputCreateName.Text.Trim();
            string writePath = Path.Combine(BaseDirectory, name + Extension);
            if (StringHelper.NameValidFile(name) == false){
                MessageBox.Show("The given name is not a valid file name"); return;
            }
            if (File.Exists(writePath))
            {
                MessageBox.Show("A skeleton with that name already exists");return;
            }
          


            string input = InputCreate.Text;
            bool validName = ValidateInputFormat(input);
            if (!validName)
            {
                MessageBox.Show("The input data is not in a valid format");return;
            }
            
            File.WriteAllText(writePath, input);
            CreatedNodes= CreateTNodesFromInput(input);
            MessageBox.Show("Created!");
            LoadFiles(BaseDirectory);

        }

        private bool ValidateInputFormat(string input)
        {
            var lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int prevDepth = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // Check for leading whitespace before dashes
                if (line.Length > 0 && char.IsWhiteSpace(line[0]))
                    return false;

                // Count depth (number of dashes)
                int depth = 0;
                while (depth < line.Length && line[depth] == '-')
                    depth++;

                string name = line.Substring(depth).Trim();

                // Name must not be empty
                if (string.IsNullOrEmpty(name))
                    return false;

                // Name must not contain internal _h (only allowed at end)
                if (name.Contains("_h") && !name.EndsWith("_h"))
                    return false;

                // Nesting can only go up/down by 1 level max
                if (depth > prevDepth + 1)
                    return false;

                prevDepth = depth;
            }

            return true;
        }


        private bool NameExists(string name)
        {
            throw new NotImplementedException();
        }

        private List<tNode> CreateTNodesFromInput(string input)
        {
            var result = new List<tNode>();
            var lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var depthStack = new Stack<tNode>();

            foreach (var rawLine in lines)
            {
                // Count how many '-' characters are at the start
                int depth = 0;
                while (depth < rawLine.Length && rawLine[depth] == '-')
                    depth++;

                string trimmedLine = rawLine.Substring(depth).Trim();
                tNodeType type = tNodeType.Bone;
                if (trimmedLine.EndsWith("Ref")) type = tNodeType.Attachment;
                if (trimmedLine.EndsWith("_h")) type = tNodeType.Helper;
                

                // Remove the _h suffix if present
                if (type == tNodeType.Helper && trimmedLine.EndsWith("_h"))
                    trimmedLine = trimmedLine.Substring(0, trimmedLine.Length - 2);

                var node = new tNode(type, trimmedLine);

                // Maintain proper parent based on depth
                while (depthStack.Count > depth)
                    depthStack.Pop();

                if (depthStack.Count > 0)
                {
                    node.Parent = depthStack.Peek();
                }
                else
                {
                    result.Add(node); // Root-level node
                }

                depthStack.Push(node);
            }

            return result;
        }


        enum tNodeType { Bone, Helper, Attachment}
        class tNode
        {
            public tNodeType Type;
            public string Name;
            public tNode? Parent = null;
            public tNode(tNodeType type, string name)
            {
                this.Type = type;
                this.Name = name;
            }
        }

        private void showFormat(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                @"Format is:
write each name of new line
if the name ends in _h then it is a helper
if the name ends in ' Ref' then it is an attachment
else it is a bone
use '-' for nesting
example:
Base_h
-Head
--Head Ref
");
        }

        private void Preview(object sender, SelectionChangedEventArgs e)
        {
            if (ListLoaded.SelectedItem != null)
            {
                int index = ListLoaded.SelectedIndex;
                CurrentTreePreview = LoadTree(LoadedFiles[index]);
                PreviewInTree(CurrentTreePreview, tree_preview);
              
            }
        }

        private void PreviewInTree(List<tNode> currentTreePreview, TreeView tree)
        {
            tree.Items.Clear();

            // Map each tNode to its TreeViewItem
            Dictionary<tNode, TreeViewItem> nodeToItem = new Dictionary<tNode, TreeViewItem>();

            foreach (var node in currentTreePreview)
            {
                TreeViewItem item = CreateItem(node.Name, node.Type  );
                nodeToItem[node] = item;

                if (node.Parent == null)
                {
                    // Root node
                    tree.Items.Add(item);
                }
                else if (nodeToItem.TryGetValue(node.Parent, out TreeViewItem parentItem))
                {
                    parentItem.Items.Add(item);
                }
            }
        }

        TreeViewItem CreateItem(string name,tNodeType type)
        {
            TreeViewItem item = new TreeViewItem();
            StackPanel s = new StackPanel();
            s.Orientation = Orientation.Horizontal;

            Image i = new Image();
            i.Source = GetImageSource(type);
            i.Width = 16;
            i.Height = 16;
            i.Margin = new Thickness(0, 0, 4, 0);

            TextBlock t = new TextBlock();
            t.Text = name;

            s.Children.Add(i);
            s.Children.Add(t);

            item.Header = s;
            return item;
        }


        private ImageSource GetImageSource(tNodeType type)
        {
            string IconsPath = System.IO.Path.Combine(AppHelper.Local, "Icons");
            if (type == tNodeType.Helper)
            {
                return   ImageHelper.LoadImageSource(System.IO.Path.Combine(IconsPath, "helper.png"));
                
            }
            else if (type == tNodeType.Bone)
            {
                return ImageHelper.LoadImageSource(System.IO.Path.Combine(IconsPath, "bone.png"));
            }
            else
            {
                return ImageHelper.LoadImageSource(System.IO.Path.Combine(IconsPath, "attach.png"));
            }
                
        }

        private void load(object sender, RoutedEventArgs e)
        {
            if (ListLoaded.SelectedItem == null) { return; }
            int index = ListLoaded.SelectedIndex;
            string path = LoadedFiles[index];
            CurrentTreePreview = LoadTree(path);
            PreviewInTree(CurrentTreePreview,tree_preview);
            CreateSkeletonInsideModel(CurrentTreePreview);

        }

        private void CreateSkeletonInsideModel(List<tNode> tree)
        {
            Dictionary<tNode, INode> createdNodes = new Dictionary<tNode, INode>();

            // First pass: create all nodes (bones/helpers/attachments)
            foreach (var t in tree)
            {
                INode node;
                if (t.Type == tNodeType.Attachment)
                {
                    node = new CAttachment(Model);
                }
                else if (t.Type == tNodeType.Helper)
                {
                    node = new CHelper(Model);
                }
                else
                {
                    node = new CBone(Model);
                }

                node.Name = t.Name;
                createdNodes[t] = node;
                Model.Nodes.Add(node);
            }

            // Second pass: assign parents and attach children
            foreach (var t in tree)
            {
                if (t.Parent != null)
                {
                    var childNode = createdNodes[t];
                    var parentNode = createdNodes[t.Parent];

                   
                    childNode.Parent.Attach(parentNode);
                }
            }

            DialogResult = true;
        }


        private List<tNode> LoadTree(string path)
        {
            List<tNode> tree = new List<tNode>();
            try
            {
                string input = File.ReadAllText(path);
                if (!ValidateInputFormat(input))
                    return new List<tNode>(); // Invalid format

                var lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var depthStack = new Stack<tNode>();

                foreach (var rawLine in lines)
                {
                    // Count depth (number of leading '-')
                    int depth = 0;
                    while (depth < rawLine.Length && rawLine[depth] == '-')
                        depth++;

                    string trimmedLine = rawLine.Substring(depth).Trim();
                    tNodeType type = tNodeType.Bone;
                    if (trimmedLine.EndsWith("_h")) { type = tNodeType.Helper; }
                    if (trimmedLine.EndsWith(" Ref")) { type = tNodeType.Attachment; }
                    

                    if (type == tNodeType.Helper)
                        trimmedLine = trimmedLine.Substring(0, trimmedLine.Length - 2); // remove _h

                    var node = new tNode(type, trimmedLine);

                    while (depthStack.Count > depth)
                        depthStack.Pop();

                    if (depthStack.Count > 0)
                    {
                        node.Parent = depthStack.Peek();
                    }

                    tree.Add(node);
                    depthStack.Push(node);
                }
            }
            catch
            {
                return new List<tNode>(); // Return empty on error
            }

            return tree;
        }

    }
}
