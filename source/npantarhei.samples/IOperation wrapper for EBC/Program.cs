using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime;

namespace IOperation_wrapper_for_EBC
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new FlowRuntimeConfiguration()
                            .AddStreamsFrom(@"
                                                /
                                                .in, toupper.process
                                                toupper.result, .out
                                             ")
                            .AddEventBasedComponent("toupper", new ToUpperEBC());

            using (var fr = new FlowRuntime(config))
            {
                fr.Process(".in", "hello");

                fr.WaitForResult(_ => Console.WriteLine(_.Data));
            }
        }


        static void Main_use_manual_wrapper(string[] args)
        {
            var config = new FlowRuntimeConfiguration()
                            .AddStreamsFrom(@"
                                                /
                                                .in, toupper
                                                toupper, .out
                                             ")
                            .AddOperation(new ToUpperOp("toupper"));

            using(var fr = new FlowRuntime(config))
            {
                fr.Process(".in", "hello");

                fr.WaitForResult(_ => Console.WriteLine(_.Data));
            }
        }
    }
}
