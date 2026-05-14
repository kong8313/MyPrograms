using System;
using System.Linq;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public abstract class MessagingEventParameters : ManagementActivityEventDetails
    {
        #region Properties

        /// <summary>
        /// Gets/sets list of identifiers to send message.
        /// </summary>
        public virtual int[] Ids { get; set; }

        #endregion
    }

    [Serializable]
    public class SendMessageToSurveysEventParameters : MessagingEventParameters
    {
    }

    [Serializable]
    public class SendMessageToInterviewersEventParameters : MessagingEventParameters
    {
    }

    [Serializable]
    public class SendMessageToGroupsEventParameters : MessagingEventParameters
    {
    }

    [ManagementEventAttribute(ManagementEvent.SendMessageToSurveys)]
    public class SendMessageToSurveysEvent : ManagementActivityEvent<SendMessageToSurveysEventParameters>
    {
        public SendMessageToSurveysEvent(IEnumerable<int> surveyIds):
            base(ManagementEventCategory.Messaging, ManagementEvent.SendMessageToSurveys)
        {
            Details = new SendMessageToSurveysEventParameters() { Ids = surveyIds.ToArray() };
        }
    }

    [ManagementEventAttribute(ManagementEvent.SendMessageToInterviewers)]
    public class SendMessageToInterviewersEvent : ManagementActivityEvent<SendMessageToInterviewersEventParameters>
    {
        public SendMessageToInterviewersEvent(IEnumerable<int> interviewerIds):
            base(ManagementEventCategory.Messaging, ManagementEvent.SendMessageToInterviewers)
        {
            Details = new SendMessageToInterviewersEventParameters() { Ids = interviewerIds.ToArray() };
        }
    }

    [ManagementEventAttribute(ManagementEvent.SendMessageToGroups)]
    public class SendMessageToGroupsEvent : ManagementActivityEvent<SendMessageToGroupsEventParameters>
    {
        public SendMessageToGroupsEvent(IEnumerable<int> groupIds):
            base(ManagementEventCategory.Messaging, ManagementEvent.SendMessageToGroups)
        {
            Details = new SendMessageToGroupsEventParameters() { Ids = groupIds.ToArray() };
        }
    }
}