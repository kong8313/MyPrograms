CREATE FUNCTION [dbo].[GetLastTimeBreak]
(
	@personId INT
)
RETURNS TABLE AS RETURN 
(
	SELECT TOP(1) *
	FROM BvTimeBreaksHistory
	WHERE InterviewerId = @personId
	ORDER BY StartTime DESC
)