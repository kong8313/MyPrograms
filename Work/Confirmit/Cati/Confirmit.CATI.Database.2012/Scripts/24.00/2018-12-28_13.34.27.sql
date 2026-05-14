GO

IF (SELECT OBJECT_ID('tempdb..#tmpSequenceState')) IS NOT NULL DROP TABLE #tmpSequenceState
GO
CREATE TABLE #tmpSequenceState
(
    [Name]           nvarchar(300)  NOT NULL,
    [PrevCurrent]    numeric(38, 0) NOT NULL,
    [PrevStart]      numeric(38, 0) NOT NULL,
    [PrevIncrement]  numeric(38, 0) NOT NULL,
    [NowStart]       numeric(38, 0) NOT NULL,
    [NowIncrement]   numeric(38, 0) NOT NULL
)
GO

GO
PRINT N'Dropping [dbo].[BvCallTransferSessions]...';


GO
DROP TABLE [dbo].[BvCallTransferSessions];


GO
PRINT N'Removing schema binding from [dbo].[GetCallByCondition]...';


GO
ALTER FUNCTION [dbo].[GetCallByCondition]
(@DialTypeId TINYINT, @ShiftTypeId INT, @SurveySid INT, @ExplicitSID INT, @ConditionValue INT, @Now DATETIME)
RETURNS TABLE 
AS
RETURN 
    (SELECT   TOP (1) [ID],
                      ExplicitSID,
                      ExplicitType,
                      SurveySID,
                      InterviewID,
                      CallState,
                      ApptId,
                      TimeInShift,
                      CallOrder,
                      Priority,
                      ConditionValue
     FROM     [dbo].BvSvySchedule
     WHERE    DialTypeId = @DialTypeId
              AND ShiftTypeId = @ShiftTypeId
              AND CallState = 2
              AND SurveySid = @SurveySid
              AND BvSvySchedule.ExplicitSID = @ExplicitSID
              AND BvSvySchedule.ConditionValue = @ConditionValue
              AND BvSvySchedule.ConditionValue <> 0
              AND BvSvySchedule.TimeInShift < @Now
     ORDER BY Priority DESC, TimeInShift, ExplicitType DESC, CallOrder)



GO
PRINT N'Removing schema binding from [dbo].[GetCallBySurvey]...';


GO
ALTER FUNCTION [dbo].[GetCallBySurvey]
(@DialTypeId TINYINT, @ShifttypeId INT, @SurveySid INT, @ExplicitSID INT, @Now DATETIME)
RETURNS TABLE 
AS
RETURN 
    (SELECT   TOP (1) [ID],
                      ExplicitSID,
                      ExplicitType,
                      SurveySID,
                      InterviewID,
                      CallState,
                      ApptId,
                      TimeInShift,
                      CallOrder,
                      Priority,
                      ConditionValue
     FROM     [dbo].BvSvySchedule
     WHERE    DialTypeId = @DialTypeId
              AND ShifttypeId = @ShifttypeId
              AND CallState = 2
              AND SurveySid = @SurveySid
              AND BvSvySchedule.ExplicitSID = @ExplicitSID
              AND ConditionValue <> 0
              AND TimeInShift < @Now
     ORDER BY Priority DESC, TimeInShift, ExplicitType DESC, CallOrder)



GO
PRINT N'Dropping [dbo].[BvDialIdSequence]...';


GO
INSERT #tmpSequenceState ([Name], [PrevCurrent], [PrevStart], [PrevIncrement], [NowStart], [NowIncrement])
SELECT N'[dbo].[BvDialIdSequence]',
        NEXT VALUE FOR [dbo].[BvDialIdSequence],
       1,
       1,
       1,
       1;

DROP SEQUENCE [dbo].[BvDialIdSequence];


GO
PRINT N'Creating [dbo].[BvBigIntArrayType]...';


GO
CREATE TYPE [dbo].[BvBigIntArrayType] AS TABLE (
    [Value] BIGINT NOT NULL);


GO
/*
The column [dbo].[BvActiveDial].[DdiNumber] is being dropped, data loss could occur.

The column [dbo].[BvActiveDial].[TelephoneNumber] is being dropped, data loss could occur.

The column [dbo].[BvActiveDial].[CampaignId] on table [dbo].[BvActiveDial] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.

The column CallId on table [dbo].[BvActiveDial] must be changed from NULL to NOT NULL. If the table contains data, the ALTER script may not work. To avoid this issue, you must add values to this column for all rows or mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.

The column InterviewId on table [dbo].[BvActiveDial] must be changed from NULL to NOT NULL. If the table contains data, the ALTER script may not work. To avoid this issue, you must add values to this column for all rows or mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.

The column MainPersonId on table [dbo].[BvActiveDial] must be changed from NULL to NOT NULL. If the table contains data, the ALTER script may not work. To avoid this issue, you must add values to this column for all rows or mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.

The column SurveyId on table [dbo].[BvActiveDial] must be changed from NULL to NOT NULL. If the table contains data, the ALTER script may not work. To avoid this issue, you must add values to this column for all rows or mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.
*/
GO
PRINT N'Starting rebuilding table [dbo].[BvActiveDial]...';


GO

DROP TABLE BvActiveDial;

GO

