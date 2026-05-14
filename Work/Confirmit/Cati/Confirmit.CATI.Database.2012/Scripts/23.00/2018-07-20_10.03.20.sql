DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH [Data]( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
SELECT 'ActivityLogging.InterviewerActivityEventTimingsThreshold', 'Threshold of Interviewer Activity Event timings', 'Logging', 'If Interviewer Activity Event takes longer than Threshold time then it will contain timings information, otherwise it will not.', 4, 0, '00:00:00'
UNION ALL
SELECT 'ActivityLogging.ManagementActivityEventTimingsThreshold', 'Threshold of Management Activity Event timings', 'Logging', 'If Management Activity Event takes longer than Threshold time then it will contain timings information, otherwise it will not.', 4, 0, '00:00:00'
UNION ALL
SELECT 'Site.StartSurveyURL', 'Start survey URL', 'System', 'URL which is used for generation of interview url.', 2, 0, 'http://localhost/wix/cati_'
UNION ALL
SELECT 'SQLServer.DefaultSqlCommandTimeout', 'Default SQL command timeout', 'System', 'This timeout is used in Data Access Layer.', 1, 0, '120'
UNION ALL
SELECT 'SQLServer.DefaultSqlConnectionTimeout', 'Default SQL connection timeout', 'System', 'This timeout is used in Data Access Layer.', 1, 0, '120'
UNION ALL
SELECT 'SQLServer.SqlServerDataPath', 'SQL Server datapath', 'System', 'Path to the master DB file (used in Backend instance registrator). Either both SqlServerDataPath and SqlServerLogPath should be empty or both should contain path.', 2, 0, ''
UNION ALL
SELECT 'SQLServer.SqlServerLogPath', 'SQL Server log path', 'System', 'Path to save DB logs. Either both SqlServerDataPath and SqlServerLogPath should be empty or both should contain path.', 2, 0, ''
UNION ALL
SELECT 'Server.AccessAllowedIPAddresses', 'Access allowed IP addresses', 'System', 'IP addresses which internal WCF services are allowed for.', 2, 0, '127.0.0.1'
UNION ALL
SELECT 'Server.ServiceStartTimeout', 'Service start timeout', 'System', 'Start CATI instance service timeout (in sec).', 1, 0, '60'
UNION ALL
SELECT 'WebServiceUrl.Authoring', 'Confirmit Authoring WS Url', 'System', 'Confirmit Authoring Web Service Url.', 2, 0, 'http://localhost/Confirmit/InternalWebServices/14.0/FusionAuthoring.asmx'
UNION ALL
SELECT 'WebServiceUrl.SurveyData', 'Confirmit SurveyData WS Url', 'System', 'Confirmit SurveyData Web Service Url.', 2, 0, 'http://localhost/confirmit/InternalWebServices/14.0/FusionSurveyData.asmx'
UNION ALL
SELECT 'WebServiceUrl.DictionaryApi', 'Confirmit Dictionary Api (RestApi) Url', 'System', 'Url to dictionary API (RestApi).', 2, 0, NULL
UNION ALL
SELECT 'Reviewer.LimitOfAmountOfInterviewsPerSession', 'Maximum number of interviews for one session', 'System', 'Maximum number of interviews for one session.', 2, 0, '100'
UNION ALL
SELECT 'Reviewer.SessionUrlTemplate', 'Url template for session to review', 'System', 'Url template for session to review.', 2, 0, NULL
UNION ALL
SELECT 'Debug.BackendStartup', 'Breakpoint on backend startup', 'Debug', 'Breakpoint on backend startup', 3, 0, 'False'
UNION ALL
SELECT 'Debug.PublishMetadataForExternalWCFServices', 'Publish Metadata For External WCF Services', 'Debug', 'Should metadata for external services be published or not. Possible values: True or False', 3, 0, 'True'
UNION ALL
SELECT 'Debug.PublishMetadataForInternalWCFServices', 'Publish Metadata For Internal WCF Services', 'Debug', 'Should metadata for internal services be published or not. Possible values: True or False', 3, 0, 'True'
UNION ALL
SELECT 'Email.NotificationEmailRecipients', 'Notification Email recipients', 'Logging', 'Email address(es) to send an email if the connection to the dialer is lost or the local dialer component is restarted.', 2, 0, NULL
UNION ALL
SELECT 'Email.AdministratorEmailAddress', 'Administrator Email address', 'Logging', 'Email address(es) to send an email whenever an interviewer account is locked out.', 2, 0, NULL
UNION ALL
SELECT 'Email.NotificationEmailBCC', 'Notification email BCC', 'Logging', 'BCC address(es) to send dialer unavailable notification, Dialer WS started notification or notifications about errors during scheduling script execution.', 2, 0, ''
UNION ALL
SELECT 'Email.NotificationExceptionLimit', 'Email notification exception limit', 'Logging', 'Limit of errors which will be detailed in the mail about errors while sample upload with Full scheduling.', 1, 0, '5'
UNION ALL
SELECT 'Logging.TraceVerbose', 'Trace Verbose enabled', 'Logging', 'Switch the logging of Verbose messages on/off.', 3, 0, 'False'
UNION ALL
SELECT 'Logging.EnableReceivingClientErrors', 'Receiving client errors enabled', 'Logging', 'Turn the logging of errors from CATI Console and CATI Monitoring Player on the server on/off.', 3, 0, 'True'
UNION ALL
SELECT 'Dialer.DialerType', 'Dialer type', 'Telephony', 'Type of the Dialer(s) which currently used with the CATI company.', 2, 0, 'NoDialler'
UNION ALL
SELECT 'Dialer.DefaultSurveyParameters', 'Dialer default survey parameters', 'Telephony', 'Set of parameters to configure the way the dialing system handles situations related to dialing routines.', 2, 0, NULL
UNION ALL
SELECT 'Dialer.AudioRecordingsPageSize', 'Audio recordings page size', 'Telephony', 'Size of pages to obtain audio for interviews.', 1, 0, '100'
UNION ALL
SELECT 'Dialer.ServiceCallsRetryLimit', 'Service calls retry limit', 'Telephony', 'Number of attempts which Backend makes to get successful answer from Dialer WS.', 1, 0, '6'
UNION ALL
SELECT 'Dialer.HealthControlStopWaitTime', 'DialerHealthControl waiting time', 'Telephony', 'DialerHealthControl thread waiting time (in ms).', 1, 0, '7000'
UNION ALL
SELECT 'Dialer.HealthControlCheckPeriod', 'DialerHealthControl check period', 'Telephony', 'Dialer Get State interval (in ms).', 1, 0, '60000'
UNION ALL
SELECT 'Dialer.AllCatiServicesAreStartedEstimatedTime', 'All CATI Services are started estimated time', 'Telephony', 'Approximate time required to start all CATI services (in ms).', 1, 0, '300000'
UNION ALL
SELECT 'Dialer.WaitDialerNotificationAtEnableDialerCommandTimeoutInMs', 'Wait dialer notification at EnableDialer command timeout', 'Telephony', 'Period to wait for successful back notification from dialer in response to supervisor EnableDialer command (in ms).', 1, 0, '10000'
UNION ALL
SELECT 'Dialer.HealthControlUnavailableTimeoutInMs', 'DialerHealthControl unavailable timeout', 'Telephony', 'Period to wait for successful response from Dialer (in ms).', 1, 0, '180000'
UNION ALL
SELECT 'Dialer.InterviewerPredictiveSafeBreakWaitTimeout', 'Interviewer predictive safe break wait timeout', 'Telephony', 'The timeout (in ms) is needed to be sure that the call won''t be delivered to interviewer in predictive mode after ''GoNotReady'' was called.', 1, 0, '5000'
UNION ALL
SELECT 'Dialer.DelayForGetAudioRecordsMs', 'Delay for get audio records', 'Telephony', 'Time to wait (in ms) for audio file creation.', 1, 0, '5000'
UNION ALL
SELECT 'Dialer.IgnoreDialerIdFromStationId', 'Ignore Dialer ID from Station ID', 'Telephony', 'Do not use Dialer ID from Station ID. Use automatically selected Dialer ID from survey instead.', 3, 0, 'False'
UNION ALL
SELECT 'Dialer.InboundAudioMessagesJson', 'Inbound audio messages json', 'Telephony', 'Audio messages that are being used during inbound call handling', 2, 0, ''
UNION ALL
SELECT 'Quotas.MaxQuestionsPerQuota', 'Max questions per quota', 'Quotas', 'Maximum number of questions which CATI quota can be based on.', 1, 0, '5'
UNION ALL
SELECT 'QuotaBalancing.TotalPeriodIsSec', 'Promotion period', 'Quotas', 'Total time (in sec) allotted for running promotion procedure for all ''quota balanced'' surveys (opened surveys with the quota chosen for balancing).', 1, 0, '900'
UNION ALL
SELECT 'QuotaBalancing.MaxCellsCount', 'Max cells to promote', 'Quotas', 'Maximal number of cells which can be promoted during one promotion session.', 1, 0, '5'
UNION ALL
SELECT 'QuotaBalancing.MinDelayInSec', 'Min delay between calls of promotion process', 'Quotas', 'Minimal delay (in sec) between calls of promotion process.', 1, 0, '10'
UNION ALL
SELECT 'AppointmentAlert.ShortInterval', 'Short interval for appointment counters', 'Supervisor', 'Short interval for appointment counters (Appointment List from the Activity Views).', 1, 0, '3600'
UNION ALL
SELECT 'AppointmentAlert.LongInterval', 'Long interval for appointment counters', 'Supervisor', 'Long interval for appointment counters (Appointment List from the Activity Views).', 1, 0, '-1'
UNION ALL
SELECT 'AsyncOperation.ActivatePortionSize', 'Asynchronous ''Activate'' portion size', 'Supervisor', 'Portion size for asynchronous activate call operation.', 1, 0, '1000'
UNION ALL
SELECT 'AsyncOperation.AddSamplePortionSize', 'Asynchronous ''Add sample'' portion size', 'Supervisor', 'Portion size for asynchronous Add sample operation.', 1, 0, '10000'
UNION ALL
SELECT 'AsyncOperation.MovePortionSize', 'Asynchronous ''Move'' portion size', 'Supervisor', 'Portion size for asynchronous move call operation.', 1, 0, '1000'
UNION ALL
SELECT 'AsyncOperation.RestoreSurveySqlTimeout', 'Restore survey SQL command timeout', 'Supervisor', 'SQL command timeout which will used inside of ''RestoreSurvey'' operation.', 4, 0, '00.00:15:00'
UNION ALL
SELECT 'AsyncOperation.AsyncOperationCleanTimeoutInHours', 'Async operation clean timeout', 'Supervisor', 'Number of hours after which the asynchronous operation record will be deleted.', 1, 0, '720'
UNION ALL
SELECT 'Replication.BackgroundReplicationSleepPeriod', 'Background replication sleep period', 'Supervisor', 'ReplicationThread interval (in ms).', 1, 0, '60000'
UNION ALL
SELECT 'Replication.ForceReplicationLockTimeout', 'Force replication lock timeout', 'Supervisor', 'Timeout to get an exclusive lock (in ms).', 1, 0, '120000'
UNION ALL
SELECT 'TelephoneBlacklist.TelephoneBlacklistLimit', 'Telephone blacklist limit', 'Supervisor', 'Limit of numbers of phone numbers in the blacklist.', 1, 0, '350000'
UNION ALL
SELECT 'AccountLocking.Enabled', 'Account locking enabled', 'Interviewing', 'Automatic locking of interviewers account functionality is switched on/off.', 3, 0, 'True'
UNION ALL
SELECT 'AccountLocking.MaxFailedLoginAttempts', 'Account locking max failed login attempts', 'Interviewing', 'Number of consecutive unsuccessful login attempts after which the account will be locked automatically.', 1, 0, '3'
UNION ALL
SELECT 'Console.StateServiceSessionTimeoutInMinutes', 'StateService session timeout', 'Interviewing', 'Period (in min) after which StateService sessions expire.', 1, 0, '600'
UNION ALL
SELECT 'Console.KeepAliveInterval', 'Keep alive interval', 'Interviewing', 'Keep alive interval (in ms).', 1, 0, '10'
UNION ALL
SELECT 'Console.ShowRedialButtonSetting', 'Show Redial button setting in Supervisor', 'Supervisor', 'Setting appearance', 3, 0, 'False'
UNION ALL
SELECT 'CacheCalls.InterviewsCountPerPerson', 'Interviews count per person', 'Interviewing', 'Number of calls in cache per interviewer logged into the console (number of calls appear in Active calls view per logged in console person).', 1, 0, '20'
UNION ALL
SELECT 'Console.InterviewsCountShownInManualMode', 'Interviews count shown in manual mode', 'Interviewing', 'Interviews count shown to an interviewer that is logged in CATI Console in manual mode.', 1, 0, '100'
UNION ALL
SELECT 'AutoLogout.AutoLogoutThreadSleepPeriod', 'AutoLogoutThread sleep period', 'Interviewing', 'AutoLogoutThread interval (in ms).', 1, 0, '3600000'
UNION ALL
SELECT 'AutoLogout.AutoLogoutTimeout', 'AutoLogout timeout', 'Interviewing', 'Time (in ms) after which the person will be logged out automatically.', 1, 0, '7200000'
UNION ALL
SELECT 'SchedulingScript.UseDirectDbAccess', 'Use direct database access inside F function', 'Scheduling script', 'Enable usage of direct database access for scheduling script F function instead of usage WebServ.', 3, 0, 'True'
UNION ALL
SELECT 'SchedulingScript.EnableRestrictedMode', 'Use restricted mode', 'Scheduling script', 'Enable restricted mode to check custom code.', 3, 0, 'False'
UNION ALL
SELECT 'SchedulingScript.SecureExternalMethods', 'Secure external methods', 'Scheduling script', 'List of secure methods which can be called from scheduling script assembly.', 2, 0, ''
UNION ALL
SELECT 'SchedulingScript.MaxParameters', 'Maximum  parameters in scheduling script', 'Scheduling script', 'Limit the amount of parameters in scheduling script.', 1, 0, '30'
UNION ALL
SELECT 'RoutineMaintenance.DailyShiftStartTime', 'Daily time of routine maintenance', 'Supervisor', 'The daily time at which the routine maintenance starts.', 4, 0, '0.00:00:00'
UNION ALL
SELECT 'RoutineMaintenance.Duration', 'Duration of routine maintenance', 'Supervisor', 'Routine maintenance duration.', 4, 0, '0.03:00:00'
UNION ALL
SELECT 'RoutineMaintenance.FrequencyExecution', 'Frequency of execution', 'Supervisor', 'Frequency of execution routing maintance operation for each company from default backend instance.', 4, 0, '0.01:00:00'
UNION ALL
SELECT 'RoutineMaintenance.WeeklyShiftDayNumber', 'Number of daily shift', 'Supervisor', 'Days offset from the start of the week when database cleanup starts. Cleanup time based on the DailyShiftStartTime.', 1, 0, '5'
UNION ALL
SELECT 'RoutineMaintenance.MonthlyShiftWeekNumber', 'Number of weekly shift', 'Supervisor', 'Weeks offset from the start of the month when database cleanup starts. Cleanup time based on the DailyShiftStartTime.', 1, 0, '1'
UNION ALL
SELECT 'RoutineMaintenance.Actions.DatabaseMaintenance.ShiftType', 'Database maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
UNION ALL
SELECT 'RoutineMaintenance.Actions.DatabaseMaintenance.IgnoredIndexes', 'List of ignored indexes', 'Supervisor', 'List of ignored system indexes that will not be rebuilded/reorginized.', 2, 0, NULL
UNION ALL
SELECT 'RoutineMaintenance.Actions.DatabaseMaintenance.IndexFragmentationDetectMode', 'Fragmentation detect mode', 'Supervisor', 'Is the name of the mode. mode specifies the scan level that is used to obtain statistics. mode is sysname. Valid inputs are DEFAULT, NULL, LIMITED, SAMPLED, or DETAILED. The default (NULL) is LIMITED.', 2, 0, 'SAMPLED'
UNION ALL
SELECT 'RoutineMaintenance.Actions.DatabaseMaintenance.RebuildIndexShiftType', 'Shift type of rebuild index activity', 'Supervisor', 'Shift type of rebuild index activity.', 1, 0, '2'
UNION ALL
SELECT 'RoutineMaintenance.Actions.DatabaseMaintenance.FragmentationIndexReorganizeThreshold', 'Fragmentation reorganize threshold', 'Supervisor', 'Reorganize index fragmentation threshold. If the index greater than the value of the threshold, we need to reorganize it.', 1, 0, '10'
UNION ALL
SELECT 'RoutineMaintenance.Actions.DatabaseMaintenance.FragmentationIndexRebuildThreshold', 'Fragmentation rebuild threshold', 'Supervisor', 'Rebuild index fragmentation threshold. If the index greater than the value of the threshold, we need to rebuild it.', 1, 0, '30'
UNION ALL
SELECT 'RoutineMaintenance.Actions.DatabaseMaintenance.MinIndexPageCount', 'Minimum index page count', 'Supervisor', 'If count of pages is used in index less than minimum index page count, so index willn''t be rebuilded/reoginized.', 1, 0, '100'
UNION ALL
SELECT 'RoutineMaintenance.Actions.DatabaseMaintenance.UpdateStatisticTables', 'List of tables', 'Supervisor', 'A list of statistics(tables) that we need to update.', 2, 0, 'BvSvySchedule,BvInterview,BvPersonRel'
UNION ALL
SELECT 'RoutineMaintenance.Actions.PersonDeferredMonitoringTableCleanup.ShiftType', 'Person deferred monitoring table cleanup maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
UNION ALL
SELECT 'RoutineMaintenance.Actions.AssignmentResourceTableCleanup.ShiftType', 'Assignment resource table cleanup Maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '2'
UNION ALL
SELECT 'RoutineMaintenance.Actions.PersonDeferredMonitoringTableCleanup.ExpirationPeriod', 'Person deferred monitoring table cleanup expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '30.00:00:00'
UNION ALL
SELECT 'RoutineMaintenance.Actions.PersonDeferredMonitoringTableCleanup.DelayBetweenDeletes', 'Delay between deletes', 'Supervisor', 'Delay (in ms) between deferred records portions deletion.', 4, 0, '0.00:00:00.000'
UNION ALL
SELECT 'RoutineMaintenance.Actions.PersonDeferredMonitoringTableCleanup.DeleteTopRows', 'Delete top rows', 'Supervisor', 'Max number of deferred records which to delete at a time.', 1, 0, '100'
UNION ALL
SELECT 'RoutineMaintenance.Actions.AnswerSubmissionAlertHistoryTableCleanup.ShiftType', 'Answer submission alert history table cleanup maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
UNION ALL
SELECT 'RoutineMaintenance.Actions.AnswerSubmissionAlertHistoryTableCleanup.ExpirationPeriod', 'Answer submission alert history table cleanup expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '30.00:00:00'
UNION ALL
SELECT 'RoutineMaintenance.Actions.AsyncOperationQueueTableCleanup.ShiftType', 'Async operation queue table cleanup maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
UNION ALL
SELECT 'RoutineMaintenance.Actions.AsyncOperationQueueTableCleanup.ExpirationPeriod', 'Async operation queue table cleanup expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '30.00:00:00'
UNION ALL
SELECT 'RoutineMaintenance.Actions.CallsSentToDialerTableCleanup.ShiftType', 'Calls sent to dialer table cleanup maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
UNION ALL
SELECT 'RoutineMaintenance.Actions.CallsSentToDialerTableCleanup.ExpirationPeriod', 'Calls sent to dialer table cleanup expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '30.00:00:00'
UNION ALL
SELECT 'RoutineMaintenance.Actions.PromotionHistoryTableCleanup.ShiftType', 'Promotion history table cleanup maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
UNION ALL
SELECT 'RoutineMaintenance.Actions.PromotionHistoryTableCleanup.ExpirationPeriod', 'Promotion history table cleanup expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '30.00:00:00'
UNION ALL
SELECT 'RoutineMaintenance.Actions.SurveyCleanup.ShiftType', 'Survey cleanup maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '2'
UNION ALL
SELECT 'RoutineMaintenance.Actions.SurveyCleanup.NotificationTimeout', 'Survey cleanup notification timeout', 'Supervisor', 'The time which passes after the warning notification was sent before the survey is really cleaned.', 4, 0, '10.00:00:00'
UNION ALL
SELECT 'RoutineMaintenance.Actions.SurveyCleanup.CleanupTimeout', 'Survey cleanup timeout', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '30.00:00:00'
UNION ALL
SELECT 'RoutineMaintenance.Actions.MessageTableCleanup.ShiftType', 'Message table cleanup maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
UNION ALL
SELECT 'RoutineMaintenance.Actions.MessageTableCleanup.ExpirationPeriod', 'Message table cleanup expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '7.00:00:00'
UNION ALL
SELECT 'RoutineMaintenance.Actions.UserSurveyListTableCleanup.ShiftType', 'User survey list table cleanup maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
UNION ALL
SELECT 'RoutineMaintenance.Actions.UserSurveyListTableCleanup.ExpirationPeriod', 'User survey list table cleanup expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '30.00:00:00'
UNION ALL
SELECT 'CallGroup.Enabled', 'Call group functionality enabled', 'Supervisor', 'Call group functionality is switched on/off.', 3, 0, 'False'
UNION ALL
SELECT 'CallGroup.EnabledForNewSurveys', 'Call group for new surveys enabled', 'Supervisor', 'Default call group value for newly created surveys', 3, 0, 'False'
UNION ALL
SELECT 'Reports.CallHistoryReportEnabled', 'CallHistory report enabled', 'Supervisor', 'Is scheduled call history report enabled?', 3, 0, 'False'
UNION ALL
SELECT 'Reports.CallHistoryReportHour', 'CallHistory report hour', 'Supervisor', 'Hour when scheduled call history report must be sent.', 1, 0, '0'
UNION ALL
SELECT 'Reports.CallHistoryReportRecepients', 'CallHistory report recepients', 'Supervisor', 'Email address(es) to send scheduled call history report.', 2, 0, NULL
UNION ALL
SELECT 'Reports.CallHistoryReportReplicatedVariablesEnabled', 'CallHistory report replicated variables enabled', 'Supervisor', 'Are replicated variables in call history report enabled?', 3, 0, 'False'
UNION ALL
SELECT 'Reports.CallHistoryReportReplicatedVariables', 'CallHistory report replicated variables', 'Supervisor', 'Replicated variables that should be included into report.', 2, 0, NULL
UNION ALL
SELECT 'Reports.CallHistoryReportCallHistoryRowsLimit', 'CallHistory report rows limit', 'Supervisor', 'Limit for call history data rows exported.', 1, 0, '1000000'
UNION ALL
SELECT 'Reports.CallHistoryReportInterviewerBreaksRowsLimit', 'CallHistory report interviewer breaks rows limit', 'Supervisor', 'Limit for interviewer breaks data rows exported.', 1, 0, '100000'
UNION ALL
SELECT 'Reports.CallHistoryReportLoginLogoutEventsRowsLimit', 'CallHistory report login/Logout events row limit', 'Supervisor', 'Limit for login/logout events data rows exported.', 1, 0, '100000'
UNION ALL
SELECT 'Reports.SurveyOverviewReportEnabled', 'Survey overview report enabled', 'Supervisor', 'Is scheduled survey overview report enabled?', 3, 0, 'False'
UNION ALL
SELECT 'Reports.SurveyOverviewReportHour', 'Survey overview report hour', 'Supervisor', 'Hour when scheduled survey overview report must be sent.', 1, 0, '0'
UNION ALL
SELECT 'Reports.SurveyOverviewReportRecepients', 'Survey overview report recepients', 'Supervisor', 'Email address(es) to send scheduled survey overview report.', 2, 0, NULL
UNION ALL
SELECT 'Reports.SurveyProductivityReportEnabled', 'Survey productivity report enabled', 'Supervisor', 'Is scheduled survey productivity report enabled?', 3, 0, 'False'
UNION ALL
SELECT 'Reports.SurveyProductivityReportHour', 'Survey productivity report hour', 'Supervisor', 'Hour when scheduled survey productivity report must be sent.', 1, 0, '0'
UNION ALL
SELECT 'Reports.SurveyProductivityReportRecepients', 'Survey productivity report recepients', 'Supervisor', 'Email address(es) to send scheduled survey productivity report.', 2, 0, NULL
UNION ALL
SELECT 'Reports.InterviewerProductivityReportEnabled', 'Interviewer productivity report enabled', 'Supervisor', 'Is scheduled interviewer productivity report enabled?', 3, 0, 'False'
UNION ALL
SELECT 'Reports.InterviewerProductivityReportHour', 'Interviewer productivity report hour', 'Supervisor', 'Hour when scheduled interviewer productivity report must be sent.', 1, 0, '0'
UNION ALL
SELECT 'Reports.InterviewerProductivityReportRecepients', 'Interviewer productivity report recepients', 'Supervisor', 'Email address(es) to send scheduled interviewer productivity report.', 2, 0, NULL
UNION ALL
SELECT 'Setup.SupervisorVirtualDirectoryName', 'Supervisor virtual directory name', 'Setup', 'Supervisor web application name', 2, 0, 'Supervisor'
UNION ALL
SELECT 'Setup.SupervisorAppPoolName', 'Supervisor application pool name', 'Setup', 'Supervisor application pool name', 2, 0, 'DefaultAppPool'
UNION ALL
SELECT 'Setup.SupervisorSiteName', 'Supervisor site name', 'Setup', 'Supervisor web site name', 2, 0, 'Default Web Site'
UNION ALL
SELECT 'Setup.IsDatabaseLoggingEnabled', 'Database logging enabled', 'Setup', 'Is database logging enabled or not. Possible values: 1 or empty', 2, 0, '1'
UNION ALL
SELECT 'Setup.IsEventlogLoggingEnabled', 'Eventlog logging enabled', 'Setup', 'Is eventlog logging enabled or not. Possible values: 1 or empty', 2, 0, '1'
UNION ALL
SELECT 'Setup.SessionStateMode', 'Session state mode', 'Setup', 'Session state mode. Possible values: InProc, SQLMode, Redis', 2, 0, 'InProc'
UNION ALL
SELECT 'Setup.RedisHostName', 'Redis host name', 'Setup', 'Redis host name. Make sense only if SessionStateMode is Redis', 2, 0, 'localhost'
UNION ALL
SELECT 'Setup.EncryptedSessionStateConnectionString', 'Encrypted session state connection string', 'Setup', 'Encrypted connection string to the session state database (use a special tool to change this setting)', 2, 0, ''
UNION ALL
SELECT 'Setup.SessionStateCookieName', 'Session state cookie name', 'Setup', 'Session state cookie name', 2, 0, 'ConfirmitCati_CookieName'
UNION ALL
SELECT 'Setup.EncryptedConfirmConnectionString', 'Encrypted confirm connection string', 'Setup', 'Encrypted connection string to confirm database (use a special tool to change this setting)', 2, 0, ''
UNION ALL
SELECT 'Setup.EncryptedConfirmlogConnectionString', 'Encrypted confirmlog connection string', 'Setup', 'Encrypted connection string to confirmlog database (use a special tool to change this setting)', 2, 0, ''
UNION ALL
SELECT 'Setup.CertificateType', 'Certificate type', 'Setup', 'Certificate type. Possible values: Test or Real', 2, 0, 'Test'
UNION ALL
SELECT 'Setup.TestCertificateName', 'Test certificate name', 'Setup', 'Test certificate name. Make sense if ''CertificateType'' parameter is Test', 2, 0, 'localhost'
UNION ALL
SELECT 'Setup.CertificatePath', 'Certificate path', 'Setup', 'Path to a certificate file. Make sense if ''CertificateType'' parameter is Real', 2, 0, ''
UNION ALL
SELECT 'Setup.EncryptedCertificatePassword', 'Encrypted certificate password', 'Setup', 'Encrypted password of a real certificate. Make sense if ''CertificateType'' parameter is Real', 2, 0, ''
UNION ALL
SELECT 'Setup.ConfirmitLinkedServerName', 'Confirmit linked server name', 'Setup', 'Confirmit linked server name. This value can be used in update scripts during DB update process', 2, 0, ''
UNION ALL
SELECT 'Setup.InstallLocation', 'Install location', 'Setup', 'A root folder of CATI installation', 2, 0, 'c:\\Program Files\\Confirmit CATI\\'
UNION ALL
SELECT 'Setup.InterviewerConsoleVersion', 'Interviewer Console version', 'Setup', 'Version of Interviewer Console', 2, 0, ''
UNION ALL
SELECT 'Setup.MonitoringConsoleVersion', 'Monitoring Console version', 'Setup', 'Version of Monitoring Console', 2, 0, ''
UNION ALL
SELECT 'Monitoring.LaunchFileAllowedTimeLifeInHours', 'Launch file allowed time life', 'Supervisor', 'Launch file allowed time life in hours.', 1, 0, '2'
UNION ALL
SELECT 'AsyncOperations.MaximumRunningAsyncOperations', 'Maximum running async operations', 'AsyncOperations', 'Maximum running async operations.', 1, 0, '5'
UNION ALL
SELECT 'AsyncOperations.TimeToTreatOperationHangedInMinutes', 'Time to treat operationHanged', 'AsyncOperations', 'Time to treat operationHanged in minutes.', 1, 0, '15'
UNION ALL
SELECT 'AsyncOperations.NumberOfRetries', 'Number of retries', 'AsyncOperations', 'Number Of retries.', 1, 0, '40'
UNION ALL
SELECT 'AsyncOperations.DelayBetweenRetriesInSeconds', 'Delay between retries in Async Operations', 'AsyncOperations', 'Delay between retries in seconds.', 1, 0, '15'
UNION ALL
SELECT 'InterviewerPassword.IsExpirationEnabled', 'CATI interviewer password expiration enabled', 'Supervisor', 'Is CATI interviewer password expiration enabled. Possible values: true or false.', 3, 0, 'False'
UNION ALL
SELECT 'InterviewerPassword.ExpirationPeriodInDays', 'CATI interviewer password expiration period', 'Supervisor', 'CATI interviewer expiration period in days.', 1, 0, '30'
UNION ALL
SELECT 'InterviewerPassword.IsResetToSamePasswordEnabled', 'CATI interviwer allowed to set the same password at change password procedure', 'Supervisor', 'Can CATI interviewer set the same password during password change procedure. Possible values: true or false.', 3, 0, 'False'
UNION ALL
SELECT 'InterviewerPassword.IsMinimumPasswordLengthEnforced', 'CATI interviewer password minimal length enforced', 'Supervisor', 'Minimal allowed length of CATI interviewer password. If 0 then any length is allowed.', 3, 0, 'False'
UNION ALL
SELECT 'InterviewerPassword.MinimumPasswordLength', 'CATI interviewer password minimal length', 'Supervisor', 'Minimal allowed length of CATI interviewer password. This option works only when IsMinimumPasswordLengthEnforced is True.', 1, 0, '6'
UNION ALL
SELECT 'InterviewerPassword.IsComplexPasswordEnforced', 'Complex password rule enforced for CATI interviewer password', 'Supervisor', 'Is complex password rule enforced for CATI interviewer password. Possible values: true or false. Complex password must must have at least 1 upper case character and 1 non-alphanumeric character.', 3, 0, 'False'
UNION ALL
SELECT 'Reports.ReportGenerationTimeout', 'Report generation timeout', 'System', 'This timeout is used for reports.', 1, 0, '120'
UNION ALL
SELECT 'WebApi.Enabled', 'WebApi enabled', 'WebApi', 'Is WebApi enabled for the company', 3, 0, 'False'
UNION ALL
SELECT 'WebApi.PageSize', 'WebApi page size', 'WebApi', 'WebApi page size', 1, 0, '10000'
UNION ALL
SELECT 'QuotaClustering.Enabled', 'Quota clustering functionality enabled', 'QuotaClustering', 'Quota clustering functionality is switched on/off.', 3, 0, 'True'
UNION ALL
SELECT 'FCD.BehaviorType', 'Type of FCD''s algorithm', 'FCD', 'Type of FCD''s algorithm. Following types are allowed: 0-delete calls, 1-disable calls with reenabling on opening cell(s)', 1, 0, '0'
UNION ALL
SELECT 'Setup.IsLoadBalancedEnvironment', 'Load balanced environmen used', 'Setup', 'Is load balanced environment use. Possible values: True or False', 2, 0, 'False'
UNION ALL
SELECT 'Setup.IsNonDisruptiveUpdateModeEnabled', 'Non disruptive update mode enabled', 'Setup', 'Type of installation: non-disruptive or disruptive. Required for load balanced environment. Possible values: True or False', 2, 0, 'True'
UNION ALL
SELECT 'Setup.LoadBalancerIsAlivePageUrl', 'Load balancer is alive page url', 'Setup', 'Url path to IsAlive.htm file. Required for non-disruptive installation in load balanced environment', 2, 0, '/IsAlive.htm'
UNION ALL
SELECT 'Setup.LoadBalancerIsAlivePageRenameTimeout', 'Load balancer is alive page rename timeout', 'Setup', 'Timeout after renaming of IsAlive.htm file', 2, 0, '180'
UNION ALL
SELECT 'MultipleAssignments.Enabled', 'Multiple assignments enabled', 'MultipleAssignments', 'Is multiple assignments enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnablePreviousPageToolbarButton', 'Console enable Previous Page toolbar button', 'Interviewing', 'Is Interviewer Console Previous Page toolbar button enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableNextPageToolbarButton', 'Console enable Next Page toolbar button', 'Interviewing', 'Is Interviewer Console Next Page toolbar button enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableAppointmentToolbarButton', 'Console enable Appointment toolbar button', 'Interviewing', 'Is Interviewer Console Appointment toolbar button enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableRedoToolbarButton', 'Console enable Redo toolbar button', 'Interviewing', 'Is Interviewer Console Redo toolbar button enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableFastForwardToolbarButton', 'Console enable Fast Forward toolbar button', 'Interviewing', 'Is Interviewer Console Fast Forward toolbar button enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableCheckSpellingToolbarButton', 'Console enable Check Spelling toolbar button', 'Interviewing', 'Is Interviewer Console Check Spelling toolbar button enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableRedialToolbarButton', 'Console enable Redial toolbar button', 'Interviewing', 'Is Interviewer Console Redial toolbar button enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableHangUpToolbarButton', 'Console enable Hang Up toolbar button', 'Interviewing', 'Is Interviewer Console Hang Up toolbar button enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableLogoutAfterFinishToolbarButton', 'Console enable Logout After Finish toolbar button', 'Interviewing', 'Is Interviewer Console Logout After Finish toolbar button enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableTerminateToolbarButton', 'Console enable Terminate toolbar button', 'Interviewing', 'Is Interviewer Console Terminate toolbar button enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableTakeBreakToolbarButton', 'Console enable Take Break toolbar button', 'Interviewing', 'Is Interviewer Console Take Break toolbar button enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableChangeTaskChoiceToolbarButton', 'Console enable Change Task Choice toolbar button', 'Interviewing', 'Is Interviewer Console Change Task Choicee toolbar button enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableMessageFormToolbarButton', 'Console enable Message Form toolbar button', 'Interviewing', 'Is Interviewer Console Message Form toolbar button enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableAppointmensListToolbarButton', 'Console enable Appointmens List toolbar button', 'Interviewing', 'Is Interviewer Console Appointmens List toolbar button enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableRefreshToolbarButton', 'Console enable Refresh toolbar button', 'Interviewing', 'Is Interviewer Console Refresh toolbar button enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableLogoutToolbarButton', 'Console enable Logout toolbar button', 'Interviewing', 'Is Interviewer Console Logout toolbar button enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableRedialNewNumberRedialDialogAbility', 'Console enable Redial new number redial dialog ability', 'Interviewing', 'Is Interviewer Console redial dialog redial new number ability enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimes', 'Console enable ability to create appointmens outside of the permitted shift times', 'Interviewing', 'Is Interviewer Console ability to create appointmens outside of the permitted shift simes enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableAbilityToCancelDial', 'Console enable ability to cancel dial', 'Interviewing', 'Is Interviewer Console ability to cancel dialling process enabled', 3, 0, 'False'
UNION ALL
SELECT 'Console.EnablePersistentConnectionClosing', 'Enable persistent connection closing', 'Toggle', 'Enable closing of persistent connection in CATI Console', 3, 0, 'True'
UNION ALL
SELECT 'Console.UseHttpsForConsoleStateService', 'Use HTTPS for ConsoleStateService', 'Toggle', 'Use HTTPS protocol for ConsoleStateService', 3, 0, 'False'
UNION ALL
SELECT 'Console.KeepAliveCallsToSave', 'KeepAlive calls to save', 'Interviewing', 'Number of KeepAlive calls to use when calculating current connection status ', 1, 0, '3'
UNION ALL
SELECT 'Console.GoodConnectionThresholdMs', 'Good connection threshold', 'Interviewing', 'Threshold for good connection status indicator in milliseconds', 1, 0, '300'
UNION ALL
SELECT 'Console.NormalConnectionThresholdMs', 'Normal connection threshold', 'Interviewing', 'Threshold for normal connection status indicator in milliseconds', 1, 0, '1000'
UNION ALL
SELECT 'Toggle.EnableSaveHistoryOptimization', 'Enable save history optimization', 'Toggle', 'Sets if SaveInterviewHistoryAndControlData optimisation enabled', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.EnableSeamlessSurveySwitching', 'Enable seamless survey switching', 'Toggle', 'Enable seamless survey switching', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.EnableReviewer', 'Enable Reviewer', 'Toggle', 'Enable Reviewer', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.EnableTCPA', 'Enable TCPA', 'Toggle', 'Enable TCPA', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.EnableSampleUpdate', 'Enable sample update', 'Toggle', 'Enable Sample update operation', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.EnableIVR', 'Enable IVR', 'Toggle', 'Enable IVR', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.EnableInbound', 'Enable inbound', 'Toggle', 'Enable inbound call functionality', 3, 0, 'False'
UNION ALL
SELECT 'RetryingService.DelayBetweenRetriesInMilliseconds', 'Delay between retries in Retrying Service', 'RetryingService', 'Delay between retries in milliseconds.', 1, 0, '1000'
UNION ALL
SELECT 'RetryingService.NumberOfRetryAttempts', 'Number of retry attempts', 'RetryingService', 'Number of retry attempts', 1, 0, '5'
UNION ALL
SELECT 'CallManagement.MaximumConfirmitVariables', 'Maximum Confirmit variables to select for Call Management', 'Call Management', 'Maximum Confirmit variables to select for Call Management.', 1, 0, '15'
UNION ALL
SELECT 'Security.AlwaysEncryptFiles', 'Always Encrypt Files', 'Security', 'Always use encrypted file transfer', 3, 0, 'False'
UNION ALL
SELECT 'Security.UserForEncryption', 'User for encryption', 'Security', 'User for encryption', 2, 0, ''
UNION ALL
SELECT 'TimeZoneBalancing.EndOfShiftThreshold', 'Time zones balancing end of shift threshold', 'Time zones balancing', 'Time at the end of a shift which controls call delivery algorithm to favour calls for a timezone in which a shift is about to be finshed', 1, 0, '0'
UNION ALL
SELECT 'Ivr.TermChar', 'TerminatingCharacter', 'Ivr', 'The terminating DTMF character for DTMF input recognition', 2, 0, '#'
UNION ALL
SELECT 'Ivr.RecordType', 'RecordType', 'Ivr', 'The media format of the resulting recording', 2, 0, 'audio/x-wav'
UNION ALL
SELECT 'Ivr.Beep', 'UseBeep', 'Ivr', 'If true, a tone is emitted just prior to recording', 3, 0, 'True'
UNION ALL
SELECT 'Ivr.MaxTime', 'MaxTime', 'Ivr', 'The maximum duration to record', 1, 0, '20'
UNION ALL
SELECT 'Ivr.FinalSilence', 'FinalSilence', 'Ivr', 'The interval of silence that indicates end of speech', 1, 0, '10'
UNION ALL
SELECT 'Ivr.DtmfTerm', 'UseDtmfTermination', 'Ivr', 'If true, any DTMF keypress not matched by an active grammar will be treated as a match of an active (anonymous) local DTMF grammar', 3, 0, 'True'
  )
  UPDATE BvSystemSettings
  SET [BvSystemSettings].[Description] = [Data].[Description], [BvSystemSettings].[DisplayName] = [Data].[DisplayName]
  FROM [Data]
  WHERE [BvSystemSettings].[SystemName] = [Data].[SystemName]
END
GO

PRINT N'Update complete.';
GO