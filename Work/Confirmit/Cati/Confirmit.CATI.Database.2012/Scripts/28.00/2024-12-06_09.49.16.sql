GO
PRINT N'Creating Procedure [dbo].[BvSpActiveDial_DeleteSingle]...';


GO
CREATE PROCEDURE [dbo].[BvSpActiveDial_DeleteSingle]
 @ID BigInt,
 @CallCompleteStatus TINYINT
AS

DECLARE @Values TABLE( Id BIGINT, Type TINYINT, DialerId INT, DialerTelephoneNumber NVARCHAR(MAX), RespondentTelephoneNumber NVARCHAR(MAX), StartTime DATETIME, AnswerTime DATETIME, InboundCallId NVARCHAR(MAX), InitialSurveyId INT, CallId INT)

DELETE FROM BvActiveDial 
	OUTPUT	deleted.Id, deleted.Type, deleted.DialerId, deleted.DialerTelephoneNumber, deleted.RespondentTelephoneNumber,
			deleted.StartTime, deleted.AnswerTime, deleted.InboundCallId, deleted.InitialSurveyId, deleted.CallId INTO @Values
		WHERE ID = @ID

DECLARE @Now DATETIME = [dbo].GetUtcNow()

INSERT INTO BvDialHistory( Id, Type, DialerId, DialerTelephoneNumber, RespondentTelephoneNumber, InboundCallId, InitialSurveyId, CallCompleteStatus, StartTime, AnswerTime, FinishTime )
	SELECT Id, Type, DialerId, DialerTelephoneNumber, RespondentTelephoneNumber, InboundCallId, InitialSurveyId, @CallCompleteStatus, StartTime, AnswerTime, @Now
		FROM @Values
	
DECLARE @CallId INT
SELECT @CallId = CallId from @values

--get primary key in separate operation to avoid deadlock on BvSvySchedule table
DECLARE @SurveyId INT = -1
DECLARE @InterviewId INT = -1
SELECT @SurveyId = SurveySID, @InterviewId = InterviewID FROM BvSvySchedule WHERE ID = @CallId

UPDATE BvSvySchedule SET DialerId = 0, ActiveDialId = 0 WHERE SurveySID = @SurveyId AND InterviewID = @InterviewId
GO
PRINT N'Altering Procedure [dbo].[BvSpActiveDial_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpActiveDial_Delete]
 @IDs BvBigIntArrayType READONLY,
 @CallCompleteStatus TINYINT
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

	DECLARE @Values TABLE( Id BIGINT, Type TINYINT, DialerId INT, DialerTelephoneNumber NVARCHAR(MAX), RespondentTelephoneNumber NVARCHAR(MAX), StartTime DATETIME, AnswerTime DATETIME, InboundCallId NVARCHAR(MAX), InitialSurveyId INT, CallId INT)

	DELETE FROM BvActiveDial 
		OUTPUT	deleted.Id, deleted.Type, deleted.DialerId, deleted.DialerTelephoneNumber, deleted.RespondentTelephoneNumber,
				deleted.StartTime, deleted.AnswerTime, deleted.InboundCallId, deleted.InitialSurveyId, deleted.CallId INTO @Values
			WHERE ID IN ( SELECT Value FROM @IDs)
	
	DECLARE @Now DATETIME = [dbo].GetUtcNow()
	
	INSERT INTO BvDialHistory( Id, Type, DialerId, DialerTelephoneNumber, RespondentTelephoneNumber, InboundCallId, InitialSurveyId, CallCompleteStatus, StartTime, AnswerTime, FinishTime )
		SELECT Id, Type, DialerId, DialerTelephoneNumber, RespondentTelephoneNumber, InboundCallId, InitialSurveyId, @CallCompleteStatus, StartTime, AnswerTime, @Now
			FROM @Values

	DECLARE @Calls TABLE( SurveySID int, InterviewID int)
	 
	INSERT INTO @Calls(SurveySID, InterviewID)--avoid deadlock on BvSvySchedule table
		SELECT SurveySID, InterviewID FROM BvSvySchedule c INNER JOIN @Values v ON c.ID = v.CallId

	UPDATE BvSvySchedule SET DialerId = 0, ActiveDialId = 0 FROM BvSvySchedule c INNER JOIN @Calls v ON c.SurveySID = v.SurveySID AND c.InterviewID = v.InterviewID

COMMIT TRAN
END
GO
PRINT N'Refreshing Procedure [dbo].[BvSpActiveDial_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpActiveDial_Insert]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpActiveDial_InsertOutboundBatch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpActiveDial_InsertOutboundBatch]';


GO
PRINT N'Update complete.';


GO