CREATE TABLE [dbo].[BvActiveDial] (
    [Id]                        BIGINT         NOT NULL,
    [Type]                      TINYINT        NOT NULL,
    [DialerId]                  INT            NOT NULL,
    [DialerTelephoneNumber]     NVARCHAR (MAX) NULL,
    [RespondentTelephoneNumber] NVARCHAR (MAX) NULL,
    [StartTime]                 DATETIME       NOT NULL,
    [AnswerTime]                DATETIME       NULL,
    [InboundCallId]             NVARCHAR (MAX) NULL,
    [TransferId]                NVARCHAR (MAX) NULL,
    [InitialSurveyId]           INT            NOT NULL,
    [State]                     TINYINT        NOT NULL,
    [SurveyId]                  INT            NOT NULL,
    [CampaignId]                BIGINT         NOT NULL,
    [InterviewId]               INT            NOT NULL,
    [CallId]                    INT            NOT NULL,
    [MainPersonId]              INT            NOT NULL,
    CONSTRAINT [PK_BvActiveDial] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
PRINT N'Creating [dbo].[BvActiveDial].[IX_BvActiveDial_CallId]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvActiveDial_CallId]
    ON [dbo].[BvActiveDial]([CallId] ASC);


GO
PRINT N'Starting rebuilding table [dbo].[BvDialHistory]...';


GO

EXECUTE sp_rename N'[dbo].[BvDialHistory].[DdiNumber]', N'DialerTelephoneNumber';

EXECUTE sp_rename N'[dbo].[BvDialHistory].[TelephoneNumber]', N'RespondentTelephoneNumber';

GO
PRINT N'Starting rebuilding table [dbo].[BvDialHistoryToInterviewHistory]...';

GO

CREATE TABLE [dbo].[tmp_ms_xx_BvDialHistoryToInterviewHistory] (
    [DialHistoryId]      BIGINT   NOT NULL,
    [InterviewHistoryId] INT      NOT NULL,
    [StartTime]          DATETIME NOT NULL,
    [FinishTime]         DATETIME NOT NULL,
    [PersonId]           INT      NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_BvDialHistoryToInterviewHistory1] PRIMARY KEY CLUSTERED ([InterviewHistoryId] ASC, [DialHistoryId] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[BvDialHistoryToInterviewHistory])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_BvDialHistoryToInterviewHistory] ([InterviewHistoryId], [DialHistoryId], [StartTime], [FinishTime], [PersonId])
        SELECT   [InterviewHistoryId],
                 [DialHistoryId],
                 [StartTime],
                 [FinishTime],
                 [PersonId]
        FROM     [dbo].[BvDialHistoryToInterviewHistory]
        ORDER BY [InterviewHistoryId] ASC, [DialHistoryId] ASC;
    END

DROP TABLE [dbo].[BvDialHistoryToInterviewHistory];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_BvDialHistoryToInterviewHistory]', N'BvDialHistoryToInterviewHistory';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_BvDialHistoryToInterviewHistory1]', N'PK_BvDialHistoryToInterviewHistory', N'OBJECT';

GO
PRINT N'Creating [dbo].[BvDialHistoryToInterviewHistory].[IX_BvDialHistoryToInterviewHistory_DialHistoryId]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvDialHistoryToInterviewHistory_DialHistoryId]
    ON [dbo].[BvDialHistoryToInterviewHistory]([DialHistoryId] ASC);


GO
PRINT N'Altering [dbo].[BvSvySchedule]...';


GO
ALTER TABLE [dbo].[BvSvySchedule]
    ADD [DialerId]     INT    CONSTRAINT [DF_BvSvySchedule_DialerId] DEFAULT (0) NOT NULL,
        [ActiveDialId] BIGINT CONSTRAINT [DF_BvSvySchedule_ActiveDialId] DEFAULT (0) NOT NULL;


GO
PRINT N'Creating [dbo].[BvDialIdSequence]...';


GO
CREATE SEQUENCE [dbo].[BvDialIdSequence]
    AS BIGINT
    START WITH 1
    INCREMENT BY 1;


GO
PRINT N'Adding schema binding to [dbo].[GetCallByCondition]...';


GO
ALTER FUNCTION [dbo].[GetCallByCondition]
(   @DialTypeId TINYINT,
	@ShiftTypeId INT,
    @SurveySid INT,
	@ExplicitSID INT,
	@ConditionValue INT,
	@Now DATETIME) 
RETURNS TABLE WITH SCHEMABINDING
AS RETURN(
		    SELECT TOP(1) [ID],
		                ExplicitSID,
						ExplicitType,
		                SurveySID,
		                InterviewID,
		                CallState,
						ApptId,
						TimeInShift,
						CallOrder,
						Priority,
						ConditionValue
		    FROM [dbo].BvSvySchedule
		    WHERE 
				DialTypeId = @DialTypeId AND
			    ShiftTypeId = @ShiftTypeId AND
				CallState = 2 AND
				SurveySid = @SurveySid AND
				BvSvySchedule.ExplicitSID = @ExplicitSID AND
				BvSvySchedule.ConditionValue  = @ConditionValue AND
				BvSvySchedule.ConditionValue <> 0 AND 
				BvSvySchedule.TimeInShift < @Now
		ORDER BY Priority DESC, TimeInShift, ExplicitType DESC, CallOrder )
GO
PRINT N'Adding schema binding to [dbo].[GetCallBySurvey]...';


GO
ALTER FUNCTION [dbo].[GetCallBySurvey]
(   
	@DialTypeId TINYINT,
    @ShifttypeId INT,
    @SurveySid INT,
    @ExplicitSID INT,
	@Now DATETIME) 
