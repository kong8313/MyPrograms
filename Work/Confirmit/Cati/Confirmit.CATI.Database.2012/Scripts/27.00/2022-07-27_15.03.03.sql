
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
	@DefaultTZID INT,
	@ITS INT
AS
SET NOCOUNT ON
    
	DECLARE @ProcessedCalls INT = 0

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
		DialTypeId [int] NOT NULL
	)

	DECLARE @CurFirstDOW INT = @@DATEFIRST
	SET DATEFIRST 7
	INSERT INTO #InterviewTimeZoneTable
	SELECT i.[ID], 
		   ISNULL(i.TimezoneID, 0), 
		   ISNULL(dbo.GetTZBias(ISNULL(@TimeToCall, GETUTCDATE()), CASE WHEN ISNULL(i.TimezoneID, 0) = 0 THEN @DefaultTZID ELSE i.TimeZoneID END), 0) Bias, 
		   CASE WHEN @ShiftTypeID = @ShiftTypeNone THEN @ShiftTypeID ELSE -ISNULL(i.TimezoneID, 0) END,
		   CASE WHEN @SurveySchedulingMode = 1 THEN ISNULL(@ITS, i.TransientState)  ELSE 0 END,
		   s.FcdAction,
		   i.DialTypeId
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

	CREATE TABLE #UpdatedInterviewsTable
	(
		InterviewID [int] NOT NULL
	)

	DECLARE @disableClr BIT
	SELECT @disableClr = DisableClrForFcd FROM BvSurvey WHERE SID = @SurveySID

	IF (@Mode = @ActivateScheduledCalls OR @Mode = @ActivateAllCalls)
	BEGIN
		IF @EnableDisabledCalls <> 0
		BEGIN

			IF @disableClr = 1
			BEGIN
				SET @sqlQuery = N'
				UPDATE  BvSvySchedule
				SET TimeInShift = 
					(CASE WHEN @TimeToCall = @TimeToCallNow 
						THEN @TimeToCallNow
						ELSE DATEADD( minute, #InterviewTimeZoneTable.Bias, @TimeToCall ) 
					END),
					Priority = @Priority,
					CallState = CASE WHEN 
						( 
							(
							SELECT DISTINCT 1 FROM BvInterviewQuotaCell AS icell 
							INNER JOIN BvSurveyQuotaCell AS qcell 
							ON icell.SurveyID = qcell.SurveyID AND icell.QuotaID = qcell.QuotaID AND icell.CellID = qcell.CellID AND qcell.IsOpen = 0
							WHERE icell.SurveyId = @SurveySID AND icell.InterviewId = #InterviewTimeZoneTable.ID
							) IS NULL
							OR #InterviewTimeZoneTable.FcdAction = 1
						) 
						THEN 2 
						ELSE 1 
					END,
					ShiftTypeID = #InterviewTimeZoneTable.ShiftTypeID,
					ExplicitSID = @PersonSID,
					ExplicitType = @ExplicitType,
					OldPriority = 0,
					ConditionValue = #InterviewTimeZoneTable.ConditionValue
				OUTPUT INSERTED.InterviewID INTO #UpdatedInterviewsTable
				FROM BvSvySchedule 
				INNER JOIN #InterviewTimeZoneTable 
					ON BvSvySchedule.[InterviewID] = #InterviewTimeZoneTable.[ID] AND BvSvySchedule.SurveySID = @SurveySID
				WHERE CallState > 0'
			END
			ELSE
			BEGIN
				EXEC BvClr_QuotaService_GetWhereForAllClosedSurveyCells @SurveySID, @alias, @whereCondition OUTPUT
				SET @sqlQuery = N'
				UPDATE  BvSvySchedule
				SET TimeInShift = ( CASE WHEN @TimeToCall = @TimeToCallNow THEN @TimeToCallNow
										 ELSE DATEADD( minute, #InterviewTimeZoneTable.Bias, @TimeToCall ) 
									END),
					Priority = @Priority,
					CallState = CASE WHEN ( NOT (' + @whereCondition + ') OR #InterviewTimeZoneTable.FcdAction = 1) THEN  2 ELSE 1 END,
					ShiftTypeID = #InterviewTimeZoneTable.ShiftTypeID,
					ExplicitSID = @PersonSID,
					ExplicitType = @ExplicitType,
					OldPriority = 0,
					ConditionValue = #InterviewTimeZoneTable.ConditionValue
				OUTPUT INSERTED.InterviewID INTO #UpdatedInterviewsTable
				FROM BvSvySchedule 
				INNER JOIN #InterviewTimeZoneTable ON BvSvySchedule.[InterviewID] = #InterviewTimeZoneTable.[ID] AND BvSvySchedule.SurveySID = @SurveySID
				LEFT JOIN BvReplicatedData_' + CAST(@SurveySID AS NVARCHAR(255)) + ' AS repl ON repl.respid = #InterviewTimeZoneTable.ID
				WHERE CallState > 0'
			END

			SET @sqlQueryParams = N'@TimeToCall DATETIME, @TimeToCallNow DATETIME, @Priority INT, @PersonSID INT, 
				@ExplicitType INT, @SurveySID INT';
   
			EXEC sp_executesql @sqlQuery, @sqlQueryParams, @TimeToCall, @TimeToCallNow, @Priority, @PersonSID, @ExplicitType,
				@SurveySID
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
			OUTPUT INSERTED.InterviewID INTO #UpdatedInterviewsTable
			FROM BvSvySchedule 
			INNER JOIN #InterviewTimeZoneTable ON BvSvySchedule.[InterviewID] = #InterviewTimeZoneTable.[ID] AND BvSvySchedule.SurveySID = @SurveySID
			WHERE CallState > 0
		END
	END
		  
	IF (@Mode = @ActivateSuspendedCalls OR @Mode = @ActivateAllCalls)
	BEGIN
		IF @disableClr = 1
		BEGIN
			EXEC Bv_ClusterQuotaService_GetCellIdQuery @SurveySID, @ClusteredQuotaName, @alias, @ClusteredCellIdQuery OUTPUT
    
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
						 DialTypeId )
				OUTPUT INSERTED.InterviewID INTO #UpdatedInterviewsTable
				SELECT DISTINCT
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
					#InterviewTimeZoneTable.DialTypeId
				FROM #InterviewTimeZoneTable
				WHERE 
				( 
					(
						SELECT DISTINCT 1 FROM BvInterviewQuotaCell AS icell 
						INNER JOIN BvSurveyQuotaCell AS qcell 
						ON icell.SurveyID = qcell.SurveyID AND icell.QuotaID = qcell.QuotaID AND icell.CellID = qcell.CellID AND qcell.IsOpen = 0
						WHERE icell.SurveyID = @SurveySID AND icell.InterviewId = #InterviewTimeZoneTable.ID 
					) IS NULL OR #InterviewTimeZoneTable.FcdAction = 1) AND
					NOT EXISTS ( SELECT [ID] 
								FROM BvSvySchedule
								WHERE BvSvySchedule.SurveySID = @SurveySID AND 
										BvSvySchedule.InterviewID = #InterviewTimeZoneTable.[ID] 
				)'
		END
		ELSE
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
					 DialTypeId )
		    OUTPUT INSERTED.InterviewID INTO #UpdatedInterviewsTable
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
				#InterviewTimeZoneTable.DialTypeId
			FROM #InterviewTimeZoneTable
			LEFT JOIN BvReplicatedData_' + CAST(@SurveySID AS NVARCHAR(255)) + ' AS repl ON repl.respid = #InterviewTimeZoneTable.ID 
			WHERE ( NOT (' + @whereCondition + ') OR #InterviewTimeZoneTable.FcdAction = 1) AND
				  NOT EXISTS ( SELECT [ID] 
							   FROM BvSvySchedule
							   WHERE BvSvySchedule.SurveySID = @SurveySID AND 
									 BvSvySchedule.InterviewID = #InterviewTimeZoneTable.[ID] )'
		END
   
		SET @sqlQueryParams = N'@ShiftTypeID INT, @Priority INT, @PersonSID INT, @ExplicitType INT, '+
			'@SurveySID INT, @TimeToCall DATETIME, @IsRandomCallDeliveryEnabled BIT, @TimeToCallNow DATETIME';
   
		EXEC sp_executesql @sqlQuery, @sqlQueryParams, @ShiftTypeID, @Priority, @PersonSID, @ExplicitType,
			@SurveySID, @TimeToCall, @IsRandomCallDeliveryEnabled, @TimeToCallNow
	END
    
	IF (@ITS IS NOT NULL)
	BEGIN
		IF (OBJECT_ID('tempdb..#InterviewIts') is null)
			CREATE TABLE #InterviewIts (Id INT, its SMALLINT)

		UPDATE BvInterview
		SET TransientState = @ITS
		   OUTPUT inserted.Id, inserted.TransientState
		   INTO #InterviewIts
		FROM BvInterview i INNER JOIN #UpdatedInterviewsTable ui on i.ID = ui.InterviewID and i.SurveySID = @SurveySID
		
		SET @ProcessedCalls = @@ROWCOUNT
	END

	DELETE BvTransferArrays WHERE BatchID = @BatchID
       
RETURN @ProcessedCalls
GO
PRINT N'Altering [dbo].[BvSpCall_Enable]...';


GO
ALTER PROCEDURE BvSpCall_Enable
	@SurveySID INT,
	@BatchID INT,
	@IsFcdMode BIT,
	@Enable BIT
AS
IF  @Enable = 1 
BEGIN
	
	DECLARE @Query NVARCHAR(MAX)

	DECLARE @disableClr BIT
	SELECT @disableClr = DisableClrForFcd FROM BvSurvey WHERE SID = @SurveySID

	IF @disableClr = 1
	BEGIN

		SET @Query = 'UPDATE BvSvySchedule SET CallState = 
		CASE WHEN (
				(
				SELECT DISTINCT 1 FROM BvInterviewQuotaCell AS icell 
				INNER JOIN BvSurveyQuotaCell AS qcell 
				ON icell.SurveyID = qcell.SurveyID AND icell.QuotaID = qcell.QuotaID AND icell.CellID = qcell.CellID AND qcell.IsOpen = 0
				WHERE icell.SurveyID = @SurveySID AND icell.InterviewId = ta.ItemID 
				) IS NULL
			)
			THEN 1
			ELSE 2 
		END
		FROM BvTransferArrays ta
		WHERE	BvSvySchedule.SurveySID = @SurveySID AND
		BvSvySchedule.InterviewID = ta.ItemID AND
		ta.BatchID = @BatchID AND
		BvSvySchedule.CallState IN (1,3)';
	END
	ELSE
	BEGIN

		DECLARE @whereCondition NVARCHAR(MAX) 
		EXEC BvClr_QuotaService_GetWhereForAllClosedSurveyCells @SurveySID, 'repl', @whereCondition OUTPUT


		SET @Query = 'UPDATE BvSvySchedule SET CallState = CASE WHEN (' + @whereCondition + ') THEN 1 ELSE 2 END
		FROM BvTransferArrays ta
		LEFT JOIN BvReplicatedData_' + CAST(@SurveySID AS NVARCHAR(255)) + ' as repl
		ON repl.respid = ta.ItemID 
		WHERE	BvSvySchedule.SurveySID = @SurveySID AND
				BvSvySchedule.InterviewID = ta.ItemID AND
				ta.BatchID = @BatchID AND
				BvSvySchedule.CallState IN (1,3)';
	END

	EXEC sp_executesql @Query, N'@SurveySID INT, @BatchID INT', @SurveySID, @BatchID
END
ELSE
BEGIN
	IF @IsFcdMode = 1 
	BEGIN
		UPDATE BvSvySchedule SET CallState = 1/*Disabled by FCD*/
		FROM BvTransferArrays ta
		WHERE	BvSvySchedule.SurveySID = @SurveySID AND
				BvSvySchedule.InterviewID = ta.ItemID AND
				ta.BatchID = @BatchID AND
				BvSvySchedule.CallState IN ( -2/*In dialer*/, 2/*Normal*/)
	END
	ELSE
	BEGIN
		UPDATE BvSvySchedule SET CallState = 3/*Disabled by User*/
		FROM BvTransferArrays ta
		WHERE	BvSvySchedule.SurveySID = @SurveySID AND
				BvSvySchedule.InterviewID = ta.ItemID AND
				ta.BatchID = @BatchID AND
				BvSvySchedule.CallState IN ( -2/*In dialer*/, 1/*Disabled by FCD*/, 2/*Normal*/)
	END
END
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

    DECLARE @disableClr BIT
	SELECT @disableClr = DisableClrForFcd FROM BvSurvey WHERE SID = @SurveySID

    IF @disableClr = 1
	BEGIN
	
	    EXEC Bv_ClusterQuotaService_GetCellIdQuery @SurveySID, @ClusteredQuotaName, @alias, @ClusteredCellIdQuery OUTPUT
	
	    SET @sqlQuery = 
          N'SET @refID = 0
            MERGE BvSvySchedule as target
            USING( SELECT ci.*, CASE WHEN 
            
            (
				SELECT DISTINCT 1 FROM BvInterviewQuotaCell AS icell 
				INNER JOIN BvSurveyQuotaCell AS qcell 
				ON icell.SurveyID = qcell.SurveyID AND icell.QuotaID = qcell.QuotaID AND icell.CellID = qcell.CellID AND qcell.IsOpen = 0
				WHERE icell.SurveyID = @SurveySID AND icell.InterviewId = @InterviewId
			) IS NULL
            
            THEN 1 ELSE 0 END, s.FcdAction, ( ' + @ClusteredCellIdQuery + ') as CellId
					     FROM ( SELECT @SurveySID as SurveySID, @InterviewId as InterviewId, @ApptID as ApptId ) ci
					     LEFT JOIN BvState s 
                            ON @transientState = s.StateId AND s.StateGroupId = @StateGroupId
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
    END
	ELSE
	BEGIN

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

    END

    
           
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
PRINT N'Update complete.';


GO
