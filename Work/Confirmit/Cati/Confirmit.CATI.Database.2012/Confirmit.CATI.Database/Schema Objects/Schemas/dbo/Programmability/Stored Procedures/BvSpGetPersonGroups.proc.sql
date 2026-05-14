CREATE PROCEDURE [dbo].[BvSpGetPersonGroups]
 @Filter NVARCHAR(MAX) = NULL,
 @CallCenterID INT
AS
	SELECT
		[g].*,
		(	
			SELECT COUNT(*)
				FROM [BvMembership] [m1]
				LEFT JOIN BvFnPerson_Get(@CallCenterID) [p] ON [p].[SID] = [m1].[ObjectSID]
				WHERE [m1].[ContainerSID] = [g].[SID] AND [p].[Name] <> '' 
		) AS [Count]
		FROM [BvPersonGroup] [g]
		WHERE [g].[Name] <> '' AND (@Filter IS NULL OR [g].[Name] LIKE @Filter)