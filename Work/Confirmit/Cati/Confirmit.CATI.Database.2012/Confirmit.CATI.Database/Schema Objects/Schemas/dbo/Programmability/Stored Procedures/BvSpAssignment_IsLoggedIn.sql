CREATE PROCEDURE [dbo].[BvSpAssignment_IsLoggedIn]
	@resourceId int,
	@surveySID int
AS
	SELECT COUNT(*) FROM [BvLoginGroup] [lg] WHERE [lg].[ObjectSID] = @resourceId AND ( [lg].[SurveySID] = 0 or [lg].[SurveySID] = @surveySID )
