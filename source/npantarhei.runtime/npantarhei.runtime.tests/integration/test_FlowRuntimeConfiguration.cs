using System;
using System.Linq;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.tests.integration
{
	[TestFixture()]
	public class test_FlowRuntimeConfiguration
	{	
		[Test()]
		public void Register_func()
		{
			var sut = new FlowRuntimeConfiguration();
			
			sut.AddFunc<string, int>("f", s => s.Length);
			
			var op = sut.Operations.First();
						
			IMessage result = null;
			op.Implementation(new Message("f", "hello"), _ => result = _, null);
		
			Assert.AreEqual(5, (int)result.Data);
		}
		
		[Test]
		public void Func_op_adapter_ignores_input_port_and_uses_opname_as_output_port()
		{
			var sut = new FlowRuntimeConfiguration();
			
			sut.AddFunc<string, string>("opname", s => s);
			
			var op = sut.Operations.First();
			Assert.AreEqual("opname", op.Name);
						
			IMessage result = null;
			op.Implementation(new Message("xyz.someport", "x"), _ => result = _, null);
			
			Assert.AreEqual("opname", result.Port.Fullname);
		}
		
		
		[Test()]
		public void Register_procedure()
		{
			var sut = new FlowRuntimeConfiguration();
			
			var result = "";
			sut.AddAction<string>("p", s => result = s, true);
			
			var op = sut.Operations.First();

			bool continuationCalled = false;
			op.Implementation(new Message("f", "hello"), _ => continuationCalled = true, null);
		
			Assert.AreEqual("hello", result);
			Assert.IsTrue(continuationCalled);
		}
		
		
		[Test()]
		public void Register_continuation_procedure()
		{
			var sut = new FlowRuntimeConfiguration();
			
			sut.AddAction<string, int>("p", Process_with_continuation);
			
			var op = sut.Operations.First();
						
			IMessage result = null;
			op.Implementation(new Message("f", "hello"), _ => result = _, null);
		
			Assert.AreEqual(5, (int)result.Data);
		}
		
		void Process_with_continuation(string s, Action<int> outputCont) { outputCont(s.Length);	}
		
		[Test]
		public void Continuation_proc_adapter_ignores_input_port_and_uses_opname_as_output_port()
		{
			var sut = new FlowRuntimeConfiguration();
			
			sut.AddAction<string, int>("opname", Process_with_continuation);
			
			var op = sut.Operations.First();
			Assert.AreEqual("opname", op.Name);
						
			IMessage result = null;
			op.Implementation(new Message("xyz.someport", "x"), _ => result = _, null);
			
			Assert.AreEqual("opname", result.Port.Fullname);
		}

		[Test]
		public void Procedure_with_2_continuations()
		{
			var sut = new FlowRuntimeConfiguration();

			sut.AddAction<int, string, bool>("opname", Bifurcate);

			var op = sut.Operations.First();

			IMessage result0 = null;
			IMessage result1 = null;
			op.Implementation(new Message("x", 2), msg =>
													   {
														   if (msg.Port.Name == "out0") result0 = msg;
														   if (msg.Port.Name == "out1") result1 = msg;
													   }, null);

			Assert.AreEqual("2x", (string)result0.Data);
			Assert.IsTrue((bool)result1.Data);
		}

		void Bifurcate(int i, Action<string> continueWith0, Action<bool> continueWith1)
		{
			continueWith0(i + "x");
			continueWith1(i%2 == 0);
		}
	}
}

