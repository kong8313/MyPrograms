/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
/* Data loading */

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__RefactorLog]') AND type in (N'U'))
	DROP TABLE [dbo].[__RefactorLog]
GO


INSERT INTO BvCallCenter(Name, Description, IsDefault, CanBeDeleted, LocalTimezoneId, DialerId ) VALUES( 'Default', 'Default call center', 1, 0, 1, 0 )

PRINT 'Data loading:'
GO
;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
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
SELECT 'Email.FeedbackSupportEmailAddress', 'Support Email for Feedback', 'Logging', 'Support email that is being used in feedback functionality', 2, 0, 'support@confirmit.com'
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
SELECT 'QuotaBalancing.TotalPeriod', 'Promotion period', 'Quotas', 'Total time allotted for running promotion procedure for all ''quota balanced'' surveys (opened surveys with the quota chosen for balancing).', 4, 0, '0.00:15:00.000'
UNION ALL
SELECT 'QuotaBalancing.MaxCellsCount', 'Max cells to promote', 'Quotas', 'Maximal number of cells which can be promoted during one promotion session.', 1, 0, '5'
UNION ALL
SELECT 'QuotaBalancing.MinDelay', 'Min delay between calls of promotion process', 'Quotas', 'Minimal delay between calls of promotion process.', 4, 0, '0.00:00:10.000'
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
SELECT 'AccountLocking.MaxFailedLoginAttemptsForced', 'Account locking max failed login attempts applied by default', 'Interviewing', 'Number of consecutive unsuccessful login attempts after which the account will be locked automatically. This setting is applied only if AccountLocking.Enabled setting is disabled. Otherwise it will be overridden by the value of the AccountLocking.MaxFailedLoginAttempts setting.', 1, 0, '100'
UNION ALL
SELECT 'Console.StateServiceSessionTimeoutInMinutes', 'StateService session timeout', 'Interviewing', 'Period (in min) after which StateService sessions expire.', 1, 0, '600'
UNION ALL
SELECT 'Console.KeepAliveInterval', 'Keep alive interval', 'Interviewing', 'Keep alive interval (in ms).', 1, 0, '10'
UNION ALL
SELECT 'Console.ShowRedialButtonSetting', 'Show Redial button setting in Supervisor', 'Supervisor', 'Setting appearance', 3, 0, 'False'
UNION ALL
SELECT 'Console.ForceUpdateToNewVersion', 'Force update to new version', 'Interviewing', 'This setting controls when the interviewer console is updated to a new version. If it is set true then it must be before a new interview can be started. If it is set false then it must be before the interviewer console application can be launched.', 3, 0, 'False'
UNION ALL
SELECT 'CacheCalls.InterviewsCountPerPerson', 'Interviews count per person', 'Interviewing', 'Number of calls in cache per interviewer logged into the console (number of calls appear in Active calls view per logged in console person).', 1, 0, '20'
UNION ALL
SELECT 'Console.InterviewsCountShownInManualMode', 'Interviews count shown in manual mode', 'Interviewing', 'Interviews count shown to an interviewer that is logged in CATI Console in manual mode.', 1, 0, '100'
UNION ALL
SELECT 'AutoLogout.AutoLogoutThreadSleepPeriod', 'AutoLogoutThread sleep period', 'Interviewing', 'AutoLogoutThread interval (in ms).', 1, 0, '3600000'
UNION ALL
SELECT 'AutoLogout.AutoLogoutTimeout', 'AutoLogout timeout', 'Interviewing', 'Time (in ms) after which the person will be logged out automatically.', 1, 0, '7200000'
UNION ALL
SELECT 'AutoLogout.AutoLogoutWebConsoleThreadSleepPeriod', 'AutoLogout WebConsole Thread sleep period', 'Interviewing', 'Time interval between executions of a procedure that logs out interviewers using Browser-based CATI Console (BBCC) who lost a connection.', 4, 0, '0.00:01:00'
UNION ALL
SELECT 'AutoLogout.AutoLogoutWebConsoleTimeout', 'AutoLogout WebConsole timeout', 'Interviewing', 'Time interval to keep interviewer using the Browser-based CATI Console (BBCC) logged-in after losing the connection to the server. Interviewer will be automatically logged out after this interval.', 4, 0, '0.00:05:00'
UNION ALL
SELECT 'SchedulingScript.UseDirectDbAccess', 'Use direct database access inside F function', 'Scheduling script', 'Enable usage of direct database access for scheduling script F function instead of usage WebServ.', 3, 0, 'True'
UNION ALL
SELECT 'SchedulingScript.EnableRestrictedMode', 'Use restricted mode', 'Scheduling script', 'Enable restricted mode to check custom code.', 3, 0, 'False'
UNION ALL
SELECT 'SchedulingScript.SecureExternalMethods', 'Secure external methods', 'Scheduling script', 'List of secure methods which can be called from scheduling script assembly.', 2, 0, ''
UNION ALL
SELECT 'SchedulingScript.MaxParameters', 'Maximum  parameters in scheduling script', 'Scheduling script', 'Limit the amount of parameters in scheduling script.', 1, 0, '30'
UNION ALL
SELECT 'SchedulingScript.MaxActionsToExecute', 'Maximum amount of actions that will be executed in scheduling script', 'Scheduling script', 'Maximum amount of actions in scheduling script that can be executed without raising an error.', 1, 0, '1000'
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
SELECT 'RoutineMaintenance.Actions.CallHistoryTableCleanup.ShiftType', 'Call history table cleanup maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '2'
UNION ALL
SELECT 'RoutineMaintenance.Actions.CallHistoryTableCleanup.ExpirationPeriod', 'Call history table cleanup expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '365.00:00:00'
UNION ALL
SELECT 'RoutineMaintenance.Actions.ServiceBrokerObjectsCleanup.ShiftType', 'Unused service broker objects cleanup shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
UNION ALL
SELECT 'RoutineMaintenance.Actions.ServiceBrokerObjectsCleanup.ExpirationPeriod', 'Unused service broker objects cleanup expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '1.00:00:00'
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
SELECT 'Reports.ScheduledInterviewerProductivityReportTemplateId', 'Interviewer productivity report template id to use for scheduled email report', 'Supervisor', 'Interviewer productivity report template id to use for scheduled email report', 1, 0, '0'
UNION ALL
SELECT 'Supervisor.SurveyList.ShowTciDialerCampaignIdColumn', 'Show TCI dialer campaign Id column', 'Supervisor', 'Display Campaign ID column in survey list that is used as a Survey ID by the TCI dialer.', 3, 0, 'False'
UNION ALL
SELECT 'Setup.SupervisorVirtualDirectoryName', 'Supervisor virtual directory name', 'Setup', 'Supervisor web application name', 2, 0, 'Supervisor'
UNION ALL
SELECT 'Setup.SupervisorAppPoolName', 'Supervisor application pool name', 'Setup', 'Supervisor application pool name', 2, 0, 'DefaultAppPool'
UNION ALL
SELECT 'Setup.SupervisorSiteName', 'Supervisor site name', 'Setup', 'Supervisor web site name', 2, 0, 'Default Web Site'
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
SELECT 'Setup.BackendVersion', 'Backend and old Supervisor version', 'Setup', 'Version of Backend and old Supervisor', 2, 0, ''
UNION ALL
SELECT 'Setup.ReleaseNumber', 'Octopus release number', 'Setup', 'Number of the latest octopus release', 2, 0, ''
UNION ALL
SELECT 'Setup.ReleaseDate', 'Octopus release date', 'Setup', 'Date of the latest octopus release', 2, 0, ''
UNION ALL
SELECT 'Setup.InterviewerAPIVersion', 'CATI Interviewer API version', 'Setup', 'Version of the CATI Interviewer API', 2, 0, ''
UNION ALL
SELECT 'Setup.BBCCVersion', 'Browser Based CATI Console version', 'Setup', 'Version of the Browser Based CATI Console', 2, 0, ''
UNION ALL
SELECT 'Monitoring.LaunchFileAllowedTimeLifeInHours', 'Launch file allowed time life', 'Supervisor', 'Launch file allowed time life in hours.', 1, 0, '2'
UNION ALL
SELECT 'Monitoring.AllowCoachingMode', 'Allow coaching mode during live monitoring', 'Supervisor', 'If true a coaching mode buttons will be shown during live monitoring. Possible values: true or false.', 3, 0, 'True'
UNION ALL
SELECT 'Monitoring.AllowBargingMode', 'Allow barging mode during live monitoring', 'Supervisor', 'If true a barging mode buttons will be shown during live monitoring. Possible values: true or false.', 3, 0, 'True'
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
SELECT 'InterviewerProperties.AttributesList', 'List of additional interviewer attributes', 'Supervisor', 'Enter up to five additional attribute names for an interviewer, separated by commas or semicolons. When you add attributes, the interviewer properties area will show one field for each attribute (up to five). If you leave this setting empty, no additional attributes section will appear.', 2, 0, ''
UNION ALL
SELECT 'Reports.ReportGenerationTimeout', 'Report generation timeout', 'System', 'This timeout is used for reports.', 1, 0, '120'
UNION ALL
SELECT 'WebApi.PageSize', 'WebApi page size', 'WebApi', 'WebApi page size', 1, 0, '10000'
UNION ALL
SELECT 'WebApi.EnableSwagger', 'Enable swagger for WebApi', 'WebApi', 'Enable swagger for CATI REST API', 3, 0, 'False'
UNION ALL
SELECT 'WebApi.RateLimiting', 'Enable IP rate limiting for CATI REST API', 'WebApi', 'A single IP address may make up to 20 requests per second, 1,000 requests per 15 minutes, and 10,000 requests per 12 hours', 3, 0, 'True'
UNION ALL
SELECT 'QuotaClustering.Enabled', 'Quota clustering functionality enabled', 'QuotaClustering', 'Quota clustering functionality is switched on/off.', 3, 0, 'True'
UNION ALL
SELECT 'FCD.BehaviorType', 'Type of FCD''s algorithm', 'FCD', 'Type of FCD''s algorithm. Following types are allowed: 0-delete calls, 1-disable calls with reenabling on opening cell(s)', 1, 0, '1'
UNION ALL
SELECT 'FCD.InterviewQuotaCellsTransactionThreshold', 'Transaction threshold for importing FCD quotas', 'FCD', 'Defines the maximum number of interview-to-quota mappings to import quotas in a single transaction. If (interviews) * (FCD quotas) in a survey exceeds threshold, import runs without transaction', 1, 0, '5000000'
UNION ALL
SELECT 'Setup.IsLoadBalancedEnvironment', 'Load balanced environmen used', 'Setup', 'Is load balanced environment use. Possible values: True or False', 2, 0, 'False'
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
SELECT 'Console.KeepAliveCallsToSave', 'KeepAlive calls to save', 'Interviewing', 'Number of KeepAlive calls to use when calculating current connection status ', 1, 0, '3'
UNION ALL
SELECT 'Console.GoodConnectionThresholdMs', 'Good connection threshold', 'Interviewing', 'Threshold for good connection status indicator in milliseconds', 1, 0, '300'
UNION ALL
SELECT 'Console.NormalConnectionThresholdMs', 'Normal connection threshold', 'Interviewing', 'Threshold for normal connection status indicator in milliseconds', 1, 0, '1000'
UNION ALL
SELECT 'Console.NoCallsTimeout', 'No_Calls timeout in seconds', 'Interviewing', 'During this timeout console will wait interview in No_Calls state', 1, 0, '60'
UNION ALL
SELECT 'Console.EnableInternalCallTransferToolbarButton', 'Console enable ability to do internal call transfer', 'Interviewing', 'Is Interviewer Console able to do internal call transfer', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableExternalCallTransferToolbarButton', 'Console enable ability to do external call transfer', 'Interviewing', 'Is Interviewer Console able to do external call transfer', 3, 0, 'True'
UNION ALL
SELECT 'Console.AllowTransferToAssignedSurveysOnly', 'Restrict internal call transfers only to groups which have surveys assigned explicitly', 'Interviewing', 'Restrict internal call transfers only to groups which have surveys assigned explicitly', 3, 0, 'False'
UNION ALL
SELECT 'Console.LinkedInterviewsLimit', 'Limit of linked interviews', 'Interviewing', 'The number of interviews for each survey returned by the GetCatiInterviews survey scripting function', 1, 0, '5'
UNION ALL
SELECT 'Toggle.EnableSeamlessSurveySwitching', 'Enable seamless survey switching', 'Toggle', 'Enable seamless survey switching', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.EnableTCPA', 'Enable TCPA', 'Toggle', 'Enable TCPA', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.EnableIVR', 'Enable IVR', 'Toggle', 'Enable IVR', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.BBCC.EnableCallTransfer', 'Enable BBCC Call Transfer', 'Toggle', 'Enable Call transfer for BBCC Consoles', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.BBCC.EnableScriptErrorsLogging', 'Enable BBCC logging for script errrors', 'Toggle', 'Enable logging of uncaught JavaScript exceptions for BBCC', 3, 0, 'True'
UNION ALL
SELECT 'Toggle.BBCC.Messaging', 'Enable messaging in BBCC', 'Toggle', 'Enable all messaging functionality in BBCC', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.BBCC.TwoWayMessaging', 'Enable 2-way messaging in BBCC', 'Toggle', 'Enable 2-way messaging functionality in BBCC', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.EnableInbound', 'Enable inbound', 'Toggle', 'Enable inbound call functionality', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.EnableInternalTransfer', 'Enable internal transfer', 'Toggle', 'Enable internal call transfer functionality', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.EnableExternalTransfer', 'Enable external transfer', 'Toggle', 'Enable external call transfer functionality', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.EnableMonitoringCoachingMode', 'Enable monitoring coaching mode ', 'Toggle', 'Enable coaching mode for live monitoring', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.EnableMonitoringBargingMode', 'Enable monitoring barging mode', 'Toggle', 'Enable barging mode for live monitoring', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.EnableDesktopConsoleLogin', 'Enable login to desktop console', 'Toggle', 'Enable login to desktop console and show link to download page with it', 3, 0, 'True'
UNION ALL
SELECT 'Toggle.EnableAlertsConfiguration', 'Enable alerts configuration', 'Toggle', 'Enable alerts configuration in CATI supervisor', 3, 0, 'False'
UNION ALL
SELECT 'RetryingService.DelayBetweenRetriesInMilliseconds', 'Delay between retries in Retrying Service', 'RetryingService', 'Delay between retries in milliseconds.', 1, 0, '1000'
UNION ALL
SELECT 'RetryingService.NumberOfRetryAttempts', 'Number of retry attempts', 'RetryingService', 'Number of retry attempts', 1, 0, '5'
UNION ALL
SELECT 'CallManagement.MaximumConfirmitVariables', 'Maximum Confirmit variables to select for Call Management', 'Call Management', 'Maximum Confirmit variables to select for Call Management.', 1, 0, '50'
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
UNION ALL
SELECT 'Ivr.TransferTimeout', 'Transfer timeout', 'Ivr', 'The timeout value (specified in seconds) determines how long the system will wait when attempting to transfer from IVR to a live agent. If the transfer to a live agent is not completed within the given timeout period it will be returned to IVR.', 4, 0, '0.00:00:30'
UNION ALL
SELECT 'Supervisor.AlwaysOpenNewUI', 'Open new UI by default', 'Supervisor', 'Indicates whether CATI Supervisor should always be loaded in a new style on launch.', 3, 0, 'True'
UNION ALL
SELECT 'Supervisor.SurveysListOldStyleEnabled', 'Show surveys list in old style', 'Supervisor', 'Indicates whether CATI Supervisor should show surveys list in old style.', 3, 0, 'True'
UNION ALL
SELECT 'Dialer.SettingsTemplatesJson', 'DialerSettingsTemplatesJson', 'Telephony', 'Dialer settings templates that are used to create dialer instances', 2, 0,
'{"DialerSettingTemplates": [{"Name": "Sytel (Open Dialer API)","DialerType": "Generic","DialerConnectionParameters": [{"Id": "ServiceAddress","Name": "Service Address","Type": "System.String","Value": "https://localhost/DialerService/DialerService.svc"},{"Id": "ServiceEndpoint","Name": "Service Endpoint","Type": "System.String","Value": "DialerServiceEndpointHttps"},{"Id": "AuthorizationKeyForOutgoingRequests","Name": "Authorization Key For Outgoing Requests","Type": "System.String","Value": "0275E046-7FFF-495B-ACFE-09B439DB4902"}],"DialerConfigurationParameters": [{"Id": "SupportedPersonModes","Name": "Supported Person Modes","Type": "System.String","Value": "Manual,CampaignAssignment"},{"Id": "IsReloginNeededOnCampaignChange","Name": "Is Relogin Needed On Campaign Change","Type": "System.Boolean","Value": "True"},{"Id": "IsHangUpSupported","Name": "Is HangUp Supported","Type": "System.Boolean","Value": "True"},{"Id": "IsPauseOrResumePlaybackSupported","Name": "Is Pause Or Resume Playback Supported","Type": "System.Boolean","Value": "False"},{"Id": "IsToggleAgentListensToPlaybackOrRespondentSupported","Name": "Is Toggle Agent Listens To Playback Or Respondent Supported","Type": "System.Boolean","Value": "False"},{"Id": "IsDynamicExtensionNumberAllowedForLocalAgents","Name": "Is Dynamic Extension Number Allowed For Local Agents","Type": "System.Boolean","Value": "False"},{"Id": "IsDynamicExtensionNumberAllowedForRemoteAgents","Name": "Is Dynamic Extension Number Allowed For Remote Agents","Type": "System.Boolean","Value": "False"}],"DialerSurveyParameters": [{"Id": "AbandonRate","Name": "Abandon Call Target Percentage Rate","Value": "1.0","Type": "System.String","Visible": "True"},{"Id": "RNAtimeout","Name": "Ring No Answer Timeout in seconds","Type": "System.Int32","Value": "20","Visible": "True"},{"Id": "AnsMachineDetect","Name": "Answer Machine Detection","Value": "True","Type": "System.Boolean","Visible": "True"},{"Id": "CallProgressToneDetection","Name": "Call Progress Tone Detection","Value": "True","Type": "System.Boolean","Visible": "True"},{"Id": "AbandonMessageName","Name": "Abandon Message Name","Value": "ABANDON","Type": "System.String","Visible": "True"},{"Id": "CTIName","Name": "CTI Name is an optional parameter to specify the default CTI name","Type": "System.String","Visible": "True"},{"Id": "CLI","Name": "Calling Line Identity value can be \"allowed\", \"blocked\", or a number to display.","Value": "allowed","Type": "System.String","Visible": "True"},{"Id": "AnsMachineAudioMessageUrl","Name": "Answer Machine Audio Message URL","Value": "","Type": "System.String","Visible": "True"}]},{"Name": "TCI","DialerType": "BvTCI","DialerConnectionParameters": [{"Id": "HostNameOrIp","Name": "Host Name or IP Address","Type": "System.String","Value": ""},{"Id": "TcpPort","Name": "TCP Port","Type": "System.Int32","Value": ""},{"Id": "ServiceAddress","Name": "Service Address","Type": "System.String","Value": "http://localhost/TciDialerService/BvTciDialer.svc"},{"Id": "ServiceEndpoint","Name": "Service Endpoint","Type": "System.String","Value": "BvTciDialerServiceEndpoint"}],"DialerConfigurationParameters": [],"DialerSurveyParameters": [{"Id": "MaxRings","Name": "No reply timeout (secs)","Type": "System.Int32","Value": "45"},{"Id": "TelephoneNumberPrefix","Name": "Telephone number prefix","Type": "System.String","Value": ""}]},{"Name": "PROTS","DialerType": "PROTS","DialerConnectionParameters": [{"Id": "HostNameOrIp","Name": "Host Name or IP Address","Type": "System.String","Value": ""},{"Id": "OutgoingTcpPort","Name": "Outgoing TCP Port","Type": "System.Int32","Value": "1810"},{"Id": "IncomingTcpPort","Name": "Incoming TCP Port","Type": "System.Int32","Value": "1811"},{"Id": "ServiceAddress","Name": "Service Address","Type": "System.String","Value": "http://localhost/ProtsDialerService/ProtsDialerService.svc"},{"Id": "ServiceEndpoint","Name": "Service Endpoint","Type": "System.String","Value": "PROTSDialerServiceEndpoint"},{"Id": "OperationsTimeout","Name": "Operations Timeout","Type": "System.Int32","Value": "7000"}],"DialerConfigurationParameters": [{"Id": "RootDirectoryForAudioRecords","Name": "Root Directory For Audio Records","Type": "System.String","Value": "C:\\DSM"}],"DialerSurveyParameters": [{"Id": "AbandonmentRate","Name": "Nuisance call abandonment rate","Type": "System.Int32","Value": "0"},{"Id": "MaxRings","Name": "No reply timeout (seconds)","Type": "System.Int32","Value": "5"},{"Id": "AnsMachineDetect","Name": "Enable answer phone detection","Type": "System.Boolean","Value": "False"},{"Id": "BillingCode","Name": "Billing Code","Type": "System.Int32","Value": "0"}]},{"Name": "Invade (Open Dialer API)","DialerType": "Generic","DialerConnectionParameters":[{"Id": "ServiceAddress","Name": "Service Address","Type": "System.String","Value": "https://localhost/DialerService/DialerService.svc"},{"Id": "ServiceEndpoint","Name": "Service Endpoint","Type": "System.String","Value": "DialerServiceEndpointHttps"},{"Id": "AuthorizationKeyForOutgoingRequests","Name": "Authorization Key For Outgoing Requests","Type": "System.String","Value": "0275E046-7FFF-495B-ACFE-09B439DB4902"}],"DialerConfigurationParameters":[{"Id": "SupportedPersonModes","Name": "Supported Person Modes","Type": "System.String","Value": "Manual,CampaignAssignment"},{"Id": "IsReloginNeededOnCampaignChange","Name": "Is Relogin Needed On Campaign Change","Type": "System.Boolean","Value": "True"},{"Id": "IsHangUpSupported","Name": "Is HangUp Supported","Type": "System.Boolean","Value": "True"},{"Id": "IsPauseOrResumePlaybackSupported","Name": "Is Pause Or Resume Playback Supported","Type": "System.Boolean","Value": "False"},{"Id": "IsToggleAgentListensToPlaybackOrRespondentSupported","Name": "Is Toggle Agent Listens To Playback Or Respondent Supported","Type": "System.Boolean","Value": "False"},{"Id": "IsDynamicExtensionNumberAllowedForLocalAgents","Name": "Is Dynamic Extension Number Allowed For Local Agents","Type": "System.Boolean","Value": "False"},{"Id": "IsDynamicExtensionNumberAllowedForRemoteAgents","Name": "Is Dynamic Extension Number Allowed For Remote Agents","Type": "System.Boolean","Value": "False"}],"DialerSurveyParameters": [{"Id": "AbandonmentRate","Name": "Nuisance call abandonment rate","Value": "0","Type": "System.Int32","Visible": "True"},{"Id": "MaxRings","Name": "No reply timeout (seconds)","Type": "System.Int32","Value": "5","Visible": "True"},{"Id": "AnsMachineDetect","Name": "Enable answer phone detection","Value": "False","Type": "System.Boolean","Visible": "True"},{"Id": "BillingCode","Name": "Billing Code","Value": "0","Type": "System.Int32","Visible": "True"}]},{"Name": "Simulator (Open Dialer API)","DialerType": "Generic","DialerConnectionParameters": [{"Id": "ServiceAddress","Name": "Service Address","Type": "System.String","Value": "https://localhost/DialerService/DialerService.svc"},{"Id": "ServiceEndpoint","Name": "Service Endpoint","Type": "System.String","Value": "DialerServiceEndpointHttps"},{"Id": "AuthorizationKeyForOutgoingRequests","Name": "Authorization Key For Outgoing Requests","Type": "System.String","Value": "0275E046-7FFF-495B-ACFE-09B439DB4902"}],"DialerConfigurationParameters": [{"Id": "SupportedPersonModes","Name": "Supported Person Modes","Type": "System.String","Value": "Manual,CampaignAssignment"},{"Id": "IsReloginNeededOnCampaignChange","Name": "Is Relogin Needed On Campaign Change","Type": "System.Boolean","Value": "True"},{"Id": "IsHangUpSupported","Name": "Is HangUp Supported","Type": "System.Boolean","Value": "True"},{"Id": "IsPauseOrResumePlaybackSupported","Name": "Is Pause Or Resume Playback Supported","Type": "System.Boolean","Value": "False"},{"Id": "IsToggleAgentListensToPlaybackOrRespondentSupported","Name": "Is Toggle Agent Listens To Playback Or Respondent Supported","Type": "System.Boolean","Value": "False"},{"Id": "IsDynamicExtensionNumberAllowedForLocalAgents","Name": "Is Dynamic Extension Number Allowed For Local Agents","Type": "System.Boolean","Value": "False"},{"Id": "IsDynamicExtensionNumberAllowedForRemoteAgents","Name": "Is Dynamic Extension Number Allowed For Remote Agents","Type": "System.Boolean","Value": "False"}],"DialerSurveyParameters": [{"Id": "AbandonRate","Name": "Abandon Call Target Percentage Rate","Value": "1.0","Type": "System.String","Visible": "True"},{"Id": "RNAtimeout","Name": "Ring No Answer Timeout in seconds","Type": "System.Int32","Value": "20","Visible": "True"},{"Id": "AnsMachineDetect","Name": "Answer Machine Detection","Value": "True","Type": "System.Boolean","Visible": "True"},{"Id": "CallProgressToneDetection","Name": "Call Progress Tone Detection","Value": "True","Type": "System.Boolean","Visible": "True"},{"Id": "AbandonMessageName","Name": "Abandon Message Name","Value": "ABANDON","Type": "System.String","Visible": "True"},{"Id": "CTIName","Name": "CTI Name is an optional parameter to specify the default CTI name","Type": "System.String","Visible": "True"},{"Id": "CLI","Name": "Calling Line Identity value can be \"allowed\", \"blocked\", or a number to display.","Value": "allowed","Type": "System.String","Visible": "True"},{"Id": "AnsMachineAudioMessageUrl","Name": "Answer Machine Audio Message URL","Value": "","Type": "System.String","Visible": "True"}]},{"Name": "AmazonConnect (Open Dialer API)","DialerType": "Generic","DialerConnectionParameters": [{"Id": "ServiceAddress","Name": "Service Address","Type": "System.String","Value": "http://aws-connect-dialer-proxy/DialerService.svc#1/1"},{"Id": "ServiceEndpoint","Name": "Service Endpoint","Type": "System.String","Value": "DialerServiceEndpointHttps"},{"Id": "AuthorizationKeyForOutgoingRequests","Name": "Authorization Key For Outgoing Requests","Type": "System.String","Value": "0275E046-7FFF-495B-ACFE-09B439DB4902"}],"DialerConfigurationParameters": [{"Id": "AwsAccessKey","Name": "AWS AccessKey","Type": "System.String","Value": ""},{"Id": "AwsSecretKey","Name": "AWS SecretKey","Type": "System.String","Value": ""},{"Id": "AwsRegion","Name": "AWS Region","Type": "System.String","Value": ""},{"Id": "AwsPublicApiUrl","Name": "AWS public API url","Type": "System.String","Value": ""},{"Id": "AwsConnectId","Name": "AWS Connect instance ID","Type": "System.String","Value": ""},{"Id": "AwsContactFlowId","Name": "AWS Connect outbount contact flow ID","Type": "System.String","Value": ""},{"Id": "AwsCallStatusQueueUrl","Name": "AWS call status SQS URL","Type": "System.String","Value": ""},{"Id": "SupportedPersonModes","Name": "Supported Person Modes","Type": "System.String","Value": "Automatic,Manual,CampaignAssignment"},{"Id": "IsReloginNeededOnCampaignChange","Name": "Is Relogin Needed On Campaign Change","Type": "System.Boolean","Value": "False"},{"Id": "IsHangUpSupported","Name": "Is HangUp Supported","Type": "System.Boolean","Value": "True"},{"Id": "IsPauseOrResumePlaybackSupported","Name": "Is Pause Or Resume Playback Supported","Type": "System.Boolean","Value": "False"},{"Id": "IsToggleAgentListensToPlaybackOrRespondentSupported","Name": "Is Toggle Agent Listens To Playback Or Respondent Supported","Type": "System.Boolean","Value": "False"},{"Id": "IsDynamicExtensionNumberAllowedForLocalAgents","Name": "Is Dynamic Extension Number Allowed For Local Agents","Type": "System.Boolean","Value": "False"},{"Id": "IsDynamicExtensionNumberAllowedForRemoteAgents","Name": "Is Dynamic Extension Number Allowed For Remote Agents","Type": "System.Boolean","Value": "False"}],"DialerSurveyParameters": [{"Id": "SourcePhoneNumber","Name": "AWS survey phone number","Value": "","Type": "System.String","Visible": "True"},{"Id": "CallerID","Name": "CallerID","Value": "","Type": "System.String","Visible": "True"},{"Id": "AnsMachineDetect","Name": "Answer Machine Detection","Value": "False","Type": "System.Boolean","Visible": "True"}]}]}'
UNION ALL
SELECT 'CallManagement.ExportCallsLimit', 'Export calls limit', 'Call Management', 'Upper limit of amount of calls that user can export on Call Management page', 1, 0, '10000'
UNION ALL
SELECT 'Toggle.EnableInboundForPreviewInPredictiveMode', 'Enable inbound for preview in predictive mode', 'Toggle', 'Enable inbound for preview in predictive mode', 3, 0, 'False'
UNION ALL
SELECT 'Surveys.DefaultCallDeliveryMode', 'Default call delivery mode', 'Surveys', 'Default call delivery mode for new surveys. Following types are allowed: 0-order by ID (lowest first), 1-random order', 1, 0, '0'
UNION ALL
SELECT 'Console.EnableLogoutFromErrorAndWaitingScreen', 'Console enable ability to log out from ''error'' and/or ''waiting'' screen', 'Interviewing', 'Is Interviewer Console ability to log out from ''error'' and/or ''waiting'' screen enabled', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableTwoWayMessaging', 'Allow interviewers and supervisors to perform 2-way messaging', 'Interviewing', 'Allow interviewers and supervisors to perform 2-way messaging', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableSoftphoneIntegration', 'Enable softphone integration for BBCC', 'Interviewing', 'Enable softphone integration for BBCC', 3, 0, 'False'
UNION ALL
SELECT 'Server.CreateCompanyDatabasesFromBackup', 'Create CATI databases from backup', 'System', ' When enabled, new CATI databases for companies will be created from backup of main CATI database. Otherwise - using database deploy. Should be ''False'' for Azure Managed SQL', 3, 0, 'True'
UNION ALL
SELECT 'Supervisor.ShowClassicStyleButton', 'Show classic style setting button in supervisor', 'Supervisor', 'Show classic style setting button in CATI Supervisor', 3, 0, 'True'
UNION ALL
SELECT 'Console.CompanyLogoUrl', 'URL for company logo', 'Interviewing', 'URL or data: URL for company logo', 2, 0, ''
UNION ALL
SELECT 'Console.BBCC.OptimisticConcurrency', 'Optimistic concurrency for interviewer state', 'Interviewing', 'Controls the way how transactions updating interviewer state are synchronized. When enabled - database locks are not used and transaction is automatically retried if there is a conflict. When disabled - exclusive database lock is placed on interviewer state.', 3, 0, 'False'
UNION ALL
SELECT 'SchedulingScript.ErrorLogSize', 'Size of Log Table for scheduling script errors', 'Scheduling script', 'Limit for amount of rows in BvScheduleError table', 1, 0, '100'
UNION ALL
SELECT 'Toggle.CatiAgent.AutoLogoutThread', 'Run AutoLogoutThread in CatiAgent', 'Toggle', 'Run AutoLogoutThread in CatiAgent', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.CatiAgent.AutoLogoutWebConsoleThread', 'Run AutoLogoutWebConsoleThread in CatiAgent', 'Toggle', 'Run AutoLogoutWebConsoleThread in CatiAgent', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.CatiAgent.DialerHealthControlThread', 'Run DialerHealthControlThread in CatiAgent', 'Toggle', 'Run DialerHealthControlThread in CatiAgent', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.CatiAgent.EmailReportsThread', 'Run EmailReportsThread in CatiAgent', 'Toggle', 'Run EmailReportsThread in CatiAgent', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.CatiAgent.ExpiredCallsThread', 'Run ExpiredCallsThread in CatiAgent', 'Toggle', 'Run ExpiredCallsThread in CatiAgent', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.CatiAgent.IvrThread', 'Run IvrThread in CatiAgent', 'Toggle', 'Run IvrThread in CatiAgent', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.CatiAgent.ReplicationThread', 'Run ReplicationThread in CatiAgent', 'Toggle', 'Run ReplicationThread in CatiAgent', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.CatiAgent.ReviewerUpdateReviewStatusThread', 'Run ReviewerUpdateReviewStatusThread in CatiAgent', 'Toggle', 'Run ReviewerUpdateReviewStatusThread in CatiAgent', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.CatiAgent.RoutineMaintenanceThread', 'Run RoutineMaintenanceThread in CatiAgent', 'Toggle', 'Run RoutineMaintenanceThread in CatiAgent', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.CatiAgent.ScheduleThread', 'Run ScheduleThread in CatiAgent', 'Toggle', 'Run ScheduleThread in CatiAgent', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.CatiAgent.ScheduleErrorsNotificationThread', 'Run ScheduleErrorsNotificationThread in CatiAgent', 'Toggle', 'Run ScheduleErrorsNotificationThread in CatiAgent', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.CatiAgent.AsyncOperationSchedulerThread', 'Run AsyncOperationSchedulerThread in CatiAgent', 'Toggle', 'Run AsyncOperationSchedulerThread in CatiAgent', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.CatiAgent.AsyncOperationsHeartBeatUpdaterThread', 'Run AsyncOperationsHeartBeatUpdaterThread in CatiAgent', 'Toggle', 'Run AsyncOperationsHeartBeatUpdaterThread in CatiAgent', 3, 0, 'False'
UNION ALL
SELECT 'RoutineMaintenance.Actions.SchedulingScriptLogTableCleanup.ShiftType', 'Scheduling script execution log table cleanup maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
UNION ALL
SELECT 'RoutineMaintenance.Actions.SchedulingScriptLogTableCleanup.ExpirationPeriod', 'Scheduling script execution log table cleanup expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '60.00:00:00'
UNION ALL
SELECT 'Toggle.UseReactSurveyList', 'Use React based survey list in supervisor', 'Toggle', 'Use React based survey list in supervisor', 3, 0, 'False'
UNION ALL
SELECT 'RecordedInterviews.MaxSaved', 'Set the maximum number of recorded interviews to be marked for retention', 'Supervisor', 'Number of recorded interviews that can be marked for retention', 1, 0, '100'
UNION ALL
SELECT 'Console.IncludeOpenEndReviewTimeInInterviewDuration', 'Enable including open end review time in interview duration', 'Console', 'This setting enables including open end review time in interview duration in all reports and data exports, for all interviews completed after this setting is enabled.', 3, 0, 'False'
UNION ALL
SELECT 'Console.EnableAppointmentTimeZoneAdjustment', 'Enable time zone adjustment in the appointment creation UI', 'Console', 'Enable time zone adjustment in the appointment creation UI', 3, 0, 'True'
UNION ALL
SELECT 'Toggle.RabbitMqCacheInvalidation', 'Use RabbitMq cache invalidation instead of sql service broker', 'Toggle', 'Use RabbitMq cache invalidation instead of sql service broker', 3, 0, 'True'
UNION ALL
SELECT 'Toggle.Supervisor.EnableScriptErrorsLogging', 'Enable CatiSupervisor logging for script errrors', 'Toggle', 'Enable logging of uncaught JavaScript exceptions for CatiSupervisor', 3, 0, 'True'
UNION ALL
SELECT 'Alerting.SchedulingErrors.IsAlertEnabled', 'Enable alerting for scheduling errors', 'Alerting', 'Enable supervisor notification when multiple scheduling errors occurring for the same survey', 3, 0, 'True'
UNION ALL
SELECT 'Alerting.SchedulingErrors.NumberOfErrors', 'Set minimum number of scheduling errors to trigger supervisor notification', 'Alerting', 'Set minimum number of scheduling errors to trigger supervisor notification', 1, 0, '5'
UNION ALL
SELECT 'Alerting.SchedulingErrors.TimePeriod', 'Set time period for accumulating scheduling errors', 'Alerting', 'Within specified time period supervisor notification will be triggered if number of scheduling errors for the same survey exceeds limit defined in Alerting.SchedulingErrors.NumberOfErrors setting', 4, 0, '0.00:05:00'
UNION ALL
SELECT 'Alerting.SchedulingErrors.NotificationFrequency', 'Set how often notification occurs', 'Alerting', 'Supervisor will only be notified once in a specified time period', 4, 0, '0.00:30:00'
UNION ALL
SELECT 'Toggle.EnableDesktopLiveMonitoring', 'Add checkbox to select monitoring interface in Interviewer List activity view', 'Toggle', 'Add checkbox to select monitoring interface in Interviewer List activity view', 3, 0, 'True'
UNION ALL
SELECT 'Alerting.NoCalls.IsAlertEnabled', 'Enable alerting for interviewer being in no calls state', 'Alerting', 'Enable supervisor notification when when interviewer stays in no calls state for too long', 3, 0, 'False'
UNION ALL
SELECT 'Alerting.NoCalls.NumberOfMinutes', 'Set minimum time for interviewer in no calls state to trigger supervisor notification', 'Alerting', 'Set minimum number of minutes for interviewer in no calls state to trigger supervisor notification', 1, 0, '2'
UNION ALL
SELECT 'Alerting.NoCalls.NumberOfInterviewers', 'Set minimum number of interviewers in no calls state to trigger supervisor notification', 'Alerting', 'Set minimum number of interviewers being in no calls state simultaneously to trigger supervisor notification', 1, 0, '1'
UNION ALL
SELECT 'Alerting.NoCalls.ExcludedInterviewerGroups', 'Set interviewer group(s) that will not trigger supervisor notification', 'Alerting', 'Set interviewer group(s) that will not trigger supervisor notification', 2, 0, ''
UNION ALL
SELECT 'Alerting.NoCalls.NotificationFrequency', 'Set how often notification occurs', 'Alerting', 'Supervisor will only be notified once in a specified time period', 4, 0, '0.00:15:00'
UNION ALL
SELECT 'Alerting.WaitingState.IsAlertEnabled', 'Enable alerting for interviewer being in waiting state', 'Alerting', 'Enable supervisor notification when when interviewer stays in waiting state for too long', 3, 0, 'False'
UNION ALL
SELECT 'Alerting.WaitingState.NumberOfMinutes', 'Set minimum time for interviewer in waiting state to trigger supervisor notification', 'Alerting', 'Set minimum number of minutes for interviewer in waiting state to trigger supervisor notification', 1, 0, '2'
UNION ALL
SELECT 'Alerting.WaitingState.NumberOfInterviewers', 'Set minimum number of interviewers in waiting state to trigger supervisor notification', 'Alerting', 'Set minimum number of interviewers being in waiting state simultaneously to trigger supervisor notification', 1, 0, '1'
UNION ALL
SELECT 'Alerting.WaitingState.ExcludedInterviewerGroups', 'Set interviewer group(s) that will not trigger supervisor notification', 'Alerting', 'Set interviewer group(s) that will not trigger supervisor notification', 2, 0, ''
UNION ALL
SELECT 'Alerting.WaitingState.NotificationFrequency', 'Set how often notification occurs', 'Alerting', 'Supervisor will only be notified once in a specified time period', 4, 0, '0.00:15:00'
UNION ALL
SELECT 'Alerting.SelectingState.IsAlertEnabled', 'Enable alerting for interviewer being in selecting state', 'Alerting', 'Enable supervisor notification when when interviewer stays in selecting for too long', 3, 0, 'False'
UNION ALL
SELECT 'Alerting.SelectingState.NumberOfMinutes', 'Set minimum time for interviewer in selecting state to trigger supervisor notification', 'Alerting', 'Set minimum number of minutes for interviewer in selecting state to trigger supervisor notification', 1, 0, '10'
UNION ALL
SELECT 'Alerting.SelectingState.NumberOfInterviewers', 'Set minimum number of interviewers in selecting state to trigger supervisor notification', 'Alerting', 'Set minimum number of interviewers being in selecting state simultaneously to trigger supervisor notification', 1, 0, '1'
UNION ALL
SELECT 'Alerting.SelectingState.ExcludedInterviewerGroups', 'Set interviewer group(s) that will not trigger supervisor notification', 'Alerting', 'Set interviewer group(s) that will not trigger supervisor notification', 2, 0, ''
UNION ALL
SELECT 'Alerting.SelectingState.NotificationFrequency', 'Set how often notification occurs', 'Alerting', 'Supervisor will only be notified once in a specified time period', 4, 0, '0.00:15:00'
UNION ALL
SELECT 'Dialer.OpenSurveysOnDialersIndividually', 'Enable opening and closing surveys on each dialer individually.', 'Telephony', 'When enabled - surveys are opened and closed on each dialer individually. When disabled - surveys are opened and closed on dialers via batch requests for dialers with the same dial type. Should be enabled to support multiple dialers of the same type pointing to different dialer webservices.', 3, 0, 'False'
UNION ALL
SELECT 'RoutineMaintenance.Actions.LargeObjectHeapFragmentation.ShiftType', 'Large object heap fragmentation shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
UNION ALL
SELECT 'CallManagement.PageSize', 'Set number of calls in a single Call Management page', 'Call Management', 'Number of calls in a single Call Management page', 1, 0, '100'
UNION ALL
SELECT 'Toggle.DirectlyInsertResponses', 'Use direct insert into survey database instead of FusionSurveyData SOAP API', 'Toggle', 'When enabled cati backend uses direct insert into survey database instead of FusionSurveyData SOAP API', 3, 0, 'True'
UNION ALL
SELECT 'Toggle.UseNewDialerApi', 'Call dialer methods from Confirmit.CatiDialer.Api', 'Toggle', 'Call dialer methods from Confirmit.CatiDialer.Api', 3, 0, 'False'
UNION ALL
SELECT 'Supervisor.ActivityViewPageSize', 'Set the maximum number of rows to be shown on a single page for the Interviewers List and Performance List Activity Views', 'Supervisor', 'Number of rows on a single Interviewers List and Performance List page', 1, 0, '500'
UNION ALL
SELECT 'Toggle.ReadInterviewLanguageDirectly', 'Read respondent language directly from database', 'Toggle', 'Read respondent language directly from database', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableAutomaticScrolling', 'Enable automatic scrolling in the interviewer screen', 'Interviewing', 'Enable automatic scrolling in the interviewer screen', 3, 0, 'True'
UNION ALL
SELECT 'Console.OrderInterviewsByPriority', 'Enable ordering interviews by priority in the manual selection screen', 'Console', 'When enabled - interviews are ordered by priority in the manual selection interface', 3, 0, 'True'
UNION ALL
SELECT 'Console.EnableInterviewsRandomization', 'Enable interviews randomization in the manual selection screen', 'Console', 'When enabled - interviews are randomized in the manual selection interface', 3, 0, 'False'
UNION ALL
SELECT 'Console.RandomizationInterviewCount', 'The pool size of interviews that will be used for randomization on the manual selection screen', 'Console', 'The pool size of interviews that will be used for randomization on the manual selection screen', 1, 0, '25000'
UNION ALL
SELECT 'Toggle.EnforceCatiHostNameForSurveys', 'Change the hostname in the survey links to the hostname from MultimodeBaseURL', 'Toggle', 'Change the hostname in the survey links to the hostname from MultimodeBaseURL', 3, 0, 'False'
UNION ALL
SELECT 'Console.ManualCallsInsideShiftOnly', 'Display only interviews on the manual selection screen that are valid for the current shift and have a time to call value either now or in the past', 'Interviewing', 'Display only interviews on the manual selection screen that are valid for the current shift and have a time to call value either now or in the past', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.SendGoNotReadyImmediately', 'Immediately notify the dialer when an interviewer enters or exits the Pending logout or Pending break state instead of waiting until the end of the interview', 'Toggle', 'Enables GoReady and GoNotReady commands to be sent to the dialer immediately when an interviewer presses either the Pending logout or Pending break button when he is doing an interview with a dialer in a survey with a predictive dialing mode', 3, 0, 'True'
UNION ALL
SELECT 'InterviewerPassword.IsChangeAfterFirstLoginRequired', 'CATI interviewer is required to change his password on the first login', 'Supervisor', 'CATI interviewer is required to change his password on the first login', 3, 0, 'False'
UNION ALL
SELECT 'Console.ManualDialTypeSelection', 'When enabled, the interviewer can select the dial type (landline or cellphone) for the login session', 'Interviewing', 'When enabled, the interviewer can select the dial type (landline or cellphone) for the login session', 3, 0, 'False'
UNION ALL
SELECT 'Console.EnforceManualSelectionForCellPhonePerson', 'When enabled, interviewers with cellphone dialtype will always use manual selection task choice', 'Interviewing', 'When enabled, interviewers with cellphone dialtype will always use manual selection task choice', 3, 0, 'False'
UNION ALL
SELECT 'Console.Metrics.EnableInterviewerMetrics', 'When enabled, the interviewer will be able to open performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to open interviewer performance metrics', 3, 0, 'True'
UNION ALL
SELECT 'Console.Metrics.EnableLoginSessionDuration', 'When enabled, the interviewer will be able to see his login session duration in the performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to see his login session duration in the performance metrics', 3, 0, 'True'
UNION ALL
SELECT 'Console.Metrics.EnableBreakDuration', 'When enabled, the interviewer will be able to see his total break duration in the performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to see his total break duration in the performance metrics', 3, 0, 'True'
UNION ALL
SELECT 'Console.Metrics.EnableAverageConnectedCallTime', 'When enabled, the interviewer will be able to see his average connected call time in the performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to see his average connected call time in the performance metrics', 3, 0, 'False'
UNION ALL
SELECT 'Console.Metrics.EnableAverageWrapTime', 'When enabled, the interviewer will be able to see his average wrap time in the performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to see his average wrap time in the performance metrics', 3, 0, 'False'
UNION ALL
SELECT 'Console.Metrics.EnableCallConnectedTime', 'When enabled, the interviewer will be able to see his call connected time in the performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to see his call connected time in the performance metrics', 3, 0, 'False'
UNION ALL
SELECT 'Console.Metrics.EnableCallAttempts', 'When enabled, the interviewer will be able to see his call attempts count in the performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to see his call attempts count in the performance metrics', 3, 0, 'False'
UNION ALL
SELECT 'Console.Metrics.EnableCallsConnected', 'When enabled, the interviewer will be able to see his calls connected count in the performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to see his calls connected count in the performance metrics', 3, 0, 'False'
UNION ALL
SELECT 'Console.Metrics.EnableRefusals', 'When enabled, the interviewer will be able to see his count of refused calls in the performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to see his count of refused calls in the performance metrics', 3, 0, 'True'
UNION ALL
SELECT 'Console.Metrics.EnableAppointmentsMade', 'When enabled, the interviewer will be able to see his count of made appointments in the performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to see his count of made appointments in the performance metrics', 3, 0, 'False'
UNION ALL
SELECT 'Console.Metrics.EnableInterviewsCompleted', 'When enabled, the interviewer will be able to see his count of completed interviews in the performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to see his count of completed interviews in the performance metrics', 3, 0, 'True'
UNION ALL
SELECT 'Console.Metrics.EnableInterviewsCompletedPerHour', 'When enabled, the interviewer will be able to see his count of completed interviews per hour in the performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to see his count of completed interviews per hour in the performance metrics', 3, 0, 'True'
UNION ALL
SELECT 'Console.Metrics.EnableCallAttemptsPerHour', 'When enabled, the interviewer will be able to see his count of call attempts per hour in the performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to see his count of completed interviews per hour in the performance metrics', 3, 0, 'False'
UNION ALL
SELECT 'Console.Metrics.EnableCallAttemptsPerComplete', 'When enabled, the interviewer will be able to see his count of call attempts per complete in the performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to see his count of completed interviews per complete in the performance metrics', 3, 0, 'True'
UNION ALL
SELECT 'Console.Metrics.EnableTotalCompletedInterviews', 'When enabled, the interviewer will be able to see the total number of completed interviews in the performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to see the total number of completed interviews in the performance metrics', 3, 0, 'True'
UNION ALL
SELECT 'Console.Metrics.EnableAverageCompletedInterviewsPerHour', 'When enabled, the interviewer will be able to see the average number of completed interviews per hour in the performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to see the average number of completed interviews per hour in the performance metrics', 3, 0, 'False'
UNION ALL
SELECT 'Console.Metrics.EnableAverageCallAttemptsPerHour', 'When enabled, the interviewer will be able to see the average number of call attempts per hour in the performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to see the average number of call attempts per hour in the performance metrics', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.EnableInterviewerMetricsConfiguration', 'Enable configuration of interviewer performance metrics', 'Toggle', 'When enabled, Interviewer statistic settings will be available under the Admin menu', 3, 0, 'True'
UNION ALL
SELECT 'Supervisor.TablesPreserveSelectionState', 'Preserve selection state on filtering and switching pages in Interviewers List', 'Supervisor', 'When enabled, selection state on filtering and switching pages preserves. Works in the Interviewers List only', 3, 0, 'False' 
UNION ALL
SELECT 'Toggle.EnableHttpKeepAliveForDialer', 'Enable HTTP keep-alive for requests to the dialer', 'Toggle', 'HTTP persistent connection, also called HTTP keep-alive, or HTTP connection reuse, is the idea of using a single TCP connection to send and receive multiple HTTP requests/responses, as opposed to opening a new connection for every single request/response pair', 3, 0, 'False'
UNION ALL
SELECT 'Server.BackendMinThreadPoolSize', 'Minimum thread pool size in backend', 'System', 'Set minimum thread pool size during start of backend service if value is more then 0', 1, 0, '0'
UNION ALL
SELECT 'Supervisor.ActivityViewLoadTest', 'Should be used for testing only, DO NOT enable it for clients', 'Supervisor', 'When enabled - 1 sec refresh interval should become available in the refresh interval dropdown in Activity View - Interviewer List', 3, 0, 'False'
UNION ALL
SELECT 'Console.Metrics.EnableCallAttemptsPerHourAboveAverageComparison', 'When enabled, Above average tag will be shown for interviewer', 'Interviewing', 'When enabled, the interviewer will be able to see the Above Average tag, indicating that they are making more call attempts per hour than the company average', 3, 0, 'False'
UNION ALL
SELECT 'Console.Metrics.EnableInterviewsCompletedPerHourAboveAverageComparison', 'When enabled, Above average tag will be shown for interviewer', 'Interviewing', 'When enabled, the interviewer will be able to see the Above Average tag, indicating that they are making more interview completes per hour than the company average', 3, 0, 'False'
UNION ALL
SELECT 'Console.Metrics.EnableCallAttemptsPerHourBelowAverageComparison', 'When enabled, Below average tag will be shown for interviewer', 'Interviewing', 'When enabled, the interviewer will be able to see the Below Average tag, indicating that they are making fewer call attempts per hour than the company average', 3, 0, 'False'
UNION ALL
SELECT 'Console.Metrics.EnableInterviewsCompletedPerHourBelowAverageComparison', 'When enabled, Below average tag will be shown for interviewer', 'Interviewing', 'When enabled, the interviewer will be able to see the Below Average tag, indicating that they are making less interview completes per hour than the company average', 3, 0, 'False'
UNION ALL
SELECT 'Console.Metrics.EnableCallAttemptsPerCompleteAboveAverageComparison', 'When enabled, Above average tag will be shown for interviewer', 'Interviewing', 'When enabled, the interviewer will be able to see the Above Average tag, indicating that they are making more call attempts per completed interview than the company average', 3, 0, 'False'
UNION ALL
SELECT 'Console.Metrics.EnableCallAttemptsPerCompleteBelowAverageComparison', 'When enabled, Below average tag will be shown for interviewer', 'Interviewing', 'When enabled, the interviewer will be able to see the Below Average tag, indicating that they are making fewer call attempts per completed interview than the company average', 3, 0, 'False'
UNION ALL
SELECT 'Console.Metrics.EnableTotalInterviewingTime', 'When enabled, the interviewer will be able to see the total interviewing time in the performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to see the total interviewing time in the performance metrics', 3, 0, 'True'
UNION ALL
SELECT 'Dialer.RespondentVariablesToSend', 'Respondent variables to send to the dialer', 'Telephony', 'A comma-separated list of respondent variables that are sent to the dialer. If the survey does not contain some variables, they are ignored.', 2, 0, ''
UNION ALL
SELECT 'Toggle.EnableDesktopMonitoringConsole', 'When eneabled,  the desktop monitoring console is available for use.', 'Toggle', 'Enables desktop monitoring console.', 3, 0, 'True'
UNION ALL
SELECT 'Toggle.NewCallManagement', 'When eneabled, new Call Management tab is available for use.', 'Toggle', 'Enables new call management tab.', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.EnableHubIntegration', 'Enables access to Manage call history in SmartHub page and data synchronization process', 'Toggle', 'When enabled, the supervisor can manage the system CATI hub and synchronize data with it.', 3, 0, 'False'
UNION ALL
SELECT 'CallHistoryHub.SyncSleepPeriod', 'Determines the interval between synchronization operations of CATI call history data to SmartHub.', 'CallHistoryHub', 'Delay between data synchronization operations of CATI call history data with SmartHub.', 4, 0, '0.00:05:00'
UNION ALL
SELECT 'CallHistoryHub.SyncEnabled', 'When eneabled, CATI call history data will be synchronizing with SmartHub system CATI hub.', 'CallHistoryHub', 'When eneabled, CATI call history data will be synchronizing with SmartHub system CATI hub.', 3, 0, 'False'
UNION ALL
SELECT 'CallHistoryHub.RetentionPeriod', 'Specifies the retention period, in days, for storing CATI call history data in the SmartHub CATI system hub.', 'CallHistoryHub', 'The retention period, in days, for storing CATI call history data in the SmartHub CATI system hub.', 1, 0, '180'
UNION ALL
SELECT 'RoutineMaintenance.Actions.FullSynchronizationOfCatiDataInHub.ShiftType', 'Flags CATI data for full synchronization in the HUB shift type to facilitate removal of old records.', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '2'
UNION ALL
SELECT 'Toggle.EnableCustomCallAttemptFields', 'When eneabled, custom call attempt fields functionality is available.', 'Toggle', 'Enables custom call attempt fields functionality.', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.EnableAgentAssistedDialling', 'When eneabled, extend the DialType with Assisted option.', 'Toggle', 'When eneabled, extend the DialType with Assisted option.', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.BvSvyScheduleDeadlockReduction', 'When eneabled, BvSvySchedule table usage is reduced.', 'Toggle', 'When eneabled, ActiveDialId and DialerId columns of BvSvySchedule table are not used.', 3, 0, 'False'
UNION ALL
SELECT 'Toggle.SendConsoleStateInRmqPayload', 'When eneabled, interviewer will be using RabbitMq to update state.', 'Toggle', 'When eneabled, interviewer will be using RabbitMq to update state.', 3, 0, 'False'
)
INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
	SELECT * FROM Data
