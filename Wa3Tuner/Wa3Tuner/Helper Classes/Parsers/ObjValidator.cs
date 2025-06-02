using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
namespace obj2mdl_batch_converter
{
    public static class ObjValidator
    {
        private static readonly Regex vertexRegex = new Regex(@"^v\s([+-]?(\d*\.\d+|\d+\.?\d*)([eE][+-]?\d+)?)\s([+-]?(\d*\.\d+|\d+\.?\d*)([eE][+-]?\d+)?)\s([+-]?(\d*\.\d+|\d+\.?\d*)([eE][+-]?\d+)?)$");
        private static readonly Regex vertexNormalRegex = new Regex(@"^vn\s([+-]?(\d*\.\d+|\d+\.?\d*)([eE][+-]?\d+)?)\s([+-]?(\d*\.\d+|\d+\.?\d*)([eE][+-]?\d+)?)\s([+-]?(\d*\.\d+|\d+\.?\d*)([eE][+-]?\d+)?)$");
        private static readonly Regex vertexTextureRegex = new Regex(@"^vt\s-?\d+(\.\d+)?\s-?\d+(\.\d+)?(\s-?\d+(\.\d+)?)?$");
        private static readonly Regex faceRegex = new Regex(@"^f\s+(\d+)(?:\/(\d*))?(?:\/(\d*))?(?:\s+(\d+)(?:\/(\d*))?(?:\/(\d*))?)*$");
        private static readonly Regex commentRegex = new Regex(@"^#.*$");
        private static readonly Regex objectRegex = new Regex(@"^o\s+.+$");
        private static readonly Regex lineRegex = new Regex(@"^l\s+\d+(\s+\d+)+$");

        private static readonly Regex groupRegex = new Regex(@"^g\s+\w+$");
        private static readonly Regex useMaterialRegex = new Regex(@"^usemtl\s+\w+$");
        private static readonly Regex smoothShadingRegex = new Regex(@"^s\s+\w+$");
        private static readonly Regex materialLibRegex = new Regex(@"^mtllib\b");

        public static bool Validate(string filePath)
        {
            if (!File.Exists(filePath)) { return false; }
            using (StreamReader reader = new StreamReader(filePath))
            {
                int lineNumber = 0;
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    line = line.Trim();
                    if (line.Length == 0) { continue; }
                    if (string.IsNullOrEmpty(line) || commentRegex.IsMatch(line)) continue; // Skip empty lines and comments
                    else if (vertexRegex.IsMatch(line) || vertexTextureRegex.IsMatch(line) ||
                             vertexNormalRegex.IsMatch(line) || faceRegex.IsMatch(line) ||
                             objectRegex.IsMatch(line) || groupRegex.IsMatch(line) ||
                             useMaterialRegex.IsMatch(line) || 
                             smoothShadingRegex.IsMatch(line) ||
                             lineRegex.IsMatch(line) ||
                             materialLibRegex.IsMatch(line))
                    {
                        continue; // Valid line
                    }
                    else
                    {
                        MessageBox.Show($"Invalid line detected at line {lineNumber}: {line}", "Invalid OBJ File");
                        return false;
                    }
                }
            }
            return true; // All lines are valid
        }
        private static void Downsize(List<int> list) { int lowest = list.Min(); for (int i = 0; i < list.Count; i++) list[i] -= lowest; }
    }

}
