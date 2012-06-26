using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using npantarhei.runtime;

namespace TelegramProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            var textfileadapter = new TextfileAdapter();
            var formatter = new Formatter();

            var config = new FlowRuntimeConfiguration()
                                .AddStreamsFrom("TelegramProblem.run.flow", Assembly.GetExecutingAssembly())

                                .AddAction<string>("read", textfileadapter.Read).MakeAsync()
                                .AddAction<string>("write", textfileadapter.Write, true)

                                .AddAction<string, string>("decompose", formatter.Decompose)
                                .AddAction<string, string>("concatenate", formatter.Concatenate)

                                .AddAction<Tuple<string, string>>("textfileadapter_config", textfileadapter.Config)
                                .AddAction<int>("formatter_config", formatter.Config);

            using(var fr = new FlowRuntime(config))
            {
                fr.UnhandledException += Console.WriteLine;
                
                fr.Process(".configFilenames", new Tuple<string,string>("source.txt", "target.txt"));
                fr.Process(".configLineWidth", 40);

                fr.Process(".run");

                fr.WaitForResult();
            }
        }
    }
}
