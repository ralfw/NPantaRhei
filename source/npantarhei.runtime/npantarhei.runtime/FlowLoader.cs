using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime
{
    internal class FlowLoader
    {
        public const string ROOT_FLOW_NAME = "/";


        public static IEnumerable<IStream> LoadFromEmbeddedResource(string flowname, Assembly resourceAssembly, string resourcename)
        {
            using (var stream = resourceAssembly.GetManifestResourceStream(resourcename))
            {
                if (stream == null) throw new InvalidOperationException(string.Format("Flow resource {0} not found in assembly {1}", resourcename, resourceAssembly.FullName));

                using (var sr = new StreamReader(stream))
                {
                    return LoadFromReader(flowname, sr);
                }
            }
        }


        public static IEnumerable<IStream> LoadFromReader(string flowname, TextReader source)
        {
            return LoadFromLines(flowname, Enumerate_lines(source));
        }

        private static IEnumerable<string> Enumerate_lines(TextReader source)
        {
            var line = source.ReadLine();
            while (line != null)
            {
                yield return line;
                line = source.ReadLine();
            }
        } 


        public static IEnumerable<IStream> LoadFromLines(string flowname, IEnumerable<string> lines)
        {
            var streams = new List<IStream>();

            var listOfPortnames = lines.Select(Remove_comment)
                                       .Where(l => l != "")
                                       .Select(l => l.Split(','));

            foreach (var portnames in listOfPortnames)
            {
                if (Line_defines_flowname(portnames))
                    flowname = portnames[0];
                else
                {
                    var fromPort = Create_qualified_port(flowname, portnames[0].Trim());
                    var toPort = Create_qualified_port(flowname, portnames[1].Trim());
                    streams.Add(new npantarhei.runtime.messagetypes.Stream(fromPort, toPort));
                }
            }

            return streams;
        } 



        private static string Remove_comment(string line)
        {
            var index = line.IndexOf("//");
            if (index >= 0) line = line.Substring(0, index);
            return line.Trim();
        }

        private static bool Line_defines_flowname(string[] portnames)
        {
            return portnames.Length == 1;
        }

        private static IPort Create_qualified_port(string flowname, string portFullname)
        {
            var port = new Port(portFullname);
            if (Is_root_flowname(flowname)) return port;
            return port.IsRooted ? Port.Uproot(port)
                                    : Port.Build(flowname, port.OperationName, port.InstanceNumber, port.Name);
        }

        private static bool Is_root_flowname(string flowname)
        {
            return flowname == ROOT_FLOW_NAME;
        }
    }
}