GO

/*
public enum ValueType
{
    Int = 1,
    String = 2,
    Bool = 3,
    Timespan = 4
}
*/

PRINT 'Loading initial RoleID to BvRole...'
GO

INSERT INTO BvRole VALUES( 0x0000 , 'System' )
INSERT INTO BvRole VALUES( 0x0001 , 'Supervisor' )
INSERT INTO BvRole VALUES( 0x0002 , 'Interviewer' )
INSERT INTO BvRole VALUES( 0x0004 , 'Coder' )
INSERT INTO BvRole VALUES( 0x0008 , 'Consultant' )
INSERT INTO BvRole VALUES( 0x0010 , 'Key-entry Clerk' )
INSERT INTO BvRole VALUES( 0x0020 , 'Web-respondents' )
INSERT INTO BvRole VALUES( 0x0040 , 'CAPI Intervievers' )
GO

PRINT 'Loading initial states to BvState...'
GO
INSERT INTO BvState (StateID, Name, Priority, FcdAction) VALUES( 1, 'Appointment', 1000, 1 )
INSERT INTO BvState (StateID, Name, Priority, AaporCode) VALUES( 2, 'Busy', 1, '3.121' )
INSERT INTO BvState (StateID, Name, Priority, AaporCode) VALUES( 3, 'No reply', 1, '3.122' )
INSERT INTO BvState (StateID, Name, Priority, AaporCode) VALUES( 4, 'Quota failure', 1, '4.8' )
INSERT INTO BvState (StateID, Name, Priority, AaporCode) VALUES( 5, 'Refusal', 1, '2.11' )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 6, 'Terminated', 1 )
INSERT INTO BvState (StateID, Name, Priority, AaporCode) VALUES( 7, 'Answer phone', 1, '3.123' )
INSERT INTO BvState (StateID, Name, Priority, AaporCode) VALUES( 8, 'Modem', 1, '4.2' )
INSERT INTO BvState (StateID, Name, Priority, AaporCode) VALUES( 9, 'Fax', 1, '4.2' )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 10, 'Congestion', 1 )
INSERT INTO BvState (StateID, Name, Priority, AaporCode) VALUES( 11, 'Unobtainable', 1, '3.1255' )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 12, 'Nuisance', 1 )
INSERT INTO BvState (StateID, Name, Priority, DA, AaporCode) VALUES( 13, 'Completed', 1, 1, '1.1' )
INSERT INTO BvState (StateID, Name, Priority, AaporCode) VALUES( 14, 'Screened', 1, '4.7' )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 15, 'Returned not dialled', 1 )
INSERT INTO BvState (StateID, Name, Priority, AaporCode) VALUES( 16, 'Fresh sample', 1, '3.11' )
INSERT INTO BvState (StateID, Name, Priority, DA) VALUES( 17, 'Blacklist', 1, 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 18, 'Not automatically dialled (ie manual dialling)', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 19, 'Status not sensed', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 20, 'Transfer to Web', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 21, 'Transfer to CATI', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 22, 'Transfer to CAPI', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 23, 'Transfer to IVR', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 24, 'Interrupted by interviewer', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 25, 'Returned dialler expired', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 26, 'Interrupted by system', 1 )
INSERT INTO BvState (StateID, Name, Priority, AaporCode) VALUES( 27, 'Filtered by call delivery', 1, '4.8' )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 28, 'Stopped', 1 )
INSERT INTO BvState (StateID, Name, Priority, AaporCode) VALUES( 29, 'Telephony failure', 1, '3.2155' )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 30, 'Error', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 31, 'Too Many Call Attempts', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 32, 'Custom2', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 33, 'Soft Appointment', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 34, 'Custom4', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 35, 'Custom5', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 36, 'Custom6', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 37, 'Custom7', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 38, 'Custom8', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 39, 'Custom9', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 40, 'Custom10', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 41, 'Custom11', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 42, 'Custom12', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 43, 'Custom13', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 44, 'Custom14', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 45, 'Custom15', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 46, 'Custom16', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 47, 'Custom17', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 48, 'Custom18', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 49, 'Custom19', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 50, 'Custom20', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 51, 'Custom21', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 52, 'Custom22', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 53, 'Custom23', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 54, 'Custom24', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 55, 'Custom25', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 56, 'Custom26', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 57, 'Custom27', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 58, 'Custom28', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 59, 'Custom29', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 60, 'Custom30', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 61, 'Custom31', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 62, 'Custom32', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 63, 'Custom33', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 64, 'Custom34', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 65, 'Custom35', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 66, 'Custom36', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 67, 'Custom37', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 68, 'Custom38', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 69, 'Custom39', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 70, 'Custom40', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 71, 'Custom41', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 72, 'Custom42', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 73, 'Custom43', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 74, 'Custom44', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 75, 'Custom45', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 76, 'Custom46', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 77, 'Custom47', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 78, 'Custom48', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 79, 'Custom49', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 80, 'Custom50', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 81, 'Custom51', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 82, 'Custom52', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 83, 'Custom53', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 84, 'Custom54', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 85, 'Custom55', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 86, 'Custom56', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 87, 'Custom57', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 88, 'Custom58', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 89, 'Custom59', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 90, 'Custom60', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 91, 'Custom61', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 92, 'Custom62', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 93, 'Custom63', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 94, 'Custom64', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 95, 'Custom65', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 96, 'Custom66', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 97, 'Custom67', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 98, 'Custom68', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 99, 'Custom69', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 100, 'Custom70', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 101, 'Custom71', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 102, 'Custom72', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 103, 'Custom73', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 104, 'Custom74', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 105, 'Custom75', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 106, 'Custom76', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 107, 'Custom77', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 108, 'Custom78', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 109, 'Custom79', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 110, 'Custom80', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 111, 'Custom81', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 112, 'Custom82', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 113, 'Custom83', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 114, 'Custom84', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 115, 'Custom85', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 116, 'Custom86', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 117, 'Custom87', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 118, 'Custom88', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 119, 'Custom89', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 120, 'Custom90', 1 )
INSERT INTO BvState (StateID, Name, Priority, FcdAction) VALUES( 1000, 'Inbound call', 1, 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 1001, 'Dropped by respondent', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 1010, 'Internal Transfer', 2000 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 1011, 'External Transfer', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 1020, 'Dial interrupted by interviewer', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 1051, 'Survey script error', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 1012, 'Canceled Transfer', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 1021, 'Externally validated number', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 1052, 'Synchronized Sample', 1 )

