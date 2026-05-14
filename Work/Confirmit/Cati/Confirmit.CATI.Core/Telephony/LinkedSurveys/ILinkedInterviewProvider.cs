using System.Collections.Generic;
using Confirmit.CATI.Core.ManagementService;

namespace Confirmit.CATI.Core.Telephony.LinkedSurveys
{
    public interface ILinkedInterviewProvider
    {
        List<CatiInterview> Find(int interviewerId, string[] projectIds, string telephoneNumber, string respondentName, string filter);
        List<CatiInterview> GetLinkedInterviews(string linkedChain);
    }
}