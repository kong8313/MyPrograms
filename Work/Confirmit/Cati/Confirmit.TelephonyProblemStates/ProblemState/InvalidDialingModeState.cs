using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class InvalidDialingModeState: CatiProblemState
    {
        public InvalidDialingModeState(int state):
            base(state)
        {}

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyInvalidDialingMode; }
        }
    }
}