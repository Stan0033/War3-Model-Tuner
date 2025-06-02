using MdxLib.Model;
using SharpGL.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing.IndexedProperties;
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
using System.Xml.Linq;
using Wa3Tuner.Helper_Classes;

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for model_starter.xaml
    /// </summary>
    public partial class model_starter : Window
    {
        private List<ModelPreset> modelPresets = new List<ModelPreset>();
       
        public  CModel  NewModel = new CModel();
        public model_starter( )
        {
            InitializeComponent();
       
            
            InitPresets();
            Fill();
            FillSequenceNames();
            ListType.SelectedIndex = 0;
            
        }
         
        private void FillSequenceNames()
        {

            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Stand", IsChecked = true, IsEnabled = false } });
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Birth", } }); //1
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Death" } });//2
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Stand Hit" } });//3
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Stand Victory" } }); //4
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Stand Ready" } });//5 
           
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Decay" } }); //6
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Decay Flesh" } }); //7
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Decay Bone" } });//8
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Attack" } });//9
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Attack One" } });//10
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Attack Two" } });//11
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Attack Slam" } });//12
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Attack Walk Stand Spin" } });//13
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Walk" } });//14
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Portrait" } }); //15
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Portrait Talk" } });//16
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Dissipate" } });//17
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Spell" } });//17
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Spell Channel" } });//19
            ListSequences.Items.Add(new ListBoxItem() { Content = new CheckBox() { Content = "Spell Throw" } });//20
          
       
            
        }// defend suffix
        // list of suffixes
        private void Fill()
        {
            for (int i = 1; i <= 6; i++)
            {
                ComboVar.Items.Add(new ComboBoxItem() { Content = i.ToString() });
            }
            for (int i = 0; i < 6; i++)
            {
                ComboUpgradeNumber.Items.Add(new ComboBoxItem() { Content = i.ToString() });
            }
            ComboVar.SelectedIndex = 0;
            ComboUpgradeNumber.SelectedIndex = 0;

        }

        private void InitPresets()
        {
            ModelPreset organic = new ModelPreset( 1, 2,7, 8, 9,15, 16, 17); organic.SetAttachments(1,2,3,4,5,6,7);
            ModelPreset mechanical = new ModelPreset(); mechanical.Inherit(organic); mechanical.SetAttachments(10,11,12,13);
            ModelPreset hero = new ModelPreset(17); hero.Inherit(organic);  
            ModelPreset heroMechanical = new ModelPreset(17); heroMechanical.Inherit(mechanical);
            ModelPreset building = new ModelPreset(1,2,6,15); building.SetAttachments(9,10,11,12,13,14);
            ModelPreset effect = new ModelPreset(1,2 ); 
            ModelPreset buff = new ModelPreset(1,2 ); 
            ModelPreset item = new ModelPreset(1,2 , 15); 
            ModelPreset attachment = new ModelPreset(); 
            ModelPreset projectile = new ModelPreset(1,2 ); 
            ModelPreset decor = new ModelPreset( ); 
            ModelPreset destr = new ModelPreset(1,2); 
            ModelPreset portrait = new ModelPreset(15,16);
            modelPresets = new List<ModelPreset>
            {
                organic,
                mechanical,
                hero,
                heroMechanical,
                building,
                effect,
                buff,
                item,
                attachment,
                projectile,
                decor,
                destr,
                portrait
            };

        }

        private void ListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            FillPreset(ListType.SelectedIndex);
        }

        private void FillPreset(int index)
        {
            for (int i = 1; i < ListSequences.Items.Count; i++)
            {
                ListBoxItem? item = ListSequences.Items[i] as ListBoxItem; if (item == null) { continue; }
                CheckBox? c = item.Content as CheckBox; if (c == null) { continue; }
                c.IsChecked = modelPresets[index].Sequences.Contains(i);
            }
            for (int i = 1; i < ListAttachments.Items.Count; i++)
            {
               
                CheckBox? c = ListAttachments.Items[i] as CheckBox; if (c == null) { continue; }
                c.IsChecked = modelPresets[index].Attachments.Contains(i);
            }
            CheckMedLar.IsChecked = index == 8;
            CheckUpgrades.IsEnabled = index == 4;
            ComboUpgradeNumber.IsEnabled = index == 4;
            ComboVar.IsEnabled = index == 4;   
            if (index == 5 || index == 6 || index == 8)
            {
                CheckMedLar.IsChecked = true;
                CheckMedLar.IsEnabled = true;
            }
            else
            {
                CheckMedLar.IsChecked = false;
                CheckMedLar.IsEnabled = false;
            }
            if (index == 0 || index == 1 || index ==2 || index == 3)
            {
                
                CheckSwim.IsEnabled = true;
            }
            else
            {
                CheckSwim.IsChecked = false;
                CheckSwim.IsEnabled = false;
            }
        }

        private void ok(object? sender, RoutedEventArgs? e)
        {
            string name = InputName.Text;
           CModel model = new CModel();
            model.Name = name;
            NewModel = model;
            GetSequences(model);
            GetAttachmentPoints(model);
            DialogResult = true;

        }
        enum UpgradeNames
        {
            None, First, Second, Third, Fourth, Fifth, Sixth
        }
        private List<string> GetCheckedItems(ListBox list)
        {
            List<string> lst = new();
            foreach (var item in list.Items)
            {
                if (item is ListBoxItem listItem)
                {

                    
                    string? content = listItem.Content.ToString();
                    if (content == null) { continue; }
                   lst.Add(content);
                }
                if (item is CheckBox c)
                {
                    string? content = c.Content.ToString();
                    if (content == null) { continue; }
                    if (c.IsChecked == true) lst.Add(content);
                }
            }
            return lst;
        }
        private void GetSequences(CModel model)
        {
            int Length = 0;
           
            if (int.TryParse(InputSequenceLen.Text, out int len))
            {
                if (len < 1000)
                {
                    Length = 1000;
                }
                else { Length = len; }
            }
            else { Length = 1000; }
            int currentTrack = 0;
            // first all sequences
            var sequences = GetCheckedItems(ListSequences);
            foreach (var item in sequences)
            {
                
                CSequence seq = new CSequence(model);
                seq.IntervalStart = currentTrack;
                seq.IntervalEnd = currentTrack + Length;
                seq.Name = item;
                currentTrack = currentTrack + Length + 1;
                model.Sequences.Add(seq);
            }


            //then all medium/large
            if (CheckMedLar.IsChecked == true)
            {
                foreach (var sequence in model.Sequences)
                {
                    CSequence seq = new CSequence(model);
                    seq.IntervalStart = currentTrack;
                    seq.IntervalEnd = currentTrack + Length;
                    currentTrack = currentTrack + Length + 1;
                    seq.Name = sequence.Name + " Medium";
                    model.Sequences.Add(seq);
                }
                foreach (var sequence in model.Sequences)
                {
                    CSequence seq = new CSequence(model);
                    seq.IntervalStart = currentTrack;
                    seq.IntervalEnd = currentTrack + Length;
                    currentTrack = currentTrack + Length + 1;
                    seq.Name = sequence.Name + " Large";
                    model.Sequences.Add(seq);
                }
            }
            // then all swim
            if (CheckSwim.IsChecked == true)
            {
                foreach (var sequence in model.Sequences)
                {
                    CSequence seq = new CSequence(model);
                    seq.IntervalStart = currentTrack;
                    seq.IntervalEnd = currentTrack + Length;
                    currentTrack = currentTrack + Length + 1;
                    seq.Name = sequence.Name + " Swim";
                    model.Sequences.Add(seq);
                }
            }

            // then all upgrades


            if (ListType.SelectedIndex == 4 && ComboUpgradeNumber.SelectedIndex > 0)
            {
                for (int i = 0; i < ComboUpgradeNumber.SelectedIndex; i++)
                {
                    if (i == 0) { continue; }

                    foreach (var sequence in model.Sequences)
                    {
                        CSequence seq = new CSequence(model);
                        seq.IntervalStart = currentTrack;
                        seq.IntervalEnd = currentTrack + Length;
                        currentTrack = currentTrack + Length + 1;
                        seq.Name = sequence.Name + " " + Enum.GetNames(typeof(UpgradeNames))[i];
                        model.Sequences.Add(seq);
                    }

                }
            }
            // then all alternate
            if (CheckAlternate.IsChecked == true)
            {
                foreach (var sequence in model.Sequences)
                {
                    CSequence seq = new CSequence(model);
                    seq.IntervalStart = currentTrack;
                    seq.IntervalEnd = currentTrack + Length;
                    currentTrack = currentTrack + Length + 1;
                    seq.Name = sequence.Name + " Alternate";
                    model.Sequences.Add(seq);
                }
                CSequence seqm = new CSequence(model);
                seqm.IntervalStart = currentTrack;
                seqm.IntervalEnd = currentTrack + Length;
                currentTrack = currentTrack + Length + 1;
                seqm.Name = "Morph";
                model.Sequences.Add(seqm);
                CSequence seqma = new CSequence(model);
                seqma.IntervalStart = currentTrack;
                seqma.IntervalEnd = currentTrack + Length;
                currentTrack = currentTrack + Length + 1;
                seqma.Name = "Morph Alternate";
                model.Sequences.Add(seqm);
            }
            // then all variations
            if (ComboVar.SelectedIndex > 0)
            {
                int variations = ComboVar.SelectedIndex;

                foreach (var sequence in model.Sequences)
                {
                    sequence.Name = sequence.Name + " 1";
                }

                for (int i = 2; i <= variations; i++)
                {
                    foreach (var sequence in model.Sequences)
                    {
                        CSequence seq = new CSequence(model);
                        seq.IntervalStart = currentTrack;
                        seq.IntervalEnd = currentTrack + Length;
                        currentTrack = currentTrack + Length + 1;
                        seq.Name = sequence.Name + " - " + i.ToString();
                        model.Sequences.Add(seq);
                    }
                }
            }
        }
        private void GetAttachmentPoints(CModel model)
        {
            var att = GetCheckedItems(ListAttachments);
            foreach (var name in att)
            {
                
                    
                    model.Nodes.Add(new CAttachment(model) { Name = name });
                
            }
             
        }
        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { ok(null, null); }
            if (e.Key == Key.Escape) {DialogResult = false;}
        }

        private void sff(object? sender, RoutedEventArgs? e)
        {
            SequenceNamesHelper.Show();
           
        }
    }
    public class ModelPreset
    {
        public string Name = string.Empty;
        public List<int> Sequences = new();
        public List<int> Attachments = new();
        public ModelPreset() { }
        public ModelPreset( params int[] args)
        {
            foreach (int arg in args)
            {
                Sequences.Add(arg);
            }
        }
        public void SetAttachments(params int[] args)
        {
            foreach (int arg in args)
            {
                Attachments.Add(arg);
            }
        }
        internal void Inherit(ModelPreset preset)
        {
           foreach (int s in preset.Sequences) { Sequences.Add(s); }
           foreach (int s in preset.Attachments) { Attachments.Add(s); }
           
        }
    }
}
    
