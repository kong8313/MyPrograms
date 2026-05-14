using System;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data.Fakes
{
    public class StubIInterviewDataServiceFactory : IInterviewDataServiceFactory 
    {
        private IInterviewDataServiceFactory _inner;

        public StubIInterviewDataServiceFactory()
        {
            _inner = null;
        }

        public IInterviewDataServiceFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IInterviewFormDataSourceService CreateFormServiceInt32Int32Delegate(int surveyId, int interviewId);
        public CreateFormServiceInt32Int32Delegate CreateFormServiceInt32Int32;

        IInterviewFormDataSourceService IInterviewDataServiceFactory.CreateFormService(int surveyId, int interviewId)
        {


            if (CreateFormServiceInt32Int32 != null)
            {
                return CreateFormServiceInt32Int32(surveyId, interviewId);
            } else if (_inner != null)
            {
                return ((IInterviewDataServiceFactory)_inner).CreateFormService(surveyId, interviewId);
            }

            return default(IInterviewFormDataSourceService);
        }

        public delegate IInterviewRespondentDataSourceService CreateRespondentServiceInt32Int32Delegate(int surveyId, int interviewId);
        public CreateRespondentServiceInt32Int32Delegate CreateRespondentServiceInt32Int32;

        IInterviewRespondentDataSourceService IInterviewDataServiceFactory.CreateRespondentService(int surveyId, int interviewId)
        {


            if (CreateRespondentServiceInt32Int32 != null)
            {
                return CreateRespondentServiceInt32Int32(surveyId, interviewId);
            } else if (_inner != null)
            {
                return ((IInterviewDataServiceFactory)_inner).CreateRespondentService(surveyId, interviewId);
            }

            return default(IInterviewRespondentDataSourceService);
        }

    }
}