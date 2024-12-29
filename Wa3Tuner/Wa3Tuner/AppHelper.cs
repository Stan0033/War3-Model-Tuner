using System;
using System.IO;

namespace W3_Texture_Finder
{
    internal static class AppHelper
    {
        static string dir = AppDomain.CurrentDomain.BaseDirectory;
        internal static string TemporaryBLPLocation = Path.Combine(dir, "Temp.blp");
    }
}