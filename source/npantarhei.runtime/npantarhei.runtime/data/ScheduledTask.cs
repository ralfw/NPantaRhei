using System;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.data
{
    class ScheduledTask : IPartionable
    {
        public IMessage Message;
        public Action<IMessage> ContinueWith;

        public string Partition
        {
            get { return Message.Port.Fullname; }
        }
    }
}