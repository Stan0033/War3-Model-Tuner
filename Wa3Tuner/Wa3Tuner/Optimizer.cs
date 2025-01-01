using MdxLib.Animator;
using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Linq;


namespace Wa3Tuner
{
    public static class Optimizer
    {
        public static CModel Model;
        public static bool DeleteIsolatedTriangles = false;
        public static bool Delete0LengthSequences = false;
        public static bool Delete0LengthGlobalSequences = false;
        public static bool DeleteUnusedGlobalSequences = false;
        public static bool DeleteUnusedHelpers = false;
        public static bool DeleteUnusedMAterials = false;
        public static bool DeleteUnusedTextures = false;
        public static bool RenameAllComponents = false;
        public static bool ClampAllUVCoordinates = false;
        public static bool AddMissingPivotPoints = false;
        public static bool MakeVisibilitiesNone = false;
        public static bool AddOrigin = false;
        public static bool EnumerateSequences = false;

        public static bool Linearize = false;
        internal static bool DeleteIsolatedVertices = false;
        internal static bool DeleteUnAnimatedSequences = false;
        internal static bool DeleteUnusedBones = false;
       
        internal static bool DeleteUnusedTextureAnimations = false;
        internal static bool DeleteUnusedKeyframes = false;
        internal static bool MergeGeosets = false;
        internal static bool DelUnusedMatrixGroups = false;

        internal static bool AddMissingVisibilities = false;
        internal static bool ClampUVs = false;
        internal static bool CalculateExtents = false;
        internal static bool ClampLights = false;
        internal static bool AddMissingGAs = false;

        public static bool SetAllStaticGAS = false;
        internal static bool DeleteDuplicateGAs = false;
        internal static bool ClampKeyframes = false;
        internal static bool DeleteIdenticalAdjascentKEyframes = false;
        internal static bool AddMissingKeyframes;
        internal static bool DeleteSimilarSimilarKEyframes = false;
        internal static bool _DetachFromNonBone;
        internal static bool Check_DeleteIdenticalAdjascentKEyframes_times;
        internal static bool DleteOverlapping1 = false;
        internal static bool DleteOverlapping2 = false;
        internal static bool InvalidTriangleUses = false;
        internal static bool ClampNormals = false;
        internal static bool DeleteTrianglesWithNoArea;

        public static bool MergeIdenticalVertices { get; internal set; }

        public static void Optimize(CModel model_)
        {
            Model = model_;
            RearrangeSEquences();
            RearrangeKeyframes_();
            RemoveEmptyGeosets();
            FixInvalidNodeRelationships();
            CreateLayerForMaterialsWithout();
            if (Linearize) { Linearize_(); }
            if (DeleteIsolatedTriangles) { DeleteIsolatedTriangles_(); }
            if (DeleteIsolatedVertices) { DeleteIsolatedVertices_(); }
            if (Delete0LengthSequences) { Delete0LengthSequences_(); }
            if (Delete0LengthGlobalSequences) { Delete0LengthGlobalSequences_(); }
            if (DeleteUnusedGlobalSequences) { DeleteUnusedGlobalSequences_(); }
            if (DeleteUnusedHelpers) { DeleteUnusedHelpers_(); }
            if (DeleteUnusedMAterials) { DeleteUnusedMaterials_(); }
            if (DeleteUnusedTextures) { DeleteUnusedTextures_(); }
            if (RenameAllComponents) { RenameAllComponents_(); }
            if (ClampAllUVCoordinates) { ClampAllUVCoordinates_(); }
            if (AddMissingPivotPoints) { AddMissingPivotPoints_(); }
            if (MakeVisibilitiesNone) { MakeVisibilitiesNone_(); }
            if (EnumerateSequences) { EnumerateSequences_(); }
            if (AddOrigin) { AddOrigin_(); }
            if (DeleteIsolatedTriangles) { DeleteIsolatedTriangles_(); }
            if (DeleteUnAnimatedSequences) { DeleteUnAnimatedSequences_(); }
            if (DeleteUnusedBones) { DeleteUnusedBones_(); }

            if (DeleteUnusedTextureAnimations) { DeleteUnusedTextureAnimations_(); }
            if (ClampKeyframes) { ClampKeyframes_(); }
            if (DeleteUnusedKeyframes) { DeleteUnusedKeyframes_(); }
            if (MergeGeosets) { MergeGeosets_(); }
            if (AddMissingVisibilities) { AddMissingVisibilities_(); }
            if (ClampUVs) { ClampUVs_(); }
            if (ClampLights) { ClampLights_(); }
            if (DeleteDuplicateGAs) { DeleteDuplicateGAs_(); }
            if (AddMissingGAs) { AddMissingGAs_(); }
            if (SetAllStaticGAS) { SetAllStaticGAS_(); }
            
            if (CalculateExtents) { CalculateExtents_(); }
            if (DeleteIdenticalAdjascentKEyframes) { DeleteIdenticalAdjascentKEyframes_(); }
            if (Check_DeleteIdenticalAdjascentKEyframes_times) { Check_DeleteIdenticalAdjascentKEyframes_times_(); }
           // if (DeleteSimilarSimilarKEyframes) { DeleteSimilarSimilarKEyframes_(); }
            if (AddMissingKeyframes) { AddMissingKeyframes_(); }
            if (_DetachFromNonBone) { _DetachFromNonBone_(); }
            if (DleteOverlapping1) DeleteIdenticalFaces();
            if (DleteOverlapping2) DeleteFullyOverLappingFaces();
            if (InvalidTriangleUses) InvalidTriangleUses_();
            if (ClampNormals) ClampNormals_();
            if (DeleteTrianglesWithNoArea) DeleteTrianglesWithNoArea_();
            if (MergeIdenticalVertices) MergeIdenticalVertices_();




             RearrangeKeyframes_();
            MakeTransformationsWithZeroTracksStatic();
            FixQuirtOfEmitters2();
            DeleteEmptyGeosets();
        }
        private static void DeleteEmptyGeosets()
        {
            foreach (CGeoset geoset in Model.Geosets.ToList())
            {
                if (geoset.Faces.Count == 0 || geoset.Vertices.Count < 3)
                { 
                    Model.Geosets.Remove(geoset); 
                } 
            }

            
            }
        private static void MergeIdenticalVertices_()
        {
            foreach (CGeoset geoset in Model.Geosets)
            {
                var verticesToRemove = new List<CGeosetVertex>(); // List of vertices to remove

                foreach (CGeosetFace face in geoset.Faces)
                {
                    foreach (CGeosetVertex vertex in geoset.Vertices)
                    {
                        if (face.Vertex1.Object != vertex && !verticesToRemove.Contains(vertex))
                        {
                            if (face.Vertex1.Object.IdenticalWith(vertex))
                            {
                                verticesToRemove.Add(vertex);
                            }
                        }
                        if (face.Vertex2.Object != vertex && !verticesToRemove.Contains(vertex))
                        {
                            if (face.Vertex2.Object.IdenticalWith(vertex))
                            {
                                verticesToRemove.Add(vertex);
                            }
                        }
                        if (face.Vertex3.Object != vertex && !verticesToRemove.Contains(vertex))
                        {
                            if (face.Vertex3.Object.IdenticalWith(vertex))
                            {
                                verticesToRemove.Add(vertex);
                            }
                        }
                    }
                }

                // After the loop finishes, remove vertices
                foreach (var vertex in verticesToRemove)
                {
                    geoset.Vertices.Remove(vertex);
                }
            }
        }


        private static void DeleteTrianglesWithNoArea_()
        {
            foreach (CGeoset geoset in Model.Geosets.ToList())
            {
                foreach (CGeosetFace face in geoset.Faces.ToList())
                {
                    if (TriangleHasNoArea(face))
                    {
                        geoset.Faces.Remove(face); continue;
                    }
                    if (TriangleRepeatVertices(face))
                    {
                        geoset.Faces.Remove(face); continue;
                    }
                }
            }
        }

        private static bool TriangleRepeatVertices(CGeosetFace face)
        {
            if (
                face.Vertex1.Object == face.Vertex2.Object ||
                face.Vertex1.Object == face.Vertex3.Object ||
                face.Vertex2.Object == face.Vertex3.Object

                ) { return true; }
            return false;
        }

        private static bool TriangleHasNoArea(CGeosetFace face)
        {
            // Access the positions of the three vertices
            var v1 = face.Vertex1.Object.Position;
            var v2 = face.Vertex2.Object.Position;
            var v3 = face.Vertex3.Object.Position;

            // Calculate the edge vectors
            var edge1 = new CVector3(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z);
            var edge2 = new CVector3(v3.X - v1.X, v3.Y - v1.Y, v3.Z - v1.Z);

            // Compute the cross product of the two edges
            var crossProduct = new CVector3(
                edge1.Y * edge2.Z - edge1.Z * edge2.Y,
                edge1.Z * edge2.X - edge1.X * edge2.Z,
                edge1.X * edge2.Y - edge1.Y * edge2.X
            );

            // If the cross product is a zero vector, the triangle has no area
            return crossProduct.X == 0 && crossProduct.Y == 0 && crossProduct.Z == 0;
        }

        private static void ClampNormals_()
        {

            foreach (CGeoset geo in Model.Geosets)
            {
                foreach (CGeosetVertex vertex in geo.Vertices)
                {
                    if (
                        vertex.Normal.X < 0 || vertex.Normal.X > 1 ||
                        vertex.Normal.Y < 0 || vertex.Normal.Y > 1 ||
                        vertex.Normal.Z < 0 || vertex.Normal.Z > 1)
                    {
                        float x = vertex.Normal.X;
                        float y = vertex.Normal.Y;
                        float z = vertex.Normal.Z;
                        if (x < -1) x = -1;
                        if (x > 1) x = 1;
                        if (y < -1) y = -1;
                        if (y > 1) y = 1;
                        if (z < -1) z = -1;
                        if (z > 1) z = 1;
                        vertex.Normal = new CVector3(x, y, z);




                    }
                }
            }
        }
       

        private static void InvalidTriangleUses_()
        {
            foreach (CGeoset geoset in Model.Geosets)
            {
                // Use ToList() to avoid modifying the collection while iterating
                foreach (CGeosetFace face1 in geoset.Faces.ToList())
                {
                    if (
                        
                        !geoset.Vertices.Contains(face1.Vertex1.Object) ||
                        !geoset.Vertices.Contains(face1.Vertex2.Object) ||
                        !geoset.Vertices.Contains(face1.Vertex3.Object)

                        )
                    {
                        geoset.Faces.Remove(face1);
                    }

                    }
                }
            }
       

