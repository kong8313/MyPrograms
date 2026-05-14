using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class PhoneNumberAlreadyInUseState : CatiProblemState
    {
        public PhoneNumberAlreadyInUseState(int state):
            base(state)
        {
        }

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyNumberAlreadyInUse; }
        }
    }
}