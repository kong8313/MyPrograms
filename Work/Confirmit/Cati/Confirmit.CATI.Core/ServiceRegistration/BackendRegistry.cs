using System.Collections.Generic;
using BvCallHandlerLibrary;
using Confirmit.CATI.Common.PerformanceCounters;
using Confirmit.CATI.Common.Validators;
using Confirmit.CATI.Core.AsynchronousTrigger;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.AsynchronousTrigger.Triggers;
using Confirmit.CATI.Core.AsynchronousTrigger.Triggers.CacheTriggers.CustomCacheTriggers.ScheduleTriggers;
using Confirmit.CATI.Core.AsynchronousTrigger.Triggers.CacheTriggers.CustomCacheTriggers.SystemSettingTriggers;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.PerformanceCounters;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging.Authoring;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Logger.Kibana;
using Confirmit.CATI.Core.Mail;
using Confirmit.CATI.Core.Mail.Feedback;
using Confirmit.CATI.Core.PersonLogin;
using Confirmit.CATI.Core.RabbitMQ;
using Confirmit.CATI.Core.RabbitMQ.CatiBackendNotification;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Repositories.SurveyEngine;
using Confirmit.CATI.Core.Repositories.SurveyEngine.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.FormDescValidators;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.CallDelivery;
using Confirmit.CATI.Core.Services.CallDelivery.Interfaces;
using Confirmit.CATI.Core.Services.CompanyService;
using Confirmit.CATI.Core.Services.Database;
using Confirmit.CATI.Core.Services.Database.Interfaces;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Database;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.Services.PersonImport;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;
using Confirmit.CATI.Core.Services.SchedulingScriptNotificationServiceImplementation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Services.Survey.Data;
using Confirmit.CATI.Core.Services.Survey.Quota;
using Confirmit.CATI.Core.Services.SurveyArchiveServiceImplementation;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.Console;
using Confirmit.CATI.Core.Telephony.Dial;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;
using Confirmit.CATI.Core.Telephony.Inbound;
using Confirmit.CATI.Core.Telephony.IVR.Interfaces;
using Confirmit.CATI.Core.Telephony.LinkedSurveys;
using Confirmit.CATI.Core.Validators;
using Confirmit.CATI.Core.Validators.Interfaces;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Core.Telephony.IVR;
using Confirmit.CATI.Core.Telephony.NotificationHandlers;
using Confirmit.CATI.Core.Reports.Interfaces;
using Confirmit.CATI.Core.Services.ApiClients;
using Confirmit.CATI.Core.Services.CleaningService;
using Confirmit.CATI.Core.Services.RecordsMigration;
using Confirmit.CATI.Core.Threading;
using Confirmit.MessageBroker.Consume.Sdk;
using Confirmit.MessageBroker.Publish.Sdk;

namespace Confirmit.CATI.Core.ServiceRegistration
{
    public class BackendRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.Register<ICallCenterRepository, CallCenterRepository>()

                //DAL
                .Register<IDatabaseEngineFactory, DatabaseEngineFactory>()
                .Register<IDatabaseConnectionProviderFactory, DatabaseConnectionProviderFactory>()
                .Register<IRemoteDataCopier, RemoteDataCopier>()

