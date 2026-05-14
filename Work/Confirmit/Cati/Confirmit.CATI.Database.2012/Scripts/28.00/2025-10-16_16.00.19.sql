GO
PRINT N'Altering Procedure [dbo].[BvSpPerson_ListByParent] Add [Description]';


GO
ALTER PROCEDURE [dbo].[BvSpPerson_ListByParent]
	@ParentSID INT,
	@CallCenterID INT
AS
	SELECT  
        BvPerson.SID AS [SID],
        10 AS [ClassID], /* BVDBS_PERSON */
        BvPerson.[Name] AS [Name],
		BvPerson.[Description] AS [Description],
  ISNULL(BvTasks.[SurveySID], 0) AS [SurveySID],
  ISNULL(BvTasks.[InterviewID], 0) AS [InterviewID],
  2 AS [RoleID] /* always CATI */  
        FROM  BvPerson
  LEFT JOIN BvTasks
	ON BvTasks.PersonSID = BvPerson.SID
  INNER JOIN BvMembership 
	ON BvPerson.SID = BvMembership.ObjectSID
  WHERE BvMembership.ContainerSID = @ParentSID AND ( BvPerson.CallCenterID = @CallCenterID OR @CallCenterID = 0 )
  ORDER BY ClassID DESC
GO
PRINT N'Update complete.';


GO
