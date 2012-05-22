using System;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.operations
{
    internal class Handle_exception
    {
        public void Process(IMessage message)
        {
            try
            {
                Continue(message);
            }
            catch(Exception ex)
            {
                UnhandledException(new FlowRuntimeException(ex, message));
            }
        }

        public event Action<IMessage> Continue;
        public event Action<FlowRuntimeException> UnhandledException;
    }
}