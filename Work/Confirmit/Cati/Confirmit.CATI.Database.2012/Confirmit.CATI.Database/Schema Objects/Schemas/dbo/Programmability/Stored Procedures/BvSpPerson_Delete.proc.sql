CREATE PROCEDURE [dbo].[BvSpPerson_Delete]
 @SID int
AS
    EXEC BvSpMembership_Delete 0, @SID

    DELETE  BvPerson WHERE SID = @SID

    DELETE FROM BvPersonRel WHERE PersonSID = @SID

	DELETE FROM BvPersonFailedLoginAttempts	WHERE PersonId = @SID

    -- delete implicit assigments
    DELETE FROM BvPersonOrGroupAssignmentOnSurvey WHERE PersonOrGroupId = @SID

    UPDATE BvSvySchedule 
    SET ExplicitSID = BvSvySchedule.SurveySID, 
        ExplicitType = 1
    WHERE ExplicitSID = @SID
