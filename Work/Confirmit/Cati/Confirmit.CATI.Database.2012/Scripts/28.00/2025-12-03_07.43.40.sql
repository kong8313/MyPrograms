PRINT N'Creating Table [dbo].[BvCallHistoryEx]...';


GO
CREATE TABLE [dbo].[BvCallHistoryEx] (
    [Id]            BIGINT   IDENTITY (1, 1) NOT NULL,
    [FiredTime]     DATETIME NOT NULL,
    [ApptID]        INT      NULL,
    [ShiftTypeID]   INT      NULL,
    [InterviewID]   INT      NOT NULL,
    [SurveyId]      INT      NOT NULL,
    [ITS]           SMALLINT NULL,
    [DialingMode]   TINYINT  NULL,
    [CallState]     SMALLINT NULL,
    [Priority]      INT      NULL,
    [TimeInShift]   DATETIME NULL,
    [ExpireTime]    DATETIME NULL,
    [ExplicitSID]   INT      NULL,
    [ExplicitType]  TINYINT  NULL,
    [CellId]        INT      NULL,
    [OperationId]   INT      NOT NULL,
    [OperationType] TINYINT  NOT NULL,
    [CallCenterId]  INT      NOT NULL,
    [BlockedByFcd]  AS       (CASE WHEN [OperationType] = (9) THEN CONVERT (BIT, (1)) WHEN [OperationType] = (11) THEN CONVERT (BIT, (1)) WHEN [OperationType] = (28) THEN CONVERT (BIT, (1)) WHEN [OperationType] = (29) THEN CONVERT (BIT, (1)) ELSE CONVERT (BIT, (0)) END) PERSISTED NOT NULL,
    [DialTypeId]    TINYINT  NULL,
    CONSTRAINT [PK_BVCallHistoryEx_ID] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating Index [dbo].[BvCallHistoryEx].[IX_BvCallHistoryExSurveyID_InterviewID_i_FiredTime]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvCallHistoryExSurveyID_InterviewID_i_FiredTime]
    ON [dbo].[BvCallHistoryEx]([SurveyId] ASC, [InterviewID] ASC)
    INCLUDE([FiredTime]);


GO
PRINT N'Creating Index [dbo].[BvCallHistoryEx].[IX_BvCallHistoryExFiredTime_i_its_SurveyId]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvCallHistoryExFiredTime_i_its_SurveyId]
    ON [dbo].[BvCallHistoryEx]([FiredTime] ASC)
    INCLUDE([ITS], [SurveyId]);


GO
PRINT N'Creating Index [dbo].[BvCallHistoryEx].[IX_BvCallHistoryEx_BlockByFCD_SurveyId_InterviewId]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvCallHistoryEx_BlockByFCD_SurveyId_InterviewId]
    ON [dbo].[BvCallHistoryEx]([BlockedByFcd] ASC, [SurveyId] ASC, [InterviewID] ASC)
    INCLUDE([FiredTime]);


GO
PRINT N'Altering Trigger [dbo].[BvTrBvSvySchedule_CallsInsert]...';


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

		INSERT INTO BvCallHistoryEx
		SELECT 
			dbo.GetUtcNow(), ApptId, ShiftTypeId, InterviewId, SurveySid, @ITS, @DialingMode, CAST(CallState AS SMALLINT), [Priority], TimeInShift, Expiretime, 
			ExplicitSid, CAST(ExplicitType AS tinyint), CellId, @OperationId, @OperationType, @CallCenterId, DialTypeId		
	    FROM inserted
	END
END
GO
PRINT N'Altering Trigger [dbo].[BvTrBvSvySchedule_CallsUpdate]...';


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

		INSERT INTO BvCallHistoryEx
		SELECT 
			dbo.GetUtcNow(), ApptId, ShiftTypeId, InterviewId, SurveySid, @ITS, @DialingMode, CAST(CallState AS SMALLINT), [Priority], TimeInShift, Expiretime, 
			ExplicitSid, CAST(ExplicitType AS tinyint), CellId, @OperationId, @OperationType, @CallCenterId, DialTypeId		
		FROM inserted
	END

