PRINT N'Altering [dbo].[BvSurvey]...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD [HiddenSearchableFields] NVARCHAR (256) CONSTRAINT [DF_BvSurvey_HiddenSearchableFields] DEFAULT ('TimeToCall') NOT NULL;


GO
PRINT N'Refreshing [dbo].[BvFnSurvey_GetByCallCenterId]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvFnSurvey_GetByCallCenterId]';


GO
PRINT N'Refreshing [dbo].[BvFnSurvey_GetByTransferBatch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvFnSurvey_GetByTransferBatch]';


GO
PRINT N'Refreshing [dbo].[RestView_BreakHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_BreakHistory]';


GO
PRINT N'Refreshing [dbo].[RestView_CallHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_CallHistory]';


GO
PRINT N'Refreshing [dbo].[RestView_Interviewer]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_Interviewer]';


GO
PRINT N'Refreshing [dbo].[RestView_Project]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_Project]';


GO
PRINT N'Altering [dbo].[BvSpGetSurveyInterviews]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetSurveyInterviews]
@SurveySID INT, @PersonSID INT, @AssignmentsListmode INT, @ConfirmitVariablePrefix NVARCHAR(MAX),  @filterQuery NVARCHAR (MAX) = NULL, @InterviewsCountShownInManualMode INT
AS
SET NOCOUNT ON	
	
	DECLARE
		@sql          AS NVARCHAR(MAX),
		@selectSql    AS NVARCHAR(MAX),
		@whereSql     AS NVARCHAR(MAX),
		@replicatedColumns	 AS NVARCHAR(MAX),
		@replicatedColumnsAliases   AS NVARCHAR(MAX),
		@replicatedDataTable AS NVARCHAR(MAX)
    
	SET @replicatedColumns = ''
	SET @replicatedColumnsAliases = ''
	SET @replicatedDataTable = 'BvReplicatedData_'+ CAST( @SurveySID AS VARCHAR(10) )

	IF EXISTS ( SELECT 1 
                FROM BvTasks 
                INNER JOIN BvPerson ON PersonSid = SID
                WHERE PersonSID = @PersonSID AND ManualSelection != 1 ) 
    BEGIN
        RETURN (0)
    END
	
	CREATE TABLE #replicatedColumnsNames (
        [ColumnName] NVARCHAR(MAX) NOT NULL
    )
    
    INSERT INTO #replicatedColumnsNames SELECT ColumnName 
					FROM BvReplicationColumns
				    INNER JOIN BvSearchableFields ON 
					BvReplicationColumns.ColumnID = BvSearchableFields.ColumnId AND 
					BvReplicationColumns.TableID = BvSearchableFields.TableID
					WHERE SurveyId = @SurveySID AND
					BvSearchableFields.UseMode = 0 -- Use in Console
       	
       	UPDATE #replicatedColumnsNames 
       	SET    @replicatedColumns = @replicatedColumns+ ',' + @replicatedDataTable + '.' + '[' + ColumnName + ']' + ' AS ' + @ConfirmitVariablePrefix + ColumnName + ' '
       	FROM #replicatedColumnsNames
       	       	
       	UPDATE #replicatedColumnsNames 
       	SET    @replicatedColumnsAliases = @replicatedColumnsAliases+ ',' + @ConfirmitVariablePrefix + ColumnName + ' '
       	FROM #replicatedColumnsNames       	       	
                           
    SET @selectSql = 'SELECT BvSvySchedule.[InterviewID],
 BvInterview.[RespondentName],
 BvInterview.[TelephoneNumber], 
 BvInterview.[TimezoneID],
 BvState.[Name] as [ITSName],
 BvSvySchedule.[Priority] as [Priority],
 BvSvySchedule.[TimeInShift] as [TimeToCall]'+
  @replicatedColumns+
 'FROM BvSvySchedule
 INNER JOIN BvSurvey ON BvSurvey.SID = BvSvySchedule.SurveySID AND BvSurvey.SID = ' + CAST(@SurveySID AS VARCHAR(16)) +'
 INNER JOIN BvInterview ON BvInterview.SurveySID = BvSvySchedule.SurveySID  AND BvInterview.[ID] = BvSvySchedule.InterviewID  AND ( BvInterview.TransientState <> 13 )'
 IF @AssignmentsListmode = 0
 BEGIN
	SET @selectSql = @selectSql + ' INNER JOIN BvLoginGroup WITH (NOLOCK) ON BvLoginGroup.PersonSID = ' + CAST(@PersonSID AS VARCHAR(16)) + ' AND BvLoginGroup.ObjectSID = BvSvySchedule.ExplicitSID'
 END

 SET @selectSql = @selectSql + ' INNER JOIN BvState ON BvState.StateGroupID = BvSurvey.StateGroupID AND BvState.StateID = BvInterview.TransientState
 LEFT JOIN '+ @replicatedDataTable + ' ON respId = InterviewID 
 WHERE BvSvySchedule.CallState = 2 AND BvSvySchedule.SurveySID = ' + CAST(@SurveySID AS VARCHAR(16))
 
	
	IF(@filterQuery IS NOT NULL AND @filterQuery <> '')			
		SET @whereSql = ' WHERE ' + @filterQuery;
	ELSE
		SET @whereSql = '';
			
	--Need this construction to perform filtration using aliases
	SET @sql = 'SELECT TOP (' + cast(@InterviewsCountShownInManualMode as varchar(10)) + ')
				InterviewID, RespondentName, TelephoneNumber, ITSName, TimeToCall, TimezoneID ' + @replicatedColumnsAliases +
			   'FROM (' + @selectSql + ')S ' + @whereSql + 'ORDER BY Priority DESC'
