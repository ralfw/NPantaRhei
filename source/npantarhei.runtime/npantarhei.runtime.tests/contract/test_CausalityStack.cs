using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.tests.contract
{
    [TestFixture]
    public class test_CausalityStack
    {
        [Test]
        public void Push()
        {
            var causalities = new Stack<Causality>();
            var sut = new CausalityStack(causalities);

            sut.Push(new Port("x"));

            Assert.AreEqual(1, causalities.Count);
            Assert.AreEqual("x", causalities.Pop().Port.Fullname);
        }

        [Test]
        public void Pop()
        {
            var causalities = new Stack<Causality>();
            causalities.Push(new Causality(new Port("x")));
            var sut = new CausalityStack(causalities);

            sut.Pop();

            Assert.AreEqual(0, causalities.Count);
        }

        [Test]
        public void IsEmpty()
        {
            var causalities = new Stack<Causality>();
            var sut = new CausalityStack(causalities);

            Assert.IsTrue(sut.IsEmpty);

            causalities.Push(new Causality(new Port("x")));

            Assert.IsFalse(sut.IsEmpty);
        }

        [Test]
        public void Peek()
        {
            var causalities = new Stack<Causality>();
            causalities.Push(new Causality(new Port("x")));
            causalities.Push(new Causality(new Port("y")));
            var sut = new CausalityStack(causalities);

            Assert.AreEqual("y", sut.Peek().Port.Fullname);
            Assert.AreEqual(2, causalities.Count);
        }

        [Test]
        public void Copy()
        {
            var causalities = new Stack<Causality>();
            causalities.Push(new Causality(new Port("x")));
            causalities.Push(new Causality(new Port("y")));
            var sut = new CausalityStack(causalities);

            var copyOfSut = sut.Copy();

            Assert.AreNotSame(copyOfSut, sut);

            Assert.AreEqual("y", copyOfSut.Peek().Port.Fullname);
            copyOfSut.Pop();
            Assert.AreEqual("x", copyOfSut.Peek().Port.Fullname);
            copyOfSut.Pop();
            Assert.IsTrue(copyOfSut.IsEmpty);

            Assert.AreEqual(2, causalities.Count);
        }
    }
}
