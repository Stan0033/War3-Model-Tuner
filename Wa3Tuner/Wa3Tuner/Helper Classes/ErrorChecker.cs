using MdxLib.Animator;
using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Xml.Linq;
using Wa3Tuner.Dialogs;
using Wa3Tuner.Helper_Classes;
namespace Wa3Tuner
{
    internal static class ErrorChecker
    {
        public static CModel CurrentModel;
        private static bool TextureUsed(CTexture texture)
        {
            foreach (CMaterial mat in CurrentModel.Materials)
            {
                foreach (CMaterialLayer layer in mat.Layers)
                {
                    if (layer.Texture.Object == texture) { return true; }
                }
            }
            foreach (INode node in CurrentModel.Nodes)
            {
                if (node is CParticleEmitter2 emitter)
                {
                    if (emitter.Texture.Object == texture) { return true; }
                }
                if (node is CParticleEmitter emitter1)
                {
                    if (emitter1.FileName == texture.FileName) { return true; }
                }
            }
            return false;
        }
        
        internal static string Inspect(CModel currentModel)
        {
            // lists with categorized errors
            StringBuilder all = new StringBuilder();
            StringBuilder unused = new StringBuilder();
            StringBuilder warnings = new StringBuilder();
            StringBuilder severe = new StringBuilder();
            StringBuilder errors = new StringBuilder();
            //BEGIN
            //----------------------------------------
            //check unused textures
            for (int i = 0; i < currentModel.Textures.Count; i++)
            {
                if (TextureUsed(currentModel.Textures[i]) == false)
                {
                    string name = currentModel.Textures[i].ReplaceableId == 0 ? currentModel.Textures[i].FileName
                        : "Replaceable ID " + currentModel.Textures[i].ReplaceableId.ToString();
                    unused.AppendLine($"Textures[{i}] ({name}) is unused");
                }
            }
            // check for triangle who use the same vertices
            for (int g = 0; g < currentModel.Geosets.Count; g++)
            {
                CGeoset geoset = currentModel.Geosets[g];
                for (int i = 0; i < geoset.Triangles.Count; i++)
                {
                    for (int x = 0; x < geoset.Triangles.Count; x++)
                    {
                        if (x == i) { continue; }
                        if (FacesShareSameVertices(geoset.Triangles[i], geoset.Triangles[x]))
                        {
                            warnings.AppendLine($"Geosets[{g}]: Triangle {i} and triangle {x} are using the same vertices");
                        }
                        if (Optimizer.FacesFullyOverlapping(geoset.Triangles[i], geoset.Triangles[x]))
                        {
                            warnings.AppendLine($"Geosets[{g}]: Triangle {i} and triangle {x} are fully overlapping");
                        }
                        if (Calculator.HasZeroArea(geoset.Triangles[i]))
                        {
                            warnings.AppendLine($"Geosets[{g}]: Triangle {i} has no area");
                        }
                    }
                }
            }
            for (int i = 0; i < currentModel.Materials.Count; i++)
            {
                if (Optimizer. MaterialUsed(currentModel.Materials[i], currentModel) == false)
                {
                    
                    unused.AppendLine($"Materials[{i}] is unused");
                    if (currentModel.Materials[i] == null) { throw new Exception("null material"); }
                    if (currentModel.Materials[i].Layers == null) { throw new Exception("null layer container"); }
                   for (int j = 0; j < currentModel.Materials[i].Layers.Count; j++)
                    {
                        var lr = currentModel.Materials[i].Layers[j];
                        if (AllTransformationValuesSame(lr.Alpha))
                        {
                            unused.AppendLine($"Material[{i}]: Layer[{j}]: alpha: all keyframes are the same");
                        }
                        if (AllTransformationValuesSame(lr.TextureId))
                        {
                            unused.AppendLine($"Material[{i}]: Layer[{j}]: TextureId: all keyframes are the same");
                        }
                        var list1 = SequencesInWhichTracksAreTheSame(currentModel, lr.Alpha);
                        var list2 = SequencesInWhichTracksAreTheSame(currentModel, lr.TextureId);
                        foreach (var item in list1)
                        {
                            unused.AppendLine($"Material[{i}]: Layer[{j}]: alpha: all keyframes are the same in sequence {item.Name}");
                        }
                        foreach (var item in list1)
                        {
                            unused.AppendLine($"Material[{i}]: Layer[{j}]: TextureId: all keyframes are the same in sequence {item.Name}");
                        }
                    }
                }
            }
            for (int i = 0; i < currentModel.TextureAnimations.Count; i++)
            {
                if (TAIsUsed(currentModel.TextureAnimations[i]) == false)
                {
                    unused.AppendLine($"TextureAnims[{i}] is unused");
                }
                if (AllTransformationValuesSame(currentModel.TextureAnimations[i].Translation))
                {
                    unused.AppendLine($"TextureAnims[{i}] all keyframes' values for translation are the same");
                }
                if (AllTransformationValuesSame(currentModel.TextureAnimations[i].Rotation))
                {
                    unused.AppendLine($"TextureAnims[{i}] all keyframes' values for translation are the same");
                }
                if (AllTransformationValuesSame(currentModel.TextureAnimations[i].Scaling))
                {
                    unused.AppendLine($"TextureAnims[{i}] all keyframes' values for translation are the same");
                }
                var list1 = SequencesInWhichTracksAreTheSame(currentModel, currentModel.TextureAnimations[i].Translation);
                var list2 = SequencesInWhichTracksAreTheSame(currentModel, currentModel.TextureAnimations[i].Rotation);
                var list3 = SequencesInWhichTracksAreTheSame(currentModel, currentModel.TextureAnimations[i].Scaling);

                foreach (var item in list1)
                {
                    unused.AppendLine($"TextureAnimation[{i}]: translation: all keyframes are the same in sequence {item.Name}");
                }
                foreach (var item in list2)
                {
                    unused.AppendLine($"TextureAnimation[{i}]: rotation: all keyframes are the same in sequence {item.Name}");
                }
                foreach (var item in list1)
                {
                    unused.AppendLine($"TextureAnimation[{i}]: scaling: all keyframes are the same in sequence {item.Name}");
                }
            }
            for (int i = 0; i < currentModel.Geosets.Count; i++)
            {
                if (currentModel.Geosets[i].Triangles.Count >= 20000)
                {
                    warnings.AppendLine($"Geosets[{i}]: more than 20,000 triangles");
                }
            }
            // unused.AppendLine($"");
            if (currentModel.Textures.Count == 0) warnings.AppendLine("No textures");
            if (currentModel.Materials.Count == 0) warnings.AppendLine("No Materials");
            if (currentModel.Sequences.Count == 0) warnings.AppendLine("No sequences");
            if (currentModel.Nodes.Any(x => x.Name.ToLower().Trim() == "origin ref") == false) warnings.AppendLine("Missing the origin attachment point");
            for (int i = 0; i < currentModel.Geosets.Count; i++)
            {
                CGeoset geo = currentModel.Geosets[i];
                if (geo.Triangles.Count == 0) { warnings.AppendLine($"Geosets[{i}]: no faces"); }
                if (geo.Vertices.Count == 0) { warnings.AppendLine($"Geosets[{i}]: no vertices"); }
                if (geo.Extents.Count != currentModel.Sequences.Count) { warnings.AppendLine($"Geosets[{i}]: number of extents not equal to number of sequences"); }
                if (ExtentsNegative(geo.Extent)) { warnings.AppendLine($"Geosets[{i}]: negative extents"); }
                if (geo.Groups.Count > 255) { warnings.AppendLine($"Geosets[{i}]: More than 255 matrix groups. Consider minimizing the wth the optmizer."); }
            }
            foreach (CSequence cSequence in currentModel.Sequences)
            {
                if (ExtentsNegative(cSequence.Extent)) { warnings.AppendLine($"Sequence '{cSequence.Name}': negative extents"); }
                if (cSequence.IntervalEnd == cSequence.IntervalStart)
                {
                    warnings.AppendLine($"Sequence '{cSequence.Name}': Zero length");
                }
            }
            for (int i = 0;i < currentModel.GlobalSequences.Count; i++)
            {
                var gs = currentModel.GlobalSequences[i];
                if (Calculator.GlobalSequenceIsUsed(currentModel, gs) == false)
                {
                    unused.AppendLine($"GlobalSequences[{i}] is unused");
                }
            }
            
            if (currentModel.Geosets.Count != currentModel.GeosetAnimations.Count) {
                warnings.AppendLine("Number of geoset animations not equal to the number of geosets");
            }
            // check tracks - repeating times, repeating frames, inconsistent frames, missing opening, closing
            List<string> ik = CheckInsonsistentKeyframes();
            List<string> rk = ChekRepeatingTimes();
            List<string> mok = ChekMissingOpeningKeyframes();
            // List<string> dk= CheckDuplicatingKeyframes();
            foreach (string i in ik) { severe.AppendLine(i); }
            foreach (string i in rk) { unused.AppendLine(i); }
            foreach (string i in mok) { unused.AppendLine(i); }
            // foreach (string i in dk) { warnings.AppendLine(i); }
            // check interpolation for visibilities
            foreach (INode node in currentModel.Nodes)
            {
                if (node is CLight light)
                {
                    if (light.Visibility.Type != MdxLib.Animator.EInterpolationType.None)
                    {
                        warnings.AppendLine($"Attachment '{node.Name}': Interpolation not set to none");
                    }
                    AppendSameKeyframes($"Light '{node.Name}': visibility: ", light.Visibility, unused, currentModel);
                    AppendSameKeyframes($"Light '{node.Name}': Color: ", light.Color, unused, currentModel);
                    AppendSameKeyframes($"Light '{node.Name}': AmbientColor: ", light.AmbientColor, unused, currentModel);
                    AppendSameKeyframes($"Light '{node.Name}': AttenuationEnd: ", light.AttenuationEnd, unused, currentModel);
                    AppendSameKeyframes($"Light '{node.Name}': AttenuationStart: ", light.AttenuationStart, unused, currentModel);
                    AppendSameKeyframes($"Light '{node.Name}': Intensity: ", light.Intensity, unused, currentModel);
                    AppendSameKeyframes($"Light '{node.Name}': AmbientIntensity: ", light.AmbientIntensity, unused, currentModel);

                }
                if (node is CAttachment attachment)
                {
                    if (attachment.Visibility.Type != MdxLib.Animator.EInterpolationType.None)
                    {
                        warnings.AppendLine($"Attachment '{node.Name}': Interpolation not set to none");
                   }
                    AppendSameKeyframes($"Attachment '{node.Name}': visibility: ", attachment.Visibility, unused, currentModel);

                }
                if (node is CParticleEmitter emitter)
                {
                    if (emitter.Visibility.Type != MdxLib.Animator.EInterpolationType.None)
                    {
                        warnings.AppendLine($"Emitter1 '{node.Name}': Interpolation not set to none");
                    }
                        AppendSameKeyframes($"Emitter1 '{node.Name}': visibility: ", emitter.Visibility, unused, currentModel);
                        AppendSameKeyframes($"Emitter1 '{node.Name}': Longitude: ", emitter.Longitude, unused, currentModel);
                        AppendSameKeyframes($"Emitter1 '{node.Name}': Latitude: ", emitter.Latitude, unused, currentModel);
                        AppendSameKeyframes($"Emitter1 '{node.Name}': EmissionRate: ", emitter.EmissionRate, unused, currentModel);
                        AppendSameKeyframes($"Emitter1 '{node.Name}': LifeSpan: ", emitter.LifeSpan, unused, currentModel);
                        AppendSameKeyframes($"Emitter1 '{node.Name}': Gravity: ", emitter.Gravity, unused, currentModel);
                        AppendSameKeyframes($"Emitter1 '{node.Name}': InitialVelocity: ", emitter.InitialVelocity, unused, currentModel);

                     
                }
                if (node is CParticleEmitter2 emitter2)
                {
                    if (emitter2.Visibility.Type != MdxLib.Animator.EInterpolationType.None)
                    {
                        warnings.AppendLine($"Emitter2 '{node.Name}': Interpolation not set to none");
                    }
                        AppendSameKeyframes($"Emitter2 '{node.Name}': visibility: ", emitter2.Visibility, unused, currentModel);
                        AppendSameKeyframes($"Emitter2 '{node.Name}': Width: ", emitter2.Width, unused, currentModel);
                        AppendSameKeyframes($"Emitter2 '{node.Name}': Length: ", emitter2.Length, unused, currentModel);
                        AppendSameKeyframes($"Emitter2 '{node.Name}': Gravity: ", emitter2.Gravity, unused, currentModel);
                        AppendSameKeyframes($"Emitter2 '{node.Name}': Speed: ", emitter2.Speed, unused, currentModel);
                        AppendSameKeyframes($"Emitter2 '{node.Name}': Latitude: ", emitter2.Latitude, unused, currentModel);
                        AppendSameKeyframes($"Emitter2 '{node.Name}': Variation: ", emitter2.Variation, unused, currentModel);
                    if (
                        emitter2.Segment1.Alpha <= 10 && 
                        emitter2.Segment2.Alpha <= 10 && 
                        emitter2.Segment3.Alpha <= 10 
                        ) 
                    {
                        warnings.AppendLine($"Emitter2 '{node.Name}': all segments have low visibility");

                    }
                }
                if (node is CRibbonEmitter ribbon)
                {
                    if (ribbon.Visibility.Type != MdxLib.Animator.EInterpolationType.None)
                    {
                        warnings.AppendLine($"Ribbon '{node.Name}': Interpolation not set to none");
                    }
                    AppendSameKeyframes($"Ribbon '{node.Name}': visibility: ", ribbon.Visibility, unused, currentModel);
                    AppendSameKeyframes($"Ribbon '{node.Name}': Color: ", ribbon.Color, unused, currentModel);
                    AppendSameKeyframes($"Ribbon '{node.Name}': Alpha: ", ribbon.Alpha, unused, currentModel);
                    AppendSameKeyframes($"Ribbon '{node.Name}': HeightAbove: ", ribbon.HeightAbove, unused, currentModel);
                    AppendSameKeyframes($"Ribbon '{node.Name}': HeightBelow: ", ribbon.HeightBelow, unused, currentModel);
                    AppendSameKeyframes($"Ribbon '{node.Name}': TextureSlot: ", ribbon.TextureSlot, unused, currentModel);
                }
            }
            List<INode> Particles = currentModel.Nodes.Where(x => x is CParticleEmitter || x is CParticleEmitter2 || x is CRibbonEmitter).ToList();
            if (currentModel.Sequences.Count == 0 && Particles.Count > 0)
            {
                errors.AppendLine("There are particle emitters, but no sequences");
            }
            // check non-bone attachment
            for (int i = 0; i < currentModel.Geosets.Count; i++)
            {
                CGeoset gep = currentModel.Geosets[i];
                if (gep.Groups.Count == 0)
                {
                    severe.AppendLine($"Geosets[{i}]: no matrix groups"); continue;
                }
                for (int j = 0; j < gep.Groups.Count; j++)
                {
                    CGeosetGroup group = gep.Groups[j];
                    for (int k = 0; k < group.Nodes.Count; k++)
                    {
                        if (group.Nodes[k].Node.Node is CBone == false)
                        {
                            INode bugged = group.Nodes[k].Node.Node;
                            severe.AppendLine($"Geosets[{i}]: group[{j}]: Node '{bugged.Name}' is not a bone"); continue;
                        }
                    }
                }
            }
            // check bone not attached to anything
            foreach (INode node in currentModel.Nodes)
            {
                if (node is CBone)
                {
                    if (NothingAttachedToBone(node))
                    {
                        warnings.AppendLine($"Nothing is attached to bone '{node.Name}'");
                    }
                }
            }
            // invalid event object
            foreach (INode node in currentModel.Nodes)
            {
                if (node is CEvent ev)
                {
                    if (ev.Tracks.Count == 0) errors.AppendLine($"Event object {ev.Name}: no tracks");
                }
            }
            
            // repeating geoset animations
            if (currentModel.Geosets.Count > 0)
            {
                for (int i = 0; i < currentModel.Geosets.Count; i++)
                {
                    CGeoset ge = currentModel.Geosets[i];
                    if (currentModel.GeosetAnimations.Count(x => x.Geoset.Object == ge) > 1)
                    {
                        warnings.AppendLine("");
                    }
                }
            }
                // inconsistent sequences
                if (currentModel.Sequences.Count > 1)
                {
                    for (int i = 0; i < currentModel.Sequences.Count; i++)
                    {
                        if (i - 1 != -1)
                        {
                            CSequence prev = currentModel.Sequences[i - 1];
                            CSequence current = currentModel.Sequences[i];
                            if (current.IntervalStart < prev.IntervalStart)
                            {
                                severe.AppendLine($"Sequence '{current.Name}' starts before the sequence before it");
                            }
                        }
                    }
                }
                // not attached to anything
                for (int g = 0; g < currentModel.Geosets.Count; g++)
                {
                    CGeoset geo = currentModel.Geosets[g];
                    for (int i = 0; i < geo.Vertices.Count; i++)
                    {
                        if (geo.Vertices[i].Group == null || geo.Vertices[i].Group.Object == null)
                        {
                            errors.AppendLine($"Geoset {g}: vertex {i} is not attached to anything"); continue;
                        }
                        if (geo.Vertices[i].Group.Object.Nodes.Count == 0)
                        {
                            errors.AppendLine($"Geoset {g}: vertex {i} is not attached to anything"); continue;
                        }
                    }
                    if (geo.Material.Object == null)
                    {
                        severe.AppendLine($"Geoset {g}: no material. The geoset will be invisible"); continue;
                    }
                    else
                    {
                        if (CurrentModel.Materials.Contains(geo.Material.Object) == false)
                        {
                            severe.AppendLine($"Geoset {g}: invalid material. The geoset will be invisible"); continue;
                        }
                    }
                }
                for (int i = 0; i < currentModel.GeosetAnimations.Count; i++)
                {
                    CGeosetAnimation anim = currentModel.GeosetAnimations[i];

                if (anim.Alpha.Animated == true)
                {
                    foreach (var sequence in currentModel.Sequences)
                    {
                        if (anim.Alpha.Any(x=> x.Time >= sequence.IntervalStart &&  x.Time <= sequence.IntervalEnd) == false)
                        {
                            warnings.AppendLine($"GeosetAnim {i}: Animated Alpha: missing entry for sequence {sequence.Name}"); continue;
                        }
                             
                }
                }
                if (anim.Color.Animated == true && anim.UseColor)
                {
                    foreach (var sequence in currentModel.Sequences)
                    {
                        if (anim.Color.Any(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd) == false)
                        {
                            warnings.AppendLine($"GeosetAnim {i}: Animated Color: missing entry for sequence {sequence.Name}"); continue;
                        }

                    }
                }
               

                // invisible?
                CGeosetAnimation ga = currentModel.GeosetAnimations[i];
                if (ga.Alpha.Static && ga.Alpha.GetValue() < 0.2)
                {
                    severe.AppendLine($"GeosetAnims[{i}]: 0 or near 0 static alpha, it may be invisible");
                }
                if (ga.Geoset == null || ga.Geoset.Object == null)
                {
                    errors.AppendLine($"GeosetAnims[{i}]: invalid geoset");
                }
                else
                {
                    if (currentModel.Geosets.Contains(ga.Geoset.Object) == false)
                    {
                        errors.AppendLine($"GeosetAnims[{i}]: invalid geoset");
                    }
                }

                foreach (CSequence sequence in currentModel.Sequences)
                {
                    if (anim.Alpha.Static == false)
                    {
                        if (anim.Alpha.Any(x => x.Time == sequence.IntervalStart) == false)
                        {
                            severe.AppendLine($"in geoset_animations[{i}]: alpha: missing opening track for sequence '{sequence.Name}'");
                        }
                    }
                    if (anim.Color.Static == false)
                    {
                        if (anim.Color.Any(x => x.Time == sequence.IntervalStart) == false)
                        {
                            severe.AppendLine($"in geoset_animations[{i}]: color: missing opening track for sequence '{sequence.Name}'");
                        }
                    }
                    if (AllTransformationValuesSame(currentModel.GeosetAnimations[i].Alpha))
                    {
                        unused.AppendLine($"GeosetAnimations[{i}] all keyframes' values for alpha are the same");
                    }
                    if (AllTransformationValuesSame(currentModel.GeosetAnimations[i].Color))
                    {
                        unused.AppendLine($"GeosetAnimations[{i}] all keyframes' values for color are the same");
                    }

                  
                }

                var list1 = SequencesInWhichTracksAreTheSame(currentModel, anim.Alpha);
                var list2 = SequencesInWhichTracksAreTheSame(currentModel, anim.Color);
                foreach (var item in list1)
                {
                    unused.AppendLine($"GeosetAnimation[{i}]: alpha: all keyframes are the same in sequence {item.Name}");
                }
                foreach (var item in list1)
                {
                    unused.AppendLine($"GeosetAnimation[{i}]: color: all keyframes are the same in sequence {item.Name}");
                }


            }
                // check for overlapping triangles
                if (currentModel.Geosets.Count > 0)
                {
                    for (int one = 0; one < currentModel.Geosets.Count; one++)
                    {
                        CGeoset geoset1 = currentModel.Geosets[one];
                        for (int face1Index = 0; face1Index < geoset1.Triangles.Count; face1Index++)
                        {
                            // Compare with other geosets (or the same geoset but without redundant checks)
                            for (int two = one; two < currentModel.Geosets.Count; two++)
                            {
                                CGeoset geoset2 = currentModel.Geosets[two];
                                // Avoid duplicate checks when comparing the same geoset
                                int startIndex = (one == two) ? face1Index + 1 : 0;
                                for (int face2Index = startIndex; face2Index < geoset2.Triangles.Count; face2Index++)
                                {
                                    // Check for overlap
                                    if (OverlapChecker.TrianglesIntersect(geoset1.Triangles[face1Index], geoset2.Triangles[face2Index]))
                                    {
                                        warnings.AppendLine(
                                            $"geosets[{one}].triangles[{face1Index}] is overlapping with geosets[{two}].triangles[{face2Index}]");
                                    }
                                }
                            }
                        }
                    }
                }
                if (unused.Length == 0 && warnings.Length == 0 &&
                    severe.Length == 0 && errors.Length == 0) { all.AppendLine("All ok"); }
                else
                {
                    all.AppendLine("----------Unused:");
                    all.AppendLine(unused.ToString());
                    all.AppendLine("----------Warnings:");
                    all.AppendLine(warnings.ToString());
                    all.AppendLine("----------Severe:");
                    all.AppendLine(severe.ToString());
                    all.AppendLine("----------Errors:");
                    all.AppendLine(errors.ToString());
                }
            return all.ToString();
        }

