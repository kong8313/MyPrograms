GO
PRINT N'Altering [dbo].[BvTrBvHistory_HistoryInsert]...';


GO
ALTER TRIGGER [BvTrBvHistory_HistoryInsert] ON [dbo].[BvHistory]
FOR INSERT
AS 
BEGIN
	SET NOCOUNT ON
		
	INSERT INTO [BvAggregateSurveyDelta]
		SELECT 
			/*[SID]*/ SurveyId,
			/*[ScheduledCallsCount]*/ 0,
			/*[SuspendedCallsCount]*/ 0,
			/*[MinutesSpentWorkingOnSurvey]*/ ISNULL(SUM(WaitingTime), 0) + ISNULL(SUM(ISNULL(Duration, ConfirmitDuration)), 0) MinutesSpentWorkingOnSurvey
		FROM inserted
		WHERE RoleId = 2
		GROUP BY SurveyId

	INSERT INTO BvHistoryDelta(SurveyId, PersonId, ITS, LogonTime, WaitingTime, FiredTime)
	SELECT 
			SurveyId,
			PersonSID,
			ISNULL(ITS, 0),
			ISNULL(WaitingTime, 0) + ISNULL(ISNULL(Duration, ConfirmitDuration), 0),
			ISNULL(WaitingTime, 0),
			FiredTime
	FROM inserted
	WHERE RoleId = 2
END
GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Update complete.';


GO
