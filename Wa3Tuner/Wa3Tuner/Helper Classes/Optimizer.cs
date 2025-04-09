using MdxLib.Animator;
using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using Wa3Tuner.Helper_Classes;

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
        public static bool AddMissingPivotPoints = true;
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
        internal static bool AddDefaultMissingOpeningKeyframes;
        internal static bool DeleteSimilarSimilarKEyframes = false;
        internal static bool _DetachFromNonBone;
        internal static bool Check_DeleteIdenticalAdjascentKEyframes_times = false;
        internal static bool DleteOverlapping1 = false;
        internal static bool DleteOverlapping2 = false;
        internal static bool InvalidTriangleUses = false;
        internal static bool ClampNormals = false;
        internal static bool DeleteTrianglesWithNoArea;
        internal static bool DeleteDuplicateGEosets = false;
        internal static bool MergeTextures = false;
        internal static bool MergeMAtertials = false;
        internal static bool MergeTAs = false;
        internal static bool MergeLayers = false;
        internal static bool MinimizeMatrixGroups = false;
        internal static bool FixNoMatrixGroups = false;
        internal static bool StretchKeyframes =false;
        internal static bool AddDefaultMissingClosingKeyframes = false;
        internal static bool MoveFirstKeyframeToStart = false;
        internal static bool MoveLastKeyframeToEnd = false;
        internal static bool DuplicateFirstKEyframeToStart = false;
        internal static bool DuplicateLastKeyframeToEnd = false;
        internal static bool OtherMissingKeyframes = false;

        public static bool MergeIdenticalVertices = false;
        internal static bool ReducematrixGruops = false;
        internal static bool TimeMiddle = false;

        public static void Optimize(CModel model_)
        {
            Model = model_;
            RearrangeSEquences();
            RearrangeKeyframes_();
            RemoveEmptyGeosets();
            FixInvalidNodeRelationships();
            CreateLayerForMaterialsWithout();
            if (Linearize) { Linearize_(); }
            if (TimeMiddle) { TimeMiddle_(); }
            if (DeleteIsolatedTriangles) { DeleteIsolatedTriangles_(); }
            if (DeleteIsolatedVertices) { DeleteIsolatedVertices_(); }
            if (Delete0LengthSequences) { Delete0LengthSequences_(); }
            if (Delete0LengthGlobalSequences) { Delete0LengthGlobalSequences_(); }
           
            if (DeleteUnusedHelpers) { DeleteUnusedHelpers_(); }
            if (DeleteUnusedMAterials) { DeleteUnusedMaterials_(); }
            if (DeleteUnusedTextures) { DeleteUnusedTextures_(); }
            if (RenameAllComponents) { RenameAllComponents_(); }
            if (ClampAllUVCoordinates) { ClampAllUVCoordinates_(); }
            if (AddMissingPivotPoints) { AddMissingPivotPoints_(); }
            if (MakeVisibilitiesNone) { MakeVisibilitiesNone_(); }
            if (EnumerateSequences) { EnumerateSequences_(); }
            if (AddOrigin) { AddOrigin_(); }
            if (DeleteUnAnimatedSequences) { DeleteUnAnimatedSequences_(); }
            if (DeleteUnusedTextureAnimations) { DeleteUnusedTextureAnimations_(); }
            if (ClampKeyframes) { ClampKeyframes_(); }
            if (MergeGeosets) { MergeGeosets_(); }
            if (AddMissingVisibilities) { AddMissingVisibilities_(); }
            if (ClampUVs) { ClampUVs_(); }
            if (ClampLights) { ClampLights_(); }
            if (DeleteDuplicateGAs) { DeleteDuplicateGAs_(); }
            if (AddMissingGAs) { AddMissingGAs_(); }
            if (SetAllStaticGAS) { SetAllStaticGAS_(); }
            if (CalculateExtents) { CalculateExtents_(new List<int>()); }
            if (DeleteIdenticalAdjascentKEyframes) { DeleteIdenticalAdjascentKEyframes_(); }
            if (Check_DeleteIdenticalAdjascentKEyframes_times) { Check_DeleteIdenticalAdjascentKEyframes_times_(); }
           if (DeleteSimilarSimilarKEyframes) { DeleteSimilarSimilarKEyframes_(); }
            if (DeleteUnusedKeyframes) { DeleteUnusedKeyframes_(); }
            if (DeleteUnusedGlobalSequences) { DeleteUnusedGlobalSequences_(); }
            if (_DetachFromNonBone) { _DetachFromNonBone_(); }
            if (ReducematrixGruops) { ReducematrixGruops_(); }
            if (DleteOverlapping1) DeleteIdenticalFaces();
            if (DleteOverlapping2) DeleteFullyOverLappingFaces();
            if (InvalidTriangleUses) InvalidTriangleUses_();
            if (ClampNormals) ClampNormals_();
            if (DeleteTrianglesWithNoArea) DeleteTrianglesWithNoArea_();
            if (MergeIdenticalVertices) MergeIdenticalVertices_();
            if (DeleteDuplicateGEosets) DeleteDuplicateGEosets_();
            if (MergeTextures) MergeTextures_();
            if (MergeLayers) MergeLayers_();
            if (MergeMAtertials) MergeMatertials_();
            if (MergeTAs) MergeTAs_();
            if (MinimizeMatrixGroups) MinimizeMatrixGroups_();
            if (FixNoMatrixGroups) FixNoMatrixGroups_();
            if (DelUnusedMatrixGroups) { DelUnusedMatrixGroups_(); }
            if (DeleteUnusedBones) { DeleteUnusedBones_(); }
            //HandleMissingKeyframes();
            FillMissingComponents();
           MakeDynamicTransformationsWithOneOfNoKeyframeStatic();
            RearrangeKeyframes_();
           // MakeTransformationsWithZeroTracksStatic(); //unused
            FixSquirtOfEmitters2();
            DeleteEmptyGeosets();
            RemoveEmptyGeosetAnimations();
            DeleteEventObjectsWithNoTRacks();
           
        }

        private static void TimeMiddle_()
        {
           foreach (var node in Model.Nodes)
            {
                if (node is CParticleEmitter2 e)
                {
                    if (e.Time < 0 || e.Time > 1)
                    {
                        e.Time = 1;
                    }
                   
                }
            }
        }


        public static void  HandleInvalidTextures(CModel model)
        {
              string White = "Textures\\white.blp";
            List<int> ids = new List<int>() { 0, 1, 2, 11, 31, 32, 33, 34, 35, 36, 37 };
            foreach (var t in model.Textures)
            {
                if (ids.Contains(t.ReplaceableId) == false)
                {
                    t.ReplaceableId = 0;
                    t.FileName = White;
                }
               
            }
        }
        private static void ReducematrixGruops_()
        {
            foreach (var geoset in Model.Geosets)
            {
                
                foreach (var group in geoset.Groups)
                {
                    if (group.Nodes.Count <= 1) { continue; }
                    List<CGeosetGroupNode> uniqueNodes = new List<CGeosetGroupNode>();
                    foreach (var node in group.Nodes.ToList())
                    {
                        if (uniqueNodes.Contains(node))
                        {
                            group.Nodes.Remove(node);
                        }
                        else
                        {
                            uniqueNodes.Add(node);
                        }
                    }
                }
            }
        }

        private static void HandleMissingKeyframes()
        {
            if (StretchKeyframes)
            {
                StretchAllKeyframes();
            }
            else if (OtherMissingKeyframes)
            {
               AddMissingKeyframesWithDefault(AddDefaultMissingOpeningKeyframes, AddDefaultMissingClosingKeyframes);
                DuplicateForMissingStartingEndingKeyframes(DuplicateFirstKEyframeToStart, DuplicateLastKeyframeToEnd);
                MoveMissingStartingEndingKeyframes(MoveFirstKeyframeToStart, MoveLastKeyframeToEnd);
            }
             
        }

        private static void MoveMissingStartingEndingKeyframes(bool starting, bool ending)
        {
            if (!starting && !ending) { return; }
            foreach (CMaterial material in Model.Materials)
            {
                foreach (CMaterialLayer layer in material.Layers)
                {
                    if (starting)
                    {
                        MoveMissingStartingTracks(layer.Alpha);
                        MoveMissingStartingTracks(layer.TextureId);
                    }
                    if (ending)
                    {
                        AddMissingEndingTracks(layer.Alpha);
                        AddMissingEndingTracks(layer.TextureId);
                    }

                }
            }
            foreach (INode node in Model.Nodes)
            {
                if (starting)
                {
                    MoveMissingStartingTracks(node.Translation);
                    MoveMissingStartingTracks(node.Rotation);
                    MoveMissingStartingTracks(node.Scaling);
                }
                if (ending)
                {
                    AddMissingEndingTracks(node.Translation);
                    AddMissingEndingTracks(node.Rotation);
                    AddMissingEndingTracks(node.Scaling);
                }

                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                    if (starting)
                    {
                        MoveMissingStartingTracks(element.EmissionRate);
                        MoveMissingStartingTracks(element.LifeSpan);
                        MoveMissingStartingTracks(element.InitialVelocity);
                        MoveMissingStartingTracks(element.Gravity);
                        MoveMissingStartingTracks(element.Longitude);
                        MoveMissingStartingTracks(element.Latitude);
                    }
                    if (ending)
                    {
                        AddMissingEndingTracks(element.EmissionRate);
                        AddMissingEndingTracks(element.LifeSpan);
                        AddMissingEndingTracks(element.InitialVelocity);
                        AddMissingEndingTracks(element.Gravity);
                        AddMissingEndingTracks(element.Longitude);
                        AddMissingEndingTracks(element.Latitude);
                    }

                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    if (starting)
                    {
                        MoveMissingStartingTracks(element.EmissionRate);
                        MoveMissingStartingTracks(element.Speed);
                        MoveMissingStartingTracks(element.Variation);
                        MoveMissingStartingTracks(element.Latitude);
                        MoveMissingStartingTracks(element.Width);
                        MoveMissingStartingTracks(element.Length);
                        MoveMissingStartingTracks(element.Gravity);
                    }
                    if (ending)
                    {
                        MoveMissingEndingTracks(element.EmissionRate);
                        MoveMissingEndingTracks(element.Speed);
                        MoveMissingEndingTracks(element.Variation);
                        MoveMissingEndingTracks(element.Latitude);
                        MoveMissingEndingTracks(element.Width);
                        MoveMissingEndingTracks(element.Length);
                        MoveMissingEndingTracks(element.Gravity);
                    }

                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    if (starting)
                    {
                        MoveMissingStartingTracks(element.HeightAbove);
                        MoveMissingStartingTracks(element.HeightBelow);
                        MoveMissingStartingTracks(element.Color);
                        MoveMissingStartingTracks(element.Alpha);
                        MoveMissingStartingTracks(element.TextureSlot);
                    }
                    if (ending)
                    {
                        MoveMissingEndingTracks(element.HeightAbove);
                        MoveMissingEndingTracks(element.HeightBelow);
                        MoveMissingEndingTracks(element.Color);
                        MoveMissingEndingTracks(element.Alpha);
                        MoveMissingEndingTracks(element.TextureSlot);
                    }

                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;
                    if (starting)
                    {
                        MoveMissingStartingTracks(element.Color);
                        MoveMissingStartingTracks(element.AmbientColor);
                        MoveMissingStartingTracks(element.Intensity);
                        MoveMissingStartingTracks(element.AmbientIntensity);
                        MoveMissingStartingTracks(element.AttenuationEnd);
                        MoveMissingStartingTracks(element.AttenuationStart);
                    }
                    if (ending)
                    {
                        MoveMissingEndingTracks(element.Color);
                        MoveMissingEndingTracks(element.AmbientColor);
                        MoveMissingEndingTracks(element.Intensity);
                        MoveMissingEndingTracks(element.AmbientIntensity);
                        MoveMissingEndingTracks(element.AttenuationEnd);
                        MoveMissingEndingTracks(element.AttenuationStart);
                    }

                }
            }
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {
                if (starting)
                {
                    MoveMissingStartingTracks(ga.Alpha);
                    MoveMissingStartingTracks(ga.Color);
                }
                if (ending)
                {
                    MoveMissingEndingTracks(ga.Alpha);
                    MoveMissingEndingTracks(ga.Color);
                }

            }
            foreach (CTextureAnimation ta in Model.TextureAnimations)
            {
                if (starting)
                {
                    MoveMissingStartingTracks(ta.Translation);
                    MoveMissingStartingTracks(ta.Rotation);
                    MoveMissingStartingTracks(ta.Scaling);
                }
                if (ending)
                {
                    MoveMissingEndingTracks(ta.Translation);
                    MoveMissingEndingTracks(ta.Rotation);
                    MoveMissingEndingTracks(ta.Scaling);
                }

            }

        }

        private static void MoveMissingEndingTracks(CAnimator<CVector3> animator)
        {
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<CVector3>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes[keyframes.Count-1].Time != to)
                        {
                            keyframes[keyframes.Count - 1].Time = to;
                        }
                    }
                }
            }
        }
        private static void MoveMissingEndingTracks(CAnimator<CVector4> animator)
        {
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<CVector4>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes[keyframes.Count - 1].Time != to)
                        {
                            keyframes[keyframes.Count - 1].Time = to;
                        }
                    }
                }
            }
        }
        private static void MoveMissingEndingTracks(CAnimator<float> animator)
        {
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<float>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes[keyframes.Count - 1].Time != to)
                        {
                            keyframes[keyframes.Count - 1].Time = to;
                        }
                    }
                }
            }
        }
        private static void MoveMissingEndingTracks(CAnimator<int> animator)
        {
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<int>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes[keyframes.Count - 1].Time != to)
                        {
                            keyframes[keyframes.Count - 1].Time = to;
                        }
                    }
                }
            }
        }

        private static void MoveMissingStartingTracks(CAnimator<CVector3> animator)
        {
            if (animator.Static == true) { return; }
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<CVector3>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes[0].Time != from)
                        {
                            keyframes[0].Time = from;
                        }
                    }
                }
            }
            
          
            
        }
        private static void MoveMissingStartingTracks(CAnimator<CVector4> animator)
        {
            if (animator.Static == true) { return; }
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<CVector4>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes[0].Time != from)
                        {
                            keyframes[0].Time = from;
                        }
                    }
                }
            }

        }
        private static void MoveMissingStartingTracks(CAnimator<float> animator)
        {
            if (animator.Static == true) { return; }
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<float>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes[0].Time != from)
                        {
                            keyframes[0].Time = from;
                        }
                    }
                }
            }

        }
        private static void MoveMissingStartingTracks(CAnimator<int> animator)
        {
            if (animator.Static == true) { return; }
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<int>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes[0].Time != from)
                        {
                            keyframes[0].Time = from;
                        }
                    }
                }
            }

        }

        private static void DuplicateForMissingStartingEndingKeyframes(bool starting, bool ending)
        {
           
            if (!starting && !ending) { return; }
            foreach (CMaterial material in Model.Materials)
            {
                foreach (CMaterialLayer layer in material.Layers)
                {
                    if (starting)
                    {
                        DuplicateMissingStartingTracks(layer.Alpha);
                        DuplicateMissingStartingTracks(layer.TextureId);
                    }
                    if (ending)
                    {
                        DuplicateMissingEndingTracks(layer.Alpha);
                        DuplicateMissingEndingTracks(layer.TextureId);
                    }

                }
            }
            foreach (INode node in Model.Nodes)
            {
                if (starting)
                {
                    DuplicateMissingStartingTracks(node.Translation);
                    DuplicateMissingStartingTracks(node.Rotation);
                    DuplicateMissingStartingTracks(node.Scaling);
                }
                if (ending)
                {
                    DuplicateMissingEndingTracks(node.Translation);
                    DuplicateMissingEndingTracks(node.Rotation);
                    DuplicateMissingEndingTracks(node.Scaling);
                }

                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                    if (starting)
                    {
                        DuplicateMissingStartingTracks(element.EmissionRate);
                        DuplicateMissingStartingTracks(element.LifeSpan);
                        DuplicateMissingStartingTracks(element.InitialVelocity);
                        DuplicateMissingStartingTracks(element.Gravity);
                        DuplicateMissingStartingTracks(element.Longitude);
                        DuplicateMissingStartingTracks(element.Latitude);
                    }
                    if (ending)
                    {
                        DuplicateMissingEndingTracks(element.EmissionRate);
                        DuplicateMissingEndingTracks(element.LifeSpan);
                        DuplicateMissingEndingTracks(element.InitialVelocity);
                        DuplicateMissingEndingTracks(element.Gravity);
                        DuplicateMissingEndingTracks(element.Longitude);
                        DuplicateMissingEndingTracks(element.Latitude);
                    }

                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    if (starting)
                    {
                        DuplicateMissingStartingTracks(element.EmissionRate);
                        DuplicateMissingStartingTracks(element.Speed);
                        DuplicateMissingStartingTracks(element.Variation);
                        DuplicateMissingStartingTracks(element.Latitude);
                        DuplicateMissingStartingTracks(element.Width);
                        DuplicateMissingStartingTracks(element.Length);
                        DuplicateMissingStartingTracks(element.Gravity);
                    }
                    if (ending)
                    {
                        DuplicateMissingEndingTracks(element.EmissionRate);
                        DuplicateMissingEndingTracks(element.Speed);
                        DuplicateMissingEndingTracks(element.Variation);
                        DuplicateMissingEndingTracks(element.Latitude);
                        DuplicateMissingEndingTracks(element.Width);
                        DuplicateMissingEndingTracks(element.Length);
                        DuplicateMissingEndingTracks(element.Gravity);
                    }

                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    if (starting)
                    {
                        DuplicateMissingStartingTracks(element.HeightAbove);
                        DuplicateMissingStartingTracks(element.HeightBelow);
                        DuplicateMissingStartingTracks(element.Color);
                        DuplicateMissingStartingTracks(element.Alpha);
                        DuplicateMissingStartingTracks(element.TextureSlot);
                    }
                    if (ending)
                    {
                        DuplicateMissingEndingTracks(element.HeightAbove);
                        DuplicateMissingEndingTracks(element.HeightBelow);
                        DuplicateMissingEndingTracks(element.Color);
                        DuplicateMissingEndingTracks(element.Alpha);
                        DuplicateMissingEndingTracks(element.TextureSlot);
                    }

                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;
                    if (starting)
                    {
                        DuplicateMissingStartingTracks(element.Color);
                        DuplicateMissingStartingTracks(element.AmbientColor);
                        DuplicateMissingStartingTracks(element.Intensity);
                        DuplicateMissingStartingTracks(element.AmbientIntensity);
                        DuplicateMissingStartingTracks(element.AttenuationEnd);
                        DuplicateMissingStartingTracks(element.AttenuationStart);
                    }
                    if (ending)
                    {
                        DuplicateMissingEndingTracks(element.Color);
                        DuplicateMissingEndingTracks(element.AmbientColor);
                        DuplicateMissingEndingTracks(element.Intensity);
                        DuplicateMissingEndingTracks(element.AmbientIntensity);
                        DuplicateMissingEndingTracks(element.AttenuationEnd);
                        DuplicateMissingEndingTracks(element.AttenuationStart);
                    }

                }
            }
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {
                if (starting)
                {
                    DuplicateMissingStartingTracks(ga.Alpha);
                    DuplicateMissingStartingTracks(ga.Color);
                }
                if (ending)
                {
                    DuplicateMissingEndingTracks(ga.Alpha);
                    DuplicateMissingEndingTracks(ga.Color);
                }

            }
            foreach (CTextureAnimation ta in Model.TextureAnimations)
            {
                if (starting)
                {
                    DuplicateMissingStartingTracks(ta.Translation);
                    DuplicateMissingStartingTracks(ta.Rotation);
                    DuplicateMissingStartingTracks(ta.Scaling);
                }
                if (ending)
                {
                    DuplicateMissingEndingTracks(ta.Translation);
                    DuplicateMissingEndingTracks(ta.Rotation);
                    DuplicateMissingEndingTracks(ta.Scaling);
                }

            }
        }

        private static void DuplicateMissingEndingTracks(CAnimator<float> animator)
        {
            if (animator.Static == true) { return; }
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<float>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes.Any(x => x.Time != from))
                        {
                            var first = keyframes.Last();
                            CAnimatorNode<float> node = new CAnimatorNode<float>();
                            node.Time = to;
                            node.Value = first.Value;
                            node.InTangent = first.InTangent;
                            node.OutTangent = first.OutTangent;
                            animator.NodeList.Add(node);
                            animator.NodeList = animator.NodeList.OrderBy(x => x.Time).ToList();
                        }
                    }
                }
            }
        }
        private static void DuplicateMissingEndingTracks(CAnimator<int> animator)
        {
            if (animator.Static == true) { return; }
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<int>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes.Any(x => x.Time != from))
                        {
                            var first = keyframes.Last();
                            CAnimatorNode<int> node = new CAnimatorNode<int>();
                            node.Time = to;
                            node.Value = first.Value;
                            node.InTangent = first.InTangent;
                            node.OutTangent = first.OutTangent;
                            animator.NodeList.Add(node);
                            animator.NodeList = animator.NodeList.OrderBy(x => x.Time).ToList();
                        }
                    }
                }
            }
        }
        private static void DuplicateMissingEndingTracks(CAnimator<CVector3> animator)
        {
            if (animator.Static == true) { return; }
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<CVector3>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes.Any(x => x.Time != from))
                        {
                            var first = keyframes.Last();
                            CAnimatorNode<CVector3> node = new CAnimatorNode<CVector3>();
                            node.Time = to;
                            node.Value = new CVector3( first.Value);
                            node.InTangent =new CVector3( first.InTangent);
                            node.OutTangent =new CVector3( first.OutTangent);
                            animator.NodeList.Add(node);
                            animator.NodeList = animator.NodeList.OrderBy(x => x.Time).ToList();
                        }
                    }
                }
            }
        }
        private static void DuplicateMissingEndingTracks(CAnimator<CVector4> animator)
        {
            if (animator.Static == true) { return; }
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<CVector4>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes.Any(x => x.Time != from))
                        {
                            var first = keyframes.Last();
                            CAnimatorNode<CVector4> node = new CAnimatorNode<CVector4>();
                            node.Time = to;
                            node.Value = new CVector4(first.Value);
                            node.InTangent = new CVector4(first.InTangent);
                            node.OutTangent = new CVector4(first.OutTangent);
                            animator.NodeList.Add(node);
                            animator.NodeList = animator.NodeList.OrderBy(x => x.Time).ToList();
                        }
                    }
                }
            }
        }
        private static void DuplicateMissingStartingTracks(CAnimator<float> animator)
        {
            if (animator.Static == true) { return; }
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<float>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes.Any(x=>x.Time != from))
                        {
                            var first = keyframes.First();
                            CAnimatorNode<float> node = new CAnimatorNode<float>();
                            node.Time = from;
                            node.Value = first.Value;
                            node.InTangent = first.InTangent;
                            node.OutTangent = first.OutTangent;
                            animator.NodeList.Add(node);
                            animator.NodeList = animator.NodeList.OrderBy(x=>x.Time).ToList();
                        }
                    }
                }
            }
        }
        private static void DuplicateMissingStartingTracks(CAnimator<int> animator)
        {
            if (animator.Static == true) { return; }
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<int >> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes.Any(x => x.Time != from))
                        {
                            var first = keyframes.First();
                            CAnimatorNode<int> node = new CAnimatorNode<int>();
                            node.Time = from;
                            node.Value = first.Value;
                            node.InTangent = first.InTangent;
                            node.OutTangent = first.OutTangent;
                            animator.NodeList = animator.NodeList.OrderBy(x => x.Time).ToList();
                        }
                    }
                }
            }
        }
        private static void DuplicateMissingStartingTracks(CAnimator<CVector3> animator)
        {
            if (animator.Static == true) { return; }
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<CVector3>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes.Any(x => x.Time != from))
                        {
                            var first = keyframes.First();
                            CAnimatorNode<CVector3> node = new CAnimatorNode<CVector3>();
                            node.Time = from;
                            node.Value = new CVector3(first.Value);
                            
                            node.InTangent = new CVector3(first.InTangent);
                            node.OutTangent = new CVector3(first.OutTangent);
                            animator.NodeList = animator.NodeList.OrderBy(x => x.Time).ToList();
                        }
                    }
                }
            }
        }
        private static void DuplicateMissingStartingTracks(CAnimator<CVector4> animator)
        {
            if (animator.Static == true) { return; }
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<CVector4>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes.Any(x => x.Time != from))
                        {
                            var first = keyframes.First();
                            CAnimatorNode<CVector4> node = new CAnimatorNode<CVector4>();
                            node.Time = from;
                            node.Value = new CVector4(first.Value);

                            node.InTangent = new CVector4(first.InTangent);
                            node.OutTangent = new CVector4(first.OutTangent);
                            animator.NodeList = animator.NodeList.OrderBy(x => x.Time).ToList();
                        }
                    }
                }
            }
        }
        private static void FixNoMatrixGroups_()
        {
           foreach (var geoset in Model.Geosets)
            {
                if (geoset.Groups.Count == 0)
                {
                    CBone dummy = new CBone(Model);
                    dummy.Name = $"DummyBone_geoset_{geoset.ObjectId}_{IDCounter.Next}";
                    CGeosetGroup group = new CGeosetGroup(Model);
                    CGeosetGroupNode node = new CGeosetGroupNode(Model);
                    node.Node.Attach(dummy);
                    group.Nodes.Add(node);  
                    geoset.Groups.Add(group);
                    foreach (CGeosetVertex vertex in geoset.Vertices)
                    {
                        vertex.Group.Attach(group);
                    }
                }
            }
        }
        public static void RemoveInvalidGeosetAnimations(CModel model)
        {
            foreach (CGeosetAnimation ga in model.GeosetAnimations.ToList())
            {
                if (ga.Geoset == null)
                {
                    model.GeosetAnimations.Remove(ga); continue;
                }
                if (ga.Geoset.Object == null)
                {
                    model.GeosetAnimations.Remove(ga); continue;
                }
                if (model.Geosets.Contains(ga.Geoset.Object) == false)
                {
                    model.GeosetAnimations.Remove(ga); continue;
                }
            }
        }
        public static void MinimizeMatrixGroups_(CModel model = null)
        {
            CModel WhichModel = model == null ? Model : model;
            foreach (CGeoset geo in WhichModel.Geosets)
            {
                repeat:
                if (geo.Groups.Count <= 1) continue;
                for (int i = 0; i < geo.Groups.Count; i++)
                {
                    if (i+1  >= geo.Groups.Count ) continue;
                    if (MatrixGroupsAreSame(geo.Groups[i], geo.Groups[i+1]))
                    {
                        ReassignGroup(geo, geo.Groups[i], geo.Groups[i + 1]);
                        geo.Groups.Remove(geo.Groups[i + 1]);
                        goto repeat;
                    }
                }
            }
        }
        private static void ReassignGroup(CGeoset geo, CGeosetGroup attach, CGeosetGroup useless)
        {
            foreach (var vertex in geo.Vertices)
            {
                if (vertex.Group.Object == useless) {
                    vertex.Group.Attach(attach);
                }
            }
        }
        private static bool MatrixGroupsAreSame(CGeosetGroup one, CGeosetGroup two)
        {
            if (one.Nodes.Count != two.Nodes.Count)
                return false;

            // Use a HashSet for O(1) lookup time
            HashSet<INode> nodes = new HashSet<INode>(one.Nodes.Select(n => n.Node.Node));

            // Ensure all nodes in 'two' exist in 'one'
            foreach (var node in two.Nodes)
            {
                if (!nodes.Contains(node.Node.Node))
                    return false;
            }

            return true;
        }

        private static void MakeDynamicTransformationsWithOneOfNoKeyframeStatic()
        {
            foreach (INode node in Model.Nodes)
            {
               
                if (node is CAttachment)
                {
                    CAttachment cAttachment = (CAttachment)node;
                    ClampStatic(cAttachment.Visibility);
                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;
                    ClampStatic(element.Visibility);
                    ClampStatic(element.AttenuationEnd);
                    ClampStatic(element.AttenuationStart);
                    ClampStatic(element.Color);
                    ClampStatic(element.AmbientColor);
                    ClampStatic(element.Intensity);
                    ClampStatic(element.AmbientIntensity);
                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                    ClampStatic(element.Visibility);
                    ClampStatic(element.EmissionRate);
                    ClampStatic(element.LifeSpan);
                    ClampStatic(element.InitialVelocity);
                    ClampStatic(element.Gravity);
                    ClampStatic(element.Longitude);
                    ClampStatic(element.Latitude);
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    ClampStatic(element.Visibility);
                    ClampStatic(element.EmissionRate);
                    ClampStatic(element.Speed);
                    ClampStatic(element.Gravity);
                    ClampStatic(element.Variation);
                    ClampStatic(element.Latitude);
                    ClampStatic(element.Width);
                    ClampStatic(element.Length);
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    ClampStatic(element.Visibility);
                    ClampStatic(element.Color);
                    ClampStatic(element.Alpha);
                    ClampStatic(element.HeightBelow);
                    ClampStatic(element.HeightAbove);
                    ClampStatic(element.TextureSlot);
                }
            }
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {
                ClampStatic(ga.Alpha, 1);
                ClampStatic(ga.Color, 1,1,1);
            }
            foreach (CMaterial material in Model.Materials)
            {
                foreach
                    (CMaterialLayer layer in material.Layers)
                {
                    ClampStatic(layer.Alpha, 1);
                    ClampStatic(layer.TextureId);
                }
            }
            foreach (CTextureAnimation ta in Model.TextureAnimations)
            {
                ClampStatic(ta.Translation);
                ClampStatic(ta.Rotation);
                ClampStatic(ta.Scaling,1,1,1);
            }
            foreach (CCamera camera in Model.Cameras)
            {
                ClampStatic(camera.Rotation);
                ClampStatic(camera.Translation);
                ClampStatic(camera.TargetTranslation);
            }
        }
        private static void ClampStatic(CAnimator<float> animator, float def = 0)
        {
            float d = 0;
            if (animator.Count == 1) { d = animator.NodeList[0].Value; }
            if (animator.Static == false && animator.Count <= 1)
            {
                animator.Clear();
                animator.MakeStatic(d);
            }
        }
        private static void ClampStatic(CAnimator<CVector3> animator, float def1 = 0, float def2 = 0, float def3 = 0)
        {
            CVector3 d = new CVector3(def1,def2,def3);
            if (animator.Count == 1) { d = animator.NodeList[0].Value; }
            if (animator.Static == false && animator.Count <= 1)
            {
                animator.Clear();
                animator.MakeStatic(d);
            }
        }
        private static void ClampStatic(CAnimator<CVector4> animator)
        {
            CVector4 d = new CVector4(0,0,0,1);
            if (animator.Count == 1) { d = animator.NodeList[0].Value; }
            if (animator.Static == false && animator.Count <= 1)
            {
                animator.Clear();
                animator.MakeStatic(d);
            }
        }
        private static void ClampStatic(CAnimator<int> animator)
        {
            int d = 0;
            if (animator.Count == 1) { d = animator.NodeList[0].Value; }
            if (animator.Static == false && animator.Count <= 1)
            {
                animator.Clear();
                animator.MakeStatic(d);
            }
        }
        private static void DelUnusedMatrixGroups_()
        {
            foreach (CGeoset geoset in Model.Geosets)
            {
                foreach (CGeosetGroup group in geoset.Groups.ToList())
                {
                    if (geoset.Vertices.Any(x=>x.Group.Object ==  group) == false)
                    {
                        geoset.Groups.Remove(group);
                    }
                }
            }
        }
        private static void MergeTAs_()
        {
            again:
            if (Model.TextureAnimations.Count > 1)
            {
                 foreach (CTextureAnimation ta1 in Model.TextureAnimations.ToList())
                {
                    foreach (CTextureAnimation ta2 in Model.TextureAnimations.ToList())
                    {
                        if (ta1 == ta2) { continue; }
                        if (TextureAnimationsSame(ta1, ta2))
                        {
                            ReassignTextureAnimation(ta1, ta2);
                           Model.TextureAnimations.Remove(ta2);
                            goto again;
                        }
                    }
                }
            }
        }
        private static void ReassignTextureAnimation(CTextureAnimation ta1, CTextureAnimation ta2)
        {
            foreach (CMaterial mat in Model.Materials)
            {
                foreach (CMaterialLayer layer in mat.Layers)
                {
                    if (layer.TextureAnimation.Object == ta2)
                    {
                        layer.TextureAnimation.Attach(ta1);
                    }
                }
            }
        }
        private static bool TextureAnimationsSame(CTextureAnimation one, CTextureAnimation two)
        {
            if (one.Translation.Static && !two.Translation.Static) return false;
            if (one.Rotation.Static && !two.Rotation.Static) return false;
            if (one.Scaling.Static && !two.Scaling.Static) return false;
            if (one.Translation.Count != two.Translation.Count) return false;
            if (one.Rotation.Count != two.Rotation.Count) return false;
            if (one.Scaling.Count != two.Scaling.Count) return false;
            for (int i = 0;i< one.Translation.Count; i++)
            {
               if ((
                    one.Translation[i].Value.X ==  two.Translation[i].Value.X &&
                    one.Translation[i].Value.Y ==  two.Translation[i].Value.Y &&
                    one.Translation[i].Value.Z ==  two.Translation[i].Value.Z 
                    ) == false) { return false; }
            }
            for (int i = 0; i < one.Scaling.Count; i++)
            {
                if ((
                     one.Scaling[i].Value.X == two.Scaling[i].Value.X &&
                     one.Scaling[i].Value.Y == two.Scaling[i].Value.Y &&
                     one.Scaling[i].Value.Z == two.Scaling[i].Value.Z
                     ) == false) { return false; }
            }
            for (int i = 0; i < one.Rotation.Count; i++)
            {
                if ((
                     one.Rotation[i].Value.X == two.Rotation[i].Value.X &&
                     one.Rotation[i].Value.Y == two.Rotation[i].Value.Y &&
                     one.Rotation[i].Value.Z == two.Rotation[i].Value.Z &&
                     one.Rotation[i].Value.W == two.Rotation[i].Value.W
                     ) == false) { return false; }
            }
            return true;
        }
        private static void MergeMatertials_()
        {
            again:
            if (Model.Materials.Count < 2) { return; }
            foreach (CMaterial mat1 in Model.Materials.ToList())
            {
                foreach (CMaterial mat2 in Model.Materials)
                {
                    if (mat1 == mat2) { continue; }
                    if (MaterialsSame(mat1, mat2))
                    {
                        ReassignMaterial(mat1, mat2);
                        Model.Materials.Remove(mat2);
                        goto again;
                    }
                }
            }
        }
        private static void ReassignMaterial(CMaterial mat1, CMaterial mat2)
        {
         foreach (CGeoset geoset in Model.Geosets)
            {
                if (geoset.Material.Object == mat2) { geoset.Material.Attach(mat1); }
            }
        }
        private static bool MaterialsSame(CMaterial mat1, CMaterial mat2)
        {
            bool same = false;
            same = mat1.SortPrimitivesFarZ = mat2.SortPrimitivesFarZ;
            same = mat1.SortPrimitivesNearZ = mat2.SortPrimitivesNearZ;
            same = mat1.FullResolution = mat2.FullResolution;
            same = mat2.Layers.Count == mat2.Layers.Count;
            if (mat1.Layers.Count == mat1.Layers.Count)
            {
                for (int i = 0; i < mat1.Layers.Count; i++)
                {
                    if (mat1.Layers[i] == null || mat2.Layers[i] == null) { continue; }
                    same = LayersSame(mat1.Layers[i], mat2.Layers[i]);
                }
            }
            return same;
        }
        private static void MergeLayers_()
        {
            foreach (CMaterial mat in Model.Materials)
            {
                again:
                if (mat.Layers.Count > 1)
                {
                    for (int i = 0; i < mat.Layers.Count; i++)
                    {
                        if (LayersSame(mat.Layers[0], mat.Layers[i + 1]))
                        {
                            mat.Layers.RemoveAt(i+1);
                            goto again;
                        }
                    }
                }
            }
        }
        private static bool LayersSame(CMaterialLayer one, CMaterialLayer two)
        {
            if (one == null || two == null) { return false; }
            bool same = true;
           same = one.Alpha.Static && two.Alpha.Static && one.Alpha.GetValue() == two.Alpha.GetValue();
           same = one.TextureId.Static && two.TextureId.Static && one.Texture.Object == two.Texture.Object;
             if (one.Alpha.Static == false && two.Alpha.Static == false)
            {
                same = one.Alpha.Count == two.Alpha.Count;
                if (one.Alpha.Count == two.Alpha.Count)
                {
                    same = AlphasSame(one.Alpha, two.Alpha);
                }
            }
            return same;
        }
        private static bool AlphasSame(CAnimator<float> one, CAnimator<float> two)
        {
            for (int i = 0; i < one.Count; i++)
            {
                if (one[i] != two[i]) { return false; }
            }
            return true;
        }
        private static void MergeTextures_()
        {
            var textures = Model.Textures.ToList(); // Work on a copy of the collection.
            int ToRemove = -1;
            start:
            if (ToRemove > -1)
            {
                textures.RemoveAt(ToRemove);
            }
            ToRemove = -1;
            if (textures.Count <= 1) { return; }
            for (int i = 0; i< textures.Count; i++)
            {
                for (int j = 0; j < textures.Count; j++)
                {
                    if (i == j) { continue; }
                    if (
                        textures[i].ReplaceableId == 0   && 
                        textures[j].ReplaceableId == 0   && 
                        textures[i].FileName == textures[j].FileName
                        )
                    {
                        ReassignTexture(textures[i], textures[j]);
                        ToRemove = j;
                        goto start;
                    }
                    if (
                       textures[i].ReplaceableId  > 0 &&
                       textures[j].ReplaceableId  >  0 &&
                       textures[i].ReplaceableId == textures[j].ReplaceableId
                       )
                    {
                        ReassignTexture(textures[i], textures[j]);
                        ToRemove = j;
                        goto start;
                    }
                }
            }
        }
        private static void ReassignTexture(CTexture first, CTexture second)
        {
            foreach (CMaterial material in Model.Materials)
            {
                foreach (CMaterialLayer layer in material.Layers)
                {
                    if (layer.Texture.Object == second)
                    {
                        layer.Texture.Attach(first);
                    }
                }
            }
        }
        private static void RemoveEmptyGeosetAnimations()
        {
            foreach (CGeosetAnimation ga in Model.GeosetAnimations.ToList())
            {
                if (Model.Geosets.Contains(ga.Geoset.Object) == false || ga.Geoset == null || ga.Geoset.Object == null)
                {
                    Model.GeosetAnimations.Remove(ga);
                }
            }
        }
        private static void DeleteDuplicateGEosets_()
        {
            again:
            foreach (CGeoset geoset1 in Model.Geosets.ToList())
            {
                foreach (CGeoset geoset2 in Model.Geosets.ToList())
                {
                    if (geoset1 != geoset2)
                    {
                    if (GeosetComparer.IdenticalPositions(geoset1, geoset2))
                    {
                         Model.Geosets.Remove(geoset2);
                            goto again;
                    }
                    }
                }
            }
        }
        private static void DeleteEmptyGeosets()
        {
            foreach (CGeoset geoset in Model.Geosets.ToList())
            {
              foreach (CGeosetTriangle face in geoset.Triangles.ToList())
                {
                    if (face.Vertex1.Object == null || 
                        face.Vertex2.Object == null ||
                        face.Vertex3.Object == null)
                    {
                        geoset.Triangles.Remove(face);
                    }
                }
            }
            foreach (CGeoset geoset in Model.Geosets.ToList())
            {
                if (geoset.Triangles.Count == 0 || geoset.Vertices.Count < 3)
                { 
                    Model.Geosets.Remove(geoset); 
                } 
            }
            }
        private static void MergeIdenticalVertices_()
        {
            foreach (CGeoset geoset in Model.Geosets)
            {
                var verticesToRemove = new HashSet<CGeosetVertex>(); // Use HashSet for efficient lookup
                var vertexMap = new Dictionary<CGeosetVertex, CGeosetVertex>(); // Map to track vertex replacements
                // Compare each vertex only once
                for (int i = 0; i < geoset.Vertices.Count; i++)
                {
                    var vertex1 = geoset.Vertices[i];
                    for (int j = i + 1; j < geoset.Vertices.Count; j++)
                    {
                        var vertex2 = geoset.Vertices[j];
                        if (VerticesIdentical(vertex1, vertex2))
                        {
                            vertexMap[vertex2] = vertex1; // Map vertex2 to vertex1
                            verticesToRemove.Add(vertex2);
                        }
                    }
                }
                // Reassign vertices in faces using the map
                foreach (CGeosetTriangle face in geoset.Triangles)
                {
                    if (vertexMap.TryGetValue(face.Vertex1.Object, out var replacement1))
                        face.Vertex1.Attach(replacement1);
                    if (vertexMap.TryGetValue(face.Vertex2.Object, out var replacement2))
                        face.Vertex2.Attach(replacement2);
                    if (vertexMap.TryGetValue(face.Vertex3.Object, out var replacement3))
                        face.Vertex3.Attach(replacement3);
                }
                // Remove duplicates from the geoset
                foreach (var vertex in verticesToRemove)
                {
                    geoset.Vertices.Remove(vertex);
                }
            }
        }
        private static bool VerticesIdentical(CGeosetVertex one, CGeosetVertex two)
        {
            return
                one.Position.X == two.Position.X &&
                one.Position.Y == two.Position.Y &&
                one.Position.Z == two.Position.Z &&
                one.TexturePosition.X == two.Position.X &&
                one.TexturePosition.Y == two.Position.Y;
        }
        private static void DeleteTrianglesWithNoArea_()
        {
            foreach (CGeoset geoset in Model.Geosets.ToList())
            {
                foreach (CGeosetTriangle face in geoset.Triangles.ToList())
                {
                    if (TriangleHasNoArea(face))
                    {
                        geoset.Triangles.Remove(face); continue;
                    }
                    if (TriangleRepeatVertices(face))
                    {
                        geoset.Triangles.Remove(face); continue;
                    }
                }
            }
        }
        private static bool TriangleRepeatVertices(CGeosetTriangle face)
        {
            if (
                face.Vertex1.Object == face.Vertex2.Object ||
                face.Vertex1.Object == face.Vertex3.Object ||
                face.Vertex2.Object == face.Vertex3.Object
                ) { return true; }
            return false;
        }
        private static bool TriangleHasNoArea(CGeosetTriangle face)
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
                foreach (CGeosetTriangle face1 in geoset.Triangles.ToList())
                {
                    if (
                        !geoset.Vertices.Contains(face1.Vertex1.Object) ||
                        !geoset.Vertices.Contains(face1.Vertex2.Object) ||
                        !geoset.Vertices.Contains(face1.Vertex3.Object)
                        )
                    {
                        geoset.Triangles.Remove(face1);
                    }
                    }
                }
            }
        private static void DeleteFullyOverLappingFaces()
        {
            foreach (CGeoset geoset in Model.Geosets)
            {
                // Use ToList() to avoid modifying the collection while iterating
                foreach (CGeosetTriangle face1 in geoset.Triangles.ToList())
                {
                    foreach (CGeosetTriangle face2 in geoset.Triangles.ToList())
                    {
                        // Skip comparison with itself
                        if (face1 == face2) { continue; }
                        // If faces are fully overlapping, remove the second one
                        if (FacesFullyOverlapping(face1, face2))
                        {
                            geoset.Triangles.Remove(face2);
                        }
                    }
                }
            }
        }
        public static bool FacesFullyOverlapping(CGeosetTriangle face1, CGeosetTriangle face2)
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
                foreach (CGeosetTriangle face1 in geoset.Triangles.ToList())
                {
                    foreach (CGeosetTriangle face2 in geoset.Triangles.ToList())
                    {
                        // Skip comparison with itself
                        if (face1 == face2) { continue; }
                        // Check if the faces share any vertices
                        if (ShareSameVertices(face1, face2))
                        {
                            // If they share vertices, remove the second face
                            geoset.Triangles.Remove(face2);
                        }
                    }
                }
            }
        }
        private static bool ShareSameVertices(CGeosetTriangle face1, CGeosetTriangle face2)
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
            int CountFaults = 0;
            // zero groups or not using groups
            foreach (CGeoset geo in Model.Geosets)
            {
                foreach (var vertex in geo.Vertices)
                {
                    if (vertex.Group == null || vertex.Group.Object == null)
                    {
                        if (geo.Groups.Count ==0)
                        {
                            CGeosetGroup group = new CGeosetGroup(Model);
                            CGeosetGroupNode gnode = new CGeosetGroupNode(Model);
                            group.Nodes.Add(gnode);
                            gnode.Node.Attach(dummy);
                            CountFaults++;
                        }
                        else
                        {
                            vertex.Group.Attach(geo.Groups[0]);
                        }
                    }
                }
            }
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
                                CountFaults++;
                                CGeosetGroupNode node = new CGeosetGroupNode(Model);
                                node.Node.Attach(dummy);
                                group.Nodes.Add(node);
                                break;
                            }
                        }
                    }
                }
            }
            if (CountFaults > 0) { Model.Nodes.Add(dummy); }
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
            foreach (INode node in Model.Nodes)
            {
                RemoveSimilarAdjascentKeyframes(node.Translation);
                RemoveSimilarAdjascentKeyframes(node.Rotation);
                RemoveSimilarAdjascentKeyframes(node.Scaling);
                if (node is CAttachment)
                {
                    CAttachment element = (CAttachment)node;
                    RemoveSimilarAdjascentKeyframes(element.Visibility);
                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                   // RemoveSimilarAdjascentKeyframes(element.Visibility);
                    RemoveSimilarAdjascentKeyframes(element.EmissionRate);
                    RemoveSimilarAdjascentKeyframes(element.LifeSpan);
                    RemoveSimilarAdjascentKeyframes(element.InitialVelocity);
                    RemoveSimilarAdjascentKeyframes(element.Gravity);
                    RemoveSimilarAdjascentKeyframes(element.Longitude);
                    RemoveSimilarAdjascentKeyframes(element.Latitude);
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                   // RemoveSimilarAdjascentKeyframes(element.Visibility);
                    RemoveSimilarAdjascentKeyframes(element.EmissionRate);
                    RemoveSimilarAdjascentKeyframes(element.Speed);
                    RemoveSimilarAdjascentKeyframes(element.Width);
                    RemoveSimilarAdjascentKeyframes(element.Gravity);
                    RemoveSimilarAdjascentKeyframes(element.Length);
                    RemoveSimilarAdjascentKeyframes(element.Latitude);
                    RemoveSimilarAdjascentKeyframes(element.Variation);
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                   // RemoveSimilarAdjascentKeyframes(element.Visibility);
                    RemoveSimilarAdjascentKeyframes(element.HeightAbove);
                    RemoveSimilarAdjascentKeyframes(element.HeightBelow);
                    RemoveSimilarAdjascentKeyframes(element.Color);
                    RemoveSimilarAdjascentKeyframes(element.Alpha);
                    RemoveSimilarAdjascentKeyframes(element.TextureSlot);
                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;
                  //  RemoveSimilarAdjascentKeyframes(element.Visibility);
                    RemoveSimilarAdjascentKeyframes(element.Color);
                    RemoveSimilarAdjascentKeyframes(element.AmbientColor);
                    RemoveSimilarAdjascentKeyframes(element.Intensity);
                    RemoveSimilarAdjascentKeyframes(element.AmbientIntensity);
                    RemoveSimilarAdjascentKeyframes(element.AttenuationEnd);
                    RemoveSimilarAdjascentKeyframes(element.AttenuationStart);
                }
            }
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {
                if (ga.Alpha.Static == false)
                {
                    RemoveSimilarAdjascentKeyframes(ga.Alpha);
                }
                if (ga.Color.Static == false) { RemoveAdjascentKeyframes(ga.Color); }
            }
            foreach (CTextureAnimation taa in Model.TextureAnimations)
            {
                RemoveSimilarAdjascentKeyframes(taa.Translation);
                RemoveSimilarAdjascentKeyframes(taa.Rotation);
                RemoveSimilarAdjascentKeyframes(taa.Scaling);
            }
            foreach (CMaterial material in Model.Materials)
            {
                foreach (CMaterialLayer layer in material.Layers)
                {
                    RemoveSimilarAdjascentKeyframes(layer.Alpha);
                    RemoveSimilarAdjascentKeyframes(layer.TextureId);
                }
            }
            foreach (CCamera cam in Model.Cameras)
            {
                RemoveSimilarAdjascentKeyframes(cam.Rotation);
            }
        }
      
        internal static float Difference(CAnimatorNode<CVector3> cAnimatorNode1, CAnimatorNode<CVector3> cAnimatorNode2, CAnimatorNode<CVector3> cAnimatorNode3)
        {
            if (cAnimatorNode1 == null || cAnimatorNode2 == null || cAnimatorNode3 == null)
            {
                return 4;
            }
            // Get the CVector3 values
            CVector3 one = cAnimatorNode1.Value;
            CVector3 two = cAnimatorNode2.Value;
            CVector3 three = cAnimatorNode3.Value;
            // Calculate differences
            CVector3 diffAbove = new CVector3(
                Math.Abs(two.X - one.X),
                Math.Abs(two.Y - one.Y),
                Math.Abs(two.Z - one.Z)
            );
            CVector3 diffBelow = new CVector3(
                Math.Abs(two.X - three.X),
                Math.Abs(two.Y - three.Y),
                Math.Abs(two.Z - three.Z)
            );
            // Calculate total magnitude of differences
            float totalDifference = Magnitude(diffAbove) + Magnitude(diffBelow);
            return totalDifference;
        }
        internal static float Difference(CAnimatorNode<CVector4> cAnimatorNode1, CAnimatorNode<CVector4> cAnimatorNode2, CAnimatorNode<CVector4> cAnimatorNode3)
        {
            if (cAnimatorNode1 == null || cAnimatorNode2 == null || cAnimatorNode3 == null)
            {
                return 4;
            }
            // Get the CVector4 values
            CVector4 one = cAnimatorNode1.Value;
            CVector4 two = cAnimatorNode2.Value;
            CVector4 three = cAnimatorNode3.Value;
            // Calculate differences
            CVector4 diffAbove = new CVector4(
                Math.Abs(two.X - one.X),
                Math.Abs(two.Y - one.Y),
                Math.Abs(two.Z - one.Z),
                Math.Abs(two.W - one.W)
            );
            CVector4 diffBelow = new CVector4(
                Math.Abs(two.X - three.X),
                Math.Abs(two.Y - three.Y),
                Math.Abs(two.Z - three.Z),
                Math.Abs(two.W - three.W)
            );
            // Calculate total magnitude of differences
            float totalDifference = Magnitude(diffAbove) + Magnitude(diffBelow);
            return totalDifference;
        }
        internal static float Magnitude(CVector3 vector)
        {
            return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }
        internal static float Magnitude(CVector4 vector)
        {
            return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z + vector.W * vector.W);
        }
        private static void RemoveSimilarAdjascentKeyframes(CAnimator<CVector3> list)
        {
            float threshold = 0.00128f;
            if (list.Static == true) { return; }
            start:
            if (list.Count < 3) { return; }
            for (int i = 0; i < list.Count; i++)
            {
                if (i - 1 != -1)
                {
                    if (list[i - 1] == null || list[i] == null || list[i + 1] == null) { continue; }
                    if (TracksBelongToSameSequence(list[i - 1].Time, list[i].Time, list[i + 1].Time))
                    {
                        float difference = Calculator.Difference(list[i - 1], list[i], list[i + 1]);
                        if (difference > 0 && difference <= threshold)
                        {
                            list.RemoveAt(i);
                            goto start;
                        }
                    }
                }
            }
        }
        private static void RemoveSimilarAdjascentKeyframes(CAnimator<CVector4> list)
        {
            float threshold = 0.0128f;
            if (list.Static == true) { return; }
            start:
            if (list.Count < 3) { return; }
            for (int i = 0; i < list.Count; i++)
            {
                if (i - 1 != -1)
                {
                    if (list[i - 1] == null || list[i] == null || list[i + 1] == null) { continue; }
                    if (TracksBelongToSameSequence(list[i - 1].Time, list[i].Time, list[i + 1].Time))
                        {
                        float difference = Calculator.DifferenceQuaternion(list[i - 1], list[i], list[i + 1]);
                        if (difference > 0 && difference <= threshold)
                        {
                            list.RemoveAt(i);
                            goto start;
                        }
                    }
                }
            }
        }
        private static void RemoveSimilarAdjascentKeyframes(CAnimator<float> list)
        {
            float threshold = 0.00128f;
            if (list.Static == true) { return; }
            start:
            if (list.Count < 3) { return; }
            for (int i = 0; i < list.Count; i++)
            {
                if (i - 1 != -1)
                {
                    if (list[i - 1] == null || list[i] == null || list[i + 1] == null) { continue; }
                    if (TracksBelongToSameSequence(list[i - 1].Time, list[i].Time, list[i + 1].Time))
                        {
                        float difference = Calculator.Difference(list[i - 1], list[i], list[i + 1]);
                        if (difference > 0 && difference <= threshold)
                        {
                            list.RemoveAt(i);
                            goto start;
                        }
                    }
                }
            }
        }
        private static void RemoveSimilarAdjascentKeyframes(CAnimator<int> list)
        {
            return; // not applicable for ints
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
        private static void CreateWhiteTextureifNo()
        {
            if (Model.Textures.Count == 0)
            {
                CreateWhiteTextures();
            }
        }
        private static void StretchKeyframesOf(CAnimator<CVector3> animator)
        {
            // Stretch keyframes to fit the sequence if it is missing the starting or ending keyframe
            foreach (var sequence in Model.Sequences)
            {
                int from = sequence.IntervalStart;
                int to = sequence.IntervalEnd;

                // Collect keyframes within the sequence interval
                List<CAnimatorNode<CVector3>> keyframes = animator
                    .Where(kf => kf.Time >= from && kf.Time <= to)
                    .OrderBy(kf => kf.Time)
                    .ToList();

                if (keyframes.Count > 1)
                {
                    bool hasStart = keyframes.Any(kf => kf.Time == from);
                    bool hasEnd = keyframes.Any(kf => kf.Time == to);

                    if (!hasStart || !hasEnd)
                    {
                        int minTime = keyframes.First().Time;
                        int maxTime = keyframes.Last().Time;

                        // Normalize keyframes within the new range
                        foreach (var keyframe in keyframes)
                        {
                            keyframe.Time = (int)(from + (float)(keyframe.Time - minTime) / (maxTime - minTime) * (to - from));
                        }
                    }
                }
            }
        }

        private static void StretchKeyframesOf(CAnimator<CVector4> animator)
        {
            // Stretch keyframes to fit the sequence if it is missing the starting or ending keyframe
            foreach (var sequence in Model.Sequences)
            {
                int from = sequence.IntervalStart;
                int to = sequence.IntervalEnd;

                // Collect keyframes within the sequence interval
                List<CAnimatorNode<CVector4>> keyframes = animator
                    .Where(kf => kf.Time >= from && kf.Time <= to)
                    .OrderBy(kf => kf.Time)
                    .ToList();

                if (keyframes.Count > 1)
                {
                    bool hasStart = keyframes.Any(kf => kf.Time == from);
                    bool hasEnd = keyframes.Any(kf => kf.Time == to);

                    if (!hasStart || !hasEnd)
                    {
                        int minTime = keyframes.First().Time;
                        int maxTime = keyframes.Last().Time;

                        // Normalize keyframes within the new range
                        foreach (var keyframe in keyframes)
                        {
                            keyframe.Time = (int)(from + (float)(keyframe.Time - minTime) / (maxTime - minTime) * (to - from));
                        }
                    }
                }
            }
        }
        private static void StretchKeyframesOf(CAnimator<float> animator)
        {
            // Stretch keyframes to fit the sequence if it is missing the starting or ending keyframe
            foreach (var sequence in Model.Sequences)
            {
                int from = sequence.IntervalStart;
                int to = sequence.IntervalEnd;

                // Collect keyframes within the sequence interval
                List<CAnimatorNode<float>> keyframes = animator
                    .Where(kf => kf.Time >= from && kf.Time <= to)
                    .OrderBy(kf => kf.Time)
                    .ToList();

                if (keyframes.Count > 1)
                {
                    bool hasStart = keyframes.Any(kf => kf.Time == from);
                    bool hasEnd = keyframes.Any(kf => kf.Time == to);

                    if (!hasStart || !hasEnd)
                    {
                        int minTime = keyframes.First().Time;
                        int maxTime = keyframes.Last().Time;

                        // Normalize keyframes within the new range
                        foreach (var keyframe in keyframes)
                        {
                            keyframe.Time = (int)(from + (float)(keyframe.Time - minTime) / (maxTime - minTime) * (to - from));
                        }
                    }
                }
            }
        }
        private static void StretchKeyframesOf(CAnimator<int> animator)
        {
            // Stretch keyframes to fit the sequence if it is missing the starting or ending keyframe
            foreach (var sequence in Model.Sequences)
            {
                int from = sequence.IntervalStart;
                int to = sequence.IntervalEnd;

                // Collect keyframes within the sequence interval
                List<CAnimatorNode<int>> keyframes = animator
                    .Where(kf => kf.Time >= from && kf.Time <= to)
                    .OrderBy(kf => kf.Time)
                    .ToList();

                if (keyframes.Count > 1)
                {
                    bool hasStart = keyframes.Any(kf => kf.Time == from);
                    bool hasEnd = keyframes.Any(kf => kf.Time == to);

                    if (!hasStart || !hasEnd)
                    {
                        int minTime = keyframes.First().Time;
                        int maxTime = keyframes.Last().Time;

                        // Normalize keyframes within the new range
                        foreach (var keyframe in keyframes)
                        {
                            keyframe.Time = (int)(from + (float)(keyframe.Time - minTime) / (maxTime - minTime) * (to - from));
                        }
                    }
                }
            }
        }
        private static void StretchAllKeyframes()
        {
            foreach (CMaterial material in Model.Materials)
            {
                foreach (CMaterialLayer layer in material.Layers)
                {

                    StretchKeyframesOf(layer.Alpha);
                    StretchKeyframesOf(layer.TextureId);
                }
            }
            foreach (INode node in Model.Nodes)
            {

                StretchKeyframesOf(node.Translation);
                StretchKeyframesOf(node.Rotation);
                StretchKeyframesOf(node.Scaling);
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;

                    StretchKeyframesOf(element.EmissionRate);
                    StretchKeyframesOf(element.LifeSpan);
                    StretchKeyframesOf(element.InitialVelocity);
                    StretchKeyframesOf(element.Gravity);
                    StretchKeyframesOf(element.Longitude);
                    StretchKeyframesOf(element.Latitude);
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;

                    StretchKeyframesOf(element.EmissionRate);
                    StretchKeyframesOf(element.Speed);
                    StretchKeyframesOf(element.Variation);
                    StretchKeyframesOf(element.Latitude);
                    StretchKeyframesOf(element.Width);
                    StretchKeyframesOf(element.Length);
                    StretchKeyframesOf(element.Gravity);
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;

                    StretchKeyframesOf(element.HeightAbove);
                    StretchKeyframesOf(element.HeightBelow);
                    StretchKeyframesOf(element.Color);
                    StretchKeyframesOf(element.Alpha);
                    StretchKeyframesOf(element.TextureSlot);
                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;

                    StretchKeyframesOf(element.Color);
                    StretchKeyframesOf(element.AmbientColor);
                    StretchKeyframesOf(element.Intensity);
                    StretchKeyframesOf(element.AmbientIntensity);
                    StretchKeyframesOf(element.AttenuationEnd);
                    StretchKeyframesOf(element.AttenuationStart);
                }
            }
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {

                StretchKeyframesOf(ga.Alpha);
                StretchKeyframesOf(ga.Color);
            }
            foreach (CTextureAnimation ta in Model.TextureAnimations)
            {

                StretchKeyframesOf(ta.Translation);
                StretchKeyframesOf(ta.Rotation);
                StretchKeyframesOf(ta.Scaling);
            }
        }
        private static void AddMissingKeyframesWithDefault(bool starting, bool ending)
        {
            if (!starting && !ending) { return; }
            foreach (CMaterial material in Model.Materials)
            {
                foreach (CMaterialLayer layer in material.Layers)
                {
                    if (starting) {
                        AddMissingStartingTracks(layer.Alpha);
                        AddMissingStartingTracks(layer.TextureId);
                    } if (ending) {
                        AddMissingEndingTracks(layer.Alpha);
                        AddMissingEndingTracks(layer.TextureId);
                    }
                    
                }
            }
            foreach (INode node in Model.Nodes)
            {
                if (starting)
                {
                    AddMissingStartingTracks(node.Translation);
                    AddMissingStartingTracks(node.Rotation);
                    AddMissingStartingTracks(node.Scaling);
                }
                if (ending)
                {
                    AddMissingEndingTracks(node.Translation);
                    AddMissingEndingTracks(node.Rotation);
                    AddMissingEndingTracks(node.Scaling);
                }
               
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                    if (starting) {
                        AddMissingStartingTracks(element.EmissionRate);
                        AddMissingStartingTracks(element.LifeSpan);
                        AddMissingStartingTracks(element.InitialVelocity);
                        AddMissingStartingTracks(element.Gravity);
                        AddMissingStartingTracks(element.Longitude);
                        AddMissingStartingTracks(element.Latitude);
                    }
                    if (ending) {
                        AddMissingEndingTracks(element.EmissionRate);
                        AddMissingEndingTracks(element.LifeSpan);
                        AddMissingEndingTracks(element.InitialVelocity);
                        AddMissingEndingTracks(element.Gravity);
                        AddMissingEndingTracks(element.Longitude);
                        AddMissingEndingTracks(element.Latitude);
                    }
                    
                }
                if (node is CParticleEmitter2) {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    if (starting) {
                        AddMissingStartingTracks(element.EmissionRate);
                        AddMissingStartingTracks(element.Speed);
                        AddMissingStartingTracks(element.Variation);
                        AddMissingStartingTracks(element.Latitude);
                        AddMissingStartingTracks(element.Width);
                        AddMissingStartingTracks(element.Length);
                        AddMissingStartingTracks(element.Gravity);
                    }
                    if (ending) {
                        AddMissingEndingTracks(element.EmissionRate);
                        AddMissingEndingTracks(element.Speed);
                        AddMissingEndingTracks(element.Variation);
                        AddMissingEndingTracks(element.Latitude);
                        AddMissingEndingTracks(element.Width);
                        AddMissingEndingTracks(element.Length);
                        AddMissingEndingTracks(element.Gravity);
                    }
                    
                }
                if (node is CRibbonEmitter) {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    if (starting)
                    {
                        AddMissingStartingTracks(element.HeightAbove);
                        AddMissingStartingTracks(element.HeightBelow);
                        AddMissingStartingTracks(element.Color);
                        AddMissingStartingTracks(element.Alpha);
                        AddMissingStartingTracks(element.TextureSlot);
                    }
                    if (ending)
                    {
                        AddMissingEndingTracks(element.HeightAbove);
                        AddMissingEndingTracks(element.HeightBelow);
                        AddMissingEndingTracks(element.Color);
                        AddMissingEndingTracks(element.Alpha);
                        AddMissingEndingTracks(element.TextureSlot);
                    }
                   
                }
                if (node is CLight) {
                    CLight element = (CLight)node;
                    if (starting) {
                        AddMissingStartingTracks(element.Color);
                        AddMissingStartingTracks(element.AmbientColor);
                        AddMissingStartingTracks(element.Intensity);
                        AddMissingStartingTracks(element.AmbientIntensity);
                        AddMissingStartingTracks(element.AttenuationEnd);
                        AddMissingStartingTracks(element.AttenuationStart);
                    }
                    if (ending) {
                        AddMissingEndingTracks(element.Color);
                        AddMissingEndingTracks(element.AmbientColor);
                        AddMissingEndingTracks(element.Intensity);
                        AddMissingEndingTracks(element.AmbientIntensity);
                        AddMissingEndingTracks(element.AttenuationEnd);
                        AddMissingEndingTracks(element.AttenuationStart);
                    }
                    
                }
            }
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {
                if (starting)
                {
                    AddMissingStartingTracks(ga.Alpha);
                    AddMissingStartingTracks(ga.Color);
                }
                if (ending)
                {
                    AddMissingEndingTracks(ga.Alpha);
                    AddMissingEndingTracks(ga.Color);
                }
               
            }
            foreach (CTextureAnimation ta in Model.TextureAnimations)
            {
                if (starting) {
                    AddMissingStartingTracks(ta.Translation);
                    AddMissingStartingTracks(ta.Rotation);
                    AddMissingStartingTracks(ta.Scaling);
                }
                if (ending) {
                    AddMissingEndingTracks(ta.Translation);
                    AddMissingEndingTracks(ta.Rotation);
                    AddMissingEndingTracks(ta.Scaling);
                }
                
            }
        }

        private static void AddMissingEndingTracks(CAnimator<CVector3> animator)
        {
            if (animator.Static == true) { return; }
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<CVector3>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes.Any(x => x.Time != from))
                        {
                            var first = keyframes.First();
                            CAnimatorNode<CVector3> node = new CAnimatorNode<CVector3>();
                            node.Time = from;
                            node.Value = new CVector3();
                            node.InTangent = new CVector3();
                            node.OutTangent = new CVector3();
                            animator.NodeList = animator.NodeList.OrderBy(x => x.Time).ToList();
                        }
                    }
                }
            }
        }
        private static void AddMissingEndingTracks(CAnimator<CVector4> animator)
        {
            if (animator.Static == true) { return; }
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<CVector4>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes.Any(x => x.Time != from))
                        {
                            var first = keyframes.First();
                            CAnimatorNode<CVector4> node = new CAnimatorNode<CVector4>();
                            node.Time = from;
                            node.Value = new CVector4(0,0,0,1);
                            node.InTangent = new CVector4(0, 0, 0, 1);
                            node.OutTangent = new CVector4(0, 0, 0, 1);
                            animator.NodeList = animator.NodeList.OrderBy(x => x.Time).ToList();
                        }
                    }
                }
            }
        }
        private static void AddMissingEndingTracks(CAnimator<float> animator)
        {
            if (animator.Static == true) { return; }
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<float>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes.Any(x => x.Time != to))
                        {
                            var first = keyframes.First();
                            CAnimatorNode<float> node = new CAnimatorNode<float>();
                            node.Time = from;
                            node.Value = 0;
                            node.InTangent = 0;
                            node.OutTangent = 0;
                            animator.NodeList = animator.NodeList.OrderBy(x => x.Time).ToList();
                        }
                    }
                }
            }
        }
        private static void AddMissingEndingTracks(CAnimator<int> animator)
        {
            if (animator.Static == true) { return; }
            if (animator.NodeList.Count > 1)
            {
                foreach (var sequence in Model.Sequences)
                {
                    int from = sequence.IntervalStart;
                    int to = sequence.IntervalEnd;
                    List<CAnimatorNode<int>> keyframes = animator
                  .Where(kf => kf.Time >= from && kf.Time <= to)
                                             .OrderBy(kf => kf.Time)
                             .ToList();
                    if (keyframes.Count > 0)
                    {
                        if (keyframes.Any(x => x.Time != to))
                        {
                            var first = keyframes.First();
                            CAnimatorNode<int> node = new CAnimatorNode<int>();
                            node.Time = from;
                            node.Value = 0;
                            node.InTangent = 0;
                            node.OutTangent = 0;
                            animator.NodeList = animator.NodeList.OrderBy(x => x.Time).ToList();
                        }
                    }
                }
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
            if (Model.GlobalSequences.Contains(animator.GlobalSequence.Object))
            {
                if (animator.Any(x => x.Time == 0) == false)
                {
                    animator.Add(new CAnimatorNode<float>(0, 0));
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
            if (Model.GlobalSequences.Contains(animator.GlobalSequence.Object))
            {
                if (animator.Any(x => x.Time == 0) == false)
                {
                    animator.Add(new CAnimatorNode<int>(0, 0));
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
            if (Model.GlobalSequences.Contains(animator.GlobalSequence.Object))
            {
                if (animator.Any(x => x.Time == 0) == false)
                {
                    animator.Add(new CAnimatorNode<CVector3>(0, new CVector3(0, 0, 0)));
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
            if (animator.GlobalSequence.Object != null)
            {
                if (animator.Any(x => x.Time == 0) == false)
                {
                    animator.Add(new CAnimatorNode<CVector4>(0, new CVector4(0, 0, 0, 1)));
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
                    if (list[i - 1] == null || list[i] == null || list[i + 1] == null) { continue; }
                    if (TracksBelongToSameSequence(list[i - 1].Time, list[i].Time, list[i + 1].Time))
                    {
                        if (list[i - 1].Time == list[i].Time)
                        {
                            list.RemoveAt(i);
                            goto start;
                        }
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
                    if (list[i - 1] == null || list[i] == null || list[i + 1] == null) { continue; }
                    if (TracksBelongToSameSequence(list[i - 1].Time, list[i].Time, list[i + 1].Time))
                    {
                        if (list[i - 1].Time == list[i].Time)
                        {
                            list.RemoveAt(i);
                            goto start;
                        }
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
                    if (list[i - 1] == null || list[i] == null || list[i+1] == null) { continue; }
                    if (TracksBelongToSameSequence(list[i - 1].Time, list[i].Time, list[i + 1].Time))
                    {
                        if (list[i - 1].Time == list[i].Time)
                        {
                            list.RemoveAt(i);
                            goto start;
                        }
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
                    if (list[i - 1] == null || list[i] == null || list[i + 1] == null) { continue; }
                    if (TracksBelongToSameSequence(list[i - 1].Time, list[i].Time, list[i + 1].Time))
                    {
                        if (list[i - 1].Time == list[i].Time)
                        {
                            list.RemoveAt(i);
                            goto start;
                        }
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
                    if (TracksHaveSameValues(key1.Value, key2.Value, key3.Value) && 
                        TracksBelongToSameSequence(key1.Time, key2.Time, key3.Time))
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
        private static void FixSquirtOfEmitters2()
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
        private static void CalculateExtents_(List<int> exceptions )
        {
            // calculate the extent of each geoset
            foreach (CGeoset geo in Model.Geosets)
            {
                if (exceptions.Count == 0)
                {
                    CalculateGeosetExtent(geo);
                }
                else
                {
                    if (exceptions.Contains(Model.Geosets.IndexOf(geo))){
                        geo.Extent = new CExtent();
                    }
                    else
                    {
                        CalculateGeosetExtent(geo);
                    }
                }
                
                

            }
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
                sequence.Extent = GetHighestExtentForVisibleGeoset(sequence);
            }
        }

        private static CExtent GetHighestExtentForVisibleGeoset(CSequence sequence)
        {
            CExtent extent = new();
            List<CExtent> all = new List<CExtent>();
            foreach (var geoset in Model.Geosets)
            {
                if (Model.GeosetAnimations.Any(x=>x.Geoset.Object == geoset))
                {
                    var gs = Model.GeosetAnimations.First(x => x.Geoset.Object == geoset);
                    if (gs.Alpha.Animated)
                    {
                        if (gs.Alpha.Any(x=>x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd))
                        {
                            var kf = gs.Alpha.First(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd);
                            if (kf.Value  > 0)
                            {

                            }
                            else
                            {
                                all.Add(geoset.Extent);
                            }
                        }
                    }
                    else
                    {
                        all.Add(geoset.Extent);
                    }
                }
                else
                {
                    all.Add(geoset.Extent);
                }
            }
            return Calculator.GetMaxExtent(all);
             
        }

        public  static void CalculateGeosetExtent(CGeoset geoset)
        {
            
                List<CVector3> vectors = new List<CVector3>();
                foreach (CGeosetVertex vertex in geoset.Vertices)
                {
                    vectors.Add(vertex.Position);
                }
                geoset.Extent = Calculator.GetExtent(vectors);
             
            
        }
        private static void ClampKeyframes_()
        {
            foreach (INode node in Model.Nodes)
            {
                for (int i = 0; i < node.Rotation.Count; i++)
                {
                    Calculator.ClampQuaternion(node.Rotation[i]);
                }
                for (int i = 0; i < node.Scaling.Count; i++)
                {
                    Calculator.ClampScaling(node.Scaling[i]);
                }
                if (node is CAttachment)
                {
                    CAttachment element = (CAttachment)node;
                    for (int y = 0; y < element.Visibility.Count; y++)
                    {
                        element.Visibility[y].Value = Calculator.ClampVisibility(element.Visibility[y].Value);
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
                            element.Visibility[i].Value = Calculator.ClampVisibility(element.Visibility[i].Value);
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
                            element.EmissionRate[i].Value = Calculator.ClampFloat(element.EmissionRate[i].Value);
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
                            element.InitialVelocity[i].Value = Calculator.ClampFloat(element.InitialVelocity[i].Value);
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
                            element.Gravity[i].Value = Calculator.ClampFloat(element.Gravity[i].Value);
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
                            element.Longitude[i].Value = Calculator.ClampFloat(element.Longitude[i].Value);
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
                            element.Latitude[i].Value = Calculator.ClampFloat(element.Latitude[i].Value);
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
                            element.Visibility[i].Value = Calculator.ClampVisibility(element.Visibility[i].Value);
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
                            element.Latitude[i].Value = Calculator.ClampFloat(element.Latitude[i].Value);
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
                            element.Width[i].Value = Calculator.ClampFloat(element.Width[i].Value);
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
                            element.Length[i].Value = Calculator.ClampFloat(element.Length[i].Value);
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
                            element.Speed[i].Value = Calculator.ClampFloat(element.Speed[i].Value);
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
                            element.Speed[i].Value = Calculator.ClampFloat(element.Speed[i].Value);
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
                            element.EmissionRate[i].Value = Calculator.ClampFloat(element.EmissionRate[i].Value);
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
                            element.Variation[i].Value = Calculator.ClampFloat(element.Variation[i]).Value;
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
                            element.Gravity[i].Value = Calculator.ClampFloat(element.Gravity[i].Value);
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
                            element.Visibility[i].Value = Calculator.ClampNormalized(element.Visibility[i].Value);
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
                            element.Color[i].Value = Calculator.ClampVector3(element.Color[i]).Value;
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
                            element.HeightAbove[i].Value = Calculator.ClampFloat(element.HeightAbove[i].Value);
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
                            element.HeightBelow[i].Value = Calculator.ClampFloat(element.HeightBelow[i].Value);
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
                            element.TextureSlot[i].Value = Calculator.ClampInt(element.TextureSlot[i].Value);
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
                            element.Visibility[i].Value = Calculator.ClampNormalized(element.Visibility[i].Value);
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
                            element.Color[i].Value = Calculator.ClampVector3(element.Color[i].Value);
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
                            element.Visibility[i].Value = Calculator.ClampNormalized(element.Visibility[i].Value);
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
                            element.AmbientColor[i].Value = Calculator.ClampVector3(element.AmbientColor[i].Value);
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
                            element.AttenuationEnd[i].Value = Calculator.ClampFloat(element.AttenuationEnd[i].Value);
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
                            element.AttenuationStart[i].Value = Calculator.ClampFloat(element.AttenuationStart[i].Value);
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
                            element.AmbientIntensity[i].Value = Calculator.ClampFloat(element.AmbientIntensity[i].Value);
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
                            element.Intensity[i].Value = Calculator.ClampFloat(element.Intensity[i].Value);
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
                            foreach (CGeosetTriangle face in geo2.Triangles)
                            {
                                geo1.Triangles.Add(face);
                            }
                        }
                    }
                    goto Start;
                }
            }
        }
        private static void RemoveUnusedKeyframes_Action(CAnimator<int> animator, bool CanBeStatic = true,  int df = 0)
        {
            if (animator.Animated == true)
            {
                foreach (var item in animator.NodeList.ToList())
                {
                    if (item.Time < 0) { animator.NodeList.Remove(item); continue; }
                    if (ValueExistsInSequences(item.Time) == false)
                    {
                        animator.NodeList.Remove(item);
                    }
                }
                if (!CanBeStatic) return;
                if (animator.Count == 0)
                {
                    animator.MakeStatic(df);
                }
                if (animator.Count == 1)
                {
                    var temp = animator.NodeList[0].Value;
                    animator.NodeList.Clear();
                    animator.MakeStatic(temp); 
                }
            }
        }
        private static void RemoveUnusedKeyframes_Action(CAnimator<float> animator, bool CanBeStatic = true, int df = 0)
        {
            if (animator.Animated == true)
            {
                foreach (var item in animator.NodeList.ToList())
                {
                    if (item.Time < 0) { animator.NodeList.Remove(item); continue; }
                    if (ValueExistsInSequences(item.Time) == false)
                    {
                        animator.NodeList.Remove(item);
                    }
                }
                if (!CanBeStatic) return;
                if (animator.Count == 0)
                {
                    animator.MakeStatic(df);
                }
                else if (animator.Count == 1)
                {
                    var temp = animator.NodeList[0].Value;
                    if (temp <= 0) { temp = df; }
                    animator.NodeList.Clear();
                    animator.MakeStatic(temp);
                }
            }
        }
        private static void RemoveUnusedKeyframes_Action(CAnimator<CVector3> animator, bool canBeStatic = true, float one =0, float two = 0, float three = 0)
        {
            if (animator.Animated == true)
            {
                foreach (var item in animator.NodeList.ToList())
                {
                    if (item.Time < 0) { animator.NodeList.Remove(item); continue; }
                    if (ValueExistsInSequences(item.Time) == false)
                    {
                        animator.NodeList.Remove(item);
                    }
                }
                if (!canBeStatic) { return; }
                if (animator.Count == 0)
                {
                    animator.MakeStatic(new CVector3(one, two, three));
                }
                else if (animator.Count == 1)
                {
                    var temp = animator.NodeList[0].Value;
                    animator.NodeList.Clear();
                    animator.MakeStatic(temp);
                }
            }
        }
        private static void RemoveUnusedKeyframes_Action(CAnimator<CVector4> animator, bool canBeStatic = true, float one = 0, float two = 0, float three = 0, float four = 1)
        {
            if (animator.Animated == true)
            {
                foreach (var item in animator.NodeList.ToList())
                {
                    if (item.Time < 0) { animator.NodeList.Remove(item); continue; }
                    if (ValueExistsInSequences(item.Time) == false)
                    {
                        animator.NodeList.Remove(item);
                    }
                }
                if (!canBeStatic) { return; }
                if (animator.Count == 0)
                {
                    animator.MakeStatic(new CVector4(one, two, three, four));
                }
                else if (animator.Count == 1)
                {
                    var temp = animator.NodeList[0].Value;
                    animator.NodeList.Clear();
                    animator.MakeStatic(temp);
                }
            }
        }
        private static bool TrackIsInAnySequence(int track)
        {
            foreach (var s in Model.Sequences)
            {
                if (track >= s.IntervalStart && track <= s.IntervalEnd) return true;
            }
            return false;
        }
        private static void DeleteUnusedKeyframes_()
        {
            List<INode> remove = new List<INode> ();
            foreach (var node in Model.Nodes)
            {
                if (node is CEvent evt)
                {
                    List<CEventTrack> tremove = new List<CEventTrack> ();
                    foreach (var track in evt.Tracks.ObjectList)
                    {
                       
                        if (TrackIsInAnySequence( track.Time) == false)
                        {
                            tremove.Add(track);
                           
                        }
                    }
                    foreach (var track in tremove)
                    {
                        evt.Tracks.Remove(track);
                    }
                    if (evt.Tracks.Count == 0) { remove.Add(node); }
                }
                
            }
            foreach (var node in remove)
            {
                Model.Nodes.Remove(node);
            }
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {
                RemoveUnusedKeyframes_Action(ga.Alpha,true, 1);
                RemoveUnusedKeyframes_Action(ga.Color, true, 1,1,1);
                 
            }
            foreach (CMaterial material in Model.Materials)
            {
                
                foreach (CMaterialLayer layer in material.Layers)
                {
                    RemoveUnusedKeyframes_Action(layer.Alpha,true, 1);
                    RemoveUnusedKeyframes_Action(layer.TextureId);
                    
                }
            }
            foreach (CTextureAnimation ta in Model.TextureAnimations)
            {
                RemoveUnusedKeyframes_Action(ta.Translation);
                RemoveUnusedKeyframes_Action(ta.Rotation);
                RemoveUnusedKeyframes_Action(ta.Scaling,true, 1,1,1);
                 
            }
            foreach (CCamera cam in Model.Cameras)
            {
                RemoveUnusedKeyframes_Action(cam.Rotation);
                RemoveUnusedKeyframes_Action(cam.Translation);
                RemoveUnusedKeyframes_Action(cam.TargetTranslation);
            }
            foreach (INode node in Model.Nodes)
            {
                RemoveUnusedKeyframes_Action(node.Scaling, false,1,1,1);
                RemoveUnusedKeyframes_Action(node.Rotation);
                RemoveUnusedKeyframes_Action(node.Translation);
                
                if (node is CAttachment att)
                {
                    RemoveUnusedKeyframes_Action(att.Visibility, false);
                }
                if (node is CParticleEmitter emitter)
                {
                    RemoveUnusedKeyframes_Action(emitter.InitialVelocity);
                    RemoveUnusedKeyframes_Action(emitter.LifeSpan);
                    RemoveUnusedKeyframes_Action(emitter.EmissionRate);
                    RemoveUnusedKeyframes_Action(emitter.Longitude);
                    RemoveUnusedKeyframes_Action(emitter.Visibility,true, 1);
                    RemoveUnusedKeyframes_Action(emitter.Latitude );
                    RemoveUnusedKeyframes_Action(emitter.Gravity);
                  }
                if (node is CParticleEmitter2 emitter2)
                {
                    RemoveUnusedKeyframes_Action(emitter2.Visibility, true, 1);
                    RemoveUnusedKeyframes_Action(emitter2.Gravity);
                    RemoveUnusedKeyframes_Action(emitter2.Speed);
                    RemoveUnusedKeyframes_Action(emitter2.Width);
                    RemoveUnusedKeyframes_Action(emitter2.Length);
                    RemoveUnusedKeyframes_Action(emitter2.EmissionRate);
                    RemoveUnusedKeyframes_Action(emitter2.Variation);
                    RemoveUnusedKeyframes_Action(emitter2.Latitude);
                 }
                if (node is CRibbonEmitter ribbon)
                {
                    RemoveUnusedKeyframes_Action(ribbon.Visibility, true, 1);
                    RemoveUnusedKeyframes_Action(ribbon.Color, true,1,1,1);
                    RemoveUnusedKeyframes_Action(ribbon.HeightAbove);
                    RemoveUnusedKeyframes_Action(ribbon.HeightBelow);
                    RemoveUnusedKeyframes_Action(ribbon.TextureSlot);
                    RemoveUnusedKeyframes_Action(ribbon.Alpha, true, 1);
                   }
                if (node is CLight light)
                {
                    RemoveUnusedKeyframes_Action(light.Color, true, 1, 1, 1);
                    RemoveUnusedKeyframes_Action(light.Visibility, true, 1);

                    RemoveUnusedKeyframes_Action(light.AmbientColor);
                    RemoveUnusedKeyframes_Action(light.Intensity);
                    RemoveUnusedKeyframes_Action(light.AmbientIntensity);
                    RemoveUnusedKeyframes_Action(light.AttenuationEnd);
                    RemoveUnusedKeyframes_Action(light.AttenuationStart);
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
            foreach (CGlobalSequence gs in Model.GlobalSequences)
            {
              //  if (track == gs.)
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
            List<INode> Remove = new List<INode>();
            for (int i =0; i< Model.Nodes.Count; i++)
            {
                INode node = Model.Nodes[i];
                if (node is CBone)
                {
                    if (BoneHasAttachees(node) == false )
                    {
                        if (NodeHasChildren(node))
                        {
                            Model.Nodes.Remove(node);
                            CAnimator<CVector3> t = node.Translation;
                            CAnimator<CVector3> s = node.Scaling;
                            CAnimator<CVector4> r = node.Rotation;
                            node = new CHelper(Model);
                            foreach (var item in t) node.Translation.Add(item);
                            foreach (var item in r) node.Rotation.Add(item);
                            foreach (var item in s) node.Scaling.Add(item);
                        }
                        else
                        {
                            Remove.Add(node);
                        }
                    }
                }
            }
            foreach (INode node in Remove) Model.Nodes.Remove(node);
        }
        private static bool NodeHasChildren(INode checkedNode)
        {
            foreach (INode loopedNode in Model.Nodes)
            {
                if (loopedNode.Parent.Node == null) { continue; }
                if (loopedNode.Parent.Node == checkedNode) { return true; }
            }
            return false;
        }
        private static bool BoneHasAttachees(INode checkedBone)
        {
           // bool NoeExistInGroups = NoeExistInGroups_(checkedBone);
            foreach (CGeoset geo in Model.Geosets)
            {
                foreach (CGeosetGroup group in geo.Groups)
                {
                    foreach (var node in group.Nodes)
                    {
                        if (node.Node.Node == checkedBone)
                        {
                            foreach (CGeosetVertex vertex in geo.Vertices)
                            {
                                if (vertex.Group.Object == group)
                                {
                                    return true;
                                }
                            }
                             }
                    }
                }
            }
            return false;
        }
        private static bool NoeExistInGroups_(INode checkedBone)
        {
            foreach (CGeoset geo in Model.Geosets)
            {
                foreach (CGeosetGroup group in geo.Groups)
                {
                    foreach (var node in group.Nodes)
                    {
                        if (node.Node.Node == checkedBone) { return true; }
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
            foreach (CGeosetTriangle face in geo.Triangles)
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
        public static void RemoveEmptyGeosets(CModel model = null)
        {
            if (model == null)
            {
                foreach (CGeoset geo in Model.Geosets.ToList())
                {
                    if (geo.Triangles.Count == 0 || geo.Vertices.Count < 3)
                    {
                        Model.Geosets.Remove(geo);
                    }
                }
            }
            else
            {
                foreach (CGeoset geo in model.Geosets.ToList())
                {
                    if (geo.Triangles.Count == 0 || geo.Vertices.Count < 3)
                    {
                        Model.Geosets.Remove(geo);
                    }
                }
            }
           
        }
        private static void DeleteIsolatedTriangles_()
        {
            foreach (CGeoset geo in Model.Geosets)
            {
                foreach (CGeosetTriangle face in geo.Triangles.ToList())
                {
                    var vertex1 = face.Vertex1;
                    var vertex2 = face.Vertex1;
                    var vertex3 = face.Vertex1;
                    if (TriangleIsIsolated(face, geo) == true)
                    {
                        geo.Triangles.Remove(face);
                    }
                }
            }
            RemoveEmptyGeosets();
        }
        private static bool TriangleIsIsolated(CGeosetTriangle face_input, CGeoset geoset)
        {
            foreach (CGeosetTriangle face in geoset.Triangles)
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
                    Model.Sequences[i].Name = names[i] + " " + IDCounter.Next_;
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
        private static void CreateMAterialIfNo()
        {
            if (Model.Materials.Count == 0)
            {
                CreateWhiteTextureifNo();
                 CMaterial mat = new CMaterial(Model);
                CMaterialLayer layer = new CMaterialLayer(Model);
                layer.Texture.Attach(Model.Textures[0]);
                mat.Layers.Add(layer);
                Model.Materials.Add(mat);
            }
        }
        private static void FillMissingComponents()
        {
            CreateLayerForMaterialsWithout();
            // materials without layers
            // layers without textures
            foreach (INode node in Model.Nodes)
            {
                // emitter2 without texture
                if (node is CParticleEmitter2 emitter)
                {
                    if (emitter.Texture == null || emitter.Texture.Object == null ||
                        Model.Textures.Contains(emitter.Texture.Object) == false)
                    {
                        CreateWhiteTextureifNo();
                        emitter.Texture.Attach(Model.Textures[0]);
                    }
                }
                // ribbon without material
                if (node is CRibbonEmitter ribbon)
                {
                    if (ribbon.Material == null || ribbon.Material.Object == null ||
                        Model.Materials.Contains(ribbon.Material.Object) == false)
                    {
                        CreateMAterialIfNo();
                        ribbon.Material.Attach(Model.Materials[0]);
                    }
                }
            }
            // geoset without material
            foreach (CGeoset geoset in Model.Geosets)
            {
                if (geoset.Material == null || geoset.Material.Object == null ||
                    Model.Materials.Contains(geoset.Material.Object) == false)
                {
                    CreateMAterialIfNo();
                    geoset.Material.Attach(Model.Materials[0]);
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
        public static bool MaterialUsed(CMaterial mat)
        {
            foreach (CGeoset geo in Model.Geosets)
            {
                if (geo.Material.Object == mat   ) { return true; }
            }
            foreach (INode node in Model.Nodes)
            {
                if (node is CRibbonEmitter ribbon)
                {
                    if (ribbon.Material.Object == mat) { return true; }
                }
            }
            return false;
        }
        public static bool MaterialUsed(CMaterial mat, CModel model)
        {
            foreach (CGeoset geo in model.Geosets)
            {
                if (geo.Material.Object == mat) { return true; }
            }
            foreach (INode node in model.Nodes)
            {
                if (node is CRibbonEmitter ribbon)
                {
                    if (ribbon.Material.Object == mat) { return true; }
                }
            }
            return false;
        }
        private static void DeleteUnusedHelpers_()
        {
            foreach (INode node in Model.Nodes.ToList())
            {
                if (node is CHelper)
                {
                    if (HasChildren(node) == false)
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
                if (Calculator.GlobalSequenceIsUsed(Model, gs) == false)
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

        internal static void FalseAll()
        {
            Optimizer.Linearize = false;
            Optimizer.DeleteIsolatedTriangles = false;
            Optimizer.DeleteIsolatedVertices = false;
            Optimizer.Delete0LengthSequences = false;
            Optimizer.Delete0LengthGlobalSequences = false;
            Optimizer.DeleteUnAnimatedSequences = false;
            Optimizer.DeleteUnusedGlobalSequences = false;
            Optimizer.DeleteUnusedBones = false;
            Optimizer.DeleteUnusedHelpers = false;
            Optimizer.DeleteUnusedMAterials = false;
            Optimizer.DeleteUnusedTextures = false;
            Optimizer.DeleteUnusedTextureAnimations = false;
            Optimizer.DeleteUnusedKeyframes = false;
            Optimizer.MergeGeosets = false;
            Optimizer.DelUnusedMatrixGroups = false;
            Optimizer.CalculateExtents = false;
            Optimizer.MakeVisibilitiesNone = false;
            Optimizer.AddMissingVisibilities = false;
            Optimizer.ClampUVs = false;
            Optimizer.ClampLights =   false;
            Optimizer.DeleteDuplicateGAs =  false;
            Optimizer.AddMissingGAs =  false;
            Optimizer.SetAllStaticGAS = false;
            Optimizer.ClampKeyframes = false;
            Optimizer.DeleteIdenticalAdjascentKEyframes =       false;
            Optimizer.Check_DeleteIdenticalAdjascentKEyframes_times = false;
            Optimizer.DeleteSimilarSimilarKEyframes = false;
            
            Optimizer.StretchKeyframes = false;


            Optimizer.OtherMissingKeyframes = false;
            Optimizer.AddDefaultMissingOpeningKeyframes = false;
            Optimizer.AddDefaultMissingClosingKeyframes = false;

            Optimizer.MoveFirstKeyframeToStart = false;
            Optimizer.MoveLastKeyframeToEnd = false;

            Optimizer.DuplicateFirstKEyframeToStart = false;
            Optimizer.DuplicateLastKeyframeToEnd = false;



            Optimizer._DetachFromNonBone = false;
            Optimizer.DleteOverlapping1 = false;
            Optimizer.DleteOverlapping2 = false;
            Optimizer.ClampNormals = false;
            Optimizer.DeleteTrianglesWithNoArea = false;
            Optimizer.MergeIdenticalVertices = false;
            Optimizer.DeleteDuplicateGEosets = false;
            Optimizer.MergeTextures = false;
            Optimizer.MergeMAtertials = false;
            Optimizer.MergeTAs = false;
            Optimizer.MergeLayers = false;
            Optimizer.MinimizeMatrixGroups = false;
            Optimizer.FixNoMatrixGroups = false;
        }

        internal static void CalculateExtentsWithException(CModel model, List<int> indexes)
        {
            Model = model;
            CalculateExtents_(indexes);
        }
    }
}
