DECLARE crPerson CURSOR FOR SELECT SID FROM BvPerson WHERE CallCenterId = 0
DECLARE @PersonId INT
OPEN crPerson
FETCH NEXT FROM crPerson INTO @PersonId
	
WHILE ( @@FETCH_STATUS = 0 ) 
BEGIN
	EXEC BvSpPerson_Delete @PersonId
	FETCH NEXT FROM crPerson INTO @PersonId
END

CLOSE crPerson
DEALLOCATE crPerson

DECLARE crPersonGroup CURSOR FOR SELECT SID FROM BvPersonGroup WHERE RoleId = 64
DECLARE @PersonGroupId INT
OPEN crPersonGroup
FETCH NEXT FROM crPersonGroup INTO @PersonGroupId
	
WHILE ( @@FETCH_STATUS = 0 ) 
BEGIN
	DELETE  BvMembership
    WHERE ContainerSID = @PersonGroupId OR ObjectSID = @PersonGroupId

    -- delete implicit assigments
    DELETE FROM BvPersonOrGroupAssignmentOnSurvey WHERE PersonOrGroupId = @PersonGroupId
        
    DELETE FROM BvPersonRel
    FROM BvPersonRel
    WHERE PersonSID = @PersonGroupId

    DELETE FROM BvNumber WHERE ObjectSID = @PersonGroupId AND ClassID = 65546

    DELETE  BvPersonGroup
        WHERE SID = @PersonGroupId

	FETCH NEXT FROM crPersonGroup INTO @PersonGroupId
END

CLOSE crPersonGroup
DEALLOCATE crPersonGroup



DELETE FROM BvHistory WHERE RoleID = 64

DELETE FROM BvPersonGroup WHERE RoleId = 64

DROP PROCEDURE BvSpSurveyProductivityReportCapi

EXEC sp_rename 'BvSpSurveyProductivityReportCati', 'BvSpSurveyProductivityReport'

GO
PRINT N'Dropping DF__BvPersonG__RoleI__27BC24D2...';


GO
ALTER TABLE [dbo].[BvPersonGroup] DROP CONSTRAINT [DF__BvPersonG__RoleI__27BC24D2];


GO
PRINT N'Dropping [dbo].[BvSpPersonGroup_GetParentGroupForSpecificRole]...';

GO
DROP PROCEDURE [dbo].[BvSpPersonGroup_GetParentGroupForSpecificRole];

GO
PRINT N'Altering [dbo].[BvPersonGroup]...';


GO
ALTER TABLE [dbo].[BvPersonGroup] DROP COLUMN [RoleID];


GO
PRINT N'Refreshing [dbo].[BvFnPersonAndGroup_Get]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvFnPersonAndGroup_Get]';


GO
PRINT N'Refreshing [dbo].[BvViewPersonAndGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvViewPersonAndGroup]';


GO
PRINT N'Refreshing [dbo].[RestView_Group]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_Group]';


GO
PRINT N'Altering [dbo].[BvSpGetAllPersonsAndGroups]...';


GO
ALTER  PROCEDURE [dbo].[BvSpGetAllPersonsAndGroups]
 @CallCenterId INT,
 @SurveyIdForExcludeAssignment INT
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
  SELECT SID as Id, Name, Description, CAST(1 AS BIT ) as IsGroup FROM BvPersonGroup pg ) d
 LEFT JOIN BvPersonOrGroupAssignmentOnSurvey pga 
 ON d.Id = pga.PersonOrGroupId AND 
    pga.SurveyId = @SurveyIdForExcludeAssignment AND 
    pga.CallCenterId = @CallCenterId
 WHERE pga.Id IS NULL OR @SurveyIdForExcludeAssignment IS NULL
GO

PRINT N'Altering [dbo].[BvSpHistory_CfData_Insert]...';

GO
ALTER PROCEDURE [dbo].[BvSpHistory_CfData_Insert]
    @ProjectID NVARCHAR(256),
    @RespondentPhone NVARCHAR(256),
    @FiredTime DATETIME,
    @InterviewID INT,
    @Status_CF NVARCHAR(256),
    @AppointmentID INT,
    @NetDuration INT,
    @GrossDuration INT,
    @TotalDuration INT,
    @InterviewerID INT,
    @RoleID INT