        private static void AppendSameKeyframes(string v, CAnimator<int> textureSlot, StringBuilder unused, CModel currentModel)
        {
            throw new NotImplementedException();
        }

        private static void AppendSameKeyframes(string v, CAnimator<CVector3> animator, StringBuilder unused, CModel model)
        {
            if (AllTransformationValuesSame(animator)) unused.AppendLine(v + "All keyframes have the same values");
            var list = SequencesInWhichTracksAreTheSame(model, animator);
            foreach (var item in list)
            {
                unused.AppendLine($"{v}: all keyframes are the same in sequence {item.Name}");
            }
        }

        private static void AppendSameKeyframes(string v, CAnimator<float> animator, StringBuilder unused, CModel model)
        {
            if (AllTransformationValuesSame(animator)) unused.AppendLine(v + "All keyframes have the same values");
            var list = SequencesInWhichTracksAreTheSame(model, animator);
            foreach (var item in list)
            {
                unused.AppendLine($"{v}: all keyframes are the same in sequence {item.Name}");
            }
        }

        private static bool AllTransformationValuesSame(CAnimator<CVector4> animator)
        {
            if (animator.Static) return false;
            if (animator.Count <= 1) return false;
            bool same = true;
            for (int i = 1; i < animator.Count; i++)
            {
                if (animator[i].Value != animator[0].Value) { same = false; break; }
            }
            return same;
        }

