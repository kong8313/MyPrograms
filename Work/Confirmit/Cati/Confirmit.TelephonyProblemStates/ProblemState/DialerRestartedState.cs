using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class DialerRestartedState : CatiProblemState
    {
        public DialerRestartedState(int state):
            base(state)
        {
        }

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyDialerRestarted; }
        }
    }
}