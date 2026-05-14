GO
PRINT N'Altering [dbo].[BvSurvey]...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD [IsLiveMonitoringEnabled] BIT CONSTRAINT [DF_BvSurvey_IsLiveMonitoringEnabled] DEFAULT (1) NOT NULL;


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
PRINT N'Refreshing [dbo].[RestView_Survey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_Survey]';


GO
PRINT N'Refreshing [dbo].[BvFnSurvey_GetByCallCenterId]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvFnSurvey_GetByCallCenterId]';


GO
PRINT N'Refreshing [dbo].[BvFnSurvey_GetByTransferBatch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvFnSurvey_GetByTransferBatch]';

GO
PRINT N'Altering [dbo].[BvPersonDeferredMonitoring]...';


GO
ALTER TABLE [dbo].[BvPersonDeferredMonitoring]
    ADD [ScreenRecordStartTime] DATETIME NULL;


GO
PRINT N'Refreshing [dbo].[BvSpCleanDeferredMonitoring]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCleanDeferredMonitoring]';


GO
PRINT N'Refreshing [dbo].[BvSpGetDeferredMonitoringStartFile]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetDeferredMonitoringStartFile]';


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
		@Target INT,
		@InternalTransferType TINYINT,
		@ExternalTransferType TINYINT,
		@IsLiveMonitoringEnabled BIT
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
		Target				   =@Target,
		InternalTransferType = @InternalTransferType,
		ExternalTransferType = @ExternalTransferType,
		IsLiveMonitoringEnabled = @IsLiveMonitoringEnabled
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
PRINT N'Altering [dbo].[BvSpGetListSurveyTasks]...';


GO
ALTER PROCEDURE BvSpGetListSurveyTasks
   @surveysBatchID int,
   @interviewersBatchID int,   
   @TimeZoneID INT,
   @CallCenterID INT,
   @UserName NVARCHAR(MAX)

