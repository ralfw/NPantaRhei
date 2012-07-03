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
                fr.Process(".in", new Tuple<int,int>(42,7));

                IMessage result = null;
                fr.WaitForResult(_ => result = _);
                Assert.AreEqual(".result", result.Port.Fullname);
                Assert.AreEqual(6, (int)result.Data);


                fr.Process(".in", new Tuple<int, int>(42, 0));

                fr.WaitForResult(_ => result = _);
                Assert.AreEqual(".fehler", result.Port.Fullname);
                Assert.AreEqual(new Tuple<int,int>(42,0), result.Data);
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
    }
}
