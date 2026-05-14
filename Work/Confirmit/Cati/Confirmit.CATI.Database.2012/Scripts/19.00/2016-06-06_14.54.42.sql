GO
PRINT N'Removing schema binding from [dbo].[GetCallByCondition]...';


GO
ALTER FUNCTION [dbo].[GetCallByCondition]
(@ShiftTypeId INT, @SurveySid INT, @ExplicitSID INT, @ConditionValue INT, @Now DATETIME)
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
     WHERE    ShiftTypeId = @ShiftTypeId
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
(@ShifttypeId INT, @SurveySid INT, @ExplicitSID INT, @Now DATETIME)
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
     WHERE    ShifttypeId = @ShifttypeId
              AND CallState = 2
              AND SurveySid = @SurveySid
              AND BvSvySchedule.ExplicitSID = @ExplicitSID
              AND ConditionValue <> 0
              AND TimeInShift < @Now
     ORDER BY Priority DESC, TimeInShift, ExplicitType DESC, CallOrder)



GO
PRINT N'Altering [dbo].[BvDialers]...';


GO
ALTER TABLE [dbo].[BvDialers]
    ADD [SampleTypeId] TINYINT CONSTRAINT [DF_BvDialers_SampleTypeId] DEFAULT (0) NOT NULL;


GO
PRINT N'Altering [dbo].[BvInterview]...';


GO
ALTER TABLE [dbo].[BvInterview]
    ADD [SampleTypeId] TINYINT CONSTRAINT [DF_BvInterview_SampleTypeId] DEFAULT (0) NOT NULL;

ALTER TABLE [dbo].[BvInterview] DROP CONSTRAINT DF_BvInterview_SampleTypeId;

GO
PRINT N'Altering [dbo].[BvPerson]...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD [SampleTypeId] TINYINT CONSTRAINT [DF_BvPerson_SampleTypeId] DEFAULT (0) NOT NULL;


GO
PRINT N'Altering [dbo].[BvSvySchedule]...';


GO
ALTER TABLE [dbo].[BvSvySchedule]
    ADD [SampleTypeId] TINYINT CONSTRAINT [DF_BvSvySchedule_SampleTypeId] DEFAULT (0) NOT NULL;

ALTER TABLE [dbo].[BvSvySchedule] DROP CONSTRAINT [DF_BvSvySchedule_SampleTypeId]


GO
PRINT N'Altering [dbo].[BvTasks]...';


GO
ALTER TABLE [dbo].[BvTasks]
    ADD [SampleTypeId] TINYINT CONSTRAINT [DF_BvTasks_SampleTypeId] DEFAULT (0) NOT NULL;

ALTER TABLE [dbo].[BvTasks] DROP CONSTRAINT [DF_BvTasks_SampleTypeId]

GO
PRINT N'Creating [dbo].[BvSampleType]...';


GO
CREATE TABLE [dbo].[BvSampleType] (
    [Id]   INT            NOT NULL,
    [Name] NVARCHAR (MAX) NOT NULL
);

GO

INSERT INTO BvSampleType(ID, Name) VALUES( 0, 'Landline' )
INSERT INTO BvSampleType(ID, Name) VALUES( 1, 'Cellphone' )

GO
PRINT N'Refreshing [dbo].[GetCountsForSample]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[GetCountsForSample]';


GO
PRINT N'Refreshing [dbo].[BvFnPerson_Get]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvFnPerson_Get]';


GO
PRINT N'Refreshing [dbo].[BvFnPerson_GetByTransferBatch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvFnPerson_GetByTransferBatch]';


GO
PRINT N'Refreshing [dbo].[BvFnPersonAndGroup_Get]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvFnPersonAndGroup_Get]';


GO
PRINT N'Adding schema binding to [dbo].[GetCallByCondition]...';