GO

PRINT 'Loading initial timezones...'
GO

exec BvSpTimezoneMaster_Insert 1, '(GMT+00:00) Dublin, Edinburgh, Lisbon, London', 0, 2, 'GMT Standard Time', '2000-10-05 02:00:00.000', 0, 0, 'GMT Daylight Time', '2000-03-05 01:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 2, '(GMT+00:00) Monrovia, Reykjavik', 0, 1, 'Greenwich Standard Time', NULL, NULL, 0, 'Greenwich Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 3, '(GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna', -60, 2, 'W. Europe Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'W. Europe Daylight Time', '2000-03-05 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 4, '(GMT+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague', -60, 2, 'Central Europe Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'Central Europe Daylight Time', '2000-03-05 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 5, '(GMT+01:00) Brussels, Copenhagen, Madrid, Paris', -60, 2, 'Romance Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'Romance Daylight Time', '2000-03-05 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 6, '(GMT+01:00) Sarajevo, Skopje, Warsaw, Zagreb', -60, 2, 'Central European Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'Central European Daylight Time', '2000-03-05 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 7, '(GMT+01:00) West Central Africa', -60, 1, 'W. Central Africa Standard Time', NULL, NULL, 0, 'W. Central Africa Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 8, '(GMT+02:00) Athens, Bucharest', -120, 2, 'GTB Standard Time', '2000-10-05 04:00:00.000', 0, 0, 'GTB Daylight Time', '2000-03-05 03:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 9, '(GMT+02:00) Chisinau', -120, 2, 'E. Europe Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'E. Europe Daylight Time', '2000-03-05 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 10, '(GMT+02:00) Cairo', -120, 1, 'Egypt Standard Time', NULL, NULL, 0, 'Egypt Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 11, '(GMT+02:00) Harare, Pretoria', -120, 1, 'South Africa Standard Time', NULL, NULL, 0, 'South Africa Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 12, '(GMT+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius', -120, 2, 'FLE Standard Time', '2000-10-05 04:00:00.000', 0, 0, 'FLE Daylight Time', '2000-03-05 03:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 13, '(GMT+02:00) Jerusalem', -120, 2, 'Jerusalem Standard Time', '2000-10-05 02:00:00.000', 0, 0, 'Jerusalem Daylight Time', '2000-03-05 02:00:00.000', 5, -60
exec BvSpTimezoneMaster_Insert 14, '(GMT+03:00) Baghdad', -180, 1, 'Arabic Standard Time', NULL, NULL, 0, 'Arabic Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 15, '(GMT+03:00) Kuwait, Riyadh', -180, 1, 'Arab Standard Time', NULL, NULL, 0, 'Arab Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 16, '(GMT+03:00) Moscow, St. Petersburg', -180, 1, 'Russia TZ 2 Standard Time', NULL, NULL, 0, 'Russia TZ 2 Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 17, '(GMT+03:00) Nairobi', -180, 1, 'E. Africa Standard Time', NULL, NULL, 0, 'E. Africa Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 18, '(GMT+03:30) Tehran', -210, 2, 'Iran Standard Time', '2000-09-03 23:59:00.000', 6, 0, 'Iran Daylight Time', '2000-03-04 00:00:00.000', 5, -60
exec BvSpTimezoneMaster_Insert 19, '(GMT+04:00) Abu Dhabi, Muscat', -240, 1, 'Arabian Standard Time', NULL, NULL, 0, 'Arabian Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 20, '(GMT+04:00) Yerevan', -240, 1, 'Caucasus Standard Time', NULL, NULL, 0, 'Caucasus Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 21, '(GMT+04:30) Kabul', -270, 1, 'Afghanistan Standard Time', NULL, NULL, 0, 'Afghanistan Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 22, '(GMT+05:00) Ekaterinburg', -300, 1, 'Russia TZ 4 Standard Time', NULL, NULL, 0, 'Russia TZ 4 Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 23, '(GMT+05:00) Ashgabat, Tashkent', -300, 1, 'West Asia Standard Time', NULL, NULL, 0, 'West Asia Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 24, '(GMT+05:30) Chennai, Kolkata, Mumbai, New Delhi', -330, 1, 'India Standard Time', NULL, NULL, 0, 'India Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 25, '(GMT+05:45) Kathmandu', -345, 1, 'Nepal Standard Time', NULL, NULL, 0, 'Nepal Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 26, '(GMT+06:00) Novosibirsk', -360, 1, 'Russia TZ 5 Standard Time', NULL, NULL, 0, 'Russia TZ 5 Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 27, '(GMT+06:00) Astana', -360, 1, 'Central Asia Standard Time', NULL, NULL, 0, 'Central Asia Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 28, '(GMT+05:30) Sri Jayawardenepura', -330, 1, 'Sri Lanka Standard Time', NULL, NULL, 0, 'Sri Lanka Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 29, '(GMT+06:30) Yangon (Rangoon)', -390, 1, 'Myanmar Standard Time', NULL, NULL, 0, 'Myanmar Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 30, '(GMT+07:00) Bangkok, Hanoi, Jakarta', -420, 1, 'SE Asia Standard Time', NULL, NULL, 0, 'SE Asia Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 31, '(GMT+07:00) Krasnoyarsk', -420, 1, 'Russia TZ 6 Standard Time', NULL, NULL, 0, 'Russia TZ 6 Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 32, '(GMT+08:00) Beijing, Chongqing, Hong Kong, Urumqi', -480, 1, 'China Standard Time', NULL, NULL, 0, 'China Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 33, '(GMT+08:00) Irkutsk', -480, 1, 'Russia TZ 7 Standard Time', NULL, NULL, 0, 'Russia TZ 7 Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 34, '(GMT+08:00) Kuala Lumpur, Singapore', -480, 1, 'Malay Peninsula Standard Time', NULL, NULL, 0, 'Malay Peninsula Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 35, '(GMT+08:00) Perth', -480, 1, 'W. Australia Standard Time', NULL, NULL, 0, 'W. Australia Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 36, '(GMT+08:00) Taipei', -480, 1, 'Taipei Standard Time', NULL, NULL, 0, 'Taipei Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 37, '(GMT+09:00) Osaka, Sapporo, Tokyo', -540, 1, 'Tokyo Standard Time', NULL, NULL, 0, 'Tokyo Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 38, '(GMT+09:00) Seoul', -540, 1, 'Korea Standard Time', NULL, NULL, 0, 'Korea Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 39, '(GMT+09:00) Yakutsk', -540, 1, 'Russia TZ 8 Standard Time', NULL, NULL, 0, 'Russia TZ 8 Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 40, '(GMT+09:30) Adelaide', -570, 2, 'Cen. Australia Standard Time', '2000-04-01 03:00:00.000', 0, 0, 'Cen. Australia Daylight Time', '2000-10-01 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 41, '(GMT+09:30) Darwin', -570, 1, 'AUS Central Standard Time', NULL, NULL, 0, 'AUS Central Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 42, '(GMT+10:00) Brisbane', -600, 1, 'E. Australia Standard Time', NULL, NULL, 0, 'E. Australia Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 43, '(GMT+10:00) Canberra, Melbourne, Sydney', -600, 2, 'AUS Eastern Standard Time', '2000-04-01 03:00:00.000', 0, 0, 'AUS Eastern Daylight Time', '2000-10-01 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 44, '(GMT+10:00) Guam, Port Moresby', -600, 1, 'West Pacific Standard Time', NULL, NULL, 0, 'West Pacific Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 45, '(GMT+10:00) Hobart', -600, 2, 'Tasmania Standard Time', '2000-04-01 03:00:00.000', 0, 0, 'Tasmania Daylight Time', '2000-10-01 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 46, '(GMT+10:00) Vladivostok', -600, 1, 'Vladivostok Standard Time', NULL, NULL, 0, 'Vladivostok Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 47, '(GMT+11:00) Solomon Is., New Caledonia', -660, 1, 'Central Pacific Standard Time', NULL, NULL, 0, 'Central Pacific Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 48, '(GMT+12:00) Auckland, Wellington', -720, 2, 'New Zealand Standard Time', '2000-04-01 03:00:00.000', 0, 0, 'New Zealand Daylight Time', '2000-09-05 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 49, '(GMT+12:00) Fiji', -720, 2, 'Fiji Standard Time', '2000-01-02 03:00:00.000', 0, 0, 'Fiji Daylight Time', '2000-11-01 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 50, '(GMT+13:00) Nuku''alofa', -780, 1, 'Tonga Standard Time', NULL, NULL, 0, 'Tonga Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 51, '(GMT-01:00) Azores', 60, 2, 'Azores Standard Time', '2000-10-05 01:00:00.000', 0, 0, 'Azores Daylight Time', '2000-03-05 00:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 52, '(GMT-01:00) Cape Verde Is.', 60, 1, 'Cape Verde Standard Time', NULL, NULL, 0, 'Cape Verde Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 53, '(GMT-02:00) Mid-Atlantic - Old', 120, 2, 'Mid-Atlantic Standard Time', '2000-09-05 02:00:00.000', 0, 0, 'Mid-Atlantic Daylight Time', '2000-03-05 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 54, '(GMT-03:00) Brasilia', 180, 2, 'E. South America Standard Time', '2000-02-03 23:59:00.000', 6, 0, 'E. South America Daylight Time', '2000-11-01 23:59:00.000', 6, -60
exec BvSpTimezoneMaster_Insert 55, '(GMT-03:00) Cayenne, Fortaleza', 180, 1, 'SA Eastern Standard Time', NULL, NULL, 0, 'SA Eastern Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 56, '(GMT-03:00) Greenland', 180, 2, 'Greenland Standard Time', '2000-10-05 23:00:00.000', 6, 0, 'Greenland Daylight Time', '2000-03-05 22:00:00.000', 6, -60
exec BvSpTimezoneMaster_Insert 57, '(GMT-03:30) Newfoundland', 210, 2, 'Newfoundland Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Newfoundland Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 58, '(GMT-04:00) Atlantic Time (Canada)', 240, 2, 'Atlantic Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Atlantic Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 59, '(GMT-04:00) Georgetown, La Paz, Manaus, San Juan', 240, 1, 'SA Western Standard Time', NULL, NULL, 0, 'SA Western Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 60, '(GMT-04:00) Santiago', 240, 2, 'Pacific SA Standard Time', '2000-04-01 23:59:00.000', 6, 0, 'Pacific SA Daylight Time', '2000-09-01 23:59:00.000', 6, -60
exec BvSpTimezoneMaster_Insert 61, '(GMT-05:00) Bogota, Lima, Quito, Rio Branco', 300, 1, 'SA Pacific Standard Time', NULL, NULL, 0, 'SA Pacific Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 62, '(GMT-05:00) Eastern Time (US & Canada)', 300, 2, 'Eastern Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Eastern Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 63, '(GMT-05:00) Indiana (East)', 300, 2, 'US Eastern Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'US Eastern Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 64, '(GMT-06:00) Central America', 360, 1, 'Central America Standard Time', NULL, NULL, 0, 'Central America Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 65, '(GMT-06:00) Central Time (US & Canada)', 360, 2, 'Central Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Central Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 66, '(GMT-06:00) Guadalajara, Mexico City, Monterrey', 360, 2, 'Mexico Standard Time', '2000-10-05 02:00:00.000', 0, 0, 'Mexico Daylight Time', '2000-04-01 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 67, '(GMT-06:00) Saskatchewan', 360, 1, 'Canada Central Standard Time', NULL, NULL, 0, 'Canada Central Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 68, '(GMT-07:00) Arizona', 420, 1, 'US Mountain Standard Time', NULL, NULL, 0, 'US Mountain Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 69, '(GMT-07:00) Chihuahua, La Paz, Mazatlan', 420, 2, 'Mexico Standard Time 2', '2000-10-05 02:00:00.000', 0, 0, 'Mexico Daylight Time 2', '2000-04-01 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 70, '(GMT-07:00) Mountain Time (US & Canada)', 420, 2, 'Mountain Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Mountain Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 71, '(GMT-08:00) Pacific Time (US & Canada)', 480, 2, 'Pacific Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Pacific Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 72, '(GMT-09:00) Alaska', 540, 2, 'Alaskan Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Alaskan Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 73, '(GMT-10:00) Hawaii', 600, 1, 'Hawaiian Standard Time', NULL, NULL, 0, 'Hawaiian Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 74, '(GMT+13:00) Samoa', -780, 2, 'Samoa Standard Time', '2000-04-01 04:00:00.000', 0, 0, 'Samoa Daylight Time', '2000-09-05 03:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 75, '(GMT-12:00) International Date Line West', 720, 1, 'Dateline Standard Time', NULL, NULL, 0, 'Dateline Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 76, '(GMT-03:00) City of Buenos Aires', 180, 1, 'Argentina Standard Time', NULL, NULL, 0, 'Argentina Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 77, '(GMT+04:00) Baku', -240, 1, 'Azerbaijan Standard Time', NULL, NULL, 0, 'Azerbaijan Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 78, '(GMT+06:00) Dhaka', -360, 1, 'Bangladesh Standard Time', NULL, NULL, 0, 'Bangladesh Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 79, '(GMT-04:00) Cuiaba', 240, 2, 'Central Brazilian Standard Time', '2000-02-03 23:59:00.000', 6, 0, 'Central Brazilian Daylight Time', '2000-11-01 23:59:00.000', 6, -60
exec BvSpTimezoneMaster_Insert 80, '(GMT-06:00) Guadalajara, Mexico City, Monterrey', 360, 2, 'Central Standard Time (Mexico)', '2000-10-05 02:00:00.000', 0, 0, 'Central Daylight Time (Mexico)', '2000-04-01 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 81, '(GMT) Coordinated Universal Time', 0, 1, 'Coordinated Universal Time', NULL, NULL, 0, 'Coordinated Universal Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 82, '(GMT+04:00) Tbilisi', -240, 1, 'Georgian Standard Time', NULL, NULL, 0, 'Georgian Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 83, '(GMT+02:00) Amman', -120, 2, 'Jordan Standard Time', '2000-10-05 01:00:00.000', 5, 0, 'Jordan Daylight Time', '2000-03-05 23:59:00.000', 4, -60
exec BvSpTimezoneMaster_Insert 84, '(GMT+03:00) Minsk', -180, 1, 'Belarus Standard Time', NULL, NULL, 0, 'Belarus Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 85, '(GMT+12:00) Petropavlovsk-Kamchatsky - Old', -720, 2, 'Kamchatka Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'Kamchatka Daylight Time', '2000-03-05 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 86, '(GMT+11:00) Magadan', -660, 1, 'Magadan Standard Time', NULL, NULL, 0, 'Magadan Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 87, '(GMT+04:00) Port Louis', -240, 1, 'Mauritius Standard Time', NULL, NULL, 0, 'Mauritius Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 88, '(GMT+02:00) Beirut', -120, 2, 'Middle East Standard Time', '2000-10-05 23:59:00.000', 6, 0, 'Middle East Daylight Time', '2000-03-05 23:59:00.000', 6, -60
exec BvSpTimezoneMaster_Insert 89, '(GMT-03:00) Montevideo', 180, 1, 'Montevideo Standard Time', NULL, NULL, 0, 'Montevideo Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 90, '(GMT+01:00) Casablanca', -60, 1, 'Morocco Standard Time', NULL, NULL, 0, 'Morocco Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 91, '(GMT-07:00) Chihuahua, La Paz, Mazatlan', 420, 2, 'Mountain Standard Time (Mexico)', '2000-10-05 02:00:00.000', 0, 0, 'Mountain Daylight Time (Mexico)', '2000-04-01 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 92, '(GMT+02:00) Windhoek', -120, 1, 'Namibia Standard Time', NULL, NULL, 0, 'Namibia Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 93, '(GMT-08:00) Baja California', 480, 2, 'Pacific Standard Time (Mexico)', '2000-11-01 02:00:00.000', 0, 0, 'Pacific Daylight Time (Mexico)', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 94, '(GMT+05:00) Islamabad, Karachi', -300, 1, 'Pakistan Standard Time', NULL, NULL, 0, 'Pakistan Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 95, '(GMT-04:00) Asuncion', 240, 2, 'Paraguay Standard Time', '2000-03-04 23:59:00.000', 6, 0, 'Paraguay Daylight Time', '2000-10-01 23:59:00.000', 6, -60
exec BvSpTimezoneMaster_Insert 96, '(GMT+02:00) Damascus', -120, 2, 'Syria Standard Time', '2000-10-04 23:59:00.000', 4, 0, 'Syria Daylight Time', '2000-03-05 00:00:00.000', 5, -60
exec BvSpTimezoneMaster_Insert 97, '(GMT+03:00) Istanbul', -180, 1, 'Turkey Standard Time', NULL, NULL, 0, 'Turkey Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 98, '(GMT+08:00) Ulaanbaatar', -480, 1, 'Ulaanbaatar Standard Time', NULL, NULL, 0, 'Ulaanbaatar Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 99, '(GMT+12:00) Coordinated Universal Time+12', -720, 1, 'UTC+12', NULL, NULL, 0, 'UTC+12', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 100, '(GMT-02:00) Coordinated Universal Time-02', 120, 1, 'UTC-02', NULL, NULL, 0, 'UTC-02', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 101, '(GMT-11:00) Coordinated Universal Time-11', 660, 1, 'UTC-11', NULL, NULL, 0, 'UTC-11', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 102, '(GMT-04:00) Caracas', 240, 1, 'Venezuela Standard Time', NULL, NULL, 0, 'Venezuela Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 103, '(GMT-03:00) Salvador', 180, 1, 'Bahia Standard Time', NULL, NULL, 0, 'Bahia Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 104, '(GMT+02:00) Tripoli', -120, 1, 'Libya Standard Time', NULL, NULL, 0, 'Libya Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 105, '(GMT-01:00) Cabo Verde Is.', 60, 1, 'Cabo Verde Standard Time', NULL, NULL, 0, 'Cabo Verde Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 106, '(GMT+14:00) Kiritimati Island', -840, 1, 'Line Islands Standard Time', NULL, NULL, 0, 'Line Islands Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 107, '(GMT+02:00) Kaliningrad', -120, 1, 'Russia TZ 1 Standard Time', NULL, NULL, 0, 'Russia TZ 1 Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 108, '(GMT+11:00) Chokurdakh', -660, 1, 'Russia TZ 10 Standard Time', NULL, NULL, 0, 'Russia TZ 10 Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 109, '(GMT+12:00) Anadyr, Petropavlovsk-Kamchatsky', -720, 1, 'Russia TZ 11 Standard Time', NULL, NULL, 0, 'Russia TZ 11 Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 110, '(GMT+04:00) Izhevsk, Samara', -240, 1, 'Russia TZ 3 Standard Time', NULL, NULL, 0, 'Russia TZ 3 Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 111, '(GMT+10:00) Vladivostok', -600, 1, 'Russia TZ 9 Standard Time', NULL, NULL, 0, 'Russia TZ 9 Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 112, '(GMT-05:00) Chetumal', 300, 1, 'Eastern Standard Time (Mexico)', NULL, NULL, 0, 'Eastern Daylight Time (Mexico)', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 113, '(GMT+02:00) Gaza, Hebron', -120, 2, 'West Bank Gaza Standard Time', '2000-10-05 01:00:00.000', 6, 0, 'West Bank Gaza Daylight Time', '2000-03-04 01:00:00.000', 6, -60
exec BvSpTimezoneMaster_Insert 114, '(GMT+04:00) Astrakhan, Ulyanovsk', -240, 1, 'Astrakhan Standard Time', NULL, NULL, 0, 'Astrakhan Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 115, '(GMT+07:00) Barnaul, Gorno-Altaysk', -420, 1, 'Altai Standard Time', NULL, NULL, 0, 'Altai Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 116, '(GMT+07:00) Hovd', -420, 1, 'W. Mongolia Standard Time', NULL, NULL, 0, 'W. Mongolia Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 117, '(GMT+07:00) Tomsk', -420, 1, 'Tomsk Standard Time', NULL, NULL, 0, 'Tomsk Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 118, '(GMT+09:00) Pyongyang', -540, 1, 'North Korea Standard Time', NULL, NULL, 0, 'North Korea Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 119, '(GMT+08:45) Eucla', -525, 1, 'Aus Central W. Standard Time', NULL, NULL, 0, 'Aus Central W. Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 120, '(GMT+09:00) Chita', -540, 1, 'Transbaikal Standard Time', NULL, NULL, 0, 'Transbaikal Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 121, '(GMT+10:30) Lord Howe Island', -630, 2, 'Lord Howe Standard Time', '2000-04-01 02:00:00.000', 0, 0, 'Lord Howe Daylight Time', '2000-10-01 02:00:00.000', 0, -30
exec BvSpTimezoneMaster_Insert 122, '(GMT+11:00) Bougainville Island', -660, 1, 'Bougainville Standard Time', NULL, NULL, 0, 'Bougainville Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 123, '(GMT+11:00) Norfolk Island', -660, 1, 'Norfolk Standard Time', NULL, NULL, 0, 'Norfolk Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 124, '(GMT+11:00) Sakhalin', -660, 1, 'Sakhalin Standard Time', NULL, NULL, 0, 'Sakhalin Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 125, '(GMT+12:45) Chatham Islands', -765, 2, 'Chatham Islands Standard Time', '2000-04-01 03:45:00.000', 0, 0, 'Chatham Islands Daylight Time', '2000-09-05 02:45:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 126, '(GMT-03:00) Araguaina', 180, 1, 'Tocantins Standard Time', NULL, NULL, 0, 'Tocantins Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 127, '(GMT-03:00) Saint Pierre and Miquelon', 180, 2, 'Saint Pierre Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Saint Pierre Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 128, '(GMT-05:00) Turks and Caicos', 300, 2, 'Turks and Caicos Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Turks and Caicos Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 129, '(GMT-05:00) Haiti', 300, 2, 'Haiti Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Haiti Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 130, '(GMT-05:00) Havana', 300, 2, 'Cuba Standard Time', '2000-11-01 01:00:00.000', 0, 0, 'Cuba Daylight Time', '2000-03-02 00:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 131, '(GMT-06:00) Easter Island', 360, 2, 'Easter Island Standard Time', '2000-04-01 22:00:00.000', 6, 0, 'Easter Island Daylight Time', '2000-09-01 22:00:00.000', 6, -60
exec BvSpTimezoneMaster_Insert 132, '(GMT-08:00) Coordinated Universal Time-08', 480, 1, 'UTC-08', NULL, NULL, 0, 'UTC-08', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 133, '(GMT-09:00) Coordinated Universal Time-09', 540, 1, 'UTC-09', NULL, NULL, 0, 'UTC-09', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 134, '(GMT-09:30) Marquesas Islands', 570, 1, 'Marquesas Standard Time', NULL, NULL, 0, 'Marquesas Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 135, '(GMT-10:00) Aleutian Islands', 600, 2, 'Aleutian Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Aleutian Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 136, '(GMT+06:00) Omsk', -360, 1, 'Omsk Standard Time', NULL, NULL, 0, 'Omsk Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 137, '(GMT+07:00) Novosibirsk', -420, 1, 'Novosibirsk Standard Time', NULL, NULL, 0, 'Novosibirsk Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 138, '(GMT+04:00) Saratov', -240, 1, 'Saratov Standard Time', NULL, NULL, 0, 'Saratov Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 139, '(GMT+13:00) Coordinated Universal Time+13', -780, 1, 'UTC+13', NULL, NULL, 0, 'UTC+13', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 140, '(GMT-03:00) Punta Arenas', 180, 1, 'Magallanes Standard Time', NULL, NULL, 0, 'Magallanes Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 141, '(GMT+02:00) Khartoum', -120, 1, 'Sudan Standard Time', NULL, NULL, 0, 'Sudan Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 142, '(GMT+00:00) Sao Tome', 0, 2, 'Sao Tome Standard Time', '2000-01-01 02:00:00.000', 2, 0, 'Sao Tome Daylight Time', '2000-01-01 00:00:00.000', 2, -60
exec BvSpTimezoneMaster_Insert 143, '(GMT+04:00) Volgograd', -240, 1, 'Volgograd Standard Time', NULL, NULL, 0, 'Volgograd Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 144, '(GMT+05:00) Qyzylorda', -300, 1, 'Qyzylorda Standard Time', NULL, NULL, 0, 'Qyzylorda Daylight Time', NULL, NULL, -60

