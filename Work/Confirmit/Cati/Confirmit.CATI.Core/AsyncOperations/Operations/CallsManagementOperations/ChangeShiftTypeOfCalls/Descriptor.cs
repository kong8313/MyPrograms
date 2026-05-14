using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.ChangeShiftTypeOfCalls
{
    public class Descriptor : IOperationDescriptor
    {
        public string Name { get { return "Change shift type of calls"; } }
        public OperationTypes OperationTypeId { get { return OperationTypes.ChangeShiftTypeOfCalls; } }
        public Type OperationParametersType { get { return typeof(Parameters); } }
        public Type OperationType { get { return typeof(Operation); } }
    }
}
