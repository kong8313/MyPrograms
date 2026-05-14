using System;
using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.Logging;
using ConfirmitDialerInterface;
using TransferState = ConfirmitDialerInterface.TransferState;

namespace Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging
{
    /// <summary>
    /// Should be used for the events without additional parameters.
    /// </summary>
    [Serializable]
    public class NoParameters : InterviewerActivityEventDetailsBase
    {
    }

    [Serializable]
    public class LoginEventParameters : InterviewerActivityEventDetailsBase
    {
        public PersonInfo PersonInfo;
        public DiallerInfo DialerInfo;
        public ConsoleDescription ConsoleDescription;
    }

    [Serializable]
    public class CallDeliveringParameters : InterviewerActivityEventDetailsBase
    {
        public string Description;
        public int? CallId;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.Login)]
    public class LoginEvent : InterviewerActivityEventBase<LoginEventParameters>
    {
        public LoginEvent()
            : base(InterviewerActivityEventType.Login)
        {
        }

        public void Save(int interviewerSid)
        {
            InterviewerSid = interviewerSid;

            Save();
        }
    }

    [Serializable]
    public class LoginToDialerEventParameters : InterviewerActivityEventDetailsBase
    {
        public int DialerId;
        public string ExtensionNumber;
        public bool IsPredictive;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.LoginToDialer)]
    public class LoginToDialerEvent : InterviewerActivityEventBase<LoginToDialerEventParameters>
    {
        public LoginToDialerEvent()
            : base(InterviewerActivityEventType.LoginToDialer)
        {
        }

        public void Save(
            int interviewerSid,
            int? surveySid,
            string surveyName,
            int dialerId,
            string extensionNumber,
            bool isPredictive)
        {
            InterviewerSid = interviewerSid;
            SurveySid = surveySid;
            SurveyName = surveyName;

            Details.DialerId = dialerId;
            Details.ExtensionNumber = extensionNumber;
            Details.IsPredictive = isPredictive;

            Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.ForcedLogout)]
    public class ForcedLogoutEvent : InterviewerActivityEventBase<NoParameters>
    {
        public ForcedLogoutEvent()
            : base(InterviewerActivityEventType.ForcedLogout)
        {
        }

        public void Save(int interviewerSid)
        {
            InterviewerSid = interviewerSid;

            Save();
        }
    }

    [Serializable]
    public class SetPendingLogoutEventParameters : InterviewerActivityEventDetailsBase
    {
        public bool Logout;
        public byte LoggedInToDialerState;
        public bool IsLoginRCToDialer;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.SetPendingLogout)]
    public class SetPendingLogoutEvent : InterviewerActivityEventBase<SetPendingLogoutEventParameters>
    {
        public SetPendingLogoutEvent()
            : base(InterviewerActivityEventType.SetPendingLogout)
        {
        }

        public void Save(
            int interviewerSid,
            int? surveySid,
            string surveyName,
            bool logout,
            int interviewId,
            byte loggedInToDialerState,
            bool isLoginRCToDialer)
        {
            InterviewerSid = interviewerSid;
            SurveySid = surveySid;
            SurveyName = surveyName;
            InterviewId = interviewId;

            Details.Logout = logout;
            Details.LoggedInToDialerState = loggedInToDialerState;
            Details.IsLoginRCToDialer = isLoginRCToDialer;

            Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.ConfirmLogout)]
    public class ConfirmLogoutEvent : InterviewerActivityEventBase<NoParameters>
    {
        public ConfirmLogoutEvent()
            : base(InterviewerActivityEventType.ConfirmLogout)
        {
        }

        public void Save(int interviewerSid)
        {
            InterviewerSid = interviewerSid;

            Save();
        }
    }

    [Serializable]
    public class TerminateTaskEventParameters : InterviewerActivityEventDetailsBase
    {
        public BvTasksEntity Task { get; set; }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.TerminateTaskFromConsoleEvent)]
    public class TerminateTaskFromConsoleEvent : InterviewerActivityEventBase<TerminateTaskEventParameters>
    {
        public TerminateTaskFromConsoleEvent()
            : base(InterviewerActivityEventType.TerminateTaskFromConsoleEvent)
        {
        }

        public void Save(int interviewerSid)
        {
            InterviewerSid = interviewerSid;

            Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.TerminateTaskByAutoLogout)]
    public class TerminateTaskByAutoLogoutEvent : InterviewerActivityEventBase<TerminateTaskEventParameters>
    {
        public TerminateTaskByAutoLogoutEvent()
            : base(InterviewerActivityEventType.TerminateTaskByAutoLogout)
        {
        }
    }

    [Serializable]
    public class SetPendingBreakStatusEventParameters : InterviewerActivityEventDetailsBase
    {
        public PendingBreakStatus PendingBreakStatus;

        public LoginState PreviousLoginState;

        public int? BreakType;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.SetPendingBreakStatus)]
    public class SetPendingBreakStatusEvent : InterviewerActivityEventBase<SetPendingBreakStatusEventParameters>
    {
        public SetPendingBreakStatusEvent()
            : base(InterviewerActivityEventType.SetPendingBreakStatus)
        {
        }

        public void Save(int interviewerSid,
            int? surveySid,
            string surveyName,
            PendingBreakStatus pendingBreakStatus,
            int interviewId,
            LoginState previousLoginState, int? breakType)
        {
            InterviewerSid = interviewerSid;
            SurveySid = surveySid;
            SurveyName = surveyName;
            InterviewId = interviewId;

            Details.PendingBreakStatus = pendingBreakStatus;
            Details.PreviousLoginState = previousLoginState;
            Details.BreakType = breakType;

            Save();
        }
    }

    [Serializable]
    public class CheckTextSpellingEventParameters : InterviewerActivityEventDetailsBase
    {
        public int LanguageId;
        public int TextBlockLength;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.CheckTextSpelling)]
    public class CheckTextSpellingEvent : InterviewerActivityEventBase<CheckTextSpellingEventParameters>
    {
        public CheckTextSpellingEvent()
            : base(InterviewerActivityEventType.CheckTextSpelling)
        {
        }

        public void Save(int languageId,
                         int textBlockLength)
        {
            Details.LanguageId = languageId;
            Details.TextBlockLength = textBlockLength;
            Save();
        }
    }

    [Serializable]
    public class GetInterviewAppointmentEventParameters : InterviewerActivityEventDetailsBase
    {
        public int InterviewId;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.GetInterviewAppointment)]
    public class GetInterviewAppointmentEvent : InterviewerActivityEventBase<GetInterviewAppointmentEventParameters>
    {
        public GetInterviewAppointmentEvent()
            : base(InterviewerActivityEventType.GetInterviewAppointment)
        {
        }

        public void Save(string projectId,
                         int interviewId)
        {
            SurveyName = projectId;
            Details.InterviewId = interviewId;
            Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.GetSurveyLanguages)]
    public class GetSurveyLanguagesEvent : InterviewerActivityEventBase<NoParameters>
    {
        public GetSurveyLanguagesEvent()
            : base(InterviewerActivityEventType.GetSurveyLanguages)
        {
        }

        public void Save(string surveyName)
        {
            SurveyName = surveyName;
            Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.GetAllAppointmentList)]
    public class GetAllAppointmentListEvent : InterviewerActivityEventBase<NoParameters>
    {
        public GetAllAppointmentListEvent()
            : base(InterviewerActivityEventType.GetAllAppointmentList)
        {
        }

        public new void Save()
        {
            base.Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.GetSurveyInterviews)]
    public class GetSurveyInterviewsEvent : InterviewerActivityEventBase<NoParameters>
    {
        public GetSurveyInterviewsEvent()
            : base(InterviewerActivityEventType.GetSurveyInterviews)
        {
        }

        public new void Save()
        {
            base.Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.GetMessages)]
    public class GetMessagesEvent : InterviewerActivityEventBase<NoParameters>
    {
        public GetMessagesEvent()
            : base(InterviewerActivityEventType.GetMessages)
        {
        }

        public new void Save()
        {
            base.Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.GetForceOpenendReview)]
    public class GetForceOpenendReviewEvent : InterviewerActivityEventBase<NoParameters>
    {
        public GetForceOpenendReviewEvent()
            : base(InterviewerActivityEventType.GetForceOpenendReview)
        {
        }

        public new void Save()
        {
            base.Save();
        }
    }

    [Serializable]
    public class GetInterviewHistoryEventParameters : InterviewerActivityEventDetailsBase
    {
        public int LanguageId;
        public string RespondentIdentity;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.GetInterviewHistory)]
    public class GetInterviewHistoryEvent : InterviewerActivityEventBase<GetInterviewHistoryEventParameters>
    {
        public GetInterviewHistoryEvent()
            : base(InterviewerActivityEventType.GetInterviewHistory)
        {
        }

        public void Save(string projectId, string respondentIdentity, int languageId)
        {
            SurveyName = projectId;
            Details.RespondentIdentity = respondentIdentity;
            Details.LanguageId = languageId;
            Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.GetOpenedSurveys)]
    public class GetOpenedSurveysEvent : InterviewerActivityEventBase<NoParameters>
    {
        public GetOpenedSurveysEvent()
            : base(InterviewerActivityEventType.GetOpenedSurveys)
        {
        }

        public new void Save()
        {
            base.Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.ContinueWorkAfterBreak)]
    public class ContinueWorkAfterBreakEvent : InterviewerActivityEventBase<NoParameters>
    {
        public ContinueWorkAfterBreakEvent()
            : base(InterviewerActivityEventType.ContinueWorkAfterBreak)
        {
        }

        public void Save(
            int interviewerSid)
        {
            InterviewerSid = interviewerSid;

            Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.TakeBreak)]
    public class TakeBreakEvent : InterviewerActivityEventBase<NoParameters>
    {
        public TakeBreakEvent()
            : base(InterviewerActivityEventType.TakeBreak)
        {
        }

        public void Save(
            int interviewerSid)
        {
            InterviewerSid = interviewerSid;

            Save();
        }
    }

    [Serializable]
    public class UpdateInterviewerModeEventParameters : InterviewerActivityEventDetailsBase
    {
        public int PersonMode;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.UpdateInterviewerMode)]
    public class UpdateInterviewerModeEvent : InterviewerActivityEventBase<UpdateInterviewerModeEventParameters>
    {
        public UpdateInterviewerModeEvent()
            : base(InterviewerActivityEventType.UpdateInterviewerMode)
        {
        }

        public void Save(
            int interviewerSid,
            int personMode)
        {
            InterviewerSid = interviewerSid;

            Details.PersonMode = personMode;

            Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.StartInterview)]
    public class StartInterviewEvent : InterviewerActivityEventBase<NoParameters>
    {
        public StartInterviewEvent()
            : base(InterviewerActivityEventType.StartInterview)
        {
        }

        public void Save(
            int interviewerSid,
            int interviewId,
            int? surveySid,
            string surveyName)
        {
            InterviewerSid = interviewerSid;
            InterviewId = interviewId;
            SurveySid = surveySid;
            SurveyName = surveyName;

            Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.CreateNewInterview)]
    public class CreateNewInterviewEvent : InterviewerActivityEventBase<NoParameters>
    {
        public CreateNewInterviewEvent()
            : base(InterviewerActivityEventType.CreateNewInterview)
        {
        }

        public void Save(
            int interviewerSid,
            int interviewId,
            int? surveySid,
            string surveyName)
        {
            InterviewerSid = interviewerSid;
            InterviewId = interviewId;
            SurveySid = surveySid;
            SurveyName = surveyName;

            Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.GetCall)]
    public class GetCallEvent : InterviewerActivityEventBase<CallDeliveringParameters>
    {
        public GetCallEvent()
            : base(InterviewerActivityEventType.GetCall)
        {
        }

        public void Save(
            int interviewerSid,
            int interviewId,
            int? surveySid,
            string surveyName,
            int? callId)
        {
            InterviewerSid = interviewerSid;
            InterviewId = interviewId;
            SurveySid = surveySid;
            SurveyName = surveyName;
            Details.CallId = callId;

            Save();
        }
    }


    [Serializable]
    public class UrlGeneratedInGetStateEventParameters : InterviewerActivityEventDetailsBase
    {
        public int? CallId;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.UrlGeneratedInGetState)]
    public class UrlGeneratedInGetStateEvent : InterviewerActivityEventBase<UrlGeneratedInGetStateEventParameters>
    {
        public UrlGeneratedInGetStateEvent()
            : base(InterviewerActivityEventType.UrlGeneratedInGetState)
        {
        }

        public void Save(
            int interviewerSid,
            int interviewId,
            int? callId,
            int? surveySid,
            string surveyName)
        {
            InterviewerSid = interviewerSid;
            InterviewId = interviewId;
            SurveySid = surveySid;
            SurveyName = surveyName;

            Details.CallId = callId;

            Save();
        }
    }

    [Serializable]
    public class SetInterviewAppointmentEventDetails : InterviewerActivityEventDetailsBase
    {
        public string ContactName;
        public DateTime Time;
        public DateTime? ExpirationTime;
        public int? State;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.SetInterviewAppointment)]
    public class SetInterviewAppointmentEvent : InterviewerActivityEventBase<SetInterviewAppointmentEventDetails>
    {
        public SetInterviewAppointmentEvent()
            : base(InterviewerActivityEventType.SetInterviewAppointment)
        {
        }

        public void Save(
            int interviewerSid,
            int? surveySid,
            string surveyName,
            string contactName,
            DateTime time,
            DateTime? expirationTime,
            int? state,
            int interviewId)
        {
            InterviewerSid = interviewerSid;
            SurveySid = surveySid;
            SurveyName = surveyName;
            InterviewId = interviewId;

            Details.ContactName = contactName;
            Details.Time = time;
            Details.ExpirationTime = expirationTime;
            Details.State = state;

            Save();
        }
    }

    [Serializable]
    public class DialEventParameters : InterviewerActivityEventDetailsBase
    {
        public int DialerId;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.Dial)]
    public class DialEvent : InterviewerActivityEventBase<DialEventParameters>
    {
        public DialEvent()
            : base(InterviewerActivityEventType.Dial)
        {
        }

        public new void Save()
        {
            base.Save();
        }
    }

    [Serializable]
    public class RedialEventParameters : InterviewerActivityEventDetailsBase
    {
        public int DialerId;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.Redial)]
    public class RedialEvent : InterviewerActivityEventBase<RedialEventParameters>
    {
        public RedialEvent() : base(InterviewerActivityEventType.Redial) { }

        public new void Save()
        {
            base.Save();
        }
    }

    [Serializable]
    public class HangupEventParameters : InterviewerActivityEventDetailsBase
    {
        public int DialerId;
        public int Initiator;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.Hangup)]
    public class HangupEvent : InterviewerActivityEventBase<HangupEventParameters>
    {
        public HangupEvent()
            : base(InterviewerActivityEventType.Hangup)
        {
        }

        public new void Save()
        {
            base.Save();
        }
    }

    [Serializable]
    public class WrapUpEventParameters : InterviewerActivityEventDetailsBase
    {
        public CompletedInterviewDetails InterviewDetails;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.WrapUp)]
    public class WrapUpEvent : InterviewerActivityEventBase<WrapUpEventParameters>
    {
        public WrapUpEvent()
            : base(InterviewerActivityEventType.WrapUp)
        {
        }

        public new void Save()
        {
            base.Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.LogoutProcess)]
    public class LogoutProcessEvent : InterviewerActivityEventBase<NoParameters>
    {
        public LogoutProcessEvent()
            : base(InterviewerActivityEventType.LogoutProcess)
        {
        }

        public void Save(
            int interviewerSid)
        {
            InterviewerSid = interviewerSid;

            Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.LogoutOnWrapUp)]
    public class LogoutOnWrapUpEvent : InterviewerActivityEventBase<NoParameters>
    {
        public LogoutOnWrapUpEvent()
            : base(InterviewerActivityEventType.LogoutOnWrapUp)
        {
        }

        public void Save(
            int interviewerSid,
            int interviewId,
            int surveySid,
            string surveyName)
        {
            InterviewerSid = interviewerSid;
            InterviewId = interviewId;
            SurveySid = surveySid;
            SurveyName = surveyName;

            Save();
        }
    }

    [Serializable]
    public class StartInterviewProcessEventParameters : InterviewerActivityEventDetailsBase
    {
        public int DialerId;
        public int DiallingMode;
        public int LoginToDiallerState;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.StartInterviewProcess)]
    public class StartInterviewProcessEvent : InterviewerActivityEventBase<StartInterviewProcessEventParameters>
    {
        public StartInterviewProcessEvent()
            : base(InterviewerActivityEventType.StartInterviewProcess)
        {
        }

        public void Save(
            int interviewerSid,
            int? surveySid,
            string surveyName,
            string phoneNumber,
            int interviewId,
            int dialerId,
            int diallingMode,
            int loginToDiallerState)
        {
            InterviewerSid = interviewerSid;
            SurveySid = surveySid;
            SurveyName = surveyName;
            PhoneNumber = phoneNumber;
            InterviewId = interviewId;

            Details.DialerId = dialerId;
            Details.DiallingMode = diallingMode;
            Details.LoginToDiallerState = loginToDiallerState;

            Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.StartInterviewProcessNoCalls)]
    public class StartInterviewProcessNoCallsEvent : InterviewerActivityEventBase<NoParameters>
    {
        public StartInterviewProcessNoCallsEvent()
            : base(InterviewerActivityEventType.StartInterviewProcessNoCalls)
        {
        }

        public void Save(
            int interviewerSid)
        {
            InterviewerSid = interviewerSid;

            Save();
        }
    }

    [Serializable]
    public class SaveInterviewHistoryAndControlDataEventParameters : InterviewerActivityEventDetailsBase
    {
        public bool SavedInWrapup = false;
        public string Status;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.SaveInterviewHistoryAndControlData)]
    public class SaveInterviewHistoryAndControlDataEvent : InterviewerActivityEventBase<SaveInterviewHistoryAndControlDataEventParameters>
    {
        public SaveInterviewHistoryAndControlDataEvent()
            : base(InterviewerActivityEventType.SaveInterviewHistoryAndControlData)
        {
        }

        public void Save(
            int interviewerSid,
            int? surveySid,
            string surveyName,
            int interviewId,
            string status)
        {
            InterviewerSid = interviewerSid;
            SurveySid = surveySid;
            SurveyName = surveyName;
            InterviewId = interviewId;

            Details.Status = status;

            Save();
        }
    }

    [Serializable]
    public class TransferStartEventParameters : InterviewerActivityEventDetailsBase
    {
        public TransferOptions Options;
        public string TransferId;
        public int? TransferGroupId;
        public TransferGroupBehavior? TransferGroupBehavior;
    }

    public class TransferSetConnectionStateEventParameters : InterviewerActivityEventDetailsBase
    {
        public string TransferId;
    }

    public class TransferFinishEventParameters : InterviewerActivityEventDetailsBase
    {
        public string TransferId;
    }

    [InterviewerActivityEvent(InterviewerActivityEventType.TransferStart)]
    public class TransferStartEvent : InterviewerActivityEventBase<TransferStartEventParameters>
    {
        public TransferStartEvent(TransferOptions options)
            : base(InterviewerActivityEventType.TransferStart)
        {
            Details.Options = options;
        }
    }

    [InterviewerActivityEvent(InterviewerActivityEventType.TransferComplete)]
    public class TransferCompleteEvent : InterviewerActivityEventBase<TransferFinishEventParameters>
    {
        public TransferCompleteEvent()
            : base(InterviewerActivityEventType.TransferComplete)
        {
        }
    }

    [InterviewerActivityEvent(InterviewerActivityEventType.TransferSetConnectionState)]
    public class TransferSetConnectionStateEvent : InterviewerActivityEventBase<TransferSetConnectionStateEventParameters>
    {
        public TransferSetConnectionStateEvent()
            : base(InterviewerActivityEventType.TransferSetConnectionState)
        {
        }
    }
    

    [InterviewerActivityEvent(InterviewerActivityEventType.TransferCancel)]
    public class TransferCancelEvent : InterviewerActivityEventBase<TransferFinishEventParameters>
    {
        public TransferCancelEvent()
            : base(InterviewerActivityEventType.TransferCancel)
        {
        }
    }

    [Serializable]
    public class StartPlaybackEventParameters : InterviewerActivityEventDetailsBase
    {
        public int DialerId;
        public string SoundFileName;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.StartPlayback)]
    public class StartPlaybackEvent : InterviewerActivityEventBase<StartPlaybackEventParameters>
    {
        public StartPlaybackEvent()
            : base(InterviewerActivityEventType.StartPlayback)
        {
        }

        public void Save(
            int interviewerSid,
            int interviewId,
            int surveySid,
            string surveyName,
            int dialerId,
            string soundFileName)
        {
            InterviewerSid = interviewerSid;
            InterviewId = interviewId;
            SurveySid = surveySid;
            SurveyName = surveyName;

            Details.DialerId = dialerId;
            Details.SoundFileName = soundFileName;

            Save();
        }
    }

    [Serializable]
    public class StopPlaybackEventParameters : InterviewerActivityEventDetailsBase
    {
        public int DialerId;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.StopPlayback)]
    public class StopPlaybackEvent : InterviewerActivityEventBase<StopPlaybackEventParameters>
    {
        public StopPlaybackEvent()
            : base(InterviewerActivityEventType.StopPlayback)
        {
        }

        public void Save(
            int interviewerSid,
            int interviewId,
            int surveySid,
            string surveyName,
            int dialerId)
        {
            InterviewerSid = interviewerSid;
            InterviewId = interviewId;
            SurveySid = surveySid;
            SurveyName = surveyName;

            Details.DialerId = dialerId;

            Save();
        }
    }

    [Serializable]
    public class PauseOrResumePlaybackEventParameters : InterviewerActivityEventDetailsBase
    {
        public int DialerId;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.PauseOrResumePlayback)]
    public class PauseOrResumePlaybackEvent : InterviewerActivityEventBase<PauseOrResumePlaybackEventParameters>
    {
        public PauseOrResumePlaybackEvent()
            : base(InterviewerActivityEventType.PauseOrResumePlayback)
        {
        }

        public void Save(
            int interviewerSid,
            int interviewId,
            int surveySid,
            string surveyName,
            int dialerId)
        {
            InterviewerSid = interviewerSid;
            InterviewId = interviewId;
            SurveySid = surveySid;
            SurveyName = surveyName;

            Details.DialerId = dialerId;

            Save();
        }
    }

    [Serializable]
    public class ToggleInterviewerListensToPlaybackOrRespondentEventParameters : InterviewerActivityEventDetailsBase
    {
        public int DialerId;
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.ToggleInterviewerListensToPlaybackOrRespondent)]
    public class ToggleInterviewerListensToPlaybackOrRespondentEvent : InterviewerActivityEventBase<ToggleInterviewerListensToPlaybackOrRespondentEventParameters>
    {
        public ToggleInterviewerListensToPlaybackOrRespondentEvent()
            : base(InterviewerActivityEventType.ToggleInterviewerListensToPlaybackOrRespondent)
        {
        }

        public void Save(int dialerId)
        {
            Details.DialerId = dialerId;

            Save();
        }
    }

    [Serializable]
    public class IncrementFailedLoginAttemptsEventParameters : InterviewerActivityEventDetailsBase
    {
        public int CurrentFailedLoginAttempts { get; set; }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.IncrementFailedLoginAttempts)]
    public class IncrementFailedLoginAttemptsEvent : InterviewerActivityEventBase<IncrementFailedLoginAttemptsEventParameters>
    {
        public IncrementFailedLoginAttemptsEvent()
            : base(InterviewerActivityEventType.IncrementFailedLoginAttempts)
        {
        }

        public void Save(int personSid, int failedLoginAttempts)
        {
            InterviewerSid = personSid;
            Details.CurrentFailedLoginAttempts = failedLoginAttempts;

            Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.ResetFailedLoginAttempts)]
    public class ResetFailedLoginAttemptsEvent : InterviewerActivityEventBase<NoParameters>
    {
        public ResetFailedLoginAttemptsEvent()
            : base(InterviewerActivityEventType.ResetFailedLoginAttempts)
        {
        }

        public void Save(int personSid)
        {
            InterviewerSid = personSid;

            Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.InterviewerLocked)]
    public class InterviewerLockedEvent : InterviewerActivityEventBase<NoParameters>
    {
        public InterviewerLockedEvent()
            : base(InterviewerActivityEventType.InterviewerLocked)
        {
        }

        public void Save(int personSid)
        {
            InterviewerSid = personSid;

            Save();
        }
    }

    [Serializable]
    public class GenerateAuthenticationKeyEventParameters : InterviewerActivityEventDetailsBase
    {
        public Guid GeneratedKey { get; set; }
        public Guid? OldKey { get; set; }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.GenerateAuthenticationKey)]
    public class GenerateAuthenticationKeyEvent : InterviewerActivityEventBase<GenerateAuthenticationKeyEventParameters>
    {
        public GenerateAuthenticationKeyEvent()
            : base(InterviewerActivityEventType.GenerateAuthenticationKey)
        {
        }

        public void Save(int personSid, Guid? oldKey, Guid generatedKey)
        {
            InterviewerSid = personSid;
            Details.OldKey = oldKey;
            Details.GeneratedKey = generatedKey;

            Save();
        }
    }

    [Serializable]
    public class OnDialerScreenPopEventParameters : InterviewerActivityEventDetailsBase
    {
        public int CallId { get; set; }
        public DialingMode CallDialingMode { get; set; }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.OnDialerScreenPopEvent)]
    public class OnDialerScreenPopEvent : InterviewerActivityEventBase<OnDialerScreenPopEventParameters>
    {
        public OnDialerScreenPopEvent()
            : base(InterviewerActivityEventType.OnDialerScreenPopEvent)
        {

        }

        public void Save(int interviewId, int callId, DialingMode callDialingMode)
        {
            InterviewId = interviewId;

            Details.CallId = callId;
            Details.CallDialingMode = callDialingMode;

            Save();
        }
    }

    [Serializable]
    public class NotifyOutcomeParameters : InterviewerActivityEventDetailsBase
    {
        public int DialerId { get; set; }
        public string TenantId { get; set; }
        public long CampaignId { get; set; }
        public long AgentId { get; set; }
        public int InterviewId { get; set; }
        public long CallId { get; set; }
        public long RawOutcome { get; set; }
        public CallOutcome TranslatedOutcome { get; set; }
        public bool IsRedialEvent { get; set; }
        public bool IsPendingInboundCall { get; set; }
        public string Note { get; set; }
        public string DialerCallerId { get; set; }
        public int RingTime { get; set; }
        public KeyValuePair<string, string>[] CallOutcomeMetadata { get; set; }
    }
    
    public abstract class OnDialerCallEventBase : InterviewerActivityEventBase<NotifyOutcomeParameters>
    {
        protected OnDialerCallEventBase(InterviewerActivityEventType eventTypeId)
            : base(eventTypeId)
        {
        }

        public void Save(string note)
        {
            Details.Note = note;
            base.Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.OnDialerCallConnectedEvent)]
    public class OnDialerCallConnectedEvent : OnDialerCallEventBase
    {
        public OnDialerCallConnectedEvent()
            : base(InterviewerActivityEventType.OnDialerCallConnectedEvent)
        {
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.OnDialerCallNotConnectedEvent)]
    public class OnDialerCallNotConnectedEvent : OnDialerCallEventBase
    {
        public OnDialerCallNotConnectedEvent()
            : base(InterviewerActivityEventType.OnDialerCallNotConnectedEvent)
        {}

        protected override IEnumerable<CustomField> GetEventCustomFields()
        {
            return new List<CustomField>
            {
                new CustomField("DialerId", Details.DialerId),
                new CustomField("Outcome", Details.TranslatedOutcome.ToString()),
            };
        }
    }

    [Serializable]
    public class OnDialerTransferStateEventDetails : InterviewerActivityEventDetailsBase
    {
        public TransferState CurrentState { get; set; }
    }

    [Serializable]
    public class OnDialerNotifyAgentStateEventDetails : InterviewerActivityEventDetailsBase
    {
        public LoginState CurrentState { get; set; }
        public LoginState CurrentDialerState { get; set; }
        public AgentStateMsgs NotificationState { get; set; }
    }

    [Serializable]
    public class OnDialerNotifyCallDroppedByRespondentEventDetails : InterviewerActivityEventDetailsBase
    {
        public BvTasksEntity Task { get; set; }
        public AgentType? AgentType { get; set; }
        public bool IsTransferedCall { get; set; }
    }

    [InterviewerActivityEvent(InterviewerActivityEventType.OnDialerTransferStateEvent)]
    public class OnDialerTransferStateEvent : InterviewerActivityEventBase<OnDialerTransferStateEventDetails>
    {
        public OnDialerTransferStateEvent()
            : base(InterviewerActivityEventType.OnDialerTransferStateEvent)
        {

        }

        public new void Save()
        {
            base.Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.OnDialerNotifyAgentStateEvent)]
    public class OnDialerNotifyAgentStateEvent : InterviewerActivityEventBase<OnDialerNotifyAgentStateEventDetails>
    {
        public OnDialerNotifyAgentStateEvent()
            : base(InterviewerActivityEventType.OnDialerNotifyAgentStateEvent)
        {

        }

        public new void Save()
        {
            base.Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.OnDialerNotifyCallDroppedByRespondentEvent)]
    public class OnDialerNotifyCallDroppedByRespondentEvent : InterviewerActivityEventBase<OnDialerNotifyCallDroppedByRespondentEventDetails>
    {
        public OnDialerNotifyCallDroppedByRespondentEvent(string companyId, long campaignId, long agentId, long callId)
            : base(InterviewerActivityEventType.OnDialerNotifyCallDroppedByRespondentEvent)
        {
            CompanyId = int.Parse(companyId);
            SurveySid = (int)campaignId;
            InterviewerSid = (int)agentId;
        }

        public void Save(int? interviewId)
        {
            InterviewId = interviewId;
            Save();
        }
    }

    [Serializable]
    public class ExecuteSchedulingScriptEventDetails : InterviewerActivityEventDetailsBase
    {
        public SchedulingScriptExecutionReason ExecutionReason { get; set; }
        public int ScheduleId { get; set; }

        public string ExtendedStatus { get; set; }
    }
    
    [InterviewerActivityEventAttribute(InterviewerActivityEventType.ExecuteSchedulingScriptEvent)]
    public class ExecuteSchedulingScriptEvent : InterviewerActivityEventBase<ExecuteSchedulingScriptEventDetails>
    {
        public ExecuteSchedulingScriptEvent()
            : base(InterviewerActivityEventType.ExecuteSchedulingScriptEvent)
        {

        }

        public new void Save()
        {
            base.Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.InsertInterviewEvent)]
    public class InsertInterviewEvent : InterviewerActivityEventBase<NoParameters>
    {
        public InsertInterviewEvent()
            : base(InterviewerActivityEventType.InsertInterviewEvent)
        {

        }

        public new void Save()
        {
            base.Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.UpdateInterviewEvent)]
    public class UpdateInterviewEvent : InterviewerActivityEventBase<NoParameters>
    {
        public UpdateInterviewEvent()
            : base(InterviewerActivityEventType.UpdateInterviewEvent)
        {

        }

        public new void Save()
        {
            base.Save();
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.KeepAliveEvent)]
    public class KeepAliveEvent : InterviewerActivityEventBase<NoParameters>
    {
        public KeepAliveEvent()
            : base(InterviewerActivityEventType.KeepAliveEvent)
        {

        }

        public new void SaveIfEventTookLongerThan(int durationInMilliseconds)
        {
            base.SaveIfEventTookLongerThan(durationInMilliseconds);
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.GetStateEvent)]
    public class GetStateEvent : InterviewerActivityEventBase<NoParameters>
    {
        public GetStateEvent()
            : base(InterviewerActivityEventType.GetStateEvent)
        {

        }

        public new void SaveIfEventTookLongerThan(int durationInMilliseconds)
        {
            base.SaveIfEventTookLongerThan(durationInMilliseconds);
        }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.GetCatiCompanyIdEvent)]
    public class GetCatiCompanyIdEvent : InterviewerActivityEventBase<NoParameters>
    {
        public GetCatiCompanyIdEvent()
            : base(InterviewerActivityEventType.GetCatiCompanyIdEvent)
        {

        }

        public void Save(int companyId, int personSid)
        {
            CompanyId = companyId;
            InterviewerSid = personSid;

            Save();
        }
    }
    [InterviewerActivityEventAttribute(InterviewerActivityEventType.ChangeInterviewerPasswordEvent)]
    public class ChangeInterviewerPasswordEvent : InterviewerActivityEventBase<NoParameters>
    {
        public ChangeInterviewerPasswordEvent()
            : base(InterviewerActivityEventType.ChangeInterviewerPasswordEvent)
        {
        }

        public void Save(int companyId, int personSid)
        {
            CompanyId = companyId;
            InterviewerSid = personSid;

            Save();
        }
    }

    [Serializable]
    public class SurveySwitchEventDetails : InterviewerActivityEventDetailsBase
    {
        public int OldSurveySid { get; set; }
        public int NewSurveySid { get; set; }
        public string NewSurveyName { get; set; }
        public string Result { get; set; }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.SurveySwitch)]
    public class SurveySwitchEvent : InterviewerActivityEventBase<SurveySwitchEventDetails>
    {
        public SurveySwitchEvent()
            : base(InterviewerActivityEventType.SurveySwitch)
        {
        }

        public new void Save()
        {
            base.Save();
        }
    }

    [Serializable]
    public class SetNextLinkedInterviewEventParameters : InterviewerActivityEventDetailsBase
    {
        public string ProjectId { get; set; }
        public int RespondentId { get; set; }
        public int CatiInterviewerId { get; set; }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.SetNextLinkedInterview)]
    public class SetNextLinkedInterviewEvent : InterviewerActivityEventBase<SetNextLinkedInterviewEventParameters>
    {
        public SetNextLinkedInterviewEvent(string projectId, int respondentId, int catiInterviewerId) :
            base(InterviewerActivityEventType.SetNextLinkedInterview)
        {
            Details.ProjectId = projectId;
            Details.CatiInterviewerId = catiInterviewerId;
            Details.RespondentId = respondentId;
        }

        public new void Save()
        {
            base.Save();
        }
    }

    [Serializable]
    public class SetNextLinkedInterviewToPreviousEventParameters : InterviewerActivityEventDetailsBase
    {
        public int CatiInterviewerId { get; set; }
        public string LinkedChain { get; set; }
    }


    [InterviewerActivityEventAttribute(InterviewerActivityEventType.SetNextLinkedInterviewToPrevious)]
    public class SetNextLinkedInterviewToPreviousEvent : InterviewerActivityEventBase<SetNextLinkedInterviewToPreviousEventParameters>
    {
        public SetNextLinkedInterviewToPreviousEvent(int catiInterviewerId, string LinkedInterviewsChain) :
            base(InterviewerActivityEventType.SetNextLinkedInterviewToPrevious)
        {
            Details.CatiInterviewerId = catiInterviewerId;
            Details.LinkedChain = LinkedInterviewsChain;
        }

        public new void Save()
        {
            base.Save();
        }
    }

    [Serializable]
    public class GetInterviewsEventParameters : InterviewerActivityEventDetailsBase
    {
        public string[] ProjectList { get; set; }
        public string TelephoneNumber { get; set; }
        public int RespondentId { get; set; }
        public string RespondentName { get; set; }
        public string Filter { get; set; }
    }

    [InterviewerActivityEventAttribute(InterviewerActivityEventType.GetInterviews)]
    public class GetInterviewsEvent : InterviewerActivityEventBase<GetInterviewsEventParameters>
    {
        public GetInterviewsEvent(string[] projectList, string telephoneNumber, string respondentName, string filter)
            : base(InterviewerActivityEventType.GetInterviews)
        {
            Details.ProjectList = projectList;
            Details.TelephoneNumber = telephoneNumber;
            Details.RespondentName = respondentName;
            Details.Filter = filter;
        }

        public new void Save()
        {
            base.Save();
        }
    }

    [Serializable]
    public class GetLinkedInterviewsEventParameters : InterviewerActivityEventDetailsBase
    {
        public int CatiInterviewerId { get; set; }
        public string LinkedChain { get; set; }
    }


    [InterviewerActivityEventAttribute(InterviewerActivityEventType.GetLinkedInterviews)]
    public class GetLinkedInterviewsEvent : InterviewerActivityEventBase<GetLinkedInterviewsEventParameters>
    {
        public GetLinkedInterviewsEvent(int catiInterviewerId, string linkedInterviewsChain) :
            base(InterviewerActivityEventType.GetLinkedInterviews)
        {
            Details.CatiInterviewerId = catiInterviewerId;
            Details.LinkedChain = linkedInterviewsChain;
        }

        public new void Save()
        {
            base.Save();
        }
    }

    [InterviewerActivityEvent(InterviewerActivityEventType.GetCallType)]
    public class CheckCallTypeEvent : InterviewerActivityEventBase<NoParameters>
    {
        public CheckCallTypeEvent() : base(InterviewerActivityEventType.GetCallType)
        {
        }
    }

    [InterviewerActivityEvent(InterviewerActivityEventType.GetPersonType)]
    public class CheckPersonTypeEvent : InterviewerActivityEventBase<NoParameters>
    {
        public CheckPersonTypeEvent(int catiInterviewerId) : base(InterviewerActivityEventType.GetPersonType)
        {
            InterviewerSid = catiInterviewerId;
        }
    }

    [InterviewerActivityEvent(InterviewerActivityEventType.EnableLiveMonitoring)]
    public class EnableLiveMonitoringEvent : InterviewerActivityEventBase<NoParameters>
    {
        public EnableLiveMonitoringEvent()
            : base(InterviewerActivityEventType.EnableLiveMonitoring)
        {
        }

        public new void Save()
        {
            base.Save();
        }
    }

    [Serializable]
    public class StopAudioRecordingEventParameters : InterviewerActivityEventDetailsBase
    {
        public string StopRecordingMode { get; set; }
        public string Warning { get; set; }
    }

    [InterviewerActivityEvent(InterviewerActivityEventType.StopRecording)]
    public class StopAudioRecordingEvent : InterviewerActivityEventBase<StopAudioRecordingEventParameters>
    {
        public StopAudioRecordingEvent(int surveySid, string surveyName, int interviewId, string stopRecordingMode) : base(InterviewerActivityEventType.StopRecording)
        {
            InterviewId = interviewId;
            SurveySid = surveySid;
            SurveyName = surveyName;
            Details.StopRecordingMode = stopRecordingMode;
        }
    }

    [Serializable]
    public class StartAudioRecordingEventParameters : InterviewerActivityEventDetailsBase
    {
        public string Label { get; set; }
        public string Warning { get; set; }
    }

    [InterviewerActivityEvent(InterviewerActivityEventType.StartRecording)]
    public class StartAudioRecordingEvent : InterviewerActivityEventBase<StartAudioRecordingEventParameters>
    {
        public StartAudioRecordingEvent(int surveySid, string surveyName, int interviewId, string label, string warning) : base(InterviewerActivityEventType.StartRecording)
        {
            InterviewId = interviewId;
            SurveySid = surveySid;
            SurveyName = surveyName;
            Details.Label = label;
            Details.Warning = warning;
        }
    }

    [Serializable]
    public class UpdateActiveQuestionEventParameters : InterviewerActivityEventDetailsBase
    {
        public string QuestionId { get; set; }

    }

    [InterviewerActivityEvent(InterviewerActivityEventType.UpdateActiveQuestion)]
    public class UpdateActiveQuestionEvent : InterviewerActivityEventBase<UpdateActiveQuestionEventParameters>
    {
        public UpdateActiveQuestionEvent(string projectId, int interviewerId, string questionId) : base(InterviewerActivityEventType.UpdateActiveQuestion)
        {
            InterviewerSid = interviewerId;
            SurveyName = projectId;
            Details.QuestionId = questionId;
        }
    }
    
    [Serializable]
    public class IsCatiGroupMemberEventParameters : InterviewerActivityEventDetailsBase
    {
        public string GroupName { get; set; }

    }

    [InterviewerActivityEvent(InterviewerActivityEventType.IsCatiGroupMember)]
    public class IsCatiGroupMemberEvent : InterviewerActivityEventBase<IsCatiGroupMemberEventParameters>
    {
        public IsCatiGroupMemberEvent(int interviewerId, string groupName) : base(InterviewerActivityEventType.IsCatiGroupMember)
        {
            InterviewerSid = interviewerId;
            Details.GroupName = groupName;
        }
    }
    
}
