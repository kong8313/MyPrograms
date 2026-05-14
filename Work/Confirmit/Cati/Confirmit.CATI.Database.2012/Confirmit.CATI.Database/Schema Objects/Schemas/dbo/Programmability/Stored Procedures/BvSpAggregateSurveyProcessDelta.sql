CREATE PROCEDURE [dbo].[BvSpAggregateSurveyProcessDelta]
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
	   VALUES(Source.[SurveyId],Source.[PersonId],Source.[ITS],Source.[LogonTime],Source.[WaitingTime],Source.[DailingsCount],Source.[StartTime]);

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
	   VALUES(Source.[SurveyId],Source.[PersonId],Source.[ITS],Source.[LogonTime],Source.[WaitingTime],Source.[DailingsCount],Source.[StartTime]);

RETURN 0
