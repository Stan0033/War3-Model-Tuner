using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using System.Windows;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;


namespace Wa3Tuner.Helper_Classes
{
    public static class FileSeeker
    {
        public static string? OpenPathFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Path Files (*.path)|*.path",
                Title = "Open Path File",
                DefaultExt = "path"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    return openFileDialog.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return null;
        }

        // Save a .path file and return the filename if successful or an empty string if cancelled.
        public static string SavePathFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Path Files (*.path)|*.path",
                Title = "Save Path File",
                DefaultExt = "path"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Here, you would save content to the file. This can be expanded to take specific content.
                    string filename = saveFileDialog.FileName;
                    // Example: save dummy content to the file.
                    File.WriteAllText(filename, "Dummy content"); // Modify this to your actual content logic.
                    return filename;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return string.Empty;
        }
        public static string? OpenModelFileDialog()
        {
            // Create an OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open Model File",
                Filter = "Model Files (*.mdl;*.mdx)|*.mdl;*.mdx",
                DefaultExt = ".mdl",
                CheckFileExists = true,
                CheckPathExists = true
            };
            // Show the dialog and get the result
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                // Return the selected file path
                return openFileDialog.FileName;
            }
            // Return null if no file was selected
            return null;
        }
        public static string openSTL()
        {
            // Create an OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open Model File",
                Filter = "STL Files (*.stl)|*.stl",
                DefaultExt = ".stl",
                CheckFileExists = true,
                CheckPathExists = true
            };
            // Show the dialog and get the result
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                // Return the selected file path
                return openFileDialog.FileName;
            }
            // Return null if no file was selected
            return "";
        }
        public static string ShowSaveFileDialog()
        {
            // Create a SaveFileDialog instance
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save File",
                Filter = "Model Files (*.mdl;*.mdx)|*.mdl;*.mdx", // Filter for .mdl and .mdx
                DefaultExt = ".mdl", // Default extension
                AddExtension = true  // Automatically add the extension
            };
            // Show the dialog and check the result
            bool? result = saveFileDialog.ShowDialog();
            return result == true ? saveFileDialog.FileName : string.Empty;
        }
        public static string OpenTGeoFileDialog()
        {
            // Create an OpenFileDialog instance
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                // Filter for ".tgeo" files
                Filter = "TGeo Files (*.tgeo)|*.tgeo",
                // Set initial directory (optional)
                InitialDirectory = @"C:\"
            };

            // Show the dialog and check if the user selected a file
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                // Return the selected file path
                return openFileDialog.FileName;
            }
            else
            {
                // Return an empty string if no file was selected
                return string.Empty;
            }
        }
        public static string OpenTGeomFileDialog()
        {
            // Create an OpenFileDialog instance
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                // Filter for ".tgeo" files
                Filter = "TGeom Files (*.tgeom)|*.tgeom",
                // Set initial directory (optional)
                InitialDirectory = @"C:\"
            };

            // Show the dialog and check if the user selected a file
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                // Return the selected file path
                return openFileDialog.FileName;
            }
            else
            {
                // Return an empty string if no file was selected
                return string.Empty;
            }
        }
        public  static string SaveTGeoFileDialog()
        {
            // Get the user's Documents folder as a default location
            string initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Create a SaveFileDialog instance for saving files
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                // Filter for ".tgeo" files
                Filter = "TGeo Files (*.tgeo)|*.tgeo",
                // Set the default file name (optional)
                FileName = "newfile.tgeo",
                // Set initial directory to the user's Documents folder
                InitialDirectory = initialDirectory
            };

            // Show the dialog and check if the user selected a file
            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                // Return the selected file path
                return saveFileDialog.FileName;
            }
            else
            {
                // Return an empty string if no file was selected
                return string.Empty;
            }
        }
        public static string SaveTGeomFileDialog()
        {
            // Get the user's Documents folder as a default location
            string initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Create a SaveFileDialog instance for saving files
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                // Filter for ".tgeo" files
                Filter = "TGeom Files (*.tgeom)|*.tgeom",
                // Set the default file name (optional)
                FileName = "newfile.tgeom",
                // Set initial directory to the user's Documents folder
                InitialDirectory = initialDirectory
            };

            // Show the dialog and check if the user selected a file
            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                // Return the selected file path
                return saveFileDialog.FileName;
            }
            else
            {
                // Return an empty string if no file was selected
                return string.Empty;
            }
        }
        public static List<string> OpenMdlMdxFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Warcraft 3 Model Files (*.mdl;*.mdx)|*.mdl;*.mdx",
                Multiselect = true,
                Title = "Select Warcraft 3 Model Files"
            };

            bool? result = openFileDialog.ShowDialog();

            return result == true ? new(openFileDialog.FileNames) : new();
        }
        public static   List<string> openBLPs()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Warcraft 3 BLP Images (*.blp)|*.blp",
                Multiselect = true,
                Title = "Select Warcraft 3 Image Files"
            };

            bool? result = openFileDialog.ShowDialog();

            return result == true ? new(openFileDialog.FileNames) : new();
        }
        public static string GetSavePathOBJ()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "OBJ Files|*.obj";
            saveFileDialog.DefaultExt = "obj";
            saveFileDialog.AddExtension = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }

            return "";
        }

        internal static string SaveSTL()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "STL Files|*.stl";
            saveFileDialog.DefaultExt = "stl";
            saveFileDialog.AddExtension = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }

            return "";
        }

        internal static string open3DS()
        {
            // Create an OpenFileDialog instance
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                // Filter for ".tgeo" files
                Filter = "3DS Files (*.3ds)|*.3ds",
                // Set initial directory (optional)
               // InitialDirectory = @"C:\"
            };

            // Show the dialog and check if the user selected a file
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                // Return the selected file path
                return openFileDialog.FileName;
            }
            else
            {
                // Return an empty string if no file was selected
                return string.Empty;
            }
        }

        internal static string SavePly()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "PLY Files|*.ply";
            saveFileDialog.DefaultExt = "ply";
            saveFileDialog.AddExtension = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }

            return "";
        }
        internal static string openPLY()
        {
            // Create an OpenFileDialog instance
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                // Filter for ".tgeo" files
                Filter = "PLY Files (*.ply)|*.ply",
                // Set initial directory (optional)
                // InitialDirectory = @"C:\"
            };

            // Show the dialog and check if the user selected a file
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                // Return the selected file path
                return openFileDialog.FileName;
            }
            else
            {
                // Return an empty string if no file was selected
                return string.Empty;
            }
        }

        internal static List<string> OpenImages()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Images",
                Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png)|*.bmp;*.jpg;*.jpeg;*.png",
                Multiselect = true
            };

            bool? result = openFileDialog.ShowDialog();

            return result == true ? new(openFileDialog.FileNames) : new();
        }
        public static string? SaveModelFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Model Files (*.mdl;*.mdx)|*.mdl;*.mdx|JSON Files (*.json)|*.json",
                Title = "Save File",
                OverwritePrompt = true,
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }

            return null;
        }
        public static string? OpenModelFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Model Files (*.mdl;*.mdx)|*.mdl;*.mdx|JSON Files (*.json)|*.json",
                Title = "Open File",
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return null;
        }

     
internal static string BrowseForFolder()
    {
        using var dlg = new FolderBrowserDialog
        {
            Description = "Select a folder",
            UseDescriptionForTitle = true
        };

        return dlg.ShowDialog() == DialogResult.OK ? dlg.SelectedPath :  "";
    }

}
}
