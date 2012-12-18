using System.ServiceModel;
using npantarhei.distribution.contract.messagetypes;

namespace npantarhei.distribution.wcf.contract
{
    [ServiceContract]
    public interface IStandInService
    {
        [OperationContract]
        void Process(HostOutput input);
    }
}