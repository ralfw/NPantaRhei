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
        public void Fire_when_ready()
        {
            var sut = new ManualResetJoin(3);

            var results = new List<List<object>>();
            Action<List<object>> onJoin = results.Add;

            sut.Process(0, "a1", Guid.Empty, onJoin);
            sut.Process(1, "b1", Guid.Empty, onJoin);
            sut.Process(2, "c1", Guid.Empty, onJoin);
            Assert.That(new object[]{new object[]{"a1", "b1", "c1"}}, Is.EqualTo(results));
        }


        [Test]
        public void Keep_firing_while_ready()
        {
            var sut = new ManualResetJoin(3);

            var results = new List<List<object>>();
            Action<List<object>> onJoin = results.Add;

            sut.Process(0, "a1", Guid.Empty, onJoin);
            sut.Process(1, "b1", Guid.Empty, onJoin);
            sut.Process(2, "c1", Guid.Empty, onJoin);

            results.Clear();
            sut.Process(0, "a2", Guid.Empty, onJoin);
            Assert.That(new object[] { new object[] { "a2", "b1", "c1" } }, Is.EqualTo(results));

            results.Clear();
            sut.Process(1, "b2", Guid.Empty, onJoin);
            Assert.That(new object[] { new object[] { "a2", "b2", "c1" } }, Is.EqualTo(results));
        }


        [Test]
        public void Deplete_more_than_ready_join()
        {
            var sut = new ManualResetJoin(3);

            var results = new List<List<object>>();
            Action<List<object>> onJoin = results.Add;

            sut.Process(0, "a1", Guid.Empty, onJoin);
            sut.Process(1, "b1", Guid.Empty, onJoin);
            sut.Process(0, "a2", Guid.Empty, onJoin);
            sut.Process(1, "b2", Guid.Empty, onJoin);
            sut.Process(1, "b3", Guid.Empty, onJoin);
            sut.Process(2, "c1", Guid.Empty, onJoin);

            Assert.That(results, Is.EquivalentTo(new object[] { new object[] { "a1", "b1", "c1" },
                                                                new object[] { "a2", "b1", "c1" },
                                                                new object[] { "a2", "b2", "c1" },
                                                                new object[] { "a2", "b3", "c1" } }));
        }


        [Test]
        public void Stay_ready_after_depletion()
        {
            var sut = new ManualResetJoin(3);

            var results = new List<List<object>>();
            Action<List<object>> onJoin = results.Add;

            sut.Process(0, "a1", Guid.Empty, onJoin);
            sut.Process(1, "b1", Guid.Empty, onJoin);
            sut.Process(0, "a2", Guid.Empty, onJoin);
            sut.Process(1, "b2", Guid.Empty, onJoin);
            sut.Process(1, "b3", Guid.Empty, onJoin);
            sut.Process(2, "c1", Guid.Empty, onJoin);

            results.Clear();
            sut.Process(2, "c2", Guid.Empty, onJoin);
            Assert.That(new object[] { new object[] { "a2", "b3", "c2" } }, Is.EqualTo(results));
        }


        [Test]
        public void Join_with_multiple_correlationIds()
        {
            var sut = new ManualResetJoin(2);

            var results = new List<List<object>>();
            Action<List<object>> onJoin = results.Add;

            var corrId1 = Guid.NewGuid();
            var corrId2 = Guid.NewGuid();

            sut.Process(0, "a1", corrId1, onJoin);
            sut.Process(0, "x1", corrId2, onJoin);
            sut.Process(1, "b1", corrId1, onJoin);
            Assert.That(new object[] { new object[] { "a1", "b1" } }, Is.EqualTo(results));
            results.Clear();

            sut.Process(1, "y1", corrId2, onJoin);
            Assert.That(new object[] { new object[] { "x1", "y1" } }, Is.EqualTo(results));
            results.Clear();

            sut.Process(1, "b2", corrId1, onJoin);
            Assert.That(new object[] { new object[] { "a1", "b2" } }, Is.EqualTo(results));
            results.Clear();

            sut.Reset(corrId2);
            sut.Process(0, "s1", corrId2, onJoin);
            sut.Process(1, "t1", corrId2, onJoin);
            Assert.That(new object[] { new object[] { "s1", "t1" } }, Is.EqualTo(results));
        }
    }
}