GO
ALTER FUNCTION [dbo].[GetCallByCondition]
(   @ShiftTypeId INT,
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
		    WHERE ShiftTypeId = @ShiftTypeId AND
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
          WHERE ShifttypeId = @ShifttypeId AND
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
PRINT N'Refreshing [dbo].[RestView_Survey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_Survey]';


GO
PRINT N'Refreshing [dbo].[BvViewPersonAndGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvViewPersonAndGroup]';


GO
PRINT N'Refreshing [dbo].[RestView_CallHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_CallHistory]';


GO
PRINT N'Refreshing [dbo].[RestView_Interviewer]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_Interviewer]';


GO
PRINT N'Altering [dbo].[BvSpCall_Activate]...';


GO
ALTER PROCEDURE [dbo].[BvSpCall_Activate]
	@SurveySID INT,
	@Mode INT,
	@BatchID INT, 
	@Priority INT,
	@PersonSID INT, 
	/* 
	 * @ShiftTypeID > 0 means specific  shift type id( BvShiftType.ID ) and should be resolved to ShiftZoneId in bvSvySchedule.ShiftTypeID
	 * @ShiftTypeID = Int32.MinValue(-2147483648) meens [None] and should ne resolved to Int32.MinValue in BvSvySchedule.ShiftTypeID
	 * @ShiftTypeID =-1 @ShiftTypeID means [Any valid] and should be resolved to -Timezone in BvSvySchedule.ShiftTypeID
	 */
	@ShiftTypeID INT,
	@TimeToCall DATETIME,
	@EnableDisabledCalls BIT,
	@DefaultTZID INT
AS
SET NOCOUNT ON

    DECLARE @ActivateScheduledCalls INT = 8 -- activate prepared scheduled calls ( FilterGenerateMode: SCHEDULEDINTERVIEWID = 8 )
    DECLARE @ActivateSuspendedCalls INT = 9 -- activate prepared suspended calls ( FilterGenerateMode: SUSPENDEDINTERVIEWID = 9 )
    DECLARE @ActivateAllCalls INT = 1 -- activate prepared suspended calls ( FilterGenerateMode: INTERVIEWID = 1 )
	DECLARE @ShiftTypeNone INT = -2147483648; --None constant for bvSvySchedule.ShiftTypeID
	DECLARE @TimeToCallNow DATETIME = '1899-12-30T00:00:00.000'
	DECLARE @TimeToCallMinute DATETIME = DATEADD( minute, 1, @TimeToCall )
	DECLARE @ExplicitType INT = 2;
	DECLARE @sqlQuery NVARCHAR(MAX)
	DECLARE @sqlQueryParams NVARCHAR(MAX)
	DECLARE @ClusteredCellIdQuery NVARCHAR(MAX)
	DECLARE @whereCondition NVARCHAR(MAX)
	DECLARE @alias NVARCHAR(25) = 'repl'

	DECLARE @IsRandomCallDeliveryEnabled BIT
	DECLARE @ClusteredQuotaName NVARCHAR(MAX)
	DECLARE @OwnerID INT
	DECLARE @SurveySchedulingMode INT
	DECLARE @StateGroupId INT

	SELECT @IsRandomCallDeliveryEnabled = IsRandomCallDeliveryEnabled,
           @SurveySchedulingMode = SurveySchedulingMode,
           @ClusteredQuotaName = ClusteredQuotaName,
	       @OwnerID = [ScheduleID],
		   @StateGroupId = StateGroupId
	FROM BvSurvey
	WHERE SID = @SurveySID

	IF (@PersonSID = 0 )
	BEGIN
	    SET @ExplicitType = 1;

		SET @PersonSID = @SurveySID
	END

	CREATE TABLE #InterviewTimeZoneTable
	(
		[ID] [int] NOT NULL,
		TimeZoneID [int] NOT NULL,
		Bias [int] NULL,
		ShiftTypeID [int] NOT NULL,
		ConditionValue [int] NOT NULL,
		FcdAction [int] NOT NULL,
		SampleTypeId [int] NOT NULL
	)

	DECLARE @CurFirstDOW INT = @@DATEFIRST
	SET DATEFIRST 7
	INSERT INTO #InterviewTimeZoneTable
	SELECT i.[ID], 
		   ISNULL(i.TimezoneID, 0), 
		   ISNULL(dbo.GetTZBias(ISNULL(@TimeToCall, GETUTCDATE()), CASE WHEN ISNULL(i.TimezoneID, 0) = 0 THEN @DefaultTZID ELSE i.TimeZoneID END), 0) Bias, 
		   CASE WHEN @ShiftTypeID = @ShiftTypeNone THEN @ShiftTypeID ELSE -ISNULL(i.TimezoneID, 0) END,
		   CASE WHEN @SurveySchedulingMode = 1 THEN i.TransientState ELSE 0 END,
		   s.FcdAction,
		   i.SampleTypeId
	FROM BvInterview i
	INNER JOIN BvTransferArrays ta ON ta.ItemId = i.[ID] AND
									  ta.BatchID = @BatchID
	INNER JOIN BvState s ON s.StateID = i.TransientState AND s.StateGroupID = @StateGroupId
	WHERE i.SurveySID = @SurveySID AND s.DA = 0 
	SET DATEFIRST @CurFirstDOW


	DECLARE @DistinctTimeZonesTable TABLE
	(
		TimeZoneID [int] NOT NULL
	)
  
	INSERT INTO @DistinctTimeZonesTable 
	SELECT DISTINCT TimeZoneID 
	FROM #InterviewTimeZoneTable


	IF (	@ShiftTypeID <> @ShiftTypeNone ) --[any valid] or specific shift we should chek too
	BEGIN 
		DECLARE @ErrorTimezoneList NVARCHAR(MAX);

		IF ISNULL( @TimeToCall,  @TimeToCallNow ) <>  @TimeToCallNow/*equal zero for DATE type(meens Set to NOW)*/
		BEGIN
			DECLARE @activeshift TABLE
			(
				ShiftID INT NOT NULL, 
				OwnerID INT NOT NULL,
				[ShiftTypeID] INT NOT NULL,
				[TimezoneID] INT
			)
	        
			INSERT INTO @activeshift EXEC BvSpGetActiveShiftsInRelativeTime @TimeToCall, @TimeToCallMinute, @DefaultTZID

			;WITH ActiveTz( TimeZoneID ) AS
			(
				SELECT DISTINCT TimeZoneID
				FROM @activeshift
				WHERE OwnerID = @OwnerID AND 
				      (ShiftTypeID = @ShiftTypeID OR @ShiftTypeID = -1)
			)
			SELECT @ErrorTimezoneList = CASE WHEN @ErrorTimezoneList IS NULL THEN ''
											 ELSE @ErrorTimezoneList + ',' 
										END + CAST( ct.TimeZoneID AS NVARCHAR(64) )
			FROM @DistinctTimeZonesTable ct 
			LEFT JOIN ActiveTz at ON ct.TimezoneID = at.TimezoneID
			WHERE at.TimezoneID IS NULL
		END
		ELSE --@TimeToCall is NULL or @TimeToCallNow
		BEGIN
			SELECT @ErrorTimezoneList = CASE WHEN @ErrorTimezoneList IS NULL THEN ''
											 ELSE @ErrorTimezoneList + ',' 
										END + CAST( ct.TimeZoneID AS NVARCHAR(64) )
			FROM @DistinctTimeZonesTable ct 
			LEFT JOIN BvTzPeriodicalShifts s ON	( ct.TimezoneID = s.tz_id OR ( ct.TimezoneID = 0 AND @DefaultTZID = s.tz_id) ) AND 
												( s.type_id = @ShiftTypeID OR @ShiftTypeID = -1 ) AND
												s.start_dt <> s.finish_dt AND 
												s.owner_id = @OwnerID
			WHERE s.shift_id IS NULL
		END
		
		IF @ErrorTimezoneList IS NOT NULL
		BEGIN
			DELETE BvTransferArrays WHERE BatchID = @BatchID

			RAISERROR( 'Operation cannot be completed, Time specified is out of shifts of selected type in following Tz: %s.', 12, 1, @ErrorTimezoneList )
			RETURN -1
		END
	END

	IF @ShiftTypeID > 0 
	BEGIN
		UPDATE #InterviewTimeZoneTable
		SET ShiftTypeID = BvShiftZones.[ID]
		FROM BvShiftZones 
		WHERE BvShiftZones.ShiftTypeID = @ShiftTypeID AND 
			  BvShiftZones.TimeZoneID = #InterviewTimeZoneTable.TimeZoneID
	END

	IF (@Mode = @ActivateScheduledCalls OR @Mode = @ActivateAllCalls)
	BEGIN
		IF @EnableDisabledCalls <> 0
		BEGIN
			UPDATE  BvSvySchedule
			SET TimeInShift = ( CASE WHEN @TimeToCall = @TimeToCallNow THEN @TimeToCallNow
									 ELSE DATEADD( minute, #InterviewTimeZoneTable.Bias, @TimeToCall ) 
								END),
				Priority = @Priority,
				CallState = 2,
				ShiftTypeID = #InterviewTimeZoneTable.ShiftTypeID,
				ExplicitSID = @PersonSID,
				ExplicitType = @ExplicitType,
				OldPriority = 0,
				ConditionValue = #InterviewTimeZoneTable.ConditionValue
			FROM BvSvySchedule 
			INNER JOIN #InterviewTimeZoneTable ON BvSvySchedule.[InterviewID] = #InterviewTimeZoneTable.[ID] AND BvSvySchedule.SurveySID = @SurveySID
			WHERE CallState > 0
		END
		ELSE
		BEGIN
			UPDATE  BvSvySchedule
			SET TimeInShift = ( CASE WHEN @TimeToCall = @TimeToCallNow THEN @TimeToCallNow
									 ELSE DATEADD( minute, #InterviewTimeZoneTable.Bias, @TimeToCall ) 
								END),
				Priority = @Priority,
				ShiftTypeID = #InterviewTimeZoneTable.ShiftTypeID,
				ExplicitSID = @PersonSID,
				ExplicitType = @ExplicitType,
				OldPriority = 0,
				ConditionValue = #InterviewTimeZoneTable.ConditionValue
			FROM BvSvySchedule 
			INNER JOIN #InterviewTimeZoneTable ON BvSvySchedule.[InterviewID] = #InterviewTimeZoneTable.[ID] AND BvSvySchedule.SurveySID = @SurveySID
			WHERE CallState > 0
		END
	END
		  
	IF (@Mode = @ActivateSuspendedCalls OR @Mode = @ActivateAllCalls)
	BEGIN
		EXEC BvClr_QuotaService_GetWhereForAllClosedSurveyCells @SurveySID, @alias, @whereCondition OUTPUT
		
		EXEC BvClr_ClusterQuotaService_GetCellIdQuery @SurveySID, @ClusteredQuotaName, @alias, @ClusteredCellIdQuery OUTPUT
     
		SET @sqlQuery = 
		N'INSERT INTO BvSvySchedule
				    (ApptID,
					 ShiftTypeID,
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
					 OldPriority,
					 ConditionValue,
					 CellId,
					 SampleTypeId )
			SELECT
				0,-- ApptID
				#InterviewTimeZoneTable.ShiftTypeID,-- ShiftTypeID
				#InterviewTimeZoneTable.[ID],
				@SurveySID,
				2 as CallStateCurrent,
				@Priority,
				(CASE WHEN @TimeToCall = @TimeToCallNow THEN @TimeToCallNow
					  ELSE DATEADD( minute, #InterviewTimeZoneTable.Bias, @TimeToCall )
				END),-- TimeInShift
				''9999-01-01 00:00:00.000'',-- ExpireTime
				@PersonSID,
				@ExplicitType,
				''00000000-0000-0000-0000-000000000000'',
				(CASE WHEN @IsRandomCallDeliveryEnabled = 0 THEN #InterviewTimeZoneTable.[ID]
					 ELSE dbo.GetRandomValue(#InterviewTimeZoneTable.[ID])
				END),
				0 /*old priority*/,
				#InterviewTimeZoneTable.ConditionValue,
				(' + @ClusteredCellIdQuery + ') /*cellId*/,
				#InterviewTimeZoneTable.SampleTypeId
			FROM #InterviewTimeZoneTable
			LEFT JOIN BvReplicatedData_' + CAST(@SurveySID AS NVARCHAR(255)) + ' AS repl ON repl.respid = #InterviewTimeZoneTable.ID 
			WHERE ( NOT (' + @whereCondition + ') OR #InterviewTimeZoneTable.FcdAction = 1) AND
				  NOT EXISTS ( SELECT [ID] 
							   FROM BvSvySchedule
							   WHERE BvSvySchedule.SurveySID = @SurveySID AND 
									 BvSvySchedule.InterviewID = #InterviewTimeZoneTable.[ID] )'
   
		SET @sqlQueryParams = N'@ShiftTypeID INT, @Priority INT, @PersonSID INT, @ExplicitType INT, '+
			'@SurveySID INT, @TimeToCall DATETIME, @IsRandomCallDeliveryEnabled BIT, @TimeToCallNow DATETIME';
   
		EXEC sp_executesql @sqlQuery, @sqlQueryParams, @ShiftTypeID, @Priority, @PersonSID, @ExplicitType,
			@SurveySID, @TimeToCall, @IsRandomCallDeliveryEnabled, @TimeToCallNow
	END
       
	DELETE BvTransferArrays WHERE BatchID = @BatchID
       
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpInterview_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpInterview_Insert]
	    @ID                         int,
        @SurveySID                  int,        
        @TimeZoneID                 int,
        @TransientState             int,
        @LastCallPersonSID          int,
        @Duration                   int,
        @TelephoneNumber            varchar( 255 ),
        @RespondentName             nvarchar( 255 ),
        @LastCallTime               datetime,
        @ExtensionNumber            varchar( 255 ),
        @LastChannelID              tinyint,
        @ConfirmitSid               varchar(64) = '',
        @DialingMode                tinyint,
		@IsSentToReview             bit,
		@SampleTypeId               tinyint
AS

 IF (@TimeZoneID > 0)
    IF NOT EXISTS (SELECT TOP (1) 1 FROM BvTimezone WHERE ID = @TimeZoneID)
       BEGIN
         RAISERROR( 'Unrecognized time zone assigned to respondent, ensure the time zone is available from the active time zone list', 16, 1 )
         RETURN (-1)  
       END 


IF @TimeZoneID = 0 
        SET @TimeZoneID = NULL

INSERT BvInterview( 
		ID,
        SurveySID,        
        TimezoneID,
        TransientState,
        LastCallPersonSID,
        Duration,
        TelephoneNumber,
        RespondentName,
        LastCallTime,
        ExtensionNumber,
        BatchID,
        LastChannelID,
        ConfirmitSid,
        DialingMode,
		IsSentToReview,
		SampleTypeId )
        VALUES(
			@ID,
            @SurveySID,            
            @TimeZoneID,
            @TransientState,
            @LastCallPersonSID,
            @Duration,
            @TelephoneNumber,
            @RespondentName,
            @LastCallTime,
            @ExtensionNumber,
            0,
            @LastChannelID,
            @ConfirmitSid,
            @DialingMode,
			@IsSentToReview,
			@SampleTypeId )
            
RETURN @ID
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
		@SampleTypeId       TINYINT
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

    SET @InterviewID = ABS( @InterviewID )

    -- Get call TZ
    SELECT @CallTZ = TimezoneID,
		   @currentTransientState = TransientState
    FROM BvInterview
    WHERE SurveySID = @SurveySID AND 
         [ID] = @InterviewID
         
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
			      @refID            = CASE WHEN FcdAction = 1 OR IsClosed = 0 OR @FcdBehaviorAlgorithm = 1 THEN ID ELSE 0 END,
				  ApptID            = @ApptID,
				  CallState         = CASE WHEN FcdAction = 1 OR IsClosed = 0 THEN @CallState ELSE @FcdBehaviorAlgorithm END,
				  Priority          = @Priority,
				  TimeInShift       = @TimeInShift1,
				  ExpireTime        = @ExpirationTime,
				  ShiftTypeID       = @ShiftTypeID,
				  ExplicitSID       = @ExplicitSID,
				  ExplicitType      = @ExplicitType,
				  RuleNumber        = @RuleNumber,
                  ConditionValue    = @ConditionValue,
				  OldPriority       = 0,
				  SampleTypeId      = @SampleTypeId
        WHEN NOT MATCHED AND ( FcdAction = 1 OR IsClosed = 0 OR @FcdBehaviorAlgorithm = 1)
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
					 SampleTypeId )
              VALUES(@ShiftTypeID,
                     @ApptID,
                     @InterviewID,
                     @SurveySID,
                     CASE WHEN FcdAction = 1 OR IsClosed = 0 THEN @CallState ELSE 1/*disabled*/ END,
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
					 @SampleTypeId);
         
        IF( (@@ROWCOUNT > 0) AND (@refoldApptID IS NULL))
           SET @refID = @@IDENTITY'
           
SET @sqlQueryParams = N'@SurveySID INT, @IsRandomCallDeliveryEnabled BIT, @CallState INT, @ApptID INT, @Priority INT, ' +
       '@TimeInShift1 DATETIME, @ExpirationTime DATETIME, @ShiftTypeID INT, @ExplicitSID INT, ' +
       '@ExplicitType INT, @RuleNumber uniqueidentifier, @InterviewID INT, @ConditionValue INT, ' +
	   '@FcdBehaviorAlgorithm INT, @StateGroupId INT, @transientState INT, @SampleTypeId TINYINT, ' +
       '@refID INT OUTPUT, @refoldApptID INT OUTPUT'
       
    EXEC sp_executesql @sqlQuery, @sqlQueryParams, @SurveySID, @IsRandomCallDeliveryEnabled, @CallState, @ApptID, @Priority,
       @TimeInShift1, @ExpirationTime, @ShiftTypeID, @ExplicitSID, @ExplicitType, @RuleNumber,
       @InterviewID, @ConditionValue, @FcdBehaviorAlgorithm, @StateGroupId, @transientState, @SampleTypeId, @refID = @ID OUTPUT, @refoldApptID = @oldApptID OUTPUT

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
		BvSvySchedule.SampleTypeId
	FROM BvSvySchedule 
	LEFT JOIN BvShiftZones ON BvSvySchedule.ShiftTypeID = BvShiftZones.[ID]
	WHERE BvSvySchedule.SurveySID = @SurveyID AND 
		 BvSvySchedule.InterviewID = @InterviewID AND
		 ( ISNULL( @OldCallState, BvSvySchedule.CallState ) > 0 OR ( @GetLiveCall <> 0 AND ISNULL( @OldCallState, BvSvySchedule.CallState ) < 0 AND ISNULL( @OldCallState, BvSvySchedule.CallState ) > -3) )
			 
RETURN @IsLockObtained
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
   [SampleTypeId]
 FROM [dbo].[BvSvySchedule]
 WHERE [ID] = @CallID
GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_ChangeShiftType]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_ChangeShiftType]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_MoveToITS]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_MoveToITS]';


GO
PRINT N'Refreshing [dbo].[BvSpCallHistory_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistory_List]';


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
PRINT N'Refreshing [dbo].[BvSpInterview_CfData_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterview_CfData_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpInterview_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterview_Update]';


GO
PRINT N'Refreshing [dbo].[BvSpInterview_UpdateRespondentFields]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterview_UpdateRespondentFields]';


GO
PRINT N'Refreshing [dbo].[BvSpInterviews_UpdateIsSentToReview_Batch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviews_UpdateIsSentToReview_Batch]';


GO
PRINT N'Refreshing [dbo].[BvSpInterviews_UpdateState_Batch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviews_UpdateState_Batch]';


GO
PRINT N'Refreshing [dbo].[BvSpInterviewsAndAppointments_Delete_Batch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviewsAndAppointments_Delete_Batch]';


GO
PRINT N'Refreshing [dbo].[BvSpNumberOfAttemptsReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpNumberOfAttemptsReport]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyModifyStateGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyModifyStateGroup]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyState_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyState_Update]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_Update_2]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_Update_2]';


GO
PRINT N'Refreshing [dbo].[BvSpTimezone_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTimezone_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpTimezone_DeleteUnused]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTimezone_DeleteUnused]';


GO
PRINT N'Refreshing [dbo].[BvSpSampleUtilisationReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSampleUtilisationReport]';


GO
PRINT N'Refreshing [dbo].[BvSpAggregateInterviewerPerformance]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAggregateInterviewerPerformance]';


GO
PRINT N'Refreshing [dbo].[BvSpAlertsHistoryAggregatedReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlertsHistoryAggregatedReport]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignment_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpCallHistoryData]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistoryData]';


GO
PRINT N'Refreshing [dbo].[BvSpGetActiveCallsForSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetActiveCallsForSurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpGetInterviewerBreaks]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviewerBreaks]';


GO
PRINT N'Refreshing [dbo].[BvSpGetMessages]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetMessages]';


GO
PRINT N'Refreshing [dbo].[BvSpGetOpenedSurveys]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetOpenedSurveys]';


GO
PRINT N'Refreshing [dbo].[BvSpGetSurveyInterviews]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSurveyInterviews]';


GO
PRINT N'Refreshing [dbo].[BvSpGetUserGroups]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetUserGroups]';


GO
PRINT N'Refreshing [dbo].[BvSpHistory_CfData_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpHistory_CfData_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpInterviewerProductivityReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviewerProductivityReport]';


GO
PRINT N'Refreshing [dbo].[BvSpLogin_SpinUp]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLogin_SpinUp]';


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
PRINT N'Refreshing [dbo].[BvSpPerson_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_ListByParent]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_ListByParent]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_SetAutomaticSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_SetAutomaticSurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_SpinUp]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_SpinUp]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_Update]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonCheckForNewMessage]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonCheckForNewMessage]';


GO
PRINT N'Refreshing [dbo].[BvSpSendMessageToGroups]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSendMessageToGroups]';


GO
PRINT N'Refreshing [dbo].[BvSpSendMessageToInterviewers]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSendMessageToInterviewers]';


GO
PRINT N'Refreshing [dbo].[BvSpSendMessageToSurveys]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSendMessageToSurveys]';


GO
PRINT N'Refreshing [dbo].[BvSpStartInterviewerBreak]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpStartInterviewerBreak]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_DeassignFromCallCenter]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_DeassignFromCallCenter]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_GetCountOfLoggedPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_GetCountOfLoggedPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyOverviewReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyOverviewReport]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyProductivityReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyProductivityReport]';


GO
PRINT N'Refreshing [dbo].[BvSpGetAllPersonsAndGroups]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetAllPersonsAndGroups]';


GO
PRINT N'Refreshing [dbo].[BvSpGetInterviewerPerformanceList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviewerPerformanceList]';


GO
PRINT N'Refreshing [dbo].[BvSpGetListSurveyTasks]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetListSurveyTasks]';


GO
PRINT N'Refreshing [dbo].[BvSpGetPersonGroupsLevel]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetPersonGroupsLevel]';


GO
PRINT N'Refreshing [dbo].[BvSpGetPersonsLevel]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetPersonsLevel]';


GO
PRINT N'Refreshing [dbo].[BvSpGetSystemWideInfo]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSystemWideInfo]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonAndGroups_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonAndGroups_List]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_GetAssignedPersonList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_GetAssignedPersonList]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignment_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_List]';


GO
PRINT N'Refreshing [dbo].[BvSpAlert_RecalculateAppointment]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlert_RecalculateAppointment]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignmentResource_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignmentResource_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpGetExtendedCallHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetExtendedCallHistory]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_GetAssignedSurveyList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_GetAssignedSurveyList]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonGroup_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonGroup_Update]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonGroup_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonGroup_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignment_Insert2]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_Insert2]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignmentResource_ListUnused]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignmentResource_ListUnused]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignmentResource_TryDelete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignmentResource_TryDelete]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_ChangePriority]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_ChangePriority]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_Enable]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_Enable]';


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
PRINT N'Refreshing [dbo].[BvSpPersonGroup_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonGroup_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpReleaseCall]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpReleaseCall]';


GO
PRINT N'Refreshing [dbo].[BvSpRemoveExpiredCalls]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpRemoveExpiredCalls]';


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
PRINT N'Refreshing [dbo].[BvSpShiftType_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpShiftType_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Clean]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Clean]';


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
PRINT N'Refreshing [dbo].[BvSpSvySch_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSvySch_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson_ForCallGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson_ForCallGroup]';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson_ForSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson_ForSurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson_ForAssignmentMode]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson_ForAssignmentMode]';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson_ForAssignmentModeClustered]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson_ForAssignmentModeClustered]';