RETURNS TABLE WITH SCHEMABINDING
AS RETURN(
          SELECT TOP(1) [ID],
                        ExplicitSID,
                        ExplicitType,
                        SurveySID,
                        InterviewID,
                        CallState,
                        ApptId,
                        TimeInShift,
                        CallOrder,
                        Priority,
                        ConditionValue
          FROM [dbo].BvSvySchedule
          WHERE 
                    DialTypeId = @DialTypeId AND
                    ShifttypeId = @ShifttypeId AND
                    CallState = 2 AND
                    SurveySid = @SurveySid AND
                    BvSvySchedule.ExplicitSID = @ExplicitSID AND
                    ConditionValue <> 0 AND
					TimeInShift < @Now
            ORDER BY Priority DESC, TimeInShift, ExplicitType DESC, CallOrder )
GO
PRINT N'Refreshing [dbo].[GetCallsForPredictiveMode]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[GetCallsForPredictiveMode]';


GO
PRINT N'Refreshing [dbo].[GetPriorityCallByExplicitSidAndShiftTypeId]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[GetPriorityCallByExplicitSidAndShiftTypeId]';


GO
PRINT N'Refreshing [dbo].[GetPriorityCallByExplicitSidAndShiftTypeIdClustered]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[GetPriorityCallByExplicitSidAndShiftTypeIdClustered]';


GO
PRINT N'Refreshing [dbo].[GetTopCallsForShiftTypeGroupCell]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[GetTopCallsForShiftTypeGroupCell]';


GO
PRINT N'Refreshing [dbo].[GetCallsForGroupForPredictiveSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[GetCallsForGroupForPredictiveSurvey]';


GO
PRINT N'Refreshing [dbo].[GetCallsPerGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[GetCallsPerGroup]';


GO
PRINT N'Refreshing [dbo].[GetHighPriorityCalls]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[GetHighPriorityCalls]';


GO
PRINT N'Altering [dbo].[BvSpActiveDial_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpActiveDial_Delete]
 @IDs BvBigIntArrayType READONLY,
 @CallCompleteStatus TINYINT
AS
BEGIN TRAN
	DECLARE @Values TABLE( Id BIGINT, Type TINYINT, DialerId INT, DialerTelephoneNumber NVARCHAR(MAX), RespondentTelephoneNumber NVARCHAR(MAX), StartTime DATETIME, AnswerTime DATETIME, InboundCallId NVARCHAR(MAX), InitialSurveyId INT, CallId INT)

	DELETE FROM BvActiveDial 
		OUTPUT	deleted.Id, deleted.Type, deleted.DialerId, deleted.DialerTelephoneNumber, deleted.RespondentTelephoneNumber,
				deleted.StartTime, deleted.AnswerTime, deleted.InboundCallId, deleted.InitialSurveyId, deleted.CallId INTO @Values
			WHERE ID IN ( SELECT Value FROM @IDs)
	
	DECLARE @Now DATETIME = [dbo].GetUtcNow()

	UPDATE BvSvySchedule SET DialerId = 0, ActiveDialId = 0 FROM BvSvySchedule c INNER JOIN @Values v ON c.ID = v.CallId

	INSERT INTO BvDialHistory( Id, Type, DialerId, DialerTelephoneNumber, RespondentTelephoneNumber, InboundCallId, InitialSurveyId, CallCompleteStatus, StartTime, AnswerTime, FinishTime )
		SELECT Id, Type, DialerId, DialerTelephoneNumber, RespondentTelephoneNumber, InboundCallId, InitialSurveyId, @CallCompleteStatus, StartTime, AnswerTime, @Now
			FROM @Values
COMMIT TRAN
GO
PRINT N'Altering [dbo].[BvSpActiveDial_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpActiveDial_Insert]
 @Type TINYINT,
 @DialerId INT,
 @DialerTelephoneNumber NVARCHAR(MAX),
 @RespondentTelephoneNumber NVARCHAR(MAX),
 @State TINYINT,
 @InboundCallId NVARCHAR(MAX),
 @InitialSurveyId INT,
 @SurveyId INT,
 @CampaignId BIGINT,
 @InterviewId INT,
 @CallId INT,
 @MainPersonId INT
AS
	DECLARE @OldIds BvBigIntArrayType 

	IF @Type = 1/*Inbound*/ 
		INSERT INTO @OldIds SELECT ID FROM BvActiveDial WHERE InboundCallId = @InboundCallId
	ELSE IF @Type = 0/*Outbound*/
	    INSERT INTO @OldIds SELECT ID FROM BvActiveDial WHERE CallId = @CallId
	
	IF @@ROWCOUNT <> 0
	BEGIN
		EXEC BvSpActiveDial_Delete @OldIds, 0/*CallCompleteStatus.Error*/
	END

	DECLARE @ID BIGINT = NEXT VALUE FOR [dbo].[BvDialIdSequence];
	
	IF @CallId IS NOT NULL
	BEGIN
		UPDATE BvSvySchedule SET ActiveDialId = @ID, DialerId = @DialerId WHERE ID = @CallId
	END

	INSERT INTO [dbo].[BvActiveDial]( [Id] ,[Type] ,[DialerId] ,[DialerTelephoneNumber] ,[RespondentTelephoneNumber] ,[StartTime] ,[State], InboundCallId, InitialSurveyId, SurveyId, CampaignId, InterviewId, CallId, MainPersonId)
		OUTPUT inserted.*
		VALUES( @ID ,@Type, @DialerId, @DialerTelephoneNumber, @RespondentTelephoneNumber, [dbo].GetUtcNow(), @State, @InboundCallId, @InitialSurveyId, @SurveyId, @CampaignId, @InterviewId, @CallId, @MainPersonId)
