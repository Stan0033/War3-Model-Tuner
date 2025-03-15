using MdxLib.Model;
using System;
using System.Collections.Generic;
using System.IO;
using W3_Texture_Finder;
using Wa3Tuner.Helper_Classes;
using War3Net.IO.Mpq;

namespace Wa3Tuner
{
    internal class BatchConverter
    {
        internal static void Convert(List<string> files)
        {
            foreach (string file in files)
            {
                CModel temp = ModelSaverLoader.Load(file);
                string extension = Path.GetExtension(file).ToLower(); 
                if (extension == ".mdl")
                {
                    string ToFileName = Path.ChangeExtension(file, ".mdx");
                    using (var Stream = new System.IO.FileStream(ToFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                    {
                        var ModelFormat = new MdxLib.ModelFormats.CMdx();
                        ModelFormat.Save(ToFileName, Stream, temp);
                    }

                }
                else if (extension == ".mdx")
                {
                    string ToFileName = Path.ChangeExtension(file, ".mdl");
                  
                    using (var Stream = new System.IO.FileStream(ToFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                    {
                        var ModelFormat = new MdxLib.ModelFormats.CMdl();
                        ModelFormat.Save(ToFileName, Stream, temp);
                    }
                    FileCleaner.CleanFile(ToFileName);
                    
                }


            }
            
        }

        internal static void ConvertBLPs(List<string> files)
        {
            if (files.Count > 0)
            {
                foreach (string file in files)
                {
                    var image = MPQHelper.GetImageSource(file);
                    string nPath = Path.ChangeExtension(file, ".png");

                    MPQHelper.ExportPNG(image, nPath);
                }
            }
        }

        internal static void ConvertMDL(string file)
        {

        }
        internal static void ConvertMDX(string file)
        {

        }
    }
}