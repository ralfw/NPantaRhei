using System;
using System.Collections.Generic;
using NUnit.Framework;

using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.operations;

namespace npantarhei.runtime.tests.operations
{
	[TestFixture]
	public class test_Create_task
	{
		[Test]
		public void Operation_name_with_portname()
		{
			var sut = new Create_task();
			
			var operations = new Dictionary<string, IOperation>();
			var operation = new Operation("x", null);
			operations.Add(operation.Name, operation);
			sut.Inject(operations);
			
			Task result = null;
			sut.Result += _ => result = _;
			
			var msg = new Message("x.In", null);
			sut.Process(msg);
			
			Assert.AreSame(msg, result.Message);
			Assert.AreSame(operation, result.Operation);
		}
		
		[Test]
		public void Operation_name_without_portname()
		{
			var sut = new Create_task();
			
			var operations = new Dictionary<string, IOperation>();
			var operation = new Operation("y", null);
			operations.Add(operation.Name, operation);
			sut.Inject(operations);
			
			Task result = null;
			sut.Result += _ => result = _;
			
			var msg = new Message("y", null);
			sut.Process(msg);
			
			Assert.AreSame(msg, result.Message);
			Assert.AreSame(operation, result.Operation);
		}
	}
}

