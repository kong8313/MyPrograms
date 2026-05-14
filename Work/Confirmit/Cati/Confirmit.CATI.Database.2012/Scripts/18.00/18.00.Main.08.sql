
GO
PRINT N'Dropping [dbo].[BvSvySchedule].[IX_BvSvyScheduleMain]...';


GO
DROP INDEX [IX_BvSvyScheduleMain]
    ON [dbo].[BvSvySchedule];


GO
PRINT N'Dropping [dbo].[BvSvySchedule].[IX_GetCallByCondition]...';


GO
DROP INDEX [IX_GetCallByCondition]
    ON [dbo].[BvSvySchedule];


GO
PRINT N'Dropping [dbo].[BvSvySchedule].[IX_GetCallBySurvey]...';


GO
DROP INDEX [IX_GetCallBySurvey]
    ON [dbo].[BvSvySchedule];


GO
PRINT N'Dropping DF_BvSvySchedule_IsInActiveShiftType...';


GO
ALTER TABLE [dbo].[BvSvySchedule] DROP CONSTRAINT [DF_BvSvySchedule_IsInActiveShiftType];


GO
PRINT N'Dropping [dbo].[BvCachedCallsInsert]...';


GO
DROP TABLE [dbo].[BvCachedCallsInsert];


GO
PRINT N'Dropping [dbo].[GetCallByCondition]...';


GO
DROP FUNCTION [dbo].[GetCallByCondition];


GO
PRINT N'Dropping [dbo].[GetCallBySurvey]...';


GO
DROP FUNCTION [dbo].[GetCallBySurvey];


GO
PRINT N'Dropping [dbo].[BvSpAddUniqueAssignment]...';


GO
DROP PROCEDURE [dbo].[BvSpAddUniqueAssignment];


GO
PRINT N'Dropping [dbo].[BvUniqueAssignments]...';


GO
DROP TABLE [dbo].[BvUniqueAssignments];


GO
PRINT N'Altering [dbo].[BvSvySchedule]...';


GO
ALTER TABLE [dbo].[BvSvySchedule] DROP COLUMN [IsInActiveShiftType];


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_BvSvyScheduleMain]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BvSvyScheduleMain]
    ON [dbo].[BvSvySchedule]([SurveySID] ASC, [ShiftTypeID] ASC, [ExplicitSID] ASC, [Priority] DESC, [TimeInShift] ASC, [ExplicitType] DESC, [CallOrder] ASC, [InterviewID] ASC)
    INCLUDE([ID], [CallState], [ApptID], [ConditionValue], [ExpireTime]);


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_GetCallByCondition]...';


GO
CREATE NONCLUSTERED INDEX [IX_GetCallByCondition]
    ON [dbo].[BvSvySchedule]([SurveySID] ASC, [ShiftTypeID] ASC, [ExplicitSID] ASC, [ConditionValue] ASC, [Priority] DESC, [TimeInShift] ASC, [ExplicitType] DESC, [CallOrder] ASC)
    INCLUDE([ID], [ExpireTime], [CallState]) WHERE ConditionValue <> 0;


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_GetCallBySurvey]...';


GO
CREATE NONCLUSTERED INDEX [IX_GetCallBySurvey]
    ON [dbo].[BvSvySchedule]([SurveySID] ASC, [ShiftTypeID] ASC, [ExplicitSID] ASC, [Priority] DESC, [TimeInShift] ASC, [ExplicitType] DESC, [CallOrder] ASC)
    INCLUDE([ID], [ExpireTime], [CallState]) WHERE ConditionValue <> 0;


GO
PRINT N'Creating [dbo].[BvInterview].[IX_BvInterview_DialingMode]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvInterview_DialingMode]
    ON [dbo].[BvInterview]([SurveySID] ASC, [DialingMode] ASC);


GO
PRINT N'Creating [dbo].[GetCallByCondition]...';


GO
CREATE FUNCTION [dbo].[GetCallByCondition]
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
PRINT N'Creating [dbo].[GetCallBySurvey]...';


GO
CREATE FUNCTION [dbo].[GetCallBySurvey]
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
PRINT N'Creating [dbo].[GetCallsForPredictiveMode]...';


GO
CREATE FUNCTION [dbo].[GetCallsForPredictiveMode]
(   @rowCount AS INT,
    @ShiftTypeId INT,
    @ExplicitSID AS INT,
    @SurveySid AS INT,
    @TimeToRun AS DATETIME) 
RETURNS TABLE
AS RETURN(
          SELECT TOP(@rowCount) [ID],
                                ExplicitSID,
								ExplicitType,
                                SurveySID,
                                InterviewID,
                                CallState,
								ApptId,
								TimeInShift,
								CallOrder,
								Priority
          FROM BvSvySchedule
          WHERE SurveySid = @SurveySid AND
                ExplicitSID = @ExplicitSID AND
                CallState = 2 AND
                TimeInShift <= @TimeToRun AND
                ShiftTypeId = @ShiftTypeId
          ORDER BY Priority DESC,
                   TimeInShift,
                   ExplicitType DESC,
                   CallOrder )
GO
PRINT N'Creating [dbo].[GetPriorityCallByExplicitSidAndShiftTypeId]...';


GO
CREATE FUNCTION [dbo].[GetPriorityCallByExplicitSidAndShiftTypeId]
(   @ExplicitSID INT,
    @ShiftTypeID INT,
	@SurveyID INT,
    @SuitableTimeForCalls DATETIME,
	@TopCount INT)
RETURNS TABLE 
AS RETURN
(
	    SELECT TOP(@TopCount) c.*
        FROM BvSvySchedule c with(readpast)
		WHERE CallState = 2 AND
			  c.ExplicitSID = @ExplicitSID and
			  c.ShiftTypeID = @ShiftTypeID and
			  TimeInShift <= @SuitableTimeForCalls AND
			  c.SurveySid = @SurveyID
		ORDER BY Priority DESC,
				 TimeInShift,
				 ExplicitType DESC,
				 CallOrder 
)
GO
PRINT N'Altering [dbo].[GetCallsForGroupForPredictiveSurvey]...';


