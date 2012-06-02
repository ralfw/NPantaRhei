using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace npantarhei.interviz
{
    class AssemblyResourceAdapter
    {
        public static Tuple<Assembly, string[]> Find_flow_resources(string assemblyFilename)
        {
            var assm = Assembly.LoadFile(assemblyFilename);

            var flowResourceNames = assm.GetManifestResourceNames()
                                        .Where(name => name.ToLower().EndsWith(".flow"))
                                        .ToArray();

            return new Tuple<Assembly, string[]>(assm, flowResourceNames);
        } 


        public static Tuple<string, string>[] Load_soures_from_resources(Tuple<Assembly, string[]> resourceDescriptions)
        {
            var sources = new List<Tuple<string,string>>();

            foreach (var resourceName in resourceDescriptions.Item2)
                using (var stream = resourceDescriptions.Item1.GetManifestResourceStream(resourceName))
                    using (var sr = new StreamReader(stream))
                    {
                        sources.Add(new Tuple<string, string>(sr.ReadToEnd(), resourceName));
                    }

            return sources.ToArray();
        }


        public static Tuple<string,string> Combine_sources(Tuple<string,string>[] sourcesFromResources)
        {
            var combinedSources = new StringWriter();

            foreach(var s in sourcesFromResources)
            {
                combinedSources.WriteLine("// {0}", s.Item2);
                combinedSources.WriteLine(s.Item1);
                combinedSources.WriteLine();
            }

            return new Tuple<string, string>("", combinedSources.ToString());
        }
    }
}
