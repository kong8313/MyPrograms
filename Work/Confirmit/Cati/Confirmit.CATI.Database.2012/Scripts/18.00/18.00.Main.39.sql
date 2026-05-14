PRINT N'Altering [dbo].[BvSpAssignment_List]...';


GO
ALTER PROCEDURE [dbo].[BvSpAssignment_List]
    @SurveySID INT,
	@CallCenterID INT
AS
SET NOCOUNT ON
    SELECT BvPersonOrGroupAssignmentOnSurvey.Id AS AssignmentSID,
                BvSurvey.SID AS SurveySID,
                BvSurvey.[Name] AS SurveyName,
                0 AS IsSurveyGroup,
                0 AS Counts,
                BvPersonOrGroupAssignmentOnSurvey.PersonOrGroupId AS PersonSID,
                BvViewPersonAndGroup.[Name] AS Name,
                BvViewPersonAndGroup.IsGroup AS IsPersonGroup
        FROM BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID) BvPersonOrGroupAssignmentOnSurvey, BvSurvey, BvViewPersonAndGroup
        WHERE   BvPersonOrGroupAssignmentOnSurvey.PersonOrGroupId = BvViewPersonAndGroup.SID
            AND BvPersonOrGroupAssignmentOnSurvey.SurveyId = @SurveySID
            AND BvSurvey.SID = @SurveySID
        UNION ALL
        SELECT BvSvySchedule.ExplicitSID AS AssignmentSID,
                BvSurvey.SID AS SurveySID,
                BvSurvey.[Name] AS SurveyName,
                0 AS IsSurveyGroup,
                COUNT(*) AS Counts,
                BvSvySchedule.ExplicitSID AS PersonSID,
                pag.[Name] AS Name,
                pag.IsGroup AS IsPersonGroup
            FROM BvSvySchedule WITH(NOLOCK), BvSurvey, BvFnPersonAndGroup_Get(@CallCenterID) pag
            WHERE --BvSvySchedule.ExplicitType = 2
            BvSvySchedule.SurveySID = @SurveySID
            AND BvSvySchedule.ExplicitSID = pag.SID
            AND BvSurvey.SID = @SurveySID
            AND BvSvySchedule.CallState > 0
        GROUP BY BvSurvey.SID, BvSurvey.[Name],
            BvSvySchedule.ExplicitSID, pag.[Name],
            pag.IsGroup

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Update complete.';


GO
