using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class NoMoreSupervisorResourcesState : CatiProblemState
    {
        public NoMoreSupervisorResourcesState(int state):
            base(state)
        {
        }

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyNoMoreSupervisorResources; }
        }
    }
}