using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Confirmit.CATI.Common
{
    [Flags]
    public enum SurveyChannels : byte
    {
        None = 0,
        Cawi = 1,
        Capi = 2,
        RandomDataGeneration = 4,
        Cati = 8,
    }

    public enum Role : byte
    {
        None = 0,
        Interviewer=0x0002,
        IVR = 0x10,
        WebRespondent=0x0020,
    }

    [DataContract(Namespace = "http://www.confirmit.com/ManagementService/08/06/2009/OperationType")]
    public enum OperationType : byte
    {
        [EnumMember]
        Undefined = 0,
        [EnumMember]
        Interview = 1,
        [EnumMember]
        WebInterview = 2,
        [EnumMember]
        AddRecordInWebInterview = 3,
        [EnumMember]
        UpdateRecordInWebInterview = 4,
        [EnumMember]
        SampleAddFullScheduling = 5,
        [EnumMember]
        SimpleAddSchedulingSample = 6,
        [EnumMember]
        EnableCalls = 7,
        [EnumMember]
        DisableCalls = 8,
        [EnumMember]
        DeleteCallsByFcd = 9,
        [EnumMember]
        DeleteCalls = 10,
        [EnumMember]
        DisableByFcd = 11,
        [EnumMember]
        MoveCallsToIts = 12,
        [EnumMember]
        ActivateCalls = 13,
        [EnumMember]
        AssignCalls = 14,
        [EnumMember]
        ChangePriorityOfCalls = 15,
        [EnumMember]
        ChangeShiftTypesOfCall = 16,
        [EnumMember]
        MovedAndReschedule = 17,
        [EnumMember]
        AddCall = 18,
        [EnumMember]
        UpdateCall = 19,
        [EnumMember]
        ChangeDiallingMode = 20,
        [EnumMember]
        ExpireByDialler = 21,
        [EnumMember]
        ReturnNotDialled = 22,
        [EnumMember]
        ExpiredCall = 23,
        [EnumMember]
        PromoteCall = 24,
        [EnumMember]
        TelephonyError = 25,
        [EnumMember]
        TerminateTask = 26,
        [EnumMember]
        NotConnectedCall = 27,
        [EnumMember]
        DisableByFcdDuringSample = 28,
        [EnumMember]
        DeleteByFcdDuringSample = 29,
        [EnumMember]
        SynchronizeEnableDisableCallState = 30,
        [EnumMember]
        SchedulingScriptExecutionError = 31,
        [EnumMember]
        DeleteCallByBlacklistInAddSample = 32,
        [EnumMember]
        UpdateBySampleUpdate = 33,
        [EnumMember]
        AddRecordFromConsole = 34,
        [EnumMember]
        AddRecordByInboundCall = 35,
        [EnumMember]
        InboundCall = 36,
        [EnumMember]
        InternalTransfer = 37,
        [EnumMember]
        DroppedByRespondent = 38,
        [EnumMember]
        EditCalls = 39,
        [EnumMember]
        EditCallHistory = 40,
        [EnumMember]
        DeleteCallHistory = 41,
        [EnumMember]
        SynchronizeRespondents = 42
    }

    public enum InboundHandlerOperationType
    {
        Undefined = 0,
        PlacedInQueue = 1,
        SendToDialer = 2,
        DropBySystemInboundDisabled = 3,
        DropBySystemDdiRecordNotFound = 4,
        DropBySystemInterviewNotFound = 5,
        DropBySystemWrongCallState = 6,
        DropBySchedulingScript = 7,
        DropByRespondent = 8,
        DropBySystemSurveyIsNotOpened = 9,
        DropBySystemSurveyIsNotFound = 10,
        DropBySystemShiftIsNotFound = 11,
        DropBySystemInternalServerError = 12,
        DropBySystemNoAgentsAvailable = 13,
        ConnectedToAgent = 20,

        Skipped = -1
    }

    public enum BlacklistPatternType: byte
    {
        Equal = 0,
        StartWith = 1
    }

    public enum CallCompleteStatus
    {
        Error = 0,
        NotConnected = 1,
        DropBySystem = 2,
        DropByRespondent = 3,
        CompleteByScript = 4,
        CompleteByConsole = 5,
        Terminated = 6
    }


    /// <summary>
    /// Represents interviewer task choice.
    /// Uses to specify several task choices allowed to interviewer.
    /// </summary>
    [Flags]
    public enum TaskChoicePermissions
    {
        Automatic = 1,
        Manual = 2,
        SurveyAssignment = 4
    }

    /// <summary>
    /// Error codes CATIConsoleWebServ can return via SOAPExceptions to a client.
    /// </summary>
    public enum CATIErrorCodes
    {
        InternalError,
        InvalidUsernameOrPassword,
        InvalidAuthenticationKey,
        CheckLicenseCannotConnectToLicenseServer,
        CheckLicenseNumberOfConcurrentInterviewersIsExceeded,
        SurveyNotFoundInFusion,
        ThePersonIsNotLoggedIn,

        ThereIsNoDialerInTheSystem,
        DialerIsNotAvailable,
        InterviewerExtensionNumberIsEmpty,
        RespondentPhoneNumberIsEmpty,
        CannotStartDialling,

        AppContradictsExclusionShifts,
        AppTimeIsOutOfShifts,

        InvalidFusionInstance,
        ManualSelectionUserAttempsToWorkInPredictiveDiallingMode,

        SpellCheckerLanguageIsNotSupported
    }

    /// <summary>
    /// All possible interview states.
    /// </summary>
    public enum InterviewState
    {
        NO_CALLS=0,
        SELECTING=1,
        WAITING=2,
        DIALLING=3,
        INTERVIEWING=4,
        OPENEND_REVIEW=5,
        INTERVIEW_WRAP_UP=6,
        REDIALLING=7,
        INTERVIEWING_INBOUND = 8,            //currently used only in interviewers activities view
        OUTGOING_TRANSFER = 9,
        INCOMING_TRANSFER = 10
    }

    /// <summary>
    /// All possible login states (used both for login state and dialer login state).
    /// </summary>
    public enum LoginState
    {
        NOT_LOGGED_IN=0,
        LOGGING_IN=1,
        LOGGED_IN=2,
        PENDING_LOGOUT = 3, //is used only for statusLogout and is not used in DialerLogin
        LOGGING_OUT=4,
        PENDING_BREAK = 5, //is used only for statusLogout and is not used in DialerLogin
        BREAK = 6, //is used only for statusLogout and is not used in DialerLogin
    }

    /// <summary>
    /// Quota types
    /// </summary>
    public enum QuotaType
    {
        Pessimistic=1,
        Optimistic=2,
        Adaptable=3
    }

    public enum AgentStateMsgs
    {
        LOGGEDIN=1,
        NOTREADY=2,
        READY=3,
        LOGGEDOUT=4
    }


    public enum CallState
    { 
        ToBeAddedFromSample = -3,
        LoadedToDialerPredictively = -2,
        InterviewInProgress = -1,
        ToBeDeleted = 0,
        DisabledByFCD = 1,
        Scheduled = 2,
        DisabledByUser = 3
    }

    public enum SurveyState
    {
        Close=0,
        Open=1,
        SoftDeleted=2
    }

    /// <summary>
    /// mode of shift types
    /// </summary>
    public enum BvdbsActionMode
    {
        BVDBS_ACTION_MODE_WEAK=1,
        BVDBS_ACTION_MODE_STRONG=2
    }

    /// <summary>
    /// cycle type of shift
    /// </summary>
    public enum ShiftCycleType
    {
        Shift=1,
        Exclusion=2
    }

    public enum DaylightType
    {
        Disable=1,
        Relative=2,
        Absolute=3,
    }

    [Flags]
    public enum FilterGenerateMode
    {                                         
        AllInterviewIds = 1,
        ScheduledInterviews = 3,
        SuspendedInterviews = 4,
        
        AllInterviews = 5,
        ScheduledInterviewIds = 8,
        SuspendedInterviewIds = 9,
        
        AllInterviewStates = 10,

		HighPriorityInterviews = 11,
        HighPriorityInterviewIds = 12,

        SentToDialerInterviews = 13,
        SentToDialerInterviewIds = 14,
        
        CallsAvailableNow = 15
    }

    public enum FilterOperator
    {
        [Description("<")]
        Less = 1,
        [Description(">")]
        Bigger = 2,
        [Description("=")]
        Equal = 3,
        [Description("<=")]
        LessEqual = 4,
        [Description(">=")]
        BiggerEqual = 5,
        [Description("<>")]
        NotEqual = 6,
        [Description("LIKE")]
        Like = 7,
        [Description("SUBFILTER")]
        Subfilter = 8,
        [Description("IsNullOrEmpty")]
        IsNullOrEmpty = 9,
        [Description("!")]
        Not = 10,
        [Description("IN")]
        In = 11,
        [Description("NOT IN")]
        NotIn = 12,
    }

    public enum AndOrOperator
    {
        Or,
        And
    }

    [Flags]
    public enum TableTypes
    {
        Subfilter = 0,
        Interview = 1,
        Call = 2,
        Appointment = 4,
        QSLVariables = 8,
        Quotas = 0x10,
        Container = 0x20,
        ShiftType = 0x40,
        Resource = 0x80,
        Web = 0x100,
        CFVariables = 0x200,
        Expression = 0x400,
        State = 0x800 | Interview,
        Timezone = 0x1000 | Interview,
        InnerShiftType = 0x2000,
        Person = 0x4000
    };

    public enum VariableTypes
    {
        Subfilter = 0,
        Integer,
        String,
        Date,
        Decimal,
        PredefinedValue
    }

    public enum SortingType
    {
        Asc,
        Desc
    }

    /// <summary>
    /// Scheduling mode used while uploading sample.
    /// </summary>
    public enum SchedulingMode
    {
        /// <summary>
        /// Scheduling rules are executed.
        /// </summary>
        Full = 1,

        /// <summary>
        /// No scheduling rules are executed.
        /// </summary>
        Simple = 2,
    }

    public enum SampleMode
    {
        Add,
        Update,
        Merge
    }

    public enum CallExplicitType
    {
        Survey = 1,
        PersonOrPersonGroup = 2
    }

    public enum CallAssignemntType
    {
        Survey,
        Person,
        Group,
        Multi
    }

    /// <summary>
    /// Confirmit variable types.
    /// </summary>
    public enum ConfirmitVariableType
    {
        Loop,
        Open,
        Numeric,
        Single,
        Multi,
        Grid,
        NotSet
    }

    /// <summary>
    /// State of call which is saved in phase.
    /// </summary>
    public enum PhaseState
    {
        /// <summary>
        /// All calls which are added from sample should have this state while all of them won't be loaded they should not be processed.
        /// </summary>
        FreshSampleCall = -3,

        /// <summary>
        /// Call was scheduled and sent to dialer in predictive mode it should have this phase.
        /// </summary>
        PreparedForPredictiveCall = -2,

        /// <summary>
        /// Interview is in progress.
        /// </summary>
        ProcessedCall = -1,

        /// <summary>
        /// Call should be deleted.
        /// </summary>
        DeletedCall = 0,

        /// <summary>
        /// disabled call
        /// </summary>
        DisabledCall = 1,

        /// <summary>
        /// 
        /// </summary>
        DefaultState = 2
    }

    /// <summary>
    /// result for AddSample operation
    /// </summary>
    public enum ProcessSampleAsyncResult
    {
        InProgress = 1,
        Success = 2,
        Error = 3,
        Aborted = 4
    }

    public enum DiallerType
    {
        NoDialler,
        MN, // Not used anymore
        BvTCI, // Not used anymore
        PROTS, // Not used anymore
        Generic
    }

    public enum DialerConfigurationType
    {
        Sytel = 1,
        InVade = 2,
        Tci = 3,
        ProTs = 4,
        Simulator = 5,
        AmazonConnect = 6
    }

    /// <summary>
    /// Appointment state
    /// </summary>
    public enum AppointmentState
    {
        /// <summary>
        /// No call has been created for this appointment
        /// </summary>
        ActiveWithoutCall = 0,

        /// <summary>
        /// Call exists for this appointment
        /// </summary>
        ActiveWithCall = 1,

        /// <summary>
        /// There was a call in the past for this appointment
        /// </summary>
        Expired = 2
    }

    public enum CallShiftType
    {
        None        = Int32.MinValue,
        AnyValid    = -1
    }

    public enum CallTypes
    {
        Outbound = 0,
        Inbound = 1,
        Transfer = 2
    }

    public enum DeadlockPriority
    {
        Supervisor          = -5,
        PeriodicalThread    = -4,
        SchedulingProcedure = -3,
        Normal              = 0,
        High                = 1,
    }

    /// <summary>
    /// Type of parameters used in scheduling scripts.
    /// </summary>
    public enum SchedulingParameterType
    {
        [XmlEnum("Integer")]
        Integer,
        [XmlEnum("ShiftType")]
        ShiftType,
        [XmlEnum("Shift")]
        Shift,
        [XmlEnum("Resource")]
        Resource,
        [XmlEnum("ExtendedStatus")]
        ExtendedStatus
    }

        /// <summary>
    /// Defines possible intervals for DateTimeRangeSelect control
        /// </summary>
    [Flags]
    public enum DateTimeRange
    {
        Range = 0,
        Today = 0x1,
        Last2Hrs = 0x2,
        Last4Hrs = 0x4,
        Last2Days = 0x8,
        TodayMinus1 = 0x10,
        TodayMinus2 = 0x20,
        TodayMinus3 = 0x40,
        TodayMinus4 = 0x80,
        TodayMinus5 = 0x100,
        TodayMinus6 = 0x200,
        TodayMinus7 = 0x400,
        ThisWeek = 0x800,
        ThisMonth = 0x1000,
        ThisYear = 0x2000,
        All = 0xFFFF
    }

    /// <summary>
    /// Defines possible event sources for client errors.
    /// </summary>
    public enum ClientErrorSource
    {
        /// <summary>
        /// An error has occured in Dialer Web Service
        /// </summary>
        DialerError,

        /// <summary>
        /// An error has occured in LoadUtility
        /// </summary>
        LoadUtilityError
    }

    /// <summary>
    /// Defines possible dialer statuses.
    /// </summary>
    public enum DialerStatus
    {
        /// <summary>
        /// Dialer is available in the system and operational
        /// </summary>
        ConnectedAndActivated,
        /// <summary>
        /// Dialer is available in the system but not operational
        /// </summary>
        ConnectedAndDeactivated,
        /// <summary>
        /// Dialer is not available in the system
        /// </summary>
        DisconnectedAndDeactivated,
        /// <summary>
        /// Dialer is not available in the system and system is trying to rich status ConnectedAndDeactivated
        /// </summary>
        DisconnectedTryingToConnect,
        /// <summary>
        /// Dialer is not available in the system and system is trying to rich status ConnectedAndActivated
        /// </summary>
        DisconnectedTryingToConnectAndActivate
    }

    /// <summary>
    /// Defines if design quota is synchronized with production quota
    /// </summary>
    public enum QuotaSyncState
    {
        /// <summary>
        /// Design quota is synchronized with production quota
        /// </summary>
        Synchronized,

        /// <summary>
        /// Design quota structure is not synchronized with production quota
        /// </summary>
        NotSynchronized
    }

    public enum CallLockMode
    {
        NoLock = 0,
        TryLockOnlyNotLive = 1,
        TryLockAny = 2
    }

    public enum CallMode
    {
        NotLive = 0,
        Live = 1
    }

    public enum AlertStatus
    {
        Ok = 0,
        Warning = 1,
        Error = 2
    }

    public enum SchedulingScriptState
    {
        NotLaunched = 0,
        PendingSynchronization = 1,
        Synchronized = 2
    }

    public enum InterviewerSubmissionAlert
    {
        All = 0,
        LastSubmission = 1,
        QuickAnswer = 2,
    }

    public enum PendingBreakStatus
    {
        Break,
        None,
    }

    public enum CallDeliveryMode
    {
        InOrder = 0,
        Random = 1,
    }

    public enum AssignmentType
    {
        Implicit = 0,
        Explicit = 1,
        ImplicitToSurveyCalls = 2
    }

    public enum UserNotificationType
    {
        SurveyCleanupNotificationWarning = 1,
        SurveyCleanupNotification = 2,
    }

    public enum SurveySchedulingMode
    {
        Normal = 0,
        CallGroup = 1
    }

    public enum FcdAlgorithmType
    {
        DeleteCalls = 0,
        DisableCallsWithReenabling = 1
    }

    public enum DialType
    {
        Landline = 0,//We can use automatic dialing for interviews with such dial type
        Cellphone = 1,//We can't use automatic dial
        Assisted = 2//equates to Manual Dialing for cellphones using Agent-Assistance
    }

    public enum DialState
    {
        /// <summary>
        /// Respondent is connected to line and routed to active agent/interviewer.
        /// </summary>
        Connected = 1,
        
        /// <summary>
        /// Respondent is connected to line, but not routed to active agent/interviewer yet. Used for inbound call, then agent/interviewer isn't connected/routed to respondent yet.
        /// </summary>
        Pending = 2,

        /// <summary>
        /// We are attempting to connect/dial to respondent.
        /// </summary>
        Dialing = 3,

        /// <summary>
        /// We are trasfering a connected respondent trom one agent to another. In this case respondent can be stay in any routing state. so respondent can be routed/connected to some agent, or not.
        /// </summary>
        Transfering = 4,

        /// <summary>
        /// Sent to dialer
        /// </summary>
        Queueing = 5
    }

    public enum UserSurveyListType
    {
        Recent = 1
    }

    public enum LinkedInterviewPhase
    {
        NotLinkedInterview = 0,
        FirstInterview = 1,
        MiddleInterview = 2,
        FinalInterview = 3
    }

    public enum InboundGroupBehavior
    {
        DeliverCallsFromTheSameSurvey = 0,
        DeliverCallsFromOtherSurvey = 1
    }

    public enum TransferGroupBehavior
    {
        Disabled = 0,
        DeliverCallsFromTheSameSurvey = 1,
        DeliverCallsFromOtherSurvey = 2
    }

    public enum ExternalTransferType
    {
        Warm = 1,
        Cold = 2
    }

    public enum InternalTransferType
    {
        Off = 0,
        Warm = 1,
        Cold = 2
    }

    public enum InboundSurveyBehavior
    {
        MatchOnly = 0,
        MatchAndCreate = 1,
        CreateOnly = 2
    }
}
