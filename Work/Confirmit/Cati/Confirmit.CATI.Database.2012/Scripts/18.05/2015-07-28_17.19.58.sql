GO
PRINT N'Altering [dbo].[BvSpPersonGroup_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpPersonGroup_Delete]
 @SID int
AS
DECLARE @GroupName NVARCHAR(MAX)

    IF EXISTS( SELECT 1 FROM BvMembership WHERE ContainerSID = @SID )
    BEGIN
        SELECT @GroupName = Name FROM BvPersonGroup WHERE SID = @SID
        RAISERROR( 'The person group "%s" cannot be deleted because it is not empty', 12, 1, @GroupName )
        RETURN (-1)
    END

    DELETE  BvMembership
        WHERE ContainerSID = @SID OR ObjectSID = @SID

    -- delete implicit assigments
    DELETE FROM BvPersonOrGroupAssignmentOnSurvey WHERE PersonOrGroupId = @SID
        
    DELETE FROM BvPersonRel
    FROM BvPersonRel
    WHERE PersonSID = @SID

    DELETE FROM BvNumber WHERE ObjectSID = @SID AND ClassID = 65546

    DELETE  BvPersonGroup
        WHERE SID = @SID
    
	-- Assign calls for removing group to survey. 
	;WITH ExplicitSIDs as (
             SELECT @SID as SID
			 UNION ALL
			 SELECT AssignmentID FROM BvAssignmentResourceItem WHERE ResourceID = @SID
	)
	UPDATE BvSvySchedule 
        SET ExplicitSID = c.SurveySID, 
            ExplicitType = 1
		FROM BvSvySchedule c
			INNER JOIN ExplicitSIDs s
			ON c.ExplicitSID = s.SID

RETURN (0)
GO
PRINT N'Update complete.';


GO
