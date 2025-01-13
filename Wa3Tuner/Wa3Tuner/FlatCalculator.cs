using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Wa3Tuner
{
    public static class FlatCalculator
    {
        private class Orientation
        {
            public Vector3 Normal; // Single normal vector for orientation
            public float Distance; // Distance to the plane (dot product with a point)

            public override bool Equals(object obj)
            {
                if (obj is not Orientation other)
                    return false;

                return Normal.Equals(other.Normal) && Distance.Equals(other.Distance);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Normal, Distance);
            }
        }

        internal static List<List<CGeosetTriangle>> CollectFlatSurfaces(CGeoset geoset)
        {
            var flatSurfaces = new List<List<CGeosetTriangle>>();
            var visited = new HashSet<CGeosetTriangle>();

            foreach (var triangle in geoset.Triangles)
            {
                if (!visited.Contains(triangle))
                {
                    // Start a new flat surface group
                    var flatSurface = CollectConnectedFlatTriangles(triangle, visited, geoset);
                    if (flatSurface.Count > 0)
                    {
                        flatSurfaces.Add(flatSurface);
                    }
                }
            }

            return flatSurfaces;
        }

        private static List<CGeosetTriangle> CollectConnectedFlatTriangles(
            CGeosetTriangle startTriangle,
            HashSet<CGeosetTriangle> visited,
            CGeoset geoset)
        {
            var connectedTriangles = new List<CGeosetTriangle>();
            var stack = new Stack<CGeosetTriangle>();

            // Determine the orientation of the starting triangle
            var startOrientation = GetTriangleOrientation(startTriangle);

            stack.Push(startTriangle);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (visited.Contains(current))
                    continue;

                visited.Add(current);
                connectedTriangles.Add(current);

                // Check neighbors
                foreach (var neighbor in GetNeighbors(current, geoset))
                {
                    if (!visited.Contains(neighbor) &&
                        AreTrianglesConnected(current, neighbor) &&
                        GetTriangleOrientation(neighbor).Equals(startOrientation))
                    {
                        stack.Push(neighbor);
                    }
                }
            }

            return connectedTriangles;
        }

        private static Orientation GetTriangleOrientation(CGeosetTriangle triangle)
        {
            // Get triangle vertices
            var v1 = triangle.Vertex1.Object.Position;
            var v2 = triangle.Vertex2.Object.Position;
            var v3 = triangle.Vertex3.Object.Position;

            // Compute the normal vector (cross product of two edges)
            var edge1 = v2 - v1;
            var edge2 = v3 - v1;
            var normal = Normalize(Vector3.Cross(V(edge1), V(edge2)));
            

            // Compute the distance of the triangle's plane from the origin
            var distance = Vector3.Dot(normal, V(v1));

            return new Orientation
            {
                Normal = normal,
                Distance = distance
            };
        }
        public static Vector3 Normalize(Vector3 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            if (magnitude > 0)
            {
                return new Vector3(vector.X / magnitude, vector.Y / magnitude, vector.Z / magnitude);
            }
            return new Vector3(0, 0, 0); // Return zero vector if the magnitude is zero
        }

        static Vector3 V(CVector3 v) { return new Vector3(v.X, v.Y, v.Z); }
        private static IEnumerable<CGeosetTriangle> GetNeighbors(CGeosetTriangle triangle, CGeoset geoset)
        {
            // Find triangles sharing at least one edge with the current triangle
            return geoset.Triangles.Where(other =>
                other != triangle &&
                AreTrianglesConnected(triangle, other));
        }

        private static bool AreTrianglesConnected(CGeosetTriangle a, CGeosetTriangle b)
        {
            // Check if two triangles share an edge (two common vertices)
            var verticesA = new[] { a.Vertex1.Object, a.Vertex2.Object, a.Vertex3.Object };
            var verticesB = new[] { b.Vertex1.Object, b.Vertex2.Object, b.Vertex3.Object };

            return verticesA.Intersect(verticesB).Count() >= 2;
        }
    }



}
