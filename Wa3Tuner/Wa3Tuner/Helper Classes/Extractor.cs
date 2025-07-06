using MdxLib.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Wa3Tuner.Helper_Classes
{
    internal static class Extractor
    {
        public static string?  GetString(object? obj)
        {
            if (obj == null) { throw new ArgumentNullException("Extractor.GetString: given parameter was null"); }
            else   if (obj is string)
            {
                return (string?)obj;
            }
            else if (obj is ListBoxItem l)
            {
                string? content = l.Content.ToString();
                return content;

            }
            else if (obj is CheckBox c)
            {
                string? conten = c.Content as string;
                return conten;

            }
            else if (obj is TextBlock X)
            {

                return X.Text;

            }
            return string.Empty;
        }
        public static string? GetString(ListBoxItem item)
        {
           
            
            return item.Content.ToString() ?? string.Empty;
        }
        public static int GetInt(TextBox box)
        {
            if (box == null) return 0;
           if (int.TryParse  (box.Text, out int value)) return value;
           return 0;
        }
        public static int GetInt(string? text)
        {
            
            if (int.TryParse(text, out int value)) return value;
            return 0;
        }
        public static int GetInt(ListBox list)
        {
            if (list.SelectedItem == null) return 0;
            ListBoxItem? item = list.SelectedItem as ListBoxItem;
            if (item == null) return 0;
            if (item.Content == null) return 0;

            string? conent = item.Content.ToString();
            return GetInt(conent);
        }

        internal static CheckBox? GetCheckBoxOfListItem(object item)
        {
            ListBoxItem? b = item as ListBoxItem; if (b == null) return null;
            StackPanel? p = b.Content as StackPanel; if (p == null) return null;
            CheckBox? c = p.Children[0] as CheckBox; if (c == null) return null;
            return c;
        }

        internal static float GetFloat(float def, string text, float min)
        {
            if (float.TryParse(text, out float value))
            {
                if (value >= min) return value;
                else return def;
            }
            else { return def; }
        }

        internal static CVector3 GetVertex(string rawString)
        {
            string trimmed = rawString.Trim();
            trimmed = trimmed.Replace(" ", "");
            trimmed = trimmed.Replace("{", "").Replace("}", "");

            string[] parts = trimmed.Split(',');

            if (parts.Length != 3)
                throw new FormatException("Vertex string must contain exactly 3 components.");

            float x = float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
            float y = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
            float z = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);

            return new CVector3(x, y, z);
        }


    }
}
