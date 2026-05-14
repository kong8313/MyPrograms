
GO
PRINT N'Creating [dbo].[BvDayAgregatedHistory]...';


GO
CREATE TABLE [dbo].[BvDayAgregatedHistory] (
    [SurveyId]      INT      NOT NULL,
    [PersonId]      INT      NOT NULL,
    [ITS]           INT      NOT NULL,
    [LogonTime]     INT      NOT NULL,
    [WaitingTime]   INT      NOT NULL,
    [DailingsCount] INT      NOT NULL,
    [StartTime]     DATETIME NOT NULL,
    CONSTRAINT [PK_BvDayAgregatedHistory] PRIMARY KEY CLUSTERED ([SurveyId] ASC, [PersonId] ASC, [ITS] ASC, [StartTime] ASC)
);


GO
PRINT N'Creating [dbo].[BvHistoryDelta]...';


GO
CREATE TABLE [dbo].[BvHistoryDelta] (
    [ID]          INT      IDENTITY (1, 1) NOT NULL,
    [SurveyId]    INT      NOT NULL,
    [PersonId]    INT      NOT NULL,
    [ITS]         INT      NOT NULL,
    [LogonTime]   INT      NOT NULL,
    [WaitingTime] INT      NOT NULL,
    [FiredTime]   DATETIME NOT NULL,
    CONSTRAINT [PK_BvHistoryDelta_Id] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[BvMonthAgregatedHistory]...';


GO
CREATE TABLE [dbo].[BvMonthAgregatedHistory] (
    [SurveyId]      INT      NOT NULL,
    [PersonId]      INT      NOT NULL,
    [ITS]           INT      NOT NULL,
    [LogonTime]     INT      NOT NULL,
    [WaitingTime]   INT      NOT NULL,
    [DailingsCount] INT      NOT NULL,
    [StartTime]     DATETIME NOT NULL,
    CONSTRAINT [PK_BvMonthAgregatedHistory] PRIMARY KEY CLUSTERED ([SurveyId] ASC, [PersonId] ASC, [ITS] ASC, [StartTime] ASC)
);


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
			ITS,
			ISNULL(WaitingTime, 0) + ISNULL(ISNULL(Duration, ConfirmitDuration), 0),
			ISNULL(WaitingTime, 0),
			FiredTime
	FROM inserted
	WHERE RoleId = 2
END
GO
PRINT N'Altering [dbo].[GetCallsForGroupForPredictiveSurvey]...';


GO
ALTER FUNCTION dbo.GetCallsForGroupForPredictiveSurvey
(
    @rowCount AS INT,
    @SurveySid AS INT,
    @ObjectSid AS INT,
	@SuitableTimeForCalls DATETIME
)
RETURNS TABLE
AS RETURN(
          SELECT TOP (@rowCount) c.*
          FROM BvActiveShiftTypeZone a
		  CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](@ObjectSid, a.Id, @SurveySID, @SuitableTimeForCalls, @rowCount) c
		  WHERE a.surveyid = @SurveySid
          ORDER BY priority DESC, TimeInShift, ExplicitType DESC, CallOrder )
GO
PRINT N'Altering [dbo].[BvSpAggregateSurveyProcessDelta]...';