        private static bool AllTransformationValuesSame(CAnimator<CVector3> animator)
        {
            if (animator.Static) return false;
            if (animator.Count <= 1) return false;
            bool same = true;
            for (int i = 1; i < animator.Count; i++)
            {
                if (animator[i].Value != animator[0].Value) { same = false; break; }
            }
            return same;
            
        }
        private static bool AllTransformationValuesSame(CAnimator<float> animator)
        {
            if (animator.Static) return false;
            if (animator.Count <= 1) return false;
            bool same = true;
            for (int i = 1; i < animator.Count; i++)
            {
                if (animator[i].Value != animator[0].Value) { same = false; break; }
            }
            return same;

        }
        private static bool AllTransformationValuesSame(CAnimator<int> animator)
        {
            if (animator.Static) return false;
            if (animator.Count <= 1) return false;
            bool same = true;
            for (int i = 1; i < animator.Count; i++)
            {
                if (animator[i].Value != animator[0].Value) { same = false; break; }
            }
            return same;

        }

        // Check if a point is inside a triangle
        // Helper methods for vector operations
        private static bool FacesShareSameVertices(CGeosetTriangle face1, CGeosetTriangle face2)
        {
            // Directly compare the 3 vertices using their Object references
            return (face1.Vertex1.Object == face2.Vertex1.Object || face1.Vertex1.Object == face2.Vertex2.Object || face1.Vertex1.Object == face2.Vertex3.Object) &&
                   (face1.Vertex2.Object == face2.Vertex1.Object || face1.Vertex2.Object == face2.Vertex2.Object || face1.Vertex2.Object == face2.Vertex3.Object) &&
                   (face1.Vertex3.Object == face2.Vertex1.Object || face1.Vertex3.Object == face2.Vertex2.Object || face1.Vertex3.Object == face2.Vertex3.Object);
        }
        private static List<string> CheckDuplicatingKeyframes()
        {
            List<string> list = new List<string>();
            foreach (INode node in CurrentModel.Nodes)
            {
                ReportDuplicatingKeyframes(node.Translation, $"At {node.GetType().Name} '{node.Name}': translation: ");
                ReportDuplicatingKeyframes(node.Rotation, $"At {node.GetType().Name} '{node.Name}': rotation: ");
                ReportDuplicatingKeyframes(node.Scaling, $"At {node.GetType().Name} '{node.Name}': scaling: ");
                if (node is CAttachment)
                {
                    CAttachment element = (CAttachment)node;
                    ReportRepeatingTimes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                    ReportDuplicatingKeyframes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                    ReportDuplicatingKeyframes(element.EmissionRate, $"At {node.GetType().Name} '{node.Name}': emission rate: ");
                    ReportDuplicatingKeyframes(element.LifeSpan, $"At {node.GetType().Name} '{node.Name}': lifespan: ");
                    ReportDuplicatingKeyframes(element.InitialVelocity, $"At {node.GetType().Name} '{node.Name}': initial velocity ");
                    ReportDuplicatingKeyframes(element.Gravity, $"At {node.GetType().Name} '{node.Name}': gravity: ");
                    ReportDuplicatingKeyframes(element.Longitude, $"At {node.GetType().Name} '{node.Name}': longtitude: ");
                    ReportDuplicatingKeyframes(element.Latitude, $"At {node.GetType().Name} '{node.Name}': latitude: ");
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    ReportDuplicatingKeyframes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                    ReportDuplicatingKeyframes(element.EmissionRate, $"At {node.GetType().Name} '{node.Name}': emission rate: ");
                    ReportDuplicatingKeyframes(element.Speed, $"At {node.GetType().Name} '{node.Name}': speed: ");
                    ReportDuplicatingKeyframes(element.Width, $"At {node.GetType().Name} '{node.Name}': width: ");
                    ReportDuplicatingKeyframes(element.Gravity, $"At {node.GetType().Name} '{node.Name}': gravity: ");
                    ReportDuplicatingKeyframes(element.Length, $"At {node.GetType().Name} '{node.Name}': length: ");
                    ReportDuplicatingKeyframes(element.Latitude, $"At {node.GetType().Name} '{node.Name}': latitude: ");
                    ReportDuplicatingKeyframes(element.Variation, $"At {node.GetType().Name} '{node.Name}': variation: ");
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    ReportDuplicatingKeyframes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                    ReportDuplicatingKeyframes(element.HeightAbove, $"At {node.GetType().Name} '{node.Name}': height above: ");
                    ReportDuplicatingKeyframes(element.HeightBelow, $"At {node.GetType().Name} '{node.Name}': height below: ");
                    ReportDuplicatingKeyframes(element.Color, $"At {node.GetType().Name} '{node.Name}': color: ");
                    ReportDuplicatingKeyframes(element.Alpha, $"At {node.GetType().Name} '{node.Name}': alpha: ");
                    ReportDuplicatingKeyframes(element.TextureSlot, $"At {node.GetType().Name} '{node.Name}': texture slot: ");
                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;
                    ReportDuplicatingKeyframes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                    ReportDuplicatingKeyframes(element.Color, $"At {node.GetType().Name} '{node.Name}': color: ");
                    ReportDuplicatingKeyframes(element.AmbientColor, $"At {node.GetType().Name} '{node.Name}': ambient color: ");
                    ReportDuplicatingKeyframes(element.Intensity, $"At {node.GetType().Name} '{node.Name}': intensity: ");
                    ReportDuplicatingKeyframes(element.AmbientIntensity, $"At {node.GetType().Name} '{node.Name}': ambient intensity: ");
                    ReportDuplicatingKeyframes(element.AttenuationEnd, $"At {node.GetType().Name} '{node.Name}': attentuation end: ");
                    ReportDuplicatingKeyframes(element.AttenuationStart, $"At {node.GetType().Name} '{node.Name}': attentuation start: ");
                }
            }
            for (int i = 0; i < CurrentModel.GeosetAnimations.Count; i++)
            {
                CGeosetAnimation ga = CurrentModel.GeosetAnimations[i];
                if (ga.Alpha.Static == false)
                {
                    ReportDuplicatingKeyframes(ga.Alpha, $"At geoset_animations[{i}]: alpha: ");
                }
                if (ga.Color.Static == false) { ReportRepeatingTimes(ga.Color, $"At geoset_animations[{i}]: color: "); }
            }
            for (int i = 0; i < CurrentModel.TextureAnimations.Count; i++)
            {
                CTextureAnimation taa = CurrentModel.TextureAnimations[i];
                ReportDuplicatingKeyframes(taa.Translation, $"At texture_animation[{i}]: translation: ");
                ReportDuplicatingKeyframes(taa.Rotation, $"At texture_animation[{i}]: rotation: ");
                ReportDuplicatingKeyframes(taa.Scaling, $"At texture_animation[{i}]: scaling: ");
            }
            for (int i = 0; i < CurrentModel.Materials.Count; i++)
            {
                for (int x = 0; x < CurrentModel.Materials[i].Layers.Count; x++)
                {
                    CMaterialLayer layer = CurrentModel.Materials[i].Layers[x];
                    ReportDuplicatingKeyframes(layer.Alpha, $"At materials[{i}]: layers[{x}] alpha: ");
                    ReportDuplicatingKeyframes(layer.TextureId, $"At materials[{i}]: layers[{x}] textureid: ");
                }
            }
            for (int i = 0; i < CurrentModel.Cameras.Count; i++)
            {
                var cam = CurrentModel.Cameras[i];
                ReportDuplicatingKeyframes(cam.Rotation, $"At cameras[{i}]: rotation: ");
            }
            list.AddRange(TemporaryList);
            TemporaryList.Clear();
            return list;
        }
        private static void ReportDuplicatingKeyframes(CAnimator<CVector3> animator, string v)
        {
            if (animator.Count > 3)
            {
                for (int i = 0; i < animator.Count - 2; i++) // Use animator.Count - 2 to ensure you don't access out of bounds
                {
                    if (SameValues(animator[i].Value, animator[i + 1].Value, animator[i + 2].Value) &&
                        TripletBelongsToSameSequence(animator[i].Time, animator[i + 1].Time, animator[i + 2].Time))
                    {
                        TemporaryList.Add($"{v}: The track at {animator[i + 1].Time} has the same values as the ones before and after it");
                    }
                }
            }
        }
        private static void ReportDuplicatingKeyframes(CAnimator<CVector4> animator, string v)
        {
            if (animator.Count > 3)
            {
                for (int i = 0; i < animator.Count - 2; i++) // Use animator.Count - 2 to ensure you don't access out of bounds
                {
                    if (SameValues(animator[i].Value, animator[i + 1].Value, animator[i + 2].Value) &&
                        TripletBelongsToSameSequence(animator[i].Time, animator[i + 1].Time, animator[i + 2].Time))
                    {
                        TemporaryList.Add($"{v}: The track at {animator[i + 1].Time} has the same values as the ones before and after it");
                    }
                }
            }
        }
        private static void ReportDuplicatingKeyframes(CAnimator<int> animator, string v)
        {
            if (animator.Count > 3)
            {
                for (int i = 0; i < animator.Count - 2; i++) // Use animator.Count - 2 to ensure you don't access out of bounds
                {
                    if (SameValues(animator[i].Value, animator[i + 1].Value, animator[i + 2].Value) &&
                        TripletBelongsToSameSequence(animator[i].Time, animator[i + 1].Time, animator[i + 2].Time))
                    {
                        TemporaryList.Add($"{v}: The track at {animator[i + 1].Time} has the same values as the ones before and after it");
                    }
                }
            }
        }
        private static bool SameValues(float one, float two, float three)
        {
            return (one == two && two == three);
        }
        private static bool SameValues(int one, int two, int three)
        {
            return (one == two && two == three);
        }
        private static bool SameValues(CVector3 one, CVector3 two, CVector3 three)
        {
            return (one.X == two.X && one.Y == two.Y && one.Z == two.Z &&
                    two.X == three.X && two.Y == three.Y && two.Z == three.Z);
        }
        private static bool SameValues(CVector4 one, CVector4 two, CVector4 three)
        {
            return (one.X == two.X && one.Y == two.Y && one.Z == two.Z && one.W == two.W &&
                    two.X == three.X && two.Y == three.Y && two.Z == three.Z && two.W == three.W);
        }
        private static bool TripletBelongsToSameSequence(int time1, int time2, int time3)
        {
            foreach (CSequence sequence in CurrentModel.Sequences)
            {
                if (
                    time1 >= sequence.IntervalStart && time1 >= sequence.IntervalEnd &&
                    time2 >= sequence.IntervalStart && time2 >= sequence.IntervalEnd &&
                    time3 >= sequence.IntervalStart && time3 >= sequence.IntervalEnd
                    )
                {
                    return true;
                }
            }
            return false;
        }
        private static void ReportDuplicatingKeyframes(CAnimator<float> animator, string v)
        {
            if (animator.Count > 3)
            {
                for (int i = 0; i < animator.Count - 2; i++) // Use animator.Count - 2 to ensure you don't access out of bounds
                {
                    if (SameValues(animator[i].Value, animator[i + 1].Value, animator[i + 2].Value) &&
                        TripletBelongsToSameSequence(animator[i].Time, animator[i + 1].Time, animator[i + 2].Time))
                    {
                        TemporaryList.Add($"{v}: The track at {animator[i + 1].Time} has the same values as the ones before and after it");
                    }
                }
            }
        }
        private static List<string> ChekMissingOpeningKeyframes()
        {
            List<string> list = new List<string>();
            foreach (INode node in CurrentModel.Nodes)
            {
                ReportMissingOpeningKeyframes(node.Translation, $"At {node.GetType().Name} '{node.Name}': translation: ");
                ReportMissingOpeningKeyframes(node.Rotation, $"At {node.GetType().Name} '{node.Name}': rotation: ");
                ReportMissingOpeningKeyframes(node.Scaling, $"At {node.GetType().Name} '{node.Name}': scaling: ");
                if (node is CAttachment)
                {
                    CAttachment element = (CAttachment)node;
                    ReportMissingOpeningKeyframes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                    ReportMissingOpeningKeyframes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                    ReportMissingOpeningKeyframes(element.EmissionRate, $"At {node.GetType().Name} '{node.Name}': emission rate: ");
                    ReportMissingOpeningKeyframes(element.LifeSpan, $"At {node.GetType().Name} '{node.Name}': lifespan: ");
                    ReportMissingOpeningKeyframes(element.InitialVelocity, $"At {node.GetType().Name} '{node.Name}': initial velocity ");
                    ReportMissingOpeningKeyframes(element.Gravity, $"At {node.GetType().Name} '{node.Name}': gravity: ");
                    ReportMissingOpeningKeyframes(element.Longitude, $"At {node.GetType().Name} '{node.Name}': longtitude: ");
                    ReportMissingOpeningKeyframes(element.Latitude, $"At {node.GetType().Name} '{node.Name}': latitude: ");
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    ReportMissingOpeningKeyframes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                    ReportMissingOpeningKeyframes(element.EmissionRate, $"At {node.GetType().Name} '{node.Name}': emission rate: ");
                    ReportMissingOpeningKeyframes(element.Speed, $"At {node.GetType().Name} '{node.Name}': speed: ");
                    ReportMissingOpeningKeyframes(element.Width, $"At {node.GetType().Name} '{node.Name}': width: ");
                    ReportMissingOpeningKeyframes(element.Gravity, $"At {node.GetType().Name} '{node.Name}': gravity: ");
                    ReportMissingOpeningKeyframes(element.Length, $"At {node.GetType().Name} '{node.Name}': length: ");
                    ReportMissingOpeningKeyframes(element.Latitude, $"At {node.GetType().Name} '{node.Name}': latitude: ");
                    ReportMissingOpeningKeyframes(element.Variation, $"At {node.GetType().Name} '{node.Name}': variation: ");
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    ReportMissingOpeningKeyframes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                    ReportMissingOpeningKeyframes(element.HeightAbove, $"At {node.GetType().Name} '{node.Name}': height above: ");
                    ReportMissingOpeningKeyframes(element.HeightBelow, $"At {node.GetType().Name} '{node.Name}': height below: ");
                    ReportMissingOpeningKeyframes(element.Color, $"At {node.GetType().Name} '{node.Name}': color: ");
                    ReportMissingOpeningKeyframes(element.Alpha, $"At {node.GetType().Name} '{node.Name}': alpha: ");
                    ReportMissingOpeningKeyframes(element.TextureSlot, $"At {node.GetType().Name} '{node.Name}': texture slot: ");
                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;
                    ReportMissingOpeningKeyframes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                    ReportMissingOpeningKeyframes(element.Color, $"At {node.GetType().Name} '{node.Name}': color: ");
                    ReportMissingOpeningKeyframes(element.AmbientColor, $"At {node.GetType().Name} '{node.Name}': ambient color: ");
                    ReportMissingOpeningKeyframes(element.Intensity, $"At {node.GetType().Name} '{node.Name}': intensity: ");
                    ReportMissingOpeningKeyframes(element.AmbientIntensity, $"At {node.GetType().Name} '{node.Name}': ambient intensity: ");
                    ReportMissingOpeningKeyframes(element.AttenuationEnd, $"At {node.GetType().Name} '{node.Name}': attentuation end: ");
                    ReportMissingOpeningKeyframes(element.AttenuationStart, $"At {node.GetType().Name} '{node.Name}': attentuation start: ");
                }
            }
            for (int i = 0; i < CurrentModel.GeosetAnimations.Count; i++)
            {
                CGeosetAnimation ga = CurrentModel.GeosetAnimations[i];
                if (ga.Alpha.Static == false)
                {
                    ReportMissingOpeningKeyframes(ga.Alpha, $"At geoset_animations[{i}]: alpha: ");
                }
                if (ga.Color.Static == false) { ReportMissingOpeningKeyframes(ga.Color, $"At geoset_animations[{i}]: color: "); }
            }
            for (int i = 0; i < CurrentModel.TextureAnimations.Count; i++)
            {
                CTextureAnimation taa = CurrentModel.TextureAnimations[i];
                ReportMissingOpeningKeyframes(taa.Translation, $"At texture_animation[{i}]: translation: ");
                ReportMissingOpeningKeyframes(taa.Rotation, $"At texture_animation[{i}]: rotation: ");
                ReportMissingOpeningKeyframes(taa.Scaling, $"At texture_animation[{i}]: scaling: ");
            }
            for (int i = 0; i < CurrentModel.Materials.Count; i++)
            {
                for (int x = 0; x < CurrentModel.Materials[i].Layers.Count; x++)
                {
                    CMaterialLayer layer = CurrentModel.Materials[i].Layers[x];
                    ReportMissingOpeningKeyframes(layer.Alpha, $"At materials[{i}]: layers[{x}] alpha: ");
                    ReportMissingOpeningKeyframes(layer.TextureId, $"At materials[{i}]: layers[{x}] textureid: ");
                }
            }
            for (int i = 0; i < CurrentModel.Cameras.Count; i++)
            {
                var cam = CurrentModel.Cameras[i];
                ReportMissingOpeningKeyframes(cam.Rotation, $"At cameras[{i}]: rotation: ");
            }
            list.AddRange(TemporaryList);
            TemporaryList.Clear();
            return list;
        }
        private static bool TrackInSequence(int track)
        {
            foreach (CSequence sequence in CurrentModel.Sequences)
            {
                if (track >= sequence.IntervalStart && track <= sequence.IntervalEnd) { return true; }
            }
            return false;
        }
        private static CSequence GetSequenceofTrack(int track)
        {
            return CurrentModel.Sequences.First(sequence => track >= sequence.IntervalStart && track <= sequence.IntervalEnd);
        }
        private static void ReportMissingOpeningKeyframes(CAnimator<CVector3> aniamtor, string v)
        {
            for (int i = 0; i < aniamtor.Count; i++)
            {
                if (TrackInSequence(aniamtor[i].Time))
                {
                    CSequence sequenceOf = GetSequenceofTrack(aniamtor[i].Time);
                    if (aniamtor.Any(x => x.Time == sequenceOf.IntervalStart) == false)
                    {
                        TemporaryList.Add($"{v}: missing starting track for sequence {sequenceOf.Name}");
                    }
                }
            }
        }
        private static void ReportMissingOpeningKeyframes(CAnimator<CVector4> aniamtor, string v)
        {
            for (int i = 0; i < aniamtor.Count; i++)
            {
                if (TrackInSequence(aniamtor[i].Time))
                {
                    CSequence sequenceOf = GetSequenceofTrack(aniamtor[i].Time);
                    if (aniamtor.Any(x => x.Time == sequenceOf.IntervalStart) == false)
                    {
                        TemporaryList.Add($"{v}: missing starting track for sequence {sequenceOf.Name}");
                    }
                }
            }
        }
        private static void ReportMissingOpeningKeyframes(CAnimator<float> aniamtor, string v)
        {
            for (int i = 0; i < aniamtor.Count; i++)
            {
                if (TrackInSequence(aniamtor[i].Time))
                {
                    CSequence sequenceOf = GetSequenceofTrack(aniamtor[i].Time);
                    if (aniamtor.Any(x => x.Time == sequenceOf.IntervalStart) == false)
                    {
                        TemporaryList.Add($"{v}: missing starting track for sequence {sequenceOf.Name}");
                    }
                }
            }
        }
        private static void ReportMissingOpeningKeyframes(CAnimator<int> aniamtor, string v)
        {
            for (int i = 0; i < aniamtor.Count; i++)
            {
                if (TrackInSequence(aniamtor[i].Time))
                {
                    CSequence sequenceOf = GetSequenceofTrack(aniamtor[i].Time);
                    if (aniamtor.Any(x => x.Time == sequenceOf.IntervalStart) == false)
                    {
                        TemporaryList.Add($"{v}: missing starting track for sequence {sequenceOf.Name}");
                    }
                }
            }
        }
        private static List<string> TemporaryList = new List<string>();
        private static List<string> ChekRepeatingTimes()
        {
            List<string> list = new List<string>();
            foreach (INode node in CurrentModel.Nodes)
            {
                ReportRepeatingTimes(node.Translation, $"At {node.GetType().Name} '{node.Name}': translation: ");
                ReportRepeatingTimes(node.Rotation, $"At {node.GetType().Name} '{node.Name}': rotation: ");
                ReportRepeatingTimes(node.Scaling, $"At {node.GetType().Name} '{node.Name}': scaling: ");
                if (node is CAttachment)
                {
                    CAttachment element = (CAttachment)node;
                    ReportRepeatingTimes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                    ReportRepeatingTimes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                    ReportRepeatingTimes(element.EmissionRate, $"At {node.GetType().Name} '{node.Name}': emission rate: ");
                    ReportRepeatingTimes(element.LifeSpan, $"At {node.GetType().Name} '{node.Name}': lifespan: ");
                    ReportRepeatingTimes(element.InitialVelocity, $"At {node.GetType().Name} '{node.Name}': initial velocity ");
                    ReportRepeatingTimes(element.Gravity, $"At {node.GetType().Name} '{node.Name}': gravity: ");
                    ReportRepeatingTimes(element.Longitude, $"At {node.GetType().Name} '{node.Name}': longtitude: ");
                    ReportRepeatingTimes(element.Latitude, $"At {node.GetType().Name} '{node.Name}': latitude: ");
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    ReportRepeatingTimes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                    ReportRepeatingTimes(element.EmissionRate, $"At {node.GetType().Name} '{node.Name}': emission rate: ");
                    ReportRepeatingTimes(element.Speed, $"At {node.GetType().Name} '{node.Name}': speed: ");
                    ReportRepeatingTimes(element.Width, $"At {node.GetType().Name} '{node.Name}': width: ");
                    ReportRepeatingTimes(element.Gravity, $"At {node.GetType().Name} '{node.Name}': gravity: ");
                    ReportRepeatingTimes(element.Length, $"At {node.GetType().Name} '{node.Name}': length: ");
                    ReportRepeatingTimes(element.Latitude, $"At {node.GetType().Name} '{node.Name}': latitude: ");
                    ReportRepeatingTimes(element.Variation, $"At {node.GetType().Name} '{node.Name}': variation: ");
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    ReportRepeatingTimes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                    ReportRepeatingTimes(element.HeightAbove, $"At {node.GetType().Name} '{node.Name}': height above: ");
                    ReportRepeatingTimes(element.HeightBelow, $"At {node.GetType().Name} '{node.Name}': height below: ");
                    ReportRepeatingTimes(element.Color, $"At {node.GetType().Name} '{node.Name}': color: ");
                    ReportRepeatingTimes(element.Alpha, $"At {node.GetType().Name} '{node.Name}': alpha: ");
                    ReportRepeatingTimes(element.TextureSlot, $"At {node.GetType().Name} '{node.Name}': texture slot: ");
                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;
                    ReportRepeatingTimes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                    ReportRepeatingTimes(element.Color, $"At {node.GetType().Name} '{node.Name}': color: ");
                    ReportRepeatingTimes(element.AmbientColor, $"At {node.GetType().Name} '{node.Name}': ambient color: ");
                    ReportRepeatingTimes(element.Intensity, $"At {node.GetType().Name} '{node.Name}': intensity: ");
                    ReportRepeatingTimes(element.AmbientIntensity, $"At {node.GetType().Name} '{node.Name}': ambient intensity: ");
                    ReportRepeatingTimes(element.AttenuationEnd, $"At {node.GetType().Name} '{node.Name}': attentuation end: ");
                    ReportRepeatingTimes(element.AttenuationStart, $"At {node.GetType().Name} '{node.Name}': attentuation start: ");
                }
            }
            for (int i = 0; i < CurrentModel.GeosetAnimations.Count; i++)
            {
                CGeosetAnimation ga = CurrentModel.GeosetAnimations[i];
                if (ga.Alpha.Static == false)
                {
                    ReportRepeatingTimes(ga.Alpha, $"At geoset_animations[{i}]: alpha: ");
                }
                if (ga.Color.Static == false) { ReportRepeatingTimes(ga.Color, $"At geoset_animations[{i}]: color: "); }
            }
            for (int i = 0; i < CurrentModel.TextureAnimations.Count; i++)
            {
                CTextureAnimation taa = CurrentModel.TextureAnimations[i];
                ReportRepeatingTimes(taa.Translation, $"At texture_animation[{i}]: translation: ");
                ReportRepeatingTimes(taa.Rotation, $"At texture_animation[{i}]: rotation: ");
                ReportRepeatingTimes(taa.Scaling, $"At texture_animation[{i}]: scaling: ");
            }
            for (int i = 0; i < CurrentModel.Materials.Count; i++)
            {
                for (int x = 0; x < CurrentModel.Materials[i].Layers.Count; x++)
                {
                    CMaterialLayer layer = CurrentModel.Materials[i].Layers[x];
                    ReportRepeatingTimes(layer.Alpha, $"At materials[{i}]: layers[{x}] alpha: ");
                    ReportRepeatingTimes(layer.TextureId, $"At materials[{i}]: layers[{x}] textureid: ");
                }
            }
            for (int i = 0; i < CurrentModel.Cameras.Count; i++)
            {
                var cam = CurrentModel.Cameras[i];
                ReportRepeatingTimes(cam.Rotation, $"At cameras[{i}]: rotation: ");
            }
            list.AddRange(TemporaryList);
            TemporaryList.Clear();
            return list;
        }
        private static void ReportRepeatingTimes(CAnimator<CVector3> animator, string prefix)
        {
            if (animator.Static) { return; }
            for (int i = 0; i < animator.Count; i++)
            {
                if (i + 1 < animator.Count)
                {
                    if (animator[i].Time == animator[i + 1].Time)
                    {
                        TemporaryList.Add($"{prefix}: Track {i} has the same time as the track after it");
                    }
                }
            }
        }
        private static void ReportRepeatingTimes(CAnimator<CVector4> animator, string prefix)
        {
            if (animator.Static) { return; }
            for (int i = 0; i < animator.Count; i++)
            {
                if (i + 1 < animator.Count)
                {
                    if (animator[i].Time == animator[i + 1].Time)
                    {
                        TemporaryList.Add($"{prefix}: Track {i} has the same time as the track after it");
                    }
                }
            }
        }
        private static void ReportRepeatingTimes(CAnimator<int> animator, string prefix)
        {
            if (animator.Static) { return; }
            for (int i = 0; i < animator.Count; i++)
            {
                if (i + 1 < animator.Count)
                {
                    if (animator[i].Time == animator[i + 1].Time)
                    {
                        TemporaryList.Add($"{prefix}: Track {i} has the same time as the track after it");
                    }
                }
            }
        }
        private static void ReportRepeatingTimes(CAnimator<float> animator, string prefix)
        {
            if (animator.Static) { return; }
            for (int i = 0; i < animator.Count; i++)
            {
                if (i + 1 < animator.Count)
                {
                    if (animator[i].Time == animator[i + 1].Time)
                    {
                        TemporaryList.Add($"{prefix}: Track {i} has the same time as the track after it");
                    }
                }
            }
        }
        private static List<string> CheckInsonsistentKeyframes()
        {
            List<string> list = new List<string>();
            foreach (INode node in CurrentModel.Nodes)
            {
                ReportInconsistentKeyframes(node.Translation, $"At {node.GetType().Name} '{node.Name}': translation: ");
                ReportInconsistentKeyframes(node.Rotation, $"At {node.GetType().Name} '{node.Name}': rotation: ");
                ReportInconsistentKeyframes(node.Scaling, $"At {node.GetType().Name} '{node.Name}': scaling: ");
                if (node is CAttachment)
                {
                    CAttachment element = (CAttachment)node;
                    ReportMissingOpeningKeyframes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                    ReportInconsistentKeyframes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                    ReportInconsistentKeyframes(element.EmissionRate, $"At {node.GetType().Name} '{node.Name}': emission rate: ");
                    ReportInconsistentKeyframes(element.LifeSpan, $"At {node.GetType().Name} '{node.Name}': lifespan: ");
                    ReportInconsistentKeyframes(element.InitialVelocity, $"At {node.GetType().Name} '{node.Name}': initial velocity ");
                    ReportInconsistentKeyframes(element.Gravity, $"At {node.GetType().Name} '{node.Name}': gravity: ");
                    ReportInconsistentKeyframes(element.Longitude, $"At {node.GetType().Name} '{node.Name}': longtitude: ");
                    ReportInconsistentKeyframes(element.Latitude, $"At {node.GetType().Name} '{node.Name}': latitude: ");
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    ReportInconsistentKeyframes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                    ReportInconsistentKeyframes(element.EmissionRate, $"At {node.GetType().Name} '{node.Name}': emission rate: ");
                    ReportInconsistentKeyframes(element.Speed, $"At {node.GetType().Name} '{node.Name}': speed: ");
                    ReportInconsistentKeyframes(element.Width, $"At {node.GetType().Name} '{node.Name}': width: ");
                    ReportInconsistentKeyframes(element.Gravity, $"At {node.GetType().Name} '{node.Name}': gravity: ");
                    ReportInconsistentKeyframes(element.Length, $"At {node.GetType().Name} '{node.Name}': length: ");
                    ReportInconsistentKeyframes(element.Latitude, $"At {node.GetType().Name} '{node.Name}': latitude: ");
                    ReportInconsistentKeyframes(element.Variation, $"At {node.GetType().Name} '{node.Name}': variation: ");
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    ReportInconsistentKeyframes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                    ReportInconsistentKeyframes(element.HeightAbove, $"At {node.GetType().Name} '{node.Name}': height above: ");
                    ReportInconsistentKeyframes(element.HeightBelow, $"At {node.GetType().Name} '{node.Name}': height below: ");
                    ReportInconsistentKeyframes(element.Color, $"At {node.GetType().Name} '{node.Name}': color: ");
                    ReportInconsistentKeyframes(element.Alpha, $"At {node.GetType().Name} '{node.Name}': alpha: ");
                    ReportInconsistentKeyframes(element.TextureSlot, $"At {node.GetType().Name} '{node.Name}': texture slot: ");
                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;
                    ReportInconsistentKeyframes(element.Visibility, $"At {node.GetType().Name} '{node.Name}': visibility: ");
                    ReportInconsistentKeyframes(element.Color, $"At {node.GetType().Name} '{node.Name}': color: ");
                    ReportInconsistentKeyframes(element.AmbientColor, $"At {node.GetType().Name} '{node.Name}': ambient color: ");
                    ReportInconsistentKeyframes(element.Intensity, $"At {node.GetType().Name} '{node.Name}': intensity: ");
                    ReportInconsistentKeyframes(element.AmbientIntensity, $"At {node.GetType().Name} '{node.Name}': ambient intensity: ");
                    ReportInconsistentKeyframes(element.AttenuationEnd, $"At {node.GetType().Name} '{node.Name}': attentuation end: ");
                    ReportInconsistentKeyframes(element.AttenuationStart, $"At {node.GetType().Name} '{node.Name}': attentuation start: ");
                }
            }
            for (int i = 0; i < CurrentModel.GeosetAnimations.Count; i++)
            {
                CGeosetAnimation ga = CurrentModel.GeosetAnimations[i];
                if (ga.Alpha.Static == false)
                {
                    ReportInconsistentKeyframes(ga.Alpha, $"At geoset_animations[{i}]: alpha: ");
                }
                if (ga.Color.Static == false) { ReportMissingOpeningKeyframes(ga.Color, $"At geoset_animations[{i}]: color: "); }
            }
            for (int i = 0; i < CurrentModel.TextureAnimations.Count; i++)
            {
                CTextureAnimation taa = CurrentModel.TextureAnimations[i];
                ReportInconsistentKeyframes(taa.Translation, $"At texture_animation[{i}]: translation: ");
                ReportInconsistentKeyframes(taa.Rotation, $"At texture_animation[{i}]: rotation: ");
                ReportInconsistentKeyframes(taa.Scaling, $"At texture_animation[{i}]: scaling: ");
            }
            for (int i = 0; i < CurrentModel.Materials.Count; i++)
            {
                for (int x = 0; x < CurrentModel.Materials[i].Layers.Count; x++)
                {
                    CMaterialLayer layer = CurrentModel.Materials[i].Layers[x];
                    ReportInconsistentKeyframes(layer.Alpha, $"At materials[{i}]: layers[{x}] alpha: ");
                    ReportInconsistentKeyframes(layer.TextureId, $"At materials[{i}]: layers[{x}] textureid: ");
                }
            }
            for (int i = 0; i < CurrentModel.Cameras.Count; i++)
            {
                var cam = CurrentModel.Cameras[i];
                ReportInconsistentKeyframes(cam.Rotation, $"At cameras[{i}]: rotation: ");
            }
            list.AddRange(TemporaryList);
            TemporaryList.Clear();
            return list;
        }
        private static void ReportInconsistentKeyframes(CAnimator<float> heightAbove, string v)
        {
            if (heightAbove.Count > 1)
            {
                for (int i = 0; i < heightAbove.Count - 1; i++)
                {
                    if (heightAbove[i].Time > heightAbove[i + 1].Time)
                    {
                        TemporaryList.Add($"{v}: Inconsistent order of keyframes");
                        return;
                    }
                }
            }
        }
        private static void ReportInconsistentKeyframes(CAnimator<CVector4> heightAbove, string v)
        {
            if (heightAbove.Count > 1)
            {
                for (int i = 0; i < heightAbove.Count - 1; i++)
                {
                    if (heightAbove[i].Time > heightAbove[i + 1].Time)
                    {
                        TemporaryList.Add($"{v}: Inconsistent order of keyframes");
                        return;
                    }
                }
            }
        }
        private static void ReportInconsistentKeyframes(CAnimator<CVector3> heightAbove, string v)
        {
            if (heightAbove.Count > 1)
            {
                for (int i = 0; i < heightAbove.Count - 1; i++)
                {
                    if (heightAbove[i].Time > heightAbove[i + 1].Time)
                    {
                        TemporaryList.Add($"{v}: Inconsistent order of keyframes");
                        return;
                    }
                }
            }
        }
        private static void ReportInconsistentKeyframes(CAnimator<int> heightAbove, string v)
        {
            if (heightAbove.Count > 1)
            {
                for (int i = 0; i < heightAbove.Count - 1; i++)
                {
                    if (heightAbove[i].Time > heightAbove[i + 1].Time)
                    {
                        TemporaryList.Add($"{v}: Inconsistent order of keyframes");
                        return;
                    }
                }
            }
        }
        private static bool NothingAttachedToBone(INode target)
        {
            foreach (CGeoset geo in CurrentModel.Geosets)
            {
                if (geo.Groups.Count == 0) continue;
                foreach (CGeosetGroup group in geo.Groups)
                {
                    foreach (var node in group.Nodes)
                    {
                        if (node.Node.Node == target) { return false; }
                    }
                }
            }
            return true;
        }
        private static bool ExtentsNegative(CExtent extent)
        {
            if (extent.Min.X > extent.Max.X) return true;
            if (extent.Min.Y > extent.Max.Y) return true;
            if (extent.Min.Z > extent.Max.Z) return true;
            return false;
        }
        private static List<CSequence> SequencesInWhichTracksAreTheSame(CModel owner, CAnimator<float> animator)
        {
            // If all tracks in a sequence have the same value, add the sequence to the list.
            var list = new List<CSequence>();

            if (animator.Static || animator.Count <= 1)
                return list;

            foreach (var sequence in owner.Sequences)
            {
                var ofSequences = animator.NodeList
                    .Where(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd)
                    .ToList();

                if (ofSequences.Count <= 1)
                    continue;

                bool same = ofSequences.All(x => x.Value == ofSequences[0].Value);

                if (same)
                    list.Add(sequence);
            }

            return list;
        }

