CREATE FUNCTION [dbo].[BvFnPerson_GetByTransferBatch]( @batchId INT )
RETURNS TABLE
AS
RETURN
(
	SELECT pr.PersonSID as Id FROM BvPersonRel pr 
	INNER JOIN BvTransferArrays ta ON (ta.BatchID = @batchId AND pr.ObjectSID = ta.ItemID)

	UNION ALL

	SELECT p.SID as Id FROM BvPerson p
	WHERE NOT EXISTS(SELECT 1 from BvTransferArrays ta WHERE ta.BatchID = @batchId)
)


