using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.data;

namespace npantarhei.runtime.tests.data
{
    [TestFixture]
    public class test_PriorityQueue
    {
        [Test]
        public void Without_prio()
        {
            var sut = new PriorityQueue<int>();

            Assert.AreEqual(0, sut.Count);

            sut.Enqueue(1);
            Assert.AreEqual(1, sut.Count);
            sut.Enqueue(2);
            Assert.AreEqual(2, sut.Count);

            Assert.AreEqual(1, sut.Dequeue());
            Assert.AreEqual(1, sut.Count);

            Assert.AreEqual(2, sut.Dequeue());
            Assert.AreEqual(0, sut.Count);

            Assert.Throws<InvalidOperationException>(() => sut.Dequeue());

            sut.Enqueue(3);
            Assert.AreEqual(3, sut.Dequeue());
        }


        [Test]
        public void New_highest_prio_is_added_at_head()
        {
            var sut = new PriorityQueue<string>();

            sut.Enqueue("0");
            sut.Enqueue(1, "1");
            sut.Enqueue(2, "2");

            Assert.AreEqual("2", sut.Dequeue());
            Assert.AreEqual("1", sut.Dequeue());
            Assert.AreEqual("0", sut.Dequeue());
        }


        [Test]
        public void Insert_before_entry_with_lower_prio()
        {
            var sut = new PriorityQueue<string>();

            sut.Enqueue("0");
            sut.Enqueue(2, "2");
            sut.Enqueue(4, "4");
            sut.Enqueue(1, "1");
            sut.Enqueue(3, "3");

            Assert.AreEqual("4", sut.Dequeue());
            Assert.AreEqual("3", sut.Dequeue());
            Assert.AreEqual("2", sut.Dequeue());
            Assert.AreEqual("1", sut.Dequeue());
            Assert.AreEqual("0", sut.Dequeue());
        }


        [Test]
        public void Append_at_end_of_streak_of_entries_with_same_prio()
        {
            var sut = new PriorityQueue<string>();

            sut.Enqueue("01");
            sut.Enqueue(1, "11");
            sut.Enqueue(2, "21");
            sut.Enqueue(1, "12");
            sut.Enqueue(2, "22");
            sut.Enqueue("02");

            Assert.AreEqual("21", sut.Dequeue());
            Assert.AreEqual("22", sut.Dequeue());
            Assert.AreEqual("11", sut.Dequeue());
            Assert.AreEqual("12", sut.Dequeue());
            Assert.AreEqual("01", sut.Dequeue());
            Assert.AreEqual("02", sut.Dequeue());
        }
    }
}
