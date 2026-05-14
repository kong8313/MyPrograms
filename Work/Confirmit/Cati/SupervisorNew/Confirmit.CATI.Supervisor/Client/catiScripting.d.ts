declare interface Int32 { }
declare interface Int64 { }
declare interface Byte { }
declare interface Double { }
declare interface Object { }
declare interface String { }
declare interface Boolean { }
declare interface DateTime { }
declare interface Array<T>
{
    Length: Int32;
    [n: number]: T;
}


declare interface ISchedulingScriptAction { }

declare class Actions {
    static RestorePreviousCallState: ISchedulingScriptAction;
    static AcceptInboundCall: ISchedulingScriptAction;
    static AssignMultipleGroups: ISchedulingScriptAction;
    static DeassignMultipleGroups: ISchedulingScriptAction;
    static RecallAfterNumberOfMinutes: ISchedulingScriptAction;
    static RecallAfterNumberOfShifts: ISchedulingScriptAction;
}

/**
 * Executes the specified action
 */
declare function ExecuteAction(action: ISchedulingScriptAction): void

/**
 * Executes the specified action with specified parameter
 */
declare function ExecuteAction(action: ISchedulingScriptAction, parameter: string): void

/**
 * Gets the value of the specified background variable
 */
declare function GetRespondentValue(fieldName: String): String
/**
 * Sets the value of the specified background variable
 */
declare function SetRespondentValue(fieldName: String, value: String): void

/**
 * Gets a numeric value of the scheduling script parameter by ID.
 * This may be either the default value defined in the scheduling script
 * or overridden value defined in the settings of a survey using this scheduling script.
 */
declare function GetParamNumeric(paramID: Int32): Int32
/**
 * Gets a numeric value of the scheduling script parameter by name.                     
 * This may be either the default value defined in the scheduling script                
 * or overridden value defined in the settings of a survey using this scheduling script.
 */
declare function GetParamNumeric(name: String): Int32
/**
 * Gets a string value of the scheduling script parameter by ID.                        
 * This may be either the default value defined in the scheduling script                
 * or overridden value defined in the settings of a survey using this scheduling script.
 */
declare function GetParamValue(paramID: Int32): String
/**
 * Gets a string value of the scheduling script parameter by name.                        
 * This may be either the default value defined in the scheduling script                  
 * or overridden value defined in the settings of a survey using this scheduling script.
 */
declare function GetParamValue(name: String): String
/**
 * Writes message into the Scheduling Script execution log
 * */
declare function LogMessage(message: String): void

declare class BvSurveyEntity {
}

declare class BvCallEntity {
    CallID: Int32
    SurveySID: Int32
    InterviewID: Int32
    CallState: Int32
    ShiftID: Int32
    TimeInShift: DateTime | null
    TimeToExpire: DateTime | null
    Priority: Int32
    Status: Int32
    Resource: Int32
    ApptID: Int32
    Lock: Boolean
    TimeZoneID: Int32
    TzTimeInShift: DateTime
    ResourceType: Int32
    OldPriority: Int32
    ConditionValue: Int32
    CellId: Int32
    DialTypeId: Byte
    Type: Byte
    DialerId: Int32
    ActiveDialId: Int32
}

declare class BvInterviewEntity {
    ID: Int32
    SurveySID: Int32
    TelephoneNumber: String
    RespondentName: String
    TimezoneID: Int32 | null
    TransientState: Int32
    LastCallTime: DateTime | null
    LastCallPersonSID: Int32
    Duration: Int32 | null
    ExtensionNumber: String
    LastChannelID: Byte
    DialingMode: Byte
    DialerId: Int32
    IsSentToReview: Boolean
    DialTypeId: Byte
    ReviewStatus: Byte
}

declare class BvInterviewWithOriginEntity extends BvInterviewEntity {
    /** Original interview properties */
    Origin: BvInterviewEntity
}

declare class MatchingShift {
    /** Internal shift ID */
    ID: Int32
    /** Public shift ID, which is shown in UI */
    ShiftId: Int32
    /** Timezone ID of Shift */
    TzID: Int32
    ShiftTypeID: Int32
    StartDate: DateTime
    FinishDate: DateTime
}

declare class ShiftService {
    GetMatchingTime(utcNowTime: DateTime, tzID: Int32): DateTime

