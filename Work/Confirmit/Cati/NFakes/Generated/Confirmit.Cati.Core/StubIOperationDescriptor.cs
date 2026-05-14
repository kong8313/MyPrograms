using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.AsyncOperations.Operations;

namespace Confirmit.CATI.Core.AsyncOperations.Framework.Fakes
{
    public class StubIOperationDescriptor : IOperationDescriptor 
    {
        private IOperationDescriptor _inner;

        public StubIOperationDescriptor()
        {
            _inner = null;
        }

        public IOperationDescriptor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private string _Name;
        public Func<string> NameGet;
        public Action<string> NameSetString;

        string IOperationDescriptor.Name
        {
            get
            {
                if (NameGet != null)
                {
                    return NameGet();
                } else if (_inner != null)
                {
                    return ((IOperationDescriptor)_inner).Name;
                }

                if (NameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Name;
                }

                return default(string);
            }

        }

        private OperationTypes _OperationTypeId;
        public Func<OperationTypes> OperationTypeIdGet;
        public Action<OperationTypes> OperationTypeIdSetOperationTypes;

        OperationTypes IOperationDescriptor.OperationTypeId
        {
            get
            {
                if (OperationTypeIdGet != null)
                {
                    return OperationTypeIdGet();
                } else if (_inner != null)
                {
                    return ((IOperationDescriptor)_inner).OperationTypeId;
                }

                if (OperationTypeIdSetOperationTypes == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _OperationTypeId;
                }

                return default(OperationTypes);
            }

        }

        private Type _OperationParametersType;
        public Func<Type> OperationParametersTypeGet;
        public Action<Type> OperationParametersTypeSetType;

        Type IOperationDescriptor.OperationParametersType
        {
            get
            {
                if (OperationParametersTypeGet != null)
                {
                    return OperationParametersTypeGet();
                } else if (_inner != null)
                {
                    return ((IOperationDescriptor)_inner).OperationParametersType;
                }

                if (OperationParametersTypeSetType == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _OperationParametersType;
                }

                return default(Type);
            }

        }

        private Type _OperationType;
        public Func<Type> OperationTypeGet;
        public Action<Type> OperationTypeSetType;

        Type IOperationDescriptor.OperationType
        {
            get
            {
                if (OperationTypeGet != null)
                {
                    return OperationTypeGet();
                } else if (_inner != null)
                {
                    return ((IOperationDescriptor)_inner).OperationType;
                }

                if (OperationTypeSetType == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _OperationType;
                }

                return default(Type);
            }

        }

    }
}