using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
namespace W3_Texture_Finder
{
    internal static class AppHelper
    {
       public static string Local = AppDomain.CurrentDomain.BaseDirectory;
        internal static string TemporaryBLPLocation = Path.Combine(Local, "Temp.blp");
        internal static string TemporaryModelLocation = Path.Combine(Local, "Temp.mdx");
        internal static string Name = "War3 Model Tuner";

        internal static BitmapSource LoadBitmapImage(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                 
                throw new FileNotFoundException("The specified image file was not found.", path);
            }
               

            return new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
        }
    }
}