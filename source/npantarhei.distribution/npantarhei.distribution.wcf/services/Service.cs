using System;
using System.ServiceModel;
using npantarhei.distribution.wcf.contract;

namespace npantarhei.distribution.wcf.services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    class Service<T> : IService<T>
    {
        private readonly Action<T> _continueWith;

        public Service(Action<T> continueWith) { _continueWith = continueWith; }

        public void Process(T input)
        {
            _continueWith(input);
        }
    }
}