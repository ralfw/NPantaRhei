using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Text;

namespace npantarhei.interviz.graphviz.adapter
{
    public class GraphVizAdapter
    {
        public static Image Compile_graph_to_image(string dotSource)
        {
            var pDotInfo = new ProcessStartInfo("dot", "-Tpng")
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
            var img = Image.FromStream(pDot.StandardOutput.BaseStream);
            pDot.StandardOutput.Close();

            pDot.WaitForExit();

            return img;
        }
    }
}