GO
ALTER FUNCTION dbo.GetCallsForGroupForPredictiveSurvey
(
    @rowCount AS INT,
    @SurveySid AS INT,
    @ObjectSid AS INT,
	@SuitableTimeForCalls DATETIME
)
RETURNS TABLE
AS RETURN(
          SELECT TOP (@rowCount) c.*
          FROM BvActiveShiftTypeZone a
		  CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](@ObjectSid, a.Id, @SurveySID, @SuitableTimeForCalls, @rowCount) c
          ORDER BY priority DESC, TimeInShift, ExplicitType DESC, CallOrder )
GO
PRINT N'Altering [dbo].[BvSpAssignment_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpAssignment_Insert]
@SID INT, 
@SurveySID INT, 
@InterviewSID INT, 
@PersonSID INT, 
@RoleID INT, 
@FromCall INT=0,
@CallCenterID INT
AS
SET NOCOUNT ON
DECLARE @InsertedAssignmentsCount INTEGER = 0

IF @InterviewSID > 0 OR @FromCall > 0 
BEGIN

            UPDATE BvSvySchedule SET
                ExplicitSID = @PersonSID, 
                ExplicitType = 2, --Person type
                Priority = CASE WHEN OldPriority > 0 THEN OldPriority ELSE Priority END,
                OldPriority = 0
            WHERE SurveySID = @SurveySID AND 
                  InterviewID = @InterviewSID AND
                  CallState > 0
END
ELSE
BEGIN
        
    IF NOT EXISTS ( SELECT * FROM BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID)
        WHERE PersonOrGroupId = @PersonSID AND SurveyId = @SurveySID)
          INSERT INTO BvPersonOrGroupAssignmentOnSurvey( PersonOrGroupId, SurveyId, CallCenterID )
              VALUES( @PersonSID, @SurveySID, @CallCenterID )
              
    SET @InsertedAssignmentsCount = @@ROWCOUNT          
   
   IF EXISTS ( SELECT SID FROM BvPerson WHERE SID = @PersonSID )
   BEGIN
	   INSERT INTO BvPersonRel( PersonSID, ObjectSID, RoleID, Type )
	   VALUES(@PersonSID, @SurveySID, 2, 2)
   END
   ELSE
   BEGIN
       INSERT INTO BvPersonRel( PersonSID, ObjectSID, RoleID, Type )
       SELECT r.PersonSid, @SurveySID, 2, 2
       FROM BVPersonRel r
	   LEFT JOIN BvPerson p 
		ON r.PersonSID = p.SID
       WHERE @PersonSID = r.ObjectSID AND
             ObjectSID != r.PersonSid AND
			 ( p.CallCenterID = @CallCenterID OR p.SID IS NULL )
   END
END

RETURN @InsertedAssignmentsCount
GO
PRINT N'Altering [dbo].[BvSpAssignment_Insert2]...';


GO
ALTER PROCEDURE [dbo].[BvSpAssignment_Insert2]
@SurveySID INT, 
@PersonSID INT,
@BatchID INT
AS
SET NOCOUNT ON

    UPDATE BvSvySchedule 
    SET ExplicitSID = @PersonSID, 
        ExplicitType = 2, --Person type
        Priority = CASE WHEN OldPriority > 0 THEN OldPriority ELSE Priority END,
        OldPriority = 0
    FROM BvTransferArrays
    WHERE BvTransferArrays.BatchID = @BatchID AND
          BvSvySchedule.SurveySID = @SurveySID AND
          BvSvySchedule.InterviewID = BvTransferArrays.ItemID AND
          BvSvySchedule.CallState > 0

RETURN (0)
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
	DECLARE @whereCondition NVARCHAR(MAX)
	DECLARE @alias NVARCHAR(25) = 'repl'

	DECLARE @IsRandomCallDeliveryEnabled BIT
	DECLARE @OwnerID INT
	DECLARE @SurveySchedulingMode INT

	SELECT @IsRandomCallDeliveryEnabled = IsRandomCallDeliveryEnabled,
           @SurveySchedulingMode = SurveySchedulingMode,
	       @OwnerID = [ScheduleID]
	FROM BvSurvey
	WHERE SID = @SurveySID

	IF (@PersonSID = 0 )
	BEGIN
	    SET @ExplicitType = 1;

		SET @PersonSID = @SurveySID
	END

	DECLARE @DisableActivationITSTable TABLE( ITS INT )

	INSERT INTO @DisableActivationITSTable
	SELECT StateID
	FROM BvState 
	INNER JOIN BvSurvey ON BvState.StateGroupID = BvSurvey.StateGroupID AND
						   BvSurvey.SID = @SurveySID
	WHERE DA = 1


	CREATE TABLE #InterviewTimeZoneTable
	(
		[ID] [int] NOT NULL,
		TimeZoneID [int] NOT NULL,
		Bias [int] NULL,
		ShiftTypeID [int] NOT NULL,
		ConditionValue [int] NOT NULL
	)

	DECLARE @CurFirstDOW INT = @@DATEFIRST
	SET DATEFIRST 7
	INSERT INTO #InterviewTimeZoneTable
	SELECT BvInterview.[ID], 
		   ISNULL(BvInterview.TimezoneID, 0), 
		   ISNULL(dbo.GetTZBias(ISNULL(@TimeToCall, GETUTCDATE()), CASE WHEN ISNULL(TimezoneID, 0) = 0 THEN @DefaultTZID ELSE TimeZoneID END), 0) Bias, 
		   CASE WHEN @ShiftTypeID = @ShiftTypeNone THEN @ShiftTypeID ELSE -ISNULL(BvInterview.TimezoneID, 0) END,
		   CASE WHEN @SurveySchedulingMode = 1 THEN TransientState ELSE 0 END
	FROM BvInterview
	INNER JOIN BvTransferArrays ta ON ta.ItemId = BvInterview.[ID] AND
									  ta.BatchID = @BatchID
	WHERE BvInterview.SurveySID = @SurveySID AND
		  BvInterview.TransientState NOT IN (SELECT * FROM @DisableActivationITSTable)
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
     
		SET @sqlQuery = 
		N'INSERT INTO BvSvySchedule
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
				#InterviewTimeZoneTable.ConditionValue
			FROM #InterviewTimeZoneTable
			LEFT JOIN BvReplicatedData_' + CAST(@SurveySID AS NVARCHAR(255)) + ' AS repl ON repl.respid = #InterviewTimeZoneTable.ID AND
						(' + @whereCondition + ')
			WHERE repl.respid IS NULL AND
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
PRINT N'Altering [dbo].[BvSpCall_ChangeShiftType]...';


