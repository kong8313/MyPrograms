DECLARE conversations_cursor CURSOR FOR 
SELECT [conversation_handle] FROM sys.conversation_endpoints
OPEN conversations_cursor;

DECLARE @conversation UNIQUEIDENTIFIER;

FETCH NEXT FROM conversations_cursor 
INTO @conversation;

WHILE @@FETCH_STATUS = 0
BEGIN
    END CONVERSATION @conversation WITH CLEANUP
    FETCH NEXT FROM conversations_cursor INTO @conversation;
END

CLOSE conversations_cursor;
DEALLOCATE conversations_cursor;
