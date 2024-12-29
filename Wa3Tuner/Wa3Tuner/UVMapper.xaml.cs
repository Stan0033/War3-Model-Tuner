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

namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for UVMapper.xaml
    /// </summary>
    public partial class UVMapper : Window
    {
        CModel Model;
        public UVMapper()
        {
            InitializeComponent();
        }
        public void Refresh(CModel model)
        {
            List_Geosets_UV.Items.Clear();
            foreach (CGeoset geoset in model.Geosets)
            {
                string item = $"Geoset {geoset.ObjectId} [{geoset.Faces.Count} faces]";
                List_Geosets_UV.Items.Add(new ListBoxItem() { Content = item });
            }
        }
        
        private void SelectedGeoset(object sender, SelectionChangedEventArgs e)
        {
            if (List_Geosets_UV.SelectedItem != null)
            {
                List_Faces_UV.Items.Clear();
                int index = List_Geosets_UV.SelectedIndex;
                for (int i = 0; i < Model.Geosets[index].Faces.Count; i++)
                {
                    List_Faces_UV.Items.Add(new ListBoxItem() { Content = $"Triangle {i}" });
                }
            }
        }

        private void SelectFaces(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void EnterU(object sender, KeyEventArgs e)
        {

        }

        private void EnterV(object sender, KeyEventArgs e)
        {

        }

        private void CopyUV(object sender, RoutedEventArgs e)
        {

        }

        private void PasteUV(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
          e.Cancel = true;
            Hide();
        }
    }
}
