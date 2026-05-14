using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.AsyncOperations.Operations;
using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public class AsyncOperationFactory : IAsyncOperationFactory
    {
        private readonly Dictionary<OperationTypes, IOperationDescriptor> _operationTypeId2OperationDescriptor;
        private readonly Dictionary<Type, IOperationDescriptor> _operationType2Descriptor;
        private readonly Dictionary<Type, IOperationDescriptor> _operationParametersType2Descriptor;

        public AsyncOperationFactory(IOperationDescriptor[] descriptors)
        {
            _operationTypeId2OperationDescriptor = new Dictionary<OperationTypes, IOperationDescriptor>();
            _operationType2Descriptor = new Dictionary<Type, IOperationDescriptor>();
            _operationParametersType2Descriptor = new Dictionary<Type, IOperationDescriptor>();

            foreach (var operationDescriptor in descriptors)
            {
                IndexDescriptor(operationDescriptor);
            }
        }

        public void IndexDescriptor(IOperationDescriptor descriptor)
        {
            _operationTypeId2OperationDescriptor[descriptor.OperationTypeId] = descriptor;
            _operationType2Descriptor[descriptor.OperationType] = descriptor;
            _operationParametersType2Descriptor[descriptor.OperationParametersType] = descriptor;
        }

        public IOperationDescriptor GetOperationDescriptorFromOperationType(OperationTypes operationType)
        {
            return _operationTypeId2OperationDescriptor[operationType];
        }

        public IOperationDescriptor GetOperationDescriptorFromOperationType(Type type)
        {
            return _operationType2Descriptor[type];
        }

        public IOperationDescriptor GetOperationDescriptorFromOperationParametersType(Type type)
        {
            return _operationParametersType2Descriptor[type];
        }

        public IAsyncOperation CreateOperationFromType(OperationTypes operationTypeId)
        {
            var descriptor = GetOperationDescriptorFromOperationType(operationTypeId);

            return CreateOperationFromDescriptor(descriptor);
        }

        public IAsyncOperation CreateOperationFromDescriptorType(OperationTypes operationType)
        {
            var descriptor = GetOperationDescriptorFromOperationType(operationType);

            return CreateOperationFromDescriptor(descriptor);
        }

        public IAsyncOperation CreateOperationFromDescriptor(IOperationDescriptor descriptor)
        {
            return (IAsyncOperation)ServiceLocator.Resolve(descriptor.OperationType);
        }
    }
}
