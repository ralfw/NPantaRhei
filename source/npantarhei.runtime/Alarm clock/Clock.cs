using System;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns;

namespace Alarm_clock
{
    internal class Clock : AOperation
    {
        public Clock() : base("Clock") {}


        private Action<IMessage> _continueWith;
        private System.Threading.Timer _timer;

        protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
        {
            switch(input.Port.Name.ToLower())
            {
                case "config":
                    _continueWith = continueWith;
                    _timer = new System.Threading.Timer(_ => _continueWith(new Message(this.Name + ".now", DateTime.Now)), 
                                                        null, 100, 1000);
                    break;
            }    
        }
    }
}