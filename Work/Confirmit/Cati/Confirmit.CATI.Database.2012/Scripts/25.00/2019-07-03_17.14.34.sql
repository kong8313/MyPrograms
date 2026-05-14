GO
PRINT N'Altering [dbo].[BvSpPerson_SpinUp]...';


GO
ALTER  PROCEDURE [dbo].[BvSpPerson_SpinUp]
    @PersonSID INT
AS
	--if person is not found then we use 0 call center id, because person group is global.
	DECLARE @CallCenterID TINYINT = ISNULL( (SELECT CallCenterID FROM BvPerson WHERE SID = @PersonSID ), 0 )
    
	declare @temp table
    (
        sid int not null,
        role_id int not null,
        type int not null
    )

    insert into @temp
        select distinct m.ContainerSID, 2, 1
        from BvMemberShip m
        inner join BvPersonGroup g on g.SID = m.ContainerSID
        where m.ObjectSID = @PersonSID

    insert into @temp values ( @PersonSID, 0, 1 )

    insert into @temp
        select distinct a.SurveyId, 2, 2 from BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID) a
		inner join @temp temp
		ON a.PersonOrGroupId = temp.sid
        where a.CallCenterID = @CallCenterID
    
	;with assignmentResources as
	(
		select ari.AssignmentID from BvAssignmentResourceItem ari 
			left join @temp as t on ari.ResourceID = t.sid
			group by ari.AssignmentID
			having COUNT(*) = COUNT(t.SID)
	)
	insert into @temp select AssignmentID, 2, 1 from assignmentResources

    delete from BvPersonRel where PersonSID = @PersonSID
    insert into BvPersonRel( PersonSID, ObjectSID, RoleID, Type )
        select @PersonSID, sid, role_id, type from @temp

   -- update Person Automatic Survey
   UPDATE BvPerson SET AutomaticSurveyID = NULL WHERE 
		SID = @PersonSID AND
		NOT EXISTS (SELECT 1 FROM @temp WHERE type = 2 AND role_id = 2 AND sid = BvPerson.AutomaticSurveyID);

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpAssignment_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpAssignment_Delete]
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

   -- update group Persons Automatic Survey
   UPDATE BvPerson SET AutomaticSurveyID = NULL WHERE 
		AutomaticSurveyID = @SurveySID AND
		EXISTS (SELECT 1 FROM @DeletedRelVar WHERE Type = 2 AND PersonSID = BvPerson.SID AND ObjectSid = @SurveySID);
   
END

RETURN @InsertedAssignmentsCount
GO
PRINT N'Refreshing [dbo].[BvSpCallCenter_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallCenter_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonGroup_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonGroup_Insert]';


GO
PRINT N'Update complete.';


GO
