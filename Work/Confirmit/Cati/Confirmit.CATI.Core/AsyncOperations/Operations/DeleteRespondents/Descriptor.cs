using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.DeleteRespondents
{
    public class Descriptor : IOperationDescriptor
    {
        public string Name { get { return "Delete Respondents"; } }
        public OperationTypes OperationTypeId { get { return OperationTypes.DeleteRespondents; } }
        public Type OperationParametersType { get { return typeof(Parameters); } }
        public Type OperationType { get { return typeof(Operation); } }
    }
}