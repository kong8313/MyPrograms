PRINT N'Add Console.NoCallsTimeout setting';


GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
      SELECT 'Toggle.EnableTransfer', 'Enable transfer', 'Toggle', 'Enable call transfer functionality', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data
END

GO
PRINT N'Dropping [dbo].[DF_BvPersonGroup_Manual_SELECTion]...';


GO
ALTER TABLE [dbo].[BvPersonGroup] DROP CONSTRAINT [DF_BvPersonGroup_Manual_SELECTion];


GO
PRINT N'Dropping [dbo].[BvSpBvID_Delete]...';


GO
DROP PROCEDURE [dbo].[BvSpBvID_Delete];


GO
PRINT N'Dropping [dbo].[BvSpExecuteForAllSurveys]...';


GO
DROP PROCEDURE [dbo].[BvSpExecuteForAllSurveys];


GO
PRINT N'Dropping [dbo].[BvSpSetObjectNumber]...';


GO
DROP PROCEDURE [dbo].[BvSpSetObjectNumber];


GO
PRINT N'Dropping [dbo].[BvNumber]...';


GO
DROP TABLE [dbo].[BvNumber];


GO
PRINT N'Altering [dbo].[BvPersonGroup]...';


GO
ALTER TABLE [dbo].[BvPersonGroup] DROP COLUMN [ManualSelection];


GO
ALTER TABLE [dbo].[BvPersonGroup]
    ADD [InboundCallBehavior]  TINYINT NOT NULL CONSTRAINT DF_BvPersonGroup_TMP1 DEFAULT(0),
        [CallTransferBehavior] TINYINT NOT NULL CONSTRAINT DF_BvPersonGroup_TMP2 DEFAULT(0);

ALTER TABLE [dbo].[BvPersonGroup] DROP CONSTRAINT DF_BvPersonGroup_TMP1;
ALTER TABLE [dbo].[BvPersonGroup] DROP CONSTRAINT DF_BvPersonGroup_TMP2;


GO
PRINT N'Refreshing [dbo].[BvViewPersonAndGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvViewPersonAndGroup]';


GO
PRINT N'Refreshing [dbo].[RestView_Group]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_Group]';


GO
PRINT N'Refreshing [dbo].[BvFnPersonAndGroup_Get]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvFnPersonAndGroup_Get]';

GO
PRINT N'Altering [dbo].[BvSpGetPersonGroupsLevel]...';

GO
ALTER PROCEDURE [dbo].[BvSpGetPersonGroupsLevel]
 @ParentSID INT,
 @Filter NVARCHAR(MAX) = NULL,
 @CallCenterID INT
AS
	SELECT
		[g].*,
		(	
			SELECT COUNT(*)
				FROM [BvMembership] [m1]
				LEFT JOIN BvFnPerson_Get(@CallCenterID) [p] ON [p].[SID] = [m1].[ObjectSID]
				WHERE [m1].[ContainerSID] = [g].[SID] AND 
					[p].[Name] <> '' AND 
					(@Filter IS NULL OR [p].[Name] LIKE @Filter)
		) AS [Count]
		FROM [BvPersonGroup] [g]
		LEFT JOIN [BvMemberShip] [m] ON [g].[SID] = [m].[ObjectSID]
		WHERE [m].[ContainerSID] = @ParentSID AND  
		  [g].[Name] <> '' AND  
		  (@Filter IS NULL OR [g].[Name] LIKE @Filter)
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
PRINT N'Altering [dbo].[BvSpPersonGroup_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpPersonGroup_Insert]
        @SID                int,
        @Name               nvarchar( 255 ),
        @Description        nvarchar( 255 ),
        @InboundCallBehavior TINYINT,
        @CallTransferBehavior TINYINT

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
        [CallTransferBehavior])
    VALUES( 
        @SID, 
        @Name,
        @Description,
        @InboundCallBehavior,
        @CallTransferBehavior)

EXEC BvSpPerson_SpinUp @SID
GO
PRINT N'Altering [dbo].[BvSpPersonGroup_List]...';


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
	   BvPersonGroup.CallTransferBehavior
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
	   BvPersonGroup.CallTransferBehavior
	FROM BvPersonGroup
	INNER JOIN BvMembership ON BvPersonGroup.SID = BvMembership.ObjectSID AND
							  BvMembership.ContainerSID = @ParentGroupId
