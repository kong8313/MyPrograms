using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.EditCalls
{
    public class Descriptor : IOperationDescriptor
    {
        public string Name => "Edit calls";
        public OperationTypes OperationTypeId => OperationTypes.EditCalls;
        public Type OperationParametersType => typeof(Parameters);
        public Type OperationType => typeof(Operation);
    }
}
