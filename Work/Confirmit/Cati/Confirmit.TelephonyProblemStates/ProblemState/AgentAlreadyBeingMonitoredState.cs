using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class AgentAlreadyBeingMonitoredState: CatiProblemState
    {
        public AgentAlreadyBeingMonitoredState(int state):
            base(state)
        {}

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyAgentAlreadyBeingMonitored; }
        }
    }
}