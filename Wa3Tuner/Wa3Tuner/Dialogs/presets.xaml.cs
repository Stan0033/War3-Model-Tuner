using MdxLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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
namespace Wa3Tuner
{
    /// <summary>
    /// Interaction logic for presets.xaml
    /// </summary>
    public partial class presets : Window
    {
        CModel Model;
        List<string> availableNames = new List<string>();
        List<string> SelectedNames = new List<string>();
        public presets(CModel model)
        {
            InitializeComponent();
            Model = model;
            InitNames();
            Create();
        }
        private void InitNames()
        {
            availableNames = new List<string>()
            {
                "Stand",
                 "Stand Hit",
                "Stand Ready",
                "Stand Victory",
                "Stand Defend",
                "Stand Channel",
                "Stand Cinematic",
                "Birth",
                "Death",
                "Decay",
                "Decay Flesh",
                "Decay Bone",
                "Dissipate",
                "Walk",
                "Walk Defend",
                "Walk Channel",
                "Portrait",
                "Portrait Talk",
                "Attack",
                "Attack One",
                "Attack Two",
                "Attack Slam",
                "Attack Walk Stand Spin",
                "Attack Defend",
                "Spell",
                "Spell One",
                "Spell Two",
                "Spell Throw",
                "Spell Channel",
                "Spell Slam",
            };
        }
        private void Create()
        {
            foreach (var name in availableNames)
            {
                CheckBox c = new CheckBox();
                c.Content = name;
                c.Checked += IncludeName;
                c.Unchecked += IncludeName;
                container.Children.Add(c);
            }
        }
        private void IncludeName(object sender, EventArgs e)
        {
            CheckBox c = (CheckBox)sender;
            bool on = c.IsChecked == true;
            if (on)
            {
                SelectedNames.Add(c.Content.ToString());
            }
            else
            {
                SelectedNames.Remove(c.Content.ToString());
            }
        }
        private void ok(object sender, RoutedEventArgs e)
        {
            bool parse = int.TryParse(inputDuration.Text, out int value);
            if (parse) {
                if (value > 100)
                {
                    int Duration = value;
                    int totalSequences = SelectedNames.Count;
                    bool upgrade = check_upgrade.IsChecked == true;  // 1
                    bool worker = check_worker.IsChecked == true;  // 3 - gold, lumber, work
                    bool alternate = check_alternate.IsChecked == true;  // 1
                    int tiers = listTiers.SelectedIndex;
                    int multiplier = 1;
                    if (upgrade) multiplier += 1;
                    if (worker) multiplier += 3;
                    if (alternate) multiplier += 1;
                    if (tiers > 0) multiplier += tiers;
                    if (Duration * multiplier > 999999) { MessageBox.Show("The total duration of all sequences would exceed 999,999");return; }
                    int CurrentLocation = 0;
                    Model.Sequences.Clear();    
                    foreach (string name in SelectedNames)
                    {
                        CSequence sequence = new CSequence(Model);
                        sequence.Name = name;
                        sequence.IntervalStart = CurrentLocation;
                        sequence.IntervalEnd = CurrentLocation + Duration;
                        CurrentLocation += Duration + 1;
                        Model.Sequences.Add(sequence);
                    }
                    if (worker)
                    {
                        CSequence sequence = new CSequence(Model);
                        sequence.IntervalStart = CurrentLocation;
                        sequence.IntervalEnd = CurrentLocation+ Duration;
                        sequence.Name = "Stand Work";
                        CurrentLocation += Duration + 1;
                        Model.Sequences.Add(sequence);
                        foreach (CSequence name in Model.Sequences.ToList())
                        {
                            CSequence sequence2 = new CSequence(Model);
                            sequence2.IntervalStart =   CurrentLocation;
                            sequence2.IntervalEnd = CurrentLocation + Duration;
                            sequence2.Name = name + " " + "Gold";
                            CurrentLocation += Duration + 1;
                            Model.Sequences.Add(sequence2);
                            CSequence sequence3 = new CSequence(Model);
                            sequence3.IntervalStart = CurrentLocation;
                            sequence3.IntervalEnd = CurrentLocation + Duration;
                            sequence3.Name = name + " " + "Lumber";
                            CurrentLocation += Duration + 1;
                            Model.Sequences.Add(sequence3);
                        }
                    }
                    //update
                    if (upgrade)
                    {
                        foreach (CSequence name in Model.Sequences.ToList())
                        {
                            CSequence sequence = new CSequence(Model);
                            sequence.IntervalStart = CurrentLocation;
                            sequence.IntervalEnd = CurrentLocation + Duration;
                            sequence.Name = name + " " + "Upgrade";
                            CurrentLocation += Duration + 1;
                            Model.Sequences.Add(sequence);
                        }
                    }
                    //tiers
                    List<string> tierNames = new List<string>()
                    {
                        "","First", "Second", "Third", "Fourth", "Fifth", "Sixth"
                    };
                    if (tiers > 0)
                    {
                        for (int i = 1; i < tiers; i++)
                        {
                            foreach (CSequence name in Model.Sequences)
                            {
                                CSequence sequence = new CSequence(Model);
                                sequence.IntervalStart = CurrentLocation;
                                sequence.IntervalEnd = CurrentLocation + Duration;
                                sequence.Name = name + " " + tierNames[i];
                                CurrentLocation += Duration + 1;
                                Model.Sequences.Add(sequence);
                            }
                        }
                    }
                    if (alternate)
                    {
                        foreach (CSequence name in Model.Sequences.ToList())
                        {
                            CSequence sequence = new CSequence(Model);
                            sequence.IntervalStart = CurrentLocation;
                            sequence.IntervalEnd = CurrentLocation + Duration;
                            sequence.Name = name.Name + " " + "Alternate";
                            CurrentLocation += Duration + 1;
                            Model.Sequences.Add(sequence);
                        }
                        CSequence a1 = new CSequence(Model);
                        a1.IntervalStart = CurrentLocation;
                        a1.IntervalEnd = CurrentLocation + Duration;
                        a1.Name = "Morph";
                        CurrentLocation += Duration + 1;
                        Model.Sequences.Add(a1);
                        CSequence a2 = new CSequence(Model);
                        a2.IntervalStart = CurrentLocation;
                        a2.IntervalEnd = CurrentLocation + Duration;
                        a2.Name = "Morph Alternate";
                        CurrentLocation += Duration + 1;
                        Model.Sequences.Add(a2);
                    }
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Invalid input for durataion. Integer greater than 100");
                }
            }
            else
            {
                MessageBox.Show("Invalid input for durataion. Integer greater than 100");
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) DialogResult = false;
            if (e.Key == Key.Enter) ok(null, null);
        }
    }
}
