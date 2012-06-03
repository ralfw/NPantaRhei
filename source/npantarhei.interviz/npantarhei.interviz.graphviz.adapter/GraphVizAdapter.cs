using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;

namespace npantarhei.interviz.graphviz.adapter
{
    public class GraphVizAdapter
    {
        public static Tuple<Image, NodeMap> Compile_graph(string dotSource)
        {
            var image = Compile_graph_to_image(dotSource);
            var map = Compile_graph_to_node_map(dotSource, image.Width, image.Height);
            return new Tuple<Image, NodeMap>(image, map);
        }


        public static Image Compile_graph_to_image(string dotSource)
        {
            return Compile_graph_to<Image>(dotSource, "png", Image.FromStream);
        }


        public static NodeMap Compile_graph_to_node_map(string dotSource, int imageWidthPixels, int imageHeightPixels)
        {
            var nodeMapSource = Compile_graph_to<string>(dotSource, "plain", s => { using(var sr = new StreamReader(s)) { return sr.ReadToEnd(); } });
            return Compile_node_map(nodeMapSource, imageWidthPixels, imageHeightPixels);
        }

        /* Node map source format:
         * 0     1 2    3
         * graph 1 0.75 3.5
         *         Width and height of graph in inches; (0,0) is at left/bottom corner of graph
         *       
         * 0    1 2     3    4    5   6...
         * node a 0.375 3.25 0.75 0.5 "node label" solid ellipse black lightgrey
         *         Center X/Y of node, width/height of node in inches
         */
        private static NodeMap Compile_node_map(string nodeMapSource, int imageWidthPixels, int imageHeightPixels)
        {
            using(var r = new StringReader(nodeMapSource))
            {
                var enUS = new CultureInfo("en-US");

                var l = r.ReadLine();
                var graphParts = l.Split(' ');
                var graphWidthInches = double.Parse(graphParts[2], enUS);
                var graphHeightInches = double.Parse(graphParts[3], enUS);

                var widthFactor = imageWidthPixels/graphWidthInches;
                var heightFactor = imageHeightPixels/graphHeightInches;

                var nodeAreaList = new List<NodeMap.NodeArea>();

                while(true)
                {
                    l = r.ReadLine();
                    if (l == null) break;

                    if (l.StartsWith("node"))
                    {
                        var nodeParts = l.Split(' ');
                        var nodeCenterXInches = double.Parse(nodeParts[2], enUS);
                        var nodeCenterYInches = double.Parse(nodeParts[3], enUS);
                        var nodeWidthInches = double.Parse(nodeParts[4], enUS);
                        var nodeHeightInches = double.Parse(nodeParts[5], enUS);

                        var nodeRectWidth = (int)(nodeWidthInches*widthFactor);
                        var nodeRectHeight = (int)(nodeHeightInches*heightFactor);
                        var nodeRectX = (int)(nodeCenterXInches*widthFactor) - nodeRectWidth/2;
                        var nodeRectY = imageHeightPixels - ((int)(nodeCenterYInches*heightFactor) - nodeRectHeight/2) - nodeRectHeight;
                        var nodeRect = new Rectangle(nodeRectX, nodeRectY, nodeRectWidth, nodeRectHeight);

                        var nodeLabel = nodeParts[6];
                        if (l.IndexOf("\"") >= 0)
                        {
                            var iLabelStart = l.IndexOf("\"");
                            var iLabelEnd = l.IndexOf("\"", iLabelStart + 1);
                            nodeLabel = l.Substring(iLabelStart + 1, iLabelEnd - iLabelStart - 1);
                        }

                        nodeAreaList.Add(new NodeMap.NodeArea(nodeLabel, nodeRect));
                    }
                }

                return new NodeMap(nodeAreaList);
            }
            return null;
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
