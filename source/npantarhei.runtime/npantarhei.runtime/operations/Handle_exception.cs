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
                ContinueWith(message);
            }
            catch (Exception ex)
            {
                ExceptionCaught(new FlowRuntimeException(ex, message));
            }
        }

        public Action<IMessage> ContinueWith;
        public Action<FlowRuntimeException> ExceptionCaught;
    }
}