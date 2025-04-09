using MdxLib.Model;
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
using War3Net.IO.Mpq;
namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for TextureBrowser.xaml
    /// </summary>
    public partial class TextureBrowser : Window
    {
        CModel Model;
        MainWindow Main_Window;
        List<string> Textures = new List<string>();
        Dictionary<string, string> Favourites = new Dictionary<string, string>();
        public TextureBrowser(MainWindow window, List<string> all)
        {
            InitializeComponent();
            Main_Window = window;
            Textures = all;
            RefreshTexturesList();
            LoadFavourites();
        }
        private void RefreshTexturesList()
        {
            FindItemListBox.Items.Clear();
            foreach (string item in Textures)
            {
                FindItemListBox.Items.Add(new ListBoxItem() { Content = item });
            }
        }
        public void SetModel(CModel model)
        {
            Model = model;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string quiery = FindSearchBox.Text.Trim();
                if (quiery.Length == 0) { RefreshTexturesList(); }
                else
                {
                    FindItemListBox.Items.Clear();
                    foreach (string item in Textures)
                    {
                        if (item.ToLower().Contains(quiery.ToLower()))
                        {
                            FindItemListBox.Items.Add(new ListBoxItem() { Content = item });
                        }
                    }
                }
            }
        }
        private void DispalyImage(object sender, SelectionChangedEventArgs e)
        {
            if (Tabs.SelectedIndex == 0)
            {
                if (FindItemListBox.SelectedItem != null)
                {
                    string name = (FindItemListBox.SelectedItem as ListBoxItem).Content.ToString();
                    ImageHolder.Source = MPQHelper.GetImageSource(name);
                }
            }
            else
            {
                if (FavItemListBox.SelectedItem != null)
                {
                   
                    string name = GetSelectedFavouritePath();
                    ImageHolder.Source = MPQHelper.GetImageSource(name);
                }
            }
            
        }
        private void addTexture(object sender, RoutedEventArgs e)
        {
            if (Tabs.SelectedIndex == 0)
            {
                if (FindItemListBox.SelectedItem != null)
                {
                    string name = (FindItemListBox.SelectedItem as ListBoxItem).Content.ToString();
                    if (Main_Window.CurrentModel.Textures.Any(x => x.FileName == name))
                    {
                        MessageBox.Show("This texture is already used by the model"); return;
                    }
                    CTexture texture = new CTexture(Main_Window.CurrentModel);
                    texture.FileName = name;
                    Main_Window.CurrentModel.Textures.Add(texture);
                    Main_Window.RefreshTexturesList();
                    Main_Window.SelectedLayer(null, null);
                }
            }
            else
            {
                string name = (FavItemListBox.SelectedItem as ListBoxItem).Content.ToString();
                if (Main_Window.CurrentModel.Textures.Any(x => x.FileName == Favourites[name]))
                {
                    MessageBox.Show("This texture is already used by the model"); return;
                }
                CTexture texture = new CTexture(Main_Window.CurrentModel);
                texture.FileName = Favourites[name];
                Main_Window.CurrentModel.Textures.Add(texture);
                Main_Window.RefreshTexturesList();
                Main_Window.SelectedLayer(null, null);

            }
            }
            private void AddTextureMat(object sender, RoutedEventArgs e)
        {
            if (Tabs.SelectedIndex == 0)
            {
                if (FindItemListBox.SelectedItem != null)
                {

                    string name = (FindItemListBox.SelectedItem as ListBoxItem).Content.ToString();
                    if (Main_Window.CurrentModel.Textures.Any(x => x.FileName == name))
                    {
                        MessageBox.Show("This texture is already used by the model"); return;
                    }
                    CTexture texture = new CTexture(Main_Window.CurrentModel);
                    texture.FileName = name;
                    CMaterial material = new CMaterial(Main_Window.CurrentModel);
                    CMaterialLayer layer = new CMaterialLayer(Main_Window.CurrentModel);
                    layer.Texture.Attach(texture);
                    material.Layers.Add(layer);
                    Main_Window.CurrentModel.Textures.Add(texture);
                    Main_Window.CurrentModel.Materials.Add(material);
                    Main_Window.RefreshMaterialsList();
                    Main_Window.RefreshTexturesList();
                    Main_Window.SelectedLayer(null, null);
                }
            }
            else
            {

                if (FavItemListBox.SelectedItem != null)
                {

                    string name = (FavItemListBox.SelectedItem as ListBoxItem).Content.ToString();
                    if (Main_Window.CurrentModel.Textures.Any(x => x.FileName == Favourites[name]))
                    {
                        MessageBox.Show("This texture is already used by the model"); return;
                    }
                    CTexture texture = new CTexture(Main_Window.CurrentModel);
                    texture.FileName = Favourites[name];
                    CMaterial material = new CMaterial(Main_Window.CurrentModel);
                    CMaterialLayer layer = new CMaterialLayer(Main_Window.CurrentModel);
                    layer.Texture.Attach(texture);
                    material.Layers.Add(layer);
                    Main_Window.CurrentModel.Textures.Add(texture);
                    Main_Window.CurrentModel.Materials.Add(material);
                    Main_Window.RefreshMaterialsList();
                    Main_Window.RefreshTexturesList();
                    Main_Window.SelectedLayer(null, null);
                }
            }

            
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
        }

        private void FindSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            string searched = FindSearchBox.Text.Trim().ToLower(); ;
            if (searched.Length == 0)
            {
                RefreshTexturesList();
            }
            else
            {
                FindItemListBox.Items.Clear();
                foreach (string item in Textures)
                {
                    if (item.ToLower().Contains(searched))  FindItemListBox.Items.Add(new ListBoxItem() { Content = item });
                }
            }
        }

        private void FavSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            string input = FavSearchBox.Text.Trim().ToLower();
          
            if (input.Length > 0)
            {
                FavItemListBox.Items.Clear();
                foreach (var item in Favourites)
                {
                    if (item.Key.ToLower().Contains(input))
                    {
                        FavItemListBox.Items.Add(item.Key);
                    }
                }
            }
            else
            {
                RefreshFavouritesList();
            }
        }
        void RefreshFavouritesList()
        {
            FavItemListBox.Items.Clear();
            foreach (var item in Favourites)
            {
                FavItemListBox.Items.Add(new ListBoxItem() { Content = item.Key});
            }
        }
        string GetSelectedFavouritePath()
        {
            string s = (FavItemListBox.SelectedItem as ListBoxItem).Content.ToString();
            return Favourites[s];
        }
        void SaveFavourites()
        {
            string path = System.IO. Path.Combine(AppHelper.Local, "Paths\\FavouriteTextures.txt");
            StringBuilder sb = new StringBuilder();
            foreach (var item in Favourites)
            {
                sb.AppendLine($"{item.Key}|{item.Value}");
            }
            File.WriteAllText(path, sb.ToString());

        }
        void LoadFavourites()
        {
            string path = System.IO.Path.Combine(AppHelper.Local, "Paths\\FavouriteTextures.txt");
            Favourites.Clear();
            if (!File.Exists(path)) return;
            foreach (var item in File.ReadAllLines(path))
            {
                string[] parts = item.Split("|");
                Favourites.Add(parts[0], parts[1]);
            }
            RefreshFavouritesList();
        }

        private void AddFavourite(object sender, RoutedEventArgs e)
        {
           if (FindItemListBox.SelectedItem != null)
            {
                string selected = (FindItemListBox.SelectedItem as ListBoxItem).Content.ToString();


                if (Favourites.Any(x=>x.Value == selected) == false)
                {
                    Input i = new Input("", "Note");
                   if (i.ShowDialog() == true)
                    {
                        string name = i.Result;
                        if(Favourites.ContainsKey(name)){
                            MessageBox.Show("There is a favourite with this note already");return;
                        }

                        Favourites.Add(name,selected);
                        if (FavSearchBox.Text.Trim().Length == 0)
                        {
                            RefreshFavouritesList();

                        }
                        SaveFavourites();
                    }
                }
                else
                {
                    MessageBox.Show("There is a favourite with this path already"); return;
                }
            }
        }

        private void DelFavourite(object sender, RoutedEventArgs e)
        {
            if (FavItemListBox.SelectedItem != null)
            {
                string selected = (FavItemListBox.SelectedItem as ListBoxItem).Content.ToString();
                Favourites.Remove(selected);
                FavItemListBox.Items.Remove(FavItemListBox.SelectedItem);
                SaveFavourites();
            }
        }

        private void TabsChange(object sender, SelectionChangedEventArgs e)
        {
            ButtonAddFavourites.Visibility = Tabs.SelectedIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
            DelFavouriteButton.Visibility = Tabs.SelectedIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void copy(object sender, RoutedEventArgs e)
        {
            if (FindItemListBox.SelectedItem != null)
            {
                string item = (FindItemListBox.SelectedItem as ListBoxItem).Content.ToString();
                Clipboard.SetText(item);
            }
        }

        private void export(object sender, RoutedEventArgs e)
        {
            string item = (FindItemListBox.SelectedItem as ListBoxItem).Content.ToString();
            string at = GetSaveLocation();
            if (at.Length == 0) return;
            MPQHelper.Export(item,at);
        }

       

private string GetSaveLocation()
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "BLP Files (*.blp)|*.blp",
            DefaultExt = ".blp",
            Title = "Save BLP File"
        };

        return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : "";
    }
        private string GetSaveLocationPng()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Files (*.png)|*.png",
                DefaultExt = ".png",
                Title = "Save PNG File"
            };

            return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : "";
        }

        private void exportPNG(object sender, RoutedEventArgs e)
        {
            string item = (FindItemListBox.SelectedItem as ListBoxItem).Content.ToString();
            string at = GetSaveLocationPng();
            if (at.Length == 0) return;
            var file = MPQHelper.GetImageSource(item);
            MPQHelper.ExportPNG(file, at);
        }
    }
}
