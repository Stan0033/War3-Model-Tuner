using MdxLib.Model;
using System;
using System.IO;
using System.Text;

namespace Wa3Tuner
{
    internal class GeosetExporter
    {
        enum ModelReadMode
        {
            Vertices, Triangles, Nothing
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
                        vertex.TexturePosition = new MdxLib.Primitives.CVector2(float.Parse(data[3]), float.Parse(data[4]));
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
    }
}