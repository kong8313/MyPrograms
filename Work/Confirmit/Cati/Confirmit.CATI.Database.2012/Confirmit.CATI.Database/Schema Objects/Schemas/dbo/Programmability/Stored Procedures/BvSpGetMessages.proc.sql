CREATE PROCEDURE BvSpGetMessages 
	@InterviewerId INT
AS
BEGIN

   BEGIN TRANSACTION

        UPDATE bvPerson SET HasNewMessage = 0 WHERE SID = @InterviewerId

		DELETE bvMessageToPerson 
			OUTPUT bvMessages.Body, bvMessages.CreateTime, bvMessages.SupervisorName
			FROM bvMessages INNER JOIN bvMessageToPerson 
			ON MessageId = bvMessages.Id And InterviewerId = @InterviewerId		

	COMMIT TRANSACTION
	
END