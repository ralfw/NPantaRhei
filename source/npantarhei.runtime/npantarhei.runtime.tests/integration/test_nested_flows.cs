using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns.operations;

namespace npantarhei.runtime.tests.integration
{
    [TestFixture]
    public class test_nested_flows
    {
        [Test]
        public void Nested_flows()
        {
            using(var fr = new FlowRuntime())
            {
                fr.AddStream(".in", "a.in");
                fr.AddStream("a.out", ".out");

                fr.AddOperation(new A());

                fr.Message += Console.WriteLine;

                fr.Process(new Message(".in", "hello"));

                IMessage result = null;
                Assert.IsTrue(fr.WaitForResult(2000, _ => result = _));
                Assert.AreEqual(".out", result.Port.Fullname);
                Assert.AreEqual("helloxy", (string)result.Data);
            }
        }
    }

    class A : Flow
    {
        public A() : base("a") {}

        protected override IEnumerable<IStream> BuildStreams()
        {
            return new[]
                       {
                           new Stream(".in", "b.in"),
                           new Stream("b.out", ".out") 
                       };
        }

        protected override IEnumerable<IOperation> BuildOperations(FlowOperationContainer container)
        {
            return new[] { new B() };
        }
    }

    class B : Flow
    {
        public B() : base("b") { }

        protected override IEnumerable<IStream> BuildStreams()
        {
            return new[]
                       {
                           // Flow B
                           new Stream(".in", "c.in"),
                           new Stream("c.out", "d.in"), // use D within B
                           new Stream("d.out", ".out"), 

                           // Flow C
                           new Stream("/c/c.in", "/c/d.in"), // use D within C
                           new Stream("/c/d.out", "/c/c.out"), 
 
                           // Flow D
                           new Stream("/d/d.in", "/d/x"),
                           new Stream("/d/x", "/d/d.out")                        
                       };
        }

        protected override IEnumerable<IOperation> BuildOperations(FlowOperationContainer container)
        {
            return container
                    .AddFunc<string, string>("x", s => s + (s.EndsWith("x") ? "y" : "x"))
                    .Operations;
        }
    }
}
