using System;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.patterns
{
    public abstract class AOperation : IOperation
    {
        protected AOperation(string name) { this.Name = name; this.Implementation = Process; }

        protected virtual void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
        {
            throw new NotImplementedException("Missing operation specific processing definition!");
        }

        public string Name { get; protected set; }
        public OperationAdapter Implementation { get; protected set; }
    }
}
