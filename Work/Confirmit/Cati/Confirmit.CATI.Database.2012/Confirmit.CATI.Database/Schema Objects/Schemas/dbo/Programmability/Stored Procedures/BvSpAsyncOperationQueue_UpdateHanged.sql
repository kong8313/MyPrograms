CREATE PROCEDURE [dbo].[BvSpAsyncOperationQueue_UpdateHanged]
	@ExecutingStateValue TINYINT  /*AsyncOperationState.Executing passed from C# to avoid copy paste*/,
	@HangedStateValue TINYINT  /*AsyncOperationState.Hanged passed from C# to avoid copy paste*/,
	@TimeToTreatOperationHangedInMinutes INT
AS
	CREATE TABLE #HangedTaskIds
	(
		Id INT NOT NULL PRIMARY KEY
	)

	INSERT INTO #HangedTaskIds SELECT Id FROM BvAsyncOperationQueue
    WHERE
	    [State] = @ExecutingStateValue AND 
		DATEDIFF(minute, HeartBeat, GETUTCDATE()) >= @TimeToTreatOperationHangedInMinutes

	IF @@ROWCOUNT > 0 
	BEGIN
		UPDATE
			BvAsyncOperationQueue
		SET 
			[State] = @HangedStateValue
		FROM #HangedTaskIds 
		WHERE BvAsyncOperationQueue.Id = #HangedTaskIds.Id AND [State] = @ExecutingStateValue AND 
		DATEDIFF(minute, HeartBeat, GETUTCDATE()) >= @TimeToTreatOperationHangedInMinutes
	END

RETURN 0
