GO
PRINT N'Insert new ITSes to BvState table';

;WITH data( StateId, Name, Priority, StateGroupID, DA, FcdAction )
AS
(
    SELECT s.StateId, s.Name, s.Priority, sg.ID, s.DA, s.FcdAction FROM BvStateGroup sg CROSS JOIN 
    (
        SELECT 1000 as StateId, 'Inbound call' as Name, 1 as Priority, 0 as DA, 1 as FcdAction
        UNION 
        SELECT 1001, 'Inbound call dropped by respondent', 1, 0, 0
        UNION 
        SELECT 1020, 'Dial interrupted by interviewer', 1, 0, 0
    ) as s
)
INSERT INTO [dbo].[BvState] (StateID, Name, Priority, StateGroupID, DA, FcdAction) SELECT StateId, Name, Priority, StateGroupID, DA, FcdAction FROM data

INSERT INTO BvThresholdITS ( SurveySID, ITS ) 
    SELECT 0, StateId FROM
    (
        SELECT 1000 as StateId
        UNION 
        SELECT 1001
        UNION 
        SELECT 1020
    ) as s

GO
PRINT N'Altering [dbo].[BvCallHistory]...';

DROP INDEX [IX_BvCallHistoryFiredTime_i_its_SurveyId] ON [dbo].[BvCallHistory]

GO
ALTER TABLE [dbo].[BvCallHistory] ALTER COLUMN [ITS] SMALLINT NULL;


GO
PRINT N'Creating [dbo].[BvCallHistory].[IX_BvCallHistoryFiredTime_i_its_SurveyId]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvCallHistoryFiredTime_i_its_SurveyId]
    ON [dbo].[BvCallHistory]([FiredTime] ASC)
    INCLUDE([ITS], [SurveyId]);


GO
/*
The type for column ITS in table [dbo].[BvHistory] is currently  INT NULL but is being changed to  SMALLINT NULL. Data loss could occur.
*/
GO
PRINT N'Starting rebuilding table [dbo].[BvHistory]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_BvHistory] (
    [ID]                    INT            IDENTITY (1, 1) NOT NULL,
    [SurveyId]              INT            NOT NULL,
    [TelephoneNumber]       NVARCHAR (255) NULL,
    [FiredTime]             DATETIME       NOT NULL,
    [InterviewId]           INT            NULL,
    [ITS]                   SMALLINT       NULL,
    [AppointmentID]         INT            NULL,
    [WaitingTime]           INT            NULL,
    [ConfirmitDuration]     INT            NULL,
    [Duration]              INT            NULL,
    [BatchId]               INT            NULL,
    [PersonSID]             INT            NULL,
    [RoleID]                TINYINT        NULL,
    [CallCenterID]          INT            NOT NULL,
    [OpenEndReviewDuration] INT            NULL
);

CREATE CLUSTERED INDEX [tmp_ms_xx_index_IX_History_Main]
    ON [dbo].[tmp_ms_xx_BvHistory]([SurveyId] ASC, [RoleID] ASC, [FiredTime] ASC, [ITS] ASC)
    ON [PRIMARY];

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[BvHistory])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_BvHistory] ON;
        INSERT INTO [dbo].[tmp_ms_xx_BvHistory] ([SurveyId], [RoleID], [FiredTime], [ITS], [ID], [TelephoneNumber], [InterviewId], [AppointmentID], [WaitingTime], [ConfirmitDuration], [Duration], [BatchId], [PersonSID], [CallCenterID], [OpenEndReviewDuration])
        SELECT   [SurveyId],
                 [RoleID],
                 [FiredTime],
                 [ITS],
                 [ID],
                 [TelephoneNumber],
                 [InterviewId],
                 [AppointmentID],
                 [WaitingTime],
                 [ConfirmitDuration],
                 [Duration],
                 [BatchId],
                 [PersonSID],
                 [CallCenterID],
                 [OpenEndReviewDuration]
        FROM     [dbo].[BvHistory]
        ORDER BY [SurveyId] ASC, [RoleID] ASC, [FiredTime] ASC, [ITS] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_BvHistory] OFF;
    END

DROP TABLE [dbo].[BvHistory];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_BvHistory]', N'BvHistory';

