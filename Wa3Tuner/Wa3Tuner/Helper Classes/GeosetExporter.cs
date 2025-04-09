using MdxLib.Animator.Animatable;
using MdxLib.Model;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Wa3Tuner.Helper_Classes;

namespace Wa3Tuner
{
    internal class GeosetExporter
    {
        enum ModelReadMode
        {
            Vertices, Triangles, Nothing,
            Material, Layer, Alpha, Translation, Rotation, Scaling,
            TextureAnim
        }
        internal static CGeoset Read(string openPath, CModel model)
        {
            CGeoset geoset = new CGeoset(model);
            bool Mode_Vertices = false;
            ModelReadMode readMode = ModelReadMode.Nothing;
           
            using (StreamReader reader = new StreamReader(openPath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == "[vertices]") { readMode = ModelReadMode.Vertices; continue; }
                    if (line == "[triangles]") { readMode = ModelReadMode.Triangles; continue; }
                    if (readMode == ModelReadMode.Nothing) { continue; }
                    if (readMode == ModelReadMode.Vertices)
                    {
                        CGeosetVertex vertex = new CGeosetVertex(model);
                        string[] data = line.Split(' ');
                        vertex.Position = new MdxLib.Primitives.CVector3(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]));
                        vertex.Normal = new MdxLib.Primitives.CVector3(float.Parse(data[3]), float.Parse(data[4]), float.Parse(data[5]));
                        vertex.TexturePosition = new MdxLib.Primitives.CVector2(float.Parse(data[6]), float.Parse(data[7]));
                        geoset.Vertices.Add(vertex);
                    }
                    if (readMode == ModelReadMode.Triangles)
                    {
                        string[] data = line.Split(' ');
                        int one = int.Parse( data[0]);
                        int two = int.Parse( data[1]);
                        int three = int.Parse( data[2]);
                        CGeosetTriangle triangle = new CGeosetTriangle(model);
                        triangle.Vertex1.Attach(geoset.Vertices[one]);
                        triangle.Vertex2.Attach(geoset.Vertices[two]);
                        triangle.Vertex3.Attach(geoset.Vertices[three]);
                        geoset.Triangles.Add(triangle);
                        
                    }
                }
            }

            return geoset;
        }

        internal static string Write(CGeoset geoset)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[vertices]");
            foreach (var vertex in geoset.Vertices)
            {
                sb.AppendLine(VertexToLine(vertex));
            }
            sb.AppendLine("[triangles]");
            foreach (var triangle in geoset.Triangles)
            {
                string f = geoset.Vertices.IndexOf(triangle.Vertex1.Object).ToString();
                string s  = geoset.Vertices.IndexOf(triangle.Vertex2.Object).ToString();
                string t = geoset.Vertices.IndexOf(triangle.Vertex3.Object).ToString();
            sb.AppendLine($"{f} {s} {t}");
            }
           return sb.ToString();
        }
        private static string VertexToLine(CGeosetVertex vertex) {


            return  $"{vertex.Position.X} {vertex.Position.Y} {vertex.Position.Z} {vertex.Normal.X} {vertex.Normal.Y} {vertex.Normal.Z} {vertex.TexturePosition.X} {vertex.TexturePosition.Y}";
        }

        internal static CGeoset ReadGeomerge(string openPath, CModel currentModel)
        {
            CGeoset g = new CGeoset(currentModel);
          try
           {
                g = ReadGeomerge_(openPath, currentModel);

           }
          catch (Exception ex) { MessageBox.Show(ex.Message, "Excepion while reading the geomerge file" );return null; ; }
          
            CBone bone = new CBone(currentModel);
            bone.Name = "ImportedGeomergeBone_" + IDCounter.Next_;
            CGeosetGroup group = new(currentModel);
            CGeosetGroupNode gnode = new CGeosetGroupNode(currentModel);
            gnode.Node.Attach(bone);    
            group.Nodes.Add(gnode);
            g.Groups.Add(group);
            currentModel.Geosets.Add(g);
            currentModel.Materials.Add(g.Material.Object);
            
            foreach (var layer in g.Material.Object.Layers)
            {
                var texture = layer.Texture.Object;
                if (texture.ReplaceableId > 0)
                {
                    var found = currentModel.Textures.FirstOrDefault(x=>x.ReplaceableId == texture.ReplaceableId);
                    if (found != null)
                    {
                        layer.Texture.Attach(found);
                    }
                    else
                    {
                        currentModel.Textures.Add(texture);
                    }
                }
                else
                {
                    var found = currentModel.Textures.FirstOrDefault(x => x.FileName == texture.FileName);
                    if (found != null)
                    {
                        layer.Texture.Attach(found);

                    }
                    else
                    {
                        currentModel.Textures.Add(texture);
                    }
                }
                if (layer.TextureAnimation != null)
                {
                    currentModel.TextureAnimations.Add(layer.TextureAnimation.Object);
                }
               
            }
            bone.PivotPoint = Calculator.GetCentroidOfGeoset(g);

            return g;
        }
        private static CGeoset ReadGeomerge_(string openPath, CModel model)
        {
            CGeoset geoset = new CGeoset(model);
            CMaterial material = new CMaterial(model);
            
          
            CTextureAnimation x = new CTextureAnimation(model);
            CMaterialLayer CurrentLayer = new CMaterialLayer(model);
            bool Mode_Vertices = false;
            bool Alpha_Animated = false;
            ModelReadMode readMode = ModelReadMode.Nothing;
            ModelReadMode txReadMode = ModelReadMode.Nothing;

            using (StreamReader reader = new StreamReader(openPath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (readMode == ModelReadMode.Nothing)
                    {
                        if (line.StartsWith("[vertices]")) { readMode = ModelReadMode.Vertices; continue; }
                        if (line.StartsWith("[triangles]")) { readMode = ModelReadMode.Triangles; continue; }
                        if (line.StartsWith("[material]")) { material = new CMaterial(model); readMode = ModelReadMode.Material; continue; }
                         
                    }
                    if (readMode == ModelReadMode.Vertices)
                    {
                        if (line.StartsWith("[End]")) { readMode = ModelReadMode.Nothing; continue; }
                        CGeosetVertex vertex = new CGeosetVertex(model);
                        string[] data = line.Split(' ');
                        vertex.Position = new MdxLib.Primitives.CVector3(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]));
                        vertex.Normal = new MdxLib.Primitives.CVector3(float.Parse(data[3]), float.Parse(data[4]), float.Parse(data[5]));
                        vertex.TexturePosition = new MdxLib.Primitives.CVector2(float.Parse(data[6]), float.Parse(data[7]));
                        geoset.Vertices.Add(vertex);
                    }
                    else if (readMode == ModelReadMode.Triangles)
                    {
                        if (line.StartsWith("[End]")) { readMode = ModelReadMode.Nothing; continue; }
                        string[] data = line.Split(' ');
                        int one = int.Parse(data[0]);
                        int two = int.Parse(data[1]);
                        int three = int.Parse(data[2]);
                        CGeosetTriangle triangle = new CGeosetTriangle(model);
                        triangle.Vertex1.Attach(geoset.Vertices[one]);
                        triangle.Vertex2.Attach(geoset.Vertices[two]);
                        triangle.Vertex3.Attach(geoset.Vertices[three]);
                        geoset.Triangles.Add(triangle);

                    }
                    else if (readMode == ModelReadMode.Material)
                    {
                        if (line.StartsWith("[End]")) { readMode = ModelReadMode.Nothing; continue; }
                        if (line.StartsWith("[layer]")) { readMode = ModelReadMode.Layer; continue; }
                        if (line.StartsWith("[TextureAnim]")) { readMode = ModelReadMode.TextureAnim; continue; }
                        if (line.StartsWith("ConstantColor")) { material.ConstantColor = true; continue; }
                        if (line.StartsWith("SortPrimitivesFarZ")) { material.SortPrimitivesFarZ = true; continue; }
                        if (line.StartsWith("SortPrimitivesNearZ")) { material.SortPrimitivesNearZ = true; continue; }
                        if (line.StartsWith("FullResolution")) { material.FullResolution = true; continue; }
                        if (line.StartsWith("PriorityPlane")) { material.PriorityPlane = int.Parse(line.Split(' ')[1]); continue; }
                    }
                    else if (readMode == ModelReadMode.Layer)
                    {
                        
                        if (line.StartsWith("[End]")) 
                        {
                             if (Alpha_Animated) { Alpha_Animated = false; continue; }
                            
                            CMaterialLayer copy = CopyLayer(CurrentLayer, model);
                          material.Layers.Add(copy ); ; CurrentLayer = new CMaterialLayer(model); 
                            continue; }
                        if (line.StartsWith("[Alpha]")) { CurrentLayer.Alpha.MakeAnimated(); Alpha_Animated = true; continue; }
                        else if (line.StartsWith("Unfogged")) { CurrentLayer.Unfogged = true; continue; }
                        else if (line.StartsWith("Unshaded")) { CurrentLayer.Unshaded = true; continue; }
                        else if (line.StartsWith("TwoSided")) { CurrentLayer.TwoSided = true; continue; }
                        else if (line.StartsWith("SphereEnvironmentMap")) { CurrentLayer.SphereEnvironmentMap = true; continue; }
                        else if (line.StartsWith("NoDepthSet")) { CurrentLayer.NoDepthSet = true; continue; }
                        else if (line.StartsWith("NoDepthTest")) { CurrentLayer.NoDepthTest = true; continue; }
                        else if (line.StartsWith("Alpha_s")) 
                        {
                            string[] data = line.Split(" ");
                            CurrentLayer.Alpha.MakeStatic(float.Parse(data[1]));
                                continue;
                        }
                        else if (line.StartsWith("FilterMode"))
                        {
                            int index = int.Parse(line.Split(" ")[1]);
                            CurrentLayer.FilterMode = (EMaterialLayerFilterMode)index; continue;
                        }
                        else if (line.StartsWith("ReplaceableId")) 
                        {
                            string[] data = line.Split(" ");
                            int id = int.Parse(data[1]);
                            CTexture tex = new CTexture(model);
                            tex.ReplaceableId = id;
                            CurrentLayer.Texture.Attach(tex);
                            continue;   
                        }
                        else if (line.StartsWith("Filename"))
                        {
                            string[] data = line.Split(" ");
                          
                            CTexture tex = new CTexture(model);
                            tex.FileName = data[1];
                            CurrentLayer.Texture.Attach(tex);
                            continue;
                        }
                        else if (Alpha_Animated)
                        {
                            string[] data = line.Split(" ");
                            int time = int.Parse(data[0]);
                            float value = float.Parse(data[1]);
                          
                            CurrentLayer.Alpha.Add(new MdxLib.Animator.CAnimatorNode<float>(time,value));
                        }

                    }
                    else if (readMode == ModelReadMode.TextureAnim)
                    {
                        if (line.StartsWith("[End]")) { CurrentLayer.TextureAnimation.Attach(x); readMode = ModelReadMode.Nothing; continue; }
                        if (txReadMode == ModelReadMode.Nothing)
                        {
                            if (line == "[Translation]") { txReadMode = ModelReadMode.Translation; continue; }
                            if (line == "[Rotation]") { txReadMode = ModelReadMode.Rotation; continue; }
                            if (line == "[Scaling]") { txReadMode = ModelReadMode.Scaling; continue; }
                        }
                        else if (txReadMode == ModelReadMode.Translation)
                        {
                            if (line == "[End]") { txReadMode = ModelReadMode.Nothing; continue; }
                            string[] data = line.Split(" ");
                            int track = int.Parse(data[0]);
                            var value= ExtractVector(data[1]);
                            x.Translation.MakeAnimated();
                            x.Translation.Add(new MdxLib.Animator.CAnimatorNode<MdxLib.Primitives.CVector3>(track, value));
                        }
                        else if(txReadMode == ModelReadMode.Rotation)
                        {
                            if (line == "[End]") { txReadMode = ModelReadMode.Nothing; continue; }
                            string[] data = line.Split(" ");
                            int track = int.Parse(data[0]);
                            var value = ExtractVector4(data[1]);
                            x.Rotation.MakeAnimated();
                            x.Rotation.Add(new MdxLib.Animator.CAnimatorNode<MdxLib.Primitives.CVector4>(track, value));

                        }
                        else if (txReadMode == ModelReadMode.Scaling)
                        {
                            if (line == "[End]") { txReadMode = ModelReadMode.Nothing; continue; }
                            string[] data = line.Split(" ");
                            int track = int.Parse(data[0]);
                            var value = ExtractVector(data[1]);
                            x.Scaling.MakeAnimated();
                            x.Scaling.Add(new MdxLib.Animator.CAnimatorNode<MdxLib.Primitives.CVector3>(track, value));


                        }

                    }
                }
               
            }
            geoset.Material.Attach(material);
            return geoset;
        }

        private static CMaterialLayer CopyLayer(CMaterialLayer o, CModel owner)
        {
            CMaterialLayer l = new CMaterialLayer(owner);
            l.Texture.Attach(o.Texture.Object);
            l.TextureAnimation.Attach(o.TextureAnimation.Object);
            l.FilterMode = o.FilterMode;
            l.Unfogged = o.Unfogged;
           l.Unshaded = o.Unshaded;
            l.TwoSided = o.TwoSided;
            l.SphereEnvironmentMap = o.SphereEnvironmentMap;
            l.NoDepthTest = o.NoDepthTest;
            l.NoDepthSet = o.NoDepthSet;
            if (o.Alpha.Static)
            {
                l.Alpha.MakeStatic(o.Alpha.GetValue());
            }
            else
            {
                foreach (var v in o.Alpha)
                {
                    l.Alpha.Add(new MdxLib.Animator.CAnimatorNode<float>(v));
                }
            }
                return l;
        }

        private static MdxLib.Primitives.CVector3 ExtractVector(string v)
        {
            string dummy = v.Replace("{", "").ToString() .Replace("}", "").ToString() .Replace(" ", "").ToString();

            string[] data = dummy.Split(",");
            float x = float.Parse(data[0]);
            float y = float.Parse(data[1]);
            float z = float.Parse(data[2]);
            return new MdxLib.Primitives. CVector3(x,y,z);
        }
        private static MdxLib.Primitives.CVector4 ExtractVector4(string v)
        {
            string dummy = v.Replace("{", "").ToString().Replace("}", "").ToString().Replace(" ", "").ToString();

            string[] data = dummy.Split(",");
            float x = float.Parse(data[0]);
            float y = float.Parse(data[1]);
            float z = float.Parse(data[2]);
            float w = float.Parse(data[3]);
            return new MdxLib.Primitives.CVector4(x, y, z,w);
        }

        internal static void ExportGeomerge(CModel currentModel, CGeoset geoset)
        {
            string path = FileSeeker.SaveTGeomFileDialog();
            if (path .Length==0) {  return; }
            string data = GenerateGeomergeDaa(geoset);
            File.WriteAllText(path, data);

         }

        private static string GenerateGeomergeDaa(CGeoset geoset)
        {
            if (geoset.Material.Object == null) { throw new Exception("The geoset has no material"); }
           
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[vertices]");
            foreach (var vertex in geoset.Vertices)
            {
                sb.AppendLine(VertexToLine(vertex));
            }
            sb.AppendLine("[End]");
            sb.AppendLine("[triangles]");
            foreach (var triangle in geoset.Triangles)
            {
                string f = geoset.Vertices.IndexOf(triangle.Vertex1.Object).ToString();
                string s = geoset.Vertices.IndexOf(triangle.Vertex2.Object).ToString();
                string t = geoset.Vertices.IndexOf(triangle.Vertex3.Object).ToString();
                sb.AppendLine($"{f} {s} {t}");
            }
            sb.AppendLine("[End]");
            sb.AppendLine("[material]");
          
            var mat = geoset.Material.Object;
            if (mat.ConstantColor) sb.AppendLine("ConstantColor");
            if (mat.SortPrimitivesFarZ) sb.AppendLine("SortPrimitivesFarZ");
            if (mat.SortPrimitivesNearZ) sb.AppendLine("SortPrimitivesNearZ");
            if (mat.FullResolution) sb.AppendLine("FullResolution");
            sb.AppendLine("PriorityPlane " + mat.PriorityPlane.ToString());
            foreach (var layer in mat.Layers)
            {
                sb.AppendLine("[layer]");
                if (layer.Unshaded) sb.AppendLine("Unshaded");
                if (layer.Unfogged) sb.AppendLine("Unfogged");
                if (layer.TwoSided) sb.AppendLine("TwoSided");
                if (layer.SphereEnvironmentMap) sb.AppendLine("SphereEnvironmentMap");
                if (layer.NoDepthSet) sb.AppendLine("NoDepthSet");
                if (layer.NoDepthSet) sb.AppendLine("NoDepthSet");
                sb.AppendLine("FilterMode "+((int)layer.FilterMode).ToString() );
                var tx = layer.Texture.Object;
                if (tx != null)
                {
                    if (tx.ReplaceableId > 0)
                    {
                        sb.AppendLine("ReplaceableId " + tx.ReplaceableId);
                    }
                    else
                    {
                        sb.AppendLine("Filename " + tx.FileName);
                    }
                }
                if (layer.Alpha.Static)
                {
                    sb.AppendLine("Alpha_s " + layer.Alpha.GetValue().ToString());
                }
                else
                {
                    sb.AppendLine("[Alpha]");
                    foreach (var kf in layer.Alpha)
                    {
                        sb.AppendLine($"{kf.Time}: {kf.Value}");
                    }
                    sb.AppendLine("[End]");
                }
                var txa = layer.TextureAnimation.Object;
                if (txa != null)
                {
                    sb.AppendLine("[TextureAnim]");
                    if (txa.Translation.Animated && txa.Translation.Count > 0)
                    {
                        sb.AppendLine("[Translation]");
                        foreach (var kf in txa.Translation)
                        {
                            sb.AppendLine($"{kf.Time}: {kf.Value}");
                        }
                        sb.AppendLine("[End]");
                    }
                    if (txa.Rotation.Animated && txa.Rotation.Count>0)
                    {
                        sb.AppendLine("[Rotation]");
                        foreach (var kf in txa.Rotation)
                        {
                            sb.AppendLine($"{kf.Time}: {kf.Value.ToString()}");
                        }
                        sb.AppendLine("[End]");
                    }
                    if (txa.Scaling.Animated && txa.Scaling.Count>0)
                    {
                        sb.AppendLine("[Scaling]");
                        foreach (var kf in txa.Scaling)
                        {
                            sb.AppendLine($"{kf.Time}: {kf.Value}");
                        }
                        sb.AppendLine("[End]");
                    }
                    sb.AppendLine("[End]");
                }
                sb.AppendLine("[End]");
            }
           
            return sb.ToString();   
        }
    }
}