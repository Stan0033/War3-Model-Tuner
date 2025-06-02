using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wa3Tuner.Helper_Classes
{
  

    public static class VertexPositionManipulator
    {
        /*
         NEGATE
        CENTER (X,Y,Z) (0)
        ALIGN (X,Y,Z)
        CENTER AT (X,Y,Z)
        CENTER AT SELECTION
        SNAP TO GRID
        FLATTEN
        ROTATE AROUND POINT
        ROTATE AROUND SELF
        SCALE (X,Y,Z)
        SET DISTANCE BETWEEN TWO VERTICES
        ARRANGE LIKE SHAPE

         */
        public static void Negate(CVector3 vector, bool x, bool y, bool z)
        {
            if (x) vector.X = -vector.X;
            if (y) vector.Y = -vector.Y;
            if (z) vector.Z = -vector.Z;
        }
        public static void Negate(CGeosetVertex vertex, bool x, bool y, bool z)
        {
            if (x) vertex.Position.X = -vertex.Position.X;
            if (y) vertex.Position.Y = -vertex.Position.Y;
            if (z) vertex.Position.Z = -vertex.Position.Z;
        }

        public static void CenterAtSelection(CVector3 vector, List<CGeosetVertex> positions, bool x, bool y, bool z)
        {
           var centroid = Calculator.GetCentroidOfVertices(positions); 
            if (x) vector.X += centroid.X;
            if (y) vector.Y = centroid.Y; 
            if (z) vector.Z = centroid.X;
           
        }
        public static void SnapToGrid(CVector3 vector, SnapType type)
        {

        }
        public static void SnapToGrid( List<CVector3> vectors, SnapType type)
        {

        }
    }
}
