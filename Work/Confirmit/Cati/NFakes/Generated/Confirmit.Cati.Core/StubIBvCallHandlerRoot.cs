using System;
using BvCallHandlerLibrary;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using ConfirmitDialerInterface;
using Confirmit.CATI.Core.Tasks;
using Confirmit.CATI.Core.Misc;

namespace BvCallHandlerLibrary.Fakes
{
    public class StubIBvCallHandlerRoot : IBvCallHandlerRoot 
    {
        private IBvCallHandlerRoot _inner;

        public StubIBvCallHandlerRoot()
        {
            _inner = null;
        }

        public IBvCallHandlerRoot Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void OnStartupDelegate();
        public OnStartupDelegate OnStartup;

        void IBvCallHandlerRoot.OnStartup()
        {

            if (OnStartup != null)
            {
                OnStartup();
            } else if (_inner != null)
            {
                ((IBvCallHandlerRoot)_inner).OnStartup();
            }
        }

        public delegate BvInterviewEntity LookupCallForInterviewerBvTasksEntityBvPersonEntityIEventDetailsDelegate(BvTasksEntity task, BvPersonEntity person, IEventDetails eventDetails);
        public LookupCallForInterviewerBvTasksEntityBvPersonEntityIEventDetailsDelegate LookupCallForInterviewerBvTasksEntityBvPersonEntityIEventDetails;

        BvInterviewEntity IBvCallHandlerRoot.LookupCallForInterviewer(BvTasksEntity task, BvPersonEntity person, IEventDetails eventDetails)
        {


            if (LookupCallForInterviewerBvTasksEntityBvPersonEntityIEventDetails != null)
            {
                return LookupCallForInterviewerBvTasksEntityBvPersonEntityIEventDetails(task, person, eventDetails);
            } else if (_inner != null)
            {
                return ((IBvCallHandlerRoot)_inner).LookupCallForInterviewer(task, person, eventDetails);
            }

            return default(BvInterviewEntity);
        }

        public delegate void CompleteCallAtTaskTerminationIfNeededBvTasksEntityDelegate(BvTasksEntity task);
        public CompleteCallAtTaskTerminationIfNeededBvTasksEntityDelegate CompleteCallAtTaskTerminationIfNeededBvTasksEntity;

        void IBvCallHandlerRoot.CompleteCallAtTaskTerminationIfNeeded(BvTasksEntity task)
        {

            if (CompleteCallAtTaskTerminationIfNeededBvTasksEntity != null)
            {
                CompleteCallAtTaskTerminationIfNeededBvTasksEntity(task);
            } else if (_inner != null)
            {
                ((IBvCallHandlerRoot)_inner).CompleteCallAtTaskTerminationIfNeeded(task);
            }
        }

        public delegate void LogoutFromDialerAtTaskTerminationIfNeededBvTasksEntityDelegate(BvTasksEntity task);
        public LogoutFromDialerAtTaskTerminationIfNeededBvTasksEntityDelegate LogoutFromDialerAtTaskTerminationIfNeededBvTasksEntity;

        void IBvCallHandlerRoot.LogoutFromDialerAtTaskTerminationIfNeeded(BvTasksEntity task)
        {

            if (LogoutFromDialerAtTaskTerminationIfNeededBvTasksEntity != null)
            {
                LogoutFromDialerAtTaskTerminationIfNeededBvTasksEntity(task);
            } else if (_inner != null)
            {
                ((IBvCallHandlerRoot)_inner).LogoutFromDialerAtTaskTerminationIfNeeded(task);
            }
        }

