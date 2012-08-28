using System;
using System.Diagnostics;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.patterns
{
    public interface IAsynchronizer
    {
        void Process(IMessage message, Action<IMessage> continueWith);

        void Start();
        void Stop();
    }

    public class AsyncWrapperOperation : IOperation
    {
        private readonly IAsynchronizer _asyncer;

        public AsyncWrapperOperation(IAsynchronizer asyncer, IOperation operationToWrap)
        {
            _asyncer = asyncer;
            this.Name = operationToWrap.Name;
            this.Implementation = (input, continueWith, unhandledException) => 
                                                asyncer.Process(input, 
                                                                output =>
                                                                    {
                                                                        Debug.Assert(operationToWrap != null, "No operation to execute on thread!");
                                                                        Debug.Assert(continueWith != null, "No continuation for operation!");
                                                                        Debug.Assert(unhandledException != null, "No handler for exceptions!");

                                                                        try
                                                                        {
                                                                            operationToWrap.Implementation(output, continueWith, unhandledException);
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            unhandledException(new FlowRuntimeException(ex, output));
                                                                        }
                                                                    });
        }

        public void Start() { _asyncer.Start(); }
        public void Stop() { _asyncer.Stop(); }

        #region IOperation implementation
        public string Name { get; private set; }
        public OperationAdapter Implementation { get; private set; }
        #endregion
    }
}