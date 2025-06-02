using MdxLib.Animator;
using MdxLib.Model;
using SharpGL.SceneGraph;
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
    /// Interaction logic for Duplcate_transforation_s_sequence__keyframes_to_other_sequences.xaml
    /// </summary>
    public partial class SequenceToSequencesSelector : Window
    {
        List<CSequence> Sequences = new List<CSequence> ();
        List<CGeosetAnimation> GeosetAnimations = new();
        List<Ttrack> Tracks = new List<Ttrack>();
        public SequenceToSequencesSelector(List<CSequence> s, List<Ttrack> tracks)
        {
            InitializeComponent();
            Sequences = s;
            Tracks = tracks;
            Fill();
        }
        public SequenceToSequencesSelector(List<CSequence> s, List<CGeosetAnimation> gas)
        {
            InitializeComponent();
            Sequences = s;
            GeosetAnimations = gas;
            Fill();
            ButtonOk.Content = "Copy";
        }
        private void Fill()
        {
            foreach (var s in Sequences)
            {
                list1.Items.Add(new ListBoxItem() { Content = $"{s.Name} [{s.IntervalStart} - {s.IntervalEnd}]" });
                list2.Items.Add(new ListBoxItem() { Content = $"{s.Name} [{s.IntervalStart} - {s.IntervalEnd}]" });
            }
            list1.SelectedIndex = 0;
        }

        private void CopySequenceKeyframesToSequences(object? sender, RoutedEventArgs? e)
        {

            List<int> indexes = GetSelectedSequences();

            if (indexes.Count == 0)
            {
                MessageBox.Show("Select at least one from the second list");
                return;
            }
            if (indexes.Contains(list1.SelectedIndex))
            {
                MessageBox.Show("A sequence selected in the first list cannot be selected also in the second list");
                return;
            }


            var copiedSequence = Sequences[list1.SelectedIndex];

            if (GeosetAnimations == null)
            {
                DealInitial(copiedSequence, indexes);
            }
            else
            {
                DealGeosetVisibilities(copiedSequence, indexes);
            }
        }
        private void DealGeosetVisibilities(CSequence copiedSequence, List<int> indexes)
        {
            foreach (var ga in GeosetAnimations)
            {
                if (!ga.Alpha.Animated)
                    continue;

                var isolated1 = ga.Alpha
                    .Where(x => x.Time >= copiedSequence.IntervalStart && x.Time < copiedSequence.IntervalEnd)
                    .ToList();

                if (isolated1.Count == 0)
                    continue;

                foreach (int index in indexes)
                {
                    if (index == Sequences.IndexOf(copiedSequence))
                        continue;

                    CSequence targetSequence = Sequences[index];
                    CopyGosetAnimationAlphas(copiedSequence, targetSequence, isolated1, ga.Alpha);
                }
            }
        }
        private void CopyGosetAnimationAlphas(
    CSequence from,
    CSequence to,
    List<CAnimatorNode<float>> sourceKeyframes,
    CAnimator<float> targetAnimator)
        {
            int sourceStart = from.IntervalStart;
            int sourceEnd = from.IntervalEnd;
            int targetStart = to.IntervalStart;
            int targetEnd = to.IntervalEnd;

            int sourceDuration = sourceEnd - sourceStart;
            int targetDuration = targetEnd - targetStart;

            if (sourceDuration > targetDuration)
                return;

            float timeScale = (float)targetDuration / sourceDuration;

            foreach (var keyframe in sourceKeyframes)
            {
                int newTime = targetStart + (int)((keyframe.Time - sourceStart) * timeScale);

                var existing = targetAnimator.NodeList.FirstOrDefault(kf => kf.Time == newTime);
                if (existing != null)
                {
                    existing.Value = keyframe.Value;
                }
                else
                {
                    targetAnimator.NodeList.Add(new CAnimatorNode<float>(newTime, keyframe.Value));
                }
            }

            targetAnimator.NodeList = targetAnimator.NodeList.OrderBy(kf => kf.Time).ToList();
        }




        private void DealInitial(CSequence copiedSequence, List<int> indexes)
        {
            // Isolate keyframes from the copied sequence
            List<Ttrack> isolated = Tracks
                .Where(x => x.Time >= copiedSequence.IntervalStart && x.Time <= copiedSequence.IntervalEnd)
            .ToList();

            foreach (int index in indexes)
            {
                if (index == Sequences.IndexOf(copiedSequence)) { continue; }
                var targetSequence = Sequences[index];
                int from = targetSequence.IntervalStart;
                int to = targetSequence.IntervalEnd;

                // Clear existing keyframes in the target sequence
                Tracks.RemoveAll(x => x.Time >= from && x.Time <= to);

                // Paste copied keyframes with adjusted times
                foreach (var track in isolated)
                {
                    var copiedKeyframe = new Ttrack(track);
                    copiedKeyframe.Time = from + (track.Time - copiedSequence.IntervalStart); // Adjust time relative to the new sequence
                    Tracks.Add(copiedKeyframe);
                }
            }

            DialogResult = true;
        }

        private List<int> GetSelectedSequences()
        {
            List<int> list = new List<int>();
            foreach (var item in list2.SelectedItems)
            {
                list.Add(list2.Items.IndexOf(item));
            }
            return list;
        }
    }
}
