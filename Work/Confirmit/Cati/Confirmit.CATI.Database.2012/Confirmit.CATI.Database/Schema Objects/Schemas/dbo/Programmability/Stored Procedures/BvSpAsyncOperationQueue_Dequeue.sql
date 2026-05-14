CREATE PROCEDURE [dbo].[BvSpAsyncOperationQueue_Dequeue]
    @OperationsLimit INT,
	@QueueuedStateValue TINYINT, /*AsyncOperationState.Queued passed from C# to avoid copy paste*/
	@ExecutingStateValue TINYINT  /*AsyncOperationState.Executing passed from C# to avoid copy paste*/
AS
	DECLARE @executingAtTheMomentOperations INT;
	SELECT @executingAtTheMomentOperations = COUNT(*) FROM BvAsyncOperationQueue WHERE [State]=@ExecutingStateValue

	IF @executingAtTheMomentOperations < @OperationsLimit
	BEGIN
	    SELECT TOP(1)
		    Id
		FROM
		    BvAsyncOperationQueue
		WHERE
		    [State] = @QueueuedStateValue AND [SurveySid] NOT IN (SELECT SurveySid FROM BvAsyncOperationQueue WHERE [State]=@ExecutingStateValue)
		ORDER BY [Priority], [ID]
	END

RETURN
GO
