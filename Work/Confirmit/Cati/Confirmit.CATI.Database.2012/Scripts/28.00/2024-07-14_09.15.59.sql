GO
PRINT N'Add Console.ManualDialTypeSelection and Console.EnforceManualSelectionForCellPhonePerson system settings';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
            (
			SELECT 'Console.Metrics.EnableInterviewerMetrics', 'When enabled, the interviewer will be able to open performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to open interviewer performance metrics', 3, 0, 'False'
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
            )
   INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
SELECT * FROM Data

END