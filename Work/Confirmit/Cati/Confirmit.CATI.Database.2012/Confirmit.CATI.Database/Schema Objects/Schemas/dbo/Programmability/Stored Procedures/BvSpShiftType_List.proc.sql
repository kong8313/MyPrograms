CREATE PROCEDURE [dbo].[BvSpShiftType_List]
        @OwnerSID int 

AS

IF @OwnerSID = 0
    SELECT  ID,  [Name],  [Color], ObjectID
        FROM    BvShiftType
ELSE
    SELECT  ID,  [Name],  [Color], ObjectID
        FROM    BvShiftType
        WHERE   OwnerSID = @OwnerSID

RETURN (0)