namespace Confirmit.CATI.Core.Telephony.Inbound
{
    public interface IDialerNotifyInboundCallDroppedByRespondentHandler
    {
        void Execute(int dialerId, int companyId, string inboundCallId);
    }
}