        private static void DeleteFullyOverLappingFaces()
        {
            foreach (CGeoset geoset in Model.Geosets)
            {
                // Use ToList() to avoid modifying the collection while iterating
                foreach (CGeosetFace face1 in geoset.Faces.ToList())
                {
                    foreach (CGeosetFace face2 in geoset.Faces.ToList())
                    {
                        // Skip comparison with itself
                        if (face1 == face2) { continue; }

                        // If faces are fully overlapping, remove the second one
                        if (FacesFullyOverlapping(face1, face2))
                        {
                            geoset.Faces.Remove(face2);
                        }
                    }
                }
            }
        }

        public static bool FacesFullyOverlapping(CGeosetFace face1, CGeosetFace face2)
        {
            // Combination 1: All conditions combined with AND
            if (face1.Vertex1 == face2.Vertex1 && face1.Vertex2 == face2.Vertex2 &&
                face1.Vertex1 == face2.Vertex2 && face1.Vertex2 == face2.Vertex1)
            {
                return true;
            }

            // Combination 2: All conditions combined with OR
            if (face1.Vertex1 == face2.Vertex1 || face1.Vertex2 == face2.Vertex2 ||
                face1.Vertex1 == face2.Vertex2 || face1.Vertex2 == face2.Vertex1)
            {
                return true;
            }

            // Combination 3: Mixed AND and OR (example: first two AND, rest OR)
            if ((face1.Vertex1 == face2.Vertex1 && face1.Vertex2 == face2.Vertex2) ||
                (face1.Vertex1 == face2.Vertex2 && face1.Vertex2 == face2.Vertex1))
            {
                return true;
            }

            // Combination 4: Mixed AND and OR (example: first two OR, rest AND)
            if ((face1.Vertex1 == face2.Vertex1 || face1.Vertex2 == face2.Vertex2) &&
                (face1.Vertex1 == face2.Vertex2 && face1.Vertex2 == face2.Vertex1))
            {
                return true;
            }

            // Combination 5: Another variation of mixed AND and OR
            if ((face1.Vertex1 == face2.Vertex1 && face1.Vertex2 == face2.Vertex1) ||
                (face1.Vertex1 == face2.Vertex2 && face1.Vertex2 == face2.Vertex2))
            {
                return true;
            }

            // Add more combinations as needed...

            // Default case
            return false;
        }





        private static bool VerticesAreInSamePosition(CGeosetVertex vertex1, CGeosetVertex vertex2)
        {
            // Check if the vertices are in the exact same position in 3D space
            return vertex1.Position.X == vertex2.Position.X &&
                   vertex1.Position.Y == vertex2.Position.Y &&
                   vertex1.Position.Z == vertex2.Position.Z;
        }


        private static void DeleteIdenticalFaces()
        {
            foreach (CGeoset geoset in Model.Geosets)
            {
                // Use ToList() to avoid modifying the collection while iterating
                foreach (CGeosetFace face1 in geoset.Faces.ToList())
                {
                    foreach (CGeosetFace face2 in geoset.Faces.ToList())
                    {
                        // Skip comparison with itself
                        if (face1 == face2) { continue; }

                        // Check if the faces share any vertices
                        if (ShareSameVertices(face1, face2))
                        {
                            // If they share vertices, remove the second face
                            geoset.Faces.Remove(face2);
                        }
                    }
                }
            }
        }

        private static bool ShareSameVertices(CGeosetFace face1, CGeosetFace face2)
        {
            // Compare all three vertices
            if (face1.Vertex1 == face2.Vertex1 || face1.Vertex2 == face2.Vertex1 || face1.Vertex3 == face2.Vertex1 ||
                face1.Vertex1 == face2.Vertex2 || face1.Vertex2 == face2.Vertex2 || face1.Vertex3 == face2.Vertex2 ||
                face1.Vertex1 == face2.Vertex3 || face1.Vertex2 == face2.Vertex3 || face1.Vertex3 == face2.Vertex3)
            {
                return true;
            }
            return false;
        }

        private static void Check_DeleteIdenticalAdjascentKEyframes_times_()
        {
            foreach (INode node in Model.Nodes)
            {
                RemoveAdjascentKeyframesTimes(node.Translation);
                RemoveAdjascentKeyframesTimes(node.Rotation);
                RemoveAdjascentKeyframesTimes(node.Scaling);
                if (node is CAttachment)
                {
                    CAttachment element = (CAttachment)node;
                    RemoveAdjascentKeyframesTimes(element.Visibility);
                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                    RemoveAdjascentKeyframesTimes(element.Visibility);
                    RemoveAdjascentKeyframesTimes(element.EmissionRate);
                    RemoveAdjascentKeyframesTimes(element.LifeSpan);
                    RemoveAdjascentKeyframesTimes(element.InitialVelocity);
                    RemoveAdjascentKeyframesTimes(element.Gravity);
                    RemoveAdjascentKeyframesTimes(element.Longitude);
                    RemoveAdjascentKeyframesTimes(element.Latitude);
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    RemoveAdjascentKeyframesTimes(element.Visibility);
                    RemoveAdjascentKeyframesTimes(element.EmissionRate);
                    RemoveAdjascentKeyframesTimes(element.Speed);
                    RemoveAdjascentKeyframesTimes(element.Width);
                    RemoveAdjascentKeyframesTimes(element.Gravity);
                    RemoveAdjascentKeyframesTimes(element.Length);
                    RemoveAdjascentKeyframesTimes(element.Latitude);
                    RemoveAdjascentKeyframesTimes(element.Variation);
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    RemoveAdjascentKeyframesTimes(element.Visibility);
                    RemoveAdjascentKeyframesTimes(element.HeightAbove);
                    RemoveAdjascentKeyframesTimes(element.HeightBelow);
                    RemoveAdjascentKeyframesTimes(element.Color);
                    RemoveAdjascentKeyframesTimes(element.Alpha);
                    RemoveAdjascentKeyframesTimes(element.TextureSlot);

                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;
                    RemoveAdjascentKeyframesTimes(element.Visibility);
                    RemoveAdjascentKeyframesTimes(element.Color);
                    RemoveAdjascentKeyframesTimes(element.AmbientColor);
                    RemoveAdjascentKeyframesTimes(element.Intensity);
                    RemoveAdjascentKeyframesTimes(element.AmbientIntensity);
                    RemoveAdjascentKeyframesTimes(element.AttenuationEnd);
                    RemoveAdjascentKeyframesTimes(element.AttenuationStart);

                }
            }
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {
                if (ga.Alpha.Static == false)
                {
                    RemoveAdjascentKeyframesTimes(ga.Alpha);
                }
                if (ga.Color.Static == false) { RemoveAdjascentKeyframes(ga.Color); }
            }
            foreach (CTextureAnimation taa in Model.TextureAnimations)
            {
                RemoveAdjascentKeyframesTimes(taa.Translation);
                RemoveAdjascentKeyframesTimes(taa.Rotation);
                RemoveAdjascentKeyframesTimes(taa.Scaling);

            }
            foreach (CMaterial material in Model.Materials)
            {
                foreach (CMaterialLayer layer in material.Layers)
                {
                    RemoveAdjascentKeyframesTimes(layer.Alpha);
                    RemoveAdjascentKeyframesTimes(layer.TextureId);
                }
            }
            foreach (CCamera cam in Model.Cameras)
            {
                RemoveAdjascentKeyframesTimes(cam.Rotation);
            }

        }

        private static void _DetachFromNonBone_()
        {
            INode dummy = new CBone(Model);
            dummy.Name = "DummyBone_FaultyVertices";
            int countFauls = 0;
            foreach (CGeoset geo in Model.Geosets)
            {
                foreach (var group in geo.Groups)
                {
                    foreach (var item in group.Nodes.ToList())
                    {
                        if (item.Node.Node is CBone == false)
                        {
                            group.Nodes.Remove(item);
                            if (group.Nodes.Count == 0)
                            {
                                countFauls++;
                                CGeosetGroupNode node = new CGeosetGroupNode(Model);
                                node.Node.Attach(dummy);
                                group.Nodes.Add(node);
                                break;
                            }
                        }
                    }
                }
            }
            if (countFauls > 0) { Model.Nodes.Add(dummy); }
            
        }

        private static void FixInvalidNodeRelationships()
        {
            foreach (INode node in Model.Nodes)
            {
                // Fix self-referencing
                if (node.Parent?.Node == node)
                {
                    node.Parent.Detach();
                    continue;
                }

                // Fix referencing invalid parent
                if (node.Parent?.Node != null && !Model.Nodes.Contains(node.Parent.Node))
                {
                    node.Parent.Detach();
                    continue;
                }

                // Fix mutually referencing nodes
                INode parent = node.Parent?.Node;
                if (parent?.Parent?.Node == node)
                {
                    node.Parent.Detach();
                    continue;
                }
            }
        }


        private static void DeleteSimilarSimilarKEyframes_()
        {
             //unused
           // throw new NotImplementedException();
        }
        private static void CreateLayerForMaterialsWithout()
        {
            foreach (CMaterial mat in Model.Materials)
            {
                if (mat.Layers.Count == 0)
                {
                    CMaterialLayer layer = CreateDummyLayer();
                    mat.Layers.Add(layer);  
                }
            }
        }
        private static CMaterialLayer CreateDummyLayer()
        {
            CMaterialLayer layer = new CMaterialLayer(Model);
            if (Model.Textures.Count == 0)
            {
                CreateWhiteTextures();
            }
           
                layer.Texture.Attach(Model.Textures[0]);
             
            return layer;
        }
        private static void CreateWhiteTextures()
        {
            CTexture texture = new CTexture(Model);
            texture.FileName = "Textures\\white.blp";
            Model.Textures.Add(texture);
        }
        private static void AddMissingKeyframes_()
        {
            foreach (CMaterial material in Model.Materials)
            {
                foreach (CMaterialLayer layer in material.Layers)
                {
                    AddMissingStartingTracks(layer.Alpha);
                    AddMissingStartingTracks(layer.TextureId);
                   
                }
            }
            foreach (INode node in Model.Nodes)
            {
                AddMissingStartingTracks(node.Translation);
                AddMissingStartingTracks(node.Rotation);
                AddMissingStartingTracks(node.Scaling);
              
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                    AddMissingStartingTracks(element.EmissionRate);
                    AddMissingStartingTracks(element.LifeSpan);
                    AddMissingStartingTracks(element.InitialVelocity);
                    AddMissingStartingTracks(element.Gravity);
                    AddMissingStartingTracks(element.Longitude);
                    AddMissingStartingTracks(element.Latitude);
                }
                if (node is CParticleEmitter2) {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    AddMissingStartingTracks(element.EmissionRate);
                    AddMissingStartingTracks(element.Speed);
                    AddMissingStartingTracks(element.Variation);
                    AddMissingStartingTracks(element.Latitude);
                    AddMissingStartingTracks(element.Width);
                    AddMissingStartingTracks(element.Length);
                    AddMissingStartingTracks(element.Gravity);
                }
                if (node is CRibbonEmitter) {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    AddMissingStartingTracks(element.HeightAbove);
                    AddMissingStartingTracks(element.HeightBelow);
                    AddMissingStartingTracks(element.Color);
                    AddMissingStartingTracks(element.Alpha);
                    AddMissingStartingTracks(element.TextureSlot);

                }
                if (node is CLight) {
                    CLight element = (CLight)node;
                    AddMissingStartingTracks(element.Color);
                    AddMissingStartingTracks(element.AmbientColor);
                    AddMissingStartingTracks(element.Intensity);
                    AddMissingStartingTracks(element.AmbientIntensity);
                    AddMissingStartingTracks(element.AttenuationEnd);
                    AddMissingStartingTracks(element.AttenuationStart);
                }
            }
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {
                AddMissingStartingTracks(ga.Alpha);
                AddMissingStartingTracks(ga.Color);

            }
            
