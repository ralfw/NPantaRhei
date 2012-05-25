using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.patterns.operations
{
    [ActiveOperation]
    public class Clock : AOperation
    {
        private const string DEFAULT_NAME = "Clock";
        private const int DEFAULT_PERIOD = 1000;

        private readonly int _periodMilliseconds;

        public Clock() : this(DEFAULT_PERIOD) { }
        public Clock(int period) : this(DEFAULT_NAME, period) { }
        public Clock(string name) : this(name, DEFAULT_PERIOD) { }
        public Clock(string name, int periodMilliseconds) : base(name) { _periodMilliseconds = periodMilliseconds; }

        private Action<IMessage> _continueWith;
        private System.Threading.Timer _timer;

        protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
        {
            if (!(input is ActivationMessage)) return;

            _continueWith = continueWith;
            _timer = new System.Threading.Timer(_ => _continueWith(new Message(this.Name + ".now", DateTime.Now)),
                                                null, 0, _periodMilliseconds);
        }
    }
}
