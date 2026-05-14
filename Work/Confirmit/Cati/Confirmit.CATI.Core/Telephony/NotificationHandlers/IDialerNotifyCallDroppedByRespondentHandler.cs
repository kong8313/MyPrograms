namespace Confirmit.CATI.Core.Telephony.NotificationHandlers
{
    public interface IDialerNotifyCallDroppedByRespondentHandler
    {
        void Execute(int dialerId,
            string companyId,
            long campaignId,
            long agentId,
            long callId);
    }
}