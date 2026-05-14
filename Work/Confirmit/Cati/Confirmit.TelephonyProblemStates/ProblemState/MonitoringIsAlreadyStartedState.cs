using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class MonitoringIsAlreadyStartedState : CatiProblemState
    {
        public MonitoringIsAlreadyStartedState(int state):
            base(state)
        {
        }

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyMonitoringIsAlreadyStarted; }
        }
    }
}