GO
ALTER PROCEDURE [dbo].[BvSpCall_ChangeShiftType]
@SurveySID   INTEGER,
	/* 
	 * @ShiftTypeID > 0 means specific shift type id( BvShiftType.ID ) and should be resolved to ShiftZoneId in bvSvySchedule.ShiftTypeID
	 * @ShiftTypeID = Int32.MinValue(-2147483648) meens [None] and should ne resolved to Int32.MinValue in BvSvySchedule.ShiftTypeID
	 * @ShiftTypeID =-1 @ShiftTypeID means [Any valid] and should be resolved to -Timezone in BvSvySchedule.ShiftTypeID
	 */
@ShiftTypeID INTEGER,
@BatchID     INTEGER,
@SiteTimeZoneID INTEGER
AS

	DECLARE @ShiftTypeNone INT = -2147483648; --None constant for bvSvySchedule.ShiftTypeID

    /*
     * Get next matching shifts by TZ
     */
    IF @ShiftTypeID <> @ShiftTypeNone
    BEGIN
        DECLARE @owner_id INT

        SELECT @owner_id = [ScheduleID] FROM BvSurvey
                WHERE [SID] = @SurveySID

        IF @owner_id IS NULL
        BEGIN
            RAISERROR( 'Scheduling script not found.', 16, 1 )
            RETURN(-1)
        END
     
        DECLARE @ErrorTimezoneList NVARCHAR(MAX)
        SET @ErrorTimezoneList = ''
        
        IF NOT EXISTS( SELECT 1 FROM BvShiftType WHERE OwnerSID = @owner_id AND ( ObjectID = @ShiftTypeID OR @ShiftTypeID = -1 ))
        BEGIN
			IF @ShiftTypeID = -1
				RAISERROR( 'Shceduling script doesn''t contain any shifttypes', 12, 1)
			ELSE
				RAISERROR( 'Scheduling script does not contain specific shift type with ID = %d', 12, 1, @ShiftTypeID)
			RETURN(-1)
        END
        
        ;WITH AvailableTz( tz_id ) AS
        (
			SELECT DISTINCT tz_id FROM BvTzPeriodicalShifts
				WHERE (type_id = @ShiftTypeID OR  @ShiftTypeID = -1) and start_dt <> finish_dt
        )
        SELECT @ErrorTimezoneList = CASE WHEN LEN(@ErrorTimezoneList) > 0 THEN @ErrorTimezoneList + ',' ELSE '' END + CAST( ISNULL(i.TimezoneID,0) AS NVARCHAR(MAX) ) FROM (
            SELECT DISTINCT TimezoneID 
            FROM  BvTransferArrays ta
            INNER JOIN BvInterview i ON i.[ID] = ta.ItemID AND 
                                        i.SurveySID = @SurveySID
            LEFT JOIN AvailableTz atz ON atz.tz_id = i.TimezoneID OR 
                                         ( i.TimezoneID IS NULL AND atz.tz_id = @SiteTimeZoneID )
            WHERE atz.tz_id IS NULL ) i

        IF LEN( @ErrorTimezoneList ) > 0
        BEGIN
            RAISERROR( 'Operation cannot be completed, the assigned scheduling script does not support the following timezone ID(s): "%s" for the selected shift type.To resolve this, in scheduling either add a default shift(s) or add the specific timezone shift(s) for this shift type.', 12, 1, @ErrorTimezoneList )
            RETURN -1
        END
    END

    -- [Any Valid] 
    IF @ShiftTypeID = -1 
    BEGIN
        UPDATE BvSvySchedule 
            SET BvSvySchedule.ShiftTypeID = -ISNULL(i.TimezoneID, 0 ),
                Priority = CASE WHEN OldPriority > 0 THEN OldPriority ELSE Priority END,
                OldPriority = 0
        FROM BvSvySchedule
        INNER JOIN BvTransferArrays ON BatchID = @BatchID
            AND ItemID = BvSvySchedule.[InterviewID] AND BvSvySchedule.SurveySID = @SurveySID
        INNER JOIN BvInterview i ON i.[ID] = BvSvySchedule.InterviewID
            AND i.SurveySID = @SurveySID
        WHERE BvSvySchedule.CallState > 0
    END
    ELSE 
    BEGIN
        IF @ShiftTypeID > 0 --Specific shift
            UPDATE BvSvySchedule 
                SET ShiftTypeID = BvShiftZones.[ID],
                    Priority = CASE WHEN OldPriority > 0 THEN OldPriority ELSE Priority END,
                    OldPriority = 0
            FROM BvSvySchedule
            INNER JOIN BvInterview ON BvInterview.SurveySID = @SurveySID
                AND BvSvySchedule.InterviewID = BvInterview.[ID]
            INNER JOIN BvShiftZones ON BvShiftZones.ShiftTypeID = @ShiftTypeID
                AND ISNULL(BvInterview.TimezoneID, 0 ) = BvShiftZones.TimeZoneID
            INNER JOIN BvTransferArrays ON BvTransferArrays.BatchID = @BatchID
                AND ItemID = BvSvySchedule.InterviewID AND BvSvySchedule.SurveySID = @SurveySID
            WHERE BvSvySchedule.CallState > 0
        ELSE--[None]
            UPDATE BvSvySchedule 
            SET ShiftTypeID = @ShiftTypeNone,
                Priority = CASE WHEN OldPriority > 0 THEN OldPriority ELSE Priority END,
                OldPriority = 0
            FROM BvSvySchedule
            INNER JOIN BvTransferArrays ON BvTransferArrays.BatchID = @BatchID
                AND ItemID = BvSvySchedule.[InterviewID] AND BvSvySchedule.SurveySID = @SurveySID
            WHERE BvSvySchedule.CallState > 0
    END
    
