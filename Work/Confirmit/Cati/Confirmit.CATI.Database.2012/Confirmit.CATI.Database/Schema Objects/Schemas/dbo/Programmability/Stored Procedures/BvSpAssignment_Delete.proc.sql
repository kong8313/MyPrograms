CREATE PROCEDURE [dbo].[BvSpAssignment_Delete]
@SurveySID INT, 
@Count INT, 
@PersonSID INT, 
@RoleID INT,
@CallCenterID INT
AS
SET NOCOUNT ON

DECLARE @InsertedAssignmentsCount INTEGER = 0

 IF @Count > 0 
 BEGIN

    UPDATE BvSvySchedule SET ExplicitSID = @SurveySID, ExplicitType = 1
    WHERE ExplicitSID = @PersonSID AND
          SurveySID = @SurveySID AND
          CallState > 0 AND
          @RoleID = 2

    RETURN @InsertedAssignmentsCount
 END
 ELSE
 BEGIN
    DELETE FROM BvPersonOrGroupAssignmentOnSurvey
      WHERE PersonOrGroupId = @PersonSID AND SurveyId = @SurveySID AND CallCenterID = @CallCenterID
    SET @InsertedAssignmentsCount = @@ROWCOUNT
 END

-- recalculate login cache
IF EXISTS ( SELECT SID FROM BvPerson WHERE SID = @PersonSID )
   EXEC BvSpPerson_SpinUp @PersonSID
ELSE
BEGIN
   DECLARE @DeletedRelVar table (  
    [PersonSID] INT NOT NULL,
    [ObjectSID] INT NOT NULL,
    [RoleID]    INT NOT NULL,
    [Type]      INT NOT NULL
   );

   DELETE BvPersonRel
   OUTPUT DELETED.* INTO @DeletedRelVar
   FROM BvPersonRel base
   WHERE ObjectSid = @SurveySID AND    --look at assignments to survey only
         Type = 2 AND                          
         PersonSid IN (SELECT SID FROM BvMembership ms --look at all persons inside current group and call center
                       INNER JOIN BvPerson p
                       ON ms.ObjectSID = p.SID
                       WHERE ms.ContainerSID = @PersonSID AND p.CallCenterID = @CallCenterID ) AND
         NOT EXISTS (SELECT *                  --if person doesn't assign directly to survey
                     FROM BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID)
                     WHERE PersonOrGroupId = base.PersonSid AND
                           SurveyId = @SurveySID) AND
         NOT EXISTS (SELECT *                  --if person doesn't belong to others groups assigned to survey
                     FROM BvMemberShip
                     INNER JOIN BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID) ON PersonOrGroupId = ContainerSid AND
                                                                     SurveyId = @SurveySID
                     WHERE ObjectSid = base.PersonSid);
   
END

RETURN @InsertedAssignmentsCount