GO
PRINT N'Altering [dbo].[BvSpActiveDial_Update]...';


GO
ALTER PROCEDURE [dbo].[BvSpActiveDial_Update]
 @Id BIGINT,
 @State TINYINT,
 @AnswerTime DATETIME,
 @TransferId NVARCHAR(MAX),
 @SurveyId INT,
 @CampaignId BIGINT,
 @InterviewId INT,
 @CallId INT,
 @MainPersonId INT
AS
	DECLARE @OldCallId INT
	DECLARE @DialerId INT
	
	UPDATE BvActiveDial
		SET @OldCallId = CallId,
		    @DialerId = DialerId,
			State = @State,
			AnswerTime = @AnswerTime,
			TransferId = @TransferId,
			SurveyId = @SurveyId,
			CampaignId = @CampaignId,
			InterviewId = @InterviewId,
			CallId = @CallId,
			MainPersonId = @MainPersonId
		WHERE Id = @Id

	IF ISNULL( @OldCallId, 0 ) <> ISNULL( @CallId, 0 )
	BEGIN
		IF @OldCallId IS NOT NULL
			UPDATE BvSvySchedule SET DialerId = 0, ActiveDialId = 0 WHERE ID = @OldCallId

		IF @CallId IS NOT NULL
			UPDATE BvSvySchedule SET DialerId = @DialerId, ActiveDialId = @Id WHERE ID = @CallId
	END
GO
PRINT N'Altering [dbo].[BvSpCall_Get]...';


GO
ALTER PROCEDURE [dbo].[BvSpCall_Get]
    @SurveyID int,
    @InterviewID int,
    @Delete int,
    @GetLiveCall int = 0
AS
	DECLARE @OldCallState INT
	DECLARE @IsLockObtained INT = 0

	IF @Delete = 1
	BEGIN
       
       UPDATE BvSvySchedule 
       SET	@OldCallState = CallState,
			CallState = -1
       WHERE SurveySID = @SurveyID AND 
             InterviewID = @InterviewID AND
             CallState > 0
             
        SET @IsLockObtained = @@ROWCOUNT
             
		UPDATE BvAppointment
		SET STATE = 2
		WHERE SurveySID = @SurveyID AND
			  InterviewSID = @InterviewID AND
			  STATE = 1
    END

	SELECT
		BvSvySchedule.[ID] callid,
		BvSvySchedule.ApptID,
		BvSvySchedule.SurveySID,
		BvSvySchedule.InterviewID iid,
		ISNULL( @OldCallState, BvSvySchedule.CallState ) as CallState,
		ISNULL( BvShiftZones.[ShiftTypeID], BvSvySchedule.[ShiftTypeID] ) ShiftID,
		BvSvySchedule.Priority,
		BvSvySchedule.TimeInShift,
		BvSvySchedule.ExpireTime TimeToExpire,
		CASE WHEN BvSvySchedule.ExplicitType = 2 THEN BvSvySchedule.ExplicitSID ELSE 0 END AS Resource,
		BvSvySchedule.ExplicitType Resource_Type,
		OldPriority,
		RuleNumber,
		ConditionValue,
		BvSvySchedule.CellId,
		BvSvySchedule.DialTypeId,
		BvSvySchedule.Type,
		BvSvySchedule.DialerId,
		BvSvySchedule.ActiveDialId
	FROM BvSvySchedule 
	LEFT JOIN BvShiftZones ON BvSvySchedule.ShiftTypeID = BvShiftZones.[ID]
	WHERE BvSvySchedule.SurveySID = @SurveyID AND 
		 BvSvySchedule.InterviewID = @InterviewID AND
		 ( ISNULL( @OldCallState, BvSvySchedule.CallState ) > 0 OR ( @GetLiveCall <> 0 AND ISNULL( @OldCallState, BvSvySchedule.CallState ) < 0 AND ISNULL( @OldCallState, BvSvySchedule.CallState ) > -3) )
			 
RETURN @IsLockObtained
GO
PRINT N'Altering [dbo].[BvSpCall_GetExpiredAndLock]...';


GO
ALTER PROCEDURE [dbo].[BvSpCall_GetExpiredAndLock]
	@LastId INT,
	@Now DATETIME
