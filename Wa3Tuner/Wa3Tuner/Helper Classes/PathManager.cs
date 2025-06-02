using MdxLib.Animator;
using MdxLib.Animator.Animatable;
using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CVector3 = MdxLib.Primitives.CVector3;

namespace Wa3Tuner.Helper_Classes
{
    public static class PathManager
    {
        public static void MovePath(cPath whichPath, CVector3 To)
        {
            if (whichPath == null || whichPath.Count < 2) return;

            // Calculate the offset from the first node to the target position
            CVector3 offset = new CVector3(To.X - whichPath.List[0].Position.X,
                                           To.Y - whichPath.List[0].Position.Y,
                                           To.Z - whichPath.List[0].Position.Z);

            // Set the first node to the new position
            whichPath.List[0] = new PathNode(new CVector3(To));

            // Move the rest of the nodes by the same offset
            for (int i = 1; i < whichPath.List.Count; i++)
            {
                whichPath.List[i] = new PathNode(new CVector3(whichPath.List[i].Position.X + offset.X,
                                                 whichPath.List[i].Position.Y + offset.Y,
                                                 whichPath.List[i].Position.Z + offset.Z));
            }
        }



        public static List<cPath> Paths = new List<cPath>();
        internal static cPath? Selected;
        

        public static void AnimateAbsolute(INode Node, int From, int To, cPath WhichPath)
        {
            if (WhichPath.Count == 0) { MessageBox.Show("Empty path"); return; }
            // Overwrite keyframes  
            if ((To - From) < WhichPath.Count)
            {
                MessageBox.Show("Interval cannot fit all keyframes of the target path");
                return;
            }

            var translation = Node.Translation;
            List<int> equalTimes = GetEqualTimesForInterval(From, To, WhichPath.Count);

            // Remove existing keyframes within the interval
            translation.NodeList.RemoveAll(x => x.Time >= From && x.Time <= To);

            for (int i = 0; i < equalTimes.Count; i++)
            {
                CAnimatorNode<CVector3> kf = new CAnimatorNode<CVector3>
                {
                    Time = equalTimes[i],
                    Value = new CVector3(WhichPath.List[i].Position)  // First keyframe position (absolute)
                };
                translation.NodeList.Add(kf);
            }

           // Node.Translation.NodeList = translation.NodeList.OrderBy(x => x.Time).ToList();
             
            
            MessageBox.Show($"Created with {equalTimes.Count} keyframes for node {Node.Name} from {From} to {To}, in a node with {translation.NodeList.Count} total keyframes");
        }
             
            

           
             


        private static List<int> GetEqualTimesForInterval(int from, int to, int totalTracks)
        {
            List<int> list = new List<int>();

            if (totalTracks <= 0)
                return list;

            if (totalTracks == 1)
            {
                list.Add(from);
                return list;
            }

            double step = (to - from) / (double)(totalTracks - 1);

            for (int i = 0; i < totalTracks; i++)
            {
                list.Add((int)Math.Round(from + i * step));
            }

            return list;
        }


        public static void OffsetPath(cPath WhichPath, Vector3 By)
        {
            for (int i = 0; i < WhichPath.Count; i++)
            {
                WhichPath.List[i] = new PathNode(new CVector3(
                    WhichPath.List[i].Position.X + By.X,
                    WhichPath.List[i].Position.Y + By.Y,
                    WhichPath.List[i].Position.Z + By.Z
                ));
            }
        }
        public static void MovePath(cPath path, Axes ax, float value)
        {
            Calculator.TranslateVectors(path.List.Select(x => x.Position).ToList(), ax, value);
        }
        public static void RotatePath(cPath path, Axes ax, float value)
        {
            if (path == null) return;
            if (path.Count < 2) return;


        }
        public static void ScalePath(cPath path, Axes ax, float value)
        {
            if (path == null) return;
            if (path.Count < 2) return;

        }
        public static void Export(cPath path)
        {
            if (path.Count < 2)
            {
                MessageBox.Show("Too few nodes"); return;
            }
            string SelectedFileName = "";
            StringBuilder sb = new StringBuilder();
            foreach (var item in path.List)
            {
                sb.AppendLine($"{item.Position.X} {item.Position.Y} {item.Position.Z}");
            }
            SelectedFileName = FileSeeker.SavePathFile();
            // call dialog
            System.IO.Path.ChangeExtension(SelectedFileName, ".path");
            if (SelectedFileName.Length > 0)
            {
                System.IO.File.WriteAllText(SelectedFileName, sb.ToString());
            }

        }
        public static bool Import()
        {
            try
            {


                string? filename = FileSeeker.OpenPathFile();
                if (filename == null) return false;
                List<string> lines = File.ReadAllLines(filename).ToList();
                if (lines.Count == 0) return false;
                cPath path = new cPath( System.IO.Path.GetFileNameWithoutExtension(filename));
                foreach (var line in lines)
                {
                    string[] parts = line.Split(' ');
                    if (parts.Length == 3)
                    {
                        float one = float.Parse(parts[0]);
                        float two = float.Parse(parts[1]);
                        float three = float.Parse(parts[2]);
                        PathNode node = new PathNode(one, two, three);
                        path.Add(node);
                    }
                    else
                    {
                        break;
                    }


                }
                PathManager.Paths.Add(path);
                return true;
            }
            catch { return false; }
            
        }

