using MdxLib.Model;
using SharpGL;
using SharpGL.SceneGraph;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Whim_GEometry_Editor.Misc;

namespace Wa3Tuner.Helper_Classes
{
    public class xLine
    {
        public Vector3 one, two;
        public xLine(Vector3[] v)
        {
            one = v[0];
            two = v[1];
        }
        public xLine(Vector3 n, Vector3 f)
        {
            one = n; two = f;
        }

        internal void Negate()
        {
            one.X= -one.X;
            one.Y=-one.Y;
            one.Z=-one.Z;

            two.X= -two.X;
            two.Y= -two.Y;
            two.Z= -two.Z;
        }
    }

    public static class RayCaster
    {
        internal static List<xLine> Lines = new();

        public static Vector3[] GetRay(OpenGL gl)
        {
            // 1. Get the projection and modelview matrices from OpenGL
            float[] projectionMatrix = new float[16];
            float[] modelviewMatrix = new float[16];
            gl.GetFloat(OpenGL.GL_PROJECTION_MATRIX, projectionMatrix);
            gl.GetFloat(OpenGL.GL_MODELVIEW_MATRIX, modelviewMatrix);

            // 2. Convert the modelview matrix into a Matrix4 (a helper class for easier manipulation)
            Matrix4x4 modelview = new Matrix4x4(
                modelviewMatrix[0], modelviewMatrix[1], modelviewMatrix[2], modelviewMatrix[3],
                modelviewMatrix[4], modelviewMatrix[5], modelviewMatrix[6], modelviewMatrix[7],
                modelviewMatrix[8], modelviewMatrix[9], modelviewMatrix[10], modelviewMatrix[11],
                modelviewMatrix[12], modelviewMatrix[13], modelviewMatrix[14], modelviewMatrix[15]
            );

            // 3. Invert the modelview matrix to get the camera's position in world space
            bool b = Matrix4x4.Invert(modelview, out Matrix4x4 invModelview);

            // 4. The camera position is at the origin (0, 0, 0) in the camera space,
            // so we transform the origin (0, 0, 0, 1) back to world space.
            Vector4 cameraPosition = new Vector4(0, 0, 0, 1);  // Origin in camera space
            cameraPosition = Vector4.Transform(cameraPosition, invModelview);  // Camera position in world space

            // 5. We assume the camera is looking in the negative Z direction
            // A simple way to get the camera target is to subtract 1 unit along the negative Z axis from the camera position
            Vector3 cameraTarget = new Vector3(cameraPosition.X, cameraPosition.Y, cameraPosition.Z - 1);

            // 6. Return the camera position and target (ray direction)
          var v= new Vector3[] { new Vector3(cameraPosition.X, cameraPosition.Y, cameraPosition.Z), cameraTarget };
            Lines.Add(new xLine(new Vector3(cameraPosition.X, cameraPosition.Y, cameraPosition.Z), cameraTarget));
            return v;
        }





    }



}
