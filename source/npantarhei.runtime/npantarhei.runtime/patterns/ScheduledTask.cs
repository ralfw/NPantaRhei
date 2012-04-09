using System;

namespace npantarhei.runtime.patterns
{
    class ScheduledTask<T>
    {
        public T Message;
        public Action<T> ContinueWith;
    }
}