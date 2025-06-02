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
    /// Interaction logic for NoKeyframesActions.xaml
    /// </summary>
    /// 
    public partial class NoKeyframesActions : Window
    {
        enum Stretch
        {
            Do, Dont
        }
        enum Overwrite
        {
            All , OVerwriteDuplicate, LeaveOnlyDuplicates, SkipDuplicates
        }
        CModel Model;
        INode? Target;
        Stretch StretchMethod = Stretch.Dont;
        Overwrite OvMethod = Overwrite.All;
        public NoKeyframesActions(CModel model, INode node)
        {
            InitializeComponent();
            Model = model;
            Target = node;
            foreach (var s in model.Sequences)
            {
                list1.Items.Add(new ListBoxItem() { Content = s.Name});
                list2.Items.Add(new ListBoxItem() { Content = s.Name});
            }

        }
        public bool Translation, Rotation, Scaling = false;

        private void ok(object? sender, RoutedEventArgs? e)
        {

            Collect();
            if (Target == null){return;}  
            if (!Translation && !Rotation && !Scaling)
            {
                MessageBox.Show("Selection at least one transformation");return;
            }
            if (list1.SelectedIndex == list2.SelectedIndex)
            {
                MessageBox.Show("Select different sequences"); return;
            }
            CSequence from = Model.Sequences[list1.SelectedIndex];
            CSequence to = Model.Sequences[list2.SelectedIndex];
            if (Translation)
            {
                if (Target.Translation.Count == 0) { MessageBox.Show("This node does not have translation keyfreams"); return; }
                
            }
            if (Rotation)
            {
                    if (Target.Rotation.Count == 0) { MessageBox.Show("This node does not have Rotation keyfreams"); return; }

                    }
            if (Scaling)
            {
                    if (Target.Scaling.Count == 0) { MessageBox.Show("This node does not have Scaling keyfreams"); return; }


                        }
                }
         
        private void Collect()
        {
            if (method_1.IsChecked == true)   StretchMethod = Stretch.Dont;   else StretchMethod = Stretch.Do;
            if (method_3.IsChecked == true) OvMethod = Overwrite.All;
            if (method_4.IsChecked == true) OvMethod = Overwrite.OVerwriteDuplicate;
            if (method_5.IsChecked == true) OvMethod = Overwrite.LeaveOnlyDuplicates;
            if (method_6.IsChecked == true) OvMethod = Overwrite.LeaveOnlyDuplicates;
                Move = action_1.IsChecked == true;
            Copy = !Move;
            Translation = check_1.IsChecked == true;
            Rotation = check_2.IsChecked == true;
            Scaling = check_3.IsChecked == true;
        }

        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) ok(null, null);
        }

        public bool Copy, Move = false;
        public NoKeyframesActions(CModel model)
        {
            InitializeComponent();
            Model = model;
        }
    }

}
