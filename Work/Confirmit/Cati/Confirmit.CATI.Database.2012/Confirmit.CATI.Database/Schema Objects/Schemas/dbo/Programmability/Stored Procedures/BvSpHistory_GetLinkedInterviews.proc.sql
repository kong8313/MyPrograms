CREATE PROCEDURE [BvSpHistory_GetLinkedInterviews]
	@LinkedInterviewSessionId INT
AS

	SELECT 
	    ROW_NUMBER()  OVER(ORDER BY h.ID) AS InterviewsOrder,
		h.SurveyId	AS SurveyId,
		s.Name		AS ProjectId, 
		s.[Description] AS SurveyName,
		h.InterviewId	AS InterviewId,
		@LinkedInterviewSessionId AS LinkedInterviewSessionId
	FROM BvHistory h
	JOIN BvSurvey s
		ON h.SurveyId = s.SID
	WHERE LinkedInterviewSessionId = @LinkedInterviewSessionId
	ORDER BY h.Id
	
RETURN 0
