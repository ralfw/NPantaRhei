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
			var sut = new Port("a/b/o#42.p");
			Assert.AreEqual("a/b/o#42.p", sut.Fullname);
            Assert.AreEqual("o#42", sut.InstanceName);
            Assert.AreEqual("42", sut.InstanceNumber);
			Assert.AreEqual("o", sut.OperationName);
			Assert.AreEqual("p", sut.Name);
			Assert.AreEqual("a/b", sut.Path);

            sut = new Port("a/b/o.p");
            Assert.AreEqual("o", sut.InstanceName);
            Assert.AreEqual("", sut.InstanceNumber);
            Assert.AreEqual("o", sut.OperationName);
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
			Assert.IsTrue(sut.HasOperation);
			
			sut = new Port(".portname");
			Assert.IsFalse(sut.HasOperation);
		}
	}
}

