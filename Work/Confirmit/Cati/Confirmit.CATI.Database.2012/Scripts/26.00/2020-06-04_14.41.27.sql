GO
PRINT N'Altering [dbo].[BvSpGetOpenedSurveys]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetOpenedSurveys]
   @PersonSID INT
AS
SET NOCOUNT ON
    DECLARE @CallCenterId INT = (SELECT CallCenterID FROM BvPerson WHERE SID = @PersonSID )
    
    declare @utcnow datetime = getutcdate()
    
    ;WITH Surveys AS
    (
        SELECT s.SID, s.[Name], CASE WHEN l.ObjectSID IS NULL THEN 0 ELSE 1 END as HasAssign
            FROM BvSurvey s
        LEFT JOIN BvPersonRel l on l.PersonSid = @PersonSID AND
                              l.ObjectSID = s.SID
        INNER JOIN BvSurveyAssignmentOnCallCenter saocc
            ON s.SID = saocc.SurveyId AND saocc.CallCenterId = @CallCenterId
        WHERE s.State = 1
    )
    SELECT DISTINCT s.SID, s.[Name] FROM Surveys s
      WHERE s.HasAssign = 1 OR EXISTS 
            (	SELECT 1
                    FROM BvSvyScheduleRuntimeStatistics srs
                    INNER JOIN BvActiveShiftTypeZone a on srs.SurveyId = a.SurveyId AND srs.ShiftTypeID = a.Id 
                    INNER JOIN BvPersonRel l ON srs.ExplicitSID = l.ObjectSID 
                    WHERE srs.SurveyId = s.SID AND l.PersonSID = @PersonSID AND l.Type = 1 
            )
	  ORDER BY s.SID DESC
    



RETURN (0)
GO
PRINT N'Update complete.';


GO
