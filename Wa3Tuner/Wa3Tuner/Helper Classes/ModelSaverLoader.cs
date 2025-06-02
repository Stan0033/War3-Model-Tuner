using MdxLib.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Wa3Tuner.Helper_Classes
{
    static class ModelSaverLoader
    {
       
        public static CModel? Load(string FromFileName)
        {
            CModel TemporaryModel = new CModel();
            string extension = System.IO.Path.GetExtension(FromFileName).ToLower();
            if (extension == ".mdx")
            {
                try
                {
                    using (var Stream = new System.IO.FileStream(FromFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        var ModelFormat = new MdxLib.ModelFormats.CMdx();
                        ModelFormat.Load(FromFileName, Stream, TemporaryModel);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Exception occured while trying to open the .mdx file");
                    // CurrentModel = new CModel();
                    return null;
                }
            }
            if (extension == ".mdl")
            {
                try
                {
                    using (var Stream = new System.IO.FileStream(FromFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        var ModelFormat = new MdxLib.ModelFormats.CMdl();
                        ModelFormat.Load(FromFileName, Stream, TemporaryModel);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Exception occured while trying to open the .mdx file");
                    //  CurrentModel = new CModel();
                    return null;
                }

            }
            return TemporaryModel;
        }

        internal static void Save(CModel model, string file)
        {
            if (file.Length == 0) { MessageBox.Show("Empty save path"); return; }
            if (
               (System.IO.Path.GetExtension(file) == ".mdx" ||
                System.IO.Path.GetExtension(file) == ".mdl") == false
                ) { MessageBox.Show("Invalid extension"); return; }
            if (model == null) { MessageBox.Show("Null model"); return; }

            if (System.IO.Path.GetExtension(file).ToLower() == ".mdl")
            {
                string ToFileName = file;
               
                using (var Stream = new System.IO.FileStream(ToFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    var ModelFormat = new MdxLib.ModelFormats.CMdl();
                    ModelFormat.Save(ToFileName, Stream, model);
                }
                FileCleaner.CleanFile(ToFileName);
               
            }
            if (System.IO.Path.GetExtension(file).ToLower() == ".mdx")
            {
                string ToFileName = file;
                using (var Stream = new System.IO.FileStream(ToFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    var ModelFormat = new MdxLib.ModelFormats.CMdx();
                    ModelFormat.Save(ToFileName, Stream, model);
                }
            }
        }
    }
}

