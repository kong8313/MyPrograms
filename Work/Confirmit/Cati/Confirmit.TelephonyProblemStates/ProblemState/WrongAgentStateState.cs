using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class WrongAgentStateState : CatiProblemState
    {
        public WrongAgentStateState(int state):
            base(state)
        {
        }

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyWrongAgentState; }
        }
    }
}