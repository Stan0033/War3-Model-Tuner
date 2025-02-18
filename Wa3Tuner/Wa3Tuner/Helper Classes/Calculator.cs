using MdxLib.Animator;
using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using static Wa3Tuner.MainWindow;
namespace Wa3Tuner
{
  

    public static class Calculator
    {
        public static int ChangeWithPercentage(int min, int max, int current, bool increase, int percentage)
        {
            // Calculate the change in value based on the percentage
            int change = (current * percentage) / 100;

            // If increasing, add the change to the current value
            if (increase)
            {
                current += change;
            }
            else
            {
                // If decreasing, subtract the change from the current value
                current -= change;
            }

            // Ensure the new value is within the specified range
            current = Math.Max(min, current);
            current = Math.Min(max, current);

            return current;
        }

        public static bool VertexWithinExtent(CVector3 Position, CVector3 MinExtent, CVector3 MaxExtent)
        {
            return Position.X >= MinExtent.X && Position.X <= MaxExtent.X &&
                   Position.Y >= MinExtent.Y && Position.Y <= MaxExtent.Y &&
                   Position.Z >= MinExtent.Z && Position.Z <= MaxExtent.Z;
        }
        public static CVector3 GetCentroidOfGeoset(CGeoset geoset)
        {
            // Ensure the geoset has vertices
            if (geoset.Vertices == null || geoset.Vertices.Count == 0)
                return new CVector3(0, 0, 0);
            // Sum all vertex positions
            float sumX = 0, sumY = 0, sumZ = 0;
            foreach (var vertex in geoset.Vertices)
            {
                sumX += vertex.Position.X;
                sumY += vertex.Position.Y;
                sumZ += vertex.Position.Z;
            }
            // Calculate the average
            int count = geoset.Vertices.Count;
            return new CVector3(sumX / count, sumY / count, sumZ / count);
        }
        internal static void ClampQuaternion(CAnimatorNode<CVector4> item)
        {
            item.Value = ClampQuaternionValue(item.Value);
            item.InTangent = ClampQuaternionValue(item.InTangent);
            item.OutTangent = ClampQuaternionValue(item.OutTangent);

        }

        private static CVector4 ClampQuaternionValue(CVector4 vector)
        {
            if 
                (vector.X < -1 || vector.X > 1 ||
                 vector.Y < -1 || vector.Y > 1 ||
                 vector.Z < -1 || vector.Z > 1 ||
                vector.W < -1 || vector.W > 1

                 ){
                return new CVector4(0,0,0,1);
            }
            return   vector;
        }

        public static bool IsValidQuaternion(float w, float x, float y, float z)
        {
            // Calculate the magnitude of the quaternion
            float magnitudeSquared = w * w + x * x + y * y + z * z;
            // Check if the magnitude is approximately 1 (allowing for floating-point errors)
            const float epsilon = 1e-6f; // Tolerance for floating-point comparison
            return Math.Abs(magnitudeSquared - 1.0f) <= epsilon;
        }
        internal static CVector2 ClampUV(CVector2 texturePosition)
        {
            float X = texturePosition.X;
            float Y = texturePosition.Y;

            // Process X coordinate
            if (X > 1)
            {
                int intPart = (int)X;
                if (X == intPart)
                {
                    // If X is an even integer, set it to 0, otherwise set it to 1
                    X = (intPart % 2 == 0) ? 0 : 1;
                }
                else
                {
                    // If X is not an integer, remove integer part
                    X = X - intPart;
                }
            }
            else if (X < 0)
            {
                X = X - (int)X + 1; // Ensuring it wraps correctly to [0,1]
            }

            // Process Y coordinate
            if (Y > 1)
            {
                int intPart = (int)Y;
                if (Y == intPart)
                {
                    // If Y is an even integer, set it to 0, otherwise set it to 1
                    Y = (intPart % 2 == 0) ? 0 : 1;
                }
                else
                {
                    // If Y is not an integer, remove integer part
                    Y = Y - intPart;
                }
            }
            else if (Y < 0)
            {
                Y = Y - (int)Y + 1; // Ensuring it wraps correctly to [0,1]
            }

            return new CVector2(X, Y);
        }

        internal static float ClampNormalized(float value)
        {
            if (value >= 0 && value <= 999999) { return value; }
            else if (value < 0) { return 0; }
            else { return 999999; }
        }
        internal static float ClampVisibility(float value)
        {
            if (value < 0) { return 0; }
        if (value > 1) { return 1; }
            return value;
        }
        internal static CAnimatorNode<float> ClampNormalized(CAnimatorNode<float> item)
        {
            if (item.Value < 0)
            {
                return new CAnimatorNode<float>(item.Time, 0);
            }
            if (item.Value > 1)
            {
                return new CAnimatorNode<float>(item.Time, 1);
            }
            return item;
        }
        internal static CVector3 ClampVector3(CVector3 value)
        {
            float x = value.X;
            float y = value.Y;
            float z = value.Z;
            if (value.X < 0) { x = 0; }
            if (value.X > 1) { x = 1; }
            if (value.Y < 0) { y = 0; }
            if (value.Y > 1) { y = 1; }
            if (value.Z < 0) { z = 0; }
            if (value.Z > 1) { z = 1; }
            return new CVector3(x, y, z);
        }
        internal static CAnimatorNode<CVector3> ClampVector3(CAnimatorNode<CVector3> item)
        {
            CVector3 value = ClampVector3(item.Value);
            return new CAnimatorNode<CVector3>(item.Time, value);
        }
        internal static CAnimatorNode<float> ClampVisibility(CAnimatorNode<float> cAnimatorNode)
        {
            throw new NotImplementedException();
        }
        internal static int ClampInt(int v)
        {
            return v >= 0 ? v : 0;
        }
        internal static CAnimatorNode<int> ClampInt(CAnimatorNode<int> v)
        {
            return new CAnimatorNode<int>(v.Time, v.Value < 0 ? 0 : v.Value);
        }
        internal static float ClampFloat(float v)
        {
            return v < 0 ? 0 : v;
        }
        internal static CAnimatorNode<float> ClampFloat(CAnimatorNode<float> v)
        {
            return new CAnimatorNode<float>(v.Time, v.Value < 0 ? 9 : v.Value);
        }
        
