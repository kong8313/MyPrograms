
GO
PRINT N'Altering Table [dbo].[BvActiveDial]...';


GO
ALTER TABLE [dbo].[BvActiveDial]
    ADD [JsonCallOutcomeMetadata] NVARCHAR (MAX) NULL,
		[RingTime]       INT            NULL,
        [DialerCallerId] NVARCHAR (255) NULL;


GO
PRINT N'Altering Table [dbo].[BvDialHistory]...';

GO
ALTER TABLE [dbo].[BvDialHistory]
    ADD [JsonCallOutcomeMetadata] NVARCHAR (MAX) NULL,
        [RingTime]                INT            NULL,
        [DialerCallerId]          NVARCHAR (255) NULL;


GO
PRINT N'Altering Procedure [dbo].[BvSpActiveDial_DeleteSingle]...';


GO
ALTER PROCEDURE [dbo].[BvSpActiveDial_DeleteSingle]
 @ID BigInt,
 @CallCompleteStatus TINYINT
AS

DECLARE @Values TABLE( Id BIGINT, Type TINYINT, DialerId INT, DialerTelephoneNumber NVARCHAR(MAX), RespondentTelephoneNumber NVARCHAR(MAX), StartTime DATETIME, AnswerTime DATETIME, InboundCallId NVARCHAR(MAX), InitialSurveyId INT, CallId INT, JsonCallOutcomeMetadata NVARCHAR(MAX), RingTime INT, DialerCallerId NVARCHAR(255))

DELETE FROM BvActiveDial 
	OUTPUT	deleted.Id, deleted.Type, deleted.DialerId, deleted.DialerTelephoneNumber, deleted.RespondentTelephoneNumber,
			deleted.StartTime, deleted.AnswerTime, deleted.InboundCallId, deleted.InitialSurveyId, deleted.CallId, deleted.JsonCallOutcomeMetadata, deleted.RingTime, deleted.DialerCallerId INTO @Values
		WHERE ID = @ID

DECLARE @Now DATETIME = [dbo].GetUtcNow()

INSERT INTO BvDialHistory( Id, Type, DialerId, DialerTelephoneNumber, RespondentTelephoneNumber, InboundCallId, InitialSurveyId, CallCompleteStatus, StartTime, AnswerTime, FinishTime, JsonCallOutcomeMetadata, RingTime, DialerCallerId )
	SELECT Id, Type, DialerId, DialerTelephoneNumber, RespondentTelephoneNumber, InboundCallId, InitialSurveyId, @CallCompleteStatus, StartTime, AnswerTime, @Now, JsonCallOutcomeMetadata, RingTime, DialerCallerId
		FROM @Values
	
DECLARE @CallId INT
SELECT @CallId = CallId from @values

--get primary key in separate operation to avoid deadlock on BvSvySchedule table
DECLARE @SurveyId INT = -1
DECLARE @InterviewId INT = -1
SELECT @SurveyId = SurveySID, @InterviewId = InterviewID FROM BvSvySchedule WHERE ID = @CallId

UPDATE BvSvySchedule SET DialerId = 0, ActiveDialId = 0 WHERE SurveySID = @SurveyId AND InterviewID = @InterviewId
GO
PRINT N'Altering Procedure [dbo].[BvSpActiveDial_Update]...';


GO
ALTER PROCEDURE [dbo].[BvSpActiveDial_Update]
 @Id BIGINT,
 @Type TINYINT,
 @State TINYINT,
 @AnswerTime DATETIME,
 @TransferId NVARCHAR(MAX),
 @SurveyId INT,
 @CampaignId BIGINT,
 @InterviewId INT,
 @CallId INT,
 @MainPersonId INT,
 @JsonTransferState NVARCHAR(MAX),
 @TransferType TINYINT,
 @JsonCallOutcomeMetadata NVARCHAR(MAX) = NULL, 
 @RingTime INT = NULL, 
 @DialerCallerId NVARCHAR(255) = NULL

