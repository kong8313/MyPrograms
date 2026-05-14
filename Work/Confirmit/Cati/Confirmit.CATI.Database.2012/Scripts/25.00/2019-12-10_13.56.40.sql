PRINT N'Add Toggle.EnableBBCCNotification setting';


GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Toggle.EnableBBCCNotification', 'Enable Browser based CATI Console notification', 'Toggle', 'Enable Browser based CATI Console notification to speed up call delivery for Browser based CATI Interviewers', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Altering [dbo].[BvTasks]...';


GO
ALTER TABLE [dbo].[BvTasks]
    ADD [IsWebConsole] BIT CONSTRAINT [DF_BvTasks_IsWebConsole] DEFAULT (0) NOT NULL;


GO
PRINT N'Refreshing [dbo].[BvSpAlert_RecalculateAll]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlert_RecalculateAll]';


GO
PRINT N'Refreshing [dbo].[BvSpDialer_Reset]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpDialer_Reset]';


GO
PRINT N'Refreshing [dbo].[BvSpFinishInterviewerBreak]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpFinishInterviewerBreak]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]';


GO
PRINT N'Refreshing [dbo].[BvSpGetInterviewerPerformanceList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviewerPerformanceList]';


GO
PRINT N'Refreshing [dbo].[BvSpGetListSurveyTasks]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetListSurveyTasks]';


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
PRINT N'Refreshing [dbo].[BvSpLogin_SpinUp]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLogin_SpinUp]';


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
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateNewSurveySid]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateNewSurveySid]';


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
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateSurveySid]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateSurveySid]';


GO
PRINT N'Refreshing [dbo].[BvSpTransfer_GetInternalTargets]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTransfer_GetInternalTargets]';


GO
PRINT N'Update complete.';


GO
