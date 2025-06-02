using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Drawing;
using War3Net.IO.Mpq;
using War3Net.Drawing.Blp;
using System.Drawing.Imaging;
using System.Windows.Documents;
namespace W3_Texture_Finder
{
    internal static class MPQHelper
    {
        internal static string? LocalModelFolder = "";
        internal static List<string> Listfile_blp_War3 = new();
        internal static List<string> Listfile_blp_War3x = new();
        internal static List<string> Listfile_blp_War3Patch = new();
        internal static List<string> Listfile_blp_War3xLocal = new();
        internal static List<string> Listfile_Models = new();
        internal static List<string> Listfile_All = new();
        internal static List<string> Listfile_Everything_war3 = new();
        internal static List<string> Listfile_Everything_war3x = new();
        internal static List<string> Listfile_Everything_war3xLocal = new();
        internal static List<string> Listfile_Everything_war3Patch = new();
        internal static string Path_War3xLocal;
        internal static string Path_War3;
        internal static string Path_War3x;
        internal static string Path_War3Patch;

        internal static void FillAllArchives()
        {
            Listfile_Everything_war3 = LoadArchive(MPQPaths.War3);
            Listfile_Everything_war3x = LoadArchive(MPQPaths.War3X);
            Listfile_Everything_war3xLocal = LoadArchive(MPQPaths.War3xLocal);
            Listfile_Everything_war3Patch = LoadArchive(MPQPaths.War3Patch);
        }
        internal static void Initialize()
        {
            if (MPQPaths.local == null) { throw new Exception("null local path"); }
            string War3PatchPath = Path.Combine(MPQPaths.local, "Paths\\War3Patch.txt");
            if (File.Exists(War3PatchPath) == false) { MessageBox.Show($"{War3PatchPath} not found"); Environment.Exit(0); }
            Listfile_blp_War3Patch = File.ReadAllLines(War3PatchPath).Where(line => line.EndsWith(".blp")).ToList();
            LoadDataBrowserLists(MPQPaths.War3, Listfile_blp_War3);
            LoadDataBrowserLists(MPQPaths.War3X, Listfile_blp_War3x);
            LoadDataBrowserLists(MPQPaths.War3xLocal, Listfile_blp_War3xLocal);
            Listfile_All.AddRange(Listfile_blp_War3);
            Listfile_All.AddRange(Listfile_blp_War3x);
            Listfile_All.AddRange(Listfile_blp_War3Patch);
            Listfile_All.AddRange(Listfile_blp_War3xLocal);
            FillAllArchives();
        }
        internal static bool FileExists(string path)
        {
            string searched = path.ToLower().Trim();
            if (Listfile_All.Any(x => x.ToLower() == searched)) { return true; }
            return File.Exists(path);
        }
        internal static bool FileExists(string path, string Archive)
        {
            if (Path.GetExtension(path.ToLower()) == ".mdx")
            {
                return Listfile_Models.Contains(path);
            }
            if (Archive == MPQPaths.War3) { return Listfile_blp_War3.Contains(path, StringComparer.OrdinalIgnoreCase); }
            if (Archive == MPQPaths.War3X) { return Listfile_blp_War3x.Contains(path, StringComparer.OrdinalIgnoreCase); }
            if (Archive == MPQPaths.War3Patch) { return Listfile_blp_War3Patch.Contains(path, StringComparer.OrdinalIgnoreCase); }
            if (Archive == MPQPaths.War3xLocal) { return Listfile_blp_War3xLocal.Contains(path, StringComparer.OrdinalIgnoreCase); }
            return false;
        }
        internal static void Export(string targetPath, string savePath, string archive = "", bool check = false)
        {

            if (check)
            {
                if (FileExists(targetPath, MPQPaths.War3)) { archive = MPQPaths.War3; }
                else if (FileExists(targetPath, MPQPaths.War3X)) { archive = MPQPaths.War3X; }
                else if (FileExists(targetPath, MPQPaths.War3xLocal)) { archive = MPQPaths.War3xLocal; }
                else if (FileExists(targetPath, MPQPaths.War3Patch)) { archive = MPQPaths.War3Patch; }
                else
                {
                    MessageBox.Show($"{targetPath} was not found in any if the four MPQs"); return;
                }
            }

                using (MpqArchive mpqArchive = MpqArchive.Open(archive))
                {
                    if (mpqArchive != null)
                    {
                        // Specify the file path within the MPQ archive
                        // Check if the file exists in the archive
                        if (mpqArchive.FileExists(targetPath))
                        {
                            using (MpqStream mpqStream = mpqArchive.OpenFile(targetPath))
                            {
                                // Create a FileStream and write the contents of the MPQ stream directly to it
                                using (FileStream fileStream = new FileStream(savePath, FileMode.Create))
                                {
                                    byte[] buffer = new byte[4096]; // 4KB buffer size
                                    int bytesRead;
                                    while ((bytesRead = mpqStream.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        fileStream.Write(buffer, 0, bytesRead);
                                    }
                                }
                            }
                        }
                    }
                }
        }
        internal static void ExportPNG(ImageSource imageSource, string outputPath)
        {
            // Convert ImageSource to BitmapSource
            var bitmapSource = (BitmapSource)imageSource;
            // Create a PngBitmapEncoder
            var encoder = new PngBitmapEncoder();
            // Add the BitmapSource to the encoder
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            // Create a FileStream to write the image file
            using (var fileStream = new FileStream(outputPath, FileMode.Create))
            {
                // Save the image to the FileStream
                encoder.Save(fileStream);
            }
        }
        internal static ImageSource? GetImageSourceExternal(string? fullPath)
        {
            if (fullPath == null)return null;
            if (File.Exists(fullPath))
            {
                using (FileStream mpqStream = File.OpenRead(fullPath))
                {
                    // Read from the stream
                    byte[] buffer = new byte[mpqStream.Length];
                    using (var fileStream = File.OpenRead(fullPath))
                    {
                        var blpFile = new BlpFile(fileStream);
                        var bitmapSource = blpFile.GetBitmapSource();
                        return bitmapSource;
                    }
                }
            }
            else
            {
                MessageBox.Show($"Missing file {fullPath}");
                return null;
            }
        }
        internal static ImageSource? GetImageSource(string path)
        {




            string archive = string.Empty;
            if (FileExists(path, MPQPaths.War3)) { archive = MPQPaths.War3; }
            if (FileExists(path, MPQPaths.War3X)) { archive = MPQPaths.War3X; }
            if (FileExists(path, MPQPaths.War3xLocal)) { archive = MPQPaths.War3xLocal; }
            if (FileExists(path, MPQPaths.War3Patch)) { archive = MPQPaths.War3Patch; }
            if (archive.Length == 0) // Could be in local folder
            {
                if (LocalModelFolder == null) { throw new Exception("null LocalModelFolder"); }
                string fullPath = Path.Combine(LocalModelFolder, path);
                if (File.Exists(fullPath))
                {
                    using (var fileStream = File.OpenRead(fullPath))
                    {
                        var blpFile = new BlpFile(fileStream);
                        var bitmapSource = blpFile.GetBitmapSource();
                        return bitmapSource;
                    }
                }
                else
                {
                    MessageBox.Show($"Missing file {path}");
                    return null;
                }
            }
            else
            {
                try
                {
                    using (MpqArchive mpqArchive = MpqArchive.Open(archive))
                    {
                        // Check if the archive is valid
                        if (mpqArchive != null)
                        {
                            // Check if the file exists in the archive
                            if (mpqArchive.FileExists(path))
                            {
                                // Open the file stream
                                using (MpqStream mpqStream = mpqArchive.OpenFile(path))
                                {
                                    // Read from the stream
                                    byte[] buffer = new byte[mpqStream.Length];
                                    mpqStream.Read(buffer, 0, buffer.Length);
                                    //-----------------------
                                    // Save the file
                                    //-----------------------
                                    string outputPath = AppHelper.TemporaryBLPLocation;
                                    File.WriteAllBytes(outputPath, buffer);
                                    using (var fileStream = File.OpenRead(outputPath))
                                    {
                                        var blpFile = new BlpFile(fileStream);
                                        var bitmapSource = blpFile.GetBitmapSource();
                                        // Delete the temporary file
                                        //   File.Delete(outputPath);
                                        // Return the ImageSource
                                        return bitmapSource;
                                    }
                                }
                            }
                            else
                            {
                                return null; // or throw an exception indicating file not found
                            }
                        }
                        else
                        {
                            return null; // or throw an exception indicating invalid archive
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, $"An excepion occured while trying to open {archive}");
                    return null;
                }
            }
            
        }
        static ImageSource ReplaceTransparentPixelsWithBlack(BitmapSource bitmapSource)
        {
            var formatConvertedBitmap = new FormatConvertedBitmap();
            formatConvertedBitmap.BeginInit();
            formatConvertedBitmap.Source = bitmapSource;
            formatConvertedBitmap.DestinationFormat = PixelFormats.Bgra32;
            formatConvertedBitmap.EndInit();
            var width = formatConvertedBitmap.PixelWidth;
            var height = formatConvertedBitmap.PixelHeight;
            var stride = width * 4;
            var pixels = new byte[height * stride];
            formatConvertedBitmap.CopyPixels(pixels, stride, 0);
            for (int i = 3; i < pixels.Length; i += 4)
            {
                if (pixels[i] == 0) // Check if the pixel is fully transparent
                {
                    pixels[i - 3] = 0; // Set blue channel to 0
                    pixels[i - 2] = 0; // Set green channel to 0
                    pixels[i - 1] = 0; // Set red channel to 0
                }
            }
            var blackBitmapSource = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, pixels, stride);
            return blackBitmapSource;
        }
        //         MPQPaths.temp
        internal static Bitmap getBitmapImage(string path, string modelFolder = "")
        {
            string archive = string.Empty;
            string FullPath = System.IO.Path.Combine(modelFolder, path);
            if (FileExists(path, MPQPaths.War3)) { archive = MPQPaths.War3; }
            if (FileExists(path, MPQPaths.War3X)) { archive = MPQPaths.War3X; }
            if (FileExists(path, MPQPaths.War3xLocal)) { archive = MPQPaths.War3xLocal; }
            if (FileExists(path, MPQPaths.War3Patch)) { archive = MPQPaths.War3Patch; }
            if (archive.Length == 0) // could be in local folder
            {
                if (File.Exists(FullPath))
                {
                    
                    using (FileStream mpqStream = File.OpenRead(FullPath))
                    {
                        // Read from the stream
                        byte[] buffer = new byte[mpqStream.Length];
                        mpqStream.Read(buffer, 0, buffer.Length);
                        //-----------------------
                        // save the file
                        //-----------------------
                        string outputPath = AppHelper.TemporaryBLPLocation;
                        File.WriteAllBytes(outputPath, buffer);
                        using (var fileStream = File.OpenRead(outputPath))
                        {
                            var blpFile = new BlpFile(fileStream);
                            var bitmapSource = blpFile.GetBitmapSource();
                            // Convert BitmapSource to Bitmap
                            var bitmap = new Bitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                            var data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                            bitmapSource.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
                            bitmap.UnlockBits(data);
                            // Delete the temporary file
                            try { File.Delete(outputPath); }
                            catch { return bitmap; }
                            // Return the Bitmap
                            return bitmap;
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"Could not find the texture at \"{FullPath}\"");
                    return GetWhiteBitmap();
                }
            }
            else
            {
                using (MpqArchive mpqArchive = MpqArchive.Open(archive))
                {
                    // Check if the archive is valid
                    if (mpqArchive != null)
                    {
                        // Specify the file path within the MPQ archive
                        // Check if the file exists in the archive
                        if (mpqArchive.FileExists(path))
                        {
                            // Open the file stream
                            using (MpqStream mpqStream = mpqArchive.OpenFile(path))
                            {
                                // Read from the stream
                                byte[] buffer = new byte[mpqStream.Length];
                                mpqStream.Read(buffer, 0, buffer.Length);
                                //-----------------------
                                // save the file
                                //-----------------------
                                string outputPath = MPQPaths.temp;
                                File.WriteAllBytes(outputPath, buffer);
                                using (var fileStream = File.OpenRead(outputPath))
                                {
                                    var blpFile = new BlpFile(fileStream);
                                    var bitmapSource = blpFile.GetBitmapSource();
                                    // Convert BitmapSource to Bitmap
                                    var bitmap = new Bitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                                    var data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                                    bitmapSource.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
                                    bitmap.UnlockBits(data);
                                    // Delete the temporary file
                                    // File.Delete(outputPath);
                                    // Return the Bitmap
                                    return bitmap;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Could not find the texture at \"{path}\"");
                            return GetWhiteBitmap();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Invalid archive");
                        return GetWhiteBitmap();
                    }
                }
            }
        }
        private static Bitmap GetWhiteBitmap()
        {
            return MPQHelper.getBitmapImage("Textures\\white.blp");
        }
        private static void LoadPresetListFile(List<string> list_blp)
        {

            string outputPath = Path.Combine(AppHelper.Local, $"Paths\\(listfile)");
            
            foreach (string item in File.ReadAllLines(outputPath))
            {
                if (Path.GetExtension(item) == ".blp")
                {
                    list_blp.Add(item);
                }
                if (Path.GetExtension(item) == ".mdx")
                {
                    if (!Listfile_Models.Contains(item)) Listfile_Models.Add(item);
                }
            }
        }
        private static void LoadDataBrowserLists(string Archive, List<string> list_blp)
        {
            string searched = "(listfile)";

            try
            {
                using (MpqArchive mpqArchive = MpqArchive.Open(Archive))
                {
                    if (mpqArchive.FileExists(searched) == false)
                    {
                        MessageBox.Show($"Could not find {searched} inside {Archive}. Switching to preset.");
                        LoadPresetListFile(list_blp);
                        return;
                    }
                    using (MpqStream mpqStream = mpqArchive.OpenFile(searched))
                    {
                        // Read from the stream
                        byte[] buffer = new byte[mpqStream.Length];
                        mpqStream.Read(buffer, 0, buffer.Length);
                        //-----------------------
                        // save the file
                        //-----------------------
                        string outputPath = MPQPaths.temp;
                        File.WriteAllBytes(outputPath, buffer);
                        foreach (string item in File.ReadAllLines(outputPath))
                        {
                            if (Path.GetExtension(item) == ".blp")
                            {
                                list_blp.Add(item);
                            }
                            if (Path.GetExtension(item) == ".mdx")
                            {
                                if (!Listfile_Models.Contains(item)) Listfile_Models.Add(item);
                            }
                            //  string v = Path.Combine(AppHelper.Local, $"Paths\\(listfile)");
                            //  File.AppendAllLines(v, new { item });
                        }


                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"An excepion occured while trying to open {Archive}");
                
            }
        }



        private static List<string> LoadArchive(string Archive)
        {
            List<string> list = new();
            string searched = "(listfile)";

            try
            {
                using (MpqArchive mpqArchive = MpqArchive.Open(Archive))
                {
                    if (mpqArchive.FileExists(searched) == false)
                    {
                        MessageBox.Show($"Could not find {searched} inside {Archive}. Switching to preset.");
                        string outputPath = Path.Combine(AppHelper.Local, $"Paths\\(listfile)");
                        return File.ReadAllLines(outputPath).ToList();
                    }
                    using (MpqStream mpqStream = mpqArchive.OpenFile(searched))
                    {
                        // Read from the stream
                        byte[] buffer = new byte[mpqStream.Length];
                        mpqStream.Read(buffer, 0, buffer.Length);
                        //-----------------------
                        // save the file
                        //-----------------------
                        string outputPath = MPQPaths.temp;
                        File.WriteAllBytes(outputPath, buffer);
                        foreach (string item in File.ReadAllLines(outputPath))
                        {
                            list.Add(item);
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"An excepion occured while trying to open {Archive}");

            }
           return list;
        }
        internal static BitmapImage? getBitmapImage_image(string path)
        {
            string archive = string.Empty;
            if (FileExists(path, MPQPaths.War3)) { archive = MPQPaths.War3; }
            if (FileExists(path, MPQPaths.War3X)) { archive = MPQPaths.War3X; }
            if (FileExists(path, MPQPaths.War3xLocal)) { archive = MPQPaths.War3xLocal; }
            if (FileExists(path, MPQPaths.War3Patch)) { archive = MPQPaths.War3Patch; }
            if (archive.Length == 0) // could be in local folder
            {
                if (File.Exists(path))
                {
                    try
                    {
                        using (FileStream mpqStream = File.OpenRead(path))
                        {
                            byte[] buffer = new byte[mpqStream.Length];
                            mpqStream.Read(buffer, 0, buffer.Length);
                            // Save the file temporarily
                            string outputPath = AppHelper.TemporaryBLPLocation;
                            File.WriteAllBytes(outputPath, buffer);
                            using (var fileStream = File.OpenRead(outputPath))
                            {
                                var blpFile = new BlpFile(fileStream);
                                var bitmapSource = blpFile.GetBitmapSource();
                                // Convert BitmapSource to BitmapImage (directly return the BitmapImage)
                                var bitmapImage = new BitmapImage();
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                                    encoder.Save(memoryStream);
                                    memoryStream.Seek(0, SeekOrigin.Begin);
                                    bitmapImage.BeginInit();
                                    bitmapImage.StreamSource = memoryStream;
                                    bitmapImage.EndInit();
                                }
                                // Delete the temporary file
                                File.Delete(outputPath);
                                // Return the BitmapImage
                                return bitmapImage;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, $"An excepion occured while trying to open {path}");
                        return null;
                    }
               }
                else
                {
                    MessageBox.Show($"Could not find the texture at \"{path}\"");
                    return GetWhiteBitmapImage();  // Return a white image as a fallback
                }
            }
            else
            {
                using (MpqArchive mpqArchive = MpqArchive.Open(archive))
                {
                    if (mpqArchive != null)
                    {
                        if (mpqArchive.FileExists(path))
                        {
                            using (MpqStream mpqStream = mpqArchive.OpenFile(path))
                            {
                                byte[] buffer = new byte[mpqStream.Length];
                                mpqStream.Read(buffer, 0, buffer.Length);
                                // Save the file temporarily
                                string outputPath = MPQPaths.temp;
                                File.WriteAllBytes(outputPath, buffer);
                                using (var fileStream = File.OpenRead(outputPath))
                                {
                                    var blpFile = new BlpFile(fileStream);
                                    var bitmapSource = blpFile.GetBitmapSource();
                                    // Convert BitmapSource to BitmapImage (directly return the BitmapImage)
                                    var bitmapImage = new BitmapImage();
                                    using (MemoryStream memoryStream = new MemoryStream())
                                    {
                                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                                        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                                        encoder.Save(memoryStream);
                                        memoryStream.Seek(0, SeekOrigin.Begin);
                                        bitmapImage.BeginInit();
                                        bitmapImage.StreamSource = memoryStream;
                                        bitmapImage.EndInit();
                                    }
                                    // Return the BitmapImage
                                    return bitmapImage;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Could not find the texture at \"{path}\"");
                            return GetWhiteBitmapImage();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Invalid archive");
                        return GetWhiteBitmapImage();
                    }
                }
            }
        }
        private static BitmapImage? GetWhiteBitmapImage()
        {
            return getBitmapImage_image("Textures\\white.blp");
        }
    }
}