exec BvSpTimezone_Activate 1/*GMT*/

PRINT 'Loading initial site...'
DECLARE @SID int

SELECT @SID = 1

/*SELECT 'Loading initial questionnaires'*/

PRINT 'Loading initial containers...'

DECLARE @SiteSID int       SELECT @SiteSID      =  @SID  SELECT @SID = @SID + 1
DECLARE @SurveysSID int    SELECT @SurveysSID   =  @SID  SELECT @SID = @SID + 1
DECLARE @ResourcesSID int  SELECT @ResourcesSID =  @SID  SELECT @SID = @SID + 1
DECLARE @PersonsSID int    SELECT @PersonsSID   =  @SID  SELECT @SID = @SID + 1
DECLARE @QnairesSID int    SELECT @QnairesSID   =  @SID  SELECT @SID = @SID + 1
DECLARE @ServersSID int    SELECT @ServersSID   =  @SID  SELECT @SID = @SID + 1
DECLARE @ServicesSID int   SELECT @ServicesSID  =  @SID  SELECT @SID = @SID + 1
DECLARE @SServicesSID int  SELECT @SServicesSID =  @SID  SELECT @SID = @SID + 1
DECLARE @DServicesSID int  SELECT @DServicesSID =  @SID  SELECT @SID = @SID + 1

