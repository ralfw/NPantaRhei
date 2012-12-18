using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using npantarhei.distribution.contract.messagetypes;
using npantarhei.distribution.wcf.contract;

namespace npantarhei.distribution.wcf.services
{
    class ChannelDispenser : IDisposable
    {
        internal struct Channel
        {
            public IService<HostOutput> StandIn;
            public DateTime ExpiresAt;
        }

        readonly Dictionary<string, Channel> _cache = new Dictionary<string, Channel>();
        private int _gcCounter;
        private const int GC_FREQUENCY = 1000;
        private const int INITIAL_LIFESPAN_SEC = 60;


        public IService<HostOutput> Get(string standInEndpointAddress)
        {
            lock (_cache)
            {
                if (++_gcCounter % GC_FREQUENCY == 0) CollectGarbage();

                Channel ch;
                if (!_cache.TryGetValue(standInEndpointAddress, out ch))
                {
                    var cf = new ChannelFactory<IService<HostOutput>>(new NetTcpBinding(), "net.tcp://" + standInEndpointAddress);
                    ch = new Channel { StandIn = cf.CreateChannel(), ExpiresAt = DateTime.Now.AddSeconds(INITIAL_LIFESPAN_SEC) };
                    _cache.Add(standInEndpointAddress, ch);
                }
                return ch.StandIn;
            }
        } 


        internal void CollectGarbage()
        {
            var expiredChannels = _cache.Select(_ => new {_.Key, _.Value.StandIn, _.Value.ExpiresAt})
                                       .Where(_ => _.ExpiresAt <= DateTime.Now)
                                       .ToArray();
            foreach (var ch in expiredChannels)
            {
                (ch.StandIn as ICommunicationObject).Close();
                _cache.Remove(ch.Key);
            }
        }


        public void Dispose()
        {
            foreach(var standIn in _cache.Select(_ => _.Value.StandIn))
                (standIn as ICommunicationObject).Close();
        }
    }
}