using MdxLib.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Wa3Tuner.Helper_Classes.Parsers
{
    public static class ParserPLY
    {

        public static void ExportASCII(List<CGeoset> geosets, CModel owner, bool AllAsOne = true)
        {
            //checks
            if (geosets.Count == 0) return;
            // get export path
            string p = FileSeeker.SavePly();
            if (p.Length == 0) return;
            // gather all geosets as one if possible
            CGeoset geoset = new CGeoset(owner);
            if (geosets.Count == 1) { geoset = geosets[0]; }
            else
            {
                foreach (var g in geosets)
                {
                    geoset.Vertices.AddRange(g.Vertices);
                    geoset.Triangles.AddRange(g.Triangles);
                }
            }
            if (AllAsOne || geosets.Count == 1)
            {
                File.WriteAllText(p, GetASCII(geoset));
            }
            else
            {
                for (int i = 0; i < geosets.Count; i++)
                {
                    string px = EnumerateFilePath(p, i);
                    File.WriteAllText(px, GetASCII(geosets[0]));
                }
            }
        }
        private static string GetASCII(CGeoset geoset)
        {
            // generate data
            StringBuilder data = new StringBuilder();
            data.AppendLine("ply");
            data.AppendLine("format ascii 1.0");
            data.AppendLine("comment - Exported from War3 Model Tuner");
            data.AppendLine($"element vertex {geoset.Vertices.Count}");
            data.AppendLine("property float x");
            data.AppendLine("property float y");
            data.AppendLine("property float z");
            data.AppendLine($"element face {geoset.Triangles.Count}");
            data.AppendLine("property list uchar int vertex_indices");
            data.AppendLine("end_header");

            // Export Vertices
            foreach (var vertex in geoset.Vertices)
            {
                var pos = vertex.Position;
                data.AppendLine($"{pos.X} {pos.Y} {pos.Z}");
            }

            // Store vertex indices in a dictionary to avoid slow lookups
            var vertexIndexMap = geoset.Vertices
                .Select((v, index) => new { Vertex = v, Index = index })
                .ToDictionary(v => v.Vertex, v => v.Index);

            // Export Faces
            foreach (var triangle in geoset.Triangles)
            {
                int v1 = vertexIndexMap[triangle.Vertex1.Object];
                int v2 = vertexIndexMap[triangle.Vertex2.Object];
                int v3 = vertexIndexMap[triangle.Vertex3.Object];

                data.AppendLine($"3 {v1} {v2} {v3}");  // Include '3' to indicate a triangle
            }
            return data.ToString();
        }
    
 
        public static void ExportBinary(List<CGeoset> geosets, CModel owner, bool AllAsOne = true)
        {
            // Checks
            if (geosets.Count == 0) return;

            // Get export path
            string p = FileSeeker.SavePly();
            if (p.Length == 0) return;

            // Gather all geosets as one if needed
            CGeoset geoset = new CGeoset(owner);
            if (geosets.Count == 1)
            {
                geoset = geosets[0];
            }
            else
            {
                foreach (var g in geosets)
                {
                    geoset.Vertices.AddRange(g.Vertices);
                    geoset.Triangles.AddRange(g.Triangles);
                }
            }

            if (AllAsOne || geosets.Count == 1)
            {
                WriteBinaryPLY(p, geoset);
            }
            else
            {
                for (int i = 0; i < geosets.Count; i++)
                {
                    string px = EnumerateFilePath(p, i);
                    WriteBinaryPLY(px, geosets[i]);
                }
            }
        }

        private static void WriteBinaryPLY(string filePath, CGeoset geoset)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (BinaryWriter writer = new BinaryWriter(fs, Encoding.ASCII))
            {
                // Write header
                StringBuilder header = new StringBuilder();
                header.AppendLine("ply");
                header.AppendLine("format binary_little_endian 1.0");
                header.AppendLine("comment - Exported from War3 Model Tuner");
                header.AppendLine($"element vertex {geoset.Vertices.Count}");
                header.AppendLine("property float x");
                header.AppendLine("property float y");
                header.AppendLine("property float z");
                header.AppendLine($"element face {geoset.Triangles.Count}");
                header.AppendLine("property list uchar int vertex_indices");
                header.AppendLine("end_header");

                // Write header as bytes
                writer.Write(Encoding.ASCII.GetBytes(header.ToString()));

                // Write vertex data
                foreach (var vertex in geoset.Vertices)
                {
                    var pos = vertex.Position;
                    writer.Write(pos.X);
                    writer.Write(pos.Y);
                    writer.Write(pos.Z);
                }

                // Store vertex indices in a dictionary to avoid slow lookups
                var vertexIndexMap = geoset.Vertices
                    .Select((v, index) => new { Vertex = v, Index = index })
                    .ToDictionary(v => v.Vertex, v => v.Index);

                // Write face data
                foreach (var triangle in geoset.Triangles)
                {
                    writer.Write((byte)3); // Number of vertices in the face
                    writer.Write(vertexIndexMap[triangle.Vertex1.Object]);
                    writer.Write(vertexIndexMap[triangle.Vertex2.Object]);
                    writer.Write(vertexIndexMap[triangle.Vertex3.Object]);
                }
            }
        
}

        private static string EnumerateFilePath(string filePath, int number)
        {
            string directory = Path.GetDirectoryName(filePath) ?? "";
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);

            return Path.Combine(directory, $"{fileNameWithoutExtension}_{number}{extension}");
        }
        public static void Import(string Filepath, CModel InModel)
        {
            if (InModel.Materials.Count == 0)
            {
                MessageBox.Show("There are no materials"); return;
            }
            CGeoset imported = new CGeoset(InModel);
            if (FileIsBinary())
            {
                imported = importBinaryPLY();
            }
            else
            {
                imported = importASCIIPLY();
            }

        }

        private static CGeoset importASCIIPLY()
        {
            throw new NotImplementedException();
        }

        private static CGeoset importBinaryPLY()
        {
            throw new NotImplementedException();
        }

        private static bool FileIsBinary()
        {
            throw new NotImplementedException();
        }
    }
}
