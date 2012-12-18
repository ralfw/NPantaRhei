using System;
using System.ServiceModel;

namespace npantarhei.distribution.wcf.services
{
    class SingletonServiceHost : ServiceHost
    {
        public SingletonServiceHost(object singleton, params Uri[] baseAddresses) : base(singleton, baseAddresses) {}
    }
}