GO
PRINT N'Altering [dbo].[BvSpPersonGroup_Update]...';


GO
ALTER PROCEDURE [dbo].[BvSpPersonGroup_Update]
        @SID                  int,
        @Name                 nvarchar( 255 ),
        @Description          nvarchar( 255 ),
        @InboundCallBehavior  TINYINT,
        @CallTransferBehavior TINYINT
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

UPDATE  BvPersonGroup
    SET @OldName = [Name],
    [Name] = @Name,
    [Description] = @Description,
    InboundCallBehavior = @InboundCallBehavior,
    CallTransferBehavior = @CallTransferBehavior
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
	

RETURN 0
GO
PRINT N'Altering [dbo].[BvSpGetSurveys]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetSurveys]
 @Filter NVARCHAR(MAX) = NULL,
 @UserName NVARCHAR(MAX) = NULL,
 @CallCenterId INT
AS
SELECT DISTINCT
 [s].[SID] AS [SID],
 [s].[Name] AS [ConfirmitID],
 [s].[Description] AS [Name]
FROM    [BvFnSurvey_GetByCallCenterId](@CallCenterId) [s] 
left join [bvUserSurveyPermission] [p] on [s].[SID] = [p].[SurveySID]
WHERE
     ( p.UserName = @UserName or @UserName is null)
 AND (@Filter IS NULL OR [s].[Description] LIKE @Filter)
 AND ( s.State <> 2)
GO
PRINT N'Altering [dbo].[BvSpPerson_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpPerson_Delete]
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
GO
PRINT N'Altering [dbo].[BvSpPerson_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpPerson_Insert]
        @SID INT, 
        @Name NVARCHAR( 255 ),  
        @FullName NVARCHAR( 255 ),
        @Description NVARCHAR( 255 ),
        @ManualSelection INT,
        @AssignmentsListMode INT,
        @PwdSaltTxt NVARCHAR(256),
		@CallGroupId INT,
		@CallCenterID INT,
		@Location NVARCHAR(256),
		@DialTypeId TINYINT,
		@Type TINYINT
AS

IF (EXISTS(SELECT 1 FROM BvPerson WHERE [Name]=@Name))
BEGIN
    RAISERROR( 'Person with name %s already exists', 12, 1, @Name )
    RETURN -1
END

INSERT  BvPerson( 
        SID,
        [Name], 
        FullName,
        [Description],
        ManualSelection, 
        AssignmentsListMode,
        PwdSaltTxt,
		CallGroupID,
		CallCenterID,
        Location,
		DialTypeId,
		Type)
    VALUES ( 
        @SID,
        @Name, 
        @FullName,
        @Description,
        @ManualSelection,
        @AssignmentsListMode, 
        @PwdSaltTxt,
		@CallGroupId,
		@CallCenterID,
        @Location,
		@DialTypeId,
		@Type)

INSERT BvPersonFailedLoginAttempts( PersonId, Count ) VALUES( @SID, 0 )

RETURN 0
GO
PRINT N'Altering [dbo].[BvSpState_Update]...';


GO
ALTER PROCEDURE [dbo].[BvSpState_Update]
 @ObjectID INT,
 @StateGroupID INT,
 @Name VARCHAR(255),
 @Priority INT,
 @DA BIT,
 @FcdAction INT
AS

DECLARE @OldPriority INT

SELECT @OldPriority = Priority
 FROM BvState 
 WHERE StateID = @ObjectID AND StateGroupID = @StateGroupID

UPDATE BvState 
 SET Priority = @Priority, [Name] = @Name, DA = @DA, FcdAction = @FcdAction
 WHERE StateID = @ObjectID AND StateGroupID = @StateGroupID

