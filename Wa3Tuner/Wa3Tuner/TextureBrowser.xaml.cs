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
        public TextureBrowser(MainWindow window, List<string> all)
        {
            InitializeComponent();
            Main_Window = window;
            Textures = all;
            RefreshList();
        }
        private void RefreshList()
        {
            ItemListBox.Items.Clear();
            foreach (string item in Textures)
            {
                ItemListBox.Items.Add(new ListBoxItem() { Content = item });
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
                string quiery = SearchBox.Text.Trim();
                if (quiery.Length == 0) { RefreshList(); }
                else
                {
                    ItemListBox.Items.Clear();
                    foreach (string item in Textures)
                    {
                        if (item.ToLower().Contains(quiery.ToLower()))
                        {
                            ItemListBox.Items.Add(new ListBoxItem() { Content = item });
                        }

                    }
                }

            }
        }

        private void DispalyImage(object sender, SelectionChangedEventArgs e)
        {
            if (ItemListBox.SelectedItem != null)
            {

                string name = (ItemListBox.SelectedItem as ListBoxItem).Content.ToString();

                ImageHolder.Source = MPQHelper.GetImageSource(name);
            }
        }
     
        private void addTexture(object sender, RoutedEventArgs e)
        {
            if (ItemListBox.SelectedItem != null)
            {

                string name = (ItemListBox.SelectedItem as ListBoxItem).Content.ToString();
                if  (Main_Window.CurrentModel.Textures.Any(x=>x.FileName == name))
                {
                    MessageBox.Show("This texture is already used by the model");return;
                }
                CTexture texture = new CTexture(Main_Window.CurrentModel);
                texture.FileName = name;
                Main_Window.CurrentModel.Textures.Add(texture);

                Main_Window.RefreshTextures();
               
            }
            }

            private void AddTextureMat(object sender, RoutedEventArgs e)
        {
            if (ItemListBox.SelectedItem != null)
            {

                string name = (ItemListBox.SelectedItem as ListBoxItem).Content.ToString();
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
                Main_Window.RefreshTextures();
            }
        }
    }
}
