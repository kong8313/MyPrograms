CREATE PROCEDURE [dbo].[BvSpFinishInterviewerBreak]
	@InterviewerId INT    	
AS
   DECLARE @currTime DATETIME
   EXEC @currTime = GetUtcNow

    ;WITH TimeBreaksHistory AS
    (
       SELECT TOP(1) *
       FROM BvTimeBreaksHistory
       WHERE InterviewerId = @InterviewerId
       ORDER BY StartTime DESC
    )
	UPDATE TimeBreaksHistory 
	SET Duration = DATEDIFF(second, StartTime, @currTime)
	WHERE Duration IS NULL
	;
    UPDATE BvTasks
    SET StartTime = @currTime,
	BreakTypeId = NULL
    WHERE PersonSID = @InterviewerId

RETURN 0