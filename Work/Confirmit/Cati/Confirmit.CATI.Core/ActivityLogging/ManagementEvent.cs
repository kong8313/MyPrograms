
namespace Confirmit.CATI.Core.ActivityLogging
{
    /// <summary>
    /// All types of management events in the CATI system. They could be performed from CATI Supervisor or Authoring (via WS).
    /// </summary>
    public enum ManagementEvent
    {
        OpenSurvey,
        CloseSurvey,
        ShutdownSurvey,
        UpdateSurvey,
        InitUserSurveyPermissions,
        InitUserTabPermissions,
        SetDialerSurveyParameters,
        ResetDialerSurveyParameters,
        SetSurveySchedulingParameters,
        ResetSurveySchedulingParameters,
        SaveConsoleSearchableFields,

        AssignResourcesToSurvey,
        DeassignResourcesFromSurvey,
        DeassignResourcesFromSurveyCalls,
        AssignSurveysToResource,
        ReplacePersonSurveyAssignment,
        ReplaceSurveyPersonAssignment,

        UpdateQuotaLimits,
        UpdateQuotaDisableCellFlags,
        SynchronizeQuota,

        CreateFilter,
        UpdateFilter,
        DeleteFilter,
        MoveSurveySpecificFiltersToSurvey,
        CopySurveySpecificFiltersToSurvey,

        SendMessageToSurveys,
        SendMessageToInterviewers,
        SendMessageToGroups,

        CreateCall,
        UpdateCall,

        ExportCallList,
        SaveCallMangementSearchableFields,

        CreateStateGroup,
        DuplicateStateGroup,
        DeleteStateGroup,
        EditState,

        ActivateDialer,
        EnableDialer,
        DectivateDialer,
        DisableDialer,
        DialerRequestCalls,
        SetDialerDefaultSurveyParameters,
        SetDialerNotificationsEmail,
        ReconnectDialer,
        StopDialerReconnection,

        ActivateTimezone,
        DeactivateTimezone,
        SetLocalTimezone,
        DeleteUnusedTimezones,

        ImportTelephoneNumbersToBlacklist,
        AddTelephoneNumberToBlacklist,
        UpdateTelephoneNumberInBlacklist,
        DeleteTelephoneNumbersFromBlacklist,
        ExportTelephoneNumbersFromBlacklist,

        CreateInterviewer,
        UpdateInterviewer,
        ImportInterviewers,
        DeleteInterviewer,
        ChangeInterviewerTaskChoice,
        ChangeInterviewerPassword,
        SetInterviewerAutomaticSurvey,
        CreateInterviewerGroup,
        UpdateInterviewerGroup,
        DeleteInterviewerGroup,
        ClearInterviewerAutomaticSurvey,
        InterviewerLockedBySupervisor,
        InterviewerUnLockedBySupervisor,

        CreateScript,
        UpdateScript,
        ScriptDelete,
        ScriptDuplicate,
        ScriptImport,
        ScriptSave,
        ScriptLaunch,

        SetActivityAlert,
        DeleteActivityAlert,
        SetActivityStatusAlert,
        DeleteActivityStatusAlert,
        StartVideoMonitoring,
        StopVideoMonitoring,
        TerminateTask,
        TerminateTaskWithReason,
        SetAppointmentListIntervals,
        StartAudioMonitoring,
        StopAudioMonitoring,

        BuildSurveyProductivityReport,
        BuildSurveySummaryReport,
        BuildInterviewerProductivityReport,
        BuildAttemptsByDispositionReport,
        BuildNumberOfAttemptsReport,
        BuildCallAttemptLog,
        BuildSurveyOverviewReport,
        BuildAggregatedAlertsHistoryReport,
        BuildAlertsHistoryReport,
        BuildInterviewerSessionsReport,

        ViewCallList,
        ViewQuota,

        AddSurveyViaWs,
        DeleteSurveyViaWs,
        AddSurveyAccessViaMs,
        DeleteSurveyAccessViaMs,
        UpdateSurveyPropertiesViaMs,
        UpdateSurveyReplicationSchemeViaMs,
        QuotaChangedViaMs,

        QuotaCellsChangedEventViaMs,
        QuotaCellsStateChangedEvent,
        DeleteRespondentsAsync,
        UpdateSurveyReplicationStatusViaMs,
        CatiOptionsChangedViaMs,
        AddTelephoneNumberToBlacklistViaWs,
        SoftDeleteSurveyViaWs,
        RestoreSoftDeletedSurveyViaWs,
        
        CreateMultimodeInstance,
        DeleteMultimodeInstance,
        StartMultimodeInstance,
        StopMultimodeInstance,
        ResynchronizeServices,

