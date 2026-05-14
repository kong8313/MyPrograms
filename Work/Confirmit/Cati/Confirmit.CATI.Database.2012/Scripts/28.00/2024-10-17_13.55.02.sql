GO
PRINT N'Altering Table [dbo].[BvPersonGroup]...';


GO
ALTER TABLE [dbo].[BvPersonGroup]
    ADD [IsAdministrative] BIT CONSTRAINT [DF_BvPersonGroup_IsAdministrative] DEFAULT 0 NOT NULL;


GO
PRINT N'Refreshing View [dbo].[BvViewPersonAndGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvViewPersonAndGroup]';


GO
PRINT N'Refreshing View [dbo].[RestView_Group]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_Group]';


GO
PRINT N'Refreshing Function [dbo].[BvFnPersonAndGroup_Get]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvFnPersonAndGroup_Get]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetActiveCallsForSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetActiveCallsForSurvey]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetAllPersonsAndGroups]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetAllPersonsAndGroups]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetPersonGroups]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetPersonGroups]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetPersonGroupsLevel]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetPersonGroupsLevel]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetSystemWideInfo]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSystemWideInfo]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpPerson_SpinUp]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_SpinUp]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpPersonAndGroups_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonAndGroups_List]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpPersonGroup_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonGroup_Delete]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpPersonGroup_GetRootGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonGroup_GetRootGroup]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpPersonGroup_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonGroup_Insert]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpPersonGroup_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonGroup_List]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpPersonGroup_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonGroup_Update]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpTransfer_GetInternalTargets]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTransfer_GetInternalTargets]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpAssignment_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_List]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpAlert_RecalculateAppointment]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlert_RecalculateAppointment]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpAssignmentResource_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignmentResource_Insert]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetDialerCallsBreakdown]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetDialerCallsBreakdown]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetExtendedCallHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetExtendedCallHistory]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpPerson_GetAssignedSurveyList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_GetAssignedSurveyList]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpPerson_GetAssignments]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_GetAssignments]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpPerson_UpdateBatched]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_UpdateBatched]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpAssignment_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_Delete]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpCallCenter_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallCenter_Delete]';


GO


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
DELETE FROM BvPersonOrGroupAssignmentOnSurvey 
    WHERE PersonOrGroupId = @SID
DELETE FROM BvPersonRel
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
SET 
    ExplicitSID = c.SurveySID, 
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

	DECLARE @rootGroups TABLE(sid int)
	INSERT INTO @rootGroups EXEC BvSpPersonGroup_GetRootGroup

	DELETE BvMembership 
	FROM BvMembership AS mem
	LEFT JOIN @rootGroups AS rg 
		ON mem.ContainerSID = rg.sid
	WHERE ObjectSID = @SID and rg.sid IS NULL

	DELETE FROM BvPersonRel
		WHERE ObjectSID = @SID AND PersonSID != @SID

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

IF @OldIsAdministrative = 1 AND @IsAdministrative = 0
BEGIN

	declare @temp table
    (
        person_sid int not null,
        role_id int not null,
        type int not null
    )

    insert into @temp
        select distinct m.ObjectSID, 2, 1
        from BvMemberShip m
        where m.ContainerSID = @SID

    insert into BvPersonRel( PersonSID, ObjectSID, RoleID, Type )
        select person_sid, @SID, role_id, type from @temp
END

RETURN 0
GO


GO



GO
PRINT N'Altering Procedure [dbo].[BvSpGetAllPersonsAndGroups]...';


GO
ALTER  PROCEDURE [dbo].[BvSpGetAllPersonsAndGroups]
 @CallCenterId INT,
 @SurveyIdForExcludeAssignment INT,
 @IncludeAdministrativeGroups BIT = 1
 AS

IF @CallCenterId IS NULL
 BEGIN
 /* Return metadata*/
 SELECT
     0  AS Id,
     '' AS Name,     
     '' as [Description],
     CAST(0 as BIT)  AS IsGroup     
     RETURN 0;
 END
 
 SELECT d.* FROM ( 
  SELECT SID as Id, Name, Description, CAST( 0 AS BIT ) as IsGroup FROM BvFnPerson_Get(@CallCenterId)
  UNION
  SELECT SID as Id, Name, Description, CAST(1 AS BIT ) as IsGroup FROM BvPersonGroup pg WHERE @IncludeAdministrativeGroups = 1 OR pg.IsAdministrative = 0) d
 LEFT JOIN BvPersonOrGroupAssignmentOnSurvey pga 
 ON d.Id = pga.PersonOrGroupId AND 
    pga.SurveyId = @SurveyIdForExcludeAssignment AND 
    pga.CallCenterId = @CallCenterId
 WHERE pga.Id IS NULL OR @SurveyIdForExcludeAssignment IS NULL
GO

PRINT N'Altering Procedure [dbo].[BvSpPersonGroup_Insert]...';

GO
ALTER PROCEDURE [dbo].[BvSpPersonGroup_Insert]
        @SID                int,
        @Name               nvarchar( 255 ),
        @Description        nvarchar( 255 ),
        @InboundCallBehavior TINYINT,
        @CallTransferBehavior TINYINT,
        @IsAdministrative BIT = 0

AS
IF EXISTS ( SELECT [SID] FROM BvPersonGroup WHERE [Name] = @Name )
BEGIN
 RAISERROR('Person group with name %s already exists', 12, 2, @Name)
 RETURN -1
END

INSERT  BvPersonGroup( 
        SID,
        [Name],
        [Description],
        [InboundCallBehavior],
        [CallTransferBehavior],
        [IsAdministrative])
    VALUES( 
        @SID, 
        @Name,
        @Description,
        @InboundCallBehavior,
        @CallTransferBehavior,
        @IsAdministrative)

EXEC BvSpPerson_SpinUp @SID

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
        inner join BvPersonGroup g on g.SID = m.ContainerSID and g.IsAdministrative = 0
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
PRINT N'Refreshing Procedure [dbo].[BvSpAssignment_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_Delete]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpCallCenter_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallCenter_Delete]';


GO


GO
PRINT N'Altering Procedure [dbo].[BvSpPersonGroup_List]...';


GO
ALTER PROCEDURE [dbo].[BvSpPersonGroup_List]
        @ParentGroupId int 

AS

IF @ParentGroupId = 0 --only root groups
	SELECT DISTINCT
	   BvPersonGroup.SID,
	   BvPersonGroup.Name,
	   BvPersonGroup.Description,
	   BvPersonGroup.InboundCallBehavior,
	   BvPersonGroup.CallTransferBehavior,
	   BvPersonGroup.IsAdministrative
	FROM BvPersonGroup
	LEFT JOIN BvMembership ON BvPersonGroup.SID = BvMembership.ObjectSID AND
							  BvMembership.ContainerSID = @ParentGroupId
	WHERE BvMembership.ObjectSID IS NULL
ELSE --child groups for passed parent group
	SELECT DISTINCT
	   BvPersonGroup.SID,
	   BvPersonGroup.Name,
	   BvPersonGroup.Description,
	   BvPersonGroup.InboundCallBehavior,
	   BvPersonGroup.CallTransferBehavior,
	   BvPersonGroup.IsAdministrative
	FROM BvPersonGroup
	INNER JOIN BvMembership ON BvPersonGroup.SID = BvMembership.ObjectSID AND
							  BvMembership.ContainerSID = @ParentGroupId
GO
PRINT N'Update complete.';


GO