            foreach (CTextureAnimation ta in Model.TextureAnimations)
            {
                AddMissingStartingTracks(ta.Translation);
                AddMissingStartingTracks(ta.Rotation);
                AddMissingStartingTracks(ta.Scaling);
              
            }
        }
             
        private static void AddMissingStartingTracks(CAnimator<float> animator)
        {
            if (animator.Count > 1 && animator.Static == false)
            {
                Start:
                foreach (var item in animator.ToList())
                {
                    int time = item.Time;
                    CSequence sequence = FindSequenceOfTrack(time);
                    if (sequence != null)
                    {
                        if (animator.Any(x => x.Time == sequence.IntervalStart) == false)
                        {
                            animator.Add(new CAnimatorNode<float>(sequence.IntervalStart, item.Value));
                            goto Start;
                        }
                    }

                }
            }
        }
        private static void AddMissingStartingTracks(CAnimator<int> animator)
        {
            if (animator.Count > 1 && animator.Static == false)
            {
                Start:
                foreach (var item in animator.ToList())
                {
                    int time = item.Time;
                    CSequence sequence = FindSequenceOfTrack(time);
                    if (sequence != null)
                    {
                        if (animator.Any(x => x.Time == sequence.IntervalStart) == false)
                        {
                            animator.Add(new CAnimatorNode<int>(sequence.IntervalStart, item.Value));
                            goto Start;
                        }
                    }

                }
            }
        }
        private static void AddMissingStartingTracks(CAnimator<CVector3> animator)
        {
            if (animator.Count > 1 && animator.Static == false)
            {
                Start:
                foreach (var item in animator.ToList())
                {
                    int time = item.Time;
                    CSequence sequence = FindSequenceOfTrack(time);
                    if (sequence != null)
                    {
                        if (animator.Any(x => x.Time == sequence.IntervalStart) == false)
                        {
                            animator.Add(new CAnimatorNode<CVector3>(sequence.IntervalStart, item.Value));
                            goto Start;
                        }
                    }

                }
            }
        }
        private static void AddMissingStartingTracks(CAnimator<CVector4> animator)
        {
            if (animator.Count > 1 && animator.Static == false)
            {
                Start:
                foreach (var item in animator.ToList())
                {
                    int time = item.Time;
                    CSequence sequence = FindSequenceOfTrack(time);
                    if (sequence != null)
                    {
                        if (animator.Any(x => x.Time == sequence.IntervalStart) == false)
                        {
                            animator.Add(new CAnimatorNode<CVector4>(sequence.IntervalStart, item.Value));
                            goto Start;
                        }
                    }

                }
            }
        }
        private  static int GetIndexOfLastTrackForSequence(int upto, CAnimator<CVector3> animator)
        {
            for (int i = animator.Count - 1; i >= 0; i--)
            {
                if  (animator[i].Time <= upto){ return i; }
            }
            return 0;
        }
        private static int GetIndexOfLastTrackForSequence(int upto, CAnimator<CVector4> animator)
        {
            for (int i = animator.Count - 1; i >= 0; i--)
            {
                if  (animator[i].Time <= upto){ return i; }
            }
            return 0;
        }
        private static CSequence FindSequenceOfTrack(int time)
        {
            foreach (CSequence sequence in Model.Sequences)
            {
                if (time >= sequence.IntervalStart && time <= sequence.IntervalEnd) { return sequence; }
            }
            return null;
        }

        private static bool hasOpening(int track)
        {
            return false;
        }
        private static CSequence BelongsToWhichSequence(int track)
        {
            foreach (CSequence sequence in Model.Sequences)
            {
                if (track >= sequence.IntervalStart && track <= sequence.IntervalEnd)
                {
                    return sequence;
                }
                
            }
            return null;
        }
        private static void DeleteIdenticalAdjascentKEyframes_()
        {
            foreach (INode node in Model.Nodes)
            {
                RemoveAdjascentKeyframes (node.Translation);
                RemoveAdjascentKeyframes (node.Rotation);
                RemoveAdjascentKeyframes (node.Scaling);
                if (node is CAttachment)
                {
                    CAttachment element = (CAttachment) node;
                    RemoveAdjascentKeyframes(element.Visibility);
                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                    RemoveAdjascentKeyframes(element.Visibility);
                    RemoveAdjascentKeyframes (element.EmissionRate);
                    RemoveAdjascentKeyframes (element.LifeSpan);
                    RemoveAdjascentKeyframes (element.InitialVelocity);
                    RemoveAdjascentKeyframes (element.Gravity);
                    RemoveAdjascentKeyframes (element.Longitude);
                    RemoveAdjascentKeyframes (element.Latitude);
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    RemoveAdjascentKeyframes(element.Visibility);
                    RemoveAdjascentKeyframes(element.EmissionRate);
                    RemoveAdjascentKeyframes(element.Speed);
                    RemoveAdjascentKeyframes(element.Width);
                    RemoveAdjascentKeyframes(element.Gravity);
                    RemoveAdjascentKeyframes(element.Length);
                    RemoveAdjascentKeyframes(element.Latitude);
                    RemoveAdjascentKeyframes(element.Variation);
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    RemoveAdjascentKeyframes(element.Visibility);
                    RemoveAdjascentKeyframes(element.HeightAbove);
                    RemoveAdjascentKeyframes(element.HeightBelow);
                    RemoveAdjascentKeyframes(element.Color);
                    RemoveAdjascentKeyframes(element.Alpha);
                    RemoveAdjascentKeyframes(element.TextureSlot);
                   
                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;
                    RemoveAdjascentKeyframes(element.Visibility);
                    RemoveAdjascentKeyframes(element.Color);
                    RemoveAdjascentKeyframes(element.AmbientColor);
                    RemoveAdjascentKeyframes(element.Intensity);
                    RemoveAdjascentKeyframes(element.AmbientIntensity);
                    RemoveAdjascentKeyframes(element.AttenuationEnd);
                    RemoveAdjascentKeyframes(element.AttenuationStart);

                }
            }
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {
                if (ga.Alpha.Static == false )
                {
                    RemoveAdjascentKeyframes(ga.Alpha);
                }
                if (ga.Color.Static == false) { RemoveAdjascentKeyframes(ga.Color);  }
            }
            foreach (CTextureAnimation taa in Model.TextureAnimations)
            {
               RemoveAdjascentKeyframes(taa.Translation);
                RemoveAdjascentKeyframes(taa.Rotation);
                  RemoveAdjascentKeyframes(taa.Scaling);
                 
            }
            foreach (CMaterial material in Model.Materials)
            {
                foreach (CMaterialLayer layer in material.Layers)
                {
                    RemoveAdjascentKeyframes(layer.Alpha);
                    RemoveAdjascentKeyframes(layer.TextureId);
                }
            }
            foreach (CCamera cam in Model.Cameras)
            {
                RemoveAdjascentKeyframes(cam.Rotation);
            }
           
        }
        private static void RemoveAdjascentKeyframesTimes(CAnimator<float> list)
        {
            if (list.Static == true) { return; }
            start:
            if (list.Count < 3) { return; }

            for (int i = 0; i < list.Count; i++)
            {
                if (i - 1 != -1)
                {
                    if (list[i-1].Time == list[i].Time)
                    {
                        list.RemoveAt(i);
                        goto start;
                    }
                }
                
            }
        }
        private static void RemoveAdjascentKeyframesTimes(CAnimator<int> list)
        {
            if (list.Static == true) { return; }
            start:
            if (list.Count < 3) { return; }

            for (int i = 0; i < list.Count; i++)
            {
                if (i - 1 != -1)
                {
                    if (list[i - 1].Time == list[i].Time)
                    {
                        list.RemoveAt(i);
                        goto start;
                    }
                }

            }
        }
        private static void RemoveAdjascentKeyframesTimes(CAnimator<CVector3> list)
        {
            if (list.Static == true) { return; }
            start:
            if (list.Count < 3) { return; }

            for (int i = 0; i < list.Count; i++)
            {
               
                if (i - 1 != -1)
                {
                    if (list[i - 1].Time == list[i].Time  )
                    {
                        list.RemoveAt(i);
                        goto start;
                    }
                }

            }
        }
        private static void RemoveAdjascentKeyframesTimes(CAnimator<CVector4> list)
        {
            if (list.Static == true) { return; }
            start:
            if (list.Count < 3) { return; }

            for (int i = 0; i < list.Count; i++)
            {
                if (i - 1 != -1)
                {
                    if (list[i - 1].Time == list[i].Time)
                    {
                        list.RemoveAt(i);
                        goto start;
                    }
                }

            }
        }

        private static void RemoveAdjascentKeyframes(CAnimator<CVector4> list)
        {
            if (list.Static == true) { return; }
            start:
            if (list.Count < 3) { return; }
          
            for (int i = 0; i < list.Count; i++)
            {
                if (i + 2 < list.Count)
                {
                    var key1 = list[i];
                    var key2 = list[i + 1];
                    var key3 = list[i + 2];
                    if (TracksHaveSameValues(key1.Value, key2.Value, key3.Value) && TracksBelongToSameSequence(key1.Time, key2.Time, key3.Time))
                    {
                        list.RemoveAt(i + 1);
                        goto start;
                    }
                }
            }
        }
        private static void RemoveAdjascentKeyframes(CAnimator<int> list)
        {
            if (list.Static == true) { return; }
            start:
            if (list.Count < 3) { return; }
          
            for (int i = 0; i < list.Count; i++)
            {
                if (i + 2 < list.Count)
                {
                    var key1 = list[i];
                    var key2 = list[i + 1];
                    var key3 = list[i + 2];
                    if (TracksHaveSameValues(key1.Value, key2.Value, key3.Value) && TracksBelongToSameSequence(key1.Time, key2.Time, key3.Time))
                    {
                        list.RemoveAt(i + 1);
                        goto start;
                    }
                }
            }
        }
        private static void RemoveAdjascentKeyframes(CAnimator<float> list)
        {
            if (list.Static == true) { return; }
            start:
            if (list.Count < 3) { return; }
           
            for (int i = 0; i < list.Count; i++)
            {
                if (i + 2 < list.Count)
                {
                    var key1 = list[i];
                    var key2 = list[i + 1];
                    var key3 = list[i + 2];
                    if (TracksHaveSameValues(key1.Value, key2.Value, key3.Value) && TracksBelongToSameSequence(key1.Time, key2.Time, key3.Time))
                    {
                        list.RemoveAt(i + 1);
                        goto start;
                    }
                }
            }
        }

