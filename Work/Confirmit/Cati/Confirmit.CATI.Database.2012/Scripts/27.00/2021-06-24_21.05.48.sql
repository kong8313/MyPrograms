GO
PRINT N'Altering [dbo].[BvPerson]...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD [EnableSoftphoneIntegration] BIT CONSTRAINT [DF_BvPerson_EnableSoftphoneIntegration] DEFAULT (1) NOT NULL;


GO
PRINT N'Refreshing [dbo].[BvViewPersonAndGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvViewPersonAndGroup]';


GO
PRINT N'Refreshing [dbo].[RestView_CallHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_CallHistory]';


GO
PRINT N'Refreshing [dbo].[RestView_Interviewer]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_Interviewer]';


GO
PRINT N'Refreshing [dbo].[BvFnPerson_Get]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvFnPerson_Get]';


GO
PRINT N'Refreshing [dbo].[BvFnPerson_GetByTransferBatch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvFnPerson_GetByTransferBatch]';


GO
PRINT N'Refreshing [dbo].[BvFnPersonAndGroup_Get]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvFnPersonAndGroup_Get]';


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
		@Type TINYINT,
        @EnableSoftphoneIntegration BIT
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
		Type,
        EnableSoftphoneIntegration)
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
		@Type,
        @EnableSoftphoneIntegration)

INSERT BvPersonFailedLoginAttempts( PersonId, Count ) VALUES( @SID, 0 )

RETURN 0
GO
PRINT N'Altering [dbo].[BvSpGetPersonsListPage]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetPersonsListPage]
 @ParentGroupsIDs NVARCHAR (MAX), 
 @PageIndex INT,
 @PageSize INT, 
 @OrderField NVARCHAR (64), 
 @IsOrderASC BIT, 
 @SearchCondition NVARCHAR (4000)=NULL,
 @CallCenterId INT
