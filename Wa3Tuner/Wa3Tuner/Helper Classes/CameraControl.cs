using System;
using System.Numerics;

namespace Wa3Tuner.Helper_Classes
{
    internal static class CameraControl
    {
        // the camera of the sharpgl viewport
        public static float eyeX = 180;
        public static float eyeY = 0;
        public static float eyeZ = 60;
        public static float CenterX = 0;
        public static float CenterY = 0;
        public static float CenterZ = 0;
        public static float UpX = 0;
        public static float UpY = 0;
        public static float UpZ = 90;
        public static float EndEyeX = 0;
        public static void Reset()
        {
            eyeX = 180;
            eyeY = 0;
            eyeZ = 60;
            CenterX = 0;
            CenterY = 0;
            CenterZ = 0;
            UpX = 0; UpY = 0;
            UpZ = 90;

        }
        private static bool movingForward = false;
        internal static float ZoomIncrement = 0.5f;

        public static void UpdateCameraPosition()
        {
            // Check the movement direction and adjust eyeX and eyeY accordingly
            if (movingForward)
            {
                // Move eyeY forward-right
                if (eyeY < EndEyeX && eyeY > -EndEyeX)
                {
                    eyeY++;
                }

                // Move eyeX forward-backward
                if (eyeX > -EndEyeX && eyeX < EndEyeX)
                {
                    eyeX--;
                }

                // Check if both have reached their limits, if so, reverse the direction
                if ((eyeX <= -EndEyeX || eyeX >= EndEyeX) && (eyeY <= -EndEyeX || eyeY >= EndEyeX))
                {
                    movingForward = false; // Reverse direction
                }
            }
            else
            {
                // Move eyeY backward-left
                if (eyeY <= EndEyeX && eyeY >= -EndEyeX)
                {
                    eyeY--;
                }

                // Move eyeX backward-forward
                if (eyeX >= -EndEyeX && eyeX <= EndEyeX)
                {
                    eyeX++;
                }

                // Check if both have reached the other limit, if so, reverse the direction again
                if ((eyeX >= EndEyeX || eyeX <= -EndEyeX) && (eyeY >= EndEyeX || eyeY <= -EndEyeX))
                {
                    movingForward = true; // Switch direction back to forward
                }
            }
        }

        internal static void RotateUp(Vector3 currentRotateCentroid, float angleDegrees)
        {
            // Convert the angle from degrees to radians
            float angleRadians = (float)(angleDegrees * Math.PI / 180.0f);

            // Get the direction vector from the eye to the centroid
            float dirX = eyeX - currentRotateCentroid.X;
            float dirY = eyeY - currentRotateCentroid.Y;
            float dirZ = eyeZ - currentRotateCentroid.Z;

            // Calculate the new eye position after rotation around the Y-axis (horizontal right rotation)
            float cosAngle = (float)Math.Cos(angleRadians);
            float sinAngle = (float)Math.Sin(angleRadians);

            // Rotate the direction vector around the Y-axis (assuming horizontal rotation)
            float newDirX = cosAngle * dirX - sinAngle * dirZ;
            float newDirZ = sinAngle * dirX + cosAngle * dirZ;

            // Update the eye position (the distance from the centroid stays the same)
            eyeX = currentRotateCentroid.X + newDirX;
            eyeZ = currentRotateCentroid.Z + newDirZ;

            // Optional: The up vector remains fixed, with UpZ set to 90
            UpX = 0.0f;
            UpY = 1.0f;  // Keep the up direction pointing upwards
            UpZ = 90.0f; // Fixed UpZ value of 90
        }
        internal static void RotateDown(Vector3 currentRotateCentroid, float angleDegrees)
        {
            // Convert the angle from degrees to radians (negative for left rotation)
            float angleRadians = (float)(-angleDegrees * Math.PI / 180.0f);

            // Get the direction vector from the eye to the centroid
            float dirX = eyeX - currentRotateCentroid.X;
            float dirY = eyeY - currentRotateCentroid.Y;
            float dirZ = eyeZ - currentRotateCentroid.Z;

            // Calculate the new eye position after rotation around the Y-axis (horizontal left rotation)
            float cosAngle = (float)Math.Cos(angleRadians);
            float sinAngle = (float)Math.Sin(angleRadians);

            // Rotate the direction vector around the Y-axis (assuming horizontal rotation)
            float newDirX = cosAngle * dirX - sinAngle * dirZ;
            float newDirZ = sinAngle * dirX + cosAngle * dirZ;

            // Update the eye position (the distance from the centroid stays the same)
            eyeX = currentRotateCentroid.X + newDirX;
            eyeZ = currentRotateCentroid.Z + newDirZ;

            // Optional: The up vector remains fixed, with UpZ set to 90
            UpX = 0.0f;
            UpY = 1.0f;  // Keep the up direction pointing upwards
            UpZ = 90.0f; // Fixed UpZ value of 90
        }

        internal static void RotateLeft(Vector3 currentRotateCentroid, float angleDegrees)
        {
            // Convert the angle from degrees to radians
            float angleRadians = (float)(angleDegrees * Math.PI / 180.0f);

            // Get the direction vector from the eye to the centroid
            float dirX = eyeX - currentRotateCentroid.X;
            float dirY = eyeY - currentRotateCentroid.Y;
            float dirZ = eyeZ - currentRotateCentroid.Z;

            // Calculate the new eye position after rotation around the Z-axis (left rotation)
            float cosAngle = (float)Math.Cos(angleRadians);
            float sinAngle = (float)Math.Sin(angleRadians);

            // Rotate the direction vector around the Z-axis
            float newDirX = cosAngle * dirX - sinAngle * dirY;
            float newDirY = sinAngle * dirX + cosAngle * dirY;

            // Update the eye position (the distance from the centroid stays the same)
            eyeX = currentRotateCentroid.X + newDirX;
            eyeY = currentRotateCentroid.Y + newDirY;

            // Keep UpZ at 90 for this camera orientation
            UpX = 0.0f;
            UpY = 1.0f;
            UpZ = 90.0f;
        }

        internal static void RotateRight(Vector3 currentRotateCentroid, float angleDegrees)
        {
            // Convert the angle from degrees to radians (negative for right rotation)
            float angleRadians = (float)(-angleDegrees * Math.PI / 180.0f);

            // Get the direction vector from the eye to the centroid
            float dirX = eyeX - currentRotateCentroid.X;
            float dirY = eyeY - currentRotateCentroid.Y;
            float dirZ = eyeZ - currentRotateCentroid.Z;

            // Calculate the new eye position after rotation around the Z-axis (right rotation)
            float cosAngle = (float)Math.Cos(angleRadians);
            float sinAngle = (float)Math.Sin(angleRadians);

            // Rotate the direction vector around the Z-axis
            float newDirX = cosAngle * dirX - sinAngle * dirY;
            float newDirY = sinAngle * dirX + cosAngle * dirY;

            // Update the eye position (the distance from the centroid stays the same)
            eyeX = currentRotateCentroid.X + newDirX;
            eyeY = currentRotateCentroid.Y + newDirY;

            // Keep UpZ at 90 for this camera orientation
            UpX = 0.0f;
            UpY = 1.0f;
            UpZ = 90.0f;
        }


    }
}