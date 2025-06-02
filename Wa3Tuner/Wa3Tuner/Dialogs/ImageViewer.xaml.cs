using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Image = System.Windows.Controls.Image;

namespace Wa3Tuner.Dialogs
{
    /// <summary>
    /// Interaction logic for ImageViewer.xaml
    /// </summary>
    public partial class ImageViewer : Window
    {
       
        public ImageViewer(string whichBitmap)
        {
            InitializeComponent();

            LoadPictureIntoImageControl(whichBitmap, mainImage);
        }
        public ImageViewer(Image? image)
        {
            InitializeComponent();

            mainImage.Source = image==null? null: image.Source;
        }

        private static void LoadPictureIntoImageControl(string imagePath, Image imageControl)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                throw new ArgumentException("Image path cannot be null or empty.", nameof(imagePath));

            if (!File.Exists(imagePath))
                throw new FileNotFoundException("The specified image file does not exist.", imagePath);

            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze(); // Make it thread-safe

                imageControl.Source = bitmap;
            }
            catch (Exception ex)
            {
                throw new IOException("Failed to load the image into the Image control.", ex);
            }
        }
    }
   
}
