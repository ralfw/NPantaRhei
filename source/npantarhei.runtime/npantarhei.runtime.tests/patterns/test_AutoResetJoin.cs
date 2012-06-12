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

            sut.Process(0, "a", Guid.Empty, onJoin);
            sut.Process(1, 1, Guid.Empty, onJoin);
            Assert.That(new object[]{"a", 1}, Is.EqualTo(result));

            sut.Process(0, "b", Guid.Empty, onJoin);
            sut.Process(0, "c", Guid.Empty, onJoin);
            sut.Process(1, 2, Guid.Empty, onJoin);
            Assert.That(result, Is.EqualTo(new object[] { "b", 2 }));

            sut.Process(1, 3, Guid.Empty, onJoin);
            Assert.That(result, Is.EqualTo(new object[] { "c", 3 }));
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


        [Test]
        public void Multiple_correlationIds()
        {
            var sut = new AutoResetJoin(2);

            List<object> result = null;
            Action<List<object>> onJoin = _ => result = _;

            var corrId1 = Guid.NewGuid();
            var corrId2 = Guid.NewGuid();

            sut.Process(0, "a", corrId1, onJoin);
            sut.Process(0, "x", corrId2, onJoin);
            sut.Process(1, 1, corrId1, onJoin);
            Assert.That(new object[] { "a", 1 }, Is.EqualTo(result));

            sut.Process(1, 10, corrId2, onJoin);
            Assert.That(new object[] { "x", 10 }, Is.EqualTo(result));

            sut.Process(0, "b", corrId1, onJoin);
            sut.Process(1, 11, corrId2, onJoin);
            sut.Process(1, 2, corrId1, onJoin);
            Assert.That(result, Is.EqualTo(new object[] { "b", 2 }));

            sut.Process(0, "y", corrId2, onJoin);
            Assert.That(result, Is.EqualTo(new object[] { "y", 11 }));
        }
    }
}
