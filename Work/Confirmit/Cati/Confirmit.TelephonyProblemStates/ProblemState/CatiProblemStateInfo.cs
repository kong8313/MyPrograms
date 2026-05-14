using System;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class CatiProblemStateInfo : ICatiProblemStateInfo
    {
        public CatiProblemStateInfo(string stationId)
        {
            StationId = stationId;
        }

        public string StationId { get; private set; }
    }
}
