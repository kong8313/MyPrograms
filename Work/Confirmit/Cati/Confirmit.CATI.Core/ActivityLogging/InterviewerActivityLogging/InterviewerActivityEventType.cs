namespace Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging
{
    public enum InterviewerActivityEventType
    {
        Login,

        LoginToDialer,

        ForcedLogout,

        SetPendingLogout,

        LogoutProcess,

        ConfirmLogout,

        UpdateInterviewerMode,

        StartInterview,

        StartInterviewProcess,

        CreateNewInterview,

        UrlGeneratedInGetState,

        SetInterviewAppointment,

        Dial,

        Hangup,

        WrapUp,

        SaveInterviewHistoryAndControlData,

        StartInterviewProcessNoCalls,

        LogoutOnWrapUp,

        GetCall,

        StartPlayback,

        StopPlayback,

        PauseOrResumePlayback,

        ToggleInterviewerListensToPlaybackOrRespondent,

        SetPendingBreakStatus,

        ContinueWorkAfterBreak,

        TakeBreak,

        IncrementFailedLoginAttempts,

        ResetFailedLoginAttempts,

        InterviewerLocked,

        GenerateAuthenticationKey,

        OnDialerScreenPopEvent,

        OnDialerCallConnectedEvent,

        OnDialerCallNotConnectedEvent,

        OnDialerTransferStateEvent,

        OnDialerNotifyAgentStateEvent,

        OnDialerNotifyCallDroppedByRespondentEvent,

        ExecuteSchedulingScriptEvent,

        InsertInterviewEvent,

        UpdateInterviewEvent,

        CheckTextSpelling,

        GetMessages,

        GetForceOpenendReview,

        GetOpenedSurveys,

        GetInterviewHistory,

        GetSurveyLanguages,

        GetSurveyInterviews,

        GetInterviewAppointment,

        GetAllAppointmentList,

        KeepAliveEvent,

        GetStateEvent,

        GetCatiCompanyIdEvent,

        ChangeInterviewerPasswordEvent,

        Redial,

        TerminateTaskByAutoLogout,

        TerminateTaskFromConsoleEvent,

        SurveySwitch,

        SetNextLinkedInterview,

        SetNextLinkedInterviewToPrevious,

        GetInterviews,

        GetLinkedInterviews,

        GetCallType,

        GetPersonType,

        TransferStart,

        TransferSetConnectionState,

        TransferComplete,

        TransferCancel,
		
        EnableLiveMonitoring,

        StopRecording,

        StartRecording,

        UpdateActiveQuestion,
        
        IsCatiGroupMember
    }
}
