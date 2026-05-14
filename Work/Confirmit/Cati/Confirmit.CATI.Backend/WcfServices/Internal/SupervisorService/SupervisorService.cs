using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;

using Confirmit.CATI.Backend.WcfServices.Tools.IPFilter;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.WcfTools.ErrorContextHandler;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.PersonImport;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.DAL.Framework;
using BvCallHandlerLibrary;
using Confirmit.CATI.Backend.WcfServices.External.MonitoringService.Diallers;
using Confirmit.CATI.Backend.WcfServices.Tools;
using Confirmit.CATI.Backend.WcfServices.Tools.Logging;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Mail;
using Confirmit.CATI.Core.Mail.Feedback;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Monitoring.Common.Contracts;
using ConfirmitDialerInterface;
using DialerCommon;
using DialerCommon.DialerParameters;
using Confirmit.CATI.Supervisor.Core.Messaging;

namespace Confirmit.CATI.Backend.WcfServices.Internal.SupervisorService
{
    [IpFilterBehavior]
    [ErrorContextHandler(WebServiceType.Internal)]
    [HeadersHandler]
    [MetricsBehaviour(TrackMethodsSeparately = false)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    [ServiceKnownType(typeof(CompilerError))]
    public class SupervisorService : ISupervisorService
    {
        private readonly Lazy<ISurveyStateService> _surveyStateService;
        private readonly Lazy<IInterviewRecordingManager> _interviewRecordingManager;
        private readonly Lazy<IDialerAvailabilityManager> _dialerAvailabilityManager;
        private readonly Lazy<IDialerFacilities> _dialerFacilities;
        private readonly Lazy<IDialerCollection> _dialerCollection;
        private readonly Lazy<IScheduleService> _scheduleService;
        private readonly Lazy<ICallQueueService> _callQueueService;
        private readonly Lazy<IAudioMonitoring> _audioMonitoring;
        private readonly Lazy<IDialerSurveyParametersManager> _dialerSurveyParametersManager;
        private readonly Lazy<IPersonService> _personService;
        private readonly Lazy<IPersonRepository> _personRepository;
        private readonly Lazy<IInboundTelephoneNumberRepository> _inboundTelephoneNumberRepository;
        private readonly Lazy<IPersonImportService> _personImportService;
        private readonly Lazy<IDialerCampaignInitializer> _dialerCampaignInitializer;
        private readonly Lazy<ITelephony> _telephony;
        private readonly Lazy<IInboundAudioMessages> _inboundAudioMessages;
        private readonly Lazy<IToggleSettings> _toggleSettings;
        private readonly Lazy<IDialerSettingsGroup> _dialerSettings;
        private readonly Lazy<IInterviewerPasswordSettingsGroup> _passwordSettings;
        private readonly Lazy<IAudioRecordsManager> _audioRecordsManager;
        private readonly Lazy<ISurveyRepository> _surveyRepository;
        private readonly Lazy<ISqlTableUpdatedPublisher> _sqlTableUpdatedPublisher;

        public SupervisorService()
        {
            _surveyStateService = new Lazy<ISurveyStateService>(() => ServiceLocator.Resolve<ISurveyStateService>());
            _interviewRecordingManager = new Lazy<IInterviewRecordingManager>(() => ServiceLocator.Resolve<IInterviewRecordingManager>());
            _dialerAvailabilityManager = new Lazy<IDialerAvailabilityManager>(() => ServiceLocator.Resolve<IDialerAvailabilityManager>());
            _dialerFacilities = new Lazy<IDialerFacilities>(() => ServiceLocator.Resolve<IDialerFacilities>());
            _dialerCollection = new Lazy<IDialerCollection>(() => ServiceLocator.Resolve<IDialerCollection>());
            _scheduleService = new Lazy<IScheduleService>(() => ServiceLocator.Resolve<IScheduleService>());
            _callQueueService = new Lazy<ICallQueueService>(() => ServiceLocator.Resolve<ICallQueueService>());
            _audioMonitoring = new Lazy<IAudioMonitoring>(() => ServiceLocator.Resolve<IAudioMonitoring>());
            _dialerSurveyParametersManager = new Lazy<IDialerSurveyParametersManager>(() => ServiceLocator.Resolve<IDialerSurveyParametersManager>());
            _personService = new Lazy<IPersonService>(() => ServiceLocator.Resolve<IPersonService>());
            _personRepository = new Lazy<IPersonRepository>(() => ServiceLocator.Resolve<IPersonRepository>());
            _inboundTelephoneNumberRepository = new Lazy<IInboundTelephoneNumberRepository>(() => ServiceLocator.Resolve<IInboundTelephoneNumberRepository>());
            _personImportService = new Lazy<IPersonImportService>(() => ServiceLocator.Resolve<IPersonImportService>());
            _dialerCampaignInitializer = new Lazy<IDialerCampaignInitializer>(() => ServiceLocator.Resolve<IDialerCampaignInitializer>());
            _telephony = new Lazy<ITelephony>(() => ServiceLocator.Resolve<ITelephony>());
            _inboundAudioMessages = new Lazy<IInboundAudioMessages>(() => ServiceLocator.Resolve<IInboundAudioMessages>());
            _toggleSettings = new Lazy<IToggleSettings>(() => ServiceLocator.Resolve<IToggleSettings>());
            _dialerSettings = new Lazy<IDialerSettingsGroup>(() => ServiceLocator.Resolve<IDialerSettingsGroup>());
            _passwordSettings = new Lazy<IInterviewerPasswordSettingsGroup>(() => ServiceLocator.Resolve<IInterviewerPasswordSettingsGroup>());
            _audioRecordsManager = new Lazy<IAudioRecordsManager>(() => ServiceLocator.Resolve<IAudioRecordsManager>());
            _surveyRepository = new Lazy<ISurveyRepository>(() => ServiceLocator.Resolve<ISurveyRepository>());
            _sqlTableUpdatedPublisher = new Lazy<ISqlTableUpdatedPublisher>(() => ServiceLocator.Resolve<ISqlTableUpdatedPublisher>());
        }

        /// <summary>
        /// Ensures that operation is executed in backend instance service.
        /// </summary>
        /// <exception cref="InvalidOperationException">Supervisor service could be executed only in the context of backend service.</exception>
        private static void EnsureIsExecutedInBackendInstance()
        {
            if (!BackendInstance.Current.IsExecutedInBackendInstance)
            {
                throw new InvalidOperationException("Supervisor service could be executed only in the context of backend service.");
            }
        }

        public void SendMessage(FeedbackForm feedback)
        {
            EnsureIsExecutedInBackendInstance();

            var feedbackMailProvider = ServiceLocator.Resolve<IFeedbackMessageCreator>();
            var mailMessage = feedbackMailProvider.GetMailMessage(feedback);
            var mailSender = ServiceLocator.Resolve<IMailSender>();
            mailSender.SendMail(mailMessage);
        }

        public void OpenSurvey(int surveySid)
        {
            EnsureIsExecutedInBackendInstance();

            _surveyStateService.Value.Open(surveySid);
        }

        public void CloseSurvey(int surveySid)
        {
            EnsureIsExecutedInBackendInstance();

            _surveyStateService.Value.CloseSurvey(surveySid);
        }

        public void ShutdownSurvey(int surveySid)
        {
            EnsureIsExecutedInBackendInstance();

            _surveyStateService.Value.ShutdownSurvey(surveySid);
        }

        public BvTasksEntity TerminateTaskByPerson(int personSid, CallOutcome? explicitIts)
        {
            EnsureIsExecutedInBackendInstance();

            if (explicitIts == null)
            {
                explicitIts = CallOutcome.InterruptedBySystem;
            }

            var task = TaskService.TerminateTask(
                personSid,
                new DatabaseTransactionOptions("SupSrv.TerminateTask", DeadlockPriority.Supervisor),
                explicitIts);

            return task;
        }

        public void TerminateTasksByDialerId(int dialerId)
        {
            EnsureIsExecutedInBackendInstance();

            TaskService.TerminateTasksAsync(
                dialerId,
                new DatabaseTransactionOptions("SupSrv.TerminateTasksByDialerId", DeadlockPriority.Supervisor));
        }

        public bool EnableDialer(int dialerId)
        {
            EnsureIsExecutedInBackendInstance();

            try
            {
                _dialerAvailabilityManager.Value.EnableDialer(dialerId);
            }
            finally
            {
                _dialerCampaignInitializer.Value.InitializeAllCampaigns();
            }

            return true;
        }

        public bool DisableDialer(int dialerId)
        {
            EnsureIsExecutedInBackendInstance();

            return _dialerAvailabilityManager.Value.DisableDialer(dialerId);
        }

        public void SetPersonParentGroups(int personSid, int[] parentGroupsSids)
        {
            EnsureIsExecutedInBackendInstance();

            using (var transaction = new DatabaseTransactionScope("SupSrv.SetPersonParentGroups", DeadlockPriority.Supervisor))
            {
                _personService.Value.SetParentGroups(personSid, parentGroupsSids);

                transaction.Commit();
            }
        }

        public void DeletePerson(int personSid)
        {
            EnsureIsExecutedInBackendInstance();

            using (var transaction = new DatabaseTransactionScope("SupSrv.DeletePerson", DeadlockPriority.Supervisor))
            {
                _personRepository.Value.Delete(personSid);

                transaction.Commit();
            }
        }

        public void DeletePersons(List<int> personSids)
        {
            EnsureIsExecutedInBackendInstance();

            try
            {
                foreach (var personSid in personSids)
                {
                    using (var transaction =
                           new DatabaseTransactionScope("SupSrv.DeletePerson", DeadlockPriority.Supervisor))
                    {
                        _personRepository.Value.Delete(personSid, false);

                        transaction.Commit();
                    }
                }
            }
            finally
            {
                PersonRepository.RefreshCache();
            }
        }

        public void LockPersonBySupervisor(int personId)
        {
            EnsureIsExecutedInBackendInstance();

            _personService.Value.LockPersonBySupervisor(personId);
        }
        
        public void LockPersonsBySupervisor(List<int> personIds)
        {
            EnsureIsExecutedInBackendInstance();

            _personService.Value.LockPersonsBySupervisor(personIds);
        }
        
        public bool IsDialerOperational(int dialerId)
        {
            EnsureIsExecutedInBackendInstance();

            var result = _dialerCollection.Value.IsDialerInitialized(dialerId);

            return result;
        }

        public void SaveSchedule(int scheduleSid, string serializedSchedule)
        {
            EnsureIsExecutedInBackendInstance();

            using (var transaction = new DatabaseTransactionScope("UpdateSchedulingScript", DeadlockPriority.Supervisor))
            {
                _scheduleService.Value.Save(scheduleSid, serializedSchedule);

                transaction.Commit();
            }
        }

        public void LaunchSchedule(int scheduleSid)
        {
            EnsureIsExecutedInBackendInstance();

            using (var transaction = new DatabaseTransactionScope("SupSrv.LaunchSchedule", DeadlockPriority.Supervisor))
            {
                _scheduleService.Value.Launch(scheduleSid);

                transaction.Commit();
            }
        }

        public void CheckSchedule(string serializedSchedule)
        {
            EnsureIsExecutedInBackendInstance();

            using (var transaction = new DatabaseTransactionScope("SupSrv.CheckSchedule", DeadlockPriority.Supervisor))
            {
                _scheduleService.Value.Check(serializedSchedule);

                transaction.Commit();
            }
        }

        public bool Schedule()
        {
            EnsureIsExecutedInBackendInstance();

            _callQueueService.Value.Schedule();
            return true;
        }

        public void ForceCallDelivery()
        {
            EnsureIsExecutedInBackendInstance();

            _callQueueService.Value.ForceCallDelivery();
        }


        public void StartMonitor(string supervisorName, int interviewerId, string telephoneNumber)
        {
            EnsureIsExecutedInBackendInstance();

            _audioMonitoring.Value.StartAudioMonitor(supervisorName, interviewerId, telephoneNumber);
        }

        public void StopMonitor(string supervisorName, int interviewerId)
        {
            EnsureIsExecutedInBackendInstance();

            _audioMonitoring.Value.StopAudioMonitor(supervisorName, interviewerId);
        }

        public int CreateOrUpdatePerson(
            int callCenterId,
            int personSid,
            string name,
            string description,
            string fullName,
            string password,
            AgentTaskChoiceMode mode,
            PersonAssignmentListMode assignmentListMode,
            TaskChoicePermissions? permissions,
            List<int> parentGroups,
            int? autoSurveyId,
            int callGroupId,
            string location,
            DialType dialType,
            AgentType agentType,
            bool enableSoftphoneIntegration,
            string[] attributes = null)
        {
            EnsureIsExecutedInBackendInstance();
            int personId;
            using (var transaction = new DatabaseTransactionScope("SupSrv.CreateOrUpdatePerson", DeadlockPriority.Supervisor))
            {
                var passwordNeedsChange = _passwordSettings.Value.IsChangeAfterFirstLoginRequired;
                personId = _personService.Value.CreateOrUpdatePerson(
                    callCenterId,
                    personSid,
                    name,
                    description,
                    fullName,
                    password,
                    mode,
                    assignmentListMode,
                    permissions,
                    parentGroups,
                    autoSurveyId,
                    callGroupId,
                    location,
                    dialType,
                    agentType,
                    enableSoftphoneIntegration,
                    passwordNeedsChange,
                    attributes);

                transaction.Commit();
            }

            return personId;
        }

        public IEnumerable<AudioRecordInfo> GetInterviewRecordings(int surveyId, int interviewId)
        {
            EnsureIsExecutedInBackendInstance();

            return _interviewRecordingManager.Value.GetInterviewRecordings(surveyId, interviewId);
        }

        public bool[] AreRecordsExists(int surveySid, int[] interviewIds)
        {
            EnsureIsExecutedInBackendInstance();

            return _interviewRecordingManager.Value.AreRecordsExists(surveySid, interviewIds);
        }

        /// <summary>
        /// Sets dialer default survey parameters.
        /// </summary>
        /// <param name="parameters"></param>
        public void SetDialerDefaultSurveyParameters(IEnumerable<DialerParameter> parameters)
        {
            EnsureIsExecutedInBackendInstance();

            using (var transaction = new DatabaseTransactionScope("SupSrv.SetDialerDefParams", DeadlockPriority.Supervisor))
            {
                _dialerSurveyParametersManager.Value.SetDialerDefaultSurveyParameters(parameters);
                transaction.Commit();
            }
        }

        public void ValidateDialerSurveyParameters(IEnumerable<DialerParameter> parameters)
        {
            EnsureIsExecutedInBackendInstance();

            _dialerSurveyParametersManager.Value.ValidateDialerSurveyParameters(parameters);
        }

        /// <summary>
        /// Sets dialer parameters for the specified survey.
        /// </summary>
        /// <param name="surveySid"></param>
        /// <param name="parameters"></param>
        public void SetDialerSurveyParameters(int surveySid, IEnumerable<DialerParameter> parameters)
        {
            EnsureIsExecutedInBackendInstance();

            _dialerSurveyParametersManager.Value.SetDialerSurveyParameters(surveySid, parameters);
        }

        public ImportResult ImportPersons(int callCenterId, DataTable dataTable, Dictionary<string, ColumnRole> columnRoleMap, ImportOptions importOptions)
        {
            EnsureIsExecutedInBackendInstance();

            return _personImportService.Value.ImportPersons(callCenterId, dataTable, columnRoleMap, importOptions);
        }

        public void ConfigureInboundDdiNumbers(int dialerId)
        {
            EnsureIsExecutedInBackendInstance();

            if (!_toggleSettings.Value.EnableInbound)
                throw new UserMessageException("Inbound feature is disabled. Configuring DDI numbers is forbidden");

            _dialerSettings.Value.OnChanged();
            BvInboundTelephoneNumberCache.Instance.OnTableChanged();
            _sqlTableUpdatedPublisher.Value.PublishInboundTelephoneNumberUpdated();
            
            var inboundDdiNumbers = _inboundTelephoneNumberRepository.Value.GetValidByDialerId(dialerId);

            var results = _telephony.Value.ConfigureInboundDdiNumbers(
                dialerId,
                inboundDdiNumbers.Select(x => new InboundDdiNumber
                {
                    Number = x.TelephoneNumber,
                    AudioMessages = _inboundAudioMessages.Value.DdiNumbersMessages(x)
                }).ToArray());

            if (results.Any(x => x != DialerErrorCode.Success))
            {
                throw new UserMessageException("Configuring DDI numbers for dialerId {" + dialerId + "} failed");
            }
        }

        public IEnumerable<LogFileInfo> GetLogFiles(int dialerId)
        {
            EnsureIsExecutedInBackendInstance();

            return _dialerFacilities.Value.GetLogFiles(dialerId);
        }

        public byte[] GetLogFileBodyZipped(int dialerId, string fileName)
        {
            EnsureIsExecutedInBackendInstance();

            return _dialerFacilities.Value.GetLogFileBodyZipped(dialerId, fileName);
        }

        public string GetDialerVersion(int dialerId)
        {
            EnsureIsExecutedInBackendInstance();

            return _dialerFacilities.Value.GetDialerVersion(dialerId);
        }

        public DialerAvailableExtendedFunctionality GetAvailableExtendedFunctionality(int dialerId)
        {
            EnsureIsExecutedInBackendInstance();

            return _dialerFacilities.Value.GetAvailableExtendedFunctionality(dialerId);
        }

        public DialerFeatures GetDialerSupportedFeatures(int dialerId)
        {
            EnsureIsExecutedInBackendInstance();

            return _dialerFacilities.Value.GetDialerSupportedFeatures(dialerId);
        }

        public AudioFile GetAudioFile(int dialerId, string audioUrl)
        {
            EnsureIsExecutedInBackendInstance();

            return _interviewRecordingManager.Value.GetAudioFile(dialerId, audioUrl);
        }
        
        public IEnumerable<DialerOverridenFeature> GetOverridenDialerSupportedFeatures(int dialerId)
        {
            EnsureIsExecutedInBackendInstance();

            return _dialerFacilities.Value.GetOverridenDialerSupportedFeatures(dialerId);
        }

        public void UpdateOverridenDialerSupportedFeature(int dialerId, string featureName, bool? overridenFeatureValue)
        {
            EnsureIsExecutedInBackendInstance();

            _dialerFacilities.Value.UpdateOverridenDialerSupportedFeature(dialerId, featureName, overridenFeatureValue);
        }

        public void SendMessageToInterviewers(IEnumerable<int> interviewerIds, bool onlineOnly, string message, string supervisorName)
        {
            EnsureIsExecutedInBackendInstance();
            SendMessageManager.SendMessageToInterviewers(interviewerIds, onlineOnly, new Message() { Body = message, SupervisorName = supervisorName });
        }

        public void SetLiveMonitoringMode(string supervisorName, int interviewerId, MonitorMode mode)
        {
            EnsureIsExecutedInBackendInstance();

            _audioMonitoring.Value.SetMonitorMode(supervisorName, interviewerId, mode);
        }

        public IEnumerable<AudioIdentityObject> GetAudioIdentities(long recordId)
        {
            EnsureIsExecutedInBackendInstance();
            var result = new List<AudioIdentityObject>();

            try
            {
                var evt = new GetDeferredRecordAudioInfoEvent();
                var record = BvPersonDeferredMonitoringPartAdapterEx.GetById(recordId);
                var audioRecords = _audioRecordsManager.Value.GetAudioRecordsInsideInterviewInterval(record.SurveySID, record.InterviewID, record.RecordCreationTime, record.InterviewDuration);

                foreach (var audioRecord in audioRecords)
                {
                    var audioIdentity = audioRecord != null ?
                        new AudioIdentityObject
                        {
                            ID = audioRecord.URI,
                            Name = audioRecord.Name,
                            TimeStamp = audioRecord.TimeStamp,
                            DialerId = audioRecord.DialerId
                        }
                        : null;

                    result.Add(audioIdentity);
                }

                evt.Finish(
                    record.PersonSID,
                    record.SurveySID,
                    _surveyRepository.Value.GetById(record.SurveySID).Name,
                    record.InterviewID,
                    record.ID,
                    !audioRecords.Any() ? new string[] { } : audioRecords.Select(x => x.Name).ToArray(),
                    !audioRecords.Any() ? new string[] { } : audioRecords.Select(x => x.URI).ToArray());
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex);

                throw new UserMessageException(ex.Message, ex);
            }
            return result;
        }

    }
}
