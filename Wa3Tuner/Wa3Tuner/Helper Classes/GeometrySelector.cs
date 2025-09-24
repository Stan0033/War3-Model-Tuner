using MdxLib.Animator.Animatable;
using MdxLib.Model;
using SharpGL.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cvector3 = MdxLib.Primitives.CVector3;

namespace Wa3Tuner.Helper_Classes
{
    internal static class GeometrySelector
    {
        //unfinished
        public static void GrowSelection(CModel model, int type)
        {
            if (model.Geosets.Count == 0) return;
            // type: 0 = geosets, 1 = vertices, 2 = faces/triangles
            if (type == 0)
            {
              
               
                var selectedGeosets = model.Geosets.Where(g => g.isSelected).ToList();
                var unselectedGeosets = model.Geosets.Where(g => !g.isSelected).ToList();
                if (selectedGeosets.Count == 0) return;
                if (unselectedGeosets.Count == 0) return;
                foreach (var selected in selectedGeosets)
                {
                    // Find the closest unselected geoset to the selected geoset
                    CGeoset closest = null;
                    var centroid = Calculator.GetCentroidOfGeoset(selected);
                    List<Cvector3> centroids = new();
                    List<float> distances = new();
                    foreach (var uns in unselectedGeosets)
                    {
                        var c = Calculator.GetCentroidOfGeoset(uns);
                      
                        distances.Add(Calculator.GetDistanceBetweenVectors(centroid, c));
                    }
                    int minIndex = distances.IndexOf(distances.Min());
                    unselectedGeosets[minIndex].isSelected = true;
                    unselectedGeosets.RemoveAt(minIndex);
                }
            }



            else if (type == 1) // Vertices
            {
                

                var selectedVertices = model.Geosets.SelectMany(y=>y.Vertices). Where(g => g.isSelected).ToList();
                var unselectedVertices = model.Geosets.SelectMany(y => y.Vertices).Where(g => !g.isSelected).ToList();
                if (selectedVertices.Count == 0) return;
                if (unselectedVertices.Count == 0) return;
                foreach (var selected in selectedVertices)
                {
                  
                    List<float> distances = new();
                    foreach (var uns in unselectedVertices)
                    {
                      

                        distances.Add(Calculator.GetDistanceBetweenVectors(selected.Position, uns.Position));
                    }
                    int minIndex = distances.IndexOf(distances.Min());
                    unselectedVertices[minIndex].isSelected = true;
                    unselectedVertices.RemoveAt(minIndex);
                }
            }

            else if (type == 2)
            {
                var selectedTriangles = model.Geosets.SelectMany(y => y.Triangles).Where(g => g.isSelected).ToList();
                var unselectedTriangles = model.Geosets.SelectMany(y => y.Triangles).Where(g => !g.isSelected).ToList();
                if (selectedTriangles.Count == 0) return;
                if (unselectedTriangles.Count == 0) return;
                foreach (var selected in selectedTriangles)
                {
                    Cvector3 centroid1 = Calculator.GetCentroidofTriangle(selected);
                    List<float> distances = new();
                    foreach (var uns in unselectedTriangles)
                    {

                        Cvector3 centroid2 = Calculator.GetCentroidofTriangle(uns);
                        distances.Add(Calculator.GetDistanceBetweenVectors(centroid1, centroid2));
                    }
                    int minIndex = distances.IndexOf(distances.Min());
                    unselectedTriangles[minIndex].isSelected = true;
                    unselectedTriangles.RemoveAt(minIndex);
                }
            }
        }
        public static void ShrinkSelection(CModel model, int type)
        {
            // type: 0 = geosets, 1 = vertices, 2 = faces
            if (type == 0)
            {
                // Build working lists up front
                var selectedGeosets = model.Geosets.Where(g => g.isSelected).ToList();
                var unselectedGeosets = model.Geosets.Where(g => !g.isSelected).ToList();

                if (selectedGeosets.Count == 0 || unselectedGeosets.Count == 0)
                    return;

                foreach (var unSel in unselectedGeosets)
                {
                    if (selectedGeosets.Count == 0) break;   // nothing left to unselect

                    var centroid = Calculator.GetCentroidOfGeoset(unSel);

                    // Find the *nearest* currently-selected geoset
                    int minIndex = 0;
                    float minDistance = float.MaxValue;

                    for (int i = 0; i < selectedGeosets.Count; i++)
                    {
                        float d = Calculator.GetDistanceBetweenVectors(
                                      centroid,
                                      Calculator.GetCentroidOfGeoset(selectedGeosets[i]));
                        if (d < minDistance)
                        {
                            minDistance = d;
                            minIndex = i;
                        }
                    }

                    // Un-select that geoset and remove it from the working list
                    selectedGeosets[minIndex].isSelected = false;
                    selectedGeosets.RemoveAt(minIndex);
                }





            }

            else if (type == 1)
            {
                // Build working lists up front
                var selectedVertices = model.Geosets.SelectMany(g => g.Vertices).Where(v => v.isSelected).ToList();
                var unselectedVertices = model.Geosets.SelectMany(g => g.Vertices).Where(v => !v.isSelected).ToList();

                if (selectedVertices.Count == 0 || unselectedVertices.Count == 0)
                    return;

                foreach (var unSel in unselectedVertices)
                {
                    if (selectedVertices.Count == 0) break; // nothing left to unselect

                    int minIndex = 0;
                    float minDistance = float.MaxValue;

                    for (int i = 0; i < selectedVertices.Count; i++)
                    {
                        float d = Calculator.GetDistanceBetweenVectors(unSel.Position, selectedVertices[i].Position);
                        if (d < minDistance)
                        {
                            minDistance = d;
                            minIndex = i;
                        }
                    }

                    // Unselect the closest selected vertex and remove it from the pool
                    selectedVertices[minIndex].isSelected = false;
                    selectedVertices.RemoveAt(minIndex);
                }

            }
            else if (type == 2)
            {
                // Build working lists
                var selectedTriangles = model.Geosets.SelectMany(g => g.Triangles).Where(t => t.isSelected).ToList();
                var unselectedTriangles = model.Geosets.SelectMany(g => g.Triangles).Where(t => !t.isSelected).ToList();

                if (selectedTriangles.Count == 0 || unselectedTriangles.Count == 0)
                    return;

                foreach (var unSel in unselectedTriangles)
                {
                    if (selectedTriangles.Count == 0) break; // nothing left to unselect

                    var centroidUnSel = Calculator.GetCentroidofTriangle(unSel);

                    int minIndex = 0;
                    float minDistance = float.MaxValue;

                    for (int i = 0; i < selectedTriangles.Count; i++)
                    {
                        var centroidSel = Calculator.GetCentroidofTriangle(selectedTriangles[i]);
                        float d = Calculator.GetDistanceBetweenVectors(centroidUnSel, centroidSel);
                        if (d < minDistance)
                        {
                            minDistance = d;
                            minIndex = i;
                        }
                    }

                    // Unselect the closest selected triangle and remove it from the pool
                    selectedTriangles[minIndex].isSelected = false;
                    selectedTriangles.RemoveAt(minIndex);
                }

            }
        }
        private static CGeosetTriangle? FindClosesTriangle()
        {
            return null;
        }
        private static CGeosetVertex? FindClosestVertex(CModel model, CGeosetVertex selected)
        {
            CGeosetVertex closest = null;
            
            

            return closest;
        }

    
    }
}
