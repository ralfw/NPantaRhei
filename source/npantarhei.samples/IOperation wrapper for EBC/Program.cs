using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime.contract;
using npantarhei.runtime.patterns;

namespace IOperation_wrapper_for_EBC
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    class ToUpperOp : AOperation
    {
        public ToUpperOp(string name) : base(name)
        {
            var ebc = new EBCtoUpper();
            _adapter = (input, continueWith, unhandledException) => { ebc.Process((string) input.Data); };

        }

        private readonly OperationAdapter _adapter;
        protected override void Process(npantarhei.runtime.contract.IMessage input, Action<npantarhei.runtime.contract.IMessage> continueWith, Action<npantarhei.runtime.contract.FlowRuntimeException> unhandledException)
        {
            _adapter(input, continueWith, unhandledException);
        }
    }

    class EBCtoUpper
    {
        public void Process(string text)
        {
            Result(text.ToUpper());
        }

        public event Action<string> Result;
    }
}