        private static void FixQuirtOfEmitters2()
        {
            foreach (INode node in Model.Nodes)
            {
                if (node is CParticleEmitter2 emitter)
                {
                    if (emitter.EmissionRate.Static == false && emitter.Squirt == true) { emitter.Squirt = false;  }
                }
            }
        }
        private static void RemoveAdjascentKeyframes(CAnimator<CVector3> list)
        {
            if (list.Static == true) { return; }
            if (list.Count < 3) { return; }
            start:
            for (int i = 0; i < list.Count; i++)
            {
                if (i + 2 < list.Count)
                {
                    var key1 = list[i];
                    var key2 = list[i + 1];
                    var key3 = list[i + 2];
                    if (TracksHaveSameValues(key1.Value, key2.Value, key3.Value) && TracksBelongToSameSequence(key1.Time, key2.Time, key3.Time))
                    {
                         
                        list.RemoveAt(i+1);
                        goto start;
                    }
                }
            }
        }
        private static bool TracksBelongToSameSequence(int one, int two, int three)
        {
            foreach (CSequence sequence in Model.Sequences)
            {
                if (
                    one>= sequence.IntervalStart && one <= sequence.IntervalEnd &&
                    two>= sequence.IntervalStart && two <= sequence.IntervalEnd &&
                    three >= sequence.IntervalStart && three <= sequence.IntervalEnd
                    ) { return true; }
            }
            return false;

        }
        private static bool TracksBelongToSameSequence(int one, int two)
        {
            foreach (CSequence sequence in Model.Sequences)
            {
                if (
                    one >= sequence.IntervalStart && one <= sequence.IntervalEnd &&
                    two >= sequence.IntervalStart && two <= sequence.IntervalEnd  
                   
                    ) { return true; }
            }
            return false;

        }
        private static bool TracksHaveSameValues(CVector3 one, CVector3 two, CVector3 three)
        {
            return one.X == two.X && one.Y == two.Y && one.Z == two.Z &&
                   two.X == three.X && two.Y == three.Y && two.Z == three.Z;
        }
        private static bool TracksHaveSameValues(float one, float two, float three)
        {
            return one == two && two == three;
        }
        private static bool TracksHaveSameValues(int one, int two, int three)
        {
            return one == two && two == three;
        }
        private static bool TracksHaveSameValues(CVector4 one, CVector4 two, CVector4 three)
        {
            return (one.X == two.X && one.Y == two.Y && one.Z == two.Z && one.W == two.W) &&
                   (three.X == two.X && three.Y == two.Y && three.Z == two.Z && three.W == two.W);
        }



        private static void CalculateExtents_()
        {
            // calculate the extent of each geoset
            foreach (CGeoset geo in Model.Geosets)  CalculateGeosetExtent(geo);
             // calculate model extent
            Model.Extent = Calculator.CalculateModelExtent(Model.Geosets.Select(x=>x.Extent).ToList());
            // calculate sequences' extents
            CalculateSequenceExtents();
             // add the list of sequence extents to the lsit of extents of each geoset
            List<CExtent> SEquenceExtents = Model.Sequences.Select(x=>x.Extent).ToList();
            foreach (CGeoset geo in Model.Geosets)
            {
                geo.Extents.Clear();
                foreach (CExtent extent in SEquenceExtents)
                {
                    CGeosetExtent gExtent = new CGeosetExtent(Model);
                    gExtent.Extent = new CExtent(extent);
                    geo.Extents.Add(gExtent);
                }
               
            }
           
        }
        private static void CalculateSequenceExtents()
        {
            foreach (CSequence sequence in Model.Sequences)
            {
                sequence.Extent = new CExtent(Model.Extent);
            }
        }
         

