using System;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIReviewerService : IReviewerService 
    {
        private IReviewerService _inner;

        public StubIReviewerService()
        {
            _inner = null;
        }

        public IReviewerService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string CreateSessionForReviewStringInt32StringBatchParametersDelegate(string sessionName, int surveyId, string userName, BatchParameters batchParameters);
        public CreateSessionForReviewStringInt32StringBatchParametersDelegate CreateSessionForReviewStringInt32StringBatchParameters;

        string IReviewerService.CreateSessionForReview(string sessionName, int surveyId, string userName, BatchParameters batchParameters)
        {


            if (CreateSessionForReviewStringInt32StringBatchParameters != null)
            {
                return CreateSessionForReviewStringInt32StringBatchParameters(sessionName, surveyId, userName, batchParameters);
            } else if (_inner != null)
            {
                return ((IReviewerService)_inner).CreateSessionForReview(sessionName, surveyId, userName, batchParameters);
            }

            return default(string);
        }

        public delegate string GetReviewerUrlTemplateDelegate();
        public GetReviewerUrlTemplateDelegate GetReviewerUrlTemplate;

        string IReviewerService.GetReviewerUrlTemplate()
        {


            if (GetReviewerUrlTemplate != null)
            {
                return GetReviewerUrlTemplate();
            } else if (_inner != null)
            {
                return ((IReviewerService)_inner).GetReviewerUrlTemplate();
            }

            return default(string);
        }

    }
}