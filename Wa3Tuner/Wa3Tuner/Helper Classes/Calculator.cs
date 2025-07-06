using MdxLib.Animator;
using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
 
 
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
 
using Wa3Tuner.Dialogs;
using Wa3Tuner.Helper_Classes;
using Whim_GEometry_Editor.Misc;
using static Wa3Tuner.MainWindow;
using Quaternion = System.Numerics.Quaternion;
namespace Wa3Tuner
{


    public static class Calculator
    {
        public enum ExtentPosition
        {
            Center,

            // Single-axis positions
            Top, Bottom,
            Left, Right,
            Front, Back,

            // Edge combinations
            TopLeft, TopRight,
            BottomLeft, BottomRight,
            TopFront, TopBack,
            BottomFront, BottomBack,
            LeftFront, LeftBack,
            RightFront, RightBack,

            // Corner combinations
            TopLeftFront, TopLeftBack,
            TopRightFront, TopRightBack,
            BottomLeftFront, BottomLeftBack,
            BottomRightFront, BottomRightBack
        }
        public static void CenterVectorAtExtent(CVector3 v, CExtent e, ExtentPosition p)
        {
            v = GetPositionAtExtent(e, p);
        }
        public static Vector3 FindClosestVector(Vector3 vec, List<Vector3> vectors)
        {
            if (vectors == null || vectors.Count == 0)
                throw new ArgumentException("Vector list is empty.");

            Vector3 closest = vectors[0];
            float minDistanceSquared = Vector3.DistanceSquared(vec, closest);

            foreach (var v in vectors)
            {
                float distanceSquared = Vector3.DistanceSquared(vec, v);
                if (distanceSquared < minDistanceSquared)
                {
                    minDistanceSquared = distanceSquared;
                    closest = v;
                }
            }

            return closest;
        }
        public static int FindClosestVectorIndex(Vector3 vec, List<Vector3> vectors)
        {
            if (vectors == null || vectors.Count == 0)
                throw new ArgumentException("Vector list is empty.");

            Vector3 closest = vectors[0];
            float minDistanceSquared = Vector3.DistanceSquared(vec, closest);

            foreach (var v in vectors)
            {
                float distanceSquared = Vector3.DistanceSquared(vec, v);
                if (distanceSquared < minDistanceSquared)
                {
                    minDistanceSquared = distanceSquared;
                    closest = v;
                }
            }

            return vectors.IndexOf(closest);
        }
        public static Vector3 FindClosestVector(Vector3 vec, List<CVector3> cvectors)
        {
            List<Vector3> vectors = new List<Vector3>();
            foreach (var v in cvectors) { vectors.Add(new Vector3(v.X, v.Y, v.Z)); }
            if (vectors == null || vectors.Count == 0)
                throw new ArgumentException("Vector list is empty.");

            Vector3 closest = vectors[0];
            float minDistanceSquared = Vector3.DistanceSquared(vec, closest);

            foreach (var v in vectors)
            {
                float distanceSquared = Vector3.DistanceSquared(vec, v);
                if (distanceSquared < minDistanceSquared)
                {
                    minDistanceSquared = distanceSquared;
                    closest = v;
                }
            }

            return closest;
        }
        public static int FindClosestVectorIndex(Vector3 vec, List<CVector3> cvectors)
        {
            List<Vector3> vectors = new List<Vector3>();
            foreach (var v in cvectors) { vectors.Add(new Vector3(v.X, v.Y, v.Z)); }
            if (vectors == null || vectors.Count == 0)
                throw new ArgumentException("Vector list is empty.");

            Vector3 closest = vectors[0];
            float minDistanceSquared = Vector3.DistanceSquared(vec, closest);

            foreach (var v in vectors)
            {
                float distanceSquared = Vector3.DistanceSquared(vec, v);
                if (distanceSquared < minDistanceSquared)
                {
                    minDistanceSquared = distanceSquared;
                    closest = v;
                }
            }

            return vectors.IndexOf(closest);
        }
        public static CVector3 GetPositionAtExtent(CExtent extent, ExtentPosition position)
        {
            // Get min and max bounds
            float minX = extent.Min.X;
            float maxX = extent.Max.X;
            float minY = extent.Min.Y;
            float maxY = extent.Max.Y;
            float minZ = extent.Min.Z;
            float maxZ = extent.Max.Z;

            // Compute center
            float centerX = (minX + maxX) / 2;
            float centerY = (minY + maxY) / 2;
            float centerZ = (minZ + maxZ) / 2;

            return position switch
            {
                ExtentPosition.Center => new CVector3(centerX, centerY, centerZ),

                // Single-axis positions
                ExtentPosition.Top => new CVector3(centerX, maxY, centerZ),
                ExtentPosition.Bottom => new CVector3(centerX, minY, centerZ),
                ExtentPosition.Left => new CVector3(minX, centerY, centerZ),
                ExtentPosition.Right => new CVector3(maxX, centerY, centerZ),
                ExtentPosition.Front => new CVector3(centerX, centerY, maxZ),
                ExtentPosition.Back => new CVector3(centerX, centerY, minZ),

                // Edge positions
                ExtentPosition.TopLeft => new CVector3(minX, maxY, centerZ),
                ExtentPosition.TopRight => new CVector3(maxX, maxY, centerZ),
                ExtentPosition.BottomLeft => new CVector3(minX, minY, centerZ),
                ExtentPosition.BottomRight => new CVector3(maxX, minY, centerZ),

                ExtentPosition.TopFront => new CVector3(centerX, maxY, maxZ),
                ExtentPosition.TopBack => new CVector3(centerX, maxY, minZ),
                ExtentPosition.BottomFront => new CVector3(centerX, minY, maxZ),
                ExtentPosition.BottomBack => new CVector3(centerX, minY, minZ),

                ExtentPosition.LeftFront => new CVector3(minX, centerY, maxZ),
                ExtentPosition.LeftBack => new CVector3(minX, centerY, minZ),
                ExtentPosition.RightFront => new CVector3(maxX, centerY, maxZ),
                ExtentPosition.RightBack => new CVector3(maxX, centerY, minZ),

                // Corner positions
                ExtentPosition.TopLeftFront => new CVector3(minX, maxY, maxZ),
                ExtentPosition.TopLeftBack => new CVector3(minX, maxY, minZ),
                ExtentPosition.TopRightFront => new CVector3(maxX, maxY, maxZ),
                ExtentPosition.TopRightBack => new CVector3(maxX, maxY, minZ),
                ExtentPosition.BottomLeftFront => new CVector3(minX, minY, maxZ),
                ExtentPosition.BottomLeftBack => new CVector3(minX, minY, minZ),
                ExtentPosition.BottomRightFront => new CVector3(maxX, minY, maxZ),
                ExtentPosition.BottomRightBack => new CVector3(maxX, minY, minZ),

                _ => new CVector3(0, 0, 0), // Fallback (should never happen)
            };
        }
        public static CExtent GetExtentFromAttachedVertices(CModel model, INode node)
        {
            // unfinished
            return new CExtent();
        }
        internal static float DegreesToRadians(float value)
        {
            return value * (float)Math.PI / 180f;
        }
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

                 )
            {
                return new CVector4(0, 0, 0, 1);
            }
            return vector;
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
        public static float ClampUV(float f)
        {
            float x = f - (float)Math.Floor(f); // Get fractional part
            return (Math.Floor(f) % 2 == 0) ? x : 1 - x;
        }

        internal static CAnimatorNode<float> ClampFloat(CAnimatorNode<float> v)
        {
            return new CAnimatorNode<float>(v.Time, v.Value < 0 ? 9 : v.Value);
        }


        public static CExtent GetExtent(List<CVector3> vectors)
        {
            if (vectors == null || vectors.Count == 0)
            {
                return new CExtent();
            }

            // Initialize min and max bounds
            CVector3 min = new CVector3(vectors[0].X, vectors[0].Y, vectors[0].Z);
            CVector3 max = new CVector3(vectors[0].X, vectors[0].Y, vectors[0].Z);

            // Find min and max coordinates
            foreach (CVector3 vector in vectors)
            {
                min = new CVector3(Math.Min(min.X, vector.X), Math.Min(min.Y, vector.Y), Math.Min(min.Z, vector.Z));
                max = new CVector3(Math.Max(max.X, vector.X), Math.Max(max.Y, vector.Y), Math.Max(max.Z, vector.Z));
            }

            // Compute the center
            CVector3 center = new CVector3(
                (min.X + max.X) / 2,
                (min.Y + max.Y) / 2,
                (min.Z + max.Z) / 2
            );

            // Compute the maximum squared distance from the center
            float maxDistanceSquared = 0;
            foreach (CVector3 vector in vectors)
            {
                float distanceSquared =
                    (vector.X - center.X) * (vector.X - center.X) +
                    (vector.Y - center.Y) * (vector.Y - center.Y) +
                    (vector.Z - center.Z) * (vector.Z - center.Z);
                maxDistanceSquared = Math.Max(maxDistanceSquared, distanceSquared);
            }

            // Compute the radius once at the end
            float radius = (float)Math.Sqrt(maxDistanceSquared);

            return new CExtent(min, max, radius);
        }