print @sql
	EXECUTE sp_executesql @sql

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
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
        @BvID bigint,
		@DestinationTableName NVARCHAR (255), 
		@ReplicationStatus BIT,
		@ScheduleID INT,
		@DialerParameters NVARCHAR(MAX),
		@IsTelephoneBlacklistSupported BIT,
		@NotificationEmail NVARCHAR(MAX),
		@EnforceHttps BIT,
		@LastTouchTime SMALLDATETIME,
		@SurveySchedulingMode SMALLINT,
		@ClusteredQuotaName NVARCHAR(256),
		@ClusteredQuotaThreshold INT,
		@HiddenSearchableFields NVARCHAR(256)
AS
SET NOCOUNT ON

EXEC   BvSpSurveyModifyStateGroup @SID, @StateGroupID

IF ISNULL( @BvID, 0 ) > 0
BEGIN
    IF EXISTS( 
        SELECT 1 FROM BvNumber 
        WHERE BvID = @BvID AND ClassID = 2 AND ObjectSID != @SID
    )
    BEGIN
        RAISERROR( 'BvID = %I64d already exists', 16, 1, @BvID )
        RETURN -1
    END
END

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
        NotificationEmail	 = @NotificationEmail,
		[EnforceHttps]       = @EnforceHttps,
        [LastTouchTime]      = @LastTouchTime,
		@OldSurveySchedulingMode = [SurveySchedulingMode],
        [SurveySchedulingMode] = @SurveySchedulingMode,
		ClusteredQuotaName   = @ClusteredQuotaName,
		ClusteredQuotaThreshold = @ClusteredQuotaThreshold,
		HiddenSearchableFields = @HiddenSearchableFields
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

IF ISNULL( @BvID, 0 ) > 0
BEGIN
    IF EXISTS ( SELECT 1 FROM BvNumber WHERE ObjectSID = @SID )
        UPDATE BvNumber SET BvID = @BvID 
        WHERE ObjectSID = @SID AND ClassID = 2
    ELSE
        INSERT INTO BvNumber ( ObjectSID, ClassID, BvID )
            VALUES ( @SID, 2, @BvID )
END
ELSE
    EXEC BvSpBvID_Delete 2, @SID

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
PRINT N'Refreshing [dbo].[BvSpAlert_RecalculateAll]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlert_RecalculateAll]';


GO
PRINT N'Refreshing [dbo].[BvSpAlert_RecalculateAppointment]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlert_RecalculateAppointment]';


GO
PRINT N'Refreshing [dbo].[BvSpAssignment_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_List]';


GO
PRINT N'Refreshing [dbo].[BvSpAttemptsByDispositionReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAttemptsByDispositionReport]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_Activate]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_Activate]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_ChangeShiftType]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_ChangeShiftType]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_MoveToITS]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_MoveToITS]';


GO
PRINT N'Refreshing [dbo].[BvSpCallHistory_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistory_List]';


GO
PRINT N'Refreshing [dbo].[BvSpCallHistoryData]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistoryData]';


GO
PRINT N'Refreshing [dbo].[BvSpCfUpdateSurveyReplicationStatus]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCfUpdateSurveyReplicationStatus]';


GO
PRINT N'Refreshing [dbo].[BvSpCheckCallOnShifts]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCheckCallOnShifts]';


