using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    [Serializable]
    public class RedialHangupDialCalledStateData : BaseStateData
    {
        public string Message { get; set; }
    }
}