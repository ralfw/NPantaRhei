using System;
using System.Threading;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns;

namespace IOperation_wrapper_for_EBC
{
    class ToUpperOp : AOperation
    {
        public ToUpperOp(string name) : base(name)
        {
            var ebc = new ToUpperEBC();
            ebc.Result += _ =>
                              {
                                  var continueWith = (Action<IMessage>)Thread.GetData(Thread.GetNamedDataSlot("continueWith"));
                                  continueWith(new Message(base.Name, _));
                              };

            _adapter = (input, continueWith, unhandledException) =>
                           {
                               Thread.AllocateNamedDataSlot("continueWith");
                               Thread.SetData(Thread.GetNamedDataSlot("continueWith"), continueWith);
                               try
                               {
                                   ebc.Process((string) input.Data);
                               }
                               finally
                               {
                                   Thread.FreeNamedDataSlot("continueWith");
                               }
                           };

        }

        private readonly OperationAdapter _adapter;
        protected override void Process(npantarhei.runtime.contract.IMessage input, Action<npantarhei.runtime.contract.IMessage> continueWith, Action<npantarhei.runtime.contract.FlowRuntimeException> unhandledException)
        {
            _adapter(input, continueWith, unhandledException);
        }
    }
}