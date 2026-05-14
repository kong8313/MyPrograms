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
GO
PRINT N'Altering [dbo].[BvSpAsyncOperationQueue_UpdateHanged]...';


GO
ALTER PROCEDURE [dbo].[BvSpAsyncOperationQueue_UpdateHanged]
	@ExecutingStateValue TINYINT  /*AsyncOperationState.Executing passed from C# to avoid copy paste*/,
	@HangedStateValue TINYINT  /*AsyncOperationState.Hanged passed from C# to avoid copy paste*/,
	@TimeToTreatOperationHangedInMinutes INT
AS
	CREATE TABLE #HangedTaskIds
	(
		Id INT NOT NULL PRIMARY KEY
	)

	INSERT INTO #HangedTaskIds SELECT Id FROM BvAsyncOperationQueue
    WHERE
	    [State] = @ExecutingStateValue AND 
		DATEDIFF(minute, HeartBeat, GETUTCDATE()) >= @TimeToTreatOperationHangedInMinutes

	IF @@ROWCOUNT > 0 
	BEGIN
		UPDATE
			BvAsyncOperationQueue
		SET 
			[State] = @HangedStateValue
		FROM #HangedTaskIds 
		WHERE BvAsyncOperationQueue.Id = #HangedTaskIds.Id AND [State] = @ExecutingStateValue AND 
		DATEDIFF(minute, HeartBeat, GETUTCDATE()) >= @TimeToTreatOperationHangedInMinutes
	END

RETURN 0
GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpLogin_SpinUp]...';


GO
ALTER  PROCEDURE [dbo].[BvSpLogin_SpinUp]
@PersonSID INTEGER
AS
declare @SurveySID int
declare @PersonMode int
    
	select @SurveySID = SurveySID
	from BvTasks where PersonSID = @PersonSID
    
    if @SurveySID is not null 
    begin
	    select @PersonMode = ManualSelection from BvPerson where sid = @PersonSID

        if(@PersonMode != 2) --is not survey selection
           SET @SurveySID = 0
    
        delete from BvLoginGroup where PersonSID = @PersonSID
        insert into BvLoginGroup WITH(TABLOCKX) select PersonSID, ObjectSID, @SurveySID
            from BvPersonRel where PersonSID = @PersonSID
    end
 
return (0)
GO
PRINT N'Update complete.';


GO