                //repositories
                .Register<IUserSurveyPermissionRepository, UserSurveyPermissionRepository>()
                .Register<IScheduledEmailReportsRepository, ScheduledEmailReportsRepository>()
                .Register<ITimezoneRepository, TimezoneRepository>()
                .Register<ISurveyRepository, SurveyRepository>()
                .RegisterSingleton<ISampleDataStorageRepository, SampleDataStorageRepository>()
                .Register<IFilterRepository, FilterRepository>()
                .Register<IScheduleRepository, ScheduleRepository>()
                .Register<IPersonGroupRepository, PersonGroupRepository>()
                .Register<IPersonRepository, PersonRepository>()
                .Register<ISurveyStateService, SurveyStateService>()
                .Register<IInterviewRepository, InterviewRepository>()
                .Register<ITaskRepository, TaskRepository>()
                .Register<IPersonSessionHistoryRepository, PersonSessionHistoryRepository>()
                .Register<ICallGroupRepository, CallGroupRepository>()
                .Register<IDialersRepository, DialersRepository>()
                .Register<IUserSurveyListRepository, UserSurveyListRepository>()
                .Register<IInboundTelephoneNumberRepository, InboundTelephoneNumberRepository>()
                .Register<IInboundCallsHistoryRepository, InboundCallsHistoryRepository>()
                .RegisterSingleton<ISystemStateRepository, SystemStateRepository>()
                .Register<IStateGroupRepository, StateGroupRepository>()
                .Register<IStateRepository, StateRepository>()
                .Register<IPersonMonitoringRepository, PersonMonitoringRepository>()
                .Register<IDeferredMonitoringRepository, DeferredMonitoringRepository>()
                .Register<IQuotaBalancingRepository, QuotaBalancingRepository>()
                .Register<IPersonDeferredMonitoringRepository, PersonDeferredMonitoringRepository>()
                .Register<IActiveDialRepository, ActiveDialRepository>()
                .Register<IHistoryRepository, HistoryRepository>()
                .Register<ITelephoneBlacklistRepository, TelephoneBlacklistRepository>()
                .Register<ILoginGroupRepository, LoginGroupRepository>()
                .Register<IIvrSettingsRepository, IvrSettingsRepository>()
                .Register<IOrderedSearchableFieldsRepository, OrderedSearchableFieldsRepository>()
                .Register<IExternalTransferTelephoneNumberRepository, ExternalTransferTelephoneNumberRepository>()
                .Register<IBreakTypeRepository, BreakTypeRepository>()
                .Register<ICallHistoryRepository, CallHistoryRepository>()
                .Register<IQuotaRepository, QuotaRepository>()
                .Register<IQuotaCellRepository, QuotaCellRepository>()
                .Register<ISeQuotaRepository, SeQuotaRepository>()
                .Register<ISeQuotaCellRepository, SeQuotaCellRepository>()
                .Register<IDialerFeaturesRepository, DialerFeaturesRepository>()
                .Register<IScheduleErrorRepository, ScheduleErrorRepository>()
                .Register<IInterviewQuotaCellRepository, InterviewQuotaCellRepository>()
                .Register<IQuotaDatabaseReader, QuotaDatabaseReader>()
                .Register<ISchedulingScriptLogRepository, SchedulingScriptLogRepository>()
                //services
                .Register<ICallCenterService, CallCenterService>()
                .Register<ITimezoneService, TimezoneService>()
                .Register<IPersonGroupService, PersonGroupService>()
                .Register<ISurveyService, SurveyService>()
                .Register<ISurveyPublishService, SurveyService>()
                .Register<ISurveyDatabaseService, SurveyDatabaseService>()
                .Register<IProjectsActivityService, ProjectsActivityService>()
                .Register<IReplicationService, ReplicationService>()
                .Register<IReplicationSchemaService, ReplicationSchemaService>()
                .Register<IReplicationSchemaInfoService, ReplicationSchemaService>()
                .Register<IReplicationIndexService, ReplicationIndexService>()
                .Register<ISampleService, SampleService>()
                .Register<ISurveyArchiveService, SurveyArchiveService>()
                .Register<IScheduleService, ScheduleService>()
                .Register<ICompanyInformationService, CompanyInformationService>()
                .Register<IQuotaClusteringConfigurationService, QuotaClusteringService>()
                .Register<IQuotaClusteringSyncService, QuotaClusteringService>()
                .Register<IQuotaInfoService, QuotaService>()
                .Register<ICallDeliveryService, CallDeliveryService>()
                .Register<IRetryingService, RetryingService>()
                .Register<IQuotaClusterService, QuotaClusterService>()
                .Register<IFcdQuotaService, QuotaService>()
                .Register<QuotaCellsUpdater>()
                .Register<InterviewQuotaStatusProvider>()
                .Register<ICallGroupService, CallGroupService>()
                .Register<IDatabaseObjectService, DatabaseObjectService>()
                .Register<IDatabaseNameService, DatabaseNameService>()
                .Register<IAssignmentService, AssignmentService>()
                .RegisterSingleton<ISurveyMetadataCacheService, SurveyMetadataCacheService>()
                .Register<IRespondentVariablesService, RespondentVariablesService>()
                .RegisterSingleton<ITokenCacheService, TokenCacheService>()
                .RegisterSingleton<IFormDescValidator, FormDescValidator>()
                .Register<IContextInfoService, ContextInfoService>()
                .Register<IQuotaBalancingService, QuotaBalancingService>()
                .Register<IIvrConsoleService, IvrConsoleService>()
                .Register<IIvrVariablesProvider, IvrVariablesProvider>()
                .Register<IInboundCallService, InboundCallService>()
                .Register<IMonitoringService, MonitoringService>()
                .Register<IDeferredMonitoringService, DeferredMonitoringService>()
                .Register<IStateGroupService, StateGroupService>()
                .Register<IDatabaseExpressionService, DatabaseExpressionService>()
                .Register<IDatabaseIdentifierService, DatabaseIdentifierService>()
                .Register<IActiveDialService, ActiveDialService>()
                .Register<ITelephoneBlacklistService, TelephoneBlacklistService>()
                .RegisterSingleton<IFilterService, FilterService>()
                .Register<ITimeBreakService, TimeBreakService>()
                .RegisterSingleton<IExternalTransferTelephoneNumberService, ExternalTransferTelephoneNumberService>()
                .Register<IInterviewQuotaCellService, InterviewQuotaCellService>()
                .Register<QuotaMatcherBuilder>()
                .Register<IReplicatedDataRepository, ReplicatedDataRepository>()
                .Register<IDatabaseAttachService, DatabaseAttachService>()
                .Register<IActiveSupervisorService, ActiveSupervisorService>()
                .Register<InterviewersAvailabilityService>()
                .Register<IOrderedSearchableFieldsService, OrderedSearchableFieldsService>()
                .Register<IMigrationService, MigrationService>()
                //handlers
                .Register<IDialerNotifyInboundCallHandler, DialerNotifyInboundCallHandler>()
                .Register<IDialerNotifyInboundCallDroppedByRespondentHandler, DialerNotifyInboundCallDroppedByRespondentHandler>()
                .Register<IDialerNotifyCallDroppedByRespondentHandler, DialerNotifyCallDroppedByRespondentHandler>()
                .Register<IInternalVoiceXmlApiFactory, InternalVoiceXmlApiFactory>()
                .Register<IServiceDiscoveryClientProxy, ServiceDiscoveryClientProxy>()
                .Register<IReviewerService, ReviewerService>()
                .Register<IIdentityService, IdentityService>()
                .Register<IRespondentsClient, RespondentsClient>()
                .Register<IFeatureToggleClient, FeatureToggleClient>()
                .Register<IInterviewerApiClient, InterviewerApiClient>()
                .Register<ISupervisorApiClient, SupervisorApiClient>()
                .Register<IDialerApiClient, DialerApiClient>()
                .Register<IResponseReviewerApiClient, ResponseReviewerApiClient>()
                .Register<IInterviewResponseDataService, InterviewResponseDataService>()

