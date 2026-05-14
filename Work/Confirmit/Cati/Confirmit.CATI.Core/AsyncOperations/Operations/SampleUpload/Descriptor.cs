using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.SampleUpload
{
    public class Descriptor : IOperationDescriptor
    {
        public string Name { get { return "Sample Upload"; } }
        public OperationTypes OperationTypeId { get { return OperationTypes.SampleUpload; } }
        public Type OperationParametersType { get { return typeof(Parameters); } }
        public Type OperationType { get { return typeof(Operation); } }
    }
}
