PRINT N'Creating [dbo].[BvCallHistory]...';


GO
CREATE TABLE [dbo].[BvCallHistory] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [FiredTime]     SMALLDATETIME NOT NULL,
    [ApptID]        INT           NULL,
    [ShiftTypeID]   INT           NULL,
    [InterviewID]   INT           NOT NULL,
    [SurveyId]      INT           NOT NULL,
    [ITS]           TINYINT       NULL,
    [DialingMode]   TINYINT       NULL,
    [CallState]     SMALLINT      NULL,
    [Priority]      INT           NULL,
    [TimeInShift]   DATETIME      NULL,
    [ExpireTime]    DATETIME      NULL,
    [ExplicitSID]   INT           NULL,
    [ExplicitType]  TINYINT       NULL,
    [CellId]        INT           NULL,
    [OperationId]   INT           NOT NULL,
    [OperationType] TINYINT       NOT NULL,
    [CallCenterId]  INT           NOT NULL,
    CONSTRAINT [PK_BVCallHistory_ID] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[BvCallHistory].[IX_BvCallHistorySurveyID_InterviewID]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvCallHistorySurveyID_InterviewID]
    ON [dbo].[BvCallHistory]([SurveyId] ASC, [InterviewID] ASC)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[GetContextData]...';


GO
CREATE FUNCTION [dbo].[GetContextData]( )
RETURNS @Context TABLE(ITS TINYINT, OperationId INT, OperationType TINYINT, CallCenterId INT, DialingMode TINYINT) 
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
		DECLARE @ITS TINYINT
		DECLARE @DialingMode TINYINT

		SELECT @ITS = ITS, @OperationID = OperationId, @OperationType = OperationType, @CallCenterId = CallCenterId, @DialingMode = DialingMode from dbo.GetContextData()

		INSERT INTO BvCallHistory
		SELECT 
			GETUTCDATE(), ApptId, ShiftTypeId, InterviewId, SurveySid, @ITS, @DialingMode, CAST(CallState AS SMALLINT), [Priority], TimeInShift, Expiretime, 
			ExplicitSid, CAST(ExplicitType AS tinyint), CellId, @OperationId, @OperationType, @CallCenterId		
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
		DECLARE @ITS TINYINT
		DECLARE @DialingMode TINYINT

		SELECT @ITS = ITS, @OperationID = OperationId, @OperationType = OperationType, @CallCenterId = CallCenterId, @DialingMode = DialingMode from dbo.GetContextData()

		INSERT INTO BvCallHistory
		SELECT 
			GETUTCDATE(), ApptId, ShiftTypeId, InterviewId, SurveySid, @ITS, @DialingMode, CAST(CallState AS SMALLINT), [Priority], TimeInShift, Expiretime, 
			ExplicitSid, CAST(ExplicitType AS tinyint), CellId, @OperationId, @OperationType, @CallCenterId		
		FROM inserted
	END

END
GO
PRINT N'Altering [dbo].[BvSpCallHistory_List]...';


GO
ALTER PROCEDURE [dbo].[BvSpCallHistory_List]
@InterviewID     INTEGER,
@SurveyID        INTEGER,
@CallCenterID	 INTEGER
AS
SET NOCOUNT OFF
     DECLARE @StateGroupID INT = ( SELECT StateGroupID FROM BvSurvey WHERE SID = @SurveyID )
	 
	 DECLARE @TelephoneNumber NVARCHAR(MAX)
	 DECLARE @RespondentName NVARCHAR(MAX)
	 DECLARE @TimezoneID INT
	 DECLARE @BatchID INT
	 DECLARE @TimeZoneName NVARCHAR(MAX)
	
	 SELECT @TelephoneNumber = ISNULL(BvInterview.TelephoneNumber, '' ),
		    @RespondentName = ISNULL(BvInterview.RespondentName, '' ),
		    @TimezoneID = ISNULL(BvInterview.TimezoneID, 0 ),
		    @BatchID = BvInterview.BatchID,
		    @TimeZoneName = ISNULL(BvTimezone.[Name], '' )
		    FROM BvInterview
		    LEFT JOIN BvTimezone ON BvTimezone.[ID] = BvInterview.TimezoneID
		    WHERE BvInterview.ID = @InterviewID AND BvInterview.SurveySID = @SurveyID

     SELECT * FROM 
     (
		 SELECT
			  BvHistory.ID AS [ID],
			  BvHistory.SurveyId AS SurveyID,
			  BvHistory.FiredTime AS EndTime,
			  BvHistory.InterviewID AS InterviewID,
			  BvState.[StateID] AS ITS_ID,
			  BvState.[Name] AS TransientState,
			  BvHistory.WaitingTime AS WaitingTime,
			  BvHistory.Duration AS Duration,
			  ISNULL( BvRole.[Name], '' ) AS Role,
			  ISNULL( BvPerson.[Name], '' ) AS Person,
			  BvHistory.AppointmentID AS AppointmentID,
			  ISNULL(BvAppointment.ContactName, '' ) AS ContactName,
			  BvAppointment.[Time] AS TimeToCall,
			  BvAppointment.ExpTime AS TimeToExpire,
			  @TelephoneNumber AS TelephoneNumber,
			  @RespondentName AS RespondentName,
			  @TimezoneID AS TimeZoneID,
			  @TimeZoneName AS TimeZone,
			  'IsHistoryItemForChildInterview' = CAST(0 AS BIT),
			  ISNULL( BvCallCenter.Name, '' ) as CallCenterName
		 FROM BvHistory
		 INNER JOIN BvState ON BvState.StateGroupID = @StateGroupID AND BvState.[StateID] = BvHistory.ITS
		 LEFT JOIN BvPerson ON BvPerson.SID = BvHistory.PersonSID
		 LEFT JOIN BvRole ON BvRole.RoleID = BvHistory.RoleID
		 LEFT JOIN BvAppointment ON BvAppointment.[ID] = BvHistory.AppointmentID
		 LEFT JOIN BvCallCenter ON BvCallCenter.ID = BvHistory.CallCenterID
		 WHERE BvHistory.InterviewID = @InterviewID
			   AND BvHistory.SurveyId = @SurveyID
		 UNION ALL
		 SELECT 0 as [ID],
				@SurveyID as SurveyID,
				StartedTime as EndTime,
				@InterviewID as InterviewID,
				NULL as ITS_ID,
				'<Fresh sample>' as TransientState,
				0 as WaitingTime,
				0 as Duration,
				'Sample' as Role,
				NULL as Person,
				NULL as AppointmentID,
				'' as ContactName,
				NULL as TimeToCall,
				NULL as TimeToExpire,
				@TelephoneNumber AS TelephoneNumber,
				@RespondentName AS RespondentName,
				@TimezoneID AS TimeZoneID,
				@TimeZoneName AS TimeZone,
				'IsHistoryItemForChildInterview' = CAST(0 AS BIT),
				'' as CallCenterName
		 FROM BvSamples WHERE BatchID =  @BatchID
	 ) t
     ORDER BY DATEADD( s, -Duration, EndTime)

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpSurvey_Clean]...';


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

    DELETE FROM BvSvySchedule WHERE SurveySid = @SurveyId
    SET @CountOfDeletedCalls = @@ROWCOUNT

    SELECT @CountOfDeletedAssignment as CountOfDeletedAssignment, @CountOfDeletedCalls as CountOfDeletedCalls
