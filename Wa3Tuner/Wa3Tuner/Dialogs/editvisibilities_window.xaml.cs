using MdxLib.Animator;
using MdxLib.Animator.Animatable;
using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wa3Tuner.Helper_Classes;
using CVector3 = MdxLib.Primitives.CVector3;

namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for editvisibilities_window.xaml
    /// </summary>
    enum ComponentType_Visibility { 
    Geoset, Scaling, Visibility, Tracks, None, EventObject
    }
    public partial class editvisibilities_window : Window
    {
        
        CModel? Model;
        CGeoset? Geoset;
        CEvent EventObject;
        List<Ttrack> Tracks = new List<Ttrack>();
        bool NotGeosetAnimation = false;
        private bool EditingVisibility = false;
        Dictionary<CSequence, bool> Visibilities = new Dictionary<CSequence, bool>();
        ComponentType_Visibility EditType = ComponentType_Visibility.None;
        private CGeosetAnimation GeosetAnim;
        private CAnimator<float> AnimatorFloat;
        INode EditedNode;

        // which geoset
        public editvisibilities_window(CModel model, CGeoset geoset)
        {
            InitializeComponent();
            Model = model;
            Geoset = geoset;
            Title = $"Edit alpha visibilites of geoset with ID {geoset.ObjectId}";
            EditType = ComponentType_Visibility.Geoset;
            GeosetAnim = new CGeosetAnimation(Model);
            AnimatorFloat = GeosetAnim.Alpha;
            GeosetAnim.Alpha.MakeAnimated();
            GenerateUI();
            
        }
        //tracks from transformation editor
        public editvisibilities_window(CModel model, List<Ttrack> tracks)
        {
            InitializeComponent();
            NotGeosetAnimation = true;
            Model = model;
            Tracks = tracks;
            EditType = ComponentType_Visibility.Tracks; ;
            foreach (CSequence sequence in model.Sequences)
            {
                CheckBox c = new CheckBox();
                c.Content =
                    sequence.Name;
                c.IsChecked = true;
                c.Checked += CheckSequence;
                c.Unchecked += CheckSequence;
                Box.Items.Add(c);
                Visibilities.Add(sequence, true);
            }
        }

        // node visibility
        public editvisibilities_window(CModel m, CAnimator<float> animator)
        {
            InitializeComponent();
            this.Model = m;
             AnimatorFloat = animator;
             
            EditType = ComponentType_Visibility.Visibility;
            GenerateUI_ForVisibilities();
        }
        // node scaling
        public editvisibilities_window(CModel m, INode node)
        {
            InitializeComponent();
            this.Model = m;

            EditedNode = node;
            EditType = ComponentType_Visibility.Scaling;

            GenerateUI_ForScalings();
        }
        public editvisibilities_window(CModel m, CEvent node)
        {
            InitializeComponent();
            this.Model = m;
            EventObject = node;
            
            EditType = ComponentType_Visibility.EventObject;

            GenerateUI_ForScalings();
        }
        private void CheckSequence(object? sender, EventArgs e)
        {
            if (Model == null) { return; }
            CheckBox? c = sender as CheckBox;
            if (c == null) { return; }
            string? name = Extractor.GetString(sender);  
            if (name == null) { return; }
            CSequence sequence = Model.Sequences.First(x => x.Name == name);
            Visibilities[sequence] = c.IsChecked == true;
        }
        private void GenerateUI()
        {
            if (Model == null) { return  ; }
            Model.Sequences.ObjectList = Model.Sequences.ObjectList.OrderBy(x => x.IntervalStart).ToList();
             CGeosetAnimation? existing = Model.GeosetAnimations.FirstOrDefault(x => x.Geoset.Object == Geoset);
            if (existing != null && existing.Alpha.Animated)
            {
               
                    foreach (var sequence in Model.Sequences)
                    {
                        int start = sequence.IntervalStart;
                        var first = existing.Alpha.FirstOrDefault(x => x.Time == start);
                        bool IsVisible = first == null ? true : (first.Value < 1 ? false : true);

                        Visibilities.Add(sequence, IsVisible);
                    }
 
            }
                  AddRemainingSEquences();
                         CreateCheckBoxes();
            }

        
         private void AddRemainingSEquences()
        {
            if (Model == null) return;
            foreach (CSequence loopedSequence in Model.Sequences)
            {
                if (Visibilities.ContainsKey(loopedSequence) == false)
                {
                    Visibilities.Add(loopedSequence, true);
                }
            }
            Visibilities = Visibilities
            .OrderBy(x => x.Key.IntervalStart)
                .ToDictionary(x => x.Key, x => x.Value);
        }
        private void CreateCheckBoxes()
        {
            foreach (var item in Visibilities)
            {
                CheckBox c = new CheckBox();
                c.IsChecked = item.Value;
                c.Content = item.Key.Name;
                c.Checked += SetVisibility;
                c.Unchecked += SetVisibility;
                Box.Items.Add(c);
            }
        }
        private void  GenerateUI_ForVisibilities()
        {
            if (Model== null) return;
        foreach (var sequence in Model.Sequences)
            {
                var start = AnimatorFloat.FirstOrDefault(x=>x.Time == sequence.IntervalStart);
                bool IsVisible = start == null?  true : (start.Value < 1? false : true);
                 Visibilities.Add(sequence, IsVisible);
            }


            CreateCheckBoxes();
        }
        private void GenerateUI_ForScalings()
        {
            if (Model == null) return;
            foreach (var sequence in Model.Sequences)
            {
                 bool IsVisible = false;
                Visibilities.Add(sequence, IsVisible);
            }


            CreateCheckBoxes();
        }

        private void SetVisibility(object? sender, EventArgs? e)
        {
            if (Model == null) return;  
            if (sender == null) return;  
            CheckBox? c = (CheckBox)sender;
            if (c == null) return;
            bool visible = c.IsChecked == true;
            int index = Box.Items.IndexOf(sender);
            Visibilities[Model.Sequences[index]] = visible;
        }
        private void FinalizeGeosetAnim()
        {
            foreach (var visibility in Visibilities)
            {
                int time = visibility.Key.IntervalStart;
                float value = visibility.Value == true ? 1 : 0; 
                if (GeosetAnim == null) { continue; }
                GeosetAnim.Alpha.Add(new MdxLib.Animator.CAnimatorNode<float>(time, value));
            }
            if (GeosetAnim == null) return;
            GeosetAnim.Alpha.MakeAnimated();
            GeosetAnim.Geoset.Attach(Geoset);
        }
        private void FinalizeAndClose(object? sender, RoutedEventArgs? e)
        {
            if (Model == null){ MessageBox.Show("null model"); return; }
            if (EditType == ComponentType_Visibility.Visibility)
            {

                DialogResult = true;
                AnimatorFloat.Clear();
                AnimatorFloat.MakeAnimated();
                foreach (var item in Visibilities)
                {
                    AnimatorFloat.Add(new CAnimatorNode<float>() { Time = item.Key.IntervalStart, Value = item.Value ? 1 : 0 });
                }

            }

            else if (EditType == ComponentType_Visibility.Tracks)
            {
                Tracks.Clear();
                foreach (var item in Visibilities)
                {
                    int value = item.Value == true ? 100 : 0;
                    Tracks.Add(new Ttrack(item.Key.IntervalStart, value));
                }
                DialogResult = true;
            }
            else if (EditType == ComponentType_Visibility.Geoset)
            {
                FinalizeGeosetAnim();
                if (Model.GeosetAnimations.Any(x => x.Geoset.Object == Geoset))
                {
                    CGeosetAnimation existing = Model.GeosetAnimations.First(x => x.Geoset.Object == Geoset);
                    Model.GeosetAnimations.Remove(existing);
                }
                Model.GeosetAnimations.Add(GeosetAnim);
                DialogResult = true;
            }
            else if (EditType == ComponentType_Visibility.Scaling)
            {
                EditedNode.Scaling.Clear();     
                foreach (var item in Visibilities)
                {
                   int track = item.Key.IntervalStart;
                    CVector3 data = item.Value == true ? new CVector3(1, 1, 1) : new CVector3(0,0,0);
                    EditedNode.Scaling.Add(new CAnimatorNode<CVector3>() { Time = track, Value = data });
                    ;
                }
                DialogResult = true;
            }
            else if (EditType == ComponentType_Visibility.EventObject)
            {
                EventObject.Tracks.Clear();
                foreach (var item in Visibilities)
                {
                    int track = item.Key.IntervalStart;
                    if (item.Value == true)
                    {
                        EventObject.Tracks.Add(new CEventTrack(Model) { Time = track });
                    }


                }
                DialogResult = true;
            }
        }
        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) FinalizeAndClose(null, null);
        }

        private void s1(object? sender, RoutedEventArgs? e)
        {
            foreach (var item in Box.Items)
            {
                CheckBox? c = item as CheckBox;
                if (c != null)
                {
                    c.IsChecked = true;
                }
            }
        }

        private void s2(object? sender, RoutedEventArgs? e)
        {
            foreach (var item in Box.Items)
            {
                CheckBox? c = item as CheckBox;
                if (c != null)
                {
                    c.IsChecked = false;
                }
            }
        }

        private void s3(object? sender, RoutedEventArgs? e)
        {
            foreach (var item in Box.Items)
            {
               
                CheckBox? c = item as CheckBox;
                if (c != null)
                {
                    bool s = c.IsChecked == true;
                    c.IsChecked = !s;
                }
            }
        }
    }
}
