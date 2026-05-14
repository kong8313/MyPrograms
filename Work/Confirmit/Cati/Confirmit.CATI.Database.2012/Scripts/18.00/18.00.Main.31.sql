PRINT N'Creating [dbo].[BvFnPersonAndGroup_Get]...';


GO
CREATE FUNCTION [dbo].[BvFnPersonAndGroup_Get]
(
	@CallCenterId int
)
RETURNS TABLE
AS
RETURN
(
	SELECT  
	    SID, 
		CallCenterID,
        Name, 
        0 as IsGroup
    FROM BvPerson
    WHERE CallCenterID = @CallCenterId
    UNION
    SELECT  
	    BvPersonGroup.SID, 
		0 as CallCenterID,
        Name, 
        1 as IsGroup
    FROM BvPersonGroup
)
GO
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
        GROUP BY BvSurvey.SID, BvSurvey.[Name], BvSvySchedule.CallState,
            BvSvySchedule.ExplicitSID, pag.[Name],
            pag.IsGroup

RETURN (0)
GO

PRINT N'Update complete.';
GO
