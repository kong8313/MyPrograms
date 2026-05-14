PRINT 'Create temporaly ConvertToTimeSpanFormat function'
GO

CREATE FUNCTION dbo.ConvertToTimeSpanFormat( @Milliseconds BIGINT ) 
	RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Result NVARCHAR(MAX)
	
	;WITH parts as
	(
		SELECT	@Milliseconds / ( 1000 * 60 * 60 * 24 ) as Days,
				@Milliseconds % ( 1000 * 60 * 60 * 24 ) / (1000 * 60 * 60 ) as Hours,
				@Milliseconds % ( 1000 * 60 * 60  ) / ( 1000 * 60 ) as Minutes,
				@Milliseconds % ( 1000 * 60 ) / 1000 as Seconds,
				@Milliseconds % ( 1000 ) as Milliseconds
	)
	SELECT @Result =
		CASE WHEN Days > 0 THEN CAST(Days AS NVARCHAR(64)) + '.' 
			 ELSE '' END + 
		CASE WHEN Hours >= 10 THEN '' ELSE '0' 
			 END  + CAST(Hours AS NVARCHAR(64)) + ':' +
		CASE WHEN Minutes >= 10 THEN '' 
			 ELSE '0' END  + CAST(Minutes AS NVARCHAR(64)) + ':' +
		CASE WHEN Seconds >= 10 THEN '' 
			 ELSE '0' END  + CAST(Seconds AS NVARCHAR(64)) +
		CASE WHEN Milliseconds >= 100 THEN '.' + CAST(Milliseconds AS NVARCHAR(64))
			 WHEN 100 > Milliseconds AND Milliseconds >= 10 THEN '.0' + CAST(Milliseconds AS NVARCHAR(64))
			 WHEN 10 > Milliseconds AND Milliseconds > 0 THEN '.00' + CAST(Milliseconds AS NVARCHAR(64))
			 ELSE '' END
		FROM parts

	RETURN @Result
END
GO

PRINT 'Move several system settings to other place...'

PRINT 'Move QuotaBalancing.PromotionHistoryCleanPeriod setting to RoutineMaintenance.Actions.PromotionHistoryTableCleanup.ExpirationPeriod'

UPDATE BvSystemSettings 
	SET SystemName = 'RoutineMaintenance.Actions.PromotionHistoryTableCleanup.ExpirationPeriod',
		Value = Value  + '.00:00:00' --convert int type (days) to timespan
	WHERE SystemName = 'QuotaBalancing.PromotionHistoryCleanPeriod'

PRINT 'Move AnswerSubmissionAlert.AnswerSubmissionAlertHistoryCleanPeriod setting to RoutineMaintenance.Actions.AnswerSubmissionAlertHistoryTableCleanup.ExpirationPeriod'

UPDATE BvSystemSettings 
	SET SystemName = 'RoutineMaintenance.Actions.AnswerSubmissionAlertHistoryTableCleanup.ExpirationPeriod',
		Value = Value  + '.00:00:00' --convert int type (days) to timespan
	WHERE SystemName = 'AnswerSubmissionAlert.AnswerSubmissionAlertHistoryCleanPeriod'

PRINT 'Move DeferredMonitoring.DeferredMonitoringCleanupDelayBetweenDeletesInMs setting to RoutineMaintenance.Actions.PersonDeferredMonitoringTableCleanup.DelayBetweenDeletes'

UPDATE BvSystemSettings 
	SET SystemName = 'RoutineMaintenance.Actions.PersonDeferredMonitoringTableCleanup.DelayBetweenDeletes',
		Value = dbo.ConvertToTimeSpanFormat( Value ) --convert int type (milliseconds) to timespan
	WHERE SystemName = 'DeferredMonitoring.DeferredMonitoringCleanupDelayBetweenDeletesInMs'

PRINT 'Move DeferredMonitoring.DeferredMonitoringCleanupDeleteTopRows setting to RoutineMaintenance.Actions.PersonDeferredMonitoringTableCleanup.DeleteTopRows'

UPDATE BvSystemSettings 
	SET SystemName = 'RoutineMaintenance.Actions.PersonDeferredMonitoringTableCleanup.DeleteTopRows'
	WHERE SystemName = 'DeferredMonitoring.DeferredMonitoringCleanupDeleteTopRows'

