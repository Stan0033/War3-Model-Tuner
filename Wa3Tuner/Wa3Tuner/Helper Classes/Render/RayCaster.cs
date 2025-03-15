using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Wa3Tuner.Helper_Classes
{
    public static class RayCaster
    {
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
    }

}
