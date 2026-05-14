using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using BvCallHandlerLibrary;
using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Backend.Resources;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.WcfTools.ErrorContextHandler;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.PersonLogin;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.ApiClients.Models;
using Confirmit.CATI.Core.Services.CheckSpelling;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.Console;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;
using Confirmit.CATI.Core.WcfServices.Clients;

using ConfirmitDialerInterface;
using DialingMode = ConfirmitDialerInterface.DialingMode;

namespace Confirmit.CATI.Backend.WcfServices.External.ConsoleService
{
    /// <summary>
    /// Stateless CATI Console WCF Web Service.
    /// 
    /// One of the main ConsoleService tasks is to provide CATI console with 
    /// interview URLs using CATI task table. CATI console uses the result URLs
    /// in order to start interviews on Confirmit. 
    /// 
    /// Interviewer authentication is being done during each call of service method.
    /// Interviewer login and password are transfered via ServiceSecurityContext and validated in 
    /// CustomUserNameValidator class.
    /// </summary>
    [ErrorContextHandler(WebServiceType.External)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ConsoleService : IConsoleService
    {
        private const int MenuInitiator = 1;

        private readonly IConsoleWsRequestsAuthoriser _consoleWsRequestsAuthoriser;
        private readonly IRedialNumberSaver _redialNumberSaver;
        private readonly IConsoleVersionValidator _versionValidator;
        private readonly IInterviewService _interviewService;
        private readonly IRespondentsClient _respondentsClient;
        private readonly ITelephony _telephony;
        private readonly IAuthoringService _authoringService;
        private readonly IDialerCollection _dialerCollection;
        private readonly IConsoleServiceHelper _callConsoleServiceHelper;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IConsoleWrapUpProcessor _consoleWrapUpProcessor;
        private readonly IConsoleStartInterviewProcessor _consoleStartInterviewProcessor;
        private readonly IConsoleLoginProcessor _consoleLoginProcessor;
        private readonly IConsoleLoginToDialerProcessor _consoleLoginToDialerProcessor;
        private readonly IConsoleTransferStartProcessor _consoleTransferStartProcessor;
        private readonly IConsoleTransferCompleteProcessor _consoleTransferCompleteProcessor;
        private readonly IConsoleTransferCancelProcessor _consoleTransferCancelProcessor;
        private readonly IConsoleTransferSetConnectionStateProcessor _consoleTransferSetConnectionStateProcessor;
        private readonly IStationInfoFactory _stationInfoFactory;
        private readonly IActiveDialService _activeDialService;
        private readonly IActiveDialRepository _activeDialRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IBvCallHandlerRoot _callHandlerRoot;
        private readonly ITimeService _timeService;
        public ConsoleService()
        {
            _consoleWsRequestsAuthoriser = ServiceLocator.Resolve<IConsoleWsRequestsAuthoriser>();

            _redialNumberSaver = ServiceLocator.Resolve<IRedialNumberSaver>();
            _versionValidator = ServiceLocator.Resolve<IConsoleVersionValidator>();
            _interviewService = ServiceLocator.Resolve<IInterviewService>();
            _respondentsClient = ServiceLocator.Resolve<IRespondentsClient>();

            _telephony = ServiceLocator.Resolve<ITelephony>();
            _authoringService = ServiceLocator.Resolve<IAuthoringService>();
            _dialerCollection = ServiceLocator.Resolve<IDialerCollection>();
            _callConsoleServiceHelper = ServiceLocator.Resolve<IConsoleServiceHelper>();
            _surveyRepository = ServiceLocator.Resolve<ISurveyRepository>();

            _consoleWrapUpProcessor = ServiceLocator.Resolve<IConsoleWrapUpProcessor>();
            _consoleStartInterviewProcessor = ServiceLocator.Resolve<IConsoleStartInterviewProcessor>();
            _consoleLoginProcessor = ServiceLocator.Resolve<IConsoleLoginProcessor>();
            _consoleLoginToDialerProcessor = ServiceLocator.Resolve<IConsoleLoginToDialerProcessor>();
            _consoleTransferStartProcessor = ServiceLocator.Resolve<IConsoleTransferStartProcessor>();
            _consoleTransferCompleteProcessor = ServiceLocator.Resolve<IConsoleTransferCompleteProcessor>();
            _consoleTransferCancelProcessor = ServiceLocator.Resolve<IConsoleTransferCancelProcessor>();
            _consoleTransferSetConnectionStateProcessor = ServiceLocator.Resolve<IConsoleTransferSetConnectionStateProcessor>();

            _stationInfoFactory = ServiceLocator.Resolve<IStationInfoFactory>();
            _activeDialService = ServiceLocator.Resolve<IActiveDialService>();
            _activeDialRepository = ServiceLocator.Resolve<IActiveDialRepository>();

            _taskRepository = ServiceLocator.Resolve<ITaskRepository>();

            _callHandlerRoot = ServiceLocator.Resolve<IBvCallHandlerRoot>();

            _timeService = ServiceLocator.Resolve<ITimeService>();
        }

        /// <summary>
        /// Login to ConsoleService service.
        /// </summary>
        /// <param name="stationId">Station identifier.</param>
        /// <param name="consoleDescription">Console description.</param>
        /// <param name="diallerInfo">Contains information about dialler</param>
        /// <param name="personInfo">Contains information about interviewer</param>
        /// <param name="catiConsoleProperties">The properties required for CATI console.</param>
        public void Login(
            string stationId,
            ConsoleDescription consoleDescription,
            out PersonInfo personInfo,
            out DiallerInfo diallerInfo,
            out CatiConsolePropertiesContainer catiConsoleProperties)
        {
            var activityEvent = new LoginEvent();
            using (new EventDetailsScope(activityEvent.Details))
            {
                // Authorization
                BvPersonEntity person;
                BvTasksEntity task;
                _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task, false);
                ////////////////////////////////////////////////////////////////////////////////////////////////

                EventDetailsScope.Current.AddTiming("_consoleWsRequestsAuthoriser.AuthoriseRequest");

                _versionValidator.ValidateVersion(consoleDescription.ConsoleVersion);

                EventDetailsScope.Current.AddTiming("_versionValidator.ValidateVersion");

                var stationInfo = _stationInfoFactory.Create(stationId, person);

                bool isAlreadyLoggedIn;
                task = _consoleLoginProcessor.Login(person, task, stationInfo, out isAlreadyLoggedIn);

                personInfo = _consoleLoginProcessor.GetPersonInfo(person, task, isAlreadyLoggedIn);
                diallerInfo = _consoleLoginProcessor.GetDialerInfo(task, stationInfo, isAlreadyLoggedIn);
                catiConsoleProperties = _consoleLoginProcessor.GetConsolePropertiesInfo();

                EventDetailsScope.Current.AddTiming("loginProcessor.Login");

            }

            activityEvent.Details.PersonInfo = personInfo;
            activityEvent.Details.DialerInfo = diallerInfo;
            activityEvent.Details.ConsoleDescription = consoleDescription;

            activityEvent.Save(personInfo.PersonId);
        }

        /// <summary>
        /// The method initiates an interviewer login to the dialler.
        /// CATI console calls this method if there is a dialer in the system.
        /// </summary>
        /// <param name="extensionNumber">The interviewer extension phone number.</param>
        /// <param name="surveyId">Survey Id can be null or empty for AUTOMATIC users.</param>
        /// <param name="isPredictive"> </param>
        /// <remarks>
        /// Confirmit surveyId like pNNNNNNN is used.
        /// Login to dialer operation is asynchronous. CATI console must call GetState method 
        /// to find out if login to MN dialer has finished.
        /// </remarks>
        public void LoginToDialer(string extensionNumber, string surveyId, out bool isPredictive)
        {
            var activityEvent = new LoginToDialerEvent();

            // Authorization
            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);

            BvSurveyEntity survey = String.IsNullOrEmpty(surveyId) ? null : _surveyRepository.GetByProjectId(surveyId);

            survey = _consoleLoginToDialerProcessor.LoginToDialer(person, task, extensionNumber, survey, out isPredictive);

            activityEvent.Save(task.PersonSID,
                               (survey != null) ? (int?)survey.SID : null,
                               (survey != null) ? survey.Name : null,
                               task.DialerId,
                               extensionNumber,
                               isPredictive);

            // TODO: surveyId is actually survey name, that is actually project name
        }

