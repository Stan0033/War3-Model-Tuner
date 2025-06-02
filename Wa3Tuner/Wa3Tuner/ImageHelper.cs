using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;

namespace Wa3Tuner
{
    internal class ImageHelper
    {


        internal static Bitmap Join(Bitmap first, Bitmap second)
        {
            // Create a new bitmap with the dimensions of the first image
            Bitmap result = new Bitmap(64, 64);

            // Set the DPI to 72x72
            result.SetResolution(72, 72);

            // Create a graphics object to draw the first image and then the second one on top
            using (Graphics g = Graphics.FromImage(result))
            {
                // Draw the first image as the base
                g.DrawImage(first, 0, 0);

                // Draw the second image on top of the first, respecting transparency
                g.DrawImage(second, 0, 0);
            }

            return result;
        }



        internal static Bitmap Load(string output)
        {
            throw new NotImplementedException();
        }

        internal static Bitmap Resize(Bitmap loadedImage, int height, int width)
        {
            // If the dimensions are already the same, return the original image
            if (loadedImage.Height == height && loadedImage.Width == width)
                return loadedImage;

            // Create a new bitmap with the desired dimensions
            Bitmap resized = new Bitmap(width, height);

            // Use high-quality interpolation for better visual results
            using (Graphics g = Graphics.FromImage(resized))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                g.DrawImage(loadedImage, 0, 0, width, height);
            }

            return resized;
        }
    }
}