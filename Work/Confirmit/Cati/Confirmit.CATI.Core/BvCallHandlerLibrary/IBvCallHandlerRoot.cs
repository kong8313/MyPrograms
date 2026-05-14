using System;
using System.Collections.Generic;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Tasks;
using ConfirmitDialerInterface;

namespace BvCallHandlerLibrary
{
    public interface IBvCallHandlerRoot
    {
        /// <summary>
        /// This method calls from Backend service on system startup
        /// </summary>
        void OnStartup();

        /// <summary>
        /// Prepares call for interviewing.
        /// Sends number to dialer if needed and number isn't from the blacklist.
        /// </summary>
        BvInterviewEntity LookupCallForInterviewer(
            BvTasksEntity task,
            BvPersonEntity person,
            IEventDetails eventDetails);

        void CompleteCallAtTaskTerminationIfNeeded(BvTasksEntity task);

        void LogoutFromDialerAtTaskTerminationIfNeeded(
            BvTasksEntity task);

        bool OnWrapUp(
            BvTasksEntity task,
            BvSurveyEntity survey,
            BvInterviewEntity currentInterview,
            BvPersonEntity person,
            BvActiveDialEntity deletedActiveDial,
            bool lookUpForNewCalls,
            WrapUpEvent activityEvent,
            InterviewStatus interviewStatus,
            int attemptNumber,
            out int? linkedInterviewSessionId,
            out TaskContext previosContext);

        bool IsPendingSurveySwitch(BvTasksEntity task);

        void TakeBreak(
            BvTasksEntity task,
            BvSurveyEntity survey,
            DialerAction dialerAction,
            bool force);

        void SwitchSurvey(int dialerId, BvTasksEntity task);
        void TryToSendSetCampaign(int dialerId, long campaignId, int agentId);
        void TryToSendGoReady(int dialerId, long campaignId, long agentId, Func<string> logInfoFunc);
        void TryToSendGoNotReady(int dialerId, long campaignId, long agentId, int? breakTypeId,
            Func<string> logInfoFunc);

        void CancelTransferIfNeed(BvTasksEntity task, BvPersonEntity person, BvActiveDialEntity deletedActiveDial = null);
    }
}