PRINT 'Move DeferredMonitoring.DeferredRecordsExpirationPeriodInDays setting to RoutineMaintenance.Actions.PersonDeferredMonitoringTableCleanup.ExpirationPeriod'

UPDATE BvSystemSettings 
	SET SystemName = 'RoutineMaintenance.Actions.PersonDeferredMonitoringTableCleanup.ExpirationPeriod',
		Value = Value  + '.00:00:00' --convert int type (days) to timespan
	WHERE SystemName = 'DeferredMonitoring.DeferredRecordsExpirationPeriodInDays'

PRINT 'Move SurveyCleanup.NotificationTimeout setting to RoutineMaintenance.Actions.SurveyCleanup.NotificationTimeout'

UPDATE BvSystemSettings 
	SET SystemName = 'RoutineMaintenance.Actions.SurveyCleanup.NotificationTimeout'
	WHERE SystemName = 'SurveyCleanup.NotificationTimeout'

PRINT 'Move SurveyCleanup.CleanupTimeout setting to RoutineMaintenance.Actions.SurveyCleanup.CleanupTimeout'

UPDATE BvSystemSettings 
	SET SystemName = 'RoutineMaintenance.Actions.SurveyCleanup.CleanupTimeout'
	WHERE SystemName = 'SurveyCleanup.CleanupTimeout'

PRINT 'Delete DeferredMonitoring.DeferredMonitoringCleanupRunPeriodInMinutes setting'

DELETE FROM BvSystemSettings 
	WHERE SystemName = 'DeferredMonitoring.DeferredMonitoringCleanupRunPeriodInMinutes'

PRINT 'Delete DeferredMonitoring.DeferredRecordsAudioObtainingPeriodInHours setting'

DELETE FROM BvSystemSettings 
	WHERE SystemName = 'DeferredMonitoring.DeferredRecordsAudioObtainingPeriodInHours'

PRINT 'Delete DeferredMonitoring.EnableDeferredRecordsCleanup setting'

DELETE FROM BvSystemSettings 
	WHERE SystemName = 'DeferredMonitoring.EnableDeferredRecordsCleanup'

PRINT 'Delete RoutineMaintenance.WeeklyTime setting'

DELETE FROM BvSystemSettings 
	WHERE SystemName = 'RoutineMaintenance.WeeklyTime'

GO

PRINT 'Drop temporaly ConvertToTimeSpanFormat function'
DROP FUNCTION [dbo].ConvertToTimeSpanFormat
GO

DECLARE @IsDefaultDDatabase BIT = CASE WHEN DB_NAME() = 'ConfirmitCATIV15' OR DB_NAME() LIKE 'ConfirmitCATIV15TEST%' THEN 1 ELSE 0 END