GO
ALTER PROCEDURE [dbo].[BvSpAggregateSurveyProcessDelta]
AS
    DECLARE @BvAggregateSurveyDelta TABLE
	(
		[ID]                          BIGINT,
		[SID]                         INT NOT NULL,
		[ScheduledCallsCount]         INT NOT NULL,
		[SuspendedCallsCount]         INT NOT NULL,
		[MinutesSpentWorkingOnSurvey] INT NOT NULL
	);

	DELETE FROM BvAggregateSurveyDelta WITH (READPAST)
	OUTPUT DELETED.* INTO @BvAggregateSurveyDelta

	UPDATE 
	    BvAggregateSurvey 
	SET 
	    BvAggregateSurvey.MinutesSpentWorkingOnSurvey += AggregatedDelta.MinutesSpentWorkingOnSurvey,
	    BvAggregateSurvey.ScheduledCallsCount += AggregatedDelta.ScheduledCallsCount,
		BvAggregateSurvey.SuspendedCallsCount += AggregatedDelta.SuspendedCallsCount
	FROM
	    BvAggregateSurvey
	INNER JOIN
	(
	    SELECT [SID],
		       SUM([ScheduledCallsCount]) ScheduledCallsCount,
		       SUM([SuspendedCallsCount]) SuspendedCallsCount,
			   SUM([MinutesSpentWorkingOnSurvey]) MinutesSpentWorkingOnSurvey
		FROM @BvAggregateSurveyDelta
		GROUP BY [SID]
	) AggregatedDelta
	ON BvAggregateSurvey.SID = AggregatedDelta.SID

	DECLARE @HistoryDelta TABLE 
	(
		[SurveyId] INT NOT NULL,
		[PersonId] INT NOT NULL,
		[ITS] INT NOT NULL,
		[LogonTime] INT NOT NULL,
		[WaitingTime] INT NOT NULL,
		[FiredTime] DATETIME NOT NULL
	)

	DELETE FROM BvHistoryDelta WITH (READPAST)
	OUTPUT DELETED.[SurveyId],
		   DELETED.[PersonId],
		   DELETED.[ITS],
		   DELETED.[LogonTime],
		   DELETED.[WaitingTime],
		   DELETED.[FiredTime] INTO @HistoryDelta

	;WITH AggregateHistory AS
	(
	   SELECT [SurveyId],
	          [PersonId],
			  [ITS],
			  SUM([LogonTime]) LogonTime,
			  SUM([WaitingTime]) WaitingTime,
			  COUNT(*) DailingsCount,
			  dateadd(day, DatePart(day, FiredTime)-1, dateadd(month, DatePart(month, FiredTime)-1, DATEADD(year, DATEDIFF(year,0,FiredTime), 0))) StartTime
	   FROM @HistoryDelta
	   GROUP BY [SurveyId], [PersonId], [ITS], dateadd(day, DatePart(day, FiredTime)-1, dateadd(month, DatePart(month, FiredTime)-1, DATEADD(year, DATEDIFF(year,0,FiredTime), 0)))
	)
	MERGE INTO BvDayAgregatedHistory AS Target
	USING ( SELECT * FROM AggregateHistory) AS Source
		ON Target.SurveyId = Source.SurveyId AND
			Target.PersonId = Source.PersonId AND
			Target.ITS = Source.ITS AND
			Target.StartTime = Source.StartTime
	WHEN MATCHED THEN
		UPDATE SET LogonTime += Source.LogonTime,
				   WaitingTime += Source.WaitingTime,
				   DailingsCount += Source.DailingsCount
	WHEN NOT MATCHED THEN
	   INSERT([SurveyId],[PersonId],[ITS],[LogonTime],[WaitingTime],[DailingsCount],[StartTime])
	   VALUES([SurveyId],[PersonId],[ITS],[LogonTime],[WaitingTime],[DailingsCount],[StartTime]);

	;WITH AggregateHistory AS
	(
	   SELECT [SurveyId],
	          [PersonId],
			  [ITS],
			  SUM([LogonTime]) LogonTime,
			  SUM([WaitingTime]) WaitingTime,
			  COUNT(*) DailingsCount,
			  dateadd(month, DatePart(month, FiredTime)-1, DATEADD(year, DATEDIFF(year,0,FiredTime), 0)) StartTime
	   FROM @HistoryDelta
	   GROUP BY [SurveyId], [PersonId], [ITS], dateadd(month, DatePart(month, FiredTime)-1, DATEADD(year, DATEDIFF(year,0,FiredTime), 0))
	)
	MERGE INTO BvMonthAgregatedHistory AS Target
	USING ( SELECT * FROM AggregateHistory) AS Source
		ON Target.SurveyId = Source.SurveyId AND
			Target.PersonId = Source.PersonId AND
			Target.ITS = Source.ITS AND
			Target.StartTime = Source.StartTime
	WHEN MATCHED THEN
		UPDATE SET LogonTime += Source.LogonTime,
				   WaitingTime += Source.WaitingTime,
				   DailingsCount += Source.DailingsCount
	WHEN NOT MATCHED THEN
	   INSERT([SurveyId],[PersonId],[ITS],[LogonTime],[WaitingTime],[DailingsCount],[StartTime])
	   VALUES([SurveyId],[PersonId],[ITS],[LogonTime],[WaitingTime],[DailingsCount],[StartTime]);

RETURN 0
GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpGetCachedCallsForPredictiveSurveyBySurvey';


GO
PRINT N'Update complete.';


GO
