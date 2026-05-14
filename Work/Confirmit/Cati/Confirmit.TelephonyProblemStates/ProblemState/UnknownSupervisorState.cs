using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class UnknownSupervisorState : CatiProblemState
    {
        public UnknownSupervisorState(int state):
            base(state)
        {
        }

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyUnknownSupervisor; }
        }
    }
}