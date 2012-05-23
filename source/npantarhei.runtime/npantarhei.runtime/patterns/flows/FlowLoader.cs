using System;
using System.Collections.Generic;
using System.IO;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.patterns.flows
{
    internal class FlowLoader
    {
        public static IEnumerable<IStream> LoadFromEmbeddedResource(string flowname, Type typeInResourceAssembly, string resourcename)
        {
            using (var stream = typeInResourceAssembly.Assembly.GetManifestResourceStream(resourcename))
            {
                if (stream == null) throw new InvalidOperationException(string.Format("Flow resource {0} not found in assembly {1}", resourcename, typeInResourceAssembly.Assembly.FullName));

                using (var sr = new StreamReader(stream))
                {
                    return LoadFromReader(flowname, sr);
                }
            }
        }

        public static IEnumerable<IStream> LoadFromFile(string flowname, string filename)
        {
            using(var sr = new StreamReader(filename))
            {
                return LoadFromReader(flowname, sr);
            }
        }


        public static IEnumerable<IStream> LoadFromReader(string flowname, TextReader source)
        {
            var streams = new List<IStream>();

            var line = source.ReadLine();
            while(line != null)
            {
                line = line.Trim();
                if (Line_is_not_whitespace_or_comment(line))
                {
                    var portnames = line.Split(',');

                    if (Line_defines_flowname(portnames))
                        flowname = portnames[0];
                    else
                    {
                        var fromPort = Create_qualified_port(flowname, portnames[0].Trim());
                        var toPort = Create_qualified_port(flowname, portnames[1].Trim());
                        streams.Add(new npantarhei.runtime.messagetypes.Stream(fromPort, toPort));
                    }
                }
                line = source.ReadLine();
            }

            return streams;
        }

        private static bool Line_is_not_whitespace_or_comment(string line)
        {
            return line != "" && !line.StartsWith("//");
        }

        private static bool Line_defines_flowname(string[] portnames)
        {
            return portnames.Length == 1;
        }

        private static Port Create_qualified_port(string flowname, string portFullname)
        {
            var port = new Port(portFullname);
            if (Is_root_flowname(flowname)) return port;
            return port.IsQualified ? new Port(port.Fullname.Substring(1)) 
                                    : new Port(string.Format("{0}/{1}", flowname, portFullname));
        }

        private static bool Is_root_flowname(string flowname)
        {
            return flowname == "/";
        }
    }
}
