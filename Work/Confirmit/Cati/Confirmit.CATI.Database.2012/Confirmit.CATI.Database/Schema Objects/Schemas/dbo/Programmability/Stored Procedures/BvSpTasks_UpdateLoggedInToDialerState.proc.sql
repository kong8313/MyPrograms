CREATE PROCEDURE [dbo].[BvSpTasks_UpdateLoggedInToDialerState]
 @PersonSID int,
 @LoggedInToDialerState tinyint
AS

UPDATE [dbo].[BvTasks]
    SET LoggedInToDialerState = @LoggedInToDialerState
WHERE PersonSID = @PersonSID

RETURN 0