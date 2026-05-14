using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.Random;
using Confirmit.CATI.Telephony.SimulatorDialerDriver;
using ConfirmitDialerInterface;

namespace SimulatorDialerDriver.Distribution
{
    public class CallOutcomeDistributor : ICallOutcomeDistributor
    {
        public CallOutcomeDistributionScenario CallOutcomeDistributionScenario { get; set; }

        public CallOutcomeDistributor(CallOutcomeDistributionScenario callOutcomeDistributionScenario)
        {
            CallOutcomeDistributionScenario = callOutcomeDistributionScenario;
        }

        public CallOutcomeDistributionData GetNextCallOutcomeDistributionData(string phoneNumber, CallManager.CallType callType)
        {
            if (!string.IsNullOrEmpty(phoneNumber) && phoneNumber.StartsWith(CallOutcomeDistributionScenario.OutcomePhonePrefix))
            {
                string remainingNumber = phoneNumber.Substring(CallOutcomeDistributionScenario.OutcomePhonePrefix.Length);

                if (!Enum.TryParse(remainingNumber, true, out CallOutcome callOutcome))
                {
                    callOutcome = CallOutcome.TelephonyFailure;
                }

                return new CallOutcomeDistributionData { CallOutcome = callOutcome, ProcessingTimeFormattedString = "1" };
            }

            var suitableOutcomes = GetSuitableCallOutcomeDistributionDataByCallType(CallOutcomeDistributionScenario.OutcomeList, callType);

            switch (CallOutcomeDistributionScenario.GenerationMethod)
            {
                case CallOutcomeGenerationMethod.Random:
                    return GetCallOutcomeDistributionData(suitableOutcomes, Randomizer.Next());
                case CallOutcomeGenerationMethod.Sequence:
                default:
                    return GetCallOutcomeDistributionData(suitableOutcomes, CallOutcomeDistributionScenario.StartIteration++);
            }
        }

        private static CallOutcomeDistributionData GetCallOutcomeDistributionData(CallOutcomeDistributionData[] outcomes, int iteration)
        {
            var sum = outcomes.Sum(x => x.DistributionWeight);
            var val = iteration % sum;

            //we can use here binary search to optimize. if amount of outcomes is too large
            foreach (var outcome in outcomes)
            {
                val -= outcome.DistributionWeight;
                if (val < 0)
                    return outcome;
            }

            return outcomes.Last();
        }

        private static CallOutcomeDistributionData[] GetSuitableCallOutcomeDistributionDataByCallType(List<CallOutcomeDistributionData> outcomes, CallManager.CallType callType)
        {
            CallOutcomeDistributionData[] availableOutcomes;

            if (callType == CallManager.CallType.Inbound ||
                callType == CallManager.CallType.Transfer)
            {
                availableOutcomes = outcomes.Where(x => x.CallOutcome == CallOutcome.DroppedByRespondent || x.CallOutcome == CallOutcome.Connected).ToArray();
            }
            else
            {
                availableOutcomes = outcomes.Where(x => x.CallOutcome != CallOutcome.DroppedByRespondent).ToArray();
            }

            return availableOutcomes;
        }
    }
}
