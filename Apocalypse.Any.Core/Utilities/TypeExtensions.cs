using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Apocalypse.Any.Core.Utilities
{
    public static class TypeExtensions
    {
        public static Type[] LoadType(this string typeName)
        {
            return LoadType(typeName, true);
        }

        public static Type[] LoadType(this string typeName, bool referenced)
        {
            return LoadType(typeName, referenced, true);
        }

        public static Type GetApocalypseTypes(this string apocType)
        {
            string baseNameSpace = "Apocalypse.Any.*.dll";
            foreach (var file in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), baseNameSpace, SearchOption.TopDirectoryOnly))
            {
                try
                {
                    Console.WriteLine(file);
                    var leAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                    var leSerializerType = leAssembly.GetTypes().Where(t => t.FullName == apocType);
                    var leType = leSerializerType.FirstOrDefault();
                    if (leType != null)
                        return leType;
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex);
                }
            }
            return null;
        }

        /// <summary>
        /// Loads a type by a string type representation (With namespace)
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="referenced"></param>
        /// <param name="gac"></param>
        /// <returns></returns>
        public static Type[] LoadType(this string typeName, bool referenced, bool gac)
        {

            //check for problematic work
            if (string.IsNullOrEmpty(typeName) || !referenced && !gac)
                return new Type[] { typeName.GetApocalypseTypes() };

            Assembly currentAssembly = Assembly.GetCallingAssembly();
            
            List<string> assemblyFullnames = new List<string>();
            List<Type> types = new List<Type>();

            if (referenced)
            {            //Check refrenced assemblies
                foreach (AssemblyName assemblyName in currentAssembly.GetReferencedAssemblies().Where(a => !a.Name.StartsWith("System.")))
                {
                    
                    //Load method resolve refrenced loaded assembly
                    Assembly assembly = Assembly.Load(assemblyName.FullName);

                    var type = assembly.GetType(typeName, false, true);
                    
                    if (type != null && !assemblyFullnames.Contains(assembly.FullName))
                    {
                        Console.WriteLine("TADA!");
                        types.Add(type);
                        assemblyFullnames.Add(assembly.FullName);
                    }
                }
            }

            if (gac)
            {
                //GAC files
                string gacPath = Environment.GetFolderPath(System.Environment.SpecialFolder.Windows) + "\\assembly";
                var files = GetGlobalAssemblyCacheFiles(gacPath);
                foreach (string file in files)
                {
                    try
                    {
                        //reflection only
                        Assembly assembly = Assembly.ReflectionOnlyLoadFrom(file);

                        //Check if type is exists in assembly
                        var type = assembly.GetType(typeName, false, true);

                        if (type != null && !assemblyFullnames.Contains(assembly.FullName))
                        {
                            types.Add(type);
                            assemblyFullnames.Add(assembly.FullName);
                        }
                    }
                    catch
                    {
                        //your custom handling
                    }
                }
            }

            return types.ToArray();
        }

        public static string[] GetGlobalAssemblyCacheFiles(this string path)
        {
            List<string> files = new List<string>();

            DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo fi in di.GetFiles("*.dll"))
            {
                files.Add(fi.FullName);
            }

            foreach (DirectoryInfo diChild in di.GetDirectories())
            {
                var files2 = GetGlobalAssemblyCacheFiles(diChild.FullName);
                files.AddRange(files2);
            }

            return files.ToArray();
        }
    }
}