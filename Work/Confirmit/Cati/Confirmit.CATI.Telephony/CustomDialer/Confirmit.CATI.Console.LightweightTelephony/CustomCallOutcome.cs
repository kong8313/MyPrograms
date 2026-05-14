namespace Confirmit.CATI.Console.LightweightTelephony
{
    /// <summary>
    /// Possible results of dialling operation.
    /// </summary>
    public enum CustomCallOutcome
    {
        Not_Defined=-1, // this value is used when interviewer is not logged in to dialer,
        // or dialling is in progress,
        // or interview is started in preview dial mode but dial command has not been initiated by the survey engine yet.
        Connected=0, // The call was connected successfully.
        Appointment=1,
        Busy=2,
        NoReply=3,
        QuotaFail=4,
        Refusal=5,
        Terminated=6,
        AnswerPhone=7,
        Modem=8,
        Fax=9,
        Congestion=10,
        Unobtainable=11,
        Nuisance=12,
        Completed=13,
        Screened=14,
        ReturnedNotDialled=15,
        FreshSample=16,
        Blacklist = 17,
        NotAutomaticallyDialled=18, /*manual dialling*/
        //RESERVED FOR FUTURE USE = 19,
        TransferToWeb=20,
        TransferToCATI=21,
        TransferToCAPI=22,
        TransferToIVR=23,
        //RESERVED FOR FUTURE USE = 24,
        ReturnedDiallerExpired = 25,
        InterruptedBySystem=26,
        FilteredByCallDelivery = 27,
        Stopped=28,
        TelephonyFailure=29,
        Error=30
    }
}