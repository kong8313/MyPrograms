using Confirmit.CATI.Telephony.SimulatorDialerDriver;

namespace SimulatorDialerDriver
{
    public interface ICallOutcomeDistributor
    {
        CallOutcomeDistributionScenario CallOutcomeDistributionScenario{get; set; }

        CallOutcomeDistributionData GetNextCallOutcomeDistributionData(string phoneNumber, CallManager.CallType callType = CallManager.CallType.Outbound);
    }
}
