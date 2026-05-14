using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using BvCallHandlerLibrary;
using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Backend.WcfServices.Tools;
using Confirmit.CATI.Backend.WcfServices.Tools.IPFilter;
using Confirmit.CATI.Backend.WcfServices.Tools.Logging;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.WcfTools.ErrorContextHandler;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Services.SurveyArchiveServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using ConfirmitDialerInterface;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.LinkedInterviews;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Telephony.LinkedSurveys;
using Microsoft.Practices.ObjectBuilder2;
using Newtonsoft.Json;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.Services.WaitingService;

namespace Confirmit.CATI.Backend.WcfServices.Internal.ManagementService
{
    [IpFilterBehavior]
    [ErrorContextHandler(WebServiceType.Internal)]
    [HeadersHandler]
    [MetricsBehaviour(TrackMethodsSeparately = true, ExcludeMethodsPrefix = "Telephony_")]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class ManagementService : IManagementService
    {
        private readonly Lazy<ISurveyRepository> _surveyRepository;
        private readonly Lazy<IUserSurveyPermissionRepository> _permissionRepository;
        private readonly Lazy<ICallCenterService> _callCenterService;
        private readonly Lazy<ICallCenterRepository> _callCenterRepository;
        private readonly Lazy<IAsyncOperationQueue> _asyncOperationQueue;
        private readonly Lazy<ISurveyService> _surveyService;
        private readonly Lazy<IScheduleRepository> _scheduleRepository;
        private readonly Lazy<ISurveyPublishService> _surveyPublishService;
        private readonly Lazy<ISurveyStateService> _surveyStateService;
        private readonly Lazy<IConfirmitDatabaseProvider> _confirmitDatabaseProvider;
        private readonly Lazy<ISampleService> _sampleService;
        private readonly Lazy<IReplicationService> _replicationService;
        private readonly Lazy<ISurveyArchiveService> _surveyArchiveService;
        private readonly Lazy<IInterviewRepository> _interviewRepository;
        private readonly Lazy<IFcdQuotaService> _fcdQuotaService;
        private readonly Lazy<ISupervisorNameProvider> _supervisorNameProvider;
        private readonly Lazy<IAsyncOperationRepository> _asyncOperationRepository;
        private readonly Lazy<IInterviewRecordingManager> _interviewRecordingManager;
        private readonly Lazy<IInterviewService> _interviewService;
        private readonly Lazy<IInterviewHistoryAndDataProcessor> _interviewHistoryAndDataProcessor;
        private readonly Lazy<ITaskRepository> _taskRepository;
        private readonly Lazy<ILinkedInterviewProvider> _linkedInterviewProvider;
        private readonly Lazy<IRedialNumberSaver> _redialNumberSaver;

        private readonly Lazy<IDialerOperationalStateNotificator> _dialerOperationalStateNotificator;
        private readonly Lazy<IDialersRepository> _dialersRepository;
        private readonly Lazy<ITelephony> _telephony;
        private readonly Lazy<IMnTciTools> _mnTciTools;
        private readonly Lazy<ICompanyInfo> _companyInfo;
        private readonly Lazy<ITelephoneBlacklistRepository> _telephoneBlacklistRepository;
        private readonly Lazy<ITelephoneBlacklistService> _telephoneBlacklistService;
        private readonly Lazy<IActiveDialRepository> _activeDialRepository;
        private readonly Lazy<IPersonRepository> _personRepository;
        private readonly Lazy<ISystemSettings> _systemSettings;
        private readonly Lazy<IShiftServiceFactory> _shiftServiceFactory;
        private readonly Lazy<IPersonGroupService> _personGroupService;
        private readonly Lazy<IInterviewResponseDataService> _interviewResponseDataService;
        private readonly Lazy<IMonitoringService> _monitoringService;
        private readonly Lazy<ISqlTableUpdatedPublisher> _sqlTableUpdatedPublisher;
        private readonly Lazy<DialingAttemptsService> _dialingAttemptsService;
        private readonly Lazy<IDatabaseLockTimeouts> _databaseLockTimeouts;
        
        public ManagementService()
        {
            _surveyRepository = new Lazy<ISurveyRepository>(() => ServiceLocator.Resolve<ISurveyRepository>());
            _permissionRepository = new Lazy<IUserSurveyPermissionRepository>(() => ServiceLocator.Resolve<IUserSurveyPermissionRepository>());
            _callCenterService = new Lazy<ICallCenterService>(() => ServiceLocator.Resolve<ICallCenterService>());
            _callCenterRepository = new Lazy<ICallCenterRepository>(() => ServiceLocator.Resolve<ICallCenterRepository>());
            _asyncOperationQueue = new Lazy<IAsyncOperationQueue>(() => ServiceLocator.Resolve<IAsyncOperationQueue>());
            _surveyService = new Lazy<ISurveyService>(() => ServiceLocator.Resolve<ISurveyService>());
            _scheduleRepository = new Lazy<IScheduleRepository>(() => ServiceLocator.Resolve<IScheduleRepository>());
            _surveyPublishService = new Lazy<ISurveyPublishService>(() => ServiceLocator.Resolve<ISurveyPublishService>());
            _surveyStateService = new Lazy<ISurveyStateService>(() => ServiceLocator.Resolve<ISurveyStateService>());
            _sampleService = new Lazy<ISampleService>(() => ServiceLocator.Resolve<ISampleService>());
            _replicationService = new Lazy<IReplicationService>(() => ServiceLocator.Resolve<IReplicationService>());
            _confirmitDatabaseProvider = new Lazy<IConfirmitDatabaseProvider>(() => ServiceLocator.Resolve<IConfirmitDatabaseProvider>());
            _surveyArchiveService = new Lazy<ISurveyArchiveService>(() => ServiceLocator.Resolve<ISurveyArchiveService>());
            _interviewRepository = new Lazy<IInterviewRepository>(() => ServiceLocator.Resolve<IInterviewRepository>());
            _fcdQuotaService = new Lazy<IFcdQuotaService>(() => ServiceLocator.Resolve<IFcdQuotaService>());
            _supervisorNameProvider = new Lazy<ISupervisorNameProvider>(() => ServiceLocator.Resolve<ISupervisorNameProvider>());
            _asyncOperationRepository = new Lazy<IAsyncOperationRepository>(() => ServiceLocator.Resolve<IAsyncOperationRepository>());
            _interviewRecordingManager = new Lazy<IInterviewRecordingManager>(() => ServiceLocator.Resolve<IInterviewRecordingManager>());
            _interviewService = new Lazy<IInterviewService>(() => ServiceLocator.Resolve<IInterviewService>());
            _interviewHistoryAndDataProcessor = new Lazy<IInterviewHistoryAndDataProcessor>(() => ServiceLocator.Resolve<IInterviewHistoryAndDataProcessor>());
            _taskRepository = new Lazy<ITaskRepository>(() => ServiceLocator.Resolve<ITaskRepository>());
            _linkedInterviewProvider = new Lazy<ILinkedInterviewProvider>(() => ServiceLocator.Resolve<ILinkedInterviewProvider>());
            _redialNumberSaver = new Lazy<IRedialNumberSaver>(() => ServiceLocator.Resolve<IRedialNumberSaver>());

            _dialerOperationalStateNotificator = new Lazy<IDialerOperationalStateNotificator>(() => ServiceLocator.Resolve<IDialerOperationalStateNotificator>());
            _dialersRepository = new Lazy<IDialersRepository>(() => ServiceLocator.Resolve<IDialersRepository>());
            _telephony = new Lazy<ITelephony>(() => ServiceLocator.Resolve<ITelephony>());
            _mnTciTools = new Lazy<IMnTciTools>(() => ServiceLocator.Resolve<IMnTciTools>());
            _companyInfo = new Lazy<ICompanyInfo>(() => ServiceLocator.Resolve<ICompanyInfo>());
            _telephoneBlacklistRepository = new Lazy<ITelephoneBlacklistRepository>(() => ServiceLocator.Resolve<ITelephoneBlacklistRepository>());
            _telephoneBlacklistService = new Lazy<ITelephoneBlacklistService>(() => ServiceLocator.Resolve<ITelephoneBlacklistService>());
            _activeDialRepository = new Lazy<IActiveDialRepository>(() => ServiceLocator.Resolve<IActiveDialRepository>());
            _personRepository = new Lazy<IPersonRepository>(() => ServiceLocator.Resolve<IPersonRepository>());
            _systemSettings = new Lazy<ISystemSettings>(() => ServiceLocator.Resolve<ISystemSettings>());
            _shiftServiceFactory = new Lazy<IShiftServiceFactory>(() => ServiceLocator.Resolve<IShiftServiceFactory>());
            _personGroupService = new Lazy<IPersonGroupService>(() => ServiceLocator.Resolve<IPersonGroupService>());
            _interviewResponseDataService = new Lazy<IInterviewResponseDataService>(() => ServiceLocator.Resolve<IInterviewResponseDataService>());
            _monitoringService = new Lazy<IMonitoringService>(() => ServiceLocator.Resolve<IMonitoringService>());
            _sqlTableUpdatedPublisher = new Lazy<ISqlTableUpdatedPublisher>(() => ServiceLocator.Resolve<ISqlTableUpdatedPublisher>());
            _dialingAttemptsService = new Lazy<DialingAttemptsService>(() => ServiceLocator.Resolve<DialingAttemptsService>());
            _databaseLockTimeouts = new Lazy<IDatabaseLockTimeouts>(() => ServiceLocator.Resolve<IDatabaseLockTimeouts>());
        }

        [LogExceptionAndNotReThrow]
        public bool IsInboundCall(int catiInterviewerId)
        {
            var evt = new CheckCallTypeEvent();

            var task = _taskRepository.Value.GetByPerson(catiInterviewerId);

            evt.UpdateEventPropertiesFromTask(task);

            evt.Save();

            return task?.CallType == (int)CallTypes.Inbound;
        }

        [LogExceptionAndNotReThrow]
        public bool IsIvrCall(int catiInterviewerId)
        {
            var evt = new CheckPersonTypeEvent(catiInterviewerId);

            var person = _personRepository.Value.GetById(catiInterviewerId);

            evt.Save();

            return person.Type == (byte)AgentType.IvrAgent;
        }

        public bool SetNextLinkedInterview(string projectId, int respondentId, int catiInterviewerId)
        {
            var evt = new SetNextLinkedInterviewEvent(projectId, respondentId, catiInterviewerId);

            bool result = true;

            try
            {
                var surveyId = _surveyRepository.Value.GetByProjectId(projectId).SID;
                result = TaskService.SetNextInterviewForPerson(catiInterviewerId, surveyId, respondentId) != null;

                evt.UpdateEventPropertiesFromTask(_taskRepository.Value.GetByPerson(catiInterviewerId));
                evt.Save();
            }
            catch (Exception ex)
            {
                result = false;
                TraceHelper.TraceException(
                    ex,
                    String.Format(
                        "SetNextLinkedInterview projectId:{0}, respondentId:{1}, catiInterviewerId:{2}",
                        projectId,
                        respondentId,
                        catiInterviewerId));
            }

            return result;
        }

        public bool SetNextLinkedInterviewToPrevious(int catiInterviewerId)
        {
            bool result = false;

            var task = _taskRepository.Value.GetByPerson(catiInterviewerId);
            var evt = new SetNextLinkedInterviewToPreviousEvent(catiInterviewerId, task.LinkedChain);

            try
            {
                if (!string.IsNullOrEmpty(task.LinkedChain))
                {
                    var previousInterview = JsonConvert.DeserializeObject<List<LinkedChainItem>>(task.LinkedChain)
                        ?.LastOrDefault();

                    if (previousInterview != null)
                    {
                        TaskService.SetNextLinkedInterviewToPrevious(catiInterviewerId, previousInterview.SurveyId,
                            previousInterview.InterviewId);
                        result = true;
                    }
                }

                evt.UpdateEventPropertiesFromTask(task);
                evt.Save();

            }
            catch (Exception ex)
            {
                result = false;
                TraceHelper.TraceException(
                    ex,
                    $"SetNextLinkedInterviewToPrevious  catiInterviewerId:{catiInterviewerId}");
            }

            return result;
        }

        public CatiInterview[] GetInterviews(string[] projectList, string telephoneNumber, string respondentName, string filter, int catiInterviewerId)
        {

            var evt = new GetInterviewsEvent(projectList, telephoneNumber, respondentName, filter);

            CatiInterview[] result = { };

            try
            {
                result = _linkedInterviewProvider.Value.Find(catiInterviewerId, projectList, telephoneNumber, respondentName, filter).ToArray();
                evt.UpdateEventPropertiesFromTask(_taskRepository.Value.GetByPerson(catiInterviewerId));
                evt.Save();
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(
                    ex,
                    String.Format(
                        "GetInterviews projectList:[{0}], telephoneNumber:{1}, respondentName:{2}, filter:{3}, catiInterviewerId:{4}",
                        projectList == null ? "<NULL>" : projectList.JoinStrings(","),
                        telephoneNumber,
                        respondentName,
                        filter,
                        catiInterviewerId));
            }

            return result;
        }

        public CatiInterview[] GetLinkedInterviews(int catiInterviewerId)
        {
            var task = _taskRepository.Value.GetByPerson(catiInterviewerId);

            var evt = new GetLinkedInterviewsEvent(catiInterviewerId, task.LinkedChain);
            var interviews = new CatiInterview[] { };

            try
            {
                interviews = _linkedInterviewProvider.Value.GetLinkedInterviews(task.LinkedChain).ToArray();
                evt.UpdateEventPropertiesFromTask(task);
                evt.Save();
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(
                    ex,
                    String.Format(
                        "GetLinkedInterviews  catiInterviewerId:{0}",
                        catiInterviewerId));
            }

            return interviews;
        }

        /// <summary>
        /// It creates a survey if it was not created.
        /// </summary>
        /// <param name="confirmitProjectID">The confirmit project ID.</param>
        /// <param name="confirmitSurveyName">Name of the confirmit survey.</param>
        /// <param name="cfSqlServerConnectionString">The survey DB SQL server connection string. Without login and password.</param>
        /// <param name="userName">User name.</param>
        public void AddSurvey(string confirmitProjectID, string confirmitSurveyName, string cfSqlServerConnectionString, string userName)
        {
            using (var dbTransactionScope = new DatabaseTransactionScope("ManSrv.AddSurvey"))
            {
                var evt = new AddSurveyViaWsEvent(0, confirmitProjectID, confirmitSurveyName, cfSqlServerConnectionString);

                var survey = _surveyService.Value.CreateSurvey(
                    confirmitProjectID,
                    confirmitSurveyName,
                    cfSqlServerConnectionString,
                    userName,
                    _confirmitDatabaseProvider.Value.GetSqlServerName(confirmitProjectID));

                evt.ObjectId = survey.SID;

                evt.Finish();

                dbTransactionScope.Commit();
            }
        }

        public int DeleteSurvey(string confirmitProjectID)
        {
            _surveyService.Value.ValidateProjectId(confirmitProjectID);

            var evt = new DeleteSurveyViaWsEvent(0, confirmitProjectID);

            var survey = _surveyRepository.Value.TryGetByName(confirmitProjectID);
            if (survey == null)
            {
                Trace.TraceWarning($"Survey '{confirmitProjectID}' can't be removed in DeleteSurvey method because project ID is wrong or survey has already been removed");
                return 0;
            }

            evt.ObjectId = survey.SID;

            var title = string.Format("Delete Survey '{0}' ({1})", survey.Name, survey.Description);

            var param = new Core.AsyncOperations.Operations.DeleteSurvey.Parameters
            {
                SurveyId = survey.SID,
                ProjectId = confirmitProjectID
            };

            var operationEntity = _asyncOperationQueue.Value.Enqueue(
                0,
                title,
                false,
                param,
                AsyncOperationConstants.HighPriority,
                _supervisorNameProvider.Value.Name);

            evt.Finish();

            return operationEntity.Id;
        }

        public void SoftDeleteSurvey(string confirmitProjectID)
        {
            _surveyService.Value.ValidateProjectId(confirmitProjectID);

            var survey = _surveyRepository.Value.TryGetByName(confirmitProjectID);
            if (survey == null)
            {
                Trace.TraceWarning($"Survey '{confirmitProjectID}' can't be removed in SoftDeleteSurvey method because project ID is wrong or survey has already been removed");
                return;
            }

            _surveyStateService.Value.ShutdownSurvey(survey.SID);

            using (var dbTransactionScope = new DatabaseTransactionScope("ManSrv.SoftDeleteSurvey"))
            {
                var evt = new SoftDeleteSurveyViaWsEvent(survey.SID, confirmitProjectID);

                survey.State = (int)SurveyState.SoftDeleted;
                BvSurveyAdapter.Update(survey);
                _sqlTableUpdatedPublisher.Value.PublishSurveyUpdated();
                
                evt.Finish();

                dbTransactionScope.Commit();
            }
        }

        public void RestoreSoftDeletedSurvey(string confirmitProjectID)
        {
            _surveyService.Value.ValidateProjectId(confirmitProjectID);

            var survey = _surveyRepository.Value.TryGetByName(confirmitProjectID);
            if (survey == null)
            {
                Trace.TraceWarning($"Survey '{confirmitProjectID}' can't be restored in RestoreSoftDeletedSurvey method because project ID is wrong or survey has already been completely removed");
                return;
            }

            if (survey.State != (int)SurveyState.SoftDeleted)
            {
                Trace.TraceWarning($"Survey '{confirmitProjectID}' can't be restored in RestoreSoftDeletedSurvey method because survey is not in SoftDeleted state");
                return;
            }

            using (var dbTransactionScope = new DatabaseTransactionScope("ManSrv.RestoreSoftDeletedSurvey"))
            {
                var evt = new RestoreSoftDeletedSurveyViaWsEvent(survey.SID, confirmitProjectID);

                survey.State = (int)SurveyState.Close;
                BvSurveyAdapter.Update(survey);
                _sqlTableUpdatedPublisher.Value.PublishSurveyUpdated();
                
                evt.Finish();

                dbTransactionScope.Commit();
            }
        }
        
        public void UpdateSurveyAccessList(string userId, string surveyId, bool enabled)
        {
            using (var dbTransactionScope = new DatabaseTransactionScope("ManSrv.UpdateSurveyAccessList"))
            {
                if (enabled)
                {
                    var evt = new AddSurveyAccessViaMsEvent(userId, surveyId);

                    _permissionRepository.Value.Insert(userId, surveyId);

                    evt.Finish();
                }
                else
                {
                    var evt = new DeleteSurveyAccessViaMsEvent(userId, surveyId);

                    _permissionRepository.Value.Delete(userId, surveyId);

                    evt.Finish();
                }

                dbTransactionScope.Commit();
            }
        }

        public void UpdateSurveyProperties(
            string confirmitProjectID,
            string confirmitProjectName,
            int? dialingMode,
            bool? openEndReview,
            bool? voiceRecording,
            bool? screenRecording,
            bool supportBlacklist,
            bool allowRespondentsDynamicCreation,
            string notificationEmail,
            bool enforceHttps)
        {
            using (var dbTransactionScope = new DatabaseTransactionScope("ManSrv.UpdateSurveyProperties"))
            {
                var evt = new UpdateSurveyPropertiesViaMsEvent(
                      confirmitProjectID
                    , confirmitProjectName
                    , dialingMode
                    , openEndReview
                    , voiceRecording
                    , screenRecording
                    , supportBlacklist
                    , allowRespondentsDynamicCreation
                    , notificationEmail
                    );

                _surveyService.Value.ValidateProjectId(confirmitProjectID);

                var survey = _surveyRepository.Value.GetByName(confirmitProjectID);

                evt.ObjectId = survey.SID;

                if (dialingMode != null)
                {
                    survey.DialMode = (byte)dialingMode;
                }

                if (confirmitProjectName != null)
                {
                    survey.Description = confirmitProjectName;
                }

                if (openEndReview != null)
                {
                    survey.ForceOpnRev = openEndReview.Value ? 1 : 0;
                }

                if (voiceRecording != null)
                {
                    survey.RecWholeInt = voiceRecording.Value ? 1 : 0;
                }

                if (screenRecording != null)
                {
                    survey.InterviewScreenRecording = screenRecording.Value;
                }

                survey.IsTelephoneBlacklistSupported = supportBlacklist;
                survey.IsRespondentsDynamicCreationAllowed = allowRespondentsDynamicCreation;
                survey.NotificationEmail = notificationEmail;
                survey.EnforceHttps = enforceHttps;

                _surveyRepository.Value.Update(survey);

                evt.Finish();

                dbTransactionScope.Commit();
            }
        }

        public bool IsSurveyOpen(string confirmitProjectID)
        {
            _surveyService.Value.ValidateProjectId(confirmitProjectID);

            var survey = _surveyRepository.Value.TryGetByName(confirmitProjectID);
            if (survey == null)
            {
                Trace.TraceWarning($"Survey '{confirmitProjectID}' can't be found in IsSurveyOpen method because project ID is wrong or survey has already been removed");
                return false;
            }

            bool isSurveyOpened = ((SurveyState)survey.State == SurveyState.Open);

            //
            // we should think that survey is opened while it has tasks (logged in interviewers)
            if (!isSurveyOpened)
            {
                isSurveyOpened = TaskService.IsSurveyHasTasks(survey.SID);
            }

            return isSurveyOpened;
        }

        /// <summary>
        /// Updates the survey data replication scheme.
        /// </summary>
        /// <param name="projectId">Confirmit project ID.</param>
        /// <param name="tables">Array of <see cref="TableInfo"/> objects with list of columns to replicate data.</param>
        public void UpdateSurveyReplicationScheme(string projectId, TableInfo[] tables)
        {
            // Transaction scopes are used in the inner methods to avoid deadlocks.
            var survey = _surveyRepository.Value.GetByName(projectId);

            var evt = new UpdateSurveyReplicationSchemeViaMsEvent(survey.SID, projectId, tables);

            _surveyService.Value.UpdateReplicationScheme(survey, tables);

            _replicationService.Value.RunForceReplication(survey.SID, CancellationToken.None);

            _surveyPublishService.Value.OnLaunchSurvey(survey.SID);

            evt.Finish();
        }

        /// <summary>
        /// Updates the survey replication status.
        /// </summary>
        /// <param name="projectId">Confirmit project ID.</param>
        /// <param name="isReplicationEnabled">If set to <c>true</c> CATI will replicate data for specified survey.</param>
        [LogExceptionAndNotReThrow]
        public void UpdateSurveyReplicationStatus(string projectId, bool isReplicationEnabled)
        {
            var survey = _surveyRepository.Value.GetByName(projectId);

            using (var dbTransactionScope = new DatabaseTransactionScope("ManSrv.UpdateReplicationStatus"))
            {
                var evt = new UpdateSurveyReplicationStatusViaMsEvent(survey.SID, projectId, isReplicationEnabled);

                _surveyService.Value.UpdateReplicationStatus(survey.SID, isReplicationEnabled);

                evt.Finish();
                dbTransactionScope.Commit();
            }
        }

        public void AddRespondent(string projectId, int respondentId, int its)
        {
            var survey = _surveyRepository.Value.GetByName(projectId);

            var evt = new AddRespondentViaWsEvent(survey.SID, survey.Name, respondentId);

            _interviewService.Value.AddRespondent(survey, respondentId, its, OperationType.AddRecordInWebInterview, Role.WebRespondent);

            evt.Finish();
        }

        public void AddRespondentFromConsole(string projectId, int respondentId, int personId)
        {
            var survey = _surveyRepository.Value.GetByName(projectId);

            var evt = new AddRespondentFromConsoleEvent(survey.SID, survey.Name, respondentId, personId);

            _interviewService.Value.AddRespondent(survey, respondentId, (int)CallOutcome.FreshSample, OperationType.AddRecordFromConsole, Role.Interviewer, personId);

            evt.Finish();
        }
        
        public void AddSample(string projectId, int batchId, int mode, int recordsCount)
        {
            ProcessSample(projectId, batchId, (int)ProcessSampleMode.Add, mode);
        }

        public void ProcessSample(string projectId, int batchId, int processSampleMode, int schedulingMode)
        {
            _surveyService.Value.ValidateProjectId(projectId);

            var survey = _surveyRepository.Value.TryGetByName(projectId);
            var title = $"Process sample for '{projectId}' ({survey?.Description})";
            var param = new Core.AsyncOperations.Operations.SampleUpload.Parameters
            {
                SurveyId = survey?.SID ?? 0,
                ProjectId = projectId,
                BatchId = batchId,
                ProcessSampleMode = (ProcessSampleMode)processSampleMode,
                SchedulingMode = (SchedulingMode)schedulingMode
            };

            string warningText = string.Empty;
            if ((ProcessSampleMode)processSampleMode == ProcessSampleMode.Update)
            {
                if ((SchedulingMode)schedulingMode == SchedulingMode.Simple)
                {
                    warningText =
                        "Error: ProcessSample could not be called in the Update mode with 'Simple Scheduling'. Only Full Scheduling mode is supported.";
                }
                else if (survey != null)
                {
                    var scheduleEntity = _scheduleRepository.Value.GetById(survey.ScheduleID);
                    if (!scheduleEntity.IsSampleUpdateRuleSet)
                    {
                        warningText =
                            $"Error: ProcessSample could not be called in the Update mode for the schedule '{scheduleEntity.Name}'. Schedule '{scheduleEntity.Name}' does not have rule which has to be executed during sample update.";
                    }
                }
            }

            if (!string.IsNullOrEmpty(warningText))
            {
                Trace.TraceWarning(warningText);
                // Need to keep information about try to add sample to be able to return error message to authoring and
                // do not prevent sample adding there
                _sampleService.Value.AddSampleRecord(batchId, param.SurveyId, param.ProcessSampleMode, ProcessSampleAsyncResult.Success);
                return;
            }

            _sampleService.Value.AddSampleRecord(batchId, param.SurveyId, param.ProcessSampleMode, ProcessSampleAsyncResult.InProgress);
            _asyncOperationQueue.Value.Enqueue(
                0,
                title,
                false,
                param,
                AsyncOperationConstants.HighPriority,
                _supervisorNameProvider.Value.Name);
        }

        public int AddSampleGetState(int batchId, out string stateDescription)
        {
            return (int)_sampleService.Value.GetState(batchId, ProcessSampleMode.Add, out stateDescription);
        }

        public int ProcessSampleGetState(int batchId, int sampleMode, out string stateDescription)
        {
            if ((batchId == 0) && (sampleMode == 0))
            {
                // ProcessSampleGetState is called just to determine if CATI supports ProcessSample
                // So we should not log any error and simply return
                stateDescription = null;
                return 0;
            }

            return (int)_sampleService.Value.GetState(batchId, (ProcessSampleMode)sampleMode, out stateDescription);
        }

        public int DeleteRespondentsAsync(int[] respIDs, string confirmitProjectID)
        {
            var survey = _surveyRepository.Value.GetByName(confirmitProjectID);

            var title = string.Format("Delete Respondents from survey '{0}' ({1})", survey.Name, survey.Description);

            var param = new Core.AsyncOperations.Operations.DeleteRespondents.Parameters
            {
                SurveyId = survey.SID,
                ProjectId = confirmitProjectID,
                RespondentIds = respIDs
            };

            var operationEntity = _asyncOperationQueue.Value.Enqueue(
                0,
                title,
                false,
                param,
                AsyncOperationConstants.HighPriority,
                _supervisorNameProvider.Value.Name);

            return operationEntity.Id;
        }

        public void OnQuotaChanged(string cfProjectId, int cfQuotaId)
        {
            var evt = new QuotaChangedViaMsEvent(0, cfProjectId, cfQuotaId);

            var survey = _surveyRepository.Value.GetByName(cfProjectId);

            evt.ObjectId = survey.SID;

            _fcdQuotaService.Value.OnQuotaUpdate(survey.SID, cfQuotaId);

            evt.Finish();
        }

        public void OnQuotaCellsChanged(string cfProjectId, int cfQuotaId, int[] openedCfCellIds, int[] closedCfCellIds, int[] optimisticallyClosedCfCellIds)
        {
            optimisticallyClosedCfCellIds = optimisticallyClosedCfCellIds ?? new int[] { };

            var evt = new QuotaCellsChangedEventViaMsEvent(0, cfProjectId, cfQuotaId, openedCfCellIds, closedCfCellIds, optimisticallyClosedCfCellIds);

            var survey = _surveyRepository.Value.GetByName(cfProjectId);

            evt.ObjectId = survey.SID;

            _fcdQuotaService.Value.OnQuotaCellsChanged(survey.SID, cfQuotaId, openedCfCellIds, closedCfCellIds, optimisticallyClosedCfCellIds);

            evt.Finish();
        }

        private void OnQuotaCellsStateChanged(
            BvSurveyEntity survey, int quotaId, List<CatiQuotaCellCountersState> quotaCellsStates)
        {
            var evt = new QuotaCellsStateChangedEvent(0, survey.ProjectId, quotaId, quotaCellsStates)
            {
                ObjectId = survey.SID
            };
            _fcdQuotaService.Value.OnQuotaCellsStateChanged(survey.SID, quotaId, quotaCellsStates);
            evt.Finish();
        }

        public void OnQuotaCellsStateChanged(string projectId, int quotaId, List<CatiQuotaCellCountersState> quotaCellsStates)
        {
            var survey = _surveyRepository.Value.GetByName(projectId);
            OnQuotaCellsStateChanged(survey, quotaId, quotaCellsStates);
        }

        public void OnQuotasCellsStatesChanged(string projectId, List<CatiQuotaCellsCountersStates> quotasCellsCountersStates)
        {
            var survey = _surveyRepository.Value.GetByName(projectId);
            foreach (var quotaCellsCountersStates in quotasCellsCountersStates)
            {
                OnQuotaCellsStateChanged(survey, quotaCellsCountersStates.QuotaId, quotaCellsCountersStates.CellsCountersStates);
            }
        }
        
        public void OnCATIOptionsChanged(bool telephonyHasBeenEnabled)
        {
            var evt = new CatiOptionsChangedViaMsEvent(
                _companyInfo.Value.CompanyId.ToString(CultureInfo.InvariantCulture),
                telephonyHasBeenEnabled);

            using (var dbLock = DatabaseLockService.CreateLock(
                       DatabaseLockTimeoutsAndRecourceNames.DialerStateOperationLockerResourceName,
                       "ManagementService.OnCATIOptionsChanged",
                       _databaseLockTimeouts.Value.DefaultLockTimeoutInMs,
                       true))
            {
                dbLock.EnterLock();
                
                using (var transactionScope = new DatabaseTransactionScope("ManSrv.OnCATIOptionsChanged"))
                {
                    var doesCompanyUseTelephony = _mnTciTools.Value.DoesCompanyUseTelephony();

                    if (!telephonyHasBeenEnabled && !doesCompanyUseTelephony)
                    {
                        Trace.TraceWarning("Attempt to disable telephony but telephony is already disabled.");
                        return;
                    }

                    foreach (var dialerEntity in _dialersRepository.Value.GetAll())
                    {
                        if (telephonyHasBeenEnabled && dialerEntity.TenantId == 0)
                        {
                            // Company created without telephony support, so we should initialize dialer 
                            // via dialer administration and then enable it.

                            dialerEntity.TenantId = _companyInfo.Value.CompanyId;

                            _dialersRepository.Value.Update(dialerEntity);
                        }

                        if (!telephonyHasBeenEnabled)
                        {
                            _dialerOperationalStateNotificator.Value.SendDialerOperationalStateNotification(dialerEntity.Id, false);
                        }
                    }

                    if (!telephonyHasBeenEnabled)
                    {
                        _telephony.Value.UninitializeDialers(true);

                        BvSpTasks_SetTelephonyProblemForLoggedInAdapter.ExecuteNonQuery(
                            0 /*means for all dialers*/,
                            (int)DialerErrorCode.NotAvailable);

                        _systemSettings.Value.Dialer.DialerType = DiallerType.NoDialler.ToString();
                    }

                    transactionScope.Commit();
                }
            }

            evt.Finish();
        }

        public void SaveInterviewHistoryAndControlData(
            InterviewHistoryData historyData,
            InterviewControlData controlData)
        {
            if (historyData.roleID == (int)Role.Interviewer)
            {
                return;
            }

            // Optimization disabled or CAPI (WTF?)
            _interviewHistoryAndDataProcessor.Value.SaveHistoryAndControlData(false, historyData, controlData,
                new BvInterviewTimings() { InterviewDurationTime = historyData.totalDuration },
                SurveyRepository.GetByName(historyData.projectID), null, null, true, null);
        }

        /// <summary>
        /// Gets cati interviewer name.
        /// Returns null if no cati interviewer with specified id exist.
        /// </summary>
        /// <param name="catiInterviewerId">CATI interviewer ID</param>
        /// <returns>Name of CATI interviewer</returns>
        public string GetCATIInterviewerName(int catiInterviewerId)
        {
            var person = _personRepository.Value.TryGetById(catiInterviewerId);

            if (person == null)
            {
                return null;
            }

            return person.Name;
        }

        public string GetCatiInterviewerDisplayName(int catiInterviewerId)
        {
            var person = _personRepository.Value.TryGetById(catiInterviewerId);

            if (person == null)
            {
                return null;
            }

            return string.IsNullOrEmpty(person.FullName) ? person.Name : person.FullName;
        }
            
        /// <summary>
        /// Gets cati station id.
        /// </summary>
        /// <param name="catiInterviewerId">CATI interviewer ID</param>
        /// <returns>StationId of CATI interviewer</returns>
        public string GetCATIStationId(int catiInterviewerId)
        {
            var task = _taskRepository.Value.GetByPerson(catiInterviewerId);

            if (task == null)
            {
                return null;
            }

            return task.StationId;
        }

        /// <summary>
        /// Gets appointment time for the respondent.
        /// Returns null if no appointment exist for the respondent.
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <param name="respondentId">Respondent ID</param>
        /// <returns>Time in the respondent time zone of the appointment set for this interview in CATI</returns>
        public DateTime? GetCATIAppointmentTime(string projectId, int respondentId)
        {
            var survey = _surveyRepository.Value.GetByName(projectId);

            var appointment = AppointmentRepository.GetNewlyCreatedAppointment(survey.SID, respondentId);

            if (appointment == null)
            {
                return null;
            }

            int tzid = appointment.TZID.GetValueOrDefault();
            if (tzid == 0)
            {
                tzid = _interviewService.Value.GetInterviewTimezoneOrDefault(survey.SID, appointment.InterviewSID);
            }

            return Core.Timezones.TimezoneManager.ConvertToTzLocalTime(tzid, appointment.Time);
        }
        
        /// <summary>
        /// Gets all dialing attempts for last cati interview attempt for the respondent.
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <param name="respondentId">Respondent ID</param>
        /// <returns>Dialing attempts for the respondent</returns>
        public CatiDialingAttempt[] GetCatiInterviewDialingAttempts(string projectId, int respondentId)
        {
            var survey = _surveyRepository.Value.GetByName(projectId);
            
            return _dialingAttemptsService.Value.GetDialingAttemptsForLastInterviewAttempt(survey.SID, respondentId).ToArray();
        }

        /// <summary>
        /// Stops interview recording.
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <param name="respondentId">Respondent ID</param>
        /// <param name="stopRecordingMode">
        /// StopRecordingMode: stop whole interview recording, or sectional or both?
        /// </param>
        [LogExceptionAndNotReThrow]
        public void StopRecording(string projectId, int respondentId, string stopRecordingMode)
        {
            _interviewRecordingManager.Value.StopRecording(projectId, respondentId, stopRecordingMode);
        }

        /// <summary>
        /// Starts open-end or sectional audio recording of the interview.
        /// </summary>
        /// <param name="projectId">The project ID (pXXXXXXX).</param>
        /// <param name="respondentId">The respondent ID (interview ID in CATI).</param>
        /// <param name="label">The label. It will be included in the name of the recorded audio file.</param>
        /// <remarks>It should work both if whole interview recording is enabled or not.
        /// If whole interview recording if in process when this method is called - it will be automatically paused.</remarks>
        [LogExceptionAndNotReThrow]
        public void StartRecording(string projectId, int respondentId, string label)
        {
            _interviewRecordingManager.Value.StartRecording(projectId, respondentId, label);
        }

        [LogExceptionAndNotReThrow]
        public void EnableLiveMonitoring(string projectId, int catiInterviewerId)
        {
            var evt = new EnableLiveMonitoringEvent();
            BvTasksEntity task;

            var survey = _surveyRepository.Value.GetByProjectId(projectId);
            if (survey.IsLiveMonitoringEnabled)
                return;
            
            using (TaskLocker taskLock = TaskLocker.Lock(_personRepository.Value.GetById(catiInterviewerId), out task))
            {
                evt.AddTiming("TaskLocker.TryLock");

                if (taskLock != null)
                {
                    task.Context.IsLiveMonitoringEnabled = true;

                    _taskRepository.Value.Update(task);
                    evt.AddTiming("TaskRepository.Update");
                }
            }

            _monitoringService.Value.SetLiveMonitoringState(catiInterviewerId, true);

            evt.UpdateEventPropertiesFromTask(task);
            evt.Save();
        }

        /// <summary>
        /// Method returns dialing mode of interview. If interview hasn't specific dialing
        /// mode survey dialing mode will be return.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="respondentId"></param>
        /// <returns></returns>
        public int GetDialingMode(string projectId, int respondentId)
        {
            var survey = _surveyRepository.Value.GetByName(projectId);
            var interview = _interviewRepository.Value.GetByIdWithCheck(survey.SID, respondentId);
            return (int)BvCallHandlerRoot.GetDialingMode((DialType)interview.DialTypeId, survey, interview);
        }

        /// <summary>
        /// Returns transient state of interview.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="respondentId"></param>
        /// <returns>Transient state of interview</returns>
        public int GetExtendedStatus(string projectId, int respondentId)
        {
            var survey = _surveyRepository.Value.GetByName(projectId);
            var interview = _interviewRepository.Value.GetById(survey.SID, respondentId);

            if (interview == null)
                throw new InternalErrorException(String.Format(
                    "Interview {0} for survey {1} not found.", respondentId, projectId));

            return interview.TransientState;
        }

        [LogExceptionAndNotReThrow]
        public void TransferToIvr(string projectId, int respondentId, string endpoint, IEnumerable<KeyValuePair<string, string>> attributes)
        {
            new IvrManager().TransferToIvr(projectId, respondentId, endpoint, attributes);
        }

        /// <summary>
        /// Add telephone number to the CATI Blacklist of this company
        /// </summary>
        /// <param name="telephoneNumber">Telephone number (non digits symbols will be omitted)</param>
        /// <param name="projectId">The project ID (pXXXXXXX)</param>
        /// <param name="respondentId">Respondent ID</param>
        [LogExceptionAndNotReThrow]
        public void AddToCATIBlacklist(string telephoneNumber, string projectId, int respondentId)
        {
            var normalizedTelephoneNumber = _telephoneBlacklistService.Value.NormalizeTelephoneNumber(telephoneNumber);
            using (var transactionScope = new DatabaseTransactionScope("ManSrv.AddToCATIBlacklist"))
            {
                var evt = new AddTelephoneNumberToBlacklistViaWsEvent(normalizedTelephoneNumber, projectId, respondentId);
                
                var survey = projectId != null ? _surveyRepository.Value.TryGetByName(projectId) : null;

                var respId = $" [{respondentId}]";
                var comment = $"Added from {projectId} {survey?.Description}";
                comment = comment.Length + respId.Length > 74 ? comment.Substring(0, 74 - respId.Length) : comment;
                comment += respId;
                
                _telephoneBlacklistRepository.Value.Insert(new BvTelephoneBlacklistEntity 
                    { DisplayPattern = normalizedTelephoneNumber, Comment = comment });
                evt.Finish();

                transactionScope.Commit();
            }
        }

        /// <summary>
        /// Method returns of EventDetails objects
        /// containing interviewer activity events.
        /// </summary>
        public IEnumerable<EventDetails> GetInterviewerActivityEventsList()
        {
            var activityEventsLoader = new ActivityEventsLoader();

            return activityEventsLoader.GetInterviewerActivityEvents().Select(activityEventsLoader.GetInterviewerActivityEventDetails);
        }

        /// <summary>
        /// Method returns of EventDetails objects
        /// containing management activity events.
        /// </summary>
        public IEnumerable<EventDetails> GetManagmentActivityEventsList()
        {
            var activityEventsLoader = new ActivityEventsLoader();

            return activityEventsLoader.GetManagementActivityEvents().Select(activityEventsLoader.GetManagementActivityEventDetails);
        }


        /// <summary>
        /// return archive data of specific survey
        /// </summary>
        /// <param name="projectId">The project ID (pXXXXXXX).</param>
        /// <returns>Archive data</returns>
        public string BackupSurveyToArchive(string projectId)
        {
            var survey = _surveyRepository.Value.GetByName(projectId);

            return _surveyArchiveService.Value.Archive(survey);
        }

        public int BeginRestoreSurveyFromArchive(string projectId, string data)
        {
            var survey = _surveyRepository.Value.GetByName(projectId);

            var title = string.Format("Restore Survey '{0}' ({1})", survey.Name, survey.Description);

            var parameters = new Core.AsyncOperations.Operations.RestoreSurvey.Parameters
            {
                SurveyId = survey.SID,
                SurveyName = survey.Name,
                Data = data
            };

            var operationEntity = _asyncOperationQueue.Value.Enqueue(
                0,
                title,
                false,
                parameters,
                AsyncOperationConstants.HighPriority,
                _supervisorNameProvider.Value.Name);

            return operationEntity.Id;
        }

        public AsyncOperationInfo GetAsyncOperationInfo(int operationId)
        {
            var operation = _asyncOperationRepository.Value.Get(operationId);

            return new AsyncOperationInfo
            {
                Type = operation.Type,
                Title = operation.Title,
                State = (AsyncOperationState)operation.State,
                Priority = operation.Priority,
                QueuedDate = operation.QueuedDate,
                StartedDate = operation.StartedDate,
                FinishedDate = operation.FinishedDate,
                TotalItemsCount = operation.TotalItemsCount,
                ProcessedItemsCount = operation.ProcessedItemsCount,
                FailedItemsCount = operation.FailedItemsCount,
                CreatedBySupervisorName = operation.CreatedBySupervisorName,
                Error = operation.Error,
                Text = operation.Text
            };
        }

        public string[] GetSurveyCallCenters(string projectId, string supervisorName)
        {
            var survey = _surveyRepository.Value.TryGetByName(projectId);

            var result = (survey == null)
                ? new[] { _callCenterService.Value.GetSupervisorCallCenter(supervisorName).Name }
                : _callCenterRepository.Value.GetAssignedToSurvey(survey.SID).Select(item => item.Name).ToArray();

            return result;
        }

        public int LaunchSurvey(string projectId, LaunchSurveyParameters parameters)
        {
            _surveyService.Value.ValidateProjectId(projectId);

            var survey = _surveyRepository.Value.TryGetByName(projectId);

            var title = string.Format("Launch Survey '{0}' ({1})", projectId, parameters.SurveyProperties.ProjectName);

            var param = new Core.AsyncOperations.Operations.LaunchSurvey.Parameters
            {
                SurveyId = survey == null ? 0 : survey.SID,
                ProjectId = projectId,
                RemoveData = parameters.RemoveData,
                PermittedUsers = parameters.PermittedUsers,
                ReplicatedTables = parameters.ReplicatedTables,
                SurveyProperties = new Core.AsyncOperations.Operations.LaunchSurvey.SurveyProperties
                {
                    ProjectName = parameters.SurveyProperties.ProjectName,
                    CfSqlServerConnectionString = parameters.SurveyProperties.CfSqlServerConnectionString,
                    CreatedUserName = parameters.SurveyProperties.CreatedUserName,
                    DialingMode = parameters.SurveyProperties.DialingMode,
                    EnforceHttps = parameters.SurveyProperties.EnforceHttps,
                    NotificationEmail = parameters.SurveyProperties.NotificationEmail,
                    OpenEndReview = parameters.SurveyProperties.OpenEndReview,
                    ScreenRecording = parameters.SurveyProperties.ScreenRecording,
                    SupportBlacklist = parameters.SurveyProperties.SupportBlacklist,
                    AllowRespondentsDynamicCreation = parameters.SurveyProperties.AllowRespondentsDynamicCreation,
                    VoiceRecording = parameters.SurveyProperties.VoiceRecording,
                    ReplicationStatus = parameters.SurveyProperties.ReplicationStatus,
                    LiveMonitoring = parameters.SurveyProperties.LiveMonitoring ?? true
                }
            };

            var operationEntity = _asyncOperationQueue.Value.Enqueue(
            0,
            title,
            false,
            param,
            AsyncOperationConstants.HighPriority,
                parameters.SurveyProperties.CreatedUserName);

            return operationEntity.Id;
        }

        public string GetVersion()
        {
            return Assembly.GetCallingAssembly().GetName().Version.ToString();
        }

        public async Task UpdateActiveQuestion(string projectId, int catiInterviewerId, string qId)
        {
            try
            {
                var evt = new UpdateActiveQuestionEvent(projectId, catiInterviewerId, qId);

                await _taskRepository.Value.UpdateActiveQuestion(projectId, catiInterviewerId, qId, DateTime.UtcNow);

                if (evt.Duration >= TimeSpan.FromSeconds(1))
                {
                    evt.Save();
                }
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(
                    ex,
                    $"UpdateActiveQuestion projectId={projectId}, interviewerId={catiInterviewerId}, questionId={qId}");
            }
        }

        public void ScheduleInterview(SchedulingScriptExecutionParameters parameters)
        {
            const string formatString = "dd/MM/yyyy HH:mm:ss";
            var interview = _interviewRepository.Value.GetById(parameters.SurveySid, parameters.InterviewId);
            if (interview.LastCallTime?.ToString(formatString) == parameters.TimeCallDelivered?.ToString(formatString))
            {
                return;
            }

            var executionReason = SchedulingScriptExecutionReason.Unspecified;
            switch (parameters.OperationType)
            {
                case OperationType.Interview:
                    executionReason = SchedulingScriptExecutionReason.Processed;
                    break;
                case OperationType.NotConnectedCall:
                    executionReason = SchedulingScriptExecutionReason.NotConnected;
                    break;
                case OperationType.TelephonyError:
                    executionReason = SchedulingScriptExecutionReason.TelephonyError;
                    break;
            }

            var options = new SchedulingScriptExecutionOptions {
                ITS = parameters.ITS,
                ExecutionReason = executionReason,
                opType = parameters.OperationType,
                LastCallTime = parameters.TimeCallDelivered,
                LastCallPersonSID = parameters.LastCallPersonId,
                CliNumber = parameters.CliNumber,
                DdiNumber = parameters.DdiNumber,
                Timings = new BvInterviewTimings() {
                    CallCenterID = parameters.CallCenterId,
                    InterviewDurationTime = parameters.InterviewDurationTime,
                    OpenEndReviewDurationTime = parameters.OpenEndReviewDurationTime,
                    TimeCallDelivered = parameters.TimeCallDelivered,
                    WaitingTime = parameters.WaitingTime,
                    PreviewTime = parameters.PreviewTime,
                    WrapTime = parameters.WrapTime,
                    ConnectedTime = parameters.ConnectedTime
                },
                ConfirmitDuration = parameters.ConfirmitDuration,
                LinkedInterviewSessionId = parameters.LinkedInterviewSessionId,
                CallCenterID = parameters.CallCenterId,
                IsLogToHistory = parameters.IsLogToHistory ?? true,
                DialingAttempts = parameters.DialingAttempts,
                CallAttemptNumber = parameters.CallAttemptNumber
            };

            _interviewRepository.Value.Update(interview, options);
        }

        public bool IsTimeInShift(string projectId, int timezoneId, DateTime dateTime)
        {
            var result = AreTimesInShift(projectId, timezoneId, new[] { dateTime });

            return result[0].IsInShift;
        }

        public TimeInShift[] AreTimesInShift(string projectId, int timezoneId, DateTime[] dateTimes)
        {
            var survey = _surveyRepository.Value.GetByProjectId(projectId);

            var shiftService = _shiftServiceFactory.Value.Get(survey.ScheduleID);

            var result = new List<TimeInShift>();
            foreach (var dateTime in dateTimes)
            {
                // check whether giving date anf time is inside assigned schedule shifts                    
                var shift = shiftService.GetExactShift(dateTime, timezoneId); // should be UTC time
                result.Add(new TimeInShift { Time = dateTime, IsInShift = shift != null });
            }

            return result.ToArray();
        }

        public bool IsCatiGroupMember(int catiInterviewerId, string groupName)
        {
            var evt = new IsCatiGroupMemberEvent(catiInterviewerId, groupName);
            
            var isCatiGroupMember = _personGroupService.Value.IsGroupContainsInterviewer(catiInterviewerId, groupName);
            
            evt.Save();

            return isCatiGroupMember;
        }

        public void SaveAlternativeNumber(int surveyId, int interviewId, string newPhoneNumber)
        {
            _redialNumberSaver.Value.SaveAlternativeNumber(surveyId, newPhoneNumber, interviewId);
        }

        public string GetInterviewVariableValue(string projectId, int interviewId, string variableName)
        {
            return _interviewResponseDataService.Value.GetInterviewVariableValue(projectId, interviewId, variableName);
        }

        public int Telephony_Login(int dialerId, long campaignId, string agentId, string agentName,
            AgentType agentType, string agentExtension, string userId, bool isPredictive, bool isLocal,
            IEnumerable<KeyValuePair<string, string>> agentAttributes)
        {
            return (int)_telephony.Value.Login(dialerId, campaignId, agentId, agentName, agentType, agentExtension, userId,
                isPredictive, isLocal, agentAttributes);
        }

        public int Telephony_SetGroups(int dialerId, long campaignId, string agentId, int[] groups)
        {
            return (int)_telephony.Value.SetGroups(dialerId, campaignId, agentId, groups);
        }

        public int Telephony_Logout(int dialerId, long campaignId, bool isPredictive, string agentId)
        {
            return (int)_telephony.Value.Logout(dialerId, campaignId, isPredictive, agentId);
        }

        public int Telephony_KillAgent(int dialerId, long campaignId, string agentId)
        {
            return (int)_telephony.Value.KillAgent(dialerId, campaignId, agentId);
        }

        public int Telephony_SetCampaign(int dialerId, long campaignId, int agentId)
        {
            try
            {
                return (int)_telephony.Value.SetCampaign(dialerId, campaignId, agentId);
            }
            catch (DialerException e)
            {
                return (int)e.ErrorCode;
            }
        }

        public int Telephony_GoReady(int dialerId, long campaignId, string agentId)
        {
            return (int)_telephony.Value.GoReady(dialerId, campaignId, agentId);
        }

        public int Telephony_GoNotReady(int dialerId, long campaignId, string agentId, string breakName)
        {
            return (int)_telephony.Value.GoNotReady(dialerId, campaignId, agentId, breakName);
        }

        public int Telephony_SendNumberToAgent(int dialerId, long campaignId, string agentId,
            DialingMode dialingMode, int contactId, int callId, string phoneNumber, bool isRecording, string callerId, Dictionary<string, object> respondentVariables)
        {
            return (int)_telephony.Value.SendNumberToAgent(dialerId, campaignId, agentId, dialingMode, contactId, callId,
                phoneNumber, isRecording, callerId, respondentVariables);
        }

        public int Telephony_SendNumberToAgentEx(int dialerId, long campaignId, string agentId,
            DialingMode dialingMode, int contactId, int callId, string phoneNumber, int callAgingTimeout,
            bool isRecording)
        {
            return (int)_telephony.Value.SendNumberToAgentEx(dialerId, campaignId, agentId, dialingMode, contactId, callId,
                phoneNumber, callAgingTimeout, isRecording);
        }

        public int Telephony_Redial(int dialerId, long campaignId, string agentId, int contactId,
            int callId, string phoneNumber, bool isRecording, string callerId)
        {
            return (int)_telephony.Value.Redial(dialerId, campaignId, agentId, contactId, callId, phoneNumber, isRecording,
                callerId);
        }

        public int Telephony_Hangup(int dialerId, long campaignId, string agentId, int interviewId, long callId)
        {
            return (int)_telephony.Value.Hangup(dialerId, campaignId, agentId, interviewId, callId);
        }

        public int Telephony_CompleteCall(int dialerId, long campaignId, string agentId,
            bool makeAgentReady, string breakName, InterviewStatus status, int interviewId, long callId)
        {
            return (int)_telephony.Value.CompleteCall(dialerId, campaignId, agentId, interviewId,
                makeAgentReady, breakName, status, callId);
        }

        public int Telephony_SetNextInterview(int dialerId, long campaignId, string agentId,
            InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId)
        {
            return (int)_telephony.Value.SetNextInterview(dialerId, campaignId, agentId, currentInterviewStatus,
                nextCampaignId, nextInterviewId, nextCallId);
        }

        public int Telephony_StopMonitor(int dialerId, string sessionId)
        {
            var agentId = "0";
            var contactId = -1;
            try
            {
                var audioMonitoringEntity = AudioMonitoringAdapter
                    .GetByCondition("[SessionID] = @SessionId", new SqlParameter("@SessionId", sessionId))
                    .FirstOrDefault();
                if (audioMonitoringEntity != null)
                    agentId = audioMonitoringEntity.InterviewerSID.ToString(CultureInfo.InvariantCulture);

                contactId = GetActiveInterviewId(agentId);
            }
            catch (Exception) { /*ignored*/ }

            return (int)_telephony.Value.StopMonitor(dialerId, agentId, contactId, sessionId);
        }

        public int Telephony_CompletePreview(int dialerId, long campaignId, string agentId, int contactId,
            int callId, string phoneNumber, bool isRecording)
        {
            return (int)_telephony.Value.CompletePreview(dialerId, campaignId, agentId, contactId, callId, phoneNumber,
                isRecording);
        }

        public int Telephony_ConnectInboundCallToAgent(int dialerId, long campaignId, string inboundCallId,
            CallInfo callInfo, AudioMessageDescriptor audioMessageDescriptor)
        {
            var agentId = callInfo.agentId;

            return (int)_telephony.Value.ConnectInboundCallToAgent(dialerId, campaignId, agentId,
                GetActiveInterviewId(agentId), inboundCallId, callInfo, audioMessageDescriptor);
        }

        public int Telephony_DropInboundCall(int dialerId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor)
        {
            _telephony.Value.DropInboundCall(dialerId, inboundCallId, audioMessageDescriptor);
            return (int)_telephony.Value.DropInboundCall(dialerId, inboundCallId, audioMessageDescriptor);
        }

        public int Telephony_TransferStart(int dialerId, long campaignId, string transferId, int agentId,
            TransferType transferType)
        {
            return (int)_telephony.Value.TransferStart(dialerId, campaignId, transferId, agentId,
                GetActiveInterviewId(agentId), transferType);
        }

        public int Telephony_TransferSetTarget(int dialerId, long campaignId, string transferId,
            TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns)
        {
            var interviewRef = GetInterviewRef(transferId);
            var agentId = interviewRef.InterviewerId;
            var contactId = interviewRef.InterviewId;

            return (int)_telephony.Value.TransferSetTarget(dialerId, campaignId, transferId, agentId, contactId, targetType,
                targetResource, borrowAgentsFromAllCampaigns);
        }

        public int Telephony_TransferSetConnectionState(int dialerId, long campaignId, string transferId,
            ConnectionState state)
        {
            var interviewRef = GetInterviewRef(transferId);
            var agentId = interviewRef.InterviewerId;
            var contactId = interviewRef.InterviewId;

            return (int)_telephony.Value.TransferSetConnectionState(dialerId, campaignId, transferId, agentId, contactId,
                state);
        }

        public int Telephony_TransferComplete(int dialerId, long campaignId, string transferId)
        {
            var interviewRef = GetInterviewRef(transferId);
            var agentId = interviewRef.InterviewerId;
            var contactId = interviewRef.InterviewId;

            return (int)_telephony.Value.TransferComplete(dialerId, campaignId, transferId, agentId, contactId);
        }

        public int Telephony_TransferCancel(int dialerId, long campaignId, string transferId)
        {
            var interviewRef = GetInterviewRef(transferId);
            var agentId = interviewRef.InterviewerId;
            var contactId = interviewRef.InterviewId;

            return (int)_telephony.Value.TransferCancel(dialerId, campaignId, transferId, agentId, contactId);
        }

        public int Telephony_StartPlayback(int dialerId, long campaignId, string agentId, int interviewId,
            int callId, string fileName, out int timeOfPlayingInSeconds)
        {
            return (int)_telephony.Value.StartPlayback(dialerId, campaignId, agentId, interviewId, callId, fileName,
               out timeOfPlayingInSeconds);
        }

        public int Telephony_StopPlayback(int dialerId, long campaignId, string agentId, int callId)
        {
            return (int)_telephony.Value.StopPlayback(dialerId, campaignId, agentId, GetActiveInterviewId(agentId), callId);
        }

        public int Telephony_PauseOrResumePlayback(int dialerId, long campaignId, string agentId, int callId)
        {
            return (int)_telephony.Value.PauseOrResumePlayback(dialerId, campaignId, agentId, GetActiveInterviewId(agentId),
                callId);
        }

        public int Telephony_ToggleInterviewerListensToPlaybackOrRespondent(int dialerId, long campaignId, string agentId, int callId)
        {
            return (int)_telephony.Value.ToggleInterviewerListensToPlaybackOrRespondent(dialerId, campaignId, agentId,
                GetActiveInterviewId(agentId), callId);
        }

        public bool Telephony_IsPersonModeSupported(int dialerId, AgentTaskChoiceMode mode)
        {
            return _telephony.Value.IsPersonModeSupported(mode, dialerId);
        }

        public bool Telephony_IsReloginNeededOnSurveyChange(int dialerId)
        {
            return _telephony.Value.IsReloginNeededOnSurveyChange(dialerId);
        }

        public bool Telephony_IsPauseOrResumePlaybackSupported(int dialerId)
        {
            return _telephony.Value.IsPauseOrResumePlaybackSupported(dialerId);
        }

        public bool Telephony_IsToggleInterviewerListensToPlaybackOrRespondentSupported(int dialerId)
        {
            return _telephony.Value.IsToggleInterviewerListensToPlaybackOrRespondentSupported(dialerId);
        }

        public bool Telephony_IsDynamicExtensionNumberAllowed(int dialerId, bool isAgentLocal)
        {
            return _telephony.Value.IsDynamicExtensionNumberAllowed(isAgentLocal, dialerId);
        }

        private int GetActiveInterviewId(string agentId) =>
            int.TryParse(agentId, out var personId) ? GetActiveInterviewId(personId) : -1;

        public int Telephony_RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl)
        {
            return (int)_telephony.Value.RegisterAgentSoftphone(companyId, dialerId, agentId, agentName, out login, out password, out host, out extension, out frontendUrl);
        }

