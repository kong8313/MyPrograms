GO
ALTER PROCEDURE [dbo].[BvSpPerson_SetAutomaticSurvey]
    @PersonId INT,
    @AutoSurveyId INT
AS
    
UPDATE BvPerson SET AutomaticSurveyID = @AutoSurveyId WHERE SID = @PersonId;

DECLARE @result as table( DialerId INT, InterviewState TINYINT, SurveySID INT,LoggedInToDialerState TINYINT, DialMode TINYINT)

DECLARE @NewSurveyDialMode int;
SELECT @NewSurveyDialMode = (SELECT DialMode FROM BvSurvey WHERE SID = @AutoSurveyId AND State = 1)

DECLARE @CurrentDialMode int;
SELECT @CurrentDialMode = (SELECT s.DialMode FROM BvTasks t JOIN BvSurvey s ON t.SurveySID = s.SID WHERE t.PersonSID = @PersonId )

UPDATE BvTasks 
    SET NewSurveySID = @AutoSurveyId 
    OUTPUT inserted.DialerId, inserted.InterviewState, inserted.SurveySID, inserted.LoggedInToDialerState, @CurrentDialMode  INTO @result
    WHERE PersonSID = @PersonId 
	AND SurveySID > 0 
	AND StatusLogout NOT IN (0, 1, 4 ) -- LoginState.LOGGING_IN || LoginState.LOGGING_OUT || LoginState.NOT_LOGGED_IN
	AND SurveySID <> @AutoSurveyId
	AND ((BvTasks.LoggedInToDialerState = 2/*LOGGED_IN*/ 
			AND ((@CurrentDialMode = 4 /*Predictive*/ AND @NewSurveyDialMode = 4) OR  (@CurrentDialMode in (2, 3) AND @NewSurveyDialMode in (2, 3)))) 
		OR	(BvTasks.LoggedInToDialerState = 0/*NOT_LOGGED_IN*/ AND @NewSurveyDialMode = 1 /*Manual*/))	

SELECT * FROM @result
GO

