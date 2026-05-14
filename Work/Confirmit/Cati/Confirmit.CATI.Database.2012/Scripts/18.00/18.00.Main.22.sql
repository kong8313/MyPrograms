PRINT N'Altering [dbo].[BvSpAsyncOperationQueue_Dequeue]...';

GO
ALTER PROCEDURE [dbo].[BvSpAsyncOperationQueue_Dequeue]
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

PRINT N'Creating [dbo].[BvSpAsyncOperationQueue_UpdateHanged]...';


GO
CREATE PROCEDURE [dbo].[BvSpAsyncOperationQueue_UpdateHanged]
	@ExecutingStateValue TINYINT  /*AsyncOperationState.Executing passed from C# to avoid copy paste*/,
	@HangedStateValue TINYINT  /*AsyncOperationState.Hanged passed from C# to avoid copy paste*/,
	@TimeToTreatOperationHangedInMinutes INT
AS
	UPDATE
	    BvAsyncOperationQueue
    SET 
	    [State] = @HangedStateValue
    WHERE
	    [State] = @ExecutingStateValue AND 
		DATEDIFF(minute, HeartBeat, GETUTCDATE()) >= @TimeToTreatOperationHangedInMinutes

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpAsyncOperationQueue_UpdateHeartBeat]...';


GO
CREATE PROCEDURE [dbo].[BvSpAsyncOperationQueue_UpdateHeartBeat]
	@Id INT
AS
    UPDATE BvAsyncOperationQueue SET HeartBeat = GETUTCDATE() WHERE Id = @Id
RETURN 0
GO

;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
(
SELECT 'AsyncOperations.MaximumRunningAsyncOperations', 'MaximumRunningAsyncOperations', 'AsyncOperations', 'Maximum running async operations.', 1, 0, '5'
UNION ALL
SELECT 'AsyncOperations.TimeToTreatOperationHangedInMinutes', 'TimeToTreatOperationHangedInMinutes', 'AsyncOperations', 'Time to treat operationHanged in minutes.', 1, 0, '15'
UNION ALL
SELECT 'AsyncOperations.NumberOfRetries', 'NumberOfRetries', 'AsyncOperations', 'Number Of retries.', 1, 0, '40'
UNION ALL
SELECT 'AsyncOperations.DelayBetweenRetriesInSeconds', 'DelayBetweenRetriesInSeconds', 'AsyncOperations', 'Delay between retries in seconds.', 1, 0, '15'
)
INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
	SELECT * FROM Data

PRINT N'Update complete.';
GO
