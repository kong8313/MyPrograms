using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.ChangePriorityOfCalls
{
    public class Descriptor : IOperationDescriptor
    {
        public string Name { get { return "Change priority of calls"; } }
        public OperationTypes OperationTypeId { get { return OperationTypes.ChangePriorityOfCalls; } }
        public Type OperationParametersType { get { return typeof(Parameters); } }
        public Type OperationType { get { return typeof(Operation); } }
    }
}