EXECUTE sp_rename N'[dbo].[BvHistory].[tmp_ms_xx_index_IX_History_Main]', N'IX_History_Main', N'INDEX';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [dbo].[BvHistory].[IX_BvHistory_InterviewerPerformance]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvHistory_InterviewerPerformance]
    ON [dbo].[BvHistory]([FiredTime] ASC, [RoleID] ASC, [PersonSID] ASC, [ITS] ASC)
    INCLUDE([WaitingTime], [ConfirmitDuration], [Duration], [OpenEndReviewDuration])
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[BvTrBvHistory_HistoryInsert]...';


GO
CREATE TRIGGER [BvTrBvHistory_HistoryInsert] ON [dbo].[BvHistory]
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
PRINT N'Refreshing [dbo].[GetCountsForSample]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[GetCountsForSample]';


GO
PRINT N'Altering [dbo].[GetContextData]...';


GO
ALTER FUNCTION [dbo].[GetContextData]( )
RETURNS @Context TABLE(ITS SMALLINT, OperationId INT, OperationType TINYINT, CallCenterId INT, DialingMode TINYINT) 
AS
BEGIN

DECLARE @contextStr NVARCHAR(MAX)

DECLARE @commaPos1 INT 
DECLARE @commaPos2 INT
DECLARE @commaPos3 INT
DECLARE @commaPos4 INT

SET @contextStr = RTRIM(REPLACE(CONVERT(VARCHAR(128),CONTEXT_INFO()), CHAR(0), CHAR(32) )); 

SET @commaPos1 = CHARINDEX(',', @contextStr) 
SET @commaPos2 = CHARINDEX(',', @contextStr, @commaPos1 + 1)
SET @commaPos3 = CHARINDEX(',', @contextStr, @commaPos2 + 1)
SET @commaPos4 = CHARINDEX(',', @contextStr, @commaPos3 + 1)

INSERT INTO @Context
SELECT	SUBSTRING(@contextStr, 1, @commaPos1 - 1), 
		SUBSTRING(@contextStr, @commaPos1+1, @commaPos2 - @commaPos1 - 1),
		SUBSTRING(@contextStr, @commaPos2+1, @commaPos3 - @commaPos2 - 1),
		SUBSTRING(@contextStr, @commaPos3+1, @commaPos4 - @commaPos3 - 1),	
		SUBSTRING(@contextStr, @commaPos4+1, len(@contextStr) - @commaPos4)	

RETURN
END
GO
PRINT N'Altering [dbo].[BvTrBvSvySchedule_CallsInsert]...';


GO
ALTER TRIGGER [BvTrBvSvySchedule_CallsInsert] ON [dbo].[BvSvySchedule]
AFTER INSERT
AS 
BEGIN
	SET NOCOUNT ON
	
	INSERT INTO BvSvyScheduleRuntimeStatisticsDelta(SurveyId, ShiftTypeID, ExplicitSID, CallState, CountDelta )
        SELECT  SurveySid, ShiftTypeId, ExplicitSID, CallState, COUNT(*) as CountDelta
                FROM inserted
                WHERE CallState IN ( -2, 2 )
                GROUP BY SurveySid, ShiftTypeId, ExplicitSID, CallState

	IF (CONTEXT_INFO() IS NOT NULL)
	BEGIN

		DECLARE @OperationType TINYINT
		DECLARE @CallCenterId INT
		DECLARE @OperationId INT
		DECLARE @ITS SMALLINT
		DECLARE @DialingMode TINYINT

		SELECT @ITS = ITS, @OperationID = OperationId, @OperationType = OperationType, @CallCenterId = CallCenterId, @DialingMode = DialingMode from dbo.GetContextData()

		INSERT INTO BvCallHistory
		SELECT 
			GETUTCDATE(), ApptId, ShiftTypeId, InterviewId, SurveySid, @ITS, @DialingMode, CAST(CallState AS SMALLINT), [Priority], TimeInShift, Expiretime, 
			ExplicitSid, CAST(ExplicitType AS tinyint), CellId, @OperationId, @OperationType, @CallCenterId, DialTypeId		
	    FROM inserted
	END
END
GO
PRINT N'Altering [dbo].[BvTrBvSvySchedule_CallsUpdate]...';


