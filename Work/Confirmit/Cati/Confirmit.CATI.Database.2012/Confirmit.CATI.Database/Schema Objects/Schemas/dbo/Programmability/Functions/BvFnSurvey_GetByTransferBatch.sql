CREATE FUNCTION [dbo].[BvFnSurvey_GetByTransferBatch]( @batchId INT )
RETURNS TABLE
AS
RETURN
(
	SELECT s.* FROM BvSurvey s 
	INNER JOIN BvTransferArrays ta ON (ta.BatchID = @batchId AND s.SID = ta.ItemID)

	UNION ALL

	SELECT * FROM BvSurvey s
	WHERE NOT EXISTS(SELECT 1 from BvTransferArrays ta WHERE ta.BatchID = @batchId)
)


