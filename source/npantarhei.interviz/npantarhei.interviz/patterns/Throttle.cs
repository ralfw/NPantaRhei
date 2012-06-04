using System;
using System.Timers;
using npantarhei.runtime.contract;
using npantarhei.runtime.patterns;

namespace todo.logik.patterns
{
    public class Throttle : AOperation
    {
        private readonly int periodInMilliseconds;
        private Timer timer;
        private IMessage message;

        public Throttle(string name, int periodInMilliseconds = 500)
            : base(name) {
            this.periodInMilliseconds = periodInMilliseconds;
        }

        protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException) {
            message = input;
            if (timer == null) {
                timer = new Timer {
                    Interval = periodInMilliseconds
                };
                timer.Elapsed +=
                    delegate {
                        timer.Dispose();
                        timer = null;
                        continueWith(message);
                    };
                timer.Start();
            }
            else {
                timer.Stop();
                timer.Start();
            }
        }
    }
}