using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Services.Survey;
using ConfirmitDialerInterface;
using System;
using System.Threading;
using Confirmit.CATI.Core.RabbitMQ.CatiBackendNotification;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.LaunchSurvey
{
    public class Operation : AsyncOperation<Descriptor, Parameters>
    {
        private readonly IUserSurveyPermissionRepository _permissionRepository;
        private readonly ISurveyRepository _surveyRepository;
        private readonly ISurveyService _surveyService;
        private readonly ISurveyPublishService _surveyPublishService;
        private readonly IConfirmitDatabaseProvider _confirmitDatabaseProvider;
        private readonly IReplicationService _replicationService;
        private readonly IAsyncOperationRepository _asyncOperationRepository;
        private readonly IQuotaClusteringSyncService _quotaClusteringSyncService;
        private readonly ISurveyMetadataCacheService _surveyMetadataCacheService;
        private readonly IFcdQuotaService _fcdQuotaService;
        private readonly ICatiBackendNotificationPublisher _catiBackendNotificationPublisher;
        private readonly IOrderedSearchableFieldsService _orderedSearchableFieldsService;
        
        public Operation(IUserSurveyPermissionRepository permissionRepository,
                        ISurveyRepository surveyRepository,
                        ISurveyService surveyService,
                        ISurveyPublishService surveyPublishService,
                        IConfirmitDatabaseProvider confirmitDatabaseProvider,
                        IReplicationService replicationService,
                        IAsyncOperationRepository asyncOperationRepository,
                        IQuotaClusteringSyncService quotaClusteringSyncService,
                        ISurveyMetadataCacheService surveyMetadataCacheService,
                        IFcdQuotaService fcdQuotaService,
                        ICatiBackendNotificationPublisher catiBackendNotificationPublisher,
                        IOrderedSearchableFieldsService orderedSearchableFieldsService)
        {
            _permissionRepository = permissionRepository;
            _surveyRepository = surveyRepository;
            _surveyService = surveyService;
            _surveyPublishService = surveyPublishService;
            _confirmitDatabaseProvider = confirmitDatabaseProvider;
            _replicationService = replicationService;
            _asyncOperationRepository = asyncOperationRepository;
            _quotaClusteringSyncService = quotaClusteringSyncService;
            _surveyMetadataCacheService = surveyMetadataCacheService;
            _fcdQuotaService = fcdQuotaService;
            _catiBackendNotificationPublisher = catiBackendNotificationPublisher;
            _orderedSearchableFieldsService = orderedSearchableFieldsService;
        }

        public override BaseAsyncOperationManagementActivityEvent<Parameters> CreateEvent(BvAsyncOperationQueueEntity entity, Parameters parameters)
        {
            return new LaunchSurveyEvent(parameters.SurveyId, parameters.ProjectId, parameters, entity);
        }

        public override AsyncOperationResult Execute(BvAsyncOperationQueueEntity entity, Parameters parameters, IAsyncOperationProgressLogger progressLogger, BaseAsyncOperationManagementActivityEvent<Parameters> evt, CancellationToken cancellationToken)
        {
            Action<string> taskLog = message => progressLogger.AppendText(entity.Id, message, evt.Duration, true);
            var result = new AsyncOperationResult { ProcessedItemsCount = 1, State = AsyncOperationState.Completed };

            using (new EventDetailsScope(evt.Details))
            {
                // 1st thing we should do is to reset survey metadata cache locally and on the remote backend servers
                if (parameters.SurveyId != 0)
                {
                    _surveyMetadataCacheService.ResetSurveyCache(parameters.SurveyId);

                    // Metadata cache should be dropped on all servers so let's send a notification

                    _catiBackendNotificationPublisher.PublishSurveyLaunched(parameters.SurveyId);

                }

                var survey = ExecuteLaunch(parameters, taskLog, out var replicationSchemaChanged, cancellationToken);

                UpdateSurveyId(entity.Id, survey.SID);


                if (replicationSchemaChanged)
                {
                    taskLog("Synchronizing respondent and response data...");
                    try
                    {
                        _replicationService.RereadSurveyReplicatedData(survey.SID, "Launch Survey", cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add(ex);
                        result.State = AsyncOperationState.Failed;

                        return result;
                    }
                }

                _fcdQuotaService.OnLaunchSurvey(survey.SID, replicationSchemaChanged || parameters.RemoveData, cancellationToken);

                _surveyPublishService.OnLaunchSurvey(survey.SID, taskLog);

                SyncQuotaClustering(survey, result, taskLog, cancellationToken);

                _orderedSearchableFieldsService.RegenerateFields(survey.SID);
            }

            return result;
        }

        private void UpdateSurveyId(int operationId, int surveyId)
        {
            var entity = _asyncOperationRepository.Get(operationId);

            if (entity.SurveySid != surveyId)
            {
                entity.SurveySid = surveyId;
                _asyncOperationRepository.Update(entity);
            }
        }

        private BvSurveyEntity ExecuteLaunch(Parameters parameters, Action<string> taskLog, out bool replicationSchemaChanged, CancellationToken cancellationToken)
        {
            CheckParameters(parameters);

            var survey = _surveyRepository.TryGetByName(parameters.ProjectId);
            if (parameters.RemoveData && survey != null)
            {
                taskLog("Deleting interview records and scheduled calls...");
                _surveyService.CleanSurvey(parameters.SurveyId, cancellationToken);
            }

            taskLog("Updating survey properties...");
            using (var transaction = new DatabaseTransactionScope("Launch survey", DeadlockPriority.High))
            {
                if (survey == null)
                {
                    survey = _surveyService.CreateSurvey(parameters.ProjectId,
                                                parameters.SurveyProperties.ProjectName,
                                                parameters.SurveyProperties.CfSqlServerConnectionString,
                                                parameters.SurveyProperties.CreatedUserName,
                                                _confirmitDatabaseProvider.GetSqlServerName(parameters.ProjectId));

                }
                
                UpdateSurveyProperty(survey, parameters);

                UpdateSurveyAccessList(survey, parameters);

                replicationSchemaChanged = _surveyService.IsReplicationSchemaChanged(survey.SID, parameters.ReplicatedTables);

                if (replicationSchemaChanged)
                    _surveyService.UpdateReplicationScheme(survey, parameters.ReplicatedTables);
                else 
                    _surveyService.UpdateQuotaBalancingConfiguration(survey.SID, parameters.ReplicatedTables);
                
                _surveyService.UpdateReplicationStatus(survey.SID, parameters.SurveyProperties.ReplicationStatus);

                transaction.Commit();
            }

            return survey;
        }

        private void SyncQuotaClustering(BvSurveyEntity survey, AsyncOperationResult result, Action<string> taskLog, CancellationToken cancellationToken)
        {
            switch (_quotaClusteringSyncService.ReinitializeCallsAndCounters(survey, taskLog, cancellationToken))
            {
                case ReinitializeQuotaClusteringStatus.NotChanged:
                    break;
                case ReinitializeQuotaClusteringStatus.Changed:
                    result.Warnings.Add("Quota clustering was reconfigured, because cluster quota was changed.");
                    break;
                case ReinitializeQuotaClusteringStatus.Disabled:
                    result.Warnings.Add("Quota clustering was disabled, because cluster quota was deleted.");
                    break;
                default:
                    throw new Exception("Unknown sync quota status");
            }
        }

        private static void CheckParameters(Parameters parameters)
        {
            if (parameters.SurveyProperties.DialingMode == null ||
                parameters.SurveyProperties.ProjectName == null ||
                parameters.SurveyProperties.OpenEndReview == null ||
                parameters.SurveyProperties.VoiceRecording == null ||
                parameters.SurveyProperties.ScreenRecording == null)
            {
                throw new ArgumentNullException(
                    "DialingMode or ProjectName or OpenEndReview or VoiceRecording or ScreenRecording is null");
            }
        }

        private void UpdateSurveyAccessList(BvSurveyEntity survey, Parameters parameters)
        {
            _permissionRepository.DeleteAllForSpecificSurvey(survey.SID);

            foreach (var userId in parameters.PermittedUsers)
            {
                _permissionRepository.Insert(userId, parameters.ProjectId);
            }
        }

        private void UpdateSurveyProperty(BvSurveyEntity survey, Parameters parameters)
        {
            survey.DialMode = GetDialMode(parameters.SurveyProperties.DialingMode);
            survey.Description = parameters.SurveyProperties.ProjectName;
            survey.ForceOpnRev = parameters.SurveyProperties.OpenEndReview.Value ? 1 : 0;
            survey.RecWholeInt = parameters.SurveyProperties.VoiceRecording.Value ? 1 : 0;
            survey.InterviewScreenRecording = parameters.SurveyProperties.ScreenRecording.Value;
            survey.IsLiveMonitoringEnabled = parameters.SurveyProperties.LiveMonitoring;
            survey.IsTelephoneBlacklistSupported = parameters.SurveyProperties.SupportBlacklist;
            survey.IsRespondentsDynamicCreationAllowed = parameters.SurveyProperties.AllowRespondentsDynamicCreation;
            survey.NotificationEmail = parameters.SurveyProperties.NotificationEmail;
            survey.EnforceHttps = parameters.SurveyProperties.EnforceHttps;

            _surveyRepository.Update(survey);
        }

        private byte GetDialMode(int? dialingMode)
        {
            if (!dialingMode.HasValue || dialingMode.Value == 0)
            {
                return (byte)DialingMode.Manual;
            }

            return (byte)dialingMode;
        }
    }
}
