using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.ConfigureClusteredQuota

{
    public class Descriptor : IOperationDescriptor
    {
        public string Name { get { return "Configure clustered quota"; } }
        public OperationTypes OperationTypeId { get { return OperationTypes.ConfigureClusteredQuota; } }
        public Type OperationParametersType { get { return typeof(Parameters); } }
        public Type OperationType { get { return typeof(Operation); } }
    }
}
