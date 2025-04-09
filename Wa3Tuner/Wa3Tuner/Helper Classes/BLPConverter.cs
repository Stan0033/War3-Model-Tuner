using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Shell;
using W3_Texture_Finder;

namespace Wa3Tuner
{
    internal class BLPConverter
    {
        internal static void Conver(string inputPath, string outputPath)
        {
            
            string ConverterExe = System.IO.Path.Combine(AppHelper.Local, "Tools\\blplabcl.exe");
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.FileName = ConverterExe;
            string opt1 = "-opt1";
            string opt2 = string.Empty;
            
            startInfo.Arguments = $"\"{inputPath}\" \"{outputPath}\" -type{0} -q{100} -mm{1} {opt1} {opt2}";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            process.Kill();
        }
    }
}