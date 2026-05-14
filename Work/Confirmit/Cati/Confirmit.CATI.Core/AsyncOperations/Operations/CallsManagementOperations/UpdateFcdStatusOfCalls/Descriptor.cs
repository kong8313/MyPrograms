using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.UpdateFcdStatusOfCalls
{
    public class Descriptor : IOperationDescriptor
    {
        public string Name { get { return "Update FCD quota"; } }
        public OperationTypes OperationTypeId { get { return OperationTypes.UpdateFcdQuota; } }
        public Type OperationParametersType { get { return typeof(Parameters); } }
        public Type OperationType { get { return typeof(Operation); } }
    }
}