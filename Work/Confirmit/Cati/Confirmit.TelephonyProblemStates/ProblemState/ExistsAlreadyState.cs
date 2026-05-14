using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class ExistsAlreadyState : CatiProblemState
    {
        public ExistsAlreadyState(int state):
            base(state)
        {}

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyAlreadyExists; }
        }
    }
}