    GetMatchingShift(utcTime: DateTime, tzID: Int32): MatchingShift

    GetNextShift(currentShift: MatchingShift, tzID: Int32, out_countSkipShifts: Int32): MatchingShift

    GetShiftAfterNumberOfShifts(currentShift: MatchingShift, tzID: Int32, numberOfShifts: Int32, isTakingExclusionIntoAccount: Boolean): MatchingShift

    GetExactShift(utcNowTime: DateTime, tzID: Int32): MatchingShift

    GetNextShift(currentShift: MatchingShift, tzID: Int32): MatchingShift

    GetShiftAfterNumberOfMinutes(utcNowTime: DateTime, tzID: Int32, countMinutes: Int32): MatchingShift

    GetShiftAfterNumberOfShifts(utcNowTime: DateTime, tzID: Int32, numberOfShifts: Int32): MatchingShift

    GetNextShiftOfSpecifiedType(utcTime: DateTime, tzID: Int32, scriptShiftTypeID: Int32): MatchingShift

    GetNextShiftByID(utcTime: DateTime, tzID: Int32, scriptShiftID: Int32): MatchingShift
}

declare class Scheduling {
    static Survey: BvSurveyEntity
    /** New interview properties */
    static Interview: BvInterviewWithOriginEntity
    /** Original call properties */
    static LastCall: BvCallEntity
    /** New call properties. Use CallShouldBeCreated() function before accessing this object */
    static NewCall: BvCallEntity
    static Time: DateTime
    static ExecutionReason: SchedulingScriptExecutionReason
    static Shifts: ShiftService
    static CallCenterID: Int32
    static ProcessSampleMode: ProcessSampleMode
    /** Only for inbound calls - contains CLI (respondent phone number) provided by the dialer */
    static CliNumber: String
    /** Only for inbound calls - contains direct dial-in (DDI) number */
    static DdiNumber: String
    /** Information about latest dialing attempt during current interviewing attempt */
    static LastDialingAttempt: DialingAttempt
    /** Array with information about all dialing attempts during current interviewing attempt */
    static LastCallDialingAttempts: Array<DialingAttempt>
    
    /** Returns an array with all call attempts. Newest-first order. */
    static GetCallHistory(): Array<CallAttempt>
    
    /**
     * Returns an array of call attempts whose status matches
     * the supplied value. Newest-first order.
     * @param callStatus  Call status to filter by.
     */
    static GetCallHistory(callStatus: ExtendedStatus): Array<CallAttempt>

    /**
     * Returns an array of call attempts whose status matches
     * the supplied value. Newest-first order.
     *
     * @param callStatus   Call status to filter by.
     * @param withinLastN  Restrict search to the specified number of most recent call attempts.
     */
    static GetCallHistory(callStatus: ExtendedStatus, withinLastN: Int32): Array<CallAttempt>

    /**
     * Returns an array of call attempts whose telephone number matches
     * the supplied value. Newest-first order.
     * @param telephoneNumber telephone number to filter by.
     */
    static GetCallHistory(telephoneNumber: String): Array<CallAttempt>
}

declare class CallAttempt {
    /**
     * Sequence number of the call attempt during the interview.
     * Corresponds to the 'CallAttemptCount' respondent field at the time of the call.
     * Can be used to link call history data with survey data from the "call history" loop.
     */
    AttemptNumber: Int32

    /** Respondent telephone number */
    TelephoneNumber: String

    /** Time when the call attempt started (UTC). */
    StartTimeUtc: DateTime

    /** Time when the call attempt started (respondent's timezone). */
    StartTimeRespondent: DateTime

    /** Time when the call attempt ended (UTC). */
    EndTimeUtc: DateTime

    /** Time when the call attempt ended (respondent's timezone). */
    EndTimeRespondent: DateTime

    /** Duration of the call attempt, in seconds. */
    Duration: Int32

    /** Interviewer identifier. 0 → the call was automatically disposed by the dialer */
    InterviwerId: Int32

    /** Extended status assigned to the interview after the call attempt. */
    ExtendedStatus: ExtendedStatus

    /** AAPOR code based on the extended status of the call attempt. */
    AaporCode: String

    /** Duration of open-ended review time, in seconds. */
    OpenEndReviewDuration: Int32

