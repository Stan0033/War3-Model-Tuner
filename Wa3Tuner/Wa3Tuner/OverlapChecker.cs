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
    class OverlapChecker
    {
        public static bool TrianglesIntersect(CGeosetTriangle first, CGeosetTriangle second)
        {
            // Check if bounding boxes of triangles overlap
            if (!BoundingBoxesOverlap(first, second))
                return false;

            // Check edge-edge intersection
            if (EdgesIntersect(first, second))
                return true;

            // Check if one triangle lies entirely inside the other
            if (TriangleInsideTriangle(first, second) || TriangleInsideTriangle(second, first))
                return true;

            return false;
        }

        private static bool BoundingBoxesOverlap(CGeosetTriangle t1, CGeosetTriangle t2)
        {
            CVector3 min1 = GetMin(t1), max1 = GetMax(t1);
            CVector3 min2 = GetMin(t2), max2 = GetMax(t2);

            return min1.X <= max2.X && max1.X >= min2.X &&
                   min1.Y <= max2.Y && max1.Y >= min2.Y &&
                   min1.Z <= max2.Z && max1.Z >= min2.Z;
        }

        private static CVector3 GetMin(CGeosetTriangle t)
        {
            return CVector3.Min(
                CVector3.Min(t.Vertex1.Object.Position, t.Vertex2.Object.Position),
                t.Vertex3.Object.Position
            );
        }

        private static CVector3 GetMax(CGeosetTriangle t)
        {
            return CVector3.Max(
                CVector3.Max(t.Vertex1.Object.Position, t.Vertex2.Object.Position),
                t.Vertex3.Object.Position
            );
        }

        // Assuming CGeosetTriangle has properties Vertex1, Vertex2, Vertex3 of type Vertex
         

        private static bool EdgesIntersect(CGeosetTriangle t1, CGeosetTriangle t2)
        {
            // Get the edges of the two faces (triangles)
            CVector3[] edges1 = GetEdges(t1);
            CVector3[] edges2 = GetEdges(t2);

            // Compare each edge of the first triangle with each edge of the second triangle
            foreach (var e1 in edges1)
            {
                foreach (var e2 in edges2)
                {
                    if (EdgeIntersectsEdge(e1, e2))
                        return true; // Return true if any pair of edges intersect
                }
            }

            return false; // Return false if no edges intersect
        }

        // Function to check if two edges intersect
        private static bool EdgeIntersectsEdge(CVector3 e1, CVector3 e2)
        {
            // Edge intersection logic (you can implement this as described earlier)
            // Return true if edges intersect, otherwise false
            // For now, assuming a placeholder
            return false;
        }


        private static CVector3[] GetEdges(CGeosetTriangle t)
        {
            return new CVector3[]
            {
            t.Vertex2.Object.Position - t.Vertex1.Object.Position,
            t.Vertex3.Object.Position - t.Vertex2.Object.Position,
            t.Vertex1.Object.Position - t.Vertex3.Object.Position
            };
        }

        private static bool EdgeIntersectsEdge(CVector3 p1, CVector3 p2, CVector3 q1, CVector3 q2)
        {
            // Vectors for edges
            CVector3 u = p2 - p1; // Edge 1 direction
            CVector3 v = q2 - q1; // Edge 2 direction
            CVector3 w = p1 - q1; // Vector between starting points

            float a = CVector3.Dot(u, u); // u·u
            float b = CVector3.Dot(u, v); // u·v
            float c = CVector3.Dot(v, v); // v·v
            float d = CVector3.Dot(u, w); // u·w
            float e = CVector3.Dot(v, w); // v·w

            float denominator = a * c - b * b; // Determinant of the coefficient matrix

            // Check if lines are parallel (denominator close to zero)
            if (Math.Abs(denominator) < 1e-6)
            {
                return false; // Edges are parallel and cannot intersect
            }

            // Solve for the line parameters (s, t) that minimize the distance
            float sNumerator = b * e - c * d;
            float tNumerator = a * e - b * d;

            float s = sNumerator / denominator;
            float t = tNumerator / denominator;

            // Check if the intersection point lies within both segments
            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                // Compute the closest points on both segments
                CVector3 closestPointOnEdge1 = p1 + s * u;
                CVector3 closestPointOnEdge2 = q1 + t * v;

                // Check if these points are the same (i.e., edges intersect)
                return CVector3.Distance(closestPointOnEdge1, closestPointOnEdge2) < 1e-6;
            }

            return false; // No intersection within the segment bounds
        }


        private static bool TriangleInsideTriangle(CGeosetTriangle inner, CGeosetTriangle outer)
        {
            return PointInsideTriangle(inner.Vertex1.Object.Position, outer) &&
                   PointInsideTriangle(inner.Vertex2.Object.Position, outer) &&
                   PointInsideTriangle(inner.Vertex3.Object.Position, outer);
        }

        private static bool PointInsideTriangle(CVector3 p, CGeosetTriangle t)
        {
            // Placeholder for Point-In-Triangle test logic
            return false;
        }
    }
}
