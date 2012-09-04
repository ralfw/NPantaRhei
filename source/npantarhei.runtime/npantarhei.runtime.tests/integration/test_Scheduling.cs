using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.operations;

namespace npantarhei.runtime.tests.integration
{
    [TestFixture]
    public class test_Scheduling
    {
        private FlowRuntimeConfiguration _config;
        private string _payload_flow;

        [SetUp]
        public void Setup()
        {
            _config = new FlowRuntimeConfiguration()
                .AddStreamsFrom(@"
                                    /
                                    .in, a
                                    a.out0, b
                                    a.out1, c
                                    b, d
                                 ")
                .AddAction<string, string, string>("a", A)
                .AddAction<string, string>("b", B)
                .AddAction<string>("c", C)
                .AddAction<string>("d", D);

            _payload_flow = "";
        }


        void A(string s, Action<string> out0, Action<string> out1)
        {
            _payload_flow += "/a(" + s + ")";
            out0(s + "1");
            out0(s + "2");
            out1(s + "3");
        }

        void B(string s, Action<string> out0)
        {
            _payload_flow += "/b(" + s + ")";
            out0(s + "1");
            out0(s + "2");
        }

        void C(string s)
        {
            _payload_flow += "/c(" + s + ")";
        }

        void D(string s)
        {
            _payload_flow += "/d(" + s + ")";
        }



        [Test]
        public void Roundrobin()
        {
            using(var fr = new FlowRuntime(_config, new Schedule_for_async_roundrobin_processing()))
            {
                fr.Process(".in", "x");

                fr.WaitForResult(1000);

                Assert.AreEqual("/a(x)/b(x1)/c(x3)/b(x2)/d(x11)/d(x12)/d(x21)/d(x22)", _payload_flow);
            }
        }


        [Test]
        public void Breadthfirst()
        {
            using (var fr = new FlowRuntime(_config, new Schedule_for_async_breadthfirst_processing()))
            {
                fr.Process(".in", "x");

                fr.WaitForResult(1000);

                Assert.AreEqual("/a(x)/b(x1)/b(x2)/c(x3)/d(x11)/d(x12)/d(x21)/d(x22)", _payload_flow);
            }
        }

        [Test]
        public void Depthfirst()
        {
            using (var fr = new FlowRuntime(_config, new Schedule_for_async_depthfirst_processing()))
            {
                fr.Process(".in", "x");

                fr.WaitForResult(1000);

                Assert.AreEqual("/a(x)/b(x1)/d(x11)/d(x12)/b(x2)/d(x21)/d(x22)/c(x3)", _payload_flow);
            }
        }

        [Test]
        public void Sync_depthfirst()
        {
            using (var fr = new FlowRuntime(_config, new Schedule_for_sync_depthfirst_processing()))
            {
                fr.Process(".in", "x");

                Assert.AreEqual("/a(x)/b(x1)/d(x11)/d(x12)/b(x2)/d(x21)/d(x22)/c(x3)", _payload_flow);
            }
        }
    }
}
