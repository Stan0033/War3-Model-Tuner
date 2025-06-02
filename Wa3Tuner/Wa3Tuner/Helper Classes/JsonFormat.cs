using MdxLib.Model;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text.Json;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace Wa3Tuner.Helper_Classes
{
   public static class JsonFormat
    { 
        public static void Save(CModel model, string p = "")
        {
         // NOT TESTED
            if (p.Length ==0)
            {
               
                string json = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(p, json);
            }
            else
            {
                string path = Save_Json();
                if (path.Length == 0) return;
                string json = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
            }
              
        }
        public static CModel? Load(string s = "")
        {
            if (s.Length == 0)
            {
                string path = Load_Json();
                if (path.Length == 0) return null;
                return JsonSerializer.Deserialize<CModel>(path);
            }
            else
            {
                 return JsonSerializer.Deserialize<CModel>(s);
            }

        } 
        public static string Save_Json()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                DefaultExt = "json",
                Title = "Save JSON File"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, "{}"); // Empty JSON
                return saveFileDialog.FileName;
            }

            return ""; // Cancelled
        }

        public static string Load_Json()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                Title = "Open JSON File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return File.ReadAllText(openFileDialog.FileName);
            }

            return ""; // Cancelled
        }
    }
}
