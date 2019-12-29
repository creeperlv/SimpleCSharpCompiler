using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SimpleCSharpCompiler.Globalization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SimpleCSharpCompiler.Core
{
    public enum TargetType
    {
        DLL, EXE
    }
    public class CoreCompiler
    {
        public static readonly Version CoreVersion = new Version(1, 0, 0, 0);
        string TargetName = Guid.NewGuid().ToString() + ".dll";
        string TargetFile = "";
        string MainClass = "";
        bool AllowUnsafe = false;
        Platform Platform = Platform.AnyCpu;
        LanguageVersion LanguageVersion = LanguageVersion.Default;
        bool CopyLibs = true;
        OptimizationLevel optimizationLevel = OptimizationLevel.Debug;
        public CoreCompiler()
        {
            TargetFile = "./" + TargetName;
            var dir = (new FileInfo(typeof(object).Assembly.Location).Directory);
            foreach (var item in dir.EnumerateFiles("*.dll"))
            {
                PreDLLRefs.Add(item);
                if (item.Name == "System.Runtime.dll")
                {
                    DLLRefs.Add(item.FullName);
                }
                //else if (item.Name == "System.Private.CoreLib.dll")
                //{
                //    DLLRefs.Add(item.FullName);
                //}
            }
        }
        TargetType TargetType = TargetType.DLL;
        List<string> files = new List<string>();
        List<string> DLLRefs = new List<string>();
        List<FileInfo> PreDLLRefs = new List<FileInfo>();
        public CoreCompiler SetOptimizationLevel(int level)
        {
            switch (level)
            {
                case 0:
                    optimizationLevel = OptimizationLevel.Debug;
                    break;
                case 1:
                    optimizationLevel = OptimizationLevel.Release;
                    break;
                default:
                    break;
            }
            return this;
        }
        public CoreCompiler AddCodeFile(string s)
        {
            files.Add(s);
            return this;
        }
        public CoreCompiler SetTargetType(TargetType type)
        {
            TargetType = type;
            return this;
        }
        public CoreCompiler SetTargetName(string n)
        {
            TargetName = n;
            return this;
        }
        public CoreCompiler SetMainClass(string n)
        {
            MainClass = n;
            return this;
        }
        public CoreCompiler SetPlatform(string p)
        {
            switch (p.ToUpper())
            {
                case "X86":
                    Platform = Platform.X86;
                    break;
                case "X64":
                    Platform = Platform.X64;
                    break;
                case "ARM":
                    Platform = Platform.Arm;
                    break;
                case "ARM64":
                    Platform = Platform.Arm64;
                    break;
                case "ANYCPU":
                    Platform = Platform.AnyCpu;
                    break;
                default:
                    Console.WriteLine(Language.GetString("General", "Unknown.Platform", "Unknown Platform."));
                    break;
            }
            return this;
        }
        public CoreCompiler SetLanguageVersion(string v)
        {
            if (v.ToUpper() == "DEFAULT")
            {
                LanguageVersion = LanguageVersion.Default;
            }
            else if (v.ToUpper() == "LATEST")
            {
                LanguageVersion = LanguageVersion.Latest;
            }
            else
            {
                try
                {

                    double d = double.Parse(v);
                    switch (d)
                    {
                        case 1:
                            LanguageVersion = LanguageVersion.CSharp1;
                            break;
                        case 2:
                            LanguageVersion = LanguageVersion.CSharp2;
                            break;
                        case 3:
                            LanguageVersion = LanguageVersion.CSharp3;
                            break;
                        case 4:
                            LanguageVersion = LanguageVersion.CSharp4;
                            break;
                        case 5:
                            LanguageVersion = LanguageVersion.CSharp5;
                            break;
                        case 6:
                            LanguageVersion = LanguageVersion.CSharp5;
                            break;
                        case 7:
                            LanguageVersion = LanguageVersion.CSharp7;
                            break;
                        case 7.1:
                            LanguageVersion = LanguageVersion.CSharp7_1;
                            break;
                        case 7.2:
                            LanguageVersion = LanguageVersion.CSharp7_2;
                            break;
                        case 7.3:
                            LanguageVersion = LanguageVersion.CSharp7_3;
                            break;
                        case 8:
                            LanguageVersion = LanguageVersion.CSharp8;
                            break;
                        default:
                            Console.WriteLine(Language.GetString("General", "Unknown.LanguageVersion", "Unknown Language Version."));
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine(Language.GetString("General", "Unknown.LanguageVersion", "Unknown Language Version."));
                }
            }
            return this;
        }
        public CoreCompiler SetCopyLibs(bool b)
        {
            CopyLibs = b;
            return this;
        }
        public CoreCompiler SetAllowUnsafe(bool b)
        {
            AllowUnsafe = b;
            return this;
        }
        public CoreCompiler SetTargetFile(string n)
        {
            TargetFile = n;
            return this;
        }
        public CoreCompiler AddDLLReference(string s)
        {

            DLLRefs.Add(s);
            return this;
        }
        public bool Compile()
        {
            List<MetadataReference> _ref = new List<MetadataReference>();
            //SourceFileResolver sourceFileResolver=new SourceFileResolver()
            var c = CSharpCompilation.Create(TargetName).WithOptions(new CSharpCompilationOptions(
                  TargetType == TargetType.DLL ? OutputKind.DynamicallyLinkedLibrary : OutputKind.ConsoleApplication,
                  optimizationLevel: optimizationLevel,
                  allowUnsafe: AllowUnsafe,
                  platform: Platform,
                  mainTypeName: MainClass == "" ? null : MainClass,
                  warningLevel: 4,
                  xmlReferenceResolver: null
                  ));
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            List<string> Usings = new List<string>();
            Console.WriteLine(Language.GetString("General", "ResSrcFile", "Resolving Source Files..."));
            foreach (var item in files)
            {
                var s = CSharpSyntaxTree.ParseText(File.ReadAllText(item), new CSharpParseOptions(LanguageVersion, kind: SourceCodeKind.Regular));

                foreach (var Syntax in s.GetRoot().ChildNodes())
                {
                    if (Syntax.IsKind(SyntaxKind.UsingDirective))
                    {

                        String @using = Syntax.ChildNodes().First().ToString();
                        if (!Usings.Contains(@using)) Usings.Add(@using);
                    }

                }
                syntaxTrees.Add(s);
            }
            foreach (var item in Usings)
            {
                foreach (var dlls in PreDLLRefs)
                {
                    //if (dlls.Name.StartsWith(item))
                    //{
                    //    DLLRefs.Add(dlls.FullName);
                    //}
                    if (dlls.Name == (item + ".dll"))
                    {
                        _ref.Add(MetadataReference.CreateFromFile(dlls.FullName));
                        //DLLRefs.Add(dlls.FullName);
                    }
                }
            }
            {
                _ref.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
                _ref.Add(MetadataReference.CreateFromFile(typeof(Console).Assembly.Location));
                _ref.Add(MetadataReference.CreateFromFile(typeof(string).Assembly.Location));
                _ref.Add(MetadataReference.CreateFromFile(typeof(System.Collections.IEnumerable).Assembly.Location));
                _ref.Add(MetadataReference.CreateFromFile(typeof(File).Assembly.Location));
            }
            foreach (var item in DLLRefs)
            {
                _ref.Add(MetadataReference.CreateFromFile(item));
            }
            c = c.AddReferences(_ref)
                .AddSyntaxTrees(syntaxTrees);
            string realFile = TargetFile;
            if (realFile.ToLower().EndsWith(".exe"))
            {
                realFile = realFile.Remove(realFile.Length - 4) + ".dll";
            }
            else if (!realFile.ToLower().EndsWith(".dll"))
            {
                realFile = realFile + ".dll";
            }
            var result = c.Emit(realFile);
            foreach (var item in result.Diagnostics)
            {
                Console.WriteLine(item.ToString());
            }
            if (result.Success == true)
            {
                DirectoryInfo TargetDir = new FileInfo(TargetFile).Directory;
                if (CopyLibs == true)
                {
                    foreach (var item in DLLRefs)
                    {
                        var fi = new FileInfo(item);
                        fi.CopyTo(Path.Combine(TargetDir.FullName, fi.Name), true);
                    }
                }
                if (result.Success)
                {
                    if (TargetType == TargetType.EXE)
                    {
                        {
                            var fi = new FileInfo(Assembly.GetAssembly(this.GetType()).Location);
                            var di = fi.Directory;
                            var runtimeConfig = new FileInfo(Path.Combine(di.FullName, "scsc.runtimeconfig.json"));
                            runtimeConfig.CopyTo(Path.Combine(TargetDir.FullName, new FileInfo(TargetFile).Name.Remove(new FileInfo(TargetFile).Name.Length - 4) + ".runtimeconfig.json"), true);
                        }
                        {
                            var fi = new FileInfo(Assembly.GetAssembly(this.GetType()).Location);
                            var di = fi.Directory;
                            var runtimeConfig = new FileInfo(Path.Combine(di.FullName, "scsc.runtimeconfig.json"));
                            runtimeConfig.CopyTo(Path.Combine(TargetDir.FullName, "SimpleCSharpCompiler.Host.runtimeconfig.json"), true);
                        }
                        {
                            var fi = new FileInfo(Assembly.GetAssembly(this.GetType()).Location);
                            var di = fi.Directory;
                            var runtimeConfig = new FileInfo(Path.Combine(di.FullName, "SimpleCSharpCompiler.Host.dll"));
                            runtimeConfig.CopyTo(Path.Combine(TargetDir.FullName, "SimpleCSharpCompiler.Host.dll"), true);
                        }
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            var fi = new FileInfo(Assembly.GetAssembly(this.GetType()).Location);
                            var di = fi.Directory;
                            var runtimeConfig = new FileInfo(Path.Combine(di.FullName, "SimpleCSharpCompiler.Host.exe"));
                            runtimeConfig.CopyTo(TargetFile, true);
                        }
                        else
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            var fi = new FileInfo(Assembly.GetAssembly(this.GetType()).Location);
                            var di = fi.Directory;
                            var runtimeConfig = new FileInfo(Path.Combine(di.FullName, "SimpleCSharpCompiler.Host"));
                            runtimeConfig.CopyTo(TargetFile, true);
                        }
                        {
                            File.WriteAllText(Path.Combine(TargetDir.FullName, "Runtime.Config"), new FileInfo(realFile).Name);
                        }
                    }
                }
            }
            return result.Success;
        }
    }
}
