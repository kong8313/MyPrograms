using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class NoMoreLicencesState : CatiProblemState
    {
        public NoMoreLicencesState(int state):
            base(state)
        {
        }

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyNoMoreLicences; }
        }
    }
}