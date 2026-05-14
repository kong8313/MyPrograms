using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Framework.Fakes
{
    public class StubIAsyncOperationParameters : IAsyncOperationParameters 
    {
        private IAsyncOperationParameters _inner;

        public StubIAsyncOperationParameters()
        {
            _inner = null;
        }

        public IAsyncOperationParameters Inner
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

    }
}