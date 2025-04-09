using MdxLib.Model;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
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
    }
    
    public static class RayCaster
    {

        public static List<xLine> Lines = new List<xLine>();
        public static Extent GetExtentFromRays(Vector3[] topLeft, Vector3[] botLeft, Vector3[] topRight, Vector3[] botRight)
        {
            // Gather all points from the rays
            Vector3[] points = topLeft.Concat(botLeft)
                                      .Concat(topRight)
                                      .Concat(botRight)
                                      .ToArray();

            // Determine min and max values
            float minX = points.Min(p => p.X);
            float minY = points.Min(p => p.Y);
            float minZ = points.Min(p => p.Z);
            float maxX = points.Max(p => p.X);
            float maxY = points.Max(p => p.Y);
            float maxZ = points.Max(p => p.Z);

            return new Extent(minX, minY, minZ, maxX, maxY, maxZ);
        }
        public static bool VertexWithinExtent(CGeosetVertex vertex, Extent extent)
        {
            Vector3 position = new Vector3(vertex.Position.X, vertex.Position.Y, vertex.Position.Z);

            return position.X >= extent.minX && position.X <= extent.maxX &&
                   position.Y >= extent.minY && position.Y <= extent.maxY &&
                   position.Z >= extent.minZ && position.Z <= extent.maxZ;
        }

 
        public static Vector3[] GetRay(
            Vector2 MousePosition,
            Vector3 CameraPosition,
            Vector3 CameraTarget,
            Vector3 CameraRoll,
            float CameraNearDistance,
            float CameraFarDistance,
            float CameraFieldOfView,
            Vector2 ScreenSize
            )
        {
            float ScreenWidth = ScreenSize.X;
            float ScreenHeight = ScreenSize.Y;
            // Convert FOV to radians
            float fovRadians = MathF.PI * CameraFieldOfView / 180.0f;
            float aspectRatio = ScreenWidth / ScreenHeight;

            // Calculate camera basis vectors
            Vector3 forward = Vector3.Normalize(CameraTarget - CameraPosition);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, CameraRoll));
            Vector3 up = Vector3.Normalize(Vector3.Cross(right, forward));

            // Convert mouse position to NDC (-1 to 1 range)
            float ndcX = (2.0f * MousePosition.X) / ScreenWidth - 1.0f;
            float ndcY = 1.0f - (2.0f * MousePosition.Y) / ScreenHeight;

            // Scale by aspect ratio and FOV
            float tanFov = MathF.Tan(fovRadians / 2.0f);
            Vector3 rayDir = Vector3.Normalize(forward + right * (ndcX * aspectRatio * tanFov) + up * (ndcY * tanFov));

            // Calculate ray start and end positions
            Vector3 From = CameraPosition + rayDir * CameraNearDistance;
            Vector3 To = CameraPosition + rayDir * CameraFarDistance;

            return new Vector3[2] { From, To };
        }
        public static bool IsRayTouchingVertex(Vector3 coordinate, float touchRange, Vector3 from, Vector3 to)
        {
            Vector3 direction = to - from;
            Vector3 fromToCoord = coordinate - from;

            float dot = Vector3.Dot(fromToCoord, Vector3.Normalize(direction));

            if (dot < 0 || dot > direction.Length())
                return false;

            Vector3 closestPoint = from + Vector3.Normalize(direction) * dot;

            return Vector3.Distance(closestPoint, coordinate) <= touchRange;
        }
        public static bool RayInsideExtent(Vector3 coordinate, Vector3 from, Vector3 to, Vector3 minExtent, Vector3 maxExtent)
        {
            Vector3 direction = to - from;
            Vector3 fromToCoord = coordinate - from;

            float dot = Vector3.Dot(fromToCoord, Vector3.Normalize(direction));

            if (dot < 0 || dot > direction.Length())
                return false;

            Vector3 closestPoint = from + Vector3.Normalize(direction) * dot;

            return closestPoint.X >= minExtent.X && closestPoint.X <= maxExtent.X &&
                   closestPoint.Y >= minExtent.Y && closestPoint.Y <= maxExtent.Y &&
                   closestPoint.Z >= minExtent.Z && closestPoint.Z <= maxExtent.Z;
        }
        public static Extent GetSelectionExtent(
      Vector2 topLeft, Vector2 topRight, Vector2 botLeft, Vector2 botRight,
      Vector3 cameraPos, Vector3 cameraTar, Vector3 cameraUp,
      OpenGL gl)
        {
            // Unproject each corner at near and far planes
            Vector3[] nearPoints = {
        Unproject(topLeft, 0.0f, cameraPos, cameraTar, cameraUp, gl),
        Unproject(topRight, 0.0f, cameraPos, cameraTar, cameraUp, gl),
        Unproject(botLeft, 0.0f, cameraPos, cameraTar, cameraUp, gl),
        Unproject(botRight, 0.0f, cameraPos, cameraTar, cameraUp, gl)
    };

            Vector3[] farPoints = {
        Unproject(topLeft, 1.0f, cameraPos, cameraTar, cameraUp, gl),
        Unproject(topRight, 1.0f, cameraPos, cameraTar, cameraUp, gl),
        Unproject(botLeft, 1.0f, cameraPos, cameraTar, cameraUp, gl),
        Unproject(botRight, 1.0f, cameraPos, cameraTar, cameraUp, gl)
    };

            // Compute min/max for bounding box
            float minX = nearPoints.Concat(farPoints).Min(p => p.X);
            float minY = nearPoints.Concat(farPoints).Min(p => p.Y);
            float minZ = nearPoints.Concat(farPoints).Min(p => p.Z);

            float maxX = nearPoints.Concat(farPoints).Max(p => p.X);
            float maxY = nearPoints.Concat(farPoints).Max(p => p.Y);
            float maxZ = nearPoints.Concat(farPoints).Max(p => p.Z);

            return new Extent(minX, minY, minZ, maxX, maxY, maxZ);
        }


        private static Vector3 Unproject(
    Vector2 screenPoint, float depth,
    Vector3 cameraPos, Vector3 cameraTar, Vector3 cameraUp,
    OpenGL gl)
        {
            int[] viewport = new int[4];
            float[] modelview = new float[16];
            float[] projection = new float[16];

            gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);
            gl.GetFloat(OpenGL.GL_MODELVIEW_MATRIX, modelview);
            gl.GetFloat(OpenGL.GL_PROJECTION_MATRIX, projection);

            // Convert screenPoint to OpenGL's normalized device coordinates (-1 to 1)
            float x = (screenPoint.X - viewport[0]) / viewport[2] * 2.0f - 1.0f;
            float y = (screenPoint.Y - viewport[1]) / viewport[3] * 2.0f - 1.0f;
            float z = depth * 2.0f - 1.0f; // Depth range is from 0 to 1, so convert to -1 to 1

            // Create 4D vector
            Vector4 screenCoords = new Vector4(x, y, z, 1.0f);

            // Compute inverse of projection * modelview
            Matrix4x4 modelViewMat = new Matrix4x4(
                modelview[0], modelview[1], modelview[2], modelview[3],
                modelview[4], modelview[5], modelview[6], modelview[7],
                modelview[8], modelview[9], modelview[10], modelview[11],
                modelview[12], modelview[13], modelview[14], modelview[15]
            );

            Matrix4x4 projectionMat = new Matrix4x4(
                projection[0], projection[1], projection[2], projection[3],
                projection[4], projection[5], projection[6], projection[7],
                projection[8], projection[9], projection[10], projection[11],
                projection[12], projection[13], projection[14], projection[15]
            );

            Matrix4x4 inverseMatrix = Matrix4x4.Invert(projectionMat * modelViewMat, out Matrix4x4 result) ? result : Matrix4x4.Identity;

            // Transform screen coordinates back to world space
            Vector4 worldCoords = Vector4.Transform(screenCoords, inverseMatrix);

            // Convert from homogeneous coordinates
            return new Vector3(worldCoords.X / worldCoords.W, worldCoords.Y / worldCoords.W, worldCoords.Z / worldCoords.W);
        }



        private static Vector3 UnprojectToWorld(Vector2 screenPoint, OpenGL gl)
        {
            double[] modelview = new double[16];
            double[] projection = new double[16];
            int[] viewport = new int[4];

            gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, modelview);
            gl.GetDouble(OpenGL.GL_PROJECTION_MATRIX, projection);
            gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);

            double winX = screenPoint.X;
            double winY = viewport[3] - screenPoint.Y; // OpenGL flips Y
            double winZ = 0.0; // Start from near plane

            Vector3 nearPoint = ComputeUnproject(winX, winY, winZ, modelview, projection, viewport);
            winZ = 1.0; // Far plane
            Vector3 farPoint = ComputeUnproject(winX, winY, winZ, modelview, projection, viewport);

            // Adjust scale
            Vector3 direction = Vector3.Normalize(farPoint - nearPoint);
            return nearPoint + direction * 100.0f; // Project further into the scene
        }

        private static Vector3 ComputeUnproject(double winX, double winY, double winZ, double[] modelview, double[] projection, int[] viewport)
        {
            Matrix4x4 modelViewMatrix = new Matrix4x4(
                (float)modelview[0], (float)modelview[4], (float)modelview[8], (float)modelview[12],
                (float)modelview[1], (float)modelview[5], (float)modelview[9], (float)modelview[13],
                (float)modelview[2], (float)modelview[6], (float)modelview[10], (float)modelview[14],
                (float)modelview[3], (float)modelview[7], (float)modelview[11], (float)modelview[15]
            );

            Matrix4x4 projectionMatrix = new Matrix4x4(
                (float)projection[0], (float)projection[4], (float)projection[8], (float)projection[12],
                (float)projection[1], (float)projection[5], (float)projection[9], (float)projection[13],
                (float)projection[2], (float)projection[6], (float)projection[10], (float)projection[14],
                (float)projection[3], (float)projection[7], (float)projection[11], (float)projection[15]
            );

            Matrix4x4 inverseMatrix;
            if (!Matrix4x4.Invert(projectionMatrix * modelViewMatrix, out inverseMatrix))
            {
                return Vector3.Zero;
            }

            Vector4 clipCoords = new Vector4(
                (float)((winX - viewport[0]) / (double)viewport[2] * 2.0 - 1.0),
                (float)((winY - viewport[1]) / (double)viewport[3] * 2.0 - 1.0),
                (float)(winZ * 2.0 - 1.0),
                1.0f
            );

            Vector4 worldCoords = Vector4.Transform(clipCoords, inverseMatrix);
            if (worldCoords.W != 0.0f)
            {
                worldCoords /= worldCoords.W;
            }

            return new Vector3(worldCoords.X, worldCoords.Y, worldCoords.Z);
        }


    }

}
