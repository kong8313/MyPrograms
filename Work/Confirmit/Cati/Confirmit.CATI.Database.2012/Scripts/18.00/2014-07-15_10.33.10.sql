GO
PRINT N'Altering [dbo].[BvSpGetListSurveyTasks]...';


GO
ALTER PROCEDURE BvSpGetListSurveyTasks
   @surveysBatchID int,
   @interviewersBatchID int,   
   @TimeZoneID INT,
   @CallCenterID INT
AS
   DECLARE @currTime DATETIME
   SET @currTime = GETUTCDATE()
   DECLARE @AmberOfLastSubmission INT
   DECLARE @RedOfLastSubmission INT
   DECLARE @AmberOfLastKeepAliveTime INT
   DECLARE @RedOfLastKeepAliveTime INT
   DECLARE @AmberOfNoActivity INT
   DECLARE @RedOfNoActivity INT

   SELECT @AmberOfLastSubmission = Amber, @RedOfLastSubmission = Red
   FROM BvThresholds 
   WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 1/*Task alert*/

   SELECT @AmberOfLastKeepAliveTime = Amber, @RedOfLastKeepAliveTime = Red
   FROM BvThresholds 
   WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 16/*Last keep alive alert*/
   
   SELECT @AmberOfNoActivity = Amber, @RedOfNoActivity = Red
   FROM BvThresholds 
   WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 18/*No activity alert*/
   
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
          tsk.StationId
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
          t.InterviewState,
          t.LoggedInToDialerState,
          t.TzID, 
          t.DiallingMode, 
          t.CallOutcome, 
          t.StatusLogout,
          t.ProblemId,
          t.StationId
   FROM BvTasks t
   LEFT JOIN BvFnSurvey_GetByTransferBatch( @surveysBatchID ) s ON (t.SurveySID = s.SID)
   INNER JOIN dbo.BvFnPerson_GetByTransferBatch(@interviewersBatchID) pta ON pta.Id = t.PersonSID
   OUTER APPLY dbo.GetLastTimeBreak(t.PersonSID) lb
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, LastKeepAliveTime, @currTime), @AmberOfLastKeepAliveTime, @RedOfLastKeepAliveTime) as lkat
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, TimeStateChanged, @currTime), @AmberOfLastSubmission, @RedOfLastSubmission ) as tsc
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, t.StartTime  , @currTime), @AmberOfNoActivity, @RedOfNoActivity ) as naa
   WHERE s.SID IS NOT NULL OR t.SurveySID = 0) as tsk
   INNER JOIN BvFnPerson_Get(@CallCenterID) p ON (tsk.PersonSID = p.SID)
   INNER JOIN BvTimezone tz ON ((CASE WHEN TzID = 0 THEN @TimeZoneID ELSE TzId END) = tz.ID)
   LEFT JOIN BvPersonMonitoring pm ON (pm.PersonSID = tsk.PersonSID)
GO
PRINT N'Altering [dbo].[BvSpFinishInterviewerBreak]...';


GO
ALTER PROCEDURE [dbo].[BvSpFinishInterviewerBreak]
	@InterviewerId INT
AS
    ;WITH TimeBreaksHistory AS
    (
       SELECT TOP(1) *
       FROM BvTimeBreaksHistory
       WHERE InterviewerId = @InterviewerId
       ORDER BY StartTime DESC
    )
	UPDATE TimeBreaksHistory 
	SET Duration = DATEDIFF(second, StartTime, GETUTCDATE())
	WHERE Duration IS NULL
	;
	UPDATE BvTasks
    SET StartTime = GETUTCDATE()
    WHERE PersonSID = @InterviewerId

RETURN 0
GO
PRINT N'Adding new Threshold Type...';


GO
if NOT EXISTS(SELECT 1 FROM [BvThresholdTypes] WHERE ID = 18)
begin
	INSERT INTO [BvThresholdTypes] VALUES(18, 'TasksAlert.NoActivity alert')
end;


GO
PRINT N'Refreshing [dbo].[BvSpAlert_RecalculateAll]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlert_RecalculateAll]';


GO
PRINT N'Refreshing [dbo].[BvSpDialer_Reset]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpDialer_Reset]';


GO
PRINT N'Refreshing [dbo].[BvSpGetInterviewerPerformanceList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviewerPerformanceList]';


GO
PRINT N'Refreshing [dbo].[BvSpGetLoggedInPersonsCount]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetLoggedInPersonsCount]';


GO
PRINT N'Refreshing [dbo].[BvSpGetSurveyInterviews]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSurveyInterviews]';


GO
PRINT N'Refreshing [dbo].[BvSpGetSystemWideInfo]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSystemWideInfo]';


GO
PRINT N'Refreshing [dbo].[BvSpInterviewTimings_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviewTimings_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpLogin_SpinUp]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLogin_SpinUp]';


GO
PRINT N'Refreshing [dbo].[BvSpPerson_ListByParent]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_ListByParent]';


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
PRINT N'Refreshing [dbo].[BvSpSurvey_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_GetCountOfLoggedPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_GetCountOfLoggedPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpTask_UpdateActiveQuestion]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTask_UpdateActiveQuestion]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_InsertAnswerSubmissionAlertIfNeeded]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_InsertAnswerSubmissionAlertIfNeeded]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_InsertUpdate_2]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_InsertUpdate_2]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_LockByPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_LockByPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_SetTelephonyProblemForLoggedIn]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_SetTelephonyProblemForLoggedIn]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UnLockByPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UnLockByPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_Update_2]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_Update_2]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateCallOutcome]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateCallOutcome]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateInterviewState]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateInterviewState]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateKeepAlive]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateKeepAlive]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateLoggedInToDialerState]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateLoggedInToDialerState]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateProblemState]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateProblemState]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateStartTime]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateStartTime]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateStatusLogout]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateStatusLogout]';


GO
PRINT N'Update complete.';


GO
