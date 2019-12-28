using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace SimpleCSharpCompiler.Globalization
{
    public class Language
    {
        static Dictionary<string, Dictionary<string, string>> lang = new Dictionary<string, Dictionary<string, string>>();
        public static void Load()
        {
            var di = new FileInfo(Assembly.GetAssembly(typeof(Language)).Location).Directory;
            var name = CultureInfo.CurrentCulture.Name;
            DirectoryInfo realD;
            if (Directory.Exists(Path.Combine(di.FullName, "Locales", name)))
            {
                realD = new DirectoryInfo(Path.Combine(di.FullName,"Locales", name));
            }
            else
            {
                //Fallback
                realD = new DirectoryInfo(Path.Combine(di.FullName, "Locales", "en-US"));
            }
            {
                Load("General", realD.FullName);
            }
        }
        public static string GetString(string Domain,string Key,string Fallback = "")
        {
            if (lang.ContainsKey(Domain))
            {
                if (lang[Domain].ContainsKey(Key))
                {
                    return lang[Domain][Key];
                }
            }
            
            return Fallback;
        }
        static void Load(string Domain, string baseFolder)
        {
            var fi = new FileInfo(Path.Combine(baseFolder, Domain + ".lang"));
            if (!lang.ContainsKey(fi.Name))
            {
                lang.Add(Domain, new Dictionary<string, string>());
            }
            var list = File.ReadAllLines(fi.FullName);
            foreach (var item in list)
            {
                if (item.StartsWith("#"))
                {

                }
                else
                {
                    if (item.IndexOf("=") > 0)
                    {
                        lang[Domain].Add(item.Substring(0, item.IndexOf("=")), item.Substring(item.IndexOf("=") + 1));
                    }
                }
            }
        }
    }
}