GO
PRINT N'Refreshing [dbo].[BvSpExecuteForAllSurveys]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpExecuteForAllSurveys]';


GO
PRINT N'Refreshing [dbo].[BvSpFilter_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpFilter_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpGetAllAppointmentsForUser]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetAllAppointmentsForUser]';


GO
PRINT N'Refreshing [dbo].[BvSpGetInterviewerBreaks]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviewerBreaks]';


GO
PRINT N'Refreshing [dbo].[BvSpGetLiveShifts]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetLiveShifts]';


GO
PRINT N'Refreshing [dbo].[BvSpGetOpenedSurveys]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetOpenedSurveys]';


GO
PRINT N'Refreshing [dbo].[BvSpGetReplicatedTable]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetReplicatedTable]';


GO
PRINT N'Refreshing [dbo].[BvSpGetSurveyActivityWithAlerts]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSurveyActivityWithAlerts]';


GO
PRINT N'Refreshing [dbo].[BvSpGetSurveysWithSurveySpecificFilters]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSurveysWithSurveySpecificFilters]';


GO
PRINT N'Refreshing [dbo].[BvSpGetSystemWideInfo]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSystemWideInfo]';


GO
PRINT N'Refreshing [dbo].[BvSpHistory_CfData_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpHistory_CfData_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpInterview_CfData_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterview_CfData_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpInterview_UpdateRespondentFields]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterview_UpdateRespondentFields]';


GO
PRINT N'Refreshing [dbo].[BvSpInterviewsAndAppointments_Delete_Batch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviewsAndAppointments_Delete_Batch]';


GO
PRINT N'Refreshing [dbo].[BvSpInterviewTimings_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviewTimings_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_GetAssignedSurveyList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_GetAssignedSurveyList]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonAndGroups_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonAndGroups_List]';


GO
PRINT N'Refreshing [dbo].[BvSpReportSampleStatusSummary]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpReportSampleStatusSummary]';


GO
PRINT N'Refreshing [dbo].[BvSpSampleStatusSummary_Get]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSampleStatusSummary_Get]';


GO
PRINT N'Refreshing [dbo].[BvSpSchedule_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSchedule_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpScheduleParam_Launch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpScheduleParam_Launch]';


GO
PRINT N'Refreshing [dbo].[BvSpSetCallDeliveryMode]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSetCallDeliveryMode]';


GO
PRINT N'Refreshing [dbo].[BvSpState_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpState_List]';


GO
PRINT N'Refreshing [dbo].[BvSpState_ListBySurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpState_ListBySurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpState_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpState_Update]';


GO
PRINT N'Refreshing [dbo].[BvSpStateGroup_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpStateGroup_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_GetAssignedPersonList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_GetAssignedPersonList]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_GetOpened]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_GetOpened]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyCleanup_GetSurveysWhichAreReadyForCleanup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyCleanup_GetSurveysWhichAreReadyForCleanup]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyCleanup_GetSurveysWhichAreReadyForNotice]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyCleanup_GetSurveysWhichAreReadyForNotice]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyModifyStateGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyModifyStateGroup]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyOverviewReportForAllPersons]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyOverviewReportForAllPersons]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyProductivityReportCapi]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyProductivityReportCapi]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyProductivityReportCati]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyProductivityReportCati]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyState_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyState_Update]';


GO
PRINT N'Refreshing [dbo].[BvSpSvySch_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSvySch_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_InsertUpdate_2]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_InsertUpdate_2]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_Update_2]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_Update_2]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateInterviewState]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateInterviewState]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateStatusLogout]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateStatusLogout]';


GO
PRINT N'Refreshing [dbo].[BvSpUserSurveyPermission_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpUserSurveyPermission_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpUserSurveyPermission_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpUserSurveyPermission_Insert]';


GO
PRINT N'Refreshing [dbo].[SetDialerSurveyParametersWhereIsNull]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[SetDialerSurveyParametersWhereIsNull]';


GO
PRINT N'Refreshing [dbo].[BvSpGetSurveys]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSurveys]';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_GetListByFolder]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_GetListByFolder]';


GO
PRINT N'Refreshing [dbo].[BvSpGetListSurveyTasks]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetListSurveyTasks]';


GO
PRINT N'Refreshing [dbo].[BvSpQueueUpSheduleTask3]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpQueueUpSheduleTask3]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyOverviewReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyOverviewReport]';


GO
PRINT N'Update complete.';


GO
