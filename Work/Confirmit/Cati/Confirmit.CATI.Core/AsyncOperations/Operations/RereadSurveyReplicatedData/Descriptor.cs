using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.RereadSurveyReplicatedData
{
    public class Descriptor : IOperationDescriptor
    {
        public string Name { get { return "Reread Survey Replicated Data"; } }
        public OperationTypes OperationTypeId { get { return OperationTypes.RereadSurveyReplicatedData; } }
        public Type OperationParametersType { get { return typeof(Parameters); } }
        public Type OperationType { get { return typeof(Operation); } }
    }
}
