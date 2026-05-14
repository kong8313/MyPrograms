PRINT N'Add AutoLogout.AutoLogoutWebConsole* settings';


GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
    SELECT 'AutoLogout.AutoLogoutWebConsoleThreadSleepPeriod', 'AutoLogout WebConsole Thread sleep period', 'Interviewing', 'Time interval between executions of a procedure that logs out interviewers using Browser-based CATI Console (BBCC) who lost a connection.', 4, 0, '0.00:01:00'
    UNION ALL
    SELECT 'AutoLogout.AutoLogoutWebConsoleTimeout', 'AutoLogout WebConsole timeout', 'Interviewing', 'Time interval to keep interviewer using the Browser-based CATI Console (BBCC) logged-in after losing the connection to the server. Interviewer will be automatically logged out after this interval.', 4, 0, '0.00:05:00'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END


GO
PRINT N'Update complete.';


GO
