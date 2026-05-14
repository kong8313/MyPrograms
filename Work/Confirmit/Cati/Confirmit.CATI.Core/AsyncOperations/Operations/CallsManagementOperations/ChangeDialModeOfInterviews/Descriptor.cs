using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.ChangeDialModeOfInterviews
{
    public class Descriptor : IOperationDescriptor
    {
        public string Name { get { return "Change dial mode of inteviews"; } }
        public OperationTypes OperationTypeId { get { return OperationTypes.ChangeDialModeOfInterviews; } }
        public Type OperationParametersType { get { return typeof(Parameters); } }
        public Type OperationType { get { return typeof(Operation); } }
    }
}
