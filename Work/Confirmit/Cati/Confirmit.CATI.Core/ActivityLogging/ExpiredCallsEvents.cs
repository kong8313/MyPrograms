using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public class ExpiredCallsEventParameters : ManagementActivityEventDetails
    {
        public List<ExpiredCallsParameter> ExpiredCalls { get; set; }

        public ExpiredCallsEventParameters()
        {
            ExpiredCalls = new List<ExpiredCallsParameter>();
        }
    }

    [Serializable]
    public class ExpiredCallsParameter
    {
        public int SurveyId { get; set; }
        public int InterviewId { get; set; }

        public ExpiredCallsParameter(int surveyId, int interviewId)
        {
            this.InterviewId = interviewId;
            this.SurveyId = surveyId;
        }

        public ExpiredCallsParameter()
        {
        }
    }

    [ManagementEventAttribute(ManagementEvent.ExpiredCalls)]
    public class ExpiredCallsEvents : ManagementActivityEvent<ExpiredCallsEventParameters>
    {
        public ExpiredCallsEvents():
            base(ManagementEventCategory.BackgroundTasks, ManagementEvent.ExpiredCalls, true)
        {
            Details = new ExpiredCallsEventParameters();
        }

        public void AddExpiredCall(int surveyId, int interviewId)
        {
            Details.ExpiredCalls.Add(new ExpiredCallsParameter(surveyId, interviewId));
        }
    }
}