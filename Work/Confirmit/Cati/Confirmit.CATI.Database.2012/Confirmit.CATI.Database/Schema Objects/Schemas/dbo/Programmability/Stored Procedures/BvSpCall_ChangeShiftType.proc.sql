CREATE PROCEDURE [dbo].[BvSpCall_ChangeShiftType]
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