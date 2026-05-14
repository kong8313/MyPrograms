using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.SynchronizeRespondents

{
    public class Descriptor : IOperationDescriptor
    {
        public string Name { get { return "Synchronize respondents"; } }
        public OperationTypes OperationTypeId { get { return OperationTypes.SynchronizeRespondents; } }
        public Type OperationParametersType { get { return typeof(Parameters); } }
        public Type OperationType { get { return typeof(Operation); } }
    }
}