        /// <summary>
        /// Returns the mode of the logged in person.
        /// Throws "ThePersonIsNotLoggedIn" exception if there is no logged in person.
        /// </summary>
        public int GetPersonMode()
        {
            // TODO: New activity event???

            // Authorization
            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            ////////////////////////////////////////////////////////////////////////////////////////////////

            return person.ManualSelection;
        }

        /// <summary>
        /// Notifies Fusion if the logged in person wants to logout.
        /// If the person has no active interviews then the function initiates logout process.
        /// </summary>
        /// <param name="logout">true if the person wants to logout, false if the person wants to continue his/her session</param>
        public void SetPendingLogout(bool logout)
        {
            var activityEvent = new SetPendingLogoutEvent();

            // Authorization
            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            ////////////////////////////////////////////////////////////////////////////////////////////////

            var updateStatusEntity = BvSpTasks_UpdateStatusLogoutAdapter.ExecuteEntity(
                person.SID,
                (byte)(logout ? LoginState.PENDING_LOGOUT : LoginState.LOGGED_IN));

            if (updateStatusEntity != null)
            {
                //There is no active interviews for the user, so initiate the logout procedure.
                if (updateStatusEntity.InterviewID.Value == 0)
                {
                    var asyncManager = ServiceLocator.Resolve<IAsyncManager>();
                    asyncManager.QueueWorkItem(() =>
                        _callConsoleServiceHelper.LogoutProcess(
                            person.SID,
                            BackendInstance.Current.CompanyId.ToString(),
                            (LoginState)updateStatusEntity.LoggedInToDialerState,
                            (bool)updateStatusEntity.IsLoginRCToDialer,
                            updateStatusEntity.ProjectID,
                            task.DialerId));
                }

                BvSurveyEntity survey = null;

                if (!string.IsNullOrEmpty(updateStatusEntity.ProjectID))
                {
                    survey = _surveyRepository.GetByName(updateStatusEntity.ProjectID);
                }

                activityEvent.Save(
                    person.SID,
                    (survey != null) ? (int?)survey.SID : null,
                    (survey != null) ? survey.Name : null,
                    logout,
                    updateStatusEntity.InterviewID.Value,
                    updateStatusEntity.LoggedInToDialerState.Value,
                    updateStatusEntity.IsLoginRCToDialer.Value);
            }
            else
            {
                //Seems user was logged out some way (so the situation will be handled at KeepAlive or GetState)
                Trace.TraceError(Strings.ThePersonIsNotLoggedIn);
            }
        }

        /// <summary>
        /// CATI console calls the method at the end of the interviewer session.
        /// CATI console must call it if and only if the interviewer login state becomes NOT_LOGGED_IN
        /// </summary>
        public void ConfirmLogout()
        {
            var activityEvent = new ConfirmLogoutEvent();

            // Authorization
            // TODO: Do we need to verify that task exists here? And simply return if task does not exist?
            BvPersonEntity person;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person);
            ////////////////////////////////////////////////////////////////////////////////////////////////

            var transactionOptions = new DatabaseTransactionOptions("ConSrv.ConfirmLogout", DeadlockPriority.Normal);

            var task = TaskService.RemoveTaskAndLogoutPersonInTransaction(person.SID, transactionOptions);

            if (task == null)
            {
                Trace.TraceWarning(
                    "ConsoleService.ConfirmLogout: Interviewer {0} ({1}) is not logged in.",
                    person.Name,
                    person.SID);
            }

