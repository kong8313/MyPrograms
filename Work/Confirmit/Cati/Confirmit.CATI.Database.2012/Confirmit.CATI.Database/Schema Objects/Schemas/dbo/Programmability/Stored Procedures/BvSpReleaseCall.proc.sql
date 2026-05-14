CREATE PROCEDURE [dbo].[BvSpReleaseCall]
	@SurveySID		INT,
	@InterviewID	INT
AS

UPDATE BvSvySchedule
SET CallState = 2
WHERE InterviewID = @InterviewID AND 
	  SurveySID = @SurveySID AND
	  CallState <> 0
