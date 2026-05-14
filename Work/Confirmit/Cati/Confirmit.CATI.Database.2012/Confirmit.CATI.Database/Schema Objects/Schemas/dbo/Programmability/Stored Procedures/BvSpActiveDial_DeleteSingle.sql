CREATE PROCEDURE [dbo].[BvSpActiveDial_DeleteSingle]
 @ID BigInt,
 @CallCompleteStatus TINYINT,
 @JsonCallOutcomeMetadata NVARCHAR(MAX) = NULL, 
 @RingTime INT = NULL, 
 @DialerCallerId NVARCHAR(255) = NULL,
 @DialerCallOutcome INT = NULL,
 @UpdateActiveDialInBvSvySchedule BIT = 1
AS

DECLARE @Values TABLE( Id BIGINT, Type TINYINT, DialerId INT, DialerTelephoneNumber NVARCHAR(MAX), RespondentTelephoneNumber NVARCHAR(MAX), StartTime DATETIME, AnswerTime DATETIME, InboundCallId NVARCHAR(MAX), InitialSurveyId INT, CallId INT, JsonCallOutcomeMetadata NVARCHAR(MAX), RingTime INT, DialerCallerId NVARCHAR(255), DialerCallOutcome INT)

DELETE FROM BvActiveDial 
	OUTPUT	deleted.Id, deleted.Type, deleted.DialerId, deleted.DialerTelephoneNumber, deleted.RespondentTelephoneNumber,
			deleted.StartTime, deleted.AnswerTime, deleted.InboundCallId, deleted.InitialSurveyId, deleted.CallId, deleted.JsonCallOutcomeMetadata, deleted.RingTime, deleted.DialerCallerId, deleted.DialerCallOutcome INTO @Values
		WHERE ID = @ID

DECLARE @Now DATETIME = [dbo].GetUtcNow()

INSERT INTO BvDialHistory( Id, Type, DialerId, DialerTelephoneNumber, RespondentTelephoneNumber, InboundCallId, InitialSurveyId, CallCompleteStatus, StartTime, AnswerTime, FinishTime, JsonCallOutcomeMetadata, RingTime, DialerCallerId, DialerCallOutcome)
	SELECT Id, Type, DialerId, DialerTelephoneNumber, RespondentTelephoneNumber, InboundCallId, InitialSurveyId, @CallCompleteStatus, StartTime, AnswerTime, @Now, ISNULL(@JsonCallOutcomeMetadata, JsonCallOutcomeMetadata), ISNULL(@RingTime, RingTime), ISNULL(@DialerCallerId, DialerCallerId), ISNULL(@DialerCallOutcome, DialerCallOutcome)
		FROM @Values
	
DECLARE @CallId INT
SELECT @CallId = CallId from @values

--get primary key in separate operation to avoid deadlock on BvSvySchedule table
DECLARE @SurveyId INT = -1
DECLARE @InterviewId INT = -1
SELECT @SurveyId = SurveySID, @InterviewId = InterviewID FROM BvSvySchedule WHERE ID = @CallId

IF @UpdateActiveDialInBvSvySchedule = 1
BEGIN
	UPDATE BvSvySchedule SET DialerId = 0, ActiveDialId = 0 WHERE SurveySID = @SurveyId AND InterviewID = @InterviewId
END