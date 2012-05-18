using System;

namespace Alarm_clock
{
    internal class Clock2
    {
        private System.Threading.Timer _timer;
        
        public Clock2()
        {
            _timer = new System.Threading.Timer(_ => CurrentTime(DateTime.Now), null, 100, 1000);
        }

        public Action<DateTime> CurrentTime = _ => { };
    }
}