CREATE PROCEDURE [dbo].[BvSpAssignmentResource_ListUnused]
AS
SET NOCOUNT ON

SELECT ID FROM BvAssignmentResource ar WHERE NOT EXISTS( SELECT 1 FROM BvSvySchedule c WHERE c.ExplicitSID = ar.ID)