AS
	DECLARE @SurveyId INT
	DECLARE @InterviewId INT
	DECLARE @OldCallState INT

	;WITH data as (
	SELECT TOP(1) * FROM dbo.[BvSvySchedule] with(readpast, INDEX([IX_BvTime]))
	WHERE CallState > 0 AND ExpireTime < @Now AND ID > @LastId
	ORDER BY ID
	)
	UPDATE data SET @OldCallState = CallState, @SurveyId = SurveySID, @InterviewId = InterviewId, CallState = -1

	UPDATE BvAppointment
	SET STATE = 2
	WHERE SurveySID = @SurveyID AND InterviewSID = @InterviewID AND STATE = 1

	SELECT
		BvSvySchedule.[ID] callid,
		BvSvySchedule.ApptID,
		BvSvySchedule.SurveySID,
		BvSvySchedule.InterviewID iid,
		ISNULL( @OldCallState, BvSvySchedule.CallState ) as CallState,
		ISNULL( BvShiftZones.[ShiftTypeID], BvSvySchedule.[ShiftTypeID] ) ShiftID,
		BvSvySchedule.Priority,
		BvSvySchedule.TimeInShift,
		BvSvySchedule.ExpireTime TimeToExpire,
		CASE WHEN BvSvySchedule.ExplicitType = 2 THEN BvSvySchedule.ExplicitSID ELSE 0 END AS Resource,
		BvSvySchedule.ExplicitType Resource_Type,
		OldPriority,
		RuleNumber,
		ConditionValue,
		BvSvySchedule.CellId,
		BvSvySchedule.DialTypeId,
		BvSvySchedule.Type,
		BvSvySchedule.[DialerId],
		BvSvySchedule.[ActiveDialId]
	FROM BvSvySchedule
	LEFT JOIN BvShiftZones ON BvSvySchedule.ShiftTypeID = BvShiftZones.[ID]
	WHERE BvSvySchedule.SurveySID = @SurveyID AND BvSvySchedule.InterviewID = @InterviewID
GO
PRINT N'Altering [dbo].[BvSpCall_GetInfo]...';


GO
ALTER PROCEDURE [dbo].[BvSpCall_GetInfo]
 @CallID INT
AS
 SELECT
   [ID] callId,
   [ApptID],
   [SurveySID],
   [InterviewID] iid,
   [CallState],
   [ShiftTypeID] ShiftID,
   [Priority],
   [TimeInShift],
   [ExpireTime] TimeToExpire,
   [ExplicitSID] Resource,
   [ExplicitType] Resource_Type,
   [OldPriority],
   [RuleNumber],
   [ConditionValue],
   [CellId],
   [DialTypeId],
   [Type],
   [DialerId],
   [ActiveDialId]
 FROM [dbo].[BvSvySchedule]
 WHERE [ID] = @CallID
GO
PRINT N'Altering [dbo].[BvSpSvySch_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpSvySch_Insert]
        @ID                 int,
        @ApptID             int,
        @SurveySID          int,
        @InterviewID        int,
        @CallState          int,
        /* 
         * @ShiftTypeID > 0 means specific shift type id( BvShiftType.ID ) and should be resolved to ShiftZoneId in bvSvySchedule.ShiftTypeID
         * @ShiftTypeID = Int32.MinValue(-2147483648) meens [None] and should ne resolved to Int32.MinValue in BvSvySchedule.ShiftTypeID
         * @ShiftTypeID =-1 @ShiftTypeID means [Any valid] and should be resolved to -Timezone in BvSvySchedule.ShiftTypeID
         */
        @ShiftTypeID        int,
        @Priority           int,
        @TimeInShift        datetime,
        @ExpireTime         datetime,
        @Resource           int,
        @RuleNumber         uniqueidentifier,
        @DefaultTimeZoneID  INT,
	    @FcdBehaviorAlgorithm INT, --0 - delete calls/ 1 - disable calls
	    @transientState     INT,
		@DialTypeId			TINYINT,
		@Type				TINYINT,
		@DialerId           INT,
		@ActiveDialId       BIGINT,
		@CallTZ			    INT
AS
SET NOCOUNT ON
DECLARE @Rows INTEGER
DECLARE @ExplicitSID INTEGER
DECLARE @ExplicitType INTEGER

