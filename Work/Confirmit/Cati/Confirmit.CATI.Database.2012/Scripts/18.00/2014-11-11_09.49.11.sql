ALTER PROCEDURE BvSpCall_Enable
	@SurveySID INT,
	@BatchID INT,
	@Enable BIT
AS
UPDATE BvSvySchedule SET CallState = CASE WHEN @Enable = 1 THEN 2 ELSE 1 END
	FROM BvTransferArrays ta
	WHERE	BvSvySchedule.SurveySID = @SurveySID AND
			BvSvySchedule.InterviewID = ta.ItemID AND
			ta.BatchID = @BatchID AND
			BvSvySchedule.CallState IN ( -2, 1, 2)

GO
PRINT N'Update complete.';
