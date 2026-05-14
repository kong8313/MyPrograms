CREATE PROCEDURE  [dbo].[BvSpSendMessageToSurveys]
	@BatchId int,	
    @MessageBody nvarchar(1024),
	@MessageSupervisorName nvarchar(50),
	@CallCenterID INT
AS

BEGIN
	
	IF @BatchId IS NULL
	BEGIN
		SELECT 0 AS InterviewerId		
		RETURN 0;
	END

	DECLARE @MessageId int
	INSERT INTO BvMessages (Body, CreateTime, SupervisorName) VALUES(@MessageBody, GETUTCDATE(), @MessageSupervisorName);
	SET @MessageId = SCOPE_IDENTITY();

	/* Survey group contains all interviewer working on survey*/
	BEGIN TRANSACTION
			DECLARE @MessageToPerson TABLE( MessageId INT, InterviewerId INT )

			UPDATE BvPerson 
				SET HasNewMessage = 1
			OUTPUT @MessageId, inserted.SID INTO @MessageToPerson (MessageId, InterviewerId)
			FROM											
				BvPerson as p
				INNER JOIN  bvTasks as t ON p.SID = t.PersonSID
				INNER JOIN 	bvTransferArrays a ON t.SurveySID = a.ItemId 
			WHERE p.CallCenterID = @CallCenterID AND a.BatchId = @BatchId

			INSERT INTO BvMessageToPerson(MessageId, InterviewerId) SELECT MessageId, InterviewerId FROM @MessageToPerson

	Select InterviewerId from @MessageToPerson

	COMMIT TRANSACTION
	
END
