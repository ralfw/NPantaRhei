using System;
using NUnit.Framework;

using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.tests
{
	[TestFixture()]
	public class test_Port
	{
		[Test()]
		public void All_properties()
		{
			var sut = new Port("a/b/o.p");
			Assert.AreEqual("a/b/o.p", sut.Fullname);
			Assert.AreEqual("o", sut.OperationName);
			Assert.AreEqual("p", sut.Name);
			Assert.AreEqual("a/b", sut.Path);
		}
		
		[Test]
		public void Backslash_gets_replaced_with_fwdslash()
		{
			var sut = new Port("a\\b\\o.p");
			Assert.AreEqual("a/b/o.p", sut.Fullname);
		}
		
		[Test]
		public void Check_for_operation_port()
		{
			var sut = new Port("opname.portname");
			Assert.IsTrue(sut.IsOperationPort);
			
			sut = new Port(".portname");
			Assert.IsFalse(sut.IsOperationPort);
		}
	}
}

