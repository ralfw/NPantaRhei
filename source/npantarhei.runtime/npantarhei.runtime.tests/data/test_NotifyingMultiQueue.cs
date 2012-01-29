using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.data;

namespace npantarhei.runtime.tests.data
{
    [TestFixture]
    public class test_NotifyingMultiQueue
    {
        private NotifyingMultiQueue<string> _sut;
        private string _result;

        [SetUp]
        public void Before_each_test()
        {
            _sut = new NotifyingMultiQueue<string>();
            _result = null;
        }
            
        [Test]
        public void Single_worker_takes_from_single_queue()
        {
            _sut.Enqueue("a", "q1");
            Assert.IsTrue(_sut.TryDequeue("w1", out _result));
            Assert.AreEqual("a", _result);
        }

        [Test]
        public void Single_worker_takes_from_multiple_queues()
        {
            _sut.Enqueue("a", "q1");
            _sut.Enqueue("b", "q1");
            _sut.Enqueue("x", "q2");

            _sut.TryDequeue("w1", out _result);
            Assert.AreEqual("a", _result);
            _sut.TryDequeue("w1", out _result);
            Assert.AreEqual("x", _result);
        }

        [Test]
        public void Single_worker_roundrobintakes_from_multiple_queues()
        {
            _sut.Enqueue("a", "q1");
            _sut.Enqueue("b", "q1");
            _sut.Enqueue("x", "q2");
            _sut.Enqueue("y", "q2");

            _sut.TryDequeue("w1", out _result);
            Assert.AreEqual("a", _result);
            _sut.TryDequeue("w1", out _result);
            Assert.AreEqual("x", _result);
            _sut.TryDequeue("w1", out _result);
            Assert.AreEqual("b", _result);
            _sut.TryDequeue("w1", out _result);
            Assert.AreEqual("y", _result);
        }

        [Test]
        public void Worker_blocks_access_to_a_queue()
        {
            _sut.Enqueue("a", "q1");
            _sut.Enqueue("b", "q1");
            _sut.TryDequeue("w1", out _result);
            Assert.IsFalse(_sut.TryDequeue("w2", out _result));
        }

        [Test]
        public void Blocked_queue_is_freed_by_blocking_worker_taking_from_another_queue()
        {
            _sut.Enqueue("a", "q1");
            _sut.Enqueue("b", "q1");
            _sut.Enqueue("x", "q2");

            _sut.TryDequeue("w1", out _result);
            _sut.TryDequeue("w1", out _result);
            Assert.AreEqual("x", _result);
            _sut.TryDequeue("w2", out _result);
            Assert.AreEqual("b", _result);
        }

        [Test]
        public void Blocked_queue_is_skipped_by_roundrobbin()
        {
            _sut.Enqueue("a", "q1");
            _sut.Enqueue("b", "q1");
            _sut.Enqueue("x", "q2");
            _sut.Enqueue("y", "q2");

            _sut.TryDequeue("w1", out _result);
            _sut.TryDequeue("w2", out _result);
            Assert.AreEqual("x", _result);
            _sut.TryDequeue("w2", out _result);
            Assert.AreEqual("y", _result);
        }


        [Test]
        public void TryDequeue_returns_false_upon_no_queues()
        {
            Assert.IsFalse(_sut.TryDequeue("w1", out _result));
        }

        [Test]
        public void TryDequeue_returns_false_upon_blocked_queues()
        {
            _sut.Enqueue("a", "q1");
            _sut.Enqueue("x", "q2");
            _sut.TryDequeue("w1", out _result);
            _sut.TryDequeue("w2", out _result);

            Assert.IsFalse(_sut.TryDequeue("w3", out _result));
        }

        [Test]
        public void TryDequeue_returns_false_upon_empty_queue()
        {
            _sut.Enqueue("a", "q1");
            _sut.TryDequeue("w1", out _result);

            Assert.IsFalse(_sut.TryDequeue("w1", out _result));
        }

        [Test]
        public void TryDequeue_returns_false_upon_no_ready_queues()
        {
            _sut.Enqueue("a", "q1");
            _sut.Enqueue("x", "q2");
            _sut.TryDequeue("w1", out _result);
            _sut.TryDequeue("w2", out _result);

            Assert.IsFalse(_sut.TryDequeue("w1", out _result));
        }

        [Test]
        public void Wait_successful_after_enqueue()
        {
            _sut.Enqueue("a", "q1");
            Assert.IsTrue(_sut.Wait(100));
        }

        [Test]
        public void Wait_unsuccessful_after_dequeue()
        {
            _sut.Enqueue("a", "q1");
            _sut.TryDequeue("w1", out _result);
            Assert.IsFalse(_sut.Wait(100));
        }

        [Test]
        public void No_signal_if_enqueue_into_blocked_queue()
        {
            _sut.Enqueue("a", "q1");
            _sut.TryDequeue("w1", out _result);
            _sut.Enqueue("b", "q1");
            Assert.IsFalse(_sut.Wait(100));   
        }

        [Test]
        public void Manually_signal()
        {
            _sut.Notify();
            Assert.IsTrue(_sut.Wait(100));
        }
    }
}
