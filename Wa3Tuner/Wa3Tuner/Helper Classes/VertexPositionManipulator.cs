using MdxLib.Model;
using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wa3Tuner.Helper_Classes
{
    public enum SnapType
    {
        Nearest,  // Closest snap point

        // Cube Corners
        TopLeftBack,
        TopRightBack,
        TopLeftFront,
        TopRightFront,

        BottomLeftBack,
        BottomRightBack,
        BottomLeftFront,
        BottomRightFront,

        // Edge Midpoints
        TopMidBack,    // Between TopLeftBack and TopRightBack
        TopMidFront,   // Between TopLeftFront and TopRightFront
        TopLeftMid,    // Between TopLeftBack and TopLeftFront
        TopRightMid,   // Between TopRightBack and TopRightFront

        BottomMidBack,  // Between BottomLeftBack and BottomRightBack
        BottomMidFront, // Between BottomLeftFront and BottomRightFront
        BottomLeftMid,  // Between BottomLeftBack and BottomLeftFront
        BottomRightMid, // Between BottomRightBack and BottomRightFront

        LeftMidBack,   // Between TopLeftBack and BottomLeftBack
        LeftMidFront,  // Between TopLeftFront and BottomLeftFront
        RightMidBack,  // Between TopRightBack and BottomRightBack
        RightMidFront, // Between TopRightFront and BottomRightFront

        MidLeft,  // Between BottomLeftBack and TopLeftBack
        MidRight, // Between BottomRightBack and TopRightBack
        MidBack,  // Between BottomLeftBack and BottomRightBack
        MidFront  // Between BottomLeftFront and BottomRightFront
    }

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
