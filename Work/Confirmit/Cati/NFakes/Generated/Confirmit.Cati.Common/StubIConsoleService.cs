using System;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ConsoleService;
using System.Data;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Common.ConsoleService.Fakes
{
    public class StubIConsoleService : IConsoleService 
    {
        private IConsoleService _inner;

        public StubIConsoleService()
        {
            _inner = null;
        }

        public IConsoleService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void LoginStringConsoleDescriptionPersonInfoOutDiallerInfoOutCatiConsolePropertiesContainerOutDelegate(string stationId, ConsoleDescription consoleDescription, out PersonInfo personInfo, out DiallerInfo diallerInfo, out CatiConsolePropertiesContainer catiConsoleProperties);
        public LoginStringConsoleDescriptionPersonInfoOutDiallerInfoOutCatiConsolePropertiesContainerOutDelegate LoginStringConsoleDescriptionPersonInfoOutDiallerInfoOutCatiConsolePropertiesContainerOut;

        void IConsoleService.Login(string stationId, ConsoleDescription consoleDescription, out PersonInfo personInfo, out DiallerInfo diallerInfo, out CatiConsolePropertiesContainer catiConsoleProperties)
        {
            personInfo = default(PersonInfo);
            diallerInfo = default(DiallerInfo);
            catiConsoleProperties = default(CatiConsolePropertiesContainer);

            if (LoginStringConsoleDescriptionPersonInfoOutDiallerInfoOutCatiConsolePropertiesContainerOut != null)
            {
                LoginStringConsoleDescriptionPersonInfoOutDiallerInfoOutCatiConsolePropertiesContainerOut(stationId, consoleDescription, out personInfo, out diallerInfo, out catiConsoleProperties);
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).Login(stationId, consoleDescription, out personInfo, out diallerInfo, out catiConsoleProperties);
            }
        }

        public delegate void LoginToDialerStringStringBooleanOutDelegate(string extensionNumber, string surveyId, out bool isPredictive);
        public LoginToDialerStringStringBooleanOutDelegate LoginToDialerStringStringBooleanOut;

        void IConsoleService.LoginToDialer(string extensionNumber, string surveyId, out bool isPredictive)
        {
            isPredictive = default(bool);

            if (LoginToDialerStringStringBooleanOut != null)
            {
                LoginToDialerStringStringBooleanOut(extensionNumber, surveyId, out isPredictive);
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).LoginToDialer(extensionNumber, surveyId, out isPredictive);
            }
        }

        public delegate int GetPersonModeDelegate();
        public GetPersonModeDelegate GetPersonMode;

        int IConsoleService.GetPersonMode()
        {


            if (GetPersonMode != null)
            {
                return GetPersonMode();
            } else if (_inner != null)
            {
                return ((IConsoleService)_inner).GetPersonMode();
            }

            return default(int);
        }

        public delegate void SetPendingLogoutBooleanDelegate(bool logout);
        public SetPendingLogoutBooleanDelegate SetPendingLogoutBoolean;

        void IConsoleService.SetPendingLogout(bool logout)
        {

            if (SetPendingLogoutBoolean != null)
            {
                SetPendingLogoutBoolean(logout);
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).SetPendingLogout(logout);
            }
        }

        public delegate void ConfirmLogoutDelegate();
        public ConfirmLogoutDelegate ConfirmLogout;

        void IConsoleService.ConfirmLogout()
        {

            if (ConfirmLogout != null)
            {
                ConfirmLogout();
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).ConfirmLogout();
            }
        }

        public delegate bool StartInterviewStringInt32Delegate(string surveyId, int interviewId);
        public StartInterviewStringInt32Delegate StartInterviewStringInt32;

        bool IConsoleService.StartInterview(string surveyId, int interviewId)
        {


            if (StartInterviewStringInt32 != null)
            {
                return StartInterviewStringInt32(surveyId, interviewId);
            } else if (_inner != null)
            {
                return ((IConsoleService)_inner).StartInterview(surveyId, interviewId);
            }

            return default(bool);
        }

        public delegate int CreateNewInterviewStringDelegate(string surveyId);
        public CreateNewInterviewStringDelegate CreateNewInterviewString;

        int IConsoleService.CreateNewInterview(string surveyId)
        {


            if (CreateNewInterviewString != null)
            {
                return CreateNewInterviewString(surveyId);
            } else if (_inner != null)
            {
                return ((IConsoleService)_inner).CreateNewInterview(surveyId);
            }

            return default(int);
        }

        public delegate Survey[] GetOpenedSurveysDelegate();
        public GetOpenedSurveysDelegate GetOpenedSurveys;

        Survey[] IConsoleService.GetOpenedSurveys()
        {


            if (GetOpenedSurveys != null)
            {
                return GetOpenedSurveys();
            } else if (_inner != null)
            {
                return ((IConsoleService)_inner).GetOpenedSurveys();
            }

            return default(Survey[]);
        }

        public delegate DataTable GetSurveyInterviewsStringArrayOfSearchParameterDelegate(string surveyId, SearchParameter[] parameters);
        public GetSurveyInterviewsStringArrayOfSearchParameterDelegate GetSurveyInterviewsStringArrayOfSearchParameter;

        DataTable IConsoleService.GetSurveyInterviews(string surveyId, SearchParameter[] parameters)
        {


            if (GetSurveyInterviewsStringArrayOfSearchParameter != null)
            {
                return GetSurveyInterviewsStringArrayOfSearchParameter(surveyId, parameters);
            } else if (_inner != null)
            {
                return ((IConsoleService)_inner).GetSurveyInterviews(surveyId, parameters);
            }

            return default(DataTable);
        }

        public delegate SpellError[] CheckTextSpellingInt32StringDelegate(int languageId, string textBlock);
        public CheckTextSpellingInt32StringDelegate CheckTextSpellingInt32String;

        SpellError[] IConsoleService.CheckTextSpelling(int languageId, string textBlock)
        {


            if (CheckTextSpellingInt32String != null)
            {
                return CheckTextSpellingInt32String(languageId, textBlock);
            } else if (_inner != null)
            {
                return ((IConsoleService)_inner).CheckTextSpelling(languageId, textBlock);
            }

            return default(SpellError[]);
        }

        public delegate Appointment[] GetInterviewAppointmentListStringInt32Delegate(string surveyId, int interviewId);
        public GetInterviewAppointmentListStringInt32Delegate GetInterviewAppointmentListStringInt32;

        Appointment[] IConsoleService.GetInterviewAppointmentList(string surveyId, int interviewId)
        {


            if (GetInterviewAppointmentListStringInt32 != null)
            {
                return GetInterviewAppointmentListStringInt32(surveyId, interviewId);
            } else if (_inner != null)
            {
                return ((IConsoleService)_inner).GetInterviewAppointmentList(surveyId, interviewId);
            }

            return default(Appointment[]);
        }

        public delegate Timezone GetInterviewTimezoneStringInt32Delegate(string surveyId, int interviewId);
        public GetInterviewTimezoneStringInt32Delegate GetInterviewTimezoneStringInt32;

        Timezone IConsoleService.GetInterviewTimezone(string surveyId, int interviewId)
        {


            if (GetInterviewTimezoneStringInt32 != null)
            {
                return GetInterviewTimezoneStringInt32(surveyId, interviewId);
            } else if (_inner != null)
            {
                return ((IConsoleService)_inner).GetInterviewTimezone(surveyId, interviewId);
            }

            return default(Timezone);
        }

        public delegate void SetInterviewAppointmentListStringInt32ArrayOfAppointmentBooleanDelegate(string surveyId, int interviewId, Appointment[] appointments, bool allowOutsideShift);
        public SetInterviewAppointmentListStringInt32ArrayOfAppointmentBooleanDelegate SetInterviewAppointmentListStringInt32ArrayOfAppointmentBoolean;

        void IConsoleService.SetInterviewAppointmentList(string surveyId, int interviewId, Appointment[] appointments, bool allowOutsideShift)
        {

            if (SetInterviewAppointmentListStringInt32ArrayOfAppointmentBoolean != null)
            {
                SetInterviewAppointmentListStringInt32ArrayOfAppointmentBoolean(surveyId, interviewId, appointments, allowOutsideShift);
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).SetInterviewAppointmentList(surveyId, interviewId, appointments, allowOutsideShift);
            }
        }

        public delegate Appointment[] GetAllAppointmentListDelegate();
        public GetAllAppointmentListDelegate GetAllAppointmentList;

        Appointment[] IConsoleService.GetAllAppointmentList()
        {


            if (GetAllAppointmentList != null)
            {
                return GetAllAppointmentList();
            } else if (_inner != null)
            {
                return ((IConsoleService)_inner).GetAllAppointmentList();
            }

            return default(Appointment[]);
        }

        public delegate Messages[] GetMessagesDelegate();
        public GetMessagesDelegate GetMessages;

        Messages[] IConsoleService.GetMessages()
        {


            if (GetMessages != null)
            {
                return GetMessages();
            } else if (_inner != null)
            {
                return ((IConsoleService)_inner).GetMessages();
            }

            return default(Messages[]);
        }

        public delegate bool GetForceOpenendReviewInt32Delegate(int attemptNumber);
        public GetForceOpenendReviewInt32Delegate GetForceOpenendReviewInt32;

        bool IConsoleService.GetForceOpenendReview(int attemptNumber)
        {


            if (GetForceOpenendReviewInt32 != null)
            {
                return GetForceOpenendReviewInt32(attemptNumber);
            } else if (_inner != null)
            {
                return ((IConsoleService)_inner).GetForceOpenendReview(attemptNumber);
            }

            return default(bool);
        }

        public delegate void DialStringInt32Int32Delegate(string phoneNumber, int initiator, int attemptNumber);
        public DialStringInt32Int32Delegate DialStringInt32Int32;

        void IConsoleService.Dial(string phoneNumber, int initiator, int attemptNumber)
        {

            if (DialStringInt32Int32 != null)
            {
                DialStringInt32Int32(phoneNumber, initiator, attemptNumber);
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).Dial(phoneNumber, initiator, attemptNumber);
            }
        }

        public delegate void CancelDialingDelegate();
        public CancelDialingDelegate CancelDialing;

        void IConsoleService.CancelDialing()
        {

            if (CancelDialing != null)
            {
                CancelDialing();
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).CancelDialing();
            }
        }

        public delegate bool HangupInt32Delegate(int initiator);
        public HangupInt32Delegate HangupInt32;

        bool IConsoleService.Hangup(int initiator)
        {


            if (HangupInt32 != null)
            {
                return HangupInt32(initiator);
            } else if (_inner != null)
            {
                return ((IConsoleService)_inner).Hangup(initiator);
            }

            return default(bool);
        }

        public delegate void TransferStartTransferOptionsDelegate(TransferOptions TransferResource);
        public TransferStartTransferOptionsDelegate TransferStartTransferOptions;

        void IConsoleService.TransferStart(TransferOptions TransferResource)
        {

            if (TransferStartTransferOptions != null)
            {
                TransferStartTransferOptions(TransferResource);
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).TransferStart(TransferResource);
            }
        }

        public delegate void TransferSetConnectionStateTransferConnectionStateDelegate(TransferConnectionState transferConnectionState);
        public TransferSetConnectionStateTransferConnectionStateDelegate TransferSetConnectionStateTransferConnectionState;

        void IConsoleService.TransferSetConnectionState(TransferConnectionState transferConnectionState)
        {

            if (TransferSetConnectionStateTransferConnectionState != null)
            {
                TransferSetConnectionStateTransferConnectionState(transferConnectionState);
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).TransferSetConnectionState(transferConnectionState);
            }
        }

        public delegate void TransferCompleteDelegate();
        public TransferCompleteDelegate TransferComplete;

        void IConsoleService.TransferComplete()
        {

            if (TransferComplete != null)
            {
                TransferComplete();
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).TransferComplete();
            }
        }

        public delegate void TransferCancelDelegate();
        public TransferCancelDelegate TransferCancel;

        void IConsoleService.TransferCancel()
        {

            if (TransferCancel != null)
            {
                TransferCancel();
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).TransferCancel();
            }
        }

        public delegate void WrapUpInt32BooleanInt32CompletedInterviewDetailsDelegate(int interviewId, bool lookUpForNewCalls, int attemptNumber, CompletedInterviewDetails details);
        public WrapUpInt32BooleanInt32CompletedInterviewDetailsDelegate WrapUpInt32BooleanInt32CompletedInterviewDetails;

        void IConsoleService.WrapUp(int interviewId, bool lookUpForNewCalls, int attemptNumber, CompletedInterviewDetails details)
        {

            if (WrapUpInt32BooleanInt32CompletedInterviewDetails != null)
            {
                WrapUpInt32BooleanInt32CompletedInterviewDetails(interviewId, lookUpForNewCalls, attemptNumber, details);
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).WrapUp(interviewId, lookUpForNewCalls, attemptNumber, details);
            }
        }

        public delegate void StartPlaybackStringInt32OutDelegate(string soundFileName, out int timeOfPlayingInSeconds);
        public StartPlaybackStringInt32OutDelegate StartPlaybackStringInt32Out;

        void IConsoleService.StartPlayback(string soundFileName, out int timeOfPlayingInSeconds)
        {
            timeOfPlayingInSeconds = default(int);

            if (StartPlaybackStringInt32Out != null)
            {
                StartPlaybackStringInt32Out(soundFileName, out timeOfPlayingInSeconds);
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).StartPlayback(soundFileName, out timeOfPlayingInSeconds);
            }
        }

        public delegate void StopPlaybackDelegate();
        public StopPlaybackDelegate StopPlayback;

        void IConsoleService.StopPlayback()
        {

            if (StopPlayback != null)
            {
                StopPlayback();
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).StopPlayback();
            }
        }

        public delegate void PauseOrResumePlaybackDelegate();
        public PauseOrResumePlaybackDelegate PauseOrResumePlayback;

        void IConsoleService.PauseOrResumePlayback()
        {

            if (PauseOrResumePlayback != null)
            {
                PauseOrResumePlayback();
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).PauseOrResumePlayback();
            }
        }

        public delegate void ToggleInterviewerListensToPlaybackOrRespondentDelegate();
        public ToggleInterviewerListensToPlaybackOrRespondentDelegate ToggleInterviewerListensToPlaybackOrRespondent;

        void IConsoleService.ToggleInterviewerListensToPlaybackOrRespondent()
        {

            if (ToggleInterviewerListensToPlaybackOrRespondent != null)
            {
                ToggleInterviewerListensToPlaybackOrRespondent();
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).ToggleInterviewerListensToPlaybackOrRespondent();
            }
        }

        public delegate void UpdatePersonModeInt32Delegate(int personMode);
        public UpdatePersonModeInt32Delegate UpdatePersonModeInt32;

        void IConsoleService.UpdatePersonMode(int personMode)
        {

            if (UpdatePersonModeInt32 != null)
            {
                UpdatePersonModeInt32(personMode);
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).UpdatePersonMode(personMode);
            }
        }

        public delegate bool SetPendingBreakStatusPendingBreakStatusNullableOfInt32Delegate(PendingBreakStatus status, int? breakType);
        public SetPendingBreakStatusPendingBreakStatusNullableOfInt32Delegate SetPendingBreakStatusPendingBreakStatusNullableOfInt32;

        bool IConsoleService.SetPendingBreakStatus(PendingBreakStatus status, int? breakType)
        {


            if (SetPendingBreakStatusPendingBreakStatusNullableOfInt32 != null)
            {
                return SetPendingBreakStatusPendingBreakStatusNullableOfInt32(status, breakType);
            } else if (_inner != null)
            {
                return ((IConsoleService)_inner).SetPendingBreakStatus(status, breakType);
            }

            return default(bool);
        }

        public delegate void ContinueWorkAfterBreakInt32Delegate(int attemptNumber);
        public ContinueWorkAfterBreakInt32Delegate ContinueWorkAfterBreakInt32;

        void IConsoleService.ContinueWorkAfterBreak(int attemptNumber)
        {

            if (ContinueWorkAfterBreakInt32 != null)
            {
                ContinueWorkAfterBreakInt32(attemptNumber);
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).ContinueWorkAfterBreak(attemptNumber);
            }
        }

        public delegate LanguageCollection GetSurveyLanguagesStringDelegate(string projectId);
        public GetSurveyLanguagesStringDelegate GetSurveyLanguagesString;

        LanguageCollection IConsoleService.GetSurveyLanguages(string projectId)
        {


            if (GetSurveyLanguagesString != null)
            {
                return GetSurveyLanguagesString(projectId);
            } else if (_inner != null)
            {
                return ((IConsoleService)_inner).GetSurveyLanguages(projectId);
            }

            return default(LanguageCollection);
        }

        public delegate QuestionHistoryCollection GetInterviewHistoryStringStringInt32Delegate(string projectId, string respondentIdentity, int languageId);
        public GetInterviewHistoryStringStringInt32Delegate GetInterviewHistoryStringStringInt32;

        QuestionHistoryCollection IConsoleService.GetInterviewHistory(string projectId, string respondentIdentity, int languageId)
        {


            if (GetInterviewHistoryStringStringInt32 != null)
            {
                return GetInterviewHistoryStringStringInt32(projectId, respondentIdentity, languageId);
            } else if (_inner != null)
            {
                return ((IConsoleService)_inner).GetInterviewHistory(projectId, respondentIdentity, languageId);
            }

            return default(QuestionHistoryCollection);
        }

        public delegate Guid GenerateAuthenticationKeyDelegate();
        public GenerateAuthenticationKeyDelegate GenerateAuthenticationKey;

        Guid IConsoleService.GenerateAuthenticationKey()
        {


            if (GenerateAuthenticationKey != null)
            {
                return GenerateAuthenticationKey();
            } else if (_inner != null)
            {
                return ((IConsoleService)_inner).GenerateAuthenticationKey();
            }

            return default(Guid);
        }

        public delegate void TerminateTaskDelegate();
        public TerminateTaskDelegate TerminateTask;

        void IConsoleService.TerminateTask()
        {

            if (TerminateTask != null)
            {
                TerminateTask();
            } else if (_inner != null)
            {
                ((IConsoleService)_inner).TerminateTask();
            }
        }

        public delegate InternalTransferTarget[] GetInternalTransferTargetsDelegate();
        public GetInternalTransferTargetsDelegate GetInternalTransferTargets;

        InternalTransferTarget[] IConsoleService.GetInternalTransferTargets()
        {


            if (GetInternalTransferTargets != null)
            {
                return GetInternalTransferTargets();
            } else if (_inner != null)
            {
                return ((IConsoleService)_inner).GetInternalTransferTargets();
            }

            return default(InternalTransferTarget[]);
        }

        public delegate ExternalTransferTarget[] GetExternalTransferTargetsDelegate();
        public GetExternalTransferTargetsDelegate GetExternalTransferTargets;

        ExternalTransferTarget[] IConsoleService.GetExternalTransferTargets()
        {


            if (GetExternalTransferTargets != null)
            {
                return GetExternalTransferTargets();
            } else if (_inner != null)
            {
                return ((IConsoleService)_inner).GetExternalTransferTargets();
            }

            return default(ExternalTransferTarget[]);
        }

    }
}