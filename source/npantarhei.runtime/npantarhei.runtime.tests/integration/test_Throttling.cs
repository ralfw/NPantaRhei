using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns.operations;

namespace npantarhei.runtime.tests.integration
{
    [TestFixture]
    public class test_Throttling
    {
        [Test]
        public void Throttle()
        {
            using (var fr = new FlowRuntime())
            {
                var foc = new FlowOperationContainer();
                foc.AddFunc<int, int>("nop1", _ =>
                                                      {
                                                          Console.WriteLine("nop1: {0}", _);
                                                          return _;
                                                      });
                foc.AddFunc<int, int>("nop2", _ =>
                                                    {
                                                        Console.WriteLine("nop2: {0}", _);
                                                        return _;
                                                    });
                fr.AddOperations(foc.Operations);

                fr.AddStream(new Stream(".in", "nop1"));
                fr.AddStream(new Stream("nop1", "nop2"));
                fr.AddStream(new Stream("nop2", ".out"));

                var are = new AutoResetEvent(false);
                fr.Result += _ => { if ((int)_.Data == -1) are.Set(); };

                fr.Throttle(100);

                new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, -1 }.ToList().ForEach(i => fr.Process(new Message(".in", i)));

                Assert.IsFalse(are.WaitOne(1000));
            }
        }
    }
}
