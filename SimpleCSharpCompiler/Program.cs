using SimpleCSharpCompiler.Core;
using SimpleCSharpCompiler.Globalization;
using System;
using System.IO;
using System.Resources;

namespace SimpleCSharpCompiler
{
    class Program
    {
        static readonly Version version = new Version(1, 0, 0, 0);
        static void Main(string[] args)
        {
            //Console.WriteLine("Simple C# Compiler, scsc");

            Language.Load();
            CoreCompiler coreCompiler = new CoreCompiler();
            int count = 0;
            bool ShowVersion = false;
            for (int i = 0; i < args.Length; i++)
            {
                string op = args[i];
                switch (op.ToUpper())
                {
                    case "-O":
                        {
                            string f = args[i + 1];
                            i++;
                            var file = new FileInfo(f);
                            if (!file.Directory.Exists) file.Directory.Create();
                            coreCompiler.SetTargetName(file.Name);
                            coreCompiler.SetTargetFile(file.FullName);
                        }
                        break;
                    case "-OPT":
                        {
                            string f = args[i + 1];
                            i++;
                            switch (f.ToUpper())
                            {
                                case "DEBUG":
                                    {
                                        coreCompiler.SetOptimizationLevel(0);
                                    }
                                    break;
                                case "RELEASE":
                                    {
                                        coreCompiler.SetOptimizationLevel(0);
                                    }
                                    break;
                                default:
                                    Console.WriteLine(Language.GetString("General","UnknownOptLevel", "Unknown Optimization Level."));
                                    break;
                            }
                        }
                        break;
                    case "-M":
                        {
                            string f = args[i + 1];
                            i++;
                            coreCompiler.SetMainClass(f);
                        }
                        break;
                    case "--LANGUAGE-VERSION":
                        {
                            string f = args[i + 1];
                            i++;
                            coreCompiler.SetLanguageVersion(f);
                        }
                        break;
                    case "-L-V":
                        {
                            string f = args[i + 1];
                            i++;
                            coreCompiler.SetLanguageVersion(f);
                        }
                        break;;
                    case "-P":
                        {
                            string f = args[i + 1];
                            i++;
                            coreCompiler.SetPlatform(f);
                        }
                        break;
                    case "-CL":
                        {
                            string f = args[i + 1];
                            i++;
                            if (f.ToUpper() == "FALSE")
                            {
                                coreCompiler.SetCopyLibs(false);
                            }else if (f.ToUpper() == "TRUE")
                            {

                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Unknown switch: -cl "+f);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                        }
                        break;
                    case "-R":
                        {
                            string f = args[i + 1];
                            i++;
                            var file = new FileInfo(f);
                            coreCompiler.AddDLLReference(file.FullName);
                        }
                        break;
                    case "--ALLOW-UNSAFE":
                        {
                            coreCompiler.SetAllowUnsafe(true);
                        }
                        break;
                    case "-UNSAFE":
                        {
                            coreCompiler.SetAllowUnsafe(true);
                        }
                        break;
                    case "-T":
                        {
                            string f = args[i + 1];
                            i++;
                            if (f.ToUpper() == "DLL")
                            {
                                coreCompiler.SetTargetType(TargetType.DLL);
                            }else if (f.ToUpper() == "EXE")
                            {
                                coreCompiler.SetTargetType(TargetType.EXE);
                            }
                        }
                        break;
                    case "-V":
                        {
                            ShowVersion = true;
                            Console.WriteLine("scsc:"+version.ToString());
                            Console.WriteLine("scsc Core:"+CoreCompiler.CoreVersion.ToString());
                        }
                        break;
                    default:
                        coreCompiler.AddCodeFile(op);
                        count++;
                        break;
                }
            }
            if (count == 0)
            {
                if (ShowVersion == true) return;
                Console.Write("scsc: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(Language.GetString("General", "FatalError", "fatal error: "));
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Language.GetString("General", "NoInputFiles", "no input files"));
                Console.WriteLine(Language.GetString("General", "Com.Ter", "compilation terminated."));
                return;
            }
            Console.WriteLine(coreCompiler.Compile());
        }
    }
}
