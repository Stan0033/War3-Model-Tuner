
using System;
using System.Globalization;
using System.IO;
using System.Linq;


namespace Wa3Tuner.Helper_Classes
{
    public static class StringHelper
    {
        
            public static string TitleCase(string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                    return string.Empty;

                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                return textInfo.ToTitleCase(text.ToLower());
            }

        internal static bool NameValidFile(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;

            // Check for invalid file name characters
            char[] invalidChars = Path.GetInvalidFileNameChars();
            if (name.IndexOfAny(invalidChars) >= 0) return false;

            // Optional: avoid reserved file names like "CON", "PRN", etc. (Windows-specific)
            string[] reservedNames = {
        "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    };
            string upperName = Path.GetFileNameWithoutExtension(name).ToUpperInvariant();
            if (reservedNames.Contains(upperName)) return false;

            return true;
        }
    }
}


