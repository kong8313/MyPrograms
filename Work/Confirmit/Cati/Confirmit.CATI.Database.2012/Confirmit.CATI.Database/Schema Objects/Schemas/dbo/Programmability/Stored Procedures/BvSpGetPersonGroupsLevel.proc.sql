CREATE PROCEDURE [dbo].[BvSpGetPersonGroupsLevel]
 @ParentSID INT,
 @Filter NVARCHAR(MAX) = NULL,
 @CallCenterID INT
AS
	SELECT
		[g].*,
		(	
			SELECT COUNT(*)
				FROM [BvMembership] [m1]
				LEFT JOIN BvFnPerson_Get(@CallCenterID) [p] ON [p].[SID] = [m1].[ObjectSID]
				WHERE [m1].[ContainerSID] = [g].[SID] AND 
					[p].[Name] <> '' AND 
					(@Filter IS NULL OR [p].[Name] LIKE @Filter)
		) AS [Count]
		FROM [BvPersonGroup] [g]
		LEFT JOIN [BvMemberShip] [m] ON [g].[SID] = [m].[ObjectSID]
		WHERE [m].[ContainerSID] = @ParentSID AND  
		  [g].[Name] <> '' AND  
		  (@Filter IS NULL OR [g].[Name] LIKE @Filter)