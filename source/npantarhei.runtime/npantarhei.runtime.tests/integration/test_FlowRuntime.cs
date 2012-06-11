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

			_are = new AutoResetEvent(false);
			_result = null;
			_sut.Result += _ =>
								{
									_result = _;
									_are.Set();
								};
		}

		[TearDown]
		public void TearDown()
		{
			_sut.Dispose();
		}


		[Test]
		public void No_processing_Just_redirect_input_to_output()
		{
            var frc = new FlowRuntimeConfiguration().AddStream(new Stream(".in", ".out"));
            _sut.Configure(frc);

			_sut.Process(new Message(".in", "hello"));

			Assert.IsTrue(_are.WaitOne(1000));
			Assert.AreEqual(".out", _result.Port.Fullname);
			Assert.AreEqual("hello", _result.Data.ToString());
		}
		
		[Test]
		public void Single_operation()
		{
            var frc = new FlowRuntimeConfiguration()
			                .AddStream(new Stream(".in", "A.in"))
			                .AddStream(new Stream("A.out", ".out"))
			
			                .AddOperation(new Operation("A", (input, outputCont, _) => outputCont(new Message("A.out", input.Data.ToString() + "x"))));
            _sut.Configure(frc);

			_sut.Process(new Message(".in", "hello"));

			Assert.IsTrue(_are.WaitOne(1000));
			Assert.AreEqual(".out", _result.Port.Fullname);
			Assert.AreEqual("hellox", _result.Data.ToString());
		}
		
		[Test]
		public void Sequence_of_operations()
		{
            var frc = new FlowRuntimeConfiguration()
			                .AddStream(new Stream(".in", "A.in"))
			                .AddStream(new Stream("A.out", "B.in"))
			                .AddStream(new Stream("B.out", ".out"))
			
			                .AddOperation(new Operation("A", (input, outputCont, _) => outputCont(new Message("A.out", input.Data.ToString() + "x"))))
			                .AddOperation(new Operation("B", (input, outputCont, _) => outputCont(new Message("B.out", input.Data.ToString() + "y"))));
			_sut.Configure(frc);

			_sut.Process(new Message(".in", "hello"));

			Assert.IsTrue(_are.WaitOne(1000));
			Assert.AreEqual(".out", _result.Port.Fullname);
			Assert.AreEqual("helloxy", _result.Data.ToString());
		}
		
		[Test]
		public void Output_with_fan_out()
		{
            var frc = new FlowRuntimeConfiguration()
                            .AddStream(new Stream(".in", "A.in"))
                            .AddStream(new Stream(".in", "B.in"))
                            .AddStream(new Stream("A.out", ".out"))
                            .AddStream(new Stream("B.out", ".out"))

                            .AddOperation(new Operation("A", (input, outputCont, _) => outputCont(new Message("A.out", input.Data.ToString() + "x"))))
                            .AddOperation(new Operation("B", (input, outputCont, _) => outputCont(new Message("B.out", input.Data.ToString() + "y"))));
			_sut.Configure(frc);

			var results = new List<IMessage>();
			var n = 0;
			_sut.Result += _ =>
							{
								results.Add(_);
								n++;
								if (n==2) _are.Set();
							};
			
			_sut.Process(new Message(".in", "hello"));

			Assert.IsTrue(_are.WaitOne(4000));
			Assert.That(results.Select(m => m.Data.ToString()).ToArray(), Is.EquivalentTo(new[]{"hellox", "helloy"}));
		}
		
		[Test]
		public void Ports_with_context()
		{
            var frc = new FlowRuntimeConfiguration()
                            .AddStream(new Stream(".in", "A/B/X.in"))
                            .AddStream(new Stream("A/B/X.out", ".out"))

                            .AddOperation(new Operation("X", (input, outputCont, _) => outputCont(new Message("A/B/X.out", input.Data.ToString() + "x"))));
            _sut.Configure(frc);
			
			_sut.Process(new Message(".in", "hello"));

			Assert.IsTrue(_are.WaitOne(1000));
			Assert.AreEqual(".out", _result.Port.Fullname);
			Assert.AreEqual("hellox", _result.Data.ToString());
		}

		[Test]
		public void Process_exception_in_operation()
		{
            var frc = new FlowRuntimeConfiguration()
			                .AddStream(new Stream(".process", "ThrowEx.in"))
			                .AddStream(new Stream("ThrowEx.out", ".out"))

                            .AddOperation(new Operation("ThrowEx", (input, outputCont, _) => { throw new NotImplementedException("xxx"); }));
            _sut.Configure(frc);

			FlowRuntimeException ex = null;
			_sut.UnhandledException += _ =>
											{
												ex = _;
												_are.Set();
											};

			_sut.Process(new Message(".process", "hello"));

			Assert.IsTrue(_are.WaitOne(1000));
			Assert.AreEqual("xxx", ex.InnerException.Message);
			Assert.AreEqual("ThrowEx.in", ex.Context.Port.Fullname);
		}

		[Test]
		public void Process_exception_in_runtime()
		{
		    _sut.Configure(new FlowRuntimeConfiguration().AddStream(new Stream(".process", "unknown op")));

			FlowRuntimeException ex = null;
			_sut.UnhandledException += _ =>
											{
												ex = _;
												_are.Set();
											};

			_sut.Process(new Message(".process", "hello"));

			Assert.IsTrue(_are.WaitOne(1000));
			Assert.AreEqual(".process", ex.Context.Port.Fullname);
			Assert.IsTrue(ex.InnerException.Message.IndexOf("unknown op") > 0);
		}

		[Test, Explicit]
		// Watch for exception output in test window. It should show an exception reporting a missing exception handler.
		public void Report_missing_exception_handler()
		{
		    var frc = new FlowRuntimeConfiguration()
		                    .AddStream(new Stream(".process", "ThrowEx.in"))
		                    .AddStream(new Stream("ThrowEx.out", ".out"))

                            .AddOperation(new Operation("ThrowEx", (input, outputCont, _) => { throw new NotImplementedException("xxx"); }));
            _sut.Configure(frc);

			_sut.Process(new Message(".process", "hello"));
		}

		[Test]
		public void Log_messages()
		{
            var frc = new FlowRuntimeConfiguration()
			                .AddStream(new Stream(".in", "A.in"))
			                .AddStream(new Stream("A.out", "B.in"))
			                .AddStream(new Stream("B.out", ".out"))

			                .AddOperation(new Operation("A", (input, outputCont, _) => outputCont(new Message("A.out", input.Data.ToString() + "x"))))
			                .AddOperation(new Operation("B", (input, outputCont, _) => outputCont(new Message("B.out", input.Data.ToString() + "y"))));
            _sut.Configure(frc);

			var messages = new List<string>();
			_sut.Message += _ => messages.Add(_.Port.Fullname);

			_sut.Process(new Message(".in", "hello"));

			Assert.IsTrue(_are.WaitOne(1000));
			Assert.That(messages.ToArray(), Is.EquivalentTo(new[]{"A.in", "B.in", ".out"}));
		}


		[Test]
		public void Multiple_instances_of_same_op_in_one_flow()
		{
            var frc = new FlowRuntimeConfiguration()
                            .AddStream(new Stream(".in", "A#1"))
                            .AddStream(new Stream("A#1.out", "B"))
                            .AddStream(new Stream("B", "A#2"))
                            .AddStream(new Stream("A#2.out", ".out"))

                            .AddOperation(new Operation("A", (input, outputCont, _) => outputCont(new Message("A.out", input.Data.ToString() + "x"))))
                            .AddOperation(new Operation("B", (input, outputCont, _) => outputCont(new Message("B", input.Data.ToString() + "y"))));
            _sut.Configure(frc);

			var messages = new List<IMessage>();
			_sut.Message += messages.Add;

			_sut.Process(new Message(".in", "hello"));

			Assert.IsTrue(_are.WaitOne(1000));
			Assert.That(messages.Select(m => m.Port.Fullname).ToArray(), Is.EquivalentTo(new[] { "A#1", "B", "A#2", ".out" }));
			Assert.AreEqual("helloxyx", (string)messages.Last().Data);
		}

		[Test]
		public void Nested_flows()
		{
            var frc = new FlowRuntimeConfiguration()
			                .AddStreamsFrom(@"
								                .in, f.in // flow with explicit ports
								                f.out, .out

								                f
								                .in, g // flow without explicit ports
								                g, .out

								                g
								                ., a // port without a name
								                a, .
								                ")

			                .AddOperation(new Operation("a", (input, outputCont, _) => outputCont(new Message("a", input.Data.ToString() + "x"))));
            _sut.Configure(frc);

		    _sut.Message += Console.WriteLine;

			_sut.Process(new Message(".in", "hello"));

			IMessage result = null;
			Assert.IsTrue(_sut.WaitForResult(500, _ => result = _));
			Assert.AreEqual("hellox", (string)result.Data);
		}
	}
}

