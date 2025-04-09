using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using W3_Texture_Finder;

namespace Wa3Tuner.Helper_Classes
{
  
    internal static class IconLoader2
    {
        public static void LoadIconToImageControl(System.Windows.Controls.Image control, string imageFileName)
        {
            string path = System.IO.Path.Combine(AppHelper.Local, $"icons\\{imageFileName}.jpg");

            // Create a BitmapImage to load the image from the file
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path, UriKind.Absolute);
            bitmap.EndInit();

            // Set the BitmapImage as the source of the Image control
            control.Source = bitmap;
        }
    }

}
