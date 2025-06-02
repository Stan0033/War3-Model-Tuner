using MdxLib.Model;
using MdxLib.Primitives;
using SharpGL;
 
using System;
using System.Collections.Generic;
 
using System.Linq;
using System.Numerics;
 
using Whim_GEometry_Editor.Misc;

namespace Wa3Tuner.Helper_Classes
{
    public class Ray
    {
        public Vector3 From, To;
        public Ray(Vector3[] v)
        {
            From = v[0];
            To = v[1];
        }
        public Ray() { From = new Vector3(); To = new Vector3(); }
        public Ray(Vector3 n, Vector3 f)
        {
            From = n; To = f;
        }

        internal void Negate()
        {
            From.X= -From.X;
            From.Y=-From.Y;
            From.Z=-From.Z;

            To.X= -To.X;
            To.Y= -To.Y;
            To.Z= -To.Z;
        }
    }

    public static class RayCaster
    {
        public static float MousePickRadius = 1f;
        internal static List<Ray> Lines = new();
        internal static List<Extent> Extents = new();

        public static Ray GetRay(
      float mouseX, float mouseY,
      float viewportWidth, float viewportHeight,
      OpenGL gl)
        {
            // Get matrices and viewport
            double[] modelview = new double[16];
            double[] projection = new double[16];
            int[] viewport = new int[4];

            gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, modelview);
            gl.GetDouble(OpenGL.GL_PROJECTION_MATRIX, projection);
            gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);

            // Invert Y because OpenGL's Y = bottom
            double realY = viewport[3] - mouseY;

            // Prepare near and far point coordinates
            double nearX = 0, nearY = 0, nearZ = 0;
            double farX = 0, farY = 0, farZ = 0;

            // Unproject near (winZ = 0)
            gl.UnProject(mouseX, realY, 0.0f, modelview, projection, viewport,
                ref nearX, ref nearY, ref nearZ);

            // Unproject far (winZ = 1)
            gl.UnProject(mouseX, realY, 1.0f, modelview, projection, viewport,
                ref farX, ref farY, ref farZ);

            Vector3 from = new Vector3((float)nearX, (float)nearY, (float)nearZ);
            Vector3 to = new Vector3((float)farX, (float)farY, (float)farZ);

