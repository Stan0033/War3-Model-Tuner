using MdxLib.Animator;
using MdxLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for editvisibilities_window.xaml
    /// </summary>
    public partial class editvisibilities_window : Window
    {
        CModel Model;
        CGeoset Geoset;
        CGeosetAnimation GeosetAnim;
        List<Ttrack> Tracks = new List<Ttrack>();
        bool NotGeosetAnimation = false;
        private bool EditingVisibility = false;
        Dictionary<CSequence, bool> Visibilities = new Dictionary<CSequence, bool>();
     
        private CAnimator<float> Animator;

        public editvisibilities_window(CModel model, CGeoset geoset)
        {
            InitializeComponent();
            Model = model;
            Geoset = geoset;
            Title = $"Edit alpha visibilites of geoset with ID {geoset.ObjectId}";
            GeosetAnim = new CGeosetAnimation(Model);
            GenerateUI();
        }
        public editvisibilities_window(CModel model, List<Ttrack> tracks)
        {
            InitializeComponent();
            NotGeosetAnimation = true;
            Model = model;
            Tracks = tracks;    
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

        public editvisibilities_window(CModel currentModel, CAnimator<float> animator)
        {
            InitializeComponent();
            this.Model = currentModel;
             Animator = animator;
            EditingVisibility = true;
            GenerateUI_ForVisibilities();
        }
      
        private void CheckSequence(object sender, EventArgs e)
        {
            CheckBox c = (CheckBox)sender;
            string name = c.Content.ToString();
            CSequence sequence = Model.Sequences.First(x => x.Name == name);
            Visibilities[sequence] = c.IsChecked == true;
        }
        private void GenerateUI()
        {
            
            Model.Sequences.ObjectList = Model.Sequences.ObjectList.OrderBy(x => x.IntervalStart).ToList();
             CGeosetAnimation existing = Model.GeosetAnimations.FirstOrDefault(x => x.Geoset.Object == Geoset);
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
        foreach (var sequence in Model.Sequences)
            {
                var start = Animator.FirstOrDefault(x=>x.Time == sequence.IntervalStart);
                bool IsVisible = start == null?  true : (start.Value < 1? false : true);
                 Visibilities.Add(sequence, IsVisible);
            }


            CreateCheckBoxes();
        }
       
        
        private void SetVisibility(object sender, EventArgs e)
        {
            CheckBox c = (CheckBox)sender;
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
                GeosetAnim.Alpha.Add(new MdxLib.Animator.CAnimatorNode<float>(time, value));
            }
            GeosetAnim.Alpha.MakeAnimated();
            GeosetAnim.Geoset.Attach(Geoset);
        }
        private void ok(object sender, RoutedEventArgs e)
        {
            if ( EditingVisibility)
            {

                DialogResult = true;
                Animator.Clear();
                Animator.MakeAnimated();
                foreach (var item in Visibilities)
                {
                    Animator.Add(new CAnimatorNode<float>() { Time = item.Key.IntervalStart, Value = item.Value ? 1 : 0 });
                }

                }

            else if (NotGeosetAnimation)
            {
                Tracks.Clear();
                foreach (var item in Visibilities)
                {
                    int value = item.Value == true ? 1 : 0;
                    Tracks.Add(new Ttrack(item.Key.IntervalStart, value));
                }
                DialogResult = true;
            }
            else
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
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) ok(null, null);
        }

        private void s1(object sender, RoutedEventArgs e)
        {
            foreach (var item in Box.Items)
            {
                CheckBox c = item as CheckBox;
                c.IsChecked = true;
            }
        }

        private void s2(object sender, RoutedEventArgs e)
        {
            foreach (var item in Box.Items)
            {
                CheckBox c = item as CheckBox;
                c.IsChecked = false;
            }
        }

        private void s3(object sender, RoutedEventArgs e)
        {
            foreach (var item in Box.Items)
            {
               
                CheckBox c = item as CheckBox;
                bool s = c.IsChecked == true;
                c.IsChecked = !s;
            }
        }
    }
}
