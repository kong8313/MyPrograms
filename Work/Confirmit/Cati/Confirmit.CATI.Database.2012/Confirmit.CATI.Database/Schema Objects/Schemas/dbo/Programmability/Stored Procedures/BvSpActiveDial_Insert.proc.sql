CREATE PROCEDURE [dbo].[BvSpActiveDial_Insert]
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
 @DialerCallerId NVARCHAR(255) = NULL,
 @DialerCallOutcome INT = NULL,
 @UpdateActiveDialInBvSvySchedule BIT = 1
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
	
	IF @CallId IS NOT NULL AND @UpdateActiveDialInBvSvySchedule = 1
	BEGIN
		UPDATE BvSvySchedule SET ActiveDialId = @ID, DialerId = @DialerId WHERE ID = @CallId
	END

	INSERT INTO [dbo].[BvActiveDial]( [Id] ,[Type] ,[DialerId] ,[DialerTelephoneNumber] ,[RespondentTelephoneNumber] ,[StartTime] ,[State], InboundCallId, InitialSurveyId, SurveyId, CampaignId, InterviewId, CallId, MainPersonId, JsonCallOutcomeMetadata, RingTime, DialerCallerId, DialerCallOutcome)
		OUTPUT inserted.*
		VALUES( @ID ,@Type, @DialerId, @DialerTelephoneNumber, @RespondentTelephoneNumber, [dbo].GetUtcNow(), @State, @InboundCallId, @InitialSurveyId, @SurveyId, @CampaignId, @InterviewId, @CallId, @MainPersonId, @JsonCallOutcomeMetadata, @RingTime, @DialerCallerId, @DialerCallOutcome)
		
GO
