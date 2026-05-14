CREATE PROCEDURE [dbo].[BvSpGetPersonsLevel]
 @ParentSID INT,
 @Filter NVARCHAR(MAX) = NULL,
 @CallCenterID INT
AS
SELECT
 [p].[SID] AS [SID],
 [p].[Name] AS [Name]
FROM   
 BvFnPerson_Get(@CallCenterID) [p]
 LEFT JOIN [BvMembership] [m] ON [p].[SID] = [m].[ObjectSID]
WHERE
 [m].[ContainerSID] = @ParentSID
 AND (@Filter IS NULL OR [p].[Name] LIKE @Filter)
