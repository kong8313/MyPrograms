CREATE PROCEDURE [dbo].[BvSpShiftType_Delete]
@OwnerSID INTEGER,
@ID       INTEGER,
@Mode     INTEGER
AS

DECLARE @Rows     INTEGER
DECLARE @Rows2    INTEGER
DECLARE @ObjectID INTEGER

SELECT  @Rows = COUNT( * ), @ObjectID = MIN( ObjectID )
    FROM    BvShiftType
    WHERE   OwnerSID = @OwnerSID
    AND ID = @ID

IF @Rows = 0
  BEGIN
    RAISERROR('Shift type with ID %i not exists', 16, 2, @ID)
    RETURN -1
  END
IF @Rows <> 1
  BEGIN
    RAISERROR('Multiple shift types with ID %i found', 16, 2, @ID)
    RETURN -1
  END

SELECT  @Rows = COUNT( * )
    FROM    BvShift
    WHERE   OwnerSID = @OwnerSID
    AND ShiftTypeID = @ObjectID

SELECT @Rows2 = COUNT( BvSvySchedule.ShiftTypeID )
    FROM BvSvySchedule, BvShiftZones
    WHERE BvShiftZones.ShiftTypeID = @ObjectID
      AND BvShiftZones.[ID] = BvSvySchedule.ShiftTypeID

IF @Rows <> 0 OR @Rows2 <> 0 BEGIN
    IF @Mode <> 2 /* BVDBS_ACTION_MODE_STRONG */
    BEGIN
        IF @Rows <> 0
          RAISERROR( 'Unable to delete shift type. Link exists on shifts', 12, 1 )
        ELSE 
          RAISERROR( 'Unable to delete shift type. Link exists on calls', 12, 1 )
        return -1
    END
    ELSE BEGIN
        DELETE  BvTimezoneShift
            WHERE   OwnerSID = @OwnerSID
            AND ShiftID IN ( SELECT  ID
                            FROM    BvShift
                            WHERE   OwnerSID = @OwnerSID
                            AND ShiftTypeID = @ObjectID )
        DELETE  BvShift
            WHERE   OwnerSID = @OwnerSID
            AND ShiftTypeID = @ObjectID
            
        DECLARE @changingTable table(ApptID INT NOT NULL)

        DELETE FROM BvSvySchedule 
        OUTPUT DELETED.ApptID
        INTO @changingTable
        WHERE ShiftTypeID IN ( SELECT [ID] FROM BvShiftZones WHERE ShiftTypeID = @ObjectID ) AND
              (CallState > 0 OR CallState = -2)
        
        UPDATE BvAppointment
        SET State = 2
        FROM @changingTable c
        WHERE c.ApptID = BvAppointment.ID
    END
END

DELETE  BvShiftType
    WHERE   OwnerSID = @OwnerSID
    AND ID = @ID

DELETE FROM BvShiftZones WHERE ShiftTypeID = @ObjectID

RETURN 0