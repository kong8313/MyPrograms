using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class AgentIsNotLoggedState : CatiProblemState
    {
        public AgentIsNotLoggedState(int state):
            base(state)
        {
        }

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyAgentInNotLogged; }
        }
    }
}