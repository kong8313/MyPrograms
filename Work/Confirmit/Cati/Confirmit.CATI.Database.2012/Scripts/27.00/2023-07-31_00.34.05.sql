GO
PRINT N'Add Alerting system settings for waiting states';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%')
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
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
)
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO
