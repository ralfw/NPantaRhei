using System;
using System.ServiceModel;
using npantarhei.distribution.contract;
using npantarhei.distribution.contract.messagetypes;
using npantarhei.distribution.wcf.contract;
using npantarhei.distribution.wcf.services;

namespace npantarhei.distribution.wcf
{
    public class WcfStandInTransceiver : IHostProxy, IStandInStub
    {
        private readonly string _localEndpointAddress;

        private readonly ServiceHost _standInService;
        private readonly IService<HostInput> _host;


        public WcfStandInTransceiver(string localEndpointAddress, string remoteEndpointAddress)
        {
            _localEndpointAddress = localEndpointAddress;

            _standInService = new SingletonServiceHost(new Service<HostOutput>(_ => ReceivedFromHost(_)));
            _standInService.AddServiceEndpoint(typeof(IService<HostOutput>), new NetTcpBinding(), "net.tcp://" + localEndpointAddress);
            _standInService.Open();

            var cf = new ChannelFactory<IService<HostInput>>(new NetTcpBinding(), "net.tcp://" + remoteEndpointAddress);
            _host = cf.CreateChannel();
        }


        #region IHostProxy
        public void SendToHost(HostInput input)
        {
            _host.Process(input);
        }
        #endregion


        #region IStandInStub
        public event Action<HostOutput> ReceivedFromHost;

        public string StandInEndpointAddress
        {
            get { return _localEndpointAddress; }
        }
        #endregion


        public void Dispose()
        {
            _standInService.Close();
            (_host as ICommunicationObject).Close();
        }
    }
}
