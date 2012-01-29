using System;
using System.Collections.Generic;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.operations;
using NUnit.Framework;

namespace npantarhei.runtime.tests.operations
{
	[TestFixture()]
	public class test_Execute_task
	{
		[Test()]
		public void Operation_with_single_output()
		{
			var sut = new Execute_task();
			
			var msg = new Message("x.in", "hello");
			var task = new Task(msg, 
								new Operation("x", (input, outputCont) => {
																		  		outputCont(input);
																		  }));
			
			IMessage result = null;
			sut.Result += _ => result = _;

			sut.Process(task);
			
			Assert.AreSame(msg, result);
		}
		
		[Test()]
		public void Operation_with_multiple_outputs()
		{
			var sut = new Execute_task();
			
			var msg = new Message("x.in", "hello");
			var task = new Task(msg, 
								new Operation("x", (input, outputCont) => {
																		  		outputCont(new Message("1", input.Data.ToString() + "x"));
																				outputCont(new Message("2", input.Data.ToString() + "y"));
																		  }));
			
			var results = new List<IMessage>();
			sut.Result += _ => results.Add(_);

			sut.Process(task);
			
			Assert.AreEqual("1", results[0].Port.Fullname);
			Assert.AreEqual("hellox", results[0].Data.ToString());
			Assert.AreEqual("2", results[1].Port.Fullname);
			Assert.AreEqual("helloy", results[1].Data.ToString());
		}
		
		[Test]
		public void Output_is_in_same_context_as_input()
		{
			var sut = new Execute_task();
			
			var msg = new Message("a/b/x.in", "hello");
			var task = new Task(msg, 
								new Operation("x", (input, outputCont) => {
																		  		outputCont(new Message("x.out", input.Data.ToString() + "x"));
																		  }));
			var results = new List<IMessage>();
			sut.Result += _ => results.Add(_);

			sut.Process(task);
			Assert.AreEqual("hellox", results[0].Data.ToString());
			Assert.AreEqual("a/b/x.out", results[0].Port.Fullname);
		}
	}
}