                //validators
                .RegisterSingleton<IMultipleAssignmentValidator, MultipleAssignmentValidator>()
                .RegisterSingleton<IPersonGroupValidator, PersonGroupValidator>()
                .Register<IInterviewFormDataWebSourceService, InterviewFormDataWebSourceService>()
                .Register<ISurveyDataRowsDatabaseUpdater, SurveyDataRowsDatabaseUpdater>()
                .Register<ISurveyDataRowsWebServiceUpdater, SurveyDataRowsWebServiceUpdater>()
                .Register<IInterviewFormDataDatabaseSourceService, InterviewFormDataDatabaseSourceService>()
                .Register<IInterviewRespondentDataSourceService, InterviewRespondentDataService>()
                .Register<IDatabaseIndexService, DatabaseIndexService>()
                .Register<IDatabaseStatisticService, DatabaseStatisticService>()
                .Register<IPersonMessageService, PersonService>()
                .Register<ISurveyCallDistributionService, SurveyService>()
                .Register<ISurveyCleaningService, SurveyCleaningService>()
                .Register<ISurveyCleaningConfirmitDataAccess, SurveyCleaningConfirmitDataAccess>()
                .Register<ISurveyCleaningDataAccess, SurveyCleaningDataAccess>()
                .Register<ICleaningServiceEmailGenerator, CleaningServiceEmailGenerator>()
                .Register<IDatabaseServerPropertiesProvider, DatabaseServerPropertiesProvider>()
                .Register<ISurveyConnectionStringProvider, SurveyConnectionStringProvider>()
                .Register<IConfirmitDatabaseProvider, ConfirmitDatabaseProvider>()
                .RegisterSingleton<IAuthoringService, WcfServices.Clients.AuthoringService>()
                .RegisterSingleton<ISurveyDataService, WcfServices.Clients.SurveyDataService>()

