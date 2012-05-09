using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using npantarhei.runtime;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace Tracing_with_Rx
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var fr = new FlowRuntime())
            {
                fr.AddStream(".in", "A");
                fr.AddStream("A", "B");
                fr.AddStream("B", "C");

                fr.AddOperations(new FlowOperationContainer()
                                    .AddFunc<int,int>("A", i => i + 1)
                                    .AddFunc<int,int>("B", i => i * 2)
                                    .AddAction<int>("C", i => Console.WriteLine("={0}", i))
                                    .Operations);

                fr.Start();


                // Trace messages selectively using Rx
                var tracer = new Subject<IMessage>();
                tracer.Where(msg => msg.Port.OperationName == "B") // message filter
                      .Select(msg => (int)msg.Data)
                      .Subscribe(i => Console.WriteLine("{0} -> B", i), // message handler
                                 _ => { }); 
                fr.Message += tracer.OnNext;


                fr.Process(new Message(".in", 1));
                fr.Process(new Message(".in", 2));
                fr.Process(new Message(".in", 3));

                fr.WaitForResult(500);
            }
        }
    }
}
