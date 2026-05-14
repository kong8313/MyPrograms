CREATE PROCEDURE [dbo].[BvSpShift_List]
        @OwnerSID int,
        @ID int,
        @TimezoneID int      
AS

IF @TimezoneID = 0 BEGIN    
    IF @ID = 0 BEGIN
        SELECT  ID,
            CycleType,
            StartDayOfWeek,
            StartTime,
            FinishDayOfWeek,
            FinishTime,
            (   SELECT  COUNT(*) 
                    FROM    BvTimezoneShift 
                    WHERE   OwnerSID = @OwnerSID 
                    AND     ShiftID = ID ) TimezoneID,
            ShiftTypeID
            FROM    BvShift
            WHERE   OwnerSID = @OwnerSID
            ORDER BY BvShift.ID
    END
    ELSE BEGIN
        SELECT  @ID ID,
            CycleType,
            StartDayOfWeek,
            StartTime,
            FinishDayOfWeek,
            FinishTime,
            0  TimezoneID,
            ShiftTypeID
            FROM    BvShift
            WHERE   OwnerSID = @OwnerSID
            AND     ID = @ID
        UNION
        SELECT  @ID ID,
            BvShift.CycleType,
            BvTimezoneShift.StartDayOfWeek,
            BvTimezoneShift.StartTime,
            BvTimezoneShift.FinishDayOfWeek,
            BvTimezoneShift.FinishTime,
            BvTimezoneShift.TimezoneID TimezoneID,
            BvShift.ShiftTypeID
            FROM    BvShift 
            JOIN    BvTimezoneShift     
                ON  BvShift.ID = BvTimezoneShift.ShiftID
                AND BvTimezoneShift.OwnerSID = @OwnerSID
            WHERE   BvShift.ID = @ID
                AND BvShift.OwnerSID = @OwnerSID
        ORDER   BY  TimezoneID
    END 
END ELSE IF @TimezoneID > 0 BEGIN
    IF @ID = 0 BEGIN
        SELECT  BvShift.ID,
            BvShift.CycleType,
            ISNULL( BvTimezoneShift.StartDayOfWeek, BvShift.StartDayOfWeek ) StartDayOfWeek,
            ISNULL( BvTimezoneShift.StartTime, BvShift.StartTime ) StartTime,
            ISNULL( BvTimezoneShift.FinishDayOfWeek, BvShift.FinishDayOfWeek ) FinishDayOfWeek,
            ISNULL( BvTimezoneShift.FinishTime, BvShift.FinishTime ) FinishTime,
            ISNULL( BvTimezoneShift.TimezoneID, 0 ) TimezoneID,
            BvShift.ShiftTypeID
            FROM    BvShift
            LEFT JOIN BvTimezoneShift ON BvShift.ID = BvTimezoneShift.ShiftID 
                        AND BvShift.OwnerSID = BvTimezoneShift.OwnerSID 
                        AND BvTimezoneShift.TimezoneID = @TimezoneID        
            WHERE   BvShift.OwnerSID = @OwnerSID
            ORDER BY BvShift.ID
    END
    ELSE BEGIN
        SELECT  ID,
            CycleType,
            StartDayOfWeek,
            StartTime,
            FinishDayOfWeek,
            FinishTime,
            0 TimezoneID,
            ShiftTypeID
            FROM    BvShift
            WHERE   OwnerSID = @OwnerSID
            AND     ID = @ID
        UNION
        SELECT  BvTimezoneShift.ShiftID ID,
            BvShift.CycleType,
            BvTimezoneShift.StartDayOfWeek,
            BvTimezoneShift.StartTime,
            BvTimezoneShift.FinishDayOfWeek,
            BvTimezoneShift.FinishTime,
            BvTimezoneShift.TimezoneID,
            BvShift.ShiftTypeID
            FROM    BvTimezoneShift
            JOIN    BvShift ON BvTimezoneShift.ShiftID = BvShift.ID
                    AND BvTimezoneShift.OwnerSID = BvShift.OwnerSID
            WHERE   BvTimezoneShift.OwnerSID = @OwnerSID
            AND     BvTimezoneShift.ShiftID = @ID
            AND     BvTimezoneShift.TimezoneID = @TimezoneID    
            ORDER BY TimezoneID 
    END
END ELSE BEGIN
    SELECT ID,
        CycleType,
        StartDayOfWeek,
        StartTime,
        FinishDayOfWeek,
        FinishTime,
        0 TimezoneID,
        ShiftTypeID
        FROM BvShift
        WHERE OwnerSID = @OwnerSID
    UNION
    SELECT BvTimezoneShift.ShiftID ID,
        BvShift.CycleType,
        BvTimezoneShift.StartDayOfWeek,
        BvTimezoneShift.StartTime,
        BvTimezoneShift.FinishDayOfWeek,
        BvTimezoneShift.FinishTime,
        BvTimezoneShift.TimezoneID,
        BvShift.ShiftTypeID
        FROM BvTimezoneShift
        JOIN BvShift ON
                BvTimezoneShift.ShiftID = BvShift.ID AND
                BvTimezoneShift.OwnerSID = BvShift.OwnerSID
        WHERE BvShift.OwnerSID = @OwnerSID
        ORDER   BY  TimezoneID
END