GO
ALTER TRIGGER [dbo].[BvTrBvSvySchedule_CallsUpdate] ON [dbo].[BvSvySchedule]
FOR UPDATE
AS
BEGIN
    SET NOCOUNT ON
     
    IF UPDATE( SurveySid ) OR UPDATE( ShiftTypeId ) OR UPDATE( ExplicitSID ) OR UPDATE( CallState )
    BEGIN
        ;WITH stat AS
        (
            SELECT  SurveySid, ShiftTypeId, ExplicitSID, CallState, COUNT(*) as CountDelta
                    FROM inserted
                    WHERE CallState IN ( -2, 2 )
                    GROUP BY SurveySid, ShiftTypeId, ExplicitSID, CallState
            UNION ALL
            SELECT  SurveySid, ShiftTypeId, ExplicitSID, CallState, -COUNT(*)
                    FROM deleted
                    WHERE CallState IN ( -2, 2 )
                    GROUP BY SurveySid, ShiftTypeId, ExplicitSID, CallState
        )
        INSERT INTO BvSvyScheduleRuntimeStatisticsDelta(SurveyId, ShiftTypeID, ExplicitSID, CallState, CountDelta )
            SELECT SurveySid, ShiftTypeId, ExplicitSID, CallState, SUM(CountDelta) as Delta
                FROM stat
                GROUP BY SurveySid, ShiftTypeId, ExplicitSID, CallState
                HAVING SUM(CountDelta) <> 0
                 
    END

	IF (CONTEXT_INFO() IS NOT NULL)
	BEGIN

		DECLARE @OperationType TINYINT
		DECLARE @CallCenterId INT
		DECLARE @OperationId INT
		DECLARE @ITS SMALLINT
		DECLARE @DialingMode TINYINT

		SELECT @ITS = ITS, @OperationID = OperationId, @OperationType = OperationType, @CallCenterId = CallCenterId, @DialingMode = DialingMode from dbo.GetContextData()

		INSERT INTO BvCallHistory
		SELECT 
			GETUTCDATE(), ApptId, ShiftTypeId, InterviewId, SurveySid, @ITS, @DialingMode, CAST(CallState AS SMALLINT), [Priority], TimeInShift, Expiretime, 
			ExplicitSid, CAST(ExplicitType AS tinyint), CellId, @OperationId, @OperationType, @CallCenterId, DialTypeId		
		FROM inserted
	END

END
GO
PRINT N'Refreshing [dbo].[RestView_CallHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_CallHistory]';


GO
PRINT N'Altering [dbo].[BvSpCall_MoveToITS]...';


