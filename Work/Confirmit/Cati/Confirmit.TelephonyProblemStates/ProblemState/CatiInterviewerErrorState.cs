using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class CatiInterviewerErrorState : CatiProblemState
    {
        public CatiInterviewerErrorState(int state)
            :base(state)
        {}

        public override string Message
        {
            get { return Strings.CatiProblem_CatiInterviewerError; }
        }
    }
}
