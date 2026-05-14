using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubISystemSettings : ISystemSettings 
    {
        private ISystemSettings _inner;

        public StubISystemSettings()
        {
            _inner = null;
        }

        public ISystemSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private IAccountLockingSettings _AccountLocking;
        public Func<IAccountLockingSettings> AccountLockingGet;
        public Action<IAccountLockingSettings> AccountLockingSetIAccountLockingSettings;

        IAccountLockingSettings ISystemSettings.AccountLocking
        {
            get
            {
                if (AccountLockingGet != null)
                {
                    return AccountLockingGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).AccountLocking;
                }

                if (AccountLockingSetIAccountLockingSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AccountLocking;
                }

                return default(IAccountLockingSettings);
            }

        }

        private IActivityLoggingSettings _ActivityLogging;
        public Func<IActivityLoggingSettings> ActivityLoggingGet;
        public Action<IActivityLoggingSettings> ActivityLoggingSetIActivityLoggingSettings;

        IActivityLoggingSettings ISystemSettings.ActivityLogging
        {
            get
            {
                if (ActivityLoggingGet != null)
                {
                    return ActivityLoggingGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).ActivityLogging;
                }

                if (ActivityLoggingSetIActivityLoggingSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ActivityLogging;
                }

                return default(IActivityLoggingSettings);
            }

        }

        private IAlertingSettings _Alerting;
        public Func<IAlertingSettings> AlertingGet;
        public Action<IAlertingSettings> AlertingSetIAlertingSettings;

        IAlertingSettings ISystemSettings.Alerting
        {
            get
            {
                if (AlertingGet != null)
                {
                    return AlertingGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Alerting;
                }

                if (AlertingSetIAlertingSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Alerting;
                }

                return default(IAlertingSettings);
            }

        }

        private IAppointmentAlertSettings _AppointmentAlert;
        public Func<IAppointmentAlertSettings> AppointmentAlertGet;
        public Action<IAppointmentAlertSettings> AppointmentAlertSetIAppointmentAlertSettings;

        IAppointmentAlertSettings ISystemSettings.AppointmentAlert
        {
            get
            {
                if (AppointmentAlertGet != null)
                {
                    return AppointmentAlertGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).AppointmentAlert;
                }

                if (AppointmentAlertSetIAppointmentAlertSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AppointmentAlert;
                }

                return default(IAppointmentAlertSettings);
            }

        }

        private IAsyncOperationSettings _AsyncOperation;
        public Func<IAsyncOperationSettings> AsyncOperationGet;
        public Action<IAsyncOperationSettings> AsyncOperationSetIAsyncOperationSettings;

        IAsyncOperationSettings ISystemSettings.AsyncOperation
        {
            get
            {
                if (AsyncOperationGet != null)
                {
                    return AsyncOperationGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).AsyncOperation;
                }

                if (AsyncOperationSetIAsyncOperationSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AsyncOperation;
                }

                return default(IAsyncOperationSettings);
            }

        }

        private IAsyncOperationsSettings _AsyncOperations;
        public Func<IAsyncOperationsSettings> AsyncOperationsGet;
        public Action<IAsyncOperationsSettings> AsyncOperationsSetIAsyncOperationsSettings;

        IAsyncOperationsSettings ISystemSettings.AsyncOperations
        {
            get
            {
                if (AsyncOperationsGet != null)
                {
                    return AsyncOperationsGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).AsyncOperations;
                }

                if (AsyncOperationsSetIAsyncOperationsSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AsyncOperations;
                }

                return default(IAsyncOperationsSettings);
            }

        }

        private IAutoLogoutSettings _AutoLogout;
        public Func<IAutoLogoutSettings> AutoLogoutGet;
        public Action<IAutoLogoutSettings> AutoLogoutSetIAutoLogoutSettings;

        IAutoLogoutSettings ISystemSettings.AutoLogout
        {
            get
            {
                if (AutoLogoutGet != null)
                {
                    return AutoLogoutGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).AutoLogout;
                }

                if (AutoLogoutSetIAutoLogoutSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AutoLogout;
                }

                return default(IAutoLogoutSettings);
            }

        }

        private ICacheCallsSettings _CacheCalls;
        public Func<ICacheCallsSettings> CacheCallsGet;
        public Action<ICacheCallsSettings> CacheCallsSetICacheCallsSettings;

        ICacheCallsSettings ISystemSettings.CacheCalls
        {
            get
            {
                if (CacheCallsGet != null)
                {
                    return CacheCallsGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).CacheCalls;
                }

                if (CacheCallsSetICacheCallsSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CacheCalls;
                }

                return default(ICacheCallsSettings);
            }

        }

        private ICallGroupSettings _CallGroup;
        public Func<ICallGroupSettings> CallGroupGet;
        public Action<ICallGroupSettings> CallGroupSetICallGroupSettings;

        ICallGroupSettings ISystemSettings.CallGroup
        {
            get
            {
                if (CallGroupGet != null)
                {
                    return CallGroupGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).CallGroup;
                }

                if (CallGroupSetICallGroupSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CallGroup;
                }

                return default(ICallGroupSettings);
            }

        }

        private ICallHistoryHubSettings _CallHistoryHub;
        public Func<ICallHistoryHubSettings> CallHistoryHubGet;
        public Action<ICallHistoryHubSettings> CallHistoryHubSetICallHistoryHubSettings;

        ICallHistoryHubSettings ISystemSettings.CallHistoryHub
        {
            get
            {
                if (CallHistoryHubGet != null)
                {
                    return CallHistoryHubGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).CallHistoryHub;
                }

                if (CallHistoryHubSetICallHistoryHubSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CallHistoryHub;
                }

                return default(ICallHistoryHubSettings);
            }

        }

        private ICallManagementSettings _CallManagement;
        public Func<ICallManagementSettings> CallManagementGet;
        public Action<ICallManagementSettings> CallManagementSetICallManagementSettings;

        ICallManagementSettings ISystemSettings.CallManagement
        {
            get
            {
                if (CallManagementGet != null)
                {
                    return CallManagementGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).CallManagement;
                }

                if (CallManagementSetICallManagementSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CallManagement;
                }

                return default(ICallManagementSettings);
            }

        }

        private IConsoleSettings _Console;
        public Func<IConsoleSettings> ConsoleGet;
        public Action<IConsoleSettings> ConsoleSetIConsoleSettings;

        IConsoleSettings ISystemSettings.Console
        {
            get
            {
                if (ConsoleGet != null)
                {
                    return ConsoleGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Console;
                }

                if (ConsoleSetIConsoleSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Console;
                }

                return default(IConsoleSettings);
            }

        }

        private IDebugSettings _Debug;
        public Func<IDebugSettings> DebugGet;
        public Action<IDebugSettings> DebugSetIDebugSettings;

        IDebugSettings ISystemSettings.Debug
        {
            get
            {
                if (DebugGet != null)
                {
                    return DebugGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Debug;
                }

                if (DebugSetIDebugSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Debug;
                }

                return default(IDebugSettings);
            }

        }

        private IDialerSettings _Dialer;
        public Func<IDialerSettings> DialerGet;
        public Action<IDialerSettings> DialerSetIDialerSettings;

        IDialerSettings ISystemSettings.Dialer
        {
            get
            {
                if (DialerGet != null)
                {
                    return DialerGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Dialer;
                }

                if (DialerSetIDialerSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Dialer;
                }

                return default(IDialerSettings);
            }

        }

        private IEmailSettings _Email;
        public Func<IEmailSettings> EmailGet;
        public Action<IEmailSettings> EmailSetIEmailSettings;

        IEmailSettings ISystemSettings.Email
        {
            get
            {
                if (EmailGet != null)
                {
                    return EmailGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Email;
                }

                if (EmailSetIEmailSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Email;
                }

                return default(IEmailSettings);
            }

        }

        private IFCDSettings _FCD;
        public Func<IFCDSettings> FCDGet;
        public Action<IFCDSettings> FCDSetIFCDSettings;

        IFCDSettings ISystemSettings.FCD
        {
            get
            {
                if (FCDGet != null)
                {
                    return FCDGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).FCD;
                }

                if (FCDSetIFCDSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _FCD;
                }

                return default(IFCDSettings);
            }

        }

        private IInterviewerPasswordSettings _InterviewerPassword;
        public Func<IInterviewerPasswordSettings> InterviewerPasswordGet;
        public Action<IInterviewerPasswordSettings> InterviewerPasswordSetIInterviewerPasswordSettings;

        IInterviewerPasswordSettings ISystemSettings.InterviewerPassword
        {
            get
            {
                if (InterviewerPasswordGet != null)
                {
                    return InterviewerPasswordGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).InterviewerPassword;
                }

                if (InterviewerPasswordSetIInterviewerPasswordSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InterviewerPassword;
                }

                return default(IInterviewerPasswordSettings);
            }

        }

        private IInterviewerPropertiesSettings _InterviewerProperties;
        public Func<IInterviewerPropertiesSettings> InterviewerPropertiesGet;
        public Action<IInterviewerPropertiesSettings> InterviewerPropertiesSetIInterviewerPropertiesSettings;

        IInterviewerPropertiesSettings ISystemSettings.InterviewerProperties
        {
            get
            {
                if (InterviewerPropertiesGet != null)
                {
                    return InterviewerPropertiesGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).InterviewerProperties;
                }

                if (InterviewerPropertiesSetIInterviewerPropertiesSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InterviewerProperties;
                }

                return default(IInterviewerPropertiesSettings);
            }

        }

        private IIvrSettings _Ivr;
        public Func<IIvrSettings> IvrGet;
        public Action<IIvrSettings> IvrSetIIvrSettings;

        IIvrSettings ISystemSettings.Ivr
        {
            get
            {
                if (IvrGet != null)
                {
                    return IvrGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Ivr;
                }

                if (IvrSetIIvrSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Ivr;
                }

                return default(IIvrSettings);
            }

        }

        private ILoggingSettings _Logging;
        public Func<ILoggingSettings> LoggingGet;
        public Action<ILoggingSettings> LoggingSetILoggingSettings;

        ILoggingSettings ISystemSettings.Logging
        {
            get
            {
                if (LoggingGet != null)
                {
                    return LoggingGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Logging;
                }

                if (LoggingSetILoggingSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Logging;
                }

                return default(ILoggingSettings);
            }

        }

        private IMonitoringSettings _Monitoring;
        public Func<IMonitoringSettings> MonitoringGet;
        public Action<IMonitoringSettings> MonitoringSetIMonitoringSettings;

        IMonitoringSettings ISystemSettings.Monitoring
        {
            get
            {
                if (MonitoringGet != null)
                {
                    return MonitoringGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Monitoring;
                }

                if (MonitoringSetIMonitoringSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Monitoring;
                }

                return default(IMonitoringSettings);
            }

        }

        private IMultipleAssignmentsSettings _MultipleAssignments;
        public Func<IMultipleAssignmentsSettings> MultipleAssignmentsGet;
        public Action<IMultipleAssignmentsSettings> MultipleAssignmentsSetIMultipleAssignmentsSettings;

        IMultipleAssignmentsSettings ISystemSettings.MultipleAssignments
        {
            get
            {
                if (MultipleAssignmentsGet != null)
                {
                    return MultipleAssignmentsGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).MultipleAssignments;
                }

                if (MultipleAssignmentsSetIMultipleAssignmentsSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MultipleAssignments;
                }

                return default(IMultipleAssignmentsSettings);
            }

        }

        private IQuotaBalancingSettings _QuotaBalancing;
        public Func<IQuotaBalancingSettings> QuotaBalancingGet;
        public Action<IQuotaBalancingSettings> QuotaBalancingSetIQuotaBalancingSettings;

        IQuotaBalancingSettings ISystemSettings.QuotaBalancing
        {
            get
            {
                if (QuotaBalancingGet != null)
                {
                    return QuotaBalancingGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).QuotaBalancing;
                }

                if (QuotaBalancingSetIQuotaBalancingSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _QuotaBalancing;
                }

                return default(IQuotaBalancingSettings);
            }

        }

        private IQuotaClusteringSettings _QuotaClustering;
        public Func<IQuotaClusteringSettings> QuotaClusteringGet;
        public Action<IQuotaClusteringSettings> QuotaClusteringSetIQuotaClusteringSettings;

        IQuotaClusteringSettings ISystemSettings.QuotaClustering
        {
            get
            {
                if (QuotaClusteringGet != null)
                {
                    return QuotaClusteringGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).QuotaClustering;
                }

                if (QuotaClusteringSetIQuotaClusteringSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _QuotaClustering;
                }

                return default(IQuotaClusteringSettings);
            }

        }

        private IQuotasSettings _Quotas;
        public Func<IQuotasSettings> QuotasGet;
        public Action<IQuotasSettings> QuotasSetIQuotasSettings;

        IQuotasSettings ISystemSettings.Quotas
        {
            get
            {
                if (QuotasGet != null)
                {
                    return QuotasGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Quotas;
                }

                if (QuotasSetIQuotasSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Quotas;
                }

                return default(IQuotasSettings);
            }

        }

        private IRecordedInterviewsSettings _RecordedInterviews;
        public Func<IRecordedInterviewsSettings> RecordedInterviewsGet;
        public Action<IRecordedInterviewsSettings> RecordedInterviewsSetIRecordedInterviewsSettings;

        IRecordedInterviewsSettings ISystemSettings.RecordedInterviews
        {
            get
            {
                if (RecordedInterviewsGet != null)
                {
                    return RecordedInterviewsGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).RecordedInterviews;
                }

                if (RecordedInterviewsSetIRecordedInterviewsSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _RecordedInterviews;
                }

                return default(IRecordedInterviewsSettings);
            }

        }

        private IReplicationSettings _Replication;
        public Func<IReplicationSettings> ReplicationGet;
        public Action<IReplicationSettings> ReplicationSetIReplicationSettings;

        IReplicationSettings ISystemSettings.Replication
        {
            get
            {
                if (ReplicationGet != null)
                {
                    return ReplicationGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Replication;
                }

                if (ReplicationSetIReplicationSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Replication;
                }

                return default(IReplicationSettings);
            }

        }

        private IReportsSettings _Reports;
        public Func<IReportsSettings> ReportsGet;
        public Action<IReportsSettings> ReportsSetIReportsSettings;

        IReportsSettings ISystemSettings.Reports
        {
            get
            {
                if (ReportsGet != null)
                {
                    return ReportsGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Reports;
                }

                if (ReportsSetIReportsSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Reports;
                }

                return default(IReportsSettings);
            }

        }

        private IRetryingServiceSettings _RetryingService;
        public Func<IRetryingServiceSettings> RetryingServiceGet;
        public Action<IRetryingServiceSettings> RetryingServiceSetIRetryingServiceSettings;

        IRetryingServiceSettings ISystemSettings.RetryingService
        {
            get
            {
                if (RetryingServiceGet != null)
                {
                    return RetryingServiceGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).RetryingService;
                }

                if (RetryingServiceSetIRetryingServiceSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _RetryingService;
                }

                return default(IRetryingServiceSettings);
            }

        }

        private IReviewerSettings _Reviewer;
        public Func<IReviewerSettings> ReviewerGet;
        public Action<IReviewerSettings> ReviewerSetIReviewerSettings;

        IReviewerSettings ISystemSettings.Reviewer
        {
            get
            {
                if (ReviewerGet != null)
                {
                    return ReviewerGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Reviewer;
                }

                if (ReviewerSetIReviewerSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Reviewer;
                }

                return default(IReviewerSettings);
            }

        }

        private IRoutineMaintenanceSettings _RoutineMaintenance;
        public Func<IRoutineMaintenanceSettings> RoutineMaintenanceGet;
        public Action<IRoutineMaintenanceSettings> RoutineMaintenanceSetIRoutineMaintenanceSettings;

        IRoutineMaintenanceSettings ISystemSettings.RoutineMaintenance
        {
            get
            {
                if (RoutineMaintenanceGet != null)
                {
                    return RoutineMaintenanceGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).RoutineMaintenance;
                }

                if (RoutineMaintenanceSetIRoutineMaintenanceSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _RoutineMaintenance;
                }

                return default(IRoutineMaintenanceSettings);
            }

        }

        private ISchedulingScriptSettings _SchedulingScript;
        public Func<ISchedulingScriptSettings> SchedulingScriptGet;
        public Action<ISchedulingScriptSettings> SchedulingScriptSetISchedulingScriptSettings;

        ISchedulingScriptSettings ISystemSettings.SchedulingScript
        {
            get
            {
                if (SchedulingScriptGet != null)
                {
                    return SchedulingScriptGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).SchedulingScript;
                }

                if (SchedulingScriptSetISchedulingScriptSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SchedulingScript;
                }

                return default(ISchedulingScriptSettings);
            }

        }

        private ISecuritySettings _Security;
        public Func<ISecuritySettings> SecurityGet;
        public Action<ISecuritySettings> SecuritySetISecuritySettings;

        ISecuritySettings ISystemSettings.Security
        {
            get
            {
                if (SecurityGet != null)
                {
                    return SecurityGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Security;
                }

                if (SecuritySetISecuritySettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Security;
                }

                return default(ISecuritySettings);
            }

        }

        private IServerSettings _Server;
        public Func<IServerSettings> ServerGet;
        public Action<IServerSettings> ServerSetIServerSettings;

        IServerSettings ISystemSettings.Server
        {
            get
            {
                if (ServerGet != null)
                {
                    return ServerGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Server;
                }

                if (ServerSetIServerSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Server;
                }

                return default(IServerSettings);
            }

        }

        private ISetupSettings _Setup;
        public Func<ISetupSettings> SetupGet;
        public Action<ISetupSettings> SetupSetISetupSettings;

        ISetupSettings ISystemSettings.Setup
        {
            get
            {
                if (SetupGet != null)
                {
                    return SetupGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Setup;
                }

                if (SetupSetISetupSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Setup;
                }

                return default(ISetupSettings);
            }

        }

        private ISiteSettings _Site;
        public Func<ISiteSettings> SiteGet;
        public Action<ISiteSettings> SiteSetISiteSettings;

        ISiteSettings ISystemSettings.Site
        {
            get
            {
                if (SiteGet != null)
                {
                    return SiteGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Site;
                }

                if (SiteSetISiteSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Site;
                }

                return default(ISiteSettings);
            }

        }

        private ISQLServerSettings _SQLServer;
        public Func<ISQLServerSettings> SQLServerGet;
        public Action<ISQLServerSettings> SQLServerSetISQLServerSettings;

        ISQLServerSettings ISystemSettings.SQLServer
        {
            get
            {
                if (SQLServerGet != null)
                {
                    return SQLServerGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).SQLServer;
                }

                if (SQLServerSetISQLServerSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SQLServer;
                }

                return default(ISQLServerSettings);
            }

        }

        private ISupervisorSettings _Supervisor;
        public Func<ISupervisorSettings> SupervisorGet;
        public Action<ISupervisorSettings> SupervisorSetISupervisorSettings;

        ISupervisorSettings ISystemSettings.Supervisor
        {
            get
            {
                if (SupervisorGet != null)
                {
                    return SupervisorGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Supervisor;
                }

                if (SupervisorSetISupervisorSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Supervisor;
                }

                return default(ISupervisorSettings);
            }

        }

        private ISurveysSettings _Surveys;
        public Func<ISurveysSettings> SurveysGet;
        public Action<ISurveysSettings> SurveysSetISurveysSettings;

        ISurveysSettings ISystemSettings.Surveys
        {
            get
            {
                if (SurveysGet != null)
                {
                    return SurveysGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Surveys;
                }

                if (SurveysSetISurveysSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Surveys;
                }

                return default(ISurveysSettings);
            }

        }

        private ITelephoneBlacklistSettings _TelephoneBlacklist;
        public Func<ITelephoneBlacklistSettings> TelephoneBlacklistGet;
        public Action<ITelephoneBlacklistSettings> TelephoneBlacklistSetITelephoneBlacklistSettings;

        ITelephoneBlacklistSettings ISystemSettings.TelephoneBlacklist
        {
            get
            {
                if (TelephoneBlacklistGet != null)
                {
                    return TelephoneBlacklistGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).TelephoneBlacklist;
                }

                if (TelephoneBlacklistSetITelephoneBlacklistSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _TelephoneBlacklist;
                }

                return default(ITelephoneBlacklistSettings);
            }

        }

        private ITimeZoneBalancingSettings _TimeZoneBalancing;
        public Func<ITimeZoneBalancingSettings> TimeZoneBalancingGet;
        public Action<ITimeZoneBalancingSettings> TimeZoneBalancingSetITimeZoneBalancingSettings;

        ITimeZoneBalancingSettings ISystemSettings.TimeZoneBalancing
        {
            get
            {
                if (TimeZoneBalancingGet != null)
                {
                    return TimeZoneBalancingGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).TimeZoneBalancing;
                }

                if (TimeZoneBalancingSetITimeZoneBalancingSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _TimeZoneBalancing;
                }

                return default(ITimeZoneBalancingSettings);
            }

        }

        private IToggleSettings _Toggle;
        public Func<IToggleSettings> ToggleGet;
        public Action<IToggleSettings> ToggleSetIToggleSettings;

        IToggleSettings ISystemSettings.Toggle
        {
            get
            {
                if (ToggleGet != null)
                {
                    return ToggleGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).Toggle;
                }

                if (ToggleSetIToggleSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Toggle;
                }

                return default(IToggleSettings);
            }

        }

        private IWebApiSettings _WebApi;
        public Func<IWebApiSettings> WebApiGet;
        public Action<IWebApiSettings> WebApiSetIWebApiSettings;

        IWebApiSettings ISystemSettings.WebApi
        {
            get
            {
                if (WebApiGet != null)
                {
                    return WebApiGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).WebApi;
                }

                if (WebApiSetIWebApiSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _WebApi;
                }

                return default(IWebApiSettings);
            }

        }

        private IWebServiceUrlSettings _WebServiceUrl;
        public Func<IWebServiceUrlSettings> WebServiceUrlGet;
        public Action<IWebServiceUrlSettings> WebServiceUrlSetIWebServiceUrlSettings;

        IWebServiceUrlSettings ISystemSettings.WebServiceUrl
        {
            get
            {
                if (WebServiceUrlGet != null)
                {
                    return WebServiceUrlGet();
                } else if (_inner != null)
                {
                    return ((ISystemSettings)_inner).WebServiceUrl;
                }

                if (WebServiceUrlSetIWebServiceUrlSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _WebServiceUrl;
                }

                return default(IWebServiceUrlSettings);
            }

        }

    }
}