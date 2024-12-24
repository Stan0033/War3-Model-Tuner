using MdxLib.Animator;
using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows.Media.Media3D;

namespace Wa3Tuner
{
    public static class Calculator
    {
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

        internal static CAnimatorNode<CVector4> ClampQuaternion(CAnimatorNode<CVector4> item)
        {
            CVector4 value = IsValidQuaternion(item.Value.W, item.Value.X, item.Value.Y, item.Value.Z) ?  item.Value  : new CVector4(0,0,0,0);
            CVector4 intan = IsValidQuaternion(item.InTangent.W, item.InTangent.X, item.InTangent.Y, item.InTangent.Z) ? item.InTangent : new CVector4(0, 0, 0, 0);
            CVector4 outtan = IsValidQuaternion(item.OutTangent.W, item.OutTangent.X, item.OutTangent.Y, item.OutTangent.Z) ? item.InTangent : new CVector4(0, 0, 0, 0);
            return new CAnimatorNode<CVector4>(item.Time, value, intan, outtan);

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
            float decimalX = X - (int) X;
            float decimalY = Y - (int)Y;

            if (texturePosition.X < -10) { X =-9 - decimalX; }
             if (texturePosition.X >10) { X = 9 + decimalX; }
             if (texturePosition.Y >10) { Y = -9 - decimalY; }
            if (texturePosition.Y <-10) { Y = 9 + decimalY; }
            return new CVector2(X, Y);

        }
       
        internal static float ClampNormalized(float value)
        {
            if (value >= 0 && value <= 999999) { return value; }
            else if (value < 0) { return 0; }
            else { return 999999; }
        }
        internal static CAnimatorNode<float> ClampNormalized(CAnimatorNode<float> item)
        {
            if ( item.Value < 0)
            {
                return new CAnimatorNode<float>(item.Time, 0);
            }
            if (item.Value  > 1)
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
            return new CVector3(x,y,z);
             
        }
        internal static CAnimatorNode<CVector3>  ClampVector3 (CAnimatorNode<CVector3> item)
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
           return v>=0? v : 0;
        }
        internal static CAnimatorNode<int> ClampInt(CAnimatorNode<int> v)
        {
            return new CAnimatorNode<int>(v.Time, v.Value < 0? 0 : v.Value);
        }

        internal static float ClampFloat(float v)
        {
           return v < 0? 0 : v;
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
                throw new ArgumentException("The list of extents cannot be null or empty.");
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
            foreach (CGeosetFace face in geoset.Faces)
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
            float x= (extent.Max.X - extent.Min.X ) /2;
            float y= (extent.Max.Y - extent.Min.Y ) /2;
            float z= (extent.Max.Z - extent.Min.Z ) /2;
            return new CVector3( x, y, z );
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
            CAnimatorNode < CVector4 > node = new CAnimatorNode<CVector4 >();
            int time = node.Time;
            CVector4 vector = node.Value;
            CVector4 new_vector = ReverseVector4(vector);
            return new CAnimatorNode<CVector4>(time, new_vector);
           
        }
    }
}
