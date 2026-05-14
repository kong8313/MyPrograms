using System;
using Confirmit.CATI.Core.AsyncOperations.Operations;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public interface IAsyncOperationFactory
    {
        void IndexDescriptor(IOperationDescriptor descriptor);
        IOperationDescriptor GetOperationDescriptorFromOperationType(OperationTypes operationType);
        IOperationDescriptor GetOperationDescriptorFromOperationType(Type type);
        IOperationDescriptor GetOperationDescriptorFromOperationParametersType(Type type);
        IAsyncOperation CreateOperationFromType(OperationTypes operationType);
        IAsyncOperation CreateOperationFromDescriptor(IOperationDescriptor descriptor);
    }
}