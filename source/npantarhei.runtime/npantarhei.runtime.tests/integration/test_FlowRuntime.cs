using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

using npantarhei.runtime;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.tests.integration
{
	[TestFixture]
	public class test_FlowRuntime
	{
        private FlowRuntime _sut;
        private AutoResetEvent _are;
        private IMessage _result;


        [SetUp]
        public void Setup()
        {
            _sut = new FlowRuntime();
            _sut.Start();

            _are = new AutoResetEvent(false);
            _result = null;
            _sut.SetResultHandler(_ =>
                                      {
                                          _result = _;
                                          _are.Set();
                                      });
        }

        [TearDown]
        public void TearDown()
        {
            _sut.Dispose();
        }


		[Test]
		public void No_processing_Just_redirect_input_to_output()
		{
			_sut.AddStream(new Stream(".in", ".out"));
			
			_sut.Process(new Message(".in", "hello"));

		    Assert.IsTrue(_are.WaitOne(1000));
			Assert.AreEqual(".out", _result.Port.Fullname);
			Assert.AreEqual("hello", _result.Data.ToString());
		}
		
		[Test]
		public void Single_operation()
		{
			_sut.AddStream(new Stream(".in", "A.in"));
			_sut.AddStream(new Stream("A.out", ".out"));
			
			_sut.AddOperation(new Operation("A", (input, outputCont) => outputCont(new Message("A.out", input.Data.ToString() + "x"))));
			
			_sut.Process(new Message(".in", "hello"));

            Assert.IsTrue(_are.WaitOne(1000));
			Assert.AreEqual(".out", _result.Port.Fullname);
			Assert.AreEqual("hellox", _result.Data.ToString());
		}
		
		[Test]
		public void Sequence_of_operations()
		{
			_sut.AddStream(new Stream(".in", "A.in"));
			_sut.AddStream(new Stream("A.out", "B.in"));
			_sut.AddStream(new Stream("B.out", ".out"));
			
			_sut.AddOperation(new Operation("A", (input, outputCont) => outputCont(new Message("A.out", input.Data.ToString() + "x"))));
			_sut.AddOperation(new Operation("B", (input, outputCont) => outputCont(new Message("B.out", input.Data.ToString() + "y"))));
			
			_sut.Process(new Message(".in", "hello"));

            Assert.IsTrue(_are.WaitOne(1000));
			Assert.AreEqual(".out", _result.Port.Fullname);
			Assert.AreEqual("helloxy", _result.Data.ToString());
		}
		
		[Test]
		public void Output_with_fan_out()
		{
			_sut.AddStream(new Stream(".in", "A.in"));
			_sut.AddStream(new Stream(".in", "B.in"));
			_sut.AddStream(new Stream("A.out", ".out"));
			_sut.AddStream(new Stream("B.out", ".out"));
			
			_sut.AddOperation(new Operation("A", (input, outputCont) => outputCont(new Message("A.out", input.Data.ToString() + "x"))));
			_sut.AddOperation(new Operation("B", (input, outputCont) => outputCont(new Message("B.out", input.Data.ToString() + "y"))));
			
			var results = new List<IMessage>();
		    var n = 0;
            _sut.SetResultHandler(_ =>
                                        {
                                            results.Add(_);
                                            n++;
                                            if (n==2) _are.Set();
                                        });
			
			_sut.Process(new Message(".in", "hello"));

            Assert.IsTrue(_are.WaitOne(1000));
			Assert.That(results.Select(m => m.Data.ToString()).ToArray(), Is.EquivalentTo(new[]{"hellox", "helloy"}));
		}
		
		[Test]
		public void Ports_with_context()
		{
			_sut.AddStream(new Stream(".in", "A/B/X.in"));
			_sut.AddStream(new Stream("A/B/X.out", ".out"));
			
			_sut.AddOperation(new Operation("X", (input, outputCont) => outputCont(new Message("A/B/X.out", input.Data.ToString() + "x"))));
			
			_sut.Process(new Message(".in", "hello"));

            Assert.IsTrue(_are.WaitOne(1000));
			Assert.AreEqual(".out", _result.Port.Fullname);
			Assert.AreEqual("hellox", _result.Data.ToString());
		}
	}
}

