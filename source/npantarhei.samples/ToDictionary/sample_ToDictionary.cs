using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using npantarhei.runtime;
using npantarhei.runtime.messagetypes;

namespace ToDictionary
{
	[TestFixture()]
	public class sample_ToDictionary
	{
		[Test()]
		public void Run_flat()
		{
			using (var fr = new FlowRuntime())
			{
				/*
				 * (.in) -> (Split) -> (Map) -> (Build) -> (.out)
				 */
				var frc = new FlowRuntimeConfiguration();
				frc.AddStream(new Stream(".in", "Split"));
				frc.AddStream(new Stream("Split", "Map"));
				frc.AddStream(new Stream("Map", "Build"));
				frc.AddStream(new Stream("Build", ".out"));

				frc.AddFunc<string, IEnumerable<string>>("Split", configuration => configuration.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries));
				frc.AddAction<IEnumerable<string>, IEnumerable<KeyValuePair<string, string>>>("Map", Map);
				frc.AddFunc<IEnumerable<KeyValuePair<string, string>>, Dictionary<string, string>>("Build", Build);
				fr.Configure(frc);

				Dictionary<string, string> dict = null;
				var are = new AutoResetEvent(false);
				fr.Result += _ =>
								{
									dict = (Dictionary<string, string>) _.Data;
									are.Set();
								};

				fr.Process(new Message(".in", "a=1;b=2"));

				Assert.IsTrue(are.WaitOne(500));
				Assert.AreEqual(2, dict.Count);
				Assert.AreEqual("1", dict["a"]);
				Assert.AreEqual("2", dict["b"]);
			}
		}
		
		
		[Test()]
		public void Run_nested()
		{
			using (var fr = new FlowRuntime())
			{
				/*
				 * (.in) -> (ToDict) -> (.out).
				 * ToDict {
				 * 		(.in) -> (Split) -> (Map) -> (Build) -> (.out)
				 * }
				 */
				var frc = new FlowRuntimeConfiguration();
				frc.AddStream(new Stream(".in", "ToDict/Split"));
				frc.AddStream(new Stream("ToDict/Split", "ToDict/Map"));
				frc.AddStream(new Stream("ToDict/Map", "ToDict/Build"));
				frc.AddStream(new Stream("ToDict/Build", ".out"));

				frc.AddFunc<string, IEnumerable<string>>("Split", configuration => configuration.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries));
				frc.AddAction<IEnumerable<string>, IEnumerable<KeyValuePair<string, string>>>("Map", Map);
				frc.AddFunc<IEnumerable<KeyValuePair<string, string>>, Dictionary<string, string>>("Build", Build);
				fr.Configure(frc);

				Dictionary<string, string> dict = null;
				var are = new AutoResetEvent(false);
				fr.Result += _ =>
								{
									dict = (Dictionary<string, string>) _.Data;
									are.Set();
								};

				fr.Process(new Message(".in", "a=1;b=2"));
				Assert.IsTrue(are.WaitOne(500));
				Assert.AreEqual(2, dict.Count);
				Assert.AreEqual("1", dict["a"]);
				Assert.AreEqual("2", dict["b"]);

				fr.Process(new Message(".in", "x=9;y=8;z=7"));
				Assert.IsTrue(are.WaitOne(500));
				Assert.AreEqual(3, dict.Count);
				Assert.AreEqual("9", dict["x"]);
				Assert.AreEqual("8", dict["y"]);
				Assert.AreEqual("7", dict["z"]);
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

