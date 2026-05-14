CREATE PROCEDURE [dbo].[BvSpTasks_UpdateNewSurveySid]
 @PersonSID int,
 @NewSurveySID int
AS

UPDATE [dbo].[BvTasks]
    SET NewSurveySID = @NewSurveySID
WHERE PersonSID = @PersonSID

RETURN 0