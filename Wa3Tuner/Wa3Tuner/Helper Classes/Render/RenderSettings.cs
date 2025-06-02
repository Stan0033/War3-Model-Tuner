using System.Windows.Media;

namespace Wa3Tuner.Helper_Classes
{
    internal class RenderSettings
    {


        public static bool RenderGroundPlane = false;
        public static bool RenderTextures = true;
        public static bool RenderShading = true;
        public static bool RenderCollisionShapes = false;
        public static bool RenderSkinning = false;
        public static bool RenderTriangles = true;
        public static bool RenderGeosetExtents = false;
        public static bool RenderGeosetExtentSphere = false;
        public static bool RenderNodes = false;
        public static bool RenderGeometry = false;
        public static float LineSpacing = 1;
        public static bool RenderGroundGrid = false;
        public static bool RenderGridX = false;
        public static bool RenderGridY = false;
        public static bool RenderSkeleton = false;

        public static bool RenderNormals = false;
        public static bool RenderAxis = true;
        public static int ViewportGridSizeOverlay = 0;
        public static int ViewportGridSize = 0;
        public static bool RenderEnabled = true;
        public static bool RenderEdges = false;
        public static bool RenderVertices = false;
        public static bool RenderFPS = true;
        public static bool RenderLighing = true;
        //----------------------------------------------------------
        //---- colors
        //----------------------------------------------------------
        // Paths
        internal static float[] Path_Node = C2F.C(Colors.Black);
        internal static float[] Path_Node_Selected = C2F.C(Colors.Green);

        internal static float[] Path_Line = C2F.C(Colors.Red);
        //----------------------------------------------------------
        internal static float[] Color_SelectedGeoset = C2F.C(Colors.Green);
        // Edge colors
        internal static float[] Color_Edge = C2F.C(Colors.Red);
        internal static float[] Color_Edge_Selected =   C2F.C(Colors.LawnGreen);
       
        internal static float[] Color_Extent = C2F.C(Colors.White);
        internal static float[] Color_CollisionShape = C2F.C(Colors.Blue);
        // Rigging
        internal static float[] Color_Skinning = C2F.C(Colors.Orange);
        // Skeleton color
        internal static float[] Color_Skeleton = C2F.C(Colors.Purple);
        // noramls color
        internal static float[] Color_Normals = C2F.C(Colors.Yellow);
        // nodes
        internal static float[] Color_NodeSelected = C2F.C(Colors.Green);
        internal static float[] Color_Node = C2F.C(Colors.DarkRed);
        // Vertex colors
        internal static float[] Color_Vertex = C2F.C(Colors.Blue);
        internal static float[] Color_VertexSelected = C2F.C(Colors.Green);
        internal static float[] Color_VertexRigged = C2F.C(Colors.Brown);
        internal static float[] Color_VertexRiggedSelected = C2F.C(Colors.RosyBrown);

        // selected triangle
        internal static float[] Color_SelectedTriangle = C2F.C(Colors.LawnGreen);

        // Background color
        internal static float[] BackgroundColor = C2F.C(Colors.LightGray);

        //----------------------------------------------------------
        // Point settings
        internal static float VertexSize = 5.0f;
        internal static float NodeSize = 5.0f;
        internal static float PathNodeSize = 5.0f;
        // Lighting settings
        internal static bool EnableLighting = true;
        internal static float[] GridColor = new float[] { 0.663f, 0.663f, 0.663f }; // DarkGray (169,169,169)
        internal static float[] AmbientColor = new float[] { 1f, 1f, 1f }; // White
        internal static float[] DiffuseColor = new float[] { 1f, 1f, 1f }; // White
        internal static float[] SpecularColor = new float[] { 1f, 1f, 1f }; // White

        internal static float LightPostion = 0.0f;
        internal static float Shininess = 32.0f; // Default for shiny surfaces

        
       


        // Camera settings
        internal static double FieldOfView = 45.0; // Typical field of view in degrees
        internal static double NearDistance = 0.1; // Near clipping plane
        internal static double FarDistance = 5000.0; // Far clipping plane

        // Culling settings
        internal static bool BackfaceCullingEnabled = true;
        internal static bool BackfaceCullingClockwise = false; // Default is counter-clockwise
    }
    public static class C2F
    {
        public static float[] C(System.Windows.Media.Color c)
        {
            return new float[]
            {
            c.R / 255f,
            c.G / 255f,
            c.B / 255f,
            c.A / 255f
            };
        }
    }


}