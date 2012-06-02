using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;

namespace npantarhei.interviz.graphviz.adapter
{
    public class NodeMap
    {
        
    }


    public class GraphVizAdapter
    {
        public static Tuple<Image, NodeMap> Compile_graph(string dotSource)
        {
            var image = Compile_graph_to_image(dotSource);
            var map = Compile_graph_to_node_map(new Tuple<string, Image>(dotSource, image));
            return new Tuple<Image, NodeMap>(image, map);
        }


        public static Image Compile_graph_to_image(string dotSource)
        {
            return Compile_graph_to<Image>(dotSource, "png", Image.FromStream);
        }


        public static NodeMap Compile_graph_to_node_map(Tuple<string,Image> flow)
        {
            var dotSource = flow.Item1;
            var graphImage = flow.Item2;

            var nodeMapSource = Compile_graph_to<string>(dotSource, "plain", s => { using(var sr = new StreamReader(s)) { return sr.ReadToEnd(); } });

            /*
             * 0     1 2    3
             * graph 1 0.75 3.5
             * 0    1 2     3    4    5   6...
             * node a 0.375 3.25 0.75 0.5 "node label" solid ellipse black lightgrey
             */
            return new NodeMap();
        }
 

        public static T Compile_graph_to<T>(string dotSource, string targetFormat, Func<Stream, T> convertStream)
        {
            var pDotInfo = new ProcessStartInfo("dot", string.Format("-T{0}", targetFormat))
                               {
                                   CreateNoWindow = true,
                                   WindowStyle = ProcessWindowStyle.Hidden,
                                   UseShellExecute = false,
                                   RedirectStandardInput = true,
                                   RedirectStandardOutput = true
                               };

            var pDot = new Process { StartInfo = pDotInfo };
            pDot.Start();

            // pass dot source to dot.exe
            pDot.StandardInput.Write(dotSource);
            pDot.StandardInput.Flush();
            pDot.StandardInput.Close();

            // read image generated
            var result = convertStream(pDot.StandardOutput.BaseStream);
            pDot.StandardOutput.Close();

            pDot.WaitForExit();

            return result;
        }
    }
}