RETURN(0)
GO
PRINT N'Altering [dbo].[BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnly]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnly]
 @SurveySID INT,
 @Count  INT, --number of requested calls
 @SuitableTimeForCalls DATETIME
AS
--best if it should be established at the connection level
--it may influence on count of recompilations
SET NOCOUNT ON

	DECLARE @FixeNumberCallsPerPerson INT = 10

      --it should be at end of the SP. it uses for decrease recompilation
	DECLARE @CachedCalls TABLE (
	  [ExplicitSID] [int] NOT NULL,
	  [ID] [int] NOT NULL,
	  [InterviewID] [int] NOT NULL,
	  [TimeInShift] [datetime] NULL,
	  [Priority] [INT] NOT NULL,
	  [CallOrder] [INT] NOT NULL,
	  [ApptID] [int] not null)
        
	;WITH orderedUpdateTable AS
	(
		SELECT TOP(@Count) c.*
        FROM BvActiveShiftTypeZone a
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](@SurveySID, a.Id, @SurveySID, @SuitableTimeForCalls, @Count) c
		WHERE a.Surveyid = @SurveySID
		ORDER BY Priority DESC,
                 TimeInShift,
				 ExplicitType DESC,
				 CallOrder
	)
    UPDATE orderedUpdateTable
    SET CallState = -2 
	OUTPUT 0,
		   inserted.[ID],
		   inserted.[interviewID],
		   inserted.[TimeInShift],
		   inserted.[Priority],
		   inserted.[CallOrder],
		   inserted.[ApptID]
	INTO @CachedCalls

    SELECT c.[ID], 
           c.[ExplicitSid], 
           @SurveySID SurveySid, 
           i.DialingMode DiallingMode,
		   c.[InterviewID], 
		   i.[TelephoneNumber],
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   0 as [GroupID]
    FROM @CachedCalls c 
    INNER JOIN BvInterview i ON c.[Interviewid] = i.[ID] AND
                                i.[SurveySID] = @SurveySID
    ORDER BY Priority DESC,
             TimeInShift,
 			 CallOrder
 
RETURN (@@ROWCOUNT)
GO
PRINT N'Altering [dbo].[BvSpGetCachedCallsForPredictiveSurveyByPersonGroup]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyByPersonGroup]
 @SurveySID INT,
 @GroupID INT,	
 @Count  INT, --number of requested calls
 @SuitableTimeForCalls DATETIME
AS
--best if it should be established at the connection level
--it may influence on count of recompilations
SET NOCOUNT ON

	DECLARE @FixeNumberCallsPerPerson INT = 10

      --it should be at end of the SP. it uses for decrease recompilation
	DECLARE @CachedCalls TABLE (
	  [ExplicitSID] [int] NOT NULL,
	  [ID] [int] NOT NULL,
	  [InterviewID] [int] NOT NULL,
	  [TimeInShift] [datetime] NULL,
	  [Priority] [INT] NOT NULL,
	  [CallOrder] [INT] NOT NULL,
	  [ApptID] [int] not null)
        
	;WITH orderedUpdateTable AS
	(
		SELECT TOP(@Count) c.*
        FROM BvActiveShiftTypeZone a
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](@groupID, a.Id, @SurveySID, @SuitableTimeForCalls, @Count) c
		WHERE a.Surveyid = @SurveySID
		ORDER BY Priority DESC,
                 TimeInShift,
				 ExplicitType DESC,
				 CallOrder
	)
    UPDATE orderedUpdateTable 
    SET CallState = -2 
	OUTPUT 0,
		   inserted.[ID],
		   inserted.[interviewID],
		   inserted.[TimeInShift],
		   inserted.[Priority],
		   inserted.[CallOrder],
		   inserted.[ApptID]
	INTO @CachedCalls

    SELECT c.[ID], 
           c.[ExplicitSid], 
           @SurveySID SurveySid, 
           i.DialingMode DiallingMode,
		   c.[InterviewID], 
		   i.[TelephoneNumber],
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   @GroupID as [GroupID]
    FROM @CachedCalls c 
    INNER JOIN BvInterview i ON c.[Interviewid] = i.[ID] AND
                                i.[SurveySID] = @SurveySID
    ORDER BY Priority DESC,
             TimeInShift,
 			 CallOrder
 
RETURN (@@ROWCOUNT)
GO
PRINT N'Altering [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]
	@SurveySID INT,
	@Count  INT,  --number of requested calls
	@SuitableTimeForCalls DATETIME
