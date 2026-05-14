CREATE PROCEDURE [dbo].[BvSpActiveDial_Delete]
 @IDs BvBigIntArrayType READONLY,
 @CallCompleteStatus TINYINT,
 @UpdateActiveDialInBvSvySchedule BIT = 1
AS

IF (SELECT COUNT(*) FROM  @IDs) = 1--optimise query for single id
BEGIN
	DECLARE @ID BigInt
	SELECT @ID = Value FROM @IDs
	EXEC BvSpActiveDial_DeleteSingle @ID, @CallCompleteStatus
END
ELSE
BEGIN
BEGIN TRAN

	DECLARE @Values TABLE( Id BIGINT, Type TINYINT, DialerId INT, DialerTelephoneNumber NVARCHAR(MAX), RespondentTelephoneNumber NVARCHAR(MAX), StartTime DATETIME, AnswerTime DATETIME, InboundCallId NVARCHAR(MAX), InitialSurveyId INT, CallId INT, JsonCallOutcomeMetadata NVARCHAR(MAX), RingTime INT, DialerCallerId NVARCHAR(255))

	DELETE FROM BvActiveDial 
		OUTPUT	deleted.Id, deleted.Type, deleted.DialerId, deleted.DialerTelephoneNumber, deleted.RespondentTelephoneNumber,
				deleted.StartTime, deleted.AnswerTime, deleted.InboundCallId, deleted.InitialSurveyId, deleted.CallId, deleted.JsonCallOutcomeMetadata, deleted.RingTime, deleted.DialerCallerId INTO @Values
			WHERE ID IN ( SELECT Value FROM @IDs)
	
	DECLARE @Now DATETIME = [dbo].GetUtcNow()
	
	INSERT INTO BvDialHistory( Id, Type, DialerId, DialerTelephoneNumber, RespondentTelephoneNumber, InboundCallId, InitialSurveyId, CallCompleteStatus, StartTime, AnswerTime, FinishTime, JsonCallOutcomeMetadata, RingTime, DialerCallerId, DialerCallOutcome)
		SELECT Id, Type, DialerId, DialerTelephoneNumber, RespondentTelephoneNumber, InboundCallId, InitialSurveyId, @CallCompleteStatus, StartTime, AnswerTime, @Now, JsonCallOutcomeMetadata, RingTime, DialerCallerId, 0 /*connected calloutcome*/
			FROM @Values

	DECLARE @Calls TABLE( SurveySID int, InterviewID int)
	 
	INSERT INTO @Calls(SurveySID, InterviewID)--avoid deadlock on BvSvySchedule table
		SELECT SurveySID, InterviewID FROM BvSvySchedule c INNER JOIN @Values v ON c.ID = v.CallId

	IF @UpdateActiveDialInBvSvySchedule = 1
	BEGIN
		UPDATE BvSvySchedule SET DialerId = 0, ActiveDialId = 0 FROM BvSvySchedule c INNER JOIN @Calls v ON c.SurveySID = v.SurveySID AND c.InterviewID = v.InterviewID
	END
COMMIT TRAN
END
GO

