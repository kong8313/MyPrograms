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
    UPDATE BvSvySchedule SET CallState = 2
	FROM BvTransferArrays ta
	WHERE	BvSvySchedule.SurveySID = @SurveySID AND
			BvSvySchedule.InterviewID = ta.ItemID AND
			ta.BatchID = @BatchID AND
			BvSvySchedule.CallState IN (1)
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
