using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.DeleteCalls
{
    public class Descriptor : IOperationDescriptor
    {
        public string Name { get { return "Deactivate calls"; } }
        public OperationTypes OperationTypeId { get { return OperationTypes.DeactivateCalls; } }
        public Type OperationParametersType { get { return typeof(Parameters); } }
        public Type OperationType { get { return typeof(Operation); } }
    }
}
