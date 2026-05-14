GO
ALTER PROCEDURE [dbo].[BvSpPerson_SetAutomaticSurvey]
    @PersonId INT,
    @AutoSurveyId INT
AS
    
UPDATE BvPerson SET AutomaticSurveyID = @AutoSurveyId WHERE SID = @PersonId;

CREATE TABLE #result( DialerId INT, InterviewState TINYINT, SurveySID INT)

UPDATE BvTasks 
    SET NewSurveySID = @AutoSurveyId 
    OUTPUT inserted.DialerId, inserted.InterviewState, inserted.SurveySID INTO #result
    WHERE PersonSID = @PersonId AND SurveySID > 0 AND StatusLogout NOT IN (0, 1, 4 ) -- LoginState.LOGGING_IN || LoginState.LOGGING_OUT || LoginState.NOT_LOGGED_IN
        AND EXISTS( SELECT 1 FROM BvSurvey WHERE SID = @AutoSurveyId AND State = 1 AND (
        (BvTasks.LoggedInToDialerState = 2/*LOGGED_IN*/ AND DialMode <> 1 /*Manual*/) OR
        (BvTasks.LoggedInToDialerState = 0/*NOT_LOGGED_IN*/ AND DialMode = 1 /*Manual*/))) AND
		SurveySID <> @AutoSurveyId

SELECT * FROM #result
/*
UPDATE BvTasks 
    SET NewSurveySID = CASE 
        WHEN EXISTS( SELECT 1 FROM BvSurvey WHERE SID = @AutoSurveyId AND State = 1 AND (
            (BvTasks.LoggedInToDialerState = 2/LOGGED_IN/ AND DialMode <> 1 /Manual/) OR
            (BvTasks.LoggedInToDialerState = 0/NOT_LOGGED_IN/ AND DialMode = 1 /Manual/))) THEN  @AutoSurveyId ELSE NewSurveySID END
    OUTPUT inserted.NewSurveySID, inserted.DialerId, inserted.InterviewState
    WHERE PersonSID = @PersonId AND SurveySID > 0 AND StatusLogout NOT IN (0, 1, 4 ) -- LoginState.LOGGING_IN || LoginState.LOGGING_OUT || LoginState.NOT_LOGGED_IN/
    */
GO

