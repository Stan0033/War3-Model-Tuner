using MdxLib.Model;
using Microsoft.Win32;
using obj2mdl_batch_converter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using Wa3Tuner.Helper_Classes;
using static Wa3Tuner.MainWindow;

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for ObjImporter.xaml
    /// </summary>
    public partial class ObjImporter : Window
    {
        CModel model;
        CBone GeneratedBone;
        public ObjImporter( CModel m)
        {
            InitializeComponent();
            model = m;
            GeneratedBone = new CBone(model);
            GeneratedBone.Name = "ObjImportGeneratedBone_" + IDCounter.Next_();
            foreach (var geoset in model.Geosets)
            {
                MainList.Items.Add(new ListBoxItem() { Content = geoset.ObjectId.ToString() });
            }
        }

        private void import(object sender, RoutedEventArgs e)
        {
            List<string> files = GetFiles();

            if (files.Count == 0) return;

           
            CGeoset JoinedGeoset = new CGeoset(model);
            JoinedGeoset.Material.Attach(model.Materials[0]);
            model.Nodes.Add(GeneratedBone);
            if (Option_4.IsChecked == true && MainList.Items.Count == 0)
            {
                MessageBox.Show("No geosets");return;
            }
            if (Option_4.IsChecked == true && MainList.SelectedItem == null)
            {
                MessageBox.Show("Select a geoset"); return;
            }
            if (Option_4.IsChecked == true && MainList.SelectedItem != null)
            {
                int id = int.Parse((MainList.SelectedItem as ListBoxItem).ToString());
                JoinedGeoset = model.Geosets.First(x=>x.ObjectId == id);
            }
            foreach (string file in files)
            {
                // get the file as cModel
                CModel obj = ConvertObj(file);

                if (obj == null) continue;
                if (Option_1.IsChecked == true) // all to 1
                {
                    CopyGeosets(obj, JoinedGeoset);
                    model.Geosets.Add(JoinedGeoset);
                }
                if (Option_2.IsChecked == true) // each file  as geoset
                {
                    CopyFileGeosetsAsGeosetToModel(obj);
                }
                if (Option_3.IsChecked == true) // each geoset cloned
                {
                    DuplicateGeosetsToModel(obj);
                }
                if (Option_4.IsChecked == true) // all files for geoset
                {
                    CopyGeosets(obj, JoinedGeoset);
                }
            }
            DialogResult = true;
        }

        private void DuplicateGeosetsToModel(CModel obj)
        {
          foreach (var geoset in obj.Geosets)
            {
                CGeoset cloned = new CGeoset(model);
                CloneGeometry(geoset,cloned);
                model.Geosets.Add(cloned);
            }
        }
        void CloneGeometry(CGeoset from, CGeoset to)
        {
            //--------------------------------------
            CGeosetGroup group = new CGeosetGroup(model);
            CGeosetGroupNode gnode = new CGeosetGroupNode(model);
            gnode.Node.Attach(GeneratedBone);
            group.Nodes.Add(gnode);
            to.Groups.Add(group);


            //--------------------------------------
            Dictionary<CGeosetVertex, CGeosetVertex> reference = new Dictionary<CGeosetVertex, CGeosetVertex>();
            foreach (var vertex in from.Vertices)
            {
                CGeosetVertex cloned = VertexCloner.Clone(vertex, model, group);
                reference.Add(vertex, cloned);
                to.Vertices.Add(cloned);
            }
            foreach (var triangle in from.Triangles)
            {
                CGeosetTriangle cloned = new CGeosetTriangle(model);
                cloned.Vertex1.Attach(reference[triangle.Vertex1.Object]);
                cloned.Vertex2.Attach(reference[triangle.Vertex2.Object]);
                cloned.Vertex3.Attach(reference[triangle.Vertex3.Object]);
                to.Triangles.Add(cloned);
            }
        }

        private void CopyFileGeosetsAsGeosetToModel(CModel obj)
        {

            if (obj.Geosets.Count == 0) { return; }
            CGeoset clonedGeoset = new CGeoset(model);
            //--------------------------------------
            CGeosetGroup group = new CGeosetGroup(model);
            CGeosetGroupNode node = new CGeosetGroupNode(model);
            node.Node.Attach(GeneratedBone);
            group.Nodes.Add(node);
            clonedGeoset.Groups.Add(group);

            //--------------------------------------
            foreach (var OBJ_Geoset in obj.Geosets)
            {
              
                Dictionary<CGeosetVertex, CGeosetVertex> reference = new Dictionary<CGeosetVertex, CGeosetVertex>();
                foreach (var vertex in OBJ_Geoset.Vertices)
                {
                    CGeosetVertex clonedVertex = VertexCloner.Clone(vertex, model, group);
                    reference.Add(vertex,clonedVertex);
                    clonedGeoset.Vertices.Add(clonedVertex);

                }
                foreach (var triangle in OBJ_Geoset.Triangles)
                {
                    CGeosetTriangle tr = new CGeosetTriangle(model);
                    tr.Vertex1.Attach(reference[triangle.Vertex1.Object]);
                    tr.Vertex2.Attach(reference[triangle.Vertex2.Object]);
                    tr.Vertex3.Attach(reference[triangle.Vertex3.Object]);
                    clonedGeoset.Triangles.Add(tr);
                }
                
            }
           
            model.Geosets.Add(clonedGeoset);
            
        }

        private void CopyGeosets(CModel obj, CGeoset toGeoset)
        {
           foreach (var geoset in obj.Geosets)
            {
                //--------------------------------------
                CGeosetGroup group = new CGeosetGroup(model);
                CGeosetGroupNode gnode = new CGeosetGroupNode(model);
                gnode.Node.Attach(GeneratedBone);
                group.Nodes.Add(gnode);
                toGeoset.Groups.Add(group);


                //--------------------------------------
                Dictionary<CGeosetVertex, CGeosetVertex> reference = new Dictionary<CGeosetVertex, CGeosetVertex>();
               foreach (var vertex in geoset.Vertices)
                {
                    CGeosetVertex cloned = VertexCloner.Clone(vertex, model, group);
                    reference.Add(vertex, cloned);
                    toGeoset.Vertices.Add(cloned);
                }
               foreach (var triangle in geoset.Triangles)
                {
                    CGeosetTriangle cloned = new CGeosetTriangle(model);
                    cloned.Vertex1.Attach(reference[triangle.Vertex1.Object]);
                    cloned.Vertex2.Attach(reference[triangle.Vertex2.Object]);
                    cloned.Vertex3.Attach(reference[triangle.Vertex3.Object]);
                    toGeoset.Triangles.Add(cloned);
                }
                
               
               
            }

        }

        private CModel ConvertObj(string file)
        {
            if (ObjValidator.Validate(file))
            {
                ObjFileParserExtended.Parse(file, false, false);
                string mdl = System.IO.Path.ChangeExtension(file, ".mdl");
                ObjFileParserExtended.Save(mdl, false, false);
                ObjFileParserExtended.Objects.Clear();
               
               var model = ModelSaverLoader.Load(mdl);
                System.IO.File.Delete(mdl);
                return model;
            }
            return null;
        }

        private List<string> GetFiles()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "OBJ Files (*.obj)|*.obj",
            DefaultExt = ".obj",
            Title = "Select OBJ Files",
            Multiselect = true
        };

        return openFileDialog.ShowDialog() == true ? new List<string>(openFileDialog.FileNames) : new List<string>();
    }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            
        }
    }

    internal class VertexCloner
    {
        internal static CGeosetVertex Clone(CGeosetVertex original, CModel model_, CGeosetGroup group)
        {
            CGeosetVertex vertex = new CGeosetVertex(model_);
            vertex.Position = new MdxLib.Primitives.CVector3(original.Position);
            vertex.TexturePosition = new MdxLib.Primitives.CVector2(original.TexturePosition);
            vertex.Normal = new MdxLib.Primitives.CVector3(original.Normal);
            vertex.Group.Attach(group);
            return vertex;
            
        }
    }
}