        public int Telephony_IvrRenderVoiceXml(int companyId, int dialerId, long campaignId, int agentId, int contactId,
            string voiceXml)
        {
            return (int)_telephony.Value.IvrRenderVoiceXml(dialerId, companyId, campaignId, agentId, contactId, voiceXml);
        }
        
        public int Telephony_StartCustomIvrInterview(int dialerId, long campaignId, string agentId, int interviewId, long callId, string respondentSurveyLink)
        {
            return (int)_telephony.Value.StartCustomIvrInterview(dialerId, campaignId, agentId, interviewId, callId, respondentSurveyLink);
        }

        private int GetActiveInterviewId(int interviewerSid)
        {
            var contactId = -1;
            try
            {
                var task = _taskRepository.Value.GetByPerson(interviewerSid);
                if (task != null)
                    contactId = task.InterviewID;
            }
            catch (Exception) { /*ignored*/ }

            return contactId;
        }

        private class InterviewRef
        {
            public int InterviewId { get; set; }
            public int InterviewerId { get; set; }
        }

        private InterviewRef GetInterviewRef(string transferId)
        {
            var result = new InterviewRef
            {
                InterviewId = -1,
                InterviewerId = -1
            };

            try
            {
                var dial = _activeDialRepository.Value.TryGetByTransferId(transferId);
                if (dial != null)
                {
                    result.InterviewerId = dial.MainPersonId;
                    result.InterviewId = dial.InterviewId;
                }
            }
            catch (Exception) { /*ignored*/ }

            return result;
        }
    }
}
