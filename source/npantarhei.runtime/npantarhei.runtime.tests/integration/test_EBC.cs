using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.operations;

namespace npantarhei.runtime.tests.integration
{
    [TestFixture]
    public class test_EBC
    {
        [Test]
        public void Run()
        {
            var config = new FlowRuntimeConfiguration()
                                .AddStreamsFrom(@"
                                                    /
                                                    .in, rechenwerk.teilen
                                                    rechenwerk.resultat, .result
                                                    rechenwerk.divisionDurchNull, .fehler
                                                    ")
                                .AddEventBasedComponent("rechenwerk", new Rechenwerk());

            using(var fr = new FlowRuntime(config))
            {
                fr.Message += Console.WriteLine;
                fr.UnhandledException += Console.WriteLine;

                fr.Process(".in", new Tuple<int,int>(42,7));

                IMessage result = null;
                Assert.IsTrue(fr.WaitForResult(1000, _ => result = _));
                Assert.AreEqual(".result", result.Port.Fullname);
                Assert.AreEqual(6, (int)result.Data);


                fr.Process(".in", new Tuple<int, int>(42, 0));

                Assert.IsTrue(fr.WaitForResult(2000, _ => result = _));
                Assert.AreEqual(".fehler", result.Port.Fullname);
                Assert.AreEqual(new Tuple<int,int>(42,0), result.Data);
            }
        }


        [Test]
        public void Active_EBC_fires_independently()
        {
            var ebc = new ActiveEbc();

            var config = new FlowRuntimeConfiguration()
                                .AddStreamsFrom(@"
                                                    /
                                                    ebc.Out, .out
                                                 ")
                                .AddEventBasedComponent("ebc", ebc);

            using(var fr = new FlowRuntime(config))
            {
                ebc.Run("hello");

                var result = "";
                Assert.IsTrue(fr.WaitForResult(1000, _ => result = (string) _.Data));
                Assert.AreEqual("hellox", result);
            }
        }


        [Test]
        public void Active_EBC_fires_in_flow()
        {
            var ebc = new ActiveEbc();

            var config = new FlowRuntimeConfiguration()
                                .AddStreamsFrom(@"
                                                    /
                                                    .in, subflow.in
                                                    subflow.out, .out

                                                    subflow
                                                    .in, ebc.Run
                                                    ebc.Out, .out
                                                 ")
                                .AddEventBasedComponent("ebc", ebc);

            using (var fr = new FlowRuntime(config))
            {
                fr.Process(".in", "hello");

                var result = "";
                Assert.IsTrue(fr.WaitForResult(2000, _ => result = (string)_.Data));
                Assert.AreEqual("hellox", result);
            }
        }


        [Test]
        public void An_active_ebc_fires_an_event_before_another_ebc_receives()
        {
            var ebc1 = new ActiveEbc();

            var config = new FlowRuntimeConfiguration()
                                .AddStreamsFrom(@"
                                                    /
                                                    ebc1.Out, ebc2.Run
                                                    ebc2.Out, .out
                                                 ")
                                .AddEventBasedComponent("ebc1", ebc1)
                                .AddEventBasedComponent("ebc2", new ActiveEbc());

            using (var fr = new FlowRuntime(config))
            {
                ebc1.Run("hello");

                var result = "";
                Assert.IsTrue(fr.WaitForResult(2000, _ => result = (string)_.Data));
                Assert.AreEqual("helloxx", result);
            }
        }


        [Test]
        public void Processing_message_with_async_EBC_method()
        {
            var config = new FlowRuntimeConfiguration()
                                .AddStreamsFrom(@"
                                                    /
                                                    .in, ebc.Run
                                                    ebc.Out, .out
                                                 ")
                                .AddEventBasedComponent("ebc", new AsyncEbc());

            using (var fr = new FlowRuntime(config))
            {
                var result = "";

                fr.Process(".in", "hello");

                Assert.IsTrue(fr.WaitForResult(2000, _ => result = (string)_.Data));
                Assert.AreEqual("hellox", result);
            }
        }


        [Test]
        public void Allow_sequential_EBC_on_same_thread()
        {
            var config = new FlowRuntimeConfiguration()
                                .AddStreamsFrom(@"
                                                    /
                                                    .in, ebc1.Run
                                                    ebc1.Out, ebc2.Run
                                                    ebc2.Out, .out
                                                 ")
                                .AddEventBasedComponent("ebc1", new ActiveEbc())
                                .AddEventBasedComponent("ebc2", new ActiveEbc());

            using (var fr = new FlowRuntime(config, new Schedule_for_sync_depthfirst_processing()))
            {
                var result = "";

                fr.Process(".in", "hello");

                fr.Result += _ => result = (string) _.Data;

                Assert.AreEqual("helloxx", result);
            }
        }


        [Test]
        public void CorrelationId_is_retained()
        {
            var config = new FlowRuntimeConfiguration()
                                .AddStreamsFrom(@"
                                                    /
                                                    .in, ebc.Run
                                                    ebc.Out, .out
                                                 ")
                                .AddEventBasedComponent("ebc", new AsyncEbc());

            using (var fr = new FlowRuntime(config, new Schedule_for_sync_depthfirst_processing()))
            {
                IMessage result = null;

                var corrId = Guid.NewGuid();
                fr.Process(new Message(".in", "hello", corrId));

                Assert.IsTrue(fr.WaitForResult( _ => result = _));
                Assert.AreEqual(corrId, result.CorrelationId);
            }
        }


        class Rechenwerk
        {
            public void Teilen(Tuple<int,int> input)
            {
                if (input.Item2 == 0)
                    DivisionDurchNull(input);
                else
                    Resultat(input.Item1/input.Item2);
            }

            public event Action<int> Resultat;
            public event Action<Tuple<int,int>> DivisionDurchNull;
        }


        class ActiveEbc
        {
            public void Run(string s)
            {
                Out(s + "x");
            }

            public event Action<string> Out;
        }


        class AsyncEbc
        {
            [AsyncMethod]
            public void Run(string s)
            {
                Out(s + "x");
            }

            public event Action<string> Out;
        }
    }
}
