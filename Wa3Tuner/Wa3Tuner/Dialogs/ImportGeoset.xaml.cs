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
using static Wa3Tuner.MainWindow;

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for ImportGeoset.xaml
    /// </summary>
    public partial class ImportGeosetDialog : Window
    {
        CModel Model;
        List<CBone> Bones = new List<CBone>();
        public CMaterial SelectedMaterial;
        public CBone SelectedNode;
        public ImportGeosetDialog(CModel model)
        {
            InitializeComponent();
            Model = model;
            Fill();
        }

        private void Fill()
        {
           
            foreach (var node in Model.Nodes)
            {
                if (node is CBone bone)
                {
                    Bones.Add(bone);
                    ComboAttachTo.Items.Add(new ComboBoxItem() { Content = bone.Name });
                }
            }
            if (Bones.Count == 0)
            {
                Check_NewBone.IsChecked = true;
                Check_NewBone.IsEnabled = false;
                ComboAttachTo.IsEnabled = false;
            }
            if (ComboAttachTo.Items.Count > 0)
            {
                ComboAttachTo.SelectedIndex = 0;
            }
            foreach (CMaterial material in Model.Materials)
            {
                string fn = "";
                if (material.Layers.Count > 0)
                {
                    string id = material.ObjectId.ToString();
                    var layer = material.Layers[0];
                    var texture = layer.Texture.Object;
                    var path = texture.FileName;
                    if (texture.ReplaceableId > 0)
                    {
                        path = "ReplaceableID" + texture.ReplaceableId.ToString();
                    }
                    fn = $"Material {id} ({path})";
                }
                else
                {
                    fn = $"Material {material.ObjectId} (N/A)";
                }
                ComboMaterial.Items.Add(new ComboBoxItem() { Content = fn });    
            }
            ComboMaterial.SelectedIndex = 0;


        }

        private void ok(object sender, RoutedEventArgs e)
        {
            SelectedMaterial = Model.Materials[ComboMaterial.SelectedIndex];
            if (Check_NewBone.IsChecked == true)
            {
                CBone generated = new CBone(Model);
                generated.Name = "ImportedGeoset_"+ IDCounter.Next_();
                SelectedNode = generated;
                Model.Nodes.Add(generated);
            }
            else
            {
                SelectedNode = Bones[ComboAttachTo.SelectedIndex];
            }
           
            DialogResult = true;
        }

        private void CheckedNB(object sender, RoutedEventArgs e)
        {
            bool nb = Check_NewBone.IsChecked == true;
            ComboAttachTo.IsEnabled = !nb;
        }
    }
}
