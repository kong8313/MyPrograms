using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class UnknownAgentState : CatiProblemState
    {
        public UnknownAgentState(int state)
            :base(state)
        {}

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyUnknownAgent; }
        }
    }
}
