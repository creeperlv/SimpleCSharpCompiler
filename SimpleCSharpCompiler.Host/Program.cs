using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SimpleCSharpCompiler.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var fi = new FileInfo(Assembly.GetAssembly(typeof(Program)).Location);
            var runtimeDir = fi.Directory;
            var config=File.ReadAllText(Path.Combine(runtimeDir.FullName,"Runtime.Config"));
            ProcessStartInfo info = new ProcessStartInfo("dotnet");
            info.Arguments = Path.Combine(runtimeDir.FullName,config);
            if (args.Length > 0)
            {
                foreach (var item in args)
                {
                    info.Arguments += " \"" + args+"\"";
                }
            }
            Process.Start(info);
        }
    }
}
