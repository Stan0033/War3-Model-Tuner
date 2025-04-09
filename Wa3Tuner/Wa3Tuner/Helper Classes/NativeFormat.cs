using MdxLib.Animator;
using MdxLib.Model;
using MdxLib.Primitives;
using Microsoft.Win32;
using SharpGL.SceneGraph.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.TextFormatting;
using System.Xml.Linq;

namespace Wa3Tuner.Helper_Classes
{
    public static class NativeFormat
    {
        public static void Save(CModel model)
        {
            string s = CollectDataForSaving(model);
            SaveDataToFile(s);
        }
        private static string CollectDataForSaving(CModel model)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[Model]");
            sb.AppendLine($"Name={model.Name}");
            sb.AppendLine($"Extent={getExtent(model.Extent)}");
            sb.AppendLine("[End]");
            //get cameras
            foreach (var cam in model.Cameras)
            {
                sb.AppendLine("[Camera]");
                sb.AppendLine($"NearDistance={cam.NearDistance}");
                sb.AppendLine($"Name={cam.Name}");
                sb.AppendLine($"FarDistance={cam.FarDistance}");
                sb.AppendLine($"FieldOfView={cam.FieldOfView}");
                sb.AppendLine($"Position={cam.Position.X} {cam.Position.Y} {cam.Position.Z}");
                sb.AppendLine($"Target={cam.TargetPosition.X} {cam.TargetPosition.Y} {cam.TargetPosition.Z}");
                if (cam.Translation.Animated && cam.Translation.Count > 0)
                {
                    sb.AppendLine("[Translation]");
                    sb.AppendLine($"Interpolation={cam.Translation.Type}");
                    sb.AppendLine($"GlobalSequence={model.GlobalSequences.IndexOf(cam.Translation.GlobalSequence.Object)}");

                    foreach (var kf in cam.Translation)
                    {
                        sb.AppendLine("Keyframe=" + GetKeyframe(kf));
                    }

                    sb.AppendLine("[End]");
                }
                if (cam.TargetTranslation.Animated && cam.TargetTranslation.Count > 0)
                {
                    sb.AppendLine("[TargetTranslation]");
                    sb.AppendLine($"Interpolation={cam.TargetTranslation.Type}");
                    sb.AppendLine($"GlobalSequence={model.GlobalSequences.IndexOf(cam.TargetTranslation.GlobalSequence.Object)}");

                    foreach (var kf in cam.Translation)
                    {
                        sb.AppendLine("Keyframe=" + GetKeyframe(kf));
                    }

                    sb.AppendLine("[End]");
                }
                if (cam.Rotation.Animated && cam.Rotation.Count > 0)
                {
                    sb.AppendLine("[Rotation]");
                    sb.AppendLine($"Interpolation={cam.Translation.Type}");
                    sb.AppendLine($"GlobalSequence={model.GlobalSequences.IndexOf(cam.Rotation.GlobalSequence.Object)}");

                    foreach (var kf in cam.Translation)
                    {
                        sb.AppendLine("Keyframe=" + GetKeyframe(kf));
                    }

                    sb.AppendLine("[End]");
                }
                sb.AppendLine("[End]");
            }
            // get textures
            foreach (var texture in model.Textures)
            {
                sb.AppendLine("[Texture]");
                sb.AppendLine($"ReplaceableID={texture.ReplaceableId}");
                sb.AppendLine($"Path={texture.FileName}");
                sb.AppendLine($"{nameof(texture.WrapWidth)}={texture.WrapWidth}");
                sb.AppendLine($"{nameof(texture.WrapHeight)}={texture.WrapHeight}");

                sb.AppendLine("[End]");
            }
           //get sequences
            foreach (var sequence in model.Sequences)
            {
                sb.AppendLine("[Sequence]");
                sb.AppendLine($"Name={sequence.Name}");
                sb.AppendLine($"Start={sequence.IntervalStart}");
                sb.AppendLine($"End={sequence.IntervalEnd}");
                sb.AppendLine($"Extent={getExtent(sequence.Extent)}");
                sb.AppendLine($"Rarity={sequence.Rarity}");
                sb.AppendLine($"MoveSpeed={sequence.MoveSpeed}");
                sb.AppendLine($"NonLooping={sequence.NonLooping}");
                

                sb.AppendLine("[End]");
            }
           //get global sequences
            foreach (var g in model.GlobalSequences)
            {
                sb.AppendLine($"[GlobalSequence={g.Duration}]");
            }
           //get texture animations
            foreach (var ta in model.TextureAnimations)
            {
                sb.AppendLine("[TextureAnimation]");
                if (ta.Translation.Animated && ta.Translation.Count > 0)
                {
                    sb.AppendLine("[Translation]");
                    sb.AppendLine($"Interpolation={ta.Translation.Type}");
                    sb.AppendLine($"GlobalSequence={model.GlobalSequences.IndexOf(ta.Translation.GlobalSequence.Object)}");
                    foreach (var kf in ta.Translation)
                    {
                        sb.AppendLine("Keyframe=" + GetKeyframe(kf));
                    }

                    sb.AppendLine("[End]");
                }
                if (ta.Rotation.Animated && ta.Rotation.Count > 0)
                {
                    sb.AppendLine("[Rotation]");
                    sb.AppendLine($"Interpolation={ta.Rotation.Type}");
                    sb.AppendLine($"GlobalSequence={model.GlobalSequences.IndexOf(ta.Rotation.GlobalSequence.Object)}");

                    foreach (var kf in ta.Translation)
                    {
                        sb.AppendLine("Keyframe=" + GetKeyframe(kf));
                    }

                    sb.AppendLine("[End]");
                }
                if (ta.Scaling.Animated && ta.Scaling.Count > 0)
                {
                    sb.AppendLine("[Scaling]");
                    sb.AppendLine($"Interpolation={ta.Scaling.Type}");
                    sb.AppendLine($"GlobalSequence={model.GlobalSequences.IndexOf(ta.Scaling.GlobalSequence.Object)}");

                    foreach (var kf in ta.Translation)
                    {
                        sb.AppendLine("Keyframe=" + GetKeyframe(kf));
                    }

                    sb.AppendLine("[End]");
                }
                sb.AppendLine("[End]");
            }
         //get materials - they use textures and texture animations
            foreach (var material in model.Materials)
            {
                sb.AppendLine("[Material]");
                sb.AppendLine($"PriorityPlane={material.PriorityPlane}");
                sb.AppendLine($"SortPrimitivesNearZ={material.SortPrimitivesNearZ}");
                sb.AppendLine($"SortPrimitivesFarZ={material.SortPrimitivesFarZ}");
                sb.AppendLine($"FullResolution={material.FullResolution}");
                foreach (var layer in material.Layers)
                {
                    sb.AppendLine("[Layer]");
                    sb.AppendLine($"FilterMode={layer.FilterMode}");
                    sb.AppendLine($"Unshaded={layer.Unshaded}");
                    sb.AppendLine($"Unfogged={layer.Unfogged}");
                    sb.AppendLine($"TwoSided={layer.TwoSided}");
                    sb.AppendLine($"SphereEnvironmentMap={layer.SphereEnvironmentMap}");
                    sb.AppendLine($"NoDepthSet={layer.NoDepthSet}");
                    sb.AppendLine($"NoDepthTest={layer.NoDepthTest}");
                    if (layer.TextureAnimation.Object != null)
                    {
                        sb.AppendLine($"TextureAnimation={model.TextureAnimations.IndexOf( layer.TextureAnimation.Object)}");
                    }
                    if (model.Textures.Contains(layer.Texture.Object))
                    {
                        sb.AppendLine($"Texture={model.Textures.IndexOf(layer.Texture.Object)}");
                    }
                    if (layer.TextureId.Animated && layer.TextureId.Count > 0)
                    {
                        sb.AppendLine("[TextureID]");
                        sb.AppendLine($"Interpolation={layer.TextureId.Type}");
                        sb.AppendLine($"GlobalSequence={model.GlobalSequences.IndexOf(layer.TextureId.GlobalSequence.Object)}");

                        foreach (var kf in layer.TextureId)
                        {
                            sb.AppendLine("Keyframe="+GetKeyframe(kf));
                        }

                        sb.AppendLine("[End]");
                    }
                    if (layer.Alpha.Static)
                    {
                        sb.AppendLine($"Alpha={layer.Alpha.GetValue()}");
                    }
                    else if (layer.Alpha.Animated && layer.Alpha.Count>0)
                    {
                        sb.AppendLine("[Alpha]");
                        sb.AppendLine($"Interpolation={layer.TextureId.Type}");
                        sb.AppendLine($"GlobalSequence={model.GlobalSequences.IndexOf(layer.Alpha.GlobalSequence.Object)}");
                        foreach (var kf in layer.Alpha)
                        {
                            sb.AppendLine("Keyframe=" + GetKeyframe(kf));
                        }

                        sb.AppendLine("[End]");
                    }

                        sb.AppendLine("[End]");
                }
                sb.AppendLine("[End]");

            
            }
           // get nodes - they are used by geoset's vertices, and they (may) use bones
            foreach (var node in model.Nodes)
            {
                sb.AppendLine($"[{node.GetType().Name}]");
                sb.AppendLine($"Name={node.Name}");
                sb.AppendLine($"Pivot={node.PivotPoint.X} {node.PivotPoint.Y} {node.PivotPoint.Z}");
                sb.AppendLine($"{nameof(node.Billboarded)}={node.Billboarded}");
                sb.AppendLine($"{nameof(node.BillboardedLockX)}={node.BillboardedLockX}");
                sb.AppendLine($"{nameof(node.BillboardedLockY)}={node.BillboardedLockY}");
                sb.AppendLine($"{nameof(node.BillboardedLockZ)}={node.BillboardedLockZ}");
                sb.AppendLine($"{nameof(node.CameraAnchored)}={node.CameraAnchored}");
                sb.AppendLine($"{nameof(node.DontInheritRotation)}={node.DontInheritRotation}");
                sb.AppendLine($"{nameof(node.DontInheritScaling)}={node.DontInheritScaling}");
                sb.AppendLine($"{nameof(node.DontInheritTranslation)}={node.DontInheritTranslation}");
                sb.AppendLine($"Parent={model.Nodes.IndexOf( node.Parent.Node)}"); // need second run to see this

                if (node.Translation.Count > 0)
                {
                    sb.AppendLine("[Translation]");
                    sb.AppendLine($"Interpolation={node.Translation.Type}");
                    sb.AppendLine($"GlobalSequence={model.GlobalSequences.IndexOf(node.Translation.GlobalSequence.Object)}");
                    foreach (var kf in node.Translation)
                    {

                        sb.AppendLine($"Keyframe={GetKeyframe(kf)}");
                    }
                    sb.AppendLine("[End]");
                }
                if (node.Rotation.Count > 0)
                {
                    sb.AppendLine("[Rotation]");
                    sb.AppendLine($"Interpolation={node.Rotation.Type}");
                    sb.AppendLine($"GlobalSequence={model.GlobalSequences.IndexOf(node.Rotation.GlobalSequence.Object)}");
                    foreach (var kf in node.Rotation)
                    {

                        sb.AppendLine($"Keyframe={GetKeyframe(kf)}");
                    }
                    sb.AppendLine("[End]");
                }
                if (node.Scaling.Count > 0)
                {
                    sb.AppendLine("[Scaling]");
                    sb.AppendLine($"Interpolation={node.Scaling.Type}");
                    sb.AppendLine($"GlobalSequence={model.GlobalSequences.IndexOf(node.Scaling.GlobalSequence.Object)}");
                    foreach (var kf in node.Scaling)
                    {

                        sb.AppendLine($"Keyframe={GetKeyframe(kf)}");
                    }
                    sb.AppendLine("[End]");
                }
                if (node is CBone bone)
                {
                    sb.AppendLine($"Interpolation={model.Geosets.IndexOf( bone.Geoset.Object)}"); // might need to run second parsing to fetch that
                    sb.AppendLine($"GeosetAnimation={model.GeosetAnimations.IndexOf( bone.GeosetAnimation.Object)}"); // might need to run second parsing to fetch that
                }
                if (node is CCollisionShape collisionShape)
                {
                    sb.AppendLine($"Type={collisionShape.Type}");
                    if (collisionShape.Type == ECollisionShapeType.Sphere)
                    {
                        sb.AppendLine($"Radius={collisionShape.Radius}");
                    }
                    else {
                        sb.AppendLine($"Vertex1={collisionShape.Vertex1.X} {collisionShape.Vertex1.Y} {collisionShape.Vertex1.Z}");
                        sb.AppendLine($"Vertex2={collisionShape.Vertex2.X} {collisionShape.Vertex2.Y} {collisionShape.Vertex2.Z}");
                    }
                }
                if (node is CEvent e)
                {
                    sb.AppendLine("[Tracks]");
                    foreach (var track in e.Tracks)
                    {
                        sb.AppendLine(track.Time.ToString()); //int
                    }
                    sb.AppendLine("[End]");
                }
                if (node is CParticleEmitter emitter)
                {
                    sb.AppendLine($"EmitterUsesMdl={emitter.EmitterUsesMdl}");
                    sb.AppendLine($"EmitterUsesTga={emitter.EmitterUsesTga}");
                    sb.AppendLine($"FileName={emitter.FileName}");
                    if (emitter.EmissionRate.Static) { sb.AppendLine($"EmissionRate={emitter.EmissionRate.GetValue()}"); }
                    if (emitter.EmissionRate.Animated && emitter.EmissionRate.Count > 0)
                    {

                    }
                    if (emitter.LifeSpan.Static) { sb.AppendLine($"LifeSpan={emitter.LifeSpan.GetValue()}"); }
                    if (emitter.LifeSpan.Animated && emitter.LifeSpan.Count > 0)
                    {

                    }
                    if (emitter.InitialVelocity.Static) { sb.AppendLine($"InitialVelocity={emitter.InitialVelocity.GetValue()}"); }
                    if (emitter.InitialVelocity.Animated && emitter.InitialVelocity.Count > 0)
                    {

                    }
                    if (emitter.Gravity.Static) { sb.AppendLine($"Gravity={emitter.Gravity.GetValue()}"); }
                    if (emitter.Gravity.Animated && emitter.Gravity.Count > 0)
                    {

                    }
                    if (emitter.Longitude.Static) { sb.AppendLine($"Longitude={emitter.Longitude.GetValue()}"); }
                    if (emitter.Longitude.Animated && emitter.Longitude.Count > 0)
                    {

                    }
                    if (emitter.Latitude.Static) { sb.AppendLine($"Latitude={emitter.Latitude.GetValue()}"); }
                    if (emitter.Latitude.Animated && emitter.Latitude.Count > 0)
                    {

                    }
                    if (emitter.Visibility.Static) { sb.AppendLine($"Visibility={emitter.Visibility.GetValue()}"); }
                    if (emitter.Visibility.Animated && emitter.Visibility.Count > 0)
                    {

                    }
                }
                if (node is CParticleEmitter2 emitter2)
                {

                }
                if (node is CLight light)
                {

                }
                if (node is CRibbonEmitter ribbon)
                {

                }
                sb.AppendLine("[End]");
            }
           //get geosets - vertices use nodes
            foreach (var geoset in model.Geosets)
            {
                sb.AppendLine("[Geoset]");
                sb.AppendLine($"Name={geoset.Name}");
                sb.AppendLine($"Material={model.Materials.IndexOf(geoset.Material.Object)}");
                sb.AppendLine($"Unselectable={geoset.Unselectable}");
                sb.AppendLine($"SelectionGroup={geoset.SelectionGroup}");
                sb.AppendLine($"Extent={getExtent(geoset.Extent)}");
                sb.AppendLine("[Vertices]");
                foreach (var vertex in geoset.Vertices)
                {
                    sb.AppendLine(GetVertex(vertex, model));
                }
                sb.AppendLine("[End]");
                sb.AppendLine("[Triangles]");
                foreach (var triangle in geoset.Triangles)
                {
                    sb.AppendLine(GetTriangle(triangle, geoset));
                }
                sb.AppendLine("[End]");
                if (geoset.Extents.Count > 0)
                {
                    sb.AppendLine("[Extents]");
                    foreach (var extent in geoset.Extents)
                    {
                        sb.AppendLine($"Extent={getExtent(extent.Extent)}");
                    }
                    sb.AppendLine("[End]");
                }
                sb.AppendLine("[End]");
            }
           // get geoset - they use geoset animations
            foreach (var ga in model.GeosetAnimations)
            {
                sb.AppendLine("[GeosetAnimation]");
                sb.AppendLine($"UseColor={ga.UseColor}");
                sb.AppendLine($"DropShadow={ga.DropShadow}");
                if (ga.Alpha.Static)
                {
                    sb.AppendLine($"Alpha={ga.Alpha.GetValue()}");
                }
                if (ga.Alpha.Animated && ga.Alpha.Count > 0)
                {
                    sb.AppendLine("[Alpha]");
                    sb.AppendLine($"Interpolation={ga.Alpha.Type}");
                    sb.AppendLine($"GlobalSequence={model.GlobalSequences.IndexOf(ga.Alpha.GlobalSequence.Object)}");
                    foreach (var kf in ga.Color)
                    {

                        sb.AppendLine($"Keyframe={GetKeyframe(kf)}");
                    }
                    sb.AppendLine("[End]");
                }
                if (ga.UseColor)
                {
                    if (ga.Color.Static)
                    {
                        var val = ga.Color.GetValue();
                        sb.AppendLine($"Color={val.X} {val.Y} {val.Z}");
                    }
                    if (ga.Color.Animated && ga.Color.Count > 0)
                    {
                        sb.AppendLine("[Color]");
                        sb.AppendLine($"Interpolation={ga.Color.Type}");
                        sb.AppendLine($"GlobalSequence={model.GlobalSequences.IndexOf(ga.Color.GlobalSequence.Object)}");

                        foreach (var kf in ga.Color)
                        {

                            sb.AppendLine($"Keyframe={GetKeyframe(kf)}");
                        }
                        sb.AppendLine("[End]");
                    }
                    sb.AppendLine("[End]");
                }
            }
            return sb.ToString();
        } 

