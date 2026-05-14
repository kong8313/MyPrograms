CREATE PROCEDURE [dbo].[BvSpAssignment_Insert]
@SID INT, 
@SurveySID INT, 
@InterviewSID INT, 
@PersonSID INT, 
@RoleID INT, 
@FromCall INT=0,
@CallCenterID INT
AS
SET NOCOUNT ON
DECLARE @InsertedAssignmentsCount INTEGER = 0

IF @InterviewSID > 0 OR @FromCall > 0 
BEGIN

            UPDATE BvSvySchedule SET
                ExplicitSID = @PersonSID, 
                ExplicitType = 2, --Person type
                Priority = CASE WHEN OldPriority > 0 THEN OldPriority ELSE Priority END,
                OldPriority = 0
            WHERE SurveySID = @SurveySID AND 
                  InterviewID = @InterviewSID AND
                  CallState > 0
END
ELSE
BEGIN
        
    IF NOT EXISTS ( SELECT * FROM BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID)
        WHERE PersonOrGroupId = @PersonSID AND SurveyId = @SurveySID)
          INSERT INTO BvPersonOrGroupAssignmentOnSurvey( PersonOrGroupId, SurveyId, CallCenterID )
              VALUES( @PersonSID, @SurveySID, @CallCenterID )
              
    SET @InsertedAssignmentsCount = @@ROWCOUNT          
   
   IF EXISTS ( SELECT SID FROM BvPerson WHERE SID = @PersonSID )
   BEGIN
	   INSERT INTO BvPersonRel( PersonSID, ObjectSID, RoleID, Type )
	   VALUES(@PersonSID, @SurveySID, 2, 2)
   END
   ELSE
   BEGIN
       INSERT INTO BvPersonRel( PersonSID, ObjectSID, RoleID, Type )
       SELECT r.PersonSid, @SurveySID, 2, 2
       FROM BVPersonRel r
	   LEFT JOIN BvPerson p 
		ON r.PersonSID = p.SID
       WHERE @PersonSID = r.ObjectSID AND
             ObjectSID != r.PersonSid AND
			 ( p.CallCenterID = @CallCenterID OR p.SID IS NULL )
   END
END

RETURN @InsertedAssignmentsCount