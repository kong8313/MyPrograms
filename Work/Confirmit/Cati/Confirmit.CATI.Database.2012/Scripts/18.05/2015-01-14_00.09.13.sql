PRINT N'Altering [dbo].[BvSpDialer_Reset]...';


GO
ALTER PROCEDURE [dbo].[BvSpDialer_Reset]
    @ProblemID INT
AS  
    UPDATE BvSvySchedule SET CallState = 2 
	FROM BvSvySchedule c
	JOIN BvSurvey s ON
        c.SurveySID = s.SID 
    WHERE CallState = -2

    UPDATE BvTasks 
    SET ProblemId = @ProblemID 
    WHERE LoggedInToDialerState = 1/*LOGGING_IN*/ OR 
          LoggedInToDialerState = 2/*LOGGED_IN*/
GO
PRINT N'Update complete.';


GO
