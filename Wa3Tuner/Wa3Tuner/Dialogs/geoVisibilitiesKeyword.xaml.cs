using MdxLib.Animator;
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

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for geoVisibilitiesKeyword.xaml
    /// </summary>
    public partial class geoVisibilitiesKeyword : Window
    {
        private CGeoset? geoset;
        private CModel? Model;
        INode? WhichNode;
        public bool Possible = true;
        public geoVisibilitiesKeyword(CGeoset v, CModel currentModel)
        {
            InitializeComponent();
            if (currentModel == null) { MessageBox.Show("Null model"); ; Possible = false; }
            else if (v == null) { MessageBox.Show("Null geoset"); Possible = false;  }
            else if (currentModel.Sequences.Count == 0) { MessageBox.Show("There are no sequences", "Nothing to work with"); ; Possible = false; }
          
            this.geoset = v;
            this.Model = currentModel;
        }
        public geoVisibilitiesKeyword(INode node, CModel? currentModel)
        {
            if (currentModel != null)
            {


                InitializeComponent();
                if (currentModel == null) { MessageBox.Show("Null model"); ; Possible = false; }
                else if (node == null) { MessageBox.Show("Null node"); ; Possible = false; }
                else if (currentModel.Nodes.Contains(node) == false) { MessageBox.Show("Node is not inside model"); ; Possible = false; }
                else if (currentModel.Sequences.Count == 0) { MessageBox.Show("There are no sequences", "Nothing to work with"); ; Possible = false; }
                else if (node is CBone) { MessageBox.Show("Bone does not supot visibilites"); ; Possible = false; }
                else if (node is CCollisionShape) { MessageBox.Show("Collision shape does not support visibilites"); ; Possible = false; }
                else if (node is CCollisionShape) { MessageBox.Show("Collision shape does not support visibilites"); ; Possible = false; }
                else if (node is CHelper) { MessageBox.Show("Helper does not suppot visibilites"); ; Possible = false; }
                else if (node is CEvent) { MessageBox.Show("Event object does not support visibilites"); ; Possible = false; }

                WhichNode = node;
                this.Model = currentModel;
            }
            else
            {
                Close(); ;
            }
        }
        private void HandleNode()
        {
            List<string> keywords = GetKeywords();
            if (keywords.Count == 0) { MessageBox.Show("Enter keyword/s"); return; }
            bool visible = r1.IsChecked == true;
            if (WhichNode is CParticleEmitter emitter)
            {
                HandleAnimator(emitter.Visibility, keywords, visible);
            }
        else if (WhichNode is CParticleEmitter2 emitter2)
            {
                HandleAnimator(emitter2.Visibility, keywords, visible);
            }
        else if (WhichNode is CRibbonEmitter r)
            {
                HandleAnimator(r.Visibility, keywords, visible);
            }
            else if (WhichNode is CLight l)
            {
                HandleAnimator(l.Visibility, keywords, visible);
            }
            else if (WhichNode is CAttachment tt)
            {
                HandleAnimator(tt.Visibility, keywords, visible);
            }
         
           
        }
        private void HandleGeoset()
        {
            if (Model == null) return;
            List<string> keywords = GetKeywords();
            if (keywords.Count == 0) { MessageBox.Show("Enter keyword/s"); return; }


            bool MakeVisibleIfContains = r1.IsChecked == true;
            CGeosetAnimation? ga = Model.GeosetAnimations.FirstOrDefault(x => x.Geoset.Object == geoset);
           
            if (ga != null)
            {
                
                ga.Alpha.Clear();
                ga.Alpha.MakeAnimated();

                foreach (CSequence sequence in Model.Sequences)
                {
                    string name = sequence.Name.ToLower();
                    bool contains = SequenceNameContainsKewords(name, keywords);
                    CAnimatorNode<float> kf = new CAnimatorNode<float>();
                    kf.Time = sequence.IntervalStart;
                    if (MakeVisibleIfContains)
                    {
                        kf.Value = contains ? 1f : 0f;
                    }
                    else
                    {
                        kf.Value = contains ? 0f : 1f;
                    }

                        ga.Alpha.Add(kf);

                }
                
            }
            else
            {
                CGeosetAnimation ga_ = new  (Model);
                ga_.Geoset.Attach(geoset);
                ga_.Alpha.MakeAnimated();
                foreach (CSequence sequence in Model.Sequences)
                {
                    string name = sequence.Name.ToLower();
                    bool contains = SequenceNameContainsKewords(name, keywords);


                    CAnimatorNode<float> kf = new CAnimatorNode<float>();
                    kf.Time = sequence.IntervalStart;
                    kf.Value = MakeVisibleIfContains ? 1 : 0;
                    ga_.Alpha.Add(kf);

                }
                Model.GeosetAnimations.Add(ga_);
            }
            DialogResult = true;
        }

        private static bool SequenceNameContainsKewords(string name, List<string> keywords)
        {
            foreach (string kw in keywords)
            {
                if (name.Contains(kw.ToLower())) return true;
            }
            return false;
        }


        private void HandleAnimator(CAnimator<float> animator, List<string> keywords, bool MakeVisibleIfContains)
        {
            animator.Clear();
            animator.MakeAnimated();
            if (Model == null) { return; }
            foreach (CSequence sequence in Model.Sequences)
            {
                string name = sequence.Name.ToLower();
                bool contains = SequenceNameContainsKewords(name, keywords);

                CAnimatorNode<float> kf = new  ();
                kf.Time = sequence.IntervalStart;
                if (MakeVisibleIfContains)
                {
                    kf.Value = contains ? 1f : 0f;
                }
                else
                {
                    kf.Value = contains ? 0f : 1f;
                }
                animator.Add(kf);

            }
        }

        private void ok(object? sender, RoutedEventArgs? e)
        {
            if (WhichNode == null) HandleGeoset();
            else HandleNode();
               
        }

        private List<string> GetKeywords()
        {
          return box.Text.Split("\n").Select(x=>x.Trim().ToLower()). ToList();
        }
    }
}
