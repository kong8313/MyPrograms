using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.MoveAndRescheduleCalls
{
    public class Descriptor : IOperationDescriptor
    {
        public string Name { get { return "Move and reschedule calls"; } }
        public OperationTypes OperationTypeId { get { return OperationTypes.MoveAndRescheduleCalls; } }
        public Type OperationParametersType { get { return typeof(Parameters); } }
        public Type OperationType { get { return typeof(Operation); } }
    }
}
