using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.EnableCalls
{
    public class Descriptor : IOperationDescriptor
    {
        public string Name { get { return "Enable calls"; } }
        public OperationTypes OperationTypeId { get { return OperationTypes.EnableCalls; } }
        public Type OperationParametersType { get { return typeof(Parameters); } }
        public Type OperationType { get { return typeof(Operation); } }
    }
}
