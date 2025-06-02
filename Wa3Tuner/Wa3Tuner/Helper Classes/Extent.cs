using MdxLib.Model;
using MdxLib.Primitives;
using System.Collections.Generic;
using System.Numerics;
using System;
using Wa3Tuner.Helper_Classes;

namespace Whim_GEometry_Editor.Misc
{
    public class Extent
    {
        internal float minX, minY, minZ, maxX, maxY, maxZ;

        // Constructor to initialize the Extent from rays
        public Extent(List<Ray> rays)
        {
            // Initialize min/max values
            minX = minY = minZ = float.MaxValue;
            maxX = maxY = maxZ = float.MinValue;

            // Loop through the rays to calculate the min/max extents
            foreach (var ray in rays)
            {
                minX = Math.Min(minX, Math.Min(ray.From.X, ray.To.X));
                maxX = Math.Max(maxX, Math.Max(ray.From.X, ray.To.X));

                minY = Math.Min(minY, Math.Min(ray.From.Y, ray.To.Y));
                maxY = Math.Max(maxY, Math.Max(ray.From.Y, ray.To.Y));

                minZ = Math.Min(minZ, Math.Min(ray.From.Z, ray.To.Z));
                maxZ = Math.Max(maxZ, Math.Max(ray.From.Z, ray.To.Z));
            }
        }

        // Method to check if a vertex is inside the extent
        public bool IsInside(CGeosetVertex vertex)
        {
            Vector3 pos = new Vector3(vertex.Position.X, vertex.Position.Y, vertex.Position.Z);

            return pos.X >= minX && pos.X <= maxX &&
                   pos.Y >= minY && pos.Y <= maxY &&
                   pos.Z >= minZ && pos.Z <= maxZ;
        }
        public Extent (float mx, float my, float mz, float xx, float xy, float xz)
        {
            minX = mx;
            minY = my;
            minZ = mz;
            maxX = xx;
            maxY = xy;
            maxZ = xz;
        }
        public Extent(CExtent e)
        {
            minX = e.Min.X; minY = e.Min.Z; minZ = e.Min.Z; maxX = e.Max.X; maxY = e.Max.Y; maxZ = e.Max.Z;
        }
        public Extent() { }
        public override string ToString()
        {
            return $"{minX} {minY} {minZ}, {maxX} {maxY} {maxZ}";
        }
    }
}