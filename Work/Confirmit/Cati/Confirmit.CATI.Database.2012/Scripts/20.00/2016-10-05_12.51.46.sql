GO
PRINT N'Altering [dbo].[BvSpCall_Enable]...';


GO
ALTER PROCEDURE BvSpCall_Enable
	@SurveySID INT,
	@BatchID INT,
	@Enable BIT
AS
IF  @Enable = 1 
BEGIN
	DECLARE @whereCondition NVARCHAR(MAX) 
	DECLARE @Query NVARCHAR(MAX)
	EXEC BvClr_QuotaService_GetWhereForAllClosedSurveyCells @SurveySID, 'repl', @whereCondition OUTPUT


    SET @Query = 'UPDATE BvSvySchedule SET CallState = 2
	FROM BvTransferArrays ta
	LEFT JOIN BvReplicatedData_' + CAST(@SurveySID AS NVARCHAR(255)) + ' as repl
	ON repl.respid = ta.ItemID 
	WHERE	NOT (' + @whereCondition + ') AND
	        BvSvySchedule.SurveySID = @SurveySID AND
			BvSvySchedule.InterviewID = ta.ItemID AND
			ta.BatchID = @BatchID AND
			BvSvySchedule.CallState IN (1)';

	EXEC sp_executesql @Query, N'@SurveySID INT, @BatchID INT', @SurveySID, @BatchID
END
ELSE
BEGIN
    UPDATE BvSvySchedule SET CallState = 1
	FROM BvTransferArrays ta
	WHERE	BvSvySchedule.SurveySID = @SurveySID AND
			BvSvySchedule.InterviewID = ta.ItemID AND
			ta.BatchID = @BatchID AND
			BvSvySchedule.CallState IN ( -2, 2)
END
GO
PRINT N'Update complete.';


GO
