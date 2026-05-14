CREATE PROCEDURE [dbo].[BvSpAssignmentResource_GetResources]
@AssignmentResourceId INT
AS
SET NOCOUNT ON

DECLARE @Resources TABLE( ID INT )
INSERT INTO @Resources SELECT ResourceId FROM BvAssignmentResourceItem WHERE AssignmentId = @AssignmentResourceId
IF @@ROWCOUNT = 0 AND NOT EXISTS( SELECT 1 FROM BvSurvey WHERE SID = @AssignmentResourceId )
BEGIN
	INSERT INTO @Resources SELECT @AssignmentResourceId
END

SELECT * FROM @Resources