        public delegate bool OnWrapUpBvTasksEntityBvSurveyEntityBvInterviewEntityBvPersonEntityBvActiveDialEntityBooleanWrapUpEventInterviewStatusInt32NullableOfInt32OutTaskContextOutDelegate(BvTasksEntity task, BvSurveyEntity survey, BvInterviewEntity currentInterview, BvPersonEntity person, BvActiveDialEntity deletedActiveDial, bool lookUpForNewCalls, WrapUpEvent activityEvent, InterviewStatus interviewStatus, int attemptNumber, out int? linkedInterviewSessionId, out TaskContext previosContext);
        public OnWrapUpBvTasksEntityBvSurveyEntityBvInterviewEntityBvPersonEntityBvActiveDialEntityBooleanWrapUpEventInterviewStatusInt32NullableOfInt32OutTaskContextOutDelegate OnWrapUpBvTasksEntityBvSurveyEntityBvInterviewEntityBvPersonEntityBvActiveDialEntityBooleanWrapUpEventInterviewStatusInt32NullableOfInt32OutTaskContextOut;

        bool IBvCallHandlerRoot.OnWrapUp(BvTasksEntity task, BvSurveyEntity survey, BvInterviewEntity currentInterview, BvPersonEntity person, BvActiveDialEntity deletedActiveDial, bool lookUpForNewCalls, WrapUpEvent activityEvent, InterviewStatus interviewStatus, int attemptNumber, out int? linkedInterviewSessionId, out TaskContext previosContext)
        {
            linkedInterviewSessionId = default(int?);
            previosContext = default(TaskContext);


            if (OnWrapUpBvTasksEntityBvSurveyEntityBvInterviewEntityBvPersonEntityBvActiveDialEntityBooleanWrapUpEventInterviewStatusInt32NullableOfInt32OutTaskContextOut != null)
            {
                return OnWrapUpBvTasksEntityBvSurveyEntityBvInterviewEntityBvPersonEntityBvActiveDialEntityBooleanWrapUpEventInterviewStatusInt32NullableOfInt32OutTaskContextOut(task, survey, currentInterview, person, deletedActiveDial, lookUpForNewCalls, activityEvent, interviewStatus, attemptNumber, out linkedInterviewSessionId, out previosContext);
            } else if (_inner != null)
            {
                return ((IBvCallHandlerRoot)_inner).OnWrapUp(task, survey, currentInterview, person, deletedActiveDial, lookUpForNewCalls, activityEvent, interviewStatus, attemptNumber, out linkedInterviewSessionId, out previosContext);
            }

            return default(bool);
        }

        public delegate bool IsPendingSurveySwitchBvTasksEntityDelegate(BvTasksEntity task);
        public IsPendingSurveySwitchBvTasksEntityDelegate IsPendingSurveySwitchBvTasksEntity;

        bool IBvCallHandlerRoot.IsPendingSurveySwitch(BvTasksEntity task)
        {


            if (IsPendingSurveySwitchBvTasksEntity != null)
            {
                return IsPendingSurveySwitchBvTasksEntity(task);
            } else if (_inner != null)
            {
                return ((IBvCallHandlerRoot)_inner).IsPendingSurveySwitch(task);
            }

            return default(bool);
        }

        public delegate void TakeBreakBvTasksEntityBvSurveyEntityDialerActionBooleanDelegate(BvTasksEntity task, BvSurveyEntity survey, DialerAction dialerAction, bool force);
        public TakeBreakBvTasksEntityBvSurveyEntityDialerActionBooleanDelegate TakeBreakBvTasksEntityBvSurveyEntityDialerActionBoolean;

        void IBvCallHandlerRoot.TakeBreak(BvTasksEntity task, BvSurveyEntity survey, DialerAction dialerAction, bool force)
        {

            if (TakeBreakBvTasksEntityBvSurveyEntityDialerActionBoolean != null)
            {
                TakeBreakBvTasksEntityBvSurveyEntityDialerActionBoolean(task, survey, dialerAction, force);
            } else if (_inner != null)
            {
                ((IBvCallHandlerRoot)_inner).TakeBreak(task, survey, dialerAction, force);
            }
        }

        public delegate void SwitchSurveyInt32BvTasksEntityDelegate(int dialerId, BvTasksEntity task);
        public SwitchSurveyInt32BvTasksEntityDelegate SwitchSurveyInt32BvTasksEntity;