    /** The difference in seconds between the interview start time and the first time the phone connection was established within the interview. Only applicable for Preview interviews */
    PreviewTime: Int32

    /** The difference in seconds between the first time the phone connection was established within the interview and the last time the phone connection was disconnected within the interview. */
    ConnectedTime: Int32

    /**
     * Time difference in seconds between the final disconnection and the start of the open-end review
     * (if enabled), or the interview end (if not enabled).
     */
    WrapTime: Int32

    /**
     * Waiting time in seconds before the interview started.
     * Includes the time taken by the dialer or system to find and/or connect the call.
     * May also include the time the interviewer spent on the survey or interview selection screen.
     */
    WaitingTime: Int32

    /** Unique identifier of the call center. */
    CallCenterId: Int32
}

declare class DialingAttempt {
    DialId: Int64
    
    TelephoneNumber: String
    /** Caller ID (CLI) used by the dialer when calling the respondent */
    DialerCallerId: String
    /** Time in seconds between the start of dialing and the time the call is connected/dropped. */
    RingTime: Int32
    /** Time when dialing is started. */
    StartTime: DateTime
    /** Time when call is disconnected or interview is finished. */
    FinishTime: DateTime
    /** Dialing attempt outcome returned by dialer (see CallOutcome enum) */
    DialerCallOutcome: CallOutcome
    
    /** Gets additional dialer call outcome metadata by a given key.
     * @param key - The key of the metadata.
     * @returns {String} - The value of the metadata or null in case there is no metadata with such key.
     * */
    GetMetadata(key: String): String
}

declare enum SchedulingScriptExecutionReason {
    Unspecified = 0,
    Expired = 1,
    NotConnected = 2,
    Processed = 3,
    MovedAndRescheduled = 4,
    Added = 5,
    Terminated = 6,
    TelephonyError = 7,
    AddedBySample = 8,
    Inbound = 9
}

declare enum ProcessSampleMode {
    Add = 0,
    Update = 1,
    Merge = 2
}

declare enum CallOutcome {
    NotDefined = -1,
    Connected = 0,
    Busy = 2,
    NoReply = 3,
    Refusal = 5,
    Terminated = 6,
    AnswerMachine = 7,
    Modem = 8,
    Fax = 9,
    Congestion = 10,
    Unobtainable = 11,
    Nuisance = 12,
    Screened = 14,
    ReturnedNotDialled = 15,
    NotAutomaticallyDialled = 18,
    ReturnedDiallerExpired = 25,
    InterruptedBySystem = 26,
    Stopped = 28,
    TelephonyFailure = 29,
    Error = 30,
    InboundCall = 1000,
    DialingInterrupted = 1020,
    ExternallyValidatedNumber = 1021
}

/**
 * Predefined interview extended statuses
 */
declare enum ExtendedStatus {

    /**
     * Appointment (1).
     * Interview abandoned with an appointment time captured to call the respondent back.
     */
    Appointment = 1,

    /**
     * Busy (2).
     * Respondent is already on a call.
     */
    Busy = 2,

    /**
     * No reply (3).
     * Respondent did not answer.
     */
    NoReply = 3,

    /**
     * Quota failure (4).
     * The quota target for the respondent has already been fulfilled.
     */
    QuotaFail = 4,

    /**
     * Refusal (5).
     * Respondent refused to be interviewed.
     */
    Refusal = 5,

    /**
     * Terminated (6).
     * Call abandoned before completion.
     */
    Terminated = 6,

    /**
     * Answer machine (7).
     * Call was answered by an answering machine or voicemail.
     */
    AnswerMachine = 7,

    /**
     * Modem (8).
     * Call reached a modem.
     */
    Modem = 8,

    /**
     * Fax (9).
     * Call reached a fax machine.
     */
    Fax = 9,

    /**
     * Congestion (10).
     * Network congestion occurred during the call.
     */
    Congestion = 10,

    /**
     * Unobtainable (11).
     * Invalid or unreachable number.
     */
    Unobtainable = 11,

    /**
     * Nuisance (12).
     * Respondent hung up before the interviewer received the call.
     */
    Nuisance = 12,

    /**
     * Completed (13).
     * Interview was successfully completed.
     */
    Completed = 13,

    /**
     * Screened (14).
     * Respondent was screened out based on interview criteria.
     */
    Screened = 14,

