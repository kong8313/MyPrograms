CREATE PROCEDURE [dbo].[BvSpPerson_GetAssignedSurveyList]
@PersonSID INT, @UserName NVARCHAR (MAX)=NULL, @CallCenterID INT
AS
IF @PersonSID IS NULL AND @UserName IS NULL
BEGIN
  SELECT 
    0 as [SID],
    '' as [Name],
    '' as [Description],
    0 as [AssignedCallsCount],
    0 as [AssignmentType]
  RETURN(0)
END

SELECT DISTINCT
  [s].[SID],
  [s].[Name],
  [s].[Description],
  0 AS [AssignedCallsCount],
  [AssignmentType] =
    CASE
      WHEN a.[Id] IS NULL THEN 0 -- 0 for implicit assignment by group
      ELSE 1 -- 1 for explicit assignment
    END
 FROM BvSurvey s 
  inner join BvUserSurveyPermission p on p.UserName = @UserName and s.SID = p.SurveySID
  left join BvPersonRel r on r.ObjectSID = s.SID and r.Type = 2 and r.RoleID = 2 and r.PersonSID = @PersonSID
  left join BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID) a on a.SurveyId = s.SID and a.PersonOrGroupId = @PersonSID 
 WHERE s.State <> 2 AND ( a.Id is not null or r.ObjectSID is not null )
  

UNION

 SELECT 
  BvSurvey.[SID],
  BvSurvey.[Name],
  BvSurvey.[Description],
  COUNT(*) AS [AssignedCallsCount],
  2 AS [AssignmentType] -- implicit assignment by call
    FROM BvSvySchedule WITH(NOLOCK), 
  BvSurvey, 
  BvViewPersonAndGroup, 
  BvUserSurveyPermission
 WHERE
        BvSvySchedule.ExplicitSID = BvViewPersonAndGroup.SID AND
		BvSvySchedule.ExplicitSID = @PersonSID AND
        BvSurvey.SID = BvSvySchedule.SurveySID AND
        BvSurvey.SID = BvUserSurveyPermission.SurveySID AND
        BvUserSurveyPermission.UserName = @UserName AND
        BvSurvey.State <> 2
    GROUP BY 
  BvSurvey.SID, 
  BvSurvey.[Name],
  BvSurvey.[Description]

RETURN (0)