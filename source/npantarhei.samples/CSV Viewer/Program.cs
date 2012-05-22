using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSV_Viewer.buffer;
using CSV_Viewer.data_model;
using CSV_Viewer.flows;
using CSV_Viewer.flows.features;
using CSV_Viewer.formatter;
using CSV_Viewer.pager;
using CSV_Viewer.portals;
using CSV_Viewer.resources;
using npantarhei.runtime;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace CSV_Viewer
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var fr = new FlowRuntime())
            {
                var pageBufferContainer = new DataContainer<PageBuffer>();

                var frontend = new Frontend();

                var main = new Main(new StartProgram(
                                        new CommandlineParser(pageBufferContainer),
                                        new TextFileAdapter(),
                                        new LineBuffer(pageBufferContainer)),
                                    new GetFirstPage(
                                        new Pager(pageBufferContainer)),
                                    new GetLastPage(
                                        new Pager(pageBufferContainer)),
                                    new TurnPage(
                                        new Pager(pageBufferContainer)), 
                                    new Formatter(),
                                    frontend);

                fr.AddOperation(main);

                fr.AddStream(".run", "main.run");
                fr.AddStream("main.exit", ".exit");
                fr.AddStream(".displayFirstPage", "main.displayFirstPage");
                fr.AddStream(".displayLastPage", "main.displayLastPage");
                fr.AddStream(".displayNextPage", "main.displayNextPage");
                fr.AddStream(".displayPrevPage", "main.displayPrevPage");

                frontend.displayFirstPage += fr.CreateEventProcessor(".displayFirstPage");
                frontend.displayLastPage += fr.CreateEventProcessor(".displayLastPage");
                frontend.displayNextPage += fr.CreateEventProcessor(".displayNextPage");
                frontend.displayPrevPage += fr.CreateEventProcessor(".displayPrevPage");

                //fr.Message += Console.WriteLine;

                fr.Process(new Message(".run", new[]{"test1.txt"}));

                fr.WaitForResult();
            }
        }
    }
}
