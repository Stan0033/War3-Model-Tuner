namespace Wa3Tuner.Helper_Classes
{
    internal class RenderSettings
    {
        // Edge colors
        internal static float[] EdgeColorSelected = new float[] { 1f, 0f, 0f }; // Red
        internal static float[] SelectedGeoset = new float[] { 54f/255f, 150f/255f,80f/255f }; // selected geoset
        internal static float[] EdgeColor = new float[] { 0f, 1f, 0f }; // Green

        // Rigging and normals colors
        internal static float[] RiggingColor = new float[] { 0f, 0f, 1f }; // Blue
        internal static float[] NormalsColor = new float[] { 1f, 1f, 0f }; // Yellow

        // Skeleton color
        internal static float[] SkeletonColor = new float[] { 0f, 1f, 1f }; // Cyan

        // Point settings
        internal static float PointSize = 1.0f;
        internal static PointType PointType = PointType.Square; // Assuming PointType is an enum

        // Vertex colors
        internal static float[] Color_VertexRigged = new float[] { 0.502f, 0.502f, 0.502f }; // Gray (128,128,128)
        internal static float[] NodeColorSelected = new float[] { 1f, 0.647f, 0f }; // Orange (255,165,0)
        internal static float[] NodeColor = new float[] { 1f, 1f, 1f }; // White
        internal static float[] Color_Vertex = new float[] { 1f, 1f, 1f }; // White
        internal static float[] Color_VertexSelected = new float[] { 1f, 0.412f, 0.706f }; // HotPink (255,105,180)

        // Lighting settings
        internal static bool EnableLighting = true;
        internal static float[] GridColor = new float[] { 0.663f, 0.663f, 0.663f }; // DarkGray (169,169,169)
        internal static float[] AmbientColor = new float[] { 1f, 1f, 1f }; // White
        internal static float[] DiffuseColor = new float[] { 1f, 1f, 1f }; // White
        internal static float[] SpecularColor = new float[] { 1f, 1f, 1f }; // White

        internal static float LightPostion = 0.0f;
        internal static float Shininess = 32.0f; // Default for shiny surfaces

        // Vertex rigged selected color
        internal static float[] Color_VertexRiggedSelected = new float[] { 1f, 0.271f, 0f }; // RedOrange (255,69,0)

        // Background color
        internal static float[] BackgroundColor = new float[] { 0.827f, 0.827f, 0.827f }; // LightGray (211,211,211)
 


        // Camera settings
        internal static double FieldOfView = 45.0; // Typical field of view in degrees
        internal static double NearDistance = 0.1; // Near clipping plane
        internal static double FarDistance = 5000.0; // Far clipping plane

        // Culling settings
        internal static bool BackfaceCullingEnabled = true;
        internal static bool BackfaceCullingClockwise = false; // Default is counter-clockwise
    }

}