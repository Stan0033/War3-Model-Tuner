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
        
    public partial class MergeHelperBoneOptons : Window
    {
        private INode Parent, Child;
        List<INode> All;
        private bool ForAll;
        private bool AndNested;
        CModel Owner;
        public MergeHelperBoneOptons(INode p, INode c,CModel owner, bool andNested = false)
        {
            InitializeComponent();
            Parent = p;
            Child = c;
            Owner = 
                owner;
            ForAll = false;
            AndNested = andNested;
        }
        public MergeHelperBoneOptons( CModel owner )
        {
            InitializeComponent();
            
            Owner =  owner;
            ForAll = true;
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            if (!ForAll)
            {
                MergeHelperWithChild(Parent, Child, AndNested);
            }
            else
            {
               // while (HasPairs())
               // {
                    for (int i = 0; i < Owner.Nodes.Count; i++)
                    {
                        INode loopedParent = Owner.Nodes[i];
                        INode child = GetOnlyBoneChildOfNode(loopedParent);
                        if (child == null) continue;

                        MergeHelperWithChild(loopedParent, child, false);
                    }
               // }
               
            }
            DialogResult = true;
        }
        private bool HasPairs()
        {
            foreach (var node in Owner.Nodes)
            {

                if (node is CHelper)
                {
                    var children = GetChldrenOfNode(node);
                    if (children.Count == 1)
                    {
                        if (children[0] is CBone) { return true; }
                    }
                }
            }
            return false;
        }
        private void MergeHelperWithChild(INode parentNode, INode childNode, bool andNested)
        {
            // if the parentnode is helper
            if (parentNode is CHelper == false) { return; }
            if (childNode is CBone == false) {    return; }
            INode parentOfParent = parentNode.Parent.Node;
            var childrenOfChild = GetChildrenOfChild(childNode);
            
            

            CBone newBone = new CBone(Owner);
            newBone.Name = parentNode.Name;
            newBone.PivotPoint = new MdxLib.Primitives.CVector3(parentNode.PivotPoint);
            CBone chldBone = childNode as CBone;
            if (chldBone.Geoset.Object != null) { newBone.Geoset.Attach(chldBone.Geoset.Object); }
            if (chldBone.GeosetAnimation.Object != null) { newBone.GeosetAnimation.Attach(chldBone.GeosetAnimation.Object); }
            // Attach newBone correctly in the hierarchy
            if (parentOfParent != null) newBone.Parent.Attach(parentOfParent);
            // attach all chldren of the parent to the parent that had changed type
            ReattachGeosetsUsingBoneToNewBone(childNode, newBone); // child is bone with vertices, parent is new bone



            // Handle different options
            if (c1.IsChecked == true) // Erase
            {
                // No need to transfer keyframes, newBone replaces the parent
            }
            else if (c2.IsChecked == true) // Preserve Parent
            {
                CopyKeyframes(parentNode, newBone);
            }
            else if (c3.IsChecked == true) // Preserve Child
            {
                CopyKeyframes(childNode, newBone);
            }
            else if (c4.IsChecked == true) // Preserve Bigger
            {
                CopyBiggerKeyframes(parentNode, childNode, newBone);
            }

            Owner.Nodes.Remove(childNode);

            // Attach all children of the old child to the newBone
            foreach (INode child in childrenOfChild)
            {
                child.Parent?.Attach(newBone);
            }
            Owner.Nodes.Remove(parentNode);
            Owner.Nodes.Remove(childNode);
            Owner.Nodes.Add(newBone);
                if (AndNested)
                {
                    if (childrenOfChild.Count == 1)
                    {
                        MergeHelperWithChild(newBone, childrenOfChild[0], andNested);
                    }
                }
            
            if (AndNested)
            {
                if (childrenOfChild.Count == 1)
                {
                    MergeHelperWithChild(childNode, childrenOfChild[0], andNested);
                }
            }
        }

        private List<INode> GetChildrenOfChild(INode childNode)
        {
            List < INode > list = new List<INode>();
            foreach (var node in Owner.Nodes)
            {
                 
                if (node.Parent.Node == childNode) {list.Add(node);}
            }
            return list;
        }

        private void CopyKeyframes(INode source, CBone target)
        {
            for (int i = 0; i < source.Translation.Count; i++)  target.Translation.Add(new MdxLib.Animator.CAnimatorNode<MdxLib.Primitives.CVector3>(source.Translation[i]));
            for (int i = 0; i < source.Rotation.Count; i++)  target.Rotation.Add(new MdxLib.Animator.CAnimatorNode<MdxLib.Primitives.CVector4>(source.Rotation[i]));
            for (int i = 0; i < source.Scaling.Count; i++)  target.Scaling.Add(new MdxLib.Animator.CAnimatorNode<MdxLib.Primitives.CVector3>(source.Scaling[i]));
            
        }

        private void CopyBiggerKeyframes(INode parentNode, INode childNode, CBone newBone)
        {
            int parentCount = parentNode.Translation.NodeList.Count + parentNode.Rotation.NodeList.Count + parentNode.Scaling.NodeList.Count;
            int childCount = childNode.Translation.NodeList.Count + childNode.Rotation.NodeList.Count + childNode.Scaling.NodeList.Count;
            if (parentCount > childCount)
            {
                CopyKeyframes(parentNode, newBone);
            }
            else if (parentCount == childCount)
            {
                CopyKeyframes(parentNode, newBone);
            }
           else  if (parentCount < childCount)
            {
                CopyKeyframes(childNode, newBone);
            }
        }

        private INode GetOnlyBoneChildOfNode(INode parent)
        {
            
            foreach (var node in Owner.Nodes)
            {
                if (node.Parent.Node == parent)
                {
                    if (node is CBone)
                    {
                        if (CountChildrenOfNode(node) == 1) { return node; }
                    }
                }
            }
            return null;
        }
        private int CountChildrenOfNode(INode node)
        {
            int count = 0;
            foreach (var n in Owner.Nodes)
            {
                if (n.Parent.Node == node) count++;
            }
            return count;
        }
        private void ReattachGeosetsUsingBoneToNewBone(INode from, INode to)
        {
            foreach (var geoset in Owner.Geosets)
            {
                foreach (var group in geoset.Groups)
                {
                    foreach (var node in group.Nodes)
                    {
                        if (node.Node.Node == from)
                        {
                            node.Node.Detach();
                            node.Node.Attach(to);
                        }
                    }
                }
            }
        }

        private List<INode> GetChldrenOfNode(INode node)
        {
            List<INode> list = new();
            foreach (var n in Owner.Nodes)
            {
                if (n.Parent.Node == node) {  list.Add(n); }
            }
            return list;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; }
            if (e.Key == Key.Enter) { ok(null, null); }
        }
        
    }
}
