CREATE PROCEDURE [dbo].[BvSpTimezone_Delete]
        @ID     int,
        @Mode   int
AS

DECLARE @Rows int
DECLARE @res bit

SELECT @Rows = COUNT( * ) FROM BvTimezone WHERE ID = @ID

IF @Rows = 0 
  BEGIN
    RAISERROR( 'Timezone %i not exists', 16, 1, @ID )
    RETURN -1
  END

SELECT @Rows = COUNT( * ) FROM BvTimezoneShift WHERE TimezoneID = @ID

IF @Rows <> 0 
  BEGIN
    RAISERROR( 'Unable to delete timezone %i. Link exists on timezone shift', 12, 1, @ID )
    RETURN -1
  END

IF EXISTS( SELECT TOP 1 BvSvySchedule.[ID] 
             FROM BvSvySchedule, BvShiftZones
            WHERE BvShiftZones.TimeZoneID = @ID
                  AND BvSvySchedule.ShiftTypeID = BvShiftZones.[ID] )
BEGIN
    RAISERROR( 'Unable to delete timezone %i. Link exists on calls', 12, 1, @ID )
    RETURN -1
END

IF EXISTS(SELECT * FROM BvCallCenter WHERE LocalTimezoneId = @ID)
BEGIN
	RAISERROR('Unable to delete timezone %i. The timezone is used in some call center', 12, 1, @ID)
	RETURN -1
END
  
SELECT @res = COUNT(*)
FROM BvInterview
WHERE TimezoneID = @ID

IF @res <> 0
BEGIN
  RAISERROR( 'Unable to delete timezone %i. Link exist on interview', 12, 1, @ID )
  RETURN -1
END

IF EXISTS(SELECT * FROM BvTimezone WHERE ParentID = @ID)
BEGIN
	RAISERROR('Unable to delete timezone %i. The timezone has custom timezones', 12, 1, @ID)
	RETURN -1	
END



BEGIN TRANSACTION

DELETE BvTimezone WHERE ID = @ID

COMMIT TRANSACTION

RETURN 0