END
GO
PRINT N'Creating View [dbo].[BvViewBothCallHistories]...';


GO
CREATE VIEW [dbo].[BvViewBothCallHistories]
AS
SELECT
    [Id], [FiredTime], [ApptID], [ShiftTypeID], [InterviewID], [SurveyId], [ITS], [DialingMode], [CallState], [Priority], 
    [TimeInShift], [ExpireTime], [ExplicitSID], [ExplicitType], [CellId], [OperationId], [OperationType], [CallCenterId],
    [BlockedByFcd], [DialTypeId]
FROM dbo.[BvCallHistory]
UNION
SELECT
    [Id], [FiredTime], [ApptID], [ShiftTypeID], [InterviewID], [SurveyId], [ITS], [DialingMode], [CallState], [Priority],
    [TimeInShift], [ExpireTime], [ExplicitSID], [ExplicitType], [CellId], [OperationId], [OperationType], [CallCenterId],
    [BlockedByFcd], [DialTypeId]
FROM dbo.[BvCallHistoryEx]
GO
PRINT N'Altering Function [dbo].[GetCountsForSample]...';


GO
ALTER FUNCTION [dbo].[GetCountsForSample]
(
	@BatchId int,
	@Its varchar(max)
)
RETURNS TABLE
AS
RETURN
(
WITH counts AS
(
SELECT  
	DENSE_RANK() OVER (ORDER BY i.SurveySId, i.Id) as RecordsInBatch,
	
	CASE
		WHEN h.InterviewId IS NOT NULL AND LEAD(h.InterviewId, 1, 0) OVER (PARTITION BY h.SurveyId, h.InterviewId ORDER BY h.id) <> h.InterviewId 
		THEN 1
		ELSE 0 
	END AS attempted,

	CASE 
		WHEN 
			LEAD(ch.Interviewid, 1, 0 ) OVER (PARTITION BY ch.SurveyId, ch.InterviewId ORDER BY ch.id, h.id) <> ch.InterviewId AND h.InterviewId IS NULL
		THEN 1 
		ELSE 0
	END AS blockedExcludedAttempted,

	CASE 
		WHEN  LEAD(ch.Interviewid, 1, 0 ) OVER (PARTITION BY ch.SurveyId, ch.InterviewId ORDER BY ch.id, h.id) <> ch.InterviewId AND ch.FiredTime > h.FiredTime
		THEN 1 
		ELSE 0
	END AS blockedAttempted,

	CASE 
		WHEN  LEAD(ch.Interviewid, 1, 0 ) OVER (PARTITION BY ch.SurveyId, ch.InterviewId ORDER BY ch.id, h.id) <> ch.InterviewId AND ch.FiredTime < h.FiredTime
		THEN 1 
		ELSE 0
	END AS AttemptedAfterBlocked,


	CASE 
		WHEN its.item IS NOT NULL AND LEAD(h.InterviewId, 1, 0) OVER (PARTITION BY h.SurveyId, h.InterviewId ORDER BY h.id) <> h.InterviewId 
		THEN 1
		ELSE 0
	END AS Completed,

	CASE
		WHEN its.item is not null AND LEAD(h.InterviewId, 1, 0) OVER (PARTITION BY h.SurveyId, h.InterviewId ORDER BY h.id) <> h.InterviewId
			THEN count(h.interviewID) OVER(PARTITION BY h.SurveyId, h.InterviewId ORDER BY h.id) ELSE 0
	END	 AS AttemptsPerComplete,
	-- To avoid sorting  partition "PARTITION BY i.surveysid, i.id  ORDER BY i.id" we will use already used partition - diiferent i.id with h.id = nulls will be in one partition but statement below will work anyway
    CASE 
		WHEN LEAD(i.id, 1, 0) OVER(PARTITION BY h.SurveyId, h.InterviewId ORDER BY h.id) <> i.id  AND  i.TransientState = 17 --Blacklist
		THEN 1
		ELSE 0
	END	AS BlockedByBlacklist
     
FROM BvInterview i
LEFT JOIN BvHistory h
	ON i.id = h.InterviewId AND i.SurveySID = h.SurveyId
LEFT JOIN dbo.utilSplitNumbers(@its, ',') its
	ON h.ITS = its.item
LEFT JOIN BvViewBothCallHistories ch	
	ON i.ID = ch.InterviewID AND i.SurveySID = ch.SurveyId AND BlockedByFcd = 1
WHERE i.batchid = @BatchId AND ISNULL(h.RoleID, 2) = 2
)
select 
	ISNULL(MAX(RecordsInBatch),0)						[InterviewsCurrent],
	ISNULL(SUM(attempted),0)							[InterviewsAttempted],
	ISNULL(SUM(blockedExcludedAttempted),0)				[BlockedExcludedAttemptedInterviews],
	ISNULL(SUM(blockedAttempted),0)						[BlockedAttemptedInterviews],
	ISNULL(SUM(AttemptedAfterBlocked),0)				[AttemptedAfterBlocked],
	ISNULL(SUM(completed),0)		AS					[InterviewsCompleted],
	ISNULL(CASE 
		WHEN SUM(completed) > 0 
		THEN
			CAST (SUM(attempted)*1.0/SUM(completed) AS REAL)
		ELSE 0
	END, 0)							AS					[AttemptedInterviewsPerComplete],
	ISNULL(CAST(ISNULL(AVG(NULLIF(AttemptsPerComplete,0)*1.0), 0) AS REAL), 0) 
									AS					[AvgAttemptsPerComplete],
    SUM(BlockedByBlacklist)			AS					[BlockedByBlacklist]
from counts
)
GO
PRINT N'Altering Procedure [dbo].[BvSpCall_MoveToITS]...';


