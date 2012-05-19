using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns.operations;

namespace npantarhei.runtime.tests.patterns
{
    [TestFixture]
    public class test_Flow
    {
        [Test]
        public void Step_down_into_a_flow()
        {
            IOperation sut = new SomeFlow();

            var msg = new Message("parent/flow.port", "hello");
            IMessage result = null;
            sut.Implementation(msg, _ => result = _, null);

            Assert.AreEqual("flow/flow.port", result.Port.Fullname);
            Assert.AreEqual("hello", (string)result.Data);
            Assert.AreEqual("parent", result.FlowStack.Pop());
        }

        [Test]
        public void Step_down_from_root_flow()
        {
            IOperation sut = new SomeFlow();

            var msg = new Message("flow.port", "hello");
            IMessage result = null;
            sut.Implementation(msg, _ => result = _, null);

            Assert.AreEqual("flow/flow.port", result.Port.Fullname);
            Assert.AreEqual("hello", (string)result.Data);
            Assert.IsTrue(result.FlowStack.IsEmpty);
        }

        [Test]
        public void Step_up_from_a_flow()
        {
            IOperation sut = new SomeFlow();

            var msg = new Message("flow/flow.port", "hello");
            msg.FlowStack.Push("parent");
            IMessage result = null;
            sut.Implementation(msg, _ => result = _, null);

            Assert.AreEqual("parent/flow.port", result.Port.Fullname);
            Assert.AreEqual("hello", (string)result.Data);
            Assert.IsTrue(result.FlowStack.IsEmpty);
        }

        [Test]
        public void Step_up_to_root_flow()
        {
            IOperation sut = new SomeFlow();

            var msg = new Message("flow/flow.port", "hello");
            IMessage result = null;
            sut.Implementation(msg, _ => result = _, null);

            Assert.AreEqual("flow.port", result.Port.Fullname);
            Assert.AreEqual("hello", (string)result.Data);
        }

        [Test]
        public void Map_streams()
        {
            var sut = new SomeFlow();

            var streams = sut.Streams.ToArray();

            Assert.AreEqual("f/f.in", streams[0].FromPort.Fullname);
            Assert.AreEqual("f/x", streams[0].ToPort.Fullname);
            Assert.AreEqual("f/x", streams[1].FromPort.Fullname);
            Assert.AreEqual("f/y.in", streams[1].ToPort.Fullname);
            Assert.AreEqual("f/y.out", streams[2].FromPort.Fullname);
            Assert.AreEqual("f/f.out", streams[2].ToPort.Fullname);
        }
    }

    class SomeFlow : Flow
    {
        public SomeFlow() : base("f") {}

        protected override IEnumerable<IStream> BuildStreams()
        {
            return new[]
                       {
                           new Stream(".in", "x"),
                           new Stream("x", "y.in"),
                           new Stream("y.out", ".out") 
                       };
        }
    }
}
