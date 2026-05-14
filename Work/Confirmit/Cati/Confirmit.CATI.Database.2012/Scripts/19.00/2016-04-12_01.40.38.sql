PRINT N'Altering [dbo].[BvSpPerson_SetAutomaticSurvey]...';
GO

ALTER PROCEDURE [dbo].[BvSpPerson_SetAutomaticSurvey]
    @PersonId INT,
    @AutoSurveyId INT
AS
    
DECLARE @OldAutomaticSurveyID int;

UPDATE BvPerson SET @OldAutomaticSurveyID = AutomaticSurveyID, AutomaticSurveyID = @AutoSurveyId WHERE SID = @PersonId;

DECLARE @result as table(DialerId INT, InterviewState TINYINT, SurveySID INT,LoggedInToDialerState TINYINT, DialMode TINYINT)

DECLARE @NewSurveyDialMode int;
SELECT @NewSurveyDialMode = (SELECT DialMode FROM BvSurvey WHERE SID = @AutoSurveyId AND State = 1)

DECLARE @CurrentSurveySID int;
SELECT @CurrentSurveySID = (SELECT SurveySID FROM BvTasks WHERE PersonSID = @PersonId)

DECLARE @CurrentDialMode int;

IF @CurrentSurveySID > 0
	SELECT @CurrentDialMode = (SELECT DialMode FROM BvSurvey WHERE SID = @CurrentSurveySID)
ELSE
	SELECT @CurrentDialMode = (SELECT DialMode FROM BvSurvey WHERE SID = @OldAutomaticSurveyID)

UPDATE BvTasks 
    SET NewSurveySID = @AutoSurveyId 
    OUTPUT inserted.DialerId, inserted.InterviewState, inserted.SurveySID, inserted.LoggedInToDialerState, @CurrentDialMode  INTO @result
    WHERE PersonSID = @PersonId 
	AND StatusLogout <> 4 -- LoginState.LOGGING_OUT
	AND SurveySID <> @AutoSurveyId
	AND (
        ((@CurrentDialMode = 4 /*Predictive*/ AND @NewSurveyDialMode = 4) OR (@CurrentDialMode in (2, 3) AND @NewSurveyDialMode in (2, 3)))
        OR (BvTasks.LoggedInToDialerState = 0/*NOT_LOGGED_IN*/ AND @NewSurveyDialMode = 1 /*Manual*/)
    )

SELECT * FROM @result
GO

PRINT N'Update complete.';
GO
