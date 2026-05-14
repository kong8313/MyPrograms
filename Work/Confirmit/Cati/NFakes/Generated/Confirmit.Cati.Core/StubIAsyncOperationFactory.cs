using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.AsyncOperations.Operations;

namespace Confirmit.CATI.Core.AsyncOperations.Framework.Fakes
{
    public class StubIAsyncOperationFactory : IAsyncOperationFactory 
    {
        private IAsyncOperationFactory _inner;

        public StubIAsyncOperationFactory()
        {
            _inner = null;
        }

        public IAsyncOperationFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void IndexDescriptorIOperationDescriptorDelegate(IOperationDescriptor descriptor);
        public IndexDescriptorIOperationDescriptorDelegate IndexDescriptorIOperationDescriptor;

        void IAsyncOperationFactory.IndexDescriptor(IOperationDescriptor descriptor)
        {

            if (IndexDescriptorIOperationDescriptor != null)
            {
                IndexDescriptorIOperationDescriptor(descriptor);
            } else if (_inner != null)
            {
                ((IAsyncOperationFactory)_inner).IndexDescriptor(descriptor);
            }
        }

        public delegate IOperationDescriptor GetOperationDescriptorFromOperationTypeOperationTypesDelegate(OperationTypes operationType);
        public GetOperationDescriptorFromOperationTypeOperationTypesDelegate GetOperationDescriptorFromOperationTypeOperationTypes;

        IOperationDescriptor IAsyncOperationFactory.GetOperationDescriptorFromOperationType(OperationTypes operationType)
        {


            if (GetOperationDescriptorFromOperationTypeOperationTypes != null)
            {
                return GetOperationDescriptorFromOperationTypeOperationTypes(operationType);
            } else if (_inner != null)
            {
                return ((IAsyncOperationFactory)_inner).GetOperationDescriptorFromOperationType(operationType);
            }

            return default(IOperationDescriptor);
        }

        public delegate IOperationDescriptor GetOperationDescriptorFromOperationTypeTypeDelegate(Type type);
        public GetOperationDescriptorFromOperationTypeTypeDelegate GetOperationDescriptorFromOperationTypeType;

        IOperationDescriptor IAsyncOperationFactory.GetOperationDescriptorFromOperationType(Type type)
        {


            if (GetOperationDescriptorFromOperationTypeType != null)
            {
                return GetOperationDescriptorFromOperationTypeType(type);
            } else if (_inner != null)
            {
                return ((IAsyncOperationFactory)_inner).GetOperationDescriptorFromOperationType(type);
            }

            return default(IOperationDescriptor);
        }

        public delegate IOperationDescriptor GetOperationDescriptorFromOperationParametersTypeTypeDelegate(Type type);
        public GetOperationDescriptorFromOperationParametersTypeTypeDelegate GetOperationDescriptorFromOperationParametersTypeType;

        IOperationDescriptor IAsyncOperationFactory.GetOperationDescriptorFromOperationParametersType(Type type)
        {


            if (GetOperationDescriptorFromOperationParametersTypeType != null)
            {
                return GetOperationDescriptorFromOperationParametersTypeType(type);
            } else if (_inner != null)
            {
                return ((IAsyncOperationFactory)_inner).GetOperationDescriptorFromOperationParametersType(type);
            }

            return default(IOperationDescriptor);
        }

        public delegate IAsyncOperation CreateOperationFromTypeOperationTypesDelegate(OperationTypes operationType);
        public CreateOperationFromTypeOperationTypesDelegate CreateOperationFromTypeOperationTypes;

        IAsyncOperation IAsyncOperationFactory.CreateOperationFromType(OperationTypes operationType)
        {


            if (CreateOperationFromTypeOperationTypes != null)
            {
                return CreateOperationFromTypeOperationTypes(operationType);
            } else if (_inner != null)
            {
                return ((IAsyncOperationFactory)_inner).CreateOperationFromType(operationType);
            }

            return default(IAsyncOperation);
        }

        public delegate IAsyncOperation CreateOperationFromDescriptorIOperationDescriptorDelegate(IOperationDescriptor descriptor);
        public CreateOperationFromDescriptorIOperationDescriptorDelegate CreateOperationFromDescriptorIOperationDescriptor;

        IAsyncOperation IAsyncOperationFactory.CreateOperationFromDescriptor(IOperationDescriptor descriptor)
        {


            if (CreateOperationFromDescriptorIOperationDescriptor != null)
            {
                return CreateOperationFromDescriptorIOperationDescriptor(descriptor);
            } else if (_inner != null)
            {
                return ((IAsyncOperationFactory)_inner).CreateOperationFromDescriptor(descriptor);
            }

            return default(IAsyncOperation);
        }

    }
}