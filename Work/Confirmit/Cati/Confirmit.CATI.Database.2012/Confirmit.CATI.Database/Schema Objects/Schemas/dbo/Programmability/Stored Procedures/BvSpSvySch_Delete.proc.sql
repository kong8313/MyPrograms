CREATE PROCEDURE [dbo].[BvSpSvySch_Delete]
@SurveySID      INTEGER,
@InterviewID    INTEGER
AS
	-- delete calls
	UPDATE BvSvySchedule 
	SET CallState = 0
	WHERE SurveySID = @SurveySID AND
			InterviewID = @InterviewID

	UPDATE BvAppointment
	SET STATE = 2
	WHERE SurveySID = @SurveySID AND
		InterviewSID = @InterviewID

RETURN (0)