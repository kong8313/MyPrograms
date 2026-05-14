using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.ExecuteRoutineMaintenance
{
    public class Descriptor : IOperationDescriptor
    {
        public string Name { get { return "Execute routine maintenance"; } }
        public OperationTypes OperationTypeId { get { return OperationTypes.ExecuteRoutineMaintenance; } }
        public Type OperationParametersType { get { return typeof(Parameters); } }
        public Type OperationType { get { return typeof(Operation); } }
    }
}
