using System;
using Confirmit.CATI.Supervisor.Classes;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Classes.Fakes
{
    public class StubICheckDeferredRecordsForQuestion : ICheckDeferredRecordsForQuestion 
    {
        private ICheckDeferredRecordsForQuestion _inner;

        public StubICheckDeferredRecordsForQuestion()
        {
            _inner = null;
        }

        public ICheckDeferredRecordsForQuestion Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<DeferredRecordInfo> GetMatchingRecordsStringStringStringStringDelegate(string surveyId, string interviewId, string initialQuestion, string userName);
        public GetMatchingRecordsStringStringStringStringDelegate GetMatchingRecordsStringStringStringString;

        List<DeferredRecordInfo> ICheckDeferredRecordsForQuestion.GetMatchingRecords(string surveyId, string interviewId, string initialQuestion, string userName)
        {


            if (GetMatchingRecordsStringStringStringString != null)
            {
                return GetMatchingRecordsStringStringStringString(surveyId, interviewId, initialQuestion, userName);
            } else if (_inner != null)
            {
                return ((ICheckDeferredRecordsForQuestion)_inner).GetMatchingRecords(surveyId, interviewId, initialQuestion, userName);
            }

            return default(List<DeferredRecordInfo>);
        }

    }
}