using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;

namespace Confirmit.CATI.Core.AsyncOperations.Framework.Fakes
{
    public class StubIAsyncBatchedOperationParameters : IAsyncBatchedOperationParameters 
    {
        private IAsyncBatchedOperationParameters _inner;

        public StubIAsyncBatchedOperationParameters()
        {
            _inner = null;
        }

        public IAsyncBatchedOperationParameters Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _SurveyId;
        public Func<int> SurveyIdGet;
        public Action<int> SurveyIdSetInt32;

        int IAsyncOperationParameters.SurveyId
        {
            get
            {
                if (SurveyIdGet != null)
                {
                    return SurveyIdGet();
                } else if (_inner != null)
                {
                    return ((IAsyncOperationParameters)_inner).SurveyId;
                }

                if (SurveyIdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SurveyId;
                }

                return default(int);
            }

        }

        private BatchParameters _BatchParameters;
        public Func<BatchParameters> BatchParametersGet;
        public Action<BatchParameters> BatchParametersSetBatchParameters;

        BatchParameters IAsyncBatchedOperationParameters.BatchParameters
        {
            get
            {
                if (BatchParametersGet != null)
                {
                    return BatchParametersGet();
                } else if (_inner != null)
                {
                    return ((IAsyncBatchedOperationParameters)_inner).BatchParameters;
                }

                if (BatchParametersSetBatchParameters == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _BatchParameters;
                }

                return default(BatchParameters);
            }

        }

    }
}