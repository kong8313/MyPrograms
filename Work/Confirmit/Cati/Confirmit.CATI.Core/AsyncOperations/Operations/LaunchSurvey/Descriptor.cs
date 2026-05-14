using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.LaunchSurvey
{
    public class Descriptor : IOperationDescriptor
    {
        public string Name { get { return "Launch Survey"; } }
        public OperationTypes OperationTypeId { get { return OperationTypes.LaunchSurvey; } }
        public Type OperationParametersType { get { return typeof(Parameters); } }
        public Type OperationType { get { return typeof(Operation); } }
    }
}
