using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class DialerCallExceptionState : CatiProblemState
    {
        public DialerCallExceptionState(int state):
            base(state)
        {
        }

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyCallException; }
        }
    }
}