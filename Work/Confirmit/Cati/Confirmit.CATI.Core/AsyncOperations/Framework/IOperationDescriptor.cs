using System;
using Confirmit.CATI.Core.AsyncOperations.Operations;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public interface IOperationDescriptor
    {
        string Name { get; }
        OperationTypes OperationTypeId { get; }
        Type OperationParametersType { get; }
        Type OperationType { get; }
    }
}