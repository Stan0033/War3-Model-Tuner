using MdxLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
 
using System.Windows.Input;


namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for edit_eventobject.xaml
    /// </summary>
    public partial class edit_eventobject : Window
    {
        private List<int> Tracks = new List<int>();
        private CModel Model;
        private CEvent Event_;
        private bool Create = false;
        private List<string> Data = new List<string>();
        List<string> Sounds = new List<string>();
        List<string> Splats = new List<string>();
        List<string> Ubers = new List<string>();
        List<string> Spawns = new List<string>();
        List<string> Footprints = new List<string>();

        public edit_eventobject(CModel model, CEvent ev)
        {
            InitializeComponent();
            Model = model;
            Event_ = ev;
            RefreshTracks();
            FillData();
            FillRomName();
            FillSequences();
        }

        private void FillRomName()
        {
            inputID.Text = Event_.Name[3].ToString();
            string data = Event_.Name.Substring(4,4);
            SelectItemFromData(data);
         
        }

        private void SelectItemFromData(string data)
        {
            
             
            for (int i = 0; i < box.Items.Count; i++)
            {
                string item = (box.Items[i]as ListBoxItem).Content.ToString();
               if (item.ToLower().StartsWith(data.ToLower())){
                    box.SelectedItem = box.Items[i];
                    box.ScrollIntoView(box.Items[i]);
                    
                     
                    break; }
            }
           
        }

        private void FillData()
        {
            string localPath = AppDomain.CurrentDomain.BaseDirectory;
            string path1 = System.IO.Path.Combine(localPath, "EventObjectData\\SoundData.txt");
            string path2 = System.IO.Path.Combine(localPath, "EventObjectData\\SpawnObjectData.txt");
            string path3 = System.IO.Path.Combine(localPath, "EventObjectData\\SplatData.txt");
            string path4 = System.IO.Path.Combine(localPath, "EventObjectData\\UberSplatData.txt");
            string path5 = System.IO.Path.Combine(localPath, "EventObjectData\\FootPrints.txt");
            Sounds = File.ReadAllLines(path1).ToList();
            Spawns = File.ReadAllLines(path2).ToList();
            Splats = File.ReadAllLines(path3).ToList();
            Ubers = File.ReadAllLines(path4).ToList();
            Footprints = File.ReadAllLines(path5).ToList();
            Data.AddRange(Sounds);
            Data.AddRange(Splats);
            Data.AddRange(Ubers);
            Data.AddRange(Spawns);
            Data.AddRange(Footprints);
            foreach (var track in Event_.Tracks)
            {
                Tracks.Add(track.Time);
            }
            RefillData();
            FillIdentifier();
            RefreshTracks();
        }
       private void FillSequences()
        {
            foreach (CSequence sequence in Model.Sequences)
            {
                SEquenceSelector.Items.Add(new ComboBoxItem() { Content = $"{sequence.Name} [{sequence.IntervalStart} - {sequence.IntervalEnd}]"});
            }
        }
        private void RefillData()
        {
            box.Items.Clear();
            foreach (string item in Data)
            {
                box.Items.Add(new ListBoxItem() { Content = item });
            }
        }
        public edit_eventobject(CModel model)
        {
            InitializeComponent();
            Model = model;
            Event_ = new CEvent(Model);

            Create = true;
            FillData();
            FillSequences();
        }
        private void FillIdentifier()
        {
            if (Event_.Name.Length == 8)
            {
                char id = Event_.Name[3];
                inputID.Text = id.ToString();
            }
        }
        private void FinalizeEvent()
        {
            Event_.Tracks.Clear();
            Tracks = Tracks.OrderBy(x => x).ToList();
            foreach (int track in Tracks)
            {
                CEventTrack t = new CEventTrack(Model);
                t.Time = track;
                Event_.Tracks.Add(t);
            }
        }
        private void RefreshTracks()
        {
            tracks.Items.Clear();
            foreach (var track in Tracks)
            {
                tracks.Items.Add(new ListBoxItem() { Content = track.ToString() });
            }
        }
        private string GetData()
        {
            string item =( box.SelectedItem as ListBoxItem).Content.ToString(); 
            string start = item.Split(' ')[0];
            return start;
        }
        private string GetPrefix()
        {
            string item = (box.SelectedItem as ListBoxItem).Content.ToString();
            if (Sounds.Contains(item)){ return "SND"; }
            if (Splats.Contains(item)) {return "SPL"; }
            if (Ubers.Contains(item)){ return "UBR"; }
            if (Spawns.Contains(item)){ return "SPN"; }
            if (Footprints.Contains(item)){ return "FPT"; }
            return "";
        }
        private void ok(object sender, RoutedEventArgs e)
        {
            if (tracks.Items.Count == 0) { MessageBox.Show("Event object without tracks is not allowed"); return; }
            if (inputID.Text.Trim().Length != 1)
            {
                MessageBox.Show("Incorrect identifier"); return;
            }
            if (char.IsLetter(inputID.Text.Trim()[0]) == false)
            {
                MessageBox.Show("Incorrect identifier. Must be a letter"); return;
            }
            if (box.SelectedItem == null)
            {
                MessageBox.Show("Select data");
                return;
            }
            char identifeir = inputID.Text.Trim()[0];
            string name = GetPrefix() + identifeir + GetData();
         Event_.Name = name;

            FinalizeEvent();
            if (Create)
            {
                Model.Nodes.Add(Event_);
            }
            DialogResult = true;
        }

       
        private void SetSequence(object sender, SelectionChangedEventArgs e)
        {
            if (SEquenceSelector.SelectedItem != null && SEquenceSelector.SelectedIndex != -1)
            {
                int index = SEquenceSelector.SelectedIndex;
                input.Text = Model.Sequences[index].IntervalStart.ToString();
            }
        }

        private void removeall(object sender, RoutedEventArgs e)
        {
            Tracks.Clear();
            RefreshTracks();    
        }

        private void remove(object sender, RoutedEventArgs e)
        {
            if (tracks.SelectedItem != null)
            {
                int index = tracks.SelectedIndex;
                Tracks.RemoveAt(index);
                RefreshTracks();
            }
        }
        private bool TrackExists(int track)
        {
            return Model.Sequences.Any(x=> track >= x.IntervalStart && track <= x.IntervalEnd);
        }
        private void add(object sender, RoutedEventArgs e)
        {
            string input_ = input.Text.Trim();
            bool parse = int.TryParse(input_, out int value);
            if (parse)
            {
                if (TrackExists(value))
                {
                    Tracks.Add(value);
                    Tracks.Sort();
                    RefreshTracks();
                }
                else
                {
                    MessageBox.Show("This track does not exist in any sequence"); return;
                }
            }
            else
            {
                MessageBox.Show("Invalid input, expected integer"); return;
            }
        }

        private void Search(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string search = Searcher.Text.Trim();
                if (search.Length == 0) { RefillData(); return; }
                else
                {
                    box.Items.Clear();
                    foreach (string item in Data)
                    {
                        if (item.ToLower().Contains(search.ToLower()))
                        {
                            box.Items.Add(new ListBoxItem() { Content = item });
                        }
                    }
                }
            }
        }
    }
}
