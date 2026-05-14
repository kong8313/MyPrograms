using System;
using System.Diagnostics;
using BvCallHandlerLibrary;
using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;
using Confirmit.CATI.Core.Telephony.DialingWorkflow;
using ConfirmitDialerInterface;
using DialingMode = ConfirmitDialerInterface.DialingMode;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public class ConsoleStartInterviewProcessor : IConsoleStartInterviewProcessor
    {
        private readonly ITimezoneService _timezoneService;
        private readonly IBvCallHandlerRoot _bvCallHandlerRoot;
        private readonly IAsyncManager _asyncManager;
        private readonly ITelephoneBlacklistService _telephoneBlacklistService;
        private readonly IActiveDialRepository _activeDialRepository;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IPersonRepository _personRepository;
        private readonly ITaskRepository _taskRepository;

        public ConsoleStartInterviewProcessor(
            ISurveyRepository surveyRepository,
            IPersonRepository personRepository,
            ITaskRepository taskRepository,
            ITimezoneService timezoneService,
            IBvCallHandlerRoot bvCallHandlerRoot,
            IAsyncManager asyncManager,
            ITelephoneBlacklistService telephoneBlacklistService,
            IActiveDialRepository activeDialRepository)
        {
            _timezoneService = timezoneService;
            _bvCallHandlerRoot = bvCallHandlerRoot;
            _asyncManager = asyncManager;
            _telephoneBlacklistService = telephoneBlacklistService;
            _activeDialRepository = activeDialRepository;
            _surveyRepository = surveyRepository;
            _personRepository = personRepository;
            _taskRepository = taskRepository;
        }

        public BvSurveyEntity Startinterview(BvPersonEntity person, BvTasksEntity task, string projectId, int interviewId, StartInterviewEvent activityEvent)
        {

            BvSurveyEntity survey = String.IsNullOrEmpty(projectId) ? null :_surveyRepository.GetByProjectId(projectId);

            BvInterviewEntity interview = null;
            if (interviewId != 0)
            {
                interview = InterviewRepository.GetByIdWithCheck(survey.SID, interviewId);
            }

            SwitchSurveyIfNeeded(ref survey, interview, activityEvent, task);

            var dialingModeType = BvCallHandlerRoot.GetDialingMode(task, survey, interview);

            var loginToDiallerState = (LoginState)task.LoggedInToDialerState;

            if ((LoginState)task.StatusLogout == LoginState.BREAK ||
               (LoginState)task.StatusLogout == LoginState.PENDING_BREAK)
            {
                throw new InternalErrorException("Incorrect statuslogout on start interview.");
            }

            var dialingMode = DialingModeFactory.CreateDialingMode(dialingModeType);
            dialingMode.BeforeStartInterview(task, person);

            // save time of starting interview
            BvSpTasks_UpdateStartTimeAdapter.ExecuteNonQuery(task.PersonSID);

            //we have just chosen survey in survey assignment mode
            //we neeed perform rescheduling
            if (task.SurveySID == 0 && survey != null && interviewId == 0)
            {
                PersonService.LoginPersonOnSurveyForSurveySelectionMode(task.PersonSID, survey.SID);
            }

            _asyncManager.QueueWorkItem(() =>
                StartInterviewProcess(
                    survey,
                    interviewId,
                    task.PersonSID,
                    loginToDiallerState));

            return survey;
        }

        private bool IsNeedToSwitchSurveyOnDialer(BvTasksEntity task, BvSurveyEntity survey)
        {
            return survey != null && (task.SelectedSurveyId ?? 0) != 0 && 
                   task.SelectedSurveyId != survey.SID &&
                   task.LoggedInToDialerState == (int)LoginState.LOGGED_IN &&
                   ( survey.DialingMode == DialingMode.Preview || survey.DialingMode == DialingMode.Automatic);
        }

        private AgentTaskChoiceMode GetTaskChoise(BvSurveyEntity survey, BvInterviewEntity interview)
        {
            if (survey == null)
            {
                return AgentTaskChoiceMode.Automatic;
            }

            if (interview == null)
            {
                return AgentTaskChoiceMode.CampaignAssignment;
            }

            return AgentTaskChoiceMode.Manual;
        }

        private void SwitchSurveyIfNeeded(ref BvSurveyEntity survey, BvInterviewEntity interview, StartInterviewEvent activityEvent, BvTasksEntity task)
        {
            using (new EventDetailsScope(activityEvent.Details))
            {
                var taskChoise = GetTaskChoise(survey, interview);

                if (taskChoise == AgentTaskChoiceMode.Manual)
                {
                    if (IsNeedToSwitchSurveyOnDialer(task, survey))
                    {
                        _bvCallHandlerRoot.TryToSendSetCampaign(task.DialerId, survey.CampaignId, task.PersonSID);
                    }

                    return;
                }

                if (!_bvCallHandlerRoot.IsPendingSurveySwitch(task))
                {
                    return;
                }

                // Also, we need to check dialing mode. We must not switch here for predictive mode.
                if (BvCallHandlerRoot.GetDialingMode(task, survey, null) == DialingMode.Predictive)
                {
                    return;
                }

                _bvCallHandlerRoot.SwitchSurvey(task.DialerId, task);
                activityEvent.AddTiming("BvCallHandlerRoot.SwitchSurvey");

                survey = _surveyRepository.GetById(task.NewSurveySID);
                // In fact, task.NewSurveySID and task.SurveySID are equal here
            }
        }

        /// <summary>
        /// It is the implementation of the request to start an interview for the logged in person.
        /// This internal method runs in a separate thread.
        /// </summary>
        /// <remarks>
        /// If a suitable interview is present then the corresponding interview is marked as "in use".
        /// It allows to avoid starting the same interview by more than one user simultaneously.
        /// </remarks>
        internal void StartInterviewProcess(BvSurveyEntity survey, int interviewId, int personId, LoginState loginToDiallerState)
        {
            var logStr = "CATIConsoleWS.StartInterviewProcess ";

            try
            {
                var startInterviewNoCallsActivityEvent = new StartInterviewProcessNoCallsEvent();
                var startInterviewActivityEvent = new StartInterviewProcessEvent();
                using (TaskLocker.Lock(personId, out var taskEntity))
                {
                    var person = _personRepository.GetById(personId);

                    if (survey != null)
                    {
                        taskEntity.SurveySID = survey.SID;
                        taskEntity.SelectedSurveyId = survey.SID;

                        //TODO: refactor, build into IDialingMode somehow
                        if (CheckIfInterviewerLoggedInPredictiveSurvey(taskEntity, survey, loginToDiallerState))
                        {
                            _taskRepository.Update(taskEntity);
                            //We must not try to start interview for predictive surveys
                            return;
                        }
                    }
                    else
                    {
                        taskEntity.SelectedSurveyId = 0;
                    }

                    var interview = TaskService.LookupByPersonSid(
                        taskEntity,
                        interviewId);

                    _taskRepository.Update(taskEntity);

                    if (interview == null)
                    {
                        // Looks like there is no call, so nothing to do here.
                        var state = (AgentTaskChoiceMode) person.ManualSelection == AgentTaskChoiceMode.Manual
                            ? InterviewState.SELECTING
                            : InterviewState.NO_CALLS;
                        var taskDialingMode = BvCallHandlerRoot.GetDialingMode(taskEntity, survey, null);
                        TaskService.MoveTaskToState(taskEntity, state, taskDialingMode);
                        startInterviewNoCallsActivityEvent.Save(personId);

                        return;
                    }

                    var surveyId = taskEntity.SurveySID;
                    interviewId = taskEntity.InterviewID;
                    survey = _surveyRepository.GetById(surveyId);

                    int tzId = _timezoneService.GetTimezoneIdOrDefaultCallCenterTimezoneId(interview.TimezoneID);

                    if (taskEntity.Context.ActiveDialId == null)
                    {
                        if (survey.IsTelephoneBlacklistSupported &&
                            _telephoneBlacklistService.IsTelephoneNumberFilteredByBlacklist(interview.TelephoneNumber))
                        {
                            BvCallHandlerRoot.ProcessBlacklistInterview(
                                taskEntity,
                                interview,
                                (AgentTaskChoiceMode) person.ManualSelection);

                            _taskRepository.Update(taskEntity);

                            return;
                        }
                    }

                    var dialingMode = DialingModeFactory.CreateDialingMode(taskEntity.DialingMode);

                    if (loginToDiallerState != LoginState.LOGGED_IN)
                    {
                        dialingMode = DialingModeFactory.CreateDialingMode(DialingMode.Manual);
                    }else if (taskEntity.Context.TransferId != null)
                    {
                        dialingMode = DialingModeFactory.CreateDialingMode(DialingMode.Automatic);
                    }

                    dialingMode.StartInterview(
                        personId,
                        taskEntity.DialerId,
                        survey,
                        interview,
                        tzId);

                    startInterviewActivityEvent.Save(
                        personId,
                        surveyId,
                        survey.Name,
                        interview.TelephoneNumber,
                        interviewId,
                        taskEntity.DialerId,
                        (int)taskEntity.DialingMode,
                        (int) loginToDiallerState);
                }
            }
            catch (Exception ex)
            {
                logStr += $"(projectId = '{survey?.ProjectId}', interviewId = '{interviewId}', personId = '{personId}'): ";

                Trace.TraceError(logStr + ex);
            }
        }

        private static bool CheckIfInterviewerLoggedInPredictiveSurvey(BvTasksEntity task, BvSurveyEntity survey, LoginState loginToDiallerState)
        {
            var surveyDialingMode = BvCallHandlerRoot.GetDialingMode(task, survey, null);

            return (surveyDialingMode == DialingMode.Predictive &&
                    loginToDiallerState == LoginState.LOGGED_IN);
        }
    }
}
