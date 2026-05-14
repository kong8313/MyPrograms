using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.ActivateCalls
{
    public class Descriptor : IOperationDescriptor
    {
        public string Name => "Activate calls";
        public OperationTypes OperationTypeId => OperationTypes.ActivateCalls;
        public Type OperationParametersType => typeof(Parameters);
        public Type OperationType => typeof(Operation);
    }
}