        private static string? GetTriangle(CGeosetTriangle triangle, CGeoset geoset)
        {
            return $"{geoset.Vertices.IndexOf(triangle.Vertex1.Object)} {geoset.Vertices.IndexOf(triangle.Vertex2.Object)} {geoset.Vertices.IndexOf(triangle.Vertex3.Object)}";
        }

        private static string? GetVertex(CGeosetVertex vertex, CModel model)
        {
            string data = $"{vertex.Position.X} {vertex.Position.Y} {vertex.Position.Z} {vertex.Normal.X} {vertex.Normal.Y} {vertex.Normal.Z} {vertex.TexturePosition.X} {vertex.TexturePosition.Y}";
           List<int> ints = new List<int>();
            if (vertex.Group != null)
            {
                if (vertex.Group.Object != null)
                {
                    foreach (var node in vertex.Group.Object.Nodes)
                    {
                        ints.Add(model.Nodes.IndexOf(node.Node.Node));
                    }
                }
            }
            data += " " + string.Join(" ", ints);
            return data;
        }

        private static string? GetKeyframe(CAnimatorNode<int> kf)
        {
            return $"{kf.Value} {kf.InTangent} {kf.OutTangent}";
        }
        private static string? GetKeyframe(CAnimatorNode<float> kf)
        {
            return $"{kf.Value} {kf.InTangent} {kf.OutTangent}";
        }
        private static string? GetKeyframe(CAnimatorNode<CVector3> kf)
        {
            return $"{kf.Value.X} {kf.Value.Y} {kf.Value.Z} {kf.InTangent.X} {kf.InTangent.Y} {kf.InTangent.Z} {kf.OutTangent.X} {kf.OutTangent.Y} {kf.OutTangent.Z}";
        }
        private static string? GetKeyframe(CAnimatorNode<CVector4> kf)
        {
            return $"{kf.Value.X} {kf.Value.Y} {kf.Value.Z} {kf.Value.W} {kf.InTangent.X} {kf.InTangent.Y} {kf.InTangent.Z} {kf.InTangent.W} {kf.OutTangent.X} {kf.OutTangent.Y} {kf.OutTangent.Z} {kf.OutTangent.W}";
        }

        private static string getExtent(CExtent e)
        {
            return $"{e.Min.X} {e.Min.Y} {e.Min.Z} {e.Max.X} {e.Max.Y} {e.Max.Z} {e.Radius}";
        }
        private static void SaveDataToFile(string data)
        {
            // Create and configure the SaveFileDialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "War3 Model Tuner Files (*.w3mt)|*.w3mt"; // Restrict to .w3mt files
            saveFileDialog.DefaultExt = ".w3mt"; // Default extension if user doesn't provide one
            saveFileDialog.AddExtension = true; // Automatically add the extension

            // Show the dialog and check if the user confirmed the save
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Write the data to the selected file
                    File.WriteAllText(saveFileDialog.FileName, data);
                    MessageBox.Show("File saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    // Handle any errors (e.g., permission issues)
                    MessageBox.Show($"An error occurred while saving the file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
