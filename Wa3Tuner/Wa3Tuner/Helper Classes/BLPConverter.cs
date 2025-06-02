using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Shell;
using W3_Texture_Finder;

namespace Wa3Tuner
{
    internal class BLPConverter
    {
        internal static void Convert(string inputPath, string outputPath, MainWindow window, string deleteFile)
        {
            window.IsEnabled = false;
            string ConverterExe = System.IO.Path.Combine(AppHelper.Local, "Tools\\blplabcl.exe");
            string tgaFile = System.IO.Path.ChangeExtension(inputPath, ".tga");
            ConvertImageToTGA(inputPath, tgaFile);
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.FileName = ConverterExe;
            string opt1 = "-opt1";
            string opt2 = string.Empty;
            
            startInfo.Arguments = $"\"{tgaFile}\" \"{outputPath}\" -type{0} -q{100} -mm{1} {opt1} {opt2}";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            process.Kill();
            File.Delete(tgaFile);
          if (File.Exists(deleteFile))  File.Delete(deleteFile);
            window.IsEnabled = true;
        }
        private static void ConvertImageToTGA(string inputPath, string tgaFile)
        {
            using (Bitmap bmp = new Bitmap(inputPath))
            {
                WriteTga(bmp, tgaFile);
            }
        }

        private static void WriteTga(Bitmap bitmap, string path)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                // Write the 18-byte TGA header
                writer.Write((byte)0);                // ID length
                writer.Write((byte)0);                // No color map
                writer.Write((byte)2);                // Uncompressed, true-color image
                writer.Write((short)0);               // Color map origin
                writer.Write((short)0);               // Color map length
                writer.Write((byte)0);                // Color map entry size
                writer.Write((short)0);               // X-origin
                writer.Write((short)0);               // Y-origin
                writer.Write((short)bitmap.Width);    // Width
                writer.Write((short)bitmap.Height);   // Height
                writer.Write((byte)(bitmap.PixelFormat == PixelFormat.Format32bppArgb ? 32 : 24)); // Bits per pixel
                writer.Write((byte)0x20);              // Image descriptor (origin in top-left)

                // Write pixel data
                for (int y = bitmap.Height - 1; y >= 0; y--) // Bottom-up
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        Color color = bitmap.GetPixel(x, y);
                        writer.Write(color.B);
                        writer.Write(color.G);
                        writer.Write(color.R);
                        if (bitmap.PixelFormat == PixelFormat.Format32bppArgb)
                        {
                            writer.Write(color.A);
                        }
                    }
                }
            }
        }

    }
}