AS
DECLARE @SurveySID INT
DECLARE @InterviewerID_BF INT
DECLARE @StatusBvFEE INT

    -- get survey sid and validate it
    SELECT @SurveySID = [Sid] FROM [BvSurvey] WHERE [Name] = @ProjectID
    
    IF @SurveySID IS NULL
    BEGIN
        RAISERROR('Survey for project %s does not exist', 16, 1, @ProjectID)
        RETURN -1
    END

    -- get interviewer and validate it
    IF ( @roleID = 2 /* CATI */ )
    BEGIN
        IF NOT EXISTS ( SELECT [Sid] FROM [BvPerson] WHERE [Sid] = @InterviewerID )
        BEGIN
            --We should ingnore wrong interviewer, because interviewer can be alredy deleted
            SET @InterviewerID_BF = 0
        END
        
        SET @InterviewerID_BF = @InterviewerID
    END
    ELSE IF ( @RoleID = 64 /* CAPI */ )
    BEGIN
        RAISERROR('CAPI data isn''t supported now.', 16, 1)
        RETURN -1
    END
    
    -- get BvFEE status by CfStatus and validate it
    SELECT @StatusBvFEE = [StatusCode_BvFEE] FROM [BvConfirmitStatus]
        WHERE [StatusCode_Cnf] = @Status_CF OR ( @Status_CF IS NULL AND [StatusCode_Cnf] IS NULL )
        
    IF @StatusBvFEE IS NULL
    BEGIN
        SET @StatusBvFEE = 30 --ERROR ITS
    END
    
    --if BvFEE status is appointment we should get latests active appointmentId for the interview
    --because CF does not pass appID but it should be stored in [Hst_Path3] field
    SELECT @AppointmentID = MAX([ID]) FROM [BvAppointment]
		WHERE [SurveySID] = @SurveySID AND InterviewSID = @InterviewID AND [State] = 0 /* has not call*/
  
	SET @AppointmentID = ISNULL(@AppointmentID, 0) --if appointment does not exist

    INSERT INTO [BvHistory]
    (
            [SurveyId],
            [TelephoneNumber],
            [FiredTime],
            [InterviewID],
            [ITS],
            [AppointmentID],
            [WaitingTime],
            [ConfirmitDuration],
            [Duration],
            BatchId,
            [PersonSID],
            [RoleID],
			[CallCenterID]
    )
    SELECT
		@SurveySID      /*Hst_ObjID*/,
		@RespondentPhone /*TelephoneNumber*/,
		@FiredTime       /*FiredTime*/,
		@InterviewID     /*InterviewID*/,
		@StatusBvFEE     /*ITS*/,
		@AppointmentID   /*AppointmentID*/,
		it.WaitingTime     /*WaitingTime*/,
		@GrossDuration /*ConfirmitDuration*/,
		ISNULL(it.InterviewDuriationTime, @TotalDuration) /*Duration*/,
		0               /*BatchId*/,
		@InterviewerID_BF /*PersonSID*/,
		@RoleID          /*RoleID*/,
		ISNULL( it.CallCenterID, 0 )
    FROM (
			SELECT @SurveySID SurveySID,
			       @InterviewID InterviewID
		 ) CfData
    LEFT JOIN BvInterviewTimings it ON CfData.SurveySID = it.SurveyID AND
                                       CfData.InterviewID = it.InterviewID
                                       
    DELETE FROM BvInterviewTimings
    WHERE InterviewID = @InterviewID AND
          SurveyID = @SurveySID

RETURN 0

GO

PRINT N'Altering [dbo].[BvSpGetPersonsLevel]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetPersonsLevel]
 @ParentSID INT,
 @Filter NVARCHAR(MAX) = NULL,
 @CallCenterID INT
AS
SELECT
 [p].[SID] AS [SID],
 [p].[Name] AS [Name]
FROM   
 BvFnPerson_Get(@CallCenterID) [p]
 LEFT JOIN [BvMembership] [m] ON [p].[SID] = [m].[ObjectSID]
WHERE
 [m].[ContainerSID] = @ParentSID
 AND (@Filter IS NULL OR [p].[Name] LIKE @Filter)
GO
PRINT N'Altering [dbo].[BvSpGetSystemWideInfo]...';


GO
ALTER PROCEDURE BvSpGetSystemWideInfo
   @BatchID INT,
   @CallCenterID INT
