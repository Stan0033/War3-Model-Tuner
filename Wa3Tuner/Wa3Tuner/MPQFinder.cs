using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Collections.Generic;
using System;
using System.Linq;

namespace W3_Texture_Finder
{
    public static class MPQPaths
    {
        public static string War3;
        public static string War3X;
        public static string War3Patch;
        public static string War3xLocal;
        public static string local = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        public static string paths = Path.Combine(local, "Paths\\Paths.txt");
        public static string temp = Path.Combine(local, "Paths\\temp.txt");
    }
    public static class MPQFinder
    {
        public static void Find()
        {

            if (File.Exists(MPQPaths.paths))
            {
                List<string> list = File.ReadAllLines(MPQPaths.paths).ToList();
                if (list.Count != 4)
                {
                    FindLost();
                }
                else
                {
                    MPQPaths.War3 = list[0];
                    MPQPaths.War3X = list[1];
                    MPQPaths.War3Patch = list[2];
                    MPQPaths.War3xLocal = list[3];
                    FindLost();
                }

            }
            else
            {
                FindLost();

            }
        }
        private static void WriteAll()
        {
            
            File.WriteAllText(MPQPaths.paths, $"{MPQPaths.War3}\n{MPQPaths.War3X}\n{MPQPaths.War3Patch}\n{MPQPaths.War3xLocal}");
        }
        private static void FindLost()
        {
            if (Findwar3() == false) { Environment.Exit(0); }
            if (Findwar3x() == false) { Environment.Exit(0); }
            if (Findwar3Patch() == false) { Environment.Exit(0); }
           if (Findwar3xlocal() == false) { Environment.Exit(0); }

            WriteAll();


        }
        public static bool Findwar3()
        {
            if (File.Exists(MPQPaths.War3)) { return true; }
            MessageBox.Show("Find war3.MPQ", "Setup");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "war3.MPQ|war3.MPQ";
            bool? result = openFileDialog.ShowDialog();
            // Check if the dialog was canceled
            if (result == true)
            {
                MPQPaths.War3 = openFileDialog.FileName;
                return true;
            }
            else
            {
                return false;
            }
            return false;
        }
        public static bool Findwar3x()
        {
            if (File.Exists(MPQPaths.War3X)) { return true; }
            MessageBox.Show("Find war3x.MPQ", "Setup");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "war3x.MPQ|war3x.MPQ";
            bool? result = openFileDialog.ShowDialog();
            // Check if the dialog was canceled
            if (result == true)
            {
                MPQPaths.War3X = openFileDialog.FileName; return true;
            }
            else
            {
                return false;
            }
            return false;
        }
        public static bool Findwar3xlocal()
        {
            if (File.Exists(MPQPaths.War3xLocal)) { return true; }
            MessageBox.Show("Find war3xLocal.MPQ", "Setup");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "war3xLocal.MPQ|war3xLocal.MPQ";
            bool? result = openFileDialog.ShowDialog();
            // Check if the dialog was canceled
            if (result == true)
            {
                MPQPaths.War3xLocal = openFileDialog.FileName; return true;
            }
            else
            {
                return false;
            }
            return false;
        }
        public static bool Findwar3Patch()
        {
            if (File.Exists(MPQPaths.War3Patch)) { return true; }
            MessageBox.Show("Find war3Patch.MPQ", "Setup");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "war3Patch.MPQ|war3Patch.MPQ";
            bool? result = openFileDialog.ShowDialog();
            // Check if the dialog was canceled
            if (result == true)
            {
                MPQPaths.War3Patch = openFileDialog.FileName; return true;
            }
            else
            {
                return false;
            }
            return false;
        }
    }
}
