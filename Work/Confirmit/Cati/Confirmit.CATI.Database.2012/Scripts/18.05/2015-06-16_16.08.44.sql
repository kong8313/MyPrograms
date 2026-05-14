PRINT N'Add new RoutineMaintenance.Actions.DatabaseMaintenance.RebuildIndexShiftType system setting'
GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
    ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
    (
	SELECT 'MultipleAssignments.Enabled', 'IsMultipleAssignmentsEnabled', 'MultipleAssignments', 'Is multiple assignments enabled', 3, 0, 'False'
        UNION ALL 
        SELECT 'RoutineMaintenance.Actions.AssignmentResourceTableCleanup.ShiftType', 'Maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '2'
    )
    INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
        SELECT * FROM Data
END

GO
PRINT N'Creating [dbo].[BvAssignmentResource]...';


GO
CREATE TABLE [dbo].[BvAssignmentResource] (
    [ID]        INT            NOT NULL,
    [Name]      NVARCHAR (MAX) NULL,
    [Qualifier] VARCHAR (900)  NULL
);


GO
PRINT N'Creating [dbo].[BvAssignmentResource].[PK_BvAssignmentResource_ID]...';


GO
CREATE UNIQUE CLUSTERED INDEX [PK_BvAssignmentResource_ID]
    ON [dbo].[BvAssignmentResource]([ID] ASC);


GO
PRINT N'Creating [dbo].[BvAssignmentResource].[IX_BvAssignmentResource_Qualifier]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BvAssignmentResource_Qualifier]
    ON [dbo].[BvAssignmentResource]([Qualifier] ASC) WITH (IGNORE_DUP_KEY = ON);


GO
PRINT N'Creating [dbo].[BvAssignmentResourceItem]...';


GO
CREATE TABLE [dbo].[BvAssignmentResourceItem] (
    [AssignmentID] INT NOT NULL,
    [ResourceID]   INT NOT NULL
);


GO
PRINT N'Creating [dbo].[BvSpAssignmentResource_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpAssignmentResource_Insert]
@Qualifier VARCHAR(900)
AS
SET NOCOUNT ON

DECLARE @Persons TABLE( ID INT)
DECLARE @ID INT = NULL

SELECT @ID = ID FROM BvAssignmentResource WHERE Qualifier = @Qualifier

IF @ID IS NOT NULL 
BEGIN
	SELECT * FROM @Persons
	RETURN @ID
END

DECLARE @Resources TABLE( ID INT) 
INSERT INTO @Resources SELECT Item FROM dbo.utilSplitNumbers(@Qualifier, ',')

DECLARE @Name NVARCHAR(MAX) = ''

SELECT @Name = @Name + Name + ',' FROM @Resources r LEFT JOIN BvViewPersonAndGroup pg ON r.ID = pg.SID ORDER BY r.ID

SET @Name = SUBSTRING(@Name, 0, LEN(@Name))

EXEC @ID = BvSpGetNewSID

INSERT INTO BvAssignmentResource( ID, Name, Qualifier ) VALUES( @ID, @Name, @Qualifier )

IF @@ROWCOUNT > 0
BEGIN 
	INSERT INTO BvAssignmentResourceItem(AssignmentID, ResourceID) SELECT @ID, ID FROM @Resources 
	DECLARE @Size INT = @@ROWCOUNT
	INSERT INTO @Persons 
		SELECT pr.PersonSID FROM BvAssignmentResourceItem ari LEFT JOIN BvPersonRel pr ON pr.ObjectSID = ari.ResourceID 
		WHERE ari.AssignmentID = @ID
		GROUP BY pr.PersonSID HAVING COUNT(*) = @Size

	INSERT INTO BvPersonRel( PersonSID, ObjectSID, RoleID, Type ) SELECT ID, @ID, 2, 1 FROM @Persons
END
ELSE
BEGIN
    SELECT @ID = ID FROM BvAssignmentResource WHERE Qualifier = @Qualifier
END

SELECT * FROM @Persons

RETURN @ID
GO


PRINT N'Altering [dbo].[BvFnPersonAndGroup_Get]...';
GO

ALTER FUNCTION [dbo].[BvFnPersonAndGroup_Get]
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
	UNION
    SELECT  
	    BvAssignmentResource.Id, 
		0 as CallCenterID,
        Name, 
        1 as IsGroup
    FROM BvAssignmentResource
)

GO

PRINT N'Altering [dbo].[BvSpPerson_SpinUp]...';
GO

ALTER PROCEDURE [dbo].[BvSpPerson_SpinUp]
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
	insert into @temp select AssignmentID, 2, 2 from assignmentResources

    delete from BvPersonRel where PersonSID = @PersonSID
    insert into BvPersonRel( PersonSID, ObjectSID, RoleID, Type )
        select @PersonSID, sid, role_id, type from @temp

