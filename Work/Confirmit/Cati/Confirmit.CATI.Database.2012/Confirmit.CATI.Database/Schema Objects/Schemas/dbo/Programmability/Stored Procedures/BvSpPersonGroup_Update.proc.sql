CREATE PROCEDURE [dbo].[BvSpPersonGroup_Update]
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