;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
(
SELECT 'RoutineMaintenance.DailyShiftStartTime', 'Daily time of routine maintenance', 'Supervisor', 'The daily time at which the routine maintenance starts.', 4, 0, '0.00:00:00'
UNION ALL 
SELECT 'RoutineMaintenance.Duration', 'Duration of routine maintenance', 'Supervisor', 'Routine maintenance duration.', 4, 0, '0.03:00:00'
UNION ALL 
SELECT 'RoutineMaintenance.WeeklyShiftDayNumber', 'Number of daily shift', 'Supervisor', 'Days offset from the start of the week when database cleanup starts. Cleanup time based on the DailyShiftStartTime.', 1, 0, '5'
UNION ALL 
SELECT 'RoutineMaintenance.MonthlyShiftWeekNumber', 'Number of weekly shift', 'Supervisor', 'Weeks offset from the start of the month when database cleanup starts. Cleanup time based on the DailyShiftStartTime.', 1, 0, '1'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.RebuildIndexes.ShiftType', 'Maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '2'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.RebuildIndexes.Ignored', 'List of ignored indexes', 'Supervisor', 'List of ignored system indexes that will not be rebuilded.', 2, 0, NULL
UNION ALL 
SELECT 'RoutineMaintenance.Actions.RebuildIndexes.FragmentationDetectMode', 'Fragmentation detect mode', 'Supervisor', 'Is the name of the mode. mode specifies the scan level that is used to obtain statistics. mode is sysname. Valid inputs are DEFAULT, NULL, LIMITED, SAMPLED, or DETAILED. The default (NULL) is LIMITED.', 2, 0, 'SAMPLED'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.RebuildIndexes.FragmentationReorganizeThreshold', 'Fragmentation reorganize threshold', 'Supervisor', 'Reorganize index fragmentation threshold. If the index greater than the value of the threshold, we need to reorganize it.', 1, 0, '10'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.RebuildIndexes.FragmentationRebuildThreshold', 'Fragmentation rebuild threshold', 'Supervisor', 'Rebuild index fragmentation threshold. If the index greater than the value of the threshold, we need to rebuild it.', 1, 0, '30'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.UpdateStatistics.ShiftType', 'Maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '2'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.UpdateStatistics.Updated', 'List of tables', 'Supervisor', 'A list of statistics(tables) that we need to update.', 2, 0, 'BvSvySchedule,BvInterview,BvPersonRel'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.PersonDeferredMonitoringTableCleanup.ShiftType', 'Maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.PersonDeferredMonitoringTableCleanup.ExpirationPeriod', 'Expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '30.00:00:00'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.PersonDeferredMonitoringTableCleanup.DelayBetweenDeletes', 'Delay between deletes', 'Supervisor', 'Delay (in ms) between deferred records portions deletion.', 4, 0, '0.00:00:00.000'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.PersonDeferredMonitoringTableCleanup.DeleteTopRows', 'Delete top rows', 'Supervisor', 'Max number of deferred records which to delete at a time.', 1, 0, '100'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.AnswerSubmissionAlertHistoryTableCleanup.ShiftType', 'Maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.AnswerSubmissionAlertHistoryTableCleanup.ExpirationPeriod', 'Expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '30.00:00:00'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.AsyncOperationQueueTableCleanup.ShiftType', 'Maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.AsyncOperationQueueTableCleanup.ExpirationPeriod', 'Expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '30.00:00:00'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.CallsSentToDialerTableCleanup.ShiftType', 'Maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.CallsSentToDialerTableCleanup.ExpirationPeriod', 'Expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '30.00:00:00'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.PromotionHistoryTableCleanup.ShiftType', 'Maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.PromotionHistoryTableCleanup.ExpirationPeriod', 'Expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '30.00:00:00'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.SurveyCleanup.ShiftType', 'Maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '2'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.SurveyCleanup.NotificationTimeout', 'Survey cleanup notification timeout', 'Supervisor', 'The time which passes after the warning notification was sent before the survey is really cleaned.', 4, 0, '10.00:00:00'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.SurveyCleanup.CleanupTimeout', 'Survey cleanup timeout', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '30.00:00:00'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.MessageTableCleanup.ShiftType', 'Maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
UNION ALL 
SELECT 'RoutineMaintenance.Actions.MessageTableCleanup.ExpirationPeriod', 'Expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '7.00:00:00'
)
MERGE BvSystemSettings as t
USING( select * from data ) AS s ([SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value])
ON t.[SystemName] = s.[SystemName]
WHEN NOT MATCHED AND @IsDefaultDDatabase = 1 THEN
	INSERT ( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) 
		VALUES( s.[SystemName], s.[DisplayName], s.[Group], s.[Description], s.[Type], s.[Hidden], s.[Value] ) 
WHEN MATCHED THEN
	UPDATE SET	[DisplayName] = s.[DisplayName],
				[Group] = s.[Group],
				[Description] = s.[Description],
				[Type] = s.[Type],
				[Hidden] = s.[Hidden];

GO
PRINT N'Creating [dbo].[BvSpAsyncOperationQueue_Cleanup]...';

GO


CREATE PROCEDURE [dbo].[BvSpAsyncOperationQueue_Cleanup]
	@State INT,
	@ExpirationDate DATETIME
AS
	DELETE FROM BvAsyncOperationQueue 
		WHERE State = @State AND COALESCE(FinishedDate, HeartBeat, StartedDate, QueuedDate) < @ExpirationDate
RETURN 0

GO
PRINT N'Update complete.';


GO
