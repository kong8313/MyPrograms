using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    [Serializable]
    public class RedialTypeChangedStateData : BaseStateData
    {
        public bool IsNewNumberDialChecked { get; set; }
    }
}