AS
BEGIN
 IF @ParentGroupsIDs IS NULL AND @PageIndex IS NULL
 BEGIN
 /* Looks like we're generating code using FMTONLY. So lets return metadata*/
 SELECT
     0  AS PersonSID,
     '' AS PersonName,
     '' AS PersonDescription,
     CAST(0 as BIT)  AS LoggedIn,
     0 as ManualSelection,
     0 as AllowedChoices,
     '' as SurveyID,
     CAST(0 as BIT)  AS IsLocked,
     CAST(NULL AS DATETIME) AS LockedDate,
	 0 AS CallGroupId,
	 '' AS CallGroupName,
     '' AS PersonLocation,
	 CAST(0 as TINYINT) AS DialTypeId,
	 CAST(0 as TINYINT) AS [Type],
	 '' AS GroupNamesJson,
     CAST(0 as BIT) AS EnableSoftphoneIntegration
     RETURN 0;
 END
 
 DECLARE @Query nvarchar(max)
 DECLARE @IDField nvarchar(64)
 SET @IDField = 'PersonSID'
   
 SET @Query = 
   N'SELECT DISTINCT [BvPerson].[SID] PersonSID,
    [BvPerson].[Name] PersonName,
    [BvPerson].[Description] PersonDescription,
    cast((case when t.[PersonSID] is null 
       then 0
       else 1 
    end) as bit) as [LoggedIn],
    [BvPerson].[ManualSelection] as ManualSelection,
    [BvPerson].[AllowedChoices] as AllowedChoices,
    ISNULL ( s.Name, '''' ) as [SurveyID],
    [BvPerson].[IsLocked] as IsLocked,
    [BvPerson].[LockedDate] as LockedDate,
	[BvCallGroup].[ID] as CallGroupId,
	ISNULL( [BvCallGroup].[Name], '''' ) as CallGroupName,
	[BvPerson].[Location] as PersonLocation,
	[BvPerson].[DialTypeId],
    [BvPerson].[EnableSoftphoneIntegration] as EnableSoftphoneIntegration,
	[BvPerson].[Type],
	''['' + (
		SELECT ''"'' + STRING_ESCAPE(BvPersonGroup.Name, ''json'') + ''",'' AS [text()]
		FROM [BvPersonGroup]
		INNER JOIN [BvMembership] ON BvPersonGroup.SID = BvMembership.ContainerSID
		WHERE BvMembership.ObjectSID = BvPerson.SID
		ORDER BY BvMembership.ObjectSID
		FOR XML PATH ('''')
	) + '']'' as GroupNamesJson
    FROM BvFnPerson_Get(' + CAST( @CallCenterId AS NVARCHAR(64)) + ') as BvPerson
	LEFT JOIN [BvCallGroup]
	  ON [BvPerson].[CallGroupID] = [BvCallGroup].ID
    LEFT JOIN [dbo].[BvMembership]
      ON [BvMembership].[ObjectSID] = [BvPerson].[SID]
    LEFT JOIN dbo.BvTasks t
      on [BvPerson].SID = t.PersonSID
    LEFT JOIN  dbo.bvsurvey s
       on s.SID = t.SurveySID and s.State <> 2
    WHERE [BvPerson].[SID] = [BvMembership].[ObjectSID] 
    AND [BvMembership].[ContainerSID] in (' + @ParentGroupsIDs + ')'
   
   IF @OrderField = '' OR @OrderField = null
   SET @OrderField = 'PersonSID' 
   
   DECLARE @TotalCount INT

   EXEC @TotalCount = BvSpGetObjectsPage @PageIndex, @PageSize, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition
   RETURN @TotalCount
END
GO
PRINT N'Refreshing [dbo].[BvSpAggregateInterviewerPerformance]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAggregateInterviewerPerformance]';


GO
PRINT N'Refreshing [dbo].[BvSpAlertsHistoryAggregatedReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlertsHistoryAggregatedReport]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignment_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpCallHistory_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistory_List]';


GO
PRINT N'Refreshing [dbo].[BvSpCallHistoryData]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistoryData]';


GO
PRINT N'Refreshing [dbo].[BvSpGetActiveCallsForSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetActiveCallsForSurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpGetInterviewerBreaks]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviewerBreaks]';


GO
PRINT N'Refreshing [dbo].[BvSpGetLoggedInPersonsCount]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetLoggedInPersonsCount]';


GO
PRINT N'Refreshing [dbo].[BvSpGetMessages]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetMessages]';


GO
PRINT N'Refreshing [dbo].[BvSpGetOpenedSurveys]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetOpenedSurveys]';


GO
PRINT N'Refreshing [dbo].[BvSpGetSurveyInterviews]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSurveyInterviews]';


GO
PRINT N'Refreshing [dbo].[BvSpGetSystemWideInfo]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSystemWideInfo]';


GO
PRINT N'Refreshing [dbo].[BvSpGetUserGroups]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetUserGroups]';


GO
PRINT N'Refreshing [dbo].[BvSpInterviewerProductivityReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviewerProductivityReport]';


GO
PRINT N'Refreshing [dbo].[BvSpLogin_SpinUp]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLogin_SpinUp]';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson_ForManualMode]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson_ForManualMode]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_GetPersonsWithWrongAutomaticSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_GetPersonsWithWrongAutomaticSurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_GetSurveys]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_GetSurveys]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_ListByParent]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_ListByParent]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_ListWithTasksByType]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_ListWithTasksByType]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_SetAutomaticSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_SetAutomaticSurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_SpinUp]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_SpinUp]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_UpdateBatched]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_UpdateBatched]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonCheckForNewMessage]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonCheckForNewMessage]';


GO
PRINT N'Refreshing [dbo].[BvSpSendMessageToGroups]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSendMessageToGroups]';


GO
PRINT N'Refreshing [dbo].[BvSpSendMessageToInterviewers]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSendMessageToInterviewers]';


GO
PRINT N'Refreshing [dbo].[BvSpSendMessageToSurveys]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSendMessageToSurveys]';


GO
PRINT N'Refreshing [dbo].[BvSpSetNextInterviewForPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSetNextInterviewForPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpStartInterviewerBreak]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpStartInterviewerBreak]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_DeassignFromCallCenter]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_DeassignFromCallCenter]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_GetCountOfLoggedPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_GetCountOfLoggedPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyOverviewReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyOverviewReport]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyProductivityReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyProductivityReport]';


GO
PRINT N'Refreshing [dbo].[BvSpGetAllPersonsAndGroups]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetAllPersonsAndGroups]';


GO
PRINT N'Refreshing [dbo].[BvSpGetInterviewerPerformanceList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviewerPerformanceList]';


GO
PRINT N'Refreshing [dbo].[BvSpGetListSurveyTasks]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetListSurveyTasks]';


GO
PRINT N'Refreshing [dbo].[BvSpGetPersonGroups]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetPersonGroups]';


GO
PRINT N'Refreshing [dbo].[BvSpGetPersonGroupsLevel]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetPersonGroupsLevel]';


GO
PRINT N'Refreshing [dbo].[BvSpGetPersonsLevel]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetPersonsLevel]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonAndGroups_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonAndGroups_List]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_GetAssignedPersonList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_GetAssignedPersonList]';


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
PRINT N'Refreshing [dbo].[BvSpPersonGroup_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonGroup_Update]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonGroup_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonGroup_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignment_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpCallCenter_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallCenter_Delete]';


GO
PRINT N'Add Console.EnableSoftphoneIntegration system settings';


GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Console.EnableSoftphoneIntegration', 'Enable softphone integration for BBCC', 'Interviewing', 'Enable softphone integration for BBCC', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END


GO
PRINT N'Update complete.';


GO
