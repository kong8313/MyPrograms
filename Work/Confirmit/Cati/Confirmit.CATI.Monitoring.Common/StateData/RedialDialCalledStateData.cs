using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    [Serializable]
    public class RedialDialCalledStateData : BaseStateData
    {
        public bool IsDialCanellationButtonVisible { get; set; }

        public string Message { get; set; }
    }
}