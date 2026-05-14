using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class DialerIsNotAvailable : CatiProblemState
    {
        public DialerIsNotAvailable(int state)
            :base(state)
        {}

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyDialerUnavailable; }
        }
    }
}
