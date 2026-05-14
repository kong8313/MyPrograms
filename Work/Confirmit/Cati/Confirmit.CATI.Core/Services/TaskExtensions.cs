using System.Diagnostics;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Services
{
    public static class TaskExtensions
    {
        public static string LogString(this BvTasksEntity task, string extraInfo = "")
        {
            if (task == null)
            {
                return string.Format("task is [null] /// call stack: {0}", new StackTrace(true));
            }

            var surveyRepository = ServiceLocator.Resolve<ISurveyRepository>();

            var surveyName = surveyRepository.GetSurveyNameOrErrorString(task.SurveySID);

            var newSurveyName = surveyRepository.GetSurveyNameOrErrorString(task.NewSurveySID);

            return string.Format(
                "dialerId='{0}', surveyName='{1}', newSurveyName='{2}', SurveySID='{3}', NewSurveySID='{4}'" +
                ", agentId='{5}', statusLogout='{6}', loggedInToDialerState='{7}'. {8} /// call stack: {9}",
                task.DialerId, surveyName, newSurveyName, task.SurveySID, task.NewSurveySID,
                task.PersonSID, (LoginState)task.StatusLogout, (LoginState)task.LoggedInToDialerState, extraInfo, new StackTrace(true));
        }

        public static LinkedInterviewPhase GetLinkedInterviewsPhase(this BvTasksEntity task)
        {
            if (task.LinkedCallId == null && task.LinkedChain == null)
            {
                return LinkedInterviewPhase.NotLinkedInterview;
            }

            if (task.LinkedCallId != null && task.LinkedChain == null)
            {
                return LinkedInterviewPhase.FirstInterview;
            }

            if (task.LinkedChain != null && task.LinkedCallId == null)
            {
                return LinkedInterviewPhase.FinalInterview;
            }

            return LinkedInterviewPhase.MiddleInterview;
        }
    }
}