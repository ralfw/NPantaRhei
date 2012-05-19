using System;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.operations
{
    internal class Nest_processing
    {
        public void Process(IMessage message)
        {
            if (message.Port.IsOperationPort)
                Continue(message);
            else
                Zoom(message);
        }

        public event Action<IMessage> Continue;
        public event Action<IMessage> Zoom;
    }
}