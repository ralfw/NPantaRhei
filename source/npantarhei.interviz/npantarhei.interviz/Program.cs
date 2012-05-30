using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using npantarhei.interviz.graphviz.adapter;
using npantarhei.runtime;
using npantarhei.runtime.messagetypes;

namespace npantarhei.interviz
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var fr = new FlowRuntime())
            {
                fr.Message += Console.WriteLine;
                fr.UnhandledException += ex => MessageBox.Show(ex.ToString());

                fr.AddStreamsFrom("npantarhei.interviz.root.flow", Assembly.GetExecutingAssembly());

                var win = new WinDesigner();

                fr.AddOperations(new FlowOperationContainer()
                                        .AddFunc<string, Image>("compile_dot_source_to_image", GraphVizAdapter.Compile_graph_to_image)
                                        .AddFunc<string[],string>("compile_flow_to_dot_source", FlowCompiler.Compile_to_dot_source)
                                        .AddFunc<string[], string>("extract_filename_from_commandline", _ => _[0])
                                        .AddAction<Tuple<string,string>>("display_flow", win.Display_flow).MakeSync()
                                        .AddAction<Image>("display_graph", win.Display_graph).MakeSync()
                                        .AddFunc<string,Tuple<string,string>>("load_flow_from_file", filename => new Tuple<string, string>(filename, File.ReadAllText(filename)))
                                        .AddAction<Tuple<string,string>>("save_flow", _ => File.WriteAllText(_.Item1, _.Item2))
                                        .AddFunc<Tuple<string[],int>,string[]>("select_current_flow", FlowCompiler.Select_flow_by_line)
                                        .AddFunc<object,object>("throttle_redrawings", _ => _)
                                        .Operations);

                win.Redraw += fr.CreateEventProcessor<Tuple<string[], int>>(".redraw");
                win.Load_flow += fr.CreateEventProcessor<string>(".load");
                win.Save_flow += fr.CreateEventProcessor<Tuple<string, string>>(".save");

                fr.Process(new npantarhei.runtime.messagetypes.Message(".run", args));

                Application.Run(win);
            }
        }
    }
}
