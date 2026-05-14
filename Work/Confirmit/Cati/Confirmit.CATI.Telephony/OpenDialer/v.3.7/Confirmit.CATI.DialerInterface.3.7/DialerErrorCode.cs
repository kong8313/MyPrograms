using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Result codes dialer can return.
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum DialerErrorCode
    {
        [EnumMember]
        Success = 0,

        [EnumMember]
        NotAvailable = 10,
        [EnumMember]
        NotSupported = 20,
        [EnumMember]
        Restarted = 30,
        [EnumMember]
        InvalidParameter = 40,
        [EnumMember]
        Forbidden = 50,
        [EnumMember]
        Exception = 60,
        [EnumMember]
        UnknownError = 70,

        [EnumMember]
        UnknownAgent = 80,
        [EnumMember]
        WrongAgentState = 90,
        [EnumMember]
        AgentAlreadyLoggedIn = 100,
        [EnumMember]
        UnknownSupervisor = 110,
        [EnumMember]
        UnknownCampaign = 111,

        [EnumMember]
        InvalidDialingMode = 120,
        [EnumMember]
        InvalidExtension = 130,
        [EnumMember]
        InvalidPhoneNumber = 140,
        [EnumMember]
        PhoneNumberAlreadyInUse = 150,
        [EnumMember]
        ResourceAlreadyInUse = 160,
        [EnumMember]
        ResourceNotFound = 170,

        [EnumMember]
        NoMoreLicences = 180,
        [EnumMember]
        NoMoreConferenceResources = 190,
        [EnumMember]
        NoMoreFreeChannels = 200,
        [EnumMember]
        NoMoreSupervisorResources = 210,
        [EnumMember]
        NoMoreVoiceResources = 211,

        [EnumMember]
        WrongStateDialingInProgress = 220,
        [EnumMember]
        WrongStatePaused = 230,
        [EnumMember]
        WrongStateResourceIsBusy = 240,
        [EnumMember]
        WrongStateAgentNotInCall = 245,
        [EnumMember]
        WrongStateNotInConversation = 246,
        [EnumMember]
        WrongState = 247, // Common WrongState code

        [EnumMember]
        AgentIsNotLoggedin = 250,
        [EnumMember]
        AgentAlreadyBeingMonitored = 260,
        [EnumMember]
        MonitoringIsAlreadyStarted = 270
    }
}