DECLARE @Group1SID int     SELECT @Group1SID    =  @SID  SELECT @SID = @SID + 1
DECLARE @Group2SID int     SELECT @Group2SID    =  @SID  SELECT @SID = @SID + 1
DECLARE @Group3SID int     SELECT @Group3SID    =  @SID  SELECT @SID = @SID + 1
DECLARE @SuperSID int      SELECT @SuperSID     =  @SID  SELECT @SID = @SID + 1
DECLARE @InterSID int      SELECT @InterSID     =  @SID  SELECT @SID = @SID + 1
DECLARE @CoderSID int      SELECT @CoderSID     =  @SID  SELECT @SID = @SID + 1
DECLARE @KeyEntrySID int   SELECT @KeyEntrySID  =  @SID  SELECT @SID = @SID + 1
DECLARE @WebRespSID int    SELECT @WebRespSID   =  @SID  SELECT @SID = @SID + 1
DECLARE @CAPISID int       SELECT @CAPISID      =  @SID  SELECT @SID = @SID + 1
DECLARE @Custom1SID int    SELECT @Custom1SID   =  @SID  SELECT @SID = @SID + 1
DECLARE @Custom2SID int    SELECT @Custom2SID   =  @SID  SELECT @SID = @SID + 1
DECLARE @Custom3SID int    SELECT @Custom3SID   =  @SID  SELECT @SID = @SID + 1
DECLARE @AutoSID int       SELECT @AutoSID      =  @SID  SELECT @SID = @SID + 1
DECLARE @SchedScrSID int   SELECT @SchedScrSID  =  @SID  SELECT @SID = @SID + 1
DECLARE @SampleScrSID int  SELECT @SampleScrSID =  @SID  SELECT @SID = @SID + 1
DECLARE @LibraryScrSID int SELECT @LibraryScrSID = @SID  SELECT @SID = @SID + 1
DECLARE @StateGroupRootSID int SELECT @StateGroupRootSID = @SID SELECT @SID = @SID + 1
DECLARE @StateGroupSID int SELECT @StateGroupSID = @SID SELECT @SID = @SID + 1
DECLARE @TCISID int        SELECT @TCISID       =  @SID  SELECT @SID = @SID + 1
DECLARE @DialersSID int    SELECT @DialersSID   =  @SID  SELECT @SID = @SID + 1
DECLARE @GarbageSID int    SELECT @GarbageSID   =  @SID  SELECT @SID = @SID + 1
DECLARE @GateSID int       SELECT @GateSID      =  @SID  SELECT @SID = @SID + 1
DECLARE @SServerSID int    SELECT @SServerSID   =  @SID  SELECT @SID = @SID + 1

