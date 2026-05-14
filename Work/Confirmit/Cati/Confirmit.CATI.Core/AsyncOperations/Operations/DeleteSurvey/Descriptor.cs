using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.DeleteSurvey
{
    public class Descriptor : IOperationDescriptor
    {
        public string Name { get { return "Delete Survey"; } }
        public OperationTypes OperationTypeId { get { return OperationTypes.DeleteSurvey; } }
        public Type OperationParametersType { get { return typeof(Parameters); } }
        public Type OperationType { get { return typeof(Operation); } }
    }
}
