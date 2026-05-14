CREATE PROCEDURE [dbo].[BvSpAsyncOperationQueue_AppendText]
    @Id INT,
	@Text NVARCHAR(MAX)
AS
    UPDATE BvAsyncOperationQueue SET [Text] = [Text] + @Text, HeartBeat = GETUTCDATE() WHERE Id = @Id
RETURN
GO