using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class NoMoreFreeChannelsState : CatiProblemState
    {
        public NoMoreFreeChannelsState(int state):
            base(state)
        {
        }

        public override string Message
        {
            get { return Strings.CatiProblem_TelephonyNoMoreFreeChannels; }
        }
    }
}