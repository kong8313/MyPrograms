namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class SuccessState: CatiProblemState
    {
        public SuccessState(int state):
            base(state)
        {}

        public override bool IsProblem
        {
            get { return false; }
        }

        public override string Message
        {
            get { return string.Empty; }
        }

        public override string ToString()
        {
            return string.Format("No problem ({0})", State);
        }
    }
}