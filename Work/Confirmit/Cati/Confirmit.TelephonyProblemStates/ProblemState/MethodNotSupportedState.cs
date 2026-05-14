using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class MethodNotSupportedState : CatiProblemState
    {
        public MethodNotSupportedState(int state)
            :base(state)
        {}

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyMethodNotSupported; }
        }
    }
}
