using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfServiceLibrary
{
    [ServiceContract]
    public interface IWcfTestService
    {
        [OperationContract]
        string GetIds();

        [OperationContract]
        string AppDomainName();

        [OperationContract]
        void StartGC();

        [OperationContract]
        string GetThreadPoolInfo();
    }


}
