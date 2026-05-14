CREATE PROCEDURE [dbo].[BvSpIsMultipleAssignmentGroup]
	@SID int
AS
	DECLARE @Exists INT = 0
	IF EXISTS (SELECT TOP(1) 1 FROM BvAssignmentResourceItem WHERE ResourceID = @SID)
	BEGIN
		SET @Exists = 1
	END

	SELECT @Exists
