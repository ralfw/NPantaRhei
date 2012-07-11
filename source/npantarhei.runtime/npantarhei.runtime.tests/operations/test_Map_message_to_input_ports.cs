using System;
using System.Collections.Generic;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.operations;

namespace npantarhei.runtime.tests.operations
{
	[TestFixture]
	public class test_Map_message_to_input_ports
	{	
		[Test]
		public void Maps_to_single_input_port()
		{
			var sut = new Map_message_to_input_ports();
			
			List<IStream> streams = new List<IStream>();
			streams.Add(new Stream("a", "a1"));
			streams.Add(new Stream("b", "b1"));
			streams.Add(new Stream("c", "c1"));
			sut.Inject(streams);
			
			string result = null;
			sut.Result += _ => result = _.Port.Fullname;
			
			sut.Process(new Message("b", "some data"));
			
			Assert.AreEqual("b1", result);
		}
		
		[Test]
		public void Data_gets_copied()
		{
			var sut = new Map_message_to_input_ports();
			
			List<IStream> streams = new List<IStream>();
			streams.Add(new Stream("a", "a1"));
			sut.Inject(streams);
			
			IMessage result = null;
			sut.Result += _ => result = _;
			
			sut.Process(new Message("a", "some data"));
			
			Assert.AreEqual("some data", result.Data);			
		}
		
		[Test]
		public void Maps_to_multiple_input_ports()
		{
			var sut = new Map_message_to_input_ports();
			
			var streams = new List<IStream>();
			streams.Add(new Stream("a", "a1"));
			streams.Add(new Stream("b", "b1"));
			streams.Add(new Stream("b", "b2"));
			streams.Add(new Stream("c", "c1"));
			sut.Inject(streams);
			
			var results = new List<string>();
			sut.Result += _ => results.Add(_.Port.Fullname);
			
			sut.Process(new Message("b", "some data"));
			
			Assert.That(results, Is.EquivalentTo(new[] {"b1", "b2"}));
		}
		
		[Test]
		public void Unknow_port_to_map_causes_no_error()
		{
			var sut = new Map_message_to_input_ports();
			
			var streams = new List<IStream>();
			sut.Inject(streams);

            var results = new List<string>();
            sut.Result += _ => results.Add(_.Port.Fullname);

		    sut.Process(new Message("b", "some data"));
            
            Assert.AreEqual(0, results.Count);
		}
	}
}

