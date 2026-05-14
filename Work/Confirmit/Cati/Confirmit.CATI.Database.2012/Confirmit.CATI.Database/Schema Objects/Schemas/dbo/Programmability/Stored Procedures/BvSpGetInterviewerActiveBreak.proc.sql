CREATE PROCEDURE [dbo].[BvSpGetInterviewerActiveBreak]
	@personId INT
AS
    WITH LastBreak AS
    (
		SELECT TOP(1) ID, StartTime, InterviewerId, Duration
		FROM BvTimeBreaksHistory
		WHERE InterviewerId = @personId
		ORDER BY StartTime DESC
	)
	SELECT ID, StartTime, InterviewerId
	FROM LastBreak
	WHERE Duration IS NULL
RETURN 0