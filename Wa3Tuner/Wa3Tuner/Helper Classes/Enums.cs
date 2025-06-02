using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wa3Tuner.Helper_Classes
{
    public enum mSelectionMode
    {
        Clear, Add, Remove
    }
    public enum PasteType
    {
        None, Normal, Negated
    }
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
        MidFront,  // Between BottomLeftFront and BottomRightFront
        TopFrontLeft,
        TopFrontRight,
        TopBackLeft,
        TopBackRight,
        BottomFrontLeft,
        BottomFrontRight,
        BottomBackLeft,
        BottomBackRight
    }
    public enum SnapType2D
    {
        Nearest,
        TopLeft, TopRight, BottomLeft, BottomRight
    }
    public enum PlayOrder
    {
        Play, Paused
    }
    public enum TransformPolarity
    {
        Increse, Decrese,
        Set
    }
    public enum PlayMode
    {
        Default, Loop, DontLoop, Cycle
    }
    public enum CameraView
    {
        Top, Bottom, Left, Right, Front, Back
    }
    public enum AnimatorMode
    {
        Translate, Rotate, Scale
    }
    public enum ModifyMode
    {
        None,
        Translate,
        Rotate,
        Scale,
        DragUV_U,
        DragUV_V,
        Inset,
        Extrude, // move 
        ExtrudeEach, // move 
        Raise,
        RotateNormals,
        Extend, // move 
        Widen,
        Sculpt,
        ExpandNarrow,//edge
        Bevel,
        DistanceX,
        DistanceY,
        DistanceZ,
        AlgnX,
        AlgnY,
        AlgnZ,

        Curve, //edge
        ScaleUV,
        ScaleUVu,
        ScaleUVv,
        RotateEach,
        ScaleEach,
    }
    public enum SceneInteractionState
    {
        None,
        DrawRectangle,
        RotatingView,
        Modify,
        DragView
    }
      public enum EditMode
    {
        Geosets, Triangles, Edges, Vertices, Normals, Nodes,
        Animator,
        UV,
        Rigging,
        None,
    }
    public enum AnimatorAxis
    {
        X, Y, Z,
        U
    }
    public enum GeometryEditMode { Geosets, Rigging, Animator, UVMapper }
    public enum CopiedKeyframe { Translation, Rotation, SCaling, all }
    public enum UVLockType
    {
        None, U, V
    }
    public enum Axes
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4,
        U = 5
    }
    public enum WorkMode
    {
        Select, Vertices, Edges, Faces
    }
    public enum RiggingAction { Add, Remove, ClearAdd }
}
