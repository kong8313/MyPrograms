PRINT N'Altering Procedure [dbo].[BvSpPerson_GetAssignments]...';


GO
ALTER procedure [dbo].[BvSpPerson_GetAssignments]
    @PersonSID  int,
    @UserName NVARCHAR(MAX),
    @CallCenterId INT
AS

/* Assignment type returned:
     0 = Implicit (via group) assignment to survey
     1 = Explicit (direct) assignment to survey
     2 = Assignment to individual calls in a survey */
	;WITH all_survey_assignments AS (

	      -- Assignments to surveys
		  SELECT a.SurveyId, ObjectSID as Resource, CASE WHEN ObjectSID = @PersonSID THEN 1 ELSE 0 END as Type, 0 as Count 
		  FROM BvPersonRel pr
		  INNER JOIN BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterId) a ON pr.ObjectSID = a.PersonOrGroupId
		  WHERE pr.PersonSID = @PersonSID AND Type = 1

		  UNION ALL

		  -- Assignments to survey calls
		  SELECT rs.SurveyId, ObjectSID as Resource, 2 as Type, SUM(TotalCount) as Count 
		  FROM BvPersonRel pr
				INNER JOIN BvSvyScheduleRuntimeStatistics rs ON pr.ObjectSID = rs.ExplicitSID
				WHERE pr.PersonSID = @PersonSID AND Type = 1
				GROUP BY rs.SurveyId, ObjectSID
	)

	SELECT s.SID as SurveyId, s.Name as ProjectId, s.Description as SurveyName, aa.Resource as ResourceId, vpg.Name as ResourceName, aa.Type, aa.Count
		  FROM all_survey_assignments aa 
		  INNER JOIN BvSurvey s ON aa.SurveyId = s.SID
		  INNER JOIN BvUserSurveyPermission p on p.UserName = @UserName and s.SID = p.SurveySID
		  INNER JOIN BvViewPersonAndGroup vpg on aa.Resource = vpg.SID
		  WHERE s.State <> 2
GO
PRINT N'Update complete.';


GO
