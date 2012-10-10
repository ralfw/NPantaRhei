using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.operations;

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


		[Test]
		public void Select_operation()
		{
			var config = new FlowRuntimeConfiguration()
								.AddFunc<int>("f", () => Thread.CurrentThread.GetHashCode())
								.AddAction("x", () => { })
								.AddStream(".in", "f")
								.AddStream("f", ".out")
								["f"].MakeAsync();

			using(var fr = new FlowRuntime(config, new Schedule_for_sync_depthfirst_processing()))
			{
				fr.Process(".in", "x");

				IMessage result = null;
				fr.WaitForResult(500, _ => result = _);

				Assert.AreNotEqual(Thread.CurrentThread.GetHashCode(), result.Data);
			}
		}


		[Test]
		public void Register_instance_operations()
		{
			var iop = new MethodOperations();
			var config = new FlowRuntimeConfiguration()
				.AddInstanceOperations(iop)
				.AddStreamsFrom(@"
									/
									.inProcedure, Procedure
									.inProcedureV, ProcedureV
									.inProcedureC, ProcedureC
										ProcedureC, .outProcedureC
									.inProcedureVC, ProcedureVC
										ProcedureVC, .outProcedureVC
									.inProcedureVCC, ProcedureVCC
										ProcedureVCC.out0, .outProcedureVCC0
										ProcedureVCC.out1, .outProcedureVCC1
									.inFunction, Function
										Function, .outFunction
									.inFunctionV, FunctionV
										FunctionV, .outFunctionV
								 ");

			using (var fr = new FlowRuntime(config, new Schedule_for_sync_depthfirst_processing()))
			{
				var results = new List<IMessage>();
				fr.Result += results.Add;

				fr.Process(".inProcedure");
				Assert.AreEqual(99, iop.Result);

				fr.Process(".inProcedureV", 42);
				Assert.AreEqual(42, iop.Result);

				fr.Process(".inProcedureC");
				Assert.AreEqual(99, results[0].Data);

				results.Clear();
				fr.Process(".inProcedureVC", 99);
				Assert.AreEqual(100, results[0].Data);

				results.Clear();
				fr.Process(".inProcedureVCC", 99);
				Assert.That(results.Select(r => r.Data).ToArray(), Is.EquivalentTo(new object[] {100, "101"}));

				results.Clear();
				fr.Process(".inFunction");
				Assert.AreEqual(99, results[0].Data);

				results.Clear();
				fr.Process(".inFunctionV", 99);
				Assert.AreEqual(100, results[0].Data);
			}
		}

		[Test]
		public void Register_static_operations()
		{
			var iop = new MethodOperations();
			var config = new FlowRuntimeConfiguration()
				.AddStaticOperations(typeof(MethodOperations))
				.AddStreamsFrom(@"
									/
									.inProcedureVC, SProcedureVC
										SProcedureVC, .outProcedureVC
									.inFunctionV, SFunctionV
										SFunctionV, .outFunctionV
								 ");

			using(var fr = new FlowRuntime(config, new Schedule_for_sync_depthfirst_processing()))
			{
				var results = new List<IMessage>();
				fr.Result += results.Add;

				fr.Process(".inProcedureVC", 99);
				Assert.AreEqual(101, results[0].Data);

				results.Clear();
				fr.Process(".inFunctionV", 99);
				Assert.AreEqual(101, results[0].Data);
			}
		}


		class MethodOperations
		{
			public int Result;

			public void Procedure() { Result = 99; }
			public void ProcedureV(int value) { Result = value; }
			public void ProcedureC(Action<int> continueWith) { continueWith(99); }
			public void ProcedureVC(int value, Action<int> continueWith) { continueWith(value + 1); }
			public void ProcedureVCC(int value, Action<int> continueWith0, Action<string> continueWith1) { continueWith0(value + 1); continueWith1((value+2).ToString()); }

			public int Function() { return 99; }
			public int FunctionV(int value) { return value + 1; }


			public static void SProcedureVC(int value, Action<int> continueWith) { continueWith(value+2); }
			public static int SFunctionV(int value) { return value + 2; }
		}
	}
}

