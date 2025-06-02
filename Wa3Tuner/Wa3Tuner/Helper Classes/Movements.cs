 
using MdxLib.Model;
 
using System.Collections.Generic;
 
using System.Numerics;
 
using System.Windows.Controls;
 

namespace Wa3Tuner.Helper_Classes
{
     public class MoveStep
    {
        public Vector3 Translation, Rotation, Scaling;
        public MoveStep(Vector3 t)
        {
            Translation = t;
            Rotation = new Vector3();
            Scaling = new Vector3();
        }
    }
    public static class Movements
    {
        public static ListBox ListBox { get; set; }
        public static List<MoveStep> Steps = new List<MoveStep>();
        public static void Clear () { Steps.Clear(); }
        public static void Add (Vector3 v) { Steps.Add(new MoveStep( v)); }
        public static void Add (MdxLib.Primitives.CVector3 v) {
           Vector3 c=  new Vector3(v.X, v.Y, v.Z);
            Steps.Add(new MoveStep( c)); 
        }
        public static void RemoveAt(int index) { Steps.RemoveAt(index); }
        public static void SetTranslation(INode node, int step)
        {
            node.PivotPoint.X = Steps[step].Translation.X;
            node.PivotPoint.Y = Steps[step].Translation.Y;
            node.PivotPoint.Z = Steps[step].Translation.Z;
        }
        public static void SetRotation(INode node, int step)
        {
            //unfinished
        }
        public static void SetScaling(INode node, int step)
        {
            //unfinished
        }
        public static void Step(INode node)
        {
            Add(node.PivotPoint);
            ListBox.Items.Add(new ListBoxItem() { Content = $"{Steps.Count}" });

        }
        public static void Peek(INode node, int step)
        {
            node.PivotPoint.X = Steps[step].Translation.X;
            node.PivotPoint.Y = Steps[step].Translation.Y;
            node.PivotPoint.Z = Steps[step].Translation.Z;
            // and translation
            // and scaling



        }
        public static void Finalize(INode node)
        {
            // unfinished

        }
         
         

         
    }
}