                //factories
                .Register<ISampleDataStorageFactory, SampleDataStorageFactory>()
                .Register<IInterviewDataServiceFactory, InterviewFormDataServiceFactory>()
                .Register<IBackendInstanceFactory, BackendInstanceFactory>()
                .RegisterSingleton<IShiftServiceFactory, ShiftServiceFactory>()

                //providers
                .Register<ISurveyDatabaseInfoProvider, SurveyDatabaseInfoProvider>()
                .Register<IDbLibProvider, DbLibProvider>()
                .Register<ILinkedInterviewProvider, LinkedInterviewProvider>()
                .Register<ISqlFilterProvider, SqlFilterProvider>()

                //misc
                .Register<ISampleBatchProcessor, SampleBatchProcessor>()
                .Register<ISampleRecordProcessorFactory, SampleRecordProcessorFactory>()
                .Register<IInputParameterValidator, InputParameterValidator>()
                .Register<IInvalidSymbolsRepairer, InvalidSymbolsRepairer>()
                .Register<ISupervisorNameProvider, BackendSupervisorNameProvider>()
                .Register<ICompanyInfo, CompanyInfo>()
                .Register<IConnectionStrings, ConnectionStrings>()
                .Register<IInstanceInfo, InstanceInfo>()
                .Register<ISurveyDatabaseEngine, SurveyDatabaseEngine>()
                .Register<ICallHistoryDataProvider, CallHistoryDataProvider>()
                .Register<IRedialNumberSaver, RedialNumberSaver>()
                .Register<ILanguageVariableProvider, LanguageVariableProvider>()
                .Register<IMultimodeInstanceName, MultimodeInstanceName>()
                .Register<IStationIdParser, StationIdParser>()
                .Register<IStationInfoFactory, StationInfoFactory>()
                .Register<IDatabaseLockTimeouts, DatabaseLockTimeouts>()
                .Register<IInterviewerSessionsReportQuery, InterviewerSessionsReportQuery>()
                .Register<IPerformanceCategoryCreator, PerformanceCategoryCreator>()
                .Register<IPerformanceCounterFactory, PerformanceCounterFactory>()
                .RegisterSingleton<IPerformanceCountersContainer, PerformanceCountersContainer>()

                //logs
                .RegisterSingleton<ILogWriter, KibanaLogWriter>()
                .Register<ISystemActivity, SystemActivity>()

                //call delivery/management
                .Register<ICallQueueService, CallQueueService>()
                .Register<ICallsManagementService, CallsManagementService>()
                .Register<ICallRequestFactory, CallRequestFactory>()
                .Register<ICallRequestResultFactory, CallRequestResultFactory>()
                .Register<IEditCallsQueryProvider, EditCallsQueryProvider>()
                .Register<IInterviewService, InterviewService>()
                .Register<IPasswordRulesChecker, PasswordRulesChecker>()
                .Register<IRespondentObtainer, RespondentDataObtainer>()
                .Register<IRespondentBatchObtainer, RespondentDataObtainer>()
                .Register<IAudioMonitoring, AudioMonitoring>()
                .Register<IInterviewRecordingManager, InterviewRecordingManager>()
                .Register<IRespondentsSynchronizationProcessor, RespondentsRespondentsSynchronizationProcessor>()
                
