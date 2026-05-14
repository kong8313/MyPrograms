PRINT N'Altering [dbo].[BvSpPerson_GetAssignments]...';


GO
ALTER procedure [dbo].[BvSpPerson_GetAssignments]
@PersonSID  int,
@UserName NVARCHAR(MAX),
@CallCenterId INT
as
	;WITH all_survey_assignments as (
		  SELECT a.SurveyId, ObjectSID as Resource, 0 as Type, 0 as Count FROM BvPersonRel pr
		  INNER JOIN BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterId) a ON pr.ObjectSID = a.PersonOrGroupId
		  WHERE pr.PersonSID = @PersonSID AND Type = 1
		  UNION ALL
		  SELECT rs.SurveyId, ObjectSID as Resource, 1 as Type, SUM(TotalCount ) as Count FROM BvPersonRel pr
				INNER JOIN BvSvyScheduleRuntimeStatistics rs ON pr.ObjectSID = rs.ExplicitSID
				WHERE pr.PersonSID = @PersonSID AND Type = 1
				GROUP BY rs.SurveyId, ObjectSID
	)
	SELECT s.SID as SurveyId, s.Name as ProjectId, s.Description as SurveyName, aa.Resource as  ResourceId, vpg.Name as ResourceName, aa.Type, aa.Count
		  FROM all_survey_assignments aa 
		  INNER JOIN BvSurvey s ON aa.SurveyId = s.SID
		  inner join BvUserSurveyPermission p on p.UserName = @UserName and s.SID = p.SurveySID
		  inner join BvViewPersonAndGroup vpg on aa.Resource = vpg.SID

return (0)
GO
PRINT N'Update complete.';


GO
