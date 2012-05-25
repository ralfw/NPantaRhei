using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSV_Viewer.buffer;
using CSV_Viewer.data_model;
using CSV_Viewer.flows;
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

                fr.AddFlow(new Main(new Formatter(), 
                                    frontend));
                fr.AddFlow(new Features(new CommandlineParser(pageBufferContainer), 
                                        new TextFileAdapter(), 
                                        new LineBuffer(pageBufferContainer), 
                                        new Pager(pageBufferContainer)));

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