    /**
     * Returned not dialled (15).
     * Call returned from the dialer without a dial attempt (specific to predictive mode).
     */
    ReturnedNotDialled = 15,

    /**
     * Fresh sample (16).
     * Respondent is new and awaiting interview.
     */
    FreshSample = 16,

    /**
     * Blacklist (17).
     * Respondent’s number is on the telephone blacklist.
     */
    Blacklist = 17,

    /**
     * Not automatically dialled (18).
     * Call was manually dialed.
     */
    NotAutomaticallyDialled = 18,

    /**
     * Transfer to Web (20).
     * Respondent was reassigned to a web interview.
     */
    TransferToWeb = 20,

    /**
     * Transfer to CATI (21).
     * Respondent was reassigned to computer-assisted telephone interviewing.
     */
    TransferToCati = 21,

    /**
     * Transfer to CAPI (22).
     * Respondent was reassigned to computer-assisted personal interviewing.
     */
    TransferToCapi = 22,

    /**
     * Transfer to IVR (23).
     * Respondent was reassigned to an interactive voice-response system.
     */
    TransferToIvr = 23,

    /**
     * Interrupted by interviewer (24).
     * Call was aborted by the interviewer.
     */
    InterruptedByInterviewer = 24,

    /**
     * Returned dialer expired (25).
     * Call was returned after dial expiration (specific to predictive mode).
     */
    ReturnedDiallerExpired = 25,

    /**
     * Interrupted by system (26).
     * Call was interrupted by the system.
     */
    InterruptedBySystem = 26,

    /**
     * Filtered by call delivery (27).
     * Call removed from the queue due to quota being full.
     */
    FilteredByCallDelivery = 27,

    /**
     * Stopped (28).
     * Call was stopped by the dialer.
     */
    Stopped = 28,

    /**
     * Telephony failure (29).
     * Technical issue prevented the call.
     */
    TelephonyFailure = 29,

    /**
     * Error (30).
     * An error occurred during the call.
     */
    Error = 30,

    /**
     * Inbound call (1000).
     * An incoming call handled by the system.
     */
    InboundCall = 1000,

    /**
     * Inbound call dropped by respondent (1001).
     * Respondent disconnected an incoming call.
     */
    InboundCallDroppedByRespondent = 1001,

    /**
     * Internal transfer (1010).
     * Call was transferred from an IVR agent to a live interviewer or between live interviewers.
     */
    InternalTransfer = 1010,

    /**
     * External transfer (1011).
     * Call was transferred to an external number.
     */
    ExternalTransfer = 1011,

    /**
     * Canceled transfer (1012).
     * Interviewer canceled the transfer.
     */
    CanceledTransfer = 1012,

    /**
     * Dialing interrupted (1020).
     * Call was interrupted while dialing.
     */
    DialingInterrupted = 1020,

    /**
     * Externally validated number (1021).
     * Number was validated by an external service.
     */
    ExternallyValidatedNumber = 1021,

    /**
     * Survey script error (1051).
     * An error occurred in the survey script.
     */
    SurveyScriptError = 1051,

    /**
     * Synchronized sample (1052).
     * Record was manually synchronized from the survey respondent list.
     */
    SynchronizedSample = 1052
}

/**
 * Gets timezone ID for the current interview
 */
declare var TimezoneID: Int32;
/**
 * Gets time of last call for the current interview
 */
declare var LastCallTime: DateTime;

/**
 * Ensures that a new call (Scheduling.NewCall) is created
 */
declare function CallShouldBeCreated(): void

/**
 * Gets a value indicating whether scheduling has been invoked due to a call expiry event
 */
declare function IsCallExpired(): Boolean

/**
 * Gets a value indicating whether the last call for current interview has been expired within the specified timeout (in minutes)
 */
declare function IsSoftExpired(timeoutInMinutes: Int32): Boolean

/**
 * Gets a value indicating whether the interviewer assigned on the last call for current interview is currently logged in
 */
declare function IsPreviousResourceLoggedIn(): Boolean

/**
 * Gets a value indicating whether the last call for current interview has been expired
 * within the specified timeout (in minutes) and interviewer is currently logged in
 */
declare function IsCallExpiredWithResourceLoggedIn(timeoutInMinutes: Int32): Boolean

