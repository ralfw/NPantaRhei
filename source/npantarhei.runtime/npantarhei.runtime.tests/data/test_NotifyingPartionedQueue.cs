using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.data;

namespace npantarhei.runtime.tests.data
{
    [TestFixture]
    public class test_NotifyingPartionedQueue
    {
        [Test]
        public void Single_partition()
        {
            var sut = new NotifyingPartionedQueue<PartitionedTestData>.PartitionRing();

            var p1 = sut.Create("a");
            Assert.AreSame(p1, sut.Next());

            sut.Create("a");
            Assert.AreSame(p1, sut.Next());
        }


        [Test]
        public void Removing_a_single_partition()
        {
            var sut = new NotifyingPartionedQueue<PartitionedTestData>.PartitionRing();

            var p1 = sut.Create("a");
            sut.Remove(p1);

            Assert.Throws<InvalidOperationException>(() => sut.Next());
        }


        [Test]
        public void Multiple_partitions()
        {
            var sut = new NotifyingPartionedQueue<PartitionedTestData>.PartitionRing();

            var p1 = sut.Create("a");
            var p2 = sut.Create("b");

            Assert.AreSame(p1, sut.Next());
            Assert.AreSame(p2, sut.Next());
        }


        [Test]
        public void Queue_messages_in_multiple_partitions()
        {
            var sut = new NotifyingPartionedQueue<PartitionedTestData>();

            sut.Enqueue(0, new PartitionedTestData("a", "1"));
            sut.Enqueue(0, new PartitionedTestData("b", "2"));
            sut.Enqueue(0, new PartitionedTestData("b", "4"));
            sut.Enqueue(0, new PartitionedTestData("c", "3"));
            sut.Enqueue(0, new PartitionedTestData("c", "5"));
            sut.Enqueue(0, new PartitionedTestData("c", "6"));

            PartitionedTestData result;
            Assert.IsTrue(sut.TryDequeue(out result));
            Assert.AreEqual("1", result.Data);
            Assert.IsTrue(sut.TryDequeue(out result));
            Assert.AreEqual("2", result.Data);
            Assert.IsTrue(sut.TryDequeue(out result));
            Assert.AreEqual("3", result.Data);
            Assert.IsTrue(sut.TryDequeue(out result));
            Assert.AreEqual("4", result.Data);
            Assert.IsTrue(sut.TryDequeue(out result));
            Assert.AreEqual("5", result.Data);
            Assert.IsTrue(sut.TryDequeue(out result));
            Assert.AreEqual("6", result.Data);
        }


        class PartitionedTestData : IPartionable
        {
            private readonly string _partition;
            private readonly string _data;

            public PartitionedTestData(string partition, string data)
            {
                _partition = partition;
                _data = data;
            }

            public string Partition { get { return _partition; } }
            public string Data { get { return _data; } }
        }

    }
}
