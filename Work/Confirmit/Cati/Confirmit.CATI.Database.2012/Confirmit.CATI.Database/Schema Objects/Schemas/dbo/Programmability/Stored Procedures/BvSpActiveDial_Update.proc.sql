CREATE PROCEDURE [dbo].[BvSpActiveDial_Update]
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
 @DialerCallerId NVARCHAR(255) = NULL,
 @DialerCallOutcome INT = NULL,
 @UpdateActiveDialInBvSvySchedule BIT = 1

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
			DialerCallerId = @DialerCallerId,
			DialerCallOutcome = @DialerCallOutcome
		WHERE Id = @Id

	IF ISNULL( @OldCallId, 0 ) <> ISNULL( @CallId, 0 ) AND @UpdateActiveDialInBvSvySchedule = 1
	BEGIN
		IF @OldCallId IS NOT NULL
			UPDATE BvSvySchedule SET DialerId = 0, ActiveDialId = 0 WHERE ID = @OldCallId

		IF @CallId IS NOT NULL
			UPDATE BvSvySchedule SET DialerId = @DialerId, ActiveDialId = @Id WHERE ID = @CallId
	END
		
GO
