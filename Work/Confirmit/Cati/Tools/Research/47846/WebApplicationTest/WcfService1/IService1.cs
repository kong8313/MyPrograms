using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfService1
{
    [ServiceContract]
    public interface IService1
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


    /*[DataContract]
    public class CompositeType
    {

        [DataMember]

        [DataMember]
    }*/
}
