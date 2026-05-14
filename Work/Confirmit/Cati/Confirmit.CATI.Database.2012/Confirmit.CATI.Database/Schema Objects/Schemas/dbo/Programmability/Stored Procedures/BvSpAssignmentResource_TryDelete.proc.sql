CREATE PROCEDURE [dbo].[BvSpAssignmentResource_TryDelete]
@AssignmentResourceId INT
AS
SET NOCOUNT ON

BEGIN TRAN
	DELETE FROM BvAssignmentResource WHERE ID = @AssignmentResourceId
	IF EXISTS( SELECT 1 FROM BvSvySchedule WHERE ExplicitSID = @AssignmentResourceId )
	BEGIN 
		ROLLBACK TRAN
	END
	ELSE
	BEGIN 
		DELETE FROM BvAssignmentResourceItem WHERE AssignmentID = @AssignmentResourceId
		DELETE FROM BvPersonRel WHERE ObjectSID = @AssignmentResourceId
		COMMIT TRAN
	END