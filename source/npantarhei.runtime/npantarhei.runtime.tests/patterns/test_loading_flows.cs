using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.patterns.flows;
using npantarhei.runtime.patterns.operations;

namespace npantarhei.runtime.tests.patterns
{
    [TestFixture]
    public class test_loading_flows
    {
        [Test]
        public void Single_flow()
        {
            var tr = new StringReader("a,b\nb,c\nc,d");

            var portnames = FlowLoader.LoadFromReader("x", tr).SelectMany(s => new[] {s.FromPort.Fullname, s.ToPort.Fullname});
            Assert.That(portnames.ToArray(), Is.EqualTo(new[]{"x/a", "x/b", "x/b", "x/c", "x/c", "x/d"}));
        }


        [Test]
        public void Comments_and_whitespace()
        {
            var tr = new StringReader(@"
                                        a, b
                                        b,  c
                                        // comment
                                        c,   d
                                       ");

            var portnames = FlowLoader.LoadFromReader("x", tr).SelectMany(s => new[] { s.FromPort.Fullname, s.ToPort.Fullname });
            Assert.That(portnames.ToArray(), Is.EqualTo(new[] { "x/a", "x/b", "x/b", "x/c", "x/c", "x/d" }));
        }

        [Test]
        public void Multiple_flows()
        {
            var tr = new StringReader(@"
                                        a, b
                                        b,  c
                                        // comment
                                        f
                                        c,   d

                                        g
                                        d,e
                                        f,g

                                        /
                                        h,i
                                       ");

            var portnames = FlowLoader.LoadFromReader("x", tr).SelectMany(s => new[] { s.FromPort.Fullname, s.ToPort.Fullname });
            Assert.That(portnames.ToArray(), Is.EqualTo(new[] { "x/a", "x/b", "x/b", "x/c", "f/c", "f/d", "g/d", "g/e", "g/f", "g/g", "h", "i" }));
        }


        [Test]
        public void From_file()
        {
            var portnames = FlowLoader.LoadFromFile("x", "flowFile.txt").SelectMany(s => new[] { s.FromPort.Fullname, s.ToPort.Fullname });
            Assert.That(portnames.ToArray(), Is.EqualTo(new[] { "x/a", "x/b", "f/b", "f/c" }));
        }

        [Test]
        public void From_embedded_resource()
        {
            var portnames = FlowLoader.LoadFromEmbeddedResource("x", this.GetType(), "npantarhei.runtime.tests.flowEmbeddedResource.txt").SelectMany(s => new[] { s.FromPort.Fullname, s.ToPort.Fullname });
            Assert.That(portnames.ToArray(), Is.EqualTo(new[] { "x/a", "x/b", "g/b", "g/c" }));
        }

        [Test]
        public void From_unkown_resource()
        {
            Assert.Throws<InvalidOperationException>(() => FlowLoader.LoadFromEmbeddedResource("x", this.GetType(), "xxx"));
        }

        [Test]
        public void Unqualified_portnames_will_not_be_qualified_by_flow_class()
        {
            var sut = new StringFlow("/", "a,b");
            var portnames = sut.Streams.SelectMany(s => new[] { s.FromPort.Fullname, s.ToPort.Fullname });
            Assert.That(portnames.ToArray(), Is.EqualTo(new[] { "a", "b" }));
        }
    }
}
