GO
PRINT N'Altering Procedure [dbo].[BvSpGetUserGroups]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetUserGroups]
    @PersonSID INT
AS
    IF NOT EXISTS( SELECT 1 FROM BvPerson WHERE SID = @PersonSID )
    BEGIN
        RAISERROR( 'The person with SID="%u" not found', 16, 1, @PersonSID )
        RETURN -1
    END

    SELECT rel.ObjectSID AS GroupSID FROM bvpersonrel AS rel
        LEFT JOIN BvPersonGroup AS gr ON rel.ObjectSID = gr.SID
    WHERE 
        rel.PersonSID = @PersonSID AND rel.RoleID = 2 AND rel.Type = 1 AND (gr.IsAdministrative = 0 OR gr.SID IS NULL)
    
    RETURN @@ROWCOUNT
GO
PRINT N'Altering Procedure [dbo].[BvSpPerson_SpinUp]...';


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

RETURN (0)
GO
PRINT N'Altering Procedure [dbo].[BvSpPersonGroup_Delete]...';


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
    WHERE ObjectSID = @SID

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
PRINT N'Altering Procedure [dbo].[BvSpPersonGroup_Update]...';


GO
ALTER PROCEDURE [dbo].[BvSpPersonGroup_Update]
        @SID                  int,
        @Name                 nvarchar( 255 ),
        @Description          nvarchar( 255 ),
        @InboundCallBehavior  TINYINT,
        @CallTransferBehavior TINYINT,
		@IsAdministrative     BIT = 0
AS

IF EXISTS ( SELECT [SID] FROM BvPersonGroup WHERE [Name] = @Name AND [SID] != @SID )
BEGIN
 RAISERROR('Person group %s already exists', 12, 2, @Name)
 RETURN -1
END

DECLARE @Rows int
SELECT  @Rows = COUNT(*)
    FROM    BvPersonGroup
    WHERE   SID = @SID

IF @Rows = 0
  BEGIN
    RAISERROR('Person group with SID %i not exists', 16, 2, @SID)
    RETURN -1
  END
IF @Rows <> 1
  BEGIN
    RAISERROR('Multiple person groups with SID %i found', 16, 2, @SID)
    RETURN -1
  END

DECLARE @OldName NVARCHAR(255)
DECLARE @OldIsAdministrative BIT

UPDATE  BvPersonGroup
    SET @OldName = [Name], 
    [Name] = @Name,
    [Description] = @Description,
    InboundCallBehavior = @InboundCallBehavior,
    CallTransferBehavior = @CallTransferBehavior,
	@OldIsAdministrative = [IsAdministrative],
	IsAdministrative = @IsAdministrative
    WHERE SID = @SID

IF @OldName <> @Name
BEGIN

	;with assignmentResources as 
	(
		select AssignmentID from BvAssignmentResourceItem WHERE ResourceID = @SID
	),
	assignmentResourceItems as
	(
		select ar.AssignmentID, ResourceID, pg.Name, ROW_NUMBER() OVER( PARTITION BY ar.AssignmentID ORDER BY SID ) as rn 
			from assignmentResources ar inner join BvAssignmentResourceItem ari ON ar.AssignmentID = ari.AssignmentID
			inner join BvViewPersonAndGroup pg ON ari.ResourceID = pg.SID
	), newAssignmentsResources( AssignmentID, Name, iteration) as
	(
		select AssignmentID, CAST( Name AS NVARCHAR(MAX)), 1 from assignmentResourceItems WHERE rn = 1
		union all 
		select nar.AssignmentID, nar.Name + ',' + ari.Name, iteration + 1 from newAssignmentsResources nar
			inner join assignmentResourceItems ari on nar.AssignmentID = ari.AssignmentID AND nar.iteration + 1 = ari.rn
	), newData( AssignmentID, Name ) as
	(
		select AssignmentID, MAX(Name) FROM newAssignmentsResources GROUP BY AssignmentID
	)
	UPDATE BvAssignmentResource SET Name = nd.Name
		FROM BvAssignmentResource ar
		INNER JOIN newData nd ON ar.ID = nd.AssignmentID

END

IF @OldIsAdministrative = 0 AND @IsAdministrative = 1
BEGIN

     -- Assign calls for administrative group to survey. 
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
END

RETURN 0
GO
PRINT N'Refreshing Procedure [dbo].[BvSpAssignment_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_Delete]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpCallCenter_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallCenter_Delete]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpPersonGroup_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonGroup_Insert]';


GO
PRINT N'Update complete.';


GO