GO
PRINT N'Refreshing [dbo].[BvSpAlert_RecalculateAll]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlert_RecalculateAll]';


GO
PRINT N'Refreshing [dbo].[BvSpFinishInterviewerBreak]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpFinishInterviewerBreak]';


GO
PRINT N'Refreshing [dbo].[BvSpGetLoggedInPersonsCount]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetLoggedInPersonsCount]';


GO
PRINT N'Refreshing [dbo].[BvSpInterviewTimings_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviewTimings_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpTask_UpdateActiveQuestion]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTask_UpdateActiveQuestion]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_InsertAnswerSubmissionAlertIfNeeded]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_InsertAnswerSubmissionAlertIfNeeded]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_InsertUpdate_2]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_InsertUpdate_2]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_LockByPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_LockByPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_SetTelephonyProblemForLoggedIn]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_SetTelephonyProblemForLoggedIn]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UnLockByPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UnLockByPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateCallOutcome]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateCallOutcome]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateInterviewState]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateInterviewState]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateKeepAlive]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateKeepAlive]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateLoggedInToDialerState]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateLoggedInToDialerState]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateNewSurveySid]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateNewSurveySid]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateProblemState]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateProblemState]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateStartTime]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateStartTime]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateStatusLogout]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateStatusLogout]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateSurveySid]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateSurveySid]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Update]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignment_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpCallCenter_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallCenter_Delete]';


GO
PRINT N'Update complete.';


GO
