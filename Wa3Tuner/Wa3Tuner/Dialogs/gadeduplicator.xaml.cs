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
    /// Interaction logic for gadeduplicator.xaml
    /// </summary>
    public partial class gadeduplicator : Window
    {
        CModel Model;
        Dictionary<CGeoset, List<CGeosetAnimation>> References = new();
        List<CGeoset> Numbers = new();
    
        public gadeduplicator(CModel m)
        {
           
            Model = m;
            if (m.Geosets.Count == 0)
            {
                MessageBox.Show("There are no geosets");
                Close();
                return;

            }
              if (m.GeosetAnimations.Count == 0)
            {
                MessageBox.Show("There are no geoset animations");
                Close();
                return;
            }
            InitializeComponent();
            
            Fill();

        }

        private void Fill()
        {
            // get geosets and their used
            foreach (var geoset in Model.Geosets)
            {
                var list = Model.GeosetAnimations.Where(x=>x.Geoset.Object==geoset).ToList();
                if (list.Count <= 1) { continue; }
                References.Add(geoset, list);
                Numbers.Add(geoset);
            }
           if (References.Count == 0)
            {
                MessageBox.Show("Thre are no duplicating geoset animations");
                Close();
                return;
            }
            foreach (var geoset in References)
            {
                list.Items.Add(new ListBoxItem() { Content = $"Geoset {geoset.Key.ObjectId} ({geoset.Value.Count})"});
            }
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedItem != null)
            {
                int index = list.SelectedIndex;
                var geoset = Numbers[index];
                Fill_Ga_Data(geoset);
            }
            else
            {
                container.Children.Clear();
                Close();
            }
        }

        private void Fill_Ga_Data(CGeoset geoset)
        {
            for (int i = 0; i < References[geoset].Count; i++) {
                //button
                var ga = References[geoset][i];

                Button b = new Button();
                b.Content = "Leave only this geoset animation";
                b.Click += (object sender, RoutedEventArgs e) =>
                {

                    //clear container
                    container.Children.Clear();
                    //remove from model
                    foreach (var ga_ in References[geoset])
                    {
                        if (ga_ == ga) continue;
                        Model.GeosetAnimations.Remove(ga_);
                    }

                    //remove from lists here
                   
                    References.Remove(geoset);
                    int geosetIndex = Numbers.IndexOf(geoset);
                    Numbers.Remove(geoset);
                    list.Items.RemoveAt(geosetIndex);
                };
                b.Padding = new Thickness(10);
                b.Width = 200;
                b.Cursor = Cursors.Hand;
                TextBlock enumeration = new TextBlock() { Text = i.ToString(), FontSize = 16, Padding = new Thickness(10) };
               // statics
               StackPanel statics = new StackPanel();
                statics.Orientation = Orientation.Horizontal;
                CheckBox alpha = new CheckBox() { Content = "Animated Alpha", IsChecked = ga.Alpha.Animated, Padding = new Thickness(5),IsEnabled=false };
                CheckBox color = new CheckBox() { Content = "Animated Color", IsChecked = ga.Color.Animated, Padding = new Thickness(5), IsEnabled = false };
                TextBox alpha_t = new TextBox() { IsReadOnly=true, Width=100,Padding= new Thickness(5), Text = ga.Alpha.GetValue().ToString(), Background= Brushes.LightGray};
                TextBox color_t = new TextBox() { IsReadOnly=true, Width=100,Padding= new Thickness(5), Text = ga.Color.GetValue().ToString(), Background = Brushes.LightGray };
                statics.Children.Add(alpha);
                statics.Children.Add(alpha_t);
                statics.Children.Add(color);
                statics.Children.Add(color_t);
                //animated data
                 Grid grid = new Grid();
                grid.Height = 250;
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                TextBox t1 = new TextBox() { IsReadOnly = true, Background = Brushes.LightGray };
                t1.AcceptsReturn = true;
                t1.TextWrapping = TextWrapping.Wrap;
                t1.Text = GetAnimatedAlpha(ga);
                
                TextBox t2 = new TextBox() { IsReadOnly = true, Background = Brushes.LightGray };
                t2.TextWrapping = TextWrapping.Wrap;
                t2.Text = getAnimatedColor(ga);
                t2.AcceptsReturn = true;
                t2.TextWrapping = TextWrapping.Wrap;
                grid.Children.Add(t1);
                ;
                grid.Children.Add(t2);
                Grid.SetColumn(t2,1);

                //tags
                CheckBox shadow = new CheckBox() { Content = "Drops Shadow", IsChecked = ga.DropShadow, Padding = new Thickness(5), IsEnabled = false };
                CheckBox usecolor = new CheckBox() { Content = "Uses Color", IsChecked = ga.UseColor, Padding = new Thickness(5), IsEnabled = false };
                StackPanel checkboxes = new StackPanel();
                checkboxes.Orientation = Orientation.Horizontal;
                checkboxes.Children.Add(shadow);
                checkboxes.Children.Add(usecolor);

                container.Children.Add(b);
                container.Children.Add(enumeration);
                container.Children.Add(statics);
                container.Children.Add(grid);
                container.Children.Add(checkboxes);

            }
           
        }

        private string getAnimatedColor(CGeosetAnimation ga)
        {
           StringBuilder s = new StringBuilder();
            foreach (var kf in ga.Color)
            {
                s.AppendLine($"({GetSequenceofTrack(kf.Time)}) {kf.Time}: {kf.Value.ToString()}");
                if (
                    ga.Alpha.Type == MdxLib.Animator.EInterpolationType.Hermite ||
                    ga.Alpha.Type == MdxLib.Animator.EInterpolationType.Bezier

                    )
                {
                    s.AppendLine($"InTan: {kf.InTangent.ToString()}");
                    s.AppendLine($"OutTan: {kf.OutTangent.ToString()}");

                }
            }
            return s.ToString();
        }
        private string GetSequenceofTrack(int track)
        {
            foreach (var sequence in Model.Sequences)
            {
                if (sequence.IntervalStart >= track && sequence.IntervalEnd >= track) return sequence.Name;
            }
            return "(No Sequence)";
        }

        private string GetAnimatedAlpha(CGeosetAnimation ga)
        {
            StringBuilder s = new StringBuilder();
            foreach (var kf in ga.Alpha)
            {
                s.AppendLine($"({GetSequenceofTrack(kf.Time)}) {kf.Time}: {kf.Value.ToString()}");
                if (
                    ga.Alpha.Type == MdxLib.Animator.EInterpolationType.Hermite ||
                    ga.Alpha.Type == MdxLib.Animator.EInterpolationType.Bezier

                    )
                {
                    s.AppendLine($"InTan: {kf.InTangent.ToString()}");
                    s.AppendLine($"OutTan: {kf.OutTangent.ToString()}");

                }
            }
            return s.ToString();
        }
    }
}
