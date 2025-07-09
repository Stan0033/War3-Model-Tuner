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
using Wa3Tuner.Helper_Classes;

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for Gradual_Visibility_Maker.xaml
    /// </summary>
    public partial class Gradual_Visibility_Maker : Window
    {
        public bool Visible;
        CModel Model;
        MainWindow Main;
        private int EstimatedGap = 0;
        private CSequence SelectedSequence;
        private List<CGeoset> ALWAYS = new List<CGeoset>();
        private List<CGeoset> Gradual = new List<CGeoset>();
        private List<CGeoset> All = new List<CGeoset>();
        private string question = "?";
        private bool pause = false; 

        public Gradual_Visibility_Maker(CModel m, MainWindow main)
        {
            InitializeComponent();
            Model = m;
            Main = main;
             
        }
        public void Work(CModel model)
        {
          Model = model;
            
            if (Model.Sequences.Count == 0)
            {
                MessageBox.Show("There are no sequences"); Hide();
                return;
            }
            SelectedSequence = Model.Sequences[0];
          
            if (Model.Geosets.Count  <=2)
            {
                MessageBox.Show("At least 2 geosets must be present"); Hide();
                return;
            }
            pause = true;
            ClearAll();
            All.AddRange(model.Geosets.ObjectList);
            FillSequences();
            pause = false;
           REfreshAll();


          
            Show();
        }
        
        private void ClearAll()
        {
            All.Clear();
            Gradual.Clear();
            ALWAYS.Clear();
            GrapShow.Text = question;
            list_sequences.Items.Clear();
            listGeosets.Items.Clear();
            listGeosetsAlways.Items.Clear();
            listGeosetsOrder.Items.Clear();

        }
        private void FillSequences()
        {
            foreach (var sequence in Model.Sequences)
            {
                list_sequences.Items.Add(new ListBoxItem() { Content= $"{sequence.Name} [{sequence.IntervalStart} - {sequence.IntervalEnd}]"});

            }
            list_sequences.SelectedIndex = 0;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
            Hide();
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            if (EstimatedGap < 1)
            {
                MessageBox.Show("Estimated gap is not valid"); return;
            }
            if (listGeosets.Items.Count != 0)
            {
                MessageBox.Show("All availsble geosets must be categorized");return;
            }
            if (listGeosetsOrder.Items.Count  <= 1)
            {
                MessageBox.Show("At least two geosets must be inside the list of gradual visibility"); return;
            }
           
            bool visible = check_1.IsChecked == true;
          
            CreateGradualVisibility(visible );
           
        }

        private void CreateGradualVisibility(bool visible)
        {
            bool ValuesValid = CheckValidDelay();
            if (!ValuesValid)
            {
                MessageBox.Show("Invalid or negative starting and/or ending delay input");
                return;
            }
            int initialDelay = int.Parse(InputDelay.Text);
            int finalDelay  =int.Parse(InputDelay2.Text);
            

            int visibility = visible ? 1 : 0;
            int counterVisibility = visible ? 0 : 1;

            if (Gradual.Count < 2)
            {
                MessageBox.Show("Not enough geosets for gradual visibility.");
                return;
            }

            int start = SelectedSequence.IntervalStart;
            int end = SelectedSequence.IntervalEnd;

            int totalGapTime = (Gradual.Count - 1) * EstimatedGap;
            int lastGeosetTime = start + initialDelay + totalGapTime;

            if (lastGeosetTime > end - finalDelay)
            {
                MessageBox.Show("Initial + final delay and spacing leave insufficient room for all geosets.");
                return;
            }

            for (int i = 0; i < Gradual.Count; i++)
            {
                var geoset = Gradual[i];
                CGeosetAnimation ga = GetGeosetAnimationForGeoset(geoset);
                if (ga.Alpha.Static) ga.Alpha.MakeAnimated();

                int timeToSet = start + initialDelay + i * EstimatedGap;
                int endKeyframeTime = end - finalDelay;

                // Safety: Ensure visibility end time isn't before visibility start
                if (endKeyframeTime < timeToSet)
                    endKeyframeTime = timeToSet;

                // Hidden at start if there's a delay
                if (timeToSet > start)
                    AddKeyframe(ga, start, counterVisibility);

                AddKeyframe(ga, timeToSet, visibility);
                CreateOrSetEndingKeyframe(ga, endKeyframeTime, visibility);
            }

            AddAllMissingSEquencesToGeosetAnimations();
            MessageBox.Show("Created!");
            InputDelay.Text = initialDelay.ToString();
            InputDelay2.Text = finalDelay.ToString();
        }

        private bool CheckValidDelay()
        {
            int nu1, nu2 = -1;
            bool n1 = int.TryParse(InputDelay2.Text ,out nu1);
            bool n2 = int.TryParse(InputDelay2.Text ,out nu2);

           
            return n1 && n1 && nu1 >= 0 && nu2 >=0;
        }

        private void CreateOrSetEndingKeyframe(CGeosetAnimation ga, int ending, int value)
        {
            CAnimatorNode<float>? last = ga.Alpha.FirstOrDefault(X => X.Time == ending);
            if (last == null)
            {
                CAnimatorNode<float> created = new CAnimatorNode<float> { Time = ending, Value = value };
                ga.Alpha.Add(created);
            }
            else
            {
                last.Value = value;
            }
        }

        private void AddKeyframe(CGeosetAnimation ga, int time, int visbility)
        {
            CAnimatorNode<float>? keyframe = ga.Alpha.FirstOrDefault(x => x.Time == time);
            if (keyframe == null)
            {
                CAnimatorNode<float> node = new MdxLib.Animator.CAnimatorNode<float>();
                node.Time = time;
                node.Value = visbility;
                ga.Alpha.Add(node);

            }
            else
            {
                keyframe.Value = visbility;
            }
        }

        private CGeosetAnimation GetGeosetAnimationForGeoset(CGeoset geoset)
        {
            var ga = Model.GeosetAnimations.FirstOrDefault(x => x.Geoset.Object == geoset);
            if (ga == null)
            {
                ga = new CGeosetAnimation(Model);
                ga.Geoset.Attach(geoset);
                Model.GeosetAnimations.Add(ga);
            }
            return ga;
        }

        private void AddAllMissingSEquencesToGeosetAnimations()
        {
         
           foreach (var ga in Model.GeosetAnimations)
            {
                if (ga.Alpha.Static) { ga.Alpha.MakeStatic(1); continue; }
                foreach (var sequence in Model.Sequences)
                {
                    if (ga.Alpha.Any(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd))

                    {
                        continue;

                    }
                    else
                    {
                        CAnimatorNode<float> node = new MdxLib.Animator.CAnimatorNode<float>();
                        node.Time = sequence.IntervalStart;
                        node.Value = 1;
                        ga.Alpha.Add(node);
                    }
                     

                }
                ga.Alpha.NodeList = ga.Alpha.NodeList.OrderBy(x => x.Time).ToList();
            }
        }

        private void add_grad_v(object sender, RoutedEventArgs e)
        {
            int index = listGeosets.SelectedIndex;
            ALWAYS.Add(All[index]);
            All.RemoveAt(index);
            RefreshALWAYSGeosets();
            REfreshAll();
        }

        private void add_grad_gradual(object sender, RoutedEventArgs e)
        {
            if (listGeosets.SelectedItem == null) return;
            int index = listGeosets.SelectedIndex;
            
            Gradual.Add(All[index]);
            Label_Gradual.Text = $"Gradual visibility in order ({Gradual.Count})";
            All.RemoveAt(index);
            RefreshGradualGeosets();
            REfreshAll();
            RefreshEstimatedGap();

        }
        private void REfreshAll()
        {
            int index = listGeosets.SelectedIndex;
            listGeosets.Items.Clear();
            foreach (var item in All) {
                if (item.Name.Length > 0)
                {
                    listGeosets.Items.Add(new ListBoxItem() { Content = $"{item.ObjectId} ({item.Name})" });
                }
                else
                {
                    listGeosets.Items.Add(new ListBoxItem() { Content = item.ObjectId });
                }
               
            }
            if (index >= 0) { listGeosets.SelectedIndex = index; }
            Label_All.Text = $"Available geosets ({All.Count})";
        }
        private void add_grad_gradual_all(object sender, RoutedEventArgs e)
        {
            foreach (var v in All)
            {
                Gradual.Add(v);
            }
            
            All.Clear();
            RefreshGradualGeosets();
            listGeosets.Items.Clear();
            RefreshEstimatedGap();
            RefreshAll_Label();
        }

        private void RefreshEstimatedGap()
        {
            
           int countGeosets = Gradual.Count;
                EstimatedGap = CalculateEstimatedGap(SelectedSequence.IntervalStart, SelectedSequence.IntervalEnd, countGeosets);
              GrapShow.Text = EstimatedGap.ToString();
            
        }
        private int CalculateEstimatedGap(int from, int to, int geosetCount)
        {
            if (geosetCount <= 1) return 0;
           

            int totalRange = to - from;
           
            if (totalRange <= 0) return 0;
            if (geosetCount > totalRange) return 0;
            // If there are more keyframes than available positions, gap is minimal (0 or 1)
            if (geosetCount >= totalRange + 1) return 1;

            return totalRange / (geosetCount - 1);
        }



        private void add_grad_v_all(object sender, RoutedEventArgs e)
        {
            foreach (var v in All)
            {
                ALWAYS.Add(v);
            }
            All.Clear();
           RefreshALWAYSGeosets();
            listGeosets.Items.Clear();
            RefreshAll_Label();

        }

        private void RefreshAll_Label()
        {
            Label_All.Text = $"Available geosets ({All.Count})";
        }

        private void moveup(object sender, RoutedEventArgs e)
        {
            if (listGeosetsOrder.SelectedItem != null) {
                int index = listGeosetsOrder.SelectedIndex;
                ListHelper.MoveElement(Gradual, index, true); }
            RefreshGradualGeosets();
            if (listGeosetsOrder.SelectedItem != null && listGeosetsOrder.SelectedIndex < listGeosetsOrder.Items.Count)
            {
                listGeosetsOrder.SelectedIndex = listGeosetsOrder.SelectedIndex - 1;
            }
            
        }

        private void movedown(object sender, RoutedEventArgs e)
        {
            if (listGeosetsOrder.SelectedItem != null)
            {
                int index = listGeosetsOrder.SelectedIndex;
                ListHelper.MoveElement(Gradual, index, false);
                RefreshGradualGeosets();
                if (listGeosetsOrder.SelectedItem != null && listGeosetsOrder.SelectedIndex >=0 && listGeosetsOrder.Items.Count>1)
                {
                    listGeosetsOrder.SelectedIndex = listGeosetsOrder.SelectedIndex + 1;
                }
            }
        }
        
        private void RefreshGradualGeosets()
        { 
            int index = listGeosetsOrder.SelectedIndex;
            listGeosetsOrder.Items.Clear();
            foreach (var item in Gradual)
            {

                if (item.Name.Length > 0)
                {
                    listGeosetsOrder.Items.Add(new ListBoxItem() { Content = $"{item.ObjectId} ({item.Name})" });
                }
                else
                {
                    listGeosetsOrder.Items.Add(new ListBoxItem() { Content = item.ObjectId });
                }

               
            }
            if (index >= 0)
            {
                listGeosetsOrder.SelectedIndex = index;
            }
            Label_Gradual.Text = $"Gradual visibility in order ({Gradual.Count})";
        }
        private void RefreshALWAYSGeosets()
        {
            int index = listGeosetsAlways.SelectedIndex;
            listGeosetsAlways.Items.Clear();
            foreach (var item in ALWAYS)
            {

                if (item.Name.Length > 0)
                {
                    listGeosetsAlways.Items.Add(new ListBoxItem() { Content = $"{item.ObjectId} ({item.Name})" });
                }
                else
                {
                    listGeosetsAlways.Items.Add(new ListBoxItem() { Content = item.ObjectId });
                }

                
            }
            if (index >= 0)
            {
                listGeosetsAlways.SelectedIndex = index;
            }
            Label_Always.Text = $"Always visible ({ALWAYS.Count})";
        }
        private void return_from_2(object sender, RoutedEventArgs e)
        {
            if (listGeosetsOrder.SelectedItem != null)
            {
                int index = listGeosetsOrder.SelectedIndex;
                All.Add(Gradual[index]);
                Gradual.RemoveAt(index);
                RefreshGradualGeosets();
                REfreshAll();
                RefreshEstimatedGap();
            }
        }

        private void return_from_2_all(object sender, RoutedEventArgs e)
        {
            All.AddRange(Gradual);
            Gradual.Clear();
            RefreshGradualGeosets();
            REfreshAll();
            RefreshEstimatedGap();
        }

        private void return_from_always(object sender, RoutedEventArgs e)
        {
            if (listGeosetsAlways.SelectedItem!= null)
            {
               int index = listGeosetsAlways.SelectedIndex;
                All.Add(ALWAYS[index]);
                ALWAYS.RemoveAt(index);
                RefreshALWAYSGeosets();
                REfreshAll();
            }
        }

        private void return_from_always_all(object sender, RoutedEventArgs e)
        {
            All.AddRange(ALWAYS);
            ALWAYS.Clear();
            RefreshALWAYSGeosets();
            REfreshAll();
        }

        private void list_sequences_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pause) return;
            SelectedSequence = Model.Sequences[ list_sequences.SelectedIndex];
            RefreshEstimatedGap();
        }

        private void nverse(object sender, RoutedEventArgs e)
        {
            Gradual.Reverse();
            RefreshGradualGeosets();
        }
    }
}
