GO
PRINT N'Altering [dbo].[BvSpActiveDial_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpActiveDial_Insert]

 @Type TINYINT,
 @DialerId INT,
 @DdiNumber NVARCHAR(MAX),
 @TelephoneNumber NVARCHAR(MAX),
 @State TINYINT,
 @InboundCallId NVARCHAR(MAX),
 @InitialSurveyId INT
AS
	DECLARE @OldIds BvIntArrayType 

	INSERT INTO @OldIds SELECT ID FROM BvActiveDial WHERE InboundCallId = @InboundCallId 
	
	IF @@ROWCOUNT <> 0
	BEGIN
		EXEC BvSpActiveDial_Delete @OldIds, 0/*CallCompleteStatus.Error*/
	END

	INSERT INTO [dbo].[BvActiveDial]( [Id] 
			,[Type] ,[DialerId] ,[DdiNumber] ,[TelephoneNumber] ,[StartTime] ,[State], InboundCallId, InitialSurveyId)
		OUTPUT inserted.*
		VALUES( NEXT VALUE FOR [dbo].[BvDialIdSequence]
			,@Type, @DialerId, @DdiNumber, @TelephoneNumber, [dbo].GetUtcNow(), @State, @InboundCallId, @InitialSurveyId)
GO
PRINT N'Update complete.';


GO