GO
ALTER PROCEDURE [dbo].[BvSpCall_MoveToITS]
@SurveySID   INTEGER,
@BatchID    INTEGER,
@StateID     INTEGER
AS
   IF (@SurveySID IS NULL AND @BatchID IS NULL AND @StateID IS NULL)
	 RETURN 0

   DECLARE @ProcessedCalls INT = 0
   DECLARE @SurveySchedulingMode INT 
   SELECT  @SurveySchedulingMode = SurveySchedulingMode
   FROM BvSurvey
   WHERE SID = @SurveySID
      
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
	 
		INSERT INTO BvCallhistoryEx
			SELECT GETUTCDATE(), c.ApptID, c.ShiftTypeID, i.Id, @SurveySID, i.its, i.Dialingmode, c.CallState, c.[Priority], c.TimeInShift, c.ExpireTime, c.ExplicitSid, c.ExplicitType, c.CellId, 
                @OperationId, @OperationType, @CallCenterId, c.DialTypeId
			FROM #InterviewIds i
			LEFT JOIN BvSvySchedule c ON c.InterviewID = i.ID AND c.SurveySID = @SurveySId 
			WHERE i.ID NOT IN (SELECT ID FROM #ids) 
   END

   EXEC BvSpDeleteTransfer @BatchID

RETURN @ProcessedCalls
GO
PRINT N'Altering Procedure [dbo].[BvSpGetExtendedCallHistory]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetExtendedCallHistory]
@InterviewID     INTEGER,
@SurveyID        INTEGER,
@CallCenterID	 INTEGER
AS
SET NOCOUNT OFF


	SELECT 
		h.[Id],
		[FiredTime],
		[ApptID],
		ITS,
		ISNULL(BvState.[Name],'')  AS TransientState,
		h.ShiftTypeId,
		h.DialingMode,
		ISNULL(BvShiftType.[Name], '' ) AS ShiftType, 
		[CallState] ,
		h.[Priority],
		[TimeInShift],
		CASE h.[ExpireTime] WHEN '9999-01-01 00:00:00.000' THEN NULL ELSE h.[ExpireTime] END AS [ExpireTime],
		[ExplicitSID],
		[ExplicitType],
		ISNULL(pg.[Name], '') AS Resource,
		[CellId],
		[OperationId],
		[OperationType],
        ISNULL(cc.Name, '') AS CallCenterName,
		ISNULL(dt.Name, '') AS DialType

	FROM BvViewBothCallHistories h
	INNER JOIN BvSurvey s ON s.SID = h.SurveyId 
	LEFT JOIN BvCallCenter cc ON cc.ID = h.CallCenterID
	LEFT JOIN BvShiftZones ON BvShiftZones.[ID] = h.ShiftTypeID  
	LEFT JOIN BvShiftType ON  BvShiftType.ObjectID = BvShiftZones.ShiftTypeID  
	LEFT JOIN BvState ON BvState.StateID = h.ITS AND BvState.StateGroupID = s.StateGroupID
	LEFT JOIN BvViewPersonAndGroup pg ON pg.SID = h.ExplicitSID
	LEFT JOIN BvDialType dt ON h.DialTypeId = dt.Id

	WHERE 
		SurveyId = @SurveyID AND InterviewID = @InterviewID
	ORDER BY h.[Id]

RETURN 0
GO
PRINT N'Altering Procedure [dbo].[BvSpReportSampleStatusSummaryForDatesRange]...';


GO
ALTER PROCEDURE [BvSpReportSampleStatusSummaryForDatesRange]
@SurveySID INT, 
@StartDate DATETIME,
@EndDate DATETIME
AS

DECLARE @Total INT

DECLARE @ItsCounts TABLE
(
	[ID] INT,
	[Name] VARCHAR(MAX),
	[Count] INT,
	[Percent] VARCHAR(MAX),
	[Total] INT
)

INSERT INTO @ItsCounts
SELECT 
	h.ITS AS [ID],
	st.Name AS [Name],
	COUNT(*) AS [Count],
	CAST(CAST( (COUNT(*)*1.0/(SUM(COUNT(*)) OVER()) * 100.0) as decimal(5,2)) as VARCHAR(MAX)) as [Percent],
	SUM(COUNT(*)) OVER()
FROM
	(
		SELECT ITS, SurveyId, FiredTime FROM Bvhistory
		UNION ALL
		SELECT ITS, SurveyId, FiredTime FROM BvViewBothCallHistories
		WHERE ITS = 15 OR ITS = 25
	) AS h
JOIN BvSurvey s
	ON s.SID = h.SurveyId
JOIN BvState st
	ON s.StateGroupID = st.StateGroupID and h.ITS = st.StateID
WHERE h.SurveyId = @SurveySID AND h.FiredTime BETWEEN @StartDate AND @EndDate
GROUP BY h.ITS, st.Name
ORDER BY h.ITS

SELECT TOP 1 @Total = [Total] FROM @ItsCounts
SELECT [ID], [Name], [Count], [Percent] FROM @ItsCounts

RETURN @Total
GO
PRINT N'Altering Procedure [dbo].[BvSpSurvey_Clean]...';


GO
ALTER PROCEDURE [dbo].[BvSpSurvey_Clean]
    @SurveyId INT
AS
    DECLARE @CountOfDeletedAssignment INT
    DECLARE @CountOfDeletedCalls INT

    DELETE BvPersonOrGroupAssignmentOnSurvey 
    WHERE SurveyId = @SurveyId

    SET @CountOfDeletedAssignment = @@ROWCOUNT
    
    DELETE FROM bvpersonrel
    WHERE type = 2 AND objectsid = @SurveyId
    
    DELETE FROM bvlogingroup WHERE surveysid = @surveyID
    
    DELETE FROM BvCallHistory Where SurveyId = @SurveyId

    DELETE FROM BvCallHistoryEx Where SurveyId = @SurveyId
    
    DELETE FROM BvSvySchedule WHERE SurveySid = @SurveyId
    SET @CountOfDeletedCalls = @@ROWCOUNT

    SELECT @CountOfDeletedAssignment as CountOfDeletedAssignment, @CountOfDeletedCalls as CountOfDeletedCalls
GO
PRINT N'Refreshing Procedure [dbo].[BvSpSampleUtilisationReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSampleUtilisationReport]';


GO
PRINT N'Update complete.';


GO
