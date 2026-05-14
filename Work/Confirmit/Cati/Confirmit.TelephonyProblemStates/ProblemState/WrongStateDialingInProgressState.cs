using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class WrongStateDialingInProgressState : CatiProblemState
    {
        public WrongStateDialingInProgressState(int state):
            base(state)
        {
        }

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyDialingInProgress; }
        }
    }
}