        private class Vector3
        {
            public float X, Y, Z = 0;
            public Vector3(float x, float y, float z) { X = x; Y = y; Z = z; }
        }
        internal static CExtent GetExent(List<CVector3> vectors)
        {
            if (vectors == null || vectors.Count == 0)
            {
                throw new ArgumentException("The list of vectors cannot be null or empty.");
            }
            // Temporary holders for min and max
            Vector3 min = new Vector3(vectors[0].X, vectors[0].Y, vectors[0].Z);
            Vector3 max = new Vector3(vectors[0].X, vectors[0].Y, vectors[0].Z);
            // Iterate through the vectors to find min and max values
            foreach (CVector3 vector in vectors)
            {
                if (vector.X < min.X) min.X = vector.X;
                if (vector.Y < min.Y) min.Y = vector.Y;
                if (vector.Z < min.Z) min.Z = vector.Z;
                if (vector.X > max.X) max.X = vector.X;
                if (vector.Y > max.Y) max.Y = vector.Y;
                if (vector.Z > max.Z) max.Z = vector.Z;
            }
            // Calculate the center of the extent
            CVector3 center = new CVector3(
                (min.X + max.X) / 2,
                (min.Y + max.Y) / 2,
                (min.Z + max.Z) / 2
            );
            // Calculate the radius as the maximum distance from the center to any vector
            float radius = 0;
            foreach (CVector3 vector in vectors)
            {
                float distanceSquared =
                    (vector.X - center.X) * (vector.X - center.X) +
                    (vector.Y - center.Y) * (vector.Y - center.Y) +
                    (vector.Z - center.Z) * (vector.Z - center.Z);
                radius = Math.Max(radius, (float)Math.Sqrt(distanceSquared));
            }
            // Finalize the CExtent
            CVector3 extentMin = new CVector3(min.X, min.Y, min.Z);
            CVector3 extentMax = new CVector3(max.X, max.Y, max.Z);
            CExtent extent = new CExtent(extentMin, extentMax, radius);
            return extent;
        }
        internal static CExtent CalculateModelExtent(List<CExtent> extents)
        {
            if (extents == null || extents.Count == 0)
            {
                return new CExtent();
            }
            // Temporary min and max holders initialized with the first extent's bounds
            Vector3 min = new Vector3(extents[0].Min.X, extents[0].Min.Y, extents[0].Min.Z);
            Vector3 max = new Vector3(extents[0].Max.X, extents[0].Max.Y, extents[0].Max.Z);
            // Iterate through the extents to find the overall min and max values
            foreach (CExtent extent in extents)
            {
                if (extent.Min.X < min.X) min.X = extent.Min.X;
                if (extent.Min.Y < min.Y) min.Y = extent.Min.Y;
                if (extent.Min.Z < min.Z) min.Z = extent.Min.Z;
                if (extent.Max.X > max.X) max.X = extent.Max.X;
                if (extent.Max.Y > max.Y) max.Y = extent.Max.Y;
                if (extent.Max.Z > max.Z) max.Z = extent.Max.Z;
            }
            // Calculate the center of the overall extent
            CVector3 center = new CVector3(
                (min.X + max.X) / 2,
                (min.Y + max.Y) / 2,
                (min.Z + max.Z) / 2
            );
            // Calculate the radius as the maximum distance from the center to any extent's Min or Max
            float radius = 0;
            foreach (CExtent extent in extents)
            {
                // Calculate distance to the extent's Max
                float maxDistanceSquared =
                    (extent.Max.X - center.X) * (extent.Max.X - center.X) +
                    (extent.Max.Y - center.Y) * (extent.Max.Y - center.Y) +
                    (extent.Max.Z - center.Z) * (extent.Max.Z - center.Z);
                // Calculate distance to the extent's Min
                float minDistanceSquared =
                    (extent.Min.X - center.X) * (extent.Min.X - center.X) +
                    (extent.Min.Y - center.Y) * (extent.Min.Y - center.Y) +
                    (extent.Min.Z - center.Z) * (extent.Min.Z - center.Z);
                // Update the radius
                radius = Math.Max(radius, (float)Math.Sqrt(maxDistanceSquared));
                radius = Math.Max(radius, (float)Math.Sqrt(minDistanceSquared));
            }
            // Convert temporary min and max back to immutable CVector3
            CVector3 finalMin = new CVector3(min.X, min.Y, min.Z);
            CVector3 finalMax = new CVector3(max.X, max.Y, max.Z);
            // Return the final extent
            return new CExtent(finalMin, finalMax, radius);
        }
        internal static void PutOnGround(CGeoset geoset)
        {
            // Find the lowest Z value among all vertices
            float closestVertexZToGround = FindClosestZToGround(geoset);
            // Move all vertices down by the closest Z value to align to the ground
            foreach (CGeosetVertex vertex in geoset.Vertices)
            {
                float x = vertex.Position.X;
                float y = vertex.Position.Y;
                float z = vertex.Position.Z;
                vertex.Position = new CVector3(x, y, z - closestVertexZToGround);
            }
        }
        private static float FindClosestZToGround(CGeoset geoset)
        {
            float closest = float.MaxValue;
            foreach (CGeosetVertex vertex in geoset.Vertices)
            {
                if (Math.Abs(vertex.Position.Z) < Math.Abs(closest) ||
                   (Math.Abs(vertex.Position.Z) == Math.Abs(closest) && vertex.Position.Z > closest))
                {
                    closest = vertex.Position.Z;
                }
            }
            return closest;
        }
        internal static void PutOnGroundTogether(List<CGeoset> all)
        {
            if (all == null || all.Count == 0)
                return;
            // Find the lowest Z-value across all geosets
            float lowestZ = float.MaxValue;
            foreach (var geoset in all)
            {
                foreach (var vertex in geoset.Vertices)
                {
                    if (vertex.Position.Z < lowestZ)
                    {
                        lowestZ = vertex.Position.Z;
                    }
                }
            }
            // Move all geosets by the lowest Z-value to align to the ground
            foreach (var geoset in all)
            {
                foreach (var vertex in geoset.Vertices)
                {
                    vertex.Position = new CVector3(
                        vertex.Position.X,
                        vertex.Position.Y,
                        vertex.Position.Z - lowestZ
                    );
                }
            }
        }
        internal static void CenterGeoset(CGeoset geoset, bool onX, bool onY, bool onZ)
        {
            // Calculate the centroid of the geoset
            CVector3 centroid = GetCentroidOfGeoset(geoset);
            // Adjust each vertex position to center the geoset on the specified axes
            foreach (CGeosetVertex vertex in geoset.Vertices)
            {
                float x = vertex.Position.X - (onX ? centroid.X : 0);
                float y = vertex.Position.Y - (onY ? centroid.Y : 0);
                float z = vertex.Position.Z - (onZ ? centroid.Z : 0);
                vertex.Position = new CVector3(x, y, z);
            }
        }
        internal static void CenterGeoset(CGeoset geoset, float X, float Y, float Z)
        {
            // Calculate the centroid of the geoset
            CVector3 centroid = GetCentroidOfGeoset(geoset);
            // Calculate the offset needed to move the centroid to the target position (X, Y, Z)
            float offsetX = X - centroid.X;
            float offsetY = Y - centroid.Y;
            float offsetZ = Z - centroid.Z;
            // Adjust each vertex position
            foreach (CGeosetVertex vertex in geoset.Vertices)
            {
                // Move each vertex by the calculated offset
                vertex.Position = new CVector3(
                    vertex.Position.X + offsetX,
                    vertex.Position.Y + offsetY,
                    vertex.Position.Z + offsetZ
                );
            }
        }
        internal static void RotateGeoset(CGeoset geoset, float x, float y, float z)
        {
            // Convert angles from degrees to radians
            float radX = (float)(x * Math.PI / 180.0);
            float radY = (float)(y * Math.PI / 180.0);
            float radZ = (float)(z * Math.PI / 180.0);
            // Precompute trigonometric values for each axis
            float cosX = (float)Math.Cos(radX), sinX = (float)Math.Sin(radX);
            float cosY = (float)Math.Cos(radY), sinY = (float)Math.Sin(radY);
            float cosZ = (float)Math.Cos(radZ), sinZ = (float)Math.Sin(radZ);
            foreach (CGeosetVertex vertex in geoset.Vertices)
            {
                // Original vertex coordinates
                float vx = vertex.Position.X;
                float vy = vertex.Position.Y;
                float vz = vertex.Position.Z;
                // Rotate around X-axis
                float ry = cosX * vy - sinX * vz;
                float rz = sinX * vy + cosX * vz;
                vy = ry;
                vz = rz;
                // Rotate around Y-axis
                float rx = cosY * vx + sinY * vz;
                rz = -sinY * vx + cosY * vz;
                vx = rx;
                vz = rz;
                // Rotate around Z-axis
                rx = cosZ * vx - sinZ * vy;
                ry = sinZ * vx + cosZ * vy;
                vx = rx;
                vy = ry;
                // Update the vertex position
                vertex.Position = new CVector3(vx, vy, vz);
            }
        }
        internal static void AlignGeosets(List<CGeoset> geosets, bool onX, bool onY, bool onZ)
        {
            if (geosets == null || geosets.Count == 0)
                return;
            // Get the centroid of the first geoset
            CVector3 firstCentroid = GetCentroidOfGeoset(geosets[0]);
            for (int i = 1; i < geosets.Count; i++)
            {
                CGeoset current = geosets[i];
                CVector3 currentCentroid = GetCentroidOfGeoset(current);
                // Calculate the difference in position along each axis
                float dx = onX ? (firstCentroid.X - currentCentroid.X) : 0;
                float dy = onY ? (firstCentroid.Y - currentCentroid.Y) : 0;
                float dz = onZ ? (firstCentroid.Z - currentCentroid.Z) : 0;
                // Adjust each vertex in the current geoset
                foreach (CGeosetVertex vertex in current.Vertices)
                {
                    vertex.Position = new CVector3(
                        vertex.Position.X + dx,
                        vertex.Position.Y + dy,
                        vertex.Position.Z + dz
                    );
                }
            }
        }
        internal static void PullTogether(List<CGeoset> geosets, bool onX, bool onY, bool onZ)
        {
            if (geosets.Count != 2 || (new[] { onX, onY, onZ }.Count(flag => flag) != 1))
                return;
            // Get the centroids of both geosets
            CVector3 centroid1 = GetCentroidOfGeoset(geosets[0]);
            CVector3 centroid2 = GetCentroidOfGeoset(geosets[1]);
            // Calculate the difference between the centroids on the selected axis
            float dx = onX ? centroid2.X - centroid1.X : 0;
            float dy = onY ? centroid2.Y - centroid1.Y : 0;
            float dz = onZ ? centroid2.Z - centroid1.Z : 0;
            // Adjust the vertices of the second geoset to align with the first geoset
            foreach (CGeosetVertex vertex in geosets[1].Vertices)
            {
                vertex.Position = new CVector3(
                    vertex.Position.X - dx,
                    vertex.Position.Y - dy,
                    vertex.Position.Z - dz
                );
            }
        }
        private static CSequence DuplicatingFromSequence;
        private static CSequence DuplicatingToSequence;
        internal static void DuplicateSequence(CSequence sequence, CSequence ToSequence, CModel Model)
        {
            DuplicatingFromSequence = sequence;
            DuplicatingToSequence = ToSequence;
            foreach (INode node in Model.Nodes)
            {
                DuplicateSequenceKeyframes(node.Translation);
                DuplicateSequenceKeyframes(node.Rotation);
                DuplicateSequenceKeyframes(node.Scaling);
                if (node is CAttachment)
                {
                    CAttachment element = (CAttachment)node;
                    DuplicateSequenceKeyframes(element.Visibility);
                }
                if (node is CParticleEmitter)
                {
                    CParticleEmitter element = (CParticleEmitter)node;
                    DuplicateSequenceKeyframes(element.Visibility);
                    DuplicateSequenceKeyframes(element.EmissionRate);
                    DuplicateSequenceKeyframes(element.LifeSpan);
                    DuplicateSequenceKeyframes(element.InitialVelocity);
                    DuplicateSequenceKeyframes(element.Gravity);
                    DuplicateSequenceKeyframes(element.Longitude);
                    DuplicateSequenceKeyframes(element.Latitude);
                }
                if (node is CParticleEmitter2)
                {
                    CParticleEmitter2 element = (CParticleEmitter2)node;
                    DuplicateSequenceKeyframes(element.Visibility);
                    DuplicateSequenceKeyframes(element.EmissionRate);
                    DuplicateSequenceKeyframes(element.Speed);
                    DuplicateSequenceKeyframes(element.Width);
                    DuplicateSequenceKeyframes(element.Gravity);
                    DuplicateSequenceKeyframes(element.Length);
                    DuplicateSequenceKeyframes(element.Latitude);
                    DuplicateSequenceKeyframes(element.Variation);
                }
                if (node is CRibbonEmitter)
                {
                    CRibbonEmitter element = (CRibbonEmitter)node;
                    DuplicateSequenceKeyframes(element.Visibility);
                    DuplicateSequenceKeyframes(element.HeightAbove);
                    DuplicateSequenceKeyframes(element.HeightBelow);
                    DuplicateSequenceKeyframes(element.Color);
                    DuplicateSequenceKeyframes(element.Alpha);
                    DuplicateSequenceKeyframes(element.TextureSlot);
                }
                if (node is CLight)
                {
                    CLight element = (CLight)node;
                    DuplicateSequenceKeyframes(element.Visibility);
                    DuplicateSequenceKeyframes(element.Color);
                    DuplicateSequenceKeyframes(element.AmbientColor);
                    DuplicateSequenceKeyframes(element.Intensity);
                    DuplicateSequenceKeyframes(element.AmbientIntensity);
                    DuplicateSequenceKeyframes(element.AttenuationEnd);
                    DuplicateSequenceKeyframes(element.AttenuationStart);
                }
            }
            foreach (CGeosetAnimation ga in Model.GeosetAnimations)
            {
                if (ga.Alpha.Static == false)
                {
                    DuplicateSequenceKeyframes(ga.Alpha);
                }
                if (ga.Color.Static == false) { DuplicateSequenceKeyframes(ga.Color); }
            }
            foreach (CTextureAnimation taa in Model.TextureAnimations)
            {
                DuplicateSequenceKeyframes(taa.Translation);
                DuplicateSequenceKeyframes(taa.Rotation);
                DuplicateSequenceKeyframes(taa.Scaling);
            }
            foreach (CMaterial material in Model.Materials)
            {
                foreach (CMaterialLayer layer in material.Layers)
                {
                    DuplicateSequenceKeyframes(layer.Alpha);
                    DuplicateSequenceKeyframes(layer.TextureId);
                }
            }
            foreach (CCamera cam in Model.Cameras)
            {
                DuplicateSequenceKeyframes(cam.Rotation);
            }
        }
        private static void DuplicateSequenceKeyframes(CAnimator<int> animator)
        {
            throw new NotImplementedException();
        }
        private static void DuplicateSequenceKeyframes(CAnimator<float> animator)
        {
            throw new NotImplementedException();
        }
        private static void DuplicateSequenceKeyframes(CAnimator<CVector3> animator)
        {
            throw new NotImplementedException();
        }
        private static void DuplicateSequenceKeyframes(CAnimator<CVector4> animator)
        {
            throw new NotImplementedException();
        }
        internal static void AverageNormals(CGeoset geoset)
        {
            // Dictionary to store accumulated normals for each vertex
            Dictionary<CGeosetVertex, NormalData> normalSums = new Dictionary<CGeosetVertex, NormalData>();
            // Step 1: Accumulate face normals for each vertex
            foreach (CGeosetTriangle face in geoset.Triangles)
            {
                // Get vertices for the current face
                CGeosetVertex vertex1 = face.Vertex1.Object;
                CGeosetVertex vertex2 = face.Vertex2.Object;
                CGeosetVertex vertex3 = face.Vertex3.Object;
                // Calculate the normal for the face using cross product of two edges
                CVector3 edge1 = new CVector3(
                    vertex2.Position.X - vertex1.Position.X,
                    vertex2.Position.Y - vertex1.Position.Y,
                    vertex2.Position.Z - vertex1.Position.Z
                );
                CVector3 edge2 = new CVector3(
                    vertex3.Position.X - vertex1.Position.X,
                    vertex3.Position.Y - vertex1.Position.Y,
                    vertex3.Position.Z - vertex1.Position.Z
                );
                // Cross product
                CVector3 faceNormal = new CVector3(
                    edge1.Y * edge2.Z - edge1.Z * edge2.Y,
                    edge1.Z * edge2.X - edge1.X * edge2.Z,
                    edge1.X * edge2.Y - edge1.Y * edge2.X
                );
                // Normalize the face normal
                float length = (float)Math.Sqrt(faceNormal.X * faceNormal.X + faceNormal.Y * faceNormal.Y + faceNormal.Z * faceNormal.Z);
                faceNormal = new CVector3(faceNormal.X / length, faceNormal.Y / length, faceNormal.Z / length);
                // Add the face normal to each vertex of the face
                AddNormal(normalSums, vertex1, faceNormal);
                AddNormal(normalSums, vertex2, faceNormal);
                AddNormal(normalSums, vertex3, faceNormal);
            }
            // Step 2: Compute the average normal for each vertex
            foreach (var kvp in normalSums)
            {
                CGeosetVertex vertex = kvp.Key;
                NormalData data = kvp.Value;
                // Compute average and normalize
                float avgX = data.X / data.Count, avgY = data.Y / data.Count, avgZ = data.Z / data.Count;
                float length = (float)Math.Sqrt(avgX * avgX + avgY * avgY + avgZ * avgZ);
                vertex.Normal = new CVector3(avgX / length, avgY / length, avgZ / length);
            }
        }
        // Struct to hold accumulated normals and counts
        private struct NormalData
        {
            public float X;
            public float Y;
            public float Z;
            public int Count;
            public NormalData(float x, float y, float z, int count)
            {
                X = x;
                Y = y;
                Z = z;
                Count = count;
            }
        }
        // Helper method to add a face normal to a vertex
        private static void AddNormal(Dictionary<CGeosetVertex, NormalData> normalSums, CGeosetVertex vertex, CVector3 faceNormal)
        {
            if (!normalSums.ContainsKey(vertex))
            {
                normalSums[vertex] = new NormalData(0, 0, 0, 0);
            }
            NormalData current = normalSums[vertex];
            normalSums[vertex] = new NormalData(
                current.X + faceNormal.X,
                current.Y + faceNormal.Y,
                current.Z + faceNormal.Z,
                current.Count + 1
            );
        }
        public static void MoveBoneAccordingToCentroid(CBone bone, CVector3 vector)
        {
        }
        // Create a custom struct to hold the bounds of the geoset
        public struct GeosetBounds
        {
            public CVector3 Min;
            public CVector3 Max;
            public GeosetBounds(CVector3 min, CVector3 max)
            {
                Min = min;
                Max = max;
            }
        }
        private static CVector3 CalculateCentroids(List<CGeoset> geosets)
        {
            List<CVector3> centroids = new List<CVector3>();
            foreach (CGeoset geoset in geosets)
            {
                centroids.Add(GetCentroidOfGeoset(geoset));
            }
            // Calculate the centroid of all centroids
            float x = 0, y = 0, z = 0;
            foreach (CVector3 centroid in centroids)
            {
                x += centroid.X;
                y += centroid.Y;
                z += centroid.Z;
            }
            int count = centroids.Count;
            if (count == 0)
                return new CVector3(0, 0, 0); // Return zero vector if no centroids exist
            return new CVector3(x / count, y / count, z / count);
        }
        internal static void ScaleToFitInExtent(CExtent extent, List<CGeoset> geosets)
        {
            // Get the centroid of the extent
            CVector3 centroid_extent = GetCentroidOfExtent(extent);
            // Calculate the centroid of all the geosets combined
            CVector3 centroid_geosets = CalculateCentroids(geosets);
            // Calculate the overall bounds of all geosets
            GeosetBounds combinedBounds = GetCombinedBounds(geosets);
            // Calculate the extents of the target extent
            CVector3 min_extent = extent.Min;
            CVector3 max_extent = extent.Max;
            // Calculate the scaling factor for each axis based on combined bounds
            float scale_x = (max_extent.X - min_extent.X) / (combinedBounds.Max.X - combinedBounds.Min.X);
            float scale_y = (max_extent.Y - min_extent.Y) / (combinedBounds.Max.Y - combinedBounds.Min.Y);
            float scale_z = (max_extent.Z - min_extent.Z) / (combinedBounds.Max.Z - combinedBounds.Min.Z);
            // Use the smallest scaling factor to maintain aspect ratio
            float scale = Math.Min(scale_x, Math.Min(scale_y, scale_z));
            // Adjust each geoset's vertices
            foreach (CGeoset geoset in geosets)
            {
                foreach (CGeosetVertex vertex in geoset.Vertices)
                {
                    // Translate to the combined centroid of the geosets
                    vertex.Position = new CVector3(
                        vertex.Position.X - centroid_geosets.X,
                        vertex.Position.Y - centroid_geosets.Y,
                        vertex.Position.Z - centroid_geosets.Z
                    );
                    // Scale the vertex
                    vertex.Position = new CVector3(
                        vertex.Position.X * scale,
                        vertex.Position.Y * scale,
                        vertex.Position.Z * scale
                    );
                    // Translate to the target centroid of the extent
                    vertex.Position = new CVector3(
                        vertex.Position.X + centroid_extent.X,
                        vertex.Position.Y + centroid_extent.Y,
                        vertex.Position.Z + centroid_extent.Z
                    );
                }
            }
        }
        private static GeosetBounds GetCombinedBounds(List<CGeoset> geosets)
        {
            CVector3 min = new CVector3(float.MaxValue, float.MaxValue, float.MaxValue);
            CVector3 max = new CVector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (CGeoset geoset in geosets)
            {
                GeosetBounds bounds = GetGeosetBounds(geoset);
                min = new CVector3(
                    Math.Min(min.X, bounds.Min.X),
                    Math.Min(min.Y, bounds.Min.Y),
                    Math.Min(min.Z, bounds.Min.Z)
                );
                max = new CVector3(
                    Math.Max(max.X, bounds.Max.X),
                    Math.Max(max.Y, bounds.Max.Y),
                    Math.Max(max.Z, bounds.Max.Z)
                );
            }
            return new GeosetBounds(min, max);
        }
        internal static void ScaleToFitInExtent(CExtent extent, CGeoset geoset)
        {
            // Calculate the centroids
            CVector3 centroid_geoset = GetCentroidOfGeoset(geoset);
            CVector3 centroid_extent = GetCentroidOfExtent(extent);
            // Calculate the extents of the geoset
            GeosetBounds geosetBounds = GetGeosetBounds(geoset);
            // Calculate the extents of the target extent
            CVector3 min_extent = extent.Min;
            CVector3 max_extent = extent.Max;
            // Calculate the scaling factor for each axis
            float scale_x = (max_extent.X - min_extent.X) / (geosetBounds.Max.X - geosetBounds.Min.X);
            float scale_y = (max_extent.Y - min_extent.Y) / (geosetBounds.Max.Y - geosetBounds.Min.Y);
            float scale_z = (max_extent.Z - min_extent.Z) / (geosetBounds.Max.Z - geosetBounds.Min.Z);
            // Use the smallest scaling factor to maintain aspect ratio
            float scale = Math.Min(scale_x, Math.Min(scale_y, scale_z));
            // Adjust each vertex
            foreach (CGeosetVertex vertex in geoset.Vertices)
            {
                // Translate to origin
                vertex.Position = new CVector3(
                    vertex.Position.X - centroid_geoset.X,
                    vertex.Position.Y - centroid_geoset.Y,
                    vertex.Position.Z - centroid_geoset.Z
                );
                // Scale
                vertex.Position = new CVector3(
                    vertex.Position.X * scale,
                    vertex.Position.Y * scale,
                    vertex.Position.Z * scale
                );
                // Translate to target centroid
                vertex.Position = new CVector3(
                    vertex.Position.X + centroid_extent.X,
                    vertex.Position.Y + centroid_extent.Y,
                    vertex.Position.Z + centroid_extent.Z
                );
            }
        }
        // Helper method to calculate the bounds of a geoset
        private static GeosetBounds GetGeosetBounds(CGeoset geoset)
        {
            CVector3 min = new CVector3(float.MaxValue, float.MaxValue, float.MaxValue);
            CVector3 max = new CVector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (CGeosetVertex vertex in geoset.Vertices)
            {
                min = new CVector3(
                    Math.Min(min.X, vertex.Position.X),
                    Math.Min(min.Y, vertex.Position.Y),
                    Math.Min(min.Z, vertex.Position.Z)
                );
                max = new CVector3(
                    Math.Max(max.X, vertex.Position.X),
                    Math.Max(max.Y, vertex.Position.Y),
                    Math.Max(max.Z, vertex.Position.Z)
                );
            }
            return new GeosetBounds(min, max);
        }
        private static CVector3 GetCentroidOfExtent(CExtent extent)
        {
            float x = (extent.Max.X - extent.Min.X) / 2;
            float y = (extent.Max.Y - extent.Min.Y) / 2;
            float z = (extent.Max.Z - extent.Min.Z) / 2;
            return new CVector3(x, y, z);
        }
        internal static CVector3 GetCentroidOfNodes(CNodeContainer nodes)
        {
            if (nodes == null || !nodes.Any())
                return new CVector3(0, 0, 0); // Return a zero vector if there are no nodes.
            // Calculate the sum of all pivot points.
            float totalX = 0, totalY = 0, totalZ = 0;
            foreach (var node in nodes)
            {
                totalX += node.PivotPoint.X;
                totalY += node.PivotPoint.Y;
                totalZ += node.PivotPoint.Z;
            }
            // Find the average for each component.
            int count = nodes.Count();
            float avgX = totalX / count;
            float avgY = totalY / count;
            float avgZ = totalZ / count;
            // Create a new CVector3 for the centroid.
            return new CVector3(avgX, avgY, avgZ);
        }
        public static string QuaternionToEuler_(CVector4 quaternion)
        {
            CVector3 q = QuaternionToEuler(quaternion);
            return $"{q.X}, {q.Y}, {q.Z}";
        }
            public static CVector3 QuaternionToEuler(CVector4 quaternion)
        {
            // Extract the components of the quaternion
            float x = quaternion.X;
            float y = quaternion.Y;
            float z = quaternion.Z;
            float w = quaternion.W;
            // Compute the Euler angles
            float sinRcosP = 2 * (w * x + y * z);
            float cosRcosP = 1 - 2 * (x * x + y * y);
            float roll = (float)Math.Atan2(sinRcosP, cosRcosP);
            float sinP = 2 * (w * y - z * x);
            float pitch;
            if (Math.Abs(sinP) >= 1)
                pitch = CopySign((float)Math.PI / 2, sinP); // Use 90 degrees if out of range
            else
                pitch = (float)Math.Asin(sinP);
            float sinYcosP = 2 * (w * z + x * y);
            float cosYcosP = 1 - 2 * (y * y + z * z);
            float yaw = (float)Math.Atan2(sinYcosP, cosYcosP);
            // Return the angles in radians as a CVector3
            return new CVector3(roll, pitch, yaw);
        }
        public static CVector4 EulerToQuaternion(CVector3 euler)
        {
            // Extract the Euler angles (in radians)
            float roll = euler.X;
            float pitch = euler.Y;
            float yaw = euler.Z;
            // Compute the half angles
            float halfRoll = roll / 2;
            float halfPitch = pitch / 2;
            float halfYaw = yaw / 2;
            // Calculate trigonometric values
            float sinRoll = (float)Math.Sin(halfRoll);
            float cosRoll = (float)Math.Cos(halfRoll);
            float sinPitch = (float)Math.Sin(halfPitch);
            float cosPitch = (float)Math.Cos(halfPitch);
            float sinYaw = (float)Math.Sin(halfYaw);
            float cosYaw = (float)Math.Cos(halfYaw);
            // Compute the quaternion components
            float w = cosRoll * cosPitch * cosYaw + sinRoll * sinPitch * sinYaw;
            float x = sinRoll * cosPitch * cosYaw - cosRoll * sinPitch * sinYaw;
            float y = cosRoll * sinPitch * cosYaw + sinRoll * cosPitch * sinYaw;
            float z = cosRoll * cosPitch * sinYaw - sinRoll * sinPitch * cosYaw;
            // Return the quaternion as a CVector4
            return new CVector4(x, y, z, w);
        }
        private static float CopySign(float value, float sign)
        {
            return Math.Sign(sign) == -1 ? -Math.Abs(value) : Math.Abs(value);
        }
        private static CVector4 ReverseVector4(CVector4 vector)
        {
            CVector3 euler = QuaternionToEuler(vector);
            CVector3 new_euler = new CVector3(-euler.X, -euler.Y, -euler.Z);
            return EulerToQuaternion(new_euler);
        }
        internal static CAnimatorNode<CVector4> ReverseVector4(CAnimatorNode<CVector4> cAnimatorNode)
        {
            CAnimatorNode<CVector4> node = new CAnimatorNode<CVector4>();
            int time = node.Time;
            CVector4 vector = node.Value;
            CVector4 new_vector = ReverseVector4(vector);
            return new CAnimatorNode<CVector4>(time, new_vector);
        }
        internal static float[] EulerToQuaternion(float x, float y, float z)
        {
            // Convert degrees to radians
            float cx = (float)Math.Cos(x * 0.5f);
            float cy = (float)Math.Cos(y * 0.5f);
            float cz = (float)Math.Cos(z * 0.5f);
            float sx = (float)Math.Sin(x * 0.5f);
            float sy = (float)Math.Sin(y * 0.5f);
            float sz = (float)Math.Sin(z * 0.5f);
            // Quaternion components
            float w = cx * cy * cz + sx * sy * sz;
            float qx = sx * cy * cz - cx * sy * sz;
            float qy = cx * sy * cz + sx * cy * sz;
            float qz = cx * cy * sz - sx * sy * cz;
            return new float[] { qx, qy, qz, w };
        }
        internal static float[] QuaternionToEuler(float x, float y, float z, float w)
        {
            // Roll (X axis rotation)
            float sinr_cosp = 2.0f * (w * x + y * z);
            float cosr_cosp = 1.0f - 2.0f * (x * x + y * y);
            float roll = (float)Math.Atan2(sinr_cosp, cosr_cosp);
            // Pitch (Y axis rotation)
            float sinp = 2.0f * (w * y - z * x);
            float pitch = Math.Abs(sinp) >= 1.0f ? (float)Math.Sign(sinp) * (float)Math.PI / 2.0f : (float)Math.Asin(sinp);
            // Yaw (Z axis rotation)
            float siny_cosp = 2.0f * (w * z + x * y);
            float cosy_cosp = 1.0f - 2.0f * (y * y + z * z);
            float yaw = (float)Math.Atan2(siny_cosp, cosy_cosp);
            return new float[] { roll, pitch, yaw };
        }
        internal static string BGRnToRGB(CVector3 v)
        {
            float r = v.Z * 255;
            float g = v.Y * 255;
            float b = v.X * 255;
            return $"{r}, {g}, {b}";
        }
        public static ImageSource ConvertBitmapToImageSource(Bitmap bitmap)
        {
            // Create a MemoryStream to hold the image data
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Save the Bitmap to the MemoryStream in PNG format
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                // Create a BitmapImage from the MemoryStream
                memoryStream.Seek(0, SeekOrigin.Begin);
                BitmapImage imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.StreamSource = memoryStream;
                imageSource.EndInit();
                return imageSource;
            }
        }
        private static Point3D GetMiddleVertex(CGeosetVertex one, CGeosetVertex two, CGeosetVertex three)
        {
            // Calculate the average of the X, Y, and Z coordinates
            double middleX = (one.Position.X + two.Position.X + three.Position.X) / 3.0;
            double middleY = (one.Position.Y + two.Position.Y + three.Position.Y) / 3.0;
            double middleZ = (one.Position.Z + two.Position.Z + three.Position.Z) / 3.0;
            // Return the calculated middle point as a Point3D
            return new Point3D(middleX, middleY, middleZ);
        }
        internal static void SubdivideGeoset(CGeoset geoset, CModel ParentModel)
        {
            foreach (var originalFace in geoset.Triangles.ToList())
            {
                // Create the 3 new faces
                CGeosetTriangle face1 = new CGeosetTriangle(ParentModel);
                CGeosetTriangle face2 = new CGeosetTriangle(ParentModel);
                CGeosetTriangle face3 = new CGeosetTriangle(ParentModel);
                // Get the middle vertex between the original triangle's vertices
                Point3D middle = GetMiddleVertex(originalFace.Vertex1.Object, originalFace.Vertex2.Object, originalFace.Vertex3.Object);
                CGeosetVertex middleVertex = new CGeosetVertex(ParentModel);
                middleVertex.Position = new CVector3((float)middle.X, (float)middle.Y, (float)middle.Z);
                // Set the texture coordinates and normals for the middle vertex
                middleVertex.TexturePosition = CalculateMiddleTexturePositon
                    (
                    originalFace.Vertex1.Object.TexturePosition,
                    originalFace.Vertex2.Object.TexturePosition,
                    originalFace.Vertex3.Object.TexturePosition 
                    );
                middleVertex.Normal = CalculateMiddleNormal(
                    originalFace.Vertex1.Object.Normal,
                    originalFace.Vertex2.Object.Normal,
                    originalFace.Vertex3.Object.Normal
                    ); 
                // Add the new middle vertex to the geoset
                geoset.Vertices.Add(middleVertex);
                // Create the three new faces
                face1.Vertex1.Attach(originalFace.Vertex1.Object);
                face1.Vertex2.Attach(originalFace.Vertex2.Object);
                face1.Vertex3.Attach(middleVertex);
                face2.Vertex1.Attach(originalFace.Vertex2.Object);
                face2.Vertex2.Attach(originalFace.Vertex3.Object);
                face2.Vertex3.Attach(middleVertex);
                face3.Vertex1.Attach(originalFace.Vertex3.Object);
                face3.Vertex2.Attach(originalFace.Vertex1.Object);
                face3.Vertex3.Attach(middleVertex);
                // Remove the original face and add the new faces
                geoset.Triangles.Remove(originalFace);
                geoset.Triangles.Add(face1);
                geoset.Triangles.Add(face2);
                geoset.Triangles.Add(face3);
            }
        }
        private static CVector3 CalculateMiddleNormal(CVector3 normal1, CVector3 normal2, CVector3 normal3)
        {
            // Average the X, Y, and Z components of the normals
            float x = (normal1.X + normal2.X + normal3.X) / 3f;
            float y = (normal1.Y + normal2.Y + normal3.Y) / 3f;
            float z = (normal1.Z + normal2.Z + normal3.Z) / 3f;
            // Return a new CVector3 with the averaged normal components
            return new CVector3(x, y, z);
        }
        private static CVector2 CalculateMiddleTexturePositon(CVector2 texturePosition1, CVector2 texturePosition2, CVector2 texturePosition3)
        {
            // Average the X and Y components of the texture positions
            float x = (texturePosition1.X + texturePosition2.X + texturePosition3.X) / 3f;
            float y = (texturePosition1.Y + texturePosition2.Y + texturePosition3.Y) / 3f;
            // Return a new CVector2 with the averaged texture coordinates
            return new CVector2(x, y);
        }
        internal static CGeoset CreateCube(CModel ModelOwner)
        {
            CGeoset geoset = new CGeoset(ModelOwner);
            // Create the 8 vertices of the cube
            CGeosetVertex v1 = new CGeosetVertex(ModelOwner) { Position = new CVector3(-1, -1, -1), Normal = new CVector3(0, 0, -1) };
            CGeosetVertex v2 = new CGeosetVertex(ModelOwner) { Position = new CVector3(1, -1, -1), Normal = new CVector3(0, 0, -1) };
            CGeosetVertex v3 = new CGeosetVertex(ModelOwner) { Position = new CVector3(1, 1, -1), Normal = new CVector3(0, 0, -1) };
            CGeosetVertex v4 = new CGeosetVertex(ModelOwner) { Position = new CVector3(-1, 1, -1), Normal = new CVector3(0, 0, -1) };
            CGeosetVertex v5 = new CGeosetVertex(ModelOwner) { Position = new CVector3(-1, -1, 1), Normal = new CVector3(0, 0, 1) };
            CGeosetVertex v6 = new CGeosetVertex(ModelOwner) { Position = new CVector3(1, -1, 1), Normal = new CVector3(0, 0, 1) };
            CGeosetVertex v7 = new CGeosetVertex(ModelOwner) { Position = new CVector3(1, 1, 1), Normal = new CVector3(0, 0, 1) };
            CGeosetVertex v8 = new CGeosetVertex(ModelOwner) { Position = new CVector3(-1, 1, 1), Normal = new CVector3(0, 0, 1) };
            // Add vertices to the geoset
            geoset.Vertices.Add(v1);
            geoset.Vertices.Add(v2);
            geoset.Vertices.Add(v3);
            geoset.Vertices.Add(v4);
            geoset.Vertices.Add(v5);
            geoset.Vertices.Add(v6);
            geoset.Vertices.Add(v7);
            geoset.Vertices.Add(v8);
            // Create the faces of the cube
            AddFace(geoset, ModelOwner, v1, v2, v3); // Bottom face 1
            AddFace(geoset, ModelOwner, v1, v3, v4); // Bottom face 2
            AddFace(geoset, ModelOwner, v5, v6, v7); // Top face 1
            AddFace(geoset, ModelOwner, v5, v7, v8); // Top face 2
            AddFace(geoset, ModelOwner, v1, v5, v8); // Left face 1
            AddFace(geoset, ModelOwner, v1, v8, v4); // Left face 2
            AddFace(geoset, ModelOwner, v2, v6, v7); // Right face 1
            AddFace(geoset, ModelOwner, v2, v7, v3); // Right face 2
            AddFace(geoset, ModelOwner, v1, v2, v6); // Front face 1
            AddFace(geoset, ModelOwner, v1, v6, v5); // Front face 2
            AddFace(geoset, ModelOwner, v4, v3, v7); // Back face 1
            AddFace(geoset, ModelOwner, v4, v7, v8); // Back face 2
            return geoset;
        }
        private static void AddFace(CGeoset geoset, CModel ModelOwner, CGeosetVertex v1, CGeosetVertex v2, CGeosetVertex v3)
        {
            CGeosetTriangle face = new CGeosetTriangle(ModelOwner);
            face.Vertex1.Attach(v1);
            face.Vertex2.Attach(v2);
            face.Vertex3.Attach(v3);
            geoset.Triangles.Add(face);
        }
        internal static CGeoset CreateCylinder(CModel ModelOwner, float radius, float height, int sides)
        {
            // Ensure the number of sides is valid
            if (sides < 3)
            {
                throw new ArgumentException("The number of sides cannot be less than 3.", nameof(sides));
            }
            CGeoset geoset = new CGeoset(ModelOwner);
            float halfHeight = height / 2;
            // Create vertices for the top and bottom circles
            List<CGeosetVertex> topVertices = new List<CGeosetVertex>();
            List<CGeosetVertex> bottomVertices = new List<CGeosetVertex>();
            for (int i = 0; i < sides; i++)
            {
                float angle = 2 * MathF.PI * i / sides;
                float x = radius * MathF.Cos(angle);
                float z = radius * MathF.Sin(angle);
                // Top circle vertex
                CGeosetVertex topVertex = new CGeosetVertex(ModelOwner)
                {
                    Position = new CVector3(x, halfHeight, z),
                    Normal = NormalizeVector(new CVector3(x, 0, z))
                };
                topVertices.Add(topVertex);
                geoset.Vertices.Add(topVertex);
                // Bottom circle vertex
                CGeosetVertex bottomVertex = new CGeosetVertex(ModelOwner)
                {
                    Position = new CVector3(x, -halfHeight, z),
                    Normal = NormalizeVector (new CVector3(x, 0, z))
                };
                bottomVertices.Add(bottomVertex);
                geoset.Vertices.Add(bottomVertex);
            }
            // Create the center vertices for the top and bottom caps
            CGeosetVertex topCenter = new CGeosetVertex(ModelOwner)
            {
                Position = new CVector3(0, halfHeight, 0),
                Normal = new CVector3(0, 1, 0)
            };
            geoset.Vertices.Add(topCenter);
            CGeosetVertex bottomCenter = new CGeosetVertex(ModelOwner)
            {
                Position = new CVector3(0, -halfHeight, 0),
                Normal = new CVector3(0, -1, 0)
            };
            geoset.Vertices.Add(bottomCenter);
            // Create faces for the top and bottom caps
            for (int i = 0; i < sides; i++)
            {
                int next = (i + 1) % sides;
                // Top cap
                AddFace(geoset, ModelOwner, topVertices[i], topVertices[next], topCenter);
                // Bottom cap
                AddFace(geoset, ModelOwner, bottomVertices[next], bottomVertices[i], bottomCenter);
            }
            // Create faces for the sides
            for (int i = 0; i < sides; i++)
            {
                int next = (i + 1) % sides;
                // Side face 1 (bottom to top)
                AddFace(geoset, ModelOwner, bottomVertices[i], topVertices[i], topVertices[next]);
                // Side face 2 (top to bottom)
                AddFace(geoset, ModelOwner, bottomVertices[i], topVertices[next], bottomVertices[next]);
            }
            return geoset;
        }
        public static CVector3 NormalizeVector(CVector3 vector)
        {
            // Calculate the length (magnitude) of the vector
            float length = MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            // Check if the length is greater than zero to avoid division by zero
            if (length > 0)
            {
                // Return a new CVector3 instance with normalized components
                return new CVector3(vector.X / length, vector.Y / length, vector.Z / length);
            }
            else
            {
                throw new InvalidOperationException("Cannot normalize a vector with length zero.");
            }
        }
        internal static CGeoset CreateCone(CModel ModelOwner, float radius, float height, int sides)
        {
            // Ensure the number of sides is at least 3
            if (sides < 3)
            {
                throw new ArgumentException("The number of sides must be at least 3.", nameof(sides));
            }
            CGeoset geoset = new CGeoset(ModelOwner);
            float halfHeight = height / 2;
            // Create the apex vertex of the cone
            CGeosetVertex apexVertex = new CGeosetVertex(ModelOwner)
            {
                Position = new CVector3(0, halfHeight, 0),
                Normal = new CVector3(0, 1, 0) // Upward normal for the apex
            };
            geoset.Vertices.Add(apexVertex);
            // Create the center vertex of the base
            CGeosetVertex baseCenterVertex = new CGeosetVertex(ModelOwner)
            {
                Position = new CVector3(0, -halfHeight, 0),
                Normal = new CVector3(0, -1, 0) // Downward normal for the base center
            };
            geoset.Vertices.Add(baseCenterVertex);
            // Create vertices around the base circle
            List<CGeosetVertex> baseVertices = new List<CGeosetVertex>();
            for (int i = 0; i < sides; i++)
            {
                float angle = 2 * MathF.PI * i / sides;
                float x = radius * MathF.Cos(angle);
                float z = radius * MathF.Sin(angle);
                CGeosetVertex baseVertex = new CGeosetVertex(ModelOwner)
                {
                    Position = new CVector3(x, -halfHeight, z),
                    Normal = new CVector3(x, 0, z) // Outward normal for the base perimeter
                };
                baseVertices.Add(baseVertex);
                geoset.Vertices.Add(baseVertex);
            }
            // Create faces for the base
            for (int i = 0; i < sides; i++)
            {
                int nextIndex = (i + 1) % sides;
                AddFace(geoset, ModelOwner, baseCenterVertex, baseVertices[nextIndex], baseVertices[i]);
            }
            // Create faces for the sides
            for (int i = 0; i < sides; i++)
            {
                int nextIndex = (i + 1) % sides;
                AddFace(geoset, ModelOwner, apexVertex, baseVertices[i], baseVertices[nextIndex]);
            }
            return geoset;
        }
        internal static CGeosetVertex CloneVertex(CGeosetVertex vertex, CModel owner, CGeoset geoset)
        {
            CGeosetVertex vertex2 = new CGeosetVertex(owner);
            vertex2.Position = new CVector3(vertex.Position.X, vertex.Position.Y,vertex.Position.Z);
            vertex2.TexturePosition = new CVector2(vertex.TexturePosition.X, vertex.TexturePosition.Y);
            vertex2.Normal = new CVector3(vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z);
            CGeosetGroup group = new CGeosetGroup(owner);
            foreach (var node in vertex.Group.Object.Nodes)
            {
                CGeosetGroupNode n = new CGeosetGroupNode(owner );
                n.Node.Attach(node.Node.Node);
                group.Nodes.Add(n);
            }
            geoset.Groups.Add(group);
            return vertex2;
        }
        internal static CGeosetVertex CloneVertex_Merge(CGeosetVertex vertex, CModel owner, CGeoset geoset)
        {
            CGeosetVertex vertex2 = new CGeosetVertex(owner);
            vertex2.Position = new CVector3(vertex.Position.X, vertex.Position.Y, vertex.Position.Z);
            vertex2.TexturePosition = new CVector2(vertex.TexturePosition.X, vertex.TexturePosition.Y);
            vertex2.Normal = new CVector3(vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z);
            vertex2.Group.Attach(geoset.Groups[0]);
            return vertex2;
        }
        internal static List<CGeoset> Fragment(CGeoset geoset, CModel owner)
        {
            List<CGeoset> list = new List<CGeoset>();
            INode first_node = null;
            if (geoset.Groups.Count > 0)
            {
                var group = geoset.Vertices[0].Group.Object;
                first_node = group.Nodes[0].Node.Node;
            }
            else
            {
                if (owner.Nodes.Any(x=>x is CBone))
                {
                    first_node = owner.Nodes.First(x=>x is CBone);
                }
                else
                { 
                    first_node = new CBone(owner);
                    first_node.Name = $"GeneratedBone_{IDCounter.Next()}";
                    owner.Nodes.Add(first_node);
                }
            }
            foreach (CGeosetTriangle face in geoset.Triangles)
            {
                CGeoset _new = new CGeoset(owner);
                CGeosetGroup group = new CGeosetGroup(owner);
                CGeosetGroupNode gnode = new CGeosetGroupNode(owner);
                gnode.Node.Attach(first_node);
                group.Nodes.Add(gnode);
                _new.Groups.Add(group);
                  CGeosetTriangle fragment = new CGeosetTriangle(owner);
                CGeosetVertex one = CloneVertex(face.Vertex1.Object, owner, geoset);
                CGeosetVertex two = CloneVertex(face.Vertex2.Object, owner, geoset);
                CGeosetVertex three = CloneVertex(face.Vertex3.Object, owner, geoset);
                fragment.Vertex1.Attach(one);
                fragment.Vertex2.Attach(two);
                fragment.Vertex3.Attach(three);
                _new.Vertices.Add(one);
                _new.Vertices.Add(two);
                _new.Vertices.Add(three);
                _new.Triangles.Add(fragment);
                _new.Material.Attach(geoset.Material.Object);
                //other
                _new.Unselectable = geoset.Unselectable;
                list.Add(_new);
            }
                return list;
        }
        private static bool CoordinatesSame(CVector3 one, CVector3 two)
        {
            return one.X == two.X && one.Y == two.Y && one.Z == two.Z;
        }
        internal static void WeldAll(CGeoset geoset, CModel currentModel)
        {
            start:
            foreach (var vertex1 in geoset.Vertices.ToList())
            {
                foreach (var vertex2 in geoset.Vertices.ToList())
                {
                    if (vertex1 == vertex2) { continue; }
                    if (CoordinatesSame(vertex1.Position, vertex2.Position))
                    {
                        foreach (var face in geoset.Triangles)
                        {
                            if (face.Vertex1.Object == vertex2) face.Vertex1.Attach(vertex1);
                            if (face.Vertex2.Object == vertex2) face.Vertex1.Attach(vertex1);
                            if (face.Vertex3.Object == vertex2) face.Vertex1.Attach(vertex1);
                        }
                        geoset.Vertices.Remove(vertex2);
                        goto start;
                    }
                }
            }
        }
        internal static System.Windows.Media.Brush BrushFromWar3Vector3(CVector3 cVector3)
        {
            // Clamp each value to the 0-1 range to avoid invalid brush colors
            double r = Math.Min(Math.Max(cVector3.Z, 0), 1);
            double g = Math.Min(Math.Max(cVector3.Y, 0), 1);
            double b = Math.Min(Math.Max(cVector3.X, 0), 1);
            // Create and return a SolidColorBrush using the vector's components
            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromScRgb(1f, (float)r, (float)g, (float)b));
        }
        internal static string VisibilityValue(float v)
        {
            if (v <= 0) return "Invisible";
            return "Visible";
        }
        internal static string GetAlpha(float v)
        {
            return (v * 100).ToString(); ;
        }
        static CGeosetVertex getTopMostVertex(CGeoset geoset)
        {
            CGeosetVertex topMostVertex = null;
            float topMostZ = float.MinValue;
            foreach (var vertex in geoset.Vertices)
            {
                if (vertex.Position.Z > topMostZ)
                {
                    topMostZ = vertex.Position.Z;
                    topMostVertex = vertex;
                }
            }
            return topMostVertex;
        }
        static CGeosetVertex getBottomMostVertex(CGeoset geoset)
        {
            CGeosetVertex bottomMostVertex = null;
            float bottomMostZ = float.MaxValue;
            foreach (var vertex in geoset.Vertices)
            {
                if (vertex.Position.Z < bottomMostZ)
                {
                    bottomMostZ = vertex.Position.Z;
                    bottomMostVertex = vertex;
                }
            }
            return bottomMostVertex;
        }
        static CGeosetVertex getLeftMostVertex(CGeoset geoset)
        {
            CGeosetVertex leftMostVertex = null;
            float leftMostY = float.MaxValue;
            foreach (var vertex in geoset.Vertices)
            {
                if (vertex.Position.Y  < leftMostY)
                {
                    leftMostY = vertex.Position.Y;
                    leftMostVertex = vertex;
                }
            }
            return leftMostVertex;
        }
        static CGeosetVertex getRightMostVertex(CGeoset geoset)
        {
            CGeosetVertex rightMostVertex = null;
            float rightmostY = float.MinValue;
            foreach (var vertex in geoset.Vertices)
            {
                if (vertex.Position.Y > rightmostY)
                {
                    rightmostY = vertex.Position.Y;
                    rightMostVertex = vertex;
                }
            }
            return rightMostVertex;
        }
      static  CGeosetVertex getFrontMostVertex(CGeoset geoset)
        {
            CGeosetVertex frontMostVertex = null;
            float frontMostX = float.MinValue;
            foreach (var vertex in geoset.Vertices)
            {
                if (vertex.Position.X > frontMostX)
                {
                    frontMostX = vertex.Position.X;
                    frontMostVertex = vertex;
                }
            }
            return frontMostVertex;
        }
        static CGeosetVertex getBackMostVertex(CGeoset geoset)
        {
            CGeosetVertex backMostVertex = null;
            float backMostX =  float.MaxValue;
            foreach (var vertex in geoset.Vertices)
            {
                if (vertex.Position.X < backMostX)
                {
                    backMostX = vertex.Position.X;
                    backMostVertex = vertex;
                }
            }
            return backMostVertex;
        }
        static bool CoordinateIsAfterCentroid(CVector3 centroid, CVector3 targetCoordinate, Side side)
        {
            switch (side)
            {
                case Side.Top:
                    return targetCoordinate.Z > centroid.Z; // Target is above the centroid (higher Z value)
                case Side.Bottom:
                    return targetCoordinate.Z < centroid.Z; // Target is below the centroid (lower Z value)
                case Side.Left:
                    return targetCoordinate.Y < centroid.Y; // Target is to the left of the centroid (lower Y value)
                case Side.Right:
                    return targetCoordinate.Y > centroid.Y; // Target is to the right of the centroid (higher Y value)
                case Side.Front:
                    return targetCoordinate.X > centroid.X; // Target is in front of the centroid (higher X value)
                case Side.Back:
                    return targetCoordinate.X < centroid.X; // Target is behind the centroid (lower X value)
                default:
                    return false;
            }
        }
        internal static void FlattenSide(CGeoset geoset, Side side)
        {
            var centroid = GetCentroidOfGeoset(geoset);
            switch (side)
            {
                case Side.Left:
                    CGeosetVertex leftmost = getLeftMostVertex(geoset);
                    foreach ( var vertex in geoset.Vertices)
                    {
                        if ( CoordinateIsAfterCentroid(centroid, vertex.Position, side)){
                            float X = vertex.Position.X;
                            float Y = leftmost.Position.Y;
                            float Z = vertex.Position.Z;
                            vertex.Position = new CVector3(X, Y, Z);
                        }
                    }
                    break; 
                case Side.Right:
                    CGeosetVertex rightmost = getRightMostVertex(geoset);
                    foreach (var vertex in geoset.Vertices)
                    {
                        if (CoordinateIsAfterCentroid(centroid, vertex.Position, side))
                        {
                            float X = vertex.Position.X;
                            float Y = rightmost.Position.Y;
                            float Z = vertex.Position.Z;
                            vertex.Position = new CVector3(X, Y, Z);
                        }
                    }
                    break;
                 case Side.Top:
                    CGeosetVertex topmost = getTopMostVertex(geoset);
                    foreach (var vertex in geoset.Vertices)
                    {
                        if (CoordinateIsAfterCentroid(centroid, vertex.Position, side))
                        {
                            float X = vertex.Position.X;
                            float Y = vertex.Position.Y;
                            float Z = topmost.Position.Z;
                            vertex.Position = new CVector3(X, Y, Z);
                        }
                    }
                    break;
                 case Side.Bottom:
                    CGeosetVertex bottommost = getBottomMostVertex(geoset);
                    foreach (var vertex in geoset.Vertices)
                    {
                        if (CoordinateIsAfterCentroid(centroid, vertex.Position, side))
                        {
                            float X = vertex.Position.X;
                            float Y = vertex.Position.Y;
                            float Z = bottommost.Position.Z;
                            vertex.Position = new CVector3(X, Y, Z);
                        }
                    }
                    break;
                  case Side.Front:
                    CGeosetVertex frontmost = getFrontMostVertex(geoset);
                    foreach (var vertex in geoset.Vertices)
                    {
                        if (CoordinateIsAfterCentroid(centroid, vertex.Position, side))
                        {
                            float X = frontmost.Position.X;
                            float Y = vertex.Position.Y;
                            float Z = vertex.Position.Z;
                            vertex.Position = new CVector3(X, Y, Z);
                        }
                    }
                    break;
                   case Side.Back:
                    CGeosetVertex backmost = getBackMostVertex(geoset);
                    foreach (var vertex in geoset.Vertices)
                    {
                        if (CoordinateIsAfterCentroid(centroid, vertex.Position, side))
                        {
                            float X = backmost.Position.X;
                            float Y = vertex.Position.Y;
                            float Z = vertex.Position.Z;
                            vertex.Position = new CVector3(X, Y, Z);
                        }
                    }
                    break;
            }
            // find the farthest vertex at.... and then based on it
        }
        internal static void Simplify(CGeoset geoset, CModel Model)
        {
            // If there are fewer than 3 faces, simplification is not possible
            if (geoset.Triangles.Count < 3) return;
            // Loop until no more surfaces can be simplified
            bool changesMade = true;
            while (changesMade)
            {
                changesMade = false;
                // Find a flat uninterrupted surface with at least 3 faces
                List<CGeosetTriangle> surface = FindUninterruptedFlatSurface(geoset);
                if (surface.Count >= 3)
                {
                    changesMade = true;
                    // 2. Find all outermost vertices of the surface
                    List<CGeosetVertex> outerVertices = FindOuterVertices(surface);
                    // 3. Destroy all faces in that group
                    foreach (CGeosetTriangle face in surface)
                    {
                        geoset.Triangles.Remove(face);
                    }
                    // 4. Create new triangles from the outer vertices
                    List<CGeosetTriangle> newFaces = CreateNewTrianglesFromVertices(outerVertices, Model);
                    // Add the new faces to the geoset
                    foreach (CGeosetTriangle face in newFaces) geoset.Triangles.Add(face);
                }
            }
        }
        private static bool AreCoplanar(CGeosetTriangle one, CGeosetTriangle two)
        {
            // Basic implementation for coplanarity check using normals or another criteria
            // For simplicity, using vertex positions as an example, assuming a method that checks normals is implemented
            var normal1 = CalculateNormal(one);
            var normal2 = CalculateNormal(two);
            return normal1.Equals(normal2); // You can refine this logic with a tolerance check
        }
        // Assuming CVertex has a Position property that is of type Vector3
        private static Vector3 CalculateNormal(CGeosetTriangle face)
        {
            // Get the positions of the three vertices
            var v1 = face.Vertex1.Object.Position;
            var v2 = face.Vertex2.Object.Position;
            var v3 = face.Vertex3.Object.Position;
            // Compute two edge vectors
            var edge1 = new Vector3(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z); // v2 - v1
            var edge2 = new Vector3(v3.X - v1.X, v3.Y - v1.Y, v3.Z - v1.Z); // v3 - v1
            // Calculate the cross product (the normal to the plane of the triangle)
            var normal = new Vector3(
                edge1.Y * edge2.Z - edge1.Z * edge2.Y, // X component
                edge1.Z * edge2.X - edge1.X * edge2.Z, // Y component
                edge1.X * edge2.Y - edge1.Y * edge2.X  // Z component
            );
            // Normalize the normal vector (make it a unit vector)
            float length = (float)Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
            if (length > 0)
            {
                normal.X /= length;
                normal.Y /= length;
                normal.Z /= length;
            }
            return normal;
        }
        private static List<CGeosetTriangle> FindUninterruptedFlatSurface(CGeoset geoset)
        {
            List<CGeosetTriangle> surface = new List<CGeosetTriangle>();
            List<CGeosetTriangle> remainingFaces = new List<CGeosetTriangle>(geoset.Triangles);
            for (int i = 0; i < remainingFaces.Count - 1; i++)
            {
                if (AreCoplanar(remainingFaces[i], remainingFaces[i + 1]))
                {
                    // Add faces to the surface group
                    surface.Add(remainingFaces[i]);
                    surface.Add(remainingFaces[i + 1]);
                    remainingFaces.RemoveAt(i);
                    remainingFaces.RemoveAt(i);
                    i--; // Adjust index after removal
                }
            }
            // Second pass: check for connected faces that share vertices
            List<CGeosetTriangle> connectedFaces = new List<CGeosetTriangle>();
            foreach (CGeosetTriangle face in surface)
            {
                foreach (CGeosetTriangle otherFace in remainingFaces)
                {
                    if (HaveCommonVertices(face, otherFace))
                    {
                        connectedFaces.Add(otherFace);
                        remainingFaces.Remove(otherFace);
                    }
                }
            }
            surface.AddRange(connectedFaces);
            return surface;
        }
        private static bool HaveCommonVertices(CGeosetTriangle face1, CGeosetTriangle face2)
        {
            // Check if two faces share at least one vertex
            return face1.Vertex1 == face2.Vertex1 || face1.Vertex1 == face2.Vertex2 || face1.Vertex1 == face2.Vertex3 ||
                   face1.Vertex2 == face2.Vertex1 || face1.Vertex2 == face2.Vertex2 || face1.Vertex2 == face2.Vertex3 ||
                   face1.Vertex3 == face2.Vertex1 || face1.Vertex3 == face2.Vertex2 || face1.Vertex3 == face2.Vertex3;
        }
        private static List<CGeosetVertex> FindOuterVertices(List<CGeosetTriangle> surface)
        {
            List<CGeosetVertex> outerVertices = new List<CGeosetVertex>();
            // For simplicity, assume we get the outer vertices from the convex hull or boundary of the surface.
            // This can be done by examining the vertices and finding the outermost ones (for example, using a convex hull algorithm).
            // Dummy example, just collecting vertices
            foreach (var face in surface)
            {
                if (!outerVertices.Contains(face.Vertex1.Object)) outerVertices.Add(face.Vertex1.Object);
                if (!outerVertices.Contains(face.Vertex2.Object)) outerVertices.Add(face.Vertex2.Object);
                if (!outerVertices.Contains(face.Vertex3.Object)) outerVertices.Add(face.Vertex3.Object);
            }
            return outerVertices;
        }
        private static List<CGeosetTriangle> CreateNewTrianglesFromVertices(List<CGeosetVertex> vertices, CModel Model)
        {
            List<CGeosetTriangle> newFaces = new List<CGeosetTriangle>();
            // For simplicity, assume the outer vertices form a simple polygon
            // We would typically triangulate the outer vertices (e.g., by using an ear clipping algorithm or similar)
            // Dummy triangulation example
            for (int i = 1; i < vertices.Count - 1; i++)
            {
                CGeosetTriangle face = new CGeosetTriangle(Model);
                face.Vertex1.Attach(vertices[0]);
                face.Vertex2.Attach(vertices[i]);
                face.Vertex3.Attach(vertices[i+1]);
                newFaces.Add(face);
            }
            return newFaces;
        }
        internal static int[] GetColor(Button button)
        {
            // Ensure the background is a SolidColorBrush
            if (button.Background is System.Windows.Media.SolidColorBrush solidColorBrush)
            {
                // Extract the color components
                System.Windows.Media.Color color = solidColorBrush.Color;
                int r = color.R;
                int g = color.G;
                int b = color.B;
                return new int[] { r, g, b };
            }
            // Return a default value or throw an exception if the brush is not a SolidColorBrush
            return new int[] { 0, 0, 0 }; // Default to black
        }
        internal static System.Windows.Media.Brush ColorToBrush(System.Windows.Media.Color color)
        {
            return new System.Windows.Media.SolidColorBrush(color);
        }
        internal static System.Windows.Media.Brush War3ColorToBrush(CVector3 color)
        {
            float R = color.Z * 255;
            float G = color.Y * 255;
            float B = color.X * 255;
            return BrushFromRGB(R, G, B);
            throw new NotImplementedException();
        }
        private static System.Windows.Media.Brush BrushFromRGB(float r, float g, float b)
        {
            // Ensure the values are clamped between 0 and 1
            r = Math.Max(0, Math.Min(1, r));
            g = Math.Max(0, Math.Min(1, g));
            b = Math.Max(0, Math.Min(1, b));
            // Convert the values to the 0-255 range
            byte red = (byte)(r * 255);
            byte green = (byte)(g * 255);
            byte blue = (byte)(b * 255);
            // Create and return the SolidColorBrush
            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(red, green, blue));
        }
        internal static System.Windows.Media.Color BrushToColor(System.Windows.Media.Brush brush)
        {
            if (brush is System.Windows.Media.SolidColorBrush solidColorBrush)
            {
                return solidColorBrush.Color;
            }
            else
            {
                throw new InvalidOperationException("Only SolidColorBrush is supported.");
            }
        }
        internal static CVector3 ColorToWar3Color(System.Windows.Media.Color color)
        {
            float r = color.B / 255;
            float g = color.G / 255;
            float b = color.R / 255;
           return new CVector3(r, g, b);
        }
        internal static int _255ToPercentage(float alpha)
        {
            if (alpha < 0) { return 0; }
            if (alpha > 255) { return 100; }
            // Convert 255-based percentage to standard percentage
             return _255To100(alpha);
        }
        public static int _255To100(float value)
        {
            if (value < 0 || value > 255) { throw new ArgumentOutOfRangeException(nameof(value), "The value must be between 0 and 255."); }
            return (int)((value / 255) * 100);
        }
        internal static System.Windows.Media.Brush RGBToBrush(int r, int g, int b)
        {
            // Validate the RGB values to ensure they are within the valid range (0-255)
            r = Math.Clamp(r, 0, 255);
            g = Math.Clamp(g, 0, 255);
            b = Math.Clamp(b, 0, 255);
            // Create a SolidColorBrush using the RGB values
            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)r, (byte)g, (byte)b));
        }
        internal static float Magnitude(CVector3 vector)
        {
            return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }
        internal static float Magnitude(CVector4 vector)
        {
            return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z + vector.W * vector.W);
        }
        internal static float Difference(CAnimatorNode<CVector4> cAnimatorNode1, CAnimatorNode<CVector4> cAnimatorNode2, CAnimatorNode<CVector4> cAnimatorNode3)
        {
            if (cAnimatorNode1 == null || cAnimatorNode2 == null || cAnimatorNode3 == null)
            {
                return 4;
            }
            // Convert Quaternion to Euler angles as CVector3
            CVector3 one = QuaternionToEuler(cAnimatorNode1.Value);
            CVector3 two = QuaternionToEuler(cAnimatorNode2.Value);
            CVector3 three = QuaternionToEuler(cAnimatorNode3.Value);
            // Calculate differences
            CVector3 diffAbove = new CVector3(
                Math.Abs(two.X - one.X),
                Math.Abs(two.Y - one.Y),
                Math.Abs(two.Z - one.Z)
            );
            CVector3 diffBelow = new CVector3(
                Math.Abs(two.X - three.X),
                Math.Abs(two.Y - three.Y),
                Math.Abs(two.Z - three.Z)
            );
            // Calculate total magnitude of differences
            float totalDifference = Magnitude(diffAbove) + Magnitude(diffBelow);
            return totalDifference;
        }
        internal static float Difference(CAnimatorNode<CVector3> cAnimatorNode1, CAnimatorNode<CVector3> cAnimatorNode2, CAnimatorNode<CVector3> cAnimatorNode3)
        {
            if (cAnimatorNode1 == null || cAnimatorNode2 == null || cAnimatorNode3 == null)
            {
                return 4;
            }
            // Get the CVector3 values
            CVector3 one = cAnimatorNode1.Value;
            CVector3 two = cAnimatorNode2.Value;
            CVector3 three = cAnimatorNode3.Value;
            // Calculate differences
            CVector3 diffAbove = new CVector3(
                Math.Abs(two.X - one.X),
                Math.Abs(two.Y - one.Y),
                Math.Abs(two.Z - one.Z)
            );
            CVector3 diffBelow = new CVector3(
                Math.Abs(two.X - three.X),
                Math.Abs(two.Y - three.Y),
                Math.Abs(two.Z - three.Z)
            );
            // Calculate total magnitude of differences
            float totalDifference = Magnitude(diffAbove) + Magnitude(diffBelow);
            return totalDifference;
        }
        internal static float Difference(CAnimatorNode<float> cAnimatorNode1, CAnimatorNode<float> cAnimatorNode2, CAnimatorNode<float> cAnimatorNode3)
        {
            if (cAnimatorNode1 == null || cAnimatorNode2 == null || cAnimatorNode3 == null)
            {
                return 4; // Default return value when any of the nodes are null
            }
            // Get the float values from the animator nodes
            float one = cAnimatorNode1.Value;
            float two = cAnimatorNode2.Value;
            float three = cAnimatorNode3.Value;
            // Calculate the absolute differences
            float diffAbove = Math.Abs(two - one);
            float diffBelow = Math.Abs(two - three);
            // Combine the differences (you can choose how to aggregate these)
            float totalDifference = diffAbove + diffBelow;
            return totalDifference;
        }
        internal static void TransferGeosetData(CGeoset first, CGeoset second, CModel owner)
        {
            // Create a mapping of original vertices to their clones
            Dictionary<CGeosetVertex, CGeosetVertex> reference = new Dictionary<CGeosetVertex, CGeosetVertex>();
            foreach (CGeosetVertex vertex in second.Vertices)
            {
                CGeosetVertex copy = CloneVertex_Merge(vertex, owner, first);
                reference.Add(vertex, copy);
                first.Vertices.Add(copy);
            }
            // Transfer faces from the second geoset to the first
            foreach (CGeosetTriangle face in second.Triangles)
            {
                CGeosetTriangle copy = new CGeosetTriangle(owner);
                copy.Vertex1.Attach(reference[face.Vertex1.Object]);
                copy.Vertex2.Attach(reference[face.Vertex2.Object]);
                copy.Vertex3.Attach(reference[face.Vertex3.Object]);
                first.Triangles.Add(copy);
            }
        }
        internal static List<List<CGeosetTriangle>> CollectTriangleGroups(CGeoset geoset)
        {
            // A face is a triangle made of 3 vertices
            List<List<CGeosetTriangle>> triangleGroups = new List<List<CGeosetTriangle>>();
            if (geoset == null || geoset.Triangles.Count <= 1)
            {
                return triangleGroups;
            }
            // Track visited faces
            HashSet<CGeosetTriangle> visited = new HashSet<CGeosetTriangle>();
            // Helper function to find connected faces
            void CollectConnectedFaces(CGeosetTriangle startFace, List<CGeosetTriangle> group)
            {
                Queue<CGeosetTriangle> toVisit = new Queue<CGeosetTriangle>();
                toVisit.Enqueue(startFace);
                while (toVisit.Count > 0)
                {
                    CGeosetTriangle currentFace = toVisit.Dequeue();
                    if (visited.Contains(currentFace))
                    {
                        continue;
                    }
                    visited.Add(currentFace);
                    group.Add(currentFace);
                    // Check all other faces for shared vertices
                    foreach (CGeosetTriangle otherFace in geoset.Triangles)
                    {
                        if (!visited.Contains(otherFace) && FacesShareVertex(currentFace, otherFace))
                        {
                            toVisit.Enqueue(otherFace);
                        }
                    }
                }
            }
            // Check if two faces share at least one vertex
            bool FacesShareVertex(CGeosetTriangle face1, CGeosetTriangle face2)
            {
                var vertices1 = new[] { face1.Vertex1.Object, face1.Vertex2.Object, face1.Vertex3.Object };
                var vertices2 = new[] { face2.Vertex1.Object, face2.Vertex2.Object, face2.Vertex3.Object };
                return vertices1.Intersect(vertices2).Any();
            }
            // Group faces into connected components
            foreach (CGeosetTriangle face in geoset.Triangles)
            {
                if (!visited.Contains(face))
                {
                    List<CGeosetTriangle> group = new List<CGeosetTriangle>();
                    CollectConnectedFaces(face, group);
                    triangleGroups.Add(group);
                }
            }
            return triangleGroups;
        }
        internal static void CopyVertex(CGeosetVertex  original, CGeosetVertex copy)
        {
            copy.Position = new CVector3(original.Position);
            copy.Normal = new CVector3(original.Normal);
            copy.TexturePosition = new CVector2(original.TexturePosition);
            copy.Group.Attach(original.Group.Object);  
        }
        internal static void CopyGroup(CGeosetGroup original, CGeosetGroup copy, CModel model)
        {
            List<INode> nodes = new List<INode>();
            foreach (var node in original.Nodes)
            {
                nodes.Add(node.Node.Node);
            }
            foreach (var node in nodes)
            {
                CGeosetGroupNode gnode = new CGeosetGroupNode(model);
                gnode.Node.Attach(node);
                copy.Nodes.Add(gnode);
            }
        }
        internal static void CleanFreeVertices(CGeoset geoset)
        {
            foreach (CGeosetVertex vertex in geoset.Vertices.ToList())
            {
                bool has = true;
               foreach (CGeosetTriangle triangle in geoset.Triangles)
                {
                    if (triangle.Vertex1.Object == vertex || triangle.Vertex2.Object == vertex || triangle.Vertex3.Object == vertex)
                    {
                        has = true;
                        break;
                    }
                }
               if (!has)
                {
                    geoset.Vertices.Remove(vertex);
                }
            }
        }
        internal static CGeoset GeosetFromTriangles(List<CGeosetTriangle> collection, CModel model, CGeoset geoset)
        {
            CGeoset newGeoset = new CGeoset(model);
            Dictionary<CGeosetVertex, CGeosetVertex> reference = new Dictionary<CGeosetVertex, CGeosetVertex>();
            foreach (CGeosetTriangle triangle in collection)
            {
                CGeosetTriangle newTriangle = new CGeosetTriangle(model);
                // Process each vertex of the triangle
                newTriangle.Vertex1.Attach(GetOrCreateVertex(triangle.Vertex1.Object, newGeoset, reference, model));
                newTriangle.Vertex2.Attach(GetOrCreateVertex(triangle.Vertex2.Object, newGeoset, reference, model));
                newTriangle.Vertex3.Attach(GetOrCreateVertex(triangle.Vertex3.Object, newGeoset, reference, model));
                newGeoset.Triangles.Add(newTriangle);
            }
            newGeoset.Material.Attach(geoset.Material.Object);
            return newGeoset;
        }
        private static CGeosetVertex GetOrCreateVertex(
            CGeosetVertex originalVertex,
            CGeoset geoset,
            Dictionary<CGeosetVertex, CGeosetVertex> reference,
            CModel model)
        {
            if (reference.TryGetValue(originalVertex, out CGeosetVertex existingVertex))
            {
                return existingVertex;
            }
            // Create a new vertex and copy properties
            CGeosetVertex newVertex = new CGeosetVertex(model);
            CopyVertex(originalVertex, newVertex);
            geoset.Vertices.Add(newVertex);
            // Add to reference for future lookups
            reference[originalVertex] = newVertex;
            return newVertex;
        }
        internal static void GiveGroupToGeoset(CGeoset geoset, CModel model)
        {
            CGeosetGroup group = new CGeosetGroup(model);
            CGeosetGroupNode node = new CGeosetGroupNode(model);
            node.Node.Attach(GetBone(model));
            group.Nodes.Add(node);
           geoset.Groups.Add(group);
        }
        private static INode GetBone(CModel model)
        {
            if (model.Nodes.Any(x=>x is CBone))
            {
                return model.Nodes.First(x=>x is CBone);
            }
            else
            {
                CBone bone = new CBone(model);
                bone.Name ="GeneratedBone_" +IDCounter.Next_();
                model.Nodes.Add(bone);
                return bone;
            }
        }
        internal static CGeoset ReAddTriangles(List<List<CGeosetTriangle>> list,
            CModel model, CGeoset geoset)
        {
           CGeoset _modified = new CGeoset(model);
            _modified.Material.Attach(geoset.Material.Object);
            GiveGroupToGeoset(_modified, model);
            foreach (var triangleList in list)
            {
                Dictionary<CGeosetVertex, CGeosetVertex> reference = new Dictionary<CGeosetVertex, CGeosetVertex>();
                foreach (var triangle in triangleList)
                {
                    CGeosetTriangle _triangle = new CGeosetTriangle(model);
                    if (reference.ContainsKey(triangle.Vertex1.Object))
                    {
                        _triangle.Vertex1.Attach(reference[triangle.Vertex1.Object]);
                    }
                    else
                    {
                        CGeosetVertex _vertex = new CGeosetVertex(model);
                        CopyVertex(triangle.Vertex1.Object, _vertex);
                        _modified.Vertices.Add(_vertex);
                        reference.Add(triangle.Vertex1.Object, _vertex);
                        _triangle.Vertex1.Attach(_vertex);
                        _vertex.Group.Attach(_modified.Groups[0]);
                    }
                    if (reference.ContainsKey(triangle.Vertex2.Object))
                    {
                        _triangle.Vertex2.Attach(reference[triangle.Vertex2.Object]);
                    }
                    else
                    {
                        CGeosetVertex _vertex = new CGeosetVertex(model);
                        CopyVertex(triangle.Vertex2.Object, _vertex);
                        _modified.Vertices.Add(_vertex);
                        reference.Add(triangle.Vertex2.Object, _vertex);
                        _triangle.Vertex2.Attach(_vertex);
                        _vertex.Group.Attach(_modified.Groups[0]);
                    }
                    if (reference.ContainsKey(triangle.Vertex3.Object))
                    {
                        _triangle.Vertex3.Attach(reference[triangle.Vertex3.Object]);
                    }
                    else
                    {
                        CGeosetVertex _vertex = new CGeosetVertex(model);
                        CopyVertex(triangle.Vertex3.Object, _vertex);
                        _modified.Vertices.Add(_vertex);
                        reference.Add(triangle.Vertex3.Object, _vertex);
                        _triangle.Vertex3.Attach(_vertex);
                        _vertex.Group.Attach(_modified.Groups[0]);
                    }
                    _modified.Triangles.Add(_triangle);
                }
            }
            return _modified;
        }
        private static CVector3 GetCentroid(List<CGeoset> geosets)
        {
            CVector3 centroid = new CVector3();
            List<CVector3> vectors = new List<CVector3>();

            foreach (CGeoset geoset in geosets)
            {
                foreach (CGeosetVertex vertex in geoset.Vertices)
                {
                    vectors.Add(vertex.Position);
                }
            }

            if (vectors.Count == 0)
            {
                return centroid; // Return default centroid if no vertices
            }

            // Calculate the centroid by averaging the positions
            float totalX = 0, totalY = 0, totalZ = 0;
            foreach (CVector3 vector in vectors)
            {
                totalX += vector.X;
                totalY += vector.Y;
                totalZ += vector.Z;
            }

            float count = vectors.Count;
            centroid = new CVector3(totalX / count, totalY / count, totalZ / count);
            return centroid;
        }

        internal static void RotateGeosetsTogether(List<CGeoset> geosets, float x, float y, float z)
        {
            CVector3 centroid = GetCentroid(geosets);

            // Calculate the rotation matrix (assuming Euler angles in degrees for simplicity)
            float radiansX = MathHelper.DegreesToRadians(x);
            float radiansY = MathHelper.DegreesToRadians(y);
            float radiansZ = MathHelper.DegreesToRadians(z);

            Matrix4x4 rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(radiansY, radiansX, radiansZ);

            // Apply rotation to each vertex
            foreach (CGeoset geoset in geosets)
            {
                foreach (CGeosetVertex vertex in geoset.Vertices)
                {
                    // Translate to origin
                    CVector3 translated = new CVector3(
                        vertex.Position.X - centroid.X,
                        vertex.Position.Y - centroid.Y,
                        vertex.Position.Z - centroid.Z
                    );

                    // Apply rotation
                    CVector3 rotated = ApplyRotation(rotationMatrix, translated);

                    // Translate back
                    vertex.Position = new CVector3(
                        rotated.X + centroid.X,
                        rotated.Y + centroid.Y,
                        rotated.Z + centroid.Z
                    );
                }
            }
        }

        private static CVector3 ApplyRotation(Matrix4x4 rotationMatrix, CVector3 vector)
        {
            return new CVector3(
                rotationMatrix.M11 * vector.X + rotationMatrix.M12 * vector.Y + rotationMatrix.M13 * vector.Z,
                rotationMatrix.M21 * vector.X + rotationMatrix.M22 * vector.Y + rotationMatrix.M23 * vector.Z,
                rotationMatrix.M31 * vector.X + rotationMatrix.M32 * vector.Y + rotationMatrix.M33 * vector.Z
            );
        }

        internal static Point3D GetFarthestPoint(CGeoset geoset)
        {
            Point3D farthestPoint = new Point3D(0, 0, 0);
            double maxDistance = 0;

            foreach (var vertex in geoset.Vertices)
            {
                Point3D point = new Point3D(vertex.Position.X, vertex.Position.Y, vertex.Position.Z);

                // Calculate the distance from the origin (0,0,0) to the current point
                double distance = Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2) + Math.Pow(point.Z, 2));

                // Update the farthest point if a new maximum distance is found
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestPoint = point;
                }
            }

            return farthestPoint;
        }
        internal static float GetFarthestDistance(CGeoset geoset)
        {
            float farthest = 0;
            CVector3 centroid = GetCentroidOfGeoset(geoset);

            // from centroid to farthest vertex
            foreach (var vertex in geoset.Vertices)
            {
                // Calculate the distance from the centroid to the current vertex
                float dx = vertex.Position.X - centroid.X;
                float dy = vertex.Position.Y - centroid.Y;
                float dz = vertex.Position.Z - centroid.Z;

                double distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);

                // Update the farthest point if a new maximum distance is found
                farthest = (float)Math.Max(farthest, distance);
            }

            return farthest;
        }

        internal static float GetDistanceBetweenPoints(Point3D centroidP, Point3D farthest)
        {
            // Calculate the distance between centroidP and farthest using the Euclidean distance formula
            double distance = Math.Sqrt(
                Math.Pow(farthest.X - centroidP.X, 2) +
                Math.Pow(farthest.Y - centroidP.Y, 2) +
                Math.Pow(farthest.Z - centroidP.Z, 2)
            );

            return (float)distance; // Convert to float and return
        }
        internal static float GetDistanceBetweenPoints(CVector3 one, CVector3 two)
        {
            // Calculate the distance between centroidP and farthest using the Euclidean distance formula
            double distance = Math.Sqrt(
                Math.Pow(one.X - two.X, 2) +
                Math.Pow(one.Y - two.Y, 2) +
                Math.Pow(one.Z - two.Z, 2)
            );

            return (float)distance; // Convert to float and return
        }
        internal static void ClampScaling(CAnimatorNode<CVector3> node)
        {
            if (node.Value.X < 0) node.Value.X = 0;
            if (node.Value.Y < 0) { node.Value.Y = 0; }
            if (node.Value.Z < 0) { node.Value.Z = 0; }

            if (node.InTangent.X < 0) node.InTangent.X = 0;
            if (node.InTangent.Y < 0) { node.InTangent.Y = 0; }
            if (node.InTangent.Z < 0) { node.InTangent.Z = 0; }

            if (node.OutTangent.X < 0) node.OutTangent.X = 0;
            if (node.OutTangent.Y < 0) { node.OutTangent.Y = 0; }
            if (node.OutTangent.Z < 0) { node.OutTangent.Z = 0; }

        }

        internal static float DifferenceQuaternion(CAnimatorNode<CVector4> cAnimatorNode1, CAnimatorNode<CVector4> cAnimatorNode2, CAnimatorNode<CVector4> cAnimatorNode3)
        {
            CVector3 e1 = Calculator.QuaternionToEuler(cAnimatorNode1.Value);
            CVector3 e2 = Calculator.QuaternionToEuler(cAnimatorNode2.Value);
            CVector3 e3 = Calculator.QuaternionToEuler(cAnimatorNode3.Value);

            // Compute absolute differences for each component
            float diffX = MathF.Abs(e1.X - e2.X) + MathF.Abs(e2.X - e3.X) + MathF.Abs(e3.X - e1.X);
            float diffY = MathF.Abs(e1.Y - e2.Y) + MathF.Abs(e2.Y - e3.Y) + MathF.Abs(e3.Y - e1.Y);
            float diffZ = MathF.Abs(e1.Z - e2.Z) + MathF.Abs(e2.Z - e3.Z) + MathF.Abs(e3.Z - e1.Z);

            // Aggregate the difference (sum or average)
            return (diffX + diffY + diffZ) / 3f; // Average difference
        }


        // Normalize the quaternion
        private static CVector4 NormalizeQuaternion(CVector4 q)
        {
            float magnitude = (float)Math.Sqrt(q.X * q.X + q.Y * q.Y + q.Z * q.Z + q.W * q.W);
            if (magnitude == 0) return new CVector4(0, 0, 0, 1); // Default identity quaternion

            return new CVector4(q.X / magnitude, q.Y / magnitude, q.Z / magnitude, q.W / magnitude);
        }

        // Compute the angular difference in degrees
        private static float QuaternionDifference(CVector4 q1, CVector4 q2)
        {
            float dot = Math.Clamp(q1.X * q2.X + q1.Y * q2.Y + q1.Z * q2.Z + q1.W * q2.W, -1f, 1f);
            return MathF.Acos(MathF.Abs(dot)) * (360f / MathF.PI); // Convert radians to degrees
        }

        internal static bool GlobalSequenceIsUsed(CModel model, CGlobalSequence gs)
        {
            foreach (var node in model.Nodes)
            {
                if (node.Translation.GlobalSequence.Object == gs) return true;
                if (node.Rotation.GlobalSequence.Object == gs) return true;
                if (node.Scaling.GlobalSequence.Object == gs) return true;
                if (node is CLight light)
                {
                    if (light.Visibility.GlobalSequence.Object == gs) return true;
                    if (light.Color.GlobalSequence.Object == gs) return true;
                    if (light.AmbientColor.GlobalSequence.Object == gs) return true;
                    if (light.AmbientIntensity.GlobalSequence.Object == gs) return true;
                    if (light.Intensity.GlobalSequence.Object == gs) return true;
                    if (light.AttenuationEnd.GlobalSequence.Object == gs) return true;
                    if (light.AttenuationStart.GlobalSequence.Object == gs) return true;
                }
                if (node is CParticleEmitter emitter)
                {
                    if (emitter.Visibility.GlobalSequence.Object == gs) return true;
                    if (emitter.EmissionRate.GlobalSequence.Object == gs) return true;
                    if (emitter.LifeSpan.GlobalSequence.Object == gs) return true;
                    if (emitter.InitialVelocity.GlobalSequence.Object == gs) return true;
                    if (emitter.Gravity.GlobalSequence.Object == gs) return true;
                    if (emitter.Longitude.GlobalSequence.Object == gs) return true;
                    if (emitter.Latitude.GlobalSequence.Object == gs) return true;
                }
                if (node is CParticleEmitter2 emitter2)
                {
                    if (emitter2.Visibility.GlobalSequence.Object == gs) return true;
                    if (emitter2.Width.GlobalSequence.Object == gs) return true;
                    if (emitter2.Length.GlobalSequence.Object == gs) return true;
                    if (emitter2.Speed.GlobalSequence.Object == gs) return true;
                    if (emitter2.Variation.GlobalSequence.Object == gs) return true;
                    if (emitter2.Latitude.GlobalSequence.Object == gs) return true;
                    if (emitter2.EmissionRate.GlobalSequence.Object == gs) return true;
                    if (emitter2.Gravity.GlobalSequence.Object == gs) return true;
                }
                if (node is CRibbonEmitter ribbon)
                {
                    if (ribbon.Visibility.GlobalSequence.Object == gs) return true;
                    if (ribbon.HeightAbove.GlobalSequence.Object == gs) return true;
                    if (ribbon.HeightBelow.GlobalSequence.Object == gs) return true;
                    if (ribbon.Color.GlobalSequence.Object == gs) return true;
                    if (ribbon.Alpha.GlobalSequence.Object == gs) return true;
                    if (ribbon.TextureSlot.GlobalSequence.Object == gs) return true;
                }
                if (node is CAttachment attachment)
                {
                    if (attachment.Visibility.GlobalSequence.Object == gs) return true;
                }
            }
            foreach (var ga in model.GeosetAnimations)
            {
                if (ga.Alpha.GlobalSequence.Object == gs) return true;
                if (ga.Color.GlobalSequence.Object == gs) return true;
            }
            foreach (var material in model.Materials)
            {
                foreach (var layer in material.Layers)
                {
                    if (layer.Alpha.GlobalSequence.Object == gs) return true;
                    if (layer.TextureId.GlobalSequence.Object == gs) return true;
                }
            }
            foreach (var cam in model.Cameras)
            {
                if (cam.Translation.GlobalSequence.Object == gs) return true;
                if (cam.TargetTranslation.GlobalSequence.Object == gs) return true;
                if (cam.Rotation.GlobalSequence.Object == gs) return true;
            }
            foreach (var ta in model.TextureAnimations)
            {
                if (ta.Translation.GlobalSequence.Object == gs) return true;
                if (ta.Rotation.GlobalSequence.Object == gs) return true;
                if (ta.Scaling.GlobalSequence.Object == gs) return true;
            }
            return false;
            throw new NotImplementedException();
        }

        internal static bool HasZeroArea(CGeosetTriangle triangle)
        {
            // If any two vertices are identical
            if (triangle.Vertex1.Object == triangle.Vertex2.Object ||
                triangle.Vertex2.Object == triangle.Vertex3.Object ||
                triangle.Vertex3.Object == triangle.Vertex1.Object)
            {
                return true;
            }

            // Get vertex positions
            CVector3 pos1 = triangle.Vertex1.Object.Position;
            CVector3 pos2 = triangle.Vertex2.Object.Position;
            CVector3 pos3 = triangle.Vertex3.Object.Position;

            // Compute the two edge vectors
            CVector3 edge1 = new CVector3(pos2.X - pos1.X, pos2.Y - pos1.Y, pos2.Z - pos1.Z);
            CVector3 edge2 = new CVector3(pos3.X - pos1.X, pos3.Y - pos1.Y, pos3.Z - pos1.Z);

            // Compute the triangle area using the cross-product magnitude formula
            float areaSquared = (edge1.Y * edge2.Z - edge1.Z * edge2.Y) * (edge1.Y * edge2.Z - edge1.Z * edge2.Y) +
                                (edge1.Z * edge2.X - edge1.X * edge2.Z) * (edge1.Z * edge2.X - edge1.X * edge2.Z) +
                                (edge1.X * edge2.Y - edge1.Y * edge2.X) * (edge1.X * edge2.Y - edge1.Y * edge2.X);

            // A triangle has zero or near-zero area if the squared area is extremely small
            return areaSquared < 1e-6f;
        }

        internal static CVector3 GetCentroidOfVertices(List<CGeosetVertex> attached)
        {
            if (attached == null || attached.Count == 0)
                return new CVector3(); // Return a default vector if the list is empty

            float sumX = 0, sumY = 0, sumZ = 0;

            foreach (var vertex in attached)
            {
                CVector3 position = vertex.Position;
                sumX += position.X;
                sumY += position.Y;
                sumZ += position.Z;
            }

            float count = attached.Count;
            return new CVector3(sumX / count, sumY / count, sumZ / count);
        }

        internal static List<List<CGeosetVertex>> FindOverlappingVertexGroups(CGeoset geoset, float Threshold)
        {
            if (geoset.Vertices.Count <= 1)
            {
                throw new Exception("Not enough vertices");
            }

            List<List<CGeosetVertex>> lists = new();
            HashSet<CGeosetVertex> visited = new();

            foreach (CGeosetVertex vertex in geoset.Vertices)
            {
                if (visited.Contains(vertex))
                {
                    continue;
                }

                List<CGeosetVertex> group = new() { vertex };
                visited.Add(vertex);

                for (int j = 0; j < geoset.Vertices.Count; j++)
                {
                    CGeosetVertex other = geoset.Vertices[j];
                    if (!visited.Contains(other) && GetDistanceBetweenPoints(vertex.Position, other.Position) <= Threshold)
                    {
                        group.Add(other);
                        visited.Add(other);
                    }
                }

                lists.Add(group);
            }

            return lists;
        }

        public static float GetVectorMagnitude(CVector3 vector)
        {
            return MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }

    }
}
