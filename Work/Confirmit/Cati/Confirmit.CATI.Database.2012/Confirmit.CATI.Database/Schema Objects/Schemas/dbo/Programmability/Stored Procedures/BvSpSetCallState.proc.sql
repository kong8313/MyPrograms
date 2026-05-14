CREATE PROCEDURE [dbo].[BvSpSetCallState]
	@SurveySID		INT,
	@InterviewID	INT,
	@state			INT
AS

UPDATE BvSvySchedule
SET CallState = @state
WHERE InterviewID = @InterviewID AND 
	  SurveySID = @SurveySID
	  
