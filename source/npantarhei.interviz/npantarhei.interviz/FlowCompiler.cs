using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace npantarhei.interviz
{
    class FlowCompiler
    {
        public static string[] Select_flow_by_line(Tuple<string[],int> source)
        {
            var index_of_first_line = Find_start_of_flow(source.Item1, source.Item2);
            var index_of_last_line = Find_end_of_flow(source.Item1, source.Item2);
            return Extract_flow(source, index_of_first_line, index_of_last_line);
        }


        private static int Find_start_of_flow(string[] source, int line_index)
        {
            while(line_index >= 0)
            {
                var line = Normalize_line(source[line_index]);
                if (line != "" && line.IndexOf(",") < 0) break;
                line_index--;
            }
            return line_index;
        }

        private static int Find_end_of_flow(string[] source, int initial_line_index)
        {
            var line_index = initial_line_index;
            while(line_index < source.Length)
            {
                var line = Normalize_line(source[line_index]);
                if (line != "" && line.IndexOf(",") < 0 && line_index > initial_line_index) break;
                line_index++;
            }
            return line_index - 1;
        }



        private static string[] Extract_flow(Tuple<string[], int> source, int index_of_first_line, int index_of_last_line)
        {
            return source.Item1
                         .Where((l, i) => index_of_first_line <= i && 
                                              i <= index_of_last_line)
                         .ToArray();
        }




        public static string Compile_to_dot_source(string[] source)
        {
            var portnames = new Dictionary<string, string>();

            var dotSource = new StringWriter();
            dotSource.WriteLine("digraph G {");

            source.Select(Normalize_line)
                  .Where(l => l != "")
                  .Select(l => l.Split(','))
                  .Where(p => p.Length == 2)
                  .ToList()
                  .ForEach(p =>
                               {
                                   var fromPortname = p[0].Trim();
                                   var toPortname = p[1].Trim();
                                   var fromPortId = Map_portname_to_id(portnames, fromPortname);
                                   var toPortId = Map_portname_to_id(portnames, toPortname);

                                   var description = "";
                                   if (fromPortname.IndexOf(".") > 0) description = fromPortname.Substring(fromPortname.IndexOf("."));
                                   if (toPortname.IndexOf(".") > 0)
                                       description = description + " / " + toPortname.Substring(toPortname.IndexOf("."));
                                   else if (description != "") description += " /";
                                   
                                   dotSource.WriteLine("{0}->{1} [label=\"   {2}\", fontsize=8];", fromPortId, toPortId, description);
                                });

            foreach (var portname in portnames)
            {
                if (portname.Key.StartsWith("."))
                    dotSource.WriteLine("{0} [shape=circle, fontsize=8, label=\"{1}\"]", portname.Value, portname.Key);
                else
                    dotSource.WriteLine("{0} [shape=box, fontsize=10, label=\"{1}\"]", portname.Value, portname.Key);
            }

            dotSource.WriteLine("}");

            Console.WriteLine(dotSource.ToString());

            return dotSource.ToString();
        }


        private static string Map_portname_to_id(Dictionary<string, string> portnames, string portname)
        {
            string portid;

            if (portname.IndexOf(".") > 0) portname = portname.Substring(0, portname.IndexOf("."));

            if (!portnames.TryGetValue(portname, out portid))
            {
                portid = "id" + portnames.Count();
                portnames.Add(portname, portid);
            }
            return portid;
        }


        private static string Normalize_line(string line)
        {
            line = line.IndexOf("//") < 0 ? line : line.Substring(0, line.IndexOf("//"));
            return line.Trim();
        }
    }
}
