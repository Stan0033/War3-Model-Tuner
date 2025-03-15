using MdxLib.Model;
using System;
using System.Text;

namespace Wa3Tuner
{
    internal class GeosetOBJConverter
    {
        internal static string GetContent(CGeoset geoset)
        {
            StringBuilder sb = new StringBuilder();

            // Write vertices (v)
            foreach (var vertex in geoset.Vertices)
            {
                sb.AppendLine($"v {vertex.Position.X} {vertex.Position.Y} {vertex.Position.Z}");
            }

            // Write normals (n)
            foreach (var vertex in geoset.Vertices)
            {
                sb.AppendLine($"vn {vertex.Normal.X} {vertex.Normal.Y} {vertex.Normal.Z}");
            }

            // Write texture coordinates (vt)
            foreach (var vertex in geoset.Vertices)
            {
                sb.AppendLine($"vt {vertex.TexturePosition.X} {vertex.TexturePosition.Y}");
            }

            // Write faces (f)
            foreach (var triangle in geoset.Triangles)
            {
                // Ensure indices are 1-based
                int index1 = geoset.Vertices.IndexOf(triangle.Vertex1.Object) + 1;
                int index2 = geoset.Vertices.IndexOf(triangle.Vertex2.Object) + 1;
                int index3 = geoset.Vertices.IndexOf(triangle.Vertex3.Object) + 1;

                sb.AppendLine($"f {index1}/{index1}/{index1} {index2}/{index2}/{index2} {index3}/{index3}/{index3}");
            }

            return sb.ToString();
        }

    }
}