DECLARE @sqlQueryParams NVARCHAR(MAX)
DECLARE @sqlQuery NVARCHAR(MAX)
DECLARE @whereCondition NVARCHAR(MAX)
DECLARE @ClusteredCellIdQuery NVARCHAR(MAX) 
DECLARE @ROWCOUNT INT = 0
DECLARE @alias NVARCHAR(25) = 'repl'
DECLARE @ShiftTypeNone INT = -2147483648; --None constant for bvSvySchedule.ShiftTypeID
DECLARE @currentTransientState INT
DECLARE @ConditionValue INT = 0
DECLARE @DialTypeIdFromBvInterview TINYINT

    SET @InterviewID = ABS( @InterviewID )

    -- Get call TZ
    SELECT @CallTZ = ISNULL( @CallTZ, TimezoneID),
        @currentTransientState = TransientState,
        @DialTypeIdFromBvInterview = DialTypeId
    FROM BvInterview
    WHERE SurveySID = @SurveySID AND 
         [ID] = @InterviewID
         
	IF @DialTypeIdFromBvInterview IS NOT NULL
		SET @DialTypeId = @DialTypeIdFromBvInterview

    SET @CallTZ = ISNULL( @CallTZ, 0 )

    IF  @ShiftTypeID <> @ShiftTypeNone --Not [None]
    BEGIN
        DECLARE @ret INT       
        DECLARE @actualShiftTypeID INT
 
		/*
			@ShiftTypeID can contain negative timezone value
			but BvSpCheckCallOnShifts does not understand such values -
			in this case it should think that @ShiftTypeID = -1 [Any Valid] 
		*/
        IF @ShiftTypeID > 0
			SET @actualShiftTypeID = @ShiftTypeID
		ELSE
			SET @actualShiftTypeID = -1
        
        /*
			Note: we remove "Checking call time to be Out of Shifts", because if time in shift isn't hit to 
			shift of specific shift type, call will be delivered bit late.
			But we should call BvSpCheckCallOnShifts without TimeInShift, because we should check that 
			specific shifttype have somoething available shifts for specific timezone.
		*/
        EXEC @ret = BvSpCheckCallOnShifts @CallTZ, @actualShiftTypeID, NULL/*@TimeInShift*/, @SurveySID, @DefaultTimeZoneID
        IF @ret <> 0
            RETURN @ret
    END

    IF @Resource = 0
    BEGIN
        SET @ExplicitSID = @SurveySID

        SET @ExplicitType = 1
        IF @ExplicitSID IS NULL
        BEGIN
            RAISERROR( 'Could not find assignment group, %i', 16, 1, @ExplicitSID )
            RETURN -50002
        END
    END
    ELSE
    BEGIN
        SET @ExplicitSID = @Resource
        SET @ExplicitType = 2
    END

    IF @ShiftTypeID > 0--meens specific shift type id
    BEGIN
        SELECT @ShiftTypeID = [ID]
            FROM BvShiftZones WHERE ShiftTypeID = @ShiftTypeID
                AND TimeZoneID = @CallTZ
    END
    ELSE IF @ShiftTypeID <> @ShiftTypeNone -- means [Any valid]
    BEGIN
		SET @ShiftTypeID = -@CallTZ
    END
    --ELSE/*@ShiftTypeID = @ShiftTypeNone*/ -- means [None]
    --BEGIN
	--	SET @ShiftTypeID = @ShiftTypeNone
    --END

    DECLARE @ExpirationTime DATETIME = @ExpireTime
    DECLARE @TimeInShift1 DATETIME = @TimeInShift
    
    IF @ExpireTime IS NULL
        SET @ExpirationTime = '9999-01-01 00:00:00.000'
    
    IF @TimeInShift IS NULL
        SET @TimeInShift1 = '1899-12-30 00:00:00.000'

    DECLARE @oldApptID INT = NULL
    
    DECLARE @IsRandomCallDeliveryEnabled BIT
    DECLARE @SurveySchedulingMode INT
    DECLARE @ClusteredQuotaName NVARCHAR(256)
    DECLARE @StateGroupId INT 
    
	SELECT @IsRandomCallDeliveryEnabled = IsRandomCallDeliveryEnabled,
                   @SurveySchedulingMode = SurveySchedulingMode,
		   @ClusteredQuotaName = ClusteredQuotaName,
		   @StateGroupId = StateGroupID
	FROM BvSurvey
	WHERE SID = @SurveySID

    IF @transientState = 0 
	BEGIN
		SET @transientState = @currentTransientState
	END
    IF @SurveySchedulingMode = 1 /*CallGroup*/
    BEGIN
    	SET @ConditionValue = @transientState
    END

    EXEC BvClr_QuotaService_GetWhereForAllClosedSurveyCells @SurveySID, @alias, @whereCondition OUTPUT
	
	EXEC BvClr_ClusterQuotaService_GetCellIdQuery @SurveySID, @ClusteredQuotaName, @alias, @ClusteredCellIdQuery OUTPUT
	
	SET @sqlQuery = 
      N'SET @refID = 0
        MERGE BvSvySchedule as target
        USING( SELECT ci.*, CASE WHEN ' + @whereCondition + ' THEN 1 ELSE 0 END, s.FcdAction, ( ' + @ClusteredCellIdQuery + ') as CellId
					 FROM ( SELECT @SurveySID as SurveySID, @InterviewId as InterviewId, @ApptID as ApptId ) ci
					 LEFT JOIN BvState s ON @transientState = s.StateId AND s.StateGroupId = @StateGroupId
					 LEFT JOIN BvReplicatedData_' + CAST(@SurveySID AS NVARCHAR(255)) + ' as repl
                     ON respid = @InterviewId
					  ) AS source (SurveySid, InterviewId, Appt, IsClosed, FcdAction, CellId)
        ON target.SurveySID = source.SurveySID AND
           target.InterviewID = source.InterviewID
        WHEN MATCHED
        THEN
			  UPDATE
			  SET @refoldApptID     = ApptID,
			      @refID            = CASE WHEN FcdAction = 1 OR IsClosed = 0 OR @FcdBehaviorAlgorithm = 1 OR @CallState = 3 THEN ID ELSE 0 END,
				  ApptID            = @ApptID,
				  CallState         = CASE WHEN FcdAction = 1 OR IsClosed = 0 OR @CallState = 3 THEN @CallState ELSE @FcdBehaviorAlgorithm END,
				  Priority          = @Priority,
				  TimeInShift       = @TimeInShift1,
				  ExpireTime        = @ExpirationTime,
				  ShiftTypeID       = @ShiftTypeID,
				  ExplicitSID       = @ExplicitSID,
				  ExplicitType      = @ExplicitType,
				  RuleNumber        = @RuleNumber,
                  ConditionValue    = @ConditionValue,
				  OldPriority       = 0,
				  Type			    = @Type,
				  DialerId          = @DialerId,
				  ActiveDialId      = @ActiveDialId
        WHEN NOT MATCHED AND ( FcdAction = 1 OR IsClosed = 0 OR @FcdBehaviorAlgorithm = 1 OR @CallState = 3)
        THEN
              INSERT(ShiftTypeID,
                     ApptID,
                     InterviewID,
                     SurveySID,
                     CallState,
                     Priority,
                     TimeInShift,
                     ExpireTime,
                     ExplicitSID,
                     ExplicitType,
                     RuleNumber,
                     CallOrder,
					 ConditionValue,
					 CellId,
					 DialTypeId,
					 Type,
					 DialerId,
					 ActiveDialId)
              VALUES(@ShiftTypeID,
                     @ApptID,
                     @InterviewID,
                     @SurveySID,
                     CASE WHEN FcdAction = 1 OR IsClosed = 0 OR @CallState = 3 /*disabled by User*/ THEN @CallState ELSE 1/*disabled by FCD*/ END,
                     @Priority,
                     @TimeInShift1,
                     @ExpirationTime,
                     @ExplicitSID,
                     @ExplicitType,
                     @RuleNumber,
                     CASE WHEN @IsRandomCallDeliveryEnabled = 0 THEN InterviewId
						  ELSE dbo.GetRandomValue(@InterviewID)
					 END,
					 @ConditionValue,
					 CellId,
					 @DialTypeId,
					 @Type,
					 @DialerId,
					 @ActiveDialId);
         
        IF( (@@ROWCOUNT > 0) AND (@refoldApptID IS NULL))
           SET @refID = @@IDENTITY'
           
