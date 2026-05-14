namespace Confirmit.CATI.Core.Telephony.Inbound
{
    public interface IDialerNotifyInboundCallHandler
    {
        void Execute(int dialerId,
            int companyId,
            string ddiNumber,
            string cliNumber,
            string inboundCallId);
    }
}