PRINT 'Update BvState table'
UPDATE BvState SET StateGroupID = @StateGroupSID

PRINT 'Load BvThresholdITS table'
INSERT INTO BvThresholdITS ( SurveySID, ITS )
        SELECT 0, StateID FROM BvState WHERE StateGroupID = @StateGroupSID

PRINT 'Insert default state group'
INSERT INTO BvStateGroup( [ID], [Name], [Order], Deleted  ) VALUES( @StateGroupSID, 'Default group', 1, 0 )


PRINT 'Loading information about person groups...'

insert into BvPersonGroup(SID, Name, Description, InboundCallBehavior, CallTransferBehavior)
values(@InterSID,   'CATI Interviewers', '', 0, 0)

PRINT 'Loading initial SID...'

INSERT INTO BvSIDCounter( SID )
VALUES( @SID )

DECLARE @DefaultScheduleID INT
EXEC @DefaultScheduleID = BvSpGetNewSID

DECLARE @DefaultScheduleXml NVARCHAR(MAX) = '<?xml version="1.0" encoding="utf-16"?><Schedule xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"><Id>80</Id><Name /><Rules><Rule><Id>13c49088-ad96-476b-a6b4-b09ddf873ae1</Id><Description /><SubRules><SubRule><Id>3a51e35b-d940-4406-9d1e-dafd5792a83f</Id><ItsId>0</ItsId><ShiftTypeId>0</ShiftTypeId><Filter>GetRespondentValue(''CallAttemptCount'') &gt;= GetParamNumeric(''MaxCall'')</Filter><FilterEnabled>true</FilterEnabled><Description>Restrict maximum call attempts</Description><SubRuleActions><SubRuleAction><Id>1</Id><ActionId>26</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">31</ParameterValue><FilterEnabled>true</FilterEnabled></SubRuleAction><SubRuleAction><Id>2</Id><ActionId>1</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant" /><FilterEnabled>true</FilterEnabled></SubRuleAction></SubRuleActions></SubRule><SubRule><Id>ff32c0f5-5d1e-4726-9de7-ea95dc99c3ed</Id><ItsId>1</ItsId><ShiftTypeId>0</ShiftTypeId><Filter>IsCallExpiredWithResourceLoggedIn(15)</Filter><FilterEnabled>true</FilterEnabled><Description>Keep personal assignment while interviewer logged in</Description><SubRuleActions><SubRuleAction><Id>3</Id><ActionId>43</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant" /><FilterEnabled>true</FilterEnabled></SubRuleAction></SubRuleActions></SubRule><SubRule><Id>7a3d1db9-53e6-4047-93ce-17f748d687e9</Id><ItsId>1</ItsId><ShiftTypeId>0</ShiftTypeId><Filter>IsCallExpired()==1</Filter><FilterEnabled>true</FilterEnabled><Description>Drop personal assignments if they cannot be serviced</Description><SubRuleActions><SubRuleAction><Id>1</Id><ActionId>30</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">-3</ParameterValue><FilterEnabled>true</FilterEnabled></SubRuleAction><SubRuleAction><Id>2</Id><ActionId>27</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">2000</ParameterValue><FilterEnabled>true</FilterEnabled></SubRuleAction><SubRuleAction><Id>3</Id><ActionId>8</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">1</ParameterValue><FilterEnabled>true</FilterEnabled></SubRuleAction></SubRuleActions></SubRule><SubRule><Id>411ed219-00d2-4c40-8cb4-59801cc07e78</Id><ItsId>33</ItsId><ShiftTypeId>0</ShiftTypeId><Filter>IsCallExpired()==1</Filter><FilterEnabled>true</FilterEnabled><Description>Expired Soft Appointment call. Drop personal assignments if they cannot be serviced</Description><SubRuleActions><SubRuleAction><Id>1</Id><ActionId>30</ActionId><Filter  /><Enabled>true</Enabled><Description  /><ParameterValue Type="Constant">-3</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction><SubRuleAction><Id>2</Id><ActionId>27</ActionId><Filter  /><Enabled>true</Enabled><Description  /><ParameterValue Type="Constant">1500</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction><SubRuleAction><Id>3</Id><ActionId>8</ActionId><Filter  /><Enabled>true</Enabled><Description  /><ParameterValue Type="Constant">1</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction></SubRuleActions></SubRule><SubRule><Id>de32fb76-39e8-4b25-bd32-b4c92a9cf0ff</Id><ItsId>1</ItsId><ShiftTypeId>0</ShiftTypeId><Filter /><FilterEnabled>false</FilterEnabled><Description>Hard Appointment</Description><SubRuleActions><SubRuleAction><Id>1</Id><ActionId>27</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">1000</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction><SubRuleAction><Id>2</Id><ActionId>5</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">0</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction><SubRuleAction><Id>3</Id><ActionId>30</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">-2</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction><SubRuleAction><Id>4</Id><ActionId>31</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">1</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction><SubRuleAction><Id>5</Id><ActionId>38</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">2</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction></SubRuleActions></SubRule><SubRule><Id>47df1d36-da64-403d-9cc0-0c74822b94bd</Id><ItsId>33</ItsId><ShiftTypeId>0</ShiftTypeId><Filter /><FilterEnabled>false</FilterEnabled><Description>Soft Appointment</Description><SubRuleActions><SubRuleAction><Id>1</Id><ActionId>5</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">0</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction><SubRuleAction><Id>2</Id><ActionId>27</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">500</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction><SubRuleAction><Id>3</Id><ActionId>30</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">-3</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction></SubRuleActions></SubRule><SubRule><Id>6cdc0632-15a2-4f4a-baf9-328eb9bb3b31</Id><ItsId>2</ItsId><ShiftTypeId>0</ShiftTypeId><Filter /><FilterEnabled>false</FilterEnabled><Description>Retry busy numbers</Description><SubRuleActions><SubRuleAction><Id>1</Id><ActionId>2</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Parameter">1</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction></SubRuleActions></SubRule><SubRule><Id>14aa9b11-236d-4473-8043-3557f9853c86</Id><ItsId>3</ItsId><ShiftTypeId>0</ShiftTypeId><Filter /><FilterEnabled>false</FilterEnabled><Description>Retry no reply numbers</Description><SubRuleActions><SubRuleAction><Id>1</Id><ActionId>3</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">1</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction></SubRuleActions></SubRule><SubRule><Id>0d2081bd-80cd-4a0e-b3c3-70533863a712</Id><ItsId>16</ItsId><ShiftTypeId>0</ShiftTypeId><Filter /><FilterEnabled>false</FilterEnabled><Description>Never attempted interviews</Description><SubRuleActions><SubRuleAction><Id>1</Id><ActionId>8</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">1</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction></SubRuleActions></SubRule><SubRule><Id>9523c6a2-426c-4fde-b524-15ec87ba83c0</Id><ItsId>15</ItsId><ShiftTypeId>0</ShiftTypeId><Filter /><FilterEnabled>false</FilterEnabled><Description>Restore call properties for numbers returned by the dialer</Description><SubRuleActions><SubRuleAction><Id>1</Id><ActionId>43</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant" /><FilterEnabled>false</FilterEnabled></SubRuleAction><SubRuleAction><Id>2</Id><ActionId>27</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">333</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction></SubRuleActions></SubRule><SubRule><Id>f4cd6f93-afe9-4a84-9bd5-0f18e37c1c3c</Id><ItsId>25</ItsId><ShiftTypeId>0</ShiftTypeId><Filter /><FilterEnabled>false</FilterEnabled><Description>Restore call properties for numbers returned by the dialer</Description><SubRuleActions><SubRuleAction><Id>1</Id><ActionId>43</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant" /><FilterEnabled>false</FilterEnabled></SubRuleAction><SubRuleAction><Id>2</Id><ActionId>27</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">666</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction></SubRuleActions></SubRule><SubRule><Id>86073911-86f6-47d6-9589-b73ddad9ea00</Id><ItsId>28</ItsId><ShiftTypeId>0</ShiftTypeId><Filter /><FilterEnabled>false</FilterEnabled><Description>Stopped to be resumed interviews</Description><SubRuleActions><SubRuleAction><Id>1</Id><ActionId>5</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">0</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction><SubRuleAction><Id>2</Id><ActionId>27</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">400</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction><SubRuleAction><Id>3</Id><ActionId>30</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">-3</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction></SubRuleActions></SubRule><SubRule><Id>ad7ab6ca-327f-44b5-91f0-e9d016365b3c</Id><ItsId>1000</ItsId><ShiftTypeId>0</ShiftTypeId><Filter /><FilterEnabled>false</FilterEnabled><Description>Accept inbound calls</Description><SubRuleActions><SubRuleAction><Id>2</Id><ActionId>27</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">3000</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction><SubRuleAction><Id>3</Id><ActionId>9</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant">SaveCliNumber</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction><SubRuleAction><Id>1</Id><ActionId>44</ActionId><Filter /><Enabled>true</Enabled><Description /><ParameterValue Type="Constant" /><FilterEnabled>false</FilterEnabled></SubRuleAction></SubRuleActions></SubRule></SubRules></Rule></Rules><ShiftTypes><ShiftType><Id>1</Id><Name>24/7</Name><ColorInt>-16751616</ColorInt></ShiftType></ShiftTypes><Shifts><Shift><Id>1</Id><ShiftTypeId>1</ShiftTypeId><Timezones><Timezone><Id xsi:nil="true" /><Data><StartDayOfWeek>Monday</StartDayOfWeek><StartTime>00:00:00</StartTime><EndDayOfWeek>Tuesday</EndDayOfWeek><EndTime>00:00:00</EndTime></Data></Timezone></Timezones></Shift><Shift><Id>2</Id><ShiftTypeId>1</ShiftTypeId><Timezones><Timezone><Id xsi:nil="true" /><Data><StartDayOfWeek>Tuesday</StartDayOfWeek><StartTime>00:00:00</StartTime><EndDayOfWeek>Wednesday</EndDayOfWeek><EndTime>00:00:00</EndTime></Data></Timezone></Timezones></Shift><Shift><Id>3</Id><ShiftTypeId>1</ShiftTypeId><Timezones><Timezone><Id xsi:nil="true" /><Data><StartDayOfWeek>Wednesday</StartDayOfWeek><StartTime>00:00:00</StartTime><EndDayOfWeek>Thursday</EndDayOfWeek><EndTime>00:00:00</EndTime></Data></Timezone></Timezones></Shift><Shift><Id>4</Id><ShiftTypeId>1</ShiftTypeId><Timezones><Timezone><Id xsi:nil="true" /><Data><StartDayOfWeek>Thursday</StartDayOfWeek><StartTime>00:00:00</StartTime><EndDayOfWeek>Friday</EndDayOfWeek><EndTime>00:00:00</EndTime></Data></Timezone></Timezones></Shift><Shift><Id>5</Id><ShiftTypeId>1</ShiftTypeId><Timezones><Timezone><Id xsi:nil="true" /><Data><StartDayOfWeek>Friday</StartDayOfWeek><StartTime>00:00:00</StartTime><EndDayOfWeek>Saturday</EndDayOfWeek><EndTime>00:00:00</EndTime></Data></Timezone></Timezones></Shift><Shift><Id>6</Id><ShiftTypeId>1</ShiftTypeId><Timezones><Timezone><Id xsi:nil="true" /><Data><StartDayOfWeek>Saturday</StartDayOfWeek><StartTime>00:00:00</StartTime><EndDayOfWeek>Sunday</EndDayOfWeek><EndTime>00:00:00</EndTime></Data></Timezone></Timezones></Shift><Shift><Id>7</Id><ShiftTypeId>1</ShiftTypeId><Timezones><Timezone><Id xsi:nil="true" /><Data><StartDayOfWeek>Sunday</StartDayOfWeek><StartTime>00:00:00</StartTime><EndDayOfWeek>Monday</EndDayOfWeek><EndTime>00:00:00</EndTime></Data></Timezone></Timezones></Shift></Shifts><Exclusions /><CustomParameters><CustomParameter><Id>1</Id><Name>BusyRetry</Name><Description>Minutes to wait until retrying a busy number</Description><Type>Integer</Type><Value>20</Value></CustomParameter><CustomParameter><Id>2</Id><Name>MaxCall</Name><Description>Maximum number of call attempts</Description><Type>Integer</Type><Value>99</Value></CustomParameter></CustomParameters><CustomScript><Id>1</Id><LanguageName>JScript.Net</LanguageName><Body>/** Saves the inbound call CLI number provided by the dialer to the ''TelephoneNumber'' field */ 
function SaveCliNumber() {
  SetRespondentValue(''TelephoneNumber'', Scheduling.CliNumber); 
}
</Body></CustomScript></Schedule>'

