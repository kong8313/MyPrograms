using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Possible results of dialing operation.
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum CallOutcome
    {
        /// <summary> -1
        /// this value is used when interviewer is not logged in to dialer,
        /// or dialing is in progress,
        /// or interview is started in preview dial mode but dial command has not been 
        /// initiated by the survey engine yet.
        /// This value must not be used as outcome by dialer
        /// </summary>
        [EnumMember]
        NotDefined = -1,

        /// <summary> The call was connected successfully = 0 </summary>
        [EnumMember]
        Connected = 0,  // The call was connected successfully.

        /// <summary> 1 
        /// For internal use. This value must not be used as outcome by dialer.
        /// </summary>
        [EnumMember]
        Appointment = 1,

        /// <summary> 2 </summary>
        [EnumMember]
        Busy = 2,

        /// <summary> 3 </summary>
        [EnumMember]
        NoReply = 3,

        /// <summary> 4 
        /// For internal use. This value must not be used as outcome by dialer.
        /// </summary>
        [EnumMember]
        QuotaFail = 4,

        /// <summary> 5 </summary>
        [EnumMember]
        Refusal = 5,

        /// <summary> 6 </summary>
        [EnumMember]
        Terminated = 6,

        /// <summary> 7 </summary>
        [EnumMember]
        AnswerMachine = 7,

        /// <summary> 8 </summary>
        [EnumMember]
        Modem = 8,

        /// <summary> 9 </summary>
        [EnumMember]
        Fax = 9,

        /// <summary> 10 </summary>
        [EnumMember]
        Congestion = 10,

        /// <summary> 11 </summary>
        [EnumMember]
        Unobtainable = 11,

        /// <summary> 12 </summary>
        [EnumMember]
        Nuisance = 12,

        /// <summary> 13
        /// For internal use. This value must not be used as outcome by dialer.
        ///  </summary>
        [EnumMember]
        Completed = 13,

        /// <summary> 14 </summary>
        [EnumMember]
        Screened = 14,

        /// <summary> 15 </summary>
        [EnumMember]
        ReturnedNotDialled = 15,

        /// <summary> 16 
        /// For internal use. This value must not be used as outcome by dialer.
        /// </summary>
        [EnumMember]
        FreshSample = 16,

        /// <summary> 17 
        /// For internal use. This value must not be used as outcome by dialer.
        /// </summary>
        [EnumMember]
        Blacklist = 17,

        /// <summary> 18 </summary>
        [EnumMember]
        NotAutomaticallyDialled = 18, /*manual dialling*/

        //RESERVED FOR FUTURE USE = 19,

        /// <summary> 20 
        /// For internal use. This value must not be used as outcome by dialer.
        /// </summary>
        [EnumMember]
        TransferToWeb = 20,

        /// <summary> 21 
        /// For internal use. This value must not be used as outcome by dialer.
        /// </summary>
        [EnumMember]
        TransferToCati = 21,

        /// <summary> 22 
        /// For internal use. This value must not be used as outcome by dialer.
        /// </summary>
        [EnumMember]
        TransferToCapi = 22,

        /// <summary> 23 
        /// For internal use. This value must not be used as outcome by dialer.
        /// </summary>
        [EnumMember]
        TransferToIvr = 23,

        /// <summary> 24
        /// Used when task is terminated from Cati Console. E.g. console is closed using X
        /// </summary>
        [EnumMember]
        InterruptedByInterviewer = 24,

        /// <summary> 25 </summary>
        [EnumMember]
        ReturnedDiallerExpired = 25,

        /// <summary> 26 </summary>
        [EnumMember]
        InterruptedBySystem = 26,

        /// <summary> 27 
        /// For internal use. This value must not be used as outcome by dialer.
        /// </summary>
        [EnumMember]
        FilteredByCallDelivery = 27,

        /// <summary> 28 </summary>
        [EnumMember]
        Stopped = 28,

        /// <summary> 29 </summary>
        [EnumMember]
        TelephonyFailure = 29,

        /// <summary> 30 </summary>
        [EnumMember]
        Error = 30,

        /// <summary> 1000 </summary>
        [EnumMember]
        InboundCall = 1000,

        /// <summary> 1001
        /// For internal use. This value must not be used as outcome by dialer.
        /// </summary>
        [EnumMember]
        DroppedByRespondent = 1001,

        /// <summary> 1010 
        /// For internal use.  This value must not be used as outcome by dialer.
        /// </summary>
        [EnumMember]
        InternalTransfer = 1010,

        /// <summary> 1011 
        /// For internal use.  This value must not be used as outcome by dialer.
        /// </summary>
        [EnumMember]
        ExternalTransfer = 1011,
        
        /// <summary> 1011 
        /// For internal use.  This value must not be used as outcome by dialer.
        /// </summary>
        [EnumMember]
        CanceledTransfer = 1012,

        /// <summary> 1020
        /// Dialer should send this outcome in case Hangup command is received while dialing is in progress
        /// </summary>
        [EnumMember]
        DialingInterrupted = 1020,

        /// <summary> 1021
        /// Dialer should send this outcome in case number was externally validated via third party service
        /// </summary>
        [EnumMember]
        ExternallyValidatedNumber = 1021,

        /// <summary> 1051
        /// For internal use. This value must not be used as outcome by dialer.
        /// </summary>
        [EnumMember]
        SurveyScriptError = 1051
    }
}
