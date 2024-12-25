using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;

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
            return false;
        }
        private static bool MaterialUsed(CMaterial material)
        {
            foreach (CGeoset geo in CurrentModel.Geosets)
            {
                if (geo.Material.Object == material) { return true; }
            }
            return false;
        }
        internal static string Inspect(CModel currentModel)
        {
             
            StringBuilder all = new StringBuilder();    
            StringBuilder unused = new StringBuilder();
            StringBuilder warnings = new StringBuilder();
            StringBuilder severe = new StringBuilder();
            StringBuilder errors = new StringBuilder();

            //check unused materials
             for (int i = 0; i < currentModel.Textures.Count; i++)
            {
                if (TextureUsed(currentModel.Textures[i]) == false)
                {
                    string name = currentModel.Textures[i].ReplaceableId == 0 ? currentModel.Textures[i].FileName
                        : "Replaceable ID " + currentModel.Textures[i].ReplaceableId.ToString();
                    unused.AppendLine($"Textures[{i}] ({name}) is unused");
                }
            }
             for    (int i = 0;i < currentModel.Materials.Count; i++)
            {
                if (MaterialUsed(currentModel.Materials[i]) == false)
                {
                    unused.AppendLine($"Materials[{i}] is unused");
                }
               
            }
             for (int i = 0; i< currentModel.TextureAnimations.Count; i++) {
             if (TAIsUsed(currentModel.TextureAnimations[i]) == false)
                {
                    unused.AppendLine($"TextureAnims[{i}] is unused");
                }
            }
            // unused.AppendLine($"");
            if (currentModel.Textures.Count ==0)  warnings.AppendLine("No textures");
            if (currentModel.Materials.Count ==0)  warnings.AppendLine("No Materials");
            if (currentModel.Sequences.Count ==0)  warnings.AppendLine("No sequences");
            if (currentModel.Nodes.Any(x=>x.Name.ToLower() == "origin ref") == false)  warnings.AppendLine("Missing the origin attachment point");
          for (int i = 0;i < currentModel.Geosets.Count; i++)
            {
                CGeoset geo = currentModel.Geosets[i];
                if (geo.Faces.Count == 0) { warnings.AppendLine($"Geosets[{i}]: no faces"); }
                if (geo.Vertices.Count == 0) { warnings.AppendLine($"Geosets[{i}]: no vertices"); }
                if (geo.Extents.Count != currentModel.Sequences.Count) { warnings.AppendLine($"Geosets[{i}]: number of extents not equal to number of sequences"); }
                if (ExtentsNegative(geo.Extent)) { warnings.AppendLine($"Geosets[{i}]: negative extents"); }
            }
            foreach (CSequence cSequence in currentModel.Sequences)
            {
                if (ExtentsNegative(cSequence.Extent)) { warnings.AppendLine($"Sequence '{cSequence.Name}': negative extents"); }
                if (cSequence.IntervalEnd == cSequence.IntervalStart)
                {
                    warnings.AppendLine($"Sequence '{cSequence.Name}': Zero length");
                }
            }
            // check tracks - repeating times, repeating frames, inconsistent frames, missing opening, closing
           
            
            // check interpolation for visibilities
            foreach (INode node in currentModel.Nodes)
            {
                if (node is CLight light)
                {

                }
                if (node is CAttachment attachment)
                {
                    if (attachment.Visibility.Type != MdxLib.Animator.EInterpolationType.None)
                    {
                        warnings.AppendLine($"Attachment '{node.Name}': Interpolation not set to none");
                    }
                }
                if (node is CParticleEmitter emitter)
                {
                    if (emitter.Visibility.Type != MdxLib.Animator.EInterpolationType.None)
                    {
                        warnings.AppendLine($"Emitter1 '{node.Name}': Interpolation not set to none");
                    }
                }
                if (node is CParticleEmitter2 emitter2)
                {
                    if (emitter2.Visibility.Type != MdxLib.Animator.EInterpolationType.None)
                    {
                        warnings.AppendLine($"Emitter2 '{node.Name}': Interpolation not set to none");
                    }
                }
                if (node is CRibbonEmitter ribbon)
                {
                    if (ribbon.Visibility.Type != MdxLib.Animator.EInterpolationType.None)
                    {
                        warnings.AppendLine($"Ribbon '{node.Name}': Interpolation not set to none");
                    }

                }
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
                    for (int k = 0; k <group.Nodes.Count; k++)
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
            
            // invisible geoset anim
            for (int i = 0; i < currentModel.GeosetAnimations.Count; i++)
            {
                CGeosetAnimation ga = currentModel.GeosetAnimations[i];
                if (ga.Alpha.Static && ga.Alpha.GetValue() < 0.2)
                {
                    severe.AppendLine($"GeosetAnims[{i}]: 0 or near 0 alpha, it may be invisible");
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
             
             for (int g =0; g < currentModel.Geosets.Count; g++)
            {
                CGeoset geo = currentModel.Geosets[g];
                for (int i = 0; i < geo.Vertices.Count; i++)
                {
                    if (geo.Vertices[i].Group == null || geo.Vertices[i].Group.Object == null   )
                    {
                        errors.AppendLine($"Geoset {g}: vertex {i} is not attached to anything"); continue;
                    }
                    if (geo.Vertices[i].Group.Object.Nodes.Count == 0)
                    {
                        errors.AppendLine($"Geoset {g}: vertex {i} is not attached to anything"); continue;
                    }
                }
                if (geo.Material.Object == null )
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
            foreach (CMaterial material in currentModel.Materials)
            {

            }

            if (unused.Length == 0 && warnings.Length ==0 && 
                severe.Length == 0 && errors.Length == 0)  { all.AppendLine("All ok"); }
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