AS  
        --1. InterviewersLoggedCount thresholds
        DECLARE @AmberOfInterviewersLoggedCountSWI INT
        DECLARE @RedOfInterviewersLoggedCountSWI INT
        SELECT @AmberOfInterviewersLoggedCountSWI = Amber, @RedOfInterviewersLoggedCountSWI = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 12/*SystemWideInfo.LoggedInterviewersCount alert*/

        --2. OpenSurveysCount thresholds
        DECLARE @AmberOfOpenSurveysCount INT
        DECLARE @RedOfOpenSurveysCount INT
        SELECT @AmberOfOpenSurveysCount = Amber, @RedOfOpenSurveysCount = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 13/*SystemWideInfo.OpenSurveysCount alert*/

        --3. CallsCount thresholds
        DECLARE @AmberOfCallsCount INT
        DECLARE @RedOfCallsCount INT
        SELECT @AmberOfCallsCount = Amber, @RedOfCallsCount = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 14/*SystemWideInfo.CallsCount alert*/


        DECLARE @count INT;
		DECLARE @countOpenSurveys INT
        DECLARE @totalInterviewers INT
        DECLARE @loggedinterviewers INT        
		DECLARE @totalInterviewersWorkedToday INT

        SELECT @count = ISNULL(SUM(StrikeRate),0)
        FROM BvAggregateSurveyAlertStatus asas
        INNER JOIN BvSurvey s ON (s.SID = asas.SID)
        INNER JOIN BvTransferArrays ta ON (ta.BatchID = @BatchID AND
                                           ta.ItemID = s.SID)
                                                  
        SELECT @totalInterviewers = COUNT(DISTINCT Person.SID) FROM BvFnPerson_Get(@CallCenterID)  Person INNER JOIN 
					 BvMembership ON Person.SID = ObjectSID INNER JOIN 
					 BvPersonGroup ON BvMembership.ContainerSID = BvPersonGroup.SID
        
		SELECT @totalInterviewersWorkedToday = COUNT(DISTINCT BvInterviewerPerformance.InterviewerId) FROM BvInterviewerPerformance

        SELECT @loggedinterviewers = COUNT(*)
        FROM BvTasks
        WHERE StatusLogout != 0 --logged out

        SELECT @countOpenSurveys = COUNT(*)
        FROM BvSurvey s
        INNER JOIN BvTransferArrays ta ON (ta.BatchID = @BatchID AND
                                           ta.ItemID = s.SID)
        WHERE s.State = 1 /*open*/
               
        SELECT         
			@totalInterviewers as TotalInterviewersCount,
			@loggedinterviewers as LoggedInterviewersCount,
            @countOpenSurveys as OpenSurveysCount,
			@totalInterviewersWorkedToday as TotalInterviewersWorkedTodayCount,
            @count as CallsCount,
            dbo.udf_AlertStatus_INT(@loggedinterviewers, @AmberOfInterviewersLoggedCountSWI, @RedOfInterviewersLoggedCountSWI) as AlertStatusOfLoggedInterviewersCount,
            dbo.udf_AlertStatus_INT(@countOpenSurveys, @AmberOfOpenSurveysCount, @RedOfOpenSurveysCount) as AlertStatusOfOpenSurveysCount,
            dbo.udf_AlertStatus_INT(@count, @AmberOfCallsCount, @RedOfCallsCount) as AlertStatusOfCallsCount
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
    
    delete from BvPersonRel where PersonSID = @PersonSID
    insert into BvPersonRel( PersonSID, ObjectSID, RoleID, Type )
        select @PersonSID, sid, role_id, type from @temp

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpPersonGroup_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpPersonGroup_Insert]
        @SID                int,
        @Name               nvarchar( 255 ),
        @Description        nvarchar( 255 ),
        @ManualSelection    int,
        @IsUser             int,
        @IsSelection        int,
        @BvID               int

AS
IF EXISTS ( SELECT [SID] FROM BvPersonGroup WHERE [Name] = @Name )
BEGIN
 RAISERROR('Person group with name %s already exists', 12, 2, @Name)
 RETURN -1
END

IF ISNULL( @BvID, 0 ) > 0
BEGIN
    EXEC @BvID = BvSpSetObjectNumber @SID, 65546, @BvID
    IF @BvID = -1
        RETURN ( 50006 )
END

INSERT  BvPersonGroup( 
        SID,
        [Name],
        [Description], 
        ManualSelection )
    VALUES( 
        @SID, 
        @Name,
        @Description, 
        @ManualSelection )

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
	   BvPersonGroup.ManualSelection
	FROM BvPersonGroup
	LEFT JOIN BvMembership ON BvPersonGroup.SID = BvMembership.ObjectSID AND
							  BvMembership.ContainerSID = @ParentGroupId
	WHERE BvMembership.ObjectSID IS NULL
ELSE --child groups for passed parent group
	SELECT DISTINCT
	   BvPersonGroup.SID,
	   BvPersonGroup.Name,
	   BvPersonGroup.Description,
	   BvPersonGroup.ManualSelection
	FROM BvPersonGroup
	INNER JOIN BvMembership ON BvPersonGroup.SID = BvMembership.ObjectSID AND
							  BvMembership.ContainerSID = @ParentGroupId
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

UPDATE  BvPersonGroup
    SET [Name] = @Name,
    [Description] = @Description,
    ManualSelection = @ManualSelection
    WHERE SID = @SID

IF ISNULL( @BvID, 0 ) > 0
 UPDATE BvNumber SET BvID = @BvID 
 WHERE ObjectSID = @SID AND ClassID = 65546
ELSE
    DELETE FROM BvNumber 
    WHERE ObjectSID = @SID AND ClassID = 65546

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpPersonGroup_GetRootGroup]...';


GO
CREATE PROCEDURE [dbo].[BvSpPersonGroup_GetRootGroup]
AS
	SELECT pg.SID
	FROM BvPersonGroup pg
	LEFT JOIN BvMembership m ON pg.Sid = m.ObjectSID
	WHERE m.ObjectSID IS NULL
RETURN 0
GO
PRINT N'Refreshing [dbo].[BvSpGetActiveCallsForSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetActiveCallsForSurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpGetPersonGroupsLevel]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetPersonGroupsLevel]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonAndGroups_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonAndGroups_List]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonGroup_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonGroup_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignment_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_List]';


GO
PRINT N'Refreshing [dbo].[BvSpAlert_RecalculateAppointment]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlert_RecalculateAppointment]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_GetAssignedSurveyList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_GetAssignedSurveyList]';


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
