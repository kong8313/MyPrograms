using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.RestoreSurvey
{
    public class Descriptor : IOperationDescriptor
    {
        public string Name { get { return "Restore Survey"; } }
        public OperationTypes OperationTypeId { get { return OperationTypes.RestoreSurvey; } }
        public Type OperationParametersType { get { return typeof(Parameters); } }
        public Type OperationType { get { return typeof(Operation); } }
    }
}
