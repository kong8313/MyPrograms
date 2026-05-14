CREATE PROCEDURE [dbo].[BvSpCall_Activate]
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

	IF (@Mode = @ActivateScheduledCalls OR @Mode = @ActivateAllCalls)
	BEGIN
		IF @EnableDisabledCalls <> 0
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
                     ELSE CHECKSUM(NEWID()) % 2147483647
                END),
                0 /*old priority*/,
                #InterviewTimeZoneTable.ConditionValue,
                (' + @ClusteredCellIdQuery + ') /*cellId*/,
                #InterviewTimeZoneTable.DialTypeId
            FROM #InterviewTimeZoneTable
            LEFT JOIN 
                BvReplicatedData_' + CAST(@SurveySID AS NVARCHAR(255)) + ' AS repl ON repl.respid = #InterviewTimeZoneTable.ID 
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