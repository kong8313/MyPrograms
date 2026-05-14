PRINT N'Altering [dbo].[BvSpGetOpenedSurveys]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetOpenedSurveys]
   @PersonSID INT
AS
SET NOCOUNT ON
    DECLARE @CallCenterId INT = (SELECT CallCenterID FROM BvPerson WHERE SID = @PersonSID )
    
    declare @utcnow datetime = getutcdate()
    
    DECLARE @OpenedSurveys TABLE( SID INT, Name NVARCHAR(256), HasAssign BIT )
 
      INSERT INTO @OpenedSurveys
      SELECT s.SID, s.[Name], CASE WHEN l.ObjectSID IS NULL THEN 0 ELSE 1 END as HasAssign
            FROM BvSurvey s
      LEFT JOIN BvPersonRel l on l.PersonSid = @PersonSID AND
                              l.ObjectSID = s.SID
      INNER JOIN BvSurveyAssignmentOnCallCenter saocc
            ON s.SID = saocc.SurveyId AND saocc.CallCenterId = @CallCenterId
      WHERE s.State = 1
 
      SELECT DISTINCT s.SID, s.[Name] FROM @OpenedSurveys s
      WHERE s.HasAssign = 1 
      UNION ALL
      SELECT s.SID, s.[Name] FROM @OpenedSurveys s
      WHERE s.HasAssign = 0 AND EXISTS ( SELECT 1
                  FROM BvPersonRel l
                  INNER JOIN BvActiveShiftTypeZone a on a.SurveyId = s.SID
                  CROSS APPLY dbo.GetPriorityCallByExplicitSidAndShiftTypeId(l.ObjectSID, a.Id, a.SurveyId, @utcnow, 1)
                  WHERE l.PersonSID = @PersonSID AND l.Type = 1 )



RETURN (0)
GO
