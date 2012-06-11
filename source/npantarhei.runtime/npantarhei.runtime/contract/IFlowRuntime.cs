using System;
using System.Collections.Generic;
using System.Reflection;

namespace npantarhei.runtime.contract
{
	public interface IFlowRuntime : IDisposable
	{
	    void Configure(FlowRuntimeConfiguration config);

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

		Action CreateEventProcessor(string portname);
		Action<T> CreateEventProcessor<T>(string portname);
		
		//void Pause();
		//void BreakOnPort(string includeFilter);
		//void BreakOnMessage(Func<IMessage, bool> includeFilter);
		//void Resume();

		void Throttle(int delayMilliseconds);
	}
}