SET @sqlQueryParams = N'@SurveySID INT, @IsRandomCallDeliveryEnabled BIT, @CallState INT, @ApptID INT, @Priority INT, ' +
       '@TimeInShift1 DATETIME, @ExpirationTime DATETIME, @ShiftTypeID INT, @ExplicitSID INT, ' +
       '@ExplicitType INT, @RuleNumber uniqueidentifier, @InterviewID INT, @ConditionValue INT, ' +
	   '@FcdBehaviorAlgorithm INT, @StateGroupId INT, @transientState INT, @DialTypeId TINYINT, ' +
       '@Type TINYINT, @DialerId INT, @ActiveDialId BIGINT, @refID INT OUTPUT, @refoldApptID INT OUTPUT'
       
    EXEC sp_executesql @sqlQuery, @sqlQueryParams, @SurveySID, @IsRandomCallDeliveryEnabled, @CallState, @ApptID, @Priority,
       @TimeInShift1, @ExpirationTime, @ShiftTypeID, @ExplicitSID, @ExplicitType, @RuleNumber,
       @InterviewID, @ConditionValue, @FcdBehaviorAlgorithm, @StateGroupId, @transientState, @DialTypeId, @Type, @DialerId, @ActiveDialId, @refID = @ID OUTPUT, @refoldApptID = @oldApptID OUTPUT

    IF @ID > 0 --call was updated or inserted
    BEGIN         
		IF @oldApptID > 0
		  UPDATE BvAppointment
		  SET State = 2
		  WHERE ID = @oldApptID
		
	    IF @ApptID > 0
		  UPDATE BvAppointment SET State = 1 WHERE ID = @ApptID 
	END
  
RETURN (@ID)
GO
PRINT N'Creating [dbo].[BvSpActiveDial_InsertOutboundBatch]...';


GO
CREATE PROCEDURE [dbo].[BvSpActiveDial_InsertOutboundBatch]
 @DialerId INT,
 @State TINYINT,
 @SurveyId INT,
 @CampaignId BIGINT,
 @InterviewIds BvIntArrayType READONLY
AS
	DECLARE @OldIds BvBigIntArrayType 

	INSERT INTO @OldIds SELECT ID FROM BvActiveDial ad INNER JOIN @InterviewIds cids ON ad.SurveyId = @SurveyId AND  ad.InterviewId = cids.Value
	
	IF @@ROWCOUNT <> 0
	BEGIN
		EXEC BvSpActiveDial_Delete @OldIds, 0/*CallCompleteStatus.Error*/
	END

	DECLARE @Calls TABLE( SurveyId INT, InterviewId INT, CallId INT, ActiveDialId BIGINT, TelephoneNumber NVARCHAR(MAX), ExtensionNumber NVARCHAR(MAX))

	UPDATE BvSvySchedule
		SET DialerId = @DialerId,
			ActiveDialId = NEXT VALUE FOR [dbo].[BvDialIdSequence]
		OUTPUT inserted.SurveySID, inserted.InterviewID, inserted.ID, inserted.ActiveDialId, i.TelephoneNumber, i.ExtensionNumber INTO @Calls
		FROM BvSvySchedule c
		INNER JOIN @InterviewIds cids ON c.InterviewID = cids.Value AND c.SurveySID = @SurveyId
		LEFT JOIN BvInterview i ON c.SurveySID = i.SurveySID AND c.InterviewID = i.ID


	INSERT INTO [dbo].[BvActiveDial]( [Id] ,[Type] ,[DialerId] ,[StartTime] ,[State], InitialSurveyId, SurveyId, CampaignId, InterviewId, CallId, DialerTelephoneNumber, RespondentTelephoneNumber, MainPersonId)
		OUTPUT inserted.*
		SELECT c.ActiveDialId, 0/*Outbound*/, @DialerId, [dbo].GetUtcNow(), @State, @SurveyId, @SurveyId, @CampaignId, c.InterviewId, c.CallId, c.ExtensionNumber, c.TelephoneNumber, 0 FROM @Calls c
GO
PRINT N'Refreshing [dbo].[BvSpReportInboundCalls]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpReportInboundCalls]';