            activityEvent.Save(person.SID);
        }

        /// <summary>
        /// Updates person task choice mode
        /// Uses when interviwer has "CHOICE" mode
        /// </summary>
        /// <remarks>
        /// Updates bvPerson table
        /// </remarks>
        /// <param name="personMode">New PersonMode</param>
        public void UpdatePersonMode(int personMode)
        {
            var activityEvent = new UpdateInterviewerModeEvent();

            // Authorization
            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            ////////////////////////////////////////////////////////////////////////////////////////////////

            if (person.AllowedChoices == null ||
                ConsoleServiceHelper.IsPersonModeAllowed((AgentTaskChoiceMode)personMode, (TaskChoicePermissions)person.AllowedChoices.Value) == false)
            {
                throw new InvalidOperationException(
                    String.Format(
                    "ConsoleService.UpdatePersonMode: Task choice is not allowed for person. " +
                    "/// task choice={0}, personId={1}",
                    personMode,
                    person.SID));
            }

            if ((AgentTaskChoiceMode)personMode == AgentTaskChoiceMode.Automatic &&
                (AgentTaskChoiceMode)person.ManualSelection == AgentTaskChoiceMode.CampaignAssignment)
            {
                var taskEntity = TaskRepository.GetByPerson(person.SID);
                taskEntity.SurveySID = 0;
                TaskRepository.Update(taskEntity);
            }

            person.ManualSelection = personMode;

            PersonRepository.Update(person);

            if (personMode != (int)AgentTaskChoiceMode.Automatic)
            {
                TaskService.MoveTaskToState(task, InterviewState.SELECTING, DialingMode.Manual);
            }

            if (personMode != (int)AgentTaskChoiceMode.CampaignAssignment)
            {
                PersonService.FillLoginGroupsAndAsyncReschedule(person.SID);
            }

            activityEvent.Save(
                person.SID,
                personMode);
        }

        /// <summary>
        /// It is the request to start an interview for the logged in person.
        /// </summary>
        /// <param name="surveyId">Survey id like pNNNNNNN</param>
        ///<param name="interviewId">Unique identifier of the interview</param>
        /// <remarks>
        /// The function is used for persons in any mode: AUTOMATIC, SURVEY_ASSIGNMENT or MANUAL.
        /// if surveyId == null and interviewId == 0
        /// then ConsoleService considers the user is in AUTOMATIC mode.
        /// if surveyId != null and interviewId == 0
        /// then ConsoleService considers the user is in SURVEY_ASSIGNMENT mode.
        /// if surveyId != null and interviewId != 0
        /// then ConsoleService considers the user is in MANUAL mode.
        /// 
        /// Confirmit surveyId like pNNNNNNN is used.
        /// 
        /// CATI console uses GetState method in order to obtain 
        /// startup interview parameters and interviewer/interview state.
        /// </remarks>
        /// <returns>
        /// true if the system successfully started searching for an interview for the interviewer,
        /// or if there is no need to search for a new interview.
        /// false otherwise.
        /// </returns>
        public bool StartInterview(string surveyId, int interviewId)
        {
            var activityEvent = new StartInterviewEvent();

            // Authorization
            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            ////////////////////////////////////////////////////////////////////////////////////////////////

            var survey = _consoleStartInterviewProcessor.Startinterview(person, task, surveyId, interviewId, activityEvent);

            activityEvent.Save(
                task.PersonSID,
                interviewId,
                (survey != null) ? (int?)survey.SID : null,
                (survey != null) ? survey.Name : null);

            return true;
        }

        /// <summary>
        /// It is the request to create a new interview for the logged in person.
        /// Can be used only in MANUAL mode
        /// </summary>
        /// <param name="surveyId">Survey id like pNNNNNNN</param>
        /// <returns></returns>
        public int CreateNewInterview(string surveyId)
        {
            var activityEvent = new CreateNewInterviewEvent();

            // Authorization
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out var person, out var task);
            ////////////////////////////////////////////////////////////////////////////////////////////////
            
            var survey = _surveyRepository.TryGetByProjectId(surveyId);

            if (survey == null)
            {
                throw new UserMessageException($"Survey '{surveyId}' is not found");
            }

            if (!survey.IsRespondentsDynamicCreationAllowed)
            {
                throw new CreateNewInterviewException("New interview can not be created because feature was disabled");
            }

            var respondentsInfo = new RespondentsInfo
            {
                Id = 0,
                Values = new Dictionary<string, object>(),
                Links = new Dictionary<string, string>()
            };
            respondentsInfo.Values.Add("TelephoneNumber", null);

            int respondentId = _respondentsClient.AddRespondent(survey.ProjectId, respondentsInfo);

            _interviewService.AddRespondent(survey, respondentId, (int)CallOutcome.FreshSample, OperationType.AddRecordFromConsole, Role.Interviewer, person.SID);

            activityEvent.Save(
                task.PersonSID,
                respondentId,
                survey.SID,
                survey.Name);

            return respondentId;
        }

        /// <summary>
        /// Returns the array of opened surveys available for the logged in person.
        /// The function is used for persons in MANUAL and SURVEY_ASSIGNMENT modes.
        /// </summary>
        public Survey[] GetOpenedSurveys()
        {
            var activityEvent = new GetOpenedSurveysEvent();

            // Authorization
            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            activityEvent.UpdateEventPropertiesFromTask(task);

            var result = (PersonService.GetOpenedSurveysForInterviewer(person.SID)
                .Select(survey => new Survey { id = survey.Name, name = survey.Description, IsRespondentsDynamicCreationAllowed = survey.IsRespondentsDynamicCreationAllowed}))
                .ToArray();

            activityEvent.Save();

            return result;
        }

        /// <summary>
        /// Returns the list of available interviews for a survey. The function is used for
        /// persons in MANUAL mode.
        /// </summary>        
        public DataTable GetSurveyInterviews(string surveyId, SearchParameter[] parameters)
        {
            var activityEvent = new GetSurveyInterviewsEvent();

            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            activityEvent.UpdateEventPropertiesFromTask(task);

            BvSurveyEntity survey = _surveyRepository.GetByName(surveyId);

            var result = ConsoleSurveyInterviewsService.GetSurveyInterviews(survey.SID, person.SID, parameters, (PersonAssignmentListMode)person.AssignmentsListMode);

            activityEvent.Save();

            return result;
        }

        /// <summary>
        /// Returns the array of spell errors.
        /// </summary>
        /// <param name="textBlock">Text block for check spelling.</param>
        /// <param name="languageId">Text block language identifier.</param>
        public SpellError[] CheckTextSpelling(int languageId, string textBlock)
        {
            var activityEvent = new CheckTextSpellingEvent();

            BvTasksEntity task;
            BvPersonEntity person;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            activityEvent.UpdateEventPropertiesFromTask(task);

            activityEvent.AddTiming("AuthoriseRequest");

            var result = new CheckSpellingService(languageId).CheckText(textBlock);

            activityEvent.Save(languageId, textBlock.Length);

            return result;
        }

        /// <summary>
        /// Returns the list of available appointments for a concrete interview.
        /// </summary>
        /// <returns>
        /// The array of <seealso cref="Appointment"/>.
        /// </returns>
        /// <remarks>
        /// CATI console calls the function only while an interview is in progress.
        /// </remarks>
        public Appointment[] GetInterviewAppointmentList(
            string confirmitSurveyId,
            int interviewId)
        {
            var activityEvent = new GetInterviewAppointmentEvent();

            BvTasksEntity task;
            BvPersonEntity person;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            activityEvent.UpdateEventPropertiesFromTask(task);

            var survey = _surveyRepository.GetByName(confirmitSurveyId);
            var appointmentEntities = SurveyService.GetAppointments(survey.SID, interviewId);

            var resultAppointments = new Appointment[appointmentEntities.Length];

            int i = 0;
            foreach (var appt in appointmentEntities)
            {
                var appointment = new Appointment
                {
                    contactName = appt.ContactName,
                    time = appt.Time,
                    expirationTime = appt.ExpTime,
                    InterviewId = appt.InterviewSID
                };

                resultAppointments[i++] = appointment;
            }

            activityEvent.Save(confirmitSurveyId, interviewId);

            return resultAppointments;
        }

        public Timezone GetInterviewTimezone(string surveyId, int interviewId)
        {
            _consoleWsRequestsAuthoriser.AuthoriseRequest();

            return
                _callConsoleServiceHelper.GetTimeZone(
                    _interviewService.GetInterviewTimezoneOrDefault(SurveyRepository.GetByName(surveyId).SID, interviewId));
        }


        /// <summary>
        /// Returns list of availaable tragets for internal transferring 
        /// </summary>
        /// <returns>List of availaable tragets for internal transferring </returns>
        public InternalTransferTarget[] GetInternalTransferTargets()
        {
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out _, out var task);

            if (task.InterviewState != (int) InterviewState.INTERVIEWING)
                throw new Exception("Wrong interviewing state");

            return BvSpTransfer_GetInternalTargetsAdapter
                .ExecuteEntityList(task.PersonSID, task.SurveySID, task.DialTypeId, task.DialerId).Select(
                    x => new InternalTransferTarget
                    {
                        Name = x.Name,
                        Description = x.Description,
                        CountOfTotalInterviewersLoggedIn = x.CountOfTotalInterviewersLoggedIn.GetValueOrDefault(),
                        CountOfFreeInterviewersLoggedIn = x.CountOfFreeInterviewersLoggedIn.GetValueOrDefault()
                    }).ToArray();
        }

        public ExternalTransferTarget[] GetExternalTransferTargets()
        {
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out _, out var task);

            if (task.InterviewState != (int)InterviewState.INTERVIEWING)
                throw new Exception("Wrong interviewing state");

            return BvSpTransfer_GetExternalTargetsAdapter.ExecuteEntityList(task.SurveySID).Select(
                x => new ExternalTransferTarget()
                {
                    TelephoneNumber = x.TelephoneNumber,
                    Description = x.Description
                }).ToArray();
        }

        /// <summary>
        /// Sets an appointment list for a definite interview.
        /// (All previously set appointments for the interview are being deleted).
        /// </summary>
        /// <remarks>
        /// CATI console calls the function only while an interview is in progress.
        /// </remarks>
        public void SetInterviewAppointmentList(string confirmitSurveyId, int interviewId, Appointment[] appointments, bool allowOutsideShift)
        {
            //TODO: always contains 1 appointment, correct method signature.
            //TODO: confirmitSurveyId is bad name, we should use same name in all methods

            var activityEvent = new SetInterviewAppointmentEvent();

            // Authorization
            BvPersonEntity person;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person);
            ////////////////////////////////////////////////////////////////////////////////////////////////

            var survey = _surveyRepository.GetByName(confirmitSurveyId);
            var appointment = appointments[0];

            _interviewService.AddAppointments(survey.SID, interviewId, 0, appointments, allowOutsideShift);

            activityEvent.Save(
                person.SID,
                survey.SID,
                survey.Name,
                appointment.contactName,
                appointment.time,
                appointment.expirationTime,
                appointment.state,
                interviewId);
        }

        /// <summary>
        /// Returns the list of all available appointments.
        /// </summary>
        /// <returns>
        /// The array of <seealso cref="Appointment"/>.
        /// </returns>
        public Appointment[] GetAllAppointmentList()
        {
            var activityEvent = new GetAllAppointmentListEvent();

            // Authorization
            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            activityEvent.UpdateEventPropertiesFromTask(task);
            ////////////////////////////////////////////////////////////////////////////////////////////////

            var allAppointments = BvSpGetAllAppointmentsForUserAdapter.ExecuteEntityList(person.SID);

            var result = allAppointments.Select(appointment => new Appointment
            {
                id = appointment.ID.Value,
                InterviewId = appointment.InterviewSID.Value,
                contactName = appointment.ContactName,
                time = appointment.Time.Value,
                expirationTime = appointment.ExpTime,
                projectID = appointment.ProjectID,
                projectName = appointment.projectName,
                appointmentTimeZone = _callConsoleServiceHelper.GetTimeZone(appointment.TZID ?? 0)
            }).ToArray();

            activityEvent.Save();

            return result;
        }

        /// <summary>
        /// Returns the list of all available messages for current interviewer
        /// </summary>
        /// <returns>
        /// The array of Messages objects/>.
        /// </returns>
        public Messages[] GetMessages()
        {
            var activityEvent = new GetMessagesEvent();

            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            activityEvent.UpdateEventPropertiesFromTask(task);

            var result = PersonService.GetMessages(person.SID).ToArray();

            activityEvent.Save();

            var messages = new Messages[result.Length];
            for (int i = 0; i < result.Length; i++)
            {
                messages[i] = new Messages
                {
                    Body = result[i].Body,
                    CreateTime = result[i].CreateTime,
                    SupervisorName = result[i].SupervisorName
                };
            }

            return messages;
        }

        /// <summary>
        /// If openend review is available for the survey the interviewer currently works on then
        /// the function switches the interviewer into openend review mode and returns true.
        /// The function returns false otherwise.
        /// </summary>
        /// <param name="attemptNumber"></param>
        public bool GetForceOpenendReview(int attemptNumber)
        {
            var activityEvent = new GetForceOpenendReviewEvent();

            // Authorization
            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            activityEvent.AddTiming("AuthoriseRequest");
            activityEvent.UpdateEventPropertiesFromTask(task);
            
            if (attemptNumber > 1)
            {
                Trace.TraceWarning(
                    "{0} attempt to make ConsoleService.GetForceOpenendReview for person {1}, InterviewId = {2}",
                    attemptNumber, person.SID, task.InterviewID);

                //Check the state
                if (task.InterviewState != (byte)InterviewState.INTERVIEWING)
                {
                    //A previous GetForceOpenendReview made the work, we must not try GetForceOpenendReview again,
                    Trace.TraceWarning(
                        "ConsoleService.GetForceOpenendReview is not proceeded at {0} attempt for person {1}, InterviewId = {2}" +
                        "because the person currently has task.InterviewState = {3}",
                        attemptNumber, person.SID, task.InterviewID, task.InterviewState);
                    return (task.InterviewState == (byte)InterviewState.OPENEND_REVIEW);
                }
            }

            //Get surveySID the person currently works on from BvTasks table.

            if (task.SurveySID == 0)
                return false;

            BvSurveyEntity survey = _surveyRepository.GetById(task.SurveySID);
            activityEvent.AddTiming("SurveyRepository.GetById");

            if (survey.ForceOpnRev > 0)
            {
                _callHandlerRoot.CancelTransferIfNeed(task, person);                
                _taskRepository.Update(task);
                TaskService.MoveTaskToState(task, InterviewState.OPENEND_REVIEW, DialingMode.Manual);
                task = _taskRepository.GetById(task.SurveySID, task.InterviewID);
                task.OpenEndReviewStartTime = _timeService.GetUtcNow();
                _taskRepository.Update(task);
                activityEvent.AddTiming("MoveTaskToState");
            }

            if (BvCallHandlerRoot.DoesHangupMakeSense(task))
            {
                try
                {
                    Hangup(0);
                    activityEvent.AddTiming("Hangup");
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning("Exception on Hangup: '{0}'.", ex);
                }
            }

            activityEvent.Save();
            return survey.ForceOpnRev > 0;
        }

        /// <summary>
        /// The method initiates dial process.
        /// CATI console can call this method only if there is a dialer in the system.
        /// </summary>
        /// <param name="phoneNumber"> The respondent telephone number </param>
        /// <param name="initiator"> 0 - script, 1 - telephone menu </param>
        /// <param name="attemptNumber"></param>
        /// <returns>
        /// <c>true</c> if dialing process was successfully st<c>false</c>, false otherwise.
        /// </returns>
        /// <remarks>
        /// Dial operation is asynchronous. CATI console must call GetState method 
        /// to find out if dialing process has finished.
        /// </remarks>
        public void Dial(string phoneNumber, int initiator, int attemptNumber)
        {
            if (initiator == MenuInitiator)
            {
                Redial(phoneNumber);
                return;
            }

            Dial(phoneNumber, attemptNumber);
        }

        /// <summary>
        /// The method cancels the dial process.
        /// CATI console can call this method only if there is a dialer in the system.
        /// </summary>
        /// <returns>
        /// <c>true</c> if cancel dialing process was successfully st<c>false</c>, <c>false</c> otherwise.
        /// </returns>
        /// <remarks>
        /// CancelDialing operation is asynchronous. CATI console must call GetState method 
        /// to find out if dialing process has finished.
        /// 
        /// !The function is not supported for the moment. It is reserved for future needs.
        /// </remarks>
        public void CancelDialing()
        {
        }

        /// <summary>
        /// Respondent hangup.
        /// CATI console can call this method only if there is a dialer in the system.
        /// </summary>
        /// <param name="initiator"> 0 - script, 1 - telephone menu, 2 - dial cancellation </param>
        /// <returns>
        /// <c>true</c> if hangup succeeded, <c>false</c> if an error occurred during hangup.
        /// </returns>
        /// <remarks>
        /// We suppose Hangup operation is synchronous.
        /// </remarks>
        public bool Hangup(int initiator)
        {
            var activityEvent = new HangupEvent();
            BvPersonEntity person = _consoleWsRequestsAuthoriser.AuthoriseRequest();

            using (new EventDetailsScope(activityEvent.Details))
            using (TaskLocker.Lock(person, out var task))
            {
                // Authorization
                activityEvent.AddTiming("AuthoriseRequest");
                activityEvent.UpdateEventPropertiesFromTask(task);
                ////////////////////////////////////////////////////////////////////////////////////////////////

                if (!_dialerCollection.IsDialerInitialized(task.DialerId))
                {
                    //we have unavailable diallier.
                    //we return true (hardly CF processed error, because true), set telephony problem state.
                    //It made for supervisor: many telephony errors are appear in
                    //activity view.
                    Trace.TraceWarning(
                        "ConsoleService.Hangup: hangup called while dialer is not available." +
                        " /// personId={0}, dialerId={1}",
                        task.PersonSID,
                        task.DialerId);
                    return true;
                }

                activityEvent.AddTiming("IsDialerInitialized");

                //Get surveySID from BvTasks table
                var surveySid = task.SurveySID;

                var survey = _surveyRepository.GetById(surveySid);
                activityEvent.AddTiming("SurveyRepository.GetById");

                //While survey mode is manual we need not command hangup.
                if (!BvCallHandlerRoot.IsLoggedInToDialer(task) || survey.DialingMode == DialingMode.Manual)
                {
                    //we return true
                    //cati console send Complete to CF, because hardly CF processed calloutcome

                    return true;
                }

                //It seems we do not need to reflect hangup in BvTasks table ???
                //So simply call iMnTciLibrary.

                _callHandlerRoot.CancelTransferIfNeed(task, person);

                var hangupResult = _activeDialService.Hangup(
                    task,
                    survey,
                    initiator);
                activityEvent.AddTiming("Hangup");

                if (hangupResult != DialerErrorCode.Success)
                {
                    // Error at hangup
                    Trace.TraceWarning(
                        "ConsoleService.Hangup: Hangup failed. /// personId={0}, dialerId={1}, Telephony error={2}",
                        task.PersonSID,
                        task.DialerId,
                        hangupResult);
                }

                activityEvent.Details.DialerId = task.DialerId;
                activityEvent.Details.Initiator = initiator;

                _taskRepository.Update(task);

            }

            activityEvent.Save();


            return true;
        }

        public void TransferStart(TransferOptions options)
        {
            var activityEvent = new TransferStartEvent(options);

            BvPersonEntity person = _consoleWsRequestsAuthoriser.AuthoriseRequest();

            using(TaskLocker.Lock(person, out var task))
            {
                _consoleTransferStartProcessor.TransferStart(task, person, options, activityEvent);

                _taskRepository.Update(task);
            }

            activityEvent.Save();
        }

        public void TransferSetConnectionState(TransferConnectionState transferConnectionState)
        {
            var activityEvent = new TransferSetConnectionStateEvent();

            BvPersonEntity person = _consoleWsRequestsAuthoriser.AuthoriseRequest();

            using (TaskLocker.Lock(person, out var task))
            {
                _consoleTransferSetConnectionStateProcessor.TransferSetConnectionState(task, person,
                    transferConnectionState, activityEvent);

                _taskRepository.Update(task);
            }

            activityEvent.Save();
        }

        public void TransferComplete()
        {
            var activityEvent = new TransferCompleteEvent();

            BvPersonEntity person = _consoleWsRequestsAuthoriser.AuthoriseRequest();

            using (TaskLocker.Lock(person, out var task))
            {
                _consoleTransferCompleteProcessor.TransferComplete(task, person, activityEvent);

                _taskRepository.Update(task);
            }

            activityEvent.Save();
        }

        public void TransferCancel()
        {
            //Idea to refactoring in future
            //state object will has got following property: Person, Task(can be null, if task doesn't exists )
            //state.Commit() will commit changes to databases and release lock, otherwise lock will be relased in Dispose without saveing changes

            //var activityEvent = new TransferCancelEvent();
            //
            //using (var state = LockedConsoleState.GetByRequest())
            //{
            //    _consoleTransferCancelProcessor.TransferCancel(state);
            //    state.Commit();
            //}
            //
            //activityEvent.Save();

            var activityEvent = new TransferCancelEvent();

            BvPersonEntity person = _consoleWsRequestsAuthoriser.AuthoriseRequest();

            using (TaskLocker.Lock(person, out var task))
            {
                _consoleTransferCancelProcessor.TransferCancel(task, person, activityEvent);

                _taskRepository.Update(task);
            }

            activityEvent.Save();
        }

        public void WrapUp(int interviewId, int attemptNumber)
        {
            WrapUp(interviewId, true, attemptNumber, new CompletedInterviewDetails { InterviewDuration = 0, Its = "13", Status = "Complete" });
        }

        /// <summary>
        /// CATI console must call this method at the moment when an interview is finished.
        /// </summary>
        public void WrapUp(int interviewId, bool lookUpForNewCalls, int attemptNumber, CompletedInterviewDetails details)
        {
            //TODO: implementation transferComplete
            var activityEvent = new WrapUpEvent();

            activityEvent.Details.InterviewDetails = details;

            // Authorization
            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);

            _consoleWrapUpProcessor.WrapUp(person, task, interviewId, lookUpForNewCalls, attemptNumber, details, WrapUpReason.CompeteInterview, activityEvent);

            activityEvent.Save();
        }

        public void StartPlayback(string soundFileName, out int timeOfPlayingInSeconds)
        {
            var activityEvent = new StartPlaybackEvent();

            // Authorization
            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            ////////////////////////////////////////////////////////////////////////////////////////////////

            if (!_dialerCollection.IsDialerInitialized(task.DialerId))
            {
                Trace.TraceWarning(
                    "ConsoleService.StartPlayback: StartPlayback called while dialer is not available." +
                    " /// personId={0}, dialerId={1}",
                    task.PersonSID,
                    task.DialerId);

                timeOfPlayingInSeconds = 0;
                return;
            }

            var survey = _surveyRepository.GetById(task.SurveySID);
            var startPlaybackResult = _telephony.StartPlayback(
                task.DialerId,
                survey.CampaignId,
                task.PersonSID.ToString(),
                task.InterviewID,
                task.CallID.Value,
                soundFileName,
                out timeOfPlayingInSeconds);

            if (startPlaybackResult != DialerErrorCode.Success)
            {
                Trace.TraceError(
                    "ConsoleService.StartPlayback: " + Strings.StartPlaybackError +
                    ". /// personId={0}, dialerId={1}, Telephony error={2}",
                    task.PersonSID,
                    task.DialerId,
                    startPlaybackResult);
            }

            activityEvent.Save(task.PersonSID, task.InterviewID, survey.SID, survey.Name, task.DialerId, soundFileName);
        }

        public void StopPlayback()
        {
            var activityEvent = new StopPlaybackEvent();

            // Authorization
            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            ////////////////////////////////////////////////////////////////////////////////////////////////

            if (!_dialerCollection.IsDialerInitialized(task.DialerId))
            {
                Trace.TraceWarning(
                    "ConsoleService.StopPlayback: StopPlayback called while dialer is not available." +
                    " /// personId={0}, dialerId={1}",
                    task.PersonSID,
                    task.DialerId);
                return;
            }

            var survey = _surveyRepository.GetById(task.SurveySID);
            var stopPlaybackResult = _telephony.StopPlayback(
                task.DialerId,
                survey.CampaignId,
                task.PersonSID.ToString(),
                task.InterviewID,
                task.CallID.Value);

            if (stopPlaybackResult != DialerErrorCode.Success)
            {
                Trace.TraceError(
                    "ConsoleService.StopPlayback: " + Strings.StopPlaybackError +
                    ". /// personId={0}, dialerId={1}, Telephony error={2}",
                    task.PersonSID,
                    task.DialerId,
                    stopPlaybackResult);
            }

            activityEvent.Save(task.PersonSID, task.InterviewID, survey.SID, survey.Name, task.DialerId);
        }

        public void PauseOrResumePlayback()
        {
            var activityEvent = new PauseOrResumePlaybackEvent();

            // Authorization
            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            ////////////////////////////////////////////////////////////////////////////////////////////////

            if (!_dialerCollection.IsDialerInitialized(task.DialerId))
            {
                Trace.TraceWarning(
                    "ConsoleService.PauseOrResumePlayback: PauseOrResumePlayback called while dialer is not available." +
                    " /// personId={0}, dialerId={1}",
                    task.PersonSID,
                    task.DialerId);
                return;
            }

            var survey = _surveyRepository.GetById(task.SurveySID);
            var pauseOrResumePlaybackResult = _telephony.PauseOrResumePlayback(
                task.DialerId,
                survey.CampaignId,
                task.PersonSID.ToString(), 
                task.InterviewID,
                task.CallID.Value);

            if (pauseOrResumePlaybackResult != DialerErrorCode.Success)
            {
                Trace.TraceError(
                    "ConsoleService.PauseOrResumePlayback: " + Strings.PauseOrResumePlaybackError +
                    ". /// personId={0}, dialerId={1}, Telephony error={2}",
                    task.PersonSID,
                    task.DialerId,
                    pauseOrResumePlaybackResult);
            }

            activityEvent.Save(task.PersonSID, task.InterviewID, survey.SID, survey.Name, task.DialerId);
        }

        public void ToggleInterviewerListensToPlaybackOrRespondent()
        {
            var activityEvent = new ToggleInterviewerListensToPlaybackOrRespondentEvent();

            // Authorization
            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            activityEvent.UpdateEventPropertiesFromTask(task);
            ////////////////////////////////////////////////////////////////////////////////////////////////

            if (!_dialerCollection.IsDialerInitialized(task.DialerId))
            {
                Trace.TraceWarning(
                    "ConsoleService.ToggleInterviewerListensToPlaybackOrRespondent: ToggleInterviewerListensToPlaybackOrRespondent called while dialer is not available." +
                    " /// personId={0}, dialerId={1}",
                    task.PersonSID,
                    task.DialerId);
                return;
            }

            var survey = _surveyRepository.GetById(task.SurveySID);
            var toggleInterviewerListensToPlaybackOrRespondentResult = _telephony.ToggleInterviewerListensToPlaybackOrRespondent(
                task.DialerId,
                survey.CampaignId,
                task.PersonSID.ToString(CultureInfo.InvariantCulture),
                task.InterviewID,
                task.CallID.Value);

            if (toggleInterviewerListensToPlaybackOrRespondentResult != DialerErrorCode.Success)
            {
                Trace.TraceError(
                    "ConsoleService.ToggleInterviewerListensToPlaybackOrRespondent: " + Strings.ToggleInterviewerListensToPlaybackOrRespondentError +
                    ". /// personId={0}, dialerId={1}, Telephony error={2}",
                    task.PersonSID,
                    task.DialerId,
                    toggleInterviewerListensToPlaybackOrRespondentResult);
            }

            activityEvent.Save(task.DialerId);
        }

        public bool SetPendingBreakStatus(PendingBreakStatus status, int? breakType)
        {
            var activityEvent = new SetPendingBreakStatusEvent();

            // Authorization
            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            ////////////////////////////////////////////////////////////////////////////////////////////////

            var previousStatusLogout = task.StatusLogout;

            var result = _callConsoleServiceHelper.SetPendingBreakStatus(
                task,
                person,
                status,
                breakType);

            activityEvent.Save(
                    person.SID,
                    task.SurveySID,
                    task.SurveySID != 0 ? _surveyRepository.GetById(task.SurveySID).ProjectId : "",
                    status,
                    task.InterviewID,
                    (LoginState)previousStatusLogout,
                    breakType);

            return result;
        }

        public void ContinueWorkAfterBreak(int attemptNumber)
        {
            var activityEvent = new ContinueWorkAfterBreakEvent();

            using (new EventDetailsScope(activityEvent.Details))
            {
                // Authorization
                BvPersonEntity person;
                BvTasksEntity task;
                _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
                ////////////////////////////////////////////////////////////////////////////////////////////////

                _callConsoleServiceHelper.ContinueWorkAfterBreak(task, attemptNumber);

                activityEvent.Save(person.SID);
            }
        }

        public LanguageCollection GetSurveyLanguages(string projectId)
        {
            var activityEvent = new GetSurveyLanguagesEvent();

            BvTasksEntity task;
            BvPersonEntity person;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            activityEvent.UpdateEventPropertiesFromTask(task);

            var result = new LanguageBuilder().GetLanguageCollection(_authoringService.GetSurveyLanguages(projectId));

            activityEvent.Save(projectId);

            return result;
        }

        public QuestionHistoryCollection GetInterviewHistory(string projectId, string respondentIdentity, int languageId)
        {
            var activityEvent = new GetInterviewHistoryEvent();

            BvTasksEntity task;
            BvPersonEntity person;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            activityEvent.UpdateEventPropertiesFromTask(task);

            var result = new QuestionHistoryBuilder().GetQuestionHistoryCollection(_authoringService.GetInterviewHistoryWithValidBackTo(projectId, respondentIdentity, languageId));

            activityEvent.Save(projectId, respondentIdentity, languageId);

            return result;
        }

        public Guid GenerateAuthenticationKey()
        {
            var evt = new GenerateAuthenticationKeyEvent();

            // Authorization
            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);
            ////////////////////////////////////////////////////////////////////////////////////////////////

            var oldKey = task.AuthenticationKey;

            TaskService.GenerateAndUpdateAuthenticationKeyForTask(task);
            var newKey = task.AuthenticationKey.Value;

            evt.Save(person.SID, oldKey, newKey);

            return newKey;
        }

        public void TerminateTask()
        {
            var terminateTaskFromConsoleEvent = new TerminateTaskFromConsoleEvent();

            // Authorization
            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);

            terminateTaskFromConsoleEvent.AddTiming("AuthoriseRequest");
            terminateTaskFromConsoleEvent.UpdateEventPropertiesFromTask(task);
            terminateTaskFromConsoleEvent.Details.Task = task;

            TaskService.TerminateTask(
               person.SID,
               new DatabaseTransactionOptions("TerminateTask.TerminateTask", DeadlockPriority.Normal),
               CallOutcome.InterruptedByInterviewer);

            terminateTaskFromConsoleEvent.AddTiming("TerminateTask");
            terminateTaskFromConsoleEvent.UpdateEventPropertiesFromTask(task);

            terminateTaskFromConsoleEvent.Save(person.SID);
        }

        private string GetPhoneNumber(string phoneNumber, string interviewTelephoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber) ? phoneNumber : interviewTelephoneNumber;
        }

        private void Redial(string phoneNumber)
        {
            var redialEvent = new RedialEvent();

            BvPersonEntity person;
            BvTasksEntity task;
            _consoleWsRequestsAuthoriser.AuthoriseRequest(out person, out task);

            redialEvent.AddTiming("AuthoriseRequest");
            redialEvent.UpdateEventPropertiesFromTask(task);

            if (task.CallOutcome == (int)CallOutcome.NotDefined)
            {
                redialEvent.AddTiming("Redial is not proceeded because the has task.CallOutcome=NotDefined");
                redialEvent.Save();
                return;
            }

            BvSurveyEntity survey = _surveyRepository.GetById(task.SurveySID);
            redialEvent.AddTiming("SurveyRepository.GetById");

            if (survey.DialingMode == DialingMode.Manual)
            {
                redialEvent.AddTiming("Redial is not proceeded because the has survey.DialMode=Manual");
                redialEvent.Save();
                return;
            }

            var interview = InterviewRepository.GetByIdWithCheck(survey.SID, task.InterviewID);
            redialEvent.AddTiming("InterviewRepository.GetByIdWithCheck");

            var currentPhoneNumber = GetPhoneNumber(phoneNumber, interview.TelephoneNumber);

            redialEvent.AddTiming("SurveyService.ProjectIdToCampaignId");


            _callHandlerRoot.CancelTransferIfNeed(task, person);

            var dial = _activeDialRepository.TryGetByCallId(task.CallID);

            var redialResult = _activeDialService.Redial(ref dial, task, survey, interview, currentPhoneNumber);

            _redialNumberSaver.SaveAlternativeNumber(task.SurveySID, currentPhoneNumber, task.InterviewID);

            if (redialResult != DialerErrorCode.Success)
            {
                BvCallHandlerRoot.ProcessTelephonyError(dial, task, redialResult);
                redialEvent.AddTiming("BvCallHandlerRoot.ProcessTelephonyError");
            }
            else
            {
                task.InterviewState = (byte)InterviewState.REDIALLING;
                task.CallOutcome = (int)CallOutcome.NotDefined;
            }

            TaskRepository.Update(task);
            redialEvent.AddTiming("TaskRepository.Update");

            redialEvent.Details.DialerId = task.DialerId;
            redialEvent.PhoneNumber = currentPhoneNumber;
            redialEvent.Save();
        }

        private void Dial(string phoneNumber, int attemptNumber)
        {
            var activityEvent = new DialEvent();

            // Authorization
            BvPersonEntity person = _consoleWsRequestsAuthoriser.AuthoriseRequest();

            using (new EventDetailsScope(activityEvent.Details))
            using (TaskLocker.Lock(person, out var task))
            {
                activityEvent.AddTiming("AuthoriseRequest");
                activityEvent.UpdateEventPropertiesFromTask(task);

                _callHandlerRoot.CancelTransferIfNeed(task, person);

                if (!ServiceLocator.Resolve<IConsoleDialProcessor>().Dial(person, task, phoneNumber, attemptNumber, activityEvent))
                    return;

                _taskRepository.Update(task);
                activityEvent.AddTiming("TaskRepository.Update");
            }

            activityEvent.Save();
        }
    }
}
