using MdxLib.Animator;
using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Wa3Tuner.Helper_Classes
{
    public static class Animator
    {
        public static void ComputeNodePosition(INode node, CModel model, int WhichKeyframe)
        {
            if (WhichKeyframe < 0) return;

            // A node is transformed with this logic:
            // translaton in space, rotation based on parent (if any), scaling relative to parent (if any)
            // first apply scaling, then rotation, then translation

            // first set animated position to position, to avoid messed up calculations
            node.AnimatedPivotPoint = new CVector3(node.PivotPoint);
            // Get local translation at this keyframe. If it doesnt contain a keyframe, assign the current position.
            CVector3 localTranslation = GetKeyframeValue(node.Translation.NodeList, WhichKeyframe, node.AnimatedPivotPoint);

            // Start with the local translation. 
            node.AnimatedPivotPoint = localTranslation;


            // If the node has a parent, accumulate its translation, rotation and scaling recursively
            if (node.Parent?.Node != null)
            {
                ComputeNodePositionBasedOnNode(node, node.Parent.Node, WhichKeyframe);
            }
        }


        private static void ComputeNodePositionBasedOnNode(INode childNode, INode parentNode, int WhichKeyframe)
        {
            if (parentNode.DontInheritScaling && parentNode.DontInheritRotation && parentNode.DontInheritTranslation) return;
            if (!parentNode.DontInheritTranslation)
            {
                CVector3 local = GetKeyframeValue(parentNode.Translation.NodeList, WhichKeyframe, new CVector3());
                childNode.AnimatedPivotPoint += local;
            }
            if (!parentNode.DontInheritRotation)
            {
                CVector4 local = GetKeyframeValue(parentNode.Rotation.NodeList, WhichKeyframe, new CVector4(0, 0, 0, 1));
                CVector3 euler = Calculator.QuaternionToEuler(local);

                CVector3 rotated = Calculator.RotateVector(childNode.AnimatedPivotPoint, parentNode.PivotPoint, euler);

                childNode.AnimatedPivotPoint = new CVector3(rotated);

            }
            if (!parentNode.DontInheritScaling)
            {
                CVector3 local = GetKeyframeValue(parentNode.Scaling.NodeList, WhichKeyframe, new CVector3(1, 1, 1));
                childNode.AnimatedPivotPoint *= local;
            }
            // Continue up the hierarchy if there is another parent
            if (parentNode.Parent?.Node != null)
            {
                ComputeNodePositionBasedOnNode(childNode, parentNode.Parent.Node, WhichKeyframe);
            }
        }

        /// <summary>
        /// Retrieves the keyframe value at a specific time, or returns a default value if no keyframe is found.
        /// </summary>
        private static CVector3 GetKeyframeValue(List<CAnimatorNode<CVector3>> keyframes, int keyframeTime, CVector3 defaultValue)
        {
            foreach (var kf in keyframes)
            {
                if (kf.Time == keyframeTime)
                    return kf.Value;
            }
            return defaultValue;
        }
        /// <summary>
        /// Retrieves the keyframe value at a specific time, or returns a default value if no keyframe is found.
        /// </summary>
        private static CVector4 GetKeyframeValue(List<CAnimatorNode<CVector4>> keyframes, int keyframeTime, CVector4 defaultValue)
        {
            foreach (var kf in keyframes)
            {
                if (kf.Time == keyframeTime)
                    return kf.Value;
            }
            return defaultValue;
        }

        //---------------------------------------------------------------------------------
        internal static void ComputeAnimatedVertexPositions(CModel owner, int WhichKeyframe)
        {
            // first make all animated positions the same as the the vertex positions to prepare the for changing by a node
            foreach (var geoset in owner.Geosets)
            {
                foreach (var vertex in geoset.Vertices)
                {
                    vertex.AnimatedPosition = new CVector3(vertex.Position);
                }
            }
            foreach (var geoset in owner.Geosets)
            {
                foreach (var vertex in geoset.Vertices)
                {
                    // prepare vectors based on which the vertex will be changed

                    CVector3 finalScaledPosition = new CVector3(1,1, 1);
                    CVector4 finalRotatedPosition = new CVector4(0, 0, 0, 1); // Identity quaternion
                    CVector3 finalTranslatedPosition = new CVector3(vertex.Position);
                    // for each node to which the vertex is attached to:
                    foreach (var groupNode in vertex.Group.Object.Nodes)
                    {

                        INode targetNode = groupNode.Node.Node;


                        // Apply transformations from all ancestors by setting finalScaling, finalRotation, finalTranslation
                        ApplyParentTransformationsRecursive(targetNode, WhichKeyframe,
                            ref finalScaledPosition, ref finalRotatedPosition, ref finalTranslatedPosition,
                            vertex);
                        CurrentLoopDepth = 0;
                        // Apply accumulated transformations in the correct order

                    }
                    // after all calculations based on the vertex's nodes influence, calculate the final animated postion of the vertex:
                    CalculateFinalAnimatedPosition(vertex, finalScaledPosition, finalRotatedPosition, finalTranslatedPosition);

                }
            }
        }

        private static void CalculateFinalAnimatedPosition(CGeosetVertex vertex, CVector3 finalScaling, CVector4 finalRotation, CVector3 finalTranslation)
        {
            // Rotate first
            vertex.AnimatedPosition = RotateVectorByQuaternion(vertex.AnimatedPosition, finalRotation);

            // Then scale
            vertex.AnimatedPosition *= finalScaling;

            // Then translate
            vertex.AnimatedPosition += finalTranslation;
        }

        private static CVector3 RotateVectorByQuaternion(CVector3 v, CVector4 q)
        {
            // Convert vector to quaternion (0, x, y, z)
            CVector4 vQuat = new CVector4(v.X, v.Y, v.Z, 0);

            // Compute q * v * q^-1
            CVector4 qConjugate = new CVector4(-q.X, -q.Y, -q.Z, q.W); // Conjugate of q
            CVector4 rotatedQuat = MultiplyQuaternions(MultiplyQuaternions(q, vQuat), qConjugate);

            // Return the rotated vector part
            return new CVector3(rotatedQuat.X, rotatedQuat.Y, rotatedQuat.Z);
        }
        private static CVector4 MultiplyQuaternions(CVector4 q1, CVector4 q2)
        {
            float w1 = q1.W, x1 = q1.X, y1 = q1.Y, z1 = q1.Z;
            float w2 = q2.W, x2 = q2.X, y2 = q2.Y, z2 = q2.Z;

            return new CVector4(
                w1 * x2 + x1 * w2 + y1 * z2 - z1 * y2,  // X
                w1 * y2 - x1 * z2 + y1 * w2 + z1 * x2,  // Y
                w1 * z2 + x1 * y2 - y1 * x2 + z1 * w2,  // Z
                w1 * w2 - x1 * x2 - y1 * y2 - z1 * z2   // W
            );
        }


        private static CVector3 OffsetPosition(CVector3 animatedPosition, CVector3 finalTranslation)
        {
            return new CVector3(
                animatedPosition.X + finalTranslation.X,
                animatedPosition.Y + finalTranslation.Y,
                animatedPosition.Z + finalTranslation.Z
                );
        }

        //---------------------------------------------------------------------------------

        private static int CurrentLoopDepth = 0;
        private static int maxLoopDepth = 100;
        /// <summary>
        /// Recursively accumulates transformations from the node and its parents.
        /// </summary>
        private static void ApplyParentTransformationsRecursive(INode node, int WhichKeyframe,
            ref CVector3 scaling, ref CVector4 rotation, ref CVector3 translation, CGeosetVertex vertex
 )
        {

            if (node == null) return;
            if (CurrentLoopDepth == maxLoopDepth) { MessageBox.Show("Too great recursive function loop detected. Terminated. Check if two nodes are mutually referencencng each other."); return; }
            CurrentLoopDepth++;
            bool inheritTranslation = !node.DontInheritTranslation;
            bool inheritRotation = !node.DontInheritRotation;
            bool inheritScalng = !node.DontInheritScaling;
            // if the node does no inherit any transformations, we can stop calculating influence by that node
            if (!inheritTranslation && !inheritRotation && !inheritScalng) { return; }
            if (inheritTranslation)
            {
                var firstKeyframe = node.Translation.FirstOrDefault(x => x.Time == WhichKeyframe);
                if (firstKeyframe != null)
                {
                    translation = OffsetPosition(translation, firstKeyframe.Value);
                }
            }
            if (inheritRotation)
            {
                var firstKeyframe = node.Rotation.FirstOrDefault(x => x.Time == WhichKeyframe);
                if (firstKeyframe != null)
                {
                    rotation = Calculator.ComputeQuaternionChange(rotation, firstKeyframe.Value);
                }
            }
            if (inheritScalng)
            {
                var firstKeyframe = node.Scaling.FirstOrDefault(x => x.Time == WhichKeyframe);
                if (firstKeyframe != null)
                {
                    scaling.X *= firstKeyframe.Value.X;
                    scaling.Y *= firstKeyframe.Value.Y;
                    scaling.Z *= firstKeyframe.Value.Z;
                }
            }


            //go to next parent in the hierarchy
            ApplyParentTransformationsRecursive(node.Parent.Node, WhichKeyframe, ref scaling, ref rotation, ref translation, vertex);


        }
        //----------------------------------------------------------------------------
        internal static void ComputeGeosetVisibilities(CModel currentModel, int track)
        {
            CSequence? seq = GetSequenceOfTrack(track, currentModel);
            if (seq == null) return;
           
            foreach (var geoset in currentModel.Geosets)
            {
                geoset.isVisibleAnimated = true;
               
                var ga = currentModel.GeosetAnimations.First(x => x.Geoset.Object == geoset);
                if (ga != null)
                {
                    if (ga.Alpha.Static)
                    {
                        float value = ga.Alpha.GetValue();
                        geoset.isVisibleAnimated = value > 0;
                    }
                    else
                    {
                        var first = ga.Alpha.FirstOrDefault(x => x.Time == track);
                        if (first != null)
                        {
                            geoset.isVisibleAnimated = first.Value > 0;
                        }
                        else
                        {
                            if (seq != null)
                            {
                                var firstOther = ga.Alpha.FirstOrDefault(x => track >= seq.IntervalStart && track <= seq.IntervalEnd);
                                if (firstOther != null)
                                {
                                    geoset.isVisibleAnimated = firstOther.Value > 0;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static CSequence? GetSequenceOfTrack(int track, CModel m)
        {
            return m.Sequences.FirstOrDefault(x => track >= x.IntervalStart && track <= x.IntervalEnd);
        }

        internal static void ComputeNodeVisibilities(CModel currentModel, int track)
        {
            foreach (var node in currentModel.Nodes)
            {

                if (node is CParticleEmitter e)
                {
                    node.IsVisibleAnimated = IsVisibleInTrack(e.Visibility, track);
                   
                }
                if (node is CParticleEmitter2 e2)
                {
                    node.IsVisibleAnimated = IsVisibleInTrack(e2.Visibility, track);
                }
                if (node is CRibbonEmitter r)
                {
                    node.IsVisibleAnimated = IsVisibleInTrack(r.Visibility, track);
                }
                if (node is CAttachment a)
                {
                    node.IsVisibleAnimated = IsVisibleInTrack(a.Visibility, track);
                }
                if (node is CLight l)
                {
                    node.IsVisibleAnimated = IsVisibleInTrack(l.Visibility, track);
                }
            }
        }

        private static bool IsVisibleInTrack(CAnimator<float> visibility, int track)
        {
            if (visibility.Static)
            {
                float vis = visibility.GetValue();
                return vis > 0f;
            }
            else
            {
                if (visibility.Count==0)return true;
                var first = visibility.FirstOrDefault(x => x.Time == track);
                if (first != null)
                {
                    return first.Value > 0f;
                }

                return true;
            }
        }
    }
}