using System;
using System.Drawing;
using System.IO;

namespace Wa3Tuner
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.IO;

    internal class ImageLoader
    {
        internal static Bitmap CreateGrassTexture(int width, int height)
        {
            Bitmap grassBitmap = new Bitmap(width, height);
            Random rand = new Random();

            using (Graphics g = Graphics.FromImage(grassBitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.ForestGreen); // Base color for grass

                // Draw random grass blades
                for (int i = 0; i < width * height / 100; i++)
                {
                    int x = rand.Next(width);
                    int y = rand.Next(height);
                    int length = rand.Next(5, 15);
                    int thickness = rand.Next(1, 3);

                    Color grassColor = Color.FromArgb(rand.Next(80, 180), 0, rand.Next(120, 200), 0);
                    using (Pen grassPen = new Pen(grassColor, thickness))
                    {
                        g.DrawLine(grassPen, x, y, x, y - length);
                    }
                }

                // Add noise for texture variation
                for (int i = 0; i < width * height / 50; i++)
                {
                    int x = rand.Next(width);
                    int y = rand.Next(height);
                    grassBitmap.SetPixel(x, y, Color.FromArgb(rand.Next(50, 100), 0, rand.Next(100, 150), 0));
                }
            }

            return grassBitmap;
        }

        /// <summary>
        /// Loads a PNG image from the specified file path and returns it as a Bitmap.
        /// </summary>
        /// <param name="path">The file path of the PNG image.</param>
        /// <returns>A Bitmap representation of the loaded image.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided path is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs while accessing the file.</exception>
        /// <exception cref="ArgumentException">Thrown when the file is not a valid PNG image.</exception>
        internal static Bitmap LoadPNG(string path)
        {
            // Validate the input path
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("The file path cannot be null, empty, or contain only white spaces.", nameof(path));
            }

            // Verify that the file exists before proceeding
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The specified file was not found: {path}");
            }

            try
            {
                // Open the file stream with read-only access
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // Ensure that the file is a PNG by checking the header
                    if (!IsPngFile(fileStream))
                    {
                        throw new ArgumentException("The provided file is not a valid PNG image.");
                    }

                    // Reset stream position before loading the image
                    fileStream.Seek(0, SeekOrigin.Begin);

                    // Load the bitmap from the stream
                    return new Bitmap(fileStream);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new IOException($"Access to the file is denied: {path}", ex);
            }
            catch (OutOfMemoryException ex)
            {
                throw new IOException("The file is not a valid image or is corrupted.", ex);
            }
            catch (Exception ex)
            {
                throw new IOException("An unexpected error occurred while loading the image.", ex);
            }
        }

       
        internal static System.Drawing.Image LoadPNGAsImage(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Image path cannot be null or empty.", nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException("The specified image file does not exist.", path);

            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return Image.FromStream(stream);
                }
            }
            catch (Exception ex)
            {
                throw new IOException("Failed to load the image from the specified path.", ex);
            }
        }

        /// <summary>
        /// Checks if the provided file stream contains a valid PNG file signature.
        /// </summary>
        /// <param name="stream">The file stream to check.</param>
        /// <returns>True if the file is a PNG; otherwise, false.</returns>
        private static bool IsPngFile(FileStream stream)
        {
            // PNG files start with an 8-byte signature: 89 50 4E 47 0D 0A 1A 0A
            byte[] pngSignature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            byte[] fileSignature = new byte[8];

            // Read the first 8 bytes of the file
            stream.Seek(0, SeekOrigin.Begin);
            int bytesRead = stream.Read(fileSignature, 0, 8);

            // Compare the signatures
            if (bytesRead < 8) return false;
            for (int i = 0; i < 8; i++)
            {
                if (fileSignature[i] != pngSignature[i])
                {
                    return false;
                }
            }
            return true;
        }
    }

}