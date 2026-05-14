using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class UnknownErrorState : CatiProblemState
    {
        public UnknownErrorState(int state)
            : base(state)
        {
        }

        public override string Message
        {
            get { return string.Format(Strings.CatiProblem_UnknownErrorCode, State); }
        }
    }
}