INSERT INTO [dbo].[BvSchedule]
    ([ScheduleID]
    ,[XmlInUse]
    ,[XmlUnderDev]
    ,[ScriptSource]
    ,[Name]
    ,[CreateDate]
    ,[ModifyDate]
    ,[RegenerateIsRequired]
    ,[DesignStateGroupID]
    ,[IsSampleUpdateRuleSet])
VALUES
    (@DefaultScheduleID
    ,''
    ,@DefaultScheduleXml
    ,NULL
    ,'Default Schedule'
    ,GETUTCDATE()
    ,GETUTCDATE()
    ,1
    ,NULL
    ,0)

DECLARE @DefaultScheduleBackupID INT
EXEC @DefaultScheduleBackupID = BvSpGetNewSID

INSERT INTO [dbo].[BvSchedule]
    ([ScheduleID]
    ,[XmlInUse]
    ,[XmlUnderDev]
    ,[ScriptSource]
    ,[Name]
    ,[CreateDate]
    ,[ModifyDate]
    ,[RegenerateIsRequired]
    ,[DesignStateGroupID]
    ,[IsSampleUpdateRuleSet])
VALUES
    (@DefaultScheduleBackupID
    ,''
    ,@DefaultScheduleXml
    ,NULL
    ,'Default Schedule Backup'
    ,GETUTCDATE()
    ,GETUTCDATE()
    ,1
    ,NULL
    ,0)

PRINT 'Loading information about reports...'
GO

INSERT INTO BvReport VALUES (2,  'Sample Status Summary',   'SampleStatusSummary.rpt',      'bv7rptu.dll')
INSERT INTO BvReport VALUES (2,  'Sample Disposition',      'SampleDisposition.rpt',      'bv7prodrptu.dll')
INSERT INTO BvReport VALUES (2,  'Survey/Person',           'SurveyPersonReport.rpt',       'bv7rptu.dll')
INSERT INTO BvReport VALUES (2,  'Production By Interviewer',   'SurveyProductionByInterviewer.rpt', 'bv7prodrptu.dll')
INSERT INTO BvReport VALUES (2,  'Production Details',      'ProductionDetails.rpt',      'bv7prodrptu.dll')
INSERT INTO BvReport VALUES (2,  'Time Outcome',         'SurveyTimeOutcome.rpt',      'bv7prodrptu.dll')
INSERT INTO BvReport VALUES (2,  'Summary of Openends',     'Summary_of_Openends.rpt',      'bv7rptu.dll')
INSERT INTO BvReport VALUES (10, 'Survey/Person',           'SurveyPersonReport.rpt',       'bv7rptu.dll')
INSERT INTO BvReport VALUES (10, 'Person Production',    'InterviewerProduction.rpt',    'bv7prodrptu.dll')
INSERT INTO BvReport VALUES (10, 'Time Outcome',         'PersonTimeOutcome.rpt',      'bv7prodrptu.dll')
INSERT INTO BvReport VALUES (12, 'Production Summary',      'ProductionSummary.rpt',      'bv7prodrptu.dll')
INSERT INTO BvReport VALUES (12, 'Surveys By Outcome',      'SurveysByOutcome.rpt',       'bv7prodrptu.dll')
GO

PRINT 'Loading information about RD property'
GO

GO

-- insert 0 
INSERT INTO BvTransferBatches VALUES( 0 )
GO

-------------------------------------------------------------------------------
-- UPDATE FOR PERSON GROUPS
-------------------------------------------------------------------------------
declare cr cursor local for select SID from BvPersonGroup
declare @sid int

    open cr

    fetch next from cr into @sid
    while ( @@FETCH_STATUS = 0 )
    begin
        exec BvSpPerson_SpinUp @sid
        fetch next from cr into @sid
    end

    close cr
    deallocate cr
GO

INSERT INTO [BvThresholdTypes] VALUES(1, 'Task alert')
INSERT INTO [BvThresholdTypes] VALUES(2, 'SurvayActivityView.InterviewersLoggedCount alert')
INSERT INTO [BvThresholdTypes] VALUES(3, 'SurvayActivityView.NextAppointmentTime alert')
INSERT INTO [BvThresholdTypes] VALUES(4, 'SurvayActivityView.TotalSampleSize alert')
INSERT INTO [BvThresholdTypes] VALUES(6, 'SurvayActivityView.ScheduledCallsCount alert')
INSERT INTO [BvThresholdTypes] VALUES(7, 'SurvayActivityView.SuspendedCallsCount alert')
INSERT INTO [BvThresholdTypes] VALUES(8, 'SurvayActivityView.MinutesSpentWorkingOnSurvey alert')
INSERT INTO [BvThresholdTypes] VALUES(9, 'SurvayActivityView.AssignedInterviewersCount alert')
INSERT INTO [BvThresholdTypes] VALUES(10, 'SurvayActivityView.StrikeRate alert')
INSERT INTO [BvThresholdTypes] VALUES(11, 'SurvayActivityView.CountCalls alert')
INSERT INTO [BvThresholdTypes] VALUES(12, 'SystemWideInfo.LoggedInterviewersCount')
INSERT INTO [BvThresholdTypes] VALUES(13, 'SystemWideInfo.OpenSurveysCount')
INSERT INTO [BvThresholdTypes] VALUES(14, 'SystemWideInfo.CallsCount')
INSERT INTO [BvThresholdTypes] VALUES(15, 'AppointmentList alert')
INSERT INTO [BvThresholdTypes] VALUES(16, 'TasksAlert.LastKeepAliveTime alert')
INSERT INTO [BvThresholdTypes] VALUES(17, 'QuickAnswerSubmission alert')
INSERT INTO [BvThresholdTypes] VALUES(18, 'TasksAlert.NoActivity alert')
INSERT INTO [BvThresholdTypes] VALUES(19, 'TasksAlert.InterviewDuration alert')
INSERT INTO [BvThresholdTypes] VALUES(20, 'TasksAlert.BreakDuration alert')

GO