GO
PRINT N'Creating [dbo].[BvSpGetExtendedCallHistory]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetExtendedCallHistory]
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
        ISNULL( cc.Name, '' ) AS CallCenterName

	FROM BvCallHistory h
	INNER JOIN BvSurvey s ON s.SID = h.SurveyId 
	LEFT JOIN BvCallCenter cc ON cc.ID = h.CallCenterID
	LEFT JOIN BvShiftZones ON BvShiftZones.[ID] = h.ShiftTypeID  
	LEFT JOIN BvShiftType ON  BvShiftType.ObjectID = BvShiftZones.ShiftTypeID  
	LEFT JOIN BvState ON BvState.StateID = h.ITS AND BvState.StateGroupID = s.StateGroupID
	LEFT JOIN BvViewPersonAndGroup pg ON pg.SID = h.ExplicitSID

	WHERE 
		SurveyId = @SurveyID AND InterviewID = @InterviewID
	ORDER BY h.[Id]

RETURN 0
GO

PRINT N'Altering [dbo].[BvSpCall_MoveToITS]...';

GO

ALTER PROCEDURE [dbo].[BvSpCall_MoveToITS]
@SurveySID   INTEGER,
@BatchID     INTEGER,
@StateID     INTEGER
AS
   DECLARE @CfDbSchemaPath NVARCHAR(255)
   DECLARE @ProcessedCalls INT = 0
   DECLARE @SurveySchedulingMode INT 
   SELECT @CfDbSchemaPath = CfDbSchemaPath,
		  @SurveySchedulingMode = SurveySchedulingMode
   FROM BvSurvey
   WHERE SID = @SurveySID
   
   CREATE TABLE #InterviewIds(Id INT, DialingMode TINYINT, its TINYINT)
   
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
   FROM #InterviewIds ids
   INNER JOIN BvState ON BvState.StateID = @StateID
   INNER JOIN BvSurvey ON BvSurvey.SID = @SurveySID AND
                          BvState.StateGroupID = BvSurvey.StateGroupID
   WHERE BvSvySchedule.SurveySID = @SurveySID AND 
         BvSvySchedule.InterviewId = ids.Id AND
         BvSvySchedule.CallState > 0
   
   IF (@@ROWCOUNT = 0 AND CONTEXT_INFO() IS NOT NULL) 
   BEGIN
	
		DECLARE @OperationType TINYINT
		DECLARE @CallCenterId INT
		DECLARE @OperationId INT

		SELECT @OperationID = OperationId, @OperationType = OperationType, @CallCenterId = CallCenterId from dbo.GetContextData()
	 
		INSERT INTO BvCallhistory
			SELECT GETUTCDATE(), c.ApptID, c.ShiftTypeID, i.Id, @SurveySID, i.its, i.Dialingmode, c.CallState, c.[Priority], c.TimeInShift, c.ExpireTime, c.ExplicitSid, c.ExplicitType, c.CellId, 
                @OperationId, @OperationType, @CallCenterId
			FROM #InterviewIds i
			LEFT JOIN BvSvySchedule c ON c.InterviewID = i.ID AND c.SurveySID = @SurveySId 
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

PRINT N'Update complete.';


GO
