using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wa3Tuner.Helper_Classes
{
    internal static class PathMaker
    {
        public static string? Make(params string[]? parts)
        {
            if (parts==null) return null;
            return Path.Combine(parts.Where(p => !string.IsNullOrEmpty(p)).ToArray());
        }
    }


}