        public  static void CalculateGeosetExtent(CGeoset geoset)
        {
            List<CVector3> vectors = new List<CVector3> ();
            foreach (CGeosetVertex vertex in geoset.Vertices)
            {
                vectors.Add(vertex.Position);
            }
            geoset.Extent = Calculator.GetExent(vectors);
        }
        private static void ClampKeyframes_()
        {
            
            foreach (INode node in Model.Nodes)
            {
                for (int i = 0; i < node.Rotation.Count; i++)
                {
                    node.Rotation[i] = Calculator.ClampQuaternion(node.Rotation[i]);
                }
                if (node is CAttachment)
                {
                    CAttachment element = (CAttachment)node;
                    for (int y = 0; y < element.Visibility.Count; y++)
                    {
                        element.Visibility[y] = Calculator.ClampNormalized(element.Visibility[y]);
                    }

                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;

                    if (element.Visibility.Static)
                    {
                        element.Visibility.MakeStatic(Calculator.ClampNormalized(element.Visibility.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Visibility.Count; i++)
                        {

                            element.Visibility[i] = Calculator.ClampNormalized(element.Visibility[i]);
                        }
                    }
                    if (element.EmissionRate.Static)
                    {
                        element.EmissionRate.MakeStatic(Calculator.ClampFloat(element.EmissionRate.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.EmissionRate.Count; i++)
                        {

                            element.EmissionRate[i] = Calculator.ClampFloat(element.EmissionRate[i]);
                        }
                    }
                    if (element.InitialVelocity.Static)
                    {
                        element.InitialVelocity.MakeStatic(Calculator.ClampFloat(element.InitialVelocity.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.InitialVelocity.Count; i++)
                        {

                            element.InitialVelocity[i] = Calculator.ClampFloat(element.InitialVelocity[i]);
                        }
                    }
                    if (element.Gravity.Static)
                    {
                        element.Gravity.MakeStatic(Calculator.ClampFloat(element.Gravity.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Gravity.Count; i++)
                        {

                            element.Gravity[i] = Calculator.ClampFloat(element.Gravity[i]);
                        }
                    }
                    if (element.Longitude.Static)
                    {
                        element.Longitude.MakeStatic(Calculator.ClampFloat(element.Longitude.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Longitude.Count; i++)
                        {

                            element.Longitude[i] = Calculator.ClampFloat(element.Longitude[i]);
                        }
                    }
                    if (element.Latitude.Static)
                    {
                        element.Latitude.MakeStatic(Calculator.ClampFloat(element.Latitude.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Latitude.Count; i++)
                        {

                            element.Latitude[i] = Calculator.ClampFloat(element.Latitude[i]);
                        }
                    }

                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    if (element.Visibility.Static)
                    {
                        element.Visibility.MakeStatic(Calculator.ClampNormalized(element.Visibility.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Visibility.Count; i++)
                        {

                            element.Visibility[i] = Calculator.ClampNormalized(element.Visibility[i]);
                        }
                    }
                    if (element.Latitude.Static)
                    {
                        element.Latitude.MakeStatic(Calculator.ClampFloat(element.Latitude.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Latitude.Count; i++)
                        {

                            element.Latitude[i] = Calculator.ClampFloat(element.Latitude[i]);
                        }
                    }
                    if (element.Width.Static)
                    {
                        element.Width.MakeStatic(Calculator.ClampFloat(element.Width.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Width.Count; i++)
                        {

                            element.Width[i] = Calculator.ClampFloat(element.Width[i]);
                        }
                    }
                    if (element.Length.Static)
                    {
                        element.Length.MakeStatic(Calculator.ClampFloat(element.Length.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Length.Count; i++)
                        {

                            element.Length[i] = Calculator.ClampFloat(element.Length[i]);
                        }
                    }
                    if (element.Speed.Static)
                    {
                        element.Speed.MakeStatic(Calculator.ClampFloat(element.Speed.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Speed.Count; i++)
                        {

                            element.Speed[i] = Calculator.ClampFloat(element.Speed[i]);
                        }
                    }
                    if (element.Speed.Static)
                    {
                        element.Speed.MakeStatic(Calculator.ClampFloat(element.Speed.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Speed.Count; i++)
                        {

                            element.Speed[i] = Calculator.ClampFloat(element.Speed[i]);
                        }
                    }
                    if (element.EmissionRate.Static)
                    {
                        element.EmissionRate.MakeStatic(Calculator.ClampFloat(element.EmissionRate.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Speed.Count; i++)
                        {

                            element.EmissionRate[i] = Calculator.ClampFloat(element.EmissionRate[i]);
                        }
                    }
                    if (element.Variation.Static)
                    {
                        element.Variation.MakeStatic(Calculator.ClampFloat(element.Variation.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Speed.Count; i++)
                        {

                            element.Variation[i] = Calculator.ClampFloat(element.Variation[i]);
                        }
                    }
                    if (element.Gravity.Static)
                    {
                        element.Gravity.MakeStatic(Calculator.ClampNormalized(element.Gravity.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Speed.Count; i++)
                        {

                            element.Gravity[i] = Calculator.ClampFloat(element.Gravity[i]);
                        }
                    }
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    if (element.Visibility.Static)
                    {
                        element.Visibility.MakeStatic(Calculator.ClampNormalized(element.Visibility.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Visibility.Count; i++)
                        {

                            element.Visibility[i] = Calculator.ClampNormalized(element.Visibility[i]);
                        }
                    }
                    if (element.Color.Static)
                    {
                        element.Color.MakeStatic(Calculator.ClampVector3(element.Color.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Color.Count; i++)
                        {

                            element.Color[i] = Calculator.ClampVector3(element.Color[i]);
                        }
                    }
                    if (element.Alpha.Static)
                    {
                        element.Alpha.MakeStatic(Calculator.ClampFloat(element.Alpha.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Alpha.Count; i++)
                        {

                            element.Alpha[i] = Calculator.ClampFloat(element.Alpha[i]);
                        }
                    }
                    if (element.HeightAbove.Static)
                    {
                        element.HeightAbove.MakeStatic(Calculator.ClampFloat(element.HeightAbove.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.HeightAbove.Count; i++)
                        {

                            element.HeightAbove[i] = Calculator.ClampFloat(element.HeightAbove[i]);
                        }
                    }
                    if (element.HeightBelow.Static)
                    {
                        element.HeightBelow.MakeStatic(Calculator.ClampFloat(element.HeightBelow.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.HeightBelow.Count; i++)
                        {

                            element.HeightBelow[i] = Calculator.ClampFloat(element.HeightBelow[i]);
                        }
                    }
                    if (element.TextureSlot.Static)
                    {
                        element.TextureSlot.MakeStatic(Calculator.ClampInt(element.TextureSlot.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.HeightBelow.Count; i++)
                        {

                            element.TextureSlot[i] = Calculator.ClampInt(element.TextureSlot[i]);
                        }
                    }
                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;
                    if (element.Visibility.Static)
                    {
                        element.Visibility.MakeStatic(Calculator.ClampNormalized(element.Visibility.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Visibility.Count; i++)
                        {

                            element.Visibility[i] = Calculator.ClampNormalized(element.Visibility[i]);
                        }
                    }
                    if (element.Color.Static)
                    {
                        element.Color.MakeStatic(Calculator.ClampVector3(element.Color.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Color.Count; i++)
                        {

                            element.Color[i] = Calculator.ClampVector3(element.Color[i]);
                        }
                    }
                    if (element.Visibility.Static)
                    {
                        element.Visibility.MakeStatic(Calculator.ClampNormalized(element.Visibility.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Visibility.Count; i++)
                        {

                            element.Visibility[i] = Calculator.ClampNormalized(element.Visibility[i]);
                        }
                    }
                    if (element.AmbientColor.Static)
                    {
                        element.AmbientColor.MakeStatic(Calculator.ClampVector3(element.AmbientColor.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Color.Count; i++)
                        {

                            element.AmbientColor[i] = Calculator.ClampVector3(element.AmbientColor[i]);
                        }
                    }
                    if (element.AttenuationEnd.Static)
                    {
                        element.AttenuationEnd.MakeStatic(Calculator.ClampFloat(element.AttenuationEnd.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.AttenuationEnd.Count; i++)
                        {

                            element.AttenuationEnd[i] = Calculator.ClampFloat(element.AttenuationEnd[i]);
                        }
                    }
                    if (element.AttenuationStart.Static)
                    {
                        element.AttenuationStart.MakeStatic(Calculator.ClampFloat(element.AttenuationStart.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.AttenuationEnd.Count; i++)
                        {

                            element.AttenuationStart[i] = Calculator.ClampFloat(element.AttenuationStart[i]);
                        }
                    }
                    if (element.AmbientIntensity.Static)
                    {
                        element.AmbientIntensity.MakeStatic(Calculator.ClampFloat(element.AmbientIntensity.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.AmbientIntensity.Count; i++)
                        {

                            element.AmbientIntensity[i] = Calculator.ClampFloat(element.AmbientIntensity[i]);
                        }
                    }
                    if (element.Intensity.Static)
                    {
                        element.Intensity.MakeStatic(Calculator.ClampFloat(element.Intensity.GetValue()));
                    }
                    else
                    {
                        for (int i = 0; i < element.Intensity.Count; i++)
                        {

                            element.Intensity[i] = Calculator.ClampFloat(element.Intensity[i]);
                        }
                    }
                }

            }
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {
                if (ga.Alpha.Static)
                {
                    float value = ga.Alpha.GetValue();
                    if (value < 0f || value > 1) ga.Alpha.MakeStatic(1);

                }
                else
                {
                    for (int i = 0; i < ga.Alpha.Count; i++)
                    {
                        ga.Alpha[i] = Calculator.ClampNormalized(ga.Alpha[i]);
                    }
                }
                if (ga.Color.Static)
                {
                    CVector3 value = ga.Color.GetValue();
                    ga.Color.MakeStatic(Calculator.ClampVector3(value));

                }
                else
                {
                    for (int i = 0; i < ga.Color.Count; i++)
                    {
                        ga.Color[i] = new CAnimatorNode<CVector3>(ga.Color[i].Time, Calculator.ClampVector3(ga.Color[i].Value));
                    }
                }
                foreach (CMaterial mat in Model.Materials)
                {
                    for (int LayerIndex = 0; LayerIndex < mat.Layers.Count; LayerIndex++)
                    {
                        if (mat.Layers[LayerIndex].Alpha.Static)
                        {
                            if (
                                mat.Layers[LayerIndex].Alpha.GetValue() < 0 ||
                                mat.Layers[LayerIndex].Alpha.GetValue() > 1

                                ) { mat.Layers[LayerIndex].Alpha.MakeStatic(1); }
                        }
                        else
                        {
                            for (int KeyframeIndex = 0; KeyframeIndex < mat.Layers[LayerIndex].Alpha.Count; KeyframeIndex++)
                            {
                                mat.Layers[LayerIndex].Alpha[KeyframeIndex] = Calculator.ClampNormalized(mat.Layers[LayerIndex].Alpha[KeyframeIndex]);
                            }

                        }
                    }

                }
            }
            for (int i = 0; i < Model.TextureAnimations.Count; i++)
            {
                if (Model.TextureAnimations[i].Rotation.Static == false)
                {
                    for (int j = 0; j < Model.TextureAnimations[i].Rotation.Count; j++)
                    {
                        Calculator.ClampQuaternion(Model.TextureAnimations[i].Rotation[j]);
                    }
                }
            }



        }

        private static void DeleteDuplicateGAs_()
        {
            List<CGeosetAnimation> originals = new List<CGeosetAnimation>();

            foreach (CGeoset geo in Model.Geosets)
            {
                if (Model.GeosetAnimations.Any(x => x.Geoset.Object == geo))
                {
                    originals.Add(
                    Model.GeosetAnimations.First(x => x.Geoset.Object == geo));
                }
            }

            Model.GeosetAnimations.Clear();
            foreach (CGeosetAnimation ga in originals) { Model.GeosetAnimations.Add(ga); };
        }

        private static void SetAllStaticGAS_()
        {
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {
                if (ga.Alpha.Static)
                {
                    if (ga.Alpha.GetValue() < 1) { ga.Alpha.MakeStatic(1); }
                }
            }
        }

        private static void AddMissingGAs_()
        {
            foreach (CGeoset geo in Model.Geosets)
            {
                if (hasGAForGeoset(geo) == false)
                {
                    CGeosetAnimation ga = new CGeosetAnimation(Model);
                    ga.Geoset.Attach(geo);
                    Model.GeosetAnimations.Add(ga);
                }
            }
        }
        private static bool hasGAForGeoset(CGeoset geoset)
        {
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {
                if (ga.Geoset.Object == geoset) { return true; }
            }
            return false;
        }
        private static void ClampLights_()
        {
            foreach (INode node in Model.Nodes)
            {
                if (node is CLight)
                {
                    CLight cLight = (CLight)node;
                    if (cLight.AttenuationStart.Static && cLight.AttenuationStart.GetValue() < 80)
                    {
                        cLight.AttenuationStart.MakeStatic(80);
                    }
                    if (cLight.AttenuationStart.Static && cLight.AttenuationStart.GetValue() > 200)
                    {
                        cLight.AttenuationStart.MakeStatic(200);
                    }
                    if (cLight.AttenuationStart.Static == false)
                    {
                        for (int i = 0; i < cLight.AttenuationStart.Count; i++)
                        {
                            if (cLight.AttenuationStart[i].Value < 80)
                            {
                                int time = cLight.AttenuationStart[i].Time;
                                cLight.AttenuationStart[i] = new CAnimatorNode<float>(time, 80);
                            }

                        }

                    }
                    if (cLight.AttenuationEnd.Static == false)
                    {
                        for (int i = 0; i < cLight.AttenuationEnd.Count; i++)
                        {
                            if (cLight.AttenuationEnd[i].Value > 200)
                            {
                                int time = cLight.AttenuationEnd[i].Time;
                                cLight.AttenuationEnd[i] = new CAnimatorNode<float>(time, 200);
                            }

                        }
                    }

                }
            }
        }

        private static void ClampUVs_()
        {
            foreach (CGeoset geo in Model.Geosets)
            {
                foreach (CGeosetVertex vertex in geo.Vertices)

                {
                    vertex.TexturePosition = Calculator.ClampUV(vertex.TexturePosition);
                }
            }
        }

        private static void AddMissingVisibilities_()
        {
            foreach (CSequence sequence in Model.Sequences)
            {
                foreach (CGeosetAnimation ga in Model.GeosetAnimations)
                {
                    if (ga.Alpha.Static == false)
                    {
                        if (ga.Alpha.Any(x => x.Time == sequence.IntervalStart) == false)
                        {
                            ga.Alpha.Add(new CAnimatorNode<float>(sequence.IntervalStart, 1));
                        }
                    }
                }
                foreach (INode node in Model.Nodes)
                {
                    if (node is CAttachment)
                    {
                        CAttachment c = (CAttachment)node;
                        if (c.Visibility.Count > 0)
                        {
                            if (c.Visibility.Any(x => x.Time == sequence.IntervalStart) == false)
                            {
                                c.Visibility.Add(new CAnimatorNode<float>(sequence.IntervalStart, 1));
                            }
                        }
                    }
                    if (node is CLight)
                    {
                        CLight c = (CLight)node;
                        if (c.Visibility.Count > 0)
                        {
                            if (c.Visibility.Any(x => x.Time == sequence.IntervalStart) == false)
                            {
                                c.Visibility.Add(new CAnimatorNode<float>(sequence.IntervalStart, 1));
                            }
                        }
                    }
                    if (node is CParticleEmitter)
                    {
                        CParticleEmitter c = (CParticleEmitter)node;
                        if (c.Visibility.Count > 0)
                        {
                            if (c.Visibility.Any(x => x.Time == sequence.IntervalStart) == false)
                            {
                                c.Visibility.Add(new CAnimatorNode<float>(sequence.IntervalStart, 1));
                            }
                        }
                    }
                    if (node is CParticleEmitter2)
                    {
                        CParticleEmitter2 c = (CParticleEmitter2)node;
                        if (c.Visibility.Count > 0)
                        {
                            if (c.Visibility.Any(x => x.Time == sequence.IntervalStart) == false)
                            {
                                c.Visibility.Add(new CAnimatorNode<float>(sequence.IntervalStart, 1));
                            }
                        }
                    }
                    if (node is CRibbonEmitter)
                    {
                        CRibbonEmitter c = (CRibbonEmitter)node;
                        if (c.Visibility.Count > 0)
                        {
                            if (c.Visibility.Any(x => x.Time == sequence.IntervalStart) == false)
                            {
                                c.Visibility.Add(new CAnimatorNode<float>(sequence.IntervalStart, 1));
                            }
                        }
                    }
                }
            }
        }

        private static void MergeGeosets_()
        { 
           Start:
            foreach (CGeoset geo1 in Model.Geosets.ToList()) 
            {
                var material = geo1.Material;
                if (Model.Geosets.Any(x => x.Material == material && x != geo1)){


                    foreach (CGeoset geo2 in Model.Geosets.ToList())
                    {
                        if (geo2.Material == geo1.Material)
                        {
                            foreach (CGeosetVertex vertex in geo2.Vertices)
                            {
                                geo1.Vertices.Add(vertex);
                            }
                            foreach (CGeosetFace face in geo2.Faces)
                            {
                                geo1.Faces.Add(face);
                            }
                        }
                    }
                    goto Start;
                }
            }

        }

        private static void DeleteUnusedKeyframes_()
        {
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {
             
                if (ga.Alpha.Static == false)
                {
                    foreach (var item in ga.Alpha.ToList())
                    {
                        if (item.Time<0) ga.Alpha.Remove(item);
                        if (ValueExistsInSequences(item.Time) == false)
                        {
                            ga.Alpha.Remove(item);
                        }

                    }


                }
                if (ga.Color.Static == false)
                {
                    foreach (var item in ga.Color.ToList())
                    {
                        if (item.Time < 0) ga.Color.Remove(item);
                        if (ValueExistsInSequences(item.Time) == false)
                        {
                            ga.Color.Remove(item);
                        }

                    }
                }
            }
            foreach (CMaterial material in Model.Materials)
            {
                foreach (CMaterialLayer layer in material.Layers)
                {
                    if (layer.Alpha.Static == false)
                    {

                        foreach (var item in layer.Alpha.ToList())
                        {
                            if (item.Time < 0) { layer.Alpha.Remove(item); }
                            if (ValueExistsInSequences(item.Time) == false)
                            {
                                layer.Alpha.Remove(item);
                            }

                        }



                    }
                    if (layer.TextureId.Static == false)
                    {
                        
                        foreach (var item in layer.TextureId.ToList())
                        {
                            if (item.Time < 0) { layer.TextureId.Remove(item); }
                            if (ValueExistsInSequences(item.Time) == false)
                            {
                                layer.TextureId.Remove(item);
                            }

                        }



                    }
                }
            }
            foreach (CTextureAnimation ta in Model.TextureAnimations)
            {
                if (ta.Translation.Static == false)
                {
                    foreach (var item in ta.Translation.ToList())
                    {
                        if (item.Time < 0) { ta.Translation.Remove(item); }
                        if (ValueExistsInSequences(item.Time) == false)
                        {
                            ta.Translation.Remove(item);
                        }

                    }
                }
                if (ta.Scaling.Static == false)
                {
                    foreach (var item in ta.Scaling.ToList())
                    {
                        if (item.Time < 0) { ta.Scaling.Remove(item); }
                        if (ValueExistsInSequences(item.Time) == false)
                        {
                            ta.Scaling.Remove(item);
                        }

                    }
                }
                if (ta.Rotation.Static == false)
                {
                    foreach (var item in ta.Rotation.ToList())
                    {
                        if (item.Time < 0) { ta.Rotation.Remove(item); }
                        if (ValueExistsInSequences(item.Time) == false)
                        {
                            ta.Rotation.Remove(item);
                        }

                    }
                }
            }
            foreach (CCamera cam in Model.Cameras)
            {
                if (cam.Rotation.Static == false)
                {
                    
                    foreach (var item in cam.Rotation.ToList())
                    {
                        if (item.Time < 0) { cam.Rotation.Remove(item); }
                        if (ValueExistsInSequences(item.Time) == false)
                        {
                            cam.Rotation.Remove(item);
                        }

                    }
                }
            }
            foreach (INode node in Model.Nodes)
            {
                foreach (var item in node.Translation.ToList())
                {
                    if (item.Time < 0) {  node.Translation.Remove(item); }
                    if (!ValueExistsInSequences(item.Time)) { node.Translation.Remove(item); }
                }
                foreach (var item in node.Rotation.ToList())
                {
                    if (item.Time < 0) { node.Rotation.Remove(item); }
                    if (!ValueExistsInSequences(item.Time)) { node.Rotation.Remove(item); }
                }
                foreach (var item in node.Scaling.ToList())
                {
                    if (item.Time < 0) { node.Scaling.Remove(item); }
                    if (!ValueExistsInSequences(item.Time)) { node.Scaling.Remove(item); }
                }
                if (node is CAttachment)
                {
                    CAttachment cAttachment = (CAttachment)node;
                    foreach (var item in cAttachment.Visibility.ToList())
                    {
                        if (item.Time < 0) { cAttachment.Visibility.Remove(item); }
                        if (!ValueExistsInSequences(item.Time)) { cAttachment.Visibility.Remove(item); }
                    }

                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                    if (!element.InitialVelocity.Static) { foreach (var item in element.InitialVelocity.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.InitialVelocity.Remove(item); } } }
                    if (!element.LifeSpan.Static) { foreach (var item in element.LifeSpan.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.LifeSpan.Remove(item); } } }
                    if (!element.EmissionRate.Static) { foreach (var item in element.EmissionRate.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.EmissionRate.Remove(item); } } }
                    if (!element.Longitude.Static) { foreach (var item in element.Longitude.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.Longitude.Remove(item); } } }
                    if (!element.Visibility.Static) { foreach (var item in element.Visibility.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.Visibility.Remove(item); } } }
                    if (!element.Latitude.Static) { foreach (var item in element.Latitude.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.Latitude.Remove(item); } } }
                    if (!element.Gravity.Static) { foreach (var item in element.Gravity.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.Gravity.Remove(item); } } }
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    if (!element.Speed.Static) { foreach (var item in element.Speed.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.Speed.Remove(item); } } }
                    if (!element.Width.Static) { foreach (var item in element.Width.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.Width.Remove(item); } } }
                    if (!element.Length.Static) { foreach (var item in element.Length.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.Length.Remove(item); } } }
                    if (!element.EmissionRate.Static) { foreach (var item in element.EmissionRate.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.EmissionRate.Remove(item); } } }
                    if (!element.Variation.Static) { foreach (var item in element.Variation.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.Variation.Remove(item); } } }
                    if (!element.Visibility.Static) { foreach (var item in element.Visibility.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.Visibility.Remove(item); } } }
                    if (!element.Latitude.Static) { foreach (var item in element.Latitude.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.Latitude.Remove(item); } } }
                    if (!element.Gravity.Static) { foreach (var item in element.Gravity.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.Gravity.Remove(item); } } }

                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    if (!element.Color.Static) { foreach (var item in element.Color.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.Color.Remove(item); } } }
                    if (!element.HeightBelow.Static) { foreach (var item in element.HeightBelow.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.HeightBelow.Remove(item); } } }
                    if (!element.HeightAbove.Static) { foreach (var item in element.HeightAbove.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.HeightAbove.Remove(item); } } }
                    if (!element.Alpha.Static) { foreach (var item in element.Alpha.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.Alpha.Remove(item); } } }
                    if (!element.TextureSlot.Static) { foreach (var item in element.TextureSlot.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.TextureSlot.Remove(item); } } }
                    if (!element.Visibility.Static) { foreach (var item in element.Visibility.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.Visibility.Remove(item); } } }

                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;
                    if (!element.Color.Static) { foreach (var item in element.Color.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.Color.Remove(item); } } }
                    if (!element.AmbientColor.Static) { foreach (var item in element.AmbientColor.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.AmbientColor.Remove(item); } } }
                    if (!element.Intensity.Static) { foreach (var item in element.Intensity.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.Intensity.Remove(item); } } }
                    if (!element.AmbientIntensity.Static) { foreach (var item in element.AmbientIntensity.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.AmbientIntensity.Remove(item); } } }
                    if (!element.AttenuationEnd.Static) { foreach (var item in element.AttenuationEnd.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.AttenuationEnd.Remove(item); } } }
                    if (!element.AttenuationStart.Static) { foreach (var item in element.AttenuationStart.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.AttenuationStart.Remove(item); } } }
                    if (!element.Visibility.Static) { foreach (var item in element.Visibility.ToList()) { if (!ValueExistsInSequences(item.Time)) { element.Visibility.Remove(item); } } }
                }
            }
        }
        private static bool ValueExistsInSequences(int track)
        {
            if (Model.Sequences == null) { return false; }
            if (Model.Sequences.Count == 0) { return false; }
            foreach (CSequence sequence in Model.Sequences)
            {
                if (track >= sequence.IntervalStart && track <= sequence.IntervalEnd)
                {
                    return true;
                }
            }
            return false;
        }
        private static void DeleteUnusedTextureAnimations_()
        {
            foreach (CTextureAnimation tx in Model.TextureAnimations.ToList())
            {
                if (TextureAnimationUsed(tx) == false) {   Model.TextureAnimations.Remove(tx); }
            }

        }

        private static bool TextureAnimationUsed(CTextureAnimation t)
        {
            foreach (CMaterial mat in Model.Materials)
            {
                foreach (CMaterialLayer layer in mat.Layers)
                {
                    if (layer.TextureAnimation.Object == t ) {  return true; }
                }
               
            }
            return false;
        }

        private static void DeleteUnusedBones_()
        {
            foreach (INode node in Model.Nodes.ToList())
            {
                if (node is CBone)
                {
                    if (BoneHasAttachees(node) == false && NodeHasChildren(node) == false)
                    {
                      Model.Nodes.Remove(node);
                    }

                }
            }
           
        }
        private static bool NodeHasChildren(INode node)
        {
            foreach (INode n in Model.Nodes)
            {
                if (n.Parent.Node == node) { return true; }
            }
            return false;
        }
        private static bool BoneHasAttachees(INode node)
        {
            foreach (CGeoset geo in Model.Geosets)
            {
                foreach (CGeosetGroup group in geo.Groups)
                {
                    foreach (var item in group.Nodes)
                    {
                        if (item.Node.ObjectId == node.ObjectId) { return true; }
                    }
                }
            }
            return false;
        }

        private static void DeleteUnAnimatedSequences_()
        {
            foreach (CSequence sequence in Model.Sequences.ToList())
            {

                int from = sequence.IntervalStart;
                int to = sequence.IntervalEnd;
                if (IntervalAnimated(from, to) == false)
                {
                    Model.Sequences.Remove(sequence);
                }
            }

        }

        public static bool IntervalAnimated(int from, int to)
        {
            foreach (INode node in Model.Nodes)
            {
                if (node.Translation.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                if (node.Rotation.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                if (node.Scaling.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                if (node is CAttachment)
                {
                    CAttachment element = (CAttachment)node;
                    if (element.Visibility.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                    if (element.Visibility.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.Latitude.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.Longitude.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.EmissionRate.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.InitialVelocity.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.Gravity.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    if (element.Visibility.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.Variation.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.Length.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.Width.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.Speed.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.Gravity.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.EmissionRate.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.Latitude.Any(x => x.Time >= from && x.Time <= to)) { return true; }


                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    if (element.Visibility.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.HeightAbove.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.HeightBelow.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.Color.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.Alpha.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.TextureSlot.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;
                    if (element.Visibility.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.Color.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.AmbientColor.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.AttenuationEnd.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.AttenuationStart.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.Intensity.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (element.AmbientIntensity.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                }
            }
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {
                if (ga.Alpha.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                if (ga.Color.Any(x => x.Time >= from && x.Time <= to)) { return true; }
            }
            foreach (CMaterial mat in Model.Materials)
            {
                foreach (CMaterialLayer layer in mat.Layers)
                {
                    if (layer.Alpha.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                    if (layer.TextureId.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                }
            }
            foreach (CCamera cam in Model.Cameras)
            {
                if (cam.Rotation.Any(x => x.Time >= from && x.Time <= to)) { return true; }
            }
            foreach (CTextureAnimation node in Model.TextureAnimations)
            {
                if (node.Translation.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                if (node.Rotation.Any(x => x.Time >= from && x.Time <= to)) { return true; }
                if (node.Scaling.Any(x => x.Time >= from && x.Time <= to)) { return true; }
            }
            return false;
        }

        private static void DeleteIsolatedVertices_()
        {
            foreach (CGeoset geo in Model.Geosets)
            {
                foreach (CGeosetVertex vertex in geo.Vertices.ToList())
                {
                    if (Vertex_Used(vertex, geo) == false)
                    {
                        geo.Vertices.Remove(vertex);
                    }
                }
            }
        }

        private static bool Vertex_Used(CGeosetVertex vertex, CGeoset geo)
        {
            foreach (CGeosetFace face in geo.Faces)
            {
                if (face.Vertex1.Object == vertex ||
                 face.Vertex2.Object == vertex ||
                 face.Vertex3.Object == vertex)
                {
                    return true;
                }


            }
            return false;
        }

        private static void RemoveEmptyGeosets()
        {
            return;
            foreach (CGeoset geo in Model.Geosets.ToList())
            {
                if (geo.Faces.Count == 0 || geo.Vertices.Count < 3)
                {
                    Model.Geosets.Remove(geo);
                }
            }
        }
        private static void DeleteIsolatedTriangles_()
        {
            foreach (CGeoset geo in Model.Geosets)
            {
                foreach (CGeosetFace face in geo.Faces.ToList())
                {
                    var vertex1 = face.Vertex1;
                    var vertex2 = face.Vertex1;
                    var vertex3 = face.Vertex1;
                    if (TriangleIsIsolated(face, geo) == true)
                    {
                        geo.Faces.Remove(face);
                    }
                }

            }
            RemoveEmptyGeosets();
        }
        private static bool TriangleIsIsolated(CGeosetFace face_input, CGeoset geoset)
        {
            foreach (CGeosetFace face in geoset.Faces)
            {
                if (face_input == face) { continue; }
                if (
                   face_input.Vertex1 == face.Vertex1 ||
                   face_input.Vertex2 == face.Vertex2 ||
                   face_input.Vertex3 == face.Vertex3 ||
                   face_input.Vertex1 == face.Vertex3 ||
                   face_input.Vertex2 == face.Vertex3
                   ) { return true; }

            }
            return false;
        }
        private static void Linearize_()
        {
            foreach (INode node in Model.Nodes)
            {
                node.Translation.Type = MdxLib.Animator.EInterpolationType.Linear;
                node.Scaling.Type = MdxLib.Animator.EInterpolationType.Linear;
                node.Rotation.Type = MdxLib.Animator.EInterpolationType.Linear;
                if (node is CAttachment)
                {
                    CAttachment cAttachment = (CAttachment)node;
                    cAttachment.Visibility.Type = MdxLib.Animator.EInterpolationType.Linear;
                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;
                    element.Visibility.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.AttenuationEnd.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.AttenuationStart.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.Color.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.AmbientColor.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.Intensity.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.AmbientIntensity.Type = MdxLib.Animator.EInterpolationType.Linear;
                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                    element.Visibility.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.EmissionRate.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.LifeSpan.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.InitialVelocity.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.Gravity.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.Longitude.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.Latitude.Type = MdxLib.Animator.EInterpolationType.Linear;

                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    element.Visibility.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.Width.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.Length.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.Gravity.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.Variation.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.Latitude.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.EmissionRate.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.Speed.Type = MdxLib.Animator.EInterpolationType.Linear;

                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    element.Visibility.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.HeightAbove.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.Alpha.Type = MdxLib.Animator.EInterpolationType.Linear;
                    element.TextureSlot.Type = MdxLib.Animator.EInterpolationType.Linear;

                }
            }
            foreach (CCamera cam in Model.Cameras)
            {
                cam.Rotation.Type = MdxLib.Animator.EInterpolationType.Linear;
            }
            foreach (CMaterial mat in Model.Materials)
            {
                foreach (CMaterialLayer layer in mat.Layers)
                {
                    layer.Alpha.Type = MdxLib.Animator.EInterpolationType.Linear;
                    layer.TextureId.Type = MdxLib.Animator.EInterpolationType.Linear;
                }
            }
            foreach (CGeosetAnimation anim in Model.GeosetAnimations)
            {
                anim.Alpha.Type = MdxLib.Animator.EInterpolationType.Linear; ;
                anim.Color.Type = MdxLib.Animator.EInterpolationType.Linear;
            }
        }

        private static void EnumerateSequences_()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < Model.Sequences.Count; i++)
            {
                if (names.Contains(Model.Sequences[i].Name))
                {
                    Model.Sequences[i].Name = names[i] + " " + IDCounter.Next_();
                }
                names.Add(Model.Sequences[i].Name);
            }
        }
        private static void AddOrigin_()
        {
            if (Model.Nodes.Any(x => x is CAttachment && x.Name.ToLower() == "origin ref") == false)
            {

                INode node = new CAttachment(Model);
                node.Name = "Origin Ref";

                Model.Nodes.Add(node);
            }
        }
        public static void RearrangeKeyframes(CModel model) { Model = model; RearrangeKeyframes_(); }
        private static void RearrangeKeyframes_()
        {
            foreach (INode node in Model.Nodes)
            {
                Rearrange(node.Translation);
                Rearrange(node.Rotation);
                Rearrange(node.Scaling);
                
                if (node is CAttachment)
                {
                    CAttachment cAttachment = (CAttachment)node;
                    Rearrange(cAttachment.Visibility);  
                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;
                    Rearrange(element.Visibility);
                    Rearrange(element.AttenuationEnd);
                    Rearrange(element.AttenuationStart);
                    Rearrange(element.Color);
                    Rearrange(element.AmbientColor);
                    Rearrange(element.Intensity);
                    Rearrange(element.AmbientIntensity);
                     
                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;

                    Rearrange(element.Visibility);
                    Rearrange(element.EmissionRate);
                    Rearrange(element.LifeSpan);
                    Rearrange(element.InitialVelocity);
                    Rearrange(element.Gravity);
                    Rearrange(element.Longitude);
                    Rearrange(element.Latitude);
                     
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    Rearrange(element.Visibility);
                    Rearrange(element.EmissionRate);
                    Rearrange(element.Speed);
                    Rearrange(element.Gravity);
                    Rearrange(element.Variation);
                    Rearrange(element.Latitude);
                    Rearrange(element.Width);
                    Rearrange(element.Length);
                    
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    Rearrange(element.Visibility);
                    Rearrange(element.Color);
                    
                    Rearrange(element.Alpha);
                    Rearrange(element.HeightBelow);
                    
                    Rearrange(element.HeightAbove);
                    Rearrange(element.TextureSlot);

                    
                }
            }
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {
                Rearrange(ga.Alpha);
                Rearrange(ga.Color);
              
            }
            foreach (CMaterial material in Model.Materials)
            {
                foreach
                    (CMaterialLayer layer in material.Layers)
                {
                    Rearrange(layer.Alpha);
                    Rearrange(layer.TextureId);
                }
            }
            foreach (CTextureAnimation ta in Model.TextureAnimations)
            {
                Rearrange(ta.Translation);
                Rearrange(ta.Rotation);
                Rearrange(ta.Scaling);
            }
            foreach (CCamera camera in Model.Cameras)
            {
                Rearrange(camera.Rotation);
            }

        }

        private static void Rearrange(CAnimator<float> animation)
        {
            if (animation.Static == false) return;
            if (animation.Count < 2) return;

            // Bubble Sort
            for (int i = 0; i < animation.Count - 1; i++)
            {
                for (int j = 0; j < animation.Count - i - 1; j++)
                {
                    if (animation[j].Time > animation[j + 1].Time)
                    {
                        // Swap animation[j] and animation[j + 1]
                        var temp = animation[j];
                        animation[j] = animation[j + 1];
                        animation[j + 1] = temp;
                    }
                }
            }
        }
        private static void Rearrange(CAnimator<CVector3> animation)
        {
            if (animation.Static == false) return;
            if (animation.Count < 2) return;

            // Bubble Sort
            for (int i = 0; i < animation.Count - 1; i++)
            {
                for (int j = 0; j < animation.Count - i - 1; j++)
                {
                    if (animation[j].Time > animation[j + 1].Time)
                    {
                        // Swap animation[j] and animation[j + 1]
                        var temp = animation[j];
                        animation[j] = animation[j + 1];
                        animation[j + 1] = temp;
                    }
                }
            }
        }
        private static void Rearrange(CAnimator<CVector4> animation)
        {
            if (animation.Static == false) return;
            if (animation.Count < 2) return;

            // Bubble Sort
            for (int i = 0; i < animation.Count - 1; i++)
            {
                for (int j = 0; j < animation.Count - i - 1; j++)
                {
                    if (animation[j].Time > animation[j + 1].Time)
                    {
                        // Swap animation[j] and animation[j + 1]
                        var temp = animation[j];
                        animation[j] = animation[j + 1];
                        animation[j + 1] = temp;
                    }
                }
            }
        }
        private static void Rearrange(CAnimator<int> animation)
        {
            if (animation.Static == false) return;
            if (animation.Count < 2) return;

            // Bubble Sort
            for (int i = 0; i < animation.Count - 1; i++)
            {
                for (int j = 0; j < animation.Count - i - 1; j++)
                {
                    if (animation[j].Time > animation[j + 1].Time)
                    {
                        // Swap animation[j] and animation[j + 1]
                        var temp = animation[j];
                        animation[j] = animation[j + 1];
                        animation[j + 1] = temp;
                    }
                }
            }
        }

        private static void SetAllStaticGA()
        {
            for (int i = 0; i < Model.GeosetAnimations.Count; i++)
            {
                if (Model.GeosetAnimations[i].Alpha.Static)
                {
                    // Model.GeosetAnimations[i].Alpha.
                }
            }
        }
        private static void DeleteUnusedTextures_()
        {
            
            foreach (CTexture texture in Model.Textures.ToList())
            {
                if (TextureUsed(texture) == false)
                {
                    Model.Textures.Remove(texture);
                }
            }
        }
        private static bool TextureUsed(CTexture texture)
        {
           
            foreach (CMaterial material in Model.Materials)
            {
                foreach (CMaterialLayer layer in material.Layers)
                {
                    if (layer.Texture.Object == texture) { return true; }
                }

            }
            foreach (INode node in Model.Nodes)
            {
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 emitter = (CParticleEmitter2)node;    

                    if (emitter.Texture.Object == texture) { return true; }
                }

            }
            return false;
        }
        private static class IDCounter
        {
            private static int counter = 0;
            public static int Next() { counter++; return counter; }
            public static string Next_()
            {
                counter++; return counter.ToString();


            }
        }
        private static void RenameAllComponents_()
        {

        }
        private static void ClampAllUVCoordinates_()
        {
            foreach (CGeoset geoset in Model.Geosets.ToList())
            {
                for (int i = 0; i < geoset.Vertices.Count; i++)
                {
                    geoset.Vertices[i].TexturePosition = ClampUV(geoset.Vertices[i].TexturePosition);
                }

            }
        }
        private static void AddMissingPivotPoints_()
        {
            for (int i = 0; i < Model.Nodes.Count; i++)
            {
                if (Model.Nodes[i].PivotPoint == null)
                {
                    Model.Nodes[i].PivotPoint = new CVector3(0, 0, 0);
                }
            }
        }
        private static void MakeTransformationsWithZeroTracksStatic()
        {
            foreach (CMaterial mat in Model.Materials)
            {
                foreach (CMaterialLayer layer in mat.Layers)
                {
                    if (layer.Alpha.Static == false && layer.Alpha.Count == 0)
                    {
                        layer.Alpha.MakeStatic(1);
                    }
                    if (layer.TextureId.Static == false && layer.TextureId.Count == 0)
                    {
                        layer.TextureId.MakeStatic(0);
                    }
                }
              
            }
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {
                if (ga.Alpha.Static == false && ga.Alpha.Count == 0)
                {
                    ga.Alpha.MakeStatic(1);
                }
                if (ga.Color.Static == false && ga.Color.Count == 0)
                {
                    ga.Color.MakeStatic(new CVector3(1,1,1));
                }
            }
            foreach (CCamera cam in Model.Cameras)
            {
                if (cam.Rotation.Static == false && cam.Rotation. Count == 0)
                {
                    cam.Rotation.MakeStatic(0);
                }
            }
            foreach (INode node in Model.Nodes)
            {
                if (node is CLight) 
                {
                    CLight element = (CLight)node;
                    if (element.Visibility.Static == false && element.Visibility.Count == 0) { element.Visibility.MakeStatic(1); }
                    if (element.Color.Static == false && element.Color.Count == 0) { element.Color.MakeStatic(new CVector3(1,1,1)); }
                    if (element.AmbientColor.Static == false && element.Color.Count == 0) { element.AmbientColor.MakeStatic(new CVector3(1, 1, 1)); }
                    if (element.Intensity.Static == false && element.Intensity.Count == 0) { element.Intensity.MakeStatic(0); }
                    if (element.AmbientIntensity.Static == false && element.AmbientIntensity.Count == 0) { element.AmbientIntensity.MakeStatic(0); }
                    if (element.AttenuationStart.Static == false && element.AttenuationStart.Count == 0) { element.AttenuationStart.MakeStatic(80); }
                    if (element.AttenuationEnd.Static == false && element.AttenuationEnd.Count == 0) { element.AttenuationEnd.MakeStatic(200); }
                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                    if (element.Visibility.Static == false && element.Visibility.Count == 0) { element.Visibility.MakeStatic(1); }
                    if (element.EmissionRate.Static == false && element.EmissionRate.Count == 0) { element.EmissionRate.MakeStatic(0); }
                    if (element.LifeSpan.Static == false && element.LifeSpan.Count == 0) { element.LifeSpan.MakeStatic(0); }
                    if (element.InitialVelocity.Static == false && element.InitialVelocity.Count == 0) { element.InitialVelocity.MakeStatic(0); }
                    if (element.Gravity.Static == false && element.Gravity.Count == 0) { element.Gravity.MakeStatic(0); }
                    if (element.Longitude.Static == false && element.Longitude.Count == 0) { element.Longitude.MakeStatic(0); }
                    if (element.Latitude.Static == false && element.Latitude.Count == 0) { element.Latitude.MakeStatic(0); }
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    if (element.Visibility.Static == false && element.Visibility.Count == 0) { element.Visibility.MakeStatic(1); }
                    if (element.Speed.Static == false && element.Speed.Count == 0) { element.Speed.MakeStatic(0); }
                    if (element.Width.Static == false && element.Width.Count == 0) { element.Width.MakeStatic(0); }
                    if (element.Length.Static == false && element.Length.Count == 0) { element.Length.MakeStatic(0); }
                    if (element.Variation.Static == false && element.Variation.Count == 0) { element.Variation.MakeStatic(0); }
                    if (element.EmissionRate.Static == false && element.Variation.Count == 0) { element.EmissionRate.MakeStatic(0); }
                    if (element.Gravity.Static == false && element.Gravity.Count == 0) { element.Gravity.MakeStatic(0); }
                    if (element.Latitude.Static == false && element.Latitude.Count == 0) { element.Latitude.MakeStatic(0); }
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    if (element.Visibility.Static == false && element.Visibility.Count == 0) { element.Visibility.MakeStatic(1); }
                    if (element.HeightAbove.Static == false && element.HeightAbove.Count == 0) { element.HeightAbove.MakeStatic(0); }
                    if (element.HeightBelow.Static == false && element.HeightBelow.Count == 0) { element.HeightBelow.MakeStatic(0); }
                    if (element.Color.Static == false && element.HeightBelow.Count == 0) { element.Color.MakeStatic(new CVector3(1,1,1)); }
                    if (element.TextureSlot.Static == false && element.TextureSlot.Count == 0) { element.TextureSlot.MakeStatic(0); }
                }

            }
             
        }
        private static MdxLib.Primitives.CVector2 ClampUV(CVector2 value)
        {
            CVector2 result = new CVector2();
            float u_int = value.X;
            float u_frac = u_int - (float)Math.Truncate(u_int);
            float v_int = value.Y;
            float v_frac = v_int - (float)Math.Truncate(v_int);
            float new_U = value.X; ;
            float new_V = value.Y;
            if (value.X < -10)
            {
                new_U = -9 - u_frac;
            }
            if (value.X > 10)
            {
                new_U = -9 + u_frac;
            }
            if (value.Y < -10)
            {
                new_V = -9 - v_frac;
            }
            if (value.Y > 10)
            {
                new_V = -9 + v_frac;
            }
            return new CVector2(new_U, new_V);
        }
        private static void Delete0LengthGlobalSequences_()
        {
            foreach (CGlobalSequence gs in Model.GlobalSequences.ToList())
            {
                if (gs.Duration == 0)
                {
                    Model.GlobalSequences.Remove(gs);
                }
            }
        }
        private static void DeleteUnusedMaterials_()
        {
            foreach (CMaterial mat in  Model.Materials.ToList())
            {
                if (MaterialUsed(mat) == false)
                {
                    Model.Materials.Remove(mat);
                }

            }
        }
        private static bool MaterialUsed(CMaterial mat)
        {
           
            foreach (CGeoset geo in Model.Geosets)
            {
                if (geo.Material.Object == mat  || geo.Material.ObjectId == mat.ObjectId) { return true; }
            }
            return false;
        }


        private static void DeleteUnusedHelpers_()
        {
            foreach (INode node in Model.Nodes.ToList())
            {
                if (node is CHelper)
                {
                    if (HasChildren(node))
                    {
                        Model.Nodes.Remove(node);
                    }
                }
            }
        }
        private static bool HasChildren(INode Inputnode)
        {
            foreach (INode node in Model.Nodes)
            {
                if (node.Parent.Node == Inputnode) { return true; }
            }
            return false;
        }
        private static void DeleteUnusedGlobalSequences_()
        {
            foreach (CGlobalSequence gs in Model.GlobalSequences.ToList())
            {
                if (gs.HasReferences == false)
                {
                    Model.GlobalSequences.Remove(gs);
                }
            }
        }

        public static void DeleteEventObjectsWithNoTRacks()
        {
             
           
            foreach (INode node in Model.Nodes.ToList())
            {
                if (node is CEvent)
                {
                    CEvent _event = (CEvent)node;
                    if (_event.Tracks.Count == 0)
                    {
                        Model.Nodes.Remove(node);
                    }
                }
            }

        }
        private static void RearrangeSEquences()
        {
            List<CSequence> sequences = new List<CSequence>();
            foreach (CSequence seq in Model.Sequences)
            {
                sequences.Add(seq);
            }
            sequences = sequences.OrderBy(seq => seq.IntervalStart).ToList();
            foreach (CSequence seq in Model.Sequences.ToList())
            {
                Model.Sequences.Remove(seq);
            }
            Model.Sequences.Clear();
            foreach (CSequence seq in sequences)
            {
                Model.Sequences.Add(seq);
            }
             
        }
       
      
       
      
        private static void Delete0LengthSequences_()
        {
            foreach (CSequence sequence in Model.Sequences.ToList())
            {
                if (sequence.IntervalStart == sequence.IntervalEnd)
                {
                    Model.Sequences.Remove(sequence);
                }
                if (sequence.IntervalStart > sequence.IntervalEnd) { Model.Sequences.Remove(sequence); }
                if (sequence.IntervalStart < 0) { Model.Sequences.Remove(sequence); }
            }
        }
        private static void ClampAllLightAttentuation_()
        {
            foreach (INode node in Model.Nodes)
            {
                if (node is CLight)
                {
                    CLight _light = (CLight)node;
                    if (_light.AttenuationStart.Static == true)
                    {
                        // _light.AttenuationStart.
                    }
                }
            }
        }
        private static void MakeVisibilitiesNone_()
        {
            foreach (INode node in Model.Nodes)
            {
                if (node is CAttachment)
                {
                    CAttachment att = (CAttachment)node;
                    att.Visibility.Type = MdxLib.Animator.EInterpolationType.None;
                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter att = (CParticleEmitter)node;
                    att.Visibility.Type = MdxLib.Animator.EInterpolationType.None;
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 att = (CParticleEmitter2)node;
                    att.Visibility.Type = MdxLib.Animator.EInterpolationType.None;
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter att = (CRibbonEmitter)node;
                    att.Visibility.Type = MdxLib.Animator.EInterpolationType.None;
                }
                if (node is CLight)
                {
                    CLight att = (CLight)node;
                    att.Visibility.Type = MdxLib.Animator.EInterpolationType.None;
                }
            }

        }
    }
}
