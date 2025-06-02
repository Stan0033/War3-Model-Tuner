using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Wa3Tuner.Helper_Classes
{
    public static class STL_Maker
    {
        public static string GenerateFile(CGeoset geoset)
        {
            if (geoset.Triangles.Count == 0) return "";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("solid ExportedGeosetFromWarcraftIIIModel");

            foreach (var triangle in geoset.Triangles)
            {
                var v1 = triangle.Vertex1.Object.Position;
                var v2 = triangle.Vertex2.Object.Position;
                var v3 = triangle.Vertex3.Object.Position;
                var normal = CalculateTriangleNormal(triangle);

                // Use InvariantCulture to ensure correct decimal points
                sb.AppendLine($"  facet normal {normal.X.ToString(CultureInfo.InvariantCulture)} {normal.Y.ToString(CultureInfo.InvariantCulture)} {normal.Z.ToString(CultureInfo.InvariantCulture)}");
                sb.AppendLine("    outer loop");
                sb.AppendLine($"      vertex {v1.X.ToString(CultureInfo.InvariantCulture)} {v1.Y.ToString(CultureInfo.InvariantCulture)} {v1.Z.ToString(CultureInfo.InvariantCulture)}");
                sb.AppendLine($"      vertex {v2.X.ToString(CultureInfo.InvariantCulture)} {v2.Y.ToString(CultureInfo.InvariantCulture)} {v2.Z.ToString(CultureInfo.InvariantCulture)}");
                sb.AppendLine($"      vertex {v3.X.ToString(CultureInfo.InvariantCulture)} {v3.Y.ToString(CultureInfo.InvariantCulture)} {v3.Z.ToString(CultureInfo.InvariantCulture)}");
                sb.AppendLine("    endloop");
                sb.AppendLine("  endfacet");
            }

            sb.AppendLine("endsolid ExportedGeosetFromWarcraftIIIModel");
            return sb.ToString();
        }

        public static void GenerateBinarySTL(CGeoset geoset, string filePath)
        {
            if (geoset.Triangles.Count == 0) return;

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(fs))
            {
                // Write 80-byte header (can be left empty)
                byte[] header = new byte[80];
                writer.Write(header);

                // Write number of triangles (4 bytes)
                writer.Write(geoset.Triangles.Count);

                foreach (var triangle in geoset.Triangles)
                {
                    var v1 = triangle.Vertex1.Object.Position;
                    var v2 = triangle.Vertex2.Object.Position;
                    var v3 = triangle.Vertex3.Object.Position;
                    var normal = CalculateTriangleNormal(triangle);

                    // Write normal vector (12 bytes)
                    writer.Write(normal.X);
                    writer.Write(normal.Y);
                    writer.Write(normal.Z);

                    // Write vertex 1 (12 bytes)
                    writer.Write(v1.X);
                    writer.Write(v1.Y);
                    writer.Write(v1.Z);

                    // Write vertex 2 (12 bytes)
                    writer.Write(v2.X);
                    writer.Write(v2.Y);
                    writer.Write(v2.Z);

                    // Write vertex 3 (12 bytes)
                    writer.Write(v3.X);
                    writer.Write(v3.Y);
                    writer.Write(v3.Z);

                    // Write attribute byte count (always 0)
                    writer.Write((ushort)0);
                }
            }

        }

        public static List<CGeoset> ReadSTL_ASCII(CModel owner, string FileName)
        {
            CBone bone = GenerateBone(owner);
            List<CGeoset> list = new List<CGeoset>();
            CGeoset currentlyBuilt = new CGeoset(owner);
            CGeosetTriangle currentlyBuiltTriangle = new CGeosetTriangle(owner);
            bool inFacet = false;
            bool inLoop = false;
            bool inSolid = false;
            int count = 0;
            Vector3 currentFacetNormal = new Vector3();

            using (StreamReader sr = new StreamReader(FileName))
            {
                string? line;
                while ((line = sr.ReadLine()?.Trim().ToLower()) != null)
                {
                    if (line.StartsWith("solid"))
                    {
                        if (inSolid)
                        {
                            throw new Exception("Cannot start a solid when the previous was not closed.");
                        }
                        inSolid = true;
                        PrepareForNextGeoset();
                    }
                    else if (line.StartsWith("endsolid"))
                    {
                        inSolid = false;
                        PrepareForNextGeoset();
                    }
                    else if (line.StartsWith("facet"))
                    {
                        if (inFacet) { throw new Exception("Cannot start a facet when the previous was not closed."); }
                        inFacet = true;

                        if (line.Contains("normal"))
                        {
                            string[] parts = line.Split(" ");
                            if (parts.Length >= 4)
                            {
                                float x = float.Parse(parts[2]);
                                float y = float.Parse(parts[3]);
                                float z = float.Parse(parts[4]);
                                currentFacetNormal = new Vector3(x, y, z);
                            }
                        }
                    }
                    else if (line.StartsWith("endfacet"))
                    {
                        inFacet = false;
                    }
                    else if (line.StartsWith("outer loop"))
                    {
                        if (inLoop) { throw new Exception("Cannot start a loop when the previous was not closed."); }
                        inLoop = true;
                    }
                    else if (line.StartsWith("endloop"))
                    {
                        if (!inLoop) { throw new Exception("Cannot end a loop when it was not started."); }
                        inLoop = false;
                        FinalizeLastTriangle();
                        count = 0; // Reset vertex count after a loop ends
                    }
                    else if (line.StartsWith("vertex"))
                    {
                        if (count > 2) { throw new Exception("Unexpected number of vertices for a loop."); }
                        string[] parts = line.Split(" ");
                        if (parts.Length != 4) { throw new Exception("Unexpected number of parts for a vertex line."); }

                        float x = float.Parse(parts[1]);
                        float y = float.Parse(parts[2]);
                        float z = float.Parse(parts[3]);

                        CGeosetVertex vertex = new CGeosetVertex(owner);
                        vertex.Position = new CVector3(x, y, z);
                        vertex.Normal = new CVector3(currentFacetNormal.X, currentFacetNormal.Y, currentFacetNormal.Z);
                        currentlyBuilt.Vertices.Add(vertex);
                        AttachVertex(currentlyBuilt, vertex, owner, bone);
                        if (count == 0) currentlyBuiltTriangle.Vertex1.Attach(vertex);
                        else if (count == 1) currentlyBuiltTriangle.Vertex2.Attach(vertex);
                        else if (count == 2) currentlyBuiltTriangle.Vertex3.Attach(vertex);

                        count++;
                    }
                }
            }

            void PrepareForNextGeoset()
            {
               
                if (currentlyBuilt.Triangles.Count > 0)
                {
                    currentlyBuilt.Material.Attach(owner.Materials[0]);
                    CBone bone = new CBone(owner);
                    bone.Name = "GeneratedSTLImportedBone_" + IDCounter.Next_;
                    owner.Nodes.Add(bone);
                    CGeosetGroup group = new CGeosetGroup(owner );
                    CGeosetGroupNode gnode = new CGeosetGroupNode(owner);
                    gnode.Node.Attach(bone);
                    group.Nodes.Add(gnode);
                    currentlyBuilt.Groups.Add(group);
                    foreach (var vertex in currentlyBuilt.Vertices)
                    {
                        vertex.Group.Attach(group);
                    }


                    list.Add(currentlyBuilt);
                }
                currentlyBuilt = new CGeoset(owner);
            }

            void FinalizeLastTriangle()
            {
                if (count == 3) // Ensure all 3 vertices are processed
                {
                    var triangle = new CGeosetTriangle(owner);
                    triangle.Vertex1.Attach(currentlyBuiltTriangle.Vertex1.Object);
                    triangle.Vertex2.Attach(currentlyBuiltTriangle.Vertex2.Object);
                    triangle.Vertex3.Attach(currentlyBuiltTriangle.Vertex3.Object);

                    currentlyBuilt.Vertices.Add(currentlyBuiltTriangle.Vertex1.Object);
                    currentlyBuilt.Vertices.Add(currentlyBuiltTriangle.Vertex2.Object);
                    currentlyBuilt.Vertices.Add(currentlyBuiltTriangle.Vertex3.Object);
                    currentlyBuilt.Triangles.Add(triangle);
                    currentlyBuiltTriangle = new CGeosetTriangle(owner); // Reset for the next triangle

                }
            }

            return list;
        }
        public static List<CGeoset> ReadSTL_Binary(CModel owner, string FileName)
        {
            CBone bone = GenerateBone(owner);
            List<CGeoset> list = new List<CGeoset>();
            CGeoset currentlyBuilt = new CGeoset(owner);
            CGeosetTriangle currentlyBuiltTriangle = new CGeosetTriangle(owner);

            using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // Skip the 80-byte header
                reader.BaseStream.Seek(80, SeekOrigin.Begin);

                // Read the number of triangles (4 bytes)
                int triangleCount = reader.ReadInt32();

                for (int i = 0; i < triangleCount; i++)
                {
                    // Read facet normal (3 floats = 12 bytes)
                    float normalX = reader.ReadSingle();
                    float normalY = reader.ReadSingle();
                    float normalZ = reader.ReadSingle();

                    // Read 3 vertices (each vertex has 3 floats = 12 bytes per vertex)
                    for (int j = 0; j < 3; j++)
                    {
                        float vertexX = reader.ReadSingle();
                        float vertexY = reader.ReadSingle();
                        float vertexZ = reader.ReadSingle();

                        CGeosetVertex vertex = new CGeosetVertex(owner);
                        vertex.Position = new CVector3(vertexX, vertexY, vertexZ);
                        vertex.Normal = new CVector3(normalX, normalY, normalZ); // Use the same normal for all 3 vertices
                        currentlyBuilt.Vertices.Add(vertex);
                        AttachVertex(currentlyBuilt, vertex, owner, bone);
                        if (j == 0) currentlyBuiltTriangle.Vertex1.Attach(vertex);
                        if (j == 1) currentlyBuiltTriangle.Vertex2.Attach(vertex);
                        if (j == 2) currentlyBuiltTriangle.Vertex3.Attach(vertex);
                    }

                    // Skip the 2-byte attribute byte count (not needed)
                    reader.ReadUInt16();

                    // Add the triangle to the geoset once all 3 vertices are read
                    currentlyBuilt.Triangles.Add(currentlyBuiltTriangle);
                    currentlyBuiltTriangle = new CGeosetTriangle(owner); // Reset for the next triangle
                }
            }

            // Add the last geoset to the list
            if (currentlyBuilt.Triangles.Count > 0)
            {
                list.Add(currentlyBuilt);
            }
            foreach (var item in list) { item.Material.Attach(owner.Materials[0]); }

            return list;
        }

        private static void AttachVertex(CGeoset geoset, CGeosetVertex vertex, CModel owner, CBone bone)
        {
            CGeosetGroup group = new CGeosetGroup(owner);
            CGeosetGroupNode gnode = new CGeosetGroupNode(owner);
            gnode.Node.Attach(bone);
            group.Nodes.Add(gnode);
            vertex.Group.Attach(group);
            geoset.Groups.Add(group);
        }

        private static CBone GenerateBone(CModel owner)
        {
            CBone bone = new CBone(owner);
            bone.Name = $"GeneratedSTLBone_{IDCounter.Next_}";
            owner.Nodes.Add(bone);
            return bone;
        }

        public static bool IsSTLBinary(string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    // Read the first 5 bytes
                    byte[] header = new byte[5];
                    fs.Read(header, 0, 5);

                    // If the first part of the file contains "solid", it's likely an ASCII STL
                    string headerString = System.Text.Encoding.ASCII.GetString(header);
                    if (headerString.StartsWith("solid"))
                    {
                        return false; // It's an ASCII STL
                    }

                    // If the file starts with non-ASCII values, it's probably binary
                    // Check if the header size (80 bytes) and the triangle count look like a binary STL structure
                    if (header.Length == 5 && header[0] != 0 && header[1] != 0 && header[2] != 0)
                    {
                        return true; // It's a binary STL
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading file: " + ex.Message);
            }
            return false; // Default to false if something goes wrong
        }



        private static Vector3 CalculateTriangleNormal(CGeosetTriangle triangle)
        {
            CVector3 normal1 = triangle.Vertex1.Object.Normal;
            CVector3 normal2 = triangle.Vertex2.Object.Normal;
            CVector3 normal3 = triangle.Vertex3.Object.Normal;

            // Sum the three normals
            Vector3 averagedNormal = new Vector3(
                normal1.X + normal2.X + normal3.X,
                normal1.Y + normal2.Y + normal3.Y,
                normal1.Z + normal2.Z + normal3.Z
            );

            // Normalize the result
            return Vector3.Normalize(averagedNormal);
        }
        private static CVector3[] DistributeNormal(CVector3 normal)
        {
            return new CVector3[] { normal, normal, normal };
        }

        internal static List<CGeoset> Import(CModel owner, string file)
        {
            if (IsSTLBinary(file))
            {
                return ReadSTL_Binary(owner, file);
            }else
            {
                return ReadSTL_ASCII(owner,file);
            }
                
        }
    }
}