INSERT INTO [BvConfirmitStatus] VALUES( 'complete', 'Complete', 	13 )
INSERT INTO [BvConfirmitStatus] VALUES( 'screened', 'Screened', 	14 )
INSERT INTO [BvConfirmitStatus] VALUES( 'quotafull','Quota Full', 	4 )
INSERT INTO [BvConfirmitStatus] VALUES( 'error',    'Error', 	    30 )
INSERT INTO [BvConfirmitStatus] VALUES( NULL,       'Incomplete', 	1 )
INSERT INTO [BvConfirmitStatus] VALUES( '1', 'Appointment', 1 )
INSERT INTO [BvConfirmitStatus] VALUES( '2', 'Busy', 2 )
INSERT INTO [BvConfirmitStatus] VALUES( '3', 'No reply', 3 )
INSERT INTO [BvConfirmitStatus] VALUES( '4', 'Quota failure', 4 )
INSERT INTO [BvConfirmitStatus] VALUES( '5', 'Refusal', 5 )
INSERT INTO [BvConfirmitStatus] VALUES( '6', 'Terminated', 6 )
INSERT INTO [BvConfirmitStatus] VALUES( '7', 'Answer phone', 7 )
INSERT INTO [BvConfirmitStatus] VALUES( '8', 'Modem', 8 )
INSERT INTO [BvConfirmitStatus] VALUES( '9', 'Fax', 9 )
INSERT INTO [BvConfirmitStatus] VALUES( '10', 'Congestion', 10 )
INSERT INTO [BvConfirmitStatus] VALUES( '11', 'Unobtainable', 11 )
INSERT INTO [BvConfirmitStatus] VALUES( '12', 'Nuisance', 12 )
INSERT INTO [BvConfirmitStatus] VALUES( '13', 'Completed', 13 )
INSERT INTO [BvConfirmitStatus] VALUES( '14', 'Screened', 14 )
INSERT INTO [BvConfirmitStatus] VALUES( '15', 'Returned not dialled', 15 )
INSERT INTO [BvConfirmitStatus] VALUES( '16', 'Fresh sample', 16 )
INSERT INTO [BvConfirmitStatus] VALUES( '17', 'Blacklist', 17 )
INSERT INTO [BvConfirmitStatus] VALUES( '18', 'Not automatically dialled (ie manual dialling)', 18 )
INSERT INTO [BvConfirmitStatus] VALUES( '19', 'Status not sensed', 19 )
INSERT INTO [BvConfirmitStatus] VALUES( '20', 'Transfer to Web', 20 )
INSERT INTO [BvConfirmitStatus] VALUES( '21', 'Transfer to CATI', 21 )
INSERT INTO [BvConfirmitStatus] VALUES( '22', 'Transfer to CAPI', 22 )
INSERT INTO [BvConfirmitStatus] VALUES( '23', 'Transfer to IVR', 23 )
INSERT INTO [BvConfirmitStatus] VALUES( '24', 'Interrupted by interviewer', 24 )
INSERT INTO [BvConfirmitStatus] VALUES( '25', 'Returned dialler expired', 25 )
INSERT INTO [BvConfirmitStatus] VALUES( '26', 'Interrupted by system', 26 )
INSERT INTO [BvConfirmitStatus] VALUES( '27', 'Filtered by call delivery', 27 )
INSERT INTO [BvConfirmitStatus] VALUES( '28', 'Stopped', 28 )
INSERT INTO [BvConfirmitStatus] VALUES( '29', 'Telephony failure', 29 )
INSERT INTO [BvConfirmitStatus] VALUES( '30', 'Error', 30 )
INSERT INTO [BvConfirmitStatus] VALUES( '31', 'Custom1', 31 )
INSERT INTO [BvConfirmitStatus] VALUES( '32', 'Custom2', 32 )
INSERT INTO [BvConfirmitStatus] VALUES( '33', 'Custom3', 33 )
INSERT INTO [BvConfirmitStatus] VALUES( '34', 'Custom4', 34 )
INSERT INTO [BvConfirmitStatus] VALUES( '35', 'Custom5', 35 )
INSERT INTO [BvConfirmitStatus] VALUES( '36', 'Custom6', 36 )
INSERT INTO [BvConfirmitStatus] VALUES( '37', 'Custom7', 37 )
INSERT INTO [BvConfirmitStatus] VALUES( '38', 'Custom8', 38 )
INSERT INTO [BvConfirmitStatus] VALUES( '39', 'Custom9', 39 )
INSERT INTO [BvConfirmitStatus] VALUES( '40', 'Custom10', 40 )
INSERT INTO [BvConfirmitStatus] VALUES( '41', 'Custom11', 41 )
INSERT INTO [BvConfirmitStatus] VALUES( '42', 'Custom12', 42 )
INSERT INTO [BvConfirmitStatus] VALUES( '43', 'Custom13', 43 )
INSERT INTO [BvConfirmitStatus] VALUES( '44', 'Custom14', 44 )
INSERT INTO [BvConfirmitStatus] VALUES( '45', 'Custom15', 45 )
INSERT INTO [BvConfirmitStatus] VALUES( '46', 'Custom16', 46 )
INSERT INTO [BvConfirmitStatus] VALUES( '47', 'Custom17', 47 )
INSERT INTO [BvConfirmitStatus] VALUES( '48', 'Custom18', 48 )
INSERT INTO [BvConfirmitStatus] VALUES( '49', 'Custom19', 49 )
INSERT INTO [BvConfirmitStatus] VALUES( '50', 'Custom20', 50 )
INSERT INTO [BvConfirmitStatus] VALUES( '51', 'Custom21', 51 )
INSERT INTO [BvConfirmitStatus] VALUES( '52', 'Custom22', 52 )
INSERT INTO [BvConfirmitStatus] VALUES( '53', 'Custom23', 53 )
INSERT INTO [BvConfirmitStatus] VALUES( '54', 'Custom24', 54 )
INSERT INTO [BvConfirmitStatus] VALUES( '55', 'Custom25', 55 )
INSERT INTO [BvConfirmitStatus] VALUES( '56', 'Custom26', 56 )
INSERT INTO [BvConfirmitStatus] VALUES( '57', 'Custom27', 57 )
INSERT INTO [BvConfirmitStatus] VALUES( '58', 'Custom28', 58 )
INSERT INTO [BvConfirmitStatus] VALUES( '59', 'Custom29', 59 )
INSERT INTO [BvConfirmitStatus] VALUES( '60', 'Custom30', 60 )
INSERT INTO [BvConfirmitStatus] VALUES( '61', 'Custom31', 61 )
INSERT INTO [BvConfirmitStatus] VALUES( '62', 'Custom32', 62 )
INSERT INTO [BvConfirmitStatus] VALUES( '63', 'Custom33', 63 )
INSERT INTO [BvConfirmitStatus] VALUES( '64', 'Custom34', 64 )
INSERT INTO [BvConfirmitStatus] VALUES( '65', 'Custom35', 65 )
INSERT INTO [BvConfirmitStatus] VALUES( '66', 'Custom36', 66 )
INSERT INTO [BvConfirmitStatus] VALUES( '67', 'Custom37', 67 )
INSERT INTO [BvConfirmitStatus] VALUES( '68', 'Custom38', 68 )
INSERT INTO [BvConfirmitStatus] VALUES( '69', 'Custom39', 69 )
INSERT INTO [BvConfirmitStatus] VALUES( '70', 'Custom40', 70 )
INSERT INTO [BvConfirmitStatus] VALUES( '71', 'Custom41', 71 )
INSERT INTO [BvConfirmitStatus] VALUES( '72', 'Custom42', 72 )
INSERT INTO [BvConfirmitStatus] VALUES( '73', 'Custom43', 73 )
INSERT INTO [BvConfirmitStatus] VALUES( '74', 'Custom44', 74 )
INSERT INTO [BvConfirmitStatus] VALUES( '75', 'Custom45', 75 )
INSERT INTO [BvConfirmitStatus] VALUES( '76', 'Custom46', 76 )
INSERT INTO [BvConfirmitStatus] VALUES( '77', 'Custom47', 77 )
INSERT INTO [BvConfirmitStatus] VALUES( '78', 'Custom48', 78 )
INSERT INTO [BvConfirmitStatus] VALUES( '79', 'Custom49', 79 )
INSERT INTO [BvConfirmitStatus] VALUES( '80', 'Custom50', 80 )
INSERT INTO [BvConfirmitStatus] VALUES( '81', 'Custom51', 81 )
INSERT INTO [BvConfirmitStatus] VALUES( '82', 'Custom52', 82 )
INSERT INTO [BvConfirmitStatus] VALUES( '83', 'Custom53', 83 )
INSERT INTO [BvConfirmitStatus] VALUES( '84', 'Custom54', 84 )
INSERT INTO [BvConfirmitStatus] VALUES( '85', 'Custom55', 85 )
INSERT INTO [BvConfirmitStatus] VALUES( '86', 'Custom56', 86 )
INSERT INTO [BvConfirmitStatus] VALUES( '87', 'Custom57', 87 )
INSERT INTO [BvConfirmitStatus] VALUES( '88', 'Custom58', 88 )
INSERT INTO [BvConfirmitStatus] VALUES( '89', 'Custom59', 89 )
INSERT INTO [BvConfirmitStatus] VALUES( '90', 'Custom60', 90 )
INSERT INTO [BvConfirmitStatus] VALUES( '91', 'Custom61', 91 )
INSERT INTO [BvConfirmitStatus] VALUES( '92', 'Custom62', 92 )
INSERT INTO [BvConfirmitStatus] VALUES( '93', 'Custom63', 93 )
INSERT INTO [BvConfirmitStatus] VALUES( '94', 'Custom64', 94 )
INSERT INTO [BvConfirmitStatus] VALUES( '95', 'Custom65', 95 )
INSERT INTO [BvConfirmitStatus] VALUES( '96', 'Custom66', 96 )
INSERT INTO [BvConfirmitStatus] VALUES( '97', 'Custom67', 97 )
INSERT INTO [BvConfirmitStatus] VALUES( '98', 'Custom68', 98 )
INSERT INTO [BvConfirmitStatus] VALUES( '99', 'Custom69', 99 )
INSERT INTO [BvConfirmitStatus] VALUES( '100', 'Custom70', 100 )
INSERT INTO [BvConfirmitStatus] VALUES( '101', 'Custom71', 101 )
INSERT INTO [BvConfirmitStatus] VALUES( '102', 'Custom72', 102 )
INSERT INTO [BvConfirmitStatus] VALUES( '103', 'Custom73', 103 )
INSERT INTO [BvConfirmitStatus] VALUES( '104', 'Custom74', 104 )
INSERT INTO [BvConfirmitStatus] VALUES( '105', 'Custom75', 105 )
INSERT INTO [BvConfirmitStatus] VALUES( '106', 'Custom76', 106 )
INSERT INTO [BvConfirmitStatus] VALUES( '107', 'Custom77', 107 )
INSERT INTO [BvConfirmitStatus] VALUES( '108', 'Custom78', 108 )
INSERT INTO [BvConfirmitStatus] VALUES( '109', 'Custom79', 109 )
INSERT INTO [BvConfirmitStatus] VALUES( '110', 'Custom80', 110 )
INSERT INTO [BvConfirmitStatus] VALUES( '111', 'Custom81', 111 )
INSERT INTO [BvConfirmitStatus] VALUES( '112', 'Custom82', 112 )
INSERT INTO [BvConfirmitStatus] VALUES( '113', 'Custom83', 113 )
INSERT INTO [BvConfirmitStatus] VALUES( '114', 'Custom84', 114 )
INSERT INTO [BvConfirmitStatus] VALUES( '115', 'Custom85', 115 )
INSERT INTO [BvConfirmitStatus] VALUES( '116', 'Custom86', 116 )
INSERT INTO [BvConfirmitStatus] VALUES( '117', 'Custom87', 117 )
INSERT INTO [BvConfirmitStatus] VALUES( '118', 'Custom88', 118 )
INSERT INTO [BvConfirmitStatus] VALUES( '119', 'Custom89', 119 )
INSERT INTO [BvConfirmitStatus] VALUES( '120', 'Custom90', 120 )
INSERT INTO [BvConfirmitStatus] VALUES( '1051', 'SurveyScriptError', 1051 )
INSERT INTO [BvConfirmitStatus] VALUES( '1010', 'Internal Transfer', 1010 )
INSERT INTO [BvConfirmitStatus] VALUES( '1000', 'Inbound Call', 1000 )
INSERT INTO [BvConfirmitStatus] VALUES( '1001', 'Dropped by respondent', 1001 )
INSERT INTO [BvConfirmitStatus] VALUES( '1020', 'Dial interrupted by interviewer', 1020 )
INSERT INTO [BvConfirmitStatus] VALUES( '1011', 'External Transfer', 1011 )
INSERT INTO [BvConfirmitStatus] VALUES( '1012', 'Canceled Transfer', 1012 )
INSERT INTO [BvConfirmitStatus] VALUES( '1021', 'Externally validated number', 1021 )
INSERT INTO [BvConfirmitStatus] VALUES( '1052', 'Synchronized Sample', 1052 )
GO

INSERT INTO BvSurveyListAlertsViewConfiguration VALUES(15, NULL, 3600, NULL, 60, 0, 3600)
GO

INSERT INTO BvDialType(ID, Name) VALUES( 0, 'Landline' )
INSERT INTO BvDialType(ID, Name) VALUES( 1, 'Cellphone' )
INSERT INTO BvDialType(ID, Name) VALUES( 2, 'Assisted' )

INSERT INTO BvBreakType (Name, Description, IsPaid) VALUES( 'Break', 'Default break', 1)

GO

INSERT INTO [dbo].[BvInterviewerProductivityReportTemplate] VALUES 
('System template', '2019-01-01 00:00:00.000', 'System', 'system', '2019-01-01 00:00:00.000', 2, 0, 1, 0, 1, 0, 0, 0,
'<Columns xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <ProductivityReportTemplateColumn>
    <DisplayName>User ID</DisplayName>
    <StandardColumnName>PersonId</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>User name</DisplayName>
    <StandardColumnName>PersonName</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>Log on time (hours)</DisplayName>
    <StandardColumnName>LogOnHours</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>Waiting time (hours)</DisplayName>
    <StandardColumnName>WaitingHours</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>Paid break time (hours)</DisplayName>
    <StandardColumnName>BreakHoursPaid</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>Unpaid break time (hours)</DisplayName>
    <StandardColumnName>BreakHoursUnpaid</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>Review Time (hours)</DisplayName>
    <StandardColumnName>OpenEndReviewHours</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn xsi:type="ProductivityReportTemplateColumnWithStatuses">
    <DisplayName>Interviews</DisplayName>
    <StandardColumnName>DialingsCount</StandardColumnName>
    <Visible>true</Visible>
    <IsIncludeStatuses>false</IsIncludeStatuses>
    <ExtendedStatuses />
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>Interviews per log on hour</DisplayName>
    <StandardColumnName>DialingsPerLogOnHours</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn xsi:type="ProductivityReportTemplateColumnWithStatuses">
    <DisplayName>Completes</DisplayName>
    <StandardColumnName>Completes</StandardColumnName>
    <Visible>true</Visible>
    <IsIncludeStatuses>true</IsIncludeStatuses>
    <ExtendedStatuses>
      <int>13</int>
    </ExtendedStatuses>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>Completes per log on hour</DisplayName>
    <StandardColumnName>CompletesPerLogOnHours</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>Interviews per complete</DisplayName>
    <StandardColumnName>DialingsPerComplete</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>Average completed interview length (min)</DisplayName>
    <StandardColumnName>AverageDuration</StandardColumnName>
  </ProductivityReportTemplateColumn>
</Columns>')

PRINT 'All done.'

