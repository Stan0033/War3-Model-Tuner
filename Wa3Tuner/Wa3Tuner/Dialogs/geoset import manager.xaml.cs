using MdxLib.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
 
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using W3_Texture_Finder;
using Wa3Tuner.Helper_Classes;
using static Wa3Tuner.Helper_Classes.PathManager;
using Path = System.IO.Path;

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for geoset_import_manager.xaml
    /// </summary>
    public partial class geoset_import_manager : Window
    {
        Dictionary<string, string> Reference = new Dictionary<string, string>();
        public bool Closed = false;
        CModel CurrentModel;
        private bool? Changed = false;

        public geoset_import_manager(CModel model)
        {
            InitializeComponent();
            Fill();
            CurrentModel = model;
        }

        private void Fill()
        {
            string path = Path.Combine(AppHelper.Local, "Geosets");
          //  MessageBox.Show(path);
            var files = GetFilesByExtension(path, ".tgeom");
            if (files.Count == 0)
            {

                MessageBox.Show("No custom geosets are created."); Closed = true; Close();  return;
            }
            for (int i=0; i<files.Count; i++)
            {
                ListBoxItem it = new ListBoxItem();
                string naked = Path.GetFileNameWithoutExtension(files[i]);
                it.Content = naked;
                list.Items.Add(it);
                Reference.Add(naked, files[i]);
            }
        }

        public static List<string> GetFilesByExtension(string directory, string extension)
        {
            if (string.IsNullOrWhiteSpace(directory) || string.IsNullOrWhiteSpace(extension))
                return new List<string>();

            if (!Directory.Exists(directory))
                return new List<string>();

            // Normalize extension (ensure it starts with a dot)
            if (!extension.StartsWith("."))
                extension = "." + extension;

            return Directory
                .EnumerateFiles(directory)
                .Where(file => string.Equals(Path.GetExtension(file), extension, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private void delete(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine(AppHelper.Local, "Geosets");
            List<ListBoxItem> selected = getSelectedItems();
            if (selected == null) return;
            if (selected.Count == 0) {MessageBox.Show("Select at least one item"); return; }
            foreach (var  item in selected)
            {
                string name = item.Content.ToString();
                string fullPath = Path.Combine(path, name + ".tgeom");
                if (File.Exists(fullPath) && Reference.ContainsKey(name))
                {
                    File.Delete(fullPath);
                    list.Items.Remove(item);
                    Reference.Remove(name);
                   
                }

            }
             

        }

        private void rename(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine(AppHelper.Local, "Geosets");
            List<ListBoxItem> selected = getSelectedItems();

            if (selected == null) return;
            if (selected.Count != 1) { MessageBox.Show("Select one item"); return; }
            var item = selected[0];
            string name = item.Content.ToString();
            string fullPath = Path.Combine(path, name + ".tgeom");
            if (File.Exists(fullPath) && Reference.ContainsKey(name))
            {
                Input i = new Input(name,"New name");
                i.ShowDialog();
                if (i.DialogResult == true)
                {
                    string newName = i.Result.ToLower();
                    if (newName.Length == 0) { MessageBox.Show("Empty name"); return; }
                    bool hasKey = Reference.Keys.Any(k => string.Equals(k, newName, StringComparison.OrdinalIgnoreCase));
                    if (hasKey)
                    {
                        MessageBox.Show("An item with that name already exists"); return;
                    }

                    string NewPath = Path.Combine(path, newName + ".tgeom");
                    File.Move(fullPath, NewPath);
                    item.Content = newName;
                    Reference.Remove(name);
                    Reference.Add(newName, NewPath);

                }
            }
        }

        private List<ListBoxItem> getSelectedItems()
        {
            if (list.SelectedItems.Count==0) return new List<ListBoxItem>();
            List < ListBoxItem > l = new List<ListBoxItem>();
           foreach (var item in list.SelectedItems)
            {
                l.Add(item as ListBoxItem);
            }
            return l;
        }

        public static List<int> GetSelectedIndexes(ListBox listBox)
        {
            var selectedIndexes = new List<int>();

            if (listBox == null || listBox.Items.Count == 0)
                return selectedIndexes;

            for (int i = 0; i < listBox.Items.Count; i++)
            {
                if (listBox.SelectedItems.Contains(listBox.Items[i]))
                    selectedIndexes.Add(i);
            }

            return selectedIndexes;
        }

        private void Import(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine(AppHelper.Local, "Geosets");
            var selected = getSelectedItems();
            if (selected == null) return;
            if (selected.Count == 0) { MessageBox.Show("Select at least one item"); return; }
            foreach (var item in selected) {


                string itemPath = Reference[item.Content.ToString()];
                CGeoset? imported = GeosetExporter.ReadGeomerge(itemPath, CurrentModel);
                if (imported == null) continue;
              
            }

            Changed = true;
        }

        private void all(object sender, RoutedEventArgs e)
        {
            foreach (var item in list.Items)
            {
                list.SelectedItems.Add(item);
            }
        }

        private void none(object sender, RoutedEventArgs e)
        {
            list.SelectedItems.Clear();
        }

        private void inverse(object sender, RoutedEventArgs e)
        {
            InvertSelection(list);


        }
        public static void InvertSelection(ListBox listBox)
        {
            if (listBox == null || listBox.Items.Count == 0)
                return;

            var itemsToSelect = new List<object>();

            foreach (var item in listBox.Items)
            {
                if (!listBox.SelectedItems.Contains(item))
                    itemsToSelect.Add(item);
            }

            listBox.SelectedItems.Clear();

            foreach (var item in itemsToSelect)
            {
                listBox.SelectedItems.Add(item);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
         
            DialogResult = Changed;
        }
    }
}
