using System;
using Confirmit.CATI.Core.Telephony.LinkedSurveys;
using System.Collections.Generic;
using Confirmit.CATI.Core.ManagementService;

namespace Confirmit.CATI.Core.Telephony.LinkedSurveys.Fakes
{
    public class StubILinkedInterviewProvider : ILinkedInterviewProvider 
    {
        private ILinkedInterviewProvider _inner;

        public StubILinkedInterviewProvider()
        {
            _inner = null;
        }

        public ILinkedInterviewProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<CatiInterview> FindInt32ArrayOfStringStringStringStringDelegate(int interviewerId, string[] projectIds, string telephoneNumber, string respondentName, string filter);
        public FindInt32ArrayOfStringStringStringStringDelegate FindInt32ArrayOfStringStringStringString;

        List<CatiInterview> ILinkedInterviewProvider.Find(int interviewerId, string[] projectIds, string telephoneNumber, string respondentName, string filter)
        {


            if (FindInt32ArrayOfStringStringStringString != null)
            {
                return FindInt32ArrayOfStringStringStringString(interviewerId, projectIds, telephoneNumber, respondentName, filter);
            } else if (_inner != null)
            {
                return ((ILinkedInterviewProvider)_inner).Find(interviewerId, projectIds, telephoneNumber, respondentName, filter);
            }

            return default(List<CatiInterview>);
        }

        public delegate List<CatiInterview> GetLinkedInterviewsStringDelegate(string linkedChain);
        public GetLinkedInterviewsStringDelegate GetLinkedInterviewsString;

        List<CatiInterview> ILinkedInterviewProvider.GetLinkedInterviews(string linkedChain)
        {


            if (GetLinkedInterviewsString != null)
            {
                return GetLinkedInterviewsString(linkedChain);
            } else if (_inner != null)
            {
                return ((ILinkedInterviewProvider)_inner).GetLinkedInterviews(linkedChain);
            }

            return default(List<CatiInterview>);
        }

    }
}