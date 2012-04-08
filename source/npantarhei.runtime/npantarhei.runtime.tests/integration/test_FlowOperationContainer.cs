using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using npantarhei.runtime;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.tests
{
	[TestFixture()]
	public class test_FlowOperationContainer
	{	
		[Test()]
		public void Register_func()
		{
			var sut = new FlowOperationContainer();
			
			sut.AddFunc<string, int>("f", s => s.Length);
			
			var op = sut.Operations.First();
						
			IMessage result = null;
			op.Implementation(new Message("f", "hello"), _ => result = _);
		
			Assert.AreEqual(5, (int)result.Data);
		}
		
		[Test]
		public void Func_op_adapter_ignores_input_port_and_uses_opname_as_output_port()
		{
			var sut = new FlowOperationContainer();
			
			sut.AddFunc<string, string>("opname", s => s);
			
			var op = sut.Operations.First();
			Assert.AreEqual("opname", op.Name);
						
			IMessage result = null;
			op.Implementation(new Message("xyz.someport", "x"), _ => result = _);
			
			Assert.AreEqual("opname", result.Port.Fullname);
		}
		
		
		[Test()]
		public void Register_procedure()
		{
			var sut = new FlowOperationContainer();
			
			var result = "";
			sut.AddAction<string>("p", s => result = s);
			
			var op = sut.Operations.First();
						
			op.Implementation(new Message("f", "hello"), null);
		
			Assert.AreEqual("hello", result);
		}
		
		
		[Test()]
		public void Register_continuation_procedure()
		{
			var sut = new FlowOperationContainer();
			
			sut.AddAction<string, int>("p", Process_with_continuation);
			
			var op = sut.Operations.First();
						
			IMessage result = null;
			op.Implementation(new Message("f", "hello"), _ => result = _);
		
			Assert.AreEqual(5, (int)result.Data);
		}
		
		void Process_with_continuation(string s, Action<int> outputCont) { outputCont(s.Length);	}
		
		[Test]
		public void Continuation_proc_adapter_ignores_input_port_and_uses_opname_as_output_port()
		{
			var sut = new FlowOperationContainer();
			
			sut.AddAction<string, int>("opname", Process_with_continuation);
			
			var op = sut.Operations.First();
			Assert.AreEqual("opname", op.Name);
						
			IMessage result = null;
			op.Implementation(new Message("xyz.someport", "x"), _ => result = _);
			
			Assert.AreEqual("opname", result.Port.Fullname);
		}
	}
}

