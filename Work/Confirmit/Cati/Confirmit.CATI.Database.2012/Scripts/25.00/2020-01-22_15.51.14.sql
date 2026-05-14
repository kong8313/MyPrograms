PRINT N'Altering [dbo].[BvSpGetLoggedInPersonsCount]...';

GO
ALTER PROCEDURE [dbo].[BvSpGetLoggedInPersonsCount]
AS

SELECT COUNT (*) FROM BvTasks t
INNER JOIN BvPerson p ON t.PersonSID = p.SID 
WHERE p.Type = 0                                -- count usual interviewers only and skip IVR agents

RETURN 0


GO
PRINT N'Update complete.';