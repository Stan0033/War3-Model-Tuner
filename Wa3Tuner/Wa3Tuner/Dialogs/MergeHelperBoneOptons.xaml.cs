using MdxLib.Model;
 
using System.Collections.Generic;
using System.Linq;
using System.Windows;
 
using System.Windows.Input;
 

namespace Wa3Tuner.Dialogs
{
        
    public partial class MergeHelperBoneOptons : Window
    {
        private INode? ParentNode, ChildNode;
       
        private bool ForAll;
        private bool AndNested;
        readonly CModel? OwnerModel;
        public MergeHelperBoneOptons(INode p, INode c,CModel owner, bool andNested = false)
        {
            InitializeComponent();
            ParentNode = p;
            ChildNode = c;
            OwnerModel = 
                owner;
            ForAll = false;
            AndNested = andNested;
        }
        public MergeHelperBoneOptons( CModel owner )
        {
            InitializeComponent();
            
            OwnerModel =  owner;
            ForAll = true;
        }

        private void ok(object? sender, RoutedEventArgs? e)
        {
            if (!ForAll)
            {
                MergeHelperWithChild(ParentNode, ChildNode, AndNested);
            }
            else
            {
                if (OwnerModel == null) return;
                bool mergedAny;
                do
                {
                    mergedAny = false;
                    List<(INode Parent, INode Child)> mergePairs = new();

                    foreach (INode parent in OwnerModel.Nodes)
                    {
                        if (parent is not CHelper) continue;

                        // Find children of the helper node
                        var children = OwnerModel.Nodes.Where(n => n.Parent?.Node == parent).ToList();
                        if (children.Count != 1) continue;

                        var child = children[0];
                        if (child is CBone)
                        {
                            mergePairs.Add((parent, child));
                        }
                    }

                    foreach (var (parent, child) in mergePairs)
                    {
                        MergeHelperWithChild(parent, child, false);
                        mergedAny = true;
                    }

                } while (mergedAny);




            }

            DialogResult = true;
        }


        private void MergeHelperWithChild(INode? parentNode, INode? childNode, bool andNested)
            {
                if (parentNode == null) return; 
                if (childNode == null) return;
                if (OwnerModel == null) return;
                // if the parentnode is helper
                if (parentNode is CHelper == false) { return; }
                if (childNode is CBone == false) {    return; }
                INode parentOfParent = parentNode.Parent.Node;
                var childrenOfChild = GetChildrenOfChild(childNode);
            
            

                CBone newBone = new CBone(OwnerModel);
                newBone.Name = parentNode.Name;
                newBone.PivotPoint = new MdxLib.Primitives.CVector3(parentNode.PivotPoint);
                CBone? chldBone = childNode as CBone;
                if (chldBone == null) {return; }
            if (chldBone.Geoset.Object != null)
            {
                newBone.Geoset.Attach(chldBone.Geoset.Object);
            }
            if (chldBone.GeosetAnimation.Object != null)
            {
                newBone.GeosetAnimation.Attach(chldBone.GeosetAnimation.Object);
            }

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

                OwnerModel.Nodes.Remove(childNode);

                // Attach all children of the old child to the newBone
                foreach (INode child in childrenOfChild)
                {
                    child.Parent?.Attach(newBone);
                }
                OwnerModel.Nodes.Remove(parentNode);
                OwnerModel.Nodes.Remove(childNode);
                OwnerModel.Nodes.Add(newBone);
            if (AndNested)
            {
                foreach (var grandChild in GetChildrenOfChild(newBone))
                {
                    if (newBone is CBone && grandChild is CBone && OwnerModel.Nodes.Contains(grandChild))
                    {
                        MergeHelperWithChild(newBone, grandChild, andNested);
                    }
                }
            }


        }

        private List<INode> GetChildrenOfChild(INode childNode)
        {
            if (OwnerModel == null) return new List<INode>();
            List < INode > list = new List<INode>();
            foreach (var node in OwnerModel.Nodes)
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

        private INode? GetOnlyBoneChildOfNode(INode parent)
        {
            
            if (OwnerModel == null) return null;
            foreach (var node in OwnerModel.Nodes)
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
            if (OwnerModel==null)return count;
            foreach (var n in OwnerModel.Nodes)
            {
                if (n.Parent.Node == node) count++;
            }
            return count;
        }
        private void ReattachGeosetsUsingBoneToNewBone(INode from, INode to)
        {
            if (OwnerModel == null) return;
            foreach (var geoset in OwnerModel.Geosets)
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
            if (OwnerModel==null) return list;
            foreach (var n in OwnerModel.Nodes)
            {
                if (n.Parent.Node == node) {  list.Add(n); }
            }
            return list;
        }
        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; }
            if (e.Key == Key.Enter) { ok(null, null); }
        }
        
    }
}
