GO
PRINT N'Altering Table [dbo].[BvSurvey]...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD [IsStateLocked] BIT CONSTRAINT [DF_BvSurvey_IsStateLocked] DEFAULT (0) NOT NULL;


GO
PRINT N'Refreshing View [dbo].[RestView_BreakHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_BreakHistory]';


GO
PRINT N'Refreshing View [dbo].[RestView_CallHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_CallHistory]';


GO
PRINT N'Refreshing View [dbo].[RestView_Interviewer]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_Interviewer]';


GO
PRINT N'Refreshing View [dbo].[RestView_Survey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_Survey]';


GO
PRINT N'Refreshing Function [dbo].[BvFnSurvey_GetByCallCenterId]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvFnSurvey_GetByCallCenterId]';


GO
PRINT N'Refreshing Function [dbo].[BvFnSurvey_GetByTransferBatch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvFnSurvey_GetByTransferBatch]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpAlert_RecalculateAll]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlert_RecalculateAll]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpAlert_RecalculateAppointment]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlert_RecalculateAppointment]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpAssignment_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignment_List]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpAssignmentResource_GetResources]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAssignmentResource_GetResources]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpAttemptsByDispositionReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAttemptsByDispositionReport]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpCall_Activate]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_Activate]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpCall_ChangeShiftType]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_ChangeShiftType]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpCall_MoveToITS]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_MoveToITS]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpCallHistory_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistory_List]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpCallHistoryData]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistoryData]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpCheckCallOnShifts]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCheckCallOnShifts]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpDialer_Reset]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpDialer_Reset]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpFilter_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpFilter_Insert]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetAllAppointmentsForUser]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetAllAppointmentsForUser]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetExtendedCallHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetExtendedCallHistory]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetInterviewerBreaks]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviewerBreaks]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetInterviewerPerformanceList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviewerPerformanceList]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetInterviews]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviews]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetLiveShifts]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetLiveShifts]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetOpenedSurveys]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetOpenedSurveys]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetReplicatedTable]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetReplicatedTable]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetSurveyActivityWithAlerts]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSurveyActivityWithAlerts]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetSurveysWithSurveySpecificFilters]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSurveysWithSurveySpecificFilters]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetSystemWideInfo]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSystemWideInfo]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpHistory_GetLinkedInterviews]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpHistory_GetLinkedInterviews]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpInterview_UpdateRespondentFields]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterview_UpdateRespondentFields]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpPerson_GetAssignedSurveyList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_GetAssignedSurveyList]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpPerson_GetAssignments]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_GetAssignments]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpPerson_SetAutomaticSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_SetAutomaticSurvey]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpPersonAndGroups_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonAndGroups_List]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpReportSampleStatusSummary]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpReportSampleStatusSummary]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpReportSampleStatusSummaryForDatesRange]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpReportSampleStatusSummaryForDatesRange]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSampleStatusSummary_Get]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSampleStatusSummary_Get]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSampleUtilisationReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSampleUtilisationReport]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSchedule_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSchedule_Delete]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpScheduleParam_Launch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpScheduleParam_Launch]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSetCallDeliveryMode]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSetCallDeliveryMode]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpState_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpState_List]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpState_ListBySurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpState_ListBySurvey]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpState_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpState_Update]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpStateGroup_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpStateGroup_Delete]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurvey_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Delete]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurvey_GetAssignedPersonList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_GetAssignedPersonList]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurvey_GetOpened]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_GetOpened]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurvey_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Insert]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurveyCleanup_GetSurveysWhichAreReadyForCleanup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyCleanup_GetSurveysWhichAreReadyForCleanup]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurveyCleanup_GetSurveysWhichAreReadyForNotice]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyCleanup_GetSurveysWhichAreReadyForNotice]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurveyModifyStateGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyModifyStateGroup]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurveyOverviewReportForAllPersons]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyOverviewReportForAllPersons]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurveyProductivityReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyProductivityReport]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurveyState_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyState_Update]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSvySch_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSvySch_Insert]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpTasks_UpdateStatusLogout]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateStatusLogout]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpTelephoneBlacklist_GetSurveysToDeleteCalls]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTelephoneBlacklist_GetSurveysToDeleteCalls]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpUserSurveyPermission_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpUserSurveyPermission_Delete]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpUserSurveyPermission_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpUserSurveyPermission_Insert]';


GO
PRINT N'Refreshing Procedure [dbo].[SetDialerSurveyParametersWhereIsNull]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[SetDialerSurveyParametersWhereIsNull]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetSurveys]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSurveys]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpLookUpByPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpPerson_GetSurveys]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPerson_GetSurveys]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSetNextInterviewForPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSetNextInterviewForPerson]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurvey_GetListByFolder]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_GetListByFolder]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpUserSurveyList_Get]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpUserSurveyList_Get]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetListSurveyTasks]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetListSurveyTasks]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpQueueUpSheduleTask3]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpQueueUpSheduleTask3]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurvey_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Update]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurveyOverviewReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyOverviewReport]';


GO
PRINT N'Update complete.';


GO