        void IBvCallHandlerRoot.SwitchSurvey(int dialerId, BvTasksEntity task)
        {

            if (SwitchSurveyInt32BvTasksEntity != null)
            {
                SwitchSurveyInt32BvTasksEntity(dialerId, task);
            } else if (_inner != null)
            {
                ((IBvCallHandlerRoot)_inner).SwitchSurvey(dialerId, task);
            }
        }

        public delegate void TryToSendSetCampaignInt32Int64Int32Delegate(int dialerId, long campaignId, int agentId);
        public TryToSendSetCampaignInt32Int64Int32Delegate TryToSendSetCampaignInt32Int64Int32;

        void IBvCallHandlerRoot.TryToSendSetCampaign(int dialerId, long campaignId, int agentId)
        {

            if (TryToSendSetCampaignInt32Int64Int32 != null)
            {
                TryToSendSetCampaignInt32Int64Int32(dialerId, campaignId, agentId);
            } else if (_inner != null)
            {
                ((IBvCallHandlerRoot)_inner).TryToSendSetCampaign(dialerId, campaignId, agentId);
            }
        }

        public delegate void TryToSendGoReadyInt32Int64Int64FuncOfStringDelegate(int dialerId, long campaignId, long agentId, Func<string> logInfoFunc);
        public TryToSendGoReadyInt32Int64Int64FuncOfStringDelegate TryToSendGoReadyInt32Int64Int64FuncOfString;

        void IBvCallHandlerRoot.TryToSendGoReady(int dialerId, long campaignId, long agentId, Func<string> logInfoFunc)
        {

            if (TryToSendGoReadyInt32Int64Int64FuncOfString != null)
            {
                TryToSendGoReadyInt32Int64Int64FuncOfString(dialerId, campaignId, agentId, logInfoFunc);
            } else if (_inner != null)
            {
                ((IBvCallHandlerRoot)_inner).TryToSendGoReady(dialerId, campaignId, agentId, logInfoFunc);
            }
        }

        public delegate void TryToSendGoNotReadyInt32Int64Int64NullableOfInt32FuncOfStringDelegate(int dialerId, long campaignId, long agentId, int? breakTypeId, Func<string> logInfoFunc);
        public TryToSendGoNotReadyInt32Int64Int64NullableOfInt32FuncOfStringDelegate TryToSendGoNotReadyInt32Int64Int64NullableOfInt32FuncOfString;

        void IBvCallHandlerRoot.TryToSendGoNotReady(int dialerId, long campaignId, long agentId, int? breakTypeId, Func<string> logInfoFunc)
        {

            if (TryToSendGoNotReadyInt32Int64Int64NullableOfInt32FuncOfString != null)
            {
                TryToSendGoNotReadyInt32Int64Int64NullableOfInt32FuncOfString(dialerId, campaignId, agentId, breakTypeId, logInfoFunc);
            } else if (_inner != null)
            {
                ((IBvCallHandlerRoot)_inner).TryToSendGoNotReady(dialerId, campaignId, agentId, breakTypeId, logInfoFunc);
            }
        }

        public delegate void CancelTransferIfNeedBvTasksEntityBvPersonEntityBvActiveDialEntityDelegate(BvTasksEntity task, BvPersonEntity person, BvActiveDialEntity deletedActiveDial);
        public CancelTransferIfNeedBvTasksEntityBvPersonEntityBvActiveDialEntityDelegate CancelTransferIfNeedBvTasksEntityBvPersonEntityBvActiveDialEntity;

        void IBvCallHandlerRoot.CancelTransferIfNeed(BvTasksEntity task, BvPersonEntity person, BvActiveDialEntity deletedActiveDial)
        {

            if (CancelTransferIfNeedBvTasksEntityBvPersonEntityBvActiveDialEntity != null)
            {
                CancelTransferIfNeedBvTasksEntityBvPersonEntityBvActiveDialEntity(task, person, deletedActiveDial);
            } else if (_inner != null)
            {
                ((IBvCallHandlerRoot)_inner).CancelTransferIfNeed(task, person, deletedActiveDial);
            }
        }

    }
}