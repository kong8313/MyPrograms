namespace Confirmit.CATI.Core.Telephony.Inbound
{
    public enum DropInboundCallReason
    {
        InboundFeatureIsDisabled = 1,
        DdiRecordIsNotFound = 2,
        UnexpectedCallState = 3,
        CallLockIsNotAcquired = 4,
        NotAcceptedBySchedulingScript = 5,
        InterviewIsNotFound = 6,
        SurveyIsNotOpened = 7,
        SurveyIsNotFound = 8,
        ShiftIsNotFound = 9,
        InternalServerError = 10,
        NoAgentsAvailable = 11
    }
}