GO
PRINT N'Refreshing [dbo].[BvSpAlert_RecalculateAppointment]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlert_RecalculateAppointment]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignment_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignment_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignment_Insert2]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_Insert2]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignment_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_List]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignmentResource_ListUnused]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignmentResource_ListUnused]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignmentResource_TryDelete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignmentResource_TryDelete]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_Activate]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_Activate]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_ChangePriority]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_ChangePriority]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_ChangeShiftType]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_ChangeShiftType]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_Enable]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_Enable]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_MoveToITS]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_MoveToITS]';


GO
PRINT N'Refreshing [dbo].[BvSpCalls_Delete_Batch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCalls_Delete_Batch]';


GO
PRINT N'Refreshing [dbo].[BvSpCluster_TryIncrenent]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCluster_TryIncrenent]';


GO
PRINT N'Refreshing [dbo].[BvSpDialer_Reset]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpDialer_Reset]';


GO
PRINT N'Refreshing [dbo].[BvSpGetAllAppointmentsForUser]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetAllAppointmentsForUser]';


GO
PRINT N'Refreshing [dbo].[BvSpGetDialerCallsBreakdown]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetDialerCallsBreakdown]';


GO
PRINT N'Refreshing [dbo].[BvSpInterview_UpdateRespondentFields]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterview_UpdateRespondentFields]';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson_ForManualMode]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson_ForManualMode]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_GetAssignedSurveyList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_GetAssignedSurveyList]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonAndGroups_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonAndGroups_List]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonGroup_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonGroup_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpReleaseCall]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpReleaseCall]';


GO
PRINT N'Refreshing [dbo].[BvSpSchedule_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSchedule_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpSetCallDeliveryMode]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSetCallDeliveryMode]';


GO
PRINT N'Refreshing [dbo].[BvSpSetCallState]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSetCallState]';


GO
PRINT N'Refreshing [dbo].[BvSpSetNextInterviewForPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSetNextInterviewForPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpShiftType_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpShiftType_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Clean]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Clean]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_DeassignFromCallCenter]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_DeassignFromCallCenter]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_DeleteFiltered]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_DeleteFiltered]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Shutdown]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Shutdown]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyCleanup_IsClean]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyCleanup_IsClean]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyModifyStateGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyModifyStateGroup]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyState_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyState_Update]';


GO
PRINT N'Refreshing [dbo].[BvSpSvySch_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSvySch_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpTimezone_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTimezone_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpTimezone_DeleteUnused]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTimezone_DeleteUnused]';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson_ForCallGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson_ForCallGroup]';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson_ForSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson_ForSurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpGetActiveCallsForSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetActiveCallsForSurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnly]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnly]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyByPersonGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyByPersonGroup]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson_ForAssignmentMode]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson_ForAssignmentMode]';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson_ForAssignmentModeClustered]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson_ForAssignmentModeClustered]';


GO
PRINT N'Refreshing [dbo].[BvSpCallCenter_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallCenter_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Update]';


GO
PRINT N'Restoring state for sequences that were re-created.';


GO
DECLARE sequenceStateCursor CURSOR LOCAL FORWARD_ONLY
    FOR SELECT [Name],
               [PrevCurrent],
               [PrevStart],
               [PrevIncrement],
               [NowStart],
               [NowIncrement]
        FROM   #tmpSequenceState;

DECLARE @sequenceName AS NVARCHAR (MAX);

DECLARE @prevCurrent AS NUMERIC (38);

DECLARE @prevStart AS NUMERIC (38);

DECLARE @prevIncrement AS NUMERIC (38);

DECLARE @nowStart AS NUMERIC (38);

DECLARE @nowIncrement AS NUMERIC (38);

BEGIN TRY
    OPEN sequenceStateCursor;
    FETCH sequenceStateCursor INTO @sequenceName, @prevCurrent, @prevStart, @prevIncrement, @nowStart, @nowIncrement;
    WHILE @@fetch_status = 0
        BEGIN
            IF @prevCurrent <> @prevStart
                BEGIN
                    DECLARE @valueStartOut AS SQL_VARIANT;
                    DECLARE @valueFinalOut AS SQL_VARIANT;
                    DECLARE @valueCount AS NUMERIC (38);
                    SET @valueCount = ABS(CEILING((CAST (@prevCurrent AS FLOAT) - @prevIncrement - @nowStart) / @nowIncrement)) + 1;
                    IF @valueCount > 0
                       AND @valueCount <= 2147483647
                        BEGIN TRY
                            EXECUTE [sp_sequence_get_range] @sequenceName, @valueCount, @valueStartOut OUTPUT, @valueFinalOut OUTPUT;
                            PRINT N'Restored current value for sequence: ' + @sequenceName + N', ' + CONVERT(nvarchar(max), @valueFinalOut);
                        END TRY
                        BEGIN CATCH
                            PRINT N'Failed to restore current value for sequence: ' + @sequenceName;
                        END CATCH
                END
            FETCH sequenceStateCursor INTO @sequenceName, @prevCurrent, @prevStart, @prevIncrement, @nowStart, @nowIncrement;
        END
END TRY
BEGIN CATCH
    PRINT ERROR_MESSAGE();
END CATCH

IF CURSOR_STATUS(N'LOCAL', N'sequenceStateCursor') >= 0
    CLOSE sequenceStateCursor;

IF CURSOR_STATUS(N'LOCAL', N'sequenceStateCursor') = -1
    DEALLOCATE sequenceStateCursor;


GO
PRINT N'Update complete.';


GO