            return new Ray(from, to);
        }
        public static bool VertexInsideRayRadius(Ray ray, CGeosetVertex vertex)
        {
            float radius = MousePickRadius;

            // Calculate the direction of the ray
            Vector3 direction = ray.To - ray.From;
            direction = Vector3.Normalize(direction); // Normalize the direction

            // The "extent" will follow the ray, we need to check if the vertex lies within this extent

            // We create a bounding box around the ray, extending `radius` in perpendicular directions
            // and along the ray direction.
            Vector3 toVertex = new Vector3(vertex.Position.X, vertex.Position.Y, vertex.Position.Z) - ray.From;

            // Project the vertex position onto the ray direction
            float projection = Vector3.Dot(toVertex, direction);

            // Get the closest point on the ray to the vertex
            Vector3 closestPoint = ray.From + direction * projection;

            // Calculate the distance from the vertex to the closest point on the ray
            float distance = (new Vector3(vertex.Position.X, vertex.Position.Y, vertex.Position.Z) - closestPoint).Length();

            // If the distance is within the radius, the vertex is "inside" the extent
            return distance <= radius;
        }





        public static bool VertexInsideSelectionExtent(List<Ray> rays, CGeosetVertex vertex)
        {
            // Create an Extent from the rays
            Extent extent = new Extent(rays);

            // Use the IsInside method of the Extent class to check if the vertex is inside
            return extent.IsInside(vertex);
        }






        internal static List<Ray> ExtentFromMouseSelectionRectangle(
       double x1, double x2, double y1, double y2,
       float viewportWidth, float viewportHeight,
       OpenGL gl)
        {
            // Get rays from the four corners of the selection rectangle
            Ray rayTopLeft = GetRay((float)x1, (float)y1, viewportWidth, viewportHeight, gl);
            Ray rayTopRight = GetRay((float)x2, (float)y1, viewportWidth, viewportHeight, gl);
            Ray rayBottomLeft = GetRay((float)x1, (float)y2, viewportWidth, viewportHeight, gl);
            Ray rayBottomRight = GetRay((float)x2, (float)y2, viewportWidth, viewportHeight, gl);
           //  RayCaster.Lines.Add(rayTopLeft);
          //  RayCaster.Lines.Add(rayTopRight);
         //  RayCaster.Lines.Add(rayBottomLeft);
         //  RayCaster.Lines.Add(rayBottomRight);

         return new List<Ray> { rayTopLeft, rayTopRight, rayBottomLeft, rayBottomRight };
        }



        internal static bool GeosetInsideExtent(CGeoset cGeoset, List<Ray> extent)
        {
            List<Vector3> vectors = new List<Vector3>() {
                extent[0].From, extent[0].To ,
                extent[1].From, extent[1].To ,
                extent[2].From, extent[2].To ,
                extent[3].From, extent[3].To
            };
           // if (cGeoset.isVisible) {cGeoset.isSelected = false; return false; }
            var ex = Calculator.GetExtent(cGeoset);
            return Calculator.ExtentsOverlap(ex, vectors);
        }

        internal static bool GeosetInsideRay(CGeoset cGeoset, Ray ray)
        {
            //if (cGeoset.isVisible == false) return false;
            var extent = Calculator.GetExtent(cGeoset.Vertices.Select(x => x.Position).ToList());
            return RayInsideExtent(extent, ray);
             
        }

        internal static bool TriangleInsideRayRadius(Ray ray, CGeosetTriangle triangle)
        {
            float radius = MousePickRadius;
            Vector3 from = ray.From;
            Vector3 to = ray.To;
            Vector3 dir = Vector3.Normalize(to - from);

            // Triangle vertices
            Vector3 v0 = new Vector3(triangle.Vertex1.Object.Position.X, triangle.Vertex1.Object.Position.Y, triangle.Vertex1.Object.Position.Z);
            Vector3 v1 = new Vector3(triangle.Vertex2.Object.Position.X, triangle.Vertex2.Object.Position.Y, triangle.Vertex2.Object.Position.Z);
            Vector3 v2 = new Vector3(triangle.Vertex3.Object.Position.X, triangle.Vertex3.Object.Position.Y, triangle.Vertex3.Object.Position.Z);

            // Compute triangle normal
            Vector3 edge1 = v1 - v0;
            Vector3 edge2 = v2 - v0;
            Vector3 normal = Vector3.Cross(edge1, edge2);

            // Avoid degenerate triangle
            if (normal.LengthSquared() < 1e-6f)
                return false;

            // Ray-triangle intersection using Möller–Trumbore algorithm
            Vector3 h = Vector3.Cross(dir, edge2);
            float a = Vector3.Dot(edge1, h);
            if (MathF.Abs(a) < 1e-6f)
                return false;

            float f = 1.0f / a;
            Vector3 s = from - v0;
            float u = f * Vector3.Dot(s, h);
            if (u < 0.0f || u > 1.0f)
                return false;

            Vector3 q = Vector3.Cross(s, edge1);
            float v = f * Vector3.Dot(dir, q);
            if (v < 0.0f || u + v > 1.0f)
                return false;

            float t = f * Vector3.Dot(edge2, q);
            if (t > 0)
            {
                // Ray intersects the triangle
                return true;
            }

            // Otherwise, check if ray comes close enough to the triangle
            // by computing the minimum distance from the ray to the triangle

            float distance = DistanceFromRayToTriangle(from, dir, v0, v1, v2);
            return distance <= radius;
        }
        private static float DistanceFromRayToTriangle(Vector3 rayOrigin, Vector3 rayDir, Vector3 v0, Vector3 v1, Vector3 v2)
        {
            // Sample a few points on the triangle and check the min distance to the ray
            Vector3[] samples = new Vector3[]
            {
        v0, v1, v2,
        (v0 + v1) * 0.5f,
        (v1 + v2) * 0.5f,
        (v2 + v0) * 0.5f,
        (v0 + v1 + v2) / 3f
            };

            float minDistance = float.MaxValue;

            foreach (var point in samples)
            {
                Vector3 toPoint = point - rayOrigin;
                float t = Vector3.Dot(toPoint, rayDir);
                Vector3 closest = rayOrigin + rayDir * t;
                float dist = (point - closest).Length();

                if (dist < minDistance)
                    minDistance = dist;
            }

            return minDistance;
        }


        private static bool RayInsideExtent(CExtent extent, Ray ray)
        {
            Extent e = new Extent(extent);

            Vector3 dir = ray.To - ray.From;
            Vector3 invDir = new Vector3(
                dir.X != 0 ? 1f / dir.X : float.PositiveInfinity,
                dir.Y != 0 ? 1f / dir.Y : float.PositiveInfinity,
                dir.Z != 0 ? 1f / dir.Z : float.PositiveInfinity
            );

            Vector3 origin = ray.From;

            float t1 = (e.minX - origin.X) * invDir.X;
            float t2 = (e.maxX - origin.X) * invDir.X;
            float t3 = (e.minY - origin.Y) * invDir.Y;
            float t4 = (e.maxY - origin.Y) * invDir.Y;
            float t5 = (e.minZ - origin.Z) * invDir.Z;
            float t6 = (e.maxZ - origin.Z) * invDir.Z;

            float tmin = MathF.Max(MathF.Max(MathF.Min(t1, t2), MathF.Min(t3, t4)), MathF.Min(t5, t6));
            float tmax = MathF.Min(MathF.Min(MathF.Max(t1, t2), MathF.Max(t3, t4)), MathF.Max(t5, t6));

            // If tmax < 0, ray is intersecting AABB but the whole AABB is behind us
            if (tmax < 0)
                return false;

            // If tmin > tmax, ray doesn't intersect AABB
            if (tmin > tmax)
                return false;

            // Optional: if you want to restrict to segment only (From-To, not infinite ray)
            float rayLength = (ray.To - ray.From).Length();
            return tmin <= rayLength && tmax >= 0;
        }

    }




}
