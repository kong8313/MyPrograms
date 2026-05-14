CREATE PROCEDURE [dbo].[BvSpCleanMessages]
@ExpirationPeriod INT
AS
BEGIN

	DELETE from bvMessages
	WHERE DateAdd(day, @ExpirationPeriod, bvMessages.CreateTime) < GETUTCDATE();
 
	WITH LastMessagesInChat AS (
		SELECT [ConversationId], MAX([Date]) as [Date]
		FROM BvConversationMessages
        GROUP BY [ConversationId])
		
	DELETE 
	FROM BvConversations 
	WHERE Id IN (									 
		SELECT ConversationId
		FROM LastMessagesInChat
		WHERE DateAdd(day, @ExpirationPeriod, [Date]) < GETUTCDATE());

END
