using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.AssignCalls
{
    public class Descriptor : IOperationDescriptor
    {
        public string Name { get { return "Assign Calls"; } }
        public OperationTypes OperationTypeId { get { return OperationTypes.AssignCalls; } }
        public Type OperationParametersType { get { return typeof(Parameters); } }
        public Type OperationType { get { return typeof(Operation); } }
    }
}
