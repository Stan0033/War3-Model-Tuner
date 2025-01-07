using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Wa3Tuner
{
    internal class GeosetComparer
    {

        internal static bool IdenticalPositions(CGeoset geoset1, CGeoset geoset2)
        {
            if (geoset1.Vertices.Count != geoset2.Vertices.Count) return false;
            if (geoset1.Faces.ObjectList.Count != geoset2.Faces.ObjectList.Count) return false;

            // Define a small tolerance for floating-point comparisons
            const float epsilon = 0.0001f;

            // Compare vertex positions, ignoring order
            List<CVector3> vertices1 = new List<CVector3>();
            for (int i = 0; i < geoset1.Vertices.Count; i++)
            {
                vertices1.Add(geoset1.Vertices[i].Position);
            }

            List<CVector3> vertices2 = new List<CVector3>();
            for (int i = 0; i < geoset2.Vertices.Count; i++)
            {
                vertices2.Add(geoset2.Vertices[i].Position);
            }

            vertices1.Sort((a, b) =>
            {
                int xComparison = a.X.CompareTo(b.X);
                if (xComparison != 0) return xComparison;
                int yComparison = a.Y.CompareTo(b.Y);
                if (yComparison != 0) return yComparison;
                return a.Z.CompareTo(b.Z);
            });

            vertices2.Sort((a, b) =>
            {
                int xComparison = a.X.CompareTo(b.X);
                if (xComparison != 0) return xComparison;
                int yComparison = a.Y.CompareTo(b.Y);
                if (yComparison != 0) return yComparison;
                return a.Z.CompareTo(b.Z);
            });

            for (int i = 0; i < vertices1.Count; i++)
            {
                if (Math.Abs(vertices1[i].X - vertices2[i].X) > epsilon ||
                    Math.Abs(vertices1[i].Y - vertices2[i].Y) > epsilon ||
                    Math.Abs(vertices1[i].Z - vertices2[i].Z) > epsilon)
                {
                    return false;
                }
            }

            // Compare faces, ignoring order
            List<int[]> faces1 = new List<int[]>();
            for (int i = 0; i < geoset1.Faces.ObjectList.Count; i++)
            {
                CGeosetFace face = geoset1.Faces.ObjectList[i];
                int[] faceVertices = new int[] { face.Vertex1.ObjectId, face.Vertex2.ObjectId, face.Vertex3.ObjectId };
                Array.Sort(faceVertices);
                faces1.Add(faceVertices);
            }

            List<int[]> faces2 = new List<int[]>();
            for (int i = 0; i < geoset2.Faces.ObjectList.Count; i++)
            {
                CGeosetFace face = geoset2.Faces.ObjectList[i];
                int[] faceVertices = new int[] { face.Vertex1.ObjectId, face.Vertex2.ObjectId, face.Vertex3.ObjectId };
                Array.Sort(faceVertices);
                faces2.Add(faceVertices);
            }

            faces1.Sort((a, b) => string.Join(",", a).CompareTo(string.Join(",", b)));
            faces2.Sort((a, b) => string.Join(",", a).CompareTo(string.Join(",", b)));

            for (int i = 0; i < faces1.Count; i++)
            {
                for (int j = 0; j < faces1[i].Length; j++)
                {
                    if (faces1[i][j] != faces2[i][j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }


    }

}