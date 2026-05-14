using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class NoMoreConferenceResourcesState : CatiProblemState
    {
        public NoMoreConferenceResourcesState(int state):
            base(state)
        {
        }

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyNoMoreConferenceResources; }
        }
    }
}