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
    /// Interaction logic for vis_v.xaml
    /// </summary>
    public partial class vis_v : Window
    {
        CModel CurrentModel;
        CSequence Sequence;
        public vis_v(CModel model)
        {
            InitializeComponent();
            CurrentModel = model;
            foreach (var sequence in CurrentModel.Sequences)
            {
                string itme = $"{sequence.Name} [{sequence.IntervalStart} - {sequence.IntervalEnd}]";
                slist.Items.Add(itme);
            }
        }

        private void set(object sender, RoutedEventArgs e)
        {
            if (Sequence != null)
            {
                float value = check1.IsChecked == true ? 1 : 0;
                int time = 0;
                if (check_start.IsChecked == true)
                {
                    time = Sequence.IntervalStart;
                }
               else  if (check_end.IsChecked == true)
                {
                    time = Sequence.IntervalEnd;
                }
                else
                {
                    bool parsed = int.TryParse(inputCustom.Text, out int t);
                    if (parsed == false) { MessageBox.Show("Expected an integer"); }
                    if (t >= Sequence.IntervalStart && t <= Sequence.IntervalEnd)
                    {
                        foreach (var geoset in CurrentModel.Geosets)
                        {
                            if (CurrentModel.GeosetAnimations.Any(x=>x.Geoset.Object == geoset))
                            {
                                var ga = CurrentModel.GeosetAnimations.First(x => x.Geoset.Object == geoset);
                                if (ga.Alpha.Static)
                                {
                                    ga.Alpha.MakeAnimated();
                                    CAnimatorNode<float> key = new();
                                    key.Time = time;
                                    key.Value = value;
                                    ga.Alpha.NodeList.Add(key);
                                    ga.Alpha.NodeList = ga.Alpha.NodeList.OrderBy(x => x.Time).ToList();
                                }
                                else
                                {
                                    if (ga.Alpha.NodeList.Any(x=>x.Time == time))
                                    {
                                        var key = ga.Alpha.NodeList.First(x => x.Time == time);
                                        key.Value = value;
                                    }
                                    else
                                    {
                                        CAnimatorNode<float> key = new();
                                        key.Time = time;
                                        key.Value = value;
                                        ga.Alpha.NodeList.Add(key);
                                        ga.Alpha.NodeList = ga.Alpha.NodeList.OrderBy(x => x.Time).ToList();
                                    }
                                    
                                }
                            }
                            else
                            {
                                CGeosetAnimation anim = new(CurrentModel);
                                CurrentModel.GeosetAnimations.Add(anim);
                                anim.Alpha.MakeAnimated();
                                CAnimatorNode<float> key = new();
                                key.Time = time;
                                key.Value = value;
                                anim.Alpha.NodeList.Add(key);
                            }
                        }
                    }
                    else
                    {
                        if (parsed == false) { MessageBox.Show("This track is not inside the selected sequence"); }
                    }
                }
            }
          
        }

        private void selected(object sender, SelectionChangedEventArgs e)
        {
            if (slist.SelectedItem != null)
            {
                Sequence = CurrentModel.Sequences[slist.SelectedIndex];

            }
        }

        private void setc(object sender, RoutedEventArgs e)
        {
            inputCustom.IsEnabled = ccustom.IsChecked == true;
        }
    }
}