IF ( @OldPriority <> @Priority )
BEGIN

 DECLARE crSurveys CURSOR LOCAL FOR SELECT [SID] FROM [BvSurvey]

 DECLARE @SurveySID INT
 DECLARE @SurveyProcedureName NVARCHAR(128)
 DECLARE @SurveysProcessed INT

 OPEN crSurveys
 FETCH NEXT FROM crSurveys INTO @SurveySID

 WHILE ( @@fetch_status = 0 )
 BEGIN
  SET @SurveyProcedureName = 'BvSpSurveyState_Update'

  EXEC @SurveyProcedureName @ObjectID, @StateGroupID, @Priority

  SET @SurveysProcessed = @SurveysProcessed + 1

  FETCH NEXT FROM crSurveys INTO @SurveySID
 END

 CLOSE crSurveys
 DEALLOCATE crSurveys

END

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpSurvey_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpSurvey_Delete]
        @surveyID int
AS
    DECLARE @State INTEGER

	IF EXISTS( SELECT 1 FROM BvTasks WHERE SurveySID = @surveyID )
	BEGIN
		DECLARE @Name NVARCHAR(MAX) 
		SELECT @Name = name FROM BvSurvey WHERE SID = @surveyID
		RAISERROR( 'Survey with name = ''%s'' can''t be deleted, because active users exist for it survey', 16, 1, @name )
		RETURN -1
	END

    DELETE FROM BvThresholdITS WHERE SurveySID = @surveyID

    DELETE FROM BvMembership WITH(ROWLOCK)
    WHERE ObjectSID = @surveyID
    
    DELETE BvAppointment 
    WHERE SurveySID = @surveyID
    
    DELETE FROM BvSvySchedule 
    WHERE SurveySID = @surveyID

    DELETE BvPersonOrGroupAssignmentOnSurvey WHERE SurveyId = @surveyID 

	DELETE BvSurveyAssignmentOnCallCenter WHERE SurveyId = @surveyID 

    DELETE BvInterview WHERE SurveySID = @surveyID
    
    EXEC BvSpMembership_Delete 0, @surveyID
    
    --delete specific survey schedule params
    DELETE FROM BvScheduleParam WHERE SurveySID = @surveyID

    DELETE  BvSurvey WHERE SID = @surveyID
    DELETE FROM BvSampleStatusSummary WHERE SurveySID = @surveyID
    
    DECLARE @FilterSID INTEGER
    SELECT @FilterSID = SID FROM BvFilters WHERE [Name] = CAST( @surveyID AS NVARCHAR(255) )
    IF @FilterSID IS NOT NULL
    BEGIN
        DELETE FROM BvFilterFields WHERE FilterSID = @FilterSID
        DELETE FROM BvFilters WHERE SID = @FilterSID
    END
    
    DELETE FROM BvFilterFields
    FROM BvFilterFields
    INNER JOIN BvFilters ON ( SID = FilterSid )
    WHERE SurveySID = @surveyID

    DELETE FROM BvFilters WHERE SurveySID = @surveyID
    
    delete from bvpersonrel where type = 2 and objectsid = @surveyID
    
    delete from bvlogingroup where surveysid = @surveyID

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpSurvey_Insert]...';


GO
ALTER  PROCEDURE [dbo].[BvSpSurvey_Insert]
        @SID int,
        @Name nvarchar( 255 ),
        @Description nvarchar( 255 ),
        @QuotaType tinyint,
		@DialMode tinyint,
        @State int,
        @forceOpnRev int,
        @StateGroupID int,
        @RecWholeInt int,
		@InterviewScreenRecording bit,
        @RouteAddress NVARCHAR(255),
        @CfDbSchemaPath NVARCHAR(255),
        @DestinationTableName NVARCHAR (255), 
		@ReplicationStatus BIT,
		@ScheduleID INT,
		@DialerParameters NVARCHAR(MAX),
		@IsTelephoneBlacklistSupported BIT,
		@NotificationEmail NVARCHAR(MAX),
		@EnforceHttps BIT,
		@SurveySchedulingMode SMALLINT,
		@SurveySqlServerName NVARCHAR(255)