AS
   DECLARE @currTime DATETIME
   EXEC @currTime = GetUtcNow
   DECLARE @AmberOfLastSubmission INT
   DECLARE @RedOfLastSubmission INT
   DECLARE @AmberOfLastKeepAliveTime INT
   DECLARE @RedOfLastKeepAliveTime INT
   DECLARE @AmberOfNoActivity INT
   DECLARE @RedOfNoActivity INT
   DECLARE @AmberOfInterviewDuration INT
   DECLARE @RedOfInterviewDuration INT
   DECLARE @AmberOfBreakDuration INT
   DECLARE @RedOfBreakDuration INT

   SELECT @AmberOfLastSubmission = Amber, @RedOfLastSubmission = Red
   FROM BvThresholds 
   WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 1/*Task alert*/

   SELECT @AmberOfLastKeepAliveTime = Amber, @RedOfLastKeepAliveTime = Red
   FROM BvThresholds 
   WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 16/*Last keep alive alert*/
   
   SELECT @AmberOfNoActivity = Amber, @RedOfNoActivity = Red
   FROM BvThresholds 
   WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 18/*No activity alert*/

   SELECT @AmberOfInterviewDuration = Amber, @RedOfInterviewDuration = Red
   FROM BvThresholds 
   WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 19/*Interview duration alert*/

   SELECT @AmberOfBreakDuration = Amber, @RedOfBreakDuration = Red
   FROM BvThresholds 
   WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 20/*Break duration alert*/
   
   SELECT tsk.InterviewID, 
          tsk.PersonSID, 
          p.Name as InterviewerName, 
          tsk.SurveySID, 
          tsk.ProjectID, 
          tsk.SurveyName,
          tsk.TimeCallDelivered, 
          tsk.State, 
          tsk.SecondsSinceLastSubmission, 
          tsk.LastSubmissionAlert, 
          tsk.LastKeepAliveTime,
          tsk.LastKeepAliveTimeAlert,
          tsk.EndOfLastActivityAlert,
		  tsk.InterviewDurationAlert,
		  tsk.BreakDurationAlert,
          tsk.InterviewState,
          tsk.LoggedInToDialerState,
          tsk.TzID, 
          tsk.DiallingMode, -- if no survey assigned to task - use manual dialing mode
          tsk.CallOutcome, 
          tsk.StatusLogout,
          tsk.ProblemId,
          tz.Bias, 
          pm.supervisorName,
          pm.MonitoringSessionID,
          tsk.StationId,
		  tsk.DialType,
		  tsk.OpenEndReviewInSeconds,
		  tsk.DialerId,
		  p.Type,
		  tsk.CallType,
		  tsk.LinkedChain,
		  tsk.CallConnectionState,
		  tsk.BreakTypeName,
		  tsk.JsonContext,
		  tsk.InterviewScreenRecording,
		  tsk.IsLiveMonitoringEnabled
   FROM
   (SELECT t.InterviewID, 
          t.PersonSID, 
          t.SurveySID, 
          ISNULL(s.Name, '') as ProjectID, 
          ISNULL(s.Description, '') as SurveyName,
          (CASE WHEN t.StatusLogout != 6 /*BREAK*/ THEN t.TimeCallDelivered 
                ELSE lb.StartTime
           END) as TimeCallDelivered, 
          t.State, 
          (CASE WHEN t.InterviewID = 0 THEN NULL ELSE ISNULL(DATEDIFF(second, TimeStateChanged, @currTime), 0) END) as SecondsSinceLastSubmission, 
          (CASE WHEN InterviewID > 0 
				THEN tsc.val
				ELSE 0
			END) LastSubmissionAlert, 
          t.LastKeepAliveTime,
          (CASE WHEN LastKeepAliveTime IS NULL 
				THEN 2 
				ELSE lkat.val
			END) LastKeepAliveTimeAlert,
          (CASE WHEN TimeCallDelivered IS NULL AND t.StartTime IS NOT NULL
                 THEN  naa.val 
                 ELSE 0 
          END) EndOfLastActivityAlert,
		  (CASE WHEN TimeCallDelivered IS NULL
                 THEN 0 
                 ELSE ida.val  
          END) InterviewDurationAlert,
		  (CASE WHEN t.StatusLogout != 6
                 THEN 0 
                 ELSE bda.val  
          END) BreakDurationAlert,
          t.InterviewState,
          t.LoggedInToDialerState,
          t.TzID, 
          t.DiallingMode, 
          t.CallOutcome, 
          t.StatusLogout,
          t.ProblemId,
          t.StationId,
		  dt.Name as DialType,
		  CASE WHEN t.OpenEndReviewStartTime IS NOT NULL THEN DATEDIFF(ss, t.OpenEndReviewStartTime, GETUTCDATE()) ELSE NULL END AS OpenEndReviewInSeconds,
		  t.DialerId,
		  t.CallType,
		  t.LinkedChain,
		  t.CallConnectionState,
		  bt.Name as BreakTypeName,
		  t.JsonContext,
		  s.InterviewScreenRecording,
		  s.IsLiveMonitoringEnabled
   FROM BvTasks t
   LEFT JOIN BvFnSurvey_GetByTransferBatch( @surveysBatchID ) s ON (t.SurveySID = s.SID)
   LEFT JOIN BvUserSurveyPermission up ON t.SurveySID = up.SurveySID AND up.UserName = @UserName
   LEFT JOIN BvBreakType bt on bt.Id = t.BreakTypeId
   INNER JOIN BvDialType dt ON t.DialTypeId = dt.Id
   INNER JOIN dbo.BvFnPerson_GetByTransferBatch(@interviewersBatchID) pta ON pta.Id = t.PersonSID
   OUTER APPLY dbo.GetLastTimeBreak(t.PersonSID) lb
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, LastKeepAliveTime, @currTime), @AmberOfLastKeepAliveTime, @RedOfLastKeepAliveTime) as lkat
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, TimeStateChanged, @currTime), @AmberOfLastSubmission, @RedOfLastSubmission ) as tsc
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, t.StartTime, @currTime), @AmberOfNoActivity, @RedOfNoActivity ) as naa
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, TimeCallDelivered, @currTime), @AmberOfInterviewDuration, @RedOfInterviewDuration ) as ida
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, lb.StartTime, @currTime), @AmberOfBreakDuration, @RedOfBreakDuration ) as bda
   WHERE (s.SID IS NOT NULL AND up.SurveySID IS NOT NULL) OR t.SurveySID = 0) as tsk
   INNER JOIN BvFnPerson_Get(@CallCenterID) p ON (tsk.PersonSID = p.SID)
   INNER JOIN BvTimezone tz ON ((CASE WHEN TzID = 0 THEN @TimeZoneID ELSE TzId END) = tz.ID)
   LEFT JOIN BvPersonMonitoring pm ON (pm.PersonSID = tsk.PersonSID)

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
PRINT N'Refreshing [dbo].[BvSpAssignmentResource_GetResources]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignmentResource_GetResources]';


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
PRINT N'Refreshing [dbo].[BvSpDialer_Reset]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpDialer_Reset]';


