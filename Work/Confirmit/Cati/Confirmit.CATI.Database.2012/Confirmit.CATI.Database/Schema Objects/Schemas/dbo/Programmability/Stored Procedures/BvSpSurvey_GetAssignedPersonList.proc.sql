CREATE PROCEDURE [dbo].[BvSpSurvey_GetAssignedPersonList]
    @SurveySID INT,
    @RoleID INT,
	@CallCenterID INT
AS
 SELECT 
      p.SID AS PersonId,
      p.Name AS PersonName
  FROM BvFnPerson_Get(@CallCenterID) p, BvPersonRel r with(
  nolock ), BvSurvey s with( nolock )
  where p.SID = r.PersonSID and r.Type = 2 and r.RoleID = @RoleID and
  r.ObjectSID = s.SID and s.SID = @SurveySID
  ORDER BY p.SID