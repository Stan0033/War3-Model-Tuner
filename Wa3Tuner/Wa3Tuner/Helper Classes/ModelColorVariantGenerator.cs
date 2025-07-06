using MdxLib.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace Wa3Tuner.Helper_Classes
{
  public static   class ModelColorVariantGenerator
    {
        public static void Generate()
        {
           var list= FileSeeker.OpenMdlMdxFiles();
            if (list.Count > 0)
            {
                foreach (var file in list)
                {
                    CModel temp = LoadModel(file);
                    if (temp != null)
                    {
                        GenerateVariations(file, temp);
                    }
                }
               
            }
        }

        private static void GenerateVariations(string file, CModel temp)
        {
            var variations = new Dictionary<string, MdxLib.Primitives.CVector3>
    {
        { "_green",      new MdxLib.Primitives.CVector3(0, 1, 0) },
        { "_red",        new MdxLib.Primitives.CVector3(1, 0, 0) },
        { "_blue",       new MdxLib.Primitives.CVector3(0, 0, 1) },
        { "_yellow",     new MdxLib.Primitives.CVector3(1, 1, 0) },
        { "_purple",     new MdxLib.Primitives.CVector3(1, 0, 1) },
        { "_cyan",       new MdxLib.Primitives.CVector3(0, 1, 1) },
        { "_orange",     new MdxLib.Primitives.CVector3(1, 0.5f, 0) },   // reddish-orange
        { "_pink",       new MdxLib.Primitives.CVector3(1, 0.4f, 0.7f) }, // light pink
        { "_lightblue",  new MdxLib.Primitives.CVector3(0.6f, 0.8f, 1) }  // light blue (BGR)
    };

            foreach (var variation in variations)
            {
                foreach (var ga in temp.GeosetAnimations)
                {
                    if (ga.UseColor)
                        ga.Color.MakeStatic(new MdxLib.Primitives.CVector3( variation.Value));
                }
                foreach (var node in temp.Nodes)
                {
                    if (node is CParticleEmitter2 e)
                    {
                        e.Segment1.Color = new MdxLib.Primitives.CVector3(variation.Value);
                        e.Segment2.Color = new MdxLib.Primitives.CVector3(variation.Value);
                        e.Segment3.Color = new MdxLib.Primitives.CVector3(variation.Value);
                    }
                }

                SaveModel(temp, file, variation.Key);
            }
        }


        private static void SaveModel(CModel temp, string file, string suffix)
        {
            string extension = Path.GetExtension(file);
            string name  = Path.GetFileNameWithoutExtension(file); ;
           string dir = Path.GetDirectoryName(file);
            string newName = name + suffix + extension;
            string FullPath = Path.Combine(dir, newName);
          

            if (extension.ToLower() == ".mdl")
            {
                string ToFileName = FullPath;
               
                using (var Stream = new System.IO.FileStream(ToFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    var ModelFormat = new MdxLib.ModelFormats.CMdl();
                    ModelFormat.Save(ToFileName, Stream, temp);
                }
                FileCleaner.CleanFile(ToFileName);
                
            }
            else if (extension.ToLower() == ".mdx")
            {
                string ToFileName = FullPath;
                using (var Stream = new System.IO.FileStream(ToFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    var ModelFormat = new MdxLib.ModelFormats.CMdx();
                    ModelFormat.Save(ToFileName, Stream, temp);
                }
            }

 
        }

        private static CModel? LoadModel(string FromFileName)
        {
            CModel? TemporaryModel = new CModel();
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
            else if (extension == ".mdl")
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
    }
}
