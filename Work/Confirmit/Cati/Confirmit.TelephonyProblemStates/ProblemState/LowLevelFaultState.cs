using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class LowLevelFaultState : CatiProblemState
    {
        public LowLevelFaultState(int state):
            base(state)
        {
        }

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyLowLevelFault; }
        }
    }
}