GO
PRINT N'Refreshing [dbo].[BvSpFilter_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpFilter_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpGetAllAppointmentsForUser]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetAllAppointmentsForUser]';


GO
PRINT N'Refreshing [dbo].[BvSpGetExtendedCallHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetExtendedCallHistory]';


GO
PRINT N'Refreshing [dbo].[BvSpGetInterviewerBreaks]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviewerBreaks]';


GO
PRINT N'Refreshing [dbo].[BvSpGetInterviewerPerformanceList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviewerPerformanceList]';


GO
PRINT N'Refreshing [dbo].[BvSpGetInterviews]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviews]';


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
PRINT N'Refreshing [dbo].[BvSpHistory_GetLinkedInterviews]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpHistory_GetLinkedInterviews]';


GO
PRINT N'Refreshing [dbo].[BvSpInterview_UpdateRespondentFields]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterview_UpdateRespondentFields]';


GO
PRINT N'Refreshing [dbo].[BvSpInterviewsAndAppointments_Delete_Batch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviewsAndAppointments_Delete_Batch]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_GetAssignedSurveyList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_GetAssignedSurveyList]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_GetAssignments]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_GetAssignments]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_SetAutomaticSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_SetAutomaticSurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonAndGroups_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonAndGroups_List]';


GO
PRINT N'Refreshing [dbo].[BvSpReportSampleStatusSummary]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpReportSampleStatusSummary]';


GO
PRINT N'Refreshing [dbo].[BvSpReportSampleStatusSummaryForDatesRange]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpReportSampleStatusSummaryForDatesRange]';


GO
PRINT N'Refreshing [dbo].[BvSpSampleStatusSummary_Get]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSampleStatusSummary_Get]';


GO
PRINT N'Refreshing [dbo].[BvSpSampleUtilisationReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSampleUtilisationReport]';


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
PRINT N'Refreshing [dbo].[BvSpSurveyProductivityReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyProductivityReport]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyState_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyState_Update]';


GO
PRINT N'Refreshing [dbo].[BvSpSvySch_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSvySch_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateStatusLogout]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateStatusLogout]';


GO
PRINT N'Refreshing [dbo].[BvSpTelephoneBlacklist_GetSurveysToDeleteCalls]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTelephoneBlacklist_GetSurveysToDeleteCalls]';


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
PRINT N'Refreshing [dbo].[BvSpPerson_GetSurveys]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_GetSurveys]';


GO
PRINT N'Refreshing [dbo].[BvSpSetNextInterviewForPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSetNextInterviewForPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_GetListByFolder]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_GetListByFolder]';


GO
PRINT N'Refreshing [dbo].[BvSpUserSurveyList_Get]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpUserSurveyList_Get]';


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