/**
 * Checks if there are any logged-in interviewers associated with the supplied resource ID and 
 * the same survey id as the current interview.
 * The resource ID can be either an individual interviewer ID or an interviewer group ID.
 * Note that dial type won't be checked.
 *
 * @param resourceId - The ID of the resource to check. This can be either:
 *  - An interviewer ID (number).
 *  - A group ID representing a collection of interviewers (number).
 * @returns {boolean}
 *  - `true` if the specified interviewer associated with the current survey is logged in or if at least one interviewer in the group is logged in.
 *  - `false` if no interviewers associated with the current survey are logged in for the provided resource ID.
 *
 */
declare function IsResourceLoggedIntoSurvey(resourceId: Int32): Boolean

/**
 * Checks if there are any ready-for-call interviewers associated with the supplied
 * resource ID and the same survey id as the current interview.
 * Interviewer is ready for call if he has NoCalls, Selecting or Waiting state.
 * The resource ID can be either an individual interviewer ID or an interviewer group ID.
 * Note that dial type won't be checked.
 *
 * @param resourceId - The ID of the resource to check. This can be either:
 *  - An interviewer ID (number).
 *  - A group ID representing a collection of interviewers (number).
 * @returns {boolean}
 *  - `true` if the specified interviewer associated with the current survey is logged in with ready-for-call state or if at least one interviewer in the group is ready for a call.
 *  - `false` if no interviewers associated with the current survey are logged in for the provided resource ID.
 *
 */
declare function IsResourceReadyForCallInSurvey(resourceId: Int32): Boolean

/**
 * Checks if there are any logged-in interviewers associated with the current survey.
 * Note that dial type won't be checked.
 *
 * @param agentType - Type of interviewer: Ivr or Live
 * @returns {boolean}
 *  - `true` if there are any logged-in interviewers associated with the current survey.
 *  - `false` if no interviewers associated with the current survey are logged in.
 *
 */
declare function IsAnyoneLoggedIntoSurvey(agentType: AgentType): Boolean

/**
 * Checks if there are any logged-in interviewers associated with the current survey.
 * Note that dial type won't be checked.
 *
 * @returns {boolean}
 *  - `true` if there are any logged-in interviewers associated with the current survey.
 *  - `false` if no interviewers associated with the current survey are logged in.
 *
 */
declare function IsAnyoneLoggedIntoSurvey(): Boolean

/**
 * Checks if there are any ready-for-call interviewers associated with the current survey.
 * Interviewer is ready for call if he has NoCalls, Selecting or Waiting state.
 * Note that dial type won't be checked.
 *
 * @param agentType - Type of interviewer: Ivr or Live
 * @returns {boolean}
 *  - `true` if there are any ready-for-call interviewers associated with the current survey.
 *  - `false` if no interviewers associated with the current survey are ready-for-call.
 *
 */
declare function IsAnyoneReadyForCallInSurvey(agentType: AgentType): Boolean

/**
 * Creates appointment with time specified in respondent timezone
 */
declare function CreateCustomAppointment(appointmentTimeInRespondentTZ: DateTime): void

declare enum AgentType {
    Live = 0,
    Ivr = 1
}

/**
 * Gets a value indicating whether interview extended status has not been changed during current scheduling script execution
 */
declare function IsITSNotChanged(): Boolean

declare function f(questionId: String): ExprObj
declare function f(questionId: String, params: String[]): ExprObj

declare class ExprObj {
    CODED: Boolean;
    DICHOTOMY: Boolean;
    COMPOUND: Boolean;
    OPEN: Boolean;
    DATE: Boolean;
    BOOL: Boolean;
    NUMERIC: Boolean;
    EXTERNAL: Boolean;

    label(): String
    text(): String
    instruction(): String
    get(): String
    setValue(sval: String): String
    setValue(sval: Object): String
    toNumber(): Double
    toInt(): Int32
    toBoolean(): Boolean
    toString(): String
    value(): String
}

declare class Interviewer {
    Id: Int32
    Name: String
    Description: String
    Location: String
}

/**
 * Gets Interviewer by Id
 */
declare function GetInterviewerById(interviewerId: Int32): Interviewer

/**
 * Gets Interviewer by Name
 */
declare function GetInterviewerByName(interviwerName: String): Interviewer
