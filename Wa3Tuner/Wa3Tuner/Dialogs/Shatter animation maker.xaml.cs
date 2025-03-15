using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for Shatter_animation_maker.xaml
    /// </summary>
    public partial class ShatterAnimationMaker : Window
    {
        MdxLib.Model.CGeoset Geoset;
        CModel Model;
        CHelper Helper;
        private float From, To = 0;
        public ShatterAnimationMaker(MdxLib.Model.CGeoset geoset, 
            MdxLib.Model.CModel model)
        {
            InitializeComponent();
            Geoset = geoset;
            Model = model;


        }
       private void SegmentGeoset()
        {

            List<CGeosetVertex> vertices_New = new List<CGeosetVertex>();
            // segment triangles
            foreach (var traingle in Geoset.Triangles)
            {
                var v1 = CloneVertex(traingle.Vertex1.Object);
                var v2 = CloneVertex(traingle.Vertex2.Object);
                var v3 = CloneVertex(traingle.Vertex3.Object);

                vertices_New.Add(v1);
                vertices_New.Add(v2);
                vertices_New.Add(v3);

            }
            //clear
            Geoset.Vertices.Clear();
            // fill new 
            foreach (var v in vertices_New) Geoset.Vertices.Add(v);
             // complete the rigging process
            Helper = new CHelper(Model);
            Model.Nodes.Add(Helper);
            Helper.Name = $"ShatteredGeoset{Geoset.ObjectId}_{IDCounter.Next_()}";
            foreach (var v in vertices_New)
            {
                CBone bone = new CBone(Model);
                bone.Name = $"ShatteredGeoset{Geoset.ObjectId}_Segment{IDCounter.Next_()}";
                bone.Parent.Attach(Helper);
                Model.Nodes.Add(bone);
                CGeosetGroup group = new CGeosetGroup(Model);
                CGeosetGroupNode gnode = new CGeosetGroupNode(Model);
                gnode.Node.Attach(bone);
                group.Nodes.Add(gnode);
                v.Group.Attach(group);
                Geoset.Groups.Add(group);
            }
           
        }

        private CGeosetVertex CloneVertex( CGeosetVertex  vertex )
        {
            
            CGeosetVertex v = new CGeosetVertex(Model);
            v.Normal = new MdxLib.Primitives.CVector3(vertex.Normal);
            v.Position = new MdxLib.Primitives.CVector3(vertex.Position);
            v.TexturePosition = new MdxLib.Primitives.CVector2(vertex.TexturePosition);
           
            return v;
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            List<CSequence> sequences = GetSelectedSequences();
            if (sequences.Count == 0) { MessageBox.Show("Select at least one sequence"); }
            bool randomizedTravelDistance = RandomizeDistanceCheck.IsChecked == true;
            bool RandomRotation = ApplyRotationCheck.IsChecked == true;
            bool Fall = ApplyFallCheck.IsChecked == true;
            bool Fade = ApplyFadeCheck.IsChecked == true;
          
            if (randomizedTravelDistance)
            {
                bool r = float.TryParse(ShatterDistanceInput.Text, out float from);
                bool r2 = float.TryParse(ShatterDistanceInput.Text, out float to);
                if (r && r2)
                {
                    if (from >= to) { MessageBox.Show("Invalid input"); return; }
                    if (from < 0) { MessageBox.Show("Invalid input"); return; }
                    From = from; To = to;
                }
                else
                {
                    MessageBox.Show("Invalid input");return;
                }
            }
            else
            {
                bool r = float.TryParse(ShatterDistanceInput.Text, out float from);
                
                if (r )
                {
                  
                    if (from < 0) { MessageBox.Show("Invalid input"); return; }
                    
                }
                else
                {
                    MessageBox.Show("Invalid input"); return;
                }
            }
                SegmentGeoset();
           
           
            DialogResult = true;
        }

        private List<CSequence> GetSelectedSequences()
        {
            List<CSequence> list = new List<CSequence>();
            foreach (var item in SequenceList.SelectedItems)
            {   
                list.Add(Model.Sequences[SequenceList.Items.IndexOf(item)]);
            }
                return list;
        }

        private void cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
