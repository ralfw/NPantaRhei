using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.tests.patterns
{
    [TestFixture]
    public class test_AutoResetJoin
    {
        [Test]
        public void Test_base_join()
        {
            var sut = new AutoResetJoin(2);

            List<object> result = null;
            Action<List<object>> onJoin = _ => result = _;

            sut.Process(0, "1", onJoin);
            sut.Process(1, 1, onJoin);
            Assert.That(new object[]{"1", 1}, Is.EqualTo(result));

            sut.Process(0, "2", onJoin);
            sut.Process(0, "3", onJoin);
            sut.Process(1, 2, onJoin);
            Assert.That(new object[] { "2", 2 }, Is.EqualTo(result));

            sut.Process(1, 3, onJoin);
            Assert.That(new object[] { "3", 3 }, Is.EqualTo(result));
        }


        [Test]
        public void Test_operation()
        {
            var sut = new AutoResetJoin<string, int>("arj");

            IMessage result = null;
            Action<IMessage> onJoin = _ => result = _;

            sut.Implementation(new Message(new Port("x.in0"), "1"), onJoin, null);
            sut.Implementation(new Message(new Port("x.in1"), 1), onJoin, null);
            Assert.AreEqual("arj", result.Port.Fullname);
            Assert.AreEqual(new Tuple<string,int>("1", 1), result.Data);
        }
    }
}