AS
BEGIN
	SET NOCOUNT ON


	IF @StateGroupID = 0
	BEGIN
		DECLARE @MinOrder INTEGER
		SELECT @MinOrder = MIN([Order]) FROM BvStateGroup
		SELECT @StateGroupID = [ID] FROM BvStateGroup WHERE [Order] = @MinOrder
	END


	IF ISNULL( @ScheduleID, 0 ) = 0
	BEGIN
		SELECT @ScheduleID = MIN( ScheduleID ) FROM BvSchedule
	END

	INSERT  BvSurvey( 
			SID, 
			[Name], 
			[Description],
			QuotaType,
			DialMode,
			State,
			ForceOpnRev,
			StateGroupID,
			RecWholeInt,
			InterviewScreenRecording,
			CfDbSchemaPath,
			DestinationTableName, 
			ReplicationStatus,
			ScheduleID,
			DialerParameters,
			IsTelephoneBlacklistSupported,
			[NotificationEmail],
			[EnforceHttps],
			SurveySchedulingMode,
			SurveySqlServerName
			)
		VALUES
		(
			@SID,
			@Name,
			@Description,
			@QuotaType,
			@DialMode,
			@State,
			@forceOpnRev,
			@StateGroupID,
			@RecWholeInt,
			@InterviewScreenRecording,
			@CfDbSchemaPath,
			@DestinationTableName, 
			@ReplicationStatus,
			@ScheduleID,
			@DialerParameters,
			@IsTelephoneBlacklistSupported,
			@NotificationEmail,
			@EnforceHttps,
			@SurveySchedulingMode,
			@SurveySqlServerName	
		)
        
	INSERT BvAggregateSurvey (SID) VALUES(@SID)
	INSERT BvAggregateSurveyAlertStatus (SID, Name, Description) VALUES(@SID, @Name, @Description)

	INSERT BvAppointmentCounters (SurveySID, SurveyName, ProjectID, CountForShortInterval, CountForLongInterval)
	VALUES(@SID, @Description, @Name, 0, 0)

	INSERT INTO BvSampleStatusSummary( SurveySID, ITS ) 
			SELECT @SID, StateID FROM BvState WHERE StateGroupID = @StateGroupID

	-- Add default schedule param of current scheduling script to BvScheduleParam table
	INSERT INTO BvScheduleParam( ScheduleID, SurveySID, ParamID, [Name], Description, Type, Value ) 
		SELECT sp.ScheduleID, @SID, sp.ParamID, sp.Name, sp.Description, sp.Type, sp.Value
					 FROM BvScheduleParam sp 
							WHERE sp.SurveySID = 0 AND sp.ScheduleID = @ScheduleID

	RETURN (0)
END
GO
PRINT N'Altering [dbo].[BvSpSurvey_Update]...';


GO
ALTER PROCEDURE [dbo].[BvSpSurvey_Update]
        @SID            int,
        @Name           nvarchar( 255 ),
        @Description        nvarchar( 255 ),
        @QuotaType      tinyint,
		@DialMode tinyint,
        @forceOpnRev int,
        @StateGroupID int,
        @RecWholeInt int,
		@InterviewScreenRecording bit,
		@DestinationTableName NVARCHAR (255), 
		@ReplicationStatus BIT,
		@ScheduleID INT,
		@DialerParameters NVARCHAR(MAX),
		@IsTelephoneBlacklistSupported BIT,
		@IsRespondentsDynamicCreationAllowed BIT,
		@NotificationEmail NVARCHAR(MAX),
		@EnforceHttps BIT,
		@LastTouchTime SMALLDATETIME,
		@SurveySchedulingMode SMALLINT,
		@ClusteredQuotaName NVARCHAR(256),
		@ClusteredQuotaThreshold INT,
		@HiddenSearchableFields NVARCHAR(256),
		@DialerId INT,
		@Target INT
AS
SET NOCOUNT ON

EXEC   BvSpSurveyModifyStateGroup @SID, @StateGroupID

DECLARE @OldSurveyDescription NVARCHAR( 255 )
DECLARE @OldScheduleID INT
DECLARE @OldSurveySchedulingMode INT

