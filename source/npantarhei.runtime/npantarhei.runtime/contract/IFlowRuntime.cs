using System;
using System.Collections.Generic;
using System.Reflection;

namespace npantarhei.runtime.contract
{
	public interface IFlowRuntime : IDisposable
	{
		void Process(string portname);
		void Process(string portname, object data);
		void Process(IMessage message);

		event Action<IMessage> Message;
		event Action<FlowRuntimeException> UnhandledException;
		event Action<IMessage> Result;

		bool WaitForResult();
		bool WaitForResult(int milliseconds);
		bool WaitForResult(Action<IMessage> processResult);
		bool WaitForResult(int milliseconds, Action<IMessage> processResult);
		
		void AddStream(IStream stream);
		void AddStream(string fromPortName, string toPortName);
		void AddStreams(IEnumerable<IStream> streams);
		void AddStreamsFrom(string resourceName, Assembly resourceAssembly);
		void AddStreamsFrom(IEnumerable<string> lines);
		void AddStreamsFrom(string text);

		void AddOperation(IOperation operation);
		void AddOperations(IEnumerable<IOperation> operations);

		void AddFlow(IFlow flow);

		Action CreateEventProcessor(string portname);
		Action<T> CreateEventProcessor<T>(string portname);
		
		//void Pause();
		//void BreakOnPort(string includeFilter);
		//void BreakOnMessage(Func<IMessage, bool> includeFilter);
		//void Resume();

		void Throttle(int delayMilliseconds);
	}
}

