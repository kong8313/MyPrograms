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
        @ConditionValue     INT,
		@FcdBehaviorAlgorithm INT --0 - delete calls/ 1 - disable calls
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
    DECLARE @ClusteredQuotaName NVARCHAR(256)
    
	SELECT @IsRandomCallDeliveryEnabled = IsRandomCallDeliveryEnabled,
		   @ClusteredQuotaName = ClusteredQuotaName
	FROM BvSurvey
	WHERE SID = @SurveySID

      
    EXEC BvClr_QuotaService_GetWhereForAllClosedSurveyCells @SurveySID, @alias, @whereCondition OUTPUT
	
	EXEC BvClr_ClusterQuotaService_GetCellIdQuery @SurveySID, @ClusteredQuotaName, @alias, @ClusteredCellIdQuery OUTPUT
	
	SET @sqlQuery = 
      N'SET @refID = 0
        MERGE BvSvySchedule as target
        USING( SELECT d.*, CASE WHEN ' + @whereCondition + ' THEN 1 ELSE 0 END, ( ' + @ClusteredCellIdQuery + ') as CellId
					 FROM ( SELECT @SurveySID as SurveyId, 
								   @InterviewId as InterviewId, 
								   @ApptID as ApptId) as d
					 LEFT JOIN BvReplicatedData_' + CAST(@SurveySID AS NVARCHAR(255)) + ' as repl
                     ON respid = @InterviewID ) AS source (SurveySid, InterviewId, Appt, IsClosed, CellId)
        ON target.SurveySID = source.SurveySID AND
           target.InterviewID = source.InterviewID
        WHEN MATCHED
        THEN
			  UPDATE
			  SET @refoldApptID     = ApptID,
			      @refID            = CASE WHEN Appt > 0 OR IsClosed = 0 THEN ID ELSE 0 END,
				  ApptID            = @ApptID,
				  CallState         = CASE WHEN Appt > 0 OR IsClosed = 0 THEN @CallState ELSE @FcdBehaviorAlgorithm END,
				  Priority          = @Priority,
				  TimeInShift       = @TimeInShift1,
				  ExpireTime        = @ExpirationTime,
				  ShiftTypeID       = @ShiftTypeID,
				  ExplicitSID       = @ExplicitSID,
				  ExplicitType      = @ExplicitType,
				  RuleNumber        = @RuleNumber,
                  ConditionValue    = @ConditionValue,
				  OldPriority       = 0
        WHEN NOT MATCHED AND ( Appt > 0 OR IsClosed = 0 OR @FcdBehaviorAlgorithm = 1)
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
					 CellId )
              VALUES(@ShiftTypeID,
                     @ApptID,
                     @InterviewID,
                     @SurveySID,
                     CASE WHEN Appt > 0 OR IsClosed = 0 THEN @CallState ELSE 1/*disabled*/ END,
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
					 CellId);
         
        IF( (@@ROWCOUNT > 0) AND (@refoldApptID IS NULL))
           SET @refID = @@IDENTITY'
           
SET @sqlQueryParams = N'@SurveySID INT, @IsRandomCallDeliveryEnabled BIT, @CallState INT, @ApptID INT, @Priority INT, ' +
       '@TimeInShift1 DATETIME, @ExpirationTime DATETIME, @ShiftTypeID INT, @ExplicitSID INT, ' +
       '@ExplicitType INT, @RuleNumber uniqueidentifier, @InterviewID INT, @ConditionValue INT, @FcdBehaviorAlgorithm INT,' +
       '@refID INT OUTPUT, @refoldApptID INT OUTPUT'
       
    EXEC sp_executesql @sqlQuery, @sqlQueryParams, @SurveySID, @IsRandomCallDeliveryEnabled, @CallState, @ApptID, @Priority,
       @TimeInShift1, @ExpirationTime, @ShiftTypeID, @ExplicitSID, @ExplicitType, @RuleNumber,
       @InterviewID, @ConditionValue, @FcdBehaviorAlgorithm, @refID = @ID OUTPUT, @refoldApptID = @oldApptID OUTPUT

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
PRINT 'Add new system setigs:'
GO
;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
(
SELECT 'FCD.BehaviorType', 'Type of FCD''s algorithm', 'FCD', 'Type of FCD''s algorithm. Following types are allowed: 0-delete calls, 1-disable calls with reenabling on opening cell(s)', 1, 0, '0'
)
INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
	SELECT * FROM Data
GO

GO
PRINT N'Update complete.';


GO