UPDATE  BvSurvey
    SET [Name]               = @Name,     
        @OldSurveyDescription = [Description],
        [Description]        = @Description,       
        QuotaType            = @QuotaType,
		DialMode             = @DialMode,         
        ForceOpnRev          = @forceOpnRev,
        StateGroupID         = @StateGroupID,
        RecWholeInt          = @RecWholeInt,
		InterviewScreenRecording = @InterviewScreenRecording,
        DestinationTableName = @DestinationTableName,
        ReplicationStatus    = @ReplicationStatus,
        ScheduleID           = @ScheduleID,
        @OldScheduleID       = ScheduleID,
        DialerParameters	 = @DialerParameters,
        IsTelephoneBlacklistSupported = @IsTelephoneBlacklistSupported,
		IsRespondentsDynamicCreationAllowed = @IsRespondentsDynamicCreationAllowed,
        NotificationEmail	 = @NotificationEmail,
		[EnforceHttps]       = @EnforceHttps,
        [LastTouchTime]      = @LastTouchTime,
		@OldSurveySchedulingMode = [SurveySchedulingMode],
        [SurveySchedulingMode] = @SurveySchedulingMode,
		ClusteredQuotaName   = @ClusteredQuotaName,
		ClusteredQuotaThreshold = @ClusteredQuotaThreshold,
		HiddenSearchableFields = @HiddenSearchableFields,
		DialerId			   = @DialerId,
		Target				   =@Target
    WHERE SID = @SID

-- SL. Should we use such optimization here? It works incorrectly with NULLs. BvSurvey allows NULL for the Description field.
IF (@OldSurveyDescription != @Description) 
BEGIN
   UPDATE BvAggregateSurveyAlertStatus
   SET Description = @Description
   WHERE SID = @SID
   
   UPDATE BvAppointmentsAlertStatus
   SET SurveyName = @Description
   WHERE SurveySID = @SID
   
   UPDATE BvAppointmentCounters
   SET SurveyName = @Description
   WHERE SurveySID = @SID
END

EXEC    BvSpMembership_Delete 0, @SID


IF @OldScheduleID <> @ScheduleID
BEGIN
    /*
     * change scheduling parameters
     */
    --delete specific survey schedule params
    DELETE FROM BvScheduleParam WHERE SurveySID = @SID
    -- Add default schedule param of current scheduling script to BvScheduleParam table
    INSERT INTO BvScheduleParam( ScheduleID, SurveySID, ParamID, [Name], Description, Type, Value ) 
        SELECT sp.ScheduleID, @SID, sp.ParamID, sp.[Name], sp.Description, sp.Type, sp.Value
            FROM BvScheduleParam sp 
                WHERE sp.SurveySID = 0 AND sp.ScheduleID = @ScheduleID
END

IF @OldSurveySchedulingMode <> @SurveySchedulingMode
BEGIN
	IF @SurveySchedulingMode = 0 
	BEGIN
		UPDATE BvSvySchedule SET ConditionValue = 0 WHERE SurveySID = @SID
	END
	ELSE
	BEGIN
		UPDATE BvSvySchedule 
			SET ConditionValue = TransientState
		FROM BvInterview 
			WHERE BvSvySchedule.SurveySID = @SID AND BvInterview.SurveySID = @SID AND BvSvySchedule.InterviewID = BvInterview.ID
	END
END

return 0
GO
PRINT N'Refreshing [dbo].[BvSpGetActiveCallsForSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetActiveCallsForSurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpGetAllPersonsAndGroups]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetAllPersonsAndGroups]';


GO
PRINT N'Refreshing [dbo].[BvSpGetPersonGroupsLevel]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetPersonGroupsLevel]';


GO
PRINT N'Refreshing [dbo].[BvSpGetSystemWideInfo]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSystemWideInfo]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_SpinUp]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_SpinUp]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonAndGroups_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonAndGroups_List]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonGroup_GetRootGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonGroup_GetRootGroup]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignment_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_List]';


GO
PRINT N'Refreshing [dbo].[BvSpAlert_RecalculateAppointment]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlert_RecalculateAppointment]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignmentResource_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignmentResource_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpGetDialerCallsBreakdown]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetDialerCallsBreakdown]';


GO
PRINT N'Refreshing [dbo].[BvSpGetExtendedCallHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetExtendedCallHistory]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_GetAssignedSurveyList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_GetAssignedSurveyList]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_GetAssignments]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_GetAssignments]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_UpdateBatched]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_UpdateBatched]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignment_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpCallCenter_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallCenter_Delete]';


GO
PRINT N'Update complete.';


GO
