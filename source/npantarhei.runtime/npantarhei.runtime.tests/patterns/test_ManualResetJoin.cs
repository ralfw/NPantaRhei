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
    public class test_ManualResetJoin
    {
        [Test]
        public void Single_join_on_new_item()
        {
            var sut = new ManualResetJoin(3);

            var results = new List<List<object>>();
            Action<List<object>> onJoin = results.Add;

            sut.Process(0, "1", onJoin);
            sut.Process(1, 1, onJoin);
            sut.Process(2, "a", onJoin);
            Assert.That(new object[]{new object[]{"1", 1, "a"}}, Is.EqualTo(results));

            results.Clear();
            sut.Process(1, 2, onJoin);
            sut.Process(0, "2", onJoin);
            Assert.That(new object[] { new object[] { "2", 2, "a" } }, Is.EqualTo(results));
        }

    
        [Test]
        public void Multiple_joins_on_new_item()
        {
            var sut = new ManualResetJoin(3);

            var results = new List<List<object>>();
            Action<List<object>> onJoin = results.Add;

            sut.Process(0, "1", onJoin);
            sut.Process(1, 1, onJoin);
            sut.Process(0, "2", onJoin);
            sut.Process(1, 2, onJoin);
            sut.Process(0, "3", onJoin);
            sut.Process(2, "a", onJoin);
            Assert.That(new object[] { new object[] { "1", 1, "a" }, new object[] { "2", 2, "a" } }, Is.EqualTo(results));

            results.Clear();
            sut.Process(1, 3, onJoin);
            Assert.That(new object[] { new object[] { "3", 3, "a" } }, Is.EqualTo(results));
        }
    }
}