AS
	DECLARE @OldCallId INT
	DECLARE @DialerId INT
	
	UPDATE BvActiveDial
		SET @OldCallId = CallId,
		    @DialerId = DialerId,
			Type = @Type,
			State = @State,
			AnswerTime = @AnswerTime,
			TransferId = @TransferId,
			SurveyId = @SurveyId,
			CampaignId = @CampaignId,
			InterviewId = @InterviewId,
			CallId = @CallId,
			MainPersonId = @MainPersonId,
			JsonTransferState = @JsonTransferState,
			TransferType = @TransferType,
			JsonCallOutcomeMetadata = @JsonCallOutcomeMetadata,
			RingTime = @RingTime,
			DialerCallerId = @DialerCallerId
		WHERE Id = @Id

	IF ISNULL( @OldCallId, 0 ) <> ISNULL( @CallId, 0 )
	BEGIN
		IF @OldCallId IS NOT NULL
			UPDATE BvSvySchedule SET DialerId = 0, ActiveDialId = 0 WHERE ID = @OldCallId

		IF @CallId IS NOT NULL
			UPDATE BvSvySchedule SET DialerId = @DialerId, ActiveDialId = @Id WHERE ID = @CallId
	END
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

	DECLARE @Values TABLE( Id BIGINT, Type TINYINT, DialerId INT, DialerTelephoneNumber NVARCHAR(MAX), RespondentTelephoneNumber NVARCHAR(MAX), StartTime DATETIME, AnswerTime DATETIME, InboundCallId NVARCHAR(MAX), InitialSurveyId INT, CallId INT, JsonCallOutcomeMetadata NVARCHAR(MAX), RingTime INT, DialerCallerId NVARCHAR(255))

	DELETE FROM BvActiveDial 
		OUTPUT	deleted.Id, deleted.Type, deleted.DialerId, deleted.DialerTelephoneNumber, deleted.RespondentTelephoneNumber,
				deleted.StartTime, deleted.AnswerTime, deleted.InboundCallId, deleted.InitialSurveyId, deleted.CallId, deleted.JsonCallOutcomeMetadata, deleted.RingTime, deleted.DialerCallerId INTO @Values
			WHERE ID IN ( SELECT Value FROM @IDs)
	
	DECLARE @Now DATETIME = [dbo].GetUtcNow()
	
	INSERT INTO BvDialHistory( Id, Type, DialerId, DialerTelephoneNumber, RespondentTelephoneNumber, InboundCallId, InitialSurveyId, CallCompleteStatus, StartTime, AnswerTime, FinishTime, JsonCallOutcomeMetadata, RingTime, DialerCallerId )
		SELECT Id, Type, DialerId, DialerTelephoneNumber, RespondentTelephoneNumber, InboundCallId, InitialSurveyId, @CallCompleteStatus, StartTime, AnswerTime, @Now, JsonCallOutcomeMetadata, RingTime, DialerCallerId
			FROM @Values

	DECLARE @Calls TABLE( SurveySID int, InterviewID int)
	 
	INSERT INTO @Calls(SurveySID, InterviewID)--avoid deadlock on BvSvySchedule table
		SELECT SurveySID, InterviewID FROM BvSvySchedule c INNER JOIN @Values v ON c.ID = v.CallId

	UPDATE BvSvySchedule SET DialerId = 0, ActiveDialId = 0 FROM BvSvySchedule c INNER JOIN @Calls v ON c.SurveySID = v.SurveySID AND c.InterviewID = v.InterviewID

COMMIT TRAN
END
GO
PRINT N'Altering Procedure [dbo].[BvSpActiveDial_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpActiveDial_Insert]
 @Type TINYINT,
 @DialerId INT,
 @DialerTelephoneNumber NVARCHAR(MAX),
 @RespondentTelephoneNumber NVARCHAR(MAX),
 @State TINYINT,
 @InboundCallId NVARCHAR(MAX),
 @InitialSurveyId INT,
 @SurveyId INT,
 @CampaignId BIGINT,
 @InterviewId INT,
 @CallId INT,
 @MainPersonId INT,
 @JsonCallOutcomeMetadata NVARCHAR(MAX) = NULL, 
 @RingTime INT = NULL, 
 @DialerCallerId NVARCHAR(255) = NULL
AS
	DECLARE @OldIds BvBigIntArrayType 

	IF @Type = 1/*Inbound*/ 
		INSERT INTO @OldIds SELECT ID FROM BvActiveDial WHERE InboundCallId = @InboundCallId
	ELSE IF @Type = 0/*Outbound*/
	    INSERT INTO @OldIds SELECT ID FROM BvActiveDial WHERE CallId = @CallId
	
	IF @@ROWCOUNT <> 0
	BEGIN
		EXEC BvSpActiveDial_Delete @OldIds, 0/*CallCompleteStatus.Error*/
	END

	DECLARE @ID BIGINT = NEXT VALUE FOR [dbo].[BvDialIdSequence];
	
	IF @CallId IS NOT NULL
	BEGIN
		UPDATE BvSvySchedule SET ActiveDialId = @ID, DialerId = @DialerId WHERE ID = @CallId
	END

	INSERT INTO [dbo].[BvActiveDial]( [Id] ,[Type] ,[DialerId] ,[DialerTelephoneNumber] ,[RespondentTelephoneNumber] ,[StartTime] ,[State], InboundCallId, InitialSurveyId, SurveyId, CampaignId, InterviewId, CallId, MainPersonId, JsonCallOutcomeMetadata, RingTime, DialerCallerId)
		OUTPUT inserted.*
		VALUES( @ID ,@Type, @DialerId, @DialerTelephoneNumber, @RespondentTelephoneNumber, [dbo].GetUtcNow(), @State, @InboundCallId, @InitialSurveyId, @SurveyId, @CampaignId, @InterviewId, @CallId, @MainPersonId, @JsonCallOutcomeMetadata, @RingTime, @DialerCallerId)
GO
PRINT N'Refreshing Procedure [dbo].[BvSpReportInboundCalls]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpReportInboundCalls]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpActiveDial_InsertOutboundBatch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpActiveDial_InsertOutboundBatch]';

GO
PRINT N'Update complete.';


GO