AS

	DECLARE @Groups TABLE(
		[ObjectSid] [int] NOT NULL,
		[GroupSize] [int] NOT NULL)
		
    DECLARE @MinDistributedCalls INT = 5
	
	INSERT INTO @Groups
    SELECT c.sid, count(*)
    FROM vLogins c with ( noexpand, INDEX([pk_vLogins]) )
	INNER JOIN BvActiveShiftTypeZone a on a.SurveyId = @SurveySid
	CROSS APPLY dbo.GetCallsForPredictiveMode(c.cnt*20, a.Id, c.sid, @SurveySID, @SuitableTimeForCalls)
	where c.SurveySID = @SurveySID
	group by c.sid
    
    DECLARE @totalCount INT 
    SELECT @totalCount = SUM(GroupSize) FROM @Groups
    DECLARE @part FLOAT = CAST(@Count AS FLOAT)/CAST(@totalCount AS FLOAT)
    DECLARE @current INT
    DECLARE @currentMinValue INT
    
    UPDATE @Groups
    SET @current = GroupSize*@part+0.5,
        @currentMinValue = CASE WHEN GroupSize < @MinDistributedCalls THEN GroupSize ELSE @MinDistributedCalls END,
        @current = CASE WHEN @current < @MinDistributedCalls THEN @currentMinValue ELSE @current END,
        GroupSize = @current
        
    DECLARE @usedCalls TABLE(
        [ObjectSid] [int] NOT NULL,
        [ID] [int] NOT NULL, 
        [Interview] [int] NOT NULL,
        [TimeInShift] [datetime] NOT NULL,
		[Priority] [INT] NOT NULL,
	    [CallOrder] [INT] NOT NULL,
		[ApptID] [INT])
        
	;WITH orderedUpdateTable as
	(    
		SELECT calls.*
		FROM @Groups g
		CROSS APPLY dbo.GetCallsForGroupForPredictiveSurvey(g.GroupSize, @SurveySID, g.ObjectSid, @SuitableTimeForCalls) calls
	)
	UPDATE orderedUpdateTable WITH(READPAST)
    SET CallState = -2 
	OUTPUT inserted.ExplicitSid,
		   inserted.[ID],
		   inserted.[interviewID],
		   inserted.[TimeInShift],
		   inserted.[Priority],
		   inserted.[CallOrder],
		   inserted.[ApptID]
	INTO @usedCalls
    
    SELECT c.ID, 
           ISNULL( p.Sid, 0 ) AS ExplicitSid, --person id (if person is assigned) or 0 (if survey or person group)
           @SurveySID AS SurveySid,
           i.DialingMode DiallingMode,
		   Interview AS InterviewID, 
		   TelephoneNumber,
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   (CASE WHEN p.Sid IS NULL AND @SurveySID <> ObjectSid THEN ObjectSid
                 ELSE 0
            END) AS GroupID --Explicit Group ID or 0 in all other cases (when call is assigned to  implicit survey group or a user)
    FROM @usedCalls c
    INNER JOIN BvInterview i ON Interview = i.ID AND   --we should avoid this join. this field can be stored in bvsvyschedule or somewhere else
                                SurveySID = @SurveySID
    LEFT JOIN BvPerson p on p.SID = ObjectSid
	ORDER BY Priority DESC,
             TimeInShift,
 			 CallOrder
	
RETURN (@@ROWCOUNT)
GO
PRINT N'Altering [dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]
 @SurveySID INT,
 @Count  INT,  --number of requested calls
 @SuitableTimeForCalls DATETIME
AS

SET NOCOUNT ON
	DECLARE @FixeNumberCallsPerPerson INT = 10

      --it should be at end of the SP. it uses for decrease recompilation
	DECLARE @CachedCalls TABLE (
	  [ExplicitSID] [int] NOT NULL,
	  [ID] [int] NOT NULL,
	  [InterviewID] [int] NOT NULL,
	  [TimeInShift] [datetime] NULL,
	  [Priority] [int] NOT NULL,
	  [CallOrder] [int] NOT NULL,
	  [ApptId] [int])
        
	;WITH orderedUpdateTable AS
	(
		SELECT c.*
		FROM BvLoginGroup g
		INNER JOIN BvActiveShiftTypeZone a on a.SurveyId=@SurveySID and a.SurveyId=g.SurveySID
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](g.PersonSID, a.Id, @SurveySID, @SuitableTimeForCalls, @FixeNumberCallsPerPerson) c
		WHERE g.PersonSid = g.ObjectSID
	)
    UPDATE orderedUpdateTable 
    SET CallState = -2 
	OUTPUT inserted.ExplicitSid,
		   inserted.[ID],
		   inserted.[interviewID],
		   inserted.[TimeInShift],
		   inserted.[Priority],
		   inserted.[CallOrder],
		   inserted.[ApptID]
	INTO @CachedCalls

    SELECT c.[ID], 
           c.[ExplicitSid], 
           @SurveySID SurveySid, 
           i.DialingMode DiallingMode,
		   c.[InterviewID], 
		   i.[TelephoneNumber],
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   0 as [GroupID]
    FROM @CachedCalls c 
    INNER JOIN BvInterview i ON c.[Interviewid] = i.[ID] AND
                                i.[SurveySID] = @SurveySID
    ORDER BY Priority DESC, TimeInShift, CallOrder
 
RETURN (@@ROWCOUNT)
GO
PRINT N'Altering [dbo].[BvSpGetOpenedSurveys]...';


GO
ALTER  PROCEDURE [dbo].[BvSpGetOpenedSurveys]
   @PersonSID INT
AS
SET NOCOUNT ON
    declare @utcnow datetime = getutcdate()
    SELECT com.SID, com.Name
    FROM (
         SELECT s.SID, s.[Name]
         FROM BvSurvey s
		 INNER JOIN BvPersonRel l on l.PersonSid = @PersonSID AND
		                              l.ObjectSID = s.SID
         WHERE s.State = 1
 
         UNION

         SELECT s.SID, s.[Name]
         FROM BvSurvey s
         WHERE s.State = 1 AND
		 EXISTS ( SELECT 1
			      FROM BvPersonRel l
			      INNER JOIN BvActiveShiftTypeZone a on a.SurveyId = s.SID
			      CROSS APPLY dbo.GetPriorityCallByExplicitSidAndShiftTypeId(l.ObjectSID, a.Id, a.SurveyId, @utcnow, 1)
			      WHERE l.PersonSID = @PersonSID
		 )) com
      ORDER BY com.Name

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpLookUpByPerson]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson]
	@personId INT,
	@SuitableTimeForCalls DATETIME
