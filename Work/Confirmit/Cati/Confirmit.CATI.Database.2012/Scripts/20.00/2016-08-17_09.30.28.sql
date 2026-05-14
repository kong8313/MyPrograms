GO

CREATE PROCEDURE [dbo].[BvSpSvyShedule_DeleteCallsByBlacklist]
    @phoneNumber VARCHAR(255)
AS 
CREATE TABLE #UpdatedRows(interviewId INT, surveySID INT)

UPDATE ss SET CallState = 0/*ToBeDeleted*/ OUTPUT inserted.InterviewID, inserted.SurveySID  INTO #UpdatedRows FROM BvSvySchedule ss 
	INNER JOIN BvInterview i ON ss.InterviewID = i.ID AND i.SurveySID = ss.SurveySID
	INNER JOIN BvSurvey s ON i.SurveySID = s.SID
	WHERE s.IsTelephoneBlacklistSupported = '1' AND i.TelephoneNumber = @phoneNumber AND ss.CallState IN (1, 2)

UPDATE i SET TransientState = 17/*Blacklisted*/ FROM BvInterview i INNER JOIN #UpdatedRows ur ON i.ID = ur.interviewId AND i.SurveySID = ur.surveySID

GO