using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.tests.patterns
{
    [TestFixture]
    public class test_causality_operations
    {
        [Test]
        public void Push()
        {
            var sut = new PushCausality("pc", new Port("handler"));
            Assert.AreEqual("pc", sut.Name);

            CausalityStack result = null;
            sut.Implementation(new Message("x", "hello"), _ => result = _.Causalities, null);

            Assert.AreEqual("handler", result.Peek().Port.Fullname);
        }


        [Test]
        public void Pop()
        {
            var sut = new PopCausality("pc");
            Assert.AreEqual("pc", sut.Name);

            var msg = new Message("x", "hello");
            msg.Causalities.Push(new Port("handler"));

            CausalityStack result = null;
            sut.Implementation(msg, _ => result = _.Causalities, null);

            Assert.IsTrue(result.IsEmpty);
        }
    }
}
