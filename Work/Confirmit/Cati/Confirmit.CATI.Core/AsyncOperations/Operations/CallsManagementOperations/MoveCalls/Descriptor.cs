using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.MoveCalls
{
    public class Descriptor : IOperationDescriptor
    {
        public string Name { get { return "Move Calls"; } }
        public OperationTypes OperationTypeId { get { return OperationTypes.MoveCalls; } }
        public Type OperationParametersType { get { return typeof(Parameters); } }
        public Type OperationType { get { return typeof(Operation); } }
    }
}
