CREATE PROCEDURE [dbo].[BvSpSendMessageToInterviewers]
	
	@BatchId int,	
	@OnlineOnly bit,
    @MessageBody nvarchar(1024),	
	@MessageSupervisorName nvarchar(50)    
AS

BEGIN

	DECLARE @MessageId int
	INSERT INTO BvMessages (Body, CreateTime, SupervisorName) VALUES(@MessageBody, GETUTCDATE(), @MessageSupervisorName);
	SET @MessageId = SCOPE_IDENTITY();

	DECLARE @MessageToPerson TABLE( MessageId INT, InterviewerId INT )

	UPDATE BvPerson SET HasNewMessage = 1 
		OUTPUT @MessageId, inserted.SID INTO @MessageToPerson (MessageId, InterviewerId) 
		FROM BvPerson p
		LEFT JOIN BvTasks t ON p.SID = t.PersonSID
		INNER JOIN bvTransferArrays ON (p.[SID] = ItemId AND BatchId = @BatchId)
		WHERE t.PersonSID IS NOT NULL OR @OnlineOnly <> 1

	INSERT INTO BvMessageToPerson (MessageId, InterviewerId)  SELECT  MessageId, InterviewerId FROM @MessageToPerson
END
