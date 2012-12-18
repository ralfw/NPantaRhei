using System.ServiceModel;

namespace npantarhei.distribution.wcf.contract
{
    [ServiceContract]
    public interface IService<in T>
    {
        [OperationContract]
        void Process(T input);
    }
}