RETURN (0)
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
    UPDATE BvSvySchedule 
        SET ExplicitSID = BvSvySchedule.SurveySID, 
            ExplicitType = 1
        WHERE ExplicitSID = @SID

	-- detect invalid multiple assignemnts and ressing calls from this assignments on "new" corresponding multiple assignemnts or group
	DECLARE @AssignmentChanges TABLE( OldId INT, NewId INT)
	DECLARE @AssignmentId INT

	DECLARE crAssignmentResource CURSOR FOR 
		SELECT AssignmentID FROM BvAssignmentResourceItem WHERE ResourceID = @SID

	OPEN crAssignmentResource 

	FETCH NEXT FROM crAssignmentResource INTO @AssignmentId 
		
	WHILE ( @@FETCH_STATUS = 0 ) 
	BEGIN
		DECLARE @Qualifier NVARCHAR(MAX) = NULL
		DECLARE @Size INT = 0
	
		SELECT @Qualifier = CASE WHEN @Qualifier IS NULL THEN '' ELSE + @Qualifier + ',' END + CAST( ResourceId AS NVARCHAR(64)), @Size = @Size + 1
			FROM BvAssignmentResourceItem WHERE AssignmentID = @AssignmentId AND ResourceID <> @SID ORDER BY ResourceID
	
		DECLARE @NewId INT = NULL
		IF @Size > 1 
		BEGIN
			EXEC @NewId = BvSpAssignmentResource_Insert @Qualifier
		END
		ELSE
		BEGIN
			SET @NewId = CAST( @Qualifier AS INT )
		END
	
		INSERT INTO @AssignmentChanges(OldId, NewId) VALUES( @AssignmentId, @NewId )
	
		FETCH NEXT FROM crAssignmentResource INTO @AssignmentId 
	END

	CLOSE crAssignmentResource
	DEALLOCATE crAssignmentResource

	UPDATE BvSvySchedule SET ExplicitSID = NewId 
	FROM BvSvySchedule c INNER JOIN @AssignmentChanges ac ON c.ExplicitSID = ac.OldId

RETURN (0)

GO

PRINT N'Altering [dbo].[BvSpPersonGroup_Update]...';
GO

ALTER PROCEDURE [dbo].[BvSpPersonGroup_Update]
        @SID                int,
        @Name               nvarchar( 255 ),
        @Description        nvarchar( 255 ),
        @ManualSelection    int,
  @BvID    int
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

IF ISNULL( @BvID, 0 ) > 0
BEGIN
    IF EXISTS( 
     SELECT 1 FROM BvNumber 
     WHERE BvID = @BvID AND ClassID = 65546 AND ObjectSID != @SID
    )
    BEGIN
     RAISERROR( 'BvID = %u already exists', 16, 1, @BvID )
     RETURN -1
    END
END

DECLARE @OldName NVARCHAR(255)

UPDATE  BvPersonGroup
    SET @OldName = [Name],
	[Name] = @Name,
    [Description] = @Description,
    ManualSelection = @ManualSelection
    WHERE SID = @SID

IF ISNULL( @BvID, 0 ) > 0
 UPDATE BvNumber SET BvID = @BvID 
 WHERE ObjectSID = @SID AND ClassID = 65546
ELSE
    DELETE FROM BvNumber 
    WHERE ObjectSID = @SID AND ClassID = 65546

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
	

RETURN 0

GO

PRINT N'Altering [dbo].[BvViewPersonAndGroup]...';
GO

ALTER VIEW BvViewPersonAndGroup AS
    SELECT  SID, 
		CallCenterID,
        Name, 
        0           IsGroup,
        FullName,
        Description
        FROM    BvPerson
    UNION
    SELECT  BvPersonGroup.SID, 
		0			CallCenterID,
        Name, 
        1           IsGroup,
        ''          FullName,
        ''          Description
    FROM    BvPersonGroup
	UNION
    SELECT  BvAssignmentResource.Id, 
		0			CallCenterID,
        Name, 
        1           IsGroup,
        ''          FullName,
        ''          Description
    FROM    BvAssignmentResource

GO

PRINT N'Creating [dbo].[BvSpAssignmentResource_GetResources]...';
GO

CREATE PROCEDURE [dbo].[BvSpAssignmentResource_GetResources]
@AssignmentResourceId INT
AS
SET NOCOUNT ON

DECLARE @Resources TABLE( ID INT )
INSERT INTO @Resources SELECT ResourceId FROM BvAssignmentResourceItem WHERE AssignmentId = @AssignmentResourceId
IF @@ROWCOUNT = 0 AND NOT EXISTS( SELECT 1 FROM BvSurvey WHERE SID = @AssignmentResourceId )
BEGIN
	INSERT INTO @Resources SELECT @AssignmentResourceId
END

SELECT * FROM @Resources

GO

PRINT N'Creating [dbo].[BvSpAssignmentResource_ListUnused]...';
GO

CREATE PROCEDURE [dbo].[BvSpAssignmentResource_ListUnused]
AS
SET NOCOUNT ON

SELECT ID FROM BvAssignmentResource ar WHERE NOT EXISTS( SELECT 1 FROM BvSvySchedule c WHERE c.ExplicitSID = ar.ID)

GO

PRINT N'Creating [dbo].[BvSpAssignmentResource_TryDelete]...';
GO

CREATE PROCEDURE [dbo].[BvSpAssignmentResource_TryDelete]
@AssignmentResourceId INT
AS
SET NOCOUNT ON

BEGIN TRAN
	DELETE FROM BvAssignmentResource WHERE ID = @AssignmentResourceId
	IF EXISTS( SELECT 1 FROM BvSvySchedule WHERE ExplicitSID = @AssignmentResourceId )
	BEGIN 
		ROLLBACK TRAN
	END
	ELSE
	BEGIN 
		DELETE FROM BvAssignmentResourceItem WHERE AssignmentID = @AssignmentResourceId
		COMMIT TRAN
	END

GO

PRINT N'Update complete.';


GO
