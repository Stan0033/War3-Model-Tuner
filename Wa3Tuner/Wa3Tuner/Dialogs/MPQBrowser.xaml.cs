using Microsoft.Win32;
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
using Path = System.IO.Path;

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for MPQBrowser.xaml
    /// </summary>
    public partial class MPQBrowser : Window
    {
        public MPQBrowser()
        {
            InitializeComponent();
            InitializeIcons();
        }
        List<string> CurrentArchive = new List<string>();
        private void InitializeIcons()
        {
            Icons = new Dictionary<string, BitmapSource>();
            string folder = Path.Combine(AppHelper.Local, "Icons");
            BitmapSource image = AppHelper.LoadBitmapImage( Path.Combine(folder, "image.png"));
            BitmapSource model = AppHelper.LoadBitmapImage( Path.Combine(folder, "model.png"));
            BitmapSource text = AppHelper.LoadBitmapImage( Path.Combine(folder, "text.png"));
            BitmapSource html = AppHelper.LoadBitmapImage( Path.Combine(folder, "html.png"));
            BitmapSource sound = AppHelper.LoadBitmapImage( Path.Combine(folder, "sound.png"));
            BitmapSource script = AppHelper.LoadBitmapImage( Path.Combine(folder, "script.png"));
            BitmapSource w3x = AppHelper.LoadBitmapImage( Path.Combine(folder, "w3x.png"));
            BitmapSource w3m = AppHelper.LoadBitmapImage( Path.Combine(folder, "w3m.png"));
            BitmapSource w3n = AppHelper.LoadBitmapImage( Path.Combine(folder, "w3n.png"));
            BitmapSource slk = AppHelper.LoadBitmapImage( Path.Combine(folder, "slk.png"));
            BitmapSource folder_icon = AppHelper.LoadBitmapImage( Path.Combine(folder, "folder.png"));
            BitmapSource unknown = AppHelper.LoadBitmapImage( Path.Combine(folder, "file.png"));
            BitmapSource mpq = AppHelper.LoadBitmapImage( Path.Combine(folder, "mpq.png"));
            BitmapSource ccd = AppHelper.LoadBitmapImage( Path.Combine(folder, "ccd.png"));
            BitmapSource fdf = AppHelper.LoadBitmapImage( Path.Combine(folder, "fdf.png"));
            BitmapSource toc = AppHelper.LoadBitmapImage( Path.Combine(folder, "toc.png"));
            BitmapSource dll = AppHelper.LoadBitmapImage( Path.Combine(folder, "dll.png"));
            BitmapSource m3d = AppHelper.LoadBitmapImage( Path.Combine(folder, "m3d.png"));
            BitmapSource exe = AppHelper.LoadBitmapImage( Path.Combine(folder, "exe.png"));
            Icons.Add(".png", image); 
            Icons.Add(".blp", image);
            Icons.Add(".bmp", image);
            Icons.Add(".tga", image);
            Icons.Add(".jpg", image);
            Icons.Add(".jpeg", image);
            Icons.Add(".pcx", image);
            Icons.Add(".mdx", model);
            Icons.Add(".mdl", model);
            Icons.Add(".flt", model);
            Icons.Add(".txt", text);
            Icons.Add(".ini", text);
            Icons.Add(".html", html);
            Icons.Add(".htm", html);
            Icons.Add(".mp3", sound);
            Icons.Add(".wav", sound);
            Icons.Add(".ogg", sound);
            Icons.Add(".mid", sound);
            Icons.Add(".midi", sound);
            Icons.Add(".dls", sound);
            Icons.Add(".ai", script);
            Icons.Add(".wai", script);
            Icons.Add(".j", script);
            Icons.Add(".js", script);
            Icons.Add(".pld", script);
            Icons.Add(".slk", slk);
            Icons.Add(".mpq", mpq);
            Icons.Add("folder", folder_icon);
            Icons.Add("file", unknown);
            Icons.Add(".w3n", w3n);
            Icons.Add(".w3m", w3m);
            Icons.Add(".w3x", w3x);
           
            Icons.Add(".ccd", ccd);
            Icons.Add(".fdf", fdf);
            Icons.Add(".toc", toc);
            Icons.Add(".dll", dll);
            Icons.Add(".m3d", m3d);
            Icons.Add(".exe", exe);
           
        }

        Dictionary<string, BitmapSource> Icons;

        TreeViewItem CreateFileItem(string FullFilename)
        {
            string fileName = Path.GetFileName(FullFilename);

            TreeViewItem item = new TreeViewItem();
            StackPanel holder = new StackPanel { Orientation = Orientation.Horizontal };
            Image icon = new Image() { Height = 16, Width = 16, Margin = new Thickness(2) };

            string extension = Path.GetExtension(FullFilename);
            if (Icons.ContainsKey(extension.ToLower()))
            {
                icon.Source = Icons[extension.ToLower()];
            }
            else
            {
                icon.Source = Icons["file"];
            }
            TextBlock title = new TextBlock { Text = fileName, Margin = new Thickness(2) };
            TextBlock FullPath = new TextBlock() 
            { Text = FullFilename, Visibility = Visibility.Collapsed }; // used when copying only
            holder.Children.Add(icon);
            holder.Children.Add(title);
            holder.Children.Add(FullPath);
            item.Header = holder;

            return item;
        }

        TreeViewItem CreateFolderItem(string FolderName)
        {
            TreeViewItem item = new TreeViewItem();
            StackPanel holder = new StackPanel { Orientation = Orientation.Horizontal };
            Image icon = new Image() { Height = 16, Width = 16, Margin = new Thickness(2) };
            icon.Source = Icons["folder"];
            TextBlock title = new TextBlock() { Text = FolderName };

            holder.Children.Add(icon);
            holder.Children.Add(title);
            item.Header = holder;
            return item;
        }

        public void RefreshTree(List<string> files, TreeView tree)
        {
            tree.Items.Clear();
            CurrentArchive = files;
            Dictionary<string, TreeViewItem> folderLookup = new Dictionary<string, TreeViewItem>();
            List<TreeViewItem> rootFiles = new List<TreeViewItem>(); // Store root-level files

            foreach (string file in files)
            {
                TreeViewItem fileItem = CreateFileItem(file);
                string directory = Path.GetDirectoryName(file);

                if (FileIsInFolder(file))
                {
                    // Ensure all parent folders exist before adding the file
                    TreeViewItem parentFolder = GetOrCreateFolder(directory, tree, folderLookup);
                    if (parentFolder != null)
                    {
                        parentFolder.Items.Add(fileItem);
                    }
                }
                else
                {
                    // Store root-level files for later addition
                    rootFiles.Add(fileItem);
                }
            }
           // MessageBox.Show((rootFiles.Count>0).ToString());
            // After creating all folders, add root-level files
            foreach (TreeViewItem fileItem in rootFiles)
            {
                tree.Items.Add(fileItem);
            }
        }


        private TreeViewItem GetOrCreateFolder(string path, TreeView tree, Dictionary<string, TreeViewItem> folderLookup)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null; // No parent folder exists
            }

            if (folderLookup.ContainsKey(path))
            {
                return folderLookup[path]; // Folder already exists
            }

            string parentPath = Path.GetDirectoryName(path);
            TreeViewItem parentFolder = GetOrCreateFolder(parentPath, tree, folderLookup); // Recursively ensure parent exists

            TreeViewItem folderItem = CreateFolderItem(Path.GetFileName(path));
            folderLookup[path] = folderItem;

            if (parentFolder != null)
            {
                parentFolder.Items.Add(folderItem);
            }
            else
            {
                tree.Items.Add(folderItem); // If there's no parent, it's a root folder
            }

            return folderItem;
        }


        bool FileIsInFolder(string fullFilePath)
        {
            return fullFilePath.Contains("\\");
        }

       
        private void openwar3(object sender, RoutedEventArgs e)
        {
            RefreshTree(MPQHelper.Listfile_Everything_war3, Tree);
        }

        private void openwar3xlocal(object sender, RoutedEventArgs e)
        {
            RefreshTree(MPQHelper.Listfile_Everything_war3xLocal, Tree);
        }

        private void openpatch(object sender, RoutedEventArgs e)
        {
            RefreshTree(MPQHelper.Listfile_Everything_war3Patch, Tree);
        }

        private void openwar3x(object sender, RoutedEventArgs e)
        {
            RefreshTree(MPQHelper.Listfile_Everything_war3x, Tree);
        }

        private void refresh(object sender, RoutedEventArgs e)
        {
            RefreshTree(CurrentArchive, Tree);
        }
        string GetItemName()
        {
            TreeViewItem item =(TreeViewItem) Tree.SelectedItem;
            StackPanel holder = item.Header as StackPanel;
            if (holder.Children.Count != 3) return "";
            TextBlock t = holder.Children [2] as TextBlock;
            return t.Text;
        }
        private void copy(object sender, RoutedEventArgs e)
        {
            if (Tree.SelectedItem != null)
            {
                Clipboard.SetText(GetItemName());
            }
        }

        private void export(object sender, RoutedEventArgs e)
        {
            string name = GetItemName();
            string path = ShowSaveFileDialog(name);
            if (path == null) return;
            MPQHelper.Export(name, path);
        }
        public static string ShowSaveFileDialog(string fileName, string initialDirectory = "")
        {
            string extension = Path.GetExtension(fileName);
            string extensionWithoutDot = extension.TrimStart('.'); // Remove leading dot

            var dialog = new SaveFileDialog
            {
                Title = "Save File",
                FileName = fileName,
                InitialDirectory = string.IsNullOrEmpty(initialDirectory) ? Directory.GetCurrentDirectory() : initialDirectory,
                Filter = $"{extensionWithoutDot.ToUpper()} Files|*.{extensionWithoutDot}",
                AddExtension = true,  // Ensures Windows appends extension if missing
                DefaultExt = extensionWithoutDot, // Ensures correct extension
                ValidateNames = true
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedPath = dialog.FileName;

                // Ensure the file has the correct extension
                if (!selectedPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                {
                    selectedPath += extension;
                }

                return selectedPath;
            }

            return null;
        }

        

        private void Window_MouseDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            
        }
    }
}