        private static List<CSequence> SequencesInWhichTracksAreTheSame(CModel owner, CAnimator<int> animator)
        {
            // If all tracks in a sequence have the same value, add the sequence to the list.
            var list = new List<CSequence>();

            if (animator.Static || animator.Count <= 1)
                return list;

            foreach (var sequence in owner.Sequences)
            {
                var ofSequences = animator.NodeList
                    .Where(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd)
                    .ToList();

                if (ofSequences.Count <= 1)
                    continue;

                bool same = ofSequences.All(x => x.Value == ofSequences[0].Value);

                if (same)
                    list.Add(sequence);
            }

            return list;
        }

        private static List<CSequence> SequencesInWhichTracksAreTheSame(CModel owner, CAnimator<CVector3> animator)
        {
            // If all tracks in a sequence have the same value, add the sequence to the list.
            var list = new List<CSequence>();

            if (animator.Static || animator.Count <= 1)
                return list;

            foreach (var sequence in owner.Sequences)
            {
                var ofSequences = animator.NodeList
                    .Where(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd)
                    .ToList();

                if (ofSequences.Count <= 1)
                    continue;

                bool same = ofSequences.All(x => x.Value == ofSequences[0].Value);

                if (same)
                    list.Add(sequence);
            }

            return list;
        }
        private static List<CSequence> SequencesInWhichTracksAreTheSame(CModel owner, CAnimator<CVector4> animator)
        {
            // If all tracks in a sequence have the same value, add the sequence to the list.
            var list = new List<CSequence>();

            if (animator.Static || animator.Count <= 1)
                return list;

            foreach (var sequence in owner.Sequences)
            {
                var ofSequences = animator.NodeList
                    .Where(x => x.Time >= sequence.IntervalStart && x.Time <= sequence.IntervalEnd)
                    .ToList();

                if (ofSequences.Count <= 1)
                    continue;

                bool same = ofSequences.All(x => x.Value == ofSequences[0].Value);

                if (same)
                    list.Add(sequence);
            }

            return list;
        }



        private static bool TAIsUsed(CTextureAnimation cTextureAnimation)
        {
            foreach (CMaterial mat in CurrentModel.Materials)
            {
                foreach (CMaterialLayer layer in mat.Layers)
                {
                    if (layer.TextureAnimation.Object == cTextureAnimation) { return true; }
                }
            }
            return false;
        }
    }
}