AS
    IF @personId is null and @SuitableTimeForCalls is null
	begin
		select 0 CallID,
		       0 SurveySID,
			   0 iid
		where 1 = 0
		return 0
	end

    DECLARE @interviewId INT
    DECLARE @rowCount INT
    DECLARE @surveyId INT

	create table #output(CallID int,
						 SurveySID int,
						 iid int)
    
    ;WITH calls AS
	(
		SELECT TOP(1) c.*
		FROM BvSvySchedule c WITH(READPAST)
		INNER JOIN BvLoginGroup p ON p.PersonSID = @personId
		INNER JOIN BvSurvey on SID = c.SurveySid AND DialMode !=  4 AND State =1
		INNER JOIN BvActiveShiftTypeZone a on a.SurveyId = BvSurvey.SID
		WHERE CallState = 2 AND
		      p.ObjectSID = c.ExplicitSID AND
			  TimeInShift <= @SuitableTimeForCalls AND
			  a.Id = c.ShiftTypeID
		ORDER BY Priority DESC,
                 TimeInShift,
				 ExplicitType DESC,
				 CallOrder
	)
	UPDATE calls
	SET CallState = -1,
	    ExpireTime = '9999-01-01 00:00:00.000',
		@interviewId = InterviewID,
		@surveyId = SurveySid
	OUTPUT
	   deleted.[ID] CallID,
	   deleted.SurveySID,
	   deleted.InterviewID iid
	INTO #output
	
	SET @rowCount = @@ROWCOUNT

	SELECT * FROM #output
	
	IF(@rowCount = 0) RETURN 0
	
	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
	      SurveysId = @surveyId AND 
	      InterviewSid = @interviewId
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpLookUpByPerson_ForAssignmentMode]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson_ForAssignmentMode]
	@surveyId INT,
	@personId INT,
	@SuitableTimeForCalls DATETIME
AS
    IF @personId is null and @SuitableTimeForCalls is null
	begin
		select 0 CallID,
		       0 SurveySID,
			   0 iid
		where 1 = 0
		return 0
	end

    DECLARE @interviewId INT
	DECLARE @callId INT
    DECLARE @rowCount INT
    
    ;WITH calls AS
	(
	    SELECT TOP(1) c.*
        FROM BvLoginGroup t
		INNER JOIN BvActiveShiftTypeZone a on a.Surveyid = t.SurveySid and t.SurveySid = @surveyId and t.PersonSID = @personId
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](t.ObjectSID, a.Id, @surveyId, @SuitableTimeForCalls, 1) c
		ORDER BY Priority DESC,
                 TimeInShift,
				 ExplicitType DESC,
				 CallOrder
	)
	UPDATE calls
	SET CallState = -1,
	    ExpireTime = '9999-01-01 00:00:00.000',
		@interviewId = InterviewID,
		@surveyId = SurveySid,
		@callId = Id

	SET @rowCount = @@ROWCOUNT

	select @callID CallID, @surveyId SurveySID, @interviewId iid
	where @callID is not null

	IF(@rowCount = 0) RETURN 0
	
	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
	      SurveysId = @surveyId AND 
	      InterviewSid = @interviewId
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpQueueUpSheduleTask3]...';


GO
ALTER PROCEDURE [dbo].[BvSpQueueUpSheduleTask3]
    @NowUTC           datetime,
    @DefaultTZ        INT
as
set nocount on

declare @rows int
 
    -- temp table for determine active shifts/survey
    create table #temp
    (
        [ID] int not null,
        SurveySID int not null
    )
 
    -- calculate live shifts 
    insert into #temp exec BvSpGetLiveShifts @NowUTC, @DefaultTZ
 
        -- copy new shifts information
     delete BvActiveShiftTypeZone
     insert into BvActiveShiftTypeZone
     select [ID], SurveySID from #temp
 
     drop table #temp
return (0)
GO
PRINT N'Altering [dbo].[BvSpSample_Finalize]...';


GO
ALTER  PROCEDURE BvSpSample_Finalize
    @BatchID INT,
    @BatchSize INT,
    @SurveySID INT
AS

DECLARE @IsRandomCallDeliveryEnabled BIT

SELECT @IsRandomCallDeliveryEnabled = IsRandomCallDeliveryEnabled
FROM BvSurvey
WHERE SID = @SurveySID

UPDATE BvSvySchedule
SET CallState = 2,
    CallOrder = CASE WHEN @IsRandomCallDeliveryEnabled = 0 THEN InterviewId
                     ELSE dbo.GetRandomValue(InterviewID)
                END
FROM BvInterview
WHERE BvInterview.SurveySid = BvSvySchedule.SurveySid AND
      BvInterview.ID = BvSvySchedule.InterviewID AND
      BvInterview.BatchID = @BatchID AND
      CallState = -3
   
RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpSchedule_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpSchedule_Delete]
       @ScheduleID int 
AS
DECLARE @rows INT

DECLARE @allHourSID INT
SELECT @allHourSID = MIN( ScheduleID ) FROM BvSchedule

/* Don't allow to delete 'All hours' schedule */
IF @allHourSID = @ScheduleID
BEGIN
	RAISERROR( 'Could not delete default scheduling script.', 12, 1)
    RETURN -1
END

IF EXISTS ( SELECT SID FROM BvSurvey WHERE ScheduleID = @ScheduleID AND State <> 2 )
BEGIN
	RAISERROR( 'Could not delete scheduling script that used by survey(s)', 12, 1)
	RETURN -1
END

