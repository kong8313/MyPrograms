CREATE  PROCEDURE [dbo].[BvSpStartInterviewerBreak]
    @InterviewerId INT,
	@SurveyId INT,
	@BreakTypeId INT
AS
BEGIN
	DECLARE @CallCenterId INT = (SELECT CallCenterID FROM BvPerson WHERE SID = @InterviewerId)
	INSERT INTO BvTimeBreaksHistory (InterviewerId, StartTime, CallCenterId, SurveyId, BreakTypeId) VALUES (@InterviewerId, GETUTCDATE(), @CallCenterId, @SurveyId, @BreakTypeId)
END