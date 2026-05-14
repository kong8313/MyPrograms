CREATE PROCEDURE [dbo].[BvSpTasks_InsertUpdate_2]
 @PersonSID int,
 @SurveySID int,
 @ExtensionNumber NVARCHAR(256),
 @LoggedInToDialerState tinyint,
 @IsLoginRCToDialer BIT,
 @DiallingMode TINYINT
AS

DECLARE @Now DATETIME = [dbo].GetUtcNow()

UPDATE [dbo].[BvTasks]
    SET TimeStateChanged = @Now,
	    SurveySID = @SurveySID,
	    InterviewID = 0,
        StatusLogout = 2, --LOGGED_IN
        LoggedInToDialerState = @LoggedInToDialerState,
        IsLoginRCToDialer = @IsLoginRCToDialer,
        DiallingMode = @DiallingMode,
		StationExtensionNumber = @ExtensionNumber
WHERE PersonSID = @PersonSID

RETURN 0
