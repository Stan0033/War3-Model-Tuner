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
        Dictionary<CSequence, bool> Visibilities = new Dictionary<CSequence, bool>();
        public editvisibilities_window(CModel model, CGeoset geoset)
        {
            InitializeComponent();
            Model = model;
            Geoset = geoset;
            Title = $"Edit alpha visibilites of geoset with ID {geoset.ObjectId}";
            GeosetAnim = new CGeosetAnimation(Model);

            GenerateUI();
        }
        private void GenerateUI()
        {
            foreach (CSequence sequence in Model.Sequences)
            {


                if (Model.GeosetAnimations.Any(x => x.Geoset.Object == Geoset))
                {
                    CGeosetAnimation existing = Model.GeosetAnimations.First(x => x.Geoset.Object == Geoset);
                    if (existing.Alpha.Static == false)
                    {
                        foreach (var item in existing.Alpha)
                        {
                            int time = item.Time;
                            CSequence _sequence = findSequenceofTime(time);
                            if (_sequence != null) {
                                if (Visibilities.ContainsKey(_sequence) == false)
                                {
                                    Visibilities.Add(_sequence, true);
                                } }
                        }
                      
                    }

                }
                

            }
            AddRemainingSEquences();
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
        private CSequence findSequenceofTime(int time)
        {
            foreach (CSequence sequence in Model.Sequences)
            {
                if (sequence.IntervalStart == time) { return sequence; }
            }
            return null;
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
}
