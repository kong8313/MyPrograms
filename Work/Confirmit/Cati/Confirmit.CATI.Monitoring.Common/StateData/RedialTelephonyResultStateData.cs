using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    [Serializable]
    public class RedialTelephonyResultStateData : BaseStateData
    {
        public string CallOutcome { get; set; }

        public bool IsConnected { get; set; }
    }
}
