using System.Collections.Generic;
using ConfirmitDialerInterface;
using SimulatorDialerDriver.Models;

namespace SimulatorDialerDriver
{
    public interface ICallManager
    {
        int CallsCount { get; }
        bool MoveCallTo(int callId, CallManager to);
        CallManager.CallInfoEx GetCallWithRemove(Interviewer interviewer);
        void AddCalls(long campaignId, List<CallInfo> calls);
        CallInfo[] GetExpiredCallsAndRemove();
        void DemandCall();
        bool WasCallDeliveredSinceLastDemand();
        int RemoveCalls(List<CallInfo> callList);
        List<CallInfo> RemoveAll();
        void AddInboundCall(long campaignId, long[] borrowAgentsFrom, CallInfo callInfo);
        void AddTranferCall(long campaignId, long[] borrowAgentsFrom, CallInfo callInfo);
    }
}