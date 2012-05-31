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
                                        .AddAction<Tuple<string,string>>("display_flow", win.Display_flow).MakeSync()
                                        .AddAction<Tuple<string[], int>>("display_flownames", win.Display_flownames).MakeSync()
                                        .AddAction<Image>("display_graph", win.Display_graph).MakeSync()
                                        .AddFunc<string[], string>("extract_filename_from_commandline", _ => _[0])
                                        .AddFunc<Tuple<string[], int>, Tuple<string[], Tuple<string[], int>>>("extract_flownames", FlowCompiler.Extract_flownames)
                                        .AddFunc<Tuple<string[],string>,int>("find_flow_headline", FlowCompiler.Find_flow_headline)
                                        .AddFunc<string,Tuple<string,string>>("load_flow_from_file", filename => new Tuple<string, string>(filename, File.ReadAllText(filename)))
                                        .AddAction<int>("move_cursor_to_flow_header", win.Move_cursor_to_flow_header).MakeSync()
                                        .AddAction<Tuple<string,string>>("save_flow", _ => File.WriteAllText(_.Item1, _.Item2))
                                        .AddFunc<Tuple<string[],int>,string[]>("select_current_flow", FlowCompiler.Select_flow_by_line)
                                        .AddFunc<Tuple<string[], Tuple<string[], int>>, Tuple<string[], int>>("select_flowname", FlowCompiler.Select_flowname)
                                        .AddFunc<object,object>("throttle_redrawings", _ => _)
                                        .Operations);

                win.Redraw += fr.CreateEventProcessor<Tuple<string[], int>>(".redraw");
                win.Load_flow += fr.CreateEventProcessor<string>(".load");
                win.Save_flow += fr.CreateEventProcessor<Tuple<string, string>>(".save");
                win.Jump_to_flow += fr.CreateEventProcessor<Tuple<string[], string>>(".jump_to_flow");

                fr.Process(new npantarhei.runtime.messagetypes.Message(".run", args));

                Application.Run(win);
            }
        }
    }
}