GO
ALTER PROCEDURE [dbo].[BvSpCall_MoveToITS]
@SurveySID   INTEGER,
@BatchID    INTEGER,
@StateID     INTEGER
AS
   DECLARE @CfDbSchemaPath NVARCHAR(255)
   DECLARE @ProcessedCalls INT = 0
   DECLARE @SurveySchedulingMode INT 
   SELECT @CfDbSchemaPath = CfDbSchemaPath,
		  @SurveySchedulingMode = SurveySchedulingMode
   FROM BvSurvey
   WHERE SID = @SurveySID
   
   CREATE TABLE #InterviewIds(Id INT, DialingMode TINYINT, its SMALLINT)
   CREATE TABLE #ids(Id INT)

   UPDATE BvInterview
   SET TransientState = @StateID 
   OUTPUT inserted.Id, inserted.DialingMode, inserted.TransientState
   INTO #InterviewIds
   FROM BvInterview i
   INNER JOIN BvTransferArrays ta ON i.ID = ta.ItemID AND
									 ta.BatchID = @BatchID AND
									 i.SurveySID = @SurveySID
   LEFT JOIN BvSvySchedule s ON i.Id = s.InterviewId AND
                                s.SurveySid = @SurveySID
   WHERE ISNULL(s.CallState, 1) > 0
         
   SET @ProcessedCalls = @@ROWCOUNT
   
   UPDATE BvSvySchedule 
   SET Priority = BvState.Priority,
       OldPriority = 0,
	   ConditionValue = CASE WHEN @SurveySchedulingMode = 1 THEN @StateID ELSE 0 END
   OUTPUT inserted.id INTO #ids
   FROM #InterviewIds ids
   INNER JOIN BvState ON BvState.StateID = @StateID
   INNER JOIN BvSurvey ON BvSurvey.SID = @SurveySID AND
                          BvState.StateGroupID = BvSurvey.StateGroupID
   WHERE BvSvySchedule.SurveySID = @SurveySID AND 
         BvSvySchedule.InterviewId = ids.Id AND
         BvSvySchedule.CallState > 0
   
   IF (@@ROWCOUNT < @ProcessedCalls AND CONTEXT_INFO() IS NOT NULL) 
   BEGIN
	
		DECLARE @OperationType TINYINT
		DECLARE @CallCenterId INT
		DECLARE @OperationId INT

		SELECT @OperationID = OperationId, @OperationType = OperationType, @CallCenterId = CallCenterId from dbo.GetContextData()
	 
		INSERT INTO BvCallhistory
			SELECT GETUTCDATE(), c.ApptID, c.ShiftTypeID, i.Id, @SurveySID, i.its, i.Dialingmode, c.CallState, c.[Priority], c.TimeInShift, c.ExpireTime, c.ExplicitSid, c.ExplicitType, c.CellId, 
                @OperationId, @OperationType, @CallCenterId, c.DialTypeId
			FROM #InterviewIds i
			LEFT JOIN BvSvySchedule c ON c.InterviewID = i.ID AND c.SurveySID = @SurveySId 
			WHERE i.ID NOT IN (SELECT ID FROM #ids) 
   END

   IF((@ProcessedCalls != 0) AND (@CfDbSchemaPath IS NOT NULL) AND (@CfDbSchemaPath != ''))
   BEGIN
	   DECLARE @Query NVARCHAR(1024)
	   SET @Query = 'UPDATE '+@CfDbSchemaPath+'.response_control '+
					'SET ITS = '+cast(@StateID as nvarchar(10))+ ' ' +
					'FROM #InterviewIds as ids '+
					'WHERE respid = ids.ID '
	   EXECUTE( @Query )
   END

   EXEC BvSpDeleteTransfer @BatchID

RETURN @ProcessedCalls
GO
PRINT N'Altering [dbo].[BvSpSurveyProductivityReport]...';


GO
ALTER PROCEDURE [dbo].[BvSpSurveyProductivityReport]
	@SurveySids NVARCHAR(MAX), 
    @PersonSIDs NVARCHAR (MAX), 
    @ITS NVARCHAR (MAX), 
    @StartDate DATETIME, 
    @EndDate DATETIME,
	@SurveyDataFilter NVARCHAR(MAX),
    @StartShiftTime DATETIME,
    @EndShiftTime DATETIME

WITH RECOMPILE

AS
IF(@SurveySids IS NULL AND 
   @PersonSIDs IS NULL AND
   @ITS IS NULL AND
   @StartDate IS NULL AND
   @EndDate IS NULL)
BEGIN
   SELECT 0 AS [PersonSID],
          '' AS [PersonCode],           
		  '' AS [PersonName],
		  0 AS [SurveySID],
          '' AS [SurveyCode],
          '' AS [SurveyName],
		  cast(0 as SMALLINT)  AS [StateID],
		  '' AS [StateName],
		  0 AS [InterviewCount],
		  0 AS [TotalInterviewCount],
          0 AS [InterviewTime]
          
   RETURN 0;
END
    
          
          
DECLARE @DefaultStateGroupID INTEGER
SELECT top(1) @DefaultStateGroupID = ID 
FROM BvStateGroup 
ORDER BY [Order] ASC;
 
CREATE TABLE #surveySids([SurveyId] int primary key, [SurveyCode] nvarchar(max), [Description] nvarchar(max))
insert into #surveySids 
SELECT [SID] AS [SurveyId],
          [Name] AS [SurveyCode],
          [Description]
FROM dbo.utilSplitNumbers( ISNULL(@SurveySids, ''), ',')
INNER JOIN BvSurvey ON SID = Item

CREATE TABLE #SelectedStatuses([StateID] SMALLINT primary key, [StateName] nvarchar(max))
insert into #SelectedStatuses
SELECT [s].[StateID],
       [s].[Name] [StateName]
FROM [BvState] [s]
LEFT JOIN dbo.utilSplitNumbers(ISNULL(@ITS, ''), ',') ON [s].[StateID] = [Item]
WHERE (@ITS IS NULL OR [Item] IS NOT NULL) AND [s].[StateGroupID] = @DefaultStateGroupID 
         
create table #persons(sid int primary key, [PersonCode] nvarchar(max), [PersonName] nvarchar(max))
insert into #persons
SELECT SID, 
       CAST([SID] AS NVARCHAR(MAX)) [PersonCode],
       [Name] [PersonName]
FROM BvPerson
LEFT JOIN dbo.utilSplitNumbers(ISNULL(@PersonSIDs, ''), ',') ON [SID] = [Item]
WHERE @PersonSIDs IS NULL OR [Item] IS NOT NULL AND EXISTS
  (
     SELECT * 
     FROM [BvPersonRel] 
     WHERE SID = PersonSid AND RoleId = 2
  )


CREATE TABLE #respids 
( 
	surveyid INT,
	respid int,
	PRIMARY KEY CLUSTERED 
	(
		[SurveyId] ASC,
		[respid] ASC
	)
)

IF (@SurveyDataFilter IS NOT NULL)
BEGIN
	DECLARE @sql NVARCHAR(MAX)
	SET @sql = N'INSERT INTO #respids SELECT ' +  @SurveySids + ', respid from [dbo].[BvReplicatedData_' + @SurveySids + '] AS CFInterview WHERE ' + @SurveyDataFilter 
	EXEC (@sql)
END


DECLARE @diff TIME

IF CAST(@StartShiftTime AS TIME) > CAST(@EndShiftTime AS TIME)
	select @diff = CAST('00:00:00' - (@StartShiftTime-@EndShiftTime) AS TIME )
else
	select @diff = CAST(@EndShiftTime-@StartShiftTime AS TIME)

;WITH BvHistory_CTE AS
(
   SELECT [history].SurveyId, [history].ITS, [history].Duration, [history].PersonSid,
   COUNT([history].its) OVER(partition by [history].[SurveyId], [history].[PersonsId]) as TotalInterviewCount
   FROM #surveySids [survey] 
   INNER JOIN [BvHistory] [history] ON [survey].[SurveyId] = [history].[SurveyId]
   LEFT JOIN #respids i on i.respid = [history].InterviewId AND i.surveyid = [history].SurveyId
   WHERE (i.respid IS NOT NULL OR @SurveyDataFilter IS NULL) AND [history].[FiredTime] BETWEEN @StartDate AND @EndDate AND
         [history].[RoleID] = 2 AND
		 (@StartShiftTime IS NULL OR CAST( firedTime - @StartShiftTime  AS TIME ) <= @diff)
),
BvHistoryWithStates_CTE AS
(
   SELECT [history].*, [state].*
   FROM BvHistory_CTE [history]
   INNER JOIN #SelectedStatuses [state] ON [state].[StateID] = [history].[ITS]
), GroupedByPersonCounts AS
(
SELECT 
	 [person].[SID] AS [PersonSID],
	 [person].[PersonCode],           
	 [person].[PersonName],
                
	 [history].[SurveyId] AS [SurveySID],
	 [history].[StateID] AS [StateID],
	 [history].[StateName],
 
	 COUNT(*) AS [InterviewCount], /* Interview count for status. */
    
	 /* Total calls count for the selected person and survey (regardless to status). */
	 MAX(TotalInterviewCount) AS [TotalInterviewCount],

	 ISNULL(SUM([history].[Duration]), 0) AS [InterviewTime] /* Interview time in seconds. */

	 FROM #persons [person]
	 INNER JOIN BvHistoryWithStates_CTE [history] ON [history].[PersonSID] = [person].[SID]

	 GROUP BY   [history].[SurveyId],
				[history].[StateId], 
				[history].[StateName],
				[person].[SID], 
				[person].[PersonCode], 
				[person].[PersonName]
 )
 SELECT 
	c.*,
	s.[SurveyCode] AS [SurveyCode],
	s.[Description] AS [SurveyName]
 FROM GroupedByPersonCounts c
 JOIN #SurveySids s
	ON c.SurveySID = s.SurveyId
 ORDER BY [PersonCode], [StateId]

RETURN 0
GO
PRINT N'Altering [dbo].[BvSpGetCallAttemptsReport_ListPage]...';


GO
ALTER PROCEDURE BvSpGetCallAttemptsReport_ListPage 
	@SupervisorName NVARCHAR(255),
	@PageNumber INT, 
	@PageSize INT, 
	@OrderField NVARCHAR (64), 
	@IsOrderASC INT,
	@SearchCondition NVARCHAR (4000) = NULL
AS
BEGIN
	IF @SupervisorName IS NULL AND @PageNumber IS NULL AND @PageSize IS NULL
	BEGIN
	/* Looks like we're generating code using FMTONLY. So lets return metadata*/
		SELECT
		0 as [ID],
		GETDATE() as [EventDate],
		0 as [SurveySID],
		'' as [ProjectID],
		'' as [ProjectName],
		'' as [InterviewerName],
		0 as [InterviewID],
		0 as [CallDuration],
		CAST( 0 as SMALLINT) as [ExtendedStatus],
		'' as [ExtendedStatusName],
		'' as [TelephoneNumber]
     
		RETURN 0;
	END
 
	DECLARE @StateGroupID INT
	SELECT @StateGroupID = ID FROM [BvStateGroup] WHERE [Order] = (SELECT MIN([Order]) FROM [BvStateGroup])
	
	DECLARE @Query NVARCHAR(MAX) = 'SELECT
		hist.[ID] as [ID],
		hist.[FiredTime] as [EventDate],
		survey.[SID] as [SurveySID],
		survey.[Name] as [ProjectID],
		survey.[Description] as [ProjectName],
		person.[Name] as [InterviewerName],
		hist.[InterviewId] as [InterviewID],
		hist.[Duration] as [CallDuration],
		hist.[ITS] as [ExtendedStatus],
		states.[Name] as [ExtendedStatusName],
		hist.[TelephoneNumber] as [TelephoneNumber]
		FROM
		[BvHistory] hist INNER JOIN [BvSurvey] survey ON hist.SurveyId = survey.[SID]
		INNER JOIN [BvUserSurveyPermission] perm ON (perm.SurveySID = survey.[SID] AND perm.UserName = ''' + @SupervisorName + ''')
		INNER JOIN [BvPerson] person ON person.[SID] = hist.[PersonSID] 
		INNER JOIN [BvState] states ON states.StateID = hist.[ITS] AND states.StateGroupID = ' + CAST(@StateGroupID AS NVARCHAR(20)) +
		' WHERE hist.[RoleID] = 2 AND hist.InterviewId IS NOT NULL AND survey.State <> 2'

	DECLARE @TotalCount INT
	exec @TotalCount = BvSpGetListPage @PageNumber, @PageSize, @OrderField, @IsOrderASC, @Query, 'ID', @SearchCondition
	RETURN @TotalCount
END
GO
PRINT N'Altering [dbo].[BvSpGetCallsSentToDialerDistribution]...';


GO
/* Stored procedure dbo.BvSpGetCallsSentToDialerDistribution returns a breakdown of calls sent to dialer per ExplicitSid ( user/group )
   for 20 min starting from @StartTime for a specified suurvey @SurveySid */
ALTER PROCEDURE [dbo].[BvSpGetCallsSentToDialerDistribution]
	@StartTime DATETIME = NULL,                      -- expects UTC time
	@SurveySid INT,
	@timezoneId INT
AS
 
DECLARE @Total INT
DECLARE @tableStartTime AS DateTime,
		@tableEndTime AS DateTime,
		@cols         AS NVARCHAR(MAX),
		@sql         AS NVARCHAR(MAX),
		@bias         AS INT;
	
IF(@StartTime IS NULL)
BEGIN
	SET @tableEndTime = GETUTCDATE();
	SET @tableStartTime =  (SELECT TOP(1) * 
							FROM(SELECT DISTINCT TOP(20) [Time] 
								 FROM [BvCallsSentToDialer] 
							     WHERE SurveySid = @SurveySid AND [Time] <= @tableEndTime  ORDER BY [Time] desc) AS r 
							ORDER BY r.[Time] ASC)
END
ELSE 
BEGIN
	SET @tablestartTime = @StartTime;
	SET @tableEndTime =  (SELECT TOP(1) * 
						  FROM (SELECT DISTINCT TOP(20) [Time] 
							    FROM [BvCallsSentToDialer] 
							    WHERE SurveySid = @SurveySid AND [Time] >= @tableStartTime ORDER BY [Time]) AS r 
						  ORDER BY r.[Time] DESC);
END
 

SELECT  @bias = DATEDIFF( [mi], @tablestartTime,  dbo.UTC2LT( @tablestartTime, Bias, DaylightType,
							StandardDayOfWeek, StandardStart, StandardBias,
							DaylightDayOfWeek, DaylightStart, DaylightBias ))
FROM  [BvTimezone]
WHERE [ID] = @timezoneId

;WITH timeList AS
(
	SELECT DISTINCT TOP(20) [Time] 
	FROM [BvCallsSentToDialer] 
	WHERE SurveySid= @SurveySid AND [Time] >= @tableStartTime ORDER BY [Time]
)
SELECT  
@cols = STUFF((SELECT N', ' +  QUOTENAME(CONVERT(nvarchar(max),  DATEADD( mi, @bias, [Time]), 21)) FROM timeList FOR XML PATH('')), 1, 2, '');
 
-- Construct the full T-SQL statement and execute dynamically. Query could look like this
/*
SELECT *
FROM (SELECT ISNULL ( g.Name,'*Survey Assignment*') as [Group/User Name], convert(char(5), DATEADD( mi,-300, [time]), 108) AS [minutes], [CallsCount]
          FROM dbo.BvActiveCallsInfo LEFT JOIN ( SELECT SID, Name FROM BvPerson UNION SELECT SID, Name from BvPersonGroup ) as g on ExplicitSid = g.SID 
          where surveysid=1 and [time] >='Dec  7 2009 11:50AM' and [time] <='Dec  7 2009 12:10PM' ) AS D
  PIVOT(MAX(CallsCount) FOR minutes IN([07:00],[07:01],[07:02],[07:03],[07:04],[07:05],[07:07],[07:08],[07:09],[07:10]) )  as  P order by [Group/User Name] ;
*/
SET @sql = N'SELECT *
FROM (SELECT ISNULL ( g.[Name],' + '''' + '*Survey Assignment*' + '''' + ') as [Group or User], 
			 CONVERT(nvarchar(max), DATEADD( mi,' + CAST( @bias AS VARCHAR(MAX)) + ', [time]), 21) AS [requestTime], 
			 [CallsCount]
          FROM [dbo].[BvCallsSentToDialer]
          LEFT JOIN BvViewPersonAndGroup g 
			  ON [ExplicitSid] = g.[SID] 
		  WHERE [SurveySID]=' + CAST( @SurveySid  AS VARCHAR(32))+ ' AND [Time] >=' + '''' + 
			  + Convert(nvarchar(max), @tableStartTime , 21) + '''' + ' AND [Time] <=' + '''' + 
			  Convert(nvarchar(max), @tableEndTime , 21) + '''' + ') AS D
  PIVOT( MAX([CallsCount]) FOR [requestTime] IN(' + @cols + N') ) AS P ORDER BY [Group or User];';
  
EXEC sp_executesql @sql;

SELECT  @total = SUM(CallsCount) FROM BvCallsSentToDialer WHERE SurveySID = @SurveySid AND [Time] BETWEEN @tableStartTime AND @tableEndTime
RETURN @total
GO

GO
PRINT N'Refreshing [dbo].[BvSpGetExtendedCallHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetExtendedCallHistory]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Clean]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Clean]';


GO
PRINT N'Refreshing [dbo].[BvSpSampleUtilisationReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSampleUtilisationReport]';


GO
PRINT N'Refreshing [dbo].[BvSpAggregateInterviewerPerformance]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAggregateInterviewerPerformance]';


GO
PRINT N'Refreshing [dbo].[BvSpAlert_RecalculateAll]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlert_RecalculateAll]';


GO
PRINT N'Refreshing [dbo].[BvSpAttemptsByDispositionReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAttemptsByDispositionReport]';


GO
PRINT N'Refreshing [dbo].[BvSpCallHistory_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistory_List]';


GO
PRINT N'Refreshing [dbo].[BvSpCallHistoryData]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistoryData]';


GO
PRINT N'Refreshing [dbo].[BvSpHistory_CfData_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpHistory_CfData_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpInterviewerProductivityReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviewerProductivityReport]';


GO
PRINT N'Refreshing [dbo].[BvSpNumberOfAttemptsReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpNumberOfAttemptsReport]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyOverviewReportForAllPersons]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyOverviewReportForAllPersons]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyOverviewReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyOverviewReport]';


GO
PRINT N'Update complete.';


GO
