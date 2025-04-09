using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for RawKeyframeRearranger.xaml
    /// </summary>
    public partial class RawKeyframeRearranger : Window
    {
        public RawKeyframeRearranger()
        {
            InitializeComponent();
        }

        private void rearrange(object sender, RoutedEventArgs e)
        {
            string text = MainTextBox.Text;
            StringBuilder updated = new StringBuilder();
            MainTextBox.Text = updated.ToString();
            var list = ExtractNumbers(text);
            list.RemoveAll(x => x.Count == 0);
             list.RemoveAll(x => x.Count > 5);
            bool sameCount = AllListsHaveSameCount(list);
             bool bezier = ListsFollowPattern(list);

            if (!sameCount  && !bezier) { MessageBox.Show("The given string is not valid list of keyframes"); return; }
            if (sameCount)
            {
                list = SortByFirstElement(list);
                if (list[0].Count == 2)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in list)
                    {
                        sb.AppendLine($"{item[0]}: {item[1]}");
                    }
                    MainTextBox.Text = sb.ToString();
                }
                if (list[0].Count == 4)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in list)
                    {
                        sb.AppendLine($"{item[0]}: {{ {item[1]}, {item[2]}, {item[3]} }}  ");
                    }
                    MainTextBox.Text = sb.ToString();
                }

                if (list[0].Count == 5)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in list)
                    {
                        sb.AppendLine($"{item[0]}: {{ {item[1]}, {item[2]}, {item[3]}, {item[4]} }}  ");
                    }
                    MainTextBox.Text = sb.ToString();
                }
            }
            else
            {
                
                if (bezier)
                {
                    var kfs = collectKFs(list);
                    kfs = kfs.OrderBy(x=>x.time).ToList();
                    StringBuilder sb = new StringBuilder();

                    if (list[0].Count == 2)
                    {
                        foreach (kf kf in kfs)
                        {
                            sb.AppendLine($"{kf.time}: {kf.data.X}");
                            sb.AppendLine($"  InTan: {kf.intan.X}");
                            sb.AppendLine($"  OutTan: {kf.outtan.X}");
                        }
                    }
                    if (list[0].Count == 4)
                    {
                        foreach (kf kf in kfs)
                        {
                            sb.AppendLine($"{kf.time}: {kf.data.X}, {kf.data.Y}, {kf.data.Z}");
                            sb.AppendLine($"  InTan: {kf.intan.X}, {kf.intan.Y}, {kf.intan.Z}");
                            sb.AppendLine($"  OutTan: {kf.outtan.X}, {kf.outtan.Y}, {kf.outtan.Z}");
                        }
                    }
                    if (list[0].Count == 5)
                    {
                        foreach (kf kf in kfs)
                        {
                            sb.AppendLine($"{kf.time}: {kf.data.X}, {kf.data.Y}, {kf.data.Z}, {kf.data.W}");
                            sb.AppendLine($"  InTan: {kf.intan.X}, {kf.intan.Y}, {kf.intan.Z}, {kf.intan.W}");
                            sb.AppendLine($"  OutTan: {kf.outtan.X}, {kf.outtan.Y}, {kf.outtan.Z}, {kf.outtan.W}");
                        }
                    }
                    MainTextBox.Text = sb.ToString();
                       
                    
                    
                    
                     
                     
                }
                else
                {
                    MessageBox.Show("The given string is not valid list of keyframes"); return;
                }
            }

        }

        public   List<List<float>> SortByFirstElement(List<List<float>> lists)
        {
            return lists.OrderBy(list => list.Count > 0 ? list[0] : float.MaxValue).ToList();
        }

      

        public static bool ListsFollowPattern(List<List<float>> list)
        {
            if (list.Count < 3) return false; // At least 3 lists are needed for a valid pattern

            int n = list[0].Count; // Get the count of the first list

            for (int i = 0; i + 2 < list.Count; i += 3)
            {
                if (list[i].Count != n) return false;       // First in triplet must have N elements
                if (list[i + 1].Count != n - 1) return false; // Second must have N-1
                if (list[i + 2].Count != n - 1) return false; // Third must have N-1
            }

            return true;
        }

        public static bool AllListsHaveSameCount(List<List<float>> lists)
        {
            if (lists.Count == 0)
                return true; // Empty list is considered valid

            int expectedCount = lists[0].Count;

            foreach (var list in lists)
            {
                if (list.Count != expectedCount)
                    return false;
            }

            return true;
        }
        List<kf> collectKFs(List<List<float>> ints)
        {
            List<kf> list = new List<kf>();

            for (int i = 0; i < ints.Count; i += 3)
            {
                kf kf = new kf();
                kf.time = ints[i][0];
                if (ints[i].Count == 2)
                {
                    kf.data = new Vector4(ints[i][1], 0, 0, 0);
                    kf.intan = new Vector4(ints[i + 1][0], 0, 0, 0);
                    kf.outtan = new Vector4(ints[i + 2][0], 0, 0, 0);
                }
                if (ints[i].Count == 4)
                {
                    kf.data = new Vector4(ints[i][1], ints[i][2], ints[i][3], 0);
                    kf.intan = new Vector4(ints[i + 1][0], ints[i + 1][1], ints[i + 1][2], 0);
                    kf.outtan = new Vector4(ints[i + 2][0], ints[i + 2][1], ints[i + 2][2], 0);

                    if (ints[i].Count == 5)
                    {
                        kf.data = new Vector4(ints[i][1], ints[i][2], ints[i][3], ints[i][4]);
                        kf.intan = new Vector4(ints[i + 1][0], ints[i + 1][1], ints[i + 1][2], ints[i + 1][3]);
                    }
                }
                list.Add(kf);
              
            }
            return list;
        }

        public static List<List<float>> ExtractNumbers(string input)
        {
            var result = new List<List<float>>();
            var lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var numberRegex = new Regex(@"-?\d+(\.\d+)?", RegexOptions.Compiled);

            foreach (var line in lines)
            {
                var numbers = new List<float>();
                foreach (Match match in numberRegex.Matches(line))
                {
                    if (float.TryParse(match.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float number))
                    {
                        numbers.Add(number);
                    }
                }
                result.Add(numbers);
            }

            return result;
        }

        private void copy(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(MainTextBox.Text);
        }
    }
    public class kf
    {
        public float time;
        public Vector4 data, intan, outtan;
    }
}