        internal static void AnimateRelative(INode Node, int From, int To, cPath WhichPath)
        {
            if (WhichPath.Count == 0) { MessageBox.Show("Empty path"); return; }

            if ((To - From) < WhichPath.Count)
            {
                MessageBox.Show("Interval cannot fit all keyframes of the target path");
                return;
            }

            var translation = Node.Translation;
            List<int> equalTimes = GetEqualTimesForInterval(From, To, WhichPath.Count);

            // Remove existing keyframes within the interval
            translation.NodeList.RemoveAll(x => x.Time >= From && x.Time <= To);

            for (int i = 0; i < equalTimes.Count; i++)
            {
                CAnimatorNode<CVector3> kf = new CAnimatorNode<CVector3>
                {
                    Time = equalTimes[i],
                    Value = WhichPath.List[i].Position // Use absolute position instead of offset
                };
                translation.NodeList.Add(kf);
            }

            translation.NodeList = translation.NodeList.OrderBy(x => x.Time).ToList();
          
            MessageBox.Show($"Created with {equalTimes.Count} keyframes for node {Node.Name} from {From} to {To}");
        }

        private static CVector3 GetTranslationOffset(CVector3 original, CVector3 FitFor)
        {
            return new CVector3(
                FitFor.X - original.X,
                FitFor.Y - original.Y,
                FitFor.Z - original.Z
            );
        }

        internal static bool Extract(INode node, CSequence sequence)
        {
            if (node.Translation.Count == 0)
            {
                MessageBox.Show("This node has no translation keyframes"); return false;
            }
            if (node.Translation.Any(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd) == false)
            {
                MessageBox.Show("There are no keyframes for the selected sequence in this node's translation"); return false;
            }
            string name = "ExtractedPath_" + node.Name + "_" + IDCounter.Next_;
            cPath path = new cPath(name);
            foreach (var kf in node.Translation)
            {
                PathNode pn = new PathNode();
                pn.Position = new CVector3(kf.Value);
                path.Add(pn);
            }
            Paths.Add(path);

            return true; 
        }

        public class PathNode
        {
            public CVector3 Position = new CVector3();
            public bool IsSelected = false;
            public PathNode() { }
            public PathNode(CVector3 position)
            {
                Position = new CVector3(position);
            }
            public PathNode(float x, float y, float z)
            {
                Position = new CVector3(x, y, z);
            }
            public PathNode(PathNode p)
            {
                Position = new CVector3(p.Position);
            }

        }
        public class cPath
        {
            public string Name = "";
            public List<PathNode> List;

            public void Add(CVector3 v) { List.Add(new PathNode(v)); }
            public void Add(PathNode v) { List.Add(v); }
            public void Remove(PathNode v) { List.Remove(v); }
            public void Remove(float x, float y, float z) { List.Remove(new PathNode(x, y, z)); }
            public void RemoveAt(int index) { List.RemoveAt(index); }
            public void Clear() { List.Clear(); }
            public bool Contains(PathNode node) { return List.Contains(node); }
            public int Count => List.Count;
            public void Reverse() { List.Reverse(); }

            internal void Duplicate(int index)
            {
                var node = List[index];
                PathNode pnode = new PathNode(node);
                List.Add(pnode);
            }

            public cPath(string name) { Name = name; List = new List<PathNode>(); }
        }
    }
}
