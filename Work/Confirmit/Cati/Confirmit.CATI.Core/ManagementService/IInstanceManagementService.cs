using System.ServiceModel;

namespace Confirmit.CATI.Core.ManagementService
{
    [ServiceContract(Name = "InstanceManagementService", Namespace = "http://www.confirmit.com/InstanceManagementService/15/05/2009")]
    public interface IInstanceManagementService
    {
        [OperationContract]
        string RegisterSchedulingServiceInstance(string instanceName);

        [OperationContract]
        void UnregisterSchedulingServiceInstance(string instanceName);
    }
}
