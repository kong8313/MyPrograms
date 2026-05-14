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

ALTER TABLE [dbo].[BvSvySchedule]
    ADD [Type] TINYINT CONSTRAINT [DF_BvSvySchedule_Type] DEFAULT (0) NOT NULL;

GO

CREATE TABLE [dbo].[BvInboundTelephoneNumber] (
    [TelephoneNumber] NVARCHAR (256) NOT NULL,
    [DialerId]        INT            NOT NULL,
    [SurveyId]        INT            NOT NULL,
    CONSTRAINT [PK_BvInboundTelephoneNumber] PRIMARY KEY CLUSTERED ([TelephoneNumber] ASC)
);

GO

CREATE NONCLUSTERED INDEX [IX_BvInboundTelephoneNumber_SurveyId]
    ON [dbo].[BvInboundTelephoneNumber]([SurveyId] ASC);

GO

EXECUTE sp_refreshsqlmodule N'[dbo].[GetPriorityCallByExplicitSidAndShiftTypeIdClustered]';


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


EXECUTE sp_refreshsqlmodule N'[dbo].[GetPriorityCallByExplicitSidAndShiftTypeId]';
EXECUTE sp_refreshsqlmodule N'[dbo].[GetTopCallsForShiftTypeGroupCell]';
EXECUTE sp_refreshsqlmodule N'[dbo].[GetCallsForPredictiveMode]';
EXECUTE sp_refreshsqlmodule N'[dbo].[GetCallsForGroupForPredictiveSurvey]';
EXECUTE sp_refreshsqlmodule N'[dbo].[GetCallsPerGroup]';
EXECUTE sp_refreshsqlmodule N'[dbo].[GetHighPriorityCalls]';

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
		BvSvySchedule.Type
	FROM BvSvySchedule 
	LEFT JOIN BvShiftZones ON BvSvySchedule.ShiftTypeID = BvShiftZones.[ID]
	WHERE BvSvySchedule.SurveySID = @SurveyID AND 
		 BvSvySchedule.InterviewID = @InterviewID AND
		 ( ISNULL( @OldCallState, BvSvySchedule.CallState ) > 0 OR ( @GetLiveCall <> 0 AND ISNULL( @OldCallState, BvSvySchedule.CallState ) < 0 AND ISNULL( @OldCallState, BvSvySchedule.CallState ) > -3) )
RETURN @IsLockObtained

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
   [Type]
 FROM [dbo].[BvSvySchedule]
 WHERE [ID] = @CallID

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
		@Type				TINYINT
AS
SET NOCOUNT ON
DECLARE @Rows INTEGER
DECLARE @ExplicitSID INTEGER
DECLARE @ExplicitType INTEGER
DECLARE @CallTZ INT

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
    SELECT @CallTZ = TimezoneID,
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
				  Type			    = @Type
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
					 Type )
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
					 @Type);
         
        IF( (@@ROWCOUNT > 0) AND (@refoldApptID IS NULL))
           SET @refID = @@IDENTITY'
           
SET @sqlQueryParams = N'@SurveySID INT, @IsRandomCallDeliveryEnabled BIT, @CallState INT, @ApptID INT, @Priority INT, ' +
       '@TimeInShift1 DATETIME, @ExpirationTime DATETIME, @ShiftTypeID INT, @ExplicitSID INT, ' +
       '@ExplicitType INT, @RuleNumber uniqueidentifier, @InterviewID INT, @ConditionValue INT, ' +
	   '@FcdBehaviorAlgorithm INT, @StateGroupId INT, @transientState INT, @DialTypeId TINYINT, ' +
       '@Type TINYINT, @refID INT OUTPUT, @refoldApptID INT OUTPUT'
       
    EXEC sp_executesql @sqlQuery, @sqlQueryParams, @SurveySID, @IsRandomCallDeliveryEnabled, @CallState, @ApptID, @Priority,
       @TimeInShift1, @ExpirationTime, @ShiftTypeID, @ExplicitSID, @ExplicitType, @RuleNumber,
       @InterviewID, @ConditionValue, @FcdBehaviorAlgorithm, @StateGroupId, @transientState, @DialTypeId, @Type, @refID = @ID OUTPUT, @refoldApptID = @oldApptID OUTPUT

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

EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignmentResource_ListUnused]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyCleanup_IsClean]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_MoveToITS]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyState_Update]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_DeassignFromCallCenter]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Delete]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCluster_TryIncrenent]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSvyShedule_DeleteCallsByBlacklist]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTimezone_Delete]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson_ForManualMode]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpShiftType_Delete]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_Activate]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_Enable]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetDialerCallsBreakdown]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_Delete]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Clean]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Shutdown]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetAllAppointmentsForUser]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_ChangePriority]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlert_RecalculateAppointment]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_GetAssignedSurveyList]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpReleaseCall]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTimezone_DeleteUnused]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_ChangeShiftType]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_Insert2]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpRemoveExpiredCalls]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSetCallDeliveryMode]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSvySch_Delete]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_Delete]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonAndGroups_List]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_List]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignmentResource_TryDelete]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonGroup_Delete]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpDialer_Reset]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_DeleteFiltered]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSchedule_Delete]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCalls_Delete_Batch]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterview_UpdateRespondentFields]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSetCallState]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_Insert]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyModifyStateGroup]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson_ForAssignmentModeClustered]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson_ForCallGroup]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson_ForSurvey]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnly]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson_ForAssignmentMode]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyByPersonGroup]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetActiveCallsForSurvey]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallCenter_Delete]';
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Update]'
GO
PRINT N'Update complete.';


GO
