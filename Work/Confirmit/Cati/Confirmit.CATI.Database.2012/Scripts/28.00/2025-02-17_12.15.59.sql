GO
PRINT N'Altering Procedure [dbo].[BvSpCheckCallOnShifts]...';


GO
ALTER PROCEDURE [dbo].[BvSpCheckCallOnShifts]
@TimeZoneID     INT,
	/* 
	 * @ShiftTypeID > 0 means specific shift type id( BvShiftType.ID )
	 * @ShiftTypeID = 0 means [None]
	 * @ShiftTypeID =-1 @ShiftTypeID means [Any valid]
	 */

@ShiftTypeID    INT, 
@TimeInShift    DATETIME,   -- In UTC
@SurveySID      INT,
@DefaultTimeZoneID INT
AS
DECLARE @Bias INT
DECLARE @OwnerID INT

      IF @ShiftTypeID IS NULL OR @ShiftTypeID = 0
          RETURN (0)

      SELECT @OwnerID = [ScheduleID] FROM BvSurvey
            WHERE [SID] = @SurveySID
            
      IF @TimeZoneID = 0
	  SET @TimeZoneID = @DefaultTimeZoneID
            
      IF NOT EXISTS ( SELECT 1 FROM BvTzPeriodicalShifts
					  WHERE (type_id = @ShiftTypeID or @ShiftTypeID = -1) AND
				             owner_id = @OwnerID and tz_id = @TimeZoneID and start_dt != finish_dt )
	   BEGIN	
			IF(@ShiftTypeID = -1)				
				RAISERROR( 'Scheduling script does not contain any shift types', 12, 1)
			ELSE
				BEGIN		
					DECLARE @ShiftTypeName as nvarchar(20)
					SET @ShiftTypeName = (select Name from BvShiftType where ObjectID = @ShiftTypeID)
				
					RAISERROR( 'Scheduling script does not contain specific shift type with name %s (%d), scheduling script id: %d, timezone id: %d', 12, 1, @ShiftTypeName, @ShiftTypeID, @OwnerID, @TimeZoneID)
				END
			RETURN (-1)
	   END

RETURN (0)
GO
PRINT N'Refreshing Procedure [dbo].[BvSpSvySch_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSvySch_Insert]';


GO
PRINT N'Update complete.';


GO
