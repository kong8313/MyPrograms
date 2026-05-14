CREATE PROCEDURE [dbo].[BvSpShiftType_Insert]
        @OwnerSID int,
        @ID int,
        @Name nvarchar(255),
        @Color int,
        @ObjectID int
AS
SET NOCOUNT ON

DECLARE @Rows int

SELECT  @Rows = COUNT(*)
    FROM    BvShiftType
    WHERE   ID = @ID
    AND OwnerSID = @OwnerSID

IF @Rows <> 0
--  return 50001    /* BVDBS_STORED_PROCEDURE_DUPLICATED_OBJECT */
    return 0

    INSERT BvShiftType( OwnerSID, ID, Name, Color )
      VALUES( @OwnerSID, @ID, @Name, @Color )

    SET @Rows = @@IDENTITY

    -- Insert shift type time zones
    INSERT INTO BvShiftZones VALUES( 0, @Rows )
    INSERT INTO BvShiftZones 
      SELECT BvTimeZone.[ID], @Rows
      FROM BvTimeZone

RETURN @Rows