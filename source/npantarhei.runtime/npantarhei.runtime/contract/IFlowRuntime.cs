using System;
using System.Collections.Generic;

namespace npantarhei.runtime.contract
{
    public interface IFlowRuntime : IDisposable
	{
		void Process(IMessage message);

        event Action<IMessage> Message;
        event Action<FlowRuntimeException> UnhandledException;
        event Action<IMessage> Result;

        bool WaitForResult(int milliseconds);
        bool WaitForResult(int milliseconds, Action<IMessage> processResult);
		
		void AddStream(IStream stream);
        void AddStream(string fromPortName, string toPortName);
		void AddOperation(IOperation operation);
		void AddOperations(IEnumerable<IOperation> operations);

        Action CreateEventProcessor(string portname);
        Action<T> CreateEventProcessor<T>(string portname);
		
        //void Pause();
        //void BreakOnPort(string includeFilter);
        //void BreakOnMessage(Func<IMessage, bool> includeFilter);
        //void Resume();

        void Throttle(int delayMilliseconds);
	}
}