        internal static CExtent CalculateModelExtent(List<CExtent> extents)
        {
            if (extents == null || extents.Count == 0)
            {
                return new CExtent();
            }
            // Temporary min and max holders initialized with the first extent's bounds
            Vector3 min = new  (extents[0].Min.X, extents[0].Min.Y, extents[0].Min.Z);
            Vector3 max = new  (extents[0].Max.X, extents[0].Max.Y, extents[0].Max.Z);
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
            float closestVertexZToGround = FindLowestZ(geoset.Vertices.ObjectList);
            // Move all vertices down by the closest Z value to align to the ground
            foreach (CGeosetVertex vertex in geoset.Vertices)
            {
                float x = vertex.Position.X;
                float y = vertex.Position.Y;
                float z = vertex.Position.Z;
                vertex.Position = new CVector3(x, y, z - closestVertexZToGround);
            }
        }
        internal static void PutOnGround(List<CGeosetVertex> Vertices)
        {
            // Find the lowest Z value among all vertices
            float closestVertexZToGround = FindLowestZ(Vertices);
            // Move all vertices down by the closest Z value to align to the ground
            foreach (CGeosetVertex vertex in Vertices)
            {
                float x = vertex.Position.X;
                float y = vertex.Position.Y;
                float z = vertex.Position.Z;
                vertex.Position = new CVector3(x, y, z - closestVertexZToGround);
            }
        }

         
        private static float FindLowestZ(List<CGeosetVertex> Vertices)
        {
            return Vertices.Min(v => v.Position.Z);
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

            // Get original centroids
            List<CVector3> centroids = geosets.Select(GetCentroidOfGeoset).ToList();
            CVector3 firstCentroid = centroids[0];

            for (int i = 1; i < geosets.Count; i++)
            {
                CVector3 centroid = centroids[i];
                float dx = onX ? (firstCentroid.X - centroid.X) : 0;
                float dy = onY ? (firstCentroid.Y - centroid.Y) : 0;
                float dz = onZ ? (firstCentroid.Z - centroid.Z) : 0;

                foreach (CGeosetVertex vertex in geosets[i].Vertices)
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
        internal static Vector3 BGRnToRGB_Vector(CVector3 v)
        {
            float r = v.Z * 255;
            float g = v.Y * 255;
            float b = v.X * 255;
            return new Vector3(r, g, b);
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
                    Normal = NormalizeVector(new CVector3(x, 0, z))
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
            vertex2.Position = new CVector3(vertex.Position.X, vertex.Position.Y, vertex.Position.Z);
            vertex2.TexturePosition = new CVector2(vertex.TexturePosition.X, vertex.TexturePosition.Y);
            vertex2.Normal = new CVector3(vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z);
            CGeosetGroup group = new CGeosetGroup(owner);
            foreach (var node in vertex.Group.Object.Nodes)
            {
                CGeosetGroupNode n = new CGeosetGroupNode(owner);
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
            INode? first_node = null;
            if (geoset.Groups.Count > 0)
            {
                var group = geoset.Vertices[0].Group.Object;
                first_node = group.Nodes[0].Node.Node;
            }
            else
            {
                if (owner.Nodes.Any(x => x is CBone))
                {
                    first_node = owner.Nodes.First(x => x is CBone);
                }
                else
                {
                    first_node = new CBone(owner);
                    first_node.Name = $"GeneratedBone_{IDCounter.Next}";
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
        static CGeosetVertex? getTopMostVertex(CGeoset geoset)
        {
            CGeosetVertex? topMostVertex = null;
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
        static CGeosetVertex? getBottomMostVertex(CGeoset geoset)
        {
            CGeosetVertex? bottomMostVertex = null;
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
        static CGeosetVertex? getLeftMostVertex(CGeoset geoset)
        {
            CGeosetVertex? leftMostVertex = null;
            float leftMostY = float.MaxValue;
            foreach (var vertex in geoset.Vertices)
            {
                if (vertex.Position.Y < leftMostY)
                {
                    leftMostY = vertex.Position.Y;
                    leftMostVertex = vertex;
                }
            }
            return leftMostVertex;
        }
        static CGeosetVertex? getRightMostVertex(CGeoset geoset)
        {
            CGeosetVertex? rightMostVertex = null;
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
        static CGeosetVertex? getFrontMostVertex(CGeoset geoset)
        {
            CGeosetVertex? frontMostVertex = null;
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
        static CGeosetVertex? getBackMostVertex(CGeoset geoset)
        {
            CGeosetVertex? backMostVertex = null;
            float backMostX = float.MaxValue;
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
                    CGeosetVertex? leftmost = getLeftMostVertex(geoset);
                    if (leftmost == null) return;
                    foreach (var vertex in geoset.Vertices)
                    {
                        if (CoordinateIsAfterCentroid(centroid, vertex.Position, side))
                        {
                            float X = vertex.Position.X;
                            float Y = leftmost.Position.Y;
                            float Z = vertex.Position.Z;
                            vertex.Position = new CVector3(X, Y, Z);
                        }
                    }
                    break;
                case Side.Right:
                    CGeosetVertex? rightmost = getRightMostVertex(geoset);
                    if (rightmost == null) return;
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
                    CGeosetVertex? topmost = getTopMostVertex(geoset);
                    if (topmost == null) return;
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
                    CGeosetVertex? bottommost = getBottomMostVertex(geoset);
                    if (bottommost == null) return;
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
                    CGeosetVertex? frontmost = getFrontMostVertex(geoset);
                    if (frontmost == null) return;
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
                    CGeosetVertex? backmost = getBackMostVertex(geoset);
                    if (backmost == null) return;
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
                face.Vertex3.Attach(vertices[i + 1]);
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
            float r = color.Z;
            float g = color.Y;
            float b = color.X;
            // Clamp values between 0 and 1, then scale to byte (0-255)
            byte red = (byte)(Math.Clamp(r, 0f, 1f) * 255);
            byte green = (byte)(Math.Clamp(g, 0f, 1f) * 255);
            byte blue = (byte)(Math.Clamp(b, 0f, 1f) * 255);

            return new SolidColorBrush(System.Windows.Media.Color.FromRgb(red, green, blue));

        }
        private static System.Windows.Media.Brush BrushFromRGB(float r, float g, float b)
        {
            // Ensure the values are clamped between 0 and 1
            r = Math.Max(0, Math.Min(1, r));
            g = Math.Max(0, Math.Min(1, g));
            b = Math.Max(0, Math.Min(1, b));

            // Convert the values to the 0-255 range
            byte red = (byte)(r * 255f);
            byte green = (byte)(g * 255f);
            byte blue = (byte)(b * 255f);

            // Create and return the SolidColorBrush
            var brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(red, green, blue));
            brush.Freeze(); // Improves performance by making it immutable
            return brush;
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
            float r = color.B / 255f;
            float g = color.G / 255f;
            float b = color.R / 255f;
            return new CVector3(b, g, r);
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
            // Transfer vertices
            var verticesToMove = second.Vertices.ToList();
            Dictionary<CGeosetVertex, CGeosetVertex> reference = new Dictionary<CGeosetVertex, CGeosetVertex>();
            foreach (var vertex in verticesToMove)
            {
                //  second.Vertices.Remove(vertex);
                CGeosetVertex copy = new CGeosetVertex(owner);
                CopyVertex(vertex, copy);
                var group = vertex.Group;
                var groupCopy = CopyVertexGroup(group.Object, owner);
                first.Groups.Add(groupCopy);
                copy.Group.Attach(groupCopy);
                reference.Add(vertex, copy);
                first.Vertices.Add(copy);
            }

            // Transfer triangles
            var trianglesToMove = second.Triangles.ToList();
            foreach (var face in trianglesToMove)
            {
              //  second.Triangles.Remove(face);
                first.Triangles.Add(new CGeosetTriangle(
                    owner,
                   reference[ face.Vertex1.Object],
                     reference[face.Vertex2.Object],
                     reference[face.Vertex3.Object]
                    ));
            }

            // Transfer groups
            var groupsToMove = second.Groups.ToList();
            foreach (var group in groupsToMove)
            {
                second.Groups.Remove(group);
                first.Groups.Add(group);
            }
        }

        private static CGeosetGroup CopyVertexGroup( CGeosetGroup  group, CModel owner)
        {
            //unfinished
            CGeosetGroup _new = new CGeosetGroup(owner);
            foreach (var node in group.Nodes)
            {
                CGeosetGroupNode gnode = new CGeosetGroupNode(owner);
                gnode.Node.Attach(node.Node.Node);
                _new.Nodes.Add(gnode);
            }
            return _new;
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
        internal static void CopyVertex(CGeosetVertex original, CGeosetVertex copy)
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
            if (reference.TryGetValue(originalVertex, out CGeosetVertex? existingVertex))
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
            if (model.Nodes.Any(x => x is CBone))
            {
                return model.Nodes.First(x => x is CBone);
            }
            else
            {
                CBone bone = new CBone(model);
                bone.Name = "GeneratedBone_" + IDCounter.Next_;
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
            float radiansX = Calculator.DegreesToRadians(x);
            float radiansY = Calculator.DegreesToRadians(y);
            float radiansZ = Calculator.DegreesToRadians(z);

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
        internal static CVector3 GetCentroidOfVectors(List<CVector3> vectors)
        {
            if (vectors == null || vectors.Count == 0) return new CVector3(); // Return a default vector if the list is empty
            if (vectors.Count == 1) return vectors[0];


            float sumX = 0, sumY = 0, sumZ = 0;

            foreach (var vector in vectors)
            {

                sumX += vector.X;
                sumY += vector.Y;
                sumZ += vector.Z;
            }

            float count = vectors.Count;
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

        internal static void MirrorGeoset(CGeoset geoset, bool x, bool y, bool z)
        {
            var centroid = GetCentroidOfGeoset(geoset);

            // Negate positions relative to the origin
            foreach (var vertex in geoset.Vertices)
            {
                if (x) vertex.Position.X = -vertex.Position.X;
                if (y) vertex.Position.Y = -vertex.Position.Y;
                if (z) vertex.Position.Z = -vertex.Position.Z;
            }

            // Translate back to original position
            foreach (var vertex in geoset.Vertices)
            {
                if (x) vertex.Position.X += 2 * centroid.X;
                if (y) vertex.Position.Y += 2 * centroid.Y;
                if (z) vertex.Position.Z += 2 * centroid.Z;
            }
        }

        internal static void DistanceGeosetFromPoint(CGeoset geoset, float x, float y, float z, float distance, DistaningMethod method)
        {
            CVector3 fromPoint = new CVector3(x, y, z);

            foreach (var vertex in geoset.Vertices)
            {
                // Manually compute the direction
                float dirX = vertex.Position.X - fromPoint.X;
                float dirY = vertex.Position.Y - fromPoint.Y;
                float dirZ = vertex.Position.Z - fromPoint.Z;

                float currentDistance = (float)Math.Sqrt(dirX * dirX + dirY * dirY + dirZ * dirZ);

                if (currentDistance == 0)
                    continue; // Avoid division by zero

                // Normalize direction
                float normX = dirX / currentDistance;
                float normY = dirY / currentDistance;
                float normZ = dirZ / currentDistance;

                float newDistance = currentDistance; // Start with the current distance

                switch (method)
                {
                    case DistaningMethod.Set:
                        newDistance = distance;
                        break;
                    case DistaningMethod.Add:
                        newDistance += distance;
                        break;
                    case DistaningMethod.Subtract:
                        newDistance -= distance;
                        break;
                    case DistaningMethod.Multiply:
                        newDistance *= distance;
                        break;
                    case DistaningMethod.Divide:
                        if (distance != 0)
                            newDistance /= distance;
                        break;
                    case DistaningMethod.Modulo:
                        if (distance != 0)
                            newDistance %= distance;
                        break;
                }

                // Compute the new position
                float newX = fromPoint.X + normX * newDistance;
                float newY = fromPoint.Y + normY * newDistance;
                float newZ = fromPoint.Z + normZ * newDistance;

                // Assign new position
                vertex.Position = new CVector3(newX, newY, newZ);
            }
        }

        internal static CExtent GetMaxExtent(List<CExtent> extents)
        {
            // Create a new extent with the biggest proportions taken from all geosets
            if (extents.Count == 0) return new CExtent();
            if (extents.Count == 1) return extents[0];

            CExtent max = new CExtent
            {
                Min = new CVector3(float.MaxValue, float.MaxValue, float.MaxValue),
                Max = new CVector3(float.MinValue, float.MinValue, float.MinValue)
            };

            foreach (var extent in extents)
            {
                max.Min = new CVector3(
                    Math.Min(max.Min.X, extent.Min.X),
                    Math.Min(max.Min.Y, extent.Min.Y),
                    Math.Min(max.Min.Z, extent.Min.Z)
                );

                max.Max = new CVector3(
                    Math.Max(max.Max.X, extent.Max.X),
                    Math.Max(max.Max.Y, extent.Max.Y),
                    Math.Max(max.Max.Z, extent.Max.Z)
                );
            }

            return max;
        }

        internal static void ScaleGeoset(CGeoset geoset, CVector3 pivotPoint, CVector3 value)
        {
            foreach (var vertex in geoset.Vertices)
            {
                // Move vertex relative to pivot
                CVector3 relativePosition = new CVector3(
                    vertex.Position.X - pivotPoint.X,
                    vertex.Position.Y - pivotPoint.Y,
                    vertex.Position.Z - pivotPoint.Z
                );

                // Scale the relative position
                relativePosition = new CVector3(
                    relativePosition.X * value.X,
                    relativePosition.Y * value.Y,
                    relativePosition.Z * value.Z
                );

                // Move vertex back
                vertex.Position = new CVector3(
                    pivotPoint.X + relativePosition.X,
                    pivotPoint.Y + relativePosition.Y,
                    pivotPoint.Z + relativePosition.Z
                );
            }
        }


        internal static void RotateGeosetAroundPivotPoint(CVector3 pivotPoint, CGeoset geoset, float x, float y, float z)
        {
            foreach (var vertex in geoset.Vertices)
            {
                // Translate vertex to pivot-centered coordinates
                CVector3 relativePosition = new CVector3(
                    vertex.Position.X - pivotPoint.X,
                    vertex.Position.Y - pivotPoint.Y,
                    vertex.Position.Z - pivotPoint.Z
                );

                // Apply rotation in order: Z → Y → X (or change order if needed)
                relativePosition = RotateAroundZ(relativePosition, z);
                relativePosition = RotateAroundY(relativePosition, y);
                relativePosition = RotateAroundX(relativePosition, x);

                // Translate vertex back to original space
                vertex.Position = new CVector3(
                    pivotPoint.X + relativePosition.X,
                    pivotPoint.Y + relativePosition.Y,
                    pivotPoint.Z + relativePosition.Z
                );
            }
        }

        // Rotates a point around the X-axis
        private static CVector3 RotateAroundX(CVector3 point, float angle)
        {
            float rad = angle * (float)Math.PI / 180f;
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);

            return new CVector3(
                point.X,
                cos * point.Y - sin * point.Z,
                sin * point.Y + cos * point.Z
            );
        }

        // Rotates a point around the Y-axis
        private static CVector3 RotateAroundY(CVector3 point, float angle)
        {
            float rad = angle * (float)Math.PI / 180f;
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);

            return new CVector3(
                cos * point.X + sin * point.Z,
                point.Y,
                -sin * point.X + cos * point.Z
            );
        }

        // Rotates a point around the Z-axis
        private static CVector3 RotateAroundZ(CVector3 point, float angle)
        {
            float rad = angle * (float)Math.PI / 180f;
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);

            return new CVector3(
                cos * point.X - sin * point.Y,
                sin * point.X + cos * point.Y,
                point.Z
            );
        }

        internal static Vector3 GetOutwardFacingDirection(CVector3 one, CVector3 two, CVector3 three)
        {
            // Compute two edge vectors
            Vector3 u = new Vector3(two.X - one.X, two.Y - one.Y, two.Z - one.Z);
            Vector3 v = new Vector3(three.X - one.X, three.Y - one.Y, three.Z - one.Z);

            // Compute normal (cross product)
            Vector3 normal = Vector3.Cross(u, v);

            // Normalize the normal
            normal = Vector3.Normalize(normal);

            // Check if the normal is pointing toward the origin
            Vector3 centroid = new Vector3((one.X + two.X + three.X) / 3,
                                           (one.Y + two.Y + three.Y) / 3,
                                           (one.Z + two.Z + three.Z) / 3);

            Vector3 toOrigin = Vector3.Normalize(new Vector3(0, 0, 0) - centroid);

            // If the normal is pointing toward the origin, flip it
            if (Vector3.Dot(normal, toOrigin) > 0)
            {
                normal = -normal;
            }

            return normal;
        }

        internal static void NegateInside(CVector2 vector, bool u, bool v)
        {
            if (u)
            {
                float minU = (float)Math.Floor(vector.X);
                float maxU = (float)Math.Ceiling(vector.X);
                vector.X = minU + maxU - vector.X;
            }
            if (v)
            {
                float minV = (float)Math.Floor(vector.Y);
                float maxV = (float)Math.Ceiling(vector.Y);
                vector.Y = minV + maxV - vector.Y;
            }
        }

        internal static void SwapUV(CVector2 vector)
        {
            var x = vector.X;
            var y = vector.Y;
            vector.X = y;
            vector.Y = x;
        }

        internal static void SwapTwo(CGeosetVertex v1, CGeosetVertex v2)
        {
            float u1 = v1.TexturePosition.X;
            float v_1 = v1.TexturePosition.Y;


            float u2 = v2.TexturePosition.X;
            float v_2 = v2.TexturePosition.Y;
            v1.TexturePosition = new CVector2(u2, v_2);
            v2.TexturePosition = new CVector2(u1, v_1);
        }



        internal static float GetCanvasPositionFromU(float u, double imageWidth, double canvasWidth)
        {
            // Map UV range (-10 to 10) to canvas coordinates (0 to canvasWidth)
            return (float)(((u + 10) / 20) * canvasWidth);
        }

        internal static float GetCanvasPositionFromV(float v, double imageHeight, double canvasHeight)
        {
            // Map UV range (-10 to 10) to canvas coordinates (0 to canvasHeight), flipping Y-axis
            return (float)((1 - ((v + 10) / 20)) * canvasHeight);
        }

        internal static CGeoset CreateSphere(int sections, int slices, CModel owner)
        {
            if (slices < 3 || sections < 3 || slices > 50 || sections > 50)
            {
                throw new ArgumentException("The number of sides cannot be less than 3 and more than 50.");
            }

            CGeoset geoset = new CGeoset(owner);
            List<CGeosetVertex> vertices = new List<CGeosetVertex>();
            float radius = 1.0f;

            for (int i = 0; i <= slices; i++)
            {
                float phi = (float)(Math.PI * i / slices);
                for (int j = 0; j <= sections; j++)
                {
                    float theta = (float)(2 * Math.PI * j / sections);
                    float x = radius * (float)(Math.Sin(phi) * Math.Cos(theta));
                    float y = radius * (float)(Math.Cos(phi));
                    float z = radius * (float)(Math.Sin(phi) * Math.Sin(theta));

                    CGeosetVertex vertex = new CGeosetVertex(owner);
                    vertex.Position = new CVector3(x, y, z);
                    geoset.Vertices.Add(vertex);
                    vertices.Add(vertex);
                }
            }

            for (int i = 0; i < slices; i++)
            {
                for (int j = 0; j < sections; j++)
                {
                    int current = i * (sections + 1) + j;
                    int next = current + sections + 1;

                    CGeosetTriangle triangle1 = new CGeosetTriangle(owner);
                    triangle1.Vertex1.Attach(vertices[current]);
                    triangle1.Vertex2.Attach(vertices[next]);
                    triangle1.Vertex3.Attach(vertices[current + 1]);
                    geoset.Triangles.Add(triangle1);

                    CGeosetTriangle triangle2 = new CGeosetTriangle(owner);
                    triangle2.Vertex1.Attach(vertices[current + 1]);
                    triangle2.Vertex2.Attach(vertices[next]);
                    triangle2.Vertex3.Attach(vertices[next + 1]);
                    geoset.Triangles.Add(triangle2);
                }
            }

            return geoset;
        }

        internal static void TranslateGeosetsTo(List<CGeoset> list, Axes axes, float value)
        {
            // TO, not BY
            if (list.Count == 0) return;

            if (list.Count == 1)
            {
                CVector3 centroid = GetCentroidOfGeoset(list[0]);
                CVector3 offset = new CVector3(
                    axes == Axes.X ? value - centroid.X : 0,
                    axes == Axes.Y ? value - centroid.Y : 0,
                    axes == Axes.Z ? value - centroid.Z : 0
                );

                foreach (var vertex in list[0].Vertices)
                {
                    vertex.Position = new CVector3(
                        vertex.Position.X + offset.X,
                        vertex.Position.Y + offset.Y,
                        vertex.Position.Z + offset.Z
                    );
                }
            }
            else
            {
                CVector3 centroid = GetCentroidOfGeosets(list);
                CVector3 offset = new CVector3(
                    axes == Axes.X ? value - centroid.X : 0,
                    axes == Axes.Y ? value - centroid.Y : 0,
                    axes == Axes.Z ? value - centroid.Z : 0
                );

                foreach (var geoset in list)
                {
                    foreach (var vertex in geoset.Vertices)
                    {
                        vertex.Position = new CVector3(
                            vertex.Position.X + offset.X,
                            vertex.Position.Y + offset.Y,
                            vertex.Position.Z + offset.Z
                        );
                    }
                }
            }
        }


        public static CVector3 GetCentroidOfGeosets(List<CGeoset> list)
        {
            if (list == null || list.Count == 0)
                return new CVector3(); // Return default if no geosets

            CVector3 total = new CVector3();
            int count = 0;

            foreach (var geoset in list)
            {
                foreach (var vertex in geoset.Vertices)
                {
                    total = new CVector3(
                        total.X + vertex.Position.X,
                        total.Y + vertex.Position.Y,
                        total.Z + vertex.Position.Z
                    );
                    count++;
                }
            }

            return count > 0
                ? new CVector3(total.X / count, total.Y / count, total.Z / count)
                : new CVector3();
        }

        internal static void ScaleGeosets(List<CGeoset> list, Axes ax, float value)
        {
            if (value <= 0) return;
            foreach (var geoset in list)
            {
                foreach (var v in geoset.Vertices)
                {
                    v.Position.X *= ax == Axes.X ? (value / 100) : 1;
                    v.Position.Y *= ax == Axes.Y ? (value / 100) : 1;
                    v.Position.Z *= ax == Axes.Z ? (value / 100) : 1;
                }
            }
        }

        internal static void TranslateVectors(List<CVector3> vectors, Axes x, float value, bool add = false)
        {
            if (vectors.Count == 0) return;

            if (vectors.Count == 1)
            {
                CVector3 v = vectors[0];
                float newValue = add ? (x == Axes.X ? v.X + value : v.X) :
                                     (x == Axes.X ? value : v.X);
                vectors[0] = new CVector3(
                    x == Axes.X ? newValue : v.X,
                    x == Axes.Y ? (add ? v.Y + value : value) : v.Y,
                    x == Axes.Z ? (add ? v.Z + value : value) : v.Z
                );
                return;
            }

            CVector3 centroid = GetCentroidOfVectors(vectors);
            float delta = value - (x == Axes.X ? centroid.X :
                                   x == Axes.Y ? centroid.Y :
                                                 centroid.Z);

            for (int i = 0; i < vectors.Count; i++)
            {
                CVector3 v = vectors[i];
                float newX = v.X + (x == Axes.X ? delta : 0);
                float newY = v.Y + (x == Axes.Y ? delta : 0);
                float newZ = v.Z + (x == Axes.Z ? delta : 0);
                if (add)
                {
                    vectors[i] = new CVector3(v.X + newX, v.Y + newY, v.Z + newZ);
                }
                else
                {
                    vectors[i] = new CVector3(newX, newY, newZ);
                }
            }
        }

        internal static void TranslateNodes(List<INode> nodes, Axes axis, float value)
        {
            if (nodes.Count == 0) return;

            // Get centroid of pivot points
            CVector3 centroid = GetCentroidOfVectors(nodes.Select(n => n.PivotPoint).ToList());

            // Calculate shift amount for the chosen axis
            float deltaX = axis == Axes.X ? value - centroid.X : 0;
            float deltaY = axis == Axes.Y ? value - centroid.Y : 0;
            float deltaZ = axis == Axes.Z ? value - centroid.Z : 0;

            // Apply translation to all nodes
            for (int i = 0; i < nodes.Count; i++)
            {
                var v = nodes[i].PivotPoint;
                nodes[i].PivotPoint = new CVector3(v.X + deltaX, v.Y + deltaY, v.Z + deltaZ);
            }
        }

        internal static void RotateNodes(List<INode> nodes, Axes axis, float value)
        {
            if (nodes.Count == 0) return;
            if (value < -360 || value > 360) return;

            CVector3 centroid = GetCentroidOfVectors(nodes.Select(n => n.PivotPoint).ToList());

            float radians = value * (float)(Math.PI / 180.0); // Convert degrees to radians

            foreach (var node in nodes)
            {
                CVector3 v = node.PivotPoint - centroid; // Translate to origin
                CVector3 rotated;

                switch (axis)
                {
                    case Axes.X:
                        rotated = new CVector3(
                            v.X,
                            v.Y * (float)Math.Cos(radians) - v.Z * (float)Math.Sin(radians),
                            v.Y * (float)Math.Sin(radians) + v.Z * (float)Math.Cos(radians)
                        );
                        break;
                    case Axes.Y:
                        rotated = new CVector3(
                            v.X * (float)Math.Cos(radians) + v.Z * (float)Math.Sin(radians),
                            v.Y,
                            -v.X * (float)Math.Sin(radians) + v.Z * (float)Math.Cos(radians)
                        );
                        break;
                    case Axes.Z:
                        rotated = new CVector3(
                            v.X * (float)Math.Cos(radians) - v.Y * (float)Math.Sin(radians),
                            v.X * (float)Math.Sin(radians) + v.Y * (float)Math.Cos(radians),
                            v.Z
                        );
                        break;
                    default:
                        return;
                }

                node.PivotPoint = rotated + centroid; // Translate back
            }
        }

        internal static void ScaleNodes(List<INode> nodes, Axes axis, float value)
        {
            if (nodes.Count < 2) return;
            if (value <= 0) return;

            float scaleFactor = value / 100.0f; // Normalize the scaling value

            CVector3 centroid = GetCentroidOfVectors(nodes.Select(n => n.PivotPoint).ToList());

            foreach (var node in nodes)
            {
                CVector3 v = node.PivotPoint - centroid; // Translate to origin

                CVector3 scaled = new CVector3(
                    v.X * (axis == Axes.X ? scaleFactor : 1),
                    v.Y * (axis == Axes.Y ? scaleFactor : 1),
                    v.Z * (axis == Axes.Z ? scaleFactor : 1)
                );

                node.PivotPoint = scaled + centroid; // Translate back
            }
        }

        private static CExtent GetExtentOfVectorsCollection(List<CVector3> vectors)
        {
            if (vectors == null || vectors.Count == 0)
                throw new ArgumentException("Vector collection cannot be null or empty.");

            CExtent extent = new CExtent();
            extent.Min = new CVector3(vectors[0]);
            extent.Max = new CVector3(vectors[0]);

            foreach (var vec in vectors)
            {
                extent.Min.X = Math.Min(extent.Min.X, vec.X);
                extent.Min.Y = Math.Min(extent.Min.Y, vec.Y);
                extent.Min.Z = Math.Min(extent.Min.Z, vec.Z);

                extent.Max.X = Math.Max(extent.Max.X, vec.X);
                extent.Max.Y = Math.Max(extent.Max.Y, vec.Y);
                extent.Max.Z = Math.Max(extent.Max.Z, vec.Z);
            }

            return extent;
        }

        public static void MirrorNodePositions(List<INode> Nodes, Axes axis)
        {
            if (Nodes.Count < 2) return;
            var extent = GetExtentOfVectorsCollection(Nodes.Select(X => X.PivotPoint).ToList());

            float midX = (extent.Min.X + extent.Max.X) / 2f;
            float midY = (extent.Min.Y + extent.Max.Y) / 2f;
            float midZ = (extent.Min.Z + extent.Max.Z) / 2f;

            for (int i = 0; i < Nodes.Count; i++)
            {
                var vec = new CVector3(Nodes[i].PivotPoint);

                switch (axis)
                {
                    case Axes.X:
                        Nodes[i].PivotPoint = new CVector3(2 * midX - vec.X, vec.Y, vec.Z);
                        break;
                    case Axes.Y:
                        Nodes[i].PivotPoint = new CVector3(vec.X, 2 * midY - vec.Y, vec.Z);
                        break;
                    case Axes.Z:
                        Nodes[i].PivotPoint = new CVector3(vec.X, vec.Y, 2 * midZ - vec.Z);
                        break;
                }
            }
        }

        internal static void DisperceVectors(List<CVector3> vectors)
        {
            var centroid = GetCentroidOfVectors(vectors);
            Random random = new Random();

            foreach (var vec in vectors)
            {
                // Compute direction from centroid
                var direction = new CVector3(vec.X - centroid.X, vec.Y - centroid.Y, vec.Z - centroid.Z);

                // Avoid zero-length direction
                float length = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y + direction.Z * direction.Z);
                if (length > 0.001f) // Ensure it's not too small
                {
                    direction.X /= length;
                    direction.Y /= length;
                    direction.Z /= length;
                }
                else
                {
                    // If too close to the centroid, assign a random direction
                    direction = new CVector3(
                        GetRandomFloat(-1, 1, 2),
                        GetRandomFloat(-1, 1, 2),
                        GetRandomFloat(-1, 1, 2)
                    );
                }

                // Increase the displacement range
                float offsetMagnitude = GetRandomFloat(5, 15, 2); // Increase the range significantly
                vec.X += direction.X * offsetMagnitude;
                vec.Y += direction.Y * offsetMagnitude;
                vec.Z += direction.Z * offsetMagnitude;
            }

            // Ensure the updated positions are reflected in the rendering

        }

        public static float GetRandomFloat(float min, float max, int Precision)
        {
            Random random = new Random();
            double scale = Math.Pow(10, Precision);
            return (float)(Math.Round(random.NextDouble() * (max - min) + min, Precision));
        }

        internal static CVector4 ComputeQuaternionChange(CVector4 LastQuaternionChange, CVector4 NewValue)
        {
            return MultiplyQuaternions(NewValue, LastQuaternionChange);
        }
        private static CVector4 MultiplyQuaternions(CVector4 q1, CVector4 q2)
        {
            float w1 = q1.W, x1 = q1.X, y1 = q1.Y, z1 = q1.Z;
            float w2 = q2.W, x2 = q2.X, y2 = q2.Y, z2 = q2.Z;

            return new CVector4(
                w1 * x2 + x1 * w2 + y1 * z2 - z1 * y2,  // X
                w1 * y2 - x1 * z2 + y1 * w2 + z1 * x2,  // Y
                w1 * z2 + x1 * y2 - y1 * x2 + z1 * w2,  // Z
                w1 * w2 - x1 * x2 - y1 * y2 - z1 * z2   // W
            );
        }


        internal static CVector3 RotateVertex(CVector3 vertexPosition, INode AroundNode, CVector4 RotationValues)
        {
            // RotationValues is a quaternion

            CVector3 nodePosition = AroundNode.PivotPoint;

            // Translate vertex to be relative to the node
            CVector3 relativePosition = vertexPosition - nodePosition;

            // Apply quaternion rotation
            CVector3 rotatedPosition = RotateByQuaternion(relativePosition, RotationValues);

            // Translate back
            return rotatedPosition + nodePosition;
        }
        // Helper function to rotate a vector using a quaternion
        private static CVector3 RotateByQuaternion(CVector3 v, CVector4 q)
        {
            // Convert quaternion to vector components
            float x = q.X, y = q.Y, z = q.Z, w = q.W;

            // Quaternion multiplication formula for rotating a vector
            float num1 = 2 * (y * v.Z - z * v.Y);
            float num2 = 2 * (z * v.X - x * v.Z);
            float num3 = 2 * (x * v.Y - y * v.X);

            return new CVector3(
                v.X + w * num1 + (y * num3 - z * num2),
                v.Y + w * num2 + (z * num1 - x * num3),
                v.Z + w * num3 + (x * num2 - y * num1)
            );
        }

        internal static CVector3 ScaleVertexRelativeToNode(CGeosetVertex WhichVertex, INode AroundNode, CVector3 ScalingValues)
        {
            // ScalingValues are normalized
            CVector3 vertexPosition = WhichVertex.AnimatedPosition;
            CVector3 nodePosition = AroundNode.PivotPoint;

            // Translate vertex to be relative to the node
            CVector3 relativePosition = vertexPosition - nodePosition;

            // Apply scaling
            CVector3 scaledPosition = new CVector3(
                relativePosition.X * ScalingValues.X,
                relativePosition.Y * ScalingValues.Y,
                relativePosition.Z * ScalingValues.Z
            );

            // Translate back
            return scaledPosition + nodePosition;
        }

        internal static void CenterVerticesAtVector(CVector3 pivotPoint, List<CGeosetVertex> vertices)
        {
            if (vertices == null || vertices.Count == 0)
                return;

            CVector3 centroid = GetCentroidOfVertices(vertices);

            // Compute the translation needed to move the centroid to the pivot point
            CVector3 translation = new CVector3(
                pivotPoint.X - centroid.X,
                pivotPoint.Y - centroid.Y,
                pivotPoint.Z - centroid.Z
            );

            // Apply the translation to all vertices
            foreach (var vertex in vertices)
            {
                vertex.Position = new CVector3(
                    vertex.Position.X + translation.X,
                    vertex.Position.Y + translation.Y,
                    vertex.Position.Z + translation.Z
                );
            }
        }

        internal static void MirrorVertices(Axes ax, List<CGeosetVertex> vertices)
        {
            //unfinished
            throw new NotImplementedException();
        }

        internal static void SwapTwoTriangles(CGeosetTriangle t1, CGeosetTriangle t2)
        {
            // Swap vertex positions between t1 and t2
            CVector3 temp1 = t1.Vertex1.Object.Position;
            CVector3 temp2 = t1.Vertex2.Object.Position;
            CVector3 temp3 = t1.Vertex3.Object.Position;

            t1.Vertex1.Object.Position = new CVector3(t2.Vertex1.Object.Position);
            t1.Vertex2.Object.Position = new CVector3(t2.Vertex2.Object.Position);
            t1.Vertex3.Object.Position = new CVector3(t2.Vertex3.Object.Position);

            t2.Vertex1.Object.Position = new CVector3(temp1);
            t2.Vertex2.Object.Position = new CVector3(temp2);
            t2.Vertex3.Object.Position = new CVector3(temp3);
        }

        internal static void CenterGeosetsAtGeoset(List<CGeoset> geosets, CGeoset atGeoset)
        {
            if (atGeoset == null) return;
            if (geosets == null) return;
            if (geosets.Count == 0) return;
            if (geosets.Count == 1)
            {
                if (geosets[0] == atGeoset) return;

                var singleGeosetCentroid = GetCentroidOfGeoset(geosets[0]);
                var targetCentroid = GetCentroidOfGeoset(atGeoset);

                var offset = targetCentroid - singleGeosetCentroid;
                MoveGeoset(geosets[0], offset);
            }
            else
            {
                var geosetsCentroid = GetCentroidOfGeosets(geosets);
                var targetCentroid = GetCentroidOfGeoset(atGeoset);

                var offset = targetCentroid - geosetsCentroid;

                foreach (var geoset in geosets)
                {
                    MoveGeoset(geoset, offset);
                }
            }
        }
        private static void MoveGeoset(CGeoset geoset, CVector3 offset)
        {
            foreach (var vertex in geoset.Vertices)
            {
                vertex.Position = new CVector3(vertex.Position.X + offset.X, vertex.Position.Y + offset.Y, vertex.Position.Z + offset.Z);
            }
        }

        internal static void RotateVectors(List<CVector3> vectors, Axes axis, float value)
        {
            if (value < -360 || value > 360) return;

            CVector3 centroid = GetCentroidOfVectors(vectors);
            float radians = value * (float)Math.PI / 180f;

            foreach (var vector in vectors)
            {
                // Translate to origin (centroid-based rotation)
                float x = vector.X - centroid.X;
                float y = vector.Y - centroid.Y;
                float z = vector.Z - centroid.Z;

                // Apply rotation depending on the axis
                if (axis == Axes.X)
                {
                    float newY = y * (float)Math.Cos(radians) - z * (float)Math.Sin(radians);
                    float newZ = y * (float)Math.Sin(radians) + z * (float)Math.Cos(radians);
                    vector.Y = newY + centroid.Y;
                    vector.Z = newZ + centroid.Z;
                }
                else if (axis == Axes.Y)
                {
                    float newX = x * (float)Math.Cos(radians) + z * (float)Math.Sin(radians);
                    float newZ = -x * (float)Math.Sin(radians) + z * (float)Math.Cos(radians);
                    vector.X = newX + centroid.X;
                    vector.Z = newZ + centroid.Z;
                }
                else if (axis == Axes.Z)
                {
                    float newX = x * (float)Math.Cos(radians) - y * (float)Math.Sin(radians);
                    float newY = x * (float)Math.Sin(radians) + y * (float)Math.Cos(radians);
                    vector.X = newX + centroid.X;
                    vector.Y = newY + centroid.Y;
                }
            }
        }

        internal static CVector3 RotateVector(CVector3 vector, CVector3 around, CVector3 by)
        {
            // Step 1: Normalize the axis (around vector)
            float length = (float)Math.Sqrt(around.X * around.X + around.Y * around.Y + around.Z * around.Z);
            CVector3 axis = new CVector3(around.X / length, around.Y / length, around.Z / length);

            // Step 2: Calculate the angle (assuming by vector represents the angle in radians)
            float angle = by.X; // Assuming 'by.X' represents the angle in radians. If it's different, adjust accordingly.
            float cosAngle = (float)Math.Cos(angle);
            float sinAngle = (float)Math.Sin(angle);

            // Step 3: Create the quaternion for the rotation
            float qx = axis.X * sinAngle;
            float qy = axis.Y * sinAngle;
            float qz = axis.Z * sinAngle;
            float qw = cosAngle;

            // Step 4: Apply the quaternion rotation formula

            // Quaternion * vector
            float vx = vector.X;
            float vy = vector.Y;
            float vz = vector.Z;

            // Quaternion conjugate (reverse the sign of the vector part)
            float qConjugateX = -qx;
            float qConjugateY = -qy;
            float qConjugateZ = -qz;

            // Quaternion multiplication: q * v * q^-1
            float resX = qw * vx + qy * vz - qz * vy;
            float resY = qw * vy + qz * vx - qx * vz;
            float resZ = qw * vz + qx * vy - qy * vx;

            // Now applying the conjugate (reverse the sign of the vector part)
            resX = qw * resX + qy * resZ - qz * resY;
            resY = qw * resY + qz * resX - qx * resZ;
            resZ = qw * resZ + qx * resY - qy * resX;

            return new CVector3(resX, resY, resZ);
        }

        internal static CVector3 GetCentroidofTriangle(CGeosetTriangle triangle)
        {
            return GetCentroidOfVertices(new List<CGeosetVertex>() { triangle.Vertex1.Object, triangle.Vertex2.Object, triangle.Vertex3.Object });
        }
        internal static  Vector3 GetCentroidofTriangleC(CGeosetTriangle triangle)
        {
            CVector3 c =  GetCentroidOfVertices(new List<CGeosetVertex>() { triangle.Vertex1.Object, triangle.Vertex2.Object, triangle.Vertex3.Object });
        return new Vector3(c.X, c.Y, c.Z);  
        }
        internal static CVector2 GetCentroidUVFromTriangle(CGeosetTriangle triangle)
        {
            CVector2 uv1 = triangle.Vertex1.Object.TexturePosition;
            CVector2 uv2 = triangle.Vertex2.Object.TexturePosition;
            CVector2 uv3 = triangle.Vertex3.Object.TexturePosition;

            return new CVector2((uv1.X + uv2.X + uv3.X) / 3, (uv1.Y + uv2.Y + uv3.Y) / 3);
        }

        internal static CVector3 GetMiddleNormalOfTriangle(CGeosetTriangle triangle)
        {
            CVector3 normal1 = triangle.Vertex1.Object.Normal;
            CVector3 normal2 = triangle.Vertex2.Object.Normal;
            CVector3 normal3 = triangle.Vertex3.Object.Normal;

            // Compute the average normal
            CVector3 averageNormal = new CVector3(
                (normal1.X + normal2.X + normal3.X) / 3,
                (normal1.Y + normal2.Y + normal3.Y) / 3,
                (normal1.Z + normal2.Z + normal3.Z) / 3
            );

            // Normalize manually
            float length = (float)Math.Sqrt(averageNormal.X * averageNormal.X +
                                            averageNormal.Y * averageNormal.Y +
                                            averageNormal.Z * averageNormal.Z);

            return length == 0 ? new CVector3(0, 0, 0) // Avoid division by zero
                               : new CVector3(averageNormal.X / length,
                                              averageNormal.Y / length,
                                              averageNormal.Z / length);
        }

        internal static void InsetTriangle(CGeoset geoset, CGeosetTriangle triangle, CModel owner)
        {
            var vectors = ScaleDownTriangle(triangle, 10);
            CGeosetVertex v1 = new CGeosetVertex(owner);
            CGeosetVertex v2 = new CGeosetVertex(owner);
            CGeosetVertex v3 = new CGeosetVertex(owner);
            CGeosetTriangle t = new CGeosetTriangle(owner);
            CopyTriangleData(triangle.Vertex1.Object, v1);
            CopyTriangleData(triangle.Vertex2.Object, v1);
            CopyTriangleData(triangle.Vertex3.Object, v1);
            v1.Position = new CVector3(vectors[0]);
            v2.Position = new CVector3(vectors[1]);
            v3.Position = new CVector3(vectors[2]);
            t.Vertex1.Attach(v1);
            t.Vertex1.Attach(v2);
            t.Vertex1.Attach(v3);
            geoset.Vertices.Add(v1);
            geoset.Vertices.Add(v2);
            geoset.Vertices.Add(v3);
            geoset.Triangles.Add(t);


        }

        private static void CopyTriangleData(CGeosetVertex @object, CGeosetVertex v1)
        {
            v1.Position = new CVector3(@object.Position);
            v1.Normal = new CVector3(@object.Normal);
            v1.TexturePosition = new CVector2(@object.TexturePosition);
        }

        internal static object InsetTriangleConnected(CGeoset geoset, CGeosetTriangle triangle)
        {
            throw new NotImplementedException();
        }
        internal static CVector3[] ScaleDownTriangle(CGeosetTriangle triangle, int percentage)
        {
            // Convert percentage to a scale factor (e.g., 50% -> 0.5)
            float scaleFactor = percentage / 100f;

            // Calculate the centroid of the triangle
            CVector3 centroid = new CVector3(
                (triangle.Vertex1.Object.Position.X + triangle.Vertex2.Object.Position.X + triangle.Vertex3.Object.Position.X) / 3,
                (triangle.Vertex1.Object.Position.Y + triangle.Vertex2.Object.Position.Y + triangle.Vertex3.Object.Position.Y) / 3,
                (triangle.Vertex1.Object.Position.Z + triangle.Vertex2.Object.Position.Z + triangle.Vertex3.Object.Position.Z) / 3
            );

            // Compute new positions
            CVector3 newVertex1 = ScalePointTowards(centroid, triangle.Vertex1.Object.Position, scaleFactor);
            CVector3 newVertex2 = ScalePointTowards(centroid, triangle.Vertex2.Object.Position, scaleFactor);
            CVector3 newVertex3 = ScalePointTowards(centroid, triangle.Vertex3.Object.Position, scaleFactor);

            return new CVector3[] { newVertex1, newVertex2, newVertex3 };
        }

        // Helper function to move a point towards the centroid
        private static CVector3 ScalePointTowards(CVector3 centroid, CVector3 point, float scaleFactor)
        {
            return new CVector3(
                centroid.X + (point.X - centroid.X) * scaleFactor,
                centroid.Y + (point.Y - centroid.Y) * scaleFactor,
                centroid.Z + (point.Z - centroid.Z) * scaleFactor
            );
        }

        internal static CVector3 GetCentroidofTriangles(List<CGeosetTriangle> triangles)
        {
            if (triangles == null || triangles.Count == 0)
                return new CVector3(0, 0, 0); // Return a default zero vector if no triangles

            CVector3 sum = new CVector3(0, 0, 0);
            int totalVertices = 0;

            foreach (var triangle in triangles)
            {
                CVector3 pos1 = triangle.Vertex1.Object.Position;
                CVector3 pos2 = triangle.Vertex2.Object.Position;
                CVector3 pos3 = triangle.Vertex3.Object.Position;

                sum += pos1 + pos2 + pos3;
                totalVertices += 3;
            }

            float scale = 1.0f / totalVertices; // Convert division into multiplication
            return new CVector3(sum.X * scale, sum.Y * scale, sum.Z * scale);
        }

        internal static CVector2 GetCentroidOfUV(List<CVector2> list)
        {
            if (list == null || list.Count == 0) { return new CVector2(); }
            if (list.Count == 1) { return list[0]; }

            float sumX = 0, sumY = 0;
            foreach (var uv in list)
            {
                sumX += uv.X;
                sumY += uv.Y;
            }

            return new CVector2(sumX / list.Count, sumY / list.Count);
        }

        internal static CVector2 RotateUVAroundCentroid(CVector2 centroid, CVector2 texturePosition, float angleDegrees)
        {
            // Convert degrees to radians
            float angleRadians = angleDegrees * (float)(Math.PI / 180.0);

            // Translate the point to the origin (centroid as the origin)
            float translatedX = texturePosition.X - centroid.X;
            float translatedY = texturePosition.Y - centroid.Y;

            // Apply rotation
            float cosA = (float)Math.Cos(angleRadians);
            float sinA = (float)Math.Sin(angleRadians);

            float rotatedX = translatedX * cosA - translatedY * sinA;
            float rotatedY = translatedX * sinA + translatedY * cosA;

            // Translate back to the original position
            return new CVector2(rotatedX + centroid.X, rotatedY + centroid.Y);
        }


        internal static CVector2 ScaleUVAroundCentroid(CVector2 centroid, CVector2 texturePosition, float scaleFactor)
        {
            // Translate the point to the origin (centroid as the origin)
            float translatedX = texturePosition.X - centroid.X;
            float translatedY = texturePosition.Y - centroid.Y;

            // Apply scaling
            float scaledX = translatedX * scaleFactor;
            float scaledY = translatedY * scaleFactor;

            // Translate back to the original position
            return new CVector2(scaledX + centroid.X, scaledY + centroid.Y);
        }

        internal static void Straighten(List<CGeoset> geosets, Axes straightenForAxis)
        {
            CVector3 centroid = GetCentroid(geosets);

            // Estimate the orientation by PCA or just bounding box alignment
            CVector3 direction = GetDominantDirection(geosets, straightenForAxis);

            // Get rotation to align that direction with selected axis
            CVector3 targetDirection = straightenForAxis switch
            {
                Axes.X => new CVector3(1, 0, 0),
                Axes.Y => new CVector3(0, 1, 0),
                Axes.Z => new CVector3(0, 0, 1),
                _ => throw new ArgumentOutOfRangeException()
            };

            Quaternion rotation = GetRotationBetween(direction, targetDirection);

            // Apply rotation around centroid
            foreach (var geoset in geosets)
            {
                foreach (var vertex in geoset.Vertices)
                {
                    CVector3 relative = vertex.Position - centroid;

                    // Rotate the vector using the quaternion
                    Vector3 rotated = RotateVectorByQuaternion(relative, rotation);

                    // Update the vertex position
                    vertex.Position = centroid + new CVector3(rotated.X, rotated.Y, rotated.Z);
                }
            }
        }

        // Helper function to apply quaternion rotation to a vector
        private static Vector3 RotateVectorByQuaternion(CVector3 vector, Quaternion quaternion)
        {
            // Convert vector to a quaternion (with 0 as scalar part)
            Quaternion vectorQuat = new Quaternion(vector.X, vector.Y, vector.Z, 0);

            // Apply the rotation: q * v * q^-1
            Quaternion result = Quaternion.Conjugate(quaternion) * vectorQuat * quaternion;

            // The rotated vector is in the vector part of the result quaternion
            return new Vector3(result.X, result.Y, result.Z);
        }

        private static CVector3 GetDominantDirection(List<CGeoset> geosets, Axes forAxis)
        {
            // Project all vertices onto the two axes not being straightened
            List<CVector3> projected = new List<CVector3>();

            foreach (var geoset in geosets)
            {
                foreach (var vertex in geoset.Vertices)
                {
                    CVector3 pos = vertex.Position;

                    CVector3 flatPos = forAxis switch
                    {
                        Axes.X => new CVector3(0, pos.Y, pos.Z),
                        Axes.Y => new CVector3(pos.X, 0, pos.Z),
                        Axes.Z => new CVector3(pos.X, pos.Y, 0),
                        _ => pos
                    };

                    projected.Add(flatPos);
                }
            }

            // Compute direction from PCA-like spread (just take the difference between max and min)
            CVector3 min = new CVector3(float.MaxValue, float.MaxValue, float.MaxValue);
            CVector3 max = new CVector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (var p in projected)
            {
                min = CVector3.Min(min, p);
                max = CVector3.Max(max, p);
            }

            return (max - min).Normalize(); // Direction of spread
        }
        private static Quaternion GetRotationBetween(CVector3 from_, CVector3 to_)
        {

            // Normalize both vectors
            Vector3 from = new Vector3(from_.X, from_.Y, from_.Z);

            Vector3 to = new Vector3(to_.X, to_.Y, to_.Z);

            // Calculate the dot product
            float dot = Vector3.Dot(from, to);

            // If the vectors are nearly identical, no rotation is needed
            if (dot >= 1.0f) return Quaternion.Identity;

            // If the vectors are opposite, rotate 180 degrees around an arbitrary perpendicular axis
            if (dot <= -1.0f)
            {
                Vector3 axis = Vector3.Cross(from, new Vector3(1, 0, 0));
                if (axis.Length() < 1e-6f) axis = Vector3.Cross(from, new Vector3(0, 1, 0));
                return Quaternion.CreateFromAxisAngle(Vector3.Normalize(axis), MathF.PI);
            }

            // Otherwise, calculate the axis of rotation
            Vector3 rotationAxis = Vector3.Cross(from, to);
            float angle = MathF.Acos(dot);

            // Return the quaternion representing the rotation
            return Quaternion.CreateFromAxisAngle(Vector3.Normalize(rotationAxis), angle);
        }

        private static CVector3 Cross_(CVector3 from, CVector3 cVector3)
        {
            throw new NotImplementedException();
        }

        internal static CVector3 RGB_Vector_to_BGR(Vector3 vector3)
        {
            float r = vector3.X / 255;
            float g = vector3.Y / 255;
            float b = vector3.Z / 255;
            return new CVector3(b, g, r);
        }

        internal static CAnimatorNode<CVector3>? ReadLine3(string? line)
        {
            if (line == null) return null;
            try
            {
                // Remove all whitespace characters (spaces, tabs, etc.)
                line = System.Text.RegularExpressions.Regex.Replace(line, @"\s+", "");

                int colonIndex = line.IndexOf(':');
                if (colonIndex == -1) return null;

                string timeStr = line.Substring(0, colonIndex);
                string vectorStr = line.Substring(colonIndex + 1);

                if (!vectorStr.StartsWith("{") || !vectorStr.EndsWith("}"))
                    return null;

                vectorStr = vectorStr.Substring(1, vectorStr.Length - 2); // remove { and }

                string[] parts = vectorStr.Split(',');
                if (parts.Length != 3) return null;

                int time = int.Parse(timeStr);
                float x = float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
                float y = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                float z = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);

                return new CAnimatorNode<CVector3>(time, new CVector3(x, y, z));
            }
            catch
            {
                return null;
            }
        }

        internal static CAnimatorNode<CVector4>? ReadLine4(string? line)
        {
            if (line == null) return null;
            try
            {
                // Remove all whitespace characters (spaces, tabs, etc.)
                line = System.Text.RegularExpressions.Regex.Replace(line, @"\s+", "");

                int colonIndex = line.IndexOf(':');
                if (colonIndex == -1) return null;

                string timeStr = line.Substring(0, colonIndex);
                string vectorStr = line.Substring(colonIndex + 1);

                if (!vectorStr.StartsWith("{") || !vectorStr.EndsWith("}"))
                    return null;

                vectorStr = vectorStr.Substring(1, vectorStr.Length - 2); // remove { and }

                string[] parts = vectorStr.Split(',');
                if (parts.Length != 4) return null;

                int time = int.Parse(timeStr);
                float x = float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
                float y = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                float z = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                float w = float.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture);

                return new CAnimatorNode<CVector4>(time, new CVector4(x, y, z, w));
            }
            catch
            {
                return null;
            }
        }

        internal static INode? GetClosestNode(CVector3 position, List<INode> nodes)
        {
            INode? closestNode = null;
            float closestDistanceSquared = float.MaxValue;

            foreach (INode node in nodes)
            {
                CVector3 pivot = node.PivotPoint;
                float dx = pivot.X - position.X;
                float dy = pivot.Y - position.Y;
                float dz = pivot.Z - position.Z;
                float distanceSquared = dx * dx + dy * dy + dz * dz;

                if (distanceSquared < closestDistanceSquared)
                {
                    closestDistanceSquared = distanceSquared;
                    closestNode = node;
                }
            }

            return closestNode;
        }

        internal static void ForceNormalsDirection(CGeosetTriangle triangle, bool inward)
        {
            // Get vertex positions
            var pos1 = triangle.Vertex1.Object.Position;
            var pos2 = triangle.Vertex2.Object.Position;
            var pos3 = triangle.Vertex3.Object.Position;

            // Step 1: Compute the triangle's face normal using cross product
            CVector3 edge1 = new CVector3(pos2.X - pos1.X, pos2.Y - pos1.Y, pos2.Z - pos1.Z);
            CVector3 edge2 = new CVector3(pos3.X - pos1.X, pos3.Y - pos1.Y, pos3.Z - pos1.Z);
            CVector3 faceNormal = Cross(edge1, edge2);
            faceNormal = Normalize(faceNormal);

            // Step 2: Determine the triangle's center
            CVector3 center = new CVector3(
                (pos1.X + pos2.X + pos3.X) / 3.0f,
                (pos1.Y + pos2.Y + pos3.Y) / 3.0f,
                (pos1.Z + pos2.Z + pos3.Z) / 3.0f
            );

            // Step 3: Get vector from center to origin (0,0,0)
            CVector3 toOrigin = new CVector3(-center.X, -center.Y, -center.Z);
            toOrigin = Normalize(toOrigin);

            // Step 4: Determine if the triangle currently faces inward or outward
            float dot = Dot(faceNormal, toOrigin);
            bool currentlyInward = dot > 0;

            // Step 5: Flip face normal direction if needed
            if (currentlyInward != inward)
            {
                faceNormal = new CVector3(-faceNormal.X, -faceNormal.Y, -faceNormal.Z);
            }

            // Step 6: Apply this direction to all three vertex normals
            triangle.Vertex1.Object.Normal = faceNormal;
            triangle.Vertex2.Object.Normal = faceNormal;
            triangle.Vertex3.Object.Normal = faceNormal;
        }
        private static CVector3 Cross(CVector3 a, CVector3 b)
        {
            return new CVector3(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
            );
        }

        private static float Dot(CVector3 a, CVector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        private static CVector3 Normalize(CVector3 v)
        {
            float length = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
            if (length == 0) return new CVector3(0, 0, 0);
            return new CVector3(v.X / length, v.Y / length, v.Z / length);
        }
        public static Vector3 CVector2Vector(CVector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
        internal static bool RayInsideGeoset(Ray lne, CGeoset geose)
        {
            CExtent extent = Calculator.GetExtent(geose.Vertices.Select(x => x.Position).ToList());
            Vector3 min = CVector2Vector(extent.Min);
            Vector3 max = CVector2Vector(extent.Max);

            Vector3 origin = lne.From;
            Vector3 direction = Vector3.Normalize(lne.To - lne.From);

            return RayIntersectsAABB(origin, direction, min, max);
        }
        private static bool RayIntersectsAABB(Vector3 origin, Vector3 direction, Vector3 min, Vector3 max)
        {
            float tmin = (min.X - origin.X) / direction.X;
            float tmax = (max.X - origin.X) / direction.X;

            if (tmin > tmax) (tmin, tmax) = (tmax, tmin);

            float tymin = (min.Y - origin.Y) / direction.Y;
            float tymax = (max.Y - origin.Y) / direction.Y;

            if (tymin > tymax) (tymin, tymax) = (tymax, tymin);

            if ((tmin > tymax) || (tymin > tmax))
                return false;

            if (tymin > tmin)
                tmin = tymin;
            if (tymax < tmax)
                tmax = tymax;

            float tzmin = (min.Z - origin.Z) / direction.Z;
            float tzmax = (max.Z - origin.Z) / direction.Z;

            if (tzmin > tzmax) (tzmin, tzmax) = (tzmax, tzmin);

            if ((tmin > tzmax) || (tzmin > tmax))
                return false;

            return true;
        }

        internal static float FlipU(float u)
        {
            return 1.0f - u;
        }

        internal static float FlipV(float v)
        {
            return 1.0f - v;
        }

        internal static CExtent GetExtent(CGeoset cGeoset)
        {
            return GetExtent(cGeoset.Vertices.Select(x => x.Position).ToList()); ;
        }

        internal static bool ExtentsOverlap(CExtent ex, List<Vector3> extent)
        {
            Extent a = new Extent(ex); // Converted extent

            // Create Extent b from the 8 points
            Extent b = new Extent
            {
                minX = extent.Min(v => v.X),
                maxX = extent.Max(v => v.X),
                minY = extent.Min(v => v.Y),
                maxY = extent.Max(v => v.Y),
                minZ = extent.Min(v => v.Z),
                maxZ = extent.Max(v => v.Z)
            };

            // Check for overlap on all three axes
            bool xOverlap = a.minX <= b.maxX && a.maxX >= b.minX;
            bool yOverlap = a.minY <= b.maxY && a.maxY >= b.minY;
            bool zOverlap = a.minZ <= b.maxZ && a.maxZ >= b.minZ;

            return xOverlap && yOverlap && zOverlap;
        }


        internal static void translateVertices(Axes x, List<CGeosetVertex> vertices, float value)
        {
            if (vertices.Count == 1)
            {
                if (x == Axes.X) vertices[0].Position.X = value;
                if (x == Axes.Y) vertices[0].Position.Y = value;
                if (x == Axes.Z) vertices[0].Position.Z = value;
            }
            else if (vertices.Count > 1)
            {
                var centroid = GetCentroidOfVertices(vertices);
                float offset = 0.0f;

                // Calculate the offset once
                if (x == Axes.X) offset = value - centroid.X;
                if (x == Axes.Y) offset = value - centroid.Y;
                if (x == Axes.Z) offset = value - centroid.Z;

                foreach (CGeosetVertex v in vertices)
                {
                    if (x == Axes.X) v.Position.X += offset;
                    if (x == Axes.Y) v.Position.Y += offset;
                    if (x == Axes.Z) v.Position.Z += offset;
                }
            }
        }

        internal static void ScaleVertices(Axes ax, List<CGeosetVertex> vertices, float value)
        {
            float normalized = value / 100f;

            if (vertices.Count > 1)
            {
                var centroid = GetCentroidOfVertices(vertices);

                foreach (var v in vertices)
                {
                    if (ax == Axes.X)
                        v.Position.X = centroid.X + (v.Position.X - centroid.X) * normalized;

                    if (ax == Axes.Y)
                        v.Position.Y = centroid.Y + (v.Position.Y - centroid.Y) * normalized;

                    if (ax == Axes.Z)
                        v.Position.Z = centroid.Z + (v.Position.Z - centroid.Z) * normalized;
                }
                return;
            }

        }
        internal static void RotateVertices(Axes axis, List<CGeosetVertex> vertices, float angleDegrees)
        {
            if (vertices.Count <= 1)
                return;

            float angleRadians = angleDegrees * (float)(Math.PI / 180.0);
            var centroid = GetCentroidOfVertices(vertices);

            foreach (var v in vertices)
            {
                float x = v.Position.X - centroid.X;
                float y = v.Position.Y - centroid.Y;
                float z = v.Position.Z - centroid.Z;

                float sin = (float)Math.Sin(angleRadians);
                float cos = (float)Math.Cos(angleRadians);

                float newX = x, newY = y, newZ = z;

                switch (axis)
                {
                    case Axes.X:
                        newY = y * cos - z * sin;
                        newZ = y * sin + z * cos;
                        break;

                    case Axes.Y:
                        newX = x * cos + z * sin;
                        newZ = -x * sin + z * cos;
                        break;

                    case Axes.Z:
                        newX = x * cos - y * sin;
                        newY = x * sin + y * cos;
                        break;
                }

                v.Position.X = centroid.X + newX;
                v.Position.Y = centroid.Y + newY;
                v.Position.Z = centroid.Z + newZ;
            }
        }

        internal static CVector3 RGB2NRRGB(int r, int g, int b)
        {
            float r2 = (float)b / 255f;
            float g2 = (float)g / 255f;
            float b2 = (float)r / 255f;
            return new CVector3(r2, g2, b2);

        }

        internal static System.Windows.Media.Brush War3ColorToBrush2(CVector3 color)
        {
            float r = color.X;
            float g = color.Y;
            float b = color.Z;
            // Clamp values between 0 and 1, then scale to byte (0-255)
            byte red = (byte)(Math.Clamp(r, 0f, 1f) * 255);
            byte green = (byte)(Math.Clamp(g, 0f, 1f) * 255);
            byte blue = (byte)(Math.Clamp(b, 0f, 1f) * 255);

            return new SolidColorBrush(System.Windows.Media.Color.FromRgb(red, green, blue));
        }

        internal static void AverageUV(List<CGeosetVertex> vertices)
        {
            if (vertices.Count < 2) return;

        }

        internal static void centerNodes(List<INode> nodes, Axes axes)
        {
            if (nodes == null || nodes.Count == 0)
                return;

            // Compute the average pivot point
            float sumX = 0, sumY = 0, sumZ = 0;
            foreach (var node in nodes)
            {
                sumX += node.PivotPoint.X;
                sumY += node.PivotPoint.Y;
                sumZ += node.PivotPoint.Z;
            }

            float avgX = sumX / nodes.Count;
            float avgY = sumY / nodes.Count;
            float avgZ = sumZ / nodes.Count;

            // Offset each node's pivot point to center them
            foreach (var node in nodes)
            {
                var pivot = node.PivotPoint;
                if ((axes & Axes.X) != 0)
                    pivot.X -= avgX;
                if ((axes & Axes.Y) != 0)
                    pivot.Y -= avgY;
                if ((axes & Axes.Z) != 0)
                    pivot.Z -= avgZ;
                node.PivotPoint = pivot;
            }
        }

        internal static void CenterVertices(List<CGeosetVertex> vertices, Axes axes)
        {
            if (vertices == null || vertices.Count == 0)
                return;

            // Compute the average pivot point
            float sumX = 0, sumY = 0, sumZ = 0;
            foreach (var node in vertices)
            {
                sumX += node.Position.X;
                sumY += node.Position.Y;
                sumZ += node.Position.Z;
            }

            float avgX = sumX / vertices.Count;
            float avgY = sumY / vertices.Count;
            float avgZ = sumZ / vertices.Count;

            // Offset each node's pivot point to center them
            foreach (var node in vertices)
            {
                var pivot = node.Position;
                if ((axes & Axes.X) != 0)
                    pivot.X -= avgX;
                if ((axes & Axes.Y) != 0)
                    pivot.Y -= avgY;
                if ((axes & Axes.Z) != 0)
                    pivot.Z -= avgZ;
                node.Position = pivot;
            }
        }

        internal static void CenterGeosetsTogether(List<CGeoset> geosets, bool onX, bool onY, bool onZ)
        {
            if (geosets.Count == 0)
            {
                return;
            }

            CVector3 centroid;

            if (geosets.Count == 1)
            {
                centroid = Calculator.GetCentroidOfGeoset(geosets[0]);
            }
            else
            {
                centroid = Calculator.GetCentroidOfGeosets(geosets);
            }

            foreach (CGeoset g in geosets)
            {
                if (g.Vertices.Count == 0) continue;
                foreach (var vertex in g.Vertices)
                {
                    var pos = vertex.Position;

                    float newX = onX ? pos.X - centroid.X : pos.X;
                    float newY = onY ? pos.Y - centroid.Y : pos.Y;
                    float newZ = onZ ? pos.Z - centroid.Z : pos.Z;

                    vertex.Position = new CVector3(newX, newY, newZ);
                }
            }
        }
        public static void SnapVector(CVector3 vector, SnapType snapType, float gridSize, float lineSpacing)
        {

            // Ensure grid size and line spacing are not zero
            if (gridSize == 0 || lineSpacing == 0) { return; }

            // Calculate half grid size for symmetric snapping
            float halfGridSize = gridSize / 2f;

            // Get the nearest multiple of lineSpacing along the X and Y axes
            float snappedX = SnapCoordinate(vector.X, halfGridSize, lineSpacing);
            float snappedY = SnapCoordinate(vector.Y, halfGridSize, lineSpacing);

            // Depending on the SnapType, adjust the Z-coordinate or apply other shifts
            float snappedZ = vector.Z; // Z is left unchanged by default

            // Apply the snapping based on the SnapType
            switch (snapType)
            {
                case SnapType.Nearest:
                    vector.X = snappedX;
                    vector.Y = snappedY;
                    break;

                case SnapType.TopFrontLeft:
                    vector.X = -halfGridSize;
                    vector.Y = halfGridSize;
                    vector.Z = gridSize; // Place it at the "top" of the grid
                    break;

                case SnapType.TopFrontRight:
                    vector.X = halfGridSize;
                    vector.Y = halfGridSize;
                    vector.Z = gridSize;
                    break;

                case SnapType.TopBackLeft:
                    vector.X = -halfGridSize;
                    vector.Y = -halfGridSize;
                    vector.Z = gridSize;
                    break;

                case SnapType.TopBackRight:
                    vector.X = halfGridSize;
                    vector.Y = -halfGridSize;
                    vector.Z = gridSize;
                    break;

                case SnapType.BottomFrontLeft:
                    vector.X = -halfGridSize;
                    vector.Y = halfGridSize;
                    vector.Z = 0; // Place it at the "bottom" of the grid
                    break;

                case SnapType.BottomFrontRight:
                    vector.X = halfGridSize;
                    vector.Y = halfGridSize;
                    vector.Z = 0;
                    break;

                case SnapType.BottomBackLeft:
                    vector.X = -halfGridSize;
                    vector.Y = -halfGridSize;
                    vector.Z = 0;
                    break;

                case SnapType.BottomBackRight:
                    vector.X = halfGridSize;
                    vector.Y = -halfGridSize;
                    vector.Z = 0;
                    break;
            }
        }

        // Helper function to snap a coordinate to the nearest line spacing
        private static float SnapCoordinate(float coordinate, float halfGridSize, float lineSpacing)
        {
            // If the coordinate is outside of the grid, snap to the closest grid line within range
            float snappedCoord = (float)Math.Round(coordinate / lineSpacing) * lineSpacing;

            // Clamp the snapped value to ensure it stays within the grid boundaries
            snappedCoord = Math.Min(Math.Max(snappedCoord, -halfGridSize), halfGridSize);

            return snappedCoord;
        }
        public static void SnapVerticesAsGroup(List<CGeosetVertex> vertices, SnapType snapType, float gridSize, float lineSpacing)
        {
            // Ensure the list has vertices
            if (vertices == null || vertices.Count == 0) { return; }

            // Get the centroid of the vertices
            CVector3 centroid = GetCentroidOfVertices(vertices);

            // Snap the centroid based on the SnapType
            SnapVector(centroid, snapType, gridSize, lineSpacing);

            // Calculate the offset (translation) needed to move the centroid to the snapped position
            CVector3 offset = centroid - GetCentroidOfVertices(vertices);

            // Apply the same offset to each vertex in the group (to move the group as a whole)
            foreach (var vertex in vertices)
            {
                vertex.Position += offset;
            }
        }
        public static void SnapNodesAsGroup(List<INode> vertices, SnapType snapType, float gridSize, float lineSpacing)
        {
            // Ensure the list has vertices
            if (vertices == null || vertices.Count == 0) { return; }

            // Get the centroid of the vertices
            CVector3 centroid = GetCentroidOfVectors(vertices.Select(x => x.PivotPoint).ToList());

            // Snap the centroid based on the SnapType
            SnapVector(centroid, snapType, gridSize, lineSpacing);

            // Calculate the offset (translation) needed to move the centroid to the snapped position
            CVector3 offset = centroid - GetCentroidOfVectors(vertices.Select(x => x.PivotPoint).ToList());

            // Apply the same offset to each vertex in the group (to move the group as a whole)
            foreach (var vertex in vertices)
            {
                vertex.PivotPoint += offset;
            }
        }

        internal static void centerNodesTogether(List<INode> nodes, bool onX, bool onY, bool onZ)
        {
            if (nodes == null || nodes.Count == 0)
                return;

            // Calculate the average position of all nodes
            float avgX = 0, avgY = 0, avgZ = 0;

            foreach (var node in nodes)
            {
                avgX += node.PivotPoint.X;
                avgY += node.PivotPoint.Y;
                avgZ += node.PivotPoint.Z;
            }

            avgX /= nodes.Count;
            avgY /= nodes.Count;
            avgZ /= nodes.Count;

            // Move each node to the new position, based on selected axes
            foreach (var node in nodes)
            {
                if (onX)
                    node.PivotPoint.X = avgX;
                if (onY)
                    node.PivotPoint.Y = avgY;
                if (onZ)
                    node.PivotPoint.Z = avgZ;
            }
        }
        internal static void CenterVerticesTogether(List<CGeosetVertex> vertices, bool onX, bool onY, bool onZ)
        {
            if (vertices == null || vertices.Count == 0)
                return;

            // Calculate the average position of all vertices
            float avgX = 0, avgY = 0, avgZ = 0;

            foreach (var vertex in vertices)
            {
                avgX += vertex.Position.X;
                avgY += vertex.Position.Y;
                avgZ += vertex.Position.Z;
            }

            avgX /= vertices.Count;
            avgY /= vertices.Count;
            avgZ /= vertices.Count;

            // Move each vertex to the new position, based on selected axes
            foreach (var vertex in vertices)
            {
                if (onX)
                    vertex.Position.X = avgX;
                if (onY)
                    vertex.Position.Y = avgY;
                if (onZ)
                    vertex.Position.Z = avgZ;
            }
        }
        internal static List<List<CGeosetTriangle>> CollectTriangleIislands(List<CGeosetTriangle> triangles)
        {
            var triangleToNeighbors = new Dictionary<CGeosetTriangle, List<CGeosetTriangle>>();
            var vertexToTriangles = new Dictionary<CGeosetVertex, List<CGeosetTriangle>>();

            // Map each vertex to the triangles it is part of
            foreach (var triangle in triangles)
            {
                CGeosetVertex[] verts = {
            triangle.Vertex1.Object,
            triangle.Vertex2.Object,
            triangle.Vertex3.Object
        };

                foreach (var v in verts)
                {
                    if (!vertexToTriangles.TryGetValue(v, out var list))
                        vertexToTriangles[v] = list = new List<CGeosetTriangle>();
                    list.Add(triangle);
                }
            }

            // Build triangle adjacency map
            foreach (var triangle in triangles)
            {
                var neighbors = new HashSet<CGeosetTriangle>();

                CGeosetVertex[] verts = {
            triangle.Vertex1.Object,
            triangle.Vertex2.Object,
            triangle.Vertex3.Object
        };

                foreach (var v in verts)
                {
                    foreach (var neighbor in vertexToTriangles[v])
                    {
                        if (neighbor != triangle)
                            neighbors.Add(neighbor);
                    }
                }

                triangleToNeighbors[triangle] = neighbors.ToList();
            }

            // DFS to find connected components (islands)
            var islands = new List<List<CGeosetTriangle>>();
            var visited = new HashSet<CGeosetTriangle>();

            foreach (var triangle in triangles)
            {
                if (visited.Contains(triangle))
                    continue;

                var island = new List<CGeosetTriangle>();
                var stack = new Stack<CGeosetTriangle>();
                stack.Push(triangle);

                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    if (!visited.Add(current))
                        continue;

                    island.Add(current);

                    foreach (var neighbor in triangleToNeighbors[current])
                    {
                        if (!visited.Contains(neighbor))
                            stack.Push(neighbor);
                    }
                }

                islands.Add(island);
            }

            return islands;
        }

        internal static void ProjectTriangleIslands(List<CGeosetTriangle> triangles)
        {
            List<List<CGeosetTriangle>> islands = CollectTriangleIislands(triangles);
            foreach (var island in islands)
            {
                // Gather unique vertices
                var vertices = new HashSet<CGeosetVertex>();
                foreach (var tri in island)
                {
                    vertices.Add(tri.Vertex1.Object);
                    vertices.Add(tri.Vertex2.Object);
                    vertices.Add(tri.Vertex3.Object);
                }

                var vertexList = vertices.ToList();
                bool isFlat = false;

                if (vertexList.Count >= 3)
                {
                    var p0 = vertexList[0].Position;
                    var p1 = vertexList[1].Position;
                    var p2 = vertexList[2].Position;

                    var normal = CVector3.Cross(p1 - p0, p2 - p0);

                    isFlat = true;
                    for (int i = 3; i < vertexList.Count; i++)
                    {
                        var point = vertexList[i].Position;
                        var toPoint = point - p0;
                        float distToPlane = CVector3.Dot(toPoint, normal);

                        if (Math.Abs(distToPlane) > 0.001f)
                        {
                            isFlat = false;
                            break;
                        }
                    }
                }

                // Projection directions
                CVector3 axisU, axisV;

                if (isFlat)
                {
                    // Same as before: use the flat plane's direction
                    var p0 = vertexList[0].Position;
                    var p1 = vertexList[1].Position;
                    var p2 = vertexList[2].Position;
                    var normal = CVector3.Cross(p1 - p0, p2 - p0).Normalize();

                    axisU = CVector3.Cross(normal, new CVector3(0, 0, 1));
                    if (axisU.Length() < 0.001f)
                        axisU = CVector3.Cross(normal, new CVector3(0, 1, 0));

                    axisU = axisU.Normalize();
                    axisV = CVector3.Cross(normal, axisU).Normalize();
                }
                else
                {
                    // Project from top (XY plane)
                    axisU = new CVector3(1, 0, 0);
                    axisV = new CVector3(0, 1, 0);
                }

                // Project and normalize
                float minU = float.MaxValue, maxU = float.MinValue;
                float minV = float.MaxValue, maxV = float.MinValue;

                var uvMap = new Dictionary<CGeosetVertex, CVector2>();
                foreach (var v in vertices)
                {
                    var pos = v.Position;
                    float u = CVector3.Dot(pos, axisU);
                    float vval = CVector3.Dot(pos, axisV);

                    uvMap[v] = new CVector2(u, vval);

                    if (u < minU) minU = u;
                    if (u > maxU) maxU = u;
                    if (vval < minV) minV = vval;
                    if (vval > maxV) maxV = vval;
                }

                float rangeU = maxU - minU;
                float rangeV = maxV - minV;

                if (rangeU == 0) rangeU = 1;
                if (rangeV == 0) rangeV = 1;

                foreach (var pair in uvMap)
                {
                    float normU = (pair.Value.X - minU) / rangeU;
                    float normV = (pair.Value.Y - minV) / rangeV;
                    pair.Key.TexturePosition = new CVector2(normU, normV);
                }
            }
        }

        internal static void ScaleVerticesBy(Axes axisMode, List<CGeosetVertex> vertices, float value, bool positive)
        {
            foreach (var vertex in vertices)
            {
                float scale = positive ? (1 + value) : (1 - value);

                switch (axisMode)
                {
                    case Axes.X:
                        vertex.Position.X *= scale;
                        break;
                    case Axes.Y:
                        vertex.Position.Y *= scale;
                        break;
                    case Axes.Z:
                        vertex.Position.Z *= scale;
                        break;
                      
                }
            }
        }

        internal static void PutOnGround_Half(List<CGeosetVertex> vertices)
        {
            if (vertices == null || vertices.Count == 0)
                return;

            float minZ = float.MaxValue;
            float maxZ = float.MinValue;

            // Find min and max Z values
            foreach (var vertex in vertices)
            {
                float z = vertex.Position.Z;
                if (z < minZ) minZ = z;
                if (z > maxZ) maxZ = z;
            }

            float height = maxZ - minZ;
            float targetCenterZ = height / 2f;
            float currentCenterZ = (maxZ + minZ) / 2f;
            float offset = currentCenterZ - targetCenterZ;

            // Move all vertices by the offset
            foreach (var vertex in vertices)
            {
                var pos = vertex.Position;
                vertex.Position = new CVector3(pos.X, pos.Y, pos.Z - offset);
            }
        }

        internal static CVector3[] GetCeilingBurningPoints(CExtent extent)
        {
            CVector3 min = extent.Min;
            CVector3 max = extent.Max;

            float insetFactor = 0.1f; // 10% inward from each edge

            float insetX = (max.X - min.X) * insetFactor;
            float insetY = (max.Y - min.Y) * insetFactor;
            float ceilingZ = max.Z;

            CVector3 first = new CVector3(min.X + insetX, min.Y + insetY, ceilingZ); // bottom-left inset
            CVector3 second = new CVector3(max.X - insetX, min.Y + insetY, ceilingZ); // bottom-right inset
            CVector3 third = new CVector3(max.X - insetX, max.Y - insetY, ceilingZ); // top-right inset
            CVector3 fourth = new CVector3(min.X + insetX, max.Y - insetY, ceilingZ); // top-left inset

            return new CVector3[] { first, second, third, fourth };
        }


    }
}