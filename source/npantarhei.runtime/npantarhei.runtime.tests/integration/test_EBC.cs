using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.contract;

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
                Assert.IsTrue(fr.WaitForResult(_ => result = (string) _.Data));
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
                                                    .in, ebc.Run
                                                    ebc.Out, .out
                                                 ")
                                .AddEventBasedComponent("ebc", ebc);

            using (var fr = new FlowRuntime(config))
            {
                fr.Process(".in", "hello");

                var result = "";
                Assert.IsTrue(fr.WaitForResult(_ => result = (string)_.Data));
                Assert.AreEqual("hellox", result);
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
    }
}
