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

        private System.Threading.Timer _timer;

        public Clock() : this(DEFAULT_PERIOD) { }
        public Clock(int periodMilliseconds) : this(DEFAULT_NAME, periodMilliseconds) { }
        public Clock(string name) : this(name, DEFAULT_PERIOD) { }
        public Clock(string name, int periodMilliseconds) : base(name)
        {
            _timer = new System.Threading.Timer(_ => Now(DateTime.Now), null, 0, periodMilliseconds);
        }


        protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
        {
            if (!(input is ActivationMessage)) return;

            Now += _ => continueWith(new Message(this.Name + ".now", _));
        }


        public event Action<DateTime> Now = _ => { };
    }
}
