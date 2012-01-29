using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using npantarhei.runtime;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.tests.integration
{
	[TestFixture]
	public class test_FlowRuntime
	{
		[Test]
		public void No_processing_Just_redirect_input_to_output()
		{
			var sut = new FlowRuntime();
			
			sut.AddStream(new Stream(".in", ".out"));
			
			IMessage result = null;
			sut.Result += _ => result = _;
			
			sut.ProcessSync(new Message(".in", "hello"));
			
			Assert.AreEqual(".out", result.Port.Fullname);
			Assert.AreEqual("hello", result.Data.ToString());
		}
		
		[Test]
		public void Single_operation()
		{
			var sut = new FlowRuntime();
			
			sut.AddStream(new Stream(".in", "A.in"));
			sut.AddStream(new Stream("A.out", ".out"));
			
			sut.AddOperation(new Operation("A", (input, outputCont) => outputCont(new Message("A.out", input.Data.ToString() + "x"))));
			
			IMessage result = null;
			sut.Result += _ => result = _;
			
			sut.ProcessSync(new Message(".in", "hello"));
			
			Assert.AreEqual(".out", result.Port.Fullname);
			Assert.AreEqual("hellox", result.Data.ToString());
		}
		
		[Test]
		public void Sequence_of_operations()
		{
			var sut = new FlowRuntime();
			
			sut.AddStream(new Stream(".in", "A.in"));
			sut.AddStream(new Stream("A.out", "B.in"));
			sut.AddStream(new Stream("B.out", ".out"));
			
			sut.AddOperation(new Operation("A", (input, outputCont) => outputCont(new Message("A.out", input.Data.ToString() + "x"))));
			sut.AddOperation(new Operation("B", (input, outputCont) => outputCont(new Message("B.out", input.Data.ToString() + "y"))));
			
			IMessage result = null;
			sut.Result += _ => result = _;
			
			sut.ProcessSync(new Message(".in", "hello"));
			
			Assert.AreEqual(".out", result.Port.Fullname);
			Assert.AreEqual("helloxy", result.Data.ToString());
		}
		
		[Test]
		public void Output_with_fan_out()
		{
			var sut = new FlowRuntime();
			
			sut.AddStream(new Stream(".in", "A.in"));
			sut.AddStream(new Stream(".in", "B.in"));
			sut.AddStream(new Stream("A.out", ".out"));
			sut.AddStream(new Stream("B.out", ".out"));
			
			sut.AddOperation(new Operation("A", (input, outputCont) => outputCont(new Message("A.out", input.Data.ToString() + "x"))));
			sut.AddOperation(new Operation("B", (input, outputCont) => outputCont(new Message("B.out", input.Data.ToString() + "y"))));
			
			var results = new List<IMessage>();
			sut.Result += _ => results.Add(_);
			
			sut.ProcessSync(new Message(".in", "hello"));
			
			Assert.That(results.Select(m => m.Data.ToString()).ToArray(), Is.EquivalentTo(new[]{"hellox", "helloy"}));
		}
		
		[Test]
		public void Ports_with_context()
		{
			var sut = new FlowRuntime();
			
			sut.AddStream(new Stream(".in", "A/B/X.in"));
			sut.AddStream(new Stream("A/B/X.out", ".out"));
			
			sut.AddOperation(new Operation("X", (input, outputCont) => outputCont(new Message("A/B/X.out", input.Data.ToString() + "x"))));
			
			IMessage result = null;
			sut.Result += _ => result = _;
			
			sut.ProcessSync(new Message(".in", "hello"));
			
			Assert.AreEqual(".out", result.Port.Fullname);
			Assert.AreEqual("hellox", result.Data.ToString());
		}
	}
}

