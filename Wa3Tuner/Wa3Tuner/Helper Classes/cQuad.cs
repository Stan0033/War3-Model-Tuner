using MdxLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Wa3Tuner.Helper_Classes
{
    // for fast uv mapping
    public class cQuad
    {
        public CGeosetTriangle Triangle1, Triangle2;
        public cQuad(CGeosetTriangle t1, CGeosetTriangle t2)
        {
            Triangle1 = t1;
            Triangle2 = t2;
        }
    }
    public static class QuadCollector
    {
        public static List<cQuad> List = new List<cQuad>();
        public static void Clear() { List.Clear(); }
        public static void Add(CGeosetTriangle one, CGeosetTriangle two)
        {
            if (List.Any(x => x.Triangle1 == one || x.Triangle2 == two || x.Triangle1 == two || x.Triangle2 == one))
            {
                MessageBox.Show("One or both of the selected triangles are already contained in the quad collection"); return;
            }
            List.Add(new cQuad(one, two));
        }
        public static void Fit()
        {
            if (List.Count == 0) { MessageBox.Show("The quad collection is empty"); return; }
            foreach (cQuad quad in List)
            {
                var allVertices = new List<CGeosetVertex>
        {
            quad.Triangle1.Vertex1.Object,
            quad.Triangle1.Vertex2.Object,
            quad.Triangle1.Vertex3.Object,
            quad.Triangle2.Vertex1.Object,
            quad.Triangle2.Vertex2.Object,
            quad.Triangle2.Vertex3.Object
        };

                // Identify the 2 shared vertices
                var sharedVertices = allVertices.GroupBy(v => v)
                                                .Where(g => g.Count() == 2)
                                                .Select(g => g.Key)
                                                .ToList();

                if (sharedVertices.Count != 2)
                {
                    MessageBox.Show("Invalid quad detected. Triangles must share exactly two vertices.");
                    continue;
                }

                // Identify the 2 unique vertices (not part of the shared edge)
                var uniqueVertices = allVertices.Except(sharedVertices).ToList();
                if (uniqueVertices.Count != 2)
                {
                    MessageBox.Show("Unexpected error: Quad does not have exactly 4 unique vertices.");
                    continue;
                }

                // Assign UVs:
                sharedVertices[0].TexturePosition = new MdxLib.Primitives.CVector2(0, 0);
                sharedVertices[1].TexturePosition = new MdxLib.Primitives.CVector2(1, 0);
                uniqueVertices[0].TexturePosition = new MdxLib.Primitives.CVector2(0, 1);
                uniqueVertices[1].TexturePosition = new MdxLib.Primitives.CVector2(1, 1);
            }
        }

        public static void RotateUV90()
        {
            if (List.Count == 0) { MessageBox.Show("The quad collection is empty");return; }
            foreach (cQuad quad in List)
            {
                var allVertices = new List<CGeosetVertex>
        {
            quad.Triangle1.Vertex1.Object,
            quad.Triangle1.Vertex2.Object,
            quad.Triangle1.Vertex3.Object,
            quad.Triangle2.Vertex1.Object,
            quad.Triangle2.Vertex2.Object,
            quad.Triangle2.Vertex3.Object
        };

                // Extract the unique UVs
                var uniqueUVs = allVertices.Select(v => v.TexturePosition).Distinct().ToList();

                if (uniqueUVs.Count != 4)
                {
                    MessageBox.Show("Invalid quad UV setup. There must be exactly 4 unique UVs.");
                    continue;
                }

                // Rotate each UV 90 degrees clockwise
                foreach (var uv in uniqueUVs)
                {
                    float oldX = uv.X, oldY = uv.Y;
                    uv.X = 1 - oldY;
                    uv.Y = oldX;
                }
            }
        }

        public static void FitCustom(float u1, float v1, float u2, float v2, float u3, float v3, float u4, float v4)
        {
            if (List.Count == 0) { MessageBox.Show("The quad collection is empty"); return; }
            foreach (cQuad quad in List)
            {
                var allVertices = new List<CGeosetVertex>
        {
            quad.Triangle1.Vertex1.Object,
            quad.Triangle1.Vertex2.Object,
            quad.Triangle1.Vertex3.Object,
            quad.Triangle2.Vertex1.Object,
            quad.Triangle2.Vertex2.Object,
            quad.Triangle2.Vertex3.Object
        };

                // Identify shared and unique vertices
                var sharedVertices = allVertices.GroupBy(v => v)
                                                .Where(g => g.Count() == 2)
                                                .Select(g => g.Key)
                                                .ToList();

                var uniqueVertices = allVertices.Except(sharedVertices).ToList();

                if (sharedVertices.Count != 2 || uniqueVertices.Count != 2)
                {
                    MessageBox.Show("Invalid quad detected. Ensure the triangles form a proper quad.");
                    continue;
                }

                // Assign custom UVs
                sharedVertices[0].TexturePosition = new MdxLib.Primitives.CVector2(u1, v1);
                sharedVertices[1].TexturePosition = new MdxLib.Primitives.CVector2(u2, v2);
                uniqueVertices[0].TexturePosition = new MdxLib.Primitives.CVector2(u3, v3);
                uniqueVertices[1].TexturePosition = new MdxLib.Primitives.CVector2(u4, v4);
            }
        }

        internal static void ClearExcept(List<CGeosetTriangle> t)
        {
            foreach (var item in List.ToList())
            {
                if (t.Contains(item.Triangle1)){ List.Remove(item); continue; }
                if (t.Contains(item.Triangle2)){ List.Remove(item); continue; }
            }
        }
    }
}
