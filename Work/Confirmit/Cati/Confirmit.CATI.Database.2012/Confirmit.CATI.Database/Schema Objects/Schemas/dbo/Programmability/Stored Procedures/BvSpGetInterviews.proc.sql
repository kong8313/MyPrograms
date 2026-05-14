CREATE PROCEDURE [dbo].[BvSpGetInterviews]
@interviews BvInterviewTypeOrdered READONLY
as
BEGIN
		SELECT s.Name AS ProjectId, i.IID AS InterviewId, bvi.RespondentName, bvi.TelephoneNumber, CAST( null AS nvarchar(MAX)) AS Filters
		FROM @interviews i
		JOIN BvInterview bvi
			ON i.SurveySid = bvi.SurveySID AND i.IID = bvi.ID
		JOIN BvSurvey s
			ON i.SurveySid = s.SID
		ORDER BY i.OrderId
END
RETURN(0)