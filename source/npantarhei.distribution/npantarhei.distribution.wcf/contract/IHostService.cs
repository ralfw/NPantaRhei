using System.ServiceModel;
using npantarhei.distribution.contract.messagetypes;

namespace npantarhei.distribution.wcf.contract
{
    [ServiceContract]
    public interface IHostService
    {
        [OperationContract]
        void Process(HostInput input);
    }
}
