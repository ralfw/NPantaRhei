using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

using npantarhei.runtime;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.tests
{
	[TestFixture()]
	public class sample_ToDictionary
	{
		[Test()]
		public void Run_flat()
		{	
			var fr = new FlowRuntime();
			
			/*
			 * (.in) -> (Split) -> (Map) -> (Build) -> (.out)
			 */
			fr.AddStream(new Stream(".in", "Split"));
			fr.AddStream(new Stream("Split", "Map"));
			fr.AddStream(new Stream("Map", "Build"));
			fr.AddStream(new Stream("Build", ".out"));
			
			var foc = new FlowOperationContainer();
			foc.RegisterFunction<string,IEnumerable<string>>("Split", 
													 configuration => configuration.Split(new[]{';'}, StringSplitOptions.RemoveEmptyEntries));
			foc.RegisterAction<IEnumerable<string>, IEnumerable<KeyValuePair<string,string>>>("Map", Map);
			foc.RegisterFunction<IEnumerable<KeyValuePair<string,string>>, Dictionary<string,string>>("Build", Build);
			fr.AddOperations(foc.Operations);
			
			Dictionary<string,string> dict = null;
			fr.Result += _ => dict = (Dictionary<string,string>)_.Data;
			
			fr.ProcessSync(new Message(".in", "a=1;b=2"));
			
			Assert.AreEqual(2, dict.Count);
			Assert.AreEqual("1", dict["a"]);
			Assert.AreEqual("2", dict["b"]);
		}
		
		
		[Test()]
		public void Run_nested()
		{	
			var fr = new FlowRuntime();
			
			/*
			 * (.in) -> (ToDict) -> (.out).
			 * ToDict {
			 * 		(.in) -> (Split) -> (Map) -> (Build) -> (.out)
			 * }
			 */
			fr.AddStream(new Stream(".in", "ToDict/Split"));
			fr.AddStream(new Stream("ToDict/Split", "ToDict/Map"));
			fr.AddStream(new Stream("ToDict/Map", "ToDict/Build"));
			fr.AddStream(new Stream("ToDict/Build", ".out"));
			
			var foc = new FlowOperationContainer();
			foc.RegisterFunction<string,IEnumerable<string>>("Split", 
													 configuration => configuration.Split(new[]{';'}, StringSplitOptions.RemoveEmptyEntries));
			foc.RegisterAction<IEnumerable<string>, IEnumerable<KeyValuePair<string,string>>>("Map", Map);
			foc.RegisterFunction<IEnumerable<KeyValuePair<string,string>>, Dictionary<string,string>>("Build", Build);
			fr.AddOperations(foc.Operations);
			
			Dictionary<string,string> dict = null;
			fr.Result += _ => dict = (Dictionary<string,string>)_.Data;
			
			fr.ProcessSync(new Message(".in", "a=1;b=2"));
			Assert.AreEqual(2, dict.Count);
			Assert.AreEqual("1", dict["a"]);
			Assert.AreEqual("2", dict["b"]);
			
			fr.ProcessSync(new Message(".in", "x=9;y=8;z=7"));
			Assert.AreEqual(3, dict.Count);
			Assert.AreEqual("9", dict["x"]);
			Assert.AreEqual("8", dict["y"]);
			Assert.AreEqual("7", dict["z"]);
		}
		
		
		[Test()]
		public void Run_async()
		{	
			var fr = new FlowRuntime();
			
			/*
			 * (.in) -> (ToDict) -> (.out).
			 * ToDict {
			 * 		(.in) -> (Split) -> (Map) -> (Build) -> (.out)
			 * }
			 */
			fr.AddStream(new Stream(".in", "ToDict/Split"));
			fr.AddStream(new Stream("ToDict/Split", "ToDict/Map"));
			fr.AddStream(new Stream("ToDict/Map", "ToDict/Build"));
			fr.AddStream(new Stream("ToDict/Build", ".out"));
			
			var foc = new FlowOperationContainer();
			foc.RegisterFunction<string,IEnumerable<string>>("Split", 
													 configuration => configuration.Split(new[]{';'}, StringSplitOptions.RemoveEmptyEntries));
			foc.RegisterAction<IEnumerable<string>, IEnumerable<KeyValuePair<string,string>>>("Map", Map);
			foc.RegisterFunction<IEnumerable<KeyValuePair<string,string>>, Dictionary<string,string>>("Build", Build);
			fr.AddOperations(foc.Operations);
			
			Dictionary<string,string> dict = null;
			AutoResetEvent are = new AutoResetEvent(false);
			fr.Result += _ => {
				dict = (Dictionary<string,string>)_.Data;
				Console.WriteLine("Runtime thread: {0}", Thread.CurrentThread.GetHashCode());
				are.Set();
			};
			
			Console.WriteLine("Caller thread: {0}", Thread.CurrentThread.GetHashCode());
			fr.Start();
			try
			{
				fr.ProcessAsync(new Message(".in", "a=1;b=2"));
				Assert.IsTrue(are.WaitOne(200));
				Assert.AreEqual(2, dict.Count);
				Assert.AreEqual("1", dict["a"]);
				Assert.AreEqual("2", dict["b"]);
				
				fr.ProcessAsync(new Message(".in", "x=9;y=8;z=7"));
				Assert.IsTrue(are.WaitOne(200));
				Assert.AreEqual(3, dict.Count);
				Assert.AreEqual("9", dict["x"]);
				Assert.AreEqual("8", dict["y"]);
				Assert.AreEqual("7", dict["z"]);
			}
			finally {
				fr.Stop();
			}
		}
		
		
		void Map(IEnumerable<string> assignments, Action<IEnumerable<KeyValuePair<string,string>>> outputCont)
		{	
			var kvPairs = assignments.Select(a => {
												  		var parts = a.Split('=');
														return new KeyValuePair<string,string>(parts[0], parts[1]);
						  				          });
			
			outputCont(kvPairs);
		}
		
		Dictionary<string,string> Build(IEnumerable<KeyValuePair<string,string>> kvPairs)
		{
			var dict = new Dictionary<string,string>();
			
			kvPairs.Aggregate(dict, (d, kvp) => {
				d.Add(kvp.Key, kvp.Value);
				return d;
			});
			
			return dict;
		}
	}
}