BEGIN TRAN

    --should we update calls with none shift type?
    UPDATE BvSvySchedule SET ShiftTypeID = -z.TimeZoneID
    FROM BvSvySchedule c
    INNER JOIN BvShiftZones z ON c.ShiftTypeID = z.[ID] 
    INNER JOIN BvShiftType t ON t.OwnerSID = @ScheduleID AND z.ShiftTypeID = t.ObjectID


    DELETE FROM BvScheduleParam WHERE ScheduleID = @ScheduleID

    DELETE FROM BvShiftZones
        WHERE ShiftTypeID IN ( 
            SELECT ObjectID FROM BvShiftType
            WHERE OwnerSID = @ScheduleID )

    DELETE  BvShift
        WHERE OwnerSID = @ScheduleID

    DELETE  BvShiftType
        WHERE OwnerSID = @ScheduleID

    DELETE  BvTimezoneShift
        WHERE OwnerSID = @ScheduleID

    DELETE FROM BvSchedule 
        WHERE   ScheduleID = @ScheduleID

	IF EXISTS ( SELECT SID FROM BvSurvey WHERE ScheduleID = @ScheduleID AND State = 2 )
	BEGIN
		UPDATE BvSurvey 
		SET ScheduleID = @allHourSID 
		WHERE ScheduleID = @ScheduleID AND State = 2
	END

COMMIT

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpLookUpByPerson_ForCallGroup]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson_ForCallGroup]
	@SurveyID INT,
	@CallGroupID INT,
	@PersonID INT,
	@Now DATETIME
AS
	DECLARE @interviewId INT
	DECLARE @rowCount INT
	DECLARE @CallID INT
	DECLARE @ConditionValue INT
		    
	;WITH ExplicitSIDs AS
	(
		SELECT p.ObjectSID as ExplicitSID FROM BvLoginGroup p WHERE p.PersonSID = @personId AND p.ObjectSID IN ( @SurveyID, @personId )
	),
	conditions AS
	(
		SELECT ExplicitSID, ConditionValue, ConditionPriority, RotatePriority FROM ExplicitSIDs
		INNER JOIN BvCallGroupConditionPerSurvey cgc ON cgc.SurveyId = @SurveyID AND cgc.CallGroupId = @CallGroupID 
	),
	calls as
	(
		SELECT TOP(1) cc.* FROM conditions c
		INNER JOIN BvActiveShiftTypeZone a on a.SurveyId = @SurveyId
		CROSS APPLY dbo.GetCallByCondition( a.Id, @surveyId, c.ExplicitSID, c.ConditionValue, @Now ) cc
		ORDER BY Priority DESC, ConditionPriority DESC, RotatePriority ASC, TimeInShift, ExplicitType DESC, CallOrder
	)
	UPDATE calls WITH(READPAST)
	SET CallState = -1,
		@CallID = ID,
		@interviewId = InterviewID,
		@surveyId = SurveySid,
		@ConditionValue = ConditionValue
	
	SET @rowCount = @@ROWCOUNT
			
	SELECT @CallID as CallID, @surveyId as SurveySID, @interviewId as iid WHERE @rowCount <> 0
		
	IF(@rowCount = 0) RETURN 0
			
	UPDATE BvCallGroupConditionPerSurvey 
		SET ConditionPriority = ConditionPriority 
		WHERE	SurveyId = @SurveyID AND
				CallGroupId = @CallGroupID AND 
				ConditionValue = @ConditionValue

	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
			SurveysId = @surveyId AND 
			InterviewSid = @interviewId

	
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpLookUpByPerson_ForSurvey]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson_ForSurvey]
      @surveyId int,
      @personId int,
	  @Now DATETIME
AS
    DECLARE @CallID INT
    DECLARE @interviewId INT
    DECLARE @rowCount INT
    
    ;WITH ExplicitSIDs AS
    (
            SELECT p.ObjectSID FROM BvLoginGroup p WHERE p.PersonSID = @personId
    )
    ,calls AS
      (
            SELECT TOP(1) cc.*
            FROM ExplicitSIDs e
			inner join BvActiveShiftTypeZone a on a.SurveyId = @surveyId
            CROSS APPLY [dbo].[GetCallBySurvey](a.Id, @surveyId, e.ObjectSID, @Now) cc
            ORDER BY Priority DESC, TimeInShift, ExplicitType DESC, CallOrder
      )
      UPDATE calls WITH(READPAST)
      SET CallState = -1,
            @CallID = ID,
            @interviewId = InterviewID

      SET @rowCount = @@ROWCOUNT
      
      SELECT @CallID as CallID, @surveyId as SurveySID, @interviewId as iid WHERE @rowCount <> 0
      
      IF(@rowCount = 0) RETURN 0
      
      UPDATE BvAppointment 
      SET State = 2 
      WHERE State = 1 AND 
            SurveysId = @surveyId AND 
            InterviewSid = @interviewId
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpSvySch_Insert]...';


GO
ALTER  PROCEDURE [dbo].[BvSpSvySch_Insert]
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
        @ConditionValue     INT
AS
SET NOCOUNT ON
DECLARE @Rows INTEGER
DECLARE @ExplicitSID INTEGER
DECLARE @ExplicitType INTEGER
DECLARE @CallTZ INT

