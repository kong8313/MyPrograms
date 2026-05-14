using System;
using System.Diagnostics;
using BvCallHandlerLibrary;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.Encryption;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public class ConsoleStateProvider : IConsoleStateProvider
    {
        private readonly ILanguageVariableProvider _languageVariableProvider;
        private readonly IInterviewService _interviewService;
        private readonly IInterviewRepository _interviewRepository;
        private readonly ITimezoneService _timezoneService;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IActiveDialRepository _activeDialRepository;
        private readonly IBvCallHandlerRoot _callHandlerRoot;
        private readonly IPersonDeferredMonitoringRepository _personDeferredMonitoringRepository;
        private readonly IPersonRepository _personRepository;
        private readonly ITaskRepository _taskRepository;

        public ConsoleStateProvider(
            ILanguageVariableProvider languageVariableProvider,
            IInterviewService interviewService,
            IInterviewRepository interviewRepository,
            ITimezoneService timezoneService,
            ISurveyRepository surveyRepository,
            IActiveDialRepository activeDialRepository,
            IBvCallHandlerRoot callHandlerRoot,
            IPersonDeferredMonitoringRepository personDeferredMonitoringRepository,
            IPersonRepository personRepository,
            ITaskRepository taskRepository)
        {
            _languageVariableProvider = languageVariableProvider;
            _interviewService = interviewService;
            _interviewRepository = interviewRepository;
            _timezoneService = timezoneService;
            _activeDialRepository = activeDialRepository;
            _callHandlerRoot = callHandlerRoot;
            _personDeferredMonitoringRepository = personDeferredMonitoringRepository;
            _personRepository = personRepository;
            _surveyRepository = surveyRepository;
            _taskRepository = taskRepository;
        }

        public State GetState(BvTasksEntity task, BvPersonEntity person, GetStateEvent evt, UrlGeneratedInGetStateEvent activityEvent)
        {
            int surveySid = task.SurveySID;
            int interviewId = task.InterviewID;
            int respondentTimezoneId = task.TzID;
            var interviewState = (InterviewState)task.InterviewState;
            var callOutcome = (CallOutcome)task.CallOutcome;
            var loginState = (LoginState)task.StatusLogout;
            var callType = (CallTypes)task.CallType;
            var loggedInToDialerState = (LoginState)task.LoggedInToDialerState;
            bool shouldReturnInterviewUrl = false;
            string encryptedUrl = null;
            Timezone respondentTimezone = null;
            string confirmitProjectId = null;
            string confirmitProjectName = null;
            bool isRecording = false;
            int deferredRecordId = 0;
            int? languageVariableValue = null;
            ExternalTransferType externalTransferType = ExternalTransferType.Warm;
            InternalTransferType internalTransferType = InternalTransferType.Off;

            // (task.NewSurveySID == task.SurveySID) means survey switch is already completed.
            var isSurveySwitched = (task.NewSurveySID > 0) && (task.NewSurveySID == task.SurveySID);
            DateTime? startBreakTime = null;

            using (new EventDetailsScope(evt.Details))
            {
                if (surveySid > 0)
                {
                    var survey = SurveyRepository.GetById(surveySid);
                    evt.AddTiming("SurveyRepository.GetById");

                    confirmitProjectId = survey.Name;
                    confirmitProjectName = survey.Description;
                    externalTransferType = (ExternalTransferType)survey.ExternalTransferType;
                    internalTransferType = (InternalTransferType)survey.InternalTransferType;

                    if (interviewId > 0)
                    {
                        if (interviewState == InterviewState.INTERVIEWING ||
                            interviewState == InterviewState.OUTGOING_TRANSFER ||
                            interviewState == InterviewState.OPENEND_REVIEW)
                        {
                            shouldReturnInterviewUrl = true;
                        }

                        if (shouldReturnInterviewUrl)
                        {
                            BvInterviewEntity interview = InterviewRepository.GetById(surveySid, interviewId);
                            evt.AddTiming("InterviewRepository.GetById");

                            languageVariableValue = _languageVariableProvider.GetLanguageForInterview(surveySid, interviewId);

                            if (isSurveySwitched)
                            {
                                using (new EventDetailsScope(evt.Details))
                                {
                                    TaskService.ResetNewSurveyId(task);
                                }
                            }

                            evt.AddTiming("languageVariableProvider.GetLanguageForInterview");

                            isRecording = survey.InterviewScreenRecording;

                            evt.AddTiming("MachineKeyEncryptionManager.Encrypt");

                            deferredRecordId = ProcessDeferredSession(
                                    task,
                                    interview);

                            var surveyUrl = ServiceLocator.Resolve<ISystemSettings>().Site.StartSurveyURL;
                            var interviewUrl = new InterviewUrlBuilder(surveyUrl, confirmitProjectId, survey.EnforceHttps);
                            var sid = _interviewService.GenereteSecurityKey(interview);

                            interviewUrl.AddParameterWithUrlEncode("__resume", 1);
                            interviewUrl.AddParameterWithUrlEncode("__catiinterviewerid", person.SID);
                            interviewUrl.AddParameterWithUrlEncode("__sid__", sid);
                            encryptedUrl = EncryptInterviewUrl(interviewUrl.Url, task);

                            // Process respondent timezone
                            if (interview.TimezoneID.HasValue)
                            {
                                respondentTimezoneId = interview.TimezoneID.Value;
                            }

                            respondentTimezone = _timezoneService.GetTimeZone(respondentTimezoneId);
                            evt.AddTiming("ConsoleServiceHelper.GetTimeZone");

                            activityEvent.Save(person.SID, interviewId, task.CallID, survey.SID, survey.Name);
                            evt.AddTiming("activityEvent.Save");
                        }
                    }
                    else // If survey assignment case information about selected survey is to be returned
                    {
                        using (new EventDetailsScope(evt.Details))
                        {
                            var dialingMode = BvCallHandlerRoot.GetDialingMode(task, survey, null);
                            SendGoNotReadyIfSurveyShouldBeSwitched(task, dialingMode);
                        }

                        evt.AddTiming("SendGoNotReadyIfSurveyShouldBeSwitched");
                    }
                }

                if (loginState == LoginState.BREAK)
                {
                    startBreakTime = TimeBreaksHistoryService.GetStartBreakTime(person.SID);
                    evt.AddTiming("TimeBreaksHistoryService.GetStartBreakTime");
                }
            }

            var transferState = GetTransferState(person, task);

            var result = new State(
                confirmitProjectId,
                confirmitProjectName,
                interviewId,
                encryptedUrl,
                respondentTimezone,
                (int)interviewState,
                (int)callOutcome,
                (int)loginState,
                (int)loggedInToDialerState,
                task.ProblemId,
                callType,
                surveySid,
                isRecording,
                startBreakTime,
                deferredRecordId,
                languageVariableValue,
                isSurveySwitched,
                transferState,
                externalTransferType,
                internalTransferType,
                task.Context.TransferOptions);

            return result;
        }

        private ConsoleTransferState GetTransferState(BvPersonEntity person, BvTasksEntity task)
        {
            if (task.CallID == null) return null;
            if (task.InterviewState != (byte)InterviewState.OUTGOING_TRANSFER &&
                task.InterviewState != (byte)InterviewState.INCOMING_TRANSFER)
                return null;

            var dial = _activeDialRepository.TryGetByCallId(task.CallID);

            if (dial == null || dial.DialState != DialState.Transfering || dial.TransferState == null)
                return DefaultTransferState(person, task);

            return dial.TransferState;
        }

        private ConsoleTransferState DefaultTransferState(BvPersonEntity person, BvTasksEntity task)
        {
            var interview = _interviewRepository.GetByIdWithCheck(task.SurveySID, task.InterviewID);
            return new ConsoleTransferState
            {
                ConnectionState = ConsoleConnectionState.InitiatorToRespondent,
                Initiator = new TransferParticipant
                {
                    ParticipantType = ParticipantType.Agent,
                    Resource = person.Name,
                    DialingState = DialingState.Connected,
                    DialingStateOutcome = DialingStateOutcome.Connected
                },
                Target = new TransferParticipant
                {
                    ParticipantType = ParticipantType.NotDefined,
                    DialingState = DialingState.Dialing,
                    DialingStateOutcome = DialingStateOutcome.NotDefined,
                    Resource = ""
                },
                Respondent = new TransferParticipant
                {
                    ParticipantType = ParticipantType.External,
                    DialingState = DialingState.Connected,
                    DialingStateOutcome = DialingStateOutcome.Connected,
                    Resource = interview.RespondentName
                }
            };
        }

        private void SendGoNotReadyIfSurveyShouldBeSwitched(BvTasksEntity task, DialingMode dialingMode)
        {
            if (!_callHandlerRoot.IsPendingSurveySwitch(task))
            {
                return;
            }

            if ((InterviewState)task.InterviewState != InterviewState.WAITING)
            {
                return;
            }

            if (dialingMode != DialingMode.Predictive)
            {
                return;
            }

            if ((LoginState)task.LoggedInToDialerState != LoginState.LOGGED_IN)
            {
                return;
            }

            // NewSurveySID is set && WAITING && Predictive && Logged in to dialer
            var survey = _surveyRepository.GetById(task.SurveySID);
            _callHandlerRoot.TryToSendGoNotReady(
                task.DialerId,
                survey.CampaignId,
                task.PersonSID,
                task.BreakTypeId,
                () => task.LogString());
        }

        private string EncryptInterviewUrl(string interviewUrl, BvTasksEntity task)
        {
            string result;

            using (var encryptor = ServiceLocator.Resolve<ICatiSymmetricEncryptor>())
            {
                encryptor.Key = task.EncryptionKey;
                encryptor.IV = task.EncryptionIV;
                result = encryptor.EncryptString(interviewUrl);
                encryptor.Clear();
            }

            return result;
        }

        private int ProcessDeferredSession(
            BvTasksEntity task,
            BvInterviewEntity interview)
        {
            var deferredRecord = _personDeferredMonitoringRepository.GetByCallId(task.CallID.Value);

            if (deferredRecord != null)
            {
                bool isValid =
                    (deferredRecord.InterviewID == task.InterviewID) &&
                    (deferredRecord.SurveySID == task.SurveySID) &&
                    (deferredRecord.PersonSID == task.PersonSID) &&
                    (deferredRecord.CallID == task.CallID);

                if (!isValid)
                {
                    var existingRecordSurvey = _surveyRepository.GetById(deferredRecord.SurveySID);
                    var existingRecordPerson = _personRepository.GetById(deferredRecord.PersonSID);

                    var newSurvey = _surveyRepository.GetById(task.SurveySID);
                    var newPerson = _personRepository.GetById(task.PersonSID);

                    Trace.TraceError(
                        "Existing deferred monitoring record is not valid.\r\n" +
                        "  Existing record: Survey={0}, Person={1}, InterviewId={2}, CallId={3}\r\n" +
                        "  New            : Survey={4}, Person={5}, InterviewId={6}, CallId={7} ",
                        existingRecordSurvey.LogInfo,
                        existingRecordPerson.LogInfo,
                        deferredRecord.InterviewID,
                        deferredRecord.CallID,
                        newSurvey.LogInfo,
                        newPerson.LogInfo,
                        task.InterviewID,
                        task.CallID);

                    BvPersonDeferredMonitoringAdapterEx.ClearCallId(deferredRecord.ID);

                    // Existing record is not valid so we must create new one
                    deferredRecord = null;
                }

            }

            if (deferredRecord == null)
            {
                // Create and insert a new record
                deferredRecord = _personDeferredMonitoringRepository.InsertEmptyDeferredRecord(
                    task.PersonSID,
                    task.SurveySID,
                    task.InterviewID,
                    task.CallID.Value,
                    task.CallCenterID,
                    interview.RespondentName,
                    interview.TelephoneNumber);
            }

            return deferredRecord.ID;
        }

    }
}