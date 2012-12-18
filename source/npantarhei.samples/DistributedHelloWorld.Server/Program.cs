using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.distribution.wcf;
using npantarhei.runtime;

namespace DistributedHelloWorld.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new FlowRuntimeConfiguration()
                                .AddFunc<string, string>("greet", Greet)
                                .AddStream(".@greet", "greet")
                                .AddStream("greet", ".@greet");
            using(var fr = new FlowRuntime(config))
            using(new WcfOperationHost(fr, "localhost:8000"))
            {
                Console.WriteLine("Waiting for names...");
                Console.ReadLine();
            }
        }


        static string Greet(string name)
        {
            Console.WriteLine("  greet {0}", name);
            return "Hello, " + name;
        }
    }
}
