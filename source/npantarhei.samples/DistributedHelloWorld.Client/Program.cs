using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using npantarhei.distribution.wcf;
using npantarhei.runtime;

namespace DistributedHelloWorld.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var portnumber = 8100 + DateTime.Now.Second;

            var config = new FlowRuntimeConfiguration()
                                .AddEventBasedComponent("ui", new UI()) 
                                .AddOperation(new WcfStandInOperation("proxy", "localhost:"+portnumber, "localhost:8000"))
                                .AddStreamsFrom("DistributedHelloWorld.Client.root.flow", 
                                                Assembly.GetExecutingAssembly());

            using(var fr = new FlowRuntime(config))
            {
                Console.WriteLine("Enter names: ");
                fr.Process(".run");
                fr.WaitForResult();
            }
        }
    }

    class UI
    {
        public void Wait_for_names()
        {
            while (true)
            {
                var name = Console.ReadLine();
                if (name != "")
                    Name(name);
                else
                {
                    Exit();
                    return;
                }
            }
        }

        public event Action<string> Name;
        public event Action Exit;

        public void Display_greeting(string greeting)
        {
            Console.WriteLine(greeting);
        }
    }
}