        Schedule,
        PeriodicalReplication,
        AutoLogoutThread,
        AutoLogoutWebConsoleThread,
        TerminateTaskWhileAutoLogout,
        ExpiredCalls,

        SetQuotaBalancing,
        ResetQuotaBalancing,

        ConfigureClusteredQuota,

        AddRespondentViaWs,

        LaunchSurvey,
        RestoreSurveyFromArchive,
        BackupSurveyToArchive,
        DeleteSurvey,

        BulkCopyInterviewerActivityEventsEvent,

        CreateDefferedMonitoringFile,
        ScheduledReportEmail,

        CallGroupInsert,
        CallGroupUpdate,
        CallGroupDelete,
        CallGroupSetPersonAssignment,
        CallGroupSetConditions,

        DatabaseUpdateScriptApplying,
        DatabaseUpdateFinish,

        CreateCallCenter,
        UpdateCallCenter,
        DeleteCallCenter,
        AssignSupervisorsToCallCenter,
        AssignSurveysToCallCenters,

        //Async call management operation events

        DeleteSelectedCalls,
        DeleteFilteredCalls,
        DeleteFilteredByClosedQuotaCellCalls,

        MoveSelectedCalls,
        MoveFilteredCalls,

        ChangeDialModeOfSelectedInterviews,
        ChangeDialModeOfFilteredInterviews,

        ActivateSelectedCalls,
        ActivateFilteredCalls,
        ActivateFilteredByCellsCalls,

        EnableSelectedCalls,
        EnableFilteredCalls,
        EnableFilteredByCellsCalls,

        DisableSelectedCalls,
        DisableFilteredCalls,
        DisableFilteredByCellsCalls,

        ChangePriorityOfSelectedCalls,
        ChangePriorityOfFilteredCalls,

        ChangeShiftTypeOfSelectedCalls,
        ChangeShiftTypeOfFilteredCalls,

        AssignSelectedCalls,
        AssignFilteredCalls,

        MoveAndRescheduleSelectedCalls,
        MoveAndRescheduleFilteredCalls,

        ChangeInterviewerLocation,
        
        SendNumbersEvent,
        AsyncOperationDequeue,
        AsyncOperationAbort,

        ViewDeferredMonitoringList,
        GetDeferredRecordAudioInfo,

        BuildTelerikReport,

        WebApiCall,

        UpdateFcdStatusOfCalls,

        // Sync Queue events should have exact ids since they are used in Confirmit code
        SyncQueueAdd = 196,
        SyncQueueDelete = 197,
        SyncQueueResync = 198,

        RoutineMaintenance,
        DialerHealthControlThread,

        UpdateGeneralSiteSettings,
        UpdateInterviewerConsoleSiteSettings,
        UpdateSecuritySiteSettings,
        UpdateSystemSettings,

        SurveyReplication,
        RereadReplication,

        InitializeSurveyMetadataCacheEvent,
        ResetSurveyMetadataCacheEvent,

        SetInterviewerDialType,
        ProcessSample,
        SampleUpload,

        ChangePriorityOfFilteredByCellsCalls,

        BuildInboundCallsReport,

        OnInboundCallNotifyEvent,

        OnInboundCallDroppedNotifyEvent,

        AddDdiNumber,
        UpdateDdiNumber,
        DeleteDdiNumbers,

        AddCallManagementCustomView,
        EditCallManagementCustomView,
        DeleteCallManagementCustomView,

        RedirectToLoginPage,

        CallHistoryExport,
        CallHistoryDelete,
        CallHistoryUpdate,

        AddIvrSetting,
        UpdateIvrSetting,
        DeleteIvrSettings,

        AddBreak,
        UpdateBreak,
        DeleteBreak,

        UpdateQuotaCellPriority,

        CreateExternalTransferNumber,
        UpdateExternalTransferNumber,
        DeleteExternalTransferNumbers,

        AddDialer,
        EditDialer,
        DeleteDialer,

        GetLogFilesEvent,
        GetLogFileBodyZippedEvent,
        GetAvailableExtendedFunctionalityEvent,

        TimezoneUpdate,

        AssignResourcesToSurveyUsingSurveyAssignmentsDialog,
        DeassignResourcesFromSurveyUsingSurveyAssignmentsDialog,

        GetDialerSupportedFeaturesEvent,
        GetOverridenDialerSupportedFeaturesEvent,
        UpdateOverridenDialerSupportedFeatureEvent,

        CopyToDefaultStateGroup,
        CopyToDefaultSchedulingScript,

        StartLegacySupervisor,

        AddRespondentFromConsole,

        EditSelectedCalls,
        EditFilteredCalls,
        ChangeInterviewerSSOType,

        SynchronizeRespondents,
		
        AddCustomTimezone,
        UpdateCustomTimezone,
        SetRespondentVariablesToSendToDialer
    }
}