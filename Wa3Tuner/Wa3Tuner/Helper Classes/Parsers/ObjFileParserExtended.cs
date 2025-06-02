using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
namespace obj2mdl_batch_converter
{
    public class RawObject
    {
        public string Name = string.Empty;
        public List<string> Vertices { get; private set; } = new();
        public List<string> Normals { get; private set; } = new();
        public List<string> TextureCoordinates { get; private set; } = new();
        public List<string> Faces { get; private set; } = new();
        public List<int> TriangleVertexIndices { get; set; } = new List<int>();
        public RawObject(string name) { Name = name; }
        public RawObject Clone()
        {
            RawObject obj = new RawObject(Name);
            obj.Vertices = Vertices.ToList();
            obj.Normals = Normals.ToList();
            obj.TextureCoordinates = TextureCoordinates.ToList();
            obj.Faces = Faces.ToList();
            obj.TriangleVertexIndices = TriangleVertexIndices.ToList();
            return obj;
        }
    }
    public static class ObjFileParserExtended
    {
        public static List<RawObject> Objects = new List<RawObject>();
        public static string Get_Geosets_MDL_String(bool bone, bool mat)
        {
            StringBuilder stringBuilder = new StringBuilder(); int count = 0;
            foreach (RawObject obj in Objects)
            {
                stringBuilder
                    .AppendLine("Geoset {")
                    .AppendLine($"\tVertices {obj.Vertices.Count} {{");
                foreach (string vertex in obj.Vertices) stringBuilder.AppendLine(FormatWithCurlyBraces(vertex));
                stringBuilder
                    .AppendLine("}")
                    .AppendLine($"\tNormals {obj.Vertices.Count} {{");
                for (int normalIndex = 0; normalIndex < obj.Vertices.Count; normalIndex++)
                {
                    if (normalIndex < obj.Normals.Count) stringBuilder.AppendLine(FormatWithCurlyBraces(obj.Normals[normalIndex]));
                    else stringBuilder.AppendLine("{ 0, 0, 0 },");
                }
                stringBuilder
                    .AppendLine("}")
                    .AppendLine($"\tTVertices {obj.Vertices.Count} {{");
                for (int tVertexIndex = 0; tVertexIndex < obj.Vertices.Count; tVertexIndex++)
                {
                    if (tVertexIndex < obj.TextureCoordinates.Count) stringBuilder.AppendLine("\t\t" + FormatWithCurlyBraces(obj.TextureCoordinates[tVertexIndex]));
                    else stringBuilder.AppendLine("\t\t{ 0, 0 },");
                }
                stringBuilder
                .AppendLine("}")
                .AppendLine($"\tVertexGroup {{");
                   for (int i = 0; i < obj.Vertices.Count; i++) { stringBuilder.AppendLine("\t\t0,"); }
                stringBuilder
                .AppendLine("}")
                .AppendLine($"\tFaces 1 {obj.TriangleVertexIndices.Count} {{")
                .AppendLine($"\t\tTriangles {{")
                .AppendLine($"\t\t{{")
                .AppendLine("\t\t\t"+string.Join(", ", obj.TriangleVertexIndices.ConvertAll(i => i.ToString()).ToArray()))
                .AppendLine($"}},\r\n\t\t}}\r\n\t}}\r\n");
                int matrix = 0;
                if (bone)  matrix = count;
                stringBuilder.AppendLine($"\t" + $"Groups 1 1 {{\nMatrices {{ {matrix} }},\n }}");
                if (mat)  stringBuilder.AppendLine($"MaterialID {count},");  
                else stringBuilder.AppendLine("MaterialID 0,");
                stringBuilder.AppendLine("SelectionGroup 0,")
                .AppendLine("}");
                count++;
            }
            return stringBuilder.ToString();
        }
        public static void Parse(string filename, bool MutlipleBones, bool multipleMaterials)
        {
            RawObject temp = new RawObject("");
            using (StreamReader reader = new StreamReader(filename))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.StartsWith("o "))
                    {
                        if (temp.Vertices.Count > 0)
                        {
                            Objects.Add(temp.Clone());
                        }
                        temp = new RawObject(line.Split(' ')[1]); continue;
                    }
                    if (line.StartsWith("v ")) temp.Vertices.Add(line.Substring(2).Trim());
                    else if (line.StartsWith("vn ")) temp.Normals.Add(line.Substring(3).Trim());
                    else if (line.StartsWith("vt ")) temp.TextureCoordinates.Add(line.Substring(3).Trim());
                    else if (line.StartsWith("f ")) temp.Faces.Add(line.Substring(2).Trim());
                }
            }
            if (temp.Vertices.Count > 0)
            {
                Objects.Add(temp.Clone());
            }
            TriangulateFaces();
        }
        private static string GetMaterials_MDL(bool single = false)
        {
            if (single) { return "Materials 1 { \tMaterial {\t\tLayer {\n\t\t\tFilterMode None,\n\t\t\tTwoSided,\n\t\t\tstatic TextureID 0,\n\t\t}\t}\n}"; }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Materials {Objects.Count} {{"); 
            for (int i = 0; i <Objects.Count; i++)
            {
                stringBuilder.AppendLine("\tMaterial {\t\tLayer {\n\t\t\tFilterMode None,\n\t\t\tTwoSided,\n\t\t\tstatic TextureID 0,\n\t\t}\t}");
            }
            stringBuilder.AppendLine("}");
            return stringBuilder.ToString();
        }
        private static string GetBones_MDL(bool single = false)
        {
            if (single) return "Bone \"base\" {\n\tObjectId 0,\n\tGeosetId 0,\n\tGeosetAnimId None,\n}\nAttachment \"Origin Ref\" {\n\tObjectId 1,\n\tAttachmentID 0,\n}\nPivotPoints 2 {\n\t{ 0, 0, 0 },\n\t{ 0, 0, 0 },\n}";
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < Objects.Count; i++)
            {
                stringBuilder.AppendLine($"Bone \"bone_{Objects[i].Name}\" {{");
                stringBuilder.AppendLine($"\tObjectId {i},");
                stringBuilder.AppendLine($"\tGeosetId {i},");
                stringBuilder.AppendLine($"\tGeosetAnimId None,");
                stringBuilder.AppendLine("}");
            }
            stringBuilder.AppendLine($"Attachment \"Origin Ref\" {{");
            stringBuilder.AppendLine($"\tObjectId {Objects.Count},");
            stringBuilder.AppendLine($"\tAttachmentID 0,");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine($"PivotPoints {Objects.Count + 1} {{");
            for (int i = 0; i < Objects.Count+1; i++)
            {
                stringBuilder.AppendLine($"\t{{ 0, 0, 0 }},");
            }
                stringBuilder.AppendLine("}");
            return stringBuilder.ToString();
        }
        private static void Downsize(List<int> list)  { int lowest = list.Min(); for (int i = 0; i < list.Count; i++) list[i] -= lowest; }
        private static void TriangulateFaces()
        {
            foreach (RawObject obj in Objects)
            {
                List<int> VertexIndices = new List<int>();
                foreach (string face in obj.Faces)
                {
                    string[] faceVertices = face.Split(' ');
                    if (faceVertices.Length == 3)
                    {
                        foreach (string faceVertex in faceVertices)
                        {
                            int first = int.Parse(faceVertex.Split('/')[0]);
                            VertexIndices.Add(first - 1); ;
                        }
                    }
                    if (faceVertices.Length == 4)
                    {
                        int first = int.Parse(faceVertices[0].Split('/')[0]);
                        int second = int.Parse(faceVertices[1].Split('/')[0]);
                        int third = int.Parse(faceVertices[2].Split('/')[0]);
                        int fourth = int.Parse(faceVertices[3].Split('/')[0]);
                        VertexIndices.Add(first - 1);
                        VertexIndices.Add(second - 1);
                        VertexIndices.Add(third - 1);
                        VertexIndices.Add(first - 1);
                        VertexIndices.Add(third - 1);
                        VertexIndices.Add(fourth - 1);
                    }
                    if (faceVertices.Length > 4)
                    {
                        List<int> indexes = new List<int>();
                        foreach (string ngon in faceVertices) { indexes.Add(int.Parse(ngon.Split('/')[0]) - 1); }
                        indexes.OrderBy(x => x);
                        while (indexes.Count > 2)
                        {
                            obj.TriangleVertexIndices.Add(indexes[0]);
                            obj.TriangleVertexIndices.Add(indexes[1]);
                            obj.TriangleVertexIndices.Add(indexes[2]);
                            indexes.RemoveAt(1);
                        }
                    }
                    while (VertexIndices.Count > 2)
                    {
                       obj. TriangleVertexIndices.Add(VertexIndices[0]);
                        obj.TriangleVertexIndices.Add(VertexIndices[1]);
                        obj.TriangleVertexIndices.Add(VertexIndices[2]);
                        VertexIndices.RemoveAt(0);
                        VertexIndices.RemoveAt(0);
                        VertexIndices.RemoveAt(0);
                    }
                }
                Downsize(obj.TriangleVertexIndices);
            }
        }
        private static int AnyEmpty()
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                if (Objects[i].Vertices.Count < 3 || Objects[i].Faces.Count == 0) { return i; }
            }
            return -1;
        }
        public static void Save(string filename, bool bones, bool mats)
        {
            string geosets =  "NumGeosets "+ Objects.Count.ToString();
            string bonesCountString = bones ? "NumBones 1" : "NumBones "+ Objects.Count.ToString();
            string name = Path.GetFileName(filename);
            int empty = AnyEmpty();
           if (empty >= 0) { MessageBox.Show($"Insufficient geometry at object {empty} at \"{filename}\""); return; }
           StringBuilder stringBuilder = new StringBuilder()
             
            .AppendLine($"Version {{\n\tFormatVersion 800,\n}}\nModel \"{name}\" {{\n\t{bonesCountString},\n\t{geosets},\n\tNumAttachments 1,\n\tBlendTime 150,\n}}\nTextures 1 {{\n\tBitmap {{\n\t\tImage \"Textures\\white.blp\",\n\t}}\n}}\nSequences 2 {{\n\tAnim \"Stand\" {{\n\t\tInterval {{ 0, 999 }},\n\t}}\n\tAnim \"Death\" {{\n\t\tInterval {{ 1000, 1999 }},\n\t}}\n}}\n")
            .AppendLine(Get_Geosets_MDL_String(bones,mats));
            if (mats) stringBuilder.AppendLine(GetMaterials_MDL())  ;
            else stringBuilder.AppendLine(GetMaterials_MDL(true));
            if (bones) stringBuilder.AppendLine(GetBones_MDL());
            else stringBuilder.AppendLine(GetBones_MDL(true));
            File.WriteAllText(filename, stringBuilder.ToString());
            Objects.Clear();
        }
        private static string FormatWithCurlyBraces(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            var parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var stringBuilder = new StringBuilder("\t\t{ ");
            stringBuilder.Append(string.Join(", ", parts))
             .Append(" },");
            return stringBuilder.ToString();
        }
    }
}
