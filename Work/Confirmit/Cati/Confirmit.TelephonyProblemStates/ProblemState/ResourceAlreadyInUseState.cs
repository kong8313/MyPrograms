using Confirmit.TelephonyProblemStates.Resources;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class ResourceAlreadyInUseState : CatiProblemState
    {
        public ResourceAlreadyInUseState(int state, string stationId):
            base(state)
        {
            StationId = stationId;
        }

        public string StationId { get; private set; }

        public override string Message
        {
            get { return string.Format(Strings.CatiProblem_TelephonyResourceInUse, StationId); }
        }
    }
}