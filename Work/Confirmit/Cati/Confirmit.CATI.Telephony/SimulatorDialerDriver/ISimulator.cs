using System.Collections.Concurrent;
using System.Security.Policy;
using ConfirmitDialerInterface;
using DialerCommon;
using SimulatorDialerDriver;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public interface ISimulator
    {
        IDialerEvents DialerEvents { get; }
        ILogger Logger { get; }
        SimulatorScenario Scenario { get; }
        RequestId RequestId { get; }
        ICallOutcomeDistributor CallOutcomeDistributor { get; }
        SimulatorActivities Activities { get; }
        Dialer GetDialerWithCheck(int companyId, int dialerId);
        Dialer TryGetDialer(int companyId, int dialerId);
        ConcurrentDictionary<string, Dialer> Dialers { get; }
        DialerErrorCode Release(int dialerId, int companyId);
    }
}