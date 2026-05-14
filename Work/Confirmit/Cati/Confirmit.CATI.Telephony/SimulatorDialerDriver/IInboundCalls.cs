namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public interface IInboundCalls
    {
        int Count { get; }
        string GenerateInboundCall(InboundCall inboundCall);
        InboundCall[] GetInboudCalls();
        void RemoveInboundCall(string inboundCallId);
    }
}