using System;
using Confirmit.CATI.Core.Batch.Interfaces;
using Confirmit.CATI.Core.Batch;

namespace Confirmit.CATI.Core.Batch.Interfaces.Fakes
{
    public class StubIBatchInitializer : IBatchInitializer 
    {
        private IBatchInitializer _inner;

        public StubIBatchInitializer()
        {
            _inner = null;
        }

        public IBatchInitializer Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InitializeIBatchUploaderBatchParametersDelegate(IBatchUploader uploader, BatchParameters parameters);
        public InitializeIBatchUploaderBatchParametersDelegate InitializeIBatchUploaderBatchParameters;

        void IBatchInitializer.Initialize(IBatchUploader uploader, BatchParameters parameters)
        {

            if (InitializeIBatchUploaderBatchParameters != null)
            {
                InitializeIBatchUploaderBatchParameters(uploader, parameters);
            } else if (_inner != null)
            {
                ((IBatchInitializer)_inner).Initialize(uploader, parameters);
            }
        }

        private Type _SupportedBatchParametersType;
        public Func<Type> SupportedBatchParametersTypeGet;
        public Action<Type> SupportedBatchParametersTypeSetType;

        Type IBatchInitializer.SupportedBatchParametersType
        {
            get
            {
                if (SupportedBatchParametersTypeGet != null)
                {
                    return SupportedBatchParametersTypeGet();
                } else if (_inner != null)
                {
                    return ((IBatchInitializer)_inner).SupportedBatchParametersType;
                }

                if (SupportedBatchParametersTypeSetType == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SupportedBatchParametersType;
                }

                return default(Type);
            }

        }

    }
}