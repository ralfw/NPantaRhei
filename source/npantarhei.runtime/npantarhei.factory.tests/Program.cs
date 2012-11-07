using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime;
using npantarhei.runtime.contract;

namespace npantarhei.factory.tests
{
    class Program
    {
        static void Main(string[] args)
        {
            FlowRuntimeFactory.Beginner.Process(".in", "world");
            FlowRuntimeFactory.Basic.RunAndWait(".in", "world2");
        }
    }


    [StaticOperations]
    class Greeting
    {
        public static void Hello(string name)
        {
            Console.WriteLine("Hello, {0}!", name);
        }
    }
}