DECLARE @sqlQueryParams NVARCHAR(MAX)
DECLARE @sqlQuery NVARCHAR(MAX)
DECLARE @whereCondition NVARCHAR(MAX)
DECLARE @ROWCOUNT INT = 0
DECLARE @alias NVARCHAR(25) = 'repl'
DECLARE @ShiftTypeNone INT = -2147483648; --None constant for bvSvySchedule.ShiftTypeID

    SET @InterviewID = ABS( @InterviewID )

    -- Get call TZ
    SELECT @CallTZ = TimezoneID 
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
	SELECT @IsRandomCallDeliveryEnabled = IsRandomCallDeliveryEnabled
	FROM BvSurvey
	WHERE SID = @SurveySID
      
    EXEC BvClr_QuotaService_GetWhereForAllClosedSurveyCells @SurveySID, @alias, @whereCondition OUTPUT

    SET @sqlQuery = 
      N'SET @refID = 0
        MERGE BvSvySchedule as target
        USING( SELECT @SurveySID, 
                      @InterviewId, 
                      @ApptID, 
                      (SELECT COUNT(*)
                       FROM BvReplicatedData_' + CAST(@SurveySID AS NVARCHAR(255)) + ' as repl
                       WHERE respid = @InterviewID AND
                             (' + @whereCondition + '))) AS source (SurveySid, InterviewId, Appt, IsClosed)
        ON target.SurveySID = source.SurveySID AND
           target.InterviewID = source.InterviewID
        WHEN MATCHED
        THEN
			  UPDATE
			  SET @refoldApptID     = ApptID,
			      @refID            = CASE WHEN Appt > 0 OR IsClosed = 0 THEN ID ELSE 0 END,
				  ApptID            = @ApptID,
				  CallState         = CASE WHEN Appt > 0 OR IsClosed = 0 THEN @CallState ELSE 0 END,
				  Priority          = @Priority,
				  TimeInShift       = @TimeInShift1,
				  ExpireTime        = @ExpirationTime,
				  ShiftTypeID       = @ShiftTypeID,
				  ExplicitSID       = @ExplicitSID,
				  ExplicitType      = @ExplicitType,
				  RuleNumber        = @RuleNumber,
                  ConditionValue    = @ConditionValue,
				  OldPriority       = 0
        WHEN NOT MATCHED AND ( Appt > 0 OR IsClosed = 0 )
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
					 ConditionValue )
              VALUES(@ShiftTypeID,
                     @ApptID,
                     @InterviewID,
                     @SurveySID,
                     @CallState,
                     @Priority,
                     @TimeInShift1,
                     @ExpirationTime,
                     @ExplicitSID,
                     @ExplicitType,
                     @RuleNumber,
                     CASE WHEN @IsRandomCallDeliveryEnabled = 0 THEN InterviewId
						  ELSE dbo.GetRandomValue(@InterviewID)
					 END,
					 @ConditionValue);
         
        IF( (@@ROWCOUNT > 0) AND (@refoldApptID IS NULL))
           SET @refID = @@IDENTITY'
        
SET @sqlQueryParams = N'@SurveySID INT, @IsRandomCallDeliveryEnabled BIT, @CallState INT, @ApptID INT, @Priority INT, ' +
       '@TimeInShift1 DATETIME, @ExpirationTime DATETIME, @ShiftTypeID INT, @ExplicitSID INT, ' +
       '@ExplicitType INT, @RuleNumber uniqueidentifier, @InterviewID INT, @ConditionValue INT, ' +
       '@refID INT OUTPUT, @refoldApptID INT OUTPUT'
       
    EXEC sp_executesql @sqlQuery, @sqlQueryParams, @SurveySID, @IsRandomCallDeliveryEnabled, @CallState, @ApptID, @Priority,
       @TimeInShift1, @ExpirationTime, @ShiftTypeID, @ExplicitSID, @ExplicitType, @RuleNumber,
       @InterviewID, @ConditionValue, @refID = @ID OUTPUT, @refoldApptID = @oldApptID OUTPUT

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
PRINT N'Refreshing [dbo].[BvSpAlert_RecalculateAppointment]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpAlert_RecalculateAppointment';


GO
PRINT N'Refreshing [dbo].[BvSpAssignment_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpAssignment_Delete';


GO
PRINT N'Refreshing [dbo].[BvSpAssignment_List]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpAssignment_List';


GO
PRINT N'Refreshing [dbo].[BvSpCall_ChangePriority]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpCall_ChangePriority';


GO
PRINT N'Refreshing [dbo].[BvSpCall_Enable]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpCall_Enable';


GO
PRINT N'Refreshing [dbo].[BvSpCall_Get]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpCall_Get';


GO
PRINT N'Refreshing [dbo].[BvSpCall_GetInfo]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpCall_GetInfo';


GO
PRINT N'Refreshing [dbo].[BvSpCall_MoveToITS]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpCall_MoveToITS';


GO
PRINT N'Refreshing [dbo].[BvSpCalls_Delete_Batch]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpCalls_Delete_Batch';


GO
PRINT N'Refreshing [dbo].[BvSpDialer_Reset]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpDialer_Reset';


GO
PRINT N'Refreshing [dbo].[BvSpGetAllAppointmentsForUser]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpGetAllAppointmentsForUser';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson_ForManualMode]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpLookUpByPerson_ForManualMode';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpPerson_Delete';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_GetAssignedSurveyList]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpPerson_GetAssignedSurveyList';


GO
PRINT N'Refreshing [dbo].[BvSpPersonAndGroups_List]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpPersonAndGroups_List';


GO
PRINT N'Refreshing [dbo].[BvSpPersonGroup_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpPersonGroup_Delete';


GO
PRINT N'Refreshing [dbo].[BvSpReleaseCall]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpReleaseCall';


GO
PRINT N'Refreshing [dbo].[BvSpRemoveExpiredCalls]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpRemoveExpiredCalls';


GO
PRINT N'Refreshing [dbo].[BvSpSample_Abandon]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpSample_Abandon';


GO
PRINT N'Refreshing [dbo].[BvSpSetCallDeliveryMode]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpSetCallDeliveryMode';


GO
PRINT N'Refreshing [dbo].[BvSpSetCallState]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpSetCallState';


GO
PRINT N'Refreshing [dbo].[BvSpShiftType_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpShiftType_Delete';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Clean]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpSurvey_Clean';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpSurvey_Delete';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_DeleteFiltered]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpSurvey_DeleteFiltered';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyCleanup_IsClean]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpSurveyCleanup_IsClean';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyModifyStateGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpSurveyModifyStateGroup';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyState_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpSurveyState_Update';


GO
PRINT N'Refreshing [dbo].[BvSpSvySch_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpSvySch_Delete';


GO
PRINT N'Refreshing [dbo].[BvSpTimezone_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpTimezone_Delete';


GO
PRINT N'Refreshing [dbo].[BvSpTimezone_DeleteUnused]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpTimezone_DeleteUnused';


GO
PRINT N'Refreshing [dbo].[BvSpUpdateInProgressCallsToScheduled]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpUpdateInProgressCallsToScheduled';


GO
PRINT N'Refreshing [dbo].[BvSpCallCenter_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpCallCenter_Delete';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpSurvey_Update';


GO
PRINT N'Update complete.';


GO