                .Register<IEnumerable<IAsynchronousTrigger>, IAsynchronousTrigger[]>()
                // Standard CacheT riggers
                // TODO: Later we  should replace RegisterInstance with just registering a singleton object,
                //       so the object should be created by container and not by us.
                .RegisterInstance<IAsynchronousTrigger>("BvPersonCache", BvPersonCache.Instance)
                .RegisterInstance<IAsynchronousTrigger>("BvPersonGroupCache", BvPersonGroupCache.Instance)
                .RegisterInstance<IAsynchronousTrigger>("BvScheduleCache", BvScheduleCache.Instance)
                .RegisterInstance<IAsynchronousTrigger>("BvScheduleParamCache", BvScheduleParamCache.Instance)
                .RegisterInstance<IAsynchronousTrigger>("BvStateCache", BvStateCache.Instance)
                .RegisterInstance<IAsynchronousTrigger>("BvSurveyCache", BvSurveyCache.Instance)
                .RegisterInstance<IAsynchronousTrigger>("BvCallCenterCache", BvCallCenterCache.Instance)
                .RegisterInstance<IAsynchronousTrigger>("BvTimezoneCache", BvTimezoneCache.Instance)
                .RegisterInstance<IAsynchronousTrigger>("BvInboundTelephoneNumberCache", BvInboundTelephoneNumberCache.Instance)
                .RegisterInstance<IAsynchronousTrigger>("BvBreakTypeCache", BvBreakTypeCache.Instance)

                // System Setting Cache Triggers
                .Register<IAsynchronousTrigger, BvSystemSettingTrigger>("BvSystemSettingTrigger")

                // Schedule Cache Triggers
                .Register<IAsynchronousTrigger, BvShiftTrigger>("BvShiftTrigger")
                .Register<IAsynchronousTrigger, BvShiftTypeTrigger>("BvShiftTypeTrigger")
                .Register<IAsynchronousTrigger, BvTimezoneShiftTrigger>("BvTimezoneShiftTrigger")

                // Rest Triggers
                .Register<IAsynchronousTrigger, BvDialersTrigger>("BvDialersTrigger")
                .Register<IAsynchronousTrigger, BvBackendInstanceTrigger>("BvBackendInstanceTrigger")
                .Register<IDatabaseAppLockService, DatabaseAppLockService>()
                .Register<IMailSender, ConfirmitAuthoringMailSender>()
                .Register<ISchedulingScriptNotificator, SchedulingScriptNotificator>()
                .Register<IFeedbackMessageCreator, FeedbackMessageCreator>()
                .Register<IExtraQuotaCounterService, ExtraQuotaCounterService>()
                .Register<IUsedCallsCalculator, UsedCallsCalculator>()
                .Register<ISchedulingScriptLogger, SchedulingScriptLogger>()

                //RabbitMQ
                .RegisterSingleton<RabbitMQConnectionProvider, RabbitMQConnectionProvider>()
                .RegisterSingleton<SqlTableUpdatedConsumptionRegistry, SqlTableUpdatedConsumptionRegistry>()
                .Register<IMessageHandler<SqlTableUpdatedMessage>, SqlTableUpdatedHandler>()
                .Register<SqlTableUpdatedWorker>()
                .RegisterSingleton<CatiBackendNotificationConsumptionRegistry, CatiBackendNotificationConsumptionRegistry>()
                .RegisterSingleton<IMessageHandler<CatiBackendNotification>, CatiBackendNotificationHandler>()
                .Register<ICatiBackendNotificationHandler, SurveyLaunchedHandler>(nameof(SurveyLaunchedHandler))
                .Register<ICatiBackendNotificationHandler, AsyncOperationCancelledHandler>(nameof(AsyncOperationCancelledHandler))
                .Register<SurveyLaunchedWorker>()
                .Register<AsyncOperationCancelledWorker>()
                .Register<IConfirmitMessageBrokerPublisher, ConfirmitMessageBrokerPublisher>()
                .Register<CatiMessageBrokerPublisher>()
                .Register<ICatiBackendNotificationPublisher, CatiBackendNotificationPublisher>()
                .RegisterSingleton<ISqlTableUpdatedPublisher, SqlTableUpdatedPublisher>()
                
                .Register<PeriodicalThreadSettings>();
        }
    }
}