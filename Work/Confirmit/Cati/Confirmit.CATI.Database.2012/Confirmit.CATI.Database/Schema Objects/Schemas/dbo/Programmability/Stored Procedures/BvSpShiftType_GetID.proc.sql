CREATE PROCEDURE [dbo].[BvSpShiftType_GetID]
@OwnerID INT,
@ID      INT
AS
DECLARE @SID INT

    SELECT @SID = ObjectID 
      FROM BvShiftType
      WHERE OwnerSID = @OwnerID
           AND [ID] = @ID

    IF @SID IS NULL
        RAISERROR( 'Could not find shift type with owner id %i and id %i', 16, 1, @OwnerID, @ID )

RETURN ISNULL(@SID, -1)