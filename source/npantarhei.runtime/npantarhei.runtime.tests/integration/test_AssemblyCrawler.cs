using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.config;
using npantarhei.runtime.contract;
using npantarhei.runtime.operations;

namespace npantarhei.runtime.tests.integration
{
    [TestFixture]
    public class test_AssemblyCrawler
    {
        [Test]
        public void Run()
        {
            var config = new FlowRuntimeConfiguration()
                .AddOperations(new AssemblyCrawler(this.GetType().Assembly))
                .AddStreamsFrom(@"
                                    /
                                    .in, f
                                    f, g
                                    g, CrawlerEbc.Process
                                    CrawlerEbc.Result, .out
                                 ");

            foreach(var op in config.Operations)
                Console.WriteLine(op.Name);

            using(var fr = new FlowRuntime(config, new Schedule_for_sync_depthfirst_processing()))
            {
                int result = 0;
                fr.Result += _ => result = (int)_.Data;

                fr.Process(".in", 2);

                Assert.AreEqual(36, result);
            }
        }
    }

    [InstanceOperations]
    public class CrawlerInstanceOps
    {
        public int f(int x)
        {
            return x + 1;
        }
    }

    [StaticOperations]
    public class CrawlerStaticOps
    {
        public static int g(int x)
        {
            return 2 * x;
        }
    }

    [EventBasedComponent]
    public class CrawlerEbc
    {
        public void Process(int x)
        {
            Result(x*x);
